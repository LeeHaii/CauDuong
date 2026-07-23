using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pixyz.Commons.Extensions;
using Pixyz.Commons.UI.Editor;

namespace Pixyz.Toolbox.Editor
{
    public class MovePivotAction : ActionInOut<IList<GameObject>, IList<GameObject>>
    {
        public override int id => 99541656;
        public override int order => 12;
        public override string menuPathRuleEngine => "Pivot/Move Pivot";
        public override string menuPathToolbox => "Pivot/Move Pivot";
        public override string tooltip => ToolboxTooltips.movePivotAction;

        public enum MovePivotOption
        {
            ToCenterOfBoundingBox,
            ToMininumOfBoundingBox,
            ToMaximumOfBoundingBox,
            ToCenterOfSelection,
            ToOtherGameObjectCenter,
            ToWorldOrigin,
            ToCustom,
            AlignPivotToWorld
        }

        //[UserParameter(tooltip: ToolboxTooltips.runOncePerObject)]
        public bool runOncePerObject = false;

        [UserParameter]
        public MovePivotOption target;

        [UserParameter("isPivotCustom", tooltip: "Transform Pivot Point on X, Y, Z according to its current transform. If uncheck, set its transform according to its parent's pivot position (or World if World space is checked).")]
        public bool relativeToCurrent;

        [UserParameter("isPivotCustom", tooltip: "Take the World as the reference. It lets you place the pivot exactly at one location (X, Y, Z) in your scene. If used simultaneously with 'Relative to current', Pivot Point will be moved from current position respecting world axis alignement.")]
        public bool worldSpace = false;

        [UserParameter("isPivotCustom", tooltip: "Translate Pivot Point on X, Y or Z")]
        public Vector3 position;

        [UserParameter("isPivotCustom", tooltip: "Rotate Pivot Point on X, Y or Z")]
        public Vector3 rotation;

        [UserParameter("movingToOtherGameObject")]
        public GameObject targetGameObject;

        private bool isPivotCustom() => target == MovePivotOption.ToCustom;

        private bool movingToOtherGameObject() => target == MovePivotOption.ToOtherGameObjectCenter;

        private HashSet<GameObject> _selectedGameObjects;

        public override IList<GameObject> run(IList<GameObject> input)
        {
            Core.Native.NativeInterface.PushAnalytic("MovePivot", target.ToString());
            _selectedGameObjects = new HashSet<GameObject>(input);
            if (!runOncePerObject)
            {
                var highestSelectedAncestors = new HashSet<GameObject>();
                for (int i = 0; i < input.Count; i++)
                {
                    Transform current = input[i].transform;
                    GameObject highestSelectedAncestor = null;
                    while (current)
                    {
                        if (_selectedGameObjects.Contains(current.gameObject))
                        {
                            highestSelectedAncestor = current.gameObject;
                        }
                        current = current.parent;
                    }
                    highestSelectedAncestors.Add(highestSelectedAncestor);
                }

                foreach (GameObject gameObject in highestSelectedAncestors)
                {
                    movePivot(gameObject);
                }
            } 
            else
            {
                foreach (GameObject gameObject in input)
                {
                    movePivot(gameObject);
                }
            }
            return input;
        }

