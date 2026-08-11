using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;

internal sealed class IfcStreamMeshRecord
{
    public long PayloadOffset;
    public Vector3Int Cell;
    public Bounds LocalBounds;
    public int TriangleCount;
    public int VertexCount;
    public int IndexCount;
    public int StyleLabel;
    public int ProductLabel;
    public long EstimatedResidentBytes;
    public bool IsOverview;

    public bool Loading;
    public bool Resident;
    public bool Failed;
    public bool BudgetReserved;
    public bool UnloadQueued;
    public GameObject RuntimeObject;
    public Mesh Mesh;
    public MeshRenderer Renderer;
    public Vector3[] SelectionVertices;
    public int[][] SelectionSubMeshes;
    public int[] TriangleProductLabels;
}

public readonly struct IfcStreamedElementSummary
{
    public Bounds LocalBounds { get; }
    public long TriangleCount { get; }
    public long VertexCount { get; }
    public long IndexCount { get; }
    public Color Color { get; }

    internal IfcStreamedElementSummary(
        Bounds localBounds,
        long triangleCount,
        long vertexCount,
        long indexCount,
        Color color)
    {
        LocalBounds = localBounds;
        TriangleCount = triangleCount;
        VertexCount = vertexCount;
        IndexCount = indexCount;
        Color = color;
    }
}

internal readonly struct IfcOverviewRaycastHit
{
    public Vector3 Point { get; }
    public Vector3 Normal { get; }
    public float Distance { get; }
    public IfcElementMetadata Metadata { get; }

    public IfcOverviewRaycastHit(
        Vector3 point,
        Vector3 normal,
        float distance,
        IfcElementMetadata metadata)
    {
        Point = point;
        Normal = normal;
        Distance = distance;
        Metadata = metadata;
    }
}

internal readonly struct IfcStreamingSettings
{
    public float CellSizeMetres { get; }
    public float LoadDistanceMetres { get; }
    public float UnloadDistanceMetres { get; }
    public float InvisibleRetentionSeconds { get; }
    public float EvaluationIntervalSeconds { get; }
    public long MaximumResidentTriangles { get; }
    public long MaximumResidentBytes { get; }
    public int MaximumResidentRenderers { get; }
    public int MeshLoadsPerFrame { get; }
    public bool GenerateMeshColliders { get; }
    public int MaximumMeshColliderTriangles { get; }
    public bool ImportTextureCoordinates { get; }
    public bool ImportTangents { get; }
    public bool ReleaseCpuMeshData { get; }
    public bool EnableDiagnostics { get; }
    public float DiagnosticsIntervalSeconds { get; }

    public IfcStreamingSettings(
        float cellSizeMetres,
        float loadDistanceMetres,
        float unloadDistanceMetres,
        float invisibleRetentionSeconds,
        float evaluationIntervalSeconds,
        long maximumResidentTriangles,
        long maximumResidentBytes,
        int maximumResidentRenderers,
        int meshLoadsPerFrame,
        bool generateMeshColliders,
        int maximumMeshColliderTriangles,
        bool importTextureCoordinates,
        bool importTangents,
        bool releaseCpuMeshData,
        bool enableDiagnostics,
        float diagnosticsIntervalSeconds)
    {
        CellSizeMetres = Mathf.Max(1f, cellSizeMetres);
        LoadDistanceMetres = Mathf.Max(CellSizeMetres, loadDistanceMetres);
        UnloadDistanceMetres = Mathf.Max(LoadDistanceMetres, unloadDistanceMetres);
        InvisibleRetentionSeconds = Mathf.Max(0f, invisibleRetentionSeconds);
        EvaluationIntervalSeconds = Mathf.Max(0.05f, evaluationIntervalSeconds);
        MaximumResidentTriangles = Math.Max(1_000L, maximumResidentTriangles);
        MaximumResidentBytes = Math.Max(16L * 1024L * 1024L, maximumResidentBytes);
        MaximumResidentRenderers = Mathf.Max(1, maximumResidentRenderers);
        MeshLoadsPerFrame = Mathf.Max(1, meshLoadsPerFrame);
        GenerateMeshColliders = generateMeshColliders;
        MaximumMeshColliderTriangles = Mathf.Max(1_000, maximumMeshColliderTriangles);
        ImportTextureCoordinates = importTextureCoordinates;
        ImportTangents = importTangents;
        ReleaseCpuMeshData = releaseCpuMeshData;
        EnableDiagnostics = enableDiagnostics;
        DiagnosticsIntervalSeconds = Mathf.Max(1f, diagnosticsIntervalSeconds);
    }
}

[DisallowMultipleComponent]
public sealed class IfcStreamedModel : MonoBehaviour
{
    private const int BinaryItemsPerYield = 16_384;
    private const int MaximumArrayCount = 100_000_000;
    private const int MaximumPooledFragments = 256;
    private const int MaximumUnloadsPerFrame = 64;
    private const double MaximumUnloadMillisecondsPerFrame = 2.5d;
    private const float ForcedElementSeconds = 8f;
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly ProfilerMarker EvaluateMarker =
        new("IFC.Streaming.Evaluate");
    private static readonly ProfilerMarker LoadMarker =
        new("IFC.Streaming.LoadFragment");

    private sealed class ElementState
    {
        public readonly List<IfcStreamMeshRecord> Records = new();
        public readonly List<Renderer> Renderers = new();
        public Bounds LocalBounds;
        public long TriangleCount;
        public long VertexCount;
        public long IndexCount;
        public Color Color = Color.white;
        public bool HasBounds;
        public bool Visible = true;
        public bool Highlighted;
    }

    private sealed class CellState
    {
        public Vector3Int Coordinate;
        public readonly List<IfcStreamMeshRecord> Records = new();
        public Bounds LocalBounds;
        public bool HasBounds;
        public bool Queued;
        public float LastDesiredTime = float.NegativeInfinity;
        public float ForcedUntil;
        public int ResidentRecordCount;
    }

    private readonly struct CellCandidate
    {
        public CellState Cell { get; }
        public float SquaredDistance { get; }

        public CellCandidate(CellState cell, float squaredDistance)
        {
            Cell = cell;
            SquaredDistance = squaredDistance;
        }
    }

    private sealed class MeshLoadData
    {
        public string Name;
        public int ProductLabel;
        public Vector3[] Vertices;
        public Vector3[] Normals;
        public Vector2[] Uvs;
        public Vector4[] Tangents;
        public int[][] SubMeshes;
        public int[] StyleLabels;
        public int[] TriangleProductLabels;
        public double DecodeMilliseconds;
    }

    private readonly struct PendingUnload
    {
        public IfcStreamMeshRecord Record { get; }
        public CellState Cell { get; }

        public PendingUnload(IfcStreamMeshRecord record, CellState cell)
        {
            Record = record;
            Cell = cell;
        }
    }

    private static readonly HashSet<IfcStreamedModel> ActiveModels = new();

