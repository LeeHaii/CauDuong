using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Pixyz.Commons.Utilities;
using System.Globalization;
using Pixyz.Plugin4Unity;
using Pixyz.ImportSDK.Native;
using System;
using System.Net.NetworkInformation;
using Pixyz.Commons.Extensions;
using UnityEditor.UIElements;
using System.Linq;
using Pixyz.OptimizeSDK;

namespace Pixyz.ImportSDK
{
    public class SceneTreeConverter
    {
        // workaround for bug :
        // https://issuetracker.unity3d.com/issues/objects-get-rendered-black-slash-dark-color-when-their-scale-is-ridiculously-small
        private const bool applyScaleOnVertices = true;

        private const string LOD_NUMBER_ATTR = "PXZ_LOD_No";
        public GameObject gameObject { get; private set; }

        private int polyCount;
        public int PolyCount => polyCount;

        private int objectCount;
        public int ObjectCount => objectCount;

        private ImportSettings settings;
        private Dictionary<uint, UnityEngine.Material> uniqueMaterials = new Dictionary<uint, UnityEngine.Material>();
        private Dictionary<UnityEngine.Color, UnityEngine.Material> uniqueLineColor = new Dictionary<UnityEngine.Color, UnityEngine.Material>();
        private Dictionary<uint, Texture2D> textures = new Dictionary<uint, Texture2D>();
        private Polygonal.Native.MeshDefinitionList meshDefinitions;
        
        private Mesh[] meshes;
        private GameObject[] partGameObjects; // GameObject which will contain a mesh
        private UnityEngine.Material[][] gameObjectsMaterials; // List of arrays of materials to assign
        private Transform[][] transformsJoints;

        private Dictionary<uint, Transform> pixyzJointToTransform = new Dictionary<uint, Transform>();
        private Scene.Native.MetadataDefinitionList metadataByOccurrences;
        private Polygonal.Native.JointList jointByOccurrences;
        private VoidHandler conversionCallback;
        private bool isCompleted = false;
        private bool isReadyForCompletion = false;

        public Scene.Native.PackedTree Scene;
        private string file;
        private int meshDefConverterProcessedCount = 0;
        private List<GameObject> gameObjects = new List<GameObject>();
        private List<LODGroup> lodGroups = new List<LODGroup>();

        uint[] noShowOccurrences;
        public SceneTreeConverter(Scene.Native.PackedTree packedTree, string file, ImportSettings settings, VoidHandler conversionCallback = null)
        {
            this.settings = settings;
            this.Scene = packedTree;
            this.conversionCallback = conversionCallback;
            this.file = file;
        }

        public void Convert()
        {
            var metadatas = Pixyz.Scene.Native.NativeInterface.GetComponentByOccurrence(Scene.occurrences, Pixyz.Scene.Native.ComponentType.Metadata, true);
            Scene.Native.MetadataList metadataList = new Scene.Native.MetadataList(metadatas.list);

            metadataByOccurrences = Pixyz.Scene.Native.NativeInterface.GetMetadatasDefinitions(metadataList);
            jointByOccurrences = new Polygonal.Native.JointList(Pixyz.Scene.Native.NativeInterface.GetComponentByOccurrence(Scene.occurrences, Pixyz.Scene.Native.ComponentType.Joint, true));

            //noShowOccurrences
            Pixyz.Scene.Native.NativeInterface.SelectPartsFromNoShow();
            noShowOccurrences = Pixyz.Scene.Native.NativeInterface.GetSelectedOccurrences().list;
            Pixyz.Scene.Native.NativeInterface.ClearSelection();

            CreateTextures();
            CreateMaterials();
            
            CreateMeshes();
            CreateTree(Scene);
            GetJoints();
            CreateRenderers();

            SetRootTransform();

            isReadyForCompletion = true;
            CheckCompletion();
        }

