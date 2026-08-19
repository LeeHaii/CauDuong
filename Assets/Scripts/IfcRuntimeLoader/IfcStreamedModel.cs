using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CauDuong.IfcStreaming;
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
    public BoxCollider BoundsCollider;
    public IfcElementMetadata Metadata;
    public Vector3[] SelectionVertices;
    public int[][] SelectionSubMeshes;
    public int[] TriangleProductLabels;
    public IfcTriangleBvh SelectionBvh;
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

internal readonly struct IfcDetailPickRegistration
{
    public IfcStreamedModel Model { get; }
    public IfcStreamMeshRecord Record { get; }
    public IfcElementMetadata Metadata { get; }

    public IfcDetailPickRegistration(
        IfcStreamedModel model,
        IfcStreamMeshRecord record,
        IfcElementMetadata metadata)
    {
        Model = model;
        Record = record;
        Metadata = metadata;
    }
}

internal readonly struct IfcStreamingSettings
{
    public float CellSizeMetres { get; }
    public float LoadDistanceMetres { get; }
    public float DetailPreloadDistanceMetres { get; }
    public float UnloadDistanceMetres { get; }
    public float ForwardPreloadHalfAngleDegrees { get; }
    public float CameraPredictionSeconds { get; }
    public float InvisibleRetentionSeconds { get; }
    public float EvaluationIntervalSeconds { get; }
    public long MaximumResidentTriangles { get; }
    public long MaximumResidentBytes { get; }
    public int MaximumResidentRenderers { get; }
    public float MeshBuildBudgetMilliseconds { get; }
    public bool GenerateMeshColliders { get; }
    public int MaximumMeshColliderTriangles { get; }
    public bool ImportNormals { get; }
    public bool ImportTextureCoordinates { get; }
    public bool ImportTangents { get; }
    public bool ReleaseCpuMeshData { get; }
    public bool UseSelectionBvh { get; }
    public bool EnableDiagnostics { get; }
    public float DiagnosticsIntervalSeconds { get; }

    public IfcStreamingSettings(
        float cellSizeMetres,
        float loadDistanceMetres,
        float detailPreloadDistanceMetres,
        float unloadDistanceMetres,
        float forwardPreloadHalfAngleDegrees,
        float cameraPredictionSeconds,
        float invisibleRetentionSeconds,
        float evaluationIntervalSeconds,
        long maximumResidentTriangles,
        long maximumResidentBytes,
        int maximumResidentRenderers,
        float meshBuildBudgetMilliseconds,
        bool generateMeshColliders,
        int maximumMeshColliderTriangles,
        bool importNormals,
        bool importTextureCoordinates,
        bool importTangents,
        bool releaseCpuMeshData,
        bool useSelectionBvh,
        bool enableDiagnostics,
        float diagnosticsIntervalSeconds)
    {
        CellSizeMetres = Mathf.Max(1f, cellSizeMetres);
        LoadDistanceMetres = Mathf.Max(CellSizeMetres, loadDistanceMetres);
        DetailPreloadDistanceMetres = Mathf.Max(
            LoadDistanceMetres,
            detailPreloadDistanceMetres);
        UnloadDistanceMetres = Mathf.Max(
            DetailPreloadDistanceMetres,
            unloadDistanceMetres);
        ForwardPreloadHalfAngleDegrees = Mathf.Clamp(
            forwardPreloadHalfAngleDegrees,
            1f,
            89f);
        CameraPredictionSeconds = Mathf.Max(0f, cameraPredictionSeconds);
        InvisibleRetentionSeconds = Mathf.Max(0f, invisibleRetentionSeconds);
        EvaluationIntervalSeconds = Mathf.Max(0.05f, evaluationIntervalSeconds);
        MaximumResidentTriangles = Math.Max(1_000L, maximumResidentTriangles);
        MaximumResidentBytes = Math.Max(16L * 1024L * 1024L, maximumResidentBytes);
        MaximumResidentRenderers = Mathf.Max(1, maximumResidentRenderers);
        MeshBuildBudgetMilliseconds = Mathf.Clamp(
            meshBuildBudgetMilliseconds,
            0.25f,
            16f);
        GenerateMeshColliders = generateMeshColliders;
        MaximumMeshColliderTriangles = Mathf.Max(1_000, maximumMeshColliderTriangles);
        ImportNormals = importNormals;
        ImportTextureCoordinates = importTextureCoordinates;
        ImportTangents = importTangents;
        ReleaseCpuMeshData = releaseCpuMeshData;
        UseSelectionBvh = useSelectionBvh;
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
    private const float MinimumBoundsColliderThicknessMetres = 0.001f;
    private const int IfcSelectionLayer = 8;
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly ProfilerMarker EvaluateMarker =
        new("IFC.Streaming.Evaluate");
    private static readonly ProfilerMarker PlanMarker =
        new("IFC.Streaming.VisibilityPlan");
    private static readonly ProfilerMarker LoadMarker =
        new("IFC.Streaming.LoadFragment");
    private static readonly ProfilerMarker DecodeMarker =
        new("IFC.Streaming.CacheReadDecode");
    private static readonly ProfilerMarker MeshConstructionMarker =
        new("IFC.Streaming.MeshConstruction");
    private static readonly ProfilerMarker ColliderSetupMarker =
        new("IFC.Streaming.ColliderSetup");
    private static readonly ProfilerMarker SelectionSetupMarker =
        new("IFC.Streaming.SelectionSetup");
    private static readonly ProfilerMarker RendererActivationMarker =
        new("IFC.Streaming.RendererActivation");

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
        public float FirstDesiredTime = float.NegativeInfinity;
        public float ForcedUntil;
        public float Priority = float.PositiveInfinity;
        public bool ReadinessRequired;
        public int ResidentRecordCount;
    }

    private readonly struct CellCandidate
    {
        public CellState Cell { get; }
        public float SquaredDistance { get; }
        public float Priority { get; }
        public bool InsideFrustum { get; }

        public CellCandidate(
            CellState cell,
            float squaredDistance,
            float priority,
            bool insideFrustum)
        {
            Cell = cell;
            SquaredDistance = squaredDistance;
            Priority = priority;
            InsideFrustum = insideFrustum;
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
        public IfcTriangleBvh SelectionBvh;
        public string SelectionBuildFailure;
        public double DecodeMilliseconds;
        public double SelectionBuildMilliseconds;
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
    private static readonly Dictionary<Collider, IfcDetailPickRegistration>
        DetailPickRegistrations = new();
    private static bool selectionPhysicsTransformsDirty;

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
    private int pickableColliderCount;
    private long overviewResidentTriangles;
    private int overviewResidentRenderers;
    private bool overviewRequested;
    private bool overviewLoadRequested;
    private bool detailVisible;
    private bool detailWanted;
    private IfcRepresentationState representationState =
        IfcRepresentationState.OverviewOnly;
    private Vector3 previousCameraPosition;
    private Vector3 previousCameraForward;
    private float previousCameraSampleTime = float.NegativeInfinity;
    private float detailPreloadStartedTime = float.NegativeInfinity;
    private float lastDetailFirstVisibleMilliseconds = -1f;
    private float detailResident50Milliseconds = -1f;
    private float detailResident90Milliseconds = -1f;
    private float detailResident100Milliseconds = -1f;
    private int budgetBlockedCount;
    private double totalQueueWaitMilliseconds;
    private double totalDecodeMilliseconds;
    private double maximumDecodeMilliseconds;
    private double totalMeshBuildMilliseconds;
    private double maximumMeshBuildMilliseconds;
    private double totalSelectionBuildMilliseconds;
    private double maximumSelectionBuildMilliseconds;
    private long residentSelectionBvhBytes;
    private Bounds modelLocalBounds;
    private bool hasModelBounds;
    private bool registeredWithGlobalBudget;
    private Matrix4x4 lastSelectionLocalToWorld;
    private bool hasSelectionTransformSample;

    public bool IsInitialized { get; private set; }
    public string CachePath => cachePath ?? string.Empty;
    public int CellCount => cells.Count;
    public int FragmentCount => records?.Count ?? 0;
    public long TotalTriangleCount { get; private set; }
    public long ResidentTriangleCount => residentTriangles;
    public long EstimatedResidentBytes => residentBytes;
    public int ResidentRendererCount => residentRenderers;
    public int PickableColliderCount => pickableColliderCount;
    public long ResidentSelectionBvhBytes => residentSelectionBvhBytes;
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
        UpdateAllRecordPickability();
    }

    private void OnDisable()
    {
        ActiveModels.Remove(this);
        UpdateAllRecordPickability();
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
        lastSelectionLocalToWorld = transform.localToWorldMatrix;
        hasSelectionTransformSample = true;
        selectionPhysicsTransformsDirty = true;
        representationState = IfcRepresentationState.OverviewOnly;
        overviewLoadRequested = true;
        overviewRequested = true;
        detailVisible = false;
        detailWanted = false;

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
                renderer.enabled = visible && detailVisible;
            }
        }

        foreach (var record in element.Records)
        {
            UpdateRecordPickability(record, visible);
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

    internal static bool TryRaycastDetailSurface(
        Ray worldRay,
        Collider boundsCollider,
        out Vector3 worldPoint,
        out Vector3 worldNormal,
        out float worldDistance)
    {
        worldPoint = default;
        worldNormal = default;
        worldDistance = float.PositiveInfinity;
        if (boundsCollider == null ||
            !DetailPickRegistrations.TryGetValue(
                boundsCollider,
                out var registration) ||
            registration.Model == null ||
            registration.Record == null ||
            !registration.Record.Resident ||
            !boundsCollider.enabled ||
            !boundsCollider.gameObject.activeInHierarchy ||
            registration.Record.SelectionBvh == null)
        {
            return false;
        }

        var record = registration.Record;
        if (record.BoundsCollider != boundsCollider ||
            record.SelectionBvh == null)
        {
            return false;
        }

        var owner = boundsCollider.transform;
        var localOrigin = owner.InverseTransformPoint(worldRay.origin);
        var localRayPoint = owner.InverseTransformPoint(
            worldRay.origin + worldRay.direction);
        var localDirection = localRayPoint - localOrigin;
        if (!record.SelectionBvh.Raycast(
                new Ray(localOrigin, localDirection),
                out var localDistance,
                out var localNormal))
        {
            return false;
        }

        var normalizedLocalDirection = localDirection.normalized;
        worldPoint = owner.TransformPoint(
            localOrigin + normalizedLocalDirection * localDistance);
        worldNormal = owner.TransformDirection(localNormal).normalized;
        if (Vector3.Dot(worldNormal, worldRay.direction) > 0f)
        {
            worldNormal = -worldNormal;
        }

        worldDistance = Vector3.Distance(worldRay.origin, worldPoint);
        return true;
    }

    internal static bool TryGetDetailPickMetadata(
        Collider collider,
        out IfcElementMetadata metadata)
    {
        metadata = null;
        if (collider == null ||
            !DetailPickRegistrations.TryGetValue(collider, out var registration) ||
            registration.Model == null ||
            registration.Record == null ||
            !registration.Record.Resident ||
            !collider.enabled ||
            !collider.gameObject.activeInHierarchy ||
            registration.Metadata == null)
        {
            return false;
        }

        metadata = registration.Metadata;
        return true;
    }

    internal static bool ConsumeSelectionPhysicsTransformsDirty()
    {
        var needsSync = selectionPhysicsTransformsDirty;
        foreach (var model in ActiveModels)
        {
            if (model == null)
            {
                continue;
            }

            var currentTransform = model.transform.localToWorldMatrix;
            if (!model.hasSelectionTransformSample ||
                currentTransform != model.lastSelectionLocalToWorld)
            {
                needsSync = true;
                model.lastSelectionLocalToWorld = currentTransform;
                model.hasSelectionTransformSample = true;
            }
        }

        selectionPhysicsTransformsDirty = false;
        return needsSync;
    }

    internal static void MarkSelectionPhysicsTransformsDirty()
    {
        selectionPhysicsTransformsDirty = true;
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

        using var planMarker = PlanMarker.Auto();
        var now = Time.unscaledTime;
        var cameraPosition = viewingCamera.transform.position;
        var cameraForward = viewingCamera.transform.forward;
        GeometryUtility.CalculateFrustumPlanes(viewingCamera, frustumPlanes);
        desiredCells.Clear();
        candidateScratch.Clear();
        unloadScratch.Clear();
        foreach (var cell in residentCells)
        {
            cell.Priority = float.PositiveInfinity;
            cell.ReadinessRequired = false;
        }

        foreach (var cell in forcedCells)
        {
            if (now > cell.ForcedUntil)
            {
                unloadScratch.Add(cell);
            }
        }

        foreach (var cell in unloadScratch)
        {
            forcedCells.Remove(cell);
        }

        var sampleDelta = previousCameraSampleTime > float.NegativeInfinity
            ? Mathf.Max(0.001f, now - previousCameraSampleTime)
            : 0f;
        var predictedCameraPosition = cameraPosition;
        var predictedCameraForward = cameraForward;
        if (sampleDelta > 0f)
        {
            var predictionScale = settings.CameraPredictionSeconds / sampleDelta;
            predictedCameraPosition +=
                (cameraPosition - previousCameraPosition) * predictionScale;
            predictedCameraForward = (
                cameraForward +
                (cameraForward - previousCameraForward) * predictionScale).normalized;
        }

        previousCameraPosition = cameraPosition;
        previousCameraForward = cameraForward;
        previousCameraSampleTime = now;

        var modelSquaredDistance = hasModelBounds
            ? TransformBounds(transform, modelLocalBounds).SqrDistance(cameraPosition)
            : float.PositiveInfinity;
        detailWanted = forcedCells.Count > 0 ||
                       modelSquaredDistance <=
                       settings.DetailPreloadDistanceMetres *
                       settings.DetailPreloadDistanceMetres;
        if (detailWanted && detailPreloadStartedTime < 0f)
        {
            detailPreloadStartedTime = now;
            detailResident50Milliseconds = -1f;
            detailResident90Milliseconds = -1f;
            detailResident100Milliseconds = -1f;
        }
        else if (!detailWanted)
        {
            detailPreloadStartedTime = float.NegativeInfinity;
        }

        var localCamera = transform.InverseTransformPoint(cameraPosition);
        var cameraMetres = localCamera * (float)metresPerUnit;
        var centerCell = new Vector3Int(
            Mathf.FloorToInt(cameraMetres.x / settings.CellSizeMetres),
            Mathf.FloorToInt(cameraMetres.y / settings.CellSizeMetres),
            Mathf.FloorToInt(cameraMetres.z / settings.CellSizeMetres));
        var radius = Mathf.CeilToInt(
            settings.DetailPreloadDistanceMetres / settings.CellSizeMetres) + 1;
        var maximumSquaredDistance =
            settings.DetailPreloadDistanceMetres *
            settings.DetailPreloadDistanceMetres;
        var fullDetailSquaredDistance =
            settings.LoadDistanceMetres * settings.LoadDistanceMetres;
        var coneCosine = Mathf.Cos(
            settings.ForwardPreloadHalfAngleDegrees * Mathf.Deg2Rad);
        var nearestVisibleSquaredDistance = float.PositiveInfinity;
        var nearestCandidateSquaredDistance = float.PositiveInfinity;

        if (detailWanted)
        {
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
                        var squaredDistance = worldBounds.SqrDistance(cameraPosition);
                        var predictedSquaredDistance = worldBounds.SqrDistance(
                            predictedCameraPosition);
                        if (Mathf.Min(squaredDistance, predictedSquaredDistance) >
                            maximumSquaredDistance)
                        {
                            continue;
                        }

                        var insideFrustum = GeometryUtility.TestPlanesAABB(
                            frustumPlanes,
                            worldBounds);
                        var insideForwardCone = IfcVisibilityPlanner.IsInsideForwardCone(
                            predictedCameraPosition,
                            predictedCameraForward,
                            worldBounds.center,
                            coneCosine);
                        if (!insideFrustum && !insideForwardCone)
                        {
                            continue;
                        }

                        var priority = IfcVisibilityPlanner.CalculateCellPriority(
                            squaredDistance,
                            false,
                            insideFrustum,
                            insideForwardCone,
                            predictedSquaredDistance);
                        cell.Priority = priority;
                        desiredCells.Add(cell);
                        candidateScratch.Add(new CellCandidate(
                            cell,
                            squaredDistance,
                            priority,
                            insideFrustum));
                        nearestCandidateSquaredDistance = Mathf.Min(
                            nearestCandidateSquaredDistance,
                            Mathf.Min(squaredDistance, predictedSquaredDistance));
                        if (insideFrustum)
                        {
                            nearestVisibleSquaredDistance = Mathf.Min(
                                nearestVisibleSquaredDistance,
                                squaredDistance);
                        }
                    }
                }
            }
        }

        if (nearestVisibleSquaredDistance < float.PositiveInfinity)
        {
            var readinessRadius = Mathf.Sqrt(nearestVisibleSquaredDistance) +
                                  settings.CellSizeMetres * 1.75f;
            var readinessSquaredDistance = readinessRadius * readinessRadius;
            foreach (var candidate in candidateScratch)
            {
                candidate.Cell.ReadinessRequired = candidate.InsideFrustum &&
                                                   candidate.SquaredDistance <=
                                                   readinessSquaredDistance;
            }
        }

        // Outside the full-detail radius, only the closest cell ring is
        // decoded. This produces a fast recognizable handoff without filling
        // the residency budget with the entire 1.15 km preload volume.
        if (nearestCandidateSquaredDistance < float.PositiveInfinity)
        {
            var preloadRingRadius = Mathf.Sqrt(nearestCandidateSquaredDistance) +
                                    settings.CellSizeMetres * 1.75f;
            var preloadRingSquaredDistance = preloadRingRadius * preloadRingRadius;
            for (var index = candidateScratch.Count - 1; index >= 0; index--)
            {
                var candidate = candidateScratch[index];
                if (candidate.Cell.ReadinessRequired ||
                    candidate.SquaredDistance <= fullDetailSquaredDistance ||
                    candidate.SquaredDistance <= preloadRingSquaredDistance)
                {
                    continue;
                }

                desiredCells.Remove(candidate.Cell);
                candidate.Cell.Priority = float.PositiveInfinity;
                candidateScratch.RemoveAt(index);
            }
        }

        candidateScratch.Sort(
            (left, right) => left.Priority.CompareTo(right.Priority));
        ClearPendingQueue();
        foreach (var cell in forcedCells)
        {
            cell.Priority = IfcVisibilityPlanner.ForcedPriority;
            desiredCells.Add(cell);
            QueueCell(cell);
        }

        foreach (var candidate in candidateScratch)
        {
            candidate.Cell.LastDesiredTime = now;
            if (candidate.Cell.FirstDesiredTime < 0f)
            {
                candidate.Cell.FirstDesiredTime = now;
            }

            QueueCell(candidate.Cell);
        }

        UpdateRepresentation();

        // While the overview is being prepared for a zoom-out, detail is the
        // only visible representation and must remain untouched.
        if (representationState == IfcRepresentationState.PreloadingOverview)
        {
            return;
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
            if (detailWanted &&
                worldBounds.SqrDistance(cameraPosition) <=
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
            if (!detailWanted)
            {
                cell.FirstDesiredTime = float.NegativeInfinity;
            }
        }
    }

    private void QueueCell(CellState cell)
    {
        if (cell.FirstDesiredTime < 0f)
        {
            cell.FirstDesiredTime = Time.unscaledTime;
        }

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

    private bool IsDetailReady()
    {
        var hasRequiredCell = false;
        foreach (var cell in desiredCells)
        {
            if (!cell.ReadinessRequired && Time.unscaledTime > cell.ForcedUntil)
            {
                continue;
            }

            hasRequiredCell = true;
            foreach (var record in cell.Records)
            {
                if (!record.Resident &&
                    !record.Failed &&
                    elements.TryGetValue(record.ProductLabel, out var element) &&
                    element.Visible)
                {
                    return false;
                }
            }
        }

        return hasRequiredCell;
    }

    private bool IsOverviewReady()
    {
        if (overviewRecords == null || overviewRecords.Count == 0)
        {
            return false;
        }

        var hasResidentRecord = false;
        foreach (var record in overviewRecords)
        {
            hasResidentRecord |= record.Resident;
            if (!record.Resident && !record.Failed)
            {
                return false;
            }
        }

        return hasResidentRecord;
    }

    private void UpdateRepresentation()
    {
        var decision = IfcVisibilityPlanner.EvaluateRepresentation(
            representationState,
            detailWanted,
            IsDetailReady(),
            IsOverviewReady());
        var wasDetailVisible = detailVisible;
        representationState = decision.State;

        if (decision.ShowDetail)
        {
            SetDetailVisible(true);
            SetOverviewVisible(false);
        }
        else
        {
            SetOverviewVisible(decision.ShowOverview);
            SetDetailVisible(false);
        }

        SetOverviewLoadRequested(decision.KeepOverviewResident);
        UpdateResidencyMilestones();
        if (!wasDetailVisible && detailVisible && detailPreloadStartedTime >= 0f)
        {
            lastDetailFirstVisibleMilliseconds =
                (Time.unscaledTime - detailPreloadStartedTime) * 1_000f;
        }
    }

    private void UpdateResidencyMilestones()
    {
        if (!detailWanted || detailPreloadStartedTime < 0f)
        {
            return;
        }

        var total = 0;
        var resident = 0;
        foreach (var cell in desiredCells)
        {
            foreach (var record in cell.Records)
            {
                if (!elements.TryGetValue(record.ProductLabel, out var element) ||
                    !element.Visible)
                {
                    continue;
                }

                total++;
                if (record.Resident)
                {
                    resident++;
                }
            }
        }

        if (total == 0)
        {
            return;
        }

        var elapsedMilliseconds =
            (Time.unscaledTime - detailPreloadStartedTime) * 1_000f;
        var ratio = (float)resident / total;
        if (ratio >= 0.5f && detailResident50Milliseconds < 0f)
        {
            detailResident50Milliseconds = elapsedMilliseconds;
        }

        if (ratio >= 0.9f && detailResident90Milliseconds < 0f)
        {
            detailResident90Milliseconds = elapsedMilliseconds;
        }

        if (resident == total && detailResident100Milliseconds < 0f)
        {
            detailResident100Milliseconds = elapsedMilliseconds;
        }
    }

    private bool TryReserveWithEviction(
        IfcStreamMeshRecord record,
        float incomingPriority)
    {
        if (GlobalBudget.TryReserve(
                record.TriangleCount,
                record.EstimatedResidentBytes,
                settings.MaximumResidentTriangles,
                settings.MaximumResidentBytes,
                settings.MaximumResidentRenderers))
        {
            return true;
        }

        // Evict complete low-priority cells until the higher-priority request
        // fits. Forced/selected cells and the ring required for an active
        // overview-to-detail handoff are never candidates.
        for (var attempt = 0; attempt < 128; attempt++)
        {
            IfcStreamedModel victimModel = null;
            CellState victimCell = null;
            var worstPriority = incomingPriority;
            var oldestDesiredTime = float.PositiveInfinity;
            foreach (var model in ActiveModels)
            {
                if (model == null || !model.IsInitialized)
                {
                    continue;
                }

                foreach (var cell in model.residentCells)
                {
                    if (!IfcVisibilityPlanner.CanEvict(
                            cell.Priority,
                            worstPriority,
                            Time.unscaledTime <= cell.ForcedUntil,
                            cell.ReadinessRequired) ||
                        (Mathf.Approximately(cell.Priority, worstPriority) &&
                         cell.LastDesiredTime >= oldestDesiredTime))
                    {
                        continue;
                    }

                    victimModel = model;
                    victimCell = cell;
                    worstPriority = cell.Priority;
                    oldestDesiredTime = cell.LastDesiredTime;
                }
            }

            if (victimModel == null || victimCell == null)
            {
                return false;
            }

            victimModel.EvictCellImmediately(victimCell);
            if (GlobalBudget.TryReserve(
                    record.TriangleCount,
                    record.EstimatedResidentBytes,
                    settings.MaximumResidentTriangles,
                    settings.MaximumResidentBytes,
                    settings.MaximumResidentRenderers))
            {
                return true;
            }
        }

        return false;
    }

    private void EvictCellImmediately(CellState cell)
    {
        foreach (var record in cell.Records)
        {
            record.UnloadQueued = false;
            if (record.Resident)
            {
                UnloadRecord(record, cell);
            }
        }

        cell.Queued = false;
        cell.FirstDesiredTime = float.NegativeInfinity;
        residentCells.Remove(cell);
    }

    private void SetOverviewVisible(bool visible)
    {
        overviewRequested = visible;
        if (overviewRecords == null)
        {
            return;
        }

        using var marker = RendererActivationMarker.Auto();
        foreach (var record in overviewRecords)
        {
            if (record.Renderer != null)
            {
                record.Renderer.enabled = visible;
            }
        }
    }

    private void SetDetailVisible(bool visible)
    {
        detailVisible = visible;
        if (records == null)
        {
            return;
        }

        using var marker = RendererActivationMarker.Auto();
        foreach (var record in records)
        {
            var elementVisible = elements.TryGetValue(
                                     record.ProductLabel,
                                     out var element) &&
                                 element.Visible;
            if (record.Renderer != null)
            {
                record.Renderer.enabled = visible && elementVisible;
            }

            UpdateRecordPickability(record, elementVisible);
        }
    }

    private void UpdateAllRecordPickability()
    {
        if (records == null)
        {
            return;
        }

        foreach (var record in records)
        {
            var elementVisible = elements.TryGetValue(
                                     record.ProductLabel,
                                     out var element) &&
                                 element.Visible;
            UpdateRecordPickability(record, elementVisible);
        }
    }

    private void UpdateRecordPickability(
        IfcStreamMeshRecord record,
        bool elementVisible)
    {
        var collider = record?.BoundsCollider;
        if (collider == null)
        {
            return;
        }

        var decision = IfcSelectionPolicy.Evaluate(
            settings.GenerateMeshColliders,
            IfcSelectionPolicy.HasValidBounds(record.LocalBounds),
            record.SelectionBvh != null,
            settings.ReleaseCpuMeshData,
            record.Resident,
            elementVisible,
            detailVisible);
        var shouldBePickable =
            isActiveAndEnabled &&
            decision.ShouldBePickable &&
            record.Metadata != null &&
            record.RuntimeObject != null &&
            record.RuntimeObject.activeInHierarchy;

        if (!shouldBePickable)
        {
            RemoveDetailPickRegistration(collider);
            SetBoundsColliderEnabled(collider, false);
            return;
        }

        if (DetailPickRegistrations.TryGetValue(
                collider,
                out var existingRegistration) &&
            existingRegistration.Model == this &&
            existingRegistration.Record == record &&
            existingRegistration.Metadata == record.Metadata)
        {
            SetBoundsColliderEnabled(collider, true);
            return;
        }

        RemoveDetailPickRegistration(collider);
        SetBoundsColliderEnabled(collider, true);
        DetailPickRegistrations[collider] = new IfcDetailPickRegistration(
            this,
            record,
            record.Metadata);
        pickableColliderCount++;
    }

    private static void SetBoundsColliderEnabled(
        BoxCollider collider,
        bool enabled)
    {
        if (collider == null || collider.enabled == enabled)
        {
            return;
        }

        collider.enabled = enabled;
        selectionPhysicsTransformsDirty = true;
    }

    private static void RemoveDetailPickRegistration(Collider collider)
    {
        if (collider == null ||
            !DetailPickRegistrations.TryGetValue(
                collider,
                out var registration))
        {
            return;
        }

        DetailPickRegistrations.Remove(collider);
        if (registration.Model != null)
        {
            registration.Model.pickableColliderCount = Mathf.Max(
                0,
                registration.Model.pickableColliderCount - 1);
        }
    }

    private void SetOverviewLoadRequested(bool requested)
    {
        if (overviewLoadRequested == requested)
        {
            return;
        }

        overviewLoadRequested = requested;
        if (requested)
        {
            StartOverviewLoadingIfNeeded();
            return;
        }

        UnloadOverviewRecords();
    }

    private void StartOverviewLoadingIfNeeded()
    {
        if (!overviewLoadRequested ||
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
            if (!overviewLoadRequested)
            {
                break;
            }

            if (record.Resident || record.Loading || record.Failed)
            {
                continue;
            }

            while (overviewLoadRequested &&
                   !TryReserveWithEviction(
                       record,
                       IfcVisibilityPlanner.FrustumPriorityBias))
            {
                budgetBlockedCount++;
                yield return null;
            }

            if (!overviewLoadRequested)
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
                    if (!overviewLoadRequested)
                    {
                        loadCancellation.Cancel();
                    }

                    yield return null;
                }

                try
                {
                    if (!overviewLoadRequested)
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

            while (overviewLoadRequested &&
                   !GlobalBudget.TryBeginMeshBuild(
                       this,
                       settings.MeshBuildBudgetMilliseconds))
            {
                yield return null;
            }

            if (!overviewLoadRequested)
            {
                record.Loading = false;
                ReleaseReservation(record);
                break;
            }

            try
            {
                var buildTimer = Stopwatch.StartNew();
                CreateOverviewRecord(record, data);
                buildTimer.Stop();
                GlobalBudget.RecordMeshBuild(buildTimer.Elapsed.TotalMilliseconds);
                totalDecodeMilliseconds += data.DecodeMilliseconds;
                maximumDecodeMilliseconds = Math.Max(
                    maximumDecodeMilliseconds,
                    data.DecodeMilliseconds);
                totalSelectionBuildMilliseconds +=
                    data.SelectionBuildMilliseconds;
                maximumSelectionBuildMilliseconds = Math.Max(
                    maximumSelectionBuildMilliseconds,
                    data.SelectionBuildMilliseconds);
                totalMeshBuildMilliseconds += buildTimer.Elapsed.TotalMilliseconds;
                maximumMeshBuildMilliseconds = Math.Max(
                    maximumMeshBuildMilliseconds,
                    buildTimer.Elapsed.TotalMilliseconds);
                UpdateRepresentation();
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

                if (!TryReserveWithEviction(record, cell.Priority))
                {
                    budgetBlockedCount++;
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
                totalSelectionBuildMilliseconds += data.SelectionBuildMilliseconds;
                maximumSelectionBuildMilliseconds = Math.Max(
                    maximumSelectionBuildMilliseconds,
                    data.SelectionBuildMilliseconds);

                while (IsWanted(cell) &&
                       !GlobalBudget.TryBeginMeshBuild(
                           this,
                           settings.MeshBuildBudgetMilliseconds))
                {
                    yield return null;
                }

                if (!IsWanted(cell))
                {
                    record.Loading = false;
                    ReleaseReservation(record);
                    cancelledFragmentLoadCount++;
                    continue;
                }

                try
                {
                    using (LoadMarker.Auto())
                    {
                        var buildTimer = Stopwatch.StartNew();
                        CreateResidentRecord(record, data, element, cell);
                        buildTimer.Stop();
                        GlobalBudget.RecordMeshBuild(
                            buildTimer.Elapsed.TotalMilliseconds);
                        totalMeshBuildMilliseconds += buildTimer.Elapsed.TotalMilliseconds;
                        maximumMeshBuildMilliseconds = Math.Max(
                            maximumMeshBuildMilliseconds,
                            buildTimer.Elapsed.TotalMilliseconds);
                        UpdateRepresentation();
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
        var importNormals = settings.ImportNormals;
        var importUvs = settings.ImportTextureCoordinates;
        var importMeshTangents = settings.ImportTangents;
        var buildSelectionBvh = settings.GenerateMeshColliders &&
                                settings.UseSelectionBvh;
        return Task.Run(
            () => ReadMeshRecordWorker(
                path,
                record,
                importNormals,
                importUvs,
                importMeshTangents,
                buildSelectionBvh,
                cancellationToken),
            cancellationToken);
    }

    private static MeshLoadData ReadMeshRecordWorker(
        string path,
        IfcStreamMeshRecord record,
        bool importNormals,
        bool importUvs,
        bool importMeshTangents,
        bool buildSelectionBvh,
        CancellationToken cancellationToken)
    {
        using var decodeMarker = DecodeMarker.Auto();
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
        var normals = importNormals ? new Vector3[normalCount] : Array.Empty<Vector3>();
        for (var index = 0; index < normalCount; index++)
        {
            var normal = new Vector3(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle());
            if (importNormals)
            {
                (normal.y, normal.z) = (normal.z, normal.y);
                var lengthSquared = normal.sqrMagnitude;
                if (lengthSquared > 1e-20f)
                {
                    normal *= 1f / (float)Math.Sqrt(lengthSquared);
                }

                normals[index] = normal;
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
        IfcTriangleBvh selectionBvh = null;
        string selectionBuildFailure = null;
        var selectionBuildMilliseconds = 0d;
        if (buildSelectionBvh && !record.IsOverview)
        {
            using var selectionMarker = SelectionSetupMarker.Auto();
            var selectionTimer = Stopwatch.StartNew();
            try
            {
                selectionBvh = new IfcTriangleBvh(vertices, subMeshes);
            }
            catch (Exception exception) when (
                exception is not OperationCanceledException &&
                exception is not OutOfMemoryException)
            {
                // A selection accelerator is optional. Preserve the readable
                // render mesh so this fragment can still use exact fallback
                // picking instead of failing its visual load.
                selectionBuildFailure = exception.Message;
            }

            selectionTimer.Stop();
            selectionBuildMilliseconds = selectionTimer.Elapsed.TotalMilliseconds;
        }

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
            SelectionBvh = selectionBvh,
            SelectionBuildFailure = selectionBuildFailure,
            DecodeMilliseconds = timer.Elapsed.TotalMilliseconds,
            SelectionBuildMilliseconds = selectionBuildMilliseconds
        };
    }

    private void CreateResidentRecord(
        IfcStreamMeshRecord record,
        MeshLoadData data,
        ElementState element,
        CellState cell)
    {
        var mesh = CreateUnityMesh(record, data);
        if (!string.IsNullOrEmpty(data.SelectionBuildFailure))
        {
            Debug.LogWarning(
                $"IFC selection BVH fallback for '{data.Name}': " +
                data.SelectionBuildFailure);
        }

        var parent = parentsByProduct.TryGetValue(data.ProductLabel, out var productParent)
            ? productParent
            : transform;
        var fragment = AcquireFragment(data.Name, parent);
        fragment.layer = IfcSelectionLayer;
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
        renderer.enabled = element.Visible && detailVisible;

        var boundsCollider = fragment.GetComponent<BoxCollider>();
        var hasValidBounds = IfcSelectionPolicy.HasValidBounds(record.LocalBounds);
        var selectionPolicy = IfcSelectionPolicy.Evaluate(
            settings.GenerateMeshColliders,
            hasValidBounds,
            data.SelectionBvh != null,
            settings.ReleaseCpuMeshData,
            resident: true,
            elementVisible: element.Visible,
            detailVisible);
        using (ColliderSetupMarker.Auto())
        {
            SetBoundsColliderEnabled(boundsCollider, false);
            if (selectionPolicy.ShouldEnableBoundsCollider)
            {
                var minimumThickness = (float)(
                    MinimumBoundsColliderThicknessMetres /
                    Math.Max(Math.Abs(metresPerUnit), 1e-9d));
                boundsCollider.center = record.LocalBounds.center;
                boundsCollider.size = IfcSelectionPolicy.WithMinimumThickness(
                    record.LocalBounds.size,
                    minimumThickness);
                selectionPhysicsTransformsDirty = true;
            }
        }

        if (selectionPolicy.ShouldReleaseCpuMeshData)
        {
            mesh.UploadMeshData(true);
        }

        record.Mesh = mesh;
        record.Renderer = renderer;
        record.RuntimeObject = fragment;
        record.BoundsCollider = boundsCollider;
        record.Metadata = productParent != null &&
                          productParent.TryGetComponent<IfcElementMetadata>(
                              out var metadata)
            ? metadata
            : null;
        record.SelectionBvh = data.SelectionBvh;
        record.Loading = false;
        record.Resident = true;
        residentSelectionBvhBytes += record.SelectionBvh?.EstimatedBytes ?? 0L;
        element.Renderers.Add(renderer);
        cell.ResidentRecordCount++;
        residentCells.Add(cell);
        residentTriangles += record.TriangleCount;
        residentBytes += record.EstimatedResidentBytes;
        residentRenderers++;
        loadedFragmentCount++;
        UpdateRecordPickability(record, element.Visible);
        if (cell.FirstDesiredTime >= 0f)
        {
            totalQueueWaitMilliseconds +=
                (Time.unscaledTime - cell.FirstDesiredTime) * 1_000d;
        }

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

        var mesh = CreateUnityMesh(record, data);
        var fragment = AcquireFragment(data.Name, transform);
        fragment.GetComponent<MeshFilter>().sharedMesh = mesh;
        SetBoundsColliderEnabled(fragment.GetComponent<BoxCollider>(), false);
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

    private Mesh CreateUnityMesh(IfcStreamMeshRecord record, MeshLoadData data)
    {
        using var marker = MeshConstructionMarker.Auto();
        var hasOnlyPositions = data.Normals.Length == 0 &&
                               data.Uvs == null &&
                               data.Tangents == null;
        if (!hasOnlyPositions)
        {
            var legacyMesh = new Mesh
            {
                name = data.Name,
                indexFormat = data.Vertices.Length > ushort.MaxValue
                    ? IndexFormat.UInt32
                    : IndexFormat.UInt16,
                vertices = data.Vertices,
                subMeshCount = data.SubMeshes.Length,
                bounds = record.LocalBounds
            };
            if (data.Normals.Length == data.Vertices.Length)
            {
                legacyMesh.normals = data.Normals;
            }

            if (data.Uvs != null && data.Uvs.Length == data.Vertices.Length)
            {
                legacyMesh.uv = data.Uvs;
            }

            if (data.Tangents != null && data.Tangents.Length == data.Vertices.Length)
            {
                legacyMesh.tangents = data.Tangents;
            }

            for (var subMesh = 0; subMesh < data.SubMeshes.Length; subMesh++)
            {
                legacyMesh.SetTriangles(data.SubMeshes[subMesh], subMesh, false);
            }

            if (settings.ImportNormals &&
                data.Normals.Length != data.Vertices.Length)
            {
                legacyMesh.RecalculateNormals();
            }

            return legacyMesh;
        }

        var meshDataArray = Mesh.AllocateWritableMeshData(1);
        var meshData = meshDataArray[0];
        meshData.SetVertexBufferParams(
            data.Vertices.Length,
            new VertexAttributeDescriptor(
                VertexAttribute.Position,
                VertexAttributeFormat.Float32,
                3));
        meshData.GetVertexData<Vector3>().CopyFrom(data.Vertices);

        var indexFormat = data.Vertices.Length > ushort.MaxValue
            ? IndexFormat.UInt32
            : IndexFormat.UInt16;
        meshData.SetIndexBufferParams(record.IndexCount, indexFormat);
        if (indexFormat == IndexFormat.UInt32)
        {
            var destination = meshData.GetIndexData<uint>();
            var destinationIndex = 0;
            foreach (var source in data.SubMeshes)
            {
                foreach (var index in source)
                {
                    destination[destinationIndex++] = (uint)index;
                }
            }
        }
        else
        {
            var destination = meshData.GetIndexData<ushort>();
            var destinationIndex = 0;
            foreach (var source in data.SubMeshes)
            {
                foreach (var index in source)
                {
                    destination[destinationIndex++] = (ushort)index;
                }
            }
        }

        var updateFlags = MeshUpdateFlags.DontRecalculateBounds |
                          MeshUpdateFlags.DontValidateIndices |
                          MeshUpdateFlags.DontNotifyMeshUsers;
        meshData.subMeshCount = data.SubMeshes.Length;
        var indexStart = 0;
        for (var subMesh = 0; subMesh < data.SubMeshes.Length; subMesh++)
        {
            var indexCount = data.SubMeshes[subMesh].Length;
            meshData.SetSubMesh(
                subMesh,
                new SubMeshDescriptor(indexStart, indexCount, MeshTopology.Triangles)
                {
                    bounds = record.LocalBounds,
                    vertexCount = data.Vertices.Length
                },
                updateFlags);
            indexStart += indexCount;
        }

        var mesh = new Mesh { name = data.Name };
        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh, updateFlags);
        mesh.bounds = record.LocalBounds;
        return mesh;
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
            SetBoundsColliderEnabled(collider, false);
        }

        var boundsCollider = fragment.GetComponent<BoxCollider>();
        RemoveDetailPickRegistration(boundsCollider);
        SetBoundsColliderEnabled(boundsCollider, false);
        fragment.transform.SetParent(parent, false);
        fragment.SetActive(true);
        return fragment;
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

        if (record.BoundsCollider != null)
        {
            RemoveDetailPickRegistration(record.BoundsCollider);
            SetBoundsColliderEnabled(record.BoundsCollider, false);
        }

        residentSelectionBvhBytes = Math.Max(
            0L,
            residentSelectionBvhBytes - (record.SelectionBvh?.EstimatedBytes ?? 0L));
        ReturnFragment(record.RuntimeObject);
        DestroyOwned(record.Mesh);
        record.RuntimeObject = null;
        record.Mesh = null;
        record.Renderer = null;
        record.BoundsCollider = null;
        record.Metadata = null;
        record.SelectionBvh = null;
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
        var boundsCollider = fragment.GetComponent<BoxCollider>();
        RemoveDetailPickRegistration(boundsCollider);
        SetBoundsColliderEnabled(boundsCollider, false);
        boundsCollider.center = Vector3.zero;
        boundsCollider.size = Vector3.one;
        selectionPhysicsTransformsDirty = true;
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
        var desiredRecordCount = 0;
        var desiredResidentCount = 0;
        foreach (var cell in desiredCells)
        {
            foreach (var record in cell.Records)
            {
                desiredRecordCount++;
                desiredResidentCount += record.Resident ? 1 : 0;
            }
        }

        var desiredResidentPercent = desiredRecordCount > 0
            ? 100f * desiredResidentCount / desiredRecordCount
            : 100f;
        Debug.Log(
            $"IFC streaming '{name}': {residentRenderers:N0}/{FragmentCount:N0} " +
            $"fragments resident, {residentTriangles:N0}/{TotalTriangleCount:N0} triangles, " +
            $"overview {overviewResidentRenderers:N0} renderers / " +
            $"{overviewResidentTriangles:N0}/{OverviewTriangleCount:N0} triangles " +
            $"({(overviewRequested ? "visible" : overviewLoadRequested ? "preloading" : "inactive")}), " +
            $"transition {representationState}, desired detail " +
            $"{desiredResidentPercent:F0}% resident; first/50/90/100% " +
            $"{lastDetailFirstVisibleMilliseconds:F0}/" +
            $"{detailResident50Milliseconds:F0}/" +
            $"{detailResident90Milliseconds:F0}/" +
            $"{detailResident100Milliseconds:F0} ms, " +
            $"{residentBytes / (1024f * 1024f):F1} MiB estimated mesh memory; " +
            $"global {global.Renderers:N0} renderers, {global.Triangles:N0} triangles, " +
            $"{global.Bytes / (1024f * 1024f):F1} MiB; " +
            $"queue/pool {pendingCells.Count:N0}/{fragmentPool.Count:N0}, " +
            $"unload queue {pendingUnloads.Count:N0}, " +
            $"loaded/unloaded/cancelled {loadedFragmentCount:N0}/" +
            $"{unloadedFragmentCount:N0}/{cancelledFragmentLoadCount:N0}; " +
            $"pickable {pickableColliderCount:N0}, selection BVH " +
            $"{residentSelectionBvhBytes / (1024f * 1024f):F1} MiB; " +
            $"decode avg/max {totalDecodeMilliseconds / completedLoads:F2}/" +
            $"{maximumDecodeMilliseconds:F2} ms, mesh build avg/max " +
            $"{totalMeshBuildMilliseconds / completedLoads:F2}/" +
            $"{maximumMeshBuildMilliseconds:F2} ms, BVH build avg/max " +
            $"{totalSelectionBuildMilliseconds / completedLoads:F2}/" +
            $"{maximumSelectionBuildMilliseconds:F2} ms, queue wait avg " +
            $"{totalQueueWaitMilliseconds / completedLoads:F1} ms, " +
            $"budget blocks {budgetBlockedCount:N0}.");
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
        if (records != null)
        {
            foreach (var record in records)
            {
                RemoveDetailPickRegistration(record.BoundsCollider);
            }
        }

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
        pickableColliderCount = 0;
        residentSelectionBvhBytes = 0L;
        overviewResidentTriangles = 0;
        overviewResidentRenderers = 0;
        overviewRequested = false;
        overviewLoadRequested = false;
        detailVisible = false;
        detailWanted = false;
        representationState = IfcRepresentationState.OverviewOnly;
        previousCameraSampleTime = float.NegativeInfinity;
        detailPreloadStartedTime = float.NegativeInfinity;
        lastDetailFirstVisibleMilliseconds = -1f;
        detailResident50Milliseconds = -1f;
        detailResident90Milliseconds = -1f;
        detailResident100Milliseconds = -1f;
        budgetBlockedCount = 0;
        totalQueueWaitMilliseconds = 0d;
        loadedFragmentCount = 0;
        unloadedFragmentCount = 0;
        cancelledFragmentLoadCount = 0;
        totalDecodeMilliseconds = 0d;
        maximumDecodeMilliseconds = 0d;
        totalMeshBuildMilliseconds = 0d;
        maximumMeshBuildMilliseconds = 0d;
        totalSelectionBuildMilliseconds = 0d;
        maximumSelectionBuildMilliseconds = 0d;
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
        private static int meshBuildFrame = -1;
        private static double meshBuildMilliseconds;
        private static readonly HashSet<IfcStreamedModel> ModelsBuiltThisFrame = new();

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

        public static bool TryBeginMeshBuild(
            IfcStreamedModel model,
            double maximumMillisecondsPerFrame)
        {
            if (meshBuildFrame != Time.frameCount)
            {
                meshBuildFrame = Time.frameCount;
                meshBuildMilliseconds = 0d;
                ModelsBuiltThisFrame.Clear();
            }

            if (ModelsBuiltThisFrame.Contains(model) ||
                (ModelsBuiltThisFrame.Count > 0 &&
                 meshBuildMilliseconds >= maximumMillisecondsPerFrame))
            {
                return false;
            }

            ModelsBuiltThisFrame.Add(model);
            return true;
        }

        public static void RecordMeshBuild(double elapsedMilliseconds)
        {
            meshBuildMilliseconds += Math.Max(0d, elapsedMilliseconds);
        }

        public static Snapshot GetSnapshot()
        {
            return new Snapshot(triangles, bytes, renderers);
        }
    }
}
