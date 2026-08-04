using System;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;
using Esri.GameEngine.Layers.Base;
using Esri.GameEngine.Map;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(IfcGeoPositionExtractor))]
public sealed class ArcGisMapLoader : MonoBehaviour
{
    private sealed class ProjectedFrame
    {
        public ProjectedFrame(
            GameObject anchor,
            Transform transform,
            IfcGeoPositionExtractor.ProjectedPlacement origin)
        {
            Anchor = anchor;
            Transform = transform;
            Origin = origin;
        }

        public GameObject Anchor { get; }
        public Transform Transform { get; }
        public IfcGeoPositionExtractor.ProjectedPlacement Origin { get; }
    }

    private readonly struct BasemapOption
    {
        public BasemapOption(string displayName, string serviceUrl)
        {
            DisplayName = displayName;
            ServiceUrl = serviceUrl;
        }

        public string DisplayName { get; }
        public string ServiceUrl { get; }
    }

    private static readonly BasemapOption[] BasemapOptions =
    {
        new(
            "ArcGIS World Street",
            "https://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer"),
        new(
            "ArcGIS World Imagery",
            "https://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer"),
        new(
            "ArcGIS World Topographic",
            "https://services.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer"),
        new(
            "ArcGIS Light Gray Canvas",
            "https://services.arcgisonline.com/ArcGIS/rest/services/Canvas/World_Light_Gray_Base/MapServer"),
        new(
            "ArcGIS Dark Gray Canvas",
            "https://services.arcgisonline.com/ArcGIS/rest/services/Canvas/World_Dark_Gray_Base/MapServer"),
        new(
            "ArcGIS World Terrain",
            "https://services.arcgisonline.com/ArcGIS/rest/services/World_Terrain_Base/MapServer"),
        new(
            "ArcGIS World Ocean",
            "https://services.arcgisonline.com/ArcGIS/rest/services/Ocean/World_Ocean_Base/MapServer")
    };

    private static readonly IReadOnlyList<string> BasemapNames =
        Array.ConvertAll(BasemapOptions, option => option.DisplayName);

    [SerializeField] private IfcGeoPositionExtractor geoPositionExtractor;
    [SerializeField] private Camera viewingCamera;
    [SerializeField] private string apiKey = string.Empty;
    [SerializeField] private string initialBasemap = "ArcGIS World Street";
    [SerializeField] private Vector3 modelFrameCorrectionEuler = new(-90f, 0f, 0f);

    [Header("Runtime Performance")]
    [Tooltip("Scales ArcGIS tile detail independently from the final render resolution.")]
    [SerializeField, Range(0.1f, 1f)] private float arcGisQualityScalingFactor = 0.6f;
    [Tooltip("Prevents the SDK from extending the camera to hundreds of kilometres in local-map mode.")]
    [SerializeField, Min(1_000f)] private float maximumFarClipDistance = 20_000f;

    private readonly Dictionary<GameObject, GameObject> modelAnchors = new();
    private readonly Dictionary<string, ProjectedFrame> projectedFrames = new();
    private ArcGISMapComponent mapComponent;
    private BasemapOption activeBasemap;
    private GameObject mapRoot;
    private bool mapInitialized;

    public static IReadOnlyList<string> AvailableBasemaps => BasemapNames;
    public string ActiveBasemap { get; private set; } = "ArcGIS World Street";
    public double LastLatitude { get; private set; }
    public double LastLongitude { get; private set; }
    public double LastElevation { get; private set; }

    public event Action<GameObject> MapLoaded;
    public event Action<string> BasemapChanged;

    private void Awake()
    {
        ResolveDependencies();
        if (!TryResolveBasemap(initialBasemap, out activeBasemap))
        {
            activeBasemap = BasemapOptions[0];
        }

        ActiveBasemap = activeBasemap.DisplayName;
    }