        private void movePivot(GameObject gameObject)
        {
            Vector3 newPivotPosition;
            Vector3 newPivotEulerRotation = Vector3.zero;
            bool rotate = false;

            switch (target)
            {
                case MovePivotOption.ToCenterOfBoundingBox:
                    newPivotPosition = gameObject.GetBoundsWorldSpace(true).center;
                    break;
                case MovePivotOption.ToMaximumOfBoundingBox:
                    newPivotPosition = gameObject.GetBoundsWorldSpace(true).max;
                    break;
                case MovePivotOption.ToMininumOfBoundingBox:
                    newPivotPosition = gameObject.GetBoundsWorldSpace(true).min;
                    break;
                case MovePivotOption.ToCustom:

                    if (worldSpace)
                    {
                        newPivotPosition = relativeToCurrent ? (gameObject.transform.position + position) : position;
                        newPivotEulerRotation = relativeToCurrent ? (gameObject.transform.rotation * Quaternion.Euler(rotation)).eulerAngles : rotation;
                    }
                    else
                    {
                        if (relativeToCurrent)
                        {
                            Vector3 newPivotLocalPosition = gameObject.transform.localRotation * position + gameObject.transform.localPosition;
                            newPivotPosition = gameObject.transform.parent != null ? gameObject.transform.parent.TransformPoint(newPivotLocalPosition) : newPivotLocalPosition;
                            newPivotEulerRotation = (gameObject.transform.rotation * Quaternion.Euler(rotation)).eulerAngles;
                        }
                        else
                        {
                            newPivotPosition = gameObject.transform.parent != null ? gameObject.transform.parent.TransformPoint(position) : position;
                            newPivotEulerRotation = gameObject.transform.parent != null ? (gameObject.transform.parent.rotation * Quaternion.Euler(rotation)).eulerAngles : rotation;
                        }
                    }
                    rotate = true;
                    break;
                case MovePivotOption.ToCenterOfSelection:
                    newPivotPosition = _selectedGameObjects.ToList().GetBoundsWorldSpace().center;
                    break;
                case MovePivotOption.ToOtherGameObjectCenter:
                    newPivotPosition = targetGameObject != null ? targetGameObject.GetBoundsWorldSpace(true).center : gameObject.GetBoundsWorldSpace(true).center;
                    break;
                case MovePivotOption.ToWorldOrigin:
                    newPivotPosition = Vector3.zero;
                    break;
                case MovePivotOption.AlignPivotToWorld:
                    newPivotPosition = gameObject.transform.position;
                    newPivotEulerRotation = Vector3.zero;
                    rotate = true;
                    break;
                default:
                    newPivotPosition = gameObject.transform.position;
                    break;
            }

            // Save children states
            var children = gameObject.GetChildren(false, false);
            Vector3[] childrenPositions = new Vector3[children.Count];
            Quaternion[] childrenRotations = new Quaternion[children.Count];
            for (int i = 0; i < children.Count; i++)
            {
                childrenPositions[i] = children[i].transform.position;
                childrenRotations[i] = children[i].transform.rotation;
            }

            // Apply translation
            Vector3 delta = gameObject.transform.position - newPivotPosition;
            Vector3 localDelta = gameObject.transform.InverseTransformVector(delta);
            gameObject.transform.position = newPivotPosition;

            // Apply rotation
            Quaternion inverseDeltaRot = Quaternion.identity;
            if (rotate)
            {
                Quaternion deltaRot = Quaternion.Inverse(gameObject.transform.rotation) * Quaternion.Euler(newPivotEulerRotation);
                inverseDeltaRot = Quaternion.Inverse(deltaRot);
                gameObject.transform.rotation *= deltaRot;
            }

            // Restore children transforms
            for (int i = 0; i < children.Count; i++)
            {
                children[i].transform.position = childrenPositions[i];
                children[i].transform.rotation = childrenRotations[i];
            }

            // Apply transformation on mesh
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter)
            {
                Mesh mesh = Mesh.Instantiate(meshFilter.sharedMesh);
                mesh.name = meshFilter.sharedMesh.name;
                var vertices = mesh.vertices;
                var normals = mesh.normals;
                var tangents = mesh.tangents;

                // Vertices
                for (int j = 0; j < vertices.Length; j++)
                {
                    vertices[j] += localDelta;
                    if (rotate)
                    {
                        vertices[j] = inverseDeltaRot * vertices[j];
                    }
                }

                // Normals
                for (int j = 0; j < normals.Length; j++)
                {
                    normals[j] = inverseDeltaRot * normals[j];
                }

                // Tangents
                for (int j = 0; j < tangents.Length; j++)
                {
                    tangents[j] = inverseDeltaRot * tangents[j];
                }

                mesh.vertices = vertices;
                mesh.normals = normals;
                mesh.tangents = tangents;

                mesh.RecalculateBounds();
                meshFilter.sharedMesh = mesh;
            }

            // Apply transformation on colliders
            BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
            if (boxCollider)
            {
                boxCollider.center += localDelta;
            }
            SphereCollider sphereCollider = gameObject.GetComponent<SphereCollider>();
            if (sphereCollider)
            {
                sphereCollider.center += localDelta;
            }
            CapsuleCollider capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
            if (capsuleCollider)
            {
                capsuleCollider.center += localDelta;
            }
            MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
            if (meshCollider)
            {
                Mesh mesh = Mesh.Instantiate(meshCollider.sharedMesh);
                mesh.name = meshCollider.sharedMesh.name;
                var vertices = mesh.vertices;
                for (int j = 0; j < vertices.Length; j++)
                {
                    vertices[j] += localDelta;
                    if (rotate)
                    {
                        vertices[j] = inverseDeltaRot * vertices[j];
                    }
                }
                mesh.vertices = vertices;
                mesh.RecalculateBounds();
                meshCollider.sharedMesh = mesh;
            }
        }
    }
}