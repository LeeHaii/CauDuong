using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using Xbim.Common.Configuration;
using Xbim.Common.Geometry;
using Xbim.Common.XbimExtensions;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.ModelGeometry.Scene;

const uint Magic = 0x4D494258; // "XBIM" in little-endian files.
const int FormatVersion = 5;
const double DefaultLinearDeflectionMillimetres = 5d;
const double DefaultAngularDeflectionDegrees = 30d;
const double DefaultSpatialCellSizeMetres = 100d;
const int DefaultMaximumTrianglesPerFragment = 50_000;
const int DefaultOverviewTargetTriangles = 200_000;
const double DefaultOverviewClusterSizeMetres = 1d;
const double DefaultOverviewBoundaryClusterSizeMetres = 0.25d;
const double DefaultOverviewRegionSizeMetres = 1_000d;

if (args.Length is not 2 and not 4 and not 6 and not 10 and not 12)
{
    Console.Error.WriteLine(
        "Usage: XbimIfcConverter <input.ifc> <output.xbimmesh> " +
        "[linearDeflectionMillimetres angularDeflectionDegrees " +
        "spatialCellSizeMetres maximumTrianglesPerFragment " +
        "overviewTargetTriangles overviewClusterSizeMetres " +
        "overviewBoundaryClusterSizeMetres overviewRegionSizeMetres " +
        "writeTextureCoordinates writeTangents]");
    return 2;
}

var inputPath = Path.GetFullPath(args[0]);
var outputPath = Path.GetFullPath(args[1]);
var linearDeflectionMillimetres = DefaultLinearDeflectionMillimetres;
var angularDeflectionDegrees = DefaultAngularDeflectionDegrees;
var spatialCellSizeMetres = DefaultSpatialCellSizeMetres;
var maximumTrianglesPerFragment = DefaultMaximumTrianglesPerFragment;
var overviewTargetTriangles = DefaultOverviewTargetTriangles;
var overviewClusterSizeMetres = DefaultOverviewClusterSizeMetres;
var overviewBoundaryClusterSizeMetres = DefaultOverviewBoundaryClusterSizeMetres;
var overviewRegionSizeMetres = DefaultOverviewRegionSizeMetres;
var writeTextureCoordinates = false;
var writeTangents = false;

if (args.Length >= 4 &&
    (!double.TryParse(
         args[2],
         NumberStyles.Float,
         CultureInfo.InvariantCulture,
         out linearDeflectionMillimetres) ||
     !double.TryParse(
         args[3],
         NumberStyles.Float,
         CultureInfo.InvariantCulture,
         out angularDeflectionDegrees)))
{
    Console.Error.WriteLine("Deflection values must be valid invariant-culture numbers.");
    return 4;
}

if (args.Length >= 6 &&
    (!double.TryParse(
         args[4],
         NumberStyles.Float,
         CultureInfo.InvariantCulture,
         out spatialCellSizeMetres) ||
     !int.TryParse(
         args[5],
         NumberStyles.Integer,
         CultureInfo.InvariantCulture,
         out maximumTrianglesPerFragment)))
{
    Console.Error.WriteLine(
        "Spatial cell size and maximum fragment triangle count must be valid numbers.");
    return 4;
}

if (args.Length >= 10 &&
    (!int.TryParse(
         args[6],
         NumberStyles.Integer,
         CultureInfo.InvariantCulture,
         out overviewTargetTriangles) ||
     !double.TryParse(
         args[7],
         NumberStyles.Float,
         CultureInfo.InvariantCulture,
         out overviewClusterSizeMetres) ||
     !double.TryParse(
         args[8],
         NumberStyles.Float,
         CultureInfo.InvariantCulture,
         out overviewBoundaryClusterSizeMetres) ||
     !double.TryParse(
         args[9],
         NumberStyles.Float,
         CultureInfo.InvariantCulture,
         out overviewRegionSizeMetres)))
{
    Console.Error.WriteLine("Overview settings must be valid invariant-culture numbers.");
    return 4;
}

if (args.Length == 12 &&
    (!bool.TryParse(args[10], out writeTextureCoordinates) ||
     !bool.TryParse(args[11], out writeTangents)))
{
    Console.Error.WriteLine("Vertex-channel flags must be valid Boolean values.");
    return 4;
}

if (!double.IsFinite(linearDeflectionMillimetres) ||
    linearDeflectionMillimetres is < 0.01d or > 1000d ||
    !double.IsFinite(angularDeflectionDegrees) ||
    angularDeflectionDegrees is < 1d or > 90d)
{
    Console.Error.WriteLine(
        "Linear deflection must be 0.01-1000 mm and angular deflection must be 1-90 degrees.");
    return 4;
}

if (overviewTargetTriangles is < 10_000 or > 5_000_000 ||
    !double.IsFinite(overviewClusterSizeMetres) ||
    overviewClusterSizeMetres is < 0.05d or > 1_000d ||
    !double.IsFinite(overviewBoundaryClusterSizeMetres) ||
    overviewBoundaryClusterSizeMetres is < 0.01d or > 1_000d ||
    overviewBoundaryClusterSizeMetres > overviewClusterSizeMetres ||
    !double.IsFinite(overviewRegionSizeMetres) ||
    overviewRegionSizeMetres is < 10d or > 1_000_000d)
{
    Console.Error.WriteLine(
        "Overview target must be 10000-5000000 triangles; cluster sizes must be " +
        "valid and boundary size may not exceed interior size; region size must " +
        "be 10-1000000 metres.");
    return 4;
}

if (!double.IsFinite(spatialCellSizeMetres) ||
    spatialCellSizeMetres is < 1d or > 100_000d ||
    maximumTrianglesPerFragment is < 1_000 or > 5_000_000)
{
    Console.Error.WriteLine(
        "Spatial cell size must be 1-100000 metres and each fragment must allow " +
        "1000-5000000 triangles.");
    return 4;
}

if (!File.Exists(inputPath))
{
    Console.Error.WriteLine($"IFC file not found: {inputPath}");
    return 3;
}

