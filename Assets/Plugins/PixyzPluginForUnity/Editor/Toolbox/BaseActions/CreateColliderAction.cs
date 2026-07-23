using System.Collections.Generic;
using System;
using UnityEngine;
using Pixyz.Commons;
using Pixyz.Commons.Extensions;
using Pixyz.OptimizeSDK;
using Pixyz.Commons.UI.Editor;
using Pixyz.OptimizeSDK.Editor.MeshProcessing;
using UnityEngine.Rendering;

namespace Pixyz.Toolbox.Editor
{
    public class CreateColliderAction : PixyzFunction
    {
        public override int id => 14249905;
        public override int order => 0;
        public override string menuPathRuleEngine => "Colliders/Create Collider";
        public override string menuPathToolbox => "Colliders/Create Collider";
        public override string tooltip => ToolboxTooltips.createColliderAction;
        protected override MaterialSyncType SyncMaterials => MaterialSyncType.SyncNone;

        public enum ProxyStrategy
        {
            Retopology,
            ConvexDecomposition,
            AxisAlignedBoundingBox,
            OriginalMesh
        }

        private bool asyncCheck() 
        {
            switch (strategy)
            {
                case ProxyStrategy.AxisAlignedBoundingBox:
                case ProxyStrategy.OriginalMesh:
                    _isAsync = false;
                    break;
                default:
                    _isAsync = true;
                    break;
            }
            return true;
        }

        private bool isConvexDecomposition() => strategy == ProxyStrategy.ConvexDecomposition;
        private bool isRetopology() => strategy == ProxyStrategy.Retopology;
        private bool isOriginalMesh() => strategy == ProxyStrategy.OriginalMesh;
        private bool isAABB() => strategy == ProxyStrategy.AxisAlignedBoundingBox;
        private bool isFeatureSizeActive() => isFieldAligned() && useFeatureSize;

        [UserParameter("asyncCheck", displayName:"Strategy", tooltip:ToolboxTooltips.createColliderStrategy)]
        public ProxyStrategy strategy = ProxyStrategy.Retopology;

        #region ConvexDecomposition Parameters

        [UserParameter("isConvexDecomposition", tooltip: ToolboxTooltips.createColliderMaxDecompo)]
        public int maxDecompositionPerMesh = 3;

        [UserParameter("isConvexDecomposition", tooltip: ToolboxTooltips.createColliderTriangles)]
        public int maxTrianglesPerMesh = 200;

        [UserParameter("isConvexDecomposition", tooltip: ToolboxTooltips.createColliderResolution)]
        public Commons.Range resolution = (Commons.Range)50f;

        #endregion

        #region Retopoligize Parameters

        [UserParameter("isRetopology", displayName: "Type", tooltip: ToolboxTooltips.createColliderType)]
        public RetopologizeType type = RetopologizeType.Standard;

        [UserParameter("isFieldAligned", displayName:"Strategy", tooltip: ToolboxTooltips.retopologizeStrategy)]
        public VertexTarget criterion = VertexTarget.Ratio;

        [UserParameter("isPolycount", displayName: "Triangles Count" ,tooltip: ToolboxTooltips.retopologizeTriangles)]
        public int targetTriangleCount = 10000;

        [UserParameter("isPolycountRatio", displayName:"Ratio" ,tooltip: ToolboxTooltips.retopologizeRatio)]
        public Commons.Range targetRatio = (Commons.Range)10f;

        [UserParameter("isFieldAligned", tooltip: ToolboxTooltips.retopologizeUseFeature)]
        public bool useFeatureSize = false;

        [UserParameter("isFeatureSizeActive", tooltip: ToolboxTooltips.retopologizeFeatureSize)]
        public float featureSize = 0.1f;

        [UserParameter("isStandard", displayName:"Mesh quality preset", tooltip: ToolboxTooltips.retopologizeQuality)]
        public GridResolutionPreset gridResolutionPreset = GridResolutionPreset.Medium;

        [UserParameter("isStandard", displayName: "Quality value", tooltip: ToolboxTooltips.retopologizeQualityValue)]
        public int gridResolution = (int)GridResolutionPreset.Medium;