        private void SetRootTransform()
        {
            if(gameObject)
            {
                // Local root matrix can be different from identity (in .pxz)
                Geom.Native.Matrix4 rootMatrix = Pixyz.Scene.Native.NativeInterface.GetLocalMatrix(Pixyz.Scene.Native.NativeInterface.GetRoot());
                Matrix4x4 transform = Conversions.ConvertMatrix(rootMatrix);

                if (this.settings.scaleFactor != 1 && !applyScaleOnVertices)
                {
                    Matrix4x4 scale = Matrix4x4.identity;
                    scale.m00 = scale.m11 = scale.m22 = this.settings.scaleFactor;
                    transform = transform * scale;
                }

                if (!this.settings.isLeftHanded && !this.settings.avoidNegativeScale)
                {
                    Matrix4x4 symm = Matrix4x4.identity;
                    symm.m00 = -1.0f;
                    transform = transform * symm;
                }

                if (this.settings.isZUp)
                {
                    Matrix4x4 rotate = Matrix4x4.identity;
                    rotate.m11 = rotate.m22 = 0.0f;
                    rotate.m12 = 1.0f;
                    rotate.m21 = -1.0f;
                    transform = transform * rotate;
                }

                Matrix4x4 matrix = MathExtensions.GetLocalMatrix(gameObject.transform);
                MathExtensions.SetFromLocalMatrix(gameObject.transform, matrix * transform);
            }
        }

        private void MeshDefConverterProcessed(MeshDefinitionToMesh meshDefinitionConverter)
        {
            meshDefConverterProcessedCount++;
            CheckCompletion();
        }

        private void CheckCompletion()
        {
            if (isReadyForCompletion && !isCompleted && meshDefConverterProcessedCount >= meshes.Length)
            {
                if (conversionCallback != null)
                {
                    isCompleted = true;
                    conversionCallback?.Invoke();
                    Clear();
                }
            }
        }

        void CreateTree(Scene.Native.PackedTree sceneTree)
        {
            partGameObjects = new GameObject[meshes.Length];
            gameObjectsMaterials = new UnityEngine.Material[meshes.Length][];
            transformsJoints = new Transform[meshes.Length][];

            for (int i = 0; i < sceneTree.occurrences.length; i++)
            {
                //Create an object
                GameObject newGameObject = CreateGameObject(sceneTree, i);
                gameObjects.Add(newGameObject);


                //Set Parent 
                if (i != 0)
                {
                    int parentIndex = sceneTree.parents[i];
                    newGameObject.transform.SetParent(gameObjects[parentIndex].transform, false);

                }
                else
                {
                    if (!gameObject)
                        gameObject = newGameObject;

                }
            }

            string filename = Path.GetFileName(file);
            string extension = Path.GetExtension(filename).ToLower();

            if (extension == ".pxz")
                gameObject.name = filename;
        }

        #region Materials

        private Dictionary<string, UnityEngine.Material> _materialsInResources;
        private Dictionary<string, UnityEngine.Material> materialsInResources
        {
            get
            {
                if (_materialsInResources == null)
                {
                    _materialsInResources = new Dictionary<string, UnityEngine.Material>();
                    foreach (var material in Resources.LoadAll<UnityEngine.Material>(""))
                    {
                        if (_materialsInResources.ContainsKey(material.name))
                        {
                            Debug.LogWarning($"There are multiple materials with the name '{material.name}' in Resources.");
                        }
                        else
                        {
                            _materialsInResources.Add(material.name, material);
                        }
                    }
                }
                return _materialsInResources;
            }
        }
        void CreateMaterials()
        {
            var allMaterials = Material.Native.NativeInterface.GetAllMaterials();
            HashSet<uint> uniqueMats = new HashSet<uint>();

            foreach(uint matId in allMaterials.list)
            {
                if(!uniqueMaterials.ContainsKey(matId))
                {
                    uniqueMats.Add(matId);
                }
            }

            var materialDefList = Material.Native.NativeInterface.GetMaterialDefinitions(new Material.Native.MaterialList(uniqueMats.ToArray()));
            var materials = Conversions.ConvertMaterialExtracts(materialDefList, ref textures, settings.shader, settings.useMaterialsInResources, materialsInResources);

            for (int i = 0; i < materialDefList.length; i++)
            {
                uniqueMaterials.Add(materialDefList[i].id, materials[i]);
            }
        }

