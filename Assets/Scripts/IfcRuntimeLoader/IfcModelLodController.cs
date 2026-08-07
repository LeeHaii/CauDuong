using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityMeshSimplifier;

[DisallowMultipleComponent]
public sealed class IfcModelLodController : MonoBehaviour
{
    private sealed class ProxyMaterialGroup
    {
        public ProxyMaterialGroup(Color color)
        {
            Color = color;
        }

        public Color Color { get; }
        public readonly Dictionary<Vector3Int, ProxyBucket> SpatialBuckets = new();
    }

    private sealed class ProxyBucket
    {
        public ProxyBucket(Vector3Int cell)
        {
            Cell = cell;
        }

        public Vector3Int Cell { get; }
        public readonly List<ProxySource> Sources = new();
    }

    private readonly struct ProxySource
    {
        public ProxySource(Mesh mesh, int subMeshIndex, Matrix4x4 transform)
        {
            Mesh = mesh;
            SubMeshIndex = subMeshIndex;
            Transform = transform;
        }

        public Mesh Mesh { get; }
        public int SubMeshIndex { get; }
        public Matrix4x4 Transform { get; }
    }

    private sealed class ProxyCandidate
    {
        public ProxyCandidate(
            ProxySource source,
            Color color,
            Vector3 localCenter,
            int triangleCount,
            float importance)
        {
            Source = source;
            Color = color;
            LocalCenter = localCenter;
            TriangleCount = triangleCount;
            Importance = importance;
        }

        public ProxySource Source { get; }
        public Color Color { get; }
        public Vector3 LocalCenter { get; }
        public int TriangleCount { get; }
        public float Importance { get; }
    }

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly List<IfcModelLodController> ScheduledControllers = new();
    private static int schedulerFrame = -1;
    private static int schedulerCursor;

    [SerializeField] private Camera viewingCamera;
    [Tooltip("Renderers covering less than this many screen pixels are skipped.")]
    [SerializeField, Min(0.01f)] private float minimumScreenAreaPixels = 4f;
    [Tooltip("Meshes at or above this triangle count use the more aggressive expensive-renderer threshold.")]
    [SerializeField, Min(100)] private int expensiveRendererTriangleThreshold = 10_000;
    [Tooltip("Minimum projected pixel area for expensive detail and proxy meshes.")]
    [SerializeField, Min(0.01f)] private float expensiveRendererMinimumScreenAreaPixels = 64f;
    [Tooltip("Extra size required before a culled renderer becomes visible again, preventing flicker while zooming.")]
    [SerializeField, Range(1f, 2f)] private float cullingExitMultiplier = 1.2f;
    [SerializeField, Min(0.05f)] private float evaluationInterval = 0.25f;
    [Tooltip("Maximum projected-bounds checks shared by every IFC model each frame.")]
    [SerializeField, Min(32)] private int globalCullingChecksPerFrame = 512;
    [Tooltip("Fair-share slice given to one model before the scheduler advances to the next model.")]
    [SerializeField, Min(8)] private int cullingChecksPerControllerSlice = 64;
    [SerializeField, Min(0f)] private float selectedRevealSeconds = 4f;

    [Header("Distance Proxy")]
    [SerializeField] private bool enableDistanceProxy = true;
    [Tooltip("Minimum camera distance at which the combined-shape proxy may replace detailed IFC meshes.")]
    [SerializeField, Min(50f)] private float minimumProxyDistance = 900f;
    [Tooltip("Proxy distance also scales with the full model radius.")]
    [SerializeField, Min(1f)] private float proxyDistanceMultiplier = 3f;
    [Tooltip("Lower exit ratio prevents rapid proxy/detail switching near the threshold.")]
    [SerializeField, Range(0.5f, 0.95f)] private float proxyExitDistanceRatio = 0.78f;
    [Tooltip("Hard source-triangle budget per model before the optional simplification pass.")]
    [SerializeField, Min(10_000)] private int maximumProxyTrianglesPerModel = 500_000;
    [Tooltip("Features smaller than this fraction of the full model diameter are omitted from the distant proxy.")]
    [SerializeField, Range(0f, 0.05f)] private float minimumProxyFeatureSizeRatio = 0.0005f;
    [Tooltip("Spatial cells keep proxy bounds tight enough for screen-size culling.")]
    [SerializeField, Range(1, 12)] private int proxySpatialCellsPerAxis = 6;
    [SerializeField, Min(1_000)] private int maxProxyVerticesPerMesh = 15_000;
    [SerializeField, Range(1, 16)] private int maximumProxyMaterials = 8;
    [Tooltip("Apply a topology-aware second pass to the already budgeted distant proxy.")]
    [SerializeField] private bool simplifyProxyMeshes = false;
    [SerializeField, Range(0.02f, 1f)] private float proxyMeshQuality = 0.2f;
    [SerializeField, Min(12)] private int minimumProxySimplificationTriangles = 300;

