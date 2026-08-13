using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public sealed class ArcGisCursorCoordinateDebugger : MonoBehaviour
{
    [Header("Cursor Query")]
    [SerializeField] private LayerMask raycastMask = ~0;
    [SerializeField] private float maximumRayDistance = 500_000f;
    [SerializeField] private QueryTriggerInteraction triggerInteraction =
        QueryTriggerInteraction.Ignore;

    [Header("Map Fallback")]
    [Tooltip("Elevation used when the cursor points at the flat ArcGIS basemap, which has no physics collider.")]
    [SerializeField] private double mapPlaneElevation;

    private Camera targetCamera;
    private ArcGISMapComponent mapComponent;

    private void Awake()
    {
        targetCamera = GetComponent<Camera>();
        ResolveMapComponent();
    }

    private void Update()
    {
        if (Keyboard.current?.mKey.wasPressedThisFrame == true)
        {
            LogCoordinateUnderCursor();
        }
    }

    [ContextMenu("Log Coordinate Under Cursor")]
    public void LogCoordinateUnderCursor()
    {
        if (Mouse.current == null)
        {
            Debug.LogWarning("[Map Coordinate] No mouse device is available.", this);
            return;
        }

        if (targetCamera == null)
        {
            targetCamera = GetComponent<Camera>();
        }

        if (targetCamera == null || !ResolveMapComponent())
        {
            Debug.LogWarning(
                "[Map Coordinate] The camera or ArcGIS map is not available yet.",
                this);
            return;
        }

        var screenPosition = Mouse.current.position.ReadValue();
        var ray = targetCamera.ScreenPointToRay(screenPosition);
        if (!TryFindWorldPoint(ray, out var worldPoint, out var source))
        {
            Debug.LogWarning(
                "[Map Coordinate] The cursor ray does not intersect the map.",
                this);
            return;
        }

        ArcGISPoint geographicPoint;
        try
        {
            var mapPoint = mapComponent.EngineToGeographic(worldPoint);
            geographicPoint = ArcGISGeometryEngine.Project(
                mapPoint,
                ArcGISSpatialReference.WGS84()) as ArcGISPoint;
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning(
                $"[Map Coordinate] ArcGIS conversion failed: {exception.Message}",
                this);
            return;
        }

        if (geographicPoint == null ||
            !double.IsFinite(geographicPoint.X) ||
            !double.IsFinite(geographicPoint.Y) ||
            !double.IsFinite(geographicPoint.Z))
        {
            Debug.LogWarning(
                "[Map Coordinate] ArcGIS returned an invalid geographic position.",
                this);
            return;
        }

        Debug.LogWarning(
            $"[Map Coordinate] Latitude: {geographicPoint.Y:F8}, " +
            $"Longitude: {geographicPoint.X:F8}, " +
            $"Elevation: {geographicPoint.Z:F3} m " +
            $"(source: {source})",
            this);
    }

    private bool TryFindWorldPoint(
        Ray ray,
        out Vector3 worldPoint,
        out string source)
    {
        if (Physics.Raycast(
                ray,
                out var hit,
                maximumRayDistance,
                raycastMask,
                triggerInteraction))
        {
            worldPoint = hit.point;
            source = hit.collider.name;
            return true;
        }

        var origin = mapComponent.OriginPosition;
        if (origin == null)
        {
            worldPoint = default;
            source = string.Empty;
            return false;
        }

        var spatialReference = origin.SpatialReference ??
                               ArcGISSpatialReference.WGS84();
        var geographicPlanePoint = new ArcGISPoint(
            origin.X,
            origin.Y,
            mapPlaneElevation,
            spatialReference);
        var enginePlanePoint = mapComponent.GeographicToEngine(
            geographicPlanePoint);
        var mapPlane = new Plane(mapComponent.transform.up, enginePlanePoint);
        if (!mapPlane.Raycast(ray, out var distance) ||
            distance < 0f ||
            distance > maximumRayDistance)
        {
            worldPoint = default;
            source = string.Empty;
            return false;
        }

        worldPoint = ray.GetPoint(distance);
        source = "ArcGIS basemap plane";
        return true;
    }

    private bool ResolveMapComponent()
    {
        if (mapComponent != null)
        {
            return true;
        }

        mapComponent = GetComponentInParent<ArcGISMapComponent>() ??
                       FindFirstObjectByType<ArcGISMapComponent>();
        return mapComponent != null;
    }

    private void OnValidate()
    {
        maximumRayDistance = Mathf.Max(1f, maximumRayDistance);
    }
}
