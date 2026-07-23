using System.Collections.Generic;
using UnityEngine;
using Pixyz.Commons.UI.Editor;
using Pixyz.Commons.Extensions;
using System.IO;
using UnityEngine.Rendering;


namespace Pixyz.Toolbox.Editor
{
    public enum OctahedronImpostorType
    {
        Octahedron = 0,
        HemiOctahedron = 1,
    }

    public enum RenderImpostorOn
    {
        Quad,
        OrientedBoundingBox,
        CustomMesh
    }

    public enum Textures
    {
        AlbedoOcclusion = 0,
        SpecularRoughness = 1,
        Normals = 2,
        Emissive = 3,
        Depth = 4,
    }

    public class BakeImpostor : PixyzFunction
    {
        public override int id => 57248795;
        public override int order => 7;
        public override string menuPathRuleEngine => "Remeshing/Create Impostor";
        public override string menuPathToolbox => "Remeshing/Create Impostor";

        [UserParameter(tooltip: ToolboxTooltips.billboardResolution)]
        public MapDimensions mapsResolution = MapDimensions._2048;
        private bool isCustom() { return mapsResolution == MapDimensions.Custom; }

        [UserParameter("isCustom", displayName: "Maps Resolution", tooltip: ToolboxTooltips.impostorResolution)]
        public int resolution = 2048;

        [UserParameter(tooltip: ToolboxTooltips.impostorAtlasSize)]
        public int atlasSize = 18;

        [UserParameter(tooltip: ToolboxTooltips.impostorType)]
        public OctahedronImpostorType impostorType = OctahedronImpostorType.Octahedron;

        [UserParameter(tooltip: ToolboxTooltips.impostorRenderOn)]
        public RenderImpostorOn renderOn = RenderImpostorOn.Quad;

        private bool isCustomMesh() { return renderOn == RenderImpostorOn.CustomMesh; }

        [UserParameter("isCustomMesh", tooltip: ToolboxTooltips.impostorCustomMesh)]
        public GameObject customMesh;

        [UserParameter(tooltip: ToolboxTooltips.impostorSaveMaps)]
        public bool saveMaps = false;
        private bool SaveMaps() { return saveMaps == true; }

        [UserParameter("SaveMaps", tooltip: ToolboxTooltips.impostorMapsPath)]
        public string mapsPath = Application.dataPath;

        private GameObject impostor;
        private GameObject source;
        private Matrix4x4 sourceLocalToWorld;

        private float m_OctahedronDiameter;
        private float m_Depth;
        private Renderer[] m_renderers;
        private Vector3 m_originalPosition = Vector3.zero;
        private Quaternion m_originalRotation = Quaternion.identity;
        private Vector3 m_originalScale = Vector3.one;
        private Vector3 boundsCenter;
        private Bounds originalBounds;


        // Maps
        private int m_GBufferRTCount = 4;
        private int m_outputTextureCount = 5;
        private Texture2D[] m_outputTextures;
        public Texture2D[] OutputTextures => m_outputTextures;
        private RenderTexture[] m_GBuffers;
        private RenderTexture m_DepthRT;
        private RenderTexture m_OutDepthRT;

        public override bool preProcess(IList<GameObject> input)
        {
            source = input[input.Count - 1];
            sourceLocalToWorld = source.transform.localToWorldMatrix;
            m_outputTextures = new Texture2D[m_outputTextureCount];
            boundsCenter = source.GetBoundsWorldSpace(true).center;
            bool result = base.preProcess(input);
            if (!result)
                return false;
            return true;
        }

