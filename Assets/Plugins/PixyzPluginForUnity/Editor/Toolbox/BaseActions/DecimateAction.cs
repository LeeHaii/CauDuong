using System.Collections.Generic;
using UnityEngine;
using Pixyz.Commons.UI.Editor;
using Pixyz.Toolbox.Editor;
using Pixyz.Commons.Extensions.Editor;
using Pixyz.OptimizeSDK.Editor.MeshProcessing;
using Pixyz.Commons;

namespace Pixyz.Toolbox.Editor
{
    public class DecimateAction : PixyzFunction
    {
        public override int id => 277054868;
        public override string menuPathRuleEngine => "Mesh/Decimate";
        public override string menuPathToolbox => "Mesh/Decimate";
        public override string tooltip => "Reduce the number of triangles";

        // No need to sync materials between Unity and Pixyz for decimation purposes
        protected override MaterialSyncType SyncMaterials => MaterialSyncType.SyncNone;

        public enum DecimationCriterion
        {
            Quality,
            Target
        }

        public enum DecimationToTargetStrategy
        {
            TriangleCount,
            Ratio
        }

        [UserParameter(tooltip:ToolboxTooltips.decimCriterion)]
        public DecimationCriterion criterion = DecimationCriterion.Quality;

        #region decimateToQualityParams
        public enum DecimationQuality
        {
            Custom = 0,
            High = 1,
            Medium = 2,
            Low = 3,
            Poor = 4
        }

        [UserParameter("isDecimQuality", displayName: "Mesh Quality Preset", tooltip: ToolboxTooltips.decimQualityPreset)]
        public DecimationQuality paramsQuality = DecimationQuality.Medium;

        [UserParameter("isDecimQuality", displayName: "Advanced", collapsable: true)]
        public AdvancedParametersQuality advancedParametersQuality;

        [System.Serializable]
        public struct AdvancedParametersQuality 
        {
            [UserParameter("isDecimQuality", tooltip: ToolboxTooltips.decimQualitySurfacic)]
            public double surfacicTolerance;

            [UserParameter("isDecimQuality", tooltip: ToolboxTooltips.decimQualityLineic)]
            public double lineicTolerance;

            [UserParameter("isDecimQuality", tooltip: ToolboxTooltips.decimQualityNormalTolerance)]
            public double normalTolerance;

            [UserParameter("isDecimQuality", displayName: "UV tolerance", tooltip: ToolboxTooltips.decimQualityUV)]
            public double uvTolerance;
            public AdvancedParametersQuality(double _surfacicTolerance = 0, double _lineicTolerance = 0, double _normalTolerance = 0, double _uvTolerance = 0)
            {
                surfacicTolerance = _surfacicTolerance;
                lineicTolerance = _lineicTolerance;
                normalTolerance = _normalTolerance;
                uvTolerance = _uvTolerance;
            }
        }

        public DecimationQuality previousParamsQuality;
        #endregion

        #region decimToTargetParams
        [UserParameter("isDecimTarget", displayName: "Strategy", tooltip: ToolboxTooltips.decimTargetStrategy)]
        public DecimationToTargetStrategy decimationToTargetStrategy = DecimationToTargetStrategy.TriangleCount;

        [UserParameter("isPolycount", displayName: "Triangle count", tooltip: ToolboxTooltips.decimTargetCount)]
        public int polycount = 5000;

        [UserParameter("isRatio", displayName: "Ratio", tooltip: ToolboxTooltips.decimTargetRatio)]
        public Range targetRatio = (Range)50f;

        [UserParameter("isDecimTarget",tooltip: ToolboxTooltips.decimTargetVertexColor)]
        public bool preservePaintedAreas = false;

        [UserParameter("isDecimTarget", displayName: "Advanced", collapsable: true)]
        public AdvancedParametersTarget advancedParametersTarget;

        public enum UVImportance
        {
            preserveSeamsAndReduceDeformation = 0,
            preserveSeams = 1,
            doNotCare = 2,
        }

        [System.Serializable]
        public struct AdvancedParametersTarget
        {
            [UserParameter("isDecimTarget", displayName: "UV Importance", tooltip: ToolboxTooltips.decimTargetUVImportance)]
            public UVImportance uvImportance;

            [UserParameter("isDecimTarget", tooltip: ToolboxTooltips.decimTargetProtectTopology)]
            public bool protectTopology;
        }

        #endregion

        private bool isPolycount() => criterion == DecimationCriterion.Target &&  decimationToTargetStrategy == DecimationToTargetStrategy.TriangleCount;

        bool isRatio() => criterion == DecimationCriterion.Target &&  decimationToTargetStrategy == DecimationToTargetStrategy.Ratio;

        bool isDecimQuality() => criterion == DecimationCriterion.Quality;
        bool isDecimTarget() => criterion == DecimationCriterion.Target;

        string decimateCriterionTooltip()
        {
            return criterion == DecimationCriterion.Quality ? ToolboxTooltips.decimQualityAction : ToolboxTooltips.decimTargetAction;
        }

        private struct DecimateToQuality
        {
            public double surfacicTolerance;
            public double lineicTolerance;
            public double normalTolerance;
            public double uvTolerance;

            public DecimateToQuality(double s, double l, double n, double t)
            {
                surfacicTolerance = s;
                lineicTolerance = l;
                normalTolerance = n;
                uvTolerance = t;
            }
        }
        private Dictionary<DecimationQuality, DecimateToQuality> qualityPresets = new Dictionary<DecimationQuality, DecimateToQuality>()
        {
            {DecimationQuality.High, new DecimateToQuality(0.0005, 0.0001, 1, -1) },
            {DecimationQuality.Medium, new DecimateToQuality(0.001, -1, 8, -1) },
            {DecimationQuality.Low, new DecimateToQuality(0.003, -1, 15, -1) },
            {DecimationQuality.Poor, new DecimateToQuality(0.01, -1, 20, -1) }
        };

