using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;

public sealed class XbimIfcLoader : MonoBehaviour
{
    private const uint MeshFileMagic = 0x4D494258;
    private const int MeshFileVersion = 2;
    private const int MaxRecordCount = 10_000_000;
    private const int MaxStringLength = 1_000_000;

    [Header("Generated Model")]
    [SerializeField] private Transform modelParent;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private bool generateMeshColliders;

    [Header("Geometry Optimization")]
    [Tooltip("Maximum curve-to-segment deviation in millimetres. Higher values reduce triangle counts.")]
    [SerializeField, Min(0.01f)] private float linearDeflectionMillimetres = 5f;
    [Tooltip("Maximum angular deviation in degrees. Higher values reduce segments around curves.")]
    [SerializeField, Range(1f, 90f)] private float angularDeflectionDegrees = 30f;

    [Header("Import Budget")]
    [Min(1)]
    [SerializeField] private int meshesPerFrame = 16;

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

        var cacheDirectory = Path.Combine(Application.temporaryCachePath, "XbimIfc");
        Directory.CreateDirectory(cacheDirectory);
        var meshPath = Path.Combine(cacheDirectory, Guid.NewGuid().ToString("N") + ".xbimmesh");

        SetStatus(
            $"Converting IFC geometry at {LinearDeflection:G4} mm / " +
            $"{AngularDeflection:G4} deg deflection...");

        Process process;

        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = converterPath,
                Arguments = BuildConverterArguments(path, meshPath),
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
            Fail($"Could not start the xBIM converter: {exception.Message}");
            yield break;
        }

        using (process)
        {
            while (!process.HasExited && !IsFileReady(meshPath))
            {
                yield return null;
            }

            if (process.HasExited && process.ExitCode != 0)
            {
                Fail($"xBIM conversion failed with exit code {process.ExitCode}.");
                DeleteTemporaryFile(meshPath);
                yield break;
            }
        }

        if (!File.Exists(meshPath) || new FileInfo(meshPath).Length == 0)
        {
            Fail("xBIM conversion completed without producing mesh data.");
            DeleteTemporaryFile(meshPath);
            yield break;
        }

        SetStatus("Creating Unity hierarchy and meshes...");

        Exception importException = null;
        IEnumerator importRoutine = null;

        try
        {
            importRoutine = ImportMeshFile(
                meshPath,
                Path.GetFileNameWithoutExtension(path),
                path);
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

        DeleteTemporaryFile(meshPath);

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
        SetStatus("IFC import complete.");
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
        string sourcePath)
    {
        using var stream = File.OpenRead(path);
        using var reader = new BinaryReader(stream, Encoding.UTF8);

        if (reader.ReadUInt32() != MeshFileMagic)
        {
            throw new InvalidDataException("The converter produced an unknown mesh format.");
        }

        var version = reader.ReadInt32();
        if (version != MeshFileVersion)
        {
            throw new InvalidDataException($"Unsupported xBIM mesh version: {version}.");
        }

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

        loadedModel = new GameObject(modelName);
        importingModel = loadedModel;
        activeMaterialCache = new Dictionary<int, Material>();
        modelMaterials[loadedModel] = new List<Material>();
        modelSourcePaths[loadedModel] = sourcePath;
        if (modelParent != null)
        {
            loadedModel.transform.SetParent(modelParent, false);
        }

        loadedModel.transform.localScale = Vector3.one * (float)metresPerUnit;

        var modelMetadata = loadedModel.AddComponent<IfcMetadataComponent>();
        modelMetadata.Initialize(
            "IfcModel",
            string.Empty,
            0,
            new[]
            {
                new KeyValuePair<string, string>(
                    "Length Scale (metres/unit)",
                    metresPerUnit.ToString("G9")),
                new KeyValuePair<string, string>(
                    "Local Origin (IFC coordinates)",
                    $"{originX:G9}, {originY:G9}, {originZ:G9}"),
                new KeyValuePair<string, string>(
                    "Linear Deflection (mm)",
                    LinearDeflection.ToString("G9", CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string>(
                    "Angular Deflection (degrees)",
                    AngularDeflection.ToString("G9", CultureInfo.InvariantCulture))
            });

        var hierarchyObjects = CreateHierarchy(hierarchy, loadedModel.transform);

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
            var uvs = new Vector2[uvCount];
            for (var uvIndex = 0; uvIndex < uvCount; uvIndex++)
            {
                uvs[uvIndex] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            }

            var tangentCount = reader.ReadInt32();
            ValidateChannelCount(tangentCount, vertexCount, "tangent");
            var tangents = new Vector4[tangentCount];
            for (var tangentIndex = 0; tangentIndex < tangentCount; tangentIndex++)
            {
                var x = reader.ReadSingle();
                var y = reader.ReadSingle();
                var z = reader.ReadSingle();
                var w = reader.ReadSingle();
                tangents[tangentIndex] = new Vector4(x, z, y, -w);
            }

            var subMeshCount = reader.ReadInt32();
            ValidateRecordCount(subMeshCount, "sub-mesh");
            if (subMeshCount == 0)
            {
                throw new InvalidDataException($"Mesh '{objectName}' has no sub-meshes.");
            }

            var subMeshes = new int[subMeshCount][];
            var materials = new Material[subMeshCount];
            for (var subMeshIndex = 0; subMeshIndex < subMeshCount; subMeshIndex++)
            {
                var styleLabel = reader.ReadInt32();
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

            if (uvCount == vertexCount)
            {
                mesh.uv = uvs;
            }

            if (tangentCount == vertexCount)
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

            if (tangentCount != vertexCount && uvCount == vertexCount)
            {
                mesh.RecalculateTangents();
            }

            mesh.RecalculateBounds();

            var element = new GameObject(objectName);
            var parent = hierarchyObjects.TryGetValue(productLabel, out var productObject)
                ? productObject.transform
                : loadedModel.transform;
            element.transform.SetParent(parent, false);
            element.AddComponent<MeshFilter>().sharedMesh = mesh;
            element.AddComponent<MeshRenderer>().sharedMaterials = materials;

            if (generateMeshColliders)
            {
                element.AddComponent<MeshCollider>().sharedMesh = mesh;
            }

            if ((meshIndex + 1) % meshesPerFrame == 0)
            {
                SetStatus($"Creating Unity meshes... {meshIndex + 1}/{meshCount}");
                yield return null;
            }
        }
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

        if (modelMaterials.Remove(model, out var materials))
        {
            foreach (var material in materials)
            {
                if (material != null)
                {
                    Destroy(material);
                }
            }
        }

        Destroy(model);
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

    private void OnValidate()
    {
        LinearDeflection = linearDeflectionMillimetres;
        AngularDeflection = angularDeflectionDegrees;
        meshesPerFrame = Mathf.Max(1, meshesPerFrame);
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