        protected override void process()
        {
            try
            {
                Core.Native.NativeInterface.PushAnalytic("BakingImpostor", "");
                UpdateProgressBar(0.25f);
                // Use Pixyz to calculate OBB
                if (renderOn == RenderImpostorOn.OrientedBoundingBox)
                {
                    uint scene = Scene.Native.NativeInterface.CreateSceneFromMeshes(Context.pixyzMeshes, Context.pixyzMatrices, false);
                    Scene.Native.OccurrenceList occurrenceList = new Scene.Native.OccurrenceList(new uint[] { scene });
                    Polygonal.Native.TopologyCategoryMask topologyMask = new Polygonal.Native.TopologyCategoryMask();
                    topologyMask.connectivity = Polygonal.Native.TopologyConnectivityMask.BOUNDARY_NONMANIFOLD;
                    topologyMask.dimension = Polygonal.Native.TopologyDimensionMask.FACE;
                    Algo.Native.NativeInterface.MergeVertices(occurrenceList, 0.0000001, topologyMask);
                    Scene.Native.NativeInterface.ResetPartTransform(occurrenceList[0]);
                    //OBB's occurrence
                    var obbOccurrence = Scene.Native.NativeInterface.CreateOBBMesh(occurrenceList[0]);
                    ////Octahedron transform
                    Geom.Native.Matrix4 pivot = Pixyz.OptimizeSDK.Conversions.ConvertMatrix(sourceLocalToWorld);
                    pivot[0][3] = boundsCenter.x;
                    pivot[1][3] = boundsCenter.y;
                    pivot[2][3] = boundsCenter.z;
                    Scene.Native.NativeInterface.SetPivotOnly(obbOccurrence, pivot);
                    Scene.Native.NativeInterface.ResetPartTransform(obbOccurrence);
                    var outputMesh = Scene.Native.NativeInterface.GetPartMesh(Scene.Native.NativeInterface.GetComponent(obbOccurrence, Scene.Native.ComponentType.Part, true));
                    Context.pixyzMeshes = new Polygonal.Native.MeshList(new uint[] { outputMesh });
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Error] {e.Message} \n {e.StackTrace}");
            }
        }

        protected override void postProcess()
        {
            base.postProcess();
            BakeTextures();
            CreateImpostor();
            DisableInput();

        }

        public void SaveTextureAsPNG(Texture2D _texture, string _fileName)
        {

            byte[] _bytes = _texture.EncodeToPNG();
            string fullPath = mapsPath + _fileName + ".png";
            System.IO.File.WriteAllBytes(fullPath, _bytes);
            Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + fullPath);

        }

        private void SetTransforms()
        {
            impostor.transform.position = boundsCenter;
            GameObject impostorPivot = new GameObject($"{source.name}_Impostor_Pivot");
            impostorPivot.transform.position = source.transform.position;
            impostorPivot.transform.rotation = source.transform.rotation;
            impostorPivot.transform.localScale = source.transform.localScale;
            impostor.transform.SetParent(impostorPivot.transform);
            impostor.transform.localRotation = Quaternion.identity;
            impostor.transform.localScale = Vector3.one;
            UnityEditor.Selection.activeObject = impostorPivot;
        }

        private void CreateImpostor()
        {


            if (renderOn == RenderImpostorOn.Quad)
            {
                // PxzImpostor
                GameObject gameObject = new GameObject("PxzImpostor", typeof(MeshFilter), typeof(MeshRenderer));


                // Mesh
                Vector3[] vertices =
                {
                    new Vector3(-0.5f,-0.5f),
                    new Vector3(0.5f,-0.5f),
                    new Vector3(0.5f,0.5f),
                    new Vector3(-0.5f,0.5f)
                };
                int[] triangles =
                {
                   0,2,1,
                   0,3,2,
                };
                Vector2[] uvs =
                {
                    new Vector2(0,0),
                    new Vector2(1,0),
                    new Vector2(1,1),
                    new Vector2(0,1)
                };


                Mesh mesh = new Mesh();
                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.uv = uvs;
                mesh.bounds = originalBounds;

                gameObject.GetComponent<MeshFilter>().mesh = mesh;
                gameObject.GetComponent<MeshRenderer>().material = CreateMaterial();
                gameObject.name = $"{source.name}_PixyzImpostor";
                impostor = gameObject;

            }
            else
            {
                GameObject output;

                if (renderOn == RenderImpostorOn.OrientedBoundingBox)
                    output = Context.PixyzMeshToUnityObject(Context.pixyzMeshes)[0];
                else
                    output = customMesh;

                output.name = $"{source.name}_PixyzImpostor";
                output.GetComponent<MeshRenderer>().material = CreateMaterial();
                impostor = output;
            }
            SetTransforms();
        }