    private readonly List<Renderer> renderers = new();
    private readonly List<Renderer> proxyRenderers = new();
    private readonly List<Mesh> proxyMeshes = new();
    private readonly List<Material> proxyMaterials = new();
    private readonly Dictionary<Renderer, float> revealUntil = new();
    private readonly Dictionary<Renderer, bool> cullingStates = new();
    private readonly Dictionary<Renderer, int> rendererTriangleCounts = new();
    private Transform proxyRoot;
    private Bounds modelBounds;
    private float nextEvaluationTime;
    private float forceDetailUntil;
    private Vector3 lastCameraPosition;
    private Quaternion lastCameraRotation;
    private float lastCameraProjection;
    private bool evaluationRequired = true;
    private bool proxyActive;
    private bool isRebuilding;
    private int cullingCursor = -1;
    private bool cullingTargetsProxy;
    private bool restartCullingAfterSweep;

    public bool IsProxyActive => proxyActive;
    public int ProxyRendererCount => proxyRenderers.Count;

    public void Rebuild()
    {
        var routine = RebuildIncrementally();
        while (routine.MoveNext())
        {
        }
    }

    public IEnumerator RebuildIncrementally()
    {
        isRebuilding = true;
        CancelCullingSweep();
        ResetCulling();
        DestroyProxyResources();
        renderers.Clear();
        cullingStates.Clear();
        rendererTriangleCounts.Clear();
        GetComponentsInChildren(true, renderers);
        var inspectedRenderers = 0;
        for (var index = renderers.Count - 1; index >= 0; index--)
        {
            var renderer = renderers[index];
            if (renderer == null ||
                renderer is not MeshRenderer ||
                renderer.GetComponentInParent<IfcInspectionMarker>() != null ||
                renderer.GetComponentInParent<IfcElementMetadata>() == null)
            {
                renderers.RemoveAt(index);
            }
            else
            {
                rendererTriangleCounts[renderer] = CountRendererTriangles(renderer);
            }

            inspectedRenderers++;
            if (inspectedRenderers % 256 == 0)
            {
                yield return null;
            }
        }

        RecalculateModelBounds();
        var proxyRoutine = BuildDistanceProxyIncrementally();
        while (proxyRoutine.MoveNext())
        {
            yield return proxyRoutine.Current;
        }

        viewingCamera ??= Camera.main;
        isRebuilding = false;
        evaluationRequired = true;
        Evaluate();
    }

    public void Reveal(IReadOnlyList<Renderer> selectedRenderers)
    {
        var expiry = Time.unscaledTime + selectedRevealSeconds;
        forceDetailUntil = Mathf.Max(forceDetailUntil, expiry);
        SetProxyActive(false);
        foreach (var renderer in selectedRenderers)
        {
            if (renderer == null)
            {
                continue;
            }

            revealUntil[renderer] = expiry;
            SetCullingState(renderer, false);
        }

        evaluationRequired = true;
        RequestCullingSweep();
    }

    private void OnEnable()
    {
        RegisterForCulling();
        Rebuild();
    }

    private void OnDisable()
    {
        UnregisterFromCulling();
        CancelCullingSweep();
        ResetCulling();
    }

    private void OnDestroy()
    {
        UnregisterFromCulling();
        CancelCullingSweep();
        ResetCulling();
        DestroyProxyResources();
    }

    private void Update()
    {
        var now = Time.unscaledTime;
        if (now >= nextEvaluationTime)
        {
            nextEvaluationTime = now + evaluationInterval;
            if (evaluationRequired || revealUntil.Count > 0 || HasCameraChanged())
            {
                Evaluate();
            }
        }

        RunCullingScheduler();
    }

