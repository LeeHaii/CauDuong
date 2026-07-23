using System.Collections.Generic;
using UnityEngine;
using Pixyz.Commons.Extensions;
using Pixyz.Commons.Extensions.Editor;
using Pixyz.Commons.UI.Editor;
using System.Linq;
using Pixyz.OptimizeSDK.Editor.MeshProcessing;

namespace Pixyz.Toolbox.Editor
{
    public class ReplaceBy : PixyzFunction
    {
        public override int id => 58476964;
        public override int order => 10;
        public override string menuPathRuleEngine => "Hierarchy/Replace by...";
        public override string menuPathToolbox => "Hierarchy/Replace by...";
        public override string tooltip => ToolboxTooltips.replaceByAction;

        // No need to sync materials between Unity and Pixyz for decimation purposes
        protected override MaterialSyncType SyncMaterials => MaterialSyncType.SyncNone;

        public enum ReplaceByMode
        {
            GameObject,
            BoundingBox,
            Mesh
        }

        private bool isReplaceByBB() => replaceBy == ReplaceByMode.BoundingBox;
        private bool isReplaceByGO() => replaceBy == ReplaceByMode.GameObject;
        private bool isReplaceByMesh() => replaceBy == ReplaceByMode.Mesh;

        private bool checkReplaceMode()
        {
            if (replaceBy == ReplaceByMode.BoundingBox)
                _isAsync = true;
            else
                _isAsync = false;

            return true;
        }

        [UserParameter("checkReplaceMode", tooltip:ToolboxTooltips.replaceByMode)]
        public ReplaceByMode replaceBy = ReplaceByMode.GameObject;

        [UserParameter("isReplaceByBB")]
        public Algo.Native.ReplaceByBoxType boundingBox = Algo.Native.ReplaceByBoxType.LocallyAligned;

        [UserParameter("isReplaceByGO")]
        public GameObject gameobject = null;

        [UserParameter("isReplaceByMesh")]
        public Mesh mesh=null;

        [UserParameter(tooltip: ToolboxTooltips.replaceByRotation)]
        public bool replaceRotation;

        [UserParameter(tooltip: ToolboxTooltips.replaceByScale)]
        public bool replaceScale;

        bool skinnedMesh;

        public override bool preProcess(IList<GameObject> input)
        {
            if(replaceBy == ReplaceByMode.GameObject)
            {
                _input = input;
                _output = _input;
                return true;
            }
            else
            {
                return base.preProcess(input);
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

        protected override void process()
        {            
            if(replaceBy == ReplaceByMode.BoundingBox)
            {
                uint scene = Scene.Native.NativeInterface.CreateSceneFromMeshes(Context.pixyzMeshes, Context.pixyzMatrices, true);
                Scene.Native.OccurrenceList occurrenceList = new Scene.Native.OccurrenceList(new uint[] { scene });

                Core.Native.NativeInterface.PushAnalytic("ReplaceByBox", "");
                //ReplaceByBox
                Algo.Native.ReplaceByOption option = new Algo.Native.ReplaceByOption();
                option.BoundingBox = boundingBox;
                option._type = Algo.Native.ReplaceByOption.Type.BOUNDINGBOX;
                Algo.Native.NativeInterface.ReplaceBy(occurrenceList, option);
                Scene.Native.NativeInterface.ResetPartTransform(occurrenceList[0]);
            }
            else if(replaceBy == ReplaceByMode.Mesh)
            {
                var inputMeshes = _input.GetMeshes();

                MeshFilter[] meshFilters = Object.FindObjectsOfType<MeshFilter>();

                UnityEngine.Material standardMat = new UnityEngine.Material(Shader.Find("Standard"));

                for(int j = 0;j< _input.Count;j++)
                {
                    Mesh refMesh = _input[j].GetComponent<MeshFilter>().sharedMesh;
                    for (int i = 0; i < meshFilters.Length; i++)
                    {
                        var instanceID = meshFilters[i].sharedMesh.GetInstanceID();
                        var referenceInstanceID = _input[j].GetComponent<MeshFilter>().sharedMesh.GetInstanceID();
                        //if (instanceID == referenceInstanceID)
                        if(compareMeshes(meshFilters[i].sharedMesh, refMesh))
                        {
                            MeshRenderer meshRenderer = meshFilters[i].gameObject.GetComponent<MeshRenderer>();
                            if (meshRenderer != null)
                            {
                                UnityEditor.Undo.RecordObject(meshFilters[i], "ReplaceByMesh_" + meshFilters[i].gameObject.name);
                                UnityEditor.Undo.RecordObject(meshRenderer, "ReplaceByMesh_" + meshFilters[i].gameObject.name);
                                UnityEngine.Material[] materials = new UnityEngine.Material[mesh.subMeshCount];
                                UnityEngine.Material baseMaterial = meshRenderer.sharedMaterials[0];
                                if (baseMaterial == null)
                                    baseMaterial = standardMat;
                                for (int m = 0; m < materials.Length; m++)
                                {
                                    materials[m] = baseMaterial;
                                }
                                meshRenderer.sharedMaterials = materials;
                                meshFilters[i].sharedMesh = mesh;

                                UnityEditor.EditorUtility.SetDirty(meshRenderer);
                                UnityEditor.EditorUtility.SetDirty(meshFilters[i]);
                            }


                        }

                    }
                }
            }
        }

        protected override void postProcess()
        {
            base.postProcess();

            if (replaceBy == ReplaceByMode.GameObject)
            {
                for (int i = 0; i < _input.Count; ++i)
                {
                    Transform transform = _input[i].transform;
                    var localPosition = transform.localPosition;
                    var localScale = transform.localScale;
                    var localRotation = transform.localRotation;
                    var parent = transform.parent;

                    var goRotation = gameobject.transform.localRotation;
                    var goScale = gameobject.transform.localScale;

                    _input[i] = SceneExtensions.Instantiate(gameobject);

                    if (transform.gameObject != null)
                        SceneExtensionsEditor.DestroyImmediateSafe(transform.gameObject);

                    transform = _input[i].transform;
                    transform.parent = parent;
                    transform.localPosition = localPosition;
                    transform.localRotation = replaceRotation ? goRotation : localRotation;
                    transform.localScale = replaceScale ? goScale : localScale;
                }
            }
            else if(replaceBy != ReplaceByMode.Mesh)
            {
                GameObject[] outputParts = Context.PixyzMeshToUnityObject(Context.pixyzMeshes);
                ReplaceInHierarchy(InputParts, outputParts);
            }
        }

        public override IList<string> getWarnings()
        {
            var warnings = new List<string>();
            if (isReplaceByMesh())
            { 
                if (skinnedMesh)
                {
                    warnings.Add("Selection contains Skinned Mesh Renderer.\nReplaceBy is not possible with SkinnedMesh.");
                }
            }
            return warnings;
        }

        public override IList<string> getErrors()
        {
            var errors = new List<string>();
            if (isReplaceByGO() && gameobject == null)
            {
                errors.Add("Gameobject field must be set !");
            }

            if(isReplaceByMesh())
            {
                if(mesh == null)
                {
                    errors.Add("Mesh field must be set !");
                }
            }
            return errors;
        }
    
        bool compareMeshes(Mesh a, Mesh b)
        {
            return a.vertices.SequenceEqual(b.vertices);
        }
    
    }
}

