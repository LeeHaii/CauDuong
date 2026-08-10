# CauDuong

CauDuong is a Windows desktop digital-twin viewer for roads, bridges, and other civil infrastructure. It turns IFC models into an interactive Unity scene where operators can inspect assets in geographic context, review BIM metadata, record operational information, take measurements, and export reports.

The project is built as a working application rather than a generic Unity template. Its main scene and dashboard are focused on infrastructure model coordination and field/maintenance workflows.

## What it does

- Imports one or more `.ifc` files while the application is running.
- Converts IFC geometry, hierarchy, materials, property sets, and type properties through a separate xBIM process.
- Optimizes large models with configurable tessellation, mesh simplification, LOD proxies, batching, and memory controls.
- Extracts IFC georeferencing and places models on an ArcGIS map, including VN-2000 to WGS84 coordinate conversion.
- Provides selectable street, imagery, topographic, terrain, ocean, and canvas basemaps.
- Lets users select infrastructure elements, inspect/edit metadata, assign condition states, and add maintenance notes.
- Stores model records, property overrides, field inspections, attached photos, and resolution state in a local SQLite database.
- Supports distance, height, and area measurements.
- Exports operational data and analytics as CSV, JSON, and PDF reports.
- Includes orbit/pan/zoom navigation and touch input.

## Technology

| Area | Technology |
| --- | --- |
| Engine | Unity `6000.3.17f1`, C# |
| Rendering | Universal Render Pipeline `17.3.0` |
| Interface | UI Toolkit, UGUI, TextMesh Pro |
| IFC pipeline | xBIM Geometry `6.3.891-netcore` in a self-contained .NET 8 sidecar |
| Mapping | ArcGIS Maps SDK for Unity, OpenStreetMap tile mathematics, VN-2000/WGS84 conversion |
| Persistence | Windows SQLite (`winsqlite3`) |
| Model optimization | UnityMeshSimplifier and Pixyz tooling |
| Asset/runtime systems | Addressables, glTFast, Input System, AI Navigation |
| Quality | Unity Test Framework with Edit Mode tests |

The xBIM converter deliberately runs outside Unity. xBIM's .NET 8 and native OpenCASCADE stack is not compatible with Unity's Mono/IL2CPP runtime, so the helper produces a compact binary mesh stream that Unity reads asynchronously.

## Requirements

- Windows 10 or 11, x64
- Unity `6000.3.17f1`
- .NET 8 SDK (required to publish the xBIM converter when making a Windows build)
- Internet access for online ArcGIS basemaps

Runtime IFC import and the operations database currently rely on Windows APIs. Other Unity targets are not supported for the complete workflow.

## Getting started

1. Clone the repository and open its root folder in Unity Hub.
2. Use Unity `6000.3.17f1` and allow the Package Manager to restore dependencies.
3. Open `Assets/Scenes/TestScene.unity`.
4. Enter Play Mode. The included default IFC models are loaded by the startup workflow.
5. Use the dashboard to add/remove models, filter and inspect elements, manage operational records, measure geometry, or export reports.

Basic camera controls:

- Left mouse drag: pan
- Right mouse drag: orbit
- Mouse wheel: zoom
- One-finger drag: pan
- Two-finger drag/pinch: orbit and zoom

## Building

Select **Windows, Mac, Linux > Windows x86_64** in Unity's Build Profiles and build normally. The build processor will:

1. Verify that `Assets/IFC/Default` contains at least one IFC file.
2. Publish `Tools/XbimIfcConverter` as a self-contained Windows x64 executable.
3. Copy the converter beside the Unity player data.
4. Package the default IFC models under `StreamingAssets/IFC/Default`.

The build fails early with a clear error if the .NET SDK or default IFC files are missing.

## Project layout

```text
Assets/
  IFC/Default/                 IFC models packaged with Windows builds
  Scenes/TestScene.unity       Main application scene
  Scripts/IfcRuntimeLoader/    IFC import, GIS, metadata, operations, reports
  Scripts/Camera/              Orbit camera and visibility helpers
  Scripts/BackEnd/             Legacy/sample housing API integration
  Tests/EditMode/              Coordinate, classification, and state tests
  UI/IfcOperations/            UI Toolkit dashboard markup and styling
  Plugins/PixyzPluginForUnity/ Pixyz CAD/BIM import and optimization tools
Packages/
  com.esri.arcgis-maps-sdk/    Embedded ArcGIS Maps SDK package
Tools/
  XbimIfcConverter/            Standalone IFC-to-mesh conversion tool
```

Local operational data is written to Unity's persistent data directory under `IfcOperations/ifc_operations.db`; it is not stored in the repository.

## Testing

Open **Window > General > Test Runner**, select **EditMode**, and run all tests. The current suite covers OSM tile calculations, infrastructure classification, and operational state behavior.

## Notes for contributors

- Keep xBIM dependencies inside `Tools/XbimIfcConverter`; do not add them directly to the Unity assemblies.
- Treat Windows x64 as the reference runtime when changing IFC import, file dialogs, SQLite, or build packaging.
- Large IFC files are expensive to tessellate. Tune the deflection and simplification settings on `XbimIfcLoader` before increasing per-frame import budgets.
- Do not commit generated folders such as `Library`, `Temp`, `Logs`, converter `bin/obj`, or local database files.