    private void Evaluate()
    {
        viewingCamera ??= Camera.main;
        if (viewingCamera == null)
        {
            return;
        }

        RememberCameraState();
        evaluationRequired = false;
        var now = Time.unscaledTime;
        var shouldUseProxy = ShouldUseProxy(now);
        SetProxyActive(shouldUseProxy);
        RequestCullingSweep();
    }

    private int ProcessCullingSlice(int maximumChecks)
    {
        if (isRebuilding || cullingCursor < 0 || viewingCamera == null)
        {
            return 0;
        }

        var targetRenderers = cullingTargetsProxy ? proxyRenderers : renderers;
        var processed = 0;
        var now = Time.unscaledTime;
        while (processed < maximumChecks && cullingCursor < targetRenderers.Count)
        {
            var renderer = targetRenderers[cullingCursor++];
            processed++;
            if (renderer == null)
            {
                continue;
            }

            if (cullingTargetsProxy)
            {
                // The distance proxy already has a strict geometry budget and
                // very few renderers. Culling its chunks again creates visible
                // gaps in long, thin infrastructure for negligible savings.
                SetCullingState(renderer, false);
                continue;
            }

            if (revealUntil.TryGetValue(renderer, out var expiry))
            {
                if (expiry > now)
                {
                    SetCullingState(renderer, false);
                    continue;
                }

                revealUntil.Remove(renderer);
            }

            CullRendererByScreenArea(renderer);
        }

        if (cullingCursor < targetRenderers.Count)
        {
            return processed;
        }

        cullingCursor = -1;
        if (restartCullingAfterSweep)
        {
            restartCullingAfterSweep = false;
            RequestCullingSweep();
        }

        return processed;
    }

    private void RequestCullingSweep()
    {
        var targetProxy = proxyActive;
        if (cullingCursor >= 0)
        {
            if (cullingTargetsProxy == targetProxy)
            {
                restartCullingAfterSweep = true;
                return;
            }
        }

        cullingTargetsProxy = targetProxy;
        cullingCursor = 0;
        restartCullingAfterSweep = false;
    }

    private void CancelCullingSweep()
    {
        cullingCursor = -1;
        restartCullingAfterSweep = false;
    }

    private void RegisterForCulling()
    {
        if (!ScheduledControllers.Contains(this))
        {
            ScheduledControllers.Add(this);
        }
    }

    private void UnregisterFromCulling()
    {
        var index = ScheduledControllers.IndexOf(this);
        if (index < 0)
        {
            return;
        }

        ScheduledControllers.RemoveAt(index);
        if (schedulerCursor > index)
        {
            schedulerCursor--;
        }

        if (schedulerCursor >= ScheduledControllers.Count)
        {
            schedulerCursor = 0;
        }
    }

    private static void RunCullingScheduler()
    {
        if (schedulerFrame == Time.frameCount || ScheduledControllers.Count == 0)
        {
            return;
        }

        schedulerFrame = Time.frameCount;
        var budget = 0;
        foreach (var controller in ScheduledControllers)
        {
            if (controller != null && controller.isActiveAndEnabled)
            {
                budget = Mathf.Max(budget, controller.globalCullingChecksPerFrame);
            }
        }

        var idleControllers = 0;
        while (budget > 0 && idleControllers < ScheduledControllers.Count)
        {
            if (schedulerCursor >= ScheduledControllers.Count)
            {
                schedulerCursor = 0;
            }

            var controller = ScheduledControllers[schedulerCursor++];
            if (controller == null || !controller.isActiveAndEnabled)
            {
                idleControllers++;
                continue;
            }

            var slice = Mathf.Min(
                budget,
                controller.cullingChecksPerControllerSlice);
            var processed = controller.ProcessCullingSlice(slice);
            budget -= processed;
            idleControllers = processed == 0 ? idleControllers + 1 : 0;
        }
    }

    private void CullRendererByScreenArea(Renderer renderer)
    {
        var triangleCount = rendererTriangleCounts.TryGetValue(renderer, out var count)
            ? count
            : CountRendererTriangles(renderer);
        var visibleThreshold = triangleCount >= expensiveRendererTriangleThreshold
            ? expensiveRendererMinimumScreenAreaPixels
            : minimumScreenAreaPixels;
        var wasCulled = cullingStates.TryGetValue(renderer, out var previousState) &&
                        previousState;
        if (wasCulled)
        {
            visibleThreshold *= cullingExitMultiplier * cullingExitMultiplier;
        }

        SetCullingState(
            renderer,
            CalculateProjectedScreenAreaPixels(viewingCamera, renderer.bounds) <
            visibleThreshold);
    }