        [UserParameter("isStandard", tooltip: ToolboxTooltips.retopologizePtCloud)]
        public bool isPointCloud = false;

        private bool isFieldAligned() { return type == RetopologizeType.FieldAligned && isRetopology(); }
        private bool isStandard() { return type == RetopologizeType.Standard && isRetopology(); }
        private bool isPolycount() => criterion == VertexTarget.TriangleCount && isFieldAligned() && isRetopology();
        private bool isPolycountRatio() => criterion == VertexTarget.Ratio && isFieldAligned() && isRetopology();

        #endregion

        private List<uint> outputMeshes = new List<uint>();
        private List<uint[]> outputDecompo = new List<uint[]>();

        private bool skinnedMesh = false;
        private GridResolutionPreset _prevGridResolutionPreset = GridResolutionPreset.Medium;
        private int _prevGridResolution = (int)GridResolutionPreset.Medium;

        public override void onBeforeDraw()
        {
            base.onBeforeDraw();

            if (_prevGridResolutionPreset != gridResolutionPreset)
            {
                gridResolution = (int)gridResolutionPreset;
                _prevGridResolutionPreset = gridResolutionPreset;
                _prevGridResolution = gridResolution;
            }
            else if (gridResolution != _prevGridResolution)
            {
                switch ((GridResolutionPreset)gridResolution)
                {
                    case GridResolutionPreset.VeryHigh:
                        gridResolutionPreset = GridResolutionPreset.VeryHigh;
                        break;
                    case GridResolutionPreset.High:
                        gridResolutionPreset = GridResolutionPreset.High;
                        break;
                    case GridResolutionPreset.Medium:
                        gridResolutionPreset = GridResolutionPreset.Medium;
                        break;
                    case GridResolutionPreset.Low:
                        gridResolutionPreset = GridResolutionPreset.Low;
                        break;
                    case GridResolutionPreset.Poor:
                        gridResolutionPreset = GridResolutionPreset.Poor;
                        break;
                    default:
                        gridResolutionPreset = GridResolutionPreset.Custom;
                        break;
                }
                _prevGridResolutionPreset = gridResolutionPreset;
                _prevGridResolution = gridResolution;
            }
        }

        public override void onSelectionChanged(IList<GameObject> selection)
        {
            base.onSelectionChanged(selection);
            skinnedMesh = false;

            foreach (var go in selection)
            {
                Renderer r = go.GetComponent<Renderer>();
                if (r == null)
                    continue;

                if (r is SkinnedMeshRenderer && !skinnedMesh)
                {
                    skinnedMesh = true;
                }
            }
        }

        public override bool preProcess(IList<GameObject> input)
        {
            switch(strategy)
            {
                case ProxyStrategy.AxisAlignedBoundingBox:
                case ProxyStrategy.OriginalMesh:
                    _input = input;
                    _output = _input;
                    break;
                default:
                    return base.preProcess(input);
            }

            return true;
        }

        protected override void process()
        {
            if (strategy == ProxyStrategy.AxisAlignedBoundingBox || strategy == ProxyStrategy.OriginalMesh)
                return;

            outputMeshes.Clear();
            outputDecompo.Clear();
            try {
                uint root = Scene.Native.NativeInterface.CreateSceneFromMeshes(Context.pixyzMeshes, Context.pixyzMatrices, true);
                Scene.Native.OccurrenceList occurrenceList = new Scene.Native.OccurrenceList(new uint[] { root });

                Core.Native.NativeInterface.PushAnalytic("CreateCollider", strategy.ToString());
                UpdateProgressBar(0.25f);

                Polygonal.Native.TopologyCategoryMask topologyCategoryMask = new Polygonal.Native.TopologyCategoryMask();
                topologyCategoryMask.connectivity = Polygonal.Native.TopologyConnectivityMask.BOUNDARY_NONMANIFOLD;
                topologyCategoryMask.dimension = Polygonal.Native.TopologyDimensionMask.FACE;
                Algo.Native.NativeInterface.MergeVertices(occurrenceList, 0.0000001, topologyCategoryMask);
                UpdateProgressBar(0.35f);
                switch (strategy)
                {
                    case ProxyStrategy.ConvexDecomposition:
                        ConvexDecompositionProcess(root);
                        break;
                    case ProxyStrategy.Retopology:
                        RetopologizeProcess(root);
                        break;
                    default:
                        break;
                }
                UpdateProgressBar(0.9f, "Post processing...");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Error] {e.Message} \n {e.StackTrace}");
            }
        }

