using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CesiumForUnity;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class OsmMapLoader : MonoBehaviour
{
    private enum TileState
    {
        Empty,
        Queued,
        Loading,
        Ready,
        Failed
    }

    private sealed class MapQuadNode
    {
        public MapQuadNode(OsmTileKey key, MapQuadNode parent)
        {
            Key = key;
            Parent = parent;
        }

        public readonly OsmTileKey Key;
        public readonly MapQuadNode Parent;
        public MapQuadNode[] Children;
        public TileVisual Visual;
        public Texture2D Texture;
        public UnityWebRequest ActiveRequest;
        public TileState State;
        public string FreshCachePath;
        public float LastVisibleTime;
        public float RetryAfterTime;
        public float RequestPriority;
        public bool Disposed;
    }

    private sealed class TileVisual
    {
        public GameObject GameObject;
        public MeshRenderer Renderer;
        public Material Material;
    }

    private static readonly TimeSpan DefaultCacheFreshness = TimeSpan.FromDays(7d);

    [Header("Source")]
    [SerializeField] private IfcGeoPositionExtractor geoPositionExtractor;
    [SerializeField] private Camera viewingCamera;
    [SerializeField] private string tileUrlTemplate =
        "https://tile.openstreetmap.org/{z}/{x}/{y}.png";
    [SerializeField] private string userAgent = "CauDuong-IFC-Viewer/1.0";

    [Header("Dynamic LOD")]
    [SerializeField, InspectorName("Maximum Zoom"), Range(1, 19)]
    private int preferredZoom = 18;
    [SerializeField, Range(1, 19)] private int minimumZoom = 12;
    [SerializeField, Range(0, 3)] private int rootTileRadius = 1;
    [SerializeField, Min(64f)] private float splitThresholdPixels = 520f;
    [SerializeField, Min(32f)] private float mergeThresholdPixels = 300f;
    [SerializeField, Range(32, 512)] private int maxResidentTiles = 192;
    [SerializeField, Range(0.05f, 1f)] private float evaluationInterval = 0.15f;
    [SerializeField, Min(0f)] private float offscreenRetentionSeconds = 2f;

    [Header("Streaming")]
    [SerializeField, Range(1, 8)] private int maxConcurrentRequests = 6;
    [SerializeField, Range(1, 4)] private int maxTextureFinalizationsPerFrame = 1;
    [SerializeField, Min(1f)] private float failedRequestRetrySeconds = 15f;

    [Header("Cache")]
    [SerializeField, Min(0f)] private float cacheFreshnessDays = 7f;
    [SerializeField, Min(1f)] private float cacheRetentionDays = 30f;

    [Header("Presentation")]
    [SerializeField] private float verticalOffsetMetres = -0.05f;
    [SerializeField, Min(0f)] private float skirtDepthMetres = 0.25f;
    [SerializeField] private bool showAttribution = true;

    private readonly Dictionary<OsmTileKey, MapQuadNode> rootNodes = new();
    private readonly List<MapQuadNode> requestQueue = new();
    private readonly List<OsmTileKey> rootsToRemove = new();
    private readonly HashSet<OsmTileKey> desiredRootKeys = new();
    private readonly Queue<TileVisual> visualPool = new();
    private readonly List<Material> runtimeMaterials = new();
    private readonly Plane[] frustumPlanes = new Plane[6];

    private Coroutine loadRoutine;
    private GameObject mapRoot;
    private GameObject attributionObject;
    private Mesh tileMesh;
    private Shader tileShader;
    private double originLatitude;
    private double originTileXAtMinimumZoom;
    private double originTileYAtMinimumZoom;
    private float nextEvaluationTime;
    private int streamGeneration;
    private int trackedTileCount;
    private int textureFinalizationsRemaining;
    private bool streaming;
    private bool mapLoadedRaised;

    public int ActiveZoom { get; private set; }
    public int LoadedTileCount { get; private set; }
    public int RequestedTileCount { get; private set; }
    public int PooledVisualCount => visualPool.Count;
    public int RootNodeCount => rootNodes.Count;

    public event Action<int, int> TileProgressChanged;
    public event Action<GameObject> MapLoaded;

    private readonly struct LocalModelBounds
    {
        public LocalModelBounds(double minY)
        {
            MinY = minY;
        }

        public double MinY { get; }
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

        ClearGeneratedMap();
    }

    private void OnDestroy()
    {
        ClearGeneratedMap();
    }

    private void Update()
    {
        textureFinalizationsRemaining = maxTextureFinalizationsPerFrame;
        if (!streaming || mapRoot == null || Time.unscaledTime < nextEvaluationTime)
        {
            return;
        }

        nextEvaluationTime = Time.unscaledTime + evaluationInterval;
        EvaluateStreaming();
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

        ClearGeneratedMap();
        loadRoutine = StartCoroutine(
            InitializeMap(modelRoot, latitude, longitude, elevation));
    }

    public void SetViewingCamera(Camera cameraToUse)
    {
        viewingCamera = cameraToUse;
    }

    public static void LatLonToTile(
        double latitude,
        double longitude,
        int zoom,
        out double tileX,
        out double tileY)
    {
        var coordinate = OsmTileMath.LatLonToTile(latitude, longitude, zoom);
        tileX = coordinate.X;
        tileY = coordinate.Y;
    }

    public static void TileToLatLon(
        double tileX,
        double tileY,
        int zoom,
        out double latitude,
        out double longitude)
    {
        OsmTileMath.TileToLatLon(
            tileX,
            tileY,
            zoom,
            out latitude,
            out longitude);
    }

    public static double LongitudeToTileX(double longitude, int zoom)
    {
        return OsmTileMath.LatLonToTile(0d, longitude, zoom).X;
    }

    public static double LatitudeToTileY(double latitude, int zoom)
    {
        return OsmTileMath.LatLonToTile(latitude, 0d, zoom).Y;
    }

    public static double GroundTileSizeMetres(double latitude, int zoom)
    {
        return OsmTileMath.GroundTileSizeMetres(latitude, zoom);
    }

    private void HandleGeoPositionApplied(
        GameObject modelRoot,
        double latitude,
        double longitude,
        double elevation)
    {
        LoadMap(modelRoot, latitude, longitude, elevation);
    }

    private IEnumerator InitializeMap(
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

        ResolveViewingCamera();
        if (viewingCamera == null)
        {
            Debug.LogWarning(
                "Cannot create the dynamic OSM map because no viewing camera is available.");
            loadRoutine = null;
            yield break;
        }

        tileShader = FindMapShader();
        if (tileShader == null)
        {
            Debug.LogError("No compatible unlit shader is available for OSM tiles.");
            loadRoutine = null;
            yield break;
        }

        var modelBounds = CalculateModelBounds(modelRoot, georeference.transform);
        originLatitude = latitude;
        var originTile = OsmTileMath.LatLonToTile(
            originLatitude,
            longitude,
            minimumZoom);
        originTileXAtMinimumZoom = originTile.X;
        originTileYAtMinimumZoom = originTile.Y;

        mapRoot = new GameObject(
            $"OpenStreetMap Dynamic z{minimumZoom}-{preferredZoom}");
        mapRoot.transform.SetParent(georeference.transform, false);
        mapRoot.transform.localPosition = new Vector3(
            0f,
            (float)modelBounds.MinY + verticalOffsetMetres,
            0f);

        tileMesh = CreateSkirtedTileMesh();
        if (showAttribution)
        {
            CreateAttribution();
        }

        StartCoroutine(PurgeExpiredCache());
        streamGeneration++;
        streaming = true;
        mapLoadedRaised = false;
        ActiveZoom = minimumZoom;
        nextEvaluationTime = 0f;
        for (var i = 0; i < maxConcurrentRequests; i++)
        {
            StartCoroutine(RequestWorker(streamGeneration));
        }

        EvaluateStreaming();
        Debug.Log(
            $"Started dynamic OpenStreetMap streaming at {latitude:F8}, " +
            $"{longitude:F8}, elevation {elevation:F3} m, zoom " +
            $"{minimumZoom}-{preferredZoom}.");
        loadRoutine = null;
        yield break;
    }

    private void EvaluateStreaming()
    {
        if (viewingCamera == null || !viewingCamera.isActiveAndEnabled)
        {
            ResolveViewingCamera();
            if (viewingCamera == null)
            {
                return;
            }
        }

        UpdateRootWindow();
        ActiveZoom = minimumZoom;
        GeometryUtility.CalculateFrustumPlanes(viewingCamera, frustumPlanes);
        foreach (var root in rootNodes.Values)
        {
            EvaluateNode(root);
        }
    }

    private void UpdateRootWindow()
    {
        var cameraLocal = mapRoot.transform.InverseTransformPoint(
            viewingCamera.transform.position);
        var rootTileSize = OsmTileMath.GroundTileSizeMetres(
            originLatitude,
            minimumZoom);
        var cameraTileX = originTileXAtMinimumZoom + cameraLocal.x / rootTileSize;
        var cameraTileY = originTileYAtMinimumZoom - cameraLocal.z / rootTileSize;
        var centerX = (int)Math.Floor(cameraTileX);
        var centerY = (int)Math.Floor(cameraTileY);

        desiredRootKeys.Clear();
        for (var y = centerY - rootTileRadius; y <= centerY + rootTileRadius; y++)
        {
            if (!OsmTileMath.IsValidTileY(y, minimumZoom))
            {
                continue;
            }

            for (var x = centerX - rootTileRadius; x <= centerX + rootTileRadius; x++)
            {
                var key = new OsmTileKey(minimumZoom, x, y);
                desiredRootKeys.Add(key);
                if (!rootNodes.ContainsKey(key))
                {
                    rootNodes.Add(key, new MapQuadNode(key, null));
                }
            }
        }

        rootsToRemove.Clear();
        foreach (var pair in rootNodes)
        {
            if (!desiredRootKeys.Contains(pair.Key))
            {
                rootsToRemove.Add(pair.Key);
            }
        }

        foreach (var key in rootsToRemove)
        {
            ReleaseNodeRecursive(rootNodes[key]);
            rootNodes.Remove(key);
        }
    }

    private void EvaluateNode(MapQuadNode node)
    {
        if (node == null || node.Disposed)
        {
            return;
        }

        var worldBounds = GetWorldBounds(node.Key);
        var visible = GeometryUtility.TestPlanesAABB(frustumPlanes, worldBounds);
        if (!visible)
        {
            SetSubtreeVisible(node, false);
            if (node.Children != null &&
                Time.unscaledTime - node.LastVisibleTime >= offscreenRetentionSeconds)
            {
                ReleaseChildren(node);
            }

            return;
        }

        node.LastVisibleTime = Time.unscaledTime;
        EnsureNodeRequested(node, worldBounds);
        if (node.State == TileState.Ready)
        {
            ActiveZoom = Mathf.Max(ActiveZoom, node.Key.Zoom);
        }

        var screenSize = CalculateScreenSizePixels(worldBounds);
        var canSplit = node.Key.Zoom < preferredZoom &&
                       screenSize >= splitThresholdPixels;

        if (canSplit && EnsureChildren(node))
        {
            foreach (var child in node.Children)
            {
                EnsureNodeRequested(child, GetWorldBounds(child.Key));
            }

            if (AreAllChildrenReady(node))
            {
                SetNodeVisible(node, false);
                foreach (var child in node.Children)
                {
                    EvaluateNode(child);
                }
            }
            else
            {
                SetNodeVisible(node, node.State == TileState.Ready);
                foreach (var child in node.Children)
                {
                    SetSubtreeVisible(child, false);
                }
            }

            return;
        }

        if (node.Children != null && screenSize <= mergeThresholdPixels)
        {
            ReleaseChildren(node);
        }

        if (node.Children != null && AreAllChildrenReady(node))
        {
            SetNodeVisible(node, false);
            foreach (var child in node.Children)
            {
                EvaluateNode(child);
            }
        }
        else
        {
            SetNodeVisible(node, node.State == TileState.Ready);
        }
    }

    private bool EnsureChildren(MapQuadNode node)
    {
        if (node.Children != null)
        {
            return true;
        }

        if (trackedTileCount + 4 > maxResidentTiles)
        {
            return false;
        }

        node.Children = new MapQuadNode[4];
        for (var i = 0; i < node.Children.Length; i++)
        {
            node.Children[i] = new MapQuadNode(node.Key.GetChild(i), node);
        }

        return true;
    }

    private void EnsureNodeRequested(MapQuadNode node, Bounds worldBounds)
    {
        if (node.State == TileState.Queued)
        {
            node.RequestPriority = CalculateRequestPriority(
                worldBounds,
                node.Key.Zoom);
            return;
        }

        if (node.State is TileState.Ready or TileState.Loading ||
            node.Disposed ||
            trackedTileCount >= maxResidentTiles ||
            node.State == TileState.Failed &&
            Time.unscaledTime < node.RetryAfterTime)
        {
            return;
        }

        var wrappedX = OsmTileMath.WrapTileX(node.Key.X, node.Key.Zoom);
        var cachePath = GetCachePath(node.Key.Zoom, wrappedX, node.Key.Y);
        node.FreshCachePath = IsCacheFresh(cachePath) ? cachePath : null;
        node.RequestPriority = CalculateRequestPriority(worldBounds, node.Key.Zoom);
        node.State = TileState.Queued;
        requestQueue.Add(node);
        trackedTileCount++;
        RequestedTileCount++;
        TileProgressChanged?.Invoke(LoadedTileCount, RequestedTileCount);
    }

    private IEnumerator RequestWorker(int generation)
    {
        while (streaming && generation == streamGeneration)
        {
            var node = DequeueHighestPriorityNode();
            if (node == null)
            {
                yield return null;
                continue;
            }

            if (node.Disposed || node.State != TileState.Queued)
            {
                continue;
            }

            node.State = TileState.Loading;
            yield return LoadNodeTexture(node, generation);
        }
    }

    private MapQuadNode DequeueHighestPriorityNode()
    {
        var bestIndex = -1;
        var bestPriority = float.PositiveInfinity;
        for (var i = requestQueue.Count - 1; i >= 0; i--)
        {
            var candidate = requestQueue[i];
            if (candidate == null ||
                candidate.Disposed ||
                candidate.State != TileState.Queued)
            {
                requestQueue.RemoveAt(i);
                continue;
            }

            if (candidate.RequestPriority < bestPriority)
            {
                bestPriority = candidate.RequestPriority;
                bestIndex = i;
            }
        }

        if (bestIndex < 0)
        {
            return null;
        }

        var node = requestQueue[bestIndex];
        requestQueue.RemoveAt(bestIndex);
        return node;
    }

    private IEnumerator LoadNodeTexture(MapQuadNode node, int generation)
    {
        Texture2D texture = null;
        if (!string.IsNullOrWhiteSpace(node.FreshCachePath))
        {
            yield return LoadCachedTextureAsync(
                node.FreshCachePath,
                loadedTexture => texture = loadedTexture);
        }

        if (texture == null &&
            !node.Disposed &&
            streaming &&
            generation == streamGeneration)
        {
            yield return DownloadTileTexture(
                node,
                loadedTexture => texture = loadedTexture);
        }

        if (node.Disposed || !streaming || generation != streamGeneration)
        {
            DestroyRuntimeObject(texture);
            yield break;
        }

        if (texture == null)
        {
            node.State = TileState.Failed;
            node.RetryAfterTime = Time.unscaledTime + failedRequestRetrySeconds;
            trackedTileCount = Mathf.Max(0, trackedTileCount - 1);
            yield break;
        }

        CompleteNodeLoad(node, texture);
    }

    private IEnumerator DownloadTileTexture(
        MapQuadNode node,
        Action<Texture2D> completed)
    {
        var wrappedX = OsmTileMath.WrapTileX(node.Key.X, node.Key.Zoom);
        if (!OsmTileMath.IsValidTileY(node.Key.Y, node.Key.Zoom))
        {
            completed(null);
            yield break;
        }

        var cachePath = GetCachePath(node.Key.Zoom, wrappedX, node.Key.Y);
        var url = tileUrlTemplate
            .Replace("{z}", node.Key.Zoom.ToString())
            .Replace("{x}", wrappedX.ToString())
            .Replace("{y}", node.Key.Y.ToString());
        using var request = UnityWebRequestTexture.GetTexture(url, true);
        node.ActiveRequest = request;
        request.timeout = 20;
        if (!string.IsNullOrWhiteSpace(userAgent))
        {
            request.SetRequestHeader("User-Agent", userAgent);
        }

        yield return request.SendWebRequest();
        node.ActiveRequest = null;
        if (node.Disposed)
        {
            completed(null);
            yield break;
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            yield return WaitForTextureFinalizeSlot();
            var texture = DownloadHandlerTexture.GetContent(request);
            ConfigureTexture(texture, $"OSM {node.Key.Zoom}/{wrappedX}/{node.Key.Y}");
            TryWriteCache(cachePath, request.downloadHandler.data);
            completed(texture);
            yield break;
        }

        if (File.Exists(cachePath))
        {
            yield return LoadCachedTextureAsync(
                cachePath,
                staleTexture =>
                {
                    if (staleTexture != null)
                    {
                        Debug.LogWarning(
                            $"OSM tile {node.Key} could not be refreshed; using cache.");
                    }

                    completed(staleTexture);
                });
            yield break;
        }

        if (request.result != UnityWebRequest.Result.ConnectionError ||
            !string.Equals(request.error, "Request aborted", StringComparison.OrdinalIgnoreCase))
        {
            Debug.LogWarning($"OSM tile {node.Key} failed: {request.error}");
        }

        completed(null);
    }

    private IEnumerator LoadCachedTextureAsync(
        string cachePath,
        Action<Texture2D> completed)
    {
        Task<byte[]> readTask;
        try
        {
            readTask = File.ReadAllBytesAsync(cachePath);
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Could not start OSM cache read: {exception.Message}");
            completed(null);
            yield break;
        }

        while (!readTask.IsCompleted)
        {
            yield return null;
        }

        if (readTask.IsFaulted || readTask.IsCanceled)
        {
            Debug.LogWarning($"Could not read cached OSM tile {cachePath}.");
            completed(null);
            yield break;
        }

        yield return WaitForTextureFinalizeSlot();
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!texture.LoadImage(readTask.Result, true))
        {
            DestroyRuntimeObject(texture);
            completed(null);
            yield break;
        }

        ConfigureTexture(texture, Path.GetFileNameWithoutExtension(cachePath));
        completed(texture);
    }

    private IEnumerator WaitForTextureFinalizeSlot()
    {
        while (textureFinalizationsRemaining <= 0)
        {
            yield return null;
        }

        textureFinalizationsRemaining--;
    }

    private void CompleteNodeLoad(MapQuadNode node, Texture2D texture)
    {
        node.Texture = texture;
        node.State = TileState.Ready;
        node.Visual = AcquireVisual(node);
        LoadedTileCount++;
        ActiveZoom = Mathf.Max(ActiveZoom, node.Key.Zoom);
        TileProgressChanged?.Invoke(LoadedTileCount, RequestedTileCount);

        if (!mapLoadedRaised)
        {
            mapLoadedRaised = true;
            MapLoaded?.Invoke(mapRoot);
        }
    }

    private TileVisual AcquireVisual(MapQuadNode node)
    {
        TileVisual visual;
        if (visualPool.Count > 0)
        {
            visual = visualPool.Dequeue();
        }
        else
        {
            visual = CreateVisual();
        }

        var localBounds = GetLocalBounds(node.Key);
        visual.GameObject.name = $"OSM {node.Key}";
        visual.GameObject.transform.SetParent(mapRoot.transform, false);
        visual.GameObject.transform.localPosition = localBounds.center;
        visual.GameObject.transform.localRotation = Quaternion.identity;
        visual.GameObject.transform.localScale = new Vector3(
            localBounds.size.x,
            Mathf.Max(0.001f, skirtDepthMetres),
            localBounds.size.z);
        visual.Material.mainTexture = node.Texture;
        if (visual.Material.HasProperty("_BaseMap"))
        {
            visual.Material.SetTexture("_BaseMap", node.Texture);
        }

        visual.GameObject.SetActive(true);
        visual.Renderer.enabled = false;
        return visual;
    }

    private TileVisual CreateVisual()
    {
        var tileObject = new GameObject("Pooled OSM Tile");
        tileObject.transform.SetParent(mapRoot.transform, false);
        var meshFilter = tileObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = tileMesh;
        var renderer = tileObject.AddComponent<MeshRenderer>();
        var material = new Material(tileShader)
        {
            name = "Runtime OSM Tile Material",
            color = Color.white
        };
        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", Color.white);
        }

        renderer.sharedMaterial = material;
        runtimeMaterials.Add(material);
        return new TileVisual
        {
            GameObject = tileObject,
            Renderer = renderer,
            Material = material
        };
    }

    private void ReleaseVisual(TileVisual visual)
    {
        if (visual == null || visual.GameObject == null)
        {
            return;
        }

        if (visual.Renderer != null)
        {
            visual.Renderer.enabled = false;
        }

        if (visual.Material != null)
        {
            visual.Material.mainTexture = null;
            if (visual.Material.HasProperty("_BaseMap"))
            {
                visual.Material.SetTexture("_BaseMap", null);
            }
        }

        visual.GameObject.SetActive(false);
        visualPool.Enqueue(visual);
    }

    private void ReleaseChildren(MapQuadNode node)
    {
        if (node.Children == null)
        {
            return;
        }

        foreach (var child in node.Children)
        {
            ReleaseNodeRecursive(child);
        }

        node.Children = null;
        SetNodeVisible(node, node.State == TileState.Ready);
    }

    private void ReleaseNodeRecursive(MapQuadNode node)
    {
        if (node == null || node.Disposed)
        {
            return;
        }

        node.Disposed = true;
        node.ActiveRequest?.Abort();
        if (node.Children != null)
        {
            foreach (var child in node.Children)
            {
                ReleaseNodeRecursive(child);
            }
        }

        requestQueue.Remove(node);
        if (node.State is TileState.Ready or TileState.Queued or TileState.Loading)
        {
            trackedTileCount = Mathf.Max(0, trackedTileCount - 1);
        }

        if (node.State == TileState.Ready)
        {
            LoadedTileCount = Mathf.Max(0, LoadedTileCount - 1);
        }

        ReleaseVisual(node.Visual);
        node.Visual = null;
        DestroyRuntimeObject(node.Texture);
        node.Texture = null;
        node.State = TileState.Empty;
    }

    private void SetSubtreeVisible(MapQuadNode node, bool visible)
    {
        if (node == null)
        {
            return;
        }

        SetNodeVisible(node, visible && node.State == TileState.Ready);
        if (node.Children == null)
        {
            return;
        }

        foreach (var child in node.Children)
        {
            SetSubtreeVisible(child, visible);
        }
    }

    private static void SetNodeVisible(MapQuadNode node, bool visible)
    {
        if (node?.Visual != null)
        {
            node.Visual.Renderer.enabled = visible;
        }
    }

    private static bool AreAllChildrenReady(MapQuadNode node)
    {
        if (node.Children == null || node.Children.Length != 4)
        {
            return false;
        }

        foreach (var child in node.Children)
        {
            if (child.State != TileState.Ready)
            {
                return false;
            }
        }

        return true;
    }

    private Bounds GetLocalBounds(OsmTileKey key)
    {
        var scale = Math.Pow(2d, key.Zoom - minimumZoom);
        return OsmTileMath.GetLocalBounds(
            key,
            originTileXAtMinimumZoom * scale,
            originTileYAtMinimumZoom * scale,
            originLatitude,
            Mathf.Max(2f, skirtDepthMetres * 2f));
    }

    private Bounds GetWorldBounds(OsmTileKey key)
    {
        var localBounds = GetLocalBounds(key);
        var matrix = mapRoot.transform.localToWorldMatrix;
        var localExtents = localBounds.extents;
        var worldExtents = new Vector3(
            Mathf.Abs(matrix.m00) * localExtents.x +
            Mathf.Abs(matrix.m01) * localExtents.y +
            Mathf.Abs(matrix.m02) * localExtents.z,
            Mathf.Abs(matrix.m10) * localExtents.x +
            Mathf.Abs(matrix.m11) * localExtents.y +
            Mathf.Abs(matrix.m12) * localExtents.z,
            Mathf.Abs(matrix.m20) * localExtents.x +
            Mathf.Abs(matrix.m21) * localExtents.y +
            Mathf.Abs(matrix.m22) * localExtents.z);
        return new Bounds(
            mapRoot.transform.TransformPoint(localBounds.center),
            worldExtents * 2f);
    }

    private float CalculateScreenSizePixels(Bounds worldBounds)
    {
        var worldSize = Mathf.Max(worldBounds.size.x, worldBounds.size.z);
        if (viewingCamera.orthographic)
        {
            return worldSize /
                   Mathf.Max(0.01f, viewingCamera.orthographicSize * 2f) *
                   viewingCamera.pixelHeight;
        }

        var closestPoint = worldBounds.ClosestPoint(viewingCamera.transform.position);
        var distance = Mathf.Max(
            0.01f,
            Vector3.Distance(viewingCamera.transform.position, closestPoint));
        var verticalScale = 2f * distance *
                            Mathf.Tan(viewingCamera.fieldOfView *
                                      0.5f *
                                      Mathf.Deg2Rad);
        return worldSize / Mathf.Max(0.01f, verticalScale) *
               viewingCamera.pixelHeight;
    }

    private float CalculateRequestPriority(Bounds worldBounds, int zoom)
    {
        var viewport = viewingCamera.WorldToViewportPoint(worldBounds.center);
        var screenDistance = new Vector2(
            viewport.x - 0.5f,
            viewport.y - 0.5f).sqrMagnitude;
        return screenDistance - zoom * 0.01f;
    }

    private static LocalModelBounds CalculateModelBounds(
        GameObject modelRoot,
        Transform referenceFrame)
    {
        var hasBounds = false;
        var minY = 0d;
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
                            minY = localPoint.y;
                            hasBounds = true;
                        }
                        else
                        {
                            minY = Math.Min(minY, localPoint.y);
                        }
                    }
                }
            }
        }

        return new LocalModelBounds(hasBounds ? minY : 0d);
    }

    private static Mesh CreateSkirtedTileMesh()
    {
        var vertices = new[]
        {
            new Vector3(-0.5f, 0f, -0.5f),
            new Vector3(0.5f, 0f, -0.5f),
            new Vector3(-0.5f, 0f, 0.5f),
            new Vector3(0.5f, 0f, 0.5f),
            new Vector3(-0.5f, 0f, -0.5f),
            new Vector3(0.5f, 0f, -0.5f),
            new Vector3(-0.5f, -1f, -0.5f),
            new Vector3(0.5f, -1f, -0.5f),
            new Vector3(0.5f, 0f, -0.5f),
            new Vector3(0.5f, 0f, 0.5f),
            new Vector3(0.5f, -1f, -0.5f),
            new Vector3(0.5f, -1f, 0.5f),
            new Vector3(0.5f, 0f, 0.5f),
            new Vector3(-0.5f, 0f, 0.5f),
            new Vector3(0.5f, -1f, 0.5f),
            new Vector3(-0.5f, -1f, 0.5f),
            new Vector3(-0.5f, 0f, 0.5f),
            new Vector3(-0.5f, 0f, -0.5f),
            new Vector3(-0.5f, -1f, 0.5f),
            new Vector3(-0.5f, -1f, -0.5f)
        };
        var uv = new[]
        {
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(1f, 0f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(1f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(0f, 0f),
            new Vector2(0f, 1f),
            new Vector2(0f, 0f)
        };
        var triangles = new[]
        {
            0, 2, 1, 2, 3, 1,
            4, 6, 5, 5, 6, 7,
            8, 10, 9, 9, 10, 11,
            12, 14, 13, 13, 14, 15,
            16, 18, 17, 17, 18, 19
        };
        var mesh = new Mesh
        {
            name = "OSM Skirted Tile Mesh",
            vertices = vertices,
            uv = uv,
            triangles = triangles
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
        panelRect.sizeDelta = new Vector2(360f, 28f);
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
        label.text = "(c) OpenStreetMap contributors - openstreetmap.org/copyright";
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

    private static string GetCacheRoot()
    {
        return Path.Combine(Application.persistentDataPath, "OpenStreetMapTiles");
    }

    private static string GetCachePath(int zoom, int tileX, int tileY)
    {
        return Path.Combine(
            GetCacheRoot(),
            zoom.ToString(),
            tileX.ToString(),
            $"{tileY}.png");
    }

    private bool IsCacheFresh(string cachePath)
    {
        try
        {
            var freshness = cacheFreshnessDays <= 0f
                ? DefaultCacheFreshness
                : TimeSpan.FromDays(cacheFreshnessDays);
            return File.Exists(cachePath) &&
                   DateTime.UtcNow - File.GetLastWriteTimeUtc(cachePath) < freshness;
        }
        catch (Exception)
        {
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

    private IEnumerator PurgeExpiredCache()
    {
        var cacheRoot = GetCacheRoot();
        if (!Directory.Exists(cacheRoot))
        {
            yield break;
        }

        var cutoff = DateTime.UtcNow - TimeSpan.FromDays(cacheRetentionDays);
        var inspected = 0;
        IEnumerable<string> files;
        try
        {
            files = Directory.EnumerateFiles(
                cacheRoot,
                "*.png",
                SearchOption.AllDirectories);
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Could not inspect the OSM cache: {exception.Message}");
            yield break;
        }

        foreach (var file in files)
        {
            try
            {
                if (File.GetLastWriteTimeUtc(file) < cutoff)
                {
                    File.Delete(file);
                }
            }
            catch (Exception)
            {
                // A locked cache entry can be retried on the next map load.
            }

            inspected++;
            if (inspected % 64 == 0)
            {
                yield return null;
            }
        }
    }

    private void ResolveGeoPositionExtractor()
    {
        if (geoPositionExtractor == null)
        {
            geoPositionExtractor = GetComponent<IfcGeoPositionExtractor>();
        }
    }

    private void ResolveViewingCamera()
    {
        if (viewingCamera == null || !viewingCamera.isActiveAndEnabled)
        {
            viewingCamera = Camera.main;
        }
    }

    private void StopStreaming()
    {
        streaming = false;
        streamGeneration++;
        foreach (var root in rootNodes.Values)
        {
            AbortRequestsRecursive(root);
        }

        StopAllCoroutines();
        loadRoutine = null;
        requestQueue.Clear();
    }

    private static void AbortRequestsRecursive(MapQuadNode node)
    {
        node?.ActiveRequest?.Abort();
        if (node?.Children == null)
        {
            return;
        }

        foreach (var child in node.Children)
        {
            AbortRequestsRecursive(child);
        }
    }

    private void ClearGeneratedMap()
    {
        StopStreaming();
        foreach (var root in rootNodes.Values)
        {
            ReleaseNodeRecursive(root);
        }

        rootNodes.Clear();
        desiredRootKeys.Clear();
        rootsToRemove.Clear();
        requestQueue.Clear();
        visualPool.Clear();
        trackedTileCount = 0;

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

        foreach (var material in runtimeMaterials)
        {
            DestroyRuntimeObject(material);
        }

        runtimeMaterials.Clear();
        DestroyRuntimeObject(tileMesh);
        tileMesh = null;
        LoadedTileCount = 0;
        RequestedTileCount = 0;
        ActiveZoom = minimumZoom;
    }

    private static void DestroyRuntimeObject(UnityEngine.Object target)
    {
        if (target == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(target);
        }
        else
        {
            DestroyImmediate(target);
        }
    }

    private void OnValidate()
    {
        minimumZoom = Mathf.Clamp(minimumZoom, 1, 19);
        preferredZoom = Mathf.Clamp(preferredZoom, minimumZoom, 19);
        mergeThresholdPixels = Mathf.Max(32f, mergeThresholdPixels);
        splitThresholdPixels = Mathf.Max(
            mergeThresholdPixels + 32f,
            splitThresholdPixels);
        maxResidentTiles = Mathf.Max(32, maxResidentTiles);
        maxConcurrentRequests = Mathf.Clamp(maxConcurrentRequests, 1, 8);
        maxTextureFinalizationsPerFrame = Mathf.Clamp(
            maxTextureFinalizationsPerFrame,
            1,
            4);
        cacheRetentionDays = Mathf.Max(cacheFreshnessDays, cacheRetentionDays);
    }
}