        public override void onBeforeDraw()
        {
            if(isDecimQuality())
            {
                base.onBeforeDraw();
                BaseExtensionsEditor.MatchEnumWithCustomValue(ref paramsQuality, previousParamsQuality, ref advancedParametersQuality.surfacicTolerance, nameof(advancedParametersQuality.surfacicTolerance), qualityPresets);
                BaseExtensionsEditor.MatchEnumWithCustomValue(ref paramsQuality, previousParamsQuality, ref advancedParametersQuality.lineicTolerance, nameof(advancedParametersQuality.lineicTolerance), qualityPresets);
                BaseExtensionsEditor.MatchEnumWithCustomValue(ref paramsQuality, previousParamsQuality, ref advancedParametersQuality.normalTolerance, nameof(advancedParametersQuality.normalTolerance), qualityPresets);
                BaseExtensionsEditor.MatchEnumWithCustomValue(ref paramsQuality, previousParamsQuality, ref advancedParametersQuality.uvTolerance, nameof(advancedParametersQuality.uvTolerance), qualityPresets);
                previousParamsQuality = paramsQuality;
            }
        }

        protected override void process()
        {
            try
            {
                UpdateProgressBar(0.25f);

                uint scene = Scene.Native.NativeInterface.CreateSceneFromMeshes(Context.pixyzMeshes, Context.pixyzMatrices, true);
                Scene.Native.OccurrenceList occurrenceList = new Scene.Native.OccurrenceList(new uint[] { scene });

                if (criterion == DecimationCriterion.Quality)
                {
                    Core.Native.NativeInterface.PushAnalytic("DecimateToQuality", paramsQuality.ToString());
                    
                    //WeldVertices
                    Polygonal.Native.TopologyCategoryMask topologyMask = new Polygonal.Native.TopologyCategoryMask();
                    topologyMask.connectivity = Polygonal.Native.TopologyConnectivityMask.BOUNDARY_NONMANIFOLD;
                    topologyMask.dimension = Polygonal.Native.TopologyDimensionMask.FACE;
                    Algo.Native.NativeInterface.MergeVertices(occurrenceList, 0.0000001f, topologyMask);
                    
                    UpdateProgressBar(0.5f);

                    Algo.Native.NativeInterface.Decimate(
                        occurrenceList,
                        advancedParametersQuality.surfacicTolerance,
                        advancedParametersQuality.lineicTolerance,
                        advancedParametersQuality.normalTolerance,
                        advancedParametersQuality.uvTolerance,
                        false);
                } 
                else if (criterion == DecimationCriterion.Target)
                {
                    Core.Native.NativeInterface.PushAnalytic("DecimateToTarget", decimationToTargetStrategy.ToString());
                    
                    //WeldVertices
                    Polygonal.Native.TopologyCategoryMask topologyMask = new Polygonal.Native.TopologyCategoryMask();
                    topologyMask.connectivity = Polygonal.Native.TopologyConnectivityMask.BOUNDARY_NONMANIFOLD;
                    topologyMask.dimension = Polygonal.Native.TopologyDimensionMask.FACE;
                    Algo.Native.NativeInterface.MergeVertices(occurrenceList, 0.0000001f, topologyMask);
                    
                    UpdateProgressBar(0.5f);

                    // HACK: scale 1000 so error is correct on Pixyz side
                    Geom.Native.Point3 p3 = new Geom.Native.Point3() { x = 1000, y = 1000, z = 1000 };
                    Geom.Native.Matrix4 nMat = Geom.Native.NativeInterface.FromTRS(new Geom.Native.Point3(), new Geom.Native.Point3(), p3);
                    Scene.Native.NativeInterface.SetLocalMatrix(scene, nMat);
                    
                    // CreateWeightsFromColors
                    if (preservePaintedAreas)
                        Algo.Native.NativeInterface.CreateVertexWeightsFromVertexColors(occurrenceList, 1.0, 100);

                    Algo.Native.DecimateOptionsSelector options = new Algo.Native.DecimateOptionsSelector();
                    options._type = decimationToTargetStrategy == DecimationToTargetStrategy.TriangleCount ? Algo.Native.DecimateOptionsSelector.Type.TRIANGLECOUNT : Algo.Native.DecimateOptionsSelector.Type.RATIO;
                    options.triangleCount = polycount;
                    options.ratio = targetRatio.value;

                    Algo.Native.NativeInterface.DecimateTarget(occurrenceList, options, (Algo.Native.UVImportanceEnum)advancedParametersTarget.uvImportance, advancedParametersTarget.protectTopology, 5000000);

                    // HACK: restore default scale
                    p3 = new Geom.Native.Point3() { x = 1, y = 1, z = 1 };
                    nMat = Geom.Native.NativeInterface.FromTRS(new Geom.Native.Point3(), new Geom.Native.Point3(), p3);
                    Scene.Native.NativeInterface.SetLocalMatrix(scene, nMat);
                }

                Scene.Native.NativeInterface.ResetPartTransform(scene);

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
            if (isDecimQuality())
            {
                if (advancedParametersQuality.surfacicTolerance <= 0)
                {
                    errors.Add("Surfacic tolerance is too low ! (must be higher than 0)");
                }
            }
            else if (isPolycount())
            {
                if (polycount <= 0)
                {
                    errors.Add("Target polycount is too low ! (must be higher than 0)");
                }
            }
            return errors.ToArray();
        }
    }
}