    private bool ShouldUseProxy(float now)
    {
        if (!enableDistanceProxy ||
            proxyRenderers.Count == 0 ||
            renderers.Count == 0 ||
            now < forceDetailUntil)
        {
            return false;
        }

        var lossyScale = transform.lossyScale;
        var maximumScale = Mathf.Max(
            Mathf.Abs(lossyScale.x),
            Mathf.Max(Mathf.Abs(lossyScale.y), Mathf.Abs(lossyScale.z)));
        var radius = Mathf.Max(
            1f,
            modelBounds.extents.magnitude * maximumScale);
        var enterDistance = Mathf.Max(
            minimumProxyDistance,
            radius * proxyDistanceMultiplier);
        var exitDistance = enterDistance * proxyExitDistanceRatio;
        var cameraDistance = Vector3.Distance(
            viewingCamera.transform.position,
            transform.TransformPoint(modelBounds.center));
        return proxyActive
            ? cameraDistance > exitDistance
            : cameraDistance > enterDistance;
    }

    private void SetProxyActive(bool active)
    {
        if (proxyActive == active)
        {
            return;
        }

        proxyActive = active;
        foreach (var renderer in proxyRenderers)
        {
            if (renderer != null)
            {
                SetCullingState(renderer, !active);
            }
        }

        foreach (var renderer in renderers)
        {
            if (renderer != null)
            {
                SetCullingState(renderer, active);
            }
        }
    }

    private void RecalculateModelBounds()
    {
        var found = false;
        foreach (var renderer in renderers)
        {
            if (renderer == null)
            {
                continue;
            }

            var bounds = renderer.bounds;
            var min = bounds.min;
            var max = bounds.max;
            EncapsulateLocalPoint(ref modelBounds, ref found, new Vector3(min.x, min.y, min.z));
            EncapsulateLocalPoint(ref modelBounds, ref found, new Vector3(max.x, min.y, min.z));
            EncapsulateLocalPoint(ref modelBounds, ref found, new Vector3(max.x, max.y, min.z));
            EncapsulateLocalPoint(ref modelBounds, ref found, new Vector3(min.x, max.y, min.z));
            EncapsulateLocalPoint(ref modelBounds, ref found, new Vector3(min.x, min.y, max.z));
            EncapsulateLocalPoint(ref modelBounds, ref found, new Vector3(max.x, min.y, max.z));
            EncapsulateLocalPoint(ref modelBounds, ref found, new Vector3(max.x, max.y, max.z));
            EncapsulateLocalPoint(ref modelBounds, ref found, new Vector3(min.x, max.y, max.z));
        }

        if (!found)
        {
            modelBounds = new Bounds(Vector3.zero, Vector3.one);
        }
    }

    private void EncapsulateLocalPoint(
        ref Bounds bounds,
        ref bool found,
        Vector3 worldPoint)
    {
        var localPoint = transform.InverseTransformPoint(worldPoint);
        if (!found)
        {
            bounds = new Bounds(localPoint, Vector3.zero);
            found = true;
            return;
        }

        bounds.Encapsulate(localPoint);
    }

