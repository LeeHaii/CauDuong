using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public sealed class IfcInspectionMarker : MonoBehaviour
{
    private static Material sharedMaterial;

    private Camera viewingCamera;

    public long InspectionId { get; private set; }

    public static IfcInspectionMarker Create(
        Transform parent,
        Vector3 worldPosition,
        long inspectionId,
        Camera camera)
    {
        var markerObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        markerObject.name = $"Field Inspection {inspectionId}";
        markerObject.transform.SetParent(parent, true);
        markerObject.transform.position = worldPosition;

        ConfigureRenderer(markerObject.GetComponent<Renderer>());
        var stem = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        stem.name = "Pin Stem";
        stem.transform.SetParent(markerObject.transform, false);
        stem.transform.localPosition = new Vector3(0f, -0.82f, 0f);
        stem.transform.localScale = new Vector3(0.16f, 0.72f, 0.16f);
        ConfigureRenderer(stem.GetComponent<Renderer>());
        var stemCollider = stem.GetComponent<Collider>();
        if (stemCollider != null)
        {
            Destroy(stemCollider);
        }

        var marker = markerObject.AddComponent<IfcInspectionMarker>();
        marker.InspectionId = inspectionId;
        marker.viewingCamera = camera;
        return marker;
    }

    private void LateUpdate()
    {
        viewingCamera ??= Camera.main;
        if (viewingCamera == null)
        {
            return;
        }

        var desiredWorldScale = Mathf.Clamp(
            Vector3.Distance(viewingCamera.transform.position, transform.position) * 0.012f,
            0.65f,
            7f);
        var parentScale = transform.parent != null
            ? transform.parent.lossyScale
            : Vector3.one;
        transform.localScale = new Vector3(
            desiredWorldScale / Mathf.Max(Mathf.Abs(parentScale.x), 0.0001f),
            desiredWorldScale / Mathf.Max(Mathf.Abs(parentScale.y), 0.0001f),
            desiredWorldScale / Mathf.Max(Mathf.Abs(parentScale.z), 0.0001f));
    }

    private static void ConfigureRenderer(Renderer renderer)
    {
        if (renderer == null)
        {
            return;
        }

        renderer.sharedMaterial = GetMaterial();
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    private static Material GetMaterial()
    {
        if (sharedMaterial != null)
        {
            return sharedMaterial;
        }

        var shader = Shader.Find("Universal Render Pipeline/Unlit") ??
                     Shader.Find("Unlit/Color") ??
                     Shader.Find("Standard");
        sharedMaterial = new Material(shader)
        {
            name = "Field Inspection Marker",
            color = new Color32(220, 47, 68, 255),
            hideFlags = HideFlags.HideAndDontSave
        };
        if (sharedMaterial.HasProperty("_BaseColor"))
        {
            sharedMaterial.SetColor("_BaseColor", new Color32(220, 47, 68, 255));
        }

        return sharedMaterial;
    }
}
