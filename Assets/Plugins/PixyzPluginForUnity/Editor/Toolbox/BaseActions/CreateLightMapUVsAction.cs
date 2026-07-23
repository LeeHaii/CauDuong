using Pixyz.Commons.Extensions.Editor;
using Pixyz.OptimizeSDK.Native;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pixyz.Commons.UI.Editor;
using Pixyz.OptimizeSDK.Editor.MeshProcessing;

namespace Pixyz.Toolbox.Editor
{
    public enum QualityMapDimensions
    {
        High = 2048,
        Medium = 1024,
        Low = 512,
        Custom = 0
    }

    public class CreateLightmapUVs : PixyzFunction
    {
        public override int id => 1651980;
        public override int order => 16;
        public override string menuPathRuleEngine => "UVs/Create UVs for Lightmaps";
        public override string menuPathToolbox => "UVs/Create UVs for Lightmaps";
        public override string tooltip => ToolboxTooltips.createUVLightMapsAction;
        protected override MaterialSyncType SyncMaterials => MaterialSyncType.SyncNone;

        [UserParameter]
        public QualityMapDimensions quality;

        [UserParameter]
        public int resolution = 1024;

        [UserParameter]
        public int padding = 2;


        private QualityMapDimensions previousQuality;
        private int previousResolution;

        protected override void process()
        {
            uint scene = Scene.Native.NativeInterface.CreateSceneFromMeshes(Context.pixyzMeshes, Context.pixyzMatrices, true);
            Scene.Native.OccurrenceList occurrenceList = new Scene.Native.OccurrenceList(new uint[] { scene });

            Core.Native.NativeInterface.PushAnalytic("CreateLightMapUVs", "");
            UpdateProgressBar(0.25f, "Reparing mesh..");

            //WeldVertices
            Polygonal.Native.TopologyCategoryMask topologyMask = new Polygonal.Native.TopologyCategoryMask();
            topologyMask.connectivity = Polygonal.Native.TopologyConnectivityMask.BOUNDARY_NONMANIFOLD;
            topologyMask.dimension = Polygonal.Native.TopologyDimensionMask.FACE;
            Algo.Native.NativeInterface.MergeVertices(occurrenceList, 0.0000001, topologyMask);
            Scene.Native.NativeInterface.ResetPartTransform(occurrenceList[0]);
            UpdateProgressBar(0.45f, "Creating UVs 0 if missing");
            //CreateProjectedUvs
            Algo.Native.NativeInterface.MapUvOnAABB(occurrenceList, false, 1, 0, false);
            Scene.Native.NativeInterface.ResetPartTransform(occurrenceList[0]);

            uint[] uniqueMeshs = Context.pixyzMeshes.list.Distinct().ToArray();

            UpdateProgressBar(0.65f, "Creating lightMapUVs");

            //CreateProjected Uvs
            Algo.Native.NativeInterface.MapUvOnAABB(occurrenceList, false, 1, 0, false);
            Scene.Native.NativeInterface.ResetPartTransform(occurrenceList[0]);
            //RepackUvs
            Algo.Native.NativeInterface.RepackUV(occurrenceList, 1, false, resolution, padding, false, 3, false);
            Scene.Native.NativeInterface.ResetPartTransform(occurrenceList[0]);
            //NormalizeUVs
            Algo.Native.NativeInterface.NormalizeUV(occurrenceList, 1, -1, true, true, false);
            Scene.Native.NativeInterface.ResetPartTransform(occurrenceList[0]);

            UpdateProgressBar(1.0f);
        }

        public override void onBeforeDraw()
        {
            base.onBeforeDraw();
            BaseExtensionsEditor.MatchEnumWithCustomValue(ref previousQuality, ref quality, ref previousResolution, ref resolution);
        }
        protected override void postProcess()
        {
            base.postProcess();

            GameObject[] outputParts = Context.PixyzMeshToUnityObject(Context.pixyzMeshes);
            ReplaceInHierarchy(InputParts, outputParts);
        }

        public override IList<string> getErrors()
        {
            var errors = new List<string>();
            if (padding > 100)
            {
                errors.Add("Padding is too high ! (must be between 1 and 100)");
            }
            if (padding < 1)
            {
                errors.Add("Padding resolution is too low ! (must be between 1 and 100)");
            }
            if (resolution < 64)
            {
                errors.Add("Resolution is too low ! (must be between 64 and 8192)");
            }
            if (resolution > 8192)
            {
                errors.Add("Resolution is too high ! (must be between 64 and 8192)");
            }
            return errors.ToArray();
        }
    }
}