        private void RetopologizeProcess(uint root)
        {
            Scene.Native.OccurrenceList occurrenceList = new Scene.Native.OccurrenceList(new uint[] { root });

            Geom.Native.AABB aabb = Scene.Native.NativeInterface.GetAABB(occurrenceList);
            
            // Proxy Mesh and Retopologize functions returns new occurrences. This will be the occurrences to return.
            var newOccurrenceList = new Scene.Native.OccurrenceList(1); // Will get the result of proxy mesh functions

            if (type == RetopologizeType.FieldAligned)
            {
                
                if (isPolycount())
                {
                    newOccurrenceList[0] = Algo.Native.NativeInterface.Retopologize(occurrenceList, targetTriangleCount, true, false, useFeatureSize ? featureSize : -1);
                }
                else if (isPolycountRatio())
                {
                    int polyCount = Scene.Native.NativeInterface.GetPolygonCount(occurrenceList, true, true, true);
                    newOccurrenceList[0] = Algo.Native.NativeInterface.Retopologize(occurrenceList, (int)(0.01f * targetRatio * polyCount), true, false, useFeatureSize ? featureSize : -1);
                }
                Algo.Native.NativeInterface.BakeVertexAttributes(newOccurrenceList, occurrenceList, true, false, false);
            }
            else if (type == RetopologizeType.Standard)
            {
                //
                if (isPointCloud)
                {
                    Process.Native.GenerateDiffuseMap generateMap = new Process.Native.GenerateDiffuseMap()
                    {
                        _type = Process.Native.GenerateDiffuseMap.Type.NO,
                    };
                    newOccurrenceList[0] = Process.Native.NativeInterface.ProxyFromPointCloud(occurrenceList, gridResolution, generateMap, false);
                }
                else
                {
                    var bakeOptions = new Process.Native.BakeOptionSelector()
                    {
                        _type = Process.Native.BakeOptionSelector.Type.NO
                    };
                    newOccurrenceList[0] = Process.Native.NativeInterface.ProxyFromMeshes(occurrenceList, gridResolution, bakeOptions, true, false);
                }
                //

                //float featureSize = Mathf.Max((float)(aabb.high.x - aabb.low.x), (float)(aabb.high.y - aabb.low.y), (float)(aabb.high.z - aabb.low.z)) / gridResolution;

                ////Remesh
                //newOccurrenceList[0] = Algo.Native.NativeInterface.ProxyMesh(occurrenceList, featureSize, Algo.Native.ElementFilter.Polygons, 0, false/*, false*/);
                //Algo.Native.NativeInterface.BakeVertexAttributes(newOccurrenceList, occurrenceList, true, false, false);

                ////DecimateToQualityVertexRemoval
                //Algo.Native.NativeInterface.Decimate(newOccurrenceList, 0.001, 0.0005, -1, -1, false);
            }

            // Get newOccurrenceList mesh and update Context with this
            Polygonal.Native.MeshList newMeshes = new Polygonal.Native.MeshList(new uint[] { Scene.Native.NativeInterface.GetPartMesh(Scene.Native.NativeInterface.GetComponent(newOccurrenceList[0], Scene.Native.ComponentType.Part, false)) });

            Context.pixyzMeshes = newMeshes;
            Context.pixyzMatrices = new Geom.Native.Matrix4List(new Geom.Native.Matrix4[] { Conversions.Identity() });
        }

