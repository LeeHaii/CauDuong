using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Pixyz.OptimizeSDK.Utils;
using Pixyz.Commons.Extensions.Editor;
using Pixyz.Commons.Extensions;
using Pixyz.Commons.UI.Editor;
using UnityEditor;
using Pixyz.OptimizeSDK;

namespace Pixyz.Toolbox.Editor
{
    public class Merge : PixyzFunction
    {
        public override int id => 511763496;
        public override int order => 8;
        public override string menuPathRuleEngine => "Hierarchy/Merge";
        public override string menuPathToolbox => "Hierarchy/Merge";
        public override string tooltip => ToolboxTooltips.mergeAction;

        public override bool updateStats => mergeByRegions();

        #region parameters
        public enum MergeMode
        {
            MergeAll,
            MergeByMaterials,
            MergeFinalLevel,
            MergeHierarchyLevel,
            MergeByNames,
            MergeByRegions
        }

        bool mergingByHierarchy() => type == MergeMode.MergeHierarchyLevel;
        bool mergingParent() => type == MergeMode.MergeAll;


        [UserParameter(tooltip: ToolboxTooltips.mergeType)]
        public MergeMode type;

        [UserParameter("mergingByHierarchy")]
        public int hierarchyLevel = 1;

        [UserParameter("mergingParent", tooltip: ToolboxTooltips.mergeKeepParent)]
        public bool keepParent = false;


        #region parameters merge by regions
        bool mergeByRegions() => type == MergeMode.MergeByRegions;
        bool mergeByNumberOfRegions() => mergeByRegions() && mergeByRegionsStrategy == MergeByRegionsStrategy.NumberOfRegions;
        bool mergeBySizeOfRegions() => mergeByRegions() && mergeByRegionsStrategy == MergeByRegionsStrategy.SizeOfRegions;

        public enum MergeStrategy
        {
            MergeGameObjects,
            MergeByMaterials
        }

        public enum MergeByRegionsStrategy
        {
            NumberOfRegions,
            SizeOfRegions
        }

        [UserParameter("mergeByRegions", displayName:"Merge By", tooltip: ToolboxTooltips.mergeByRegionsMode)]
        public MergeByRegionsStrategy mergeByRegionsStrategy = MergeByRegionsStrategy.NumberOfRegions;

        [UserParameter("mergeByNumberOfRegions", tooltip: ToolboxTooltips.numberOfRegions)]
        public int numberOfRegions = 10;

        [UserParameter("mergeBySizeOfRegions", tooltip: ToolboxTooltips.sizeOfRegions)]
        public float sizeOfRegions = 10;

        [UserParameter("mergeByRegions", tooltip: ToolboxTooltips.mergeStrategy)]
        public MergeStrategy mergeStrategy = MergeStrategy.MergeByMaterials;

        #endregion
        #endregion

        #region action
        public override bool preProcess(IList<GameObject> input)
        {
            Core.Native.NativeInterface.PushAnalytic("Merge", type.ToString());
            switch (type)
            {
                case MergeMode.MergeByRegions:
                    return base.preProcess(input);
                default:
                    _input = input;
                    _output = _input;
                    break;
            }

            return true;
        }