    private readonly Dictionary<Vector3Int, CellState> cells = new();
    private readonly Dictionary<int, ElementState> elements = new();
    private readonly HashSet<CellState> desiredCells = new();
    private readonly HashSet<CellState> residentCells = new();
    private readonly HashSet<CellState> forcedCells = new();
    private readonly List<CellState> unloadScratch = new();
    private readonly List<CellCandidate> candidateScratch = new();
    private readonly Queue<CellState> pendingCells = new();
    private readonly Queue<PendingUnload> pendingUnloads = new();
    private readonly Stack<GameObject> fragmentPool = new();
    private readonly Plane[] frustumPlanes = new Plane[6];
    private MaterialPropertyBlock highlightBlock;

    private IReadOnlyDictionary<int, Transform> parentsByProduct;
    private IReadOnlyDictionary<int, Material> materialsByStyle;
    private IReadOnlyList<IfcStreamMeshRecord> records;
    private IReadOnlyList<IfcStreamMeshRecord> overviewRecords;
    private IfcStreamingSettings settings;
    private string cachePath;
    private double metresPerUnit;
    private Camera viewingCamera;
    private Coroutine loadRoutine;
    private Coroutine overviewLoadRoutine;
    private CancellationTokenSource lifetimeCancellation;
    private float nextEvaluationTime;
    private float nextDiagnosticsTime;
    private float nextBudgetRetryTime;
    private long residentTriangles;
    private long residentBytes;
    private int residentRenderers;
    private int loadedFragmentCount;
    private int unloadedFragmentCount;
    private int cancelledFragmentLoadCount;
    private long overviewResidentTriangles;
    private int overviewResidentRenderers;
    private bool overviewRequested;
    private double totalDecodeMilliseconds;
    private double maximumDecodeMilliseconds;
    private double totalMeshBuildMilliseconds;
    private double maximumMeshBuildMilliseconds;
    private Bounds modelLocalBounds;
    private bool hasModelBounds;
    private bool registeredWithGlobalBudget;

    public bool IsInitialized { get; private set; }
    public string CachePath => cachePath ?? string.Empty;
    public int CellCount => cells.Count;
    public int FragmentCount => records?.Count ?? 0;
    public long TotalTriangleCount { get; private set; }
    public long ResidentTriangleCount => residentTriangles;
    public long EstimatedResidentBytes => residentBytes;
    public int ResidentRendererCount => residentRenderers;
    public long OverviewTriangleCount { get; private set; }
    public long OverviewResidentTriangleCount => overviewResidentTriangles;

    private void Awake()
    {
        // MaterialPropertyBlock creates a native Unity object internally, so it
        // must not be constructed by a MonoBehaviour field initializer.
        highlightBlock = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        ActiveModels.Add(this);
    }

    private void OnDisable()
    {
        ActiveModels.Remove(this);
    }

    internal void Initialize(
        string geometryCachePath,
        double modelMetresPerUnit,
        IReadOnlyList<IfcStreamMeshRecord> meshRecords,
        IReadOnlyList<IfcStreamMeshRecord> surfaceOverviewRecords,
        IReadOnlyDictionary<int, Transform> productParents,
        IReadOnlyDictionary<int, Material> styleMaterials,
        IfcStreamingSettings streamingSettings)
    {
        highlightBlock ??= new MaterialPropertyBlock();
        ReleaseAll();
        lifetimeCancellation = new CancellationTokenSource();
        cachePath = geometryCachePath;
        metresPerUnit = modelMetresPerUnit;
        records = meshRecords;
        overviewRecords = surfaceOverviewRecords;
        parentsByProduct = productParents;
        materialsByStyle = styleMaterials;
        settings = streamingSettings;

        foreach (var record in records)
        {
            if (!cells.TryGetValue(record.Cell, out var cell))
            {
                cell = new CellState { Coordinate = record.Cell };
                cells.Add(record.Cell, cell);
            }

            cell.Records.Add(record);
            Encapsulate(ref cell.LocalBounds, ref cell.HasBounds, record.LocalBounds);

            if (!elements.TryGetValue(record.ProductLabel, out var element))
            {
                element = new ElementState();
                elements.Add(record.ProductLabel, element);
            }

            element.Records.Add(record);
            Encapsulate(ref element.LocalBounds, ref element.HasBounds, record.LocalBounds);
            element.TriangleCount += record.TriangleCount;
            element.VertexCount += record.VertexCount;
            element.IndexCount += record.IndexCount;
            Encapsulate(ref modelLocalBounds, ref hasModelBounds, record.LocalBounds);
            if (element.Records.Count == 1 &&
                styleMaterials.TryGetValue(record.StyleLabel, out var material))
            {
                element.Color = ReadMaterialColor(material);
            }

            TotalTriangleCount += record.TriangleCount;
        }

        foreach (var overviewRecord in overviewRecords)
        {
            OverviewTriangleCount += overviewRecord.TriangleCount;
        }

        GlobalBudget.RegisterModel();
        registeredWithGlobalBudget = true;
        IsInitialized = true;
        nextEvaluationTime = 0f;
        nextDiagnosticsTime = Time.unscaledTime + settings.DiagnosticsIntervalSeconds;
        Debug.Log(
            $"IFC streaming index ready for '{name}': {CellCount:N0} cells, " +
            $"{FragmentCount:N0} detail fragments, {TotalTriangleCount:N0} detail " +
            $"triangles and {OverviewTriangleCount:N0} surface-overview triangles on disk.");
    }

    public bool TryGetElementSummary(
        int productLabel,
        out IfcStreamedElementSummary summary)
    {
        if (elements.TryGetValue(productLabel, out var element) && element.HasBounds)
        {
            summary = new IfcStreamedElementSummary(
                element.LocalBounds,
                element.TriangleCount,
                element.VertexCount,
                element.IndexCount,
                element.Color);
            return true;
        }

        summary = default;
        return false;
    }

    public List<Renderer> GetElementRenderers(int productLabel)
    {
        return elements.TryGetValue(productLabel, out var element)
            ? element.Renderers
            : null;
    }

    public bool TryGetElementWorldBounds(int productLabel, out Bounds bounds)
    {
        if (elements.TryGetValue(productLabel, out var element) && element.HasBounds)
        {
            bounds = TransformBounds(transform, element.LocalBounds);
            return true;
        }

        bounds = default;
        return false;
    }

    public void SetElementVisible(int productLabel, bool visible)
    {
        if (!elements.TryGetValue(productLabel, out var element))
        {
            return;
        }

        element.Visible = visible;
        foreach (var renderer in element.Renderers)
        {
            if (renderer != null)
            {
                renderer.enabled = visible;
            }
        }

        if (visible)
        {
            nextEvaluationTime = 0f;
        }
    }

    public void SetElementHighlighted(int productLabel, bool highlighted)
    {
        if (!elements.TryGetValue(productLabel, out var element))
        {
            return;
        }

        element.Highlighted = highlighted;
        foreach (var renderer in element.Renderers)
        {
            ApplyHighlight(renderer, highlighted);
        }

        if (highlighted)
        {
            RequestElement(productLabel);
        }
    }