try
{
    XbimServices.Current.ConfigureServices(
        services => services.AddXbimToolkit(toolkit => toolkit.AddGeometryServices()));

    Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

    using var model = IfcStore.Open(inputPath);
    var unitsPerMetre = model.ModelFactors.OneMetre;
    var metresPerUnit = unitsPerMetre > 0d ? 1d / unitsPerMetre : 1d;
    var linearDeflectionModelUnits =
        linearDeflectionMillimetres * unitsPerMetre / 1000d;
    var angularDeflectionRadians = angularDeflectionDegrees * Math.PI / 180d;

    model.ModelFactors.DeflectionTolerance = linearDeflectionModelUnits;
    model.ModelFactors.DeflectionAngle = angularDeflectionRadians;

    var context = new Xbim3DModelContext(model);
    context.CreateContext();

    var shapeInstances = context.ShapeInstances()
        .Where(shape => shape.RepresentationType ==
                        XbimGeometryRepresentationType.OpeningsAndAdditionsIncluded)
        .ToList();
    var productStyleLabels = new Dictionary<int, int>();
    var shapeExports = shapeInstances
        .Select(shape => new ShapeExport(
            shape,
            ResolveStyleLabel(model, shape, productStyleLabels)))
        .ToList();

    var productLabels = shapeExports
        .Select(export => export.Shape.IfcProductLabel)
        .ToHashSet();

    var hasOrigin = TryFindLocalOrigin(context, shapeExports, out var origin);
    ProjectedGeoReference? projectedGeoReference =
        hasOrigin &&
        TryCreateProjectedGeoReference(
            model,
            origin,
            metresPerUnit,
            out var resolvedGeoReference)
            ? resolvedGeoReference
            : null;
    var hierarchy = BuildHierarchy(
        model,
        productLabels,
        metresPerUnit,
        projectedGeoReference);
    var styleLabels = shapeExports
        .Select(export => export.StyleLabel)
        .Append(0)
        .Distinct()
        .OrderBy(label => label)
        .ToList();
    var styles = styleLabels
        .Select(label => ExtractStyle(model, label))
        .ToList();

    using var output = File.Create(outputPath);
    using var writer = new BinaryWriter(output);

    writer.Write(Magic);
    writer.Write(FormatVersion);
    writer.Write(metresPerUnit);
    writer.Write(origin.X);
    writer.Write(origin.Y);
    writer.Write(origin.Z);

    writer.Write(styles.Count);
    foreach (var style in styles)
    {
        writer.Write(style.Label);
        writer.Write(style.Name);
        writer.Write(style.Diffuse.R);
        writer.Write(style.Diffuse.G);
        writer.Write(style.Diffuse.B);
        writer.Write(style.Alpha);
        writer.Write(style.Specular.R);
        writer.Write(style.Specular.G);
        writer.Write(style.Specular.B);
        writer.Write(style.Smoothness);
    }

    writer.Write(hierarchy.Count);
    foreach (var node in hierarchy)
    {
        writer.Write(node.Label);
        writer.Write(node.ParentLabel);
        writer.Write(node.Name);
        writer.Write(node.IfcType);
        writer.Write(node.GlobalId);
        writer.Write(node.Properties.Count);

        foreach (var property in node.Properties)
        {
            writer.Write(property.Key);
            writer.Write(property.Value);
        }
    }

    var meshCountPosition = output.Position;
    writer.Write(0);

    var meshCount = 0;

    foreach (var shapeExport in shapeExports)
    {
        var shapeInstance = shapeExport.Shape;
        var geometry = context.ShapeGeometry(shapeInstance);
        var shapeData = ((IXbimShapeGeometryData)geometry).ShapeData;
        if (shapeData is not { Length: > 0 })
        {
            continue;
        }

        using var shapeStream = new MemoryStream(shapeData, writable: false);
        using var shapeReader = new BinaryReader(shapeStream);
        var triangulation = shapeReader.ReadShapeTriangulation();
        var sourceVertices = triangulation.Vertices.ToList();
        var indices = triangulation.Faces.SelectMany(face => face.Indices).ToArray();

        if (sourceVertices.Count == 0 || indices.Length < 3)
        {
            continue;
        }

        var transformedVertices = new Vector3d[sourceVertices.Count];
        for (var index = 0; index < sourceVertices.Count; index++)
        {
            var transformed = shapeInstance.Transformation.Transform(sourceVertices[index]);
            transformedVertices[index] = new Vector3d(
                transformed.X,
                transformed.Y,
                transformed.Z);
        }

        for (var index = 0; index < transformedVertices.Length; index++)
        {
            transformedVertices[index] -= origin;
        }

        ValidateIndices(indices, transformedVertices.Length);
        var normals = CalculateNormals(transformedVertices, indices);
        var uvs = writeTextureCoordinates
            ? CalculateBoxProjectedUvs(
                transformedVertices,
                normals,
                metresPerUnit)
            : Array.Empty<Uv>();
        var tangents = writeTangents
            ? CalculateTangents(normals)
            : Array.Empty<Tangent>();

        var product = model.Instances[shapeInstance.IfcProductLabel] as IIfcProduct;
        var ifcType = product?.GetType().Name ?? "IfcProduct";
        var displayName = ReadText(product, "Name");
        var objectName = string.IsNullOrWhiteSpace(displayName)
            ? $"{ifcType}_{shapeInstance.IfcProductLabel}"
            : displayName;

        var fragments = PartitionShape(
            transformedVertices,
            normals,
            uvs,
            tangents,
            indices,
            metresPerUnit,
            spatialCellSizeMetres,
            maximumTrianglesPerFragment);
        foreach (var fragment in fragments)
        {
            WriteMeshFragment(
                writer,
                fragment,
                $"{objectName}_Cell_{fragment.Cell.X}_{fragment.Cell.Y}_{fragment.Cell.Z}_" +
                $"Mesh_{meshCount + 1}",
                shapeInstance.IfcProductLabel,
                shapeExport.StyleLabel);
            meshCount++;
        }
    }

    var overviewFragments = BuildSurfaceOverview(
        context,
        shapeExports,
        origin,
        metresPerUnit,
        overviewTargetTriangles,
        overviewClusterSizeMetres,
        overviewBoundaryClusterSizeMetres,
        overviewRegionSizeMetres,
        Math.Min(maximumTrianglesPerFragment, 50_000));
    writer.Write(overviewFragments.Count);
    for (var overviewIndex = 0;
         overviewIndex < overviewFragments.Count;
         overviewIndex++)
    {
        WriteOverviewFragment(
            writer,
            overviewFragments[overviewIndex],
            $"IFC_Surface_Overview_{overviewIndex + 1}");
    }

    writer.Flush();
    var endPosition = output.Position;
    output.Position = meshCountPosition;
    writer.Write(meshCount);
    output.Position = endPosition;

    Console.WriteLine(
        $"Converted {meshCount} meshes, {hierarchy.Count} hierarchy nodes, " +
        $"{styles.Count} styles at {metresPerUnit:G6} metres/unit with " +
        $"{linearDeflectionMillimetres:G6} mm linear / " +
        $"{angularDeflectionDegrees:G6} deg angular deflection, " +
        $"{spatialCellSizeMetres:G6} m cells and at most " +
        $"{maximumTrianglesPerFragment:N0} triangles/fragment and " +
        $"{overviewFragments.Sum(fragment => fragment.TriangleCount):N0} " +
        $"surface-overview triangles in {overviewFragments.Count:N0} fragments " +
        $"to {outputPath}");
    return 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception);
    return 1;
}

static IReadOnlyList<MeshFragment> PartitionShape(
    IReadOnlyList<Vector3d> vertices,
    IReadOnlyList<Vector3d> normals,
    IReadOnlyList<Uv> uvs,
    IReadOnlyList<Tangent> tangents,
    IReadOnlyList<int> indices,
    double metresPerUnit,
    double cellSizeMetres,
    int maximumTrianglesPerFragment)
{
    var buildersByCell = new Dictionary<SpatialCell, List<MeshFragmentBuilder>>();
    for (var index = 0; index + 2 < indices.Count; index += 3)
    {
        var index0 = indices[index];
        var index1 = indices[index + 1];
        var index2 = indices[index + 2];
        var centroid = (vertices[index0] + vertices[index1] + vertices[index2]) *
                       (1d / 3d);
        var cell = new SpatialCell(
            ToCellCoordinate(centroid.X * metresPerUnit, cellSizeMetres),
            ToCellCoordinate(centroid.Y * metresPerUnit, cellSizeMetres),
            ToCellCoordinate(centroid.Z * metresPerUnit, cellSizeMetres));

        if (!buildersByCell.TryGetValue(cell, out var builders))
        {
            builders = new List<MeshFragmentBuilder>();
            buildersByCell.Add(cell, builders);
        }

        var builder = builders.Count > 0 ? builders[^1] : null;
        if (builder == null || builder.TriangleCount >= maximumTrianglesPerFragment)
        {
            builder = new MeshFragmentBuilder(
                cell,
                vertices,
                normals,
                uvs,
                tangents);
            builders.Add(builder);
        }

        builder.AddTriangle(index0, index1, index2);
    }

    return buildersByCell
        .OrderBy(pair => pair.Key.X)
        .ThenBy(pair => pair.Key.Y)
        .ThenBy(pair => pair.Key.Z)
        .SelectMany(pair => pair.Value)
        .Select(builder => builder.Build())
        .ToList();
}

static int ToCellCoordinate(double valueMetres, double cellSizeMetres)
{
    var coordinate = Math.Floor(valueMetres / cellSizeMetres);
    return (int)Math.Clamp(coordinate, int.MinValue, int.MaxValue);
}

