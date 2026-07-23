using UnityEditor;
using UnityEngine;
using Pixyz.LODTools;
using Pixyz.OptimizeSDK.Runtime;
using Pixyz.Commons.Extensions;
using Pixyz.Commons.Extensions.Editor;
using System.Linq;
using System;

namespace Pixyz.LODTools.Editor
{
    [CustomEditor(typeof(LODRule))]
    public class LODRuleInspector : UnityEditor.Editor
    {
        [SerializeField]
        private LODRule _rule;

        [SerializeField]
        private bool isRepairFoldout = false;
        [SerializeField]
        private bool isDecimateFoldout = false;
        [SerializeField]
        private bool isOcclusionFoldout = false;
        [SerializeField]
        private bool isRemeshFoldout = false;
        [SerializeField]
        private bool isCombineFoldout = false;
        [SerializeField]
        private bool isBillboardFoldout = false;
        [SerializeField]
        private bool isImposterFoldout = false;
        [SerializeField]
        private float _fieldNumberSize = 150;
        [SerializeField]
        private float _toggleSize = 40;
        [SerializeField]
        private float _fieldToggleSize = 150;
        [SerializeField]
        private float _popUpSize = 150;
        [SerializeField]
        private float _labelSize = 225;

        private bool isAdvancedTargetOpened;
        private bool isAdvancedQualityOpened;

