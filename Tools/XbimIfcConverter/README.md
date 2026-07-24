# xBIM IFC Converter

This helper keeps xBIM's .NET 8 and native OpenCASCADE geometry stack outside
Unity's Mono/IL2CPP process. It converts IFC files into a compact versioned
stream that `XbimIfcLoader` reads at runtime.

The stream includes:

- the IFC project length-unit conversion to metres
- project/site/building/storey/product hierarchy
- property-set and type-property metadata
- IFC surface colours, transparency, specular colour, and smoothness
- positions, normals, generated box-projection UVs, tangents, and sub-meshes

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

The Unity Windows build processor publishes and copies this helper into the
player automatically. Runtime IFC geometry import is Windows x64 only.