    public void RequestElement(int productLabel)
    {
        if (!elements.TryGetValue(productLabel, out var element) ||
            element.Records.Count == 0)
        {
            return;
        }

        viewingCamera ??= Camera.main ?? FindFirstObjectByType<Camera>();
        var localCamera = viewingCamera != null
            ? transform.InverseTransformPoint(viewingCamera.transform.position)
            : Vector3.zero;
        var selected = element.Records[0];
        var selectedDistance = selected.LocalBounds.SqrDistance(localCamera);
        foreach (var record in element.Records)
        {
            if (record.Renderer != null)
            {
                selected = record;
                selectedDistance = 0f;
                break;
            }

            var distance = record.LocalBounds.SqrDistance(localCamera);
            if (distance < selectedDistance)
            {
                selected = record;
                selectedDistance = distance;
            }
        }

        if (cells.TryGetValue(selected.Cell, out var cell))
        {
            cell.ForcedUntil = Mathf.Max(
                cell.ForcedUntil,
                Time.unscaledTime + ForcedElementSeconds);
            forcedCells.Add(cell);
            QueueCell(cell);
            StartLoadingIfNeeded();
        }
    }

    internal static bool TryRaycastSurfaceOverview(
        Ray worldRay,
        out IfcOverviewRaycastHit overviewHit)
    {
        overviewHit = default;
        var found = false;
        var nearestDistance = float.PositiveInfinity;
        foreach (var model in ActiveModels)
        {
            if (model == null ||
                !model.IsInitialized ||
                !model.overviewRequested ||
                model.overviewRecords == null)
            {
                continue;
            }

            var localOrigin = model.transform.InverseTransformPoint(worldRay.origin);
            var localPoint = model.transform.InverseTransformPoint(
                worldRay.origin + worldRay.direction);
            var localDirection = (localPoint - localOrigin).normalized;
            if (localDirection.sqrMagnitude <= Mathf.Epsilon)
            {
                continue;
            }

            var localRay = new Ray(localOrigin, localDirection);
            foreach (var record in model.overviewRecords)
            {
                if (!record.Resident ||
                    record.Renderer == null ||
                    !record.Renderer.enabled ||
                    record.SelectionVertices == null ||
                    record.SelectionSubMeshes == null ||
                    record.TriangleProductLabels == null ||
                    !record.LocalBounds.IntersectRay(localRay))
                {
                    continue;
                }

                var triangleOffset = 0;
                foreach (var indices in record.SelectionSubMeshes)
                {
                    for (var index = 0; index + 2 < indices.Length; index += 3)
                    {
                        if (triangleOffset >= record.TriangleProductLabels.Length)
                        {
                            break;
                        }

                        var productLabel =
                            record.TriangleProductLabels[triangleOffset++];
                        if (!TryIntersectTriangle(
                                localOrigin,
                                localDirection,
                                record.SelectionVertices[indices[index]],
                                record.SelectionVertices[indices[index + 1]],
                                record.SelectionVertices[indices[index + 2]],
                                out var localDistance,
                                out var localNormal))
                        {
                            continue;
                        }

                        var worldPoint = model.transform.TransformPoint(
                            localOrigin + localDirection * localDistance);
                        var worldDistance = Vector3.Distance(
                            worldRay.origin,
                            worldPoint);
                        if (worldDistance >= nearestDistance ||
                            !model.parentsByProduct.TryGetValue(
                                productLabel,
                                out var productTransform) ||
                            productTransform == null ||
                            !productTransform.TryGetComponent<IfcElementMetadata>(
                                out var metadata))
                        {
                            continue;
                        }

                        var worldNormal = model.transform
                            .TransformDirection(localNormal)
                            .normalized;
                        if (Vector3.Dot(worldNormal, worldRay.direction) > 0f)
                        {
                            worldNormal = -worldNormal;
                        }

                        nearestDistance = worldDistance;
                        overviewHit = new IfcOverviewRaycastHit(
                            worldPoint,
                            worldNormal,
                            worldDistance,
                            metadata);
                        found = true;
                    }
                }
            }
        }

        return found;
    }

    private static bool TryIntersectTriangle(
        Vector3 rayOrigin,
        Vector3 rayDirection,
        Vector3 vertex0,
        Vector3 vertex1,
        Vector3 vertex2,
        out float distance,
        out Vector3 normal)
    {
        distance = 0f;
        normal = default;
        var edge1 = vertex1 - vertex0;
        var edge2 = vertex2 - vertex0;
        var cross = Vector3.Cross(rayDirection, edge2);
        var determinant = Vector3.Dot(edge1, cross);
        if (Mathf.Abs(determinant) < 0.000001f)
        {
            return false;
        }

        var inverseDeterminant = 1f / determinant;
        var originOffset = rayOrigin - vertex0;
        var u = Vector3.Dot(originOffset, cross) * inverseDeterminant;
        if (u < 0f || u > 1f)
        {
            return false;
        }

        var secondCross = Vector3.Cross(originOffset, edge1);
        var v = Vector3.Dot(rayDirection, secondCross) * inverseDeterminant;
        if (v < 0f || u + v > 1f)
        {
            return false;
        }

        distance = Vector3.Dot(edge2, secondCross) * inverseDeterminant;
        if (distance < 0f)
        {
            return false;
        }

        normal = Vector3.Cross(edge1, edge2).normalized;
        return true;
    }

    private void Update()
    {
        if (!IsInitialized)
        {
            return;
        }

        if (Time.unscaledTime >= nextEvaluationTime)
        {
            EvaluateStreaming();
            nextEvaluationTime = Time.unscaledTime + settings.EvaluationIntervalSeconds;
        }

        ProcessPendingUnloads();
        StartLoadingIfNeeded();
        StartOverviewLoadingIfNeeded();
        if (settings.EnableDiagnostics && Time.unscaledTime >= nextDiagnosticsTime)
        {
            nextDiagnosticsTime = Time.unscaledTime + settings.DiagnosticsIntervalSeconds;
            LogDiagnostics();
        }
    }