    private void OnEnable()
    {
        ResolveDependencies();
        if (geoPositionExtractor != null)
        {
            geoPositionExtractor.GeoPositionApplied -= HandleGeoPositionApplied;
            geoPositionExtractor.GeoPositionApplied += HandleGeoPositionApplied;
        }
    }

    private void OnDisable()
    {
        if (geoPositionExtractor != null)
        {
            geoPositionExtractor.GeoPositionApplied -= HandleGeoPositionApplied;
        }
    }

    public bool SetBasemap(string displayName)
    {
        if (!TryResolveBasemap(displayName, out var option))
        {
            return false;
        }

        activeBasemap = option;
        ActiveBasemap = option.DisplayName;
        if (mapInitialized)
        {
            RebuildMap();
        }

        BasemapChanged?.Invoke(ActiveBasemap);
        Debug.Log($"ArcGIS basemap changed to {ActiveBasemap}.");
        return true;
    }

    public void LoadMap(
        GameObject modelRoot,
        double latitude,
        double longitude,
        double elevation)
    {
        if (modelRoot == null ||
            !double.IsFinite(latitude) ||
            !double.IsFinite(longitude) ||
            !double.IsFinite(elevation))
        {
            Debug.LogWarning("Cannot create the ArcGIS map because the IFC position is invalid.");
            return;
        }

        EnsureMapInfrastructure(longitude, latitude, elevation);
        if (geoPositionExtractor != null &&
            geoPositionExtractor.TryReadProjectedPlacement(
                modelRoot,
                out var projectedPlacement))
        {
            AnchorProjectedModel(modelRoot, projectedPlacement);
        }
        else
        {
            AnchorGeographicModel(modelRoot, longitude, latitude, elevation);
        }

        LastLatitude = latitude;
        LastLongitude = longitude;
        LastElevation = elevation;
        MapLoaded?.Invoke(mapRoot);
        Debug.Log(
            $"ArcGIS map positioned at latitude {latitude:F8}, " +
            $"longitude {longitude:F8}, elevation {elevation:F3} m " +
            $"using {ActiveBasemap}.");
    }

    private void HandleGeoPositionApplied(
        GameObject modelRoot,
        double latitude,
        double longitude,
        double elevation)
    {
        LoadMap(modelRoot, latitude, longitude, elevation);
    }

    private void EnsureMapInfrastructure(
        double longitude,
        double latitude,
        double elevation)
    {
        if (mapComponent == null)
        {
            mapRoot = new GameObject("ArcGIS Map");
            mapComponent = mapRoot.AddComponent<ArcGISMapComponent>();
            mapComponent.MapType = ArcGISMapType.Local;
            mapComponent.APIKey = apiKey;
        }

        mapComponent.MeshCollidersEnabled = false;

        mapComponent.OriginPosition = new ArcGISPoint(
            longitude,
            latitude,
            elevation,
            ArcGISSpatialReference.WGS84());

        EnsureCamera();
        if (!mapInitialized)
        {
            RebuildMap();
            mapInitialized = true;
        }
    }

    private void EnsureCamera()
    {
        if (viewingCamera == null)
        {
            viewingCamera = Camera.main ?? FindFirstObjectByType<Camera>();
        }

        if (viewingCamera == null)
        {
            Debug.LogWarning("No camera is available for ArcGIS map rendering.");
            return;
        }

        if (!viewingCamera.transform.IsChildOf(mapComponent.transform))
        {
            viewingCamera.transform.SetParent(mapComponent.transform, true);
        }

        if (!viewingCamera.TryGetComponent<ArcGISCameraComponent>(out var arcGisCamera))
        {
            arcGisCamera = viewingCamera.gameObject.AddComponent<ArcGISCameraComponent>();
        }


        arcGisCamera.qualityScalingFactor = arcGisQualityScalingFactor;
        arcGisCamera.UpdateClippingPlanes = false;
        viewingCamera.farClipPlane = Mathf.Min(
            viewingCamera.farClipPlane,
            maximumFarClipDistance);
    }