        private UnityEngine.Material GetMaterial(uint materialId)
        {
            UnityEngine.Material material;
            if(uniqueMaterials.TryGetValue(materialId, out material))
            {
                return material;
            }
            else
            {
                return GetDefaultMaterial();
            }
        }

        private UnityEngine.Material GetLineMaterial(UnityEngine.Color color)
        {
            if (uniqueLineColor.ContainsKey(color))
            {
                return uniqueLineColor[color];
            }
            else
            {
                Shader shader = ShaderUtilities.GetPixyzLineShader();
                if (shader == null)
                    throw new Exception("Shader 'Pixyz/Simple Lines' not found. Make sure it is in your project and included in your build if you are running from a build.");
                UnityEngine.Material material = new UnityEngine.Material(shader);
                material.name = "Line #" + ColorUtility.ToHtmlStringRGB(color);
                material.color = color;
                uniqueLineColor.Add(color, material);
                return material;
            }
        }

        private UnityEngine.Material _defaultMaterial;
        private UnityEngine.Material GetDefaultMaterial()
        {
            if (_defaultMaterial == null)
            {
                _defaultMaterial = new UnityEngine.Material(settings.shader ?? ShaderUtilities.GetDefaultShader());
                _defaultMaterial.name = "DefaultMaterial";
                _defaultMaterial.color = new UnityEngine.Color(0.5f, 0.5f, 0.5f, 1f);
                uniqueMaterials.Add(0, _defaultMaterial);
            }
            return _defaultMaterial;
        }

        private UnityEngine.Material _pointMaterial;
        private UnityEngine.Material GetPointMaterial()
        {
            if (_pointMaterial == null)
            {
                var shader = Shader.Find("Pixyz/Splats");
                if (shader == null)
                    throw new Exception("Shader 'Pixyz/Splats' not found. Make sure it is in your project and included in your build if you are running from a build.");
                _pointMaterial = new UnityEngine.Material(shader);
                _pointMaterial.name = "Point Unlit";
            }
            return _pointMaterial;
        }
        
        private void CreateTextures()
        {
            var allImages = Material.Native.NativeInterface.GetAllImages();
            
            for (int i = 0; i < allImages.length; i++)
            {
                if (allImages[i] == 0)
                    continue;
                var imageDef = Material.Native.NativeInterface.GetImageDefinition(allImages[i]);
                if (!textures.ContainsKey(imageDef.id))
                    textures.Add(
                        imageDef.id,
                        Conversions.ConvertImageDefinition(imageDef));
                
            }
        }
        #endregion

        #region Meshes
        void CreateMeshes()
        {
            var parts = Pixyz.Scene.Native.NativeInterface.GetComponentByOccurrence(Scene.occurrences, Pixyz.Scene.Native.ComponentType.Part, true);
            Scene.Native.PartList partList = new Scene.Native.PartList(parts.list);
            Polygonal.Native.MeshList meshList = Pixyz.Scene.Native.NativeInterface.GetPartsMeshes(partList);
            Dictionary<uint, Mesh> convertesMesh = new Dictionary<uint, Mesh>();

            meshDefinitions = Polygonal.Native.NativeInterface.GetMeshDefinitions(meshList);
            //meshDefConverters = new MeshDefinitionToMesh[meshDefinitions.length];
            meshes = new Mesh[meshDefinitions.length];

            for(int i = 0; i < meshDefinitions.length; i++)
            {
                if (convertesMesh.ContainsKey(meshDefinitions[i].id))
                {
                    meshes[i] = convertesMesh[meshDefinitions[i].id];
                    continue;
                }

                Mesh mesh = new Mesh();
                mesh.name = "Mesh_" + meshDefinitions[i].id;

                this.meshes[i] = mesh;
                //TODO: ignore empty meshes
                Conversions.ConvertMeshDefinition(meshDefinitions[i], mesh, applyScaleOnVertices ? settings.scaleFactor : 1.0f);
                convertesMesh.Add(meshDefinitions[i].id, mesh);
                //this.meshes[i].name = "Mesh_" + (uint)meshList[i].GetInstanceID();//i;
            }
        }

