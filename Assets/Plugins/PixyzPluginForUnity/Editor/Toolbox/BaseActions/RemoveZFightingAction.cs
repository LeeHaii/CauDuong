using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixyz.OptimizeSDK.Editor.MeshProcessing;
using Pixyz.Commons.Extensions;

namespace Pixyz.Toolbox.Editor
{
    public class RemoveZFightingAction : PixyzFunction
    {
        public override int id => 1455705145;
        public override int order => 4;
        public override string menuPathRuleEngine => "Mesh/Remove Z-Fighting";
        public override string menuPathToolbox => "Mesh/Remove Z-Fighting";
        public override string tooltip => ToolboxTooltips.removeZFightingAction;
        protected override MaterialSyncType SyncMaterials => MaterialSyncType.SyncNone;

        private float sceneSize = 0f;
        private float inputSize = 0f;
        private float coeff = 0.00001f;
        private float maxRatio = 0.01f;

        // used for warning
        private bool hasRenderer = false;

        public override bool preProcess(IList<GameObject> input)
        {
            onSelectionChanged(input.GetChildren(true, true)); // needed for batch calls
            return base.preProcess(input);
        }

        protected override void process()
        {
            try
            {
                UpdateProgressBar(0.25f, "Removing Z-Fighting...");
                
                uint scene = Scene.Native.NativeInterface.CreateSceneFromMeshes(Context.pixyzMeshes, Context.pixyzMatrices, true);
                Scene.Native.OccurrenceList occurrenceList = new Scene.Native.OccurrenceList(new uint[] { scene });

                // Don't use NativeInterface.RemoveZFighting because it is specific to Pixyz Viewer
                float offset = sceneSize * coeff;
                float finalOffset = inputSize == 0f ? offset : Mathf.Min(offset, inputSize * maxRatio);

                Core.Native.NativeInterface.PushAnalytic("RemoveZFighting", $"{finalOffset}");
                Algo.Native.NativeInterface.VertexOffset(occurrenceList, -finalOffset);
                Scene.Native.NativeInterface.ResetPartTransform(occurrenceList[0]);
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

        public override void onSelectionChanged(IList<GameObject> selection)
        {
            base.onSelectionChanged(selection);
            try
            {
                // In try catch block to avoid having errors if scene is empty
                Bounds bounds = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().GetRootGameObjects().GetChildren(true, true).GetBoundsWorldSpace();
                sceneSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

                bounds = selection.GetChildren(true, true).GetBoundsWorldSpace();
                inputSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            }
            catch { }
            hasRenderer = true;
            foreach (var go in selection)
            {
                if (go.GetComponent<Renderer>() == null) hasRenderer = false;
            }
        }

        public override IList<string> getWarnings()
        {
            var warnings = new List<string>();
            if (sceneSize * coeff > inputSize * maxRatio)
            {
                warnings.Add("Scene dimensions seem way larger than input GameObjects' dimensions. Result cannot be guaranteed.");
            }
            if (!hasRenderer)
            {
                warnings.Add("We detected that you selected a GameObject having no Renderer. This function should be performed on GameObjects causing Z-Fighting (most likely having Renderers), not on a subtree.");
            }
            return warnings;
        }
    }
}