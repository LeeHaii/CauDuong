// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_PROCESS_INTERFACE_PROCESSINTERFACE_C_H_
#define _PXZ_PROCESS_INTERFACE_PROCESSINTERFACE_C_H_

#include "ProcessTypes_c.h"

PXZ_EXPORTED char * Process_getLastError();

// Automatically decimates a selection of meshes, using as a target a triangle count or a ratio (reduction percentage), and bakes Normals information into a texture (plus other textures).
PXZ_EXPORTED void Process_decimateTargetBake(Scene_OccurrenceList occurrences, Algo_DecimateOptionsSelector decimationTargetType, Process_BakeOptions bakingOptions, Core_Boolean overrideExistingUV);
// Automatically generates 3 LODs for the current selection.
PXZ_EXPORTED void Process_generateLODChain(Scene_OccurrenceList occurrences, Process_DecimateParametersList decimateParametersList);
// Automatically generates one unique optimized mesh out of the models in the scene, with material(s).
PXZ_EXPORTED void Process_generatePhantomMesh(Scene_OccurrenceList occurrences, Process_GenerateDiffuseMap generateDiffuseMap);
// The Guided import automatically converts and readies your 3D model(s) with guided parameters.
PXZ_EXPORTED Scene_OccurrenceList Process_guidedImport(IO_ImportFilePathList fileNames, Process_CoordinateSystemOptions coordinateSystem, Process_TessellationSettings tessellation, Process_ImportOptions otherOptions, Core_Boolean importLines, Core_Boolean importPoints, Core_Boolean importHidden, Core_Boolean importPMI, Core_Boolean importVariants, Core_Boolean useAlternativeImporter);
// Automatically generates a Proxy Mesh out of a selection of meshes, with optional automatic textures generation.
PXZ_EXPORTED Scene_Occurrence Process_proxyFromMeshes(Scene_OccurrenceList occurrences, Core_Int gridResolution, Process_BakeOptionSelector generateTextures, Core_Bool transferAnimations, Core_Bool keepOriginalMesh);
// Automatically generates a Proxy Mesh out of a selection of Point Cloud, with optional automatic diffuse texture generation.
PXZ_EXPORTED Scene_Occurrence Process_proxyFromPointCloud(Scene_OccurrenceList occurrences, Core_Int gridResolution, Process_GenerateDiffuseMap generateDiffuseTexture, Core_Bool keepOriginalPointCloud);
// Automatically creates optimized meshes out of any 3D model (mesh or CAD).
PXZ_EXPORTED void Process_runGenericProcess(Core_Boolean overrideExistingUVs, Core_Boolean repackUVs);



#endif
