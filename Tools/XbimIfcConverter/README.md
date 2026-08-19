# xBIM IFC Converter

This helper keeps xBIM's .NET 8 and native OpenCASCADE geometry stack outside
Unity's Mono/IL2CPP process. It converts IFC files into a compact versioned
stream that `XbimIfcLoader` reads at runtime.

The version 6 stream includes:

- the IFC project length-unit conversion to metres
- project/site/building/storey/product hierarchy
- property-set and type-property metadata
- IFC surface colours, transparency, specular colour, and smoothness
- positions, optional normals, generated box-projection UVs, tangents, and sub-meshes
- a compact random-access header for every mesh fragment
- model-local bounds, product identity, material identity, and spatial-cell keys

Geometry is split by triangle centroid into metre-sized spatial cells and each
record also has a hard triangle limit. Unity reads the hierarchy and compact
record headers once, then seeks directly to only the fragment payloads needed
by the current camera. The complete geometry therefore remains persistent on
disk without requiring matching Unity Mesh, MeshRenderer, collider, or GPU
buffer allocations.

IFC commonly contains surface styles but no reusable texture-coordinate data.
The converter therefore generates deterministic metre-scaled UVs from the
triangulated geometry.

The converter is intentionally a sidecar executable. Installing xBIM assemblies
directly into Unity through NuGetForUnity would load a .NET 8/C++ geometry stack
that is incompatible with Unity's Mono and IL2CPP runtimes.

## Local publish

```powershell
dotnet publish XbimIfcConverter.csproj --configuration Release `
  --runtime win-x64 --self-contained true
```

## Tessellation controls

The optional converter arguments are:

```text
XbimIfcConverter input.ifc output.xbimmesh linearDeflectionMm angularDeflectionDegrees \
  spatialCellSizeMetres maximumTrianglesPerFragment overviewTargetTriangles \
  overviewClusterSizeMetres overviewBoundaryClusterSizeMetres overviewRegionSizeMetres \
  writeNormals writeTextureCoordinates writeTangents
```

Unity exposes the same values on `XbimIfcLoader`. Higher values produce fewer
triangles on curved geometry:

- `5 mm / 30 degrees` is the balanced runtime default.
- `20 mm / 45 degrees` is a useful coarse-performance preset.
- `1 mm / 10 degrees` is a fine-quality preset with substantially more geometry.

Linear deflection is converted from millimetres to the IFC model's native unit
before xBIM creates its geometry context.

The spatial defaults are `100 m` cells and at most `20,000` triangles per
fragment. These are cache-layout controls rather than view settings; changing
either value produces a new fingerprinted cache on the next runtime import.

Normals default to disabled in Unity because the current IFC material is
unlit. Enable `importNormals` when switching to a lit shader; the setting is
part of the cache fingerprint and safely regenerates older converted models.

## Material colour resolution

The converter preserves xBIM's explicit per-shape style first. When an exporter
does not place that style on the generated shape, it resolves the colour from:

1. the product's styled representation items
2. its associated IFC material presentation
3. its type-level representation or material presentation

`IfcSurfaceStyleRendering.DiffuseColour`, colour factors, transparency,
specular colour, and the base surface colour are transferred into cached Unity
materials. Shapes with different IFC style labels receive different material
instances.

The Unity Windows build processor publishes and copies this helper into the
player automatically. Runtime IFC geometry import is Windows x64 only.