        private UnityEngine.Material CreateMaterial()
        {
            Shader shader = Shader.Find("Pixyz/PxzImpostor");

            UnityEngine.Material material = new UnityEngine.Material(shader);


            //shader properties
            material.SetTexture(Shader.PropertyToID("_PxzImpostorAlbedoOcclusion"), m_outputTextures[(int)Textures.AlbedoOcclusion]);
            material.SetTexture(Shader.PropertyToID("_PxzImpostorSpecularRoughness"), m_outputTextures[(int)Textures.SpecularRoughness]);
            material.SetTexture(Shader.PropertyToID("_PxzImpostorNormals"), m_outputTextures[(int)Textures.Normals]);
            material.SetTexture(Shader.PropertyToID("_PxzImpostorEmission"), m_outputTextures[(int)Textures.Emissive]);
            material.SetTexture(Shader.PropertyToID("_PxzImpostorDepth"), m_outputTextures[(int)Textures.Depth]);



            if (impostorType == OctahedronImpostorType.HemiOctahedron)
                material.SetFloat("_FullOcta", 0);
            if (renderOn == RenderImpostorOn.OrientedBoundingBox || renderOn == RenderImpostorOn.CustomMesh)
                material.SetFloat("_PreciseMesh", 1);



            material.SetInt("_PxzImpostorFramesCount", atlasSize);
            material.SetFloat("_OctahedronDiameter", m_OctahedronDiameter);
            material.enableInstancing = true;
            return material;
        }

        public override IList<string> getErrors()
        {
            var errors = new List<string>();
            if (isCustom())
            {
                if (resolution < 64)
                {
                    errors.Add("Maps resolution is too low ! (must be between 64 and 8192)");
                }
                if (resolution > 8192)
                {
                    errors.Add("Maps resolution is too high ! (must be between 64 and 8192)");
                }
            }
            if (isCustomMesh())
            {
                if (customMesh == null)
                    errors.Add("Please select the GameObject on which the impostor should be rendered on");

            }
            if (atlasSize < 8)
            {
                errors.Add("Minimum atlas size is 8x8");
            }
            if (atlasSize % 2 != 0)
            {
                errors.Add("Atlas size must be a pair number");
            }
            return errors.ToArray();
        }