        private void ConvexDecompositionProcess(uint root)
        {
            Scene.Native.OccurrenceList occurrenceList = new Scene.Native.OccurrenceList(new uint[] { root });
            var occurrences = Scene.Native.NativeInterface.GetPartOccurrences(root);

            int vertexCount = 2 + maxTrianglesPerMesh / 2; //Euler characteristic
            int res = (int)convertRange(resolution, 10000, 64000000);

            Algo.Native.NativeInterface.ConvexDecomposition(occurrenceList, maxDecompositionPerMesh, vertexCount, true, res, 0.001);
            
            foreach (var occurrence in occurrences.list)
            {
                var newPartOccurrences = Scene.Native.NativeInterface.GetChildren(occurrence);
                var parts = new Scene.Native.PartList(Scene.Native.NativeInterface.GetComponentByOccurrence(newPartOccurrences, Scene.Native.ComponentType.Part, true));
                Polygonal.Native.MeshList meshes = Scene.Native.NativeInterface.GetPartsMeshes(parts);
                outputDecompo.Add(meshes);
            }

            Scene.Native.NativeInterface.ResetPartTransform(root);
        }


        protected override void postProcess()
        {
            base.postProcess();

            switch (strategy)
            {
                case ProxyStrategy.ConvexDecomposition:
                        PostProcessConvexDecomposition();
                    break;
                case ProxyStrategy.AxisAlignedBoundingBox:
                        PostProcessAxisAlignedBoundingBox();
                    break;
                case ProxyStrategy.OriginalMesh:
                        PostProcessOriginalMesh();
                    break;
                case ProxyStrategy.Retopology:
                        PostProcessRetopology();
                    break;
            }
        }

        private void PostProcessRetopology()
        {
            _output = Context.PixyzMeshToUnityObject(Context.pixyzMeshes);
            
            int polyCount = _output.GetMeshes().GetPolyCount();
            foreach (var go in (IList<GameObject>)Output)
            {
                go.name = "Retopo-" + polyCount;
                MeshCollider collider = go.GetOrAddComponent<MeshCollider>();
                MeshFilter filter = go.GetComponent<MeshFilter>();
                collider.sharedMesh = filter.sharedMesh;
                GameObject.DestroyImmediate(go.GetComponent<Renderer>());
                GameObject.DestroyImmediate(filter);
            }
        }

        private void PostProcessOriginalMesh()
        {
            Dictionary<Mesh, Mesh> cleanedMesh = new Dictionary<Mesh, Mesh>();
            foreach (GameObject gameObject in _input)
            {
                MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
                if (!meshFilter)
                    continue;

                Mesh mesh = meshFilter.sharedMesh;

                MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
                if (!mesh || !meshRenderer)
                    continue;

                //Check if there is submesh with topology other than triangle/Quadrangles
                bool fastAttribution = true;
                for (int i = 0; i < mesh.subMeshCount; ++i)
                {
                    SubMeshDescriptor desc = mesh.GetSubMesh(i);
                    if (desc.topology == MeshTopology.Quads || desc.topology == MeshTopology.Triangles)
                        continue;
                    fastAttribution = false;
                }

                if (fastAttribution)
                {
                    MeshCollider meshCollider = gameObject.GetOrAddComponent<MeshCollider>();
                    meshCollider.sharedMesh = mesh;
                }
                else
                {
                    if (mesh.subMeshCount == 1)
                        continue;

                    if(cleanedMesh.ContainsKey(mesh))
                    {
                        MeshCollider collider = gameObject.GetOrAddComponent<MeshCollider>();
                        collider.sharedMesh = cleanedMesh[mesh];

                        continue;
                    }

                    //We need to create a new a mesh without submesh with the wrong topology and also remove all vertex attributes from submeshes that we are deleting.
                    Mesh physicMesh = createCleanMesh(mesh);

                    if (physicMesh.subMeshCount == 0)
                        continue;

                    MeshCollider meshCollider = gameObject.GetOrAddComponent<MeshCollider>();
                    meshCollider.sharedMesh = physicMesh;

                    cleanedMesh.Add(mesh, physicMesh);
                }
            }
        }

        private void PostProcessAxisAlignedBoundingBox()
        {
            foreach (GameObject gameObject in _input)
            {
                MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
                if (!meshFilter)
                    continue;
                Mesh mesh = meshFilter.sharedMesh;
                MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
                if (!mesh || !meshRenderer)
                    continue;
                BoxCollider boxCollider = gameObject.GetOrAddComponent<BoxCollider>();
                boxCollider.center = mesh.bounds.center;
                boxCollider.size = mesh.bounds.size;
            }
        }