static void WriteMeshFragment(
    BinaryWriter writer,
    MeshFragment fragment,
    string objectName,
    int productLabel,
    int styleLabel)
{
    var recordStart = writer.BaseStream.Position;
    writer.Write(0L); // Bytes after this field; patched when the record is complete.
    writer.Write(fragment.Cell.X);
    writer.Write(fragment.Cell.Y);
    writer.Write(fragment.Cell.Z);
    writer.Write((float)fragment.Minimum.X);
    writer.Write((float)fragment.Minimum.Y);
    writer.Write((float)fragment.Minimum.Z);
    writer.Write((float)fragment.Maximum.X);
    writer.Write((float)fragment.Maximum.Y);
    writer.Write((float)fragment.Maximum.Z);
    writer.Write(fragment.TriangleCount);
    writer.Write(fragment.Vertices.Length);
    writer.Write(fragment.Indices.Length);
    writer.Write(styleLabel);
    writer.Write(productLabel);

    // The payload intentionally remains self-contained. Unity can seek directly
    // to it from the compact record header without deserializing other cells.
    writer.Write(objectName);
    writer.Write(productLabel);
    writer.Write(fragment.Vertices.Length);
    foreach (var vertex in fragment.Vertices)
    {
        writer.Write((float)vertex.X);
        writer.Write((float)vertex.Y);
        writer.Write((float)vertex.Z);
    }

    writer.Write(fragment.Normals.Length);
    foreach (var normal in fragment.Normals)
    {
        writer.Write((float)normal.X);
        writer.Write((float)normal.Y);
        writer.Write((float)normal.Z);
    }

    writer.Write(fragment.Uvs.Length);
    foreach (var uv in fragment.Uvs)
    {
        writer.Write(uv.X);
        writer.Write(uv.Y);
    }

    writer.Write(fragment.Tangents.Length);
    foreach (var tangent in fragment.Tangents)
    {
        writer.Write(tangent.X);
        writer.Write(tangent.Y);
        writer.Write(tangent.Z);
        writer.Write(tangent.W);
    }

    writer.Write(1); // A shape instance has one resolved IFC surface style.
    writer.Write(styleLabel);
    writer.Write(fragment.Indices.Length);
    foreach (var index in fragment.Indices)
    {
        writer.Write(index);
    }

    var recordEnd = writer.BaseStream.Position;
    writer.BaseStream.Position = recordStart;
    writer.Write(recordEnd - recordStart - sizeof(long));
    writer.BaseStream.Position = recordEnd;
}

static IReadOnlyList<OverviewFragment> BuildSurfaceOverview(
    Xbim3DModelContext context,
    IReadOnlyList<ShapeExport> shapeExports,
    Vector3d origin,
    double metresPerUnit,
    int targetTriangles,
    double initialClusterSizeMetres,
    double initialBoundaryClusterSizeMetres,
    double regionSizeMetres,
    int maximumTrianglesPerFragment)
{
    var clusterSize = initialClusterSizeMetres;
    var boundaryClusterSize = initialBoundaryClusterSizeMetres;
    IReadOnlyList<OverviewFragment> fragments = Array.Empty<OverviewFragment>();
    const int maximumAttempts = 8;
    for (var attempt = 0; attempt < maximumAttempts; attempt++)
    {
        var builder = new SurfaceOverviewBuilder(
            metresPerUnit,
            clusterSize,
            boundaryClusterSize,
            regionSizeMetres,
            maximumTrianglesPerFragment);
        foreach (var shapeExport in shapeExports)
        {
            if (!TryReadTransformedShape(
                    context,
                    shapeExport.Shape,
                    origin,
                    out var vertices,
                    out var indices))
            {
                continue;
            }

            builder.AddShape(
                vertices,
                indices,
                shapeExport.Shape.IfcProductLabel,
                shapeExport.StyleLabel);
        }

        fragments = builder.Build();
        var triangleCount = fragments.Sum(fragment => fragment.TriangleCount);
        Console.WriteLine(
            $"Surface overview attempt {attempt + 1}: {triangleCount:N0} triangles " +
            $"at {clusterSize:G6} m interior / " +
            $"{boundaryClusterSize:G6} m boundary clustering.");
        if (triangleCount <= targetTriangles || attempt == maximumAttempts - 1)
        {
            return fragments;
        }

        var scale = Math.Clamp(
            Math.Sqrt((double)triangleCount / targetTriangles),
            1.35d,
            3d);
        clusterSize *= scale;
        boundaryClusterSize *= scale;
    }

    return fragments;
}

static bool TryReadTransformedShape(
    Xbim3DModelContext context,
    XbimShapeInstance shapeInstance,
    Vector3d origin,
    out Vector3d[] vertices,
    out int[] indices)
{
    vertices = Array.Empty<Vector3d>();
    indices = Array.Empty<int>();
    var geometry = context.ShapeGeometry(shapeInstance);
    var shapeData = ((IXbimShapeGeometryData)geometry).ShapeData;
    if (shapeData is not { Length: > 0 })
    {
        return false;
    }

    using var shapeStream = new MemoryStream(shapeData, writable: false);
    using var shapeReader = new BinaryReader(shapeStream);
    var triangulation = shapeReader.ReadShapeTriangulation();
    var sourceVertices = triangulation.Vertices.ToList();
    indices = triangulation.Faces.SelectMany(face => face.Indices).ToArray();
    if (sourceVertices.Count == 0 || indices.Length < 3)
    {
        return false;
    }

    vertices = new Vector3d[sourceVertices.Count];
    for (var index = 0; index < sourceVertices.Count; index++)
    {
        var transformed = shapeInstance.Transformation.Transform(sourceVertices[index]);
        vertices[index] = new Vector3d(
            transformed.X - origin.X,
            transformed.Y - origin.Y,
            transformed.Z - origin.Z);
    }

    ValidateIndices(indices, vertices.Length);
    return true;
}

static void WriteOverviewFragment(
    BinaryWriter writer,
    OverviewFragment fragment,
    string objectName)
{
    var recordStart = writer.BaseStream.Position;
    writer.Write(0L);
    writer.Write(fragment.Cell.X);
    writer.Write(fragment.Cell.Y);
    writer.Write(fragment.Cell.Z);
    writer.Write((float)fragment.Minimum.X);
    writer.Write((float)fragment.Minimum.Y);
    writer.Write((float)fragment.Minimum.Z);
    writer.Write((float)fragment.Maximum.X);
    writer.Write((float)fragment.Maximum.Y);
    writer.Write((float)fragment.Maximum.Z);
    writer.Write(fragment.TriangleCount);
    writer.Write(fragment.Vertices.Length);
    writer.Write(fragment.IndexCount);
    writer.Write(0);
    writer.Write(0);

    writer.Write(objectName);
    writer.Write(0);
    writer.Write(fragment.Vertices.Length);
    foreach (var vertex in fragment.Vertices)
    {
        writer.Write((float)vertex.X);
        writer.Write((float)vertex.Y);
        writer.Write((float)vertex.Z);
    }

    writer.Write(fragment.Normals.Length);
    foreach (var normal in fragment.Normals)
    {
        writer.Write((float)normal.X);
        writer.Write((float)normal.Y);
        writer.Write((float)normal.Z);
    }

    writer.Write(0); // Surface overview materials currently do not use UVs.
    writer.Write(0); // Surface overview materials currently do not use tangents.
    writer.Write(fragment.SubMeshIndices.Length);
    for (var subMesh = 0; subMesh < fragment.SubMeshIndices.Length; subMesh++)
    {
        writer.Write(fragment.StyleLabels[subMesh]);
        var indices = fragment.SubMeshIndices[subMesh];
        writer.Write(indices.Length);
        foreach (var index in indices)
        {
            writer.Write(index);
        }
    }

    writer.Write(fragment.TriangleProductLabels.Length);
    foreach (var productLabel in fragment.TriangleProductLabels)
    {
        writer.Write(productLabel);
    }

    var recordEnd = writer.BaseStream.Position;
    writer.BaseStream.Position = recordStart;
    writer.Write(recordEnd - recordStart - sizeof(long));
    writer.BaseStream.Position = recordEnd;
}

static bool TryFindLocalOrigin(
    Xbim3DModelContext context,
    IReadOnlyList<ShapeExport> shapeExports,
    out Vector3d origin)
{
    foreach (var shapeExport in shapeExports)
    {
        var geometry = context.ShapeGeometry(shapeExport.Shape);
        var shapeData = ((IXbimShapeGeometryData)geometry).ShapeData;
        if (shapeData is not { Length: > 0 })
        {
            continue;
        }

        using var shapeStream = new MemoryStream(shapeData, writable: false);
        using var shapeReader = new BinaryReader(shapeStream);
        var vertices = shapeReader
            .ReadShapeTriangulation()
            .Vertices
            .ToList();
        if (vertices.Count == 0)
        {
            continue;
        }

        var firstVertex = vertices[0];
        var transformed = shapeExport.Shape.Transformation.Transform(firstVertex);
        origin = new Vector3d(transformed.X, transformed.Y, transformed.Z);
        return true;
    }

    origin = Vector3d.Zero;
    return false;
}

