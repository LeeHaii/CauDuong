using System.Collections.Generic;
using UnityEngine;
using Pixyz.OptimizeSDK.Native;
using Pixyz.Commons.UI.Editor;

namespace Pixyz.Toolbox.Editor
{
    public class GenerateBillboardAction : PixyzFunction
    {
        public override int id => 57241105;
        public override int order => 7;
        public override string menuPathRuleEngine => "Remeshing/Create Billboard";
        public override string menuPathToolbox => "Remeshing/Create Billboard";
        public override string tooltip => ToolboxTooltips.billboardAction;

        [UserParameter(tooltip: ToolboxTooltips.billboardResolution)]
        public MapDimensions mapsResolution = MapDimensions._1024;
        private bool isCustom() { return mapsResolution == MapDimensions.Custom; }

        [UserParameter("isCustom", tooltip:"Output maps resolution")]
        public int resolution = 1024;

        [UserParameter(displayName:"X+")]
        public bool xPositive = true;

        [UserParameter(displayName: "X-")]
        public bool xNegative = true;

        [UserParameter(displayName: "Y+")]
        public bool yPositive = true;

        [UserParameter(displayName: "Y-")]
        public bool yNegative = true;

        [UserParameter(displayName: "Z+")]
        public bool zPositive = true;

        [UserParameter(displayName: "Z-")]
        public bool zNegative = true;

        protected override void process()
        {
            try
            {
                Core.Native.NativeInterface.PushAnalytic("GenerateBillboard", "");
                UpdateProgressBar(0.25f);
                uint scene = Scene.Native.NativeInterface.CreateSceneFromMeshes(Context.pixyzMeshes, Context.pixyzMatrices, true);
                Scene.Native.OccurrenceList occurrenceList = new Scene.Native.OccurrenceList(new uint[] { scene });
                //WeldVertices
                Polygonal.Native.TopologyCategoryMask topologyMask = new Polygonal.Native.TopologyCategoryMask();
                topologyMask.connectivity = Polygonal.Native.TopologyConnectivityMask.BOUNDARY_NONMANIFOLD;
                topologyMask.dimension = Polygonal.Native.TopologyDimensionMask.FACE;
                Algo.Native.NativeInterface.MergeVertices(occurrenceList, 0.0000001, topologyMask);
                Scene.Native.NativeInterface.ResetPartTransform(occurrenceList[0]);
                //CreateBillboard
                var outputOccurrence = Algo.Native.NativeInterface.CreateBillboard(occurrenceList, mapsResolution == MapDimensions.Custom ? resolution : (int)mapsResolution, xPositive, xNegative, yPositive, yNegative, zPositive, zNegative, true, false);
                var outputMesh = Scene.Native.NativeInterface.GetPartMesh(Scene.Native.NativeInterface.GetComponent(outputOccurrence, Scene.Native.ComponentType.Part, true));
                Scene.Native.NativeInterface.ResetPartTransform(occurrenceList[0]);
                Context.pixyzMeshes = new Polygonal.Native.MeshList(new uint[] { outputMesh });
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

            _output = Context.PixyzMeshToUnityObject(Context.pixyzMeshes);
            
            foreach (GameObject go in _output)
            {
                go.name = "Imposter";
                UnityEngine.Material material = go.GetComponent<MeshRenderer>().sharedMaterial;
                material.SetFloat("_Mode", 2);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
            }

            DisableInput();
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
    }
}