        #endregion

        GameObject CreateGameObject(Scene.Native.PackedTree sceneTree, int occurrenceIndex)
        {
            //uint occurrence = sceneTree.occurrences.list[occurrenceIndex];
            var gameObject = new GameObject(sceneTree.names[occurrenceIndex]);

            //Transformation
            int matrixIndex = sceneTree.transformIndices[occurrenceIndex];
            
            if(matrixIndex != -1)
            {
                Matrix4x4 matrix = Conversions.ConvertMatrix(sceneTree.transformMatrices[matrixIndex]);
                    //matrix = sceneTree.transformMatrices[matrixIndex].ToUnityObject();
                if(settings.scaleFactor != 1.0f && applyScaleOnVertices)
                {
                    matrix.m03 *= settings.scaleFactor;
                    matrix.m13 *= settings.scaleFactor;
                    matrix.m23 *= settings.scaleFactor;
                }
                gameObject.transform.SetFromLocalMatrix(matrix);
            }

            //Geometry
            bool hasPart = meshDefinitions[occurrenceIndex].id != 0; // meshDefConverters[occurrenceIndex].meshDefinition.id != 0;
            if(hasPart)
            {
                //MeshDefinitionToMesh meshConverter = meshDefConverters[occurrenceIndex];
                Mesh mesh = this.meshes[occurrenceIndex];
                UnityEngine.Material[] materials = new UnityEngine.Material[0];

                if (this.meshes[occurrenceIndex].vertexCount != 0)
                {
                    polyCount += mesh.GetPolycount();
                    objectCount++;

                    //Assign materials
                    uint matRef = Scene.materials.list[occurrenceIndex];
                    
                    materials = new UnityEngine.Material[mesh.subMeshCount];
                    for(int i=0; i<materials.Length;i++)
                    {
                        if(mesh.GetSubMesh(i).topology == MeshTopology.Lines)
                        {
                            //if (i < mesh.subMeshCount - mesh.lines.length)
                            //    return UnityEngine.Color.black;
                            //ImportSDK.Core.Native.ColorAlpha color = meshDefinition.lines[submesh - (subMeshCount - meshDefinition.lines.length)].color;
                            //c = new UnityEngine.Color((float)color.r, (float)color.g, (float)color.b, (float)color.a);
                            materials[i] = GetLineMaterial(UnityEngine.Color.black);
                        }
                        else if(mesh.GetSubMesh(i).topology == MeshTopology.Points)
                        {
                            materials[i] = GetPointMaterial();
                        }
                        else
                        {
                            if (matRef <= 0) //Not having material on part
                            {
                                if (i > meshDefinitions[occurrenceIndex].dressedPolys.length - 1)
                                {
                                    materials[i] = GetLineMaterial(UnityEngine.Color.black);
                                }
                                else
                                {
                                    uint subMeshMatId = meshDefinitions[occurrenceIndex].dressedPolys[i].material;
                                    UnityEngine.Material newMaterial = GetMaterial(subMeshMatId);
                                    materials[i] = GetMaterial(subMeshMatId);
                                }
                            }
                            else //Having material on part
                            {
                                materials[i] = GetMaterial(matRef);
                            }
                        }
                    }
                }

                gameObjectsMaterials[occurrenceIndex] = materials;
                partGameObjects[occurrenceIndex] = gameObject;
            }

            //Metadata 
            Metadata metadata = null;
            if (metadataByOccurrences.list[occurrenceIndex]._base.length > 0 && settings.loadMetadata)
            {
                Scene.Native.PropertyValueList valueList = metadataByOccurrences.list[occurrenceIndex]._base;
                string[] names = new string[metadataByOccurrences.list[occurrenceIndex]._base.length];
                string[] values = new string[metadataByOccurrences.list[occurrenceIndex]._base.length];
                for (int i = 0; i < valueList.length; i++)
                {
                    names[i] = valueList.list[i].name;
                    values[i] = valueList.list[i].value;
                }
                metadata = gameObject.AddComponent<Metadata>();
                metadata.type = "Metadata";

                metadata.setProperties(new Properties(names, values));
            }

            // Joints
            if (jointByOccurrences[occurrenceIndex] > 0)
            {
                pixyzJointToTransform.Add(Pixyz.Scene.Native.NativeInterface.GetOccurrenceJoint(sceneTree.occurrences[occurrenceIndex]), gameObject.transform);
            }

            // Custom properties
            if (sceneTree.customProperties[occurrenceIndex].length > 0 && settings.loadMetadata)
            {
                if (metadata == null)
                {
                    metadata = gameObject.AddComponent<Metadata>();
                    metadata.type = "Metadata";
                }

                for (int i = 0; i < sceneTree.customProperties[occurrenceIndex].length; i++)
                {
                    string name = sceneTree.customProperties[occurrenceIndex][i].key;
                    string value = sceneTree.customProperties[occurrenceIndex][i].value;
                    metadata.addOrSetProperty(name, value);
                }
            }

            meshDefConverterProcessedCount++;

            if (noShowOccurrences.Contains(sceneTree.occurrences[occurrenceIndex]))
                gameObject.SetActive(false);

            return gameObject;
        }