    private void EvaluateStreaming()
    {
        using var marker = EvaluateMarker.Auto();
        viewingCamera ??= Camera.main ?? FindFirstObjectByType<Camera>();
        if (viewingCamera == null)
        {
            return;
        }

        GeometryUtility.CalculateFrustumPlanes(viewingCamera, frustumPlanes);
        desiredCells.Clear();
        candidateScratch.Clear();
        unloadScratch.Clear();
        foreach (var cell in forcedCells)
        {
            if (Time.unscaledTime > cell.ForcedUntil)
            {
                unloadScratch.Add(cell);
            }
        }

        foreach (var cell in unloadScratch)
        {
            forcedCells.Remove(cell);
        }

        var modelOutsideDetailRange =
            hasModelBounds &&
            TransformBounds(transform, modelLocalBounds).SqrDistance(
                viewingCamera.transform.position) >
            settings.UnloadDistanceMetres * settings.UnloadDistanceMetres;
        SetOverviewRequested(modelOutsideDetailRange);
        if (modelOutsideDetailRange && forcedCells.Count == 0)
        {
            unloadScratch.Clear();
            unloadScratch.AddRange(residentCells);
            foreach (var cell in unloadScratch)
            {
                UnloadCell(cell);
            }

            return;
        }

        var localCamera = transform.InverseTransformPoint(viewingCamera.transform.position);
        var cameraMetres = localCamera * (float)metresPerUnit;
        var centerCell = new Vector3Int(
            Mathf.FloorToInt(cameraMetres.x / settings.CellSizeMetres),
            Mathf.FloorToInt(cameraMetres.y / settings.CellSizeMetres),
            Mathf.FloorToInt(cameraMetres.z / settings.CellSizeMetres));
        var radius = Mathf.CeilToInt(
            settings.LoadDistanceMetres / settings.CellSizeMetres) + 1;
        var maximumSquaredDistance =
            settings.LoadDistanceMetres * settings.LoadDistanceMetres;

        for (var x = centerCell.x - radius; x <= centerCell.x + radius; x++)
        {
            for (var y = centerCell.y - radius; y <= centerCell.y + radius; y++)
            {
                for (var z = centerCell.z - radius; z <= centerCell.z + radius; z++)
                {
                    if (!cells.TryGetValue(new Vector3Int(x, y, z), out var cell))
                    {
                        continue;
                    }

                    var worldBounds = TransformBounds(transform, cell.LocalBounds);
                    var squaredDistance = worldBounds.SqrDistance(
                        viewingCamera.transform.position);
                    if (squaredDistance > maximumSquaredDistance ||
                        !GeometryUtility.TestPlanesAABB(frustumPlanes, worldBounds))
                    {
                        continue;
                    }

                    desiredCells.Add(cell);
                    candidateScratch.Add(new CellCandidate(cell, squaredDistance));
                }
            }
        }

        candidateScratch.Sort(
            (left, right) => left.SquaredDistance.CompareTo(right.SquaredDistance));
        ClearPendingQueue();
        var now = Time.unscaledTime;
        foreach (var cell in forcedCells)
        {
            QueueCell(cell);
        }

        foreach (var candidate in candidateScratch)
        {
            candidate.Cell.LastDesiredTime = now;
            QueueCell(candidate.Cell);
        }

        unloadScratch.Clear();
        foreach (var cell in residentCells)
        {
            if (desiredCells.Contains(cell) ||
                now <= cell.ForcedUntil ||
                now - cell.LastDesiredTime <= settings.InvisibleRetentionSeconds)
            {
                continue;
            }

            var worldBounds = TransformBounds(transform, cell.LocalBounds);
            if (worldBounds.SqrDistance(viewingCamera.transform.position) <=
                settings.UnloadDistanceMetres * settings.UnloadDistanceMetres &&
                GeometryUtility.TestPlanesAABB(frustumPlanes, worldBounds))
            {
                continue;
            }

            unloadScratch.Add(cell);
        }

        foreach (var cell in unloadScratch)
        {
            UnloadCell(cell);
        }
    }

    private void QueueCell(CellState cell)
    {
        foreach (var record in cell.Records)
        {
            record.UnloadQueued = false;
        }

        if (cell.ResidentRecordCount > 0)
        {
            residentCells.Add(cell);
        }

        if (cell.Queued || !HasLoadableRecord(cell))
        {
            return;
        }

        cell.Queued = true;
        pendingCells.Enqueue(cell);
    }

    private void ClearPendingQueue()
    {
        while (pendingCells.Count > 0)
        {
            pendingCells.Dequeue().Queued = false;
        }
    }

