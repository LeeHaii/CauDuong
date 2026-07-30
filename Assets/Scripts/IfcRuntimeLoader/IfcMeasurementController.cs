using System;
using System.Collections.Generic;
using CauDuong.IfcOperations;
using UnityEngine;
using UnityEngine.EventSystems;
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

    private void Awake()
    {
        viewingCamera ??= Camera.main;
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

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (ActiveMode == IfcMeasurementMode.Area && pendingPoints.Count >= 3)
            {
                CompleteArea();
            }
            else
            {
                CancelPendingMeasurement();
                StatusChanged?.Invoke("Đã hủy phép đo hiện tại.");
            }

            return;
        }

        if (!Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }

        var screenPosition = Mouse.current.position.ReadValue();
        if (IsPointerOverDashboard(screenPosition) ||
            !TryGetIfcPoint(screenPosition, out var point))
        {
            return;
        }

        AddPoint(point);
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
    }

    public void Stop()
    {
        CancelPendingMeasurement();
        ActiveMode = IfcMeasurementMode.None;
        IsCapturingInput = false;
        ModeChanged?.Invoke(ActiveMode);
    }

    public void ClearMeasurements()
    {
        CancelPendingMeasurement();
        foreach (var measurement in completedMeasurements)
        {
            if (measurement != null)
            {
                Destroy(measurement);
            }
        }

        completedMeasurements.Clear();
        StatusChanged?.Invoke("Đã xóa tất cả phép đo 3D.");
    }

    private void AddPoint(Vector3 point)
    {
        EnsurePendingMeasurement();
        pendingPoints.Add(point);
        CreateMarker(point, pendingMeasurement.transform);
        UpdatePendingLine();

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
        }
    }

    private void CompleteLinearMeasurement(float value, string label)
    {
        CompletePendingMeasurement();
        StatusChanged?.Invoke($"{label}: {value:N2} m");
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

    private void EnsurePendingMeasurement()
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
        pendingLine.widthMultiplier = GetMarkerScale() * 0.28f;
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
        marker.transform.localScale = Vector3.one * GetMarkerScale();

        if (marker.TryGetComponent<Collider>(out var collider))
        {
            Destroy(collider);
        }

        if (marker.TryGetComponent<Renderer>(out var renderer))
        {
            renderer.sharedMaterial = measurementMaterial;
        }
    }

    private float GetMarkerScale()
    {
        if (viewingCamera == null)
        {
            return 0.4f;
        }

        return Mathf.Clamp(
            Vector3.Distance(viewingCamera.transform.position, viewingCamera.transform.position +
                viewingCamera.transform.forward * 10f) * 0.04f,
            0.25f,
            1.5f);
    }

    private bool TryGetIfcPoint(Vector2 screenPosition, out Vector3 point)
    {
        point = default;
        viewingCamera ??= Camera.main;
        if (viewingCamera == null)
        {
            return false;
        }

        var hits = Physics.RaycastAll(
            viewingCamera.ScreenPointToRay(screenPosition),
            viewingCamera.farClipPlane);
        var nearestDistance = float.PositiveInfinity;
        var found = false;

        foreach (var hit in hits)
        {
            if (hit.distance >= nearestDistance ||
                hit.transform.GetComponentInParent<IfcElementMetadata>() == null)
            {
                continue;
            }

            nearestDistance = hit.distance;
            point = hit.point;
            found = true;
        }

        return found;
    }

    private bool IsPointerOverDashboard(Vector2 screenPosition)
    {
        var dashboardRoot = dashboardDocument?.rootVisualElement;
        if (dashboardRoot?.panel != null)
        {
            var panelPosition = RuntimePanelUtils.ScreenToPanel(
                dashboardRoot.panel,
                screenPosition);
            var picked = dashboardRoot.panel.Pick(panelPosition);
            return picked != null && picked != dashboardRoot;
        }

        return EventSystem.current != null &&
               EventSystem.current.IsPointerOverGameObject();
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