        private void OnEnable()
        {
            if (target == null)
                return;

            _rule = (LODRule)serializedObject.targetObject;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (EditorGUI.ChangeCheckScope check = new EditorGUI.ChangeCheckScope())
            {
                DisplayRepairParam();
                DisplayRemeshParam();
                DisplayOcculusionParam();
                DisplayDecimate();
                DisplayCombineParam();
                DisplayBillboardParam();
                DisplayBakeImpostor();

                if (check.changed)
                {
                    EditorUtility.SetDirty(target);
                }
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private void DisplayRepairParam()
        {
            EditorGUILayout.BeginHorizontal();

            _rule.isRepairActivated = EditorGUILayout.ToggleLeft("", _rule.isRepairActivated, GUILayout.Width(_toggleSize));

            isRepairFoldout = EditorExtensions.CustomFoldout(isRepairFoldout);

            EditorExtensions.DisplaySeparator("Repair Mesh", true);

            EditorGUILayout.EndHorizontal();

            if (isRepairFoldout)
            {
                EditorGUI.indentLevel += 2;
                if (_rule.isRepairActivated)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Distance tolerance", GUILayout.MaxWidth(_labelSize));
                    _rule.repairParameters.tolerance = EditorGUILayout.DoubleField(_rule.repairParameters.tolerance, GUILayout.MaxWidth(_fieldNumberSize));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Orient faces", GUILayout.MaxWidth(_labelSize));
                    _rule.repairParameters.orientFaces = EditorGUILayout.Toggle(new GUIContent(name, Toolbox.ToolboxTooltips.repairAction), _rule.repairParameters.orientFaces);
                    EditorGUILayout.EndHorizontal();              
                }
                else
                {
                    EditorGUILayout.LabelField("Vertices within a very small tolerance will be welded in any case");
                }
                EditorGUI.indentLevel -= 2;
                EditorGUILayout.Space();
            }
        }
        private void DisplayBillboardParam()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            bool otherOptiActivated = _rule.isCombineActivated || _rule.isDecimateToQualityActivated || _rule.isDecimateToTargetActivated || _rule.isRemeshActivated || _rule.isRemeshFieldAlignedActivated || _rule.isOcclusionActivated || _rule.isImpostorActivated;

            EditorGUI.BeginDisabledGroup(otherOptiActivated);
            _rule.isBillboardActivated = EditorGUILayout.ToggleLeft("", _rule.isBillboardActivated, GUILayout.Width(_toggleSize));

            if (EditorGUI.EndChangeCheck())
            {
                if (_rule.isBillboardActivated)
                {
                    _rule.isCombineMaterialsActivated = false;
                    _rule.isCombineMeshesByMaterialsActivated = false;
                    _rule.isDecimateToQualityActivated = false;
                    _rule.isDecimateToTargetActivated = false;
                    _rule.isRemeshActivated = false;
                    _rule.isRemeshFieldAlignedActivated = false;
                    _rule.isOcclusionActivated = false;
                    _rule.isImpostorActivated = false;
                    _rule.isCombineActivated = false;

                    isCombineFoldout = false;
                    isDecimateFoldout = false;
                    isRemeshFoldout = false;
                    isOcclusionFoldout = false;
                    isImposterFoldout = false;
                }
                else
                {
                    isBillboardFoldout = false;
                }
            }
            EditorGUI.EndDisabledGroup();

            isBillboardFoldout = EditorExtensions.CustomFoldout(isBillboardFoldout);

            EditorExtensions.DisplaySeparator("Generate Billboard", true);

            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(!_rule.isBillboardActivated);

            EditorGUI.indentLevel += 2;

            if (otherOptiActivated)
            {
                EditorGUILayout.LabelField("Other optimizations are active");
            }

            if (isBillboardFoldout)
            {

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Maps resolution", GUILayout.MaxWidth(_labelSize));
                _rule.billoardParamters.resolution = EditorGUILayout.IntField(_rule.billoardParamters.resolution, GUILayout.Width(_fieldNumberSize));
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("X+", GUILayout.MaxWidth(_labelSize));
                _rule.billoardParamters.XPositiveEnable = EditorGUILayout.ToggleLeft("", _rule.billoardParamters.XPositiveEnable, GUILayout.Width(_fieldToggleSize));
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("X-", GUILayout.MaxWidth(_labelSize));
                _rule.billoardParamters.XNegativeEnable = EditorGUILayout.ToggleLeft("", _rule.billoardParamters.XNegativeEnable, GUILayout.Width(_fieldToggleSize));
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Y+", GUILayout.MaxWidth(_labelSize));
                _rule.billoardParamters.YPositiveEnable = EditorGUILayout.ToggleLeft("", _rule.billoardParamters.YPositiveEnable, GUILayout.Width(_fieldToggleSize));
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Y-", GUILayout.MaxWidth(_labelSize));
                _rule.billoardParamters.YNegativeEnable = EditorGUILayout.ToggleLeft("", _rule.billoardParamters.YNegativeEnable, GUILayout.Width(_fieldToggleSize));
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Z+", GUILayout.MaxWidth(_labelSize));
                _rule.billoardParamters.ZPositiveEnable = EditorGUILayout.ToggleLeft("", _rule.billoardParamters.ZPositiveEnable, GUILayout.Width(_fieldToggleSize));
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Z-", GUILayout.MaxWidth(_labelSize));
                _rule.billoardParamters.ZNegativeEnable = EditorGUILayout.ToggleLeft("", _rule.billoardParamters.ZNegativeEnable, GUILayout.Width(_fieldToggleSize));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            EditorGUI.indentLevel -= 2;

            EditorGUI.EndDisabledGroup();
        }

        enum CombineType
        {
            MergeAll,
            MergeByMaterials
        }
        CombineType combineType;
        private void DisplayCombineParam()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginDisabledGroup(_rule.isBillboardActivated || _rule.isImpostorActivated || _rule.isRemeshActivated || _rule.isRemeshFieldAlignedActivated);

            _rule.isCombineActivated = EditorGUILayout.ToggleLeft("", _rule.isCombineActivated, GUILayout.Width(_toggleSize));

            //if (EditorGUI.EndChangeCheck())
            //{
            //    if (!combineState)
            //    {
            //        _rule.isCombineActivated = false;
            //        _rule.isCombineMaterialsActivated = false;
            //        _rule.isCombineMeshesByMaterialsActivated = false;
            //        isCombineFoldout = false;
            //    }
            //    else
            //    {
            //        _rule.isCombineActivated = true;
            //        _rule.isCombineMaterialsActivated = true;
            //        _rule.isBillboardActivated = false;
            //    }
            //}

            EditorGUI.EndDisabledGroup();

            isCombineFoldout = EditorExtensions.CustomFoldout(isCombineFoldout);

            EditorExtensions.DisplaySeparator("Merge & Combine", true);
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(!_rule.isCombineActivated);

            EditorGUI.indentLevel += 2;
            if (_rule.isRemeshActivated || _rule.isRemeshFieldAlignedActivated)
            {
                EditorGUILayout.LabelField("The active retopolization operation already includes a mesh combination");
            }

            if (isCombineFoldout)
            {
                EditorGUI.BeginChangeCheck();

                //if (EditorGUI.EndChangeCheck())
                //{
                //    _rule.isCombineActivated = true;
                //}

                //if (combineState)
                //    _rule.isCombineMeshesByMaterialsActivated = !_rule.isCombineMaterialsActivated;

                //Merge type selection

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Merge", GUILayout.Width(_labelSize));
                int mergeMode = Array.IndexOf(Enum.GetValues(typeof(CombineMeshesParameters.MergeType)), _rule.combineMeshesParameters.mergeType);
                mergeMode = EditorGUILayout.Popup(mergeMode, new GUIContent[] { new GUIContent("Merge All"), new GUIContent("Merge By Materials") }, GUILayout.Width(_popUpSize));
                _rule.combineMeshesParameters.mergeType = (CombineMeshesParameters.MergeType)Enum.GetValues(typeof(CombineMeshesParameters.MergeType)).GetValue(mergeMode);
                EditorGUILayout.EndHorizontal();

                if (_rule.combineMeshesParameters.mergeType == CombineMeshesParameters.MergeType.MergeAll)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Combine materials", GUILayout.MaxWidth(_labelSize));
                    _rule.isCombineMaterialsActivated = EditorGUILayout.ToggleLeft("", _rule.isCombineMaterialsActivated, GUILayout.Width(_fieldToggleSize));
                    EditorGUILayout.EndHorizontal();

                    if (_rule.isCombineMaterialsActivated)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Re-Generate UV", GUILayout.MaxWidth(_labelSize));
                        _rule.combineMeshesParameters.forceUVGeneration = EditorGUILayout.ToggleLeft("", _rule.combineMeshesParameters.forceUVGeneration, GUILayout.Width(_fieldToggleSize));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Maps resolution", GUILayout.MaxWidth(_labelSize));
                        _rule.combineMeshesParameters.resolution = EditorGUILayout.IntField(_rule.combineMeshesParameters.resolution, GUILayout.Width(_fieldNumberSize));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Padding", GUILayout.MaxWidth(_labelSize));
                        _rule.combineMeshesParameters.padding = EditorGUILayout.IntField(_rule.combineMeshesParameters.padding, GUILayout.Width(_fieldNumberSize));
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.Space();
            }
            EditorGUI.indentLevel -= 2;
            EditorGUI.EndDisabledGroup();
        }