static bool TryCreateProjectedGeoReference(
    IfcStore model,
    Vector3d localOrigin,
    double metresPerUnit,
    out ProjectedGeoReference geoReference)
{
    geoReference = default;
    var mapConversion = model.Instances.FirstOrDefault(
        entity => entity.GetType().Name.Contains(
            "IfcMapConversion",
            StringComparison.OrdinalIgnoreCase));
    if (mapConversion == null)
    {
        Console.Error.WriteLine("IFC model does not expose an IfcMapConversion entity.");
        return false;
    }

    if (!TryReadNumber(ReadMember(mapConversion, "Eastings"), out var eastings) ||
        !TryReadNumber(ReadMember(mapConversion, "Northings"), out var northings))
    {
        Console.Error.WriteLine(
            $"IfcMapConversion #{mapConversion.EntityLabel} has invalid Eastings or Northings.");
        return false;
    }

    var targetCrs = ReadMember(mapConversion, "TargetCRS");
    var projectedCrs = ReadText(targetCrs, "Name");
    if (string.IsNullOrWhiteSpace(projectedCrs))
    {
        Console.Error.WriteLine(
            $"IfcMapConversion #{mapConversion.EntityLabel} has no projected CRS name.");
        return false;
    }

    var orthogonalHeight = ReadOptionalNumber(
        mapConversion,
        "OrthogonalHeight",
        0d);
    var xAxisAbscissa = ReadOptionalNumber(
        mapConversion,
        "XAxisAbscissa",
        1d);
    var xAxisOrdinate = ReadOptionalNumber(
        mapConversion,
        "XAxisOrdinate",
        0d);
    var scale = ReadOptionalNumber(mapConversion, "Scale", 1d);
    var axisLength = Math.Sqrt(
        xAxisAbscissa * xAxisAbscissa +
        xAxisOrdinate * xAxisOrdinate);

    if (!double.IsFinite(scale) ||
        scale <= 0d ||
        !double.IsFinite(axisLength) ||
        axisLength <= 1e-12d)
    {
        return false;
    }

    xAxisAbscissa /= axisLength;
    xAxisOrdinate /= axisLength;

    var localX = localOrigin.X * metresPerUnit;
    var localY = localOrigin.Y * metresPerUnit;
    var easting =
        eastings +
        scale * (xAxisAbscissa * localX - xAxisOrdinate * localY);
    var northing =
        northings +
        scale * (xAxisOrdinate * localX + xAxisAbscissa * localY);
    var elevation =
        orthogonalHeight +
        scale * localOrigin.Z * metresPerUnit;

    if (!TryConvertProjectedToWgs84(
            projectedCrs,
            easting,
            northing,
            out var latitude,
            out var longitude))
    {
        Console.Error.WriteLine(
            $"Unsupported IFC projected CRS '{projectedCrs}'. " +
            "RefLatitude and RefLongitude will be used when available.");
        return false;
    }

    geoReference = new ProjectedGeoReference(
        projectedCrs,
        easting,
        northing,
        elevation,
        latitude,
        longitude,
        xAxisAbscissa,
        xAxisOrdinate,
        scale);
    return true;
}

static double ReadOptionalNumber(
    object instance,
    string memberName,
    double fallback)
{
    return TryReadNumber(ReadMember(instance, memberName), out var value)
        ? value
        : fallback;
}