    private void RebuildMap()
    {
        if (mapComponent == null)
        {
            return;
        }

        var map = new ArcGISMap(ArcGISMapType.Local)
        {
            Basemap = new ArcGISBasemap(
                activeBasemap.ServiceUrl,
                ArcGISLayerType.ArcGISImageLayer,
                ResolveApiKey())
        };
        mapComponent.Map = map;
    }

    private void AnchorProjectedModel(
        GameObject modelRoot,
        IfcGeoPositionExtractor.ProjectedPlacement placement)
    {
        if (!projectedFrames.TryGetValue(
                placement.CoordinateSystemKey,
                out var projectedFrame) ||
            projectedFrame.Anchor == null)
        {
            projectedFrame = CreateProjectedFrame(placement);
            projectedFrames[placement.CoordinateSystemKey] = projectedFrame;
        }

        modelRoot.transform.SetParent(projectedFrame.Transform, false);
        modelRoot.transform.localPosition = new Vector3(
            (float)(placement.Easting - projectedFrame.Origin.Easting),
            (float)(placement.Elevation - projectedFrame.Origin.Elevation),
            (float)(placement.Northing - projectedFrame.Origin.Northing));
        modelAnchors[modelRoot] = projectedFrame.Anchor;

        Debug.Log(
            $"Placed IFC model '{modelRoot.name}' in shared projected frame " +
            $"{placement.CoordinateSystemKey} at E {placement.Easting:F3}, " +
            $"N {placement.Northing:F3}, Z {placement.Elevation:F3} m.");
    }

    private ProjectedFrame CreateProjectedFrame(
        IfcGeoPositionExtractor.ProjectedPlacement placement)
    {
        var anchor = new GameObject(
            $"IFC Projected Anchor ({placement.CoordinateSystemKey})");
        anchor.transform.SetParent(mapComponent.transform, false);

        var unityFrame = new GameObject("Unity Ground Frame");
        unityFrame.transform.SetParent(anchor.transform, false);
        unityFrame.transform.localRotation =
            Quaternion.Euler(modelFrameCorrectionEuler);

        var projectedTransform = new GameObject("Projected Coordinate Frame").transform;
        projectedTransform.SetParent(unityFrame.transform, false);
        if (TryCalculateProjectedFrameTransform(
                placement,
                out var yawDegrees,
                out var eastingScale,
                out var northingScale))
        {
            projectedTransform.localRotation =
                Quaternion.Euler(0f, yawDegrees, 0f);
            projectedTransform.localScale = new Vector3(
                eastingScale,
                1f,
                northingScale);
        }

        var location = anchor.AddComponent<ArcGISLocationComponent>();
        location.SurfacePlacementMode = ArcGISSurfacePlacementMode.AbsoluteHeight;
        location.Position = new ArcGISPoint(
            placement.Longitude,
            placement.Latitude,
            placement.Elevation,
            ArcGISSpatialReference.WGS84());
        location.Rotation = new ArcGISRotation(0d, 0d, 0d);

        return new ProjectedFrame(anchor, projectedTransform, placement);
    }

    private void AnchorGeographicModel(
        GameObject modelRoot,
        double longitude,
        double latitude,
        double elevation)
    {
        if (!modelAnchors.TryGetValue(modelRoot, out var anchor) || anchor == null)
        {
            anchor = new GameObject($"{modelRoot.name} ArcGIS Anchor");
            anchor.transform.SetParent(mapComponent.transform, false);

            var modelFrame = new GameObject($"{modelRoot.name} Unity Frame");
            modelFrame.transform.SetParent(anchor.transform, false);
            modelFrame.transform.localRotation =
                Quaternion.Euler(modelFrameCorrectionEuler);

            modelRoot.transform.SetParent(modelFrame.transform, false);
            modelRoot.transform.localPosition = Vector3.zero;
            modelAnchors[modelRoot] = anchor;
        }

        var location = anchor.GetComponent<ArcGISLocationComponent>() ??
                       anchor.AddComponent<ArcGISLocationComponent>();
        location.SurfacePlacementMode = ArcGISSurfacePlacementMode.AbsoluteHeight;
        location.Position = new ArcGISPoint(
            longitude,
            latitude,
            elevation,
            ArcGISSpatialReference.WGS84());
        location.Rotation = new ArcGISRotation(0d, 0d, 0d);
    }

