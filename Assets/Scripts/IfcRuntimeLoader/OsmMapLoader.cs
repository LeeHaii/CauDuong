using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CesiumForUnity;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class OsmMapLoader : MonoBehaviour
{
    private const double EarthCircumferenceMetres = 40075016.68557849d;
    private const double MaximumMercatorLatitude = 85.05112878d;
    private static readonly TimeSpan MinimumCacheLifetime = TimeSpan.FromDays(7d);

    [Header("Source")]
    [SerializeField] private IfcGeoPositionExtractor geoPositionExtractor;
    [SerializeField] private string tileUrlTemplate =
        "https://tile.openstreetmap.org/{z}/{x}/{y}.png";
    [SerializeField] private string userAgent = "CauDuong-IFC-Viewer/1.0";

    [Header("Coverage")]
    [SerializeField, Range(1, 19)] private int preferredZoom = 18;
    [SerializeField, Range(1, 19)] private int minimumZoom = 12;
    [SerializeField, Min(0)] private int paddingTiles = 1;
    [SerializeField, Range(1, 15)] private int maxTilesPerAxis = 9;
    [SerializeField, Range(1, 225)] private int maxTotalTiles = 49;

    [Header("Presentation")]
    [SerializeField] private float verticalOffsetMetres = -0.05f;
    [SerializeField] private bool showAttribution = true;

    private readonly List<UnityEngine.Object> runtimeAssets = new();
    private Coroutine loadRoutine;
    private GameObject mapRoot;
    private GameObject attributionObject;

    public int ActiveZoom { get; private set; }
    public int LoadedTileCount { get; private set; }
    public int RequestedTileCount { get; private set; }

    public event Action<int, int> TileProgressChanged;
    public event Action<GameObject> MapLoaded;

    private readonly struct LocalBounds
    {
        public LocalBounds(
            double minX,
            double maxX,
            double minY,
            double minZ,
            double maxZ)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MinZ = minZ;
            MaxZ = maxZ;
        }

        public double MinX { get; }
        public double MaxX { get; }
        public double MinY { get; }
        public double MinZ { get; }
        public double MaxZ { get; }
    }

    private readonly struct TileCoverage
    {
        public TileCoverage(
            int zoom,
            int minX,
            int maxX,
            int minY,
            int maxY,
            double modelTileX,
            double modelTileY,
            double tileSizeMetres)
        {
            Zoom = zoom;
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
            ModelTileX = modelTileX;
            ModelTileY = modelTileY;
            TileSizeMetres = tileSizeMetres;
        }

        public int Zoom { get; }
        public int MinX { get; }
        public int MaxX { get; }
        public int MinY { get; }
        public int MaxY { get; }
        public double ModelTileX { get; }
        public double ModelTileY { get; }
        public double TileSizeMetres { get; }
        public int Width => MaxX - MinX + 1;
        public int Height => MaxY - MinY + 1;
        public int TileCount => Width * Height;
    }

    private void OnEnable()
    {
        ResolveGeoPositionExtractor();
        if (geoPositionExtractor != null)
        {
            geoPositionExtractor.GeoPositionApplied += HandleGeoPositionApplied;
        }
    }

    private void OnDisable()
    {
        if (geoPositionExtractor != null)
        {
            geoPositionExtractor.GeoPositionApplied -= HandleGeoPositionApplied;
        }

        if (loadRoutine != null)
        {
            StopCoroutine(loadRoutine);
            loadRoutine = null;
        }
    }

    private void OnDestroy()
    {
        ClearGeneratedMap();
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
            latitude is < -90d or > 90d ||
            longitude is < -180d or > 180d)
        {
            Debug.LogWarning("Cannot load the OSM map because the IFC position is invalid.");
            return;
        }

        if (loadRoutine != null)
        {
            StopCoroutine(loadRoutine);
        }

        ClearGeneratedMap();
        loadRoutine = StartCoroutine(
            BuildMap(modelRoot, latitude, longitude, elevation));
    }

    public static double LongitudeToTileX(double longitude, int zoom)
    {
        var tileCount = Math.Pow(2d, zoom);
        return (longitude + 180d) / 360d * tileCount;
    }

    public static double LatitudeToTileY(double latitude, int zoom)
    {
        var clampedLatitude = Math.Clamp(
            latitude,
            -MaximumMercatorLatitude,
            MaximumMercatorLatitude);
        var latitudeRadians = clampedLatitude * Math.PI / 180d;
        var tileCount = Math.Pow(2d, zoom);
        return (1d -
                Math.Asinh(Math.Tan(latitudeRadians)) / Math.PI) /
               2d *
               tileCount;
    }

    public static double GroundTileSizeMetres(double latitude, int zoom)
    {
        var clampedLatitude = Math.Clamp(
            latitude,
            -MaximumMercatorLatitude,
            MaximumMercatorLatitude);
        return EarthCircumferenceMetres *
               Math.Cos(clampedLatitude * Math.PI / 180d) /
               Math.Pow(2d, zoom);
    }

    private void HandleGeoPositionApplied(
        GameObject modelRoot,
        double latitude,
        double longitude,
        double elevation)
    {
        LoadMap(modelRoot, latitude, longitude, elevation);
    }

    private IEnumerator BuildMap(
        GameObject modelRoot,
        double latitude,
        double longitude,
        double elevation)
    {
        var georeference = modelRoot.GetComponentInParent<CesiumGeoreference>();
        if (georeference == null)
        {
            Debug.LogWarning(
                "Cannot create the OSM map because the IFC model is not under a CesiumGeoreference.");
            loadRoutine = null;
            yield break;
        }

        var bounds = CalculateModelBounds(modelRoot, georeference.transform);
        var coverage = CalculateCoverage(latitude, longitude, bounds);
        ActiveZoom = coverage.Zoom;
        RequestedTileCount = coverage.TileCount;
        LoadedTileCount = 0;

        mapRoot = new GameObject(
            $"OpenStreetMap z{coverage.Zoom} ({coverage.Width}x{coverage.Height})");
        mapRoot.transform.SetParent(georeference.transform, false);
        mapRoot.transform.localPosition = new Vector3(
            0f,
            (float)bounds.MinY + verticalOffsetMetres,
            0f);

        var tileMesh = CreateTileMesh();
        runtimeAssets.Add(tileMesh);
        var shader = FindMapShader();
        if (shader == null)
        {
            Debug.LogError("No compatible unlit shader is available for OSM tiles.");
            ClearGeneratedMap();
            loadRoutine = null;
            yield break;
        }

        if (showAttribution)
        {
            CreateAttribution();
        }

        for (var tileY = coverage.MinY; tileY <= coverage.MaxY; tileY++)
        {
            for (var tileX = coverage.MinX; tileX <= coverage.MaxX; tileX++)
            {
                var tileObject = CreateTileObject(
                    tileMesh,
                    shader,
                    coverage,
                    tileX,
                    tileY);
                var texture = default(Texture2D);
                yield return LoadTileTexture(
                    coverage.Zoom,
                    tileX,
                    tileY,
                    loadedTexture => texture = loadedTexture);

                if (texture != null && tileObject != null)
                {
                    ApplyTexture(tileObject, texture);
                    LoadedTileCount++;
                }

                TileProgressChanged?.Invoke(LoadedTileCount, RequestedTileCount);
            }
        }

        Debug.Log(
            $"Loaded {LoadedTileCount}/{RequestedTileCount} OpenStreetMap tiles " +
            $"at zoom {ActiveZoom} for IFC position {latitude:F8}, {longitude:F8}.");
        loadRoutine = null;
        MapLoaded?.Invoke(mapRoot);
    }

    private TileCoverage CalculateCoverage(
        double latitude,
        double longitude,
        LocalBounds bounds)
    {
        var highestZoom = Mathf.Clamp(preferredZoom, 1, 19);
        var lowestZoom = Mathf.Clamp(minimumZoom, 1, highestZoom);
        TileCoverage lastCoverage = default;

        for (var zoom = highestZoom; zoom >= lowestZoom; zoom--)
        {
            lastCoverage = CreateCoverage(latitude, longitude, bounds, zoom);
            if (lastCoverage.Width <= maxTilesPerAxis &&
                lastCoverage.Height <= maxTilesPerAxis &&
                lastCoverage.TileCount <= maxTotalTiles)
            {
                return lastCoverage;
            }
        }

        Debug.LogWarning(
            $"The IFC footprint needs {lastCoverage.TileCount} tiles at zoom " +
            $"{lastCoverage.Zoom}, above the configured limit of {maxTotalTiles}. " +
            "The map will be cropped around the IFC origin.");
        return CropCoverage(lastCoverage);
    }

    private TileCoverage CreateCoverage(
        double latitude,
        double longitude,
        LocalBounds bounds,
        int zoom)
    {
        var modelTileX = LongitudeToTileX(longitude, zoom);
        var modelTileY = LatitudeToTileY(latitude, zoom);
        var tileSize = GroundTileSizeMetres(latitude, zoom);
        var minX = (int)Math.Floor(modelTileX + bounds.MinX / tileSize) -
                   paddingTiles;
        var maxX = (int)Math.Floor(modelTileX + bounds.MaxX / tileSize) +
                   paddingTiles;
        var minY = (int)Math.Floor(modelTileY - bounds.MaxZ / tileSize) -
                   paddingTiles;
        var maxY = (int)Math.Floor(modelTileY - bounds.MinZ / tileSize) +
                   paddingTiles;
        return new TileCoverage(
            zoom,
            minX,
            maxX,
            minY,
            maxY,
            modelTileX,
            modelTileY,
            tileSize);
    }

    private TileCoverage CropCoverage(TileCoverage coverage)
    {
        var width = Math.Min(coverage.Width, maxTilesPerAxis);
        var height = Math.Min(coverage.Height, maxTilesPerAxis);
        while (width * height > maxTotalTiles)
        {
            if (width >= height && width > 1)
            {
                width--;
            }
            else if (height > 1)
            {
                height--;
            }
            else
            {
                break;
            }
        }

        var centreX = (int)Math.Floor(coverage.ModelTileX);
        var centreY = (int)Math.Floor(coverage.ModelTileY);
        var minX = centreX - width / 2;
        var minY = centreY - height / 2;
        return new TileCoverage(
            coverage.Zoom,
            minX,
            minX + width - 1,
            minY,
            minY + height - 1,
            coverage.ModelTileX,
            coverage.ModelTileY,
            coverage.TileSizeMetres);
    }

    private static LocalBounds CalculateModelBounds(
        GameObject modelRoot,
        Transform referenceFrame)
    {
        var hasBounds = false;
        var minX = 0d;
        var maxX = 0d;
        var minY = 0d;
        var minZ = 0d;
        var maxZ = 0d;

        foreach (var renderer in modelRoot.GetComponentsInChildren<Renderer>(true))
        {
            var bounds = renderer.bounds;
            for (var x = -1; x <= 1; x += 2)
            {
                for (var y = -1; y <= 1; y += 2)
                {
                    for (var z = -1; z <= 1; z += 2)
                    {
                        var worldPoint = bounds.center +
                                         Vector3.Scale(
                                             bounds.extents,
                                             new Vector3(x, y, z));
                        var localPoint = referenceFrame.InverseTransformPoint(worldPoint);
                        if (!hasBounds)
                        {
                            minX = maxX = localPoint.x;
                            minY = localPoint.y;
                            minZ = maxZ = localPoint.z;
                            hasBounds = true;
                        }
                        else
                        {
                            minX = Math.Min(minX, localPoint.x);
                            maxX = Math.Max(maxX, localPoint.x);
                            minY = Math.Min(minY, localPoint.y);
                            minZ = Math.Min(minZ, localPoint.z);
                            maxZ = Math.Max(maxZ, localPoint.z);
                        }
                    }
                }
            }
        }

        return hasBounds
            ? new LocalBounds(minX, maxX, minY, minZ, maxZ)
            : new LocalBounds(0d, 0d, 0d, 0d, 0d);
    }

    private GameObject CreateTileObject(
        Mesh tileMesh,
        Shader shader,
        TileCoverage coverage,
        int tileX,
        int tileY)
    {
        var tileObject = new GameObject($"OSM {coverage.Zoom}/{tileX}/{tileY}");
        tileObject.transform.SetParent(mapRoot.transform, false);
        tileObject.transform.localPosition = new Vector3(
            (float)((tileX + 0.5d - coverage.ModelTileX) *
                    coverage.TileSizeMetres),
            0f,
            (float)((coverage.ModelTileY - tileY - 0.5d) *
                    coverage.TileSizeMetres));
        tileObject.transform.localScale = new Vector3(
            (float)coverage.TileSizeMetres,
            1f,
            (float)coverage.TileSizeMetres);

        var meshFilter = tileObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = tileMesh;
        var renderer = tileObject.AddComponent<MeshRenderer>();
        var material = new Material(shader)
        {
            name = $"OSM {coverage.Zoom}/{tileX}/{tileY}",
            color = Color.white
        };
        renderer.sharedMaterial = material;
        runtimeAssets.Add(material);
        return tileObject;
    }

    private IEnumerator LoadTileTexture(
        int zoom,
        int tileX,
        int tileY,
        Action<Texture2D> completed)
    {
        var tileCount = 1 << zoom;
        var wrappedX = ((tileX % tileCount) + tileCount) % tileCount;
        if (tileY < 0 || tileY >= tileCount)
        {
            completed(null);
            yield break;
        }

        var cachePath = GetCachePath(zoom, wrappedX, tileY);
        if (TryLoadFreshCachedTexture(cachePath, out var cachedTexture))
        {
            runtimeAssets.Add(cachedTexture);
            completed(cachedTexture);
            yield break;
        }

        var url = tileUrlTemplate
            .Replace("{z}", zoom.ToString())
            .Replace("{x}", wrappedX.ToString())
            .Replace("{y}", tileY.ToString());
        using var request = UnityWebRequestTexture.GetTexture(url, true);
        if (!string.IsNullOrWhiteSpace(userAgent))
        {
            request.SetRequestHeader("User-Agent", userAgent);
        }

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            var texture = DownloadHandlerTexture.GetContent(request);
            ConfigureTexture(texture, $"OSM {zoom}/{wrappedX}/{tileY}");
            runtimeAssets.Add(texture);
            TryWriteCache(cachePath, request.downloadHandler.data);
            completed(texture);
            yield break;
        }

        if (TryLoadCachedTexture(cachePath, out var staleTexture))
        {
            runtimeAssets.Add(staleTexture);
            Debug.LogWarning(
                $"OSM tile {zoom}/{wrappedX}/{tileY} could not be refreshed; " +
                "using the cached copy.");
            completed(staleTexture);
            yield break;
        }

        Debug.LogWarning(
            $"OSM tile {zoom}/{wrappedX}/{tileY} failed: {request.error}");
        completed(null);
    }

    private void ApplyTexture(GameObject tileObject, Texture2D texture)
    {
        var material = tileObject.GetComponent<MeshRenderer>().sharedMaterial;
        material.mainTexture = texture;
        if (material.HasProperty("_BaseMap"))
        {
            material.SetTexture("_BaseMap", texture);
        }

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", Color.white);
        }
    }

    private static Mesh CreateTileMesh()
    {
        var mesh = new Mesh
        {
            name = "OSM Tile Mesh",
            vertices = new[]
            {
                new Vector3(-0.5f, 0f, -0.5f),
                new Vector3(0.5f, 0f, -0.5f),
                new Vector3(-0.5f, 0f, 0.5f),
                new Vector3(0.5f, 0f, 0.5f)
            },
            uv = new[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0f, 1f),
                new Vector2(1f, 1f)
            },
            triangles = new[] { 0, 2, 1, 2, 3, 1 }
        };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Shader FindMapShader()
    {
        return Shader.Find("Universal Render Pipeline/Unlit") ??
               Shader.Find("Unlit/Texture") ??
               Shader.Find("Sprites/Default");
    }

    private void CreateAttribution()
    {
        attributionObject = new GameObject("OpenStreetMap Attribution");
        attributionObject.transform.SetParent(transform, false);
        var canvas = attributionObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;

        var scaler = attributionObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 1f;

        var panelObject = new GameObject("Background");
        panelObject.transform.SetParent(attributionObject.transform, false);
        var panelRect = panelObject.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(1f, 0f);
        panelRect.anchorMax = new Vector2(1f, 0f);
        panelRect.pivot = new Vector2(1f, 0f);
        panelRect.anchoredPosition = new Vector2(-10f, 10f);
        panelRect.sizeDelta = new Vector2(330f, 28f);
        var panel = panelObject.AddComponent<Image>();
        panel.color = new Color(0f, 0f, 0f, 0.65f);
        panel.raycastTarget = false;

        var textObject = new GameObject("Text");
        textObject.transform.SetParent(panelObject.transform, false);
        var textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(8f, 2f);
        textRect.offsetMax = new Vector2(-8f, -2f);
        var label = textObject.AddComponent<TextMeshProUGUI>();
        label.text =
            "© OpenStreetMap contributors · openstreetmap.org/copyright";
        label.fontSize = 13f;
        label.color = Color.white;
        label.alignment = TextAlignmentOptions.MidlineRight;
        label.raycastTarget = false;
    }

    private static void ConfigureTexture(Texture2D texture, string textureName)
    {
        if (texture == null)
        {
            return;
        }

        texture.name = textureName;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        texture.anisoLevel = 2;
    }

    private static string GetCachePath(int zoom, int tileX, int tileY)
    {
        return Path.Combine(
            Application.temporaryCachePath,
            "OpenStreetMapTiles",
            zoom.ToString(),
            tileX.ToString(),
            $"{tileY}.png");
    }

    private static bool TryLoadFreshCachedTexture(
        string cachePath,
        out Texture2D texture)
    {
        texture = null;
        try
        {
            if (!File.Exists(cachePath) ||
                DateTime.UtcNow - File.GetLastWriteTimeUtc(cachePath) >=
                MinimumCacheLifetime)
            {
                return false;
            }
        }
        catch (Exception)
        {
            return false;
        }

        return TryLoadCachedTexture(cachePath, out texture);
    }

    private static bool TryLoadCachedTexture(
        string cachePath,
        out Texture2D texture)
    {
        texture = null;
        try
        {
            if (!File.Exists(cachePath))
            {
                return false;
            }

            var bytes = File.ReadAllBytes(cachePath);
            texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(bytes, true))
            {
                Destroy(texture);
                texture = null;
                return false;
            }

            ConfigureTexture(texture, Path.GetFileNameWithoutExtension(cachePath));
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Could not read cached OSM tile: {exception.Message}");
            return false;
        }
    }

    private static void TryWriteCache(string cachePath, byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
        {
            return;
        }

        try
        {
            var directory = Path.GetDirectoryName(cachePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllBytes(cachePath, bytes);
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Could not cache OSM tile: {exception.Message}");
        }
    }

    private void ResolveGeoPositionExtractor()
    {
        if (geoPositionExtractor == null)
        {
            geoPositionExtractor = GetComponent<IfcGeoPositionExtractor>();
        }
    }

    private void ClearGeneratedMap()
    {
        if (mapRoot != null)
        {
            DestroyRuntimeObject(mapRoot);
            mapRoot = null;
        }

        if (attributionObject != null)
        {
            DestroyRuntimeObject(attributionObject);
            attributionObject = null;
        }

        foreach (var runtimeAsset in runtimeAssets)
        {
            if (runtimeAsset != null)
            {
                DestroyRuntimeObject(runtimeAsset);
            }
        }

        runtimeAssets.Clear();
        LoadedTileCount = 0;
        RequestedTileCount = 0;
    }

    private static void DestroyRuntimeObject(UnityEngine.Object target)
    {
        if (Application.isPlaying)
        {
            Destroy(target);
        }
        else
        {
            DestroyImmediate(target);
        }
    }
}