static bool TryConvertProjectedToWgs84(
    string projectedCrs,
    double easting,
    double northing,
    out double latitude,
    out double longitude)
{
    latitude = 0d;
    longitude = 0d;
    if (!projectedCrs.Contains(
            "VN2000",
            StringComparison.OrdinalIgnoreCase) &&
        !projectedCrs.Contains(
            "VN-2000",
            StringComparison.OrdinalIgnoreCase))
    {
        return false;
    }

    var centralMeridian = 0d;
    var scaleFactor = 0d;
    var utmMatch = Regex.Match(
        projectedCrs,
        @"UTM[^0-9]*(?<zone>[0-9]{1,2})",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    if (utmMatch.Success &&
        int.TryParse(
            utmMatch.Groups["zone"].Value,
            NumberStyles.None,
            CultureInfo.InvariantCulture,
            out var zone) &&
        zone is >= 1 and <= 60)
    {
        centralMeridian = zone * 6d - 183d;
        scaleFactor = 0.9996d;
    }
    else
    {
        // Autodesk's VN2000_*d* names use Vietnam's 3-degree TM grid.
        var meridianMatch = Regex.Match(
            projectedCrs,
            @"(?<degrees>[0-9]{3})[dD](?<minutes>[0-9]{2})",
            RegexOptions.CultureInvariant);
        if (!meridianMatch.Success ||
            !double.TryParse(
                meridianMatch.Groups["degrees"].Value,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var degrees) ||
            !double.TryParse(
                meridianMatch.Groups["minutes"].Value,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var minutes) ||
            minutes >= 60d)
        {
            return false;
        }

        centralMeridian = degrees + minutes / 60d;
        scaleFactor = 0.9999d;
    }

    InverseTransverseMercator(
        easting,
        northing,
        centralMeridian,
        scaleFactor,
        500_000d,
        0d,
        out latitude,
        out longitude);
    TransformVn2000ToWgs84(ref latitude, ref longitude);

    return double.IsFinite(latitude) &&
           double.IsFinite(longitude) &&
           latitude is >= -90d and <= 90d &&
           longitude is >= -180d and <= 180d;
}

static void InverseTransverseMercator(
    double easting,
    double northing,
    double centralMeridianDegrees,
    double scaleFactor,
    double falseEasting,
    double falseNorthing,
    out double latitudeDegrees,
    out double longitudeDegrees)
{
    const double semiMajorAxis = 6_378_137d;
    const double inverseFlattening = 298.257_223_563d;
    var flattening = 1d / inverseFlattening;
    var eccentricitySquared = flattening * (2d - flattening);
    var secondEccentricitySquared =
        eccentricitySquared / (1d - eccentricitySquared);
    var eccentricityFourth = eccentricitySquared * eccentricitySquared;
    var eccentricitySixth = eccentricityFourth * eccentricitySquared;

    var meridionalArc = (northing - falseNorthing) / scaleFactor;
    var mu = meridionalArc /
             (semiMajorAxis *
              (1d -
               eccentricitySquared / 4d -
               3d * eccentricityFourth / 64d -
               5d * eccentricitySixth / 256d));
    var e1 =
        (1d - Math.Sqrt(1d - eccentricitySquared)) /
        (1d + Math.Sqrt(1d - eccentricitySquared));
    var e1Squared = e1 * e1;
    var e1Cubed = e1Squared * e1;
    var e1Fourth = e1Squared * e1Squared;
    var footprintLatitude =
        mu +
        (3d * e1 / 2d - 27d * e1Cubed / 32d) * Math.Sin(2d * mu) +
        (21d * e1Squared / 16d - 55d * e1Fourth / 32d) * Math.Sin(4d * mu) +
        151d * e1Cubed / 96d * Math.Sin(6d * mu) +
        1_097d * e1Fourth / 512d * Math.Sin(8d * mu);

    var sine = Math.Sin(footprintLatitude);
    var cosine = Math.Cos(footprintLatitude);
    var tangent = Math.Tan(footprintLatitude);
    var tangentSquared = tangent * tangent;
    var c1 = secondEccentricitySquared * cosine * cosine;
    var n1 = semiMajorAxis /
             Math.Sqrt(1d - eccentricitySquared * sine * sine);
    var r1 =
        semiMajorAxis * (1d - eccentricitySquared) /
        Math.Pow(1d - eccentricitySquared * sine * sine, 1.5d);
    var d = (easting - falseEasting) / (n1 * scaleFactor);
    var d2 = d * d;
    var d3 = d2 * d;
    var d4 = d2 * d2;
    var d5 = d4 * d;
    var d6 = d3 * d3;

    var latitude =
        footprintLatitude -
        n1 * tangent / r1 *
        (d2 / 2d -
         (5d + 3d * tangentSquared + 10d * c1 -
          4d * c1 * c1 - 9d * secondEccentricitySquared) *
         d4 / 24d +
         (61d + 90d * tangentSquared +
          298d * c1 + 45d * tangentSquared * tangentSquared -
          252d * secondEccentricitySquared -
          3d * c1 * c1) *
         d6 / 720d);
    var longitude =
        centralMeridianDegrees * Math.PI / 180d +
        (d -
         (1d + 2d * tangentSquared + c1) * d3 / 6d +
         (5d - 2d * c1 + 28d * tangentSquared -
          3d * c1 * c1 +
          8d * secondEccentricitySquared +
          24d * tangentSquared * tangentSquared) *
         d5 / 120d) /
        cosine;

    latitudeDegrees = latitude * 180d / Math.PI;
    longitudeDegrees = longitude * 180d / Math.PI;
}

static void TransformVn2000ToWgs84(
    ref double latitudeDegrees,
    ref double longitudeDegrees)
{
    // EPSG:6960, using the coordinate-frame rotation convention.
    const double semiMajorAxis = 6_378_137d;
    const double inverseFlattening = 298.257_223_563d;
    const double translationX = -191.904_414_29d;
    const double translationY = -39.303_182_79d;
    const double translationZ = -111.450_328_35d;
    const double rotationXArcSeconds = -0.009_288_36d;
    const double rotationYArcSeconds = 0.019_754_79d;
    const double rotationZArcSeconds = -0.004_273_72d;
    const double scalePartsPerMillion = 0.252_906_278d;

    GeodeticToEcef(
        latitudeDegrees,
        longitudeDegrees,
        0d,
        semiMajorAxis,
        inverseFlattening,
        out var sourceX,
        out var sourceY,
        out var sourceZ);

    var arcSecondsToRadians = Math.PI / (180d * 3_600d);
    var rotationX = rotationXArcSeconds * arcSecondsToRadians;
    var rotationY = rotationYArcSeconds * arcSecondsToRadians;
    var rotationZ = rotationZArcSeconds * arcSecondsToRadians;
    var scale = 1d + scalePartsPerMillion * 1e-6d;

    var targetX =
        translationX +
        scale * (sourceX + rotationZ * sourceY - rotationY * sourceZ);
    var targetY =
        translationY +
        scale * (-rotationZ * sourceX + sourceY + rotationX * sourceZ);
    var targetZ =
        translationZ +
        scale * (rotationY * sourceX - rotationX * sourceY + sourceZ);

    EcefToGeodetic(
        targetX,
        targetY,
        targetZ,
        semiMajorAxis,
        inverseFlattening,
        out latitudeDegrees,
        out longitudeDegrees);
}

static void GeodeticToEcef(
    double latitudeDegrees,
    double longitudeDegrees,
    double height,
    double semiMajorAxis,
    double inverseFlattening,
    out double x,
    out double y,
    out double z)
{
    var flattening = 1d / inverseFlattening;
    var eccentricitySquared = flattening * (2d - flattening);
    var latitude = latitudeDegrees * Math.PI / 180d;
    var longitude = longitudeDegrees * Math.PI / 180d;
    var sineLatitude = Math.Sin(latitude);
    var primeVerticalRadius =
        semiMajorAxis /
        Math.Sqrt(1d - eccentricitySquared * sineLatitude * sineLatitude);

    x = (primeVerticalRadius + height) *
        Math.Cos(latitude) *
        Math.Cos(longitude);
    y = (primeVerticalRadius + height) *
        Math.Cos(latitude) *
        Math.Sin(longitude);
    z = (primeVerticalRadius * (1d - eccentricitySquared) + height) *
        sineLatitude;
}

static void EcefToGeodetic(
    double x,
    double y,
    double z,
    double semiMajorAxis,
    double inverseFlattening,
    out double latitudeDegrees,
    out double longitudeDegrees)
{
    var flattening = 1d / inverseFlattening;
    var eccentricitySquared = flattening * (2d - flattening);
    var longitude = Math.Atan2(y, x);
    var horizontal = Math.Sqrt(x * x + y * y);
    var latitude = Math.Atan2(
        z,
        horizontal * (1d - eccentricitySquared));

    for (var iteration = 0; iteration < 8; iteration++)
    {
        var sine = Math.Sin(latitude);
        var primeVerticalRadius =
            semiMajorAxis /
            Math.Sqrt(1d - eccentricitySquared * sine * sine);
        var height = horizontal / Math.Cos(latitude) - primeVerticalRadius;
        latitude = Math.Atan2(
            z,
            horizontal *
            (1d -
             eccentricitySquared *
             primeVerticalRadius /
             (primeVerticalRadius + height)));
    }

    latitudeDegrees = latitude * 180d / Math.PI;
    longitudeDegrees = longitude * 180d / Math.PI;
}

static List<HierarchyNode> BuildHierarchy(
    IfcStore model,
    IReadOnlySet<int> productLabels,
    double metresPerUnit,
    ProjectedGeoReference? projectedGeoReference)
{
    var definitions = model.Instances
        .OfType<IIfcObjectDefinition>()
        .ToDictionary(definition => definition.EntityLabel);
    var parentByChild = new Dictionary<int, int>();

    foreach (var relation in model.Instances.OfType<IIfcRelAggregates>())
    {
        foreach (var child in relation.RelatedObjects)
        {
            parentByChild[child.EntityLabel] = relation.RelatingObject.EntityLabel;
        }
    }

    foreach (var relation in model.Instances.OfType<IIfcRelNests>())
    {
        foreach (var child in relation.RelatedObjects)
        {
            parentByChild[child.EntityLabel] = relation.RelatingObject.EntityLabel;
        }
    }

    foreach (var relation in model.Instances.OfType<IIfcRelContainedInSpatialStructure>())
    {
        foreach (var child in relation.RelatedElements)
        {
            parentByChild[child.EntityLabel] = relation.RelatingStructure.EntityLabel;
        }
    }

    var projectLabel = model.Instances
        .OfType<IIfcProject>()
        .Select(project => project.EntityLabel)
        .FirstOrDefault();

    var requiredLabels = new HashSet<int>();
    if (projectLabel != 0)
    {
        requiredLabels.Add(projectLabel);
    }

    foreach (var productLabel in productLabels)
    {
        var current = productLabel;
        var visited = new HashSet<int>();

        while (current != 0 && visited.Add(current))
        {
            requiredLabels.Add(current);
            current = parentByChild.GetValueOrDefault(current);
        }

        if (!parentByChild.ContainsKey(productLabel) && projectLabel != 0)
        {
            parentByChild[productLabel] = projectLabel;
        }
    }

    var directPropertySets = BuildDirectPropertySetMap(model);
    var typePropertySets = BuildTypePropertySetMap(model);

    return requiredLabels
        .Where(definitions.ContainsKey)
        .OrderBy(label => GetHierarchyDepth(label, parentByChild))
        .ThenBy(label => label)
        .Select(label =>
        {
            var definition = definitions[label];
            var root = definition as IIfcRoot;
            var ifcType = definition.GetType().Name;
            var name = root?.Name?.ToString();
            var displayName = string.IsNullOrWhiteSpace(name)
                ? $"{ifcType}_{label}"
                : name;
            var parentLabel = parentByChild.GetValueOrDefault(label);
            if (!requiredLabels.Contains(parentLabel))
            {
                parentLabel = label == projectLabel ? 0 : projectLabel;
            }

            return new HierarchyNode(
                label,
                parentLabel,
                displayName,
                ifcType,
                root?.GlobalId.ToString() ?? string.Empty,
                ExtractMetadata(
                    definition,
                    directPropertySets.GetValueOrDefault(label),
                    typePropertySets.GetValueOrDefault(label),
                    metresPerUnit,
                    projectedGeoReference));
        })
        .ToList();
}

static Dictionary<int, List<IIfcPropertySet>> BuildDirectPropertySetMap(IfcStore model)
{
    var result = new Dictionary<int, List<IIfcPropertySet>>();

    foreach (var relation in model.Instances.OfType<IIfcRelDefinesByProperties>())
    {
        if (relation.RelatingPropertyDefinition is not IIfcPropertySet propertySet)
        {
            continue;
        }

        foreach (var relatedObject in relation.RelatedObjects)
        {
            AddPropertySet(result, relatedObject.EntityLabel, propertySet);
        }
    }

    return result;
}

static Dictionary<int, List<IIfcPropertySet>> BuildTypePropertySetMap(IfcStore model)
{
    var result = new Dictionary<int, List<IIfcPropertySet>>();

    foreach (var relation in model.Instances.OfType<IIfcRelDefinesByType>())
    {
        var propertySets = EnumerateObjects(
                ReadMember(relation.RelatingType, "HasPropertySets"))
            .OfType<IIfcPropertySet>()
            .ToList();

        foreach (var relatedObject in relation.RelatedObjects)
        {
            foreach (var propertySet in propertySets)
            {
                AddPropertySet(result, relatedObject.EntityLabel, propertySet);
            }
        }
    }

    return result;
}

static void AddPropertySet(
    IDictionary<int, List<IIfcPropertySet>> map,
    int label,
    IIfcPropertySet propertySet)
{
    if (!map.TryGetValue(label, out var sets))
    {
        sets = new List<IIfcPropertySet>();
        map[label] = sets;
    }

    if (sets.All(existing => existing.EntityLabel != propertySet.EntityLabel))
    {
        sets.Add(propertySet);
    }
}

static List<KeyValuePair<string, string>> ExtractMetadata(
    IIfcObjectDefinition definition,
    IEnumerable<IIfcPropertySet>? directPropertySets,
    IEnumerable<IIfcPropertySet>? typePropertySets,
    double metresPerUnit,
    ProjectedGeoReference? projectedGeoReference)
{
    var values = new Dictionary<string, string>(StringComparer.Ordinal);
    AddMetadata(values, "Name", ReadText(definition, "Name"));
    AddMetadata(values, "Description", ReadText(definition, "Description"));
    AddMetadata(values, "Object Type", ReadText(definition, "ObjectType"));
    AddMetadata(values, "Tag", ReadText(definition, "Tag"));

    if (definition.GetType().Name.Contains("IfcSite", StringComparison.OrdinalIgnoreCase))
    {
        AddMetadata(
            values,
            "RefLatitude",
            FormatIfcValue(ReadMember(definition, "RefLatitude")));
        AddMetadata(
            values,
            "RefLongitude",
            FormatIfcValue(ReadMember(definition, "RefLongitude")));

        if (TryReadNumber(ReadMember(definition, "RefElevation"), out var elevation))
        {
            AddMetadata(
                values,
                "RefElevation",
                (elevation * metresPerUnit).ToString("R", CultureInfo.InvariantCulture));
        }

        if (projectedGeoReference is { } mapReference)
        {
            AddMetadata(
                values,
                "MapConversion/OriginLatitude",
                mapReference.Latitude.ToString("R", CultureInfo.InvariantCulture));
            AddMetadata(
                values,
                "MapConversion/OriginLongitude",
                mapReference.Longitude.ToString("R", CultureInfo.InvariantCulture));
            AddMetadata(
                values,
                "MapConversion/OriginElevation",
                mapReference.Elevation.ToString("R", CultureInfo.InvariantCulture));
            AddMetadata(values, "MapConversion/ProjectedCRS", mapReference.ProjectedCrs);
            AddMetadata(
                values,
                "MapConversion/OriginEasting",
                mapReference.Easting.ToString("R", CultureInfo.InvariantCulture));
            AddMetadata(
                values,
                "MapConversion/OriginNorthing",
                mapReference.Northing.ToString("R", CultureInfo.InvariantCulture));
            AddMetadata(
                values,
                "MapConversion/XAxisAbscissa",
                mapReference.XAxisAbscissa.ToString("R", CultureInfo.InvariantCulture));
            AddMetadata(
                values,
                "MapConversion/XAxisOrdinate",
                mapReference.XAxisOrdinate.ToString("R", CultureInfo.InvariantCulture));
            AddMetadata(
                values,
                "MapConversion/Scale",
                mapReference.Scale.ToString("R", CultureInfo.InvariantCulture));
        }
    }

    AppendPropertySets(values, directPropertySets, "Property");
    AppendPropertySets(values, typePropertySets, "Type Property");

    return values.ToList();
}

static void AppendPropertySets(
    IDictionary<string, string> values,
    IEnumerable<IIfcPropertySet>? propertySets,
    string category)
{
    if (propertySets == null)
    {
        return;
    }

    foreach (var propertySet in propertySets)
    {
        var setName = propertySet.Name?.ToString();
        if (string.IsNullOrWhiteSpace(setName))
        {
            setName = $"Pset_{propertySet.EntityLabel}";
        }

        foreach (var property in propertySet.HasProperties)
        {
            var propertyName = property.Name.ToString();
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                propertyName = $"Property_{property.EntityLabel}";
            }

            var value = FormatPropertyValue(property);
            values[$"{category}/{setName}/{propertyName}"] = value;
        }
    }
}