        private string[] _outputNames; // We'll use this to reconstruct objects
        protected override void process()
        {
            if (type != MergeMode.MergeByRegions)
                return; // Done Unity side

            try
            {
                uint root = Scene.Native.NativeInterface.CreateSceneFromMeshes(Context.pixyzMeshes, Context.pixyzMatrices, true);
                Scene.Native.OccurrenceList occurrenceList = new Scene.Native.OccurrenceList(new uint[] { root });

                UpdateProgressBar(0.25f);

                Polygonal.Native.TopologyCategoryMask topologyCategoryMask = new Polygonal.Native.TopologyCategoryMask();
                topologyCategoryMask.connectivity = Polygonal.Native.TopologyConnectivityMask.BOUNDARY_NONMANIFOLD;
                topologyCategoryMask.dimension = Polygonal.Native.TopologyDimensionMask.FACE;
                Algo.Native.NativeInterface.MergeVertices(occurrenceList, 0.0000001, topologyCategoryMask);
                
                UpdateProgressBar(0.35f);

                var mergeByRegionsParameters = new Scene.Native.MergeByRegionsStrategy();
                mergeByRegionsParameters._type = mergeByNumberOfRegions() ? Scene.Native.MergeByRegionsStrategy.Type.NUMBEROFREGIONS : Scene.Native.MergeByRegionsStrategy.Type.SIZEOFREGIONS;
                mergeByRegionsParameters.NumberOfRegions = numberOfRegions;
                mergeByRegionsParameters.SizeOfRegions = sizeOfRegions;

                var output = Scene.Native.NativeInterface.MergeByRegions(occurrenceList, mergeByRegionsParameters, (Scene.Native.MergeStrategy)mergeStrategy);

                // Remove unnecessary patches
                Algo.Native.NativeInterface.DeletePatches(output, true);

                // A hierarchy is created by NativeInterface.MergeByRegions, bake transforms directly into the meshes to avoid transformations issues
                Scene.Native.NativeInterface.ResetTransform(root, true, true, false);

                // Used in post process
                _outputNames = Core.Native.NativeInterface.GetProperties(new Core.Native.EntityList(output), "Name", "");

                // Get newOccurrenceList mesh and update Context with this
                Polygonal.Native.MeshList newMeshes = Scene.Native.NativeInterface.GetPartsMeshes(new Scene.Native.PartList(Scene.Native.NativeInterface.GetComponentByOccurrence(output, Scene.Native.ComponentType.Part, false)));

                Context.pixyzMeshes = newMeshes;
                var pixyzMatrices = new Geom.Native.Matrix4List(newMeshes.length);
                for (int i = 0; i < Context.pixyzMeshes.length; i++)
                {
                    pixyzMatrices[i] = Conversions.Identity();
                }

                UpdateProgressBar(0.9f, "Post processing...");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Error] {e.Message} \n {e.StackTrace}");
            }
        }

        protected override void postProcess()
        {
            base.postProcess();

            switch (type)
            {
                case MergeMode.MergeAll:
                    //Undo.RegisterFullObjectHierarchyUndo(_input.GetHighestAncestor(), "Merge");
                    _output = MergeAll(_input).ToList();
                    break;
                case MergeMode.MergeFinalLevel:
                    _output = MergeFinalLevel(_input).ToList();
                    break;
                case MergeMode.MergeByNames:
                    _output = MergeByNames(_input).ToList();
                    break;
                case MergeMode.MergeHierarchyLevel:
                    _output = MergeHierarchyLevel(_input).ToList();
                    break;
                case MergeMode.MergeByMaterials:
                    _output = MergeByMaterials(_input).ToList();
                    break;
                case MergeMode.MergeByRegions:
                    _output = PostProcessMergeByRegions();
                    break;
                default:
                    break;
            }
            UpdateProgressBar(1f, "Post processing...");
        }

        private IList<GameObject> MergeAll(IList<GameObject> input)
        {
            if (keepParent)
            {
                // Merge selected assemblies one by one (highest common ancestors)
                var highestSelectedAncestors = input.GetHighestAncestors();
                foreach (GameObject gameObject in highestSelectedAncestors)
                {
                    gameObject.MergeChildren();
                }
                return highestSelectedAncestors.ToArray();
            }
            else
            {
                // Merge all together
                return new GameObject[] { input.Merge() };
            }
        }

        private IList<GameObject> MergeFinalLevel(IList<GameObject> input)
        {
            List<GameObject> finalAssemblies = new List<GameObject>();

            // Could be improved to be more efficient (parse tree in depth)
            foreach (GameObject g in input)
            {
                if (isFinalAssembly(g) && !finalAssemblies.Contains(g))
                {
                    finalAssemblies.Add(g);
                }
            }

            foreach (GameObject finalAssembly in finalAssemblies)
            {
                finalAssembly.MergeChildren();
            }

            // Get output gameobjects to display correctly process info
            var output = from g in input where g != null select g;
            return output.ToList();
        }

