using CauDuong.IfcOperations;
using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public sealed class IfcInspectionMarker : MonoBehaviour
{
    private static Material openMaterial;
    private static Material operationalMaterial;
    private static Material warningMaterial;
    private static Material criticalMaterial;
    private static Material repairingMaterial;
    private static Material pointMaterial;
    private static Material textMaterial;

    private Camera viewingCamera;
    private Transform callout;
    private Renderer calloutBackground;
    private TextMesh calloutLabel;
    private string calloutTitle;
    private float nextSurfaceProbeTime;

    public long InspectionId { get; private set; }
    public IfcElementMetadata LinkedElement { get; private set; }

    public void AssignLinkedElement(IfcElementMetadata metadata)
    {
        if (metadata != null)
        {
            LinkedElement = metadata;
        }
    }

    public static IfcInspectionMarker Create(
        Transform parent,
        Vector3 worldPosition,
        long inspectionId,
        IfcElementMetadata linkedElement,
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
            GetOpenMaterial());

        var textObject = new GameObject("Callout Label");
        textObject.transform.SetParent(calloutRoot.transform, false);
        textObject.transform.localPosition = new Vector3(0f, 0f, -0.065f);
        var label = textObject.AddComponent<TextMesh>();
        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.font = font;
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
        marker.LinkedElement = linkedElement;
        marker.viewingCamera = camera;
        marker.callout = calloutRoot.transform;
        marker.calloutBackground = background.GetComponent<Renderer>();
        marker.calloutLabel = label;
        marker.calloutTitle = title;
        marker.SetInspectionStatus(isResolved);
        return marker;
    }

    public void SetInspectionStatus(bool isResolved)
    {
        ApplyStatus(
            isResolved ? "ĐÃ XỬ LÝ" : "CHƯA XỬ LÝ",
            isResolved ? GetOperationalMaterial() : GetOpenMaterial());
    }

    public void SetElementStatus(IfcOperationalStatus status, bool hasUserUpdate)
    {
        if (IfcInspectionCalloutPolicy.ShouldShowUnresolved(status, hasUserUpdate))
        {
            ApplyStatus("CHƯA XỬ LÝ", GetOpenMaterial());
            return;
        }

        switch (status)
        {
            case IfcOperationalStatus.Warning:
                ApplyStatus("BẢO TRÌ", GetWarningMaterial());
                break;
            case IfcOperationalStatus.Critical:
                ApplyStatus("HỎNG HÓC", GetCriticalMaterial());
                break;
            case IfcOperationalStatus.Repairing:
                ApplyStatus("ĐANG SỬA", GetRepairingMaterial());
                break;
            default:
                ApplyStatus("CHƯA XỬ LÝ", GetOpenMaterial());
                break;
        }
    }

    private void ApplyStatus(string statusText, Material material)
    {
        if (calloutLabel != null)
        {
            calloutLabel.text = $"{statusText}\n{Truncate(calloutTitle, 34)}";
        }

        if (calloutBackground != null)
        {
            calloutBackground.sharedMaterial = material;
        }
    }

    private void LateUpdate()
    {
        viewingCamera ??= Camera.main;
        if (viewingCamera == null)
        {
            return;
        }

        var desiredWorldScale = Mathf.Clamp(
            Vector3.Distance(viewingCamera.transform.position, transform.position) *
            0.018f * GetScreenScaleCompensation(viewingCamera),
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

        if (LinkedElement == null && Time.unscaledTime >= nextSurfaceProbeTime)
        {
            nextSurfaceProbeTime = Time.unscaledTime + 1f;
            TryLinkSurfaceBelow();
        }
    }

    private void TryLinkSurfaceBelow()
    {
        var origin = transform.position + Vector3.up * 250f;
        var hits = Physics.RaycastAll(
            origin,
            Vector3.down,
            500f,
            ~0,
            QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (left, right) => left.distance.CompareTo(right.distance));
        foreach (var hit in hits)
        {
            if (hit.collider == null || hit.collider.transform.IsChildOf(transform))
            {
                continue;
            }

            var metadata = hit.collider.GetComponentInParent<IfcElementMetadata>();
            if (metadata == null)
            {
                continue;
            }

            LinkedElement = metadata;
            return;
        }
    }

    private static float GetScreenScaleCompensation(Camera camera)
    {
        var pixelHeight = camera.pixelHeight > 0 ? camera.pixelHeight : Screen.height;
        return Mathf.Clamp(1080f / Mathf.Max(480f, pixelHeight), 0.85f, 1.6f);
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

    private static Material GetOpenMaterial()
    {
        return openMaterial ??= CreateMaterial(
            "Open Inspection Callout",
            new Color32(181, 105, 8, 255));
    }

    private static Material GetOperationalMaterial()
    {
        return operationalMaterial ??= CreateMaterial(
            "Operational Inspection Callout",
            new Color32(8, 126, 87, 255));
    }

    private static Material GetWarningMaterial()
    {
        return warningMaterial ??= CreateMaterial(
            "Warning Inspection Callout",
            new Color32(214, 140, 17, 255));
    }

    private static Material GetCriticalMaterial()
    {
        return criticalMaterial ??= CreateMaterial(
            "Critical Inspection Callout",
            new Color32(190, 42, 55, 255));
    }

    private static Material GetRepairingMaterial()
    {
        return repairingMaterial ??= CreateMaterial(
            "Repairing Inspection Callout",
            new Color32(33, 104, 205, 255));
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