static string FormatPropertyValue(object property)
{
    var simpleValueNames = new[]
    {
        "NominalValue",
        "EnumerationValues",
        "ListValues",
        "PropertyReference",
        "UpperBoundValue",
        "LowerBoundValue",
        "SetPointValue",
        "DefiningValues",
        "DefinedValues"
    };

    var parts = new List<string>();
    var hasValueMember = false;
    foreach (var memberName in simpleValueNames)
    {
        var rawValue = ReadMember(property, memberName);
        hasValueMember |= rawValue != null;
        var formatted = FormatIfcValue(rawValue);
        if (!string.IsNullOrWhiteSpace(formatted))
        {
            parts.Add(formatted);
        }
    }

    if (parts.Count > 0)
    {
        return string.Join(" - ", parts.Distinct(StringComparer.Ordinal));
    }

    if (hasValueMember)
    {
        return string.Empty;
    }

    var nestedProperties = EnumerateObjects(ReadMember(property, "HasProperties"));
    foreach (var nested in nestedProperties)
    {
        var name = ReadText(nested, "Name");
        var value = FormatPropertyValue(nested);
        if (!string.IsNullOrWhiteSpace(value))
        {
            parts.Add(string.IsNullOrWhiteSpace(name) ? value : $"{name}: {value}");
        }
    }

    return parts.Count > 0 ? string.Join("; ", parts) : property.ToString() ?? string.Empty;
}

static string FormatIfcValue(object? value, int depth = 0)
{
    if (value == null || depth > 4)
    {
        return string.Empty;
    }

    if (value is string text)
    {
        return text;
    }

    if (value is IEnumerable enumerable)
    {
        var items = new List<string>();
        foreach (var item in enumerable)
        {
            var formatted = FormatIfcValue(item, depth + 1);
            if (!string.IsNullOrWhiteSpace(formatted))
            {
                items.Add(formatted);
            }
        }

        return string.Join(", ", items);
    }

    var wrappedValue = ReadMember(value, "Value");
    if (wrappedValue != null && !ReferenceEquals(wrappedValue, value))
    {
        var formatted = FormatIfcValue(wrappedValue, depth + 1);
        if (!string.IsNullOrWhiteSpace(formatted))
        {
            return formatted;
        }
    }

    return value is IFormattable formattable
        ? formattable.ToString(null, CultureInfo.InvariantCulture) ?? string.Empty
        : value.ToString() ?? string.Empty;
}

static void AddMetadata(
    IDictionary<string, string> values,
    string key,
    string? value)
{
    if (!string.IsNullOrWhiteSpace(value))
    {
        values[key] = value;
    }
}

static int ResolveStyleLabel(
    IfcStore model,
    XbimShapeInstance shape,
    IDictionary<int, int> productStyleLabels)
{
    if (shape.StyleLabel > 0)
    {
        return shape.StyleLabel;
    }

    if (productStyleLabels.TryGetValue(shape.IfcProductLabel, out var cached))
    {
        return cached;
    }

    var product = model.Instances[shape.IfcProductLabel] as IIfcProduct;
    var resolved = product == null ? 0 : ResolveProductStyleLabel(product);
    productStyleLabels[shape.IfcProductLabel] = resolved;
    return resolved;
}