        void GetJoints()
        {
            transformsJoints = new Transform[meshDefinitions.length][];
            for (int i = 0; i < meshDefinitions.length; i++)
            {
                transformsJoints[i] = new Transform[meshDefinitions[i].joints.length];
                for (int j = 0; j < meshDefinitions[i].joints.length; j++)
                {
                    if (pixyzJointToTransform.TryGetValue(meshDefinitions[i].joints[j], out Transform joint))
                    {
                        transformsJoints[i][j] = joint;
                    }
                }
            }
        }

        private void CreateRenderers()
        {
            for (int i = 0; i < meshDefinitions.length; i++)
            {
                if (meshDefinitions[i].id == 0) continue;

                GameObject gameObject = partGameObjects[i];
                Mesh mesh = meshes[i];
                UnityEngine.Material[] materials = gameObjectsMaterials[i];

                Renderer meshRenderer;

                if (meshDefinitions[i].joints.length == 0)
                {
                    if (mesh.vertexCount > 0)
                    {
                        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
                        meshFilter.sharedMesh = mesh;

                        if (materials.Length == 0)
                        {
                            materials = new UnityEngine.Material[1];
                            materials[0] = new UnityEngine.Material(ShaderUtilities.GetDefaultShader());
                        }
                        meshRenderer = gameObject.AddComponent<MeshRenderer>();
                        meshRenderer.sharedMaterials = materials;
                    }

                }
                else
                {
                    var skinnedMeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
                    skinnedMeshRenderer.sharedMesh = mesh;
                    skinnedMeshRenderer.bones = transformsJoints[i];
                    //skinnedMeshRenderer.rootBone = bones[0];
                    meshRenderer = skinnedMeshRenderer;
                    meshRenderer.sharedMaterials = materials;
                }
                
            }
        }

        private int GetLODNumber(int occurrence)
        {
            int length = metadataByOccurrences[occurrence]._base.list.Length;
            for (int i = 0; i < length; ++i)
            {
                if (metadataByOccurrences[occurrence]._base.list[i].name == LOD_NUMBER_ATTR)
                {
                    int num = -1;
                    int.TryParse(metadataByOccurrences[occurrence]._base.list[i].value, out num);
                    return num;
                }
            }
            return -1;
        }

        private void Clear()
        {
            foreach (LODGroup lodGroup in lodGroups)
            {
                lodGroup.RecalculateBounds();
            }
            try
            {
                Core.Native.NativeInterface.ResetSession();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}

