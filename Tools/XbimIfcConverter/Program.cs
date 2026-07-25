using System.Collections;
using System.Globalization;
using System.Reflection;
using Xbim.Common.Configuration;
using Xbim.Common.Geometry;
using Xbim.Common.XbimExtensions;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.ModelGeometry.Scene;

const uint Magic = 0x4D494258; // "XBIM" in little-endian files.
const int FormatVersion = 2;
const double DefaultLinearDeflectionMillimetres = 5d;
const double DefaultAngularDeflectionDegrees = 30d;

if (args.Length is not 2 and not 4)
{
    Console.Error.WriteLine(
        "Usage: XbimIfcConverter <input.ifc> <output.xbimmesh> " +
        "[linearDeflectionMillimetres angularDeflectionDegrees]");
    return 2;
}

var inputPath = Path.GetFullPath(args[0]);
var outputPath = Path.GetFullPath(args[1]);
var linearDeflectionMillimetres = DefaultLinearDeflectionMillimetres;
var angularDeflectionDegrees = DefaultAngularDeflectionDegrees;

if (args.Length == 4 &&
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

if (!double.IsFinite(linearDeflectionMillimetres) ||
    linearDeflectionMillimetres is < 0.01d or > 1000d ||
    !double.IsFinite(angularDeflectionDegrees) ||
    angularDeflectionDegrees is < 1d or > 90d)
{
    Console.Error.WriteLine(
        "Linear deflection must be 0.01-1000 mm and angular deflection must be 1-90 degrees.");
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

    var hierarchy = BuildHierarchy(model, productLabels);
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

    var originPosition = output.Position;
    writer.Write(0d);
    writer.Write(0d);
    writer.Write(0d);

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
    var hasOrigin = false;
    var origin = Vector3d.Zero;

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

        if (!hasOrigin)
        {
            origin = transformedVertices[0];
            hasOrigin = true;
        }

        for (var index = 0; index < transformedVertices.Length; index++)
        {
            transformedVertices[index] -= origin;
        }

        ValidateIndices(indices, transformedVertices.Length);
        var normals = CalculateNormals(transformedVertices, indices);
        var uvs = CalculateBoxProjectedUvs(
            transformedVertices,
            normals,
            metresPerUnit);
        var tangents = CalculateTangents(normals);

        var product = model.Instances[shapeInstance.IfcProductLabel] as IIfcProduct;
        var ifcType = product?.GetType().Name ?? "IfcProduct";
        var displayName = ReadText(product, "Name");
        var objectName = string.IsNullOrWhiteSpace(displayName)
            ? $"{ifcType}_{shapeInstance.IfcProductLabel}"
            : displayName;

        writer.Write($"{objectName}_Mesh_{meshCount + 1}");
        writer.Write(shapeInstance.IfcProductLabel);

        writer.Write(transformedVertices.Length);
        foreach (var vertex in transformedVertices)
        {
            writer.Write((float)vertex.X);
            writer.Write((float)vertex.Y);
            writer.Write((float)vertex.Z);
        }

        writer.Write(normals.Length);
        foreach (var normal in normals)
        {
            writer.Write((float)normal.X);
            writer.Write((float)normal.Y);
            writer.Write((float)normal.Z);
        }

        writer.Write(uvs.Length);
        foreach (var uv in uvs)
        {
            writer.Write(uv.X);
            writer.Write(uv.Y);
        }

        writer.Write(tangents.Length);
        foreach (var tangent in tangents)
        {
            writer.Write(tangent.X);
            writer.Write(tangent.Y);
            writer.Write(tangent.Z);
            writer.Write(tangent.W);
        }

        writer.Write(1); // One style per xBIM shape instance.
        writer.Write(shapeExport.StyleLabel);
        writer.Write(indices.Length);
        foreach (var index in indices)
        {
            writer.Write(index);
        }

        meshCount++;
    }

    writer.Flush();
    var endPosition = output.Position;
    output.Position = originPosition;
    writer.Write(origin.X);
    writer.Write(origin.Y);
    writer.Write(origin.Z);
    output.Position = meshCountPosition;
    writer.Write(meshCount);
    output.Position = endPosition;

    Console.WriteLine(
        $"Converted {meshCount} meshes, {hierarchy.Count} hierarchy nodes, " +
        $"{styles.Count} styles at {metresPerUnit:G6} metres/unit with " +
        $"{linearDeflectionMillimetres:G6} mm linear / " +
        $"{angularDeflectionDegrees:G6} deg angular deflection to {outputPath}");
    return 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception);
    return 1;
}

static List<HierarchyNode> BuildHierarchy(
    IfcStore model,
    IReadOnlySet<int> productLabels)
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
                    typePropertySets.GetValueOrDefault(label)));
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
    IEnumerable<IIfcPropertySet>? typePropertySets)
{
    var values = new Dictionary<string, string>(StringComparer.Ordinal);
    AddMetadata(values, "Name", ReadText(definition, "Name"));
    AddMetadata(values, "Description", ReadText(definition, "Description"));
    AddMetadata(values, "Object Type", ReadText(definition, "ObjectType"));
    AddMetadata(values, "Tag", ReadText(definition, "Tag"));

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

internal readonly record struct ShapeExport(
    XbimShapeInstance Shape,
    int StyleLabel);

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