        private void DisplayRemeshParam()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(_rule.isBillboardActivated || _rule.isImpostorActivated);

            bool combineState = EditorGUILayout.ToggleLeft("", _rule.isRemeshActivated || _rule.isRemeshFieldAlignedActivated, GUILayout.Width(_toggleSize));

            if (EditorGUI.EndChangeCheck())
            {
                if (!combineState)
                {
                    _rule.isRemeshActivated = false;
                    _rule.isRemeshFieldAlignedActivated = false;
                    isRemeshFoldout = false;
                }
                else
                {
                    _rule.isRemeshActivated = true;
                    _rule.isOcclusionActivated = false;
                    _rule.isCombineActivated = false;
                    _rule.isBillboardActivated = false;
                }
            }

            EditorGUI.EndDisabledGroup();

            isRemeshFoldout = EditorExtensions.CustomFoldout(isRemeshFoldout);

            EditorExtensions.DisplaySeparator("Retopologize", true);
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(!_rule.isRemeshActivated && !_rule.isRemeshFieldAlignedActivated);

            if (isRemeshFoldout)
            {
                EditorGUI.indentLevel += 2;
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Type", GUILayout.MaxWidth(_labelSize));
                int value = EditorGUILayout.Popup(_rule.isRemeshActivated ? 0 : 1, new GUIContent[] { new GUIContent("Standard"), new GUIContent("Field Aligned") }, GUILayout.Width(_popUpSize));
                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    _rule.isRemeshActivated = value == 0;
                    _rule.isRemeshFieldAlignedActivated = value == 1;
                }

                if (_rule.isRemeshActivated)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Quality preset", GUILayout.MaxWidth(_labelSize));

                    EditorGUI.BeginChangeCheck();
                    _rule.remeshParameters.qualityPreset = (RemeshQualityPreset)EditorGUILayout.EnumPopup(_rule.remeshParameters.qualityPreset, GUILayout.Width(_popUpSize));

