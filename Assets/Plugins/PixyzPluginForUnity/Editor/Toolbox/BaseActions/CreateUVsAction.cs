using System.Collections.Generic;
using UnityEngine;
using Pixyz.OptimizeSDK.Native;
using Pixyz.Commons.UI.Editor;
using Pixyz.OptimizeSDK.Editor.MeshProcessing;

namespace Pixyz.Toolbox.Editor
{

    public class CreateUVs : PixyzFunction
    {
        public override int id => 451666;
        public override int order => 15;
        public override string menuPathRuleEngine => "UVs/Create UVs";
        public override string menuPathToolbox => "UVs/Create UVs";
        public override string tooltip => ToolboxTooltips.createUVAction;
        protected override MaterialSyncType SyncMaterials => MaterialSyncType.SyncNone;

        [UserParameter]
        public bool useLocalBoundingBox = true;

        [UserParameter(tooltip:ToolboxTooltips.uvSize)]
        public float uvSize = 0.1f;

        [UserParameter]
        public int uvChannel = 0;


        protected override void process()
        {
            try
            {
                Core.Native.NativeInterface.PushAnalytic("CreateProjectedUVs", "");
                UpdateProgressBar(0.25f, "Creating UVs..");
                uint scene = Scene.Native.NativeInterface.CreateSceneFromMeshes(Context.pixyzMeshes, Context.pixyzMatrices, true);
                Scene.Native.OccurrenceList occurrenceList = new Scene.Native.OccurrenceList(new uint[] { scene });

                //WeldVertices
                Polygonal.Native.TopologyCategoryMask topologyMask = new Polygonal.Native.TopologyCategoryMask();
                topologyMask.connectivity = Polygonal.Native.TopologyConnectivityMask.BOUNDARY_NONMANIFOLD;
                topologyMask.dimension = Polygonal.Native.TopologyDimensionMask.FACE;
                Algo.Native.NativeInterface.MergeVertices(occurrenceList, 0.0000001, topologyMask);
                Scene.Native.NativeInterface.ResetPartTransform(occurrenceList[0]);
                //CreateProjectedUvs
                Algo.Native.NativeInterface.MapUvOnAABB(occurrenceList, useLocalBoundingBox, uvSize, uvChannel, true);
                Scene.Native.NativeInterface.ResetPartTransform(occurrenceList[0]);
                UpdateProgressBar(1f);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Error] {e.Message} \n {e.StackTrace}");
            }
        }

        protected override void postProcess()
        {
            base.postProcess();

            GameObject[] outputParts = Context.PixyzMeshToUnityObject(Context.pixyzMeshes);
            HashSet<Mesh> meshes = new HashSet<Mesh>();

            foreach(GameObject go in outputParts)
            {
                Renderer r = go.GetComponent<Renderer>();
                if(r != null)
                {
                    if(r is SkinnedMeshRenderer)
                    {
                        meshes.Add(((SkinnedMeshRenderer)r).sharedMesh);
                    }
                    else
                    {
                        meshes.Add(r.GetComponent<MeshFilter>().sharedMesh);
                    }
                }
            }

            foreach(Mesh mesh in meshes)
            {
                Vector2[] uvs = null;
                switch(uvChannel)
                {
                    case 0:
                        uvs = mesh.uv;
                        break;
                    case 1:
                        uvs = mesh.uv2;
                        break;
                    case 2:
                        uvs = mesh.uv3;
                        break;
                    case 3:
                        uvs = mesh.uv4;
                        break;
                    case 4:
                        uvs = mesh.uv5;
                        break;
                    case 5:
                        uvs = mesh.uv6;
                        break;
                    case 6:
                        uvs = mesh.uv7;
                        break;
                    case 7:
                        uvs = mesh.uv8;
                        break;
                }
                for(int i = 0; i < uvs.Length;++i)
                {
                    uvs[i].x = -uvs[i].x;
                }
                switch (uvChannel)
                {
                    case 0:
                        mesh.uv = uvs;
                        break;
                    case 1:
                        mesh.uv2 = uvs;
                        break;
                    case 2:
                        mesh.uv3 = uvs;
                        break;
                    case 3:
                        mesh.uv4 = uvs;
                        break;
                    case 4:
                        mesh.uv5 = uvs;
                        break;
                    case 5:
                        mesh.uv6 = uvs;
                        break;
                    case 6:
                        mesh.uv7 = uvs;
                        break;
                    case 7:
                        mesh.uv8 = uvs;
                        break;
                }
            }

            ReplaceInHierarchy(InputParts, outputParts);
        }
    }
}