    private IEnumerator BuildDistanceProxyIncrementally()
    {
        if (!enableDistanceProxy || renderers.Count == 0)
        {
            yield break;
        }

        var lossyScale = transform.lossyScale;
        var maximumScale = Mathf.Max(
            Mathf.Abs(lossyScale.x),
            Mathf.Max(Mathf.Abs(lossyScale.y), Mathf.Abs(lossyScale.z)));
        var modelDiameter = Mathf.Max(
            1f,
            modelBounds.extents.magnitude * 2f * maximumScale);
        var candidates = new List<ProxyCandidate>();
        for (var rendererIndex = 0; rendererIndex < renderers.Count; rendererIndex++)
        {
            var renderer = renderers[rendererIndex];
            if (renderer == null ||
                !renderer.TryGetComponent<MeshFilter>(out var meshFilter) ||
                meshFilter.sharedMesh == null ||
                !meshFilter.sharedMesh.isReadable)
            {
                continue;
            }

            var mesh = meshFilter.sharedMesh;
            var featureDiameter = renderer.bounds.extents.magnitude * 2f;
            if (featureDiameter < modelDiameter * minimumProxyFeatureSizeRatio)
            {
                continue;
            }

            var materials = renderer.sharedMaterials;
            var localTransform = transform.worldToLocalMatrix *
                                 meshFilter.transform.localToWorldMatrix;
            var localCenter = transform.InverseTransformPoint(renderer.bounds.center);
            for (var subMeshIndex = 0;
                 subMeshIndex < mesh.subMeshCount;
                 subMeshIndex++)
            {
                var triangleCount = (int)Math.Min(
                    int.MaxValue,
                    (long)(mesh.GetIndexCount(subMeshIndex) / 3));
                if (triangleCount <= 0)
                {
                    continue;
                }

                var material = materials.Length == 0
                    ? null
                    : materials[Mathf.Min(subMeshIndex, materials.Length - 1)];
                var importance = featureDiameter *
                                 (1f + Mathf.Log10(triangleCount + 1f));
                candidates.Add(new ProxyCandidate(
                    new ProxySource(mesh, subMeshIndex, localTransform),
                    ReadMaterialColor(material),
                    localCenter,
                    triangleCount,
                    importance));
            }

            if ((rendererIndex + 1) % 128 == 0)
            {
                yield return null;
            }
        }

        if (candidates.Count == 0)
        {
            yield break;
        }

        candidates.Sort(
            (left, right) =>
                right.Importance.CompareTo(left.Importance));
        var selectedCandidates = new List<ProxyCandidate>();
        var selectedCandidateSet = new HashSet<ProxyCandidate>();
        var selectedTriangleCount = 0;
        var dominantFeatureBudget = maximumProxyTrianglesPerModel / 2;
        for (var candidateIndex = 0;
             candidateIndex < candidates.Count &&
             selectedTriangleCount < dominantFeatureBudget;
             candidateIndex++)
        {
            var candidate = candidates[candidateIndex];
            selectedCandidates.Add(candidate);
            selectedCandidateSet.Add(candidate);
            selectedTriangleCount += candidate.TriangleCount;
        }

        var candidatesByCell =
            new Dictionary<Vector3Int, List<ProxyCandidate>>();
        foreach (var candidate in candidates)
        {
            if (selectedCandidateSet.Contains(candidate))
            {
                continue;
            }

            var cell = GetProxyCell(candidate.LocalCenter);
            if (!candidatesByCell.TryGetValue(cell, out var cellCandidates))
            {
                cellCandidates = new List<ProxyCandidate>();
                candidatesByCell.Add(cell, cellCandidates);
            }

            cellCandidates.Add(candidate);
        }

        var coverageBuckets =
            new List<List<ProxyCandidate>>(candidatesByCell.Values);
        foreach (var bucket in coverageBuckets)
        {
            bucket.Sort(
                (left, right) =>
                    right.Importance.CompareTo(left.Importance));
        }

        var coverageIndices = new int[coverageBuckets.Count];
        var hasCoverageCandidates = true;
        while (hasCoverageCandidates &&
               selectedTriangleCount < maximumProxyTrianglesPerModel)
        {
            hasCoverageCandidates = false;
            for (var bucketIndex = 0;
                 bucketIndex < coverageBuckets.Count &&
                 selectedTriangleCount < maximumProxyTrianglesPerModel;
                 bucketIndex++)
            {
                var bucket = coverageBuckets[bucketIndex];
                var candidateIndex = coverageIndices[bucketIndex];
                if (candidateIndex >= bucket.Count)
                {
                    continue;
                }

                hasCoverageCandidates = true;
                var candidate = bucket[candidateIndex];
                coverageIndices[bucketIndex] = candidateIndex + 1;
                selectedCandidates.Add(candidate);
                selectedTriangleCount += candidate.TriangleCount;
            }
        }

        var materialGroups = new List<ProxyMaterialGroup>(maximumProxyMaterials);
        for (var candidateIndex = 0;
             candidateIndex < selectedCandidates.Count;
             candidateIndex++)
        {
            var candidate = selectedCandidates[candidateIndex];
            var materialGroup = FindProxyMaterialGroup(materialGroups, candidate.Color);
            var cell = GetProxyCell(candidate.LocalCenter);
            if (!materialGroup.SpatialBuckets.TryGetValue(cell, out var bucket))
            {
                bucket = new ProxyBucket(cell);
                materialGroup.SpatialBuckets.Add(cell, bucket);
            }

            bucket.Sources.Add(candidate.Source);
            if ((candidateIndex + 1) % 512 == 0)
            {
                yield return null;
            }
        }

        proxyRoot = new GameObject("Distance Proxy").transform;
        proxyRoot.SetParent(transform, false);
        proxyRoot.gameObject.layer = gameObject.layer;

        foreach (var materialGroup in materialGroups)
        {
            var material = CreateProxyMaterial(materialGroup.Color);
            proxyMaterials.Add(material);
            foreach (var bucket in materialGroup.SpatialBuckets.Values)
            {
                var start = 0;
                while (start < bucket.Sources.Count)
                {
                    var count = CalculateProxyChunkSize(bucket.Sources, start);
                    BuildProxyChunk(bucket.Sources, start, count, material, bucket.Cell);
                    start += count;
                    yield return null;
                }
            }
        }

        foreach (var renderer in proxyRenderers)
        {
            renderer.forceRenderingOff = true;
        }
    }