                    EditorGUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck())
                    {
                        if(_rule.remeshParameters.qualityPreset != RemeshQualityPreset.Custom)
                            _rule.remeshParameters.qualityValue = (int)_rule.remeshParameters.qualityPreset;
                    }

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Quality value", GUILayout.MaxWidth(_labelSize));
                    _rule.remeshParameters.qualityValue = EditorGUILayout.IntField(_rule.remeshParameters.qualityValue, GUILayout.Width(_fieldNumberSize));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Bake Maps", GUILayout.MaxWidth(_labelSize));
                    _rule.remeshParameters.bakeMaps = EditorGUILayout.ToggleLeft("", _rule.remeshParameters.bakeMaps, GUILayout.Width(_fieldToggleSize));
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.BeginDisabledGroup(!_rule.remeshParameters.bakeMaps);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Maps resolution", GUILayout.MaxWidth(_labelSize));
                    int mapsResolutionIndex = Array.IndexOf(Enum.GetValues(typeof(MapDimensionPreset)), (MapDimensionPreset)_rule.remeshParameters.mapsResolution);
                    if (mapsResolutionIndex == -1)
                        mapsResolutionIndex = 0;
                    mapsResolutionIndex = EditorGUILayout.Popup(mapsResolutionIndex, new GUIContent[] { new GUIContent("Custom"), new GUIContent("256"), new GUIContent("512"), new GUIContent("1024"), new GUIContent("2048"), new GUIContent("4096"), new GUIContent("8192") }, GUILayout.Width(_popUpSize));
                    EditorGUILayout.EndHorizontal();
                    var vall = (int)Enum.GetValues(typeof(MapDimensionPreset)).GetValue(mapsResolutionIndex);
                    if (mapsResolutionIndex != 0)
                        _rule.remeshParameters.mapsResolution = (int)Enum.GetValues(typeof(MapDimensionPreset)).GetValue(mapsResolutionIndex);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Resolution", GUILayout.MaxWidth(_labelSize));
                    _rule.remeshParameters.mapsResolution = EditorGUILayout.IntField(_rule.remeshParameters.mapsResolution, GUILayout.Width(_fieldNumberSize));
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.EndDisabledGroup();                    
                }
                else if (_rule.isRemeshFieldAlignedActivated)
                {
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Target type", GUILayout.MaxWidth(_labelSize));
                    int ratioSelect = EditorGUILayout.Popup(_rule.remeshFieldAlignedParameters.isTargetCount ? 0 : 1, new GUIContent[] { new GUIContent("Polycount"), new GUIContent("Ratio") }, GUILayout.Width(_popUpSize));
                    EditorGUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck())
                    {
                        _rule.remeshFieldAlignedParameters.isTargetCount = ratioSelect == 0;
                    }

                    if (_rule.remeshFieldAlignedParameters.isTargetCount)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Polycount", GUILayout.MaxWidth(_labelSize));
                        _rule.remeshFieldAlignedParameters.targetTriangleCount = EditorGUILayout.IntField(_rule.remeshFieldAlignedParameters.targetTriangleCount, GUILayout.Width(_fieldNumberSize));
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Ratio", GUILayout.MaxWidth(_labelSize));
                        _rule.remeshFieldAlignedParameters.targetRatio = EditorGUILayout.Slider((float)_rule.remeshFieldAlignedParameters.targetRatio * 100.0f, 0.0f, 100.0f, GUILayout.Width(_popUpSize)) / 100.0f;
                        EditorGUILayout.EndHorizontal();
                    }
                    
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Use feature size", GUILayout.MaxWidth(_labelSize));
                    _rule.remeshFieldAlignedParameters.useFeatureSize = EditorGUILayout.ToggleLeft("", _rule.remeshFieldAlignedParameters.useFeatureSize, GUILayout.Width(_fieldToggleSize));
                    EditorGUILayout.EndHorizontal();

                    if(_rule.remeshFieldAlignedParameters.useFeatureSize)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Feature size", GUILayout.MaxWidth(_labelSize));

                        _rule.remeshFieldAlignedParameters.featureSize = EditorGUILayout.DoubleField(_rule.remeshFieldAlignedParameters.featureSize, GUILayout.Width(_fieldNumberSize));
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Bake maps", GUILayout.MaxWidth(_labelSize));
                    _rule.remeshFieldAlignedParameters.bakeMaps = EditorGUILayout.ToggleLeft("", _rule.remeshFieldAlignedParameters.bakeMaps, GUILayout.Width(_fieldToggleSize));
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.BeginDisabledGroup(!_rule.remeshFieldAlignedParameters.bakeMaps);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Maps resolution", GUILayout.MaxWidth(_labelSize));
                    int mapsResolutionIndex = Array.IndexOf(Enum.GetValues(typeof(MapDimensionPreset)), (MapDimensionPreset)_rule.remeshFieldAlignedParameters.mapsResolution);
                    if (mapsResolutionIndex == -1)
                        mapsResolutionIndex = 0;
                    mapsResolutionIndex = EditorGUILayout.Popup(mapsResolutionIndex, new GUIContent[] { new GUIContent("Custom"), new GUIContent("256"), new GUIContent("512"), new GUIContent("1024"), new GUIContent("2048"), new GUIContent("4096"), new GUIContent("8192") }, GUILayout.Width(_popUpSize));
                    EditorGUILayout.EndHorizontal();
                    var vall = (int)Enum.GetValues(typeof(MapDimensionPreset)).GetValue(mapsResolutionIndex);
                    if (mapsResolutionIndex != 0)
                        _rule.remeshFieldAlignedParameters.mapsResolution = (int)Enum.GetValues(typeof(MapDimensionPreset)).GetValue(mapsResolutionIndex);


                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Maps resolution", GUILayout.MaxWidth(_labelSize));
                    _rule.remeshFieldAlignedParameters.mapsResolution = EditorGUILayout.IntField(_rule.remeshFieldAlignedParameters.mapsResolution, GUILayout.Width(_fieldNumberSize));
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.EndDisabledGroup();

                }
                EditorGUI.indentLevel -= 2;
                EditorGUILayout.Space();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void DisplayDecimate()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginDisabledGroup(_rule.isBillboardActivated || _rule.isImpostorActivated);

            bool decimateState = EditorGUILayout.ToggleLeft("", _rule.isDecimateToQualityActivated || _rule.isDecimateToTargetActivated, GUILayout.Width(_toggleSize));

            if (EditorGUI.EndChangeCheck())
            {
                if (!decimateState)
                {
                    _rule.isDecimateToQualityActivated = false;
                    _rule.isDecimateToTargetActivated = false;
                    isDecimateFoldout = false;
                }
                else
                {
                    _rule.isDecimateToTargetActivated = true;
                    _rule.isBillboardActivated = false;
                }
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginChangeCheck();

            bool foldout = EditorExtensions.CustomFoldout(isDecimateFoldout);

            if (EditorGUI.EndChangeCheck())
            {
                isDecimateFoldout = foldout;
            }

            EditorExtensions.DisplaySeparator("Decimate", true);
            EditorGUILayout.EndHorizontal();


            EditorGUI.BeginDisabledGroup(!_rule.isDecimateToQualityActivated && !_rule.isDecimateToTargetActivated);

            if (isDecimateFoldout)
            {
                EditorGUI.indentLevel += 2;
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Criterion", GUILayout.MaxWidth(_labelSize));
                int value = EditorGUILayout.Popup(_rule.isDecimateToTargetActivated ? 0 : 1, new GUIContent[] { new GUIContent("To Target"), new GUIContent("To Quality") }, GUILayout.Width(_popUpSize));
                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    _rule.isDecimateToTargetActivated = value == 0;
                    _rule.isDecimateToQualityActivated = value == 1;
                }

                int ratioSelect = 0;
                
                if (_rule.isDecimateToTargetActivated)
                {
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Strategy", GUILayout.MaxWidth(_labelSize));
                    ratioSelect = EditorGUILayout.Popup(_rule.decimateToTarget.isTargetCount ? 0 : 1, new GUIContent[] { new GUIContent("Triangle count"), new GUIContent("Ratio") }, GUILayout.Width(_popUpSize));
                    _rule.decimateToTarget.isTargetCount = ratioSelect == 0 ? true : false;
                    EditorGUILayout.EndHorizontal();

                    if (_rule.decimateToTarget.isTargetCount)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Triangle count", GUILayout.MaxWidth(_labelSize));
                        _rule.decimateToTarget.polycount = EditorGUILayout.IntField(_rule.decimateToTarget.polycount, GUILayout.Width(_fieldNumberSize));
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Ratio", GUILayout.MaxWidth(_labelSize));
                        _rule.decimateToTarget.ratio = EditorGUILayout.Slider((float)_rule.decimateToTarget.ratio, 0.0f, 100.0f, GUILayout.Width(150));
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Preserve painted areas", GUILayout.MaxWidth(_labelSize));
                    _rule.decimateToTarget.preservePaintedAreas = EditorGUILayout.ToggleLeft("", _rule.decimateToTarget.preservePaintedAreas, GUILayout.Width(_fieldToggleSize));
                    EditorGUILayout.EndHorizontal();

                    isAdvancedTargetOpened = EditorGUILayout.Foldout(isAdvancedTargetOpened, new GUIContent("Advanced"));

                    if(isAdvancedTargetOpened)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("UV importance", GUILayout.MaxWidth(_labelSize));
                        int uvImportanceIndex = (int)_rule.decimateToTarget.uvImportance;
                        uvImportanceIndex = EditorGUILayout.Popup(uvImportanceIndex, new GUIContent[] { new GUIContent("Preserve seams and reduce deformation"), new GUIContent("Preserve seams"), new GUIContent("Do not care") }, GUILayout.Width(_popUpSize));
                        _rule.decimateToTarget.uvImportance = (UvImportancePreset)Enum.GetValues(typeof(UvImportancePreset)).GetValue(uvImportanceIndex);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Protect topology", GUILayout.MaxWidth(_labelSize));
                        _rule.decimateToTarget.protectTopology = EditorGUILayout.ToggleLeft("", _rule.decimateToTarget.protectTopology, GUILayout.Width(_fieldToggleSize));
                        EditorGUILayout.EndHorizontal();
                    }

                }
                else if(_rule.isDecimateToQualityActivated)
                {
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Mesh Quality Preset", GUILayout.MaxWidth(_labelSize));

                    _rule.decimateToQualityParam.quality = (OptimizeSDK.Runtime.QualityLevel)EditorGUILayout.EnumPopup(_rule.decimateToQualityParam.quality, GUILayout.Width(_popUpSize));
                    EditorGUILayout.EndHorizontal();
                    
                    if (EditorGUI.EndChangeCheck())
                    {

                        switch (_rule.decimateToQualityParam.quality)
                        {
                            case OptimizeSDK.Runtime.QualityLevel.High:
                                _rule.decimateToQualityParam.lineicTolerance = DecimateToQualityParameters.High().lineicTolerance;
                                _rule.decimateToQualityParam.normalTolerance = DecimateToQualityParameters.High().normalTolerance;
                                _rule.decimateToQualityParam.uvTolerance = DecimateToQualityParameters.High().uvTolerance;
                                _rule.decimateToQualityParam.surfacicTolerance = DecimateToQualityParameters.High().surfacicTolerance;
                                break;
                            case OptimizeSDK.Runtime.QualityLevel.Medium:
                                _rule.decimateToQualityParam.lineicTolerance = DecimateToQualityParameters.Medium().lineicTolerance;
                                _rule.decimateToQualityParam.normalTolerance = DecimateToQualityParameters.Medium().normalTolerance;
                                _rule.decimateToQualityParam.uvTolerance = DecimateToQualityParameters.Medium().uvTolerance;
                                _rule.decimateToQualityParam.surfacicTolerance = DecimateToQualityParameters.Medium().surfacicTolerance;
                                break;
                            case OptimizeSDK.Runtime.QualityLevel.Low:
                                _rule.decimateToQualityParam.lineicTolerance = DecimateToQualityParameters.Low().lineicTolerance;
                                _rule.decimateToQualityParam.normalTolerance = DecimateToQualityParameters.Low().normalTolerance;
                                _rule.decimateToQualityParam.uvTolerance = DecimateToQualityParameters.Low().uvTolerance;
                                _rule.decimateToQualityParam.surfacicTolerance = DecimateToQualityParameters.Low().surfacicTolerance;
                                break;
                            case OptimizeSDK.Runtime.QualityLevel.Poor:
                                _rule.decimateToQualityParam.lineicTolerance = DecimateToQualityParameters.Poor().lineicTolerance;
                                _rule.decimateToQualityParam.normalTolerance = DecimateToQualityParameters.Poor().normalTolerance;
                                _rule.decimateToQualityParam.uvTolerance = DecimateToQualityParameters.Poor().uvTolerance;
                                _rule.decimateToQualityParam.surfacicTolerance = DecimateToQualityParameters.Poor().surfacicTolerance;
                                break;
                            default:
                                break;
                        }    
                    }
                    
                    isAdvancedQualityOpened = EditorGUILayout.Foldout(isAdvancedQualityOpened, new GUIContent("Advanced"));
                    if (isAdvancedQualityOpened)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Surfacic tolerance", GUILayout.MaxWidth(_labelSize));
                        _rule.decimateToQualityParam.surfacicTolerance = EditorGUILayout.DoubleField(_rule.decimateToQualityParam.surfacicTolerance, GUILayout.Width(_fieldNumberSize));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Lineic tolerance", GUILayout.MaxWidth(_labelSize));
                        _rule.decimateToQualityParam.lineicTolerance = EditorGUILayout.DoubleField(_rule.decimateToQualityParam.lineicTolerance, GUILayout.Width(_fieldNumberSize));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Normal tolerance", GUILayout.MaxWidth(_labelSize));
                        _rule.decimateToQualityParam.normalTolerance = EditorGUILayout.DoubleField(_rule.decimateToQualityParam.normalTolerance, GUILayout.Width(_fieldNumberSize));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("UV tolerance", GUILayout.MaxWidth(_labelSize));
                        _rule.decimateToQualityParam.uvTolerance = EditorGUILayout.DoubleField(_rule.decimateToQualityParam.uvTolerance, GUILayout.Width(_fieldNumberSize));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Fobid UV foldovers", GUILayout.MaxWidth(_labelSize));
                        _rule.remeshFieldAlignedParameters.bakeMaps = EditorGUILayout.ToggleLeft("", _rule.remeshFieldAlignedParameters.bakeMaps, GUILayout.Width(_fieldToggleSize));
                        EditorGUILayout.EndHorizontal();

                    }

                }

                EditorGUI.indentLevel -= 2;
            }

            
            EditorGUILayout.Space();
            
            EditorGUI.EndDisabledGroup();
        }

        private void DisplayOcculusionParam()
        {
            EditorGUI.BeginDisabledGroup(_rule.isRemeshActivated || _rule.isRemeshFieldAlignedActivated || _rule.isBillboardActivated || _rule.isImpostorActivated);

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();

            _rule.isOcclusionActivated = EditorGUILayout.ToggleLeft("", _rule.isOcclusionActivated, GUILayout.Width(_toggleSize));

            if (EditorGUI.EndChangeCheck())
            {
                if (_rule.isOcclusionActivated)
                {
                    _rule.isBillboardActivated = false;
                }
            }

            EditorGUI.EndDisabledGroup();

            isOcclusionFoldout = EditorExtensions.CustomFoldout(isOcclusionFoldout);

            EditorExtensions.DisplaySeparator("Remove Hidden", true);
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(!_rule.isOcclusionActivated);

            EditorGUI.indentLevel += 2;

            if (_rule.isRemeshActivated || _rule.isRemeshFieldAlignedActivated)
            {
                EditorGUILayout.LabelField("The active retopolization operation already includes a hidden removal operation");
            }

            if (isOcclusionFoldout)
            {
                EditorGUI.BeginChangeCheck();

                //Strategy
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Strategy", GUILayout.Width(_labelSize));
                int occlusionMode = EditorGUILayout.Popup((int)_rule.occlusionParameters.mode, new GUIContent[] { new GUIContent("Standard"), new GUIContent("Advanced") }, GUILayout.Width(_popUpSize));
                EditorGUILayout.EndHorizontal();

                if(_rule.occlusionParameters.mode == OcclusionMode.Standard)
                {
                    //Precision Preset
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Precision Preset", GUILayout.Width(_labelSize));
                    var enumVals = Enum.GetValues(typeof(PrecisonPreset));
                    int precisionPresetIndex = Array.IndexOf(Enum.GetValues(typeof(PrecisonPreset)), (PrecisonPreset)_rule.occlusionParameters.precision);
                    if (precisionPresetIndex == -1) precisionPresetIndex = 0;
                    precisionPresetIndex = EditorGUILayout.Popup(precisionPresetIndex, new GUIContent[] { new GUIContent("Custom"), new GUIContent("Low"), new GUIContent("Medium"), new GUIContent("High") }, GUILayout.Width(_popUpSize));
                    if (precisionPresetIndex != 0)
                        _rule.occlusionParameters.precision = (int)Enum.GetValues(typeof(PrecisonPreset)).GetValue(precisionPresetIndex);
                    EditorGUILayout.EndHorizontal();

                    //Precision Custom
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Precision", GUILayout.Width(_labelSize));
                    _rule.occlusionParameters.precision = EditorGUILayout.IntField(_rule.occlusionParameters.precision, GUILayout.Width(_fieldNumberSize));
                    EditorGUILayout.EndHorizontal();
                }
                else if(_rule.occlusionParameters.mode == OcclusionMode.Advanced)
                {
                    //Precision Preset
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Precision", GUILayout.Width(_labelSize));
                    int densityPresetIndex = Array.IndexOf(Enum.GetValues(typeof(Density)), _rule.occlusionParameters.density);
                    if (densityPresetIndex == -1) densityPresetIndex = 0;
                    densityPresetIndex = EditorGUILayout.Popup(densityPresetIndex, new GUIContent[] { new GUIContent("Low"), new GUIContent("Medium"), new GUIContent("High") }, GUILayout.Width(_popUpSize));
                    if (densityPresetIndex != 0)
                        _rule.occlusionParameters.density = (int)Enum.GetValues(typeof(Density)).GetValue(densityPresetIndex);
                    EditorGUILayout.EndHorizontal();
                }

                //Level
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Level", GUILayout.Width(_labelSize));
                int selectionLevelIndex = Array.IndexOf(Enum.GetValues(typeof(SelectionLevel)), (SelectionLevel)_rule.occlusionParameters.selectionLevel);
                if (selectionLevelIndex == -1) selectionLevelIndex = 0;
                selectionLevelIndex = EditorGUILayout.Popup(selectionLevelIndex, new GUIContent[] { new GUIContent("GameObject"), new GUIContent("SubMesh"), new GUIContent("Triangles") }, GUILayout.Width(_popUpSize));
                _rule.occlusionParameters.selectionLevel = (SelectionLevel)Enum.GetValues(typeof(SelectionLevel)).GetValue(selectionLevelIndex);
                EditorGUILayout.EndHorizontal();

                //Neighbour Preservation
                if(_rule.occlusionParameters.selectionLevel == SelectionLevel.Triangles)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Neighbours preservation", GUILayout.MaxWidth(_labelSize));
                    _rule.occlusionParameters.neighbourPreservation = EditorGUILayout.IntField(_rule.occlusionParameters.neighbourPreservation, GUILayout.Width(_fieldNumberSize));
                    EditorGUILayout.EndHorizontal();
                }
                
                //Consider transparency opaque
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Consider transparency opaque", GUILayout.MaxWidth(_labelSize));
                _rule.occlusionParameters.considerTransparentOpaque = EditorGUILayout.ToggleLeft("", _rule.occlusionParameters.considerTransparentOpaque, GUILayout.Width(_fieldToggleSize));
                EditorGUILayout.EndHorizontal();


                if (EditorGUI.EndChangeCheck())
                {
                    _rule.occlusionParameters.mode = (OcclusionMode)occlusionMode;
                    //_rule.occlusionParameters.density = (Density)Enum.GetValues(typeof(Density)).GetValue(precisionPresetIndex);
                }

                if (_rule.occlusionParameters.mode == OcclusionMode.Advanced)
                {

                    //PreserveCavities
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("PreserveCavities", GUILayout.MaxWidth(_labelSize));
                    _rule.occlusionParameters.preserveCavities = EditorGUILayout.ToggleLeft("", _rule.occlusionParameters.preserveCavities, GUILayout.Width(_fieldToggleSize));
                    EditorGUILayout.EndHorizontal();

                }
                EditorGUILayout.Space();
            }
            EditorGUI.indentLevel -= 2;

            EditorGUI.EndDisabledGroup();
        }
    
        private void DisplayBakeImpostor()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            bool otherOptiActivated = _rule.isCombineActivated || _rule.isDecimateToQualityActivated || _rule.isDecimateToTargetActivated || _rule.isRemeshActivated || _rule.isRemeshFieldAlignedActivated || _rule.isOcclusionActivated || _rule.isBillboardActivated;

            EditorGUI.BeginDisabledGroup(otherOptiActivated);
            _rule.isImpostorActivated = EditorGUILayout.ToggleLeft("", _rule.isImpostorActivated, GUILayout.Width(_toggleSize));

            if (EditorGUI.EndChangeCheck())
            {
                if (_rule.isImpostorActivated)
                {
                    _rule.isCombineMaterialsActivated = false;
                    _rule.isCombineMeshesByMaterialsActivated = false;
                    _rule.isDecimateToQualityActivated = false;
                    _rule.isDecimateToTargetActivated = false;
                    _rule.isRemeshActivated = false;
                    _rule.isRemeshFieldAlignedActivated = false;
                    _rule.isOcclusionActivated = false;
                    _rule.isCombineActivated = false;

                    isCombineFoldout = false;
                    isDecimateFoldout = false;
                    isRemeshFoldout = false;
                    isOcclusionFoldout = false;
                    isBillboardFoldout = false;
                }
                else
                {
                    isImposterFoldout = false;
                }
            }
            EditorGUI.EndDisabledGroup();

            isImposterFoldout = EditorExtensions.CustomFoldout(isImposterFoldout);

            EditorExtensions.DisplaySeparator("Create Impostor", true);

            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(!_rule.isImpostorActivated);

            EditorGUI.indentLevel += 2;

            if (otherOptiActivated)
            {
                EditorGUILayout.LabelField("Other optimizations are active");
            }

            if (isImposterFoldout)
            {
                //Maps resolution
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Maps resolution", GUILayout.MaxWidth(_labelSize));
                int resolutionIndex = Array.IndexOf(Enum.GetValues(typeof(MapDimensionPreset)), (MapDimensionPreset)_rule.impostorParameters.resolution);
                if (resolutionIndex == -1) resolutionIndex = 0;
                resolutionIndex = EditorGUILayout.Popup(resolutionIndex, new GUIContent[] { new GUIContent("Custom"), new GUIContent("256"), new GUIContent("512"), new GUIContent("1024"), new GUIContent("2048"), new GUIContent("4096"), new GUIContent("8192") }, GUILayout.Width(_popUpSize));
                if (resolutionIndex != 0)
                    _rule.impostorParameters.resolution = (int)Enum.GetValues(typeof(MapDimensionPreset)).GetValue(resolutionIndex);
                EditorGUILayout.EndHorizontal();

                if (resolutionIndex == 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Resolution", GUILayout.MaxWidth(_labelSize));
                    _rule.impostorParameters.resolution = EditorGUILayout.IntField(_rule.impostorParameters.resolution);
                    EditorGUILayout.EndHorizontal();
                }


                //Atlas size
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Atlas size", GUILayout.MaxWidth(_labelSize));
                _rule.impostorParameters.atlasSize = EditorGUILayout.IntField(_rule.impostorParameters.atlasSize, GUILayout.Width(_fieldNumberSize));
                EditorGUILayout.EndHorizontal();

                //Impostor type
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Impostor type", GUILayout.MaxWidth(_labelSize));
                int typeIndex = (int)_rule.impostorParameters.type;
                typeIndex = EditorGUILayout.Popup((int)_rule.impostorParameters.type, new GUIContent[] { new GUIContent("Octahedron"), new GUIContent("Hemi octahedron") }, GUILayout.Width(_popUpSize));
                _rule.impostorParameters.type = (ImpostorType)Enum.GetValues(typeof(ImpostorType)).GetValue(typeIndex);
                EditorGUILayout.EndHorizontal();

                //Render on
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Render on", GUILayout.MaxWidth(_labelSize));
                int renderOnIndex = (int)_rule.impostorParameters.renderOn;
                renderOnIndex = EditorGUILayout.Popup(renderOnIndex, new GUIContent[] { new GUIContent("Quad"), new GUIContent("Oriented bounding box"), new GUIContent("Custom mesh") }, GUILayout.Width(_popUpSize));
                _rule.impostorParameters.renderOn = (RenderImpostorOn)Enum.GetValues(typeof(RenderImpostorOn)).GetValue(renderOnIndex);
                EditorGUILayout.EndHorizontal();

                if(_rule.impostorParameters.renderOn == RenderImpostorOn.CustomMesh)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Custom mesh", GUILayout.MaxWidth(_labelSize));
                    _rule.impostorParameters.customMesh = (GameObject)EditorGUILayout.ObjectField(_rule.impostorParameters.customMesh, typeof(GameObject), true, GUILayout.Width(_popUpSize));
                    EditorGUILayout.EndHorizontal();
                }

                //Save maps
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Save maps", GUILayout.MaxWidth(_labelSize));
                _rule.impostorParameters.saveMaps = EditorGUILayout.ToggleLeft("", _rule.impostorParameters.saveMaps, GUILayout.Width(_fieldToggleSize));
                EditorGUILayout.EndHorizontal();

                if(_rule.impostorParameters.saveMaps)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Maps path", GUILayout.MaxWidth(_labelSize));
                    _rule.impostorParameters.mapsPath = EditorGUILayout.TextField(_rule.impostorParameters.mapsPath, GUILayout.Width(_fieldNumberSize));
                    EditorGUILayout.EndHorizontal();
                }
            }
            

            EditorGUI.indentLevel -= 2;

            EditorGUI.EndDisabledGroup();
        }
    }
}