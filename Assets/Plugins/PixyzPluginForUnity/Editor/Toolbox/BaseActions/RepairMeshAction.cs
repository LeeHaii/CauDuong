using System.Collections.Generic;
using UnityEngine;
using Pixyz.OptimizeSDK.Native;
using Pixyz.Commons.UI.Editor;
using Pixyz.OptimizeSDK.Editor.MeshProcessing;

namespace Pixyz.Toolbox.Editor
{
    public class RepairMeshAction : PixyzFunction
    {
        public override int id => 1455705;
        public override int order => 2;
        public override string menuPathRuleEngine => "Mesh/Repair";
        public override string menuPathToolbox => "Mesh/Repair";
        public override string tooltip => ToolboxTooltips.repairAction;
        protected override MaterialSyncType SyncMaterials => MaterialSyncType.SyncNone;

        [UserParameter(tooltip: ToolboxTooltips.repairDistanceTolerance)]
        public double distanceTolerance = 0.0001;

        [UserParameter(tooltip: ToolboxTooltips.repairOrientFaces)]
        public bool orientFaces = false;

        protected override void process()
        {
            try
            {
                UpdateProgressBar(0.25f, "Repairing meshes..");
                uint scene = Scene.Native.NativeInterface.CreateSceneFromMeshes(Context.pixyzMeshes, Context.pixyzMatrices, true);
                Scene.Native.OccurrenceList occurrenceList = new Scene.Native.OccurrenceList(new uint[] { scene });
                //RepairMeshes
                Algo.Native.NativeInterface.RepairMesh(occurrenceList, distanceTolerance, true, true);
                Scene.Native.NativeInterface.ResetPartTransform(occurrenceList[0]);
                Core.Native.NativeInterface.PushAnalytic("RepairMeshes", $"{Context.pixyzMeshes}, {distanceTolerance}, true, {orientFaces}, {Context.pixyzMatrices}");
                UpdateProgressBar(1f);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Error] {e.Message} /n {e.StackTrace}");
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
            if (distanceTolerance <= 0)
            {
                errors.Add("Distance tolerance is too low ! (must be higher than 0)");
            }
            return errors.ToArray();
        }
    }
}