    private bool HasLoadableRecord(CellState cell)
    {
        foreach (var record in cell.Records)
        {
            if (!record.Resident &&
                !record.Loading &&
                !record.Failed &&
                elements.TryGetValue(record.ProductLabel, out var element) &&
                element.Visible)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsWanted(CellState cell)
    {
        return desiredCells.Contains(cell) || Time.unscaledTime <= cell.ForcedUntil;
    }

    private void StartLoadingIfNeeded()
    {
        if (loadRoutine == null &&
            pendingCells.Count > 0 &&
            Time.unscaledTime >= nextBudgetRetryTime)
        {
            loadRoutine = StartCoroutine(LoadPendingCells());
        }
    }

    private void SetOverviewRequested(bool requested)
    {
        if (overviewRequested == requested)
        {
            return;
        }

        overviewRequested = requested;
        if (requested)
        {
            foreach (var record in overviewRecords)
            {
                if (record.Renderer != null)
                {
                    record.Renderer.enabled = true;
                }
            }

            StartOverviewLoadingIfNeeded();
            return;
        }

        UnloadOverviewRecords();
    }

    private void StartOverviewLoadingIfNeeded()
    {
        if (!overviewRequested ||
            overviewLoadRoutine != null ||
            overviewRecords == null ||
            overviewRecords.Count == 0)
        {
            return;
        }

        foreach (var record in overviewRecords)
        {
            if (!record.Resident && !record.Loading && !record.Failed)
            {
                overviewLoadRoutine = StartCoroutine(LoadOverviewRecords());
                return;
            }
        }
    }

    private IEnumerator LoadOverviewRecords()
    {
        foreach (var record in overviewRecords)
        {
            if (!overviewRequested)
            {
                break;
            }

            if (record.Resident || record.Loading || record.Failed)
            {
                continue;
            }

            while (overviewRequested &&
                   !GlobalBudget.TryConsumeLoadSlot(settings.MeshLoadsPerFrame))
            {
                yield return null;
            }

            while (overviewRequested &&
                   !GlobalBudget.TryReserve(
                       record.TriangleCount,
                       record.EstimatedResidentBytes,
                       settings.MaximumResidentTriangles,
                       settings.MaximumResidentBytes,
                       settings.MaximumResidentRenderers))
            {
                yield return null;
            }

            if (!overviewRequested)
            {
                break;
            }

            record.BudgetReserved = true;
            record.Loading = true;
            MeshLoadData data = null;
            Exception loadException = null;
            CancellationTokenSource loadCancellation = null;
            Task<MeshLoadData> readTask = null;
            try
            {
                loadCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                    lifetimeCancellation.Token);
                readTask = ReadMeshRecordAsync(record, loadCancellation.Token);
            }
            catch (Exception exception)
            {
                loadException = exception;
            }

            if (readTask != null)
            {
                while (!readTask.IsCompleted)
                {
                    if (!overviewRequested)
                    {
                        loadCancellation.Cancel();
                    }

                    yield return null;
                }

                try
                {
                    if (!overviewRequested)
                    {
                        loadCancellation.Cancel();
                    }

                    data = readTask.GetAwaiter().GetResult();
                    loadCancellation.Token.ThrowIfCancellationRequested();
                }
                catch (Exception exception)
                {
                    loadException = exception;
                }
            }

            loadCancellation?.Dispose();
            if (loadException != null || data == null)
            {
                record.Loading = false;
                var cancelled = loadException is OperationCanceledException;
                record.Failed = !cancelled;
                ReleaseReservation(record);
                if (!cancelled)
                {
                    Debug.LogError(
                        $"Could not load IFC surface overview at {record.Cell}: " +
                        (loadException?.Message ?? "No mesh data was read."));
                }

                continue;
            }

            try
            {
                var buildTimer = Stopwatch.StartNew();
                CreateOverviewRecord(record, data);
                buildTimer.Stop();
                totalDecodeMilliseconds += data.DecodeMilliseconds;
                maximumDecodeMilliseconds = Math.Max(
                    maximumDecodeMilliseconds,
                    data.DecodeMilliseconds);
                totalMeshBuildMilliseconds += buildTimer.Elapsed.TotalMilliseconds;
                maximumMeshBuildMilliseconds = Math.Max(
                    maximumMeshBuildMilliseconds,
                    buildTimer.Elapsed.TotalMilliseconds);
            }
            catch (Exception exception)
            {
                record.Loading = false;
                record.Failed = true;
                ReleaseReservation(record);
                Debug.LogError(
                    $"Could not create IFC surface-overview mesh at " +
                    $"{record.Cell}: {exception.Message}");
            }

            yield return null;
        }

        overviewLoadRoutine = null;
    }

    private IEnumerator LoadPendingCells()
    {
        var stoppedForBudget = false;
        while (pendingCells.Count > 0)
        {
            var cell = pendingCells.Dequeue();
            cell.Queued = false;
            if (!IsWanted(cell))
            {
                continue;
            }

            foreach (var record in cell.Records)
            {
                if (!IsWanted(cell) ||
                    record.Resident ||
                    record.Loading ||
                    record.Failed ||
                    !elements.TryGetValue(record.ProductLabel, out var element) ||
                    !element.Visible)
                {
                    continue;
                }

                while (!GlobalBudget.TryConsumeLoadSlot(settings.MeshLoadsPerFrame))
                {
                    yield return null;
                }

                if (!GlobalBudget.TryReserve(
                        record.TriangleCount,
                        record.EstimatedResidentBytes,
                        settings.MaximumResidentTriangles,
                        settings.MaximumResidentBytes,
                        settings.MaximumResidentRenderers))
                {
                    stoppedForBudget = true;
                    break;
                }

                record.BudgetReserved = true;
                record.Loading = true;
                MeshLoadData data = null;
                Exception loadException = null;
                CancellationTokenSource loadCancellation = null;
                Task<MeshLoadData> readTask = null;
                try
                {
                    loadCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                        lifetimeCancellation.Token);
                    readTask = ReadMeshRecordAsync(record, loadCancellation.Token);
                }
                catch (Exception exception)
                {
                    loadException = exception;
                }

                if (readTask != null)
                {
                    while (!readTask.IsCompleted)
                    {
                        if (!IsWanted(cell))
                        {
                            loadCancellation.Cancel();
                        }

                        yield return null;
                    }

                    try
                    {
                        if (!IsWanted(cell))
                        {
                            loadCancellation.Cancel();
                        }

                        data = readTask.GetAwaiter().GetResult();
                        loadCancellation.Token.ThrowIfCancellationRequested();
                    }
                    catch (Exception exception)
                    {
                        loadException = exception;
                    }
                }

                loadCancellation?.Dispose();

                if (loadException != null || data == null)
                {
                    record.Loading = false;
                    var cancelled = loadException is OperationCanceledException;
                    record.Failed = !cancelled;
                    ReleaseReservation(record);
                    if (cancelled)
                    {
                        cancelledFragmentLoadCount++;
                    }
                    else
                    {
                        Debug.LogError(
                            $"Could not stream IFC fragment at {record.Cell}: " +
                            (loadException?.Message ?? "No mesh data was read."));
                    }

                    continue;
                }

                totalDecodeMilliseconds += data.DecodeMilliseconds;
                maximumDecodeMilliseconds = Math.Max(
                    maximumDecodeMilliseconds,
                    data.DecodeMilliseconds);

                try
                {
                    using (LoadMarker.Auto())
                    {
                        var buildTimer = Stopwatch.StartNew();
                        CreateResidentRecord(record, data, element, cell);
                        buildTimer.Stop();
                        totalMeshBuildMilliseconds += buildTimer.Elapsed.TotalMilliseconds;
                        maximumMeshBuildMilliseconds = Math.Max(
                            maximumMeshBuildMilliseconds,
                            buildTimer.Elapsed.TotalMilliseconds);
                    }
                }
                catch (Exception exception)
                {
                    record.Loading = false;
                    record.Failed = true;
                    ReleaseReservation(record);
                    Debug.LogError(
                        $"Could not create Unity mesh for IFC fragment at " +
                        $"{record.Cell}: {exception.Message}");
                }

                yield return null;
            }

            if (stoppedForBudget)
            {
                QueueCell(cell);
                break;
            }
        }

        if (stoppedForBudget)
        {
            nextBudgetRetryTime = Time.unscaledTime + settings.EvaluationIntervalSeconds;
        }

        loadRoutine = null;
    }

    private Task<MeshLoadData> ReadMeshRecordAsync(
        IfcStreamMeshRecord record,
        CancellationToken cancellationToken)
    {
        var path = cachePath;
        var importUvs = settings.ImportTextureCoordinates;
        var importMeshTangents = settings.ImportTangents;
        return Task.Run(
            () => ReadMeshRecordWorker(
                path,
                record,
                importUvs,
                importMeshTangents,
                cancellationToken),
            cancellationToken);
    }

    private static MeshLoadData ReadMeshRecordWorker(
        string path,
        IfcStreamMeshRecord record,
        bool importUvs,
        bool importMeshTangents,
        CancellationToken cancellationToken)
    {
        var timer = Stopwatch.StartNew();
        using var stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            1024 * 1024,
            FileOptions.RandomAccess);
        using var reader = new BinaryReader(stream, Encoding.UTF8, false);
        stream.Position = record.PayloadOffset;
        var objectName = ReadSafeString(reader);
        var productLabel = reader.ReadInt32();
        if (productLabel != record.ProductLabel)
        {
            throw new InvalidDataException("The IFC stream index does not match its payload.");
        }

        var vertexCount = ReadCount(reader, "vertex");
        if (vertexCount != record.VertexCount)
        {
            throw new InvalidDataException("The IFC vertex count changed after indexing.");
        }