        private void PostProcessConvexDecomposition()
        {
            _output = (IList<GameObject>)Input;
            List<GameObject> gameObjects = new List<GameObject>();
            for (int i = 0; i < outputDecompo.Count; i++)
            {
                uint[] decomposition = outputDecompo[i];
                GameObject current = InputParts[i];
                var decompoObjects = Context.PixyzMeshToUnityObject(new Polygonal.Native.MeshList(decomposition));
                var meshes = decompoObjects.GetMeshes();

                // Destroys previous colliders
                foreach (var oldCollider in current.GetComponents<MeshCollider>())
                {
                    UnityEditor.Undo.DestroyObjectImmediate(oldCollider);
                }

                // Adds new colliders
                foreach (var mesh in meshes)
                {
                    if (mesh.subMeshCount == 0 || mesh.vertexCount == 0)
                        continue;

                    MeshFilter meshFilter = current.GetComponent<MeshFilter>();
                    Mesh originalMesh = meshFilter.sharedMesh;

                    meshFilter.sharedMesh = mesh;
                    Debug.Log(current.name);
                    //We have to do this that way because when creating a MeshCollider by default it will take the mesh on the meshFilter... so it might throw errors if we leave the uncleaned mesh(with lines & points) in the meshfilter
                    MeshCollider meshCollider = UnityEditor.Undo.AddComponent<MeshCollider>(current);
                    //meshCollider.sharedMesh = cleanedMesh;
                    meshCollider.convex = true;

                    //Then we re-assign the original mesh with lines & points
                    meshFilter.sharedMesh = originalMesh;
                }
                gameObjects.AddRange(decompoObjects);
            }

            foreach (var go in gameObjects)
                GameObject.DestroyImmediate(go); // Don't use Undo here, those were temporary objects
        }

        public override IList<string> getErrors()
        {
            var errors = new List<string>();
            if (isConvexDecomposition())
            {
                if (maxDecompositionPerMesh <= 0)
                {
                    errors.Add("Max decomposition is too low ! (must be higher than 0)");
                }
                if (maxTrianglesPerMesh <= 0)
                {
                    errors.Add("Max triangles is too low ! (must be higher than 0)");
                }
                if (maxTrianglesPerMesh > 255)
                {
                    errors.Add("Max triangles is too high ! (must be lower than 264)");
                }
            }

            if (isStandard())
            {
                if (featureSize <= 0 && useFeatureSize)
                    errors.Add("Feature size is too low ! (must be higher than 0)");
            }

            if(skinnedMesh)
            {
                if (!isConvexDecomposition())
                    errors.Add("Selection contains Skinned Mesh Renderer.\nCreating a collider is not possible.");
            }

            return errors.ToArray();
        }

        public override IList<string> getWarnings()
        {
            var warnings = new List<string>();
            if (isConvexDecomposition() && skinnedMesh)
            {
                warnings.Add("Selection contains Skinned Mesh Renderer but the output convex decomposition won't be animated.");
            }

            if (isStandard())
            {
                if (gridResolution >= 1000)
                    warnings.Add("Quality value is too high! (The execution can take a lot of time)");
                if (gridResolution <= 0)
                    warnings.Add("Quality value is too low! (must be higher than 0)");
            }
            return warnings;
        }

        private double convertRange(double range, double minv, double maxv)
        {
            double minp = 0;
            double maxp = 100;

            double minlog = Math.Log(minv);
            double maxlog = Math.Log(maxv);

            // calculate adjustment factor
            double scale = (maxlog - minlog) / (maxp - minp);
            return Math.Exp(minlog + scale * (range - minp));
        }