        public override IList<string> getWarnings()
        {
            var warnings = new List<string>();
            if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null)
            {
                warnings.Add("Baking maps is only compatible with built-in render pipeline");
            }
            return warnings.ToArray();
        }



        private Matrix4x4 GetCameraFacingMatrix(OctahedronImpostorType impostorType, int frames, int x, int y)
        {
            Matrix4x4 cameraFacingMatrix = Matrix4x4.identity;
            Vector3 forwardVector = Vector3.zero;

            if (impostorType == OctahedronImpostorType.Octahedron)
            {
                forwardVector = OctaEncode(((float)(x) / (float)(frames - 1)) * 2.0f - 1.0f, ((float)(y) / (float)(frames - 1)) * 2.0f - 1.0f);
            }
            else if (impostorType == OctahedronImpostorType.HemiOctahedron)
            {
                forwardVector = HemiOctaEncode(((float)(x) / (float)(frames - 1)) * 2.0f - 1.0f, ((float)(y) / (float)(frames - 1)) * 2.0f - 1.0f);

            }

            Quaternion rotation = Quaternion.LookRotation(-forwardVector, Vector3.up);
            cameraFacingMatrix = Matrix4x4.Rotate(rotation).inverse;
            return cameraFacingMatrix;
        }


        private Vector3 OctaEncode(float x, float y)
        {
            Vector3 forward = new Vector3(x, 1.0f - Mathf.Abs(x) - Mathf.Abs(y), y);
            float t = Mathf.Clamp01(-forward.y);
            forward.x = forward.x + (forward.x >= 0 ? -t : t);
            forward.z = forward.z + (forward.z >= 0 ? -t : t);
            return Vector3.Normalize(forward);
        }

        private Vector3 HemiOctaEncode(float x, float y)
        {
            float tempx = x;
            float tempy = y;

            x = (tempx + tempy) * 0.5f;
            y = (tempx - tempy) * 0.5f;
            Vector3 forward = new Vector3(x, 1.0f - Mathf.Abs(x) - Mathf.Abs(y), y);
            return Vector3.Normalize(forward);
        }

        private void InitRenderTextures()
        {
            int res = mapsResolution == MapDimensions.Custom ? resolution : (int)mapsResolution;
            m_GBuffers = new RenderTexture[m_GBufferRTCount];

            // https://docs.unity3d.com/2019.4/Documentation/Manual/RenderTech-DeferredShading.html
            // RT0, ARGB32 format: Diffuse color (RGB), occlusion (A).
            // RT1, ARGB32 format: Specular color

            for (int i = 0; i < 2; i++)
            {
                m_GBuffers[i] = new RenderTexture(res, res, 16, RenderTextureFormat.ARGB32);
                m_GBuffers[i].Create();
            }

            // https://docs.unity3d.com/2019.4/Documentation/Manual/RenderTech-DeferredShading.html
            // RT2, ARGB2101010 format: World space normal(RGB), unused(A).
            // RT3, ARGB2101010(non - HDR) or ARGBHalf(HDR) format: Emission + lighting + lightmaps+ reflection probes buffer.

            for (int i = 2; i < 4; i++)
            {
                m_GBuffers[i] = new RenderTexture(res, res, 16, RenderTextureFormat.ARGBHalf);
                m_GBuffers[i].Create();
            }
            m_DepthRT = new RenderTexture(res, res, 16, RenderTextureFormat.Depth);
            m_DepthRT.Create();


            m_OutDepthRT = new RenderTexture(res, res, 16, RenderTextureFormat.ARGBHalf);
            m_OutDepthRT.Create();


        }

        private Bounds TransformBounds(Bounds bounds, Matrix4x4 matrix)
        {
            var center = matrix.MultiplyPoint3x4(bounds.center);
            var extents = bounds.extents;

            var Xaxis = matrix.MultiplyVector(new Vector3(extents.x, 0, 0));
            var Yaxis = matrix.MultiplyVector(new Vector3(0, extents.y, 0));
            var Zaxis = matrix.MultiplyVector(new Vector3(0, 0, extents.z));

            extents.x = Mathf.Abs(Xaxis.x) + Mathf.Abs(Yaxis.x) + Mathf.Abs(Zaxis.x);
            extents.y = Mathf.Abs(Xaxis.y) + Mathf.Abs(Yaxis.y) + Mathf.Abs(Zaxis.y);
            extents.z = Mathf.Abs(Xaxis.z) + Mathf.Abs(Yaxis.z) + Mathf.Abs(Zaxis.z);

            return new Bounds { center = center, extents = extents };
        }

        void BakeTextures()
        {
            SaveTransform();
            UpdateProgressBar(0, "Please Wait... Preparing Resouces");
            GetAllRenderers();
            CalculateOctahedronDiameter(impostorType);
            InitRenderTextures();

            //DisplayProgress(0.5f, "Please Wait... Baking");

            CommandBuffer commandBuffer = new CommandBuffer();
            RenderTargetIdentifier[] rtIDs = new RenderTargetIdentifier[m_GBufferRTCount];
            for (int i = 0; i < m_GBufferRTCount; i++)
            {
                rtIDs[i] = m_GBuffers[i];
            }
            commandBuffer.SetRenderTarget(rtIDs, m_DepthRT);
            commandBuffer.ClearRenderTarget(true, true, Color.clear, 1);


            List<MeshFilter> validMeshFilters = new List<MeshFilter>();
            for (int i = 0; i < m_renderers.Length; i++)
            {
                if (m_renderers[i] == null || !m_renderers[i].enabled)
                {
                    validMeshFilters.Add(null);
                    continue;
                }

                MeshFilter meshFilter = m_renderers[i].GetComponent<MeshFilter>();
                if (meshFilter == null || meshFilter.sharedMesh == null)
                {
                    validMeshFilters.Add(null);
                    continue;
                }

                validMeshFilters.Add(meshFilter);
            }

            int validMeshFilterCount = validMeshFilters.Count;

            float frameSize = m_OctahedronDiameter * 0.5f;

            for (int i = 0; i < atlasSize; i++)
            {
                for (int j = 0; j < atlasSize; j++)
                {
                    Bounds singleFrame = new Bounds();
                    Matrix4x4 cameraFacingMatrix = GetCameraFacingMatrix(impostorType, atlasSize, i, j);

                    for (int k = 0; k < validMeshFilterCount; k++)
                    {
                        if (validMeshFilters[k] == null)
                            continue;
                        if (singleFrame.size == Vector3.zero)
                            singleFrame = TransformBounds(validMeshFilters[k].sharedMesh.bounds, source.gameObject.transform.worldToLocalMatrix * m_renderers[k].localToWorldMatrix);
                        else
                            singleFrame.Encapsulate(TransformBounds(validMeshFilters[k].sharedMesh.bounds, source.gameObject.transform.worldToLocalMatrix * m_renderers[k].localToWorldMatrix));
                    }

                    singleFrame = TransformBounds(singleFrame, cameraFacingMatrix);
                    // Matrices
                    Matrix4x4 ViewMatrix = cameraFacingMatrix.inverse * Matrix4x4.LookAt(singleFrame.center - new Vector3(0, 0, m_Depth * 0.5f), singleFrame.center, Vector3.up);
                    Matrix4x4 OrthoProjection = Matrix4x4.Ortho(-frameSize, frameSize, -frameSize, frameSize, 0, -m_Depth);
                    ViewMatrix = ViewMatrix.inverse * source.gameObject.transform.worldToLocalMatrix;
                    commandBuffer.SetViewProjectionMatrices(ViewMatrix, OrthoProjection);


                    float res = mapsResolution == MapDimensions.Custom ? resolution : (float)mapsResolution;

                    // Reduce viewport to fit a single frame
                    Rect frame = new Rect((res / atlasSize) * i, (res / atlasSize) * j, (res / atlasSize), (res / atlasSize));
                    commandBuffer.SetViewport(frame);

                    commandBuffer.SetGlobalMatrix("_ViewMatrix", ViewMatrix);
                    commandBuffer.SetGlobalMatrix("_InvViewMatrix", ViewMatrix.inverse);
                    commandBuffer.SetGlobalMatrix("_ProjMatrix", OrthoProjection);
                    commandBuffer.SetGlobalMatrix("_ViewProjMatrix", OrthoProjection * ViewMatrix);
                    commandBuffer.SetGlobalVector("_WorldSpaceCameraPos", Vector4.zero);

                    for (int k = 0; k < validMeshFilters.Count; k++)
                    {
                        if (validMeshFilters[k] == null)
                            continue;

                        UnityEngine.Material[] materials = m_renderers[k].sharedMaterials;

                        for (int m = 0; m < materials.Length; m++)
                        {
                            UnityEngine.Material renderMaterial = materials[m];
                            commandBuffer.DrawRenderer(m_renderers[k], renderMaterial, m, renderMaterial.FindPass("DEFERRED"));

                        }
                    }

                    Graphics.ExecuteCommandBuffer(commandBuffer);
                }

            }


            UpdateProgressBar(0.6f, "Please Wait... Packing Textures");

            Shader packerShader = Shader.Find("Pixyz/Packer");
            UnityEngine.Material packerMaterial = new UnityEngine.Material(packerShader);

            // albedo
            PackTexture(ref m_GBuffers[0], ref m_GBuffers[0], 1, packerMaterial, m_GBuffers[1]);


            // fix emission 
            if (!UnityEditor.SceneView.lastActiveSceneView.sceneLighting)
            {
                PackTexture(ref m_GBuffers[3], ref m_GBuffers[3], 3, packerMaterial);
            }

            // copy depth from z-buffer to m_OutDepthRT
            PackTexture(ref m_OutDepthRT, ref m_OutDepthRT, 4, packerMaterial, m_DepthRT);


            UpdateProgressBar(0.75f, "Please Wait...Setting Textures!");

            //Built-in RenderPipeline
            m_outputTextures[(int)Textures.AlbedoOcclusion] = RenderTextureToTexture2D(ref m_GBuffers[0], "Albedo & Occlusion", saveMaps);
            m_outputTextures[(int)Textures.SpecularRoughness] = RenderTextureToTexture2D(ref m_GBuffers[1], "Specular & Smoothness", saveMaps);
            m_outputTextures[(int)Textures.Normals] = RenderTextureToTexture2D(ref m_GBuffers[2], "Normals", saveMaps);
            m_outputTextures[(int)Textures.Emissive] = RenderTextureToTexture2D(ref m_GBuffers[3], "Emission", saveMaps); 
            m_outputTextures[(int)Textures.Depth] = RenderTextureToTexture2D(ref m_OutDepthRT, "Depth", saveMaps);

            UpdateProgressBar(1f, "Done!");
            ReleaseRenderTextures();
            RestoreTransform();

        }

        private void PackTexture(ref RenderTexture source, ref RenderTexture destination, int passIndex, UnityEngine.Material packerMaterial, Texture extraTexture = null)
        {
            if (extraTexture != null)
                packerMaterial.SetTexture("_ExtraTexture", extraTexture);

            if (source == destination)
            {
                RenderTexture tempTex = RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format);
                Graphics.Blit(source, tempTex, packerMaterial, passIndex);
                Graphics.Blit(tempTex, destination);
                RenderTexture.ReleaseTemporary(tempTex);
            }
            else
            {
                Graphics.Blit(source, destination, packerMaterial, passIndex);
            }
        }

        private void ReleaseRenderTextures()
        {
            RenderTexture.active = null;
            foreach (var renderTexture in m_GBuffers)
            {
                renderTexture.Release();
            }
            m_DepthRT.Release();
            m_OutDepthRT.Release();
            m_GBuffers = null;
        }
        private void SaveTransform()
        {
            m_originalPosition = source.gameObject.transform.position;
            m_originalRotation = source.gameObject.transform.rotation;
            m_originalScale = source.gameObject.transform.localScale;
            source.gameObject.transform.position = Vector3.zero;
            source.gameObject.transform.rotation = Quaternion.identity;
            source.gameObject.transform.localScale = Vector3.one;

        }
        private void RestoreTransform()
        {
            source.gameObject.transform.position = m_originalPosition;
            source.gameObject.transform.rotation = m_originalRotation;
            source.gameObject.transform.localScale = m_originalScale;
        }
        void CalculateOctahedronDiameter(OctahedronImpostorType impostorType)
        {
            m_OctahedronDiameter = 0;
            m_Depth = 0;

            for (int i = 0; i < atlasSize; i++)
            {
                for (int j = 0; j <= atlasSize; j++)
                {
                    Bounds singleFrame = new Bounds();
                    Matrix4x4 cameraFacingMatrix = GetCameraFacingMatrix(impostorType, atlasSize, i, j);
                    for (int k = 0; k < m_renderers.Length; k++)
                    {
                        MeshFilter meshFilter = m_renderers[k].GetComponent<MeshFilter>();
                        if (meshFilter == null || meshFilter.sharedMesh == null)
                            continue;

                        if (singleFrame.size == Vector3.zero)
                            singleFrame = TransformBounds(meshFilter.sharedMesh.bounds, source.gameObject.transform.worldToLocalMatrix * m_renderers[k].localToWorldMatrix);
                        else
                            singleFrame.Encapsulate(TransformBounds(meshFilter.sharedMesh.bounds, source.gameObject.transform.worldToLocalMatrix * m_renderers[k].localToWorldMatrix));
                    }
                    if (i == 0 && j == 0)
                        originalBounds = singleFrame;

                    singleFrame = TransformBounds(singleFrame, cameraFacingMatrix);
                    m_OctahedronDiameter = Mathf.Max(m_OctahedronDiameter, singleFrame.size.x, singleFrame.size.y);
                    m_Depth = Mathf.Max(m_Depth, singleFrame.size.z);

                }
            }
        }


        void GetAllRenderers()
        {
            if (source != null)
                m_renderers = source.gameObject.GetComponentsInChildren<Renderer>();
        }

        private Texture2D RenderTextureToTexture2D(ref RenderTexture currentRT, string fileName, bool save = false)
        {
            Texture2D texture = new Texture2D(currentRT.width, currentRT.height);
            texture.name = fileName;
            RenderTexture tmpRT = RenderTexture.active;
            RenderTexture.active = currentRT;
            texture.ReadPixels(new Rect(0, 0, currentRT.width, currentRT.height), 0, 0);
            RenderTexture.active = tmpRT;
            texture.Apply();
            if (save)
                SaveTextureAsPNG(texture, fileName);
            return texture;

        }
    }
}