        var vertices = new Vector3[vertexCount];
        for (var index = 0; index < vertexCount; index++)
        {
            vertices[index] = new Vector3(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle());
            (vertices[index].y, vertices[index].z) =
                (vertices[index].z, vertices[index].y);
            if ((index + 1) % BinaryItemsPerYield == 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        var normalCount = ReadChannelCount(reader, vertexCount, "normal");
        var normals = new Vector3[normalCount];
        for (var index = 0; index < normalCount; index++)
        {
            normals[index] = new Vector3(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle());
            (normals[index].y, normals[index].z) =
                (normals[index].z, normals[index].y);
            var lengthSquared = normals[index].sqrMagnitude;
            if (lengthSquared > 1e-20f)
            {
                normals[index] *= 1f / (float)Math.Sqrt(lengthSquared);
            }

            if ((index + 1) % BinaryItemsPerYield == 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        var uvCount = ReadChannelCount(reader, vertexCount, "UV");
        var uvs = importUvs ? new Vector2[uvCount] : null;
        for (var index = 0; index < uvCount; index++)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            if (uvs != null)
            {
                uvs[index] = new Vector2(x, y);
            }

            if ((index + 1) % BinaryItemsPerYield == 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        var tangentCount = ReadChannelCount(reader, vertexCount, "tangent");
        var tangents = importMeshTangents ? new Vector4[tangentCount] : null;
        for (var index = 0; index < tangentCount; index++)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var w = reader.ReadSingle();
            if (tangents != null)
            {
                tangents[index] = new Vector4(x, z, y, -w);
            }

            if ((index + 1) % BinaryItemsPerYield == 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        var subMeshCount = ReadCount(reader, "sub-mesh");
        if (subMeshCount == 0)
        {
            throw new InvalidDataException("An IFC fragment has no sub-meshes.");
        }

        var subMeshes = new int[subMeshCount][];
        var styleLabels = new int[subMeshCount];
        var totalIndexCount = 0;
        for (var subMesh = 0; subMesh < subMeshCount; subMesh++)
        {
            styleLabels[subMesh] = reader.ReadInt32();
            var indexCount = ReadCount(reader, "index");
            if (indexCount % 3 != 0)
            {
                throw new InvalidDataException("An IFC fragment has non-triangular indices.");
            }

            totalIndexCount += indexCount;
            var indices = new int[indexCount];
            for (var index = 0; index < indexCount; index++)
            {
                indices[index] = reader.ReadInt32();
                if (indices[index] < 0 || indices[index] >= vertexCount)
                {
                    throw new InvalidDataException("An IFC fragment index is out of range.");
                }

                if ((index + 1) % BinaryItemsPerYield == 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }

            for (var index = 0; index + 2 < indices.Length; index += 3)
            {
                (indices[index + 1], indices[index + 2]) =
                    (indices[index + 2], indices[index + 1]);
            }

            subMeshes[subMesh] = indices;
        }

        if (totalIndexCount != record.IndexCount)
        {
            throw new InvalidDataException("The IFC index count changed after indexing.");
        }

        int[] triangleProductLabels = null;
        if (record.IsOverview)
        {
            var productCount = ReadCount(reader, "overview product");
            if (productCount != record.TriangleCount)
            {
                throw new InvalidDataException(
                    "The IFC surface-overview product map does not match its triangles.");
            }

            triangleProductLabels = new int[productCount];
            for (var index = 0; index < productCount; index++)
            {
                triangleProductLabels[index] = reader.ReadInt32();
                if ((index + 1) % BinaryItemsPerYield == 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
        timer.Stop();
        return new MeshLoadData
        {
            Name = objectName,
            ProductLabel = productLabel,
            Vertices = vertices,
            Normals = normals,
            Uvs = uvs,
            Tangents = tangents,
            SubMeshes = subMeshes,
            StyleLabels = styleLabels,
            TriangleProductLabels = triangleProductLabels,
            DecodeMilliseconds = timer.Elapsed.TotalMilliseconds
        };
    }

    private void CreateResidentRecord(
        IfcStreamMeshRecord record,
        MeshLoadData data,
        ElementState element,
        CellState cell)
    {
        var mesh = new Mesh
        {
            name = data.Name,
            indexFormat = data.Vertices.Length > ushort.MaxValue
                ? IndexFormat.UInt32
                : IndexFormat.UInt16,
            vertices = data.Vertices,
            subMeshCount = data.SubMeshes.Length
        };
        if (data.Normals.Length == data.Vertices.Length)
        {
            mesh.normals = data.Normals;
        }

        if (data.Uvs != null && data.Uvs.Length == data.Vertices.Length)
        {
            mesh.uv = data.Uvs;
        }

        if (data.Tangents != null && data.Tangents.Length == data.Vertices.Length)
        {
            mesh.tangents = data.Tangents;
        }

        for (var subMesh = 0; subMesh < data.SubMeshes.Length; subMesh++)
        {
            mesh.SetTriangles(data.SubMeshes[subMesh], subMesh, false);
        }

        if (data.Normals.Length != data.Vertices.Length)
        {
            mesh.RecalculateNormals();
        }

        mesh.RecalculateBounds();

        var parent = parentsByProduct.TryGetValue(data.ProductLabel, out var productParent)
            ? productParent
            : transform;
        var fragment = AcquireFragment(data.Name, parent);
        var meshFilter = fragment.GetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;
        var renderer = fragment.GetComponent<MeshRenderer>();
        var materials = new Material[data.StyleLabels.Length];
        for (var index = 0; index < materials.Length; index++)
        {
            if (!materialsByStyle.TryGetValue(data.StyleLabels[index], out materials[index]))
            {
                materialsByStyle.TryGetValue(0, out materials[index]);
            }
        }

        renderer.sharedMaterials = materials;
        renderer.enabled = element.Visible;

        var keepReadable = false;
        var boundsCollider = fragment.GetComponent<BoxCollider>();
        if (settings.GenerateMeshColliders)
        {
            var hasUsableColliderTriangle = ContainsUsableColliderTriangle(
                data.Vertices,
                data.SubMeshes);
            if (hasUsableColliderTriangle)
            {
                boundsCollider.center = mesh.bounds.center;
                boundsCollider.size = mesh.bounds.size;
                boundsCollider.enabled = true;
                keepReadable = true;
            }
        }

        if (!keepReadable)
        {
            boundsCollider.enabled = false;
        }

        if (settings.ReleaseCpuMeshData && !keepReadable)
        {
            mesh.UploadMeshData(true);
        }

        record.Mesh = mesh;
        record.Renderer = renderer;
        record.RuntimeObject = fragment;
        record.Loading = false;
        record.Resident = true;
        element.Renderers.Add(renderer);
        cell.ResidentRecordCount++;
        residentCells.Add(cell);
        residentTriangles += record.TriangleCount;
        residentBytes += record.EstimatedResidentBytes;
        residentRenderers++;
        loadedFragmentCount++;
        ApplyHighlight(renderer, element.Highlighted);
    }

    private void CreateOverviewRecord(
        IfcStreamMeshRecord record,
        MeshLoadData data)
    {
        if (data.TriangleProductLabels == null ||
            data.TriangleProductLabels.Length != record.TriangleCount)
        {
            throw new InvalidDataException(
                "The IFC surface overview is missing its product identity map.");
        }

        var mesh = new Mesh
        {
            name = data.Name,
            indexFormat = data.Vertices.Length > ushort.MaxValue
                ? IndexFormat.UInt32
                : IndexFormat.UInt16,
            vertices = data.Vertices,
            subMeshCount = data.SubMeshes.Length
        };
        if (data.Normals.Length == data.Vertices.Length)
        {
            mesh.normals = data.Normals;
        }

        for (var subMesh = 0; subMesh < data.SubMeshes.Length; subMesh++)
        {
            mesh.SetTriangles(data.SubMeshes[subMesh], subMesh, false);
        }

        if (data.Normals.Length != data.Vertices.Length)
        {
            mesh.RecalculateNormals();
        }

        mesh.RecalculateBounds();
        var fragment = AcquireFragment(data.Name, transform);
        fragment.GetComponent<MeshFilter>().sharedMesh = mesh;
        fragment.GetComponent<BoxCollider>().enabled = false;
        var renderer = fragment.GetComponent<MeshRenderer>();
        var materials = new Material[data.StyleLabels.Length];
        for (var index = 0; index < materials.Length; index++)
        {
            if (!materialsByStyle.TryGetValue(data.StyleLabels[index], out materials[index]))
            {
                materialsByStyle.TryGetValue(0, out materials[index]);
            }
        }

        renderer.sharedMaterials = materials;
        renderer.enabled = overviewRequested;
        record.Mesh = mesh;
        record.Renderer = renderer;
        record.RuntimeObject = fragment;
        record.SelectionVertices = data.Vertices;
        record.SelectionSubMeshes = data.SubMeshes;
        record.TriangleProductLabels = data.TriangleProductLabels;
        record.Loading = false;
        record.Resident = true;
        overviewResidentTriangles += record.TriangleCount;
        overviewResidentRenderers++;
    }

    private GameObject AcquireFragment(string objectName, Transform parent)
    {
        GameObject fragment;
        if (fragmentPool.Count > 0)
        {
            fragment = fragmentPool.Pop();
            fragment.name = objectName;
        }
        else
        {
            fragment = new GameObject(objectName);
            fragment.AddComponent<MeshFilter>();
            fragment.AddComponent<MeshRenderer>();
            var collider = fragment.AddComponent<BoxCollider>();
            collider.enabled = false;
        }

        fragment.transform.SetParent(parent, false);
        fragment.SetActive(true);
        return fragment;
    }

    private static bool ContainsUsableColliderTriangle(
        IReadOnlyList<Vector3> vertices,
        IReadOnlyList<int[]> subMeshes)
    {
        // IFC tessellation can contain collapsed faces. MeshRenderer tolerates
        // them, but PhysX reports an error when the entire MeshCollider has no
        // triangle with three distinct, non-collinear positions.
        foreach (var indices in subMeshes)
        {
            for (var index = 0; index + 2 < indices.Length; index += 3)
            {
                var vertex0 = vertices[indices[index]];
                var edge1 = vertices[indices[index + 1]] - vertex0;
                var edge2 = vertices[indices[index + 2]] - vertex0;
                var maximumEdgeSquared = Mathf.Max(
                    edge1.sqrMagnitude,
                    edge2.sqrMagnitude,
                    (edge2 - edge1).sqrMagnitude);
                if (maximumEdgeSquared <= Mathf.Epsilon)
                {
                    continue;
                }

                // Scale-relative test: reject triangles whose area has
                // collapsed compared with their longest edge.
                if (Vector3.Cross(edge1, edge2).sqrMagnitude >
                    maximumEdgeSquared * maximumEdgeSquared * 1e-12f)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void UnloadCell(CellState cell)
    {
        foreach (var record in cell.Records)
        {
            // An in-flight read owns its reservation and observes IsWanted on
            // every frame. Let that coroutine cancel itself instead of racing
            // a newly desired cell against a queued unload.
            if (!record.Resident || record.UnloadQueued)
            {
                continue;
            }

            record.UnloadQueued = true;
            pendingUnloads.Enqueue(new PendingUnload(record, cell));
        }

        residentCells.Remove(cell);
    }

    private void ProcessPendingUnloads()
    {
        if (pendingUnloads.Count == 0)
        {
            return;
        }

        var timer = Stopwatch.StartNew();
        var processed = 0;
        while (pendingUnloads.Count > 0 && processed < MaximumUnloadsPerFrame)
        {
            var pending = pendingUnloads.Dequeue();
            if (!pending.Record.UnloadQueued)
            {
                continue;
            }

            pending.Record.UnloadQueued = false;
            UnloadRecord(pending.Record, pending.Cell);
            processed++;
            if (timer.Elapsed.TotalMilliseconds >= MaximumUnloadMillisecondsPerFrame)
            {
                break;
            }
        }
    }

    private void UnloadOverviewRecords()
    {
        if (overviewRecords == null)
        {
            return;
        }

        foreach (var record in overviewRecords)
        {
            if (!record.Resident)
            {
                if (record.Loading)
                {
                    record.Loading = false;
                    ReleaseReservation(record);
                }

                continue;
            }

            ReturnFragment(record.RuntimeObject);
            DestroyOwned(record.Mesh);
            record.RuntimeObject = null;
            record.Mesh = null;
            record.Renderer = null;
            record.SelectionVertices = null;
            record.SelectionSubMeshes = null;
            record.TriangleProductLabels = null;
            record.Loading = false;
            record.Resident = false;
            overviewResidentTriangles = Math.Max(
                0L,
                overviewResidentTriangles - record.TriangleCount);
            overviewResidentRenderers = Mathf.Max(0, overviewResidentRenderers - 1);
            ReleaseReservation(record);
        }
    }

    private void UnloadRecord(IfcStreamMeshRecord record, CellState cell)
    {
        record.UnloadQueued = false;
        if (!record.Resident)
        {
            if (record.Loading)
            {
                record.Loading = false;
                ReleaseReservation(record);
            }

            return;
        }

        if (elements.TryGetValue(record.ProductLabel, out var element))
        {
            element.Renderers.Remove(record.Renderer);
        }

        ReturnFragment(record.RuntimeObject);
        DestroyOwned(record.Mesh);
        record.RuntimeObject = null;
        record.Mesh = null;
        record.Renderer = null;
        record.Loading = false;
        record.Resident = false;
        cell.ResidentRecordCount = Mathf.Max(0, cell.ResidentRecordCount - 1);
        residentTriangles -= record.TriangleCount;
        residentBytes -= record.EstimatedResidentBytes;
        residentRenderers = Mathf.Max(0, residentRenderers - 1);
        unloadedFragmentCount++;
        ReleaseReservation(record);
    }

    private static void ReleaseReservation(IfcStreamMeshRecord record)
    {
        if (!record.BudgetReserved)
        {
            return;
        }

        record.BudgetReserved = false;
        GlobalBudget.Release(
            record.TriangleCount,
            record.EstimatedResidentBytes,
            1);
    }

    private void ReturnFragment(GameObject fragment)
    {
        if (fragment == null)
        {
            return;
        }

        var renderer = fragment.GetComponent<MeshRenderer>();
        renderer.SetPropertyBlock(null);
        renderer.enabled = false;
        renderer.sharedMaterials = Array.Empty<Material>();
        fragment.GetComponent<MeshFilter>().sharedMesh = null;
        fragment.GetComponent<BoxCollider>().enabled = false;
        fragment.transform.SetParent(transform, false);
        fragment.SetActive(false);
        if (fragmentPool.Count < Math.Min(
                MaximumPooledFragments,
                settings.MaximumResidentRenderers))
        {
            fragmentPool.Push(fragment);
        }
        else
        {
            DestroyOwned(fragment);
        }
    }

    private void ApplyHighlight(Renderer renderer, bool highlighted)
    {
        if (renderer == null)
        {
            return;
        }

        var materials = renderer.sharedMaterials;
        for (var index = 0; index < materials.Length; index++)
        {
            if (!highlighted)
            {
                renderer.SetPropertyBlock(null, index);
                continue;
            }

            var color = ReadMaterialColor(materials[index]);
            color = new Color(
                color.r * 1.35f,
                color.g * 1.35f,
                color.b * 1.35f,
                color.a);
            highlightBlock.Clear();
            highlightBlock.SetColor(BaseColorId, color);
            highlightBlock.SetColor(ColorId, color);
            renderer.SetPropertyBlock(highlightBlock, index);
        }
    }

    private void LogDiagnostics()
    {
        var global = GlobalBudget.GetSnapshot();
        var completedLoads = Math.Max(1, loadedFragmentCount);
        Debug.Log(
            $"IFC streaming '{name}': {residentRenderers:N0}/{FragmentCount:N0} " +
            $"fragments resident, {residentTriangles:N0}/{TotalTriangleCount:N0} triangles, " +
            $"overview {overviewResidentRenderers:N0} renderers / " +
            $"{overviewResidentTriangles:N0}/{OverviewTriangleCount:N0} triangles " +
            $"({(overviewRequested ? "visible" : "inactive")}), " +
            $"{residentBytes / (1024f * 1024f):F1} MiB estimated mesh memory; " +
            $"global {global.Renderers:N0} renderers, {global.Triangles:N0} triangles, " +
            $"{global.Bytes / (1024f * 1024f):F1} MiB; " +
            $"queue/pool {pendingCells.Count:N0}/{fragmentPool.Count:N0}, " +
            $"unload queue {pendingUnloads.Count:N0}, " +
            $"loaded/unloaded/cancelled {loadedFragmentCount:N0}/" +
            $"{unloadedFragmentCount:N0}/{cancelledFragmentLoadCount:N0}; " +
            $"decode avg/max {totalDecodeMilliseconds / completedLoads:F2}/" +
            $"{maximumDecodeMilliseconds:F2} ms, mesh build avg/max " +
            $"{totalMeshBuildMilliseconds / completedLoads:F2}/" +
            $"{maximumMeshBuildMilliseconds:F2} ms.");
    }

    private void ReleaseAll()
    {
        if (loadRoutine != null)
        {
            StopCoroutine(loadRoutine);
            loadRoutine = null;
        }

        if (overviewLoadRoutine != null)
        {
            StopCoroutine(overviewLoadRoutine);
            overviewLoadRoutine = null;
        }

        lifetimeCancellation?.Cancel();
        UnloadOverviewRecords();

        foreach (var cell in cells.Values)
        {
            foreach (var record in cell.Records)
            {
                UnloadRecord(record, cell);
            }
        }

        lifetimeCancellation?.Dispose();
        lifetimeCancellation = null;
        while (fragmentPool.Count > 0)
        {
            DestroyOwned(fragmentPool.Pop());
        }
        cells.Clear();
        elements.Clear();
        desiredCells.Clear();
        residentCells.Clear();
        forcedCells.Clear();
        pendingCells.Clear();
        pendingUnloads.Clear();
        candidateScratch.Clear();
        unloadScratch.Clear();
        TotalTriangleCount = 0;
        OverviewTriangleCount = 0;
        residentTriangles = 0;
        residentBytes = 0;
        residentRenderers = 0;
        overviewResidentTriangles = 0;
        overviewResidentRenderers = 0;
        overviewRequested = false;
        loadedFragmentCount = 0;
        unloadedFragmentCount = 0;
        cancelledFragmentLoadCount = 0;
        totalDecodeMilliseconds = 0d;
        maximumDecodeMilliseconds = 0d;
        totalMeshBuildMilliseconds = 0d;
        maximumMeshBuildMilliseconds = 0d;
        modelLocalBounds = default;
        hasModelBounds = false;
        IsInitialized = false;

        if (registeredWithGlobalBudget)
        {
            GlobalBudget.UnregisterModel();
            registeredWithGlobalBudget = false;
        }
    }

    private void OnDestroy()
    {
        ReleaseAll();
    }

    private static Bounds TransformBounds(Transform owner, Bounds localBounds)
    {
        var center = owner.TransformPoint(localBounds.center);
        var extents = localBounds.extents;
        var axisX = owner.TransformVector(extents.x, 0f, 0f);
        var axisY = owner.TransformVector(0f, extents.y, 0f);
        var axisZ = owner.TransformVector(0f, 0f, extents.z);
        extents = new Vector3(
            Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x),
            Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y),
            Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z));
        return new Bounds(center, extents * 2f);
    }

    private static void Encapsulate(
        ref Bounds aggregate,
        ref bool hasBounds,
        Bounds value)
    {
        if (!hasBounds)
        {
            aggregate = value;
            hasBounds = true;
        }
        else
        {
            aggregate.Encapsulate(value);
        }
    }

    private static Color ReadMaterialColor(Material material)
    {
        if (material == null)
        {
            return Color.white;
        }

        if (material.HasProperty(BaseColorId))
        {
            return material.GetColor(BaseColorId);
        }

        return material.HasProperty(ColorId)
            ? material.GetColor(ColorId)
            : Color.white;
    }

    private static string ReadSafeString(BinaryReader reader)
    {
        var value = reader.ReadString();
        if (value.Length > 1_000_000)
        {
            throw new InvalidDataException("An IFC mesh name exceeds the supported length.");
        }

        return value;
    }

    private static int ReadCount(BinaryReader reader, string label)
    {
        var count = reader.ReadInt32();
        if (count < 0 || count > MaximumArrayCount)
        {
            throw new InvalidDataException($"Invalid IFC {label} count: {count}.");
        }

        return count;
    }

    private static int ReadChannelCount(
        BinaryReader reader,
        int vertexCount,
        string label)
    {
        var count = ReadCount(reader, label);
        if (count != 0 && count != vertexCount)
        {
            throw new InvalidDataException(
                $"IFC {label} count {count} does not match vertex count {vertexCount}.");
        }

        return count;
    }

    private static void DestroyOwned(UnityEngine.Object target)
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

    private static class GlobalBudget
    {
        internal readonly struct Snapshot
        {
            public long Triangles { get; }
            public long Bytes { get; }
            public int Renderers { get; }

            public Snapshot(long triangles, long bytes, int renderers)
            {
                Triangles = triangles;
                Bytes = bytes;
                Renderers = renderers;
            }
        }

        private static long triangles;
        private static long bytes;
        private static int renderers;
        private static int activeModels;
        private static int loadSlotFrame = -1;
        private static int loadSlotsUsed;

        public static void RegisterModel()
        {
            activeModels++;
        }

        public static void UnregisterModel()
        {
            activeModels = Mathf.Max(0, activeModels - 1);
            if (activeModels == 0)
            {
                triangles = 0;
                bytes = 0;
                renderers = 0;
            }
        }

        public static bool TryReserve(
            long triangleCount,
            long byteCount,
            long maximumTriangles,
            long maximumBytes,
            int maximumRenderers)
        {
            if (triangles + triangleCount > maximumTriangles ||
                bytes + byteCount > maximumBytes ||
                renderers + 1 > maximumRenderers)
            {
                return false;
            }

            triangles += triangleCount;
            bytes += byteCount;
            renderers++;
            return true;
        }

        public static void Release(long triangleCount, long byteCount, int rendererCount)
        {
            triangles = Math.Max(0L, triangles - triangleCount);
            bytes = Math.Max(0L, bytes - byteCount);
            renderers = Mathf.Max(0, renderers - rendererCount);
        }

        public static bool TryConsumeLoadSlot(int maximumLoadsPerFrame)
        {
            if (loadSlotFrame != Time.frameCount)
            {
                loadSlotFrame = Time.frameCount;
                loadSlotsUsed = 0;
            }

            if (loadSlotsUsed >= maximumLoadsPerFrame)
            {
                return false;
            }

            loadSlotsUsed++;
            return true;
        }

        public static Snapshot GetSnapshot()
        {
            return new Snapshot(triangles, bytes, renderers);
        }
    }
}
