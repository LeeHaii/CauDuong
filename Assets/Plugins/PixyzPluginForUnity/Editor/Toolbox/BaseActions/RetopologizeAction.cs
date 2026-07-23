using System.Collections.Generic;
using UnityEngine;
using Pixyz.Commons;
using Pixyz.Commons.Extensions;
using Pixyz.OptimizeSDK;
using Pixyz.OptimizeSDK.Native;
using Pixyz.Commons.UI.Editor;
using UnityEngine.Rendering;

namespace Pixyz.Toolbox.Editor
{
    public enum MapDimensions
    {
        _8192 = 8192,
        _4096 = 4096,
        _2048 = 2048,
        _1024 = 1024,
        _512 = 512,
        _256 = 256,
        Custom = 0
    }

    public enum GridResolutionPreset
    {
        Custom = 0,
        Poor = 10,
        Low = 25,
        Medium = 50,
        High = 100,
        VeryHigh = 200
    }

    public enum VertexTarget
    {
        TriangleCount,
        Ratio
    }
    public enum RetopologizeType
    {
        Standard,
        FieldAligned
    }

    public class RetopologizeAction : PixyzFunction
    {
        public override int id => 57249905;
        public override int order => 6;
        public override string menuPathRuleEngine => "Remeshing/Retopologize";
        public override string menuPathToolbox => "Remeshing/Retopologize";
        public override string tooltip => ToolboxTooltips.retopologizeAction;
        private bool isFieldAligned() { return type == RetopologizeType.FieldAligned; }
        private bool isStandard() { return type == RetopologizeType.Standard; }
        private bool isCustom() { return mapsResolution == MapDimensions.Custom && isBakingMaps(); }
        private bool isPolycount() => criterion == VertexTarget.TriangleCount && isFieldAligned();
        private bool isPolycountRatio() => criterion == VertexTarget.Ratio && isFieldAligned();
        private bool isBakingMaps() => bakeMaps;
        private bool isFeatureSizeActive() => isFieldAligned() && useFeatureSize;

        [UserParameter(tooltip: ToolboxTooltips.retopologizeType)]
        public RetopologizeType type = RetopologizeType.Standard;

        [UserParameter("isFieldAligned", displayName:"Strategy", tooltip: ToolboxTooltips.retopologizeStrategy)]
        public VertexTarget criterion = VertexTarget.Ratio;

        [UserParameter("isPolycount", displayName:"Triangles", tooltip: ToolboxTooltips.retopologizeTriangles)]
        public int targetTriangleCount = 10000;

        [UserParameter("isPolycountRatio", displayName:"Ratio", tooltip: ToolboxTooltips.retopologizeRatio)]
        public Range targetRatio = (Range)10f;

        [UserParameter("isStandard", displayName:"Quality preset", tooltip: ToolboxTooltips.retopologizeQuality)]
        public GridResolutionPreset gridResolutionPreset = GridResolutionPreset.Medium;

        [UserParameter("isStandard", displayName: "Quality value", tooltip: ToolboxTooltips.retopologizeQualityValue)]
        public int gridResolution = (int)GridResolutionPreset.Medium;

        [UserParameter("isFieldAligned", tooltip: ToolboxTooltips.retopologizeUseFeature)]
        public bool useFeatureSize = false;

        [UserParameter("isFeatureSizeActive", tooltip: ToolboxTooltips.retopologizeFeatureSize)]
        public float featureSize = 0.1f;

        [UserParameter(tooltip: ToolboxTooltips.retopologizeBake)]
        public bool bakeMaps = true;

        [UserParameter("isBakingMaps", tooltip: ToolboxTooltips.retopologizeMapResolution)]
        public MapDimensions mapsResolution = MapDimensions._1024;

        [UserParameter("isCustom", tooltip: ToolboxTooltips.retopologizeMapResolution)]
        public int resolution = 1024;

        [UserParameter("isStandard", tooltip: ToolboxTooltips.retopologizePtCloud)]
        public bool isPointCloud = false;

        private GridResolutionPreset _prevGridResolutionPreset = GridResolutionPreset.Medium;
        private int _prevGridResolution = (int)GridResolutionPreset.Medium;
        private bool skinnedMesh = false;

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

        public override IList<string> getWarnings()
        {
            var warnings = new List<string>();
            if (isStandard())
            {
                if (gridResolution >= 1000)
                    warnings.Add("Quality value is too high! (The execution can take a lot of time)");
            }
            if (bakeMaps && UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null)
            {
                warnings.Add("Baking maps is only compatible with built-in render pipeline");
            }
            return warnings.ToArray();
        }

        public override IList<string> getErrors()
        {
            var errors = new List<string>();
            if (isStandard())
            {
                if (gridResolution <= 0)
                    errors.Add("Quality value is too low! (must be higher than 0)");
            }
            if(isFieldAligned())
            {
                if (featureSize <= 0 && useFeatureSize)
                    errors.Add("Feature size is too low ! (must be higher than 0)");
            }
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
            if (skinnedMesh)
            {
                errors.Add("Selection contains Skinned Mesh Renderer.\nRetopologize is not possible with SkinnedMesh.");
            }
            return errors.ToArray();
        }
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