    private int CalculateProxyChunkSize(
        IReadOnlyList<ProxySource> sources,
        int start)
    {
        var count = 0;
        var vertexCount = 0;
        while (start + count < sources.Count)
        {
            var sourceVertexCount = sources[start + count].Mesh.vertexCount;
            if (count > 0 && vertexCount + sourceVertexCount > maxProxyVerticesPerMesh)
            {
                break;
            }

            vertexCount += sourceVertexCount;
            count++;
        }

        return Mathf.Max(1, count);
    }

    private ProxyMaterialGroup FindProxyMaterialGroup(
        List<ProxyMaterialGroup> groups,
        Color color)
    {
        ProxyMaterialGroup nearest = null;
        var nearestDistance = float.PositiveInfinity;
        foreach (var group in groups)
        {
            var difference = new Vector3(
                color.r - group.Color.r,
                color.g - group.Color.g,
                color.b - group.Color.b);
            var distance = difference.sqrMagnitude;
            if (distance < nearestDistance)
            {
                nearest = group;
                nearestDistance = distance;
            }
        }

        if (nearest != null &&
            (nearestDistance <= 0.025f || groups.Count >= maximumProxyMaterials))
        {
            return nearest;
        }

        var created = new ProxyMaterialGroup(color);
        groups.Add(created);
        return created;
    }

    private Vector3Int GetProxyCell(Vector3 localCenter)
    {
        var size = modelBounds.size;
        var maximumSize = Mathf.Max(size.x, Mathf.Max(size.y, size.z));
        return new Vector3Int(
            GetProxyCellAxis(localCenter.x, modelBounds.min.x, size.x, maximumSize),
            GetProxyCellAxis(localCenter.y, modelBounds.min.y, size.y, maximumSize),
            GetProxyCellAxis(localCenter.z, modelBounds.min.z, size.z, maximumSize));
    }

    private int GetProxyCellAxis(
        float value,
        float minimum,
        float size,
        float maximumModelSize)
    {
        if (size <= Mathf.Max(0.001f, maximumModelSize * 0.05f))
        {
            return 0;
        }

        return Mathf.Clamp(
            Mathf.FloorToInt((value - minimum) / size * proxySpatialCellsPerAxis),
            0,
            proxySpatialCellsPerAxis - 1);
    }

    private void BuildProxyChunk(
        IReadOnlyList<ProxySource> sources,
        int start,
        int count,
        Material material,
        Vector3Int cell)
    {
        var combine = new CombineInstance[count];
        for (var index = 0; index < count; index++)
        {
            var source = sources[start + index];
            combine[index] = new CombineInstance
            {
                mesh = source.Mesh,
                subMeshIndex = source.SubMeshIndex,
                transform = source.Transform
            };
        }

        var combinedMesh = new Mesh
        {
            name = $"IFC Distance Proxy {cell.x}-{cell.y}-{cell.z}-" +
                   $"{proxyMeshes.Count + 1}",
            indexFormat = IndexFormat.UInt32
        };
        combinedMesh.CombineMeshes(combine, true, true, false);
        combinedMesh.RecalculateBounds();

        var mesh = SimplifyProxyMesh(combinedMesh);
        mesh.name = combinedMesh.name;
        mesh.RecalculateBounds();
        mesh.UploadMeshData(true);
        proxyMeshes.Add(mesh);

        if (mesh != combinedMesh)
        {
            DestroyOwned(combinedMesh);
        }

        var proxyObject = new GameObject(mesh.name);
        proxyObject.layer = gameObject.layer;
        proxyObject.transform.SetParent(proxyRoot, false);
        proxyObject.AddComponent<MeshFilter>().sharedMesh = mesh;
        var renderer = proxyObject.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = false;
        renderer.lightProbeUsage = LightProbeUsage.Off;
        renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
        proxyRenderers.Add(renderer);
        rendererTriangleCounts[renderer] = CountRendererTriangles(renderer);
    }

