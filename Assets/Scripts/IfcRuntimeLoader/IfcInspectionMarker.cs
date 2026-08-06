using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public sealed class IfcInspectionMarker : MonoBehaviour
{
    private static Material openMaterial;
    private static Material resolvedMaterial;
    private static Material pointMaterial;
    private static Material textMaterial;

    private Camera viewingCamera;
    private Transform callout;

    public long InspectionId { get; private set; }

    public static IfcInspectionMarker Create(
        Transform parent,
        Vector3 worldPosition,
        long inspectionId,
        Camera camera,
        string title,
        bool isResolved)
    {
        var markerObject = new GameObject($"Field Inspection {inspectionId}");
        markerObject.transform.SetParent(parent, true);
        markerObject.transform.position = worldPosition;

        var point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        point.name = "Coordinate Point";
        point.transform.SetParent(markerObject.transform, false);
        point.transform.localScale = Vector3.one * 0.38f;
        ConfigureRenderer(point.GetComponent<Renderer>(), GetPointMaterial());

        var stem = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        stem.name = "Callout Stem";
        stem.transform.SetParent(markerObject.transform, false);
        stem.transform.localPosition = new Vector3(0f, 1.35f, 0f);
        stem.transform.localScale = new Vector3(0.07f, 1.35f, 0.07f);
        ConfigureRenderer(stem.GetComponent<Renderer>(), GetPointMaterial());
        RemoveCollider(stem);

        var calloutRoot = new GameObject("Callout");
        calloutRoot.transform.SetParent(markerObject.transform, false);
        calloutRoot.transform.localPosition = new Vector3(0f, 3.05f, 0f);

        var background = GameObject.CreatePrimitive(PrimitiveType.Cube);
        background.name = "Callout Background";
        background.transform.SetParent(calloutRoot.transform, false);
        background.transform.localScale = new Vector3(4.9f, 1.45f, 0.1f);
        ConfigureRenderer(
            background.GetComponent<Renderer>(),
            GetStatusMaterial(isResolved));

        var textObject = new GameObject("Callout Label");
        textObject.transform.SetParent(calloutRoot.transform, false);
        textObject.transform.localPosition = new Vector3(0f, 0f, -0.065f);
        var label = textObject.AddComponent<TextMesh>();
        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.font = font;
        label.text =
            $"{(isResolved ? "ĐÃ XỬ LÝ" : "CHƯA XỬ LÝ")}\n{Truncate(title, 34)}";
        label.alignment = TextAlignment.Center;
        label.anchor = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.fontSize = 64;
        label.characterSize = 0.12f;
        label.fontStyle = FontStyle.Bold;
        var labelRenderer = textObject.GetComponent<MeshRenderer>();
        labelRenderer.sharedMaterial = GetTextMaterial(font);
        labelRenderer.shadowCastingMode = ShadowCastingMode.Off;
        labelRenderer.receiveShadows = false;
        labelRenderer.sortingOrder = 10;

        var marker = markerObject.AddComponent<IfcInspectionMarker>();
        marker.InspectionId = inspectionId;
        marker.viewingCamera = camera;
        marker.callout = calloutRoot.transform;
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
            Vector3.Distance(viewingCamera.transform.position, transform.position) * 0.018f,
            0.9f,
            12f);
        var parentScale = transform.parent != null
            ? transform.parent.lossyScale
            : Vector3.one;
        transform.localScale = new Vector3(
            desiredWorldScale / Mathf.Max(Mathf.Abs(parentScale.x), 0.0001f),
            desiredWorldScale / Mathf.Max(Mathf.Abs(parentScale.y), 0.0001f),
            desiredWorldScale / Mathf.Max(Mathf.Abs(parentScale.z), 0.0001f));

        if (callout != null)
        {
            var direction = callout.position - viewingCamera.transform.position;
            if (direction.sqrMagnitude > 0.001f)
            {
                callout.rotation = Quaternion.LookRotation(
                    direction.normalized,
                    viewingCamera.transform.up);
            }
        }
    }

    private static string Truncate(string value, int maxLength)
    {
        var text = string.IsNullOrWhiteSpace(value) ? "Ghi nhận hiện trường" : value.Trim();
        return text.Length <= maxLength
            ? text
            : text.Substring(0, maxLength - 1) + "…";
    }

    private static void RemoveCollider(GameObject target)
    {
        var collider = target.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }
    }

    private static void ConfigureRenderer(Renderer renderer, Material material)
    {
        if (renderer == null)
        {
            return;
        }

        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    private static Material GetPointMaterial()
    {
        return pointMaterial ??= CreateMaterial(
            "Inspection Coordinate Point",
            new Color32(222, 49, 72, 255));
    }

    private static Material GetStatusMaterial(bool isResolved)
    {
        if (isResolved)
        {
            return resolvedMaterial ??= CreateMaterial(
                "Resolved Inspection Callout",
                new Color32(8, 126, 87, 255));
        }

        return openMaterial ??= CreateMaterial(
            "Open Inspection Callout",
            new Color32(181, 105, 8, 255));
    }

    private static Material GetTextMaterial(Font font)
    {
        if (textMaterial != null)
        {
            return textMaterial;
        }

        textMaterial = new Material(font.material)
        {
            name = "Inspection Callout Text",
            hideFlags = HideFlags.HideAndDontSave,
            renderQueue = (int)RenderQueue.Overlay + 1
        };
        textMaterial.SetInt("unity_GUIZTestMode", (int)CompareFunction.Always);
        if (textMaterial.HasProperty("_ZTest"))
        {
            textMaterial.SetInt("_ZTest", (int)CompareFunction.Always);
        }

        return textMaterial;
    }

    private static Material CreateMaterial(string name, Color color)
    {
        var shader = Resources.Load<Shader>("IfcInspectionCallout") ??
                     Shader.Find("Hidden/IfcInspectionCallout") ??
                     Shader.Find("Universal Render Pipeline/Unlit") ??
                     Shader.Find("Unlit/Color") ??
                     Shader.Find("Standard");
        var material = new Material(shader)
        {
            name = name,
            color = color,
            hideFlags = HideFlags.HideAndDontSave
        };
        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }

        if (material.HasProperty("_Cull"))
        {
            material.SetFloat("_Cull", 0f);
        }

        if (material.HasProperty("_ZTest"))
        {
            material.SetInt("_ZTest", (int)CompareFunction.Always);
        }

        if (material.HasProperty("_ZWrite"))
        {
            material.SetInt("_ZWrite", 0);
        }

        material.renderQueue = (int)RenderQueue.Overlay;

        return material;
    }
}
