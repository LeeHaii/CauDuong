using System;
using System.Collections.Generic;
using CauDuong.IfcOperations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public enum IfcMeasurementMode
{
    None,
    Distance,
    Height,
    Area
}

[DisallowMultipleComponent]
public sealed class IfcMeasurementController : MonoBehaviour
{
    [SerializeField] private Camera viewingCamera;
    [SerializeField] private UIDocument dashboardDocument;
    [SerializeField] private Color measurementColor = new(0.08f, 0.75f, 1f, 1f);

    private readonly List<Vector3> pendingPoints = new();
    private readonly List<GameObject> completedMeasurements = new();
    private GameObject pendingMeasurement;
    private LineRenderer pendingLine;
    private Material measurementMaterial;

    public static bool IsCapturingInput { get; private set; }
    public IfcMeasurementMode ActiveMode { get; private set; }

    public event Action<string> StatusChanged;
    public event Action<IfcMeasurementMode> ModeChanged;
    public event Action<string, string, bool> HudChanged;

    private void Awake()
    {
        ResolveViewingCamera();
        dashboardDocument ??= GetComponent<UIDocument>();
        measurementMaterial = CreateMeasurementMaterial();
    }

    private void OnDisable()
    {
        CancelPendingMeasurement();
        ActiveMode = IfcMeasurementMode.None;
        IsCapturingInput = false;
    }

    private void OnDestroy()
    {
        if (measurementMaterial != null)
        {
            Destroy(measurementMaterial);
        }
    }

    private void Update()
    {
        if (ActiveMode == IfcMeasurementMode.None || Mouse.current == null)
        {
            return;
        }

        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Stop();
            StatusChanged?.Invoke("Đã thoát chế độ đo 3D.");
            return;
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (ActiveMode == IfcMeasurementMode.Area && pendingPoints.Count >= 3)
            {
                CompleteArea();
            }
            else
            {
                Stop();
                StatusChanged?.Invoke("Đã hủy phép đo và thoát chế độ đo 3D.");
            }

            return;
        }

        if (!Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }

        var screenPosition = Mouse.current.position.ReadValue();
        HandlePrimaryClick(screenPosition);
    }

    private void HandlePrimaryClick(Vector2 screenPosition)
    {
        if (IfcUiHitTest.IsPointerOverInteractiveUi(dashboardDocument, screenPosition))
        {
            return;
        }

        if (!IfcInteractionRaycaster.TryRaycast(
                ResolveViewingCamera(),
                screenPosition,
                out var hit,
                out _))
        {
            StatusChanged?.Invoke(
                "Không tìm thấy bề mặt IFC. Hãy nhấp trực tiếp lên mô hình.");
            return;
        }

        var point = hit.point;
        AddPoint(point);
    }

    private Camera ResolveViewingCamera()
    {
        if (viewingCamera == null)
        {
            viewingCamera = Camera.main;
        }

        if (viewingCamera == null)
        {
            viewingCamera = FindFirstObjectByType<Camera>();
        }

        return viewingCamera;
    }

    public void Begin(IfcMeasurementMode mode)
    {
        CancelPendingMeasurement();
        ActiveMode = mode;
        IsCapturingInput = mode != IfcMeasurementMode.None;
        ModeChanged?.Invoke(mode);

        StatusChanged?.Invoke(mode switch
        {
            IfcMeasurementMode.Distance =>
                "Đo khoảng cách: chọn 2 điểm trên mô hình.",
            IfcMeasurementMode.Height =>
                "Đo chiều cao: chọn 2 điểm trên mô hình.",
            IfcMeasurementMode.Area =>
                "Đo diện tích: chọn ít nhất 3 điểm, nhấn chuột phải để hoàn tất.",
            _ => "Đã tắt công cụ đo."
        });
        HudChanged?.Invoke(
            GetModeTitle(mode),
            GetModeInstruction(mode),
            mode != IfcMeasurementMode.None);
    }

    public void Stop()
    {
        CancelPendingMeasurement();
        ActiveMode = IfcMeasurementMode.None;
        IsCapturingInput = false;
        ModeChanged?.Invoke(ActiveMode);
        HudChanged?.Invoke(string.Empty, string.Empty, false);
    }

    public void ClearMeasurements()
    {
        Stop();
        foreach (var measurement in completedMeasurements)
        {
            if (measurement != null)
            {
                Destroy(measurement);
            }
        }

        completedMeasurements.Clear();
        StatusChanged?.Invoke(
            "Đã xóa tất cả phép đo và trở lại điều khiển camera.");
    }

    private void AddPoint(Vector3 point)
    {
        EnsurePendingMeasurement(point);
        pendingPoints.Add(point);
        CreateMarker(point, pendingMeasurement.transform);
        UpdatePendingLine();

        if (pendingPoints.Count == 1 &&
            ActiveMode is IfcMeasurementMode.Distance or IfcMeasurementMode.Height)
        {
            var message = "Đã đặt điểm 1/2. Chọn điểm thứ hai.";
            StatusChanged?.Invoke(message);
            HudChanged?.Invoke(GetModeTitle(ActiveMode), message, true);
        }

        if (ActiveMode == IfcMeasurementMode.Distance && pendingPoints.Count == 2)
        {
            CompleteLinearMeasurement(
                IfcMeasurementMath.Distance(pendingPoints[0], pendingPoints[1]),
                "Khoảng cách");
        }
        else if (ActiveMode == IfcMeasurementMode.Height && pendingPoints.Count == 2)
        {
            var first = pendingPoints[0];
            var second = pendingPoints[1];
            pendingLine.positionCount = 3;
            pendingLine.SetPosition(0, first);
            pendingLine.SetPosition(1, new Vector3(first.x, second.y, first.z));
            pendingLine.SetPosition(2, second);
            CompleteLinearMeasurement(
                IfcMeasurementMath.Height(first, second),
                "Chiều cao");
        }
        else if (ActiveMode == IfcMeasurementMode.Area)
        {
            StatusChanged?.Invoke(
                $"Đo diện tích: đã chọn {pendingPoints.Count} điểm.");
            HudChanged?.Invoke(
                GetModeTitle(ActiveMode),
                $"Đã đặt {pendingPoints.Count} điểm. Chuột phải để hoàn tất.",
                true);
        }
    }

    private void CompleteLinearMeasurement(float value, string label)
    {
        CompletePendingMeasurement();
        StatusChanged?.Invoke($"{label}: {value:N2} m");
        HudChanged?.Invoke(label, $"{value:N2} m", true);
        StopWithoutDestroyingCompleted();
    }

    private void CompleteArea()
    {
        pendingLine.positionCount = pendingPoints.Count + 1;
        for (var index = 0; index < pendingPoints.Count; index++)
        {
            pendingLine.SetPosition(index, pendingPoints[index]);
        }

        pendingLine.SetPosition(pendingPoints.Count, pendingPoints[0]);
        var area = IfcMeasurementMath.PolygonArea(pendingPoints);
        CompletePendingMeasurement();
        StatusChanged?.Invoke($"Diện tích: {area:N2} m²");
        HudChanged?.Invoke("Diện tích", $"{area:N2} m²", true);
        StopWithoutDestroyingCompleted();
    }

    private void CompletePendingMeasurement()
    {
        if (pendingMeasurement != null)
        {
            completedMeasurements.Add(pendingMeasurement);
        }

        pendingMeasurement = null;
        pendingLine = null;
        pendingPoints.Clear();
    }

    private void StopWithoutDestroyingCompleted()
    {
        ActiveMode = IfcMeasurementMode.None;
        IsCapturingInput = false;
        ModeChanged?.Invoke(ActiveMode);
    }

    private void EnsurePendingMeasurement(Vector3 firstPoint)
    {
        if (pendingMeasurement != null)
        {
            return;
        }

        pendingMeasurement = new GameObject($"IFC Measurement - {ActiveMode}");
        pendingMeasurement.transform.SetParent(transform, true);
        pendingLine = pendingMeasurement.AddComponent<LineRenderer>();
        pendingLine.useWorldSpace = true;
        pendingLine.loop = false;
        pendingLine.material = measurementMaterial;
        pendingLine.startColor = measurementColor;
        pendingLine.endColor = measurementColor;
        pendingLine.widthMultiplier = GetMarkerScale(firstPoint) * 0.28f;
        pendingLine.numCapVertices = 4;
        pendingLine.numCornerVertices = 3;
    }

    private void UpdatePendingLine()
    {
        pendingLine.positionCount = pendingPoints.Count;
        for (var index = 0; index < pendingPoints.Count; index++)
        {
            pendingLine.SetPosition(index, pendingPoints[index]);
        }
    }

    private void CreateMarker(Vector3 point, Transform parent)
    {
        var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.name = "Measurement Point";
        marker.transform.SetParent(parent, true);
        marker.transform.position = point;
        marker.transform.localScale = Vector3.one * GetMarkerScale(point);

        if (marker.TryGetComponent<Collider>(out var collider))
        {
            Destroy(collider);
        }

        if (marker.TryGetComponent<Renderer>(out var renderer))
        {
            renderer.sharedMaterial = measurementMaterial;
        }
    }

    private float GetMarkerScale(Vector3 point)
    {
        if (viewingCamera == null)
        {
            return 0.4f;
        }

        var distance = Vector3.Distance(viewingCamera.transform.position, point);
        if (viewingCamera.orthographic)
        {
            return Mathf.Clamp(
                viewingCamera.orthographicSize * 0.018f,
                0.08f,
                12f);
        }

        var worldUnitsPerPixel =
            2f * distance *
            Mathf.Tan(viewingCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) /
            Mathf.Max(1f, Screen.height);
        return Mathf.Clamp(worldUnitsPerPixel * 10f, 0.08f, 12f);
    }

    private void CancelPendingMeasurement()
    {
        pendingPoints.Clear();
        pendingLine = null;
        if (pendingMeasurement != null)
        {
            Destroy(pendingMeasurement);
            pendingMeasurement = null;
        }
    }

    private static string GetModeTitle(IfcMeasurementMode mode)
    {
        return mode switch
        {
            IfcMeasurementMode.Distance => "ĐO KHOẢNG CÁCH",
            IfcMeasurementMode.Height => "ĐO CHIỀU CAO",
            IfcMeasurementMode.Area => "ĐO DIỆN TÍCH",
            _ => string.Empty
        };
    }

    private static string GetModeInstruction(IfcMeasurementMode mode)
    {
        return mode switch
        {
            IfcMeasurementMode.Distance => "Chọn 2 điểm trên mô hình IFC.",
            IfcMeasurementMode.Height => "Chọn điểm đáy và điểm đỉnh.",
            IfcMeasurementMode.Area =>
                "Chọn ít nhất 3 điểm, nhấn chuột phải để hoàn tất.",
            _ => string.Empty
        };
    }

    private Material CreateMeasurementMaterial()
    {
        var shader = Shader.Find("Sprites/Default") ??
                     Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
        {
            throw new InvalidOperationException(
                "No compatible shader was found for IFC measurement graphics.");
        }

        var material = new Material(shader)
        {
            name = "IFC Measurement Material",
            color = measurementColor
        };
        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", measurementColor);
        }

        if (material.HasProperty("_Cull"))
        {
            material.SetFloat("_Cull", (float)CullMode.Off);
        }

        return material;
    }
}