        protected override void process()
        {
            try
            {
                Core.Native.NativeInterface.PushAnalytic("Retopologize", type.ToString());
                UpdateProgressBar(0.25f);

                uint root = Scene.Native.NativeInterface.CreateSceneFromMeshes(Context.pixyzMeshes, Context.pixyzMatrices, true);
                Scene.Native.OccurrenceList occurrenceList = new Scene.Native.OccurrenceList(new uint[] { root });
                
                //WeldVertices
                Polygonal.Native.TopologyCategoryMask topologyMask = new Polygonal.Native.TopologyCategoryMask();
                topologyMask.connectivity = Polygonal.Native.TopologyConnectivityMask.BOUNDARY_NONMANIFOLD;
                topologyMask.dimension = Polygonal.Native.TopologyDimensionMask.FACE;
                Algo.Native.NativeInterface.MergeVertices(occurrenceList, 0.0000001, topologyMask);
                
                UpdateProgressBar(0.35f, "Remeshing...");

                var newOccurrenceList = new Scene.Native.OccurrenceList(1);
                if (type == RetopologizeType.FieldAligned)
                {
                    if (isPolycount())
                    {
                        //RemeshFieldAligned
                        newOccurrenceList[0] = Algo.Native.NativeInterface.Retopologize(occurrenceList, targetTriangleCount, true, false, useFeatureSize ? featureSize : -1);
                        Algo.Native.NativeInterface.BakeVertexAttributes(newOccurrenceList, occurrenceList, true, false, false);
                    }
                    else if (isPolycountRatio())
                    {
                        //RemeshFieldAlignedToRatio
                        int polyCount = Scene.Native.NativeInterface.GetPolygonCount(occurrenceList, true, false, true);
                        int target = (int)((targetRatio / 100f) * polyCount);
                        newOccurrenceList[0] = Algo.Native.NativeInterface.Retopologize(occurrenceList, target, true, false, useFeatureSize ? featureSize : -1);
                        Algo.Native.NativeInterface.BakeVertexAttributes(newOccurrenceList, occurrenceList, true, false, false);
                    }

                    if (bakeMaps)
                    {
                        UpdateProgressBar(0.65f, "Baking...");

                        Algo.Native.BakeMapList mapsToBake = new Algo.Native.BakeMapList(new Algo.Native.BakeMap[] {
                            new Algo.Native.BakeMap() { type = Algo.Native.MapType.Diffuse },
                            new Algo.Native.BakeMap() { type = Algo.Native.MapType.Normal },
                            new Algo.Native.BakeMap() { type = Algo.Native.MapType.Metallic },
                            new Algo.Native.BakeMap() { type = Algo.Native.MapType.Roughness },
                            new Algo.Native.BakeMap() { type = Algo.Native.MapType.Opacity },
                            new Algo.Native.BakeMap() { type = Algo.Native.MapType.AO },
                            new Algo.Native.BakeMap() { type = Algo.Native.MapType.Emissive },
                        });

                        OptimizeSDKUtils.BakeMaterials(occurrenceList, newOccurrenceList[0], mapsResolution == MapDimensions.Custom ? resolution : (int)mapsResolution, 1, true, isPointCloud ? Algo.Native.BakingMethod.ProjOnly : Pixyz.Algo.Native.BakingMethod.RayOnly, mapsToBake, Context.pixyzMatrices, Conversions.Identity());
                    }
                }
                else if (type == RetopologizeType.Standard)
                {
                    if (isPointCloud)
                    {
                        Process.Native.GenerateDiffuseMap generateMap = new Process.Native.GenerateDiffuseMap() {
                            _type = bakeMaps ? Process.Native.GenerateDiffuseMap.Type.YES : Process.Native.GenerateDiffuseMap.Type.NO,
                            yes = new Process.Native.BakeDiffuseOptions()
                            {
                                mapResolution = resolution,
                                padding = 1
                            }
                        };
                        newOccurrenceList[0] = Process.Native.NativeInterface.ProxyFromPointCloud(occurrenceList, gridResolution, generateMap, false);
                    }
                    else
                    {
                        var bakeOptions = new Process.Native.BakeOptionSelector();
                        bakeOptions._type = bakeMaps ? Process.Native.BakeOptionSelector.Type.YES : Process.Native.BakeOptionSelector.Type.NO;
                        bakeOptions.Yes = new Process.Native.BakeOptions()
                        {
                            padding = 1,
                            resolution = mapsResolution == MapDimensions.Custom ? resolution : (int)mapsResolution,
                            textures = new Algo.Native.BakeMaps() { 
                                diffuse = true, 
                                ambientOcclusion = true, 
                                metallic = true, 
                                emissive = true, 
                                normal = true, 
                                opacity = true, 
                                roughness = true 
                            }
                        };
                        newOccurrenceList[0] = Process.Native.NativeInterface.ProxyFromMeshes(occurrenceList, gridResolution, bakeOptions, true, false);
                    }
                }

                Scene.Native.NativeInterface.ResetTransform(Scene.Native.NativeInterface.GetRoot(), true, true, false);
                Scene.Native.NativeInterface.TransferMaterialsOnPatches(Scene.Native.NativeInterface.GetRoot());
                Context.pixyzMeshes = Scene.Native.NativeInterface.GetPartsMeshes(new Scene.Native.PartList(new uint[] { Scene.Native.NativeInterface.GetComponent(newOccurrenceList[0], Scene.Native.ComponentType.Part, true) }));
                Context.pixyzMatrices = new Geom.Native.Matrix4List(new Geom.Native.Matrix4[] { Conversions.Identity() });

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
            DisableInput();

            int polyCount = _output.GetMeshes().GetPolyCount();
            foreach (var go in (IList<GameObject>)Output) go.name = "Retopo-" + polyCount;
        }
    }
}