static int ResolveProductStyleLabel(object product)
{
    // Explicit representation-item styles override material presentation styles.
    var styleLabel = FindSurfaceStyleLabel(ReadMember(product, "Representation"));
    if (styleLabel > 0)
    {
        return styleLabel;
    }

    styleLabel = FindAssociatedMaterialStyleLabel(product);
    if (styleLabel > 0)
    {
        return styleLabel;
    }

    foreach (var relationName in new[] { "IsTypedBy", "IsDefinedBy" })
    {
        foreach (var relation in EnumerateObjects(ReadMember(product, relationName)))
        {
            var relatingType = ReadMember(relation, "RelatingType");
            if (relatingType == null)
            {
                continue;
            }

            styleLabel = FindSurfaceStyleLabel(ReadMember(relatingType, "RepresentationMaps"));
            if (styleLabel > 0)
            {
                return styleLabel;
            }

            styleLabel = FindAssociatedMaterialStyleLabel(relatingType);
            if (styleLabel > 0)
            {
                return styleLabel;
            }
        }
    }

    return 0;
}

static int FindAssociatedMaterialStyleLabel(object productOrType)
{
    foreach (var relation in EnumerateObjects(ReadMember(productOrType, "HasAssociations")))
    {
        var material = ReadMember(relation, "RelatingMaterial");
        if (material == null)
        {
            continue;
        }

        var styleLabel = FindSurfaceStyleLabel(material);
        if (styleLabel > 0)
        {
            return styleLabel;
        }
    }

    return 0;
}

static int FindSurfaceStyleLabel(object? root)
{
    if (root == null)
    {
        return 0;
    }

    string[] traversedMembers =
    [
        "Representations",
        "RepresentationMaps",
        "MappedRepresentation",
        "MappingSource",
        "Items",
        "StyledByItem",
        "Styles",
        "LayerAssignments",
        "LayerStyles",
        "HasRepresentation",
        "ForLayerSet",
        "LayerSet",
        "MaterialLayers",
        "ForProfileSet",
        "ProfileSet",
        "MaterialProfiles",
        "ForConstituentSet",
        "ConstituentSet",
        "MaterialConstituents",
        "Materials",
        "Material"
    ];

    var pending = new Queue<(object Value, int Depth)>();
    var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
    pending.Enqueue((root, 0));

    while (pending.Count > 0)
    {
        var (value, depth) = pending.Dequeue();
        if (depth > 12 || value is string)
        {
            continue;
        }

        if (value is IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                if (item != null)
                {
                    pending.Enqueue((item, depth));
                }
            }

            continue;
        }

        var valueType = value.GetType();
        if (!valueType.IsValueType && !visited.Add(value))
        {
            continue;
        }

        if (IsSurfaceStyleEntity(value) &&
            TryReadNumber(ReadMember(value, "EntityLabel"), out var label))
        {
            return (int)label;
        }

        foreach (var memberName in traversedMembers)
        {
            var memberValue = ReadMember(value, memberName);
            if (memberValue != null)
            {
                pending.Enqueue((memberValue, depth + 1));
            }
        }
    }

    return 0;
}

static bool IsSurfaceStyleEntity(object value)
{
    var valueType = value.GetType();
    return valueType.Name.Equals("IfcSurfaceStyle", StringComparison.OrdinalIgnoreCase) ||
           valueType.GetInterfaces().Any(interfaceType =>
               interfaceType.Name.Equals(
                   "IIfcSurfaceStyle",
                   StringComparison.OrdinalIgnoreCase));
}

static StyleRecord ExtractStyle(IfcStore model, int styleLabel)
{
    var defaultStyle = new StyleRecord(
        styleLabel,
        styleLabel == 0 ? "IFC Default" : $"IFC Style {styleLabel}",
        new Colour(0.78f, 0.8f, 0.82f),
        1f,
        new Colour(0.04f, 0.04f, 0.04f),
        0.45f);

    if (styleLabel <= 0)
    {
        return defaultStyle;
    }

    object? styleEntity;
    try
    {
        styleEntity = model.Instances[styleLabel];
    }
    catch
    {
        return defaultStyle;
    }

    if (styleEntity == null)
    {
        return defaultStyle;
    }

    var name = ReadText(styleEntity, "Name");
    var style = defaultStyle with
    {
        Name = string.IsNullOrWhiteSpace(name) ? defaultStyle.Name : name
    };

    var styleItems = EnumerateObjects(ReadMember(styleEntity, "Styles")).ToList();
    if (styleItems.Count == 0)
    {
        styleItems.Add(styleEntity);
    }

    foreach (var styleItem in styleItems)
    {
        if (TryReadColour(ReadMember(styleItem, "SurfaceColour"), out var diffuse))
        {
            style = style with { Diffuse = diffuse };
        }

        var diffuseValue = ReadMember(styleItem, "DiffuseColour");
        if (TryReadColour(diffuseValue, out var renderedDiffuse))
        {
            style = style with { Diffuse = renderedDiffuse };
        }
        else if (TryReadNumber(diffuseValue, out var diffuseFactor))
        {
            var factor = Clamp01(diffuseFactor);
            style = style with
            {
                Diffuse = new Colour(
                    style.Diffuse.R * factor,
                    style.Diffuse.G * factor,
                    style.Diffuse.B * factor)
            };
        }

        if (TryReadNumber(ReadMember(styleItem, "Transparency"), out var transparency))
        {
            style = style with { Alpha = Clamp01(1d - transparency) };
        }

        var specularValue = ReadMember(styleItem, "SpecularColour");
        if (TryReadColour(specularValue, out var specularColour))
        {
            style = style with { Specular = specularColour };
        }
        else if (TryReadNumber(specularValue, out var specularFactor))
        {
            var value = Clamp01(specularFactor);
            style = style with { Specular = new Colour(value, value, value) };
        }

        var highlight = ReadMember(styleItem, "SpecularHighlight");
        if (TryReadNumber(highlight, out var highlightValue))
        {
            var typeName = highlight?.GetType().Name ?? string.Empty;
            var smoothness = typeName.Contains("Roughness", StringComparison.OrdinalIgnoreCase)
                ? 1d - highlightValue
                : 1d - Math.Sqrt(2d / Math.Max(2d, highlightValue + 2d));
            style = style with { Smoothness = Clamp01(smoothness) };
        }
    }

    return style;
}

static bool TryReadColour(object? value, out Colour colour)
{
    colour = default;
    if (!TryReadNumber(ReadMember(value, "Red"), out var red) ||
        !TryReadNumber(ReadMember(value, "Green"), out var green) ||
        !TryReadNumber(ReadMember(value, "Blue"), out var blue))
    {
        return false;
    }

    colour = new Colour(Clamp01(red), Clamp01(green), Clamp01(blue));
    return true;
}

static bool TryReadNumber(object? value, out double number, int depth = 0)
{
    number = 0d;
    if (value == null || depth > 4)
    {
        return false;
    }

    if (value is IConvertible convertible)
    {
        try
        {
            number = convertible.ToDouble(CultureInfo.InvariantCulture);
            return double.IsFinite(number);
        }
        catch
        {
            // Fall through to xBIM's wrapped Value property.
        }
    }

    var wrappedValue = ReadMember(value, "Value");
    return wrappedValue != null &&
           !ReferenceEquals(wrappedValue, value) &&
           TryReadNumber(wrappedValue, out number, depth + 1);
}

static object? ReadMember(object? instance, string memberName)
{
    if (instance == null)
    {
        return null;
    }

    var instanceType = instance.GetType();
    var property = instanceType.GetProperty(
        memberName,
        BindingFlags.Instance | BindingFlags.Public);
    if (property != null)
    {
        return property.GetValue(instance);
    }

    foreach (var interfaceType in instanceType.GetInterfaces())
    {
        property = interfaceType.GetProperty(memberName);
        if (property != null)
        {
            return property.GetValue(instance);
        }
    }

    return null;
}

static string ReadText(object? instance, string memberName)
{
    return ReadMember(instance, memberName)?.ToString() ?? string.Empty;
}

