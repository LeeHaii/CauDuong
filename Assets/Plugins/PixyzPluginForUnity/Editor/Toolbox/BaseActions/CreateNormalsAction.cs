using System.Collections.Generic;
using UnityEngine;
using Pixyz;
using Pixyz.OptimizeSDK.Native;
using Pixyz.Commons.UI.Editor;
using Pixyz.OptimizeSDK.Editor.MeshProcessing;

namespace Pixyz.Toolbox.Editor
{
    public class CreateNormals : PixyzFunction
    {
        public override int id => 219151512;
        public override int order => 14;
        public override string menuPathRuleEngine => "Normals/Create Normals";
        public override string menuPathToolbox => "Normals/Create Normals";
        public override string tooltip => ToolboxTooltips.createNormalsAction;
        protected override MaterialSyncType SyncMaterials => MaterialSyncType.SyncNone;

        [UserParameter]
        public double smoothingAngle = 25;

        [UserParameter]
        public bool areaWeighting = true;


        protected override void process()
        {
            try
            {
                UpdateProgressBar(0.25f, "Creating normals");
                Core.Native.NativeInterface.PushAnalytic("CreateNormals", "");

                uint scene = Scene.Native.NativeInterface.CreateSceneFromMeshes(Context.pixyzMeshes, Context.pixyzMatrices, true);
                Scene.Native.OccurrenceList occurrenceList = new Scene.Native.OccurrenceList(new uint[] { scene });

                //WeldVertices
                Polygonal.Native.TopologyCategoryMask topologyMask = new Polygonal.Native.TopologyCategoryMask();
                topologyMask.connectivity = Polygonal.Native.TopologyConnectivityMask.BOUNDARY_NONMANIFOLD;
                topologyMask.dimension = Polygonal.Native.TopologyDimensionMask.FACE;
                Algo.Native.NativeInterface.MergeVertices(occurrenceList, 0.0000001, topologyMask);
                Scene.Native.NativeInterface.ResetPartTransform(occurrenceList[0]);
                //CreateNormals
                Algo.Native.NativeInterface.CreateNormals(occurrenceList, smoothingAngle, true, areaWeighting);
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
            ReplaceInHierarchy(InputParts, outputParts);
        }

        public override IList<string> getErrors()
        {
            var errors = new List<string>();
            if (smoothingAngle < 0)
            {
                errors.Add("Smoothing angle is too low ! (must be between 0 and 180)");
            }
            if (smoothingAngle > 180)
            {
                errors.Add("Smoothing angle is too high ! (must be between 0 and 180)");
            }
            return errors;
        }
    }
}
