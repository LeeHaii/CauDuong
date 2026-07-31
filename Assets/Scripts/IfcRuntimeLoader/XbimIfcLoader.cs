using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityMeshSimplifier;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;

public sealed class XbimIfcLoader : MonoBehaviour
{
    private const uint MeshFileMagic = 0x4D494258;
    private const int RawMeshFileVersion = 2;
    private const int OptimizedMeshFileVersion = 3;
    private const int OptimizationRevision = 2;
    private const int MaxRecordCount = 10_000_000;
    private const int MaxStringLength = 1_000_000;
    private const string SourcePathProperty = "Source IFC Path";

    [Header("Generated Model")]
    [SerializeField] private Transform modelParent;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private bool generateMeshColliders;

    [Header("Geometry Optimization")]
    [Tooltip("Maximum curve-to-segment deviation in millimetres. Higher values reduce triangle counts.")]
    [SerializeField, Min(0.01f)] private float linearDeflectionMillimetres = 5f;
    [Tooltip("Maximum angular deviation in degrees. Higher values reduce segments around curves.")]
    [SerializeField, Range(1f, 90f)] private float angularDeflectionDegrees = 30f;

    [Header("Post-Tessellation Simplification")]
    [SerializeField] private bool simplifyMeshes = true;
    [Tooltip("Meshes below this triangle count are kept intact.")]
    [SerializeField, Min(12)] private int minimumSimplificationTriangles = 1_000;
    [SerializeField, Range(0.05f, 1f)] private float standardMeshQuality = 0.65f;
    [SerializeField, Min(1000)] private int aggressiveSimplificationTriangles = 5_000;
    [SerializeField, Range(0.03f, 1f)] private float aggressiveMeshQuality = 0.45f;
    [SerializeField, Min(5000)] private int extremeSimplificationTriangles = 20_000;
    [SerializeField, Range(0.02f, 1f)] private float extremeMeshQuality = 0.3f;
    [Tooltip("Quality used for broad road, terrain, marking, and pavement surfaces.")]
    [SerializeField, Range(0.02f, 1f)] private float broadSurfaceMeshQuality = 0.55f;
    [Tooltip("Boundary-preserving QEM is slower on first import but does not weld unrelated IFC patches.")]
    [SerializeField] private bool useQuadricSimplification = true;
    [SerializeField] private bool preserveBoundaryEdges = true;

    [Header("Mesh Memory")]
    [Tooltip("IFC colors do not require UVs unless textured materials are added later.")]
    [SerializeField] private bool importTextureCoordinates;
    [SerializeField] private bool importTangents;
    [Tooltip("Releases CPU-side mesh copies after physics cooking and GPU upload.")]
    [SerializeField] private bool releaseCpuMeshData = true;

    [Header("Import Budget")]
    [Min(1)]
    [SerializeField] private int meshesPerFrame = 16;
    [Tooltip("Yield after processing approximately this many source triangles.")]
    [Min(1_000)]
    [SerializeField] private int sourceTrianglesPerFrame = 40_000;
    [Tooltip("Maximum time allowed for one native xBIM conversion.")]
    [Min(30f)]
    [SerializeField] private float converterTimeoutSeconds = 900f;

    private readonly List<GameObject> loadedModels = new();
    private readonly Dictionary<GameObject, List<Material>> modelMaterials = new();
    private readonly Dictionary<GameObject, string> modelSourcePaths = new();
    private readonly Queue<string> pendingImports = new();
    private Dictionary<int, Material> activeMaterialCache = new();
    private GameObject loadedModel;
    private GameObject importingModel;
    private Coroutine loadRoutine;

    public bool IsLoading { get; private set; }
    public GameObject LoadedModel => loadedModel;
    public IReadOnlyList<GameObject> LoadedModels => loadedModels;
    public long LastSourceTriangleCount { get; private set; }
    public long LastOptimizedTriangleCount { get; private set; }
    public float LastTriangleReduction =>
        LastSourceTriangleCount > 0
            ? 1f - (float)LastOptimizedTriangleCount / LastSourceTriangleCount
            : 0f;
    public float LinearDeflection
    {
        get => linearDeflectionMillimetres;
        set => linearDeflectionMillimetres = Mathf.Clamp(value, 0.01f, 1000f);
    }

    public float AngularDeflection
    {
        get => angularDeflectionDegrees;
        set => angularDeflectionDegrees = Mathf.Clamp(value, 1f, 90f);
    }

    public event Action<string> StatusChanged;
    public event Action<GameObject> LoadCompleted;
    public event Action<string> LoadFailed;
    public event Action ModelsChanged;

    private void Awake()
    {
        if (!TryGetComponent<IfcGeoPositionExtractor>(out _))
        {
            gameObject.AddComponent<IfcGeoPositionExtractor>();
        }
    }

    private void OnEnable()
    {
        RecoverLoadedModelsAfterDomainReload();
    }

    public void LoadIFC(string path)
    {
        if (IsLoading || loadRoutine != null)
        {
            pendingImports.Enqueue(path);
            SetStatus($"Queued IFC import: {Path.GetFileName(path)}");
            return;
        }

        loadRoutine = StartCoroutine(LoadIfcRoutine(path));
    }

    public string GetModelSourcePath(GameObject model)
    {
        return model != null && modelSourcePaths.TryGetValue(model, out var path)
            ? path
            : string.Empty;
    }

    public void RemoveModel(GameObject model)
    {
        if (model == null || model == importingModel || !loadedModels.Remove(model))
        {
            return;
        }

        modelSourcePaths.Remove(model);
        DestroyModelResources(model);
        loadedModel = loadedModels.Count > 0 ? loadedModels[^1] : null;
        ModelsChanged?.Invoke();
    }

    public void ClearModels()
    {
        for (var index = loadedModels.Count - 1; index >= 0; index--)
        {
            var model = loadedModels[index];
            modelSourcePaths.Remove(model);
            DestroyModelResources(model);
        }

        loadedModels.Clear();
        loadedModel = null;
        ModelsChanged?.Invoke();
    }

    private IEnumerator LoadIfcRoutine(string path)
    {
        IsLoading = true;

        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            Fail($"IFC file not found: {path}");
            yield break;
        }