        /// <summary>
        /// Return true if a GameObject is a "Final Part":
        /// * Does not contain any children
        /// * Has a mesh
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        private bool isFinalPart(GameObject g)
        {
            if (g.GetChildren(false, false).Count > 0) return false;
            if (g.GetComponent<MeshRenderer>() == null) return false;
            return true;
        }

        /// <summary>
        /// Return true if a GameObject is a "Final Assembly":
        /// * Has children
        /// * Contains only "Final Part" as children
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        bool isFinalAssembly(GameObject g)
        {
            var children = g.GetChildren(false, false);
            if (children.Count == 0) return false;
            foreach (var child in children)
            {
                if (!isFinalPart(child)) return false;
            }
            return true;
        }

        /// <summary>
        /// Merge n-level bellow input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private IList<GameObject> MergeHierarchyLevel(IList<GameObject> input)
        {
            // Level is relative to input (not to scene root)
            // Level should be above 1
            if (hierarchyLevel < 1) hierarchyLevel = 1;

            var highestCommonAncestors = SceneExtensions.GetHighestAncestors(input);
            foreach (GameObject assembly in highestCommonAncestors)
            {
                MergeHierarchyLevelRecursively(assembly, 1);
            }

            // Get output gameobjects to display correctly process info
            var output = from g in input where g != null select g;
            return output.ToList();
        }

        private void MergeHierarchyLevelRecursively(GameObject g, int currentLevel)
        {
            if (currentLevel == hierarchyLevel)
            {
                g.MergeChildren();
                return;
            }
            currentLevel++;
            foreach (GameObject child in g.GetChildren(false, false))
                MergeHierarchyLevelRecursively(child, currentLevel);
        }

        private IList<GameObject> MergeByNames(IList<GameObject> input)
        {
            Dictionary<string, List<GameObject>> namesDict = new Dictionary<string, List<GameObject>>();

            foreach (GameObject g in input)
            {
                if (!namesDict.ContainsKey(g.name))
                {
                    namesDict[g.name] = new List<GameObject>() { g };
                }
                else
                {
                    namesDict[g.name].Add(g);
                }
            }

            // Merge groups (and their children)
            foreach (var group in namesDict)
            {
                if (group.Value.Count > 1)
                {
                    var extendedGroup = new List<GameObject>();
                    foreach (var g in group.Value)
                    {
                        if (g != null)
                            extendedGroup.AddRange(g.GetChildren(true, true));
                    }
                    extendedGroup.Merge();
                }
            }

            // Get output gameobjects to display correctly process info
            var output = from g in input where g != null select g;
            return output.ToList();
        }

        private IList<GameObject> MergeByMaterials(IList<GameObject> input)
        {
            var mergeAllAction = new Merge();
            mergeAllAction.type = MergeMode.MergeAll;
            mergeAllAction.Input = input;
            input = mergeAllAction.run(input);

            var explodeAction = new ExplodeSubmeshesAction();
            input = explodeAction.run(input);

            // Get output gameobjects to display correctly process info
            var output = from g in input where g != null select g;
            return output.ToList();
        }

