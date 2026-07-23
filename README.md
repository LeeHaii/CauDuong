# TPTwinTemplate

Welcome to the **TPTwinTemplate**, a Unity project template tailored for creating Digital Twin applications. This project is configured with the Universal Render Pipeline (URP) and includes essential systems for dynamic model loading, interaction, and visualization.

## Overview

This template provides a robust foundation for architectural visualization, digital twins, and BIM (Building Information Modeling) data integration. It is designed to work with Addressables for efficient asset loading, allowing for dynamic loading of apartments or environments.

## Features

- **Dynamic Environment Loading**: Uses Unity Addressables (`ApartmentLoader.cs`) to dynamically load environments (like apartments) at runtime based on user selection or configuration.
- **First Person Movement**: Includes a pre-configured first-person controller (`FirstPersonMovement.cs`) for navigating the digital twin environment.
- **Interactions**: Built-in system for object hovering and interaction (`HoverManager.cs`).
- **BIM Integration**: Structured to support BIM data visualization and handling.
- **Pixyz Integration**: Includes integration for advanced CAD, BIM, and 3D data import pipelines using Pixyz.
- **Player Spawning**: Setup for player spawning and lifecycle management (`PlayerSpawner.cs`).

## Included Packages & Dependencies

This template comes pre-packaged with several useful Unity packages for a Digital Twin workflow:
- **Unity Universal Render Pipeline (URP)** (v17.3.0) for high-quality graphics and performance.
- **Addressables** (v2.9.1) for asset management and dynamic loading.
- **GLTFast** (`com.unity.cloud.gltfast` v6.19.0) for fast glTF model loading at runtime.
- **Multiplayer Center** (`com.unity.multiplayer.center` v1.0.1) for cooperative or multiplayer digital twin experiences.
- **Input System** (v1.19.0) for modern cross-platform input handling.
- **AI Navigation** (v2.0.12) for automated navigation and pathfinding.
- **MCP for Unity** (`com.coplaydev.unity-mcp`) for model context protocol integration.
- **Pixyz Plugin** for powerful CAD and 3D data preparation and import.

## Getting Started

1. Open the project using **Unity 6000.3.17f1** or newer.
2. The project uses **URP**, ensure your target platform is compatible.
3. Check the `Assets/Scripts` directory to find the core logic:
   - `ApartmentLoader.cs`: Handles loading the addressable environment.
   - `FirstPersonMovement.cs`: Script managing player movement.
4. Modify the `PlayerPrefs` or default load arguments to specify which Addressable key to load via the `ApartmentLoader`.

## Project Structure

- `Assets/Scripts/Bim`: Contains logic for BIM data processing.
- `Assets/Scripts/Camera`: Camera control scripts.
- `Assets/Scripts/Scene`: Scene management utilities.
- `Assets/Scripts/BackEnd`: Scripts for handling backend connections or data fetching.
- `Assets/Prefabs`: Reusable project prefabs.
- `Assets/Settings`: Render pipeline and project-specific settings.

---
*Created as a template for Unity Digital Twin projects.*