        var converterPath = GetConverterPath();
        if (!File.Exists(converterPath))
        {
            Fail(
                "The xBIM converter is missing. Publish Tools/XbimIfcConverter " +
                "before testing in the Editor, or build the Windows player once.");
            yield break;
        }

        var cacheDirectory = Path.Combine(
            Application.persistentDataPath,
            "XbimIfcCache");
        Directory.CreateDirectory(cacheDirectory);
        var meshPath = GetConvertedMeshCachePath(path, cacheDirectory);
        if (IsMeshFileValid(meshPath, RawMeshFileVersion))
        {
            SetStatus($"Loading cached IFC geometry: {Path.GetFileName(path)}");
        }
        else
        {
            DeleteTemporaryFile(meshPath);
            var partialPath =
                meshPath + "." + Guid.NewGuid().ToString("N") + ".partial";
            SetStatus(
                $"Converting IFC geometry at {LinearDeflection:G4} mm / " +
                $"{AngularDeflection:G4} deg deflection...");

            Process process;
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = converterPath,
                    Arguments = BuildConverterArguments(path, partialPath),
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    WorkingDirectory = Path.GetDirectoryName(converterPath)
                };

                process = new Process { StartInfo = startInfo };
                process.Start();
            }
            catch (Exception exception)
            {
                DeleteTemporaryFile(partialPath);
                Fail($"Could not start the xBIM converter: {exception.Message}");
                yield break;
            }

            var conversionTimer = Stopwatch.StartNew();
            using (process)
            {
                while (!IsFileReady(partialPath))
                {
                    if (process.HasExited)
                    {
                        var exitCode = process.ExitCode;
                        DeleteTemporaryFile(partialPath);
                        Fail(
                            exitCode == 0
                                ? "xBIM conversion completed without producing mesh data."
                                : $"xBIM conversion failed with exit code {exitCode}.");
                        yield break;
                    }

                    if (conversionTimer.Elapsed.TotalSeconds >=
                        converterTimeoutSeconds)
                    {
                        TryTerminateProcess(process);
                        DeleteTemporaryFile(partialPath);
                        Fail(
                            $"xBIM conversion timed out after " +
                            $"{converterTimeoutSeconds:G0} seconds.");
                        yield break;
                    }

                    yield return null;
                }
            }

            try
            {
                if (File.Exists(meshPath))
                {
                    File.Delete(meshPath);
                }

                File.Move(partialPath, meshPath);
            }
            catch (Exception exception)
            {
                DeleteTemporaryFile(partialPath);
                Fail($"Could not store the IFC geometry cache: {exception.Message}");
                yield break;
            }
        }

        if (!IsMeshFileValid(meshPath, RawMeshFileVersion))
        {
            Fail("xBIM conversion completed without producing mesh data.");
            yield break;
        }

        var optimizedMeshPath = GetOptimizedMeshCachePath(path, cacheDirectory);
        var hasOptimizedCache = IsMeshFileValid(
            optimizedMeshPath,
            OptimizedMeshFileVersion);
        if (!hasOptimizedCache)
        {
            DeleteTemporaryFile(optimizedMeshPath);
        }

        SetStatus(
            hasOptimizedCache
                ? "Creating Unity objects from optimized mesh cache..."
                : "Creating and optimizing Unity meshes...");

        Exception importException = null;
        IEnumerator importRoutine = null;

        try
        {
            importRoutine = ImportMeshFile(
                hasOptimizedCache ? optimizedMeshPath : meshPath,
                Path.GetFileNameWithoutExtension(path),
                path,
                hasOptimizedCache ? null : optimizedMeshPath);
        }
        catch (Exception exception)
        {
            importException = exception;
        }

        if (importException == null)
        {
            while (true)
            {
                bool hasNext;
                try
                {
                    hasNext = importRoutine.MoveNext();
                }
                catch (Exception exception)
                {
                    importException = exception;
                    break;
                }

                if (!hasNext)
                {
                    break;
                }

                yield return importRoutine.Current;
            }
        }

        if (importException != null)
        {
            Fail($"Could not create Unity meshes: {importException.Message}");
            yield break;
        }

        IsLoading = false;
        loadRoutine = null;
        var lodController = loadedModel.GetComponent<IfcModelLodController>() ??
                            loadedModel.AddComponent<IfcModelLodController>();
        lodController.Rebuild();
        loadedModels.Add(loadedModel);
        modelSourcePaths[loadedModel] = path;
        importingModel = null;
        SetStatus(
            $"IFC import complete: {LastSourceTriangleCount:N0} to " +
            $"{LastOptimizedTriangleCount:N0} triangles " +
            $"({LastTriangleReduction:P1} reduction).");
        LoadCompleted?.Invoke(loadedModel);
        ModelsChanged?.Invoke();
        StartNextQueuedImport();
    }

    private static bool IsFileReady(string path)
    {
        if (!File.Exists(path))
        {
            return false;
        }

        try
        {
            using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
            return stream.Length > 0;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    private IEnumerator ImportMeshFile(
        string path,
        string modelName,
        string sourcePath,
        string optimizedCachePath)
    {
        using var stream = File.OpenRead(path);
        using var reader = new BinaryReader(stream, Encoding.UTF8);

        if (reader.ReadUInt32() != MeshFileMagic)
        {
            throw new InvalidDataException("The converter produced an unknown mesh format.");
        }

        var version = reader.ReadInt32();
        if (version != RawMeshFileVersion &&
            version != OptimizedMeshFileVersion)
        {
            throw new InvalidDataException($"Unsupported xBIM mesh version: {version}.");
        }

        var isOptimizedCache = version == OptimizedMeshFileVersion;
        var cachedSourceTriangleCount = isOptimizedCache
            ? reader.ReadInt64()
            : 0L;
        var metresPerUnit = reader.ReadDouble();
        if (!double.IsFinite(metresPerUnit) || metresPerUnit <= 0d || metresPerUnit > 1_000_000d)
        {
            throw new InvalidDataException($"Invalid IFC length scale: {metresPerUnit}.");
        }

        var originX = reader.ReadDouble();
        var originY = reader.ReadDouble();
        var originZ = reader.ReadDouble();

        var styles = ReadStyles(reader);
        var hierarchy = ReadHierarchy(reader);

        var meshCount = reader.ReadInt32();
        ValidateRecordCount(meshCount, "mesh");

        var optimizedPartialPath = string.IsNullOrWhiteSpace(optimizedCachePath)
            ? null
            : optimizedCachePath + ".partial";
        if (optimizedPartialPath != null)
        {
            DeleteTemporaryFile(optimizedPartialPath);
        }

        using var optimizedCache = optimizedPartialPath != null
            ? new OptimizedMeshCacheWriter(
                optimizedPartialPath,
                optimizedCachePath)
            : null;
        var optimizedWriter = optimizedCache?.Writer;
        if (optimizedWriter != null)
        {
            WriteOptimizedHeader(
                optimizedWriter,
                metresPerUnit,
                originX,
                originY,
                originZ,
                styles,
                hierarchy,
                meshCount);
        }

        loadedModel = new GameObject(modelName);
        importingModel = loadedModel;
        activeMaterialCache = new Dictionary<int, Material>();
        LastSourceTriangleCount = isOptimizedCache
            ? cachedSourceTriangleCount
            : 0;
        LastOptimizedTriangleCount = 0;
        modelMaterials[loadedModel] = new List<Material>();
        modelSourcePaths[loadedModel] = sourcePath;
        if (modelParent != null)
        {
            loadedModel.transform.SetParent(modelParent, false);
        }

        loadedModel.transform.localScale = Vector3.one * (float)metresPerUnit;

        var modelMetadata = loadedModel.AddComponent<IfcMetadataComponent>();
        var modelProperties = new List<KeyValuePair<string, string>>
        {
            new(
                "Length Scale (metres/unit)",
                metresPerUnit.ToString("G9")),
            new(SourcePathProperty, sourcePath),
            new(
                "Local Origin (IFC coordinates)",
                $"{originX:G9}, {originY:G9}, {originZ:G9}"),
            new(
                "Linear Deflection (mm)",
                LinearDeflection.ToString("G9", CultureInfo.InvariantCulture)),
            new(
                "Angular Deflection (degrees)",
                AngularDeflection.ToString("G9", CultureInfo.InvariantCulture)),
            new(
                "Post-Tessellation Simplification",
                simplifyMeshes
                    ? useQuadricSimplification
                        ? "Adaptive clustering + QEM"
                        : "Adaptive clustering"
                    : "Disabled")
        };
        modelMetadata.Initialize("IfcModel", string.Empty, 0, modelProperties);

        var hierarchyObjects = CreateHierarchy(hierarchy, loadedModel.transform);
        var processedTrianglesThisFrame = 0;

        for (var meshIndex = 0; meshIndex < meshCount; meshIndex++)
        {
            var objectName = ReadSafeString(reader, "mesh name");
            var productLabel = reader.ReadInt32();

            var vertexCount = reader.ReadInt32();
            ValidateCount(vertexCount, "vertex");

            var vertices = new Vector3[vertexCount];
            for (var vertexIndex = 0; vertexIndex < vertexCount; vertexIndex++)
            {
                var x = reader.ReadSingle();
                var y = reader.ReadSingle();
                var z = reader.ReadSingle();
                vertices[vertexIndex] = new Vector3(x, z, y);
            }

            var normalCount = reader.ReadInt32();
            ValidateChannelCount(normalCount, vertexCount, "normal");
            var normals = new Vector3[normalCount];
            for (var normalIndex = 0; normalIndex < normalCount; normalIndex++)
            {
                var x = reader.ReadSingle();
                var y = reader.ReadSingle();
                var z = reader.ReadSingle();
                normals[normalIndex] = new Vector3(x, z, y).normalized;
            }

            var uvCount = reader.ReadInt32();
            ValidateChannelCount(uvCount, vertexCount, "UV");
            var uvs = importTextureCoordinates ? new Vector2[uvCount] : null;
            for (var uvIndex = 0; uvIndex < uvCount; uvIndex++)
            {
                var x = reader.ReadSingle();
                var y = reader.ReadSingle();
                if (uvs != null)
                {
                    uvs[uvIndex] = new Vector2(x, y);
                }
            }

            var tangentCount = reader.ReadInt32();
            ValidateChannelCount(tangentCount, vertexCount, "tangent");
            var tangents = importTangents ? new Vector4[tangentCount] : null;
            for (var tangentIndex = 0; tangentIndex < tangentCount; tangentIndex++)
            {
                var x = reader.ReadSingle();
                var y = reader.ReadSingle();
                var z = reader.ReadSingle();
                var w = reader.ReadSingle();
                if (tangents != null)
                {
                    tangents[tangentIndex] = new Vector4(x, z, y, -w);
                }
            }

            var subMeshCount = reader.ReadInt32();
            ValidateRecordCount(subMeshCount, "sub-mesh");
            if (subMeshCount == 0)
            {
                throw new InvalidDataException($"Mesh '{objectName}' has no sub-meshes.");
            }

            var subMeshes = new int[subMeshCount][];
            var materials = new Material[subMeshCount];
            var styleLabels = new int[subMeshCount];
            for (var subMeshIndex = 0; subMeshIndex < subMeshCount; subMeshIndex++)
            {
                var styleLabel = reader.ReadInt32();
                styleLabels[subMeshIndex] = styleLabel;
                var indexCount = reader.ReadInt32();
                ValidateCount(indexCount, "index");
                if (indexCount % 3 != 0)
                {
                    throw new InvalidDataException(
                        $"Mesh '{objectName}' has a non-triangular index count.");
                }

                var indices = new int[indexCount];
                for (var index = 0; index < indexCount; index++)
                {
                    indices[index] = reader.ReadInt32();
                    if (indices[index] < 0 || indices[index] >= vertexCount)
                    {
                        throw new InvalidDataException(
                            $"Mesh '{objectName}' contains an out-of-range index.");
                    }
                }

                for (var index = 0; index + 2 < indices.Length; index += 3)
                {
                    (indices[index + 1], indices[index + 2]) =
                        (indices[index + 2], indices[index + 1]);
                }

                subMeshes[subMeshIndex] = indices;
                materials[subMeshIndex] = GetMaterial(
                    styles.TryGetValue(styleLabel, out var style)
                        ? style
                        : IfcMaterialStyle.Default(styleLabel));
            }

            var mesh = new Mesh
            {
                name = objectName,
                indexFormat = vertexCount > ushort.MaxValue
                    ? IndexFormat.UInt32
                    : IndexFormat.UInt16,
                vertices = vertices
            };

            if (normalCount == vertexCount)
            {
                mesh.normals = normals;
            }

            if (uvs != null && uvCount == vertexCount)
            {
                mesh.uv = uvs;
            }

            if (tangents != null && tangentCount == vertexCount)
            {
                mesh.tangents = tangents;
            }

            mesh.subMeshCount = subMeshCount;
            for (var subMeshIndex = 0; subMeshIndex < subMeshCount; subMeshIndex++)
            {
                mesh.SetTriangles(subMeshes[subMeshIndex], subMeshIndex, false);
            }

            if (normalCount != vertexCount)
            {
                mesh.RecalculateNormals();
            }

            if (importTangents &&
                tangentCount != vertexCount &&
                importTextureCoordinates &&
                uvCount == vertexCount)
            {
                mesh.RecalculateTangents();
            }

            mesh.RecalculateBounds();
            var sourceTriangleCount = CountTriangles(subMeshes);
            if (!isOptimizedCache)
            {
                LastSourceTriangleCount += sourceTriangleCount;
            }

            processedTrianglesThisFrame += sourceTriangleCount;
            if (!isOptimizedCache)
            {
                mesh = SimplifyMesh(mesh, sourceTriangleCount);
            }

            LastOptimizedTriangleCount += CountTriangles(mesh);
            if (optimizedWriter != null)
            {
                WriteOptimizedMesh(
                    optimizedWriter,
                    objectName,
                    productLabel,
                    mesh,
                    styleLabels);
            }

            var element = new GameObject(objectName);
            var parent = hierarchyObjects.TryGetValue(productLabel, out var productObject)
                ? productObject.transform
                : loadedModel.transform;
            element.transform.SetParent(parent, false);
            element.AddComponent<MeshFilter>().sharedMesh = mesh;
            element.AddComponent<MeshRenderer>().sharedMaterials = materials;

            if (generateMeshColliders)
            {
                var meshCollider = element.AddComponent<MeshCollider>();
                meshCollider.cookingOptions =
                    MeshColliderCookingOptions.CookForFasterSimulation |
                    MeshColliderCookingOptions.EnableMeshCleaning |
                    MeshColliderCookingOptions.WeldColocatedVertices |
                    MeshColliderCookingOptions.UseFastMidphase;
                meshCollider.sharedMesh = mesh;
            }

            if (releaseCpuMeshData)
            {
                mesh.UploadMeshData(true);
            }

            if ((meshIndex + 1) % meshesPerFrame == 0 ||
                processedTrianglesThisFrame >= sourceTrianglesPerFrame)
            {
                SetStatus($"Creating Unity meshes... {meshIndex + 1}/{meshCount}");
                processedTrianglesThisFrame = 0;
                yield return null;
            }
        }

        if (optimizedWriter != null)
        {
            optimizedCache.Commit(LastSourceTriangleCount);
        }

        var reductionPercent = LastTriangleReduction * 100f;
        modelProperties.Add(new KeyValuePair<string, string>(
            "Source Triangles",
            LastSourceTriangleCount.ToString("N0", CultureInfo.InvariantCulture)));
        modelProperties.Add(new KeyValuePair<string, string>(
            "Optimized Triangles",
            LastOptimizedTriangleCount.ToString("N0", CultureInfo.InvariantCulture)));
        modelProperties.Add(new KeyValuePair<string, string>(
            "Triangle Reduction",
            $"{reductionPercent:F1}%"));
        modelMetadata.Initialize("IfcModel", string.Empty, 0, modelProperties);
    }

    private Mesh SimplifyMesh(Mesh sourceMesh, int sourceTriangleCount)
    {
        if (!simplifyMeshes ||
            sourceTriangleCount < minimumSimplificationTriangles)
        {
            OptimizeMeshBuffers(sourceMesh);
            return sourceMesh;
        }

        var quality = sourceTriangleCount >= extremeSimplificationTriangles
            ? extremeMeshQuality
            : sourceTriangleCount >= aggressiveSimplificationTriangles
                ? aggressiveMeshQuality
                : standardMeshQuality;
        var isBroadSurface = IsBroadSurfaceMesh(sourceMesh.name);
        if (isBroadSurface)
        {
            quality = Mathf.Min(quality, broadSurfaceMeshQuality);
        }

        if (!useQuadricSimplification)
        {
            OptimizeMeshBuffers(sourceMesh);
            return sourceMesh;
        }

        var resultMesh = sourceMesh;
        try
        {
            var options = SimplificationOptions.Default;
            options.PreserveBorderEdges = preserveBoundaryEdges;
            options.PreserveUVSeamEdges = importTextureCoordinates;
            options.PreserveUVFoldoverEdges = importTextureCoordinates;
            options.PreserveSurfaceCurvature = false;
            options.EnableSmartLink = true;
            options.MaxIterationCount = 100;
            options.Agressiveness = 7d;

            var simplifier = new MeshSimplifier
            {
                SimplificationOptions = options
            };
            simplifier.Initialize(sourceMesh);
            simplifier.SimplifyMesh(quality);

            var simplifiedMesh = simplifier.ToMesh();
            simplifiedMesh.name = sourceMesh.name;
            simplifiedMesh.RecalculateBounds();

            var simplifiedTriangleCount = CountTriangles(simplifiedMesh);
            if (simplifiedTriangleCount > 0 &&
                simplifiedTriangleCount < sourceTriangleCount)
            {
                resultMesh = simplifiedMesh;
            }
            else
            {
                Destroy(simplifiedMesh);
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                $"Could not simplify IFC mesh '{sourceMesh.name}': {exception.Message}");
        }

        OptimizeMeshBuffers(resultMesh);
        if (resultMesh != sourceMesh)
        {
            Destroy(sourceMesh);
        }

        return resultMesh;
    }

    private static int CountTriangles(IReadOnlyList<int[]> subMeshes)
    {
        var count = 0;
        foreach (var indices in subMeshes)
        {
            count += indices.Length / 3;
        }

        return count;
    }

    private static int CountTriangles(Mesh mesh)
    {
        var count = 0;
        for (var subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
        {
            count += (int)(mesh.GetIndexCount(subMeshIndex) / 3);
        }

        return count;
    }

    private static void OptimizeMeshBuffers(Mesh mesh)
    {
        mesh.OptimizeIndexBuffers();
        mesh.OptimizeReorderVertexBuffer();
    }

    private static bool IsBroadSurfaceMesh(string meshName)
    {
        if (string.IsNullOrWhiteSpace(meshName))
        {
            return false;
        }

        var value = meshName.ToLowerInvariant();
        return value.Contains("topo") ||
               value.Contains("pave") ||
               value.Contains("thảm") ||
               value.Contains("tham") ||
               value.Contains("mặt đường") ||
               value.Contains("mat duong") ||
               value.Contains("vạch") ||
               value.Contains("vach") ||
               value.Contains("taluy") ||
               value.Contains("ledat");
    }

    private Dictionary<int, IfcMaterialStyle> ReadStyles(BinaryReader reader)
    {
        var styleCount = reader.ReadInt32();
        ValidateRecordCount(styleCount, "material style");
        var styles = new Dictionary<int, IfcMaterialStyle>(styleCount);

        for (var index = 0; index < styleCount; index++)
        {
            var label = reader.ReadInt32();
            var style = new IfcMaterialStyle(
                label,
                ReadSafeString(reader, "material name"),
                new Color(
                    Mathf.Clamp01(reader.ReadSingle()),
                    Mathf.Clamp01(reader.ReadSingle()),
                    Mathf.Clamp01(reader.ReadSingle()),
                    Mathf.Clamp01(reader.ReadSingle())),
                new Color(
                    Mathf.Clamp01(reader.ReadSingle()),
                    Mathf.Clamp01(reader.ReadSingle()),
                    Mathf.Clamp01(reader.ReadSingle()),
                    1f),
                Mathf.Clamp01(reader.ReadSingle()));
            styles[label] = style;
        }

        return styles;
    }

    private List<IfcHierarchyNode> ReadHierarchy(BinaryReader reader)
    {
        var nodeCount = reader.ReadInt32();
        ValidateRecordCount(nodeCount, "hierarchy node");
        var nodes = new List<IfcHierarchyNode>(nodeCount);

        for (var index = 0; index < nodeCount; index++)
        {
            var label = reader.ReadInt32();
            var parentLabel = reader.ReadInt32();
            var name = ReadSafeString(reader, "hierarchy name");
            var ifcType = ReadSafeString(reader, "IFC type");
            var globalId = ReadSafeString(reader, "GlobalId");
            var propertyCount = reader.ReadInt32();
            ValidateRecordCount(propertyCount, "metadata property");

            var properties = new List<KeyValuePair<string, string>>(propertyCount);
            for (var propertyIndex = 0; propertyIndex < propertyCount; propertyIndex++)
            {
                properties.Add(new KeyValuePair<string, string>(
                    ReadSafeString(reader, "metadata key"),
                    ReadSafeString(reader, "metadata value")));
            }

            nodes.Add(new IfcHierarchyNode(
                label,
                parentLabel,
                name,
                ifcType,
                globalId,
                properties));
        }

        return nodes;
    }

    private static Dictionary<int, GameObject> CreateHierarchy(
        IReadOnlyList<IfcHierarchyNode> nodes,
        Transform root)
    {
        var objects = new Dictionary<int, GameObject>(nodes.Count);

        foreach (var node in nodes)
        {
            var nodeObject = new GameObject(node.Name);
            nodeObject.transform.SetParent(root, false);
            nodeObject.AddComponent<IfcMetadataComponent>()
                .Initialize(node.IfcType, node.GlobalId, node.Label, node.Properties);
            objects[node.Label] = nodeObject;
        }

        foreach (var node in nodes)
        {
            if (node.ParentLabel != 0 &&
                objects.TryGetValue(node.Label, out var child) &&
                objects.TryGetValue(node.ParentLabel, out var parent))
            {
                child.transform.SetParent(parent.transform, false);
            }
        }

        return objects;
    }

    private Material GetMaterial(IfcMaterialStyle style)
    {
        if (activeMaterialCache.TryGetValue(style.Label, out var cached))
        {
            return cached;
        }

        Material material;
        if (defaultMaterial != null && style.Label == 0)
        {
            material = new Material(defaultMaterial);
        }
        else
        {
            var shader = GetIfcShader();
            if (shader == null)
            {
                throw new InvalidOperationException("No compatible IFC shader was found.");
            }

            material = new Material(shader);
        }

        material.name = string.IsNullOrWhiteSpace(style.Name)
            ? $"IFC Material {style.Label}"
            : style.Name;

        SetColorIfPresent(material, "_Color", style.Diffuse);
        SetColorIfPresent(material, "_BaseColor", style.Diffuse);
        SetColorIfPresent(material, "_SpecColor", style.Specular);
        SetColorIfPresent(material, "_IfcSpecColor", style.Specular);
        SetFloatIfPresent(material, "_Smoothness", style.Smoothness);
        SetFloatIfPresent(material, "_Glossiness", style.Smoothness);
        SetFloatIfPresent(material, "_Cull", (float)CullMode.Off);
        SetFloatIfPresent(material, "_CullMode", (float)CullMode.Off);
        SetFloatIfPresent(material, "_CullModeForward", (float)CullMode.Off);
        SetFloatIfPresent(material, "_DoubleSidedEnable", 1f);

        ConfigureTransparency(material, style.Diffuse.a);
        activeMaterialCache[style.Label] = material;
        if (importingModel != null &&
            modelMaterials.TryGetValue(importingModel, out var materials))
        {
            materials.Add(material);
        }

        return material;
    }

    private static Shader GetIfcShader()
    {
        if (GraphicsSettings.currentRenderPipeline == null)
        {
            return Resources.Load<Shader>("Shaders/IfcDoubleSided") ??
                   Shader.Find("CauDuong/IFC Double Sided") ??
                   Shader.Find("Standard");
        }

        return Shader.Find("Universal Render Pipeline/Lit") ??
               Shader.Find("HDRP/Lit") ??
               Shader.Find("Standard");
    }

    private static void ConfigureTransparency(Material material, float alpha)
    {
        var transparent = alpha < 0.999f;
        SetFloatIfPresent(material, "_Surface", transparent ? 1f : 0f);
        SetFloatIfPresent(material, "_SrcBlend", transparent
            ? (float)BlendMode.SrcAlpha
            : (float)BlendMode.One);
        SetFloatIfPresent(material, "_DstBlend", transparent
            ? (float)BlendMode.OneMinusSrcAlpha
            : (float)BlendMode.Zero);
        SetFloatIfPresent(material, "_ZWrite", transparent ? 0f : 1f);

        if (transparent)
        {
            material.SetOverrideTag("RenderType", "Transparent");
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.renderQueue = (int)RenderQueue.Transparent;
        }
        else
        {
            material.SetOverrideTag("RenderType", "Opaque");
            material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.renderQueue = -1;
        }
    }

    private static void SetColorIfPresent(Material material, string property, Color value)
    {
        if (material.HasProperty(property))
        {
            material.SetColor(property, value);
        }
    }

    private static void SetFloatIfPresent(Material material, string property, float value)
    {
        if (material.HasProperty(property))
        {
            material.SetFloat(property, value);
        }
    }

    private void DestroyModelResources(GameObject model)
    {
        if (model == null)
        {
            return;
        }

        foreach (var meshFilter in model.GetComponentsInChildren<MeshFilter>(true))
        {
            if (meshFilter.sharedMesh != null)
            {
                Destroy(meshFilter.sharedMesh);
            }
        }

        if (!modelMaterials.Remove(model, out var materials))
        {
            var uniqueMaterials = new HashSet<Material>();
            foreach (var renderer in model.GetComponentsInChildren<Renderer>(true))
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material != null)
                    {
                        uniqueMaterials.Add(material);
                    }
                }
            }

            materials = new List<Material>(uniqueMaterials);
        }

        foreach (var material in materials)
        {
            if (material != null)
            {
                Destroy(material);
            }
        }

        Destroy(model);
    }

    private void RecoverLoadedModelsAfterDomainReload()
    {
        if (loadedModels.Count > 0)
        {
            return;
        }

        var recovered = FindObjectsByType<IfcMetadataComponent>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);
        var interruptedImports = new List<string>();
        foreach (var metadata in recovered)
        {
            if (metadata == null || metadata.IfcType != "IfcModel")
            {
                continue;
            }

            var model = metadata.gameObject;
            metadata.Properties.TryGetValue(
                SourcePathProperty,
                out var sourcePath);
            if (!metadata.Properties.ContainsKey("Optimized Triangles"))
            {
                if (!string.IsNullOrWhiteSpace(sourcePath) &&
                    File.Exists(sourcePath))
                {
                    interruptedImports.Add(sourcePath);
                }

                DestroyModelResources(model);
                continue;
            }

            loadedModels.Add(model);
            loadedModel = model;

            if (!string.IsNullOrWhiteSpace(sourcePath))
            {
                modelSourcePaths[model] = sourcePath;
            }

            var uniqueMaterials = new HashSet<Material>();
            foreach (var renderer in model.GetComponentsInChildren<Renderer>(true))
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material != null)
                    {
                        uniqueMaterials.Add(material);
                    }
                }
            }

            modelMaterials[model] = new List<Material>(uniqueMaterials);
        }

        if (loadedModels.Count == 0 && interruptedImports.Count == 0)
        {
            return;
        }

        IsLoading = false;
        importingModel = null;
        loadRoutine = null;
        if (loadedModels.Count > 0)
        {
            StartCoroutine(NotifyRecoveredModelsNextFrame());
        }

        foreach (var sourcePath in interruptedImports)
        {
            pendingImports.Enqueue(sourcePath);
        }

        StartNextQueuedImport();
    }

    private IEnumerator NotifyRecoveredModelsNextFrame()
    {
        yield return null;
        ModelsChanged?.Invoke();
    }

    private void OnDestroy()
    {
        if (importingModel != null && !loadedModels.Contains(importingModel))
        {
            DestroyModelResources(importingModel);
        }

        for (var index = loadedModels.Count - 1; index >= 0; index--)
        {
            DestroyModelResources(loadedModels[index]);
        }

        loadedModels.Clear();
    }

    private static string GetConverterPath()
    {
#if UNITY_EDITOR_WIN
        return Path.GetFullPath(Path.Combine(
            Application.dataPath,
            "..",
            "Tools",
            "XbimIfcConverter",
            "bin",
            "Release",
            "net8.0",
            "win-x64",
            "publish",
            "XbimIfcConverter.exe"));
#elif UNITY_STANDALONE_WIN
        return Path.Combine(
            Application.dataPath,
            "XbimIfcConverter",
            "XbimIfcConverter.exe");
#else
        return string.Empty;
#endif
    }

    private static string Quote(string value)
    {
        return "\"" + value.Replace("\"", "\\\"") + "\"";
    }

    private string BuildConverterArguments(string inputPath, string outputPath)
    {
        return string.Join(
            " ",
            Quote(inputPath),
            Quote(outputPath),
            LinearDeflection.ToString("R", CultureInfo.InvariantCulture),
            AngularDeflection.ToString("R", CultureInfo.InvariantCulture));
    }

    private string GetConvertedMeshCachePath(
        string inputPath,
        string cacheDirectory)
    {
        var fingerprint = string.Join(
            "|",
            BuildSourceFingerprint(inputPath),
            LinearDeflection.ToString("R", CultureInfo.InvariantCulture),
            AngularDeflection.ToString("R", CultureInfo.InvariantCulture),
            RawMeshFileVersion.ToString(CultureInfo.InvariantCulture));
        return BuildCachePath(inputPath, cacheDirectory, fingerprint, string.Empty);
    }

    private string GetOptimizedMeshCachePath(
        string inputPath,
        string cacheDirectory)
    {
        var fingerprint = string.Join(
            "|",
            BuildSourceFingerprint(inputPath),
            LinearDeflection.ToString("R", CultureInfo.InvariantCulture),
            AngularDeflection.ToString("R", CultureInfo.InvariantCulture),
            OptimizationRevision.ToString(CultureInfo.InvariantCulture),
            simplifyMeshes,
            minimumSimplificationTriangles,
            standardMeshQuality.ToString("R", CultureInfo.InvariantCulture),
            aggressiveSimplificationTriangles,
            aggressiveMeshQuality.ToString("R", CultureInfo.InvariantCulture),
            extremeSimplificationTriangles,
            extremeMeshQuality.ToString("R", CultureInfo.InvariantCulture),
            broadSurfaceMeshQuality.ToString("R", CultureInfo.InvariantCulture),
            useQuadricSimplification,
            preserveBoundaryEdges,
            importTextureCoordinates,
            importTangents,
            OptimizedMeshFileVersion);
        return BuildCachePath(
            inputPath,
            cacheDirectory,
            fingerprint,
            "-optimized");
    }

    private static string BuildSourceFingerprint(string inputPath)
    {
        var file = new FileInfo(inputPath);
        return string.Join(
            "|",
            Path.GetFullPath(inputPath).ToUpperInvariant(),
            file.Length.ToString(CultureInfo.InvariantCulture),
            file.LastWriteTimeUtc.Ticks.ToString(CultureInfo.InvariantCulture));
    }

    private static string BuildCachePath(
        string inputPath,
        string cacheDirectory,
        string fingerprint,
        string suffix)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(fingerprint));
        var hashBuilder = new StringBuilder(hash.Length * 2);
        foreach (var value in hash)
        {
            hashBuilder.Append(value.ToString("x2", CultureInfo.InvariantCulture));
        }

        var hashText = hashBuilder.ToString();
        var safeName = Path.GetFileNameWithoutExtension(inputPath);
        foreach (var character in Path.GetInvalidFileNameChars())
        {
            safeName = safeName.Replace(character, '_');
        }

        if (safeName.Length > 48)
        {
            safeName = safeName.Substring(0, 48);
        }

        return Path.Combine(
            cacheDirectory,
            $"{safeName}{suffix}-{hashText.Substring(0, 20)}.xbimmesh");
    }

    private static bool IsMeshFileValid(string path, int expectedVersion)
    {
        if (!IsFileReady(path))
        {
            return false;
        }

        try
        {
            using var stream = File.OpenRead(path);
            using var reader = new BinaryReader(stream, Encoding.UTF8);
            return stream.Length >= sizeof(uint) + sizeof(int) &&
                   reader.ReadUInt32() == MeshFileMagic &&
                   reader.ReadInt32() == expectedVersion;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static void WriteOptimizedHeader(
        BinaryWriter writer,
        double metresPerUnit,
        double originX,
        double originY,
        double originZ,
        IReadOnlyDictionary<int, IfcMaterialStyle> styles,
        IReadOnlyList<IfcHierarchyNode> hierarchy,
        int meshCount)
    {
        writer.Write(MeshFileMagic);
        writer.Write(OptimizedMeshFileVersion);
        writer.Write(0L);
        writer.Write(metresPerUnit);
        writer.Write(originX);
        writer.Write(originY);
        writer.Write(originZ);

        writer.Write(styles.Count);
        foreach (var pair in styles)
        {
            var style = pair.Value;
            writer.Write(style.Label);
            writer.Write(style.Name ?? string.Empty);
            writer.Write(style.Diffuse.r);
            writer.Write(style.Diffuse.g);
            writer.Write(style.Diffuse.b);
            writer.Write(style.Diffuse.a);
            writer.Write(style.Specular.r);
            writer.Write(style.Specular.g);
            writer.Write(style.Specular.b);
            writer.Write(style.Smoothness);
        }

        writer.Write(hierarchy.Count);
        foreach (var node in hierarchy)
        {
            writer.Write(node.Label);
            writer.Write(node.ParentLabel);
            writer.Write(node.Name ?? string.Empty);
            writer.Write(node.IfcType ?? string.Empty);
            writer.Write(node.GlobalId ?? string.Empty);
            writer.Write(node.Properties.Count);
            foreach (var property in node.Properties)
            {
                writer.Write(property.Key ?? string.Empty);
                writer.Write(property.Value ?? string.Empty);
            }
        }

        writer.Write(meshCount);
    }

    private static void WriteOptimizedMesh(
        BinaryWriter writer,
        string objectName,
        int productLabel,
        Mesh mesh,
        IReadOnlyList<int> styleLabels)
    {
        writer.Write(objectName ?? string.Empty);
        writer.Write(productLabel);

        var vertices = mesh.vertices;
        writer.Write(vertices.Length);
        foreach (var vertex in vertices)
        {
            writer.Write(vertex.x);
            writer.Write(vertex.z);
            writer.Write(vertex.y);
        }

        var normals = mesh.normals;
        writer.Write(normals.Length);
        foreach (var normal in normals)
        {
            writer.Write(normal.x);
            writer.Write(normal.z);
            writer.Write(normal.y);
        }

        var uvs = mesh.uv;
        writer.Write(uvs.Length);
        foreach (var uv in uvs)
        {
            writer.Write(uv.x);
            writer.Write(uv.y);
        }

        var tangents = mesh.tangents;
        writer.Write(tangents.Length);
        foreach (var tangent in tangents)
        {
            writer.Write(tangent.x);
            writer.Write(tangent.z);
            writer.Write(tangent.y);
            writer.Write(-tangent.w);
        }

        writer.Write(mesh.subMeshCount);
        for (var subMeshIndex = 0;
             subMeshIndex < mesh.subMeshCount;
             subMeshIndex++)
        {
            writer.Write(styleLabels[subMeshIndex]);
            var triangles = mesh.GetTriangles(subMeshIndex);
            writer.Write(triangles.Length);
            for (var index = 0; index + 2 < triangles.Length; index += 3)
            {
                writer.Write(triangles[index]);
                writer.Write(triangles[index + 2]);
                writer.Write(triangles[index + 1]);
            }
        }
    }

    private static void CommitOptimizedCache(
        string partialPath,
        string cachePath)
    {
        try
        {
            if (File.Exists(cachePath))
            {
                File.Delete(cachePath);
            }

            File.Move(partialPath, cachePath);
        }
        catch (Exception exception)
        {
            DeleteTemporaryFile(partialPath);
            Debug.LogWarning(
                $"Could not store the optimized IFC mesh cache: " +
                exception.Message);
        }
    }

    private sealed class OptimizedMeshCacheWriter : IDisposable
    {
        private readonly string partialPath;
        private readonly string cachePath;
        private FileStream stream;
        private BinaryWriter writer;
        private bool committed;

        public BinaryWriter Writer => writer;

        public OptimizedMeshCacheWriter(
            string outputPartialPath,
            string outputCachePath)
        {
            partialPath = outputPartialPath;
            cachePath = outputCachePath;
            stream = File.Create(partialPath);
            writer = new BinaryWriter(stream, Encoding.UTF8, true);
        }

        public void Commit(long sourceTriangleCount)
        {
            if (committed || writer == null)
            {
                return;
            }

            writer.Flush();
            stream.Position = sizeof(uint) + sizeof(int);
            writer.Write(sourceTriangleCount);
            writer.Flush();
            writer.Dispose();
            stream.Dispose();
            writer = null;
            stream = null;
            committed = true;
            CommitOptimizedCache(partialPath, cachePath);
        }

        public void Dispose()
        {
            writer?.Dispose();
            stream?.Dispose();
            writer = null;
            stream = null;
            if (!committed)
            {
                DeleteTemporaryFile(partialPath);
            }
        }
    }

    private static void TryTerminateProcess(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill();
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                $"Could not terminate the timed-out xBIM converter: " +
                exception.Message);
        }
    }

    private void OnValidate()
    {
        LinearDeflection = linearDeflectionMillimetres;
        AngularDeflection = angularDeflectionDegrees;
        minimumSimplificationTriangles = Mathf.Max(12, minimumSimplificationTriangles);
        aggressiveSimplificationTriangles = Mathf.Max(
            minimumSimplificationTriangles,
            aggressiveSimplificationTriangles);
        extremeSimplificationTriangles = Mathf.Max(
            aggressiveSimplificationTriangles,
            extremeSimplificationTriangles);
        standardMeshQuality = Mathf.Clamp(standardMeshQuality, 0.05f, 1f);
        aggressiveMeshQuality = Mathf.Clamp(
            aggressiveMeshQuality,
            0.03f,
            standardMeshQuality);
        extremeMeshQuality = Mathf.Clamp(
            extremeMeshQuality,
            0.02f,
            aggressiveMeshQuality);
        broadSurfaceMeshQuality = Mathf.Clamp(
            broadSurfaceMeshQuality,
            0.02f,
            standardMeshQuality);
        meshesPerFrame = Mathf.Max(1, meshesPerFrame);
        sourceTrianglesPerFrame = Mathf.Max(1_000, sourceTrianglesPerFrame);
        converterTimeoutSeconds = Mathf.Max(30f, converterTimeoutSeconds);
    }

    private static string ReadSafeString(BinaryReader reader, string label)
    {
        var value = reader.ReadString();
        if (value.Length > MaxStringLength)
        {
            throw new InvalidDataException($"{label} exceeds the supported length.");
        }

        return value;
    }

    private static void ValidateChannelCount(int count, int vertexCount, string label)
    {
        ValidateCount(count, label);
        if (count != 0 && count != vertexCount)
        {
            throw new InvalidDataException(
                $"The {label} count {count} does not match vertex count {vertexCount}.");
        }
    }

    private static void ValidateCount(int count, string label)
    {
        if (count < 0 || count > 100_000_000)
        {
            throw new InvalidDataException($"Invalid {label} count: {count}.");
        }
    }

    private static void ValidateRecordCount(int count, string label)
    {
        if (count < 0 || count > MaxRecordCount)
        {
            throw new InvalidDataException($"Invalid {label} count: {count}.");
        }
    }

    private void Fail(string message)
    {
        if (importingModel != null)
        {
            modelSourcePaths.Remove(importingModel);
            DestroyModelResources(importingModel);
            importingModel = null;
            loadedModel = loadedModels.Count > 0 ? loadedModels[^1] : null;
        }

        IsLoading = false;
        loadRoutine = null;
        Debug.LogError(message);
        SetStatus(message);
        LoadFailed?.Invoke(message);
        StartNextQueuedImport();
    }

    private void StartNextQueuedImport()
    {
        if (loadRoutine != null || IsLoading || pendingImports.Count == 0)
        {
            return;
        }

        loadRoutine = StartCoroutine(LoadIfcRoutine(pendingImports.Dequeue()));
    }

    private void SetStatus(string message)
    {
        StatusChanged?.Invoke(message);
    }

    private static void DeleteTemporaryFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Could not remove temporary IFC mesh data: {exception.Message}");
        }
    }

    private readonly struct IfcMaterialStyle
    {
        public int Label { get; }
        public string Name { get; }
        public Color Diffuse { get; }
        public Color Specular { get; }
        public float Smoothness { get; }

        public IfcMaterialStyle(
            int label,
            string name,
            Color diffuse,
            Color specular,
            float smoothness)
        {
            Label = label;
            Name = name;
            Diffuse = diffuse;
            Specular = specular;
            Smoothness = smoothness;
        }

        public static IfcMaterialStyle Default(int label)
        {
            return new IfcMaterialStyle(
                label,
                label == 0 ? "IFC Default" : $"IFC Style {label}",
                new Color(0.78f, 0.8f, 0.82f, 1f),
                new Color(0.04f, 0.04f, 0.04f, 1f),
                0.45f);
        }
    }

    private sealed class IfcHierarchyNode
    {
        public int Label { get; }
        public int ParentLabel { get; }
        public string Name { get; }
        public string IfcType { get; }
        public string GlobalId { get; }
        public IReadOnlyList<KeyValuePair<string, string>> Properties { get; }

        public IfcHierarchyNode(
            int label,
            int parentLabel,
            string name,
            string ifcType,
            string globalId,
            IReadOnlyList<KeyValuePair<string, string>> properties)
        {
            Label = label;
            ParentLabel = parentLabel;
            Name = name;
            IfcType = ifcType;
            GlobalId = globalId;
            Properties = properties;
        }
    }
}