        private Mesh createCleanMesh(Mesh mesh)
        {
            //We need to create a new a mesh without submesh with the wrong topology and also remove all vertex attributes from submeshes that we are deleting.
            Mesh physicMesh = new Mesh();
            Dictionary<VertexAttribute, bool> activeAttributes = new Dictionary<VertexAttribute, bool>();
            foreach (int value in Enum.GetValues(typeof(VertexAttribute)))
            {
                activeAttributes.Add((VertexAttribute)value, mesh.HasVertexAttribute((VertexAttribute)value));
            }

            List<Vector3> positions = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector4> tangents = new List<Vector4>();
            List<Color32> colors32 = new List<Color32>();
            List<Vector2> uv = new List<Vector2>();
            List<Vector2> uv2 = new List<Vector2>();
            List<Vector2> uv3 = new List<Vector2>(0);
            List<Vector2> uv4 = new List<Vector2>(0);
            List<Vector2> uv5 = new List<Vector2>(0);
            List<Vector2> uv6 = new List<Vector2>(0);
            List<Vector2> uv7 = new List<Vector2>(0);
            List<Vector2> uv8 = new List<Vector2>(0);
            List<BoneWeight> boneWeights = new List<BoneWeight>();
            List<int> triangles = new List<int>();
            List<ushort> trianglesShort = new List<ushort>(0);
            Dictionary<int, int> mapIndices = new Dictionary<int, int>();
            List<SubMeshDescriptor> submeshes = new List<SubMeshDescriptor>();

            for (int i = 0; i < mesh.subMeshCount; ++i)
            {
                SubMeshDescriptor desc = mesh.GetSubMesh(i);

                if (desc.topology != MeshTopology.Triangles && desc.topology != MeshTopology.Quads)
                    continue;

                int baseVertex = positions.Count;

                Vector3[] rawPositions = mesh.vertices;
                Vector3[] rawNormals = mesh.normals;
                Vector4[] rawTangents = mesh.tangents;
                Color32[] rawColors32 = mesh.colors32;
                Vector2[] rawUv = mesh.uv;
                Vector2[] rawUv2 = mesh.uv2;
                Vector2[] rawUv3 = mesh.uv3;
                Vector2[] rawUv4 = mesh.uv4;
                Vector2[] rawUv5 = mesh.uv5;
                Vector2[] rawUv6 = mesh.uv6;
                Vector2[] rawUv7 = mesh.uv7;
                Vector2[] rawUv8 = mesh.uv8;
                BoneWeight[] rawBoneWeights = mesh.boneWeights;

                if (mesh.indexFormat == IndexFormat.UInt16)
                {
                    List<ushort> tri = new List<ushort>();
                    mesh.GetIndices(tri, i, false);
                    SubMeshDescriptor submesh = new SubMeshDescriptor(trianglesShort.Count, tri.Count, desc.topology);
                    if (desc.baseVertex != 0)
                        submesh.baseVertex = baseVertex;
                    submesh.topology = desc.topology;
                    int attrCount = 0;

                    for (int j = 0; j < tri.Count; ++j)
                    {
                        int index = tri[j] + desc.baseVertex;
                        if (!mapIndices.ContainsKey(index))
                        {
                            if (activeAttributes[VertexAttribute.Position])
                                positions.Add(rawPositions[index]);

                            if (activeAttributes[VertexAttribute.Normal])
                                normals.Add(rawNormals[index]);

                            if (activeAttributes[VertexAttribute.Tangent])
                                tangents.Add(rawTangents[index]);

                            if (activeAttributes[VertexAttribute.Color])
                                colors32.Add(rawColors32[index]);

                            if (activeAttributes[VertexAttribute.TexCoord0])
                                uv.Add(rawUv[index]);

                            if (activeAttributes[VertexAttribute.TexCoord1])
                                uv2.Add(rawUv2[index]);

                            if (activeAttributes[VertexAttribute.TexCoord2])
                                uv3.Add(rawUv3[index]);

                            if (activeAttributes[VertexAttribute.TexCoord3])
                                uv4.Add(rawUv4[index]);

                            if (activeAttributes[VertexAttribute.TexCoord4])
                                uv5.Add(rawUv5[index]);

                            if (activeAttributes[VertexAttribute.TexCoord5])
                                uv6.Add(rawUv6[index]);

                            if (activeAttributes[VertexAttribute.TexCoord6])
                                uv7.Add(rawUv7[index]);

                            if (activeAttributes[VertexAttribute.TexCoord7])
                                uv8.Add(rawUv8[index]);

                            if (activeAttributes[VertexAttribute.BlendWeight])
                                boneWeights.Add(rawBoneWeights[index]);

                            mapIndices.Add(index, (ushort)attrCount);
                            trianglesShort.Add((ushort)attrCount);
                            ++attrCount;
                        }
                        else
                            trianglesShort.Add((ushort)mapIndices[index]);
                    }
                    submeshes.Add(submesh);
                }
                else
                {
                    List<int> tri = new List<int>();
                    mesh.GetIndices(tri, i, false);

                    SubMeshDescriptor submesh = new SubMeshDescriptor(triangles.Count, tri.Count, desc.topology);
                    if (desc.baseVertex != 0)
                        submesh.baseVertex = baseVertex;
                    submesh.topology = desc.topology;
                    int attrCount = baseVertex;

                    for (int j = 0; j < tri.Count; ++j)
                    {
                        int index = tri[j] + desc.baseVertex;
                        if (!mapIndices.ContainsKey(index))
                        {
                            if (activeAttributes[VertexAttribute.Position])
                                positions.Add(rawPositions[index]);

                            if (activeAttributes[VertexAttribute.Normal])
                                normals.Add(rawNormals[index]);

                            if (activeAttributes[VertexAttribute.Tangent])
                                tangents.Add(rawTangents[index]);

                            if (activeAttributes[VertexAttribute.Color])
                                colors32.Add(rawColors32[index]);

                            if (activeAttributes[VertexAttribute.TexCoord0])
                                uv.Add(rawUv[index]);

                            if (activeAttributes[VertexAttribute.TexCoord1])
                                uv2.Add(rawUv2[index]);

                            if (activeAttributes[VertexAttribute.TexCoord2])
                                uv3.Add(rawUv3[index]);

                            if (activeAttributes[VertexAttribute.TexCoord3])
                                uv4.Add(rawUv4[index]);

                            if (activeAttributes[VertexAttribute.TexCoord4])
                                uv5.Add(rawUv5[index]);

                            if (activeAttributes[VertexAttribute.TexCoord5])
                                uv6.Add(rawUv6[index]);

                            if (activeAttributes[VertexAttribute.TexCoord6])
                                uv7.Add(rawUv7[index]);

                            if (activeAttributes[VertexAttribute.TexCoord7])
                                uv8.Add(rawUv8[index]);

                            if (activeAttributes[VertexAttribute.BlendWeight])
                                boneWeights.Add(rawBoneWeights[index]);

                            mapIndices.Add(index, attrCount);
                            triangles.Add(attrCount);
                            ++attrCount;
                        }
                        else
                            triangles.Add(mapIndices[index]);
                    }
                    submeshes.Add(submesh);
                }
            }

            physicMesh.indexFormat = mesh.indexFormat;
            physicMesh.vertices = positions.ToArray();
            physicMesh.normals = normals.ToArray();
            physicMesh.tangents = tangents.ToArray();
            physicMesh.colors32 = colors32.ToArray();
            physicMesh.uv = uv.ToArray();
            physicMesh.uv2 = uv2.ToArray();
            physicMesh.uv3 = uv3.ToArray();
            physicMesh.uv4 = uv4.ToArray();
            physicMesh.uv5 = uv5.ToArray();
            physicMesh.uv6 = uv6.ToArray();
            physicMesh.uv7 = uv7.ToArray();
            physicMesh.uv8 = uv8.ToArray();
            physicMesh.boneWeights = boneWeights.ToArray();
            physicMesh.subMeshCount = submeshes.Count;

            if (physicMesh.indexFormat == IndexFormat.UInt16)
            {
                for (int i = 0; i < submeshes.Count; ++i)
                {
                    SubMeshDescriptor submesh = submeshes[i];
                    ushort[] tri = new ushort[submesh.indexCount];
                    Array.Copy(trianglesShort.ToArray(), submesh.indexStart, tri, 0, submesh.indexCount);
                    physicMesh.SetIndices(tri, submesh.topology, i, true, submesh.baseVertex);
                }
            }
            else
            {
                for (int i = 0; i < submeshes.Count; ++i)
                {
                    SubMeshDescriptor submesh = submeshes[i];
                    int[] tri = new int[submesh.indexCount];
                    Array.Copy(triangles.ToArray(), submesh.indexStart, tri, 0, submesh.indexCount);
                    physicMesh.SetIndices(tri, submesh.topology, i, true, submesh.baseVertex);
                }
            }

            return physicMesh;
        }
    }
}