    private Mesh SimplifyProxyMesh(Mesh sourceMesh)
    {
        var triangleCount = (int)(sourceMesh.GetIndexCount(0) / 3);
        if (!simplifyProxyMeshes ||
            proxyMeshQuality >= 0.999f ||
            triangleCount < minimumProxySimplificationTriangles)
        {
            return sourceMesh;
        }

        try
        {
            var options = SimplificationOptions.Default;
            options.PreserveBorderEdges = false;
            options.PreserveUVSeamEdges = false;
            options.PreserveUVFoldoverEdges = false;
            options.PreserveSurfaceCurvature = true;
            // IFC patches that merely touch must not be welded together into the
            // rectangular artifacts seen in the original distance proxies.
            options.EnableSmartLink = false;

            var simplifier = new MeshSimplifier
            {
                SimplificationOptions = options
            };
            simplifier.Initialize(sourceMesh);
            simplifier.SimplifyMesh(proxyMeshQuality);
            var simplifiedMesh = simplifier.ToMesh();
            if (simplifiedMesh.GetIndexCount(0) > 0 &&
                simplifiedMesh.GetIndexCount(0) < sourceMesh.GetIndexCount(0))
            {
                return simplifiedMesh;
            }

            DestroyOwned(simplifiedMesh);
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                $"Could not simplify IFC distance proxy '{sourceMesh.name}': " +
                exception.Message,
                this);
        }

        return sourceMesh;
    }

    private static Color ReadMaterialColor(Material material)
    {
        var color = material != null && material.HasProperty(BaseColorId)
            ? material.GetColor(BaseColorId)
            : material != null && material.HasProperty(ColorId)
                ? material.GetColor(ColorId)
                : new Color(0.55f, 0.59f, 0.63f, 1f);
        color.a = 1f;
        return color;
    }

    private static Material CreateProxyMaterial(Color color)
    {
        var shader = Resources.Load<Shader>("Shaders/IfcDoubleSided") ??
                     Shader.Find("Universal Render Pipeline/Unlit") ??
                     Shader.Find("Unlit/Color") ??
                     Shader.Find("Standard");
        var material = new Material(shader)
        {
            name = "IFC Distance Proxy Material",
            color = color
        };
        if (material.HasProperty(BaseColorId))
        {
            material.SetColor(BaseColorId, color);
        }

        if (material.HasProperty(ColorId))
        {
            material.SetColor(ColorId, color);
        }

        if (material.HasProperty("_Cull"))
        {
            material.SetFloat("_Cull", (float)CullMode.Off);
        }

        return material;
    }

    private bool HasCameraChanged()
    {
        if (viewingCamera == null)
        {
            return false;
        }

        var projection = viewingCamera.orthographic
            ? viewingCamera.orthographicSize
            : viewingCamera.fieldOfView;
        return (viewingCamera.transform.position - lastCameraPosition).sqrMagnitude > 0.01f ||
               Quaternion.Angle(viewingCamera.transform.rotation, lastCameraRotation) > 0.05f ||
               Mathf.Abs(projection - lastCameraProjection) > 0.01f;
    }

    private void RememberCameraState()
    {
        lastCameraPosition = viewingCamera.transform.position;
        lastCameraRotation = viewingCamera.transform.rotation;
        lastCameraProjection = viewingCamera.orthographic
            ? viewingCamera.orthographicSize
            : viewingCamera.fieldOfView;
    }

    private void SetCullingState(Renderer renderer, bool culled)
    {
        if (cullingStates.TryGetValue(renderer, out var previous) && previous == culled)
        {
            return;
        }

        cullingStates[renderer] = culled;
        renderer.forceRenderingOff = culled;
    }

    public static float CalculateProjectedDiameterPixels(Camera camera, Bounds bounds)
    {
        if (camera == null || camera.pixelHeight <= 0)
        {
            return float.PositiveInfinity;
        }

        var pixelHeight = camera.pixelHeight;
        var radius = Mathf.Max(0.001f, bounds.extents.magnitude);
        if (camera.orthographic)
        {
            return radius * pixelHeight / Mathf.Max(0.001f, camera.orthographicSize);
        }

        var distance = Mathf.Max(
            0.001f,
            Vector3.Distance(camera.transform.position, bounds.center) - radius);
        var verticalSize = 2f * distance *
                           Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        return radius * 2f * pixelHeight / Mathf.Max(0.001f, verticalSize);
    }

    public static float CalculateProjectedScreenAreaPixels(Camera camera, Bounds bounds)
    {
        if (camera == null || camera.pixelWidth <= 0 || camera.pixelHeight <= 0)
        {
            return float.PositiveInfinity;
        }

        var min = bounds.min;
        var max = bounds.max;
        var minimumX = float.PositiveInfinity;
        var minimumY = float.PositiveInfinity;
        var maximumX = float.NegativeInfinity;
        var maximumY = float.NegativeInfinity;
        for (var cornerIndex = 0; cornerIndex < 8; cornerIndex++)
        {
            var corner = new Vector3(
                (cornerIndex & 1) == 0 ? min.x : max.x,
                (cornerIndex & 2) == 0 ? min.y : max.y,
                (cornerIndex & 4) == 0 ? min.z : max.z);
            var screenPoint = camera.WorldToScreenPoint(corner);
            if (screenPoint.z <= camera.nearClipPlane)
            {
                // Near-plane intersections cannot be represented by projecting
                // only the AABB corners. Keep them visible until fully in front.
                return float.PositiveInfinity;
            }

            minimumX = Mathf.Min(minimumX, screenPoint.x);
            minimumY = Mathf.Min(minimumY, screenPoint.y);
            maximumX = Mathf.Max(maximumX, screenPoint.x);
            maximumY = Mathf.Max(maximumY, screenPoint.y);
        }

        var pixelRect = camera.pixelRect;
        minimumX = Mathf.Clamp(minimumX, pixelRect.xMin, pixelRect.xMax);
        minimumY = Mathf.Clamp(minimumY, pixelRect.yMin, pixelRect.yMax);
        maximumX = Mathf.Clamp(maximumX, pixelRect.xMin, pixelRect.xMax);
        maximumY = Mathf.Clamp(maximumY, pixelRect.yMin, pixelRect.yMax);
        return Mathf.Max(0f, maximumX - minimumX) *
               Mathf.Max(0f, maximumY - minimumY);
    }

    private static int CountRendererTriangles(Renderer renderer)
    {
        if (renderer == null ||
            !renderer.TryGetComponent<MeshFilter>(out var meshFilter) ||
            meshFilter.sharedMesh == null)
        {
            return 0;
        }

        var mesh = meshFilter.sharedMesh;
        var count = 0L;
        for (var subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
        {
            count += (long)(mesh.GetIndexCount(subMeshIndex) / 3);
        }

        return (int)Math.Min(int.MaxValue, count);
    }

    private void ResetCulling()
    {
        foreach (var renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.forceRenderingOff = false;
            }
        }

        foreach (var renderer in proxyRenderers)
        {
            if (renderer != null)
            {
                renderer.forceRenderingOff = true;
            }
        }

        proxyActive = false;
        revealUntil.Clear();
        cullingStates.Clear();
    }

    private void DestroyProxyResources()
    {
        if (proxyRoot != null)
        {
            DestroyOwned(proxyRoot.gameObject);
            proxyRoot = null;
        }

        foreach (var mesh in proxyMeshes)
        {
            DestroyOwned(mesh);
        }

        foreach (var material in proxyMaterials)
        {
            DestroyOwned(material);
        }

        foreach (var renderer in proxyRenderers)
        {
            if (renderer == null)
            {
                continue;
            }

            cullingStates.Remove(renderer);
            rendererTriangleCounts.Remove(renderer);
            revealUntil.Remove(renderer);
        }

        proxyRenderers.Clear();
        proxyMeshes.Clear();
        proxyMaterials.Clear();
        proxyActive = false;
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
}