static IEnumerable<object> EnumerateObjects(object? value)
{
    if (value is not IEnumerable enumerable || value is string)
    {
        yield break;
    }

    foreach (var item in enumerable)
    {
        if (item != null)
        {
            yield return item;
        }
    }
}

static int GetHierarchyDepth(
    int label,
    IReadOnlyDictionary<int, int> parentByChild)
{
    var depth = 0;
    var current = label;
    var visited = new HashSet<int>();

    while (parentByChild.TryGetValue(current, out current) &&
           current != 0 &&
           visited.Add(current) &&
           depth < 256)
    {
        depth++;
    }

    return depth;
}

static void ValidateIndices(IEnumerable<int> indices, int vertexCount)
{
    foreach (var index in indices)
    {
        if (index < 0 || index >= vertexCount)
        {
            throw new InvalidDataException(
                $"Triangulation index {index} is outside the vertex range {vertexCount}.");
        }
    }
}

static Vector3d[] CalculateNormals(
    IReadOnlyList<Vector3d> vertices,
    IReadOnlyList<int> indices)
{
    var normals = new Vector3d[vertices.Count];

    for (var index = 0; index + 2 < indices.Count; index += 3)
    {
        var first = indices[index];
        var second = indices[index + 1];
        var third = indices[index + 2];
        var normal = Vector3d.Cross(
            vertices[second] - vertices[first],
            vertices[third] - vertices[first]);

        normals[first] += normal;
        normals[second] += normal;
        normals[third] += normal;
    }

    for (var index = 0; index < normals.Length; index++)
    {
        normals[index] = normals[index].NormalizedOr(Vector3d.Up);
    }

    return normals;
}

static Uv[] CalculateBoxProjectedUvs(
    IReadOnlyList<Vector3d> vertices,
    IReadOnlyList<Vector3d> normals,
    double metresPerUnit)
{
    var uvs = new Uv[vertices.Count];

    for (var index = 0; index < vertices.Count; index++)
    {
        var vertex = vertices[index] * metresPerUnit;
        var normal = normals[index].Absolute;

        uvs[index] = normal.X >= normal.Y && normal.X >= normal.Z
            ? new Uv((float)vertex.Y, (float)vertex.Z)
            : normal.Y >= normal.Z
                ? new Uv((float)vertex.X, (float)vertex.Z)
                : new Uv((float)vertex.X, (float)vertex.Y);
    }

    return uvs;
}

static Tangent[] CalculateTangents(IReadOnlyList<Vector3d> normals)
{
    var tangents = new Tangent[normals.Count];

    for (var index = 0; index < normals.Count; index++)
    {
        var normal = normals[index];
        var seed = Math.Abs(normal.Z) < 0.9d
            ? new Vector3d(0d, 0d, 1d)
            : new Vector3d(0d, 1d, 0d);
        var tangent = (seed - normal * Vector3d.Dot(seed, normal))
            .NormalizedOr(new Vector3d(1d, 0d, 0d));

        tangents[index] = new Tangent(
            (float)tangent.X,
            (float)tangent.Y,
            (float)tangent.Z,
            1f);
    }

    return tangents;
}

static float Clamp01(double value)
{
    return (float)Math.Clamp(value, 0d, 1d);
}

internal readonly record struct SpatialCell(int X, int Y, int Z);

internal sealed record MeshFragment(
    SpatialCell Cell,
    Vector3d Minimum,
    Vector3d Maximum,
    Vector3d[] Vertices,
    Vector3d[] Normals,
    Uv[] Uvs,
    Tangent[] Tangents,
    int[] Indices)
{
    public int TriangleCount => Indices.Length / 3;
}

internal sealed class MeshFragmentBuilder
{
    private readonly IReadOnlyList<Vector3d> sourceVertices;
    private readonly IReadOnlyList<Vector3d> sourceNormals;
    private readonly IReadOnlyList<Uv> sourceUvs;
    private readonly IReadOnlyList<Tangent> sourceTangents;
    private readonly Dictionary<int, int> localIndexBySource = new();
    private readonly List<Vector3d> vertices = new();
    private readonly List<Vector3d> normals = new();
    private readonly List<Uv> uvs = new();
    private readonly List<Tangent> tangents = new();
    private readonly List<int> indices = new();
    private Vector3d minimum;
    private Vector3d maximum;
    private bool hasBounds;

    public MeshFragmentBuilder(
        SpatialCell cell,
        IReadOnlyList<Vector3d> sourceVertices,
        IReadOnlyList<Vector3d> sourceNormals,
        IReadOnlyList<Uv> sourceUvs,
        IReadOnlyList<Tangent> sourceTangents)
    {
        Cell = cell;
        this.sourceVertices = sourceVertices;
        this.sourceNormals = sourceNormals;
        this.sourceUvs = sourceUvs;
        this.sourceTangents = sourceTangents;
    }

    public SpatialCell Cell { get; }
    public int TriangleCount => indices.Count / 3;

    public void AddTriangle(int index0, int index1, int index2)
    {
        indices.Add(GetOrAddVertex(index0));
        indices.Add(GetOrAddVertex(index1));
        indices.Add(GetOrAddVertex(index2));
    }

    public MeshFragment Build()
    {
        return new MeshFragment(
            Cell,
            minimum,
            maximum,
            vertices.ToArray(),
            normals.ToArray(),
            uvs.ToArray(),
            tangents.ToArray(),
            indices.ToArray());
    }

    private int GetOrAddVertex(int sourceIndex)
    {
        if (localIndexBySource.TryGetValue(sourceIndex, out var localIndex))
        {
            return localIndex;
        }

        localIndex = vertices.Count;
        localIndexBySource.Add(sourceIndex, localIndex);
        var vertex = sourceVertices[sourceIndex];
        vertices.Add(vertex);
        if (sourceNormals.Count == sourceVertices.Count)
        {
            normals.Add(sourceNormals[sourceIndex]);
        }

        if (sourceUvs.Count == sourceVertices.Count)
        {
            uvs.Add(sourceUvs[sourceIndex]);
        }

        if (sourceTangents.Count == sourceVertices.Count)
        {
            tangents.Add(sourceTangents[sourceIndex]);
        }

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
}

internal readonly record struct ShapeExport(
    XbimShapeInstance Shape,
    int StyleLabel);

internal readonly record struct ProjectedGeoReference(
    string ProjectedCrs,
    double Easting,
    double Northing,
    double Elevation,
    double Latitude,
    double Longitude,
    double XAxisAbscissa,
    double XAxisOrdinate,
    double Scale);

internal sealed record HierarchyNode(
    int Label,
    int ParentLabel,
    string Name,
    string IfcType,
    string GlobalId,
    IReadOnlyList<KeyValuePair<string, string>> Properties);

internal sealed record StyleRecord(
    int Label,
    string Name,
    Colour Diffuse,
    float Alpha,
    Colour Specular,
    float Smoothness);

internal readonly record struct Colour(float R, float G, float B);
internal readonly record struct Uv(float X, float Y);
internal readonly record struct Tangent(float X, float Y, float Z, float W);

internal readonly record struct Vector3d(double X, double Y, double Z)
{
    public static Vector3d Zero => new(0d, 0d, 0d);
    public static Vector3d Up => new(0d, 0d, 1d);
    public Vector3d Absolute => new(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));

    public static Vector3d operator +(Vector3d left, Vector3d right)
    {
        return new Vector3d(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    }

    public static Vector3d operator -(Vector3d left, Vector3d right)
    {
        return new Vector3d(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }

    public static Vector3d operator *(Vector3d vector, double scalar)
    {
        return new Vector3d(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);
    }

    public static double Dot(Vector3d left, Vector3d right)
    {
        return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
    }

    public static Vector3d Cross(Vector3d left, Vector3d right)
    {
        return new Vector3d(
            left.Y * right.Z - left.Z * right.Y,
            left.Z * right.X - left.X * right.Z,
            left.X * right.Y - left.Y * right.X);
    }

    public Vector3d NormalizedOr(Vector3d fallback)
    {
        var magnitude = Math.Sqrt(Dot(this, this));
        return magnitude > 1e-12d ? this * (1d / magnitude) : fallback;
    }
}
