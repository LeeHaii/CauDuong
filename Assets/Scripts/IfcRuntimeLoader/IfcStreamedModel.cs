using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;

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

    public bool Loading;
    public bool Resident;
    public bool Failed;
    public GameObject RuntimeObject;
    public Mesh Mesh;
    public MeshRenderer Renderer;
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
        public Material[] Materials;
    }

    private readonly Dictionary<Vector3Int, CellState> cells = new();
    private readonly Dictionary<int, ElementState> elements = new();
    private readonly HashSet<CellState> desiredCells = new();
    private readonly HashSet<CellState> residentCells = new();
    private readonly HashSet<CellState> forcedCells = new();
    private readonly List<CellState> unloadScratch = new();
    private readonly List<CellCandidate> candidateScratch = new();
    private readonly Queue<CellState> pendingCells = new();
    private readonly Plane[] frustumPlanes = new Plane[6];
    private MaterialPropertyBlock highlightBlock;

    private IReadOnlyDictionary<int, Transform> parentsByProduct;
    private IReadOnlyDictionary<int, Material> materialsByStyle;
    private IReadOnlyList<IfcStreamMeshRecord> records;
    private IfcStreamingSettings settings;
    private string cachePath;
    private double metresPerUnit;
    private Camera viewingCamera;
    private FileStream cacheStream;
    private BinaryReader cacheReader;
    private Coroutine loadRoutine;
    private float nextEvaluationTime;
    private float nextDiagnosticsTime;
    private float nextBudgetRetryTime;
    private long residentTriangles;
    private long residentBytes;
    private int residentRenderers;
    private int loadedFragmentCount;
    private int unloadedFragmentCount;
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

    private void Awake()
    {
        // MaterialPropertyBlock creates a native Unity object internally, so it
        // must not be constructed by a MonoBehaviour field initializer.
        highlightBlock = new MaterialPropertyBlock();
    }

    internal void Initialize(
        string geometryCachePath,
        double modelMetresPerUnit,
        IReadOnlyList<IfcStreamMeshRecord> meshRecords,
        IReadOnlyDictionary<int, Transform> productParents,
        IReadOnlyDictionary<int, Material> styleMaterials,
        IfcStreamingSettings streamingSettings)
    {
        highlightBlock ??= new MaterialPropertyBlock();
        ReleaseAll();
        cachePath = geometryCachePath;
        metresPerUnit = modelMetresPerUnit;
        records = meshRecords;
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

        GlobalBudget.RegisterModel();
        registeredWithGlobalBudget = true;
        IsInitialized = true;
        nextEvaluationTime = 0f;
        nextDiagnosticsTime = Time.unscaledTime + settings.DiagnosticsIntervalSeconds;
        Debug.Log(
            $"IFC streaming index ready for '{name}': {CellCount:N0} cells, " +
            $"{FragmentCount:N0} fragments, {TotalTriangleCount:N0} triangles on disk.");
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

        StartLoadingIfNeeded();
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

        if (hasModelBounds &&
            forcedCells.Count == 0 &&
            TransformBounds(transform, modelLocalBounds).SqrDistance(
                viewingCamera.transform.position) >
            settings.UnloadDistanceMetres * settings.UnloadDistanceMetres)
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
        var now = Time.unscaledTime;
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
        if (cell.Queued || !HasLoadableRecord(cell))
        {
            return;
        }

        cell.Queued = true;
        pendingCells.Enqueue(cell);
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

                record.Loading = true;
                MeshLoadData data = null;
                Exception loadException = null;
                IEnumerator readRoutine = null;
                try
                {
                    readRoutine = ReadMeshRecord(record, value => data = value);
                }
                catch (Exception exception)
                {
                    loadException = exception;
                }

                if (readRoutine != null)
                {
                    while (true)
                    {
                        bool hasNext;
                        try
                        {
                            hasNext = readRoutine.MoveNext();
                        }
                        catch (Exception exception)
                        {
                            loadException = exception;
                            break;
                        }

                        if (!hasNext)
                        {
                            break;
                        }

                        yield return readRoutine.Current;
                    }
                }

                if (loadException != null || data == null)
                {
                    record.Loading = false;
                    record.Failed = true;
                    GlobalBudget.Release(
                        record.TriangleCount,
                        record.EstimatedResidentBytes,
                        1);
                    Debug.LogError(
                        $"Could not stream IFC fragment at {record.Cell}: " +
                        (loadException?.Message ?? "No mesh data was read."));
                    continue;
                }

                try
                {
                    using (LoadMarker.Auto())
                    {
                        CreateResidentRecord(record, data, element, cell);
                    }
                }
                catch (Exception exception)
                {
                    record.Loading = false;
                    record.Failed = true;
                    GlobalBudget.Release(
                        record.TriangleCount,
                        record.EstimatedResidentBytes,
                        1);
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

    private IEnumerator ReadMeshRecord(
        IfcStreamMeshRecord record,
        Action<MeshLoadData> completed)
    {
        EnsureReader();
        cacheStream.Position = record.PayloadOffset;
        var objectName = ReadSafeString(cacheReader);
        var productLabel = cacheReader.ReadInt32();
        if (productLabel != record.ProductLabel)
        {
            throw new InvalidDataException("The IFC stream index does not match its payload.");
        }

        var vertexCount = ReadCount(cacheReader, "vertex");
        if (vertexCount != record.VertexCount)
        {
            throw new InvalidDataException("The IFC vertex count changed after indexing.");
        }

        var vertices = new Vector3[vertexCount];
        for (var index = 0; index < vertexCount; index++)
        {
            vertices[index] = new Vector3(
                cacheReader.ReadSingle(),
                cacheReader.ReadSingle(),
                cacheReader.ReadSingle());
            (vertices[index].y, vertices[index].z) =
                (vertices[index].z, vertices[index].y);
            if ((index + 1) % BinaryItemsPerYield == 0)
            {
                yield return null;
            }
        }

        var normalCount = ReadChannelCount(cacheReader, vertexCount, "normal");
        var normals = new Vector3[normalCount];
        for (var index = 0; index < normalCount; index++)
        {
            normals[index] = new Vector3(
                cacheReader.ReadSingle(),
                cacheReader.ReadSingle(),
                cacheReader.ReadSingle());
            (normals[index].y, normals[index].z) =
                (normals[index].z, normals[index].y);
            normals[index].Normalize();
            if ((index + 1) % BinaryItemsPerYield == 0)
            {
                yield return null;
            }
        }

        var uvCount = ReadChannelCount(cacheReader, vertexCount, "UV");
        var uvs = settings.ImportTextureCoordinates ? new Vector2[uvCount] : null;
        for (var index = 0; index < uvCount; index++)
        {
            var x = cacheReader.ReadSingle();
            var y = cacheReader.ReadSingle();
            if (uvs != null)
            {
                uvs[index] = new Vector2(x, y);
            }

            if ((index + 1) % BinaryItemsPerYield == 0)
            {
                yield return null;
            }
        }

        var tangentCount = ReadChannelCount(cacheReader, vertexCount, "tangent");
        var tangents = settings.ImportTangents ? new Vector4[tangentCount] : null;
        for (var index = 0; index < tangentCount; index++)
        {
            var x = cacheReader.ReadSingle();
            var y = cacheReader.ReadSingle();
            var z = cacheReader.ReadSingle();
            var w = cacheReader.ReadSingle();
            if (tangents != null)
            {
                tangents[index] = new Vector4(x, z, y, -w);
            }

            if ((index + 1) % BinaryItemsPerYield == 0)
            {
                yield return null;
            }
        }

        var subMeshCount = ReadCount(cacheReader, "sub-mesh");
        if (subMeshCount == 0)
        {
            throw new InvalidDataException("An IFC fragment has no sub-meshes.");
        }

        var subMeshes = new int[subMeshCount][];
        var materials = new Material[subMeshCount];
        var totalIndexCount = 0;
        for (var subMesh = 0; subMesh < subMeshCount; subMesh++)
        {
            var styleLabel = cacheReader.ReadInt32();
            var indexCount = ReadCount(cacheReader, "index");
            if (indexCount % 3 != 0)
            {
                throw new InvalidDataException("An IFC fragment has non-triangular indices.");
            }

            totalIndexCount += indexCount;
            var indices = new int[indexCount];
            for (var index = 0; index < indexCount; index++)
            {
                indices[index] = cacheReader.ReadInt32();
                if (indices[index] < 0 || indices[index] >= vertexCount)
                {
                    throw new InvalidDataException("An IFC fragment index is out of range.");
                }

                if ((index + 1) % BinaryItemsPerYield == 0)
                {
                    yield return null;
                }
            }

            for (var index = 0; index + 2 < indices.Length; index += 3)
            {
                (indices[index + 1], indices[index + 2]) =
                    (indices[index + 2], indices[index + 1]);
            }

            subMeshes[subMesh] = indices;
            if (!materialsByStyle.TryGetValue(styleLabel, out var material))
            {
                materialsByStyle.TryGetValue(0, out material);
            }

            materials[subMesh] = material;
        }

        if (totalIndexCount != record.IndexCount)
        {
            throw new InvalidDataException("The IFC index count changed after indexing.");
        }

        completed(new MeshLoadData
        {
            Name = objectName,
            ProductLabel = productLabel,
            Vertices = vertices,
            Normals = normals,
            Uvs = uvs,
            Tangents = tangents,
            SubMeshes = subMeshes,
            Materials = materials
        });
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

        var fragment = new GameObject(data.Name);
        var parent = parentsByProduct.TryGetValue(data.ProductLabel, out var productParent)
            ? productParent
            : transform;
        fragment.transform.SetParent(parent, false);
        fragment.AddComponent<MeshFilter>().sharedMesh = mesh;
        var renderer = fragment.AddComponent<MeshRenderer>();
        renderer.sharedMaterials = data.Materials;
        renderer.enabled = element.Visible;

        var keepReadable = false;
        if (settings.GenerateMeshColliders)
        {
            var hasUsableColliderTriangle = ContainsUsableColliderTriangle(
                data.Vertices,
                data.SubMeshes);
            if (hasUsableColliderTriangle &&
                record.TriangleCount <= settings.MaximumMeshColliderTriangles)
            {
                var collider = fragment.AddComponent<MeshCollider>();
                collider.cookingOptions = MeshColliderCookingOptions.UseFastMidphase;
                collider.sharedMesh = mesh;
            }
            else if (hasUsableColliderTriangle)
            {
                var collider = fragment.AddComponent<BoxCollider>();
                collider.center = mesh.bounds.center;
                collider.size = mesh.bounds.size;
                keepReadable = true;
            }
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
            UnloadRecord(record, cell);
        }

        residentCells.Remove(cell);
    }

    private void UnloadRecord(IfcStreamMeshRecord record, CellState cell)
    {
        if (!record.Resident)
        {
            if (record.Loading)
            {
                record.Loading = false;
                GlobalBudget.Release(
                    record.TriangleCount,
                    record.EstimatedResidentBytes,
                    1);
            }

            return;
        }

        if (elements.TryGetValue(record.ProductLabel, out var element))
        {
            element.Renderers.Remove(record.Renderer);
        }

        DestroyOwned(record.RuntimeObject);
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
        GlobalBudget.Release(
            record.TriangleCount,
            record.EstimatedResidentBytes,
            1);
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

    private void EnsureReader()
    {
        if (cacheReader != null)
        {
            return;
        }

        cacheStream = new FileStream(
            cachePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            1024 * 1024,
            FileOptions.RandomAccess);
        cacheReader = new BinaryReader(cacheStream, Encoding.UTF8, true);
    }

    private void LogDiagnostics()
    {
        var global = GlobalBudget.GetSnapshot();
        Debug.Log(
            $"IFC streaming '{name}': {residentRenderers:N0}/{FragmentCount:N0} " +
            $"fragments resident, {residentTriangles:N0}/{TotalTriangleCount:N0} triangles, " +
            $"{residentBytes / (1024f * 1024f):F1} MiB estimated mesh memory; " +
            $"global {global.Renderers:N0} renderers, {global.Triangles:N0} triangles, " +
            $"{global.Bytes / (1024f * 1024f):F1} MiB; " +
            $"loaded/unloaded {loadedFragmentCount:N0}/{unloadedFragmentCount:N0}.");
    }

    private void ReleaseAll()
    {
        if (loadRoutine != null)
        {
            StopCoroutine(loadRoutine);
            loadRoutine = null;
        }

        foreach (var cell in cells.Values)
        {
            foreach (var record in cell.Records)
            {
                UnloadRecord(record, cell);
            }
        }

        cacheReader?.Dispose();
        cacheStream?.Dispose();
        cacheReader = null;
        cacheStream = null;
        cells.Clear();
        elements.Clear();
        desiredCells.Clear();
        residentCells.Clear();
        forcedCells.Clear();
        pendingCells.Clear();
        candidateScratch.Clear();
        unloadScratch.Clear();
        TotalTriangleCount = 0;
        residentTriangles = 0;
        residentBytes = 0;
        residentRenderers = 0;
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