    private static bool TryCalculateProjectedFrameTransform(
        IfcGeoPositionExtractor.ProjectedPlacement placement,
        out float yawDegrees,
        out float eastingScale,
        out float northingScale)
    {
        yawDegrees = 0f;
        eastingScale = 1f;
        northingScale = 1f;
        if (!TryProjectedToWebMercator(
                placement.Easting,
                placement.Northing,
                placement,
                out var originX,
                out var originY) ||
            !TryProjectedToWebMercator(
                placement.Easting + 1d,
                placement.Northing,
                placement,
                out var eastX,
                out var eastY) ||
            !TryProjectedToWebMercator(
                placement.Easting,
                placement.Northing + 1d,
                placement,
                out var northX,
                out var northY))
        {
            return false;
        }

        var eastDeltaX = eastX - originX;
        var eastDeltaY = eastY - originY;
        var northDeltaX = northX - originX;
        var northDeltaY = northY - originY;
        var resolvedEastingScale = Math.Sqrt(
            eastDeltaX * eastDeltaX + eastDeltaY * eastDeltaY);
        var resolvedNorthingScale = Math.Sqrt(
            northDeltaX * northDeltaX + northDeltaY * northDeltaY);
        if (!double.IsFinite(resolvedEastingScale) ||
            !double.IsFinite(resolvedNorthingScale) ||
            resolvedEastingScale <= 0d ||
            resolvedNorthingScale <= 0d)
        {
            return false;
        }

        yawDegrees = (float)(
            Math.Atan2(-eastDeltaY, eastDeltaX) * 180d / Math.PI);
        eastingScale = (float)resolvedEastingScale;
        northingScale = (float)resolvedNorthingScale;
        return true;
    }

    private static bool TryProjectedToWebMercator(
        double easting,
        double northing,
        IfcGeoPositionExtractor.ProjectedPlacement placement,
        out double x,
        out double y)
    {
        const double earthRadius = 6_378_137d;
        x = 0d;
        y = 0d;
        if (!Vn2000CoordinateConverter.TryConvertToWgs84(
                easting,
                northing,
                placement.CentralMeridianDegrees,
                placement.ProjectionScaleFactor,
                out var latitude,
                out var longitude))
        {
            return false;
        }

        latitude = Math.Clamp(latitude, -85.05112878d, 85.05112878d);
        var latitudeRadians = latitude * Math.PI / 180d;
        x = earthRadius * longitude * Math.PI / 180d;
        y = earthRadius * Math.Log(
            Math.Tan(Math.PI / 4d + latitudeRadians / 2d));
        return double.IsFinite(x) && double.IsFinite(y);
    }

    private void ResolveDependencies()
    {
        geoPositionExtractor ??= GetComponent<IfcGeoPositionExtractor>();
        viewingCamera ??= Camera.main;
    }

    private string ResolveApiKey()
    {
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            return apiKey;
        }

        return ArcGISProjectSettingsAsset.Instance?.APIKey ?? string.Empty;
    }

    private static bool TryResolveBasemap(
        string displayName,
        out BasemapOption resolvedOption)
    {
        foreach (var option in BasemapOptions)
        {
            if (!option.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            resolvedOption = option;
            return true;
        }

        resolvedOption = default;
        return false;
    }

    private void OnValidate()
    {
        arcGisQualityScalingFactor = Mathf.Clamp(
            arcGisQualityScalingFactor,
            0.1f,
            1f);
        maximumFarClipDistance = Mathf.Max(1_000f, maximumFarClipDistance);
    }
}
