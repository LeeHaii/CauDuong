using System.Collections.Generic;
using UnityEngine;
using Pixyz.OptimizeSDK;
using Pixyz.OptimizeSDK.Native;
using Pixyz.Commons.UI.Editor;
using Pixyz.Commons.Extensions;

namespace Pixyz.Toolbox.Editor
{
    public class CombineAction : PixyzFunction
    {
        public override int id => 57243405;
        public override int order => 9;
        public override string menuPathRuleEngine => "Hierarchy/Combine";
        public override string menuPathToolbox => "Hierarchy/Combine";
        public override string tooltip => ToolboxTooltips.combineAction;

        [UserParameter(tooltip: ToolboxTooltips.combineMapResolution)]
        public MapDimensions mapsResolution = MapDimensions._1024;

        [UserParameter(displayName: "Recreate UV", tooltip: ToolboxTooltips.combineUVGen)]
        public bool forceUVGeneration = false;
        private bool isCustom() { return mapsResolution == MapDimensions.Custom; }

        [UserParameter("isCustom", tooltip:"Output maps resolution")]
        public int resolution = 1024;

        private string _outputName;

        private bool skinnedMesh = false;

        public override bool preProcess(IList<GameObject> input)
        {
            _outputName = input.GetHighestAncestor().name;
            return base.preProcess(input);
        }

        protected override void process()
        {
            try
            {
                uint scene = Scene.Native.NativeInterface.CreateSceneFromMeshes(Context.pixyzMeshes, Context.pixyzMatrices, true);
                Scene.Native.OccurrenceList occurrenceList = new Scene.Native.OccurrenceList(new uint[] { scene });

                Core.Native.NativeInterface.PushAnalytic("Combine", "");
                UpdateProgressBar(0.25f);
                
                //WeldVertices
                Polygonal.Native.TopologyCategoryMask topologyMask = new Polygonal.Native.TopologyCategoryMask()
                {
                    dimension = Polygonal.Native.TopologyDimensionMask.FACE,
                    connectivity = Polygonal.Native.TopologyConnectivityMask.BOUNDARY_NONMANIFOLD
                };
                Algo.Native.NativeInterface.MergeVertices(occurrenceList, 0.0000001, topologyMask);
                UpdateProgressBar(0.43f);

                //CombineMeshes
                resolution = mapsResolution == MapDimensions.Custom ? resolution : (int)mapsResolution;
                var bakeOptions = new Algo.Native.BakeOption
                {
                    bakingMethod = Algo.Native.BakingMethod.RayOnly,
                    resolution = resolution,
                    padding = 0,
                    textures = new Algo.Native.BakeMaps()
                    {
                        ambientOcclusion = true,
                        diffuse = true,
                        metallic = true,
                        normal = true,
                        opacity = true,
                        roughness = true,
                        emissive = true
                    }
                };
                uint occurrence = Algo.Native.NativeInterface.CombineMeshes(occurrenceList, bakeOptions, forceUVGeneration);
                uint outputMesh = Scene.Native.NativeInterface.GetPartMesh(Scene.Native.NativeInterface.GetComponent(occurrence, Scene.Native.ComponentType.Part, true));
                Scene.Native.NativeInterface.TransferMaterialsOnPatches(occurrence);
                Scene.Native.NativeInterface.ResetTransform(scene, true, true, false);
                Context.pixyzMeshes = new Polygonal.Native.MeshList(new uint[] { outputMesh });
                Context.pixyzMatrices = new Geom.Native.Matrix4List(new Geom.Native.Matrix4[] { Conversions.Identity() });

                UpdateProgressBar(1f);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Error] {e.Message} /n {e.StackTrace}");
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

        protected override void postProcess()
        {
            base.postProcess();

            _output = Context.PixyzMeshToUnityObject(Context.pixyzMeshes);
            
            foreach (GameObject go in (IList<GameObject>)Output)
            {
                go.name = _outputName;
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
                else if (resolution > 8192)
                {
                    errors.Add("Maps resolution is too high ! (must be between 64 and 8192)");
                }
            }
            if (skinnedMesh)
            {
                errors.Add("Selection contains Skinned Mesh Renderer.\nCombine is not possible with SkinnedMesh.");
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