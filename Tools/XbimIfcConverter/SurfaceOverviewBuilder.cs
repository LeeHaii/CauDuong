internal sealed class SurfaceOverviewBuilder
{
    private readonly double metresPerUnit;
    private readonly double interiorClusterSizeMetres;
    private readonly double boundaryClusterSizeMetres;
    private readonly double regionSizeMetres;
    private readonly int maximumTrianglesPerFragment;
    private readonly Dictionary<ClusterKey, int> clusterIndexByKey = new();
    private readonly List<ClusterAccumulator> clusters = new();
    private readonly Dictionary<TriangleKey, OverviewTriangle> triangles = new();

    public SurfaceOverviewBuilder(
        double modelMetresPerUnit,
        double interiorClusterMetres,
        double boundaryClusterMetres,
        double overviewRegionMetres,
        int fragmentTriangleLimit)
    {
        metresPerUnit = modelMetresPerUnit;
        interiorClusterSizeMetres = Math.Max(0.05d, interiorClusterMetres);
        boundaryClusterSizeMetres = Math.Max(
            0.01d,
            Math.Min(interiorClusterSizeMetres, boundaryClusterMetres));
        regionSizeMetres = Math.Max(interiorClusterSizeMetres, overviewRegionMetres);
        maximumTrianglesPerFragment = Math.Max(1_000, fragmentTriangleLimit);
    }

    public int CandidateTriangleCount => triangles.Count;

    public void AddShape(
        IReadOnlyList<Vector3d> vertices,
        IReadOnlyList<int> indices,
        int productLabel,
        int styleLabel)
    {
        if (vertices.Count == 0 || indices.Count < 3)
        {
            return;
        }

        var boundaryVertices = FindBoundaryVertices(vertices.Count, indices);
        var clusterBySourceVertex = new int[vertices.Count];
        Array.Fill(clusterBySourceVertex, -1);
        for (var index = 0; index + 2 < indices.Count; index += 3)
        {
            var source0 = indices[index];
            var source1 = indices[index + 1];
            var source2 = indices[index + 2];
            var cluster0 = GetOrAddCluster(
                vertices[source0],
                boundaryVertices[source0],
                productLabel,
                styleLabel,
                source0,
                clusterBySourceVertex);
            var cluster1 = GetOrAddCluster(
                vertices[source1],
                boundaryVertices[source1],
                productLabel,
                styleLabel,
                source1,
                clusterBySourceVertex);
            var cluster2 = GetOrAddCluster(
                vertices[source2],
                boundaryVertices[source2],
                productLabel,
                styleLabel,
                source2,
                clusterBySourceVertex);
            if (cluster0 == cluster1 || cluster1 == cluster2 || cluster2 == cluster0)
            {
                continue;
            }

            var key = TriangleKey.Create(cluster0, cluster1, cluster2);
            triangles.TryAdd(
                key,
                new OverviewTriangle(
                    cluster0,
                    cluster1,
                    cluster2,
                    productLabel,
                    styleLabel));
        }
    }

    public IReadOnlyList<OverviewFragment> Build()
    {
        var positions = new Vector3d[clusters.Count];
        for (var index = 0; index < clusters.Count; index++)
        {
            positions[index] = clusters[index].Average;
        }

        var buildersByCell = new Dictionary<SpatialCell, List<OverviewFragmentBuilder>>();
        foreach (var triangle in triangles.Values)
        {
            var vertex0 = positions[triangle.Index0];
            var vertex1 = positions[triangle.Index1];
            var vertex2 = positions[triangle.Index2];
            var cross = Vector3d.Cross(vertex1 - vertex0, vertex2 - vertex0);
            if (Vector3d.Dot(cross, cross) <= 1e-20d)
            {
                continue;
            }

            var centroid = (vertex0 + vertex1 + vertex2) * (1d / 3d);
            var cell = new SpatialCell(
                ToRegionCoordinate(centroid.X * metresPerUnit),
                ToRegionCoordinate(centroid.Y * metresPerUnit),
                ToRegionCoordinate(centroid.Z * metresPerUnit));
            if (!buildersByCell.TryGetValue(cell, out var builders))
            {
                builders = new List<OverviewFragmentBuilder>();
                buildersByCell.Add(cell, builders);
            }

            var builder = builders.Count > 0 ? builders[^1] : null;
            if (builder == null || builder.TriangleCount >= maximumTrianglesPerFragment)
            {
                builder = new OverviewFragmentBuilder(cell, positions);
                builders.Add(builder);
            }

            builder.AddTriangle(triangle);
        }

        return buildersByCell
            .OrderBy(pair => pair.Key.X)
            .ThenBy(pair => pair.Key.Y)
            .ThenBy(pair => pair.Key.Z)
            .SelectMany(pair => pair.Value)
            .Select(builder => builder.Build())
            .Where(fragment => fragment.TriangleCount > 0)
            .ToList();
    }

    private int GetOrAddCluster(
        Vector3d vertex,
        bool boundary,
        int productLabel,
        int styleLabel,
        int sourceIndex,
        int[] clusterBySourceVertex)
    {
        if (clusterBySourceVertex[sourceIndex] >= 0)
        {
            return clusterBySourceVertex[sourceIndex];
        }

        var clusterSize = boundary
            ? boundaryClusterSizeMetres
            : interiorClusterSizeMetres;
        var key = new ClusterKey(
            productLabel,
            styleLabel,
            boundary,
            Quantize(vertex.X * metresPerUnit, clusterSize),
            Quantize(vertex.Y * metresPerUnit, clusterSize),
            Quantize(vertex.Z * metresPerUnit, clusterSize));
        if (!clusterIndexByKey.TryGetValue(key, out var clusterIndex))
        {
            clusterIndex = clusters.Count;
            clusterIndexByKey.Add(key, clusterIndex);
            clusters.Add(new ClusterAccumulator());
        }

        clusters[clusterIndex].Add(vertex);
        clusterBySourceVertex[sourceIndex] = clusterIndex;
        return clusterIndex;
    }

    private int ToRegionCoordinate(double valueMetres)
    {
        return (int)Math.Clamp(
            Math.Floor(valueMetres / regionSizeMetres),
            int.MinValue,
            int.MaxValue);
    }

    private static long Quantize(double value, double cellSize)
    {
        var coordinate = Math.Floor(value / cellSize);
        return double.IsFinite(coordinate)
            ? (long)Math.Clamp(coordinate, long.MinValue, long.MaxValue)
            : 0L;
    }

    private static bool[] FindBoundaryVertices(
        int vertexCount,
        IReadOnlyList<int> indices)
    {
        var edgeUse = new Dictionary<EdgeKey, byte>(indices.Count);
        for (var index = 0; index + 2 < indices.Count; index += 3)
        {
            CountEdge(edgeUse, indices[index], indices[index + 1]);
            CountEdge(edgeUse, indices[index + 1], indices[index + 2]);
            CountEdge(edgeUse, indices[index + 2], indices[index]);
        }

        var boundary = new bool[vertexCount];
        foreach (var pair in edgeUse)
        {
            if (pair.Value != 1)
            {
                continue;
            }

            boundary[pair.Key.Index0] = true;
            boundary[pair.Key.Index1] = true;
        }

        return boundary;
    }

    private static void CountEdge(
        IDictionary<EdgeKey, byte> edgeUse,
        int index0,
        int index1)
    {
        var key = EdgeKey.Create(index0, index1);
        edgeUse.TryGetValue(key, out var count);
        edgeUse[key] = count < byte.MaxValue ? (byte)(count + 1) : count;
    }

    private readonly record struct ClusterKey(
        int ProductLabel,
        int StyleLabel,
        bool Boundary,
        long X,
        long Y,
        long Z);

    private readonly record struct EdgeKey(int Index0, int Index1)
    {
        public static EdgeKey Create(int index0, int index1)
        {
            return index0 <= index1
                ? new EdgeKey(index0, index1)
                : new EdgeKey(index1, index0);
        }
    }

    private readonly record struct TriangleKey(int Index0, int Index1, int Index2)
    {
        public static TriangleKey Create(int index0, int index1, int index2)
        {
            if (index0 > index1)
            {
                (index0, index1) = (index1, index0);
            }

            if (index1 > index2)
            {
                (index1, index2) = (index2, index1);
            }

            if (index0 > index1)
            {
                (index0, index1) = (index1, index0);
            }

            return new TriangleKey(index0, index1, index2);
        }
    }

    private sealed class ClusterAccumulator
    {
        private Vector3d sum;
        private int count;

        public Vector3d Average => count > 0 ? sum * (1d / count) : Vector3d.Zero;

        public void Add(Vector3d vertex)
        {
            sum += vertex;
            count++;
        }
    }
}

internal readonly record struct OverviewTriangle(
    int Index0,
    int Index1,
    int Index2,
    int ProductLabel,
    int StyleLabel);

internal sealed record OverviewFragment(
    SpatialCell Cell,
    Vector3d Minimum,
    Vector3d Maximum,
    Vector3d[] Vertices,
    Vector3d[] Normals,
    int[] StyleLabels,
    int[][] SubMeshIndices,
    int[] TriangleProductLabels)
{
    public int TriangleCount => TriangleProductLabels.Length;
    public int IndexCount => SubMeshIndices.Sum(indices => indices.Length);
}

internal sealed class OverviewFragmentBuilder
{
    private readonly IReadOnlyList<Vector3d> sourcePositions;
    private readonly Dictionary<int, int> localIndexByCluster = new();
    private readonly List<Vector3d> vertices = new();
    private readonly Dictionary<int, List<OverviewLocalTriangle>> trianglesByStyle = new();
    private Vector3d minimum;
    private Vector3d maximum;
    private bool hasBounds;

    public OverviewFragmentBuilder(
        SpatialCell cell,
        IReadOnlyList<Vector3d> positions)
    {
        Cell = cell;
        sourcePositions = positions;
    }

    public SpatialCell Cell { get; }
    public int TriangleCount { get; private set; }

    public void AddTriangle(OverviewTriangle triangle)
    {
        if (!trianglesByStyle.TryGetValue(triangle.StyleLabel, out var triangles))
        {
            triangles = new List<OverviewLocalTriangle>();
            trianglesByStyle.Add(triangle.StyleLabel, triangles);
        }

        triangles.Add(new OverviewLocalTriangle(
            GetOrAddVertex(triangle.Index0),
            GetOrAddVertex(triangle.Index1),
            GetOrAddVertex(triangle.Index2),
            triangle.ProductLabel));
        TriangleCount++;
    }

    public OverviewFragment Build()
    {
        var styleLabels = trianglesByStyle.Keys.OrderBy(label => label).ToArray();
        var subMeshes = new int[styleLabels.Length][];
        var products = new List<int>(TriangleCount);
        for (var styleIndex = 0; styleIndex < styleLabels.Length; styleIndex++)
        {
            var triangles = trianglesByStyle[styleLabels[styleIndex]];
            var indices = new int[triangles.Count * 3];
            for (var triangleIndex = 0; triangleIndex < triangles.Count; triangleIndex++)
            {
                var triangle = triangles[triangleIndex];
                var outputIndex = triangleIndex * 3;
                indices[outputIndex] = triangle.Index0;
                indices[outputIndex + 1] = triangle.Index1;
                indices[outputIndex + 2] = triangle.Index2;
                products.Add(triangle.ProductLabel);
            }

            subMeshes[styleIndex] = indices;
        }

        var vertexArray = vertices.ToArray();
        var normals = CalculateNormals(vertexArray, subMeshes);
        return new OverviewFragment(
            Cell,
            minimum,
            maximum,
            vertexArray,
            normals,
            styleLabels,
            subMeshes,
            products.ToArray());
    }

    private int GetOrAddVertex(int clusterIndex)
    {
        if (localIndexByCluster.TryGetValue(clusterIndex, out var localIndex))
        {
            return localIndex;
        }

        localIndex = vertices.Count;
        localIndexByCluster.Add(clusterIndex, localIndex);
        var vertex = sourcePositions[clusterIndex];
        vertices.Add(vertex);
        if (!hasBounds)
        {
            minimum = vertex;
            maximum = vertex;
            hasBounds = true;
        }
        else
        {
            minimum = new Vector3d(
                Math.Min(minimum.X, vertex.X),
                Math.Min(minimum.Y, vertex.Y),
                Math.Min(minimum.Z, vertex.Z));
            maximum = new Vector3d(
                Math.Max(maximum.X, vertex.X),
                Math.Max(maximum.Y, vertex.Y),
                Math.Max(maximum.Z, vertex.Z));
        }

        return localIndex;
    }

    private static Vector3d[] CalculateNormals(
        IReadOnlyList<Vector3d> vertices,
        IReadOnlyList<int[]> subMeshes)
    {
        var normals = new Vector3d[vertices.Count];
        foreach (var indices in subMeshes)
        {
            for (var index = 0; index + 2 < indices.Length; index += 3)
            {
                var index0 = indices[index];
                var index1 = indices[index + 1];
                var index2 = indices[index + 2];
                var faceNormal = Vector3d.Cross(
                    vertices[index1] - vertices[index0],
                    vertices[index2] - vertices[index0]);
                normals[index0] += faceNormal;
                normals[index1] += faceNormal;
                normals[index2] += faceNormal;
            }
        }

        for (var index = 0; index < normals.Length; index++)
        {
            normals[index] = normals[index].NormalizedOr(Vector3d.Up);
        }

        return normals;
    }

    private readonly record struct OverviewLocalTriangle(
        int Index0,
        int Index1,
        int Index2,
        int ProductLabel);
}