        private List<GameObject> PostProcessMergeByRegions()
        {
            // We don't support merging for skinned mesh with this function so let's get rid of all input
            DeleteAllInput();

            var output = new List<GameObject>();

            GameObject newRoot = new GameObject("Regions");
            Selection.activeGameObject = newRoot;

            output.Add(newRoot);
            Undo.RegisterCreatedObjectUndo(newRoot, "MergeByRegions");

            // We now need to recreate a new hierarchy
            GameObject[] outputParts = Context.PixyzMeshToUnityObject(Context.pixyzMeshes);
            Dictionary<int, Transform> regionRoots = new Dictionary<int, Transform>();
            for (int i = 0; i < outputParts.Length; i++)
            {
                if (_outputNames.Length > i && _outputNames[i].StartsWith("Region_"))
                {
                    if (mergeStrategy == MergeStrategy.MergeByMaterials)
                    {
                        // Get region index
                        int.TryParse(Regex.Match(_outputNames[i], @"\d+").Value, out int index);
                        regionRoots.TryGetValue(index, out Transform root);
                        if (root == null)
                        {
                            root = new GameObject("Region_" + index.ToString()).transform;
                            output.Add(root.gameObject);
                            root.parent = newRoot.transform;
                            regionRoots[index] = root;
                        }

                        outputParts[i].transform.parent = root;
                        outputParts[i].name = _outputNames[i];
                    }
                    else
                    {
                        outputParts[i].transform.parent = newRoot.transform;
                        outputParts[i].name = _outputNames[i];
                    }
                    output.Add(outputParts[i]);
                }
                else
                {
                    // Should not happen
                    SceneExtensionsEditor.DestroyImmediateSafe(outputParts[i]);
                    Debug.LogWarning("A GameObject has been deleted.");
                }
            }

            // Move pivot point to centers of newly created GameObjects
            MovePivotAction movePivotAction = new MovePivotAction();
            movePivotAction.target = MovePivotAction.MovePivotOption.ToCenterOfBoundingBox;
            movePivotAction.runOncePerObject = true;
            movePivotAction.run(newRoot.GetChildren(true, true));

            Undo.RegisterFullObjectHierarchyUndo(newRoot, "MergeByRegions");

            return output;
        }     
        #endregion

        #region warnings_and_errors
        private bool skinnedMesh = false;
        private bool sameLevel = true;
        private int maxDepth = 0;

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

            // Check if all selected objects are at the same level in hierarchy
            int lastLevel = -1;
            foreach (GameObject obj in UnityEditor.Selection.gameObjects)
            {
                // Get object depth
                int level = 0;
                Transform transform = obj.transform;
                while (transform != null)
                {
                    level += 1;
                    transform = transform.parent;
                }

                // Check if level is same as last object (if not first object)
                if (lastLevel != -1 && lastLevel != level)
                {
                    sameLevel = false;
                    break;
                }
                lastLevel = level;
                sameLevel = true;
            }

            // Get max depth to potentially display a warning
            maxDepth = GetMaxDepth(selection);
        }

        private int GetMaxDepth(IList<GameObject> roots)
        {
            int maxDepth = 0;
            foreach (var root in roots)
            {
                int rootMaxDepth = 0;
                GetMaxDepthRecursive(0, root, ref maxDepth);
                if (rootMaxDepth > maxDepth) maxDepth = rootMaxDepth;
            }
            return maxDepth;
        }

        private void GetMaxDepthRecursive(int currentDepth, GameObject g, ref int maxDepth)
        {
            currentDepth++;
            if (currentDepth > maxDepth) maxDepth = currentDepth;
            foreach (var child in g.GetChildren(false, false))
            {
                GetMaxDepthRecursive(currentDepth, child, ref maxDepth);
            }
        }

        public override IList<string> getWarnings()
        {
            var warnings = new List<string>();
            if (type == MergeMode.MergeHierarchyLevel && sameLevel && hierarchyLevel > maxDepth)
            {
                warnings.Add($"Hierarchy level is higher than tree depth ({maxDepth})");
            }
            return warnings;
        }
        public override IList<string> getErrors()
        {
            var errors = new List<string>();
            if (type == MergeMode.MergeHierarchyLevel && !sameLevel)
            {
                errors.Add("Selected GameObjects are not at the same level in Hierarchy.");
            }
            if (type == MergeMode.MergeHierarchyLevel && hierarchyLevel < 1)
            {
                errors.Add("Hierarchy level is too low! (must be higher than 1)");
            }
            if (mergeBySizeOfRegions() && sizeOfRegions <= 0)
            {
                errors.Add("Size of regions is too low! (must be higher than 0)");
            }
            if (mergeByNumberOfRegions() && numberOfRegions <= 0)
            {
                errors.Add("Size of regions is too low! (must be higher than 0)");
            }
            if (skinnedMesh)
            {
                errors.Add("Selection contains Skinned Mesh Renderer.\nMerge is not possible with SkinnedMesh.");
            }
            return errors;
        }
        #endregion
    }
}