// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_ALGO_INTERFACE_ALGOINTERFACE_C_H_
#define _PXZ_ALGO_INTERFACE_ALGOINTERFACE_C_H_

#include "AlgoTypes_c.h"

PXZ_EXPORTED char * Algo_getLastError();

// Assemble faces of CAD shapes
PXZ_EXPORTED void Algo_assembleCAD(Scene_OccurrenceList occurrences, Geom_Distance tolerance, Core_Boolean removeDuplicatedFaces);
// Create the BRep shape from a Tessellated shape with Domain Patch Attributes (after tessellate)
PXZ_EXPORTED void Algo_backToInitialBRep(Scene_OccurrenceList occurrences);
// bakes impostors textures
PXZ_EXPORTED Algo_OctahedralImpostor Algo_bakeImpostor(Scene_Occurrence occurrence, Core_Int XFrames, Core_Int YFrames, Core_Bool hemi, Core_Int resolution, Core_Int padding, Core_Boolean roughness, Core_Boolean metallic, Core_Boolean ao);
// Bake vertex attributes on meshes from other meshes
PXZ_EXPORTED void Algo_bakeVertexAttributes(Scene_OccurrenceList destinationOccurrences, Scene_OccurrenceList sourceOccurrences, Core_Boolean skinnedMesh, Core_Boolean positions, Core_Boolean useCurrentPositionAsTPose);
// calculate the normal of each point of a Point Cloud
PXZ_EXPORTED void Algo_calculateNormalsInPointClouds(Scene_OccurrenceList occurrences);
// Explode and (re)merge a set of mesh parts by visible materials
PXZ_EXPORTED Scene_OccurrenceList Algo_combineMeshesByMaterials(Scene_OccurrenceList occurrences, Core_Boolean mergeNoMaterials, Scene_MergeHiddenPartsMode mergeHiddenPartsMode);
// Create instances when there are similar parts.
PXZ_EXPORTED void Algo_convertSimilarOccurencesToInstances(Scene_OccurrenceList occurrences, Core_Boolean checkMeshTopo, Core_Boolean checkVertexPositions, Core_Int vertexPositionPrecision, Core_Boolean checkUVTopo, Core_Boolean checkUVVertexPositions, Core_Int UVPositionprecision, Core_Boolean keepExistingPrototypes);
// Explode each mesh to approximated convex decomposition
PXZ_EXPORTED void Algo_convexDecomposition(Scene_OccurrenceList occurrences, Core_Int maxCount, Core_Int vertexCount, Core_Boolean approximate, Core_Int resolution, Core_Double concavity);
// crack polygonal edges according to given criteria
PXZ_EXPORTED void Algo_crackEdges(Scene_OccurrenceList occurrences, Core_Boolean useAttributesFilter, Core_Boolean useSharpEdgeFilter, Geom_Angle sharpAngleFilter, Core_Boolean useNonManifoldFilter);
// Create free edges from patch borders
PXZ_EXPORTED void Algo_createFreeEdgesFromPatches(Scene_OccurrenceList occurrences);
// Create identified patch from existing patch (this is usefull before cloning for baking)
PXZ_EXPORTED void Algo_createIdentifiedPatchesFromPatches(Scene_OccurrenceList occurrences);
// Create instances when there are similar parts. This can be used to repair instances or to simplify a model that has similar parts that could be instanciated instead to reduce the number of unique meshes (reduces drawcalls, GPU memory usage and file size). Using 1.0 (100%) in all similarity criterias is non destructive. Using lower values will help finding more similar parts, even if their polycount or dimensions varies a bit.
PXZ_EXPORTED void Algo_createInstancesBySimilarity(Scene_OccurrenceList occurrences, Core_Coeff dimensionsSimilarity, Core_Coeff polycountSimilarity, Core_Boolean ignoreSymmetry, Core_Boolean keepExistingPrototypes, Core_Boolean createNewOccurrencesForPrototypes);
// Create normal attributes on tessellations
PXZ_EXPORTED void Algo_createNormals(Scene_OccurrenceList occurrences, Geom_Angle sharpEdge, Core_Boolean override, Core_Boolean useAreaWeighting);
// Create tangent attributes on tessellations
PXZ_EXPORTED void Algo_createTangents(Scene_OccurrenceList occurrences, Geom_Angle sharpEdge, Core_Int uvChannel, Core_Boolean override);
// Create visibility patches from existing patches
PXZ_EXPORTED void Algo_createVisibilityPatchesFromPatch(Scene_OccurrenceList occurrences);
// reduce the polygon count by removing some vertices
PXZ_EXPORTED void Algo_decimate(Scene_OccurrenceList occurrences, Geom_Distance surfacicTolerance, Geom_Distance lineicTolerance, Geom_Angle normalTolerance, Core_Double texCoordTolerance, Core_Boolean releaseConstraintOnSmallArea);
// reduce the polygon count by collapsing some edges to obtain an simplified mesh
PXZ_EXPORTED void Algo_decimateEdgeCollapse(Scene_OccurrenceList occurrences, Geom_Distance surfacicTolerance, Core_Double boundaryWeight, Core_Double normalWeight, Core_Double UVWeight, Core_Double sharpNormalWeight, Core_Double UVSeamWeight, Geom_Angle normalMaxDeviation, Core_Boolean forbidUVOverlaps, Core_Double UVMaxDeviation, Core_Double UVSeamMaxDeviation, Core_Boolean protectTopology, Algo_QualitySpeedTradeoff qualityTradeoff);
// decimate Point Cloud Occurences according to tolerance 
PXZ_EXPORTED void Algo_decimatePointClouds(Scene_OccurrenceList occurrences, Geom_Distance tolerance);
// reduce the polygon count by collapsing some edges to obtain a target triangle count (iterative version that use less memory)
PXZ_EXPORTED void Algo_decimateTarget(Scene_OccurrenceList occurrences, Algo_DecimateOptionsSelector targetStrategy, Algo_UVImportanceEnum UVImportance, Core_Boolean protectTopology, Core_Int iterativeThreshold);
// Delete designed attribute on tessellations
PXZ_EXPORTED void Algo_deleteAttibute(Scene_Occurrence occurrence, Algo_AttributType type);
// Delete BRep representation on parts
PXZ_EXPORTED void Algo_deleteBRepShapes(Scene_OccurrenceList occurrences, Core_Boolean onlyTessellated);
// Delete all free vertices of the mesh of given parts
PXZ_EXPORTED void Algo_deleteFreeVertices(Scene_OccurrenceList occurrences);
// Delete all free line of the mesh of given parts
PXZ_EXPORTED void Algo_deleteLines(Scene_OccurrenceList occurrences);
// Remove normal attributes on tessellations
PXZ_EXPORTED void Algo_deleteNormals(Scene_OccurrenceList occurrences);
// Delete patches attributes on tessellations
PXZ_EXPORTED void Algo_deletePatches(Scene_OccurrenceList occurrences, Core_Boolean keepOnePatchByMaterial);
// Delete all polygons of the mesh of given parts
PXZ_EXPORTED void Algo_deletePolygons(Scene_OccurrenceList occurrences);
// Remove tangent attributes on tessellations
PXZ_EXPORTED void Algo_deleteTangents(Scene_OccurrenceList occurrences);
// Delete texture coordinates on tessellations
PXZ_EXPORTED void Algo_deleteTextureCoordinates(Scene_OccurrenceList occurrences, Core_Int channel);
// delete the visibility patches of given occurrences
PXZ_EXPORTED void Algo_deleteVisibilityPatches(Scene_OccurrenceList occurrences);
// returns the max error to set to reach a given target
PXZ_EXPORTED Core_Double Algo_evalDecimateErrorForTarget(Scene_OccurrenceList occurrences, Algo_DecimateOptionsSelector TargetStrategy, Core_Double boundaryWeight, Core_Double normalWeight, Core_Double UVWeight, Core_Double sharpNormalWeight, Core_Double UVSeamWeight, Core_Boolean forbidUVFoldovers, Core_Boolean protectTopology);
// Explode all CAD Parts by body
PXZ_EXPORTED void Algo_explodeBodies(Scene_OccurrenceList occurrences, Core_Boolean groupOpenShells);
// Explode connected set of polygons to parts
PXZ_EXPORTED void Algo_explodeConnectedMeshes(Scene_OccurrenceList occurrences);
// Explode all parts by material
PXZ_EXPORTED void Algo_explodePartByMaterials(Scene_OccurrenceList occurrences);
// Explode all parts by patch
PXZ_EXPORTED void Algo_explodePatches(Scene_OccurrenceList occurrences);
// Explode parts to respect a maximum vertex count
PXZ_EXPORTED void Algo_explodeVertexCount(Scene_OccurrenceList occurrences, Core_Int maxVertexCount, Core_Int maxTriangleCount, Core_Boolean countMergedVerticesOnce);
// Explode parts by voxel
PXZ_EXPORTED void Algo_explodeVoxel(Scene_OccurrenceList occurrences, Geom_Distance voxelSize);
// Extract neutral axis from tessellations
PXZ_EXPORTED void Algo_extractNeutralAxis(Scene_OccurrenceList occurrences, Geom_Distance maxDiameter, Core_Boolean removeOriginalMesh);
// returns all the tessellation of the given occurrences (only returns editable mesh, see algo.toEditableMesh)
PXZ_EXPORTED Polygonal_TessellationList Algo_getTessellations(Scene_OccurrenceList occurrences);
// returns the visibility statistics for some occurrences
typedef struct {
   Core_Int visibleCountFront;
   Core_Int visibleCountBack;
} Algo_getVisibilityStatsReturn;
PXZ_EXPORTED Algo_getVisibilityStatsReturn Algo_getVisibilityStats(Scene_OccurrenceList occurrences);
// Create cad patches on tessellation (needed by some functions)
PXZ_EXPORTED void Algo_identifyPatches(Scene_OccurrenceList occurrences, Core_Boolean useAttributesFilter, Core_Boolean useSharpEdgeFilter, Geom_Angle sharpAngle, Core_Boolean useBoundaryFilter, Core_Boolean useNonManifoldFilter, Core_Boolean useLineEdgeFilter, Core_Boolean useQuadLineFilter);
// Generate a textured quadrangle over an existing mesh of coplanar lines
PXZ_EXPORTED void Algo_lineToTexture(Scene_OccurrenceList lines, Algo_UseColorOption useColor, Core_Int resolution, Core_Int thickness);
// List features from tessellations
PXZ_EXPORTED Algo_OccurrenceFeaturesList Algo_listFeatures(Scene_OccurrenceList occurrences, Core_Boolean throughHoles, Core_Boolean blindHoles, Geom_Distance maxDiameter);
// Replace the tessellations of the selected parts by a marching cube representation
PXZ_EXPORTED Scene_Occurrence Algo_marchingCubes(Scene_OccurrenceList occurrences, Geom_Distance voxelSize, Algo_ElementFilter elements, Core_Int dilation, Core_Boolean surfacic);
// merge near vertices according to the given distance
PXZ_EXPORTED void Algo_mergeVertices(Scene_OccurrenceList occurrences, Geom_Distance maxDistance, Polygonal_TopologyCategoryMask mask);
// Apply noise to vertex positions along their normals
PXZ_EXPORTED void Algo_noiseMesh(Scene_OccurrenceList occurrences, Geom_Distance maxAmplitude);
// Optimize CAD Face loops by merging useless loop edges
PXZ_EXPORTED void Algo_optimizeCADLoops(Scene_OccurrenceList occurrences);
// Optimize mesh for rendering (lossless, only reindexing)
PXZ_EXPORTED void Algo_optimizeForRendering(Scene_OccurrenceList occurrences);
// Sort sub meshes by materials
PXZ_EXPORTED void Algo_optimizeSubMeshes(Scene_OccurrenceList occurrences);
// Resizes scene textures based on a number of texels per 3D space units (e.g: mm)
PXZ_EXPORTED void Algo_optimizeTextureSize(Scene_Occurrence root, Core_Double texelPerMm);
// Replace the tessellations of the selected parts by a proxy mesh based on a voxelization
PXZ_EXPORTED Scene_Occurrence Algo_proxyMesh(Scene_OccurrenceList occurrences, Geom_Distance voxelSize, Algo_ElementFilter elements, Core_Int dilation, Core_Boolean surfacic);
// Remove some features from tessellations
PXZ_EXPORTED void Algo_removeHoles(Scene_OccurrenceList occurrences, Core_Boolean throughHoles, Core_Boolean blindHoles, Core_Boolean surfacicHoles, Geom_Distance maxDiameter, Material_Material fillWithMaterial);
// Repair CAD shapes, assemble faces, remove duplicated faces, optimize loops and repair topology
PXZ_EXPORTED void Algo_repairCAD(Scene_OccurrenceList occurrences, Geom_Distance tolerance, Core_Boolean orient);
// Replace geometries by other shapes, or primitives
PXZ_EXPORTED void Algo_replaceBy(Scene_OccurrenceList occurrences, Algo_ReplaceByOption replaceBy);
// Replace objects by a bounding box
PXZ_EXPORTED void Algo_replaceByBox(Scene_OccurrenceList occurrences, Algo_ReplaceByBoxType boxType);
// Replace objects by convex hull
PXZ_EXPORTED void Algo_replaceByConvexHull(Scene_OccurrenceList occurrences);
// Replace objects by a primitive shapes
PXZ_EXPORTED void Algo_replaceByPrimitive(Scene_OccurrenceList occurrences, Algo_PrimitiveShapeParameters primitive, Core_Boolean generateUV);
// Update the tessellated representation of each CAD part with new tessellation parameters
PXZ_EXPORTED void Algo_retessellate(Scene_OccurrenceList occurrences, Geom_Distance maxSag, Geom_Distance maxLength, Geom_Angle maxAngle, Core_Boolean createNormals, Algo_UVGenerationMode uvMode, Core_Int uvChannel, Core_Double uvPadding, Core_Boolean createTangents, Core_Boolean createFreeEdges);
// Replace the tessellations of the selected parts by a retopology of the external hull
PXZ_EXPORTED Scene_Occurrence Algo_retopologize(Scene_OccurrenceList occurrences, Core_Int targetTriangleCount, Core_Bool pureQuad, Core_Bool pointCloud, Geom_Distance precision);
// Selects occurrences in the whole scene that are similar to the selected occurrences. If several occurrences are selected, the selection afterwards will contain similar parts for each input occurrence.
PXZ_EXPORTED void Algo_selectSimilar(Scene_OccurrenceList occurrences, Core_Coeff dimensionsSimilarity, Core_Coeff polycountSimilarity, Core_Boolean ignoreSymmetry);
PXZ_EXPORTED void Algo_smoothMesh(Scene_OccurrenceList occurrences, Algo_CostEvaluation mode, Core_Int maxIterations, Core_Boolean lockSignificantEdges);
// Extrudes a circular section along an underlying polyline (curve)
PXZ_EXPORTED void Algo_sweep(Scene_OccurrenceList occurrences, Geom_Distance radius, Core_Int sides, Core_Boolean createNormals, Core_Boolean keepLines, Core_Boolean generateUV);
// Create a tessellated representation from a CAD representation for each given part
PXZ_EXPORTED void Algo_tessellate(Scene_OccurrenceList occurrences, Geom_Distance maxSag, Geom_Distance maxLength, Geom_Angle maxAngle, Core_Boolean createNormals, Algo_UVGenerationMode uvMode, Core_Int uvChannel, Core_Double uvPadding, Core_Boolean createTangents, Core_Boolean createFreeEdges, Core_Boolean keepBRepShape, Core_Boolean overrideExistingTessellation);
// Creates a tessellated representation from a CAD representation for each given part. It multiplies the length of the diagonal of the bounding box by the sagRatio. If the output value is above maxSag, then maxSag is used as tessellation value. Else if the output value is below maxSag, it is used as tessellation value.
PXZ_EXPORTED void Algo_tessellateRelativelyToAABB(Scene_OccurrenceList occurrences, Geom_Distance maxSag, Core_Double sagRatio, Geom_Distance maxLength, Geom_Angle maxAngle, Core_Boolean createNormals, Algo_UVGenerationMode uvMode, Core_Int uvChannel, Core_Double uvPadding, Core_Boolean createTangents, Core_Boolean createFreeEdges, Core_Boolean keepBRepShape, Core_Boolean overrideExistingTessellation);
// Replace the tessellations of the selected parts by a voxelization of the external skin
PXZ_EXPORTED Scene_Occurrence Algo_voxelize(Scene_OccurrenceList occurrences, Geom_Distance voxelSize, Algo_ElementFilter elements, Core_Int dilation, Core_Boolean useCurrentAnimationPosition);
// Explode point clouds to voxels
PXZ_EXPORTED void Algo_voxelizePointClouds(Scene_OccurrenceList occurrences, Geom_Distance voxelSize);

// ----------------------------------------------------
// Baking
// ----------------------------------------------------

// Bake UV from a mesh to another mesh
PXZ_EXPORTED void Algo_bakeUV(Scene_Occurrence source, Scene_Occurrence destination, Core_Int sourceChannel, Core_Int destinationChannel, Geom_Distance tolerance);
// Combine all given meshes to one mesh with one material (baked)
PXZ_EXPORTED Scene_Occurrence Algo_combineMeshes(Scene_OccurrenceList occurrences, Algo_BakeOption bakingOptions, Core_Boolean overrideExistingUVs);

// ----------------------------------------------------
// Hidden Detection
// All hidden part detection/removal related function
// ----------------------------------------------------

// Create visilibity information on parts viewed from a set of camera automatically placed on a sphere around the scene
PXZ_EXPORTED void Algo_createVisibilityInformation(Scene_OccurrenceList occurrences, Algo_SelectionLevel level, Core_Int resolution, Core_Int sphereCount, Core_Double fovX, Core_Boolean considerTransparentOpaque);
// Create visilibity information on parts viewed from a given set of camera
PXZ_EXPORTED void Algo_createVisibilityInformationFromViewPoints(Scene_OccurrenceList occurrences, Geom_Point3List cameraPositions, Geom_Point3List cameraDirections, Geom_Point3List cameraUps, Core_Int resolution, Core_Double fovX, Core_Boolean considerTransparentOpaque);
// Return parts occurrences not viewed from a sphere around the scene
PXZ_EXPORTED Scene_OccurrenceList Algo_getHiddenOccurrences(Scene_OccurrenceList occurrences, Core_Int resolution, Core_Int sphereCount, Core_Double fovX, Core_Boolean considerTransparentOpaque);
// Delete parts, patches or polygons not viewed from a sphere around the scene
PXZ_EXPORTED Core_BoolList Algo_hiddenRemoval(Scene_OccurrenceList occurrences, Algo_SelectionLevel level, Core_Int resolution, Core_Int sphereCount, Core_Double fovX, Core_Boolean considerTransparentOpaque, Core_Int adjacencyDepth);
// Delete parts, patches or polygons not viewed from spheres generated with a set of camera position
PXZ_EXPORTED Core_BoolList Algo_hiddenRemovalCamera(Scene_OccurrenceList occurrences, Algo_SelectionLevel level, Geom_Point3List cameraPositions, Core_Int resolution, Core_Int sphereCount, Core_Double fovX, Core_Boolean considerTransparentOpaque, Core_Int adjacencyDepth);
// Delete parts, patches or polygons not viewed from a set of camera position/orientation
PXZ_EXPORTED Core_BoolList Algo_hiddenRemovalViewPoints(Scene_OccurrenceList occurrences, Algo_SelectionLevel level, Geom_Point3List cameraPositions, Geom_Point3List cameraDirections, Geom_Point3List cameraUps, Core_Int resolution, Core_Double fovX, Core_Boolean considerTransparentOpaque, Core_Int adjacencyDepth);
// Select parts not viewed from a sphere around the scene
PXZ_EXPORTED void Algo_hiddenSelection(Scene_OccurrenceList occurrences, Core_Int resolution, Core_Int sphereCount, Core_Double fovX, Core_Boolean considerTransparentOpaque);
// Create visilibity information on parts viewed from a set of camera automatically generated
PXZ_EXPORTED void Algo_smartHiddenCreateVisibilityInformation(Scene_OccurrenceList occurrences, Geom_Distance voxelSize, Geom_Volume minimumCavityVolume, Core_Int resolution, Algo_SmartHiddenType mode, Core_Boolean considerTransparentOpaque);
// Delete parts, patches or polygons not viewed from a set of camera automatically generated
PXZ_EXPORTED Core_BoolList Algo_smartHiddenRemoval(Scene_OccurrenceList occurrences, Algo_SelectionLevel level, Geom_Distance voxelSize, Geom_Volume minimumCavityVolume, Core_Int resolution, Algo_SmartHiddenType mode, Core_Boolean considerTransparentOpaque, Core_Int adjacencyDepth);
// Select parts not viewed from a set of camera automatically generated
PXZ_EXPORTED void Algo_smartHiddenSelection(Scene_OccurrenceList occurrences, Geom_Distance voxelSize, Geom_Volume minimumCavityVolume, Core_Int resolution, Algo_SmartHiddenType mode, Core_Boolean considerTransparentOpaque);

// ----------------------------------------------------
// Sawing
// ----------------------------------------------------

// Saw the mesh with an axis-aligned bounding box
PXZ_EXPORTED void Algo_sawWithAABB(Scene_OccurrenceList occurrences, Geom_AABB aabb, Algo_SawingMode mode, Core_String innerSuffix, Core_String outerSuffix);
// Saw the mesh with an oriented bounding box
PXZ_EXPORTED void Algo_sawWithOBB(Scene_OccurrenceList occurrences, Geom_OBB obb, Algo_SawingMode mode, Core_String innerSuffix, Core_String outerSuffix);
// Saw the mesh with a plane
PXZ_EXPORTED void Algo_sawWithPlane(Scene_OccurrenceList occurrences, Geom_Point3 planeOrigin, Geom_Vector3 planeNormal, Algo_SawingMode mode, Core_String innerSuffix, Core_String outerSuffix);

// ----------------------------------------------------
// UV Mapping
// ----------------------------------------------------

// Apply a transformation matrix on texture coordinates
PXZ_EXPORTED void Algo_applyUvTransform(Scene_OccurrenceList occurrences, Geom_Matrix4 matrix, Core_Int channel);
// Generates the texture coordinates and automatically cut
PXZ_EXPORTED void Algo_automaticUVMapping(Scene_OccurrenceList occurrences, Core_Int channel, Core_Double maxAngleDistorsion, Core_Double maxAreaDistorsion, Core_Bool sharpToSeam, Core_Bool forbidOverlapping);
// Copy an UV channel to another UV channel
PXZ_EXPORTED void Algo_copyUV(Scene_OccurrenceList occurrences, Core_Int sourceChannel, Core_Int destinationChannel);
// Generate texture coordinates using the projection on object Axis Aligned Bounding Box
PXZ_EXPORTED void Algo_mapUvOnAABB(Scene_OccurrenceList occurrences, Core_Bool useLocalAABB, Geom_Distance uv3dSize, Core_Int channel, Core_Boolean overrideExistingUVs);
// Generate texture coordinates using the projection on a box
PXZ_EXPORTED void Algo_mapUvOnBox(Scene_OccurrenceList occurrences, Algo_Box box, Core_Int channel, Core_Boolean overrideExistingUVs);
// Generate texture coordinates using the projection on object AABB, with same scale on each axis
PXZ_EXPORTED void Algo_mapUvOnCubicAABB(Scene_OccurrenceList occurrences, Geom_Distance uv3dSize, Core_Int channel, Core_Boolean overrideExistingUVs);
// Generate texture coordinates using the projection on custom AABB
PXZ_EXPORTED void Algo_mapUvOnCustomAABB(Scene_OccurrenceList occurrences, Geom_AABB aabb, Geom_Distance uv3dSize, Core_Int channel, Core_Boolean overrideExistingUVs);
// Generate texture coordinates using the projection on a cylinder
PXZ_EXPORTED void Algo_mapUvOnCylinder(Scene_OccurrenceList occurrences, Algo_Cylinder cylinder, Core_Int channel, Core_Boolean overrideExistingUVs);
// Generate texture coordinates using the projection on a fitting cylinder
PXZ_EXPORTED void Algo_mapUvOnFittingCylinder(Scene_OccurrenceList occurrences, Core_Int channel, Core_Boolean overrideExistingUVs, Core_Boolean useAABB);
// Generate texture coordinates using the projection on a fitting sphere
PXZ_EXPORTED void Algo_mapUvOnFittingSphere(Scene_OccurrenceList occurrences, Core_Int channel, Core_Boolean overrideExistingUVs, Core_Boolean useAABB);
// Generate texture coordinates using the projection on object Minimum Bounding Box
PXZ_EXPORTED void Algo_mapUvOnMBB(Scene_OccurrenceList occurrences, Core_Bool useLocalMBB, Geom_Distance uv3dSize, Core_Int channel, Core_Boolean overrideExistingUVs);
// Generate texture coordinates using the projection on a plane
PXZ_EXPORTED void Algo_mapUvOnPlane(Scene_OccurrenceList occurrences, Algo_Plane plane, Core_Int channel, Core_Boolean overrideExistingUVs);
// Generate texture coordinates using the projection on a sphere
PXZ_EXPORTED void Algo_mapUvOnSphere(Scene_OccurrenceList occurrences, Algo_Sphere sphere, Core_Int channel, Core_Boolean overrideExistingUVs);
// Normalize UVs to fit in the [0-1] uv space
PXZ_EXPORTED void Algo_normalizeUV(Scene_OccurrenceList occurrences, Core_Int sourceUVChannel, Core_Int destinationUVChannel, Core_Boolean uniform, Core_Boolean sharedUVSpace, Core_Boolean ignoreNullIslands);
// Relax UVs
PXZ_EXPORTED void Algo_relaxUV(Scene_OccurrenceList occurrences, Algo_RelaxUVMethod method, Core_Int iterations, Core_Int channel);
// Remove one or all UV channel(s)
PXZ_EXPORTED void Algo_removeUV(Scene_OccurrenceList occurrences, Core_Int channel);
// Pack existing UV (create atlas)
PXZ_EXPORTED Scene_OccurrenceList Algo_repackUV(Scene_OccurrenceList occurrences, Core_Int channel, Core_Boolean shareMap, Core_Int resolution, Core_Int padding, Core_Boolean uniformRatio, Core_Int iterations, Core_Boolean removeOverlaps);
// Apply a scale on texture coordinates
PXZ_EXPORTED void Algo_scaleUV(Scene_OccurrenceList occurrences, Core_Double scaleU, Core_Double scaleV, Core_Int channel);
// Create UV patches with disk-like topology
PXZ_EXPORTED void Algo_segmentDiskFront(Scene_OccurrenceList occurrences, Core_Double threshold, Core_Int channel);
// Smooth texture coordinates
PXZ_EXPORTED void Algo_smoothUV(Scene_OccurrenceList occurrences, Core_Int iterations, Core_Int channel);
// Swap two UV channels
PXZ_EXPORTED void Algo_swapUvChannels(Scene_OccurrenceList occurrences, Core_Int firstChannel, Core_Int secondChannel);

// ----------------------------------------------------
// fitting
// ----------------------------------------------------

// Returns the fitting cylinder of a set of occurrences (based on MBB)
PXZ_EXPORTED Geom_Affine Algo_getFittingCylinder(Scene_OccurrenceList occurrences, Core_Boolean useAABB);
// Returns the fitting sphere of a set of occurrences
PXZ_EXPORTED Geom_Affine Algo_getFittingSphere(Scene_OccurrenceList occurrences, Core_Boolean useAABB);

// ----------------------------------------------------
// map_generation
// Map Generation functions
// ----------------------------------------------------

// Bake texture maps on meshes from self or other meshes
PXZ_EXPORTED Material_ImageList Algo_bakeMaps(Scene_OccurrenceList destinationOccurrences, Scene_OccurrenceList sourceOccurrences, Algo_BakeMapList mapsToBake, Core_Int channel, Core_Int resolution, Core_Int padding, Core_Boolean shareMaps, Core_String mapSuffix, Algo_CustomBakeMapList additionalCustomMaps, Geom_Distance tolerance, Algo_BakingMethod method, Core_Coeff opacityThreshold, Core_Boolean useCurrentPosition, Geom_Distance offset, Algo_getPixelValueList callbackList);
// Convert an existing normal map between Object-space and Tangent-space
PXZ_EXPORTED Material_Image Algo_convertNormalMap(Scene_OccurrenceList occurrences, Material_Image normalMap, Core_Int uvChannel, Core_Boolean sourceIsObjectSpace, Core_Boolean destinationIsObjectSpace, Core_Boolean sourceIsRightHanded, Core_Boolean destinationIsRightHanded, Core_Boolean replaceMap, Core_Int resolution, Core_Int padding);
// Create a billboard imposter
PXZ_EXPORTED Scene_Occurrence Algo_createBillboard(Scene_OccurrenceList occurrences, Core_Int resolution, Core_Bool XPositive, Core_Bool XNegative, Core_Bool YPositive, Core_Bool YNegative, Core_Bool ZPositive, Core_Bool ZNegative, Core_Bool moveFacesToCenter, Core_Bool leftHandedNormalMap);
// Orient a tangent space normal map (all Z positive)
PXZ_EXPORTED void Algo_orientNormalMap(Material_Image normalMap);

// ----------------------------------------------------
// repair
// Polygonal repair functions
// ----------------------------------------------------

// Smooth the tessellations by moving the vertices to the barycenter of their neighbors
PXZ_EXPORTED void Algo_barySmooth(Scene_OccurrenceList occurrences, Core_Int iteration);
// Remove moebius strip by topologically cracking them (make it orientable)
PXZ_EXPORTED void Algo_crackMoebiusStrips(Scene_OccurrenceList occurrences, Core_Int maxEdgeCount);
// Splits non-manifold vertices
PXZ_EXPORTED void Algo_crackNonManifoldVertices(Scene_OccurrenceList occurrences);
// Identify cavities and create occurrences to show them
PXZ_EXPORTED Scene_Occurrence Algo_createCavityOccurrences(Scene_OccurrenceList occurrences, Geom_Distance voxelSize, Geom_Volume minimumCavityVolume, Algo_SmartHiddenType mode, Scene_Occurrence parent);
// Invert the orientation of tessellation elements
PXZ_EXPORTED void Algo_invertOrientation(Scene_OccurrenceList occurrences);
// Splits moebius ring
PXZ_EXPORTED void Algo_moebiusCracker(Scene_OccurrenceList occurrences);
// Orient tessellation elements
PXZ_EXPORTED void Algo_orient(Scene_OccurrenceList occurrences, Core_Boolean makeOrientable, Core_Boolean useArea, Algo_OrientStrategy orientStrategy);
// Properly orient all polygons in the same direction, using a specified viewpoint
PXZ_EXPORTED void Algo_orientFromCamera(Scene_OccurrenceList occurrences, Geom_Point3 cameraPosition, Geom_Point3 cameraDirection, Geom_Point3 cameraUp, Core_Int resolution, Core_Double fovX);
// Orient all connect polygones in the same orientation of the polygon selectionned
PXZ_EXPORTED void Algo_orientFromFace();
// Orient existing normal according to the polygons clockwise
PXZ_EXPORTED void Algo_orientNormals(Scene_OccurrenceList occurrences);
// Resmesh surfacic holes of tessellations
PXZ_EXPORTED void Algo_remeshSurfacicHoles(Scene_OccurrenceList occurrences, Geom_Distance maxDiameter);
// Remove some kinds of degenerated polygons
PXZ_EXPORTED void Algo_removeDegeneratedPolygons(Scene_OccurrenceList occurrences, Geom_Distance tolerance);
// Remove multiple polygon
PXZ_EXPORTED void Algo_removeMultiplePolygon(Scene_OccurrenceList occurrences);
// Remove Z-fighting (surfaces overlaping) by slightly shrinking the selected parts' surfaces
PXZ_EXPORTED Geom_Distance Algo_removeZFighting(Scene_OccurrenceList occurrences);
// Launch the repair process to repair a disconnected or not clean tessellation
PXZ_EXPORTED void Algo_repairMesh(Scene_OccurrenceList occurrences, Geom_Distance tolerance, Core_Bool crackNonManifold, Core_Boolean orient);
// Create normal on an existing normal set when normal is null (polygons appears black)
PXZ_EXPORTED void Algo_repairNullNormals(Scene_OccurrenceList occurrences);
// Remove non manifold edges and try to reconnect manifold groups of triangles
PXZ_EXPORTED void Algo_separateToManifold(Scene_OccurrenceList occurrences);
// Sew boundaries between them
PXZ_EXPORTED void Algo_sewBoundary(Scene_OccurrenceList occurrences, Geom_Distance maxDistance);
// Properly orient all polygons in the same direction, using visibility attributes
PXZ_EXPORTED void Algo_smartOrient(Scene_OccurrenceList occurrences, Geom_Distance voxelSize, Geom_Volume minimumCavityVolume, Core_Int resolution, Algo_SmartHiddenType mode, Core_Boolean considerTransparentOpaque, Algo_SmartOrientStrategy orientStrategy);
// Move the vertices by the offsset along their normal
PXZ_EXPORTED void Algo_vertexOffset(Scene_OccurrenceList occurrences, Geom_Distance offset);

// ----------------------------------------------------
// tessellation conversion
// Polygonal conversion related functions
// ----------------------------------------------------

// Sswap edges to make triangles more equilateral
PXZ_EXPORTED void Algo_equilateralize(Scene_OccurrenceList occurrences, Core_Int maxIterations);
// Merge all triangle polygons in the meshes to quadrangles
PXZ_EXPORTED void Algo_quadify(Scene_OccurrenceList occurrences);
// Advanced function to requadify a triangle tessellation coming from full quad mesh
PXZ_EXPORTED void Algo_requadify(Scene_OccurrenceList occurrences, Core_Bool forceFullQuad);
// Convert all static mesh to editable mesh
PXZ_EXPORTED void Algo_toEditableMesh(Scene_OccurrenceList occurrences);
// Convert all editable mesh to static mesh
PXZ_EXPORTED void Algo_toStaticMesh(Scene_OccurrenceList occurrences);
// Split all non-triangle polygons in the meshes to triangles
PXZ_EXPORTED void Algo_triangularize(Scene_OccurrenceList occurrences);

// ----------------------------------------------------
// vertex weights
// ----------------------------------------------------

// Use vertex colors attributes on meshes of the given occurrence to create vertex weights attributes used by the decimation functions, the finals weights will be computed with w = offset + (red - blue) * scale
PXZ_EXPORTED void Algo_createVertexWeightsFromVertexColors(Scene_OccurrenceList occurrences, Core_Double offset, Core_Double scale);
// Use visibility attributes on meshes of the given occurrence to create vertex weights attributes used by the decimation functions. The finals weights will be computed with w = offset + (visibility/maxVisibility) * scale
PXZ_EXPORTED void Algo_createVertexWeightsFromVisibilityAttributes(Scene_OccurrenceList occurrences, Core_Double offset, Core_Double scale);

// ----------------------------------------------------
// visibility
// ----------------------------------------------------

// Create visibility attributes on tessellations
PXZ_EXPORTED void Algo_createVisibilityAttributes(Scene_OccurrenceList occurrences);
// Delete visibility attributes on tessellations
PXZ_EXPORTED void Algo_deleteVisibilityAttributes(Scene_OccurrenceList occurrences);
// Add one count to all visiblility attributes (poly and patch) on transparent patches
PXZ_EXPORTED void Algo_flagVisibilityAttributesOnTransparents(Scene_OccurrenceList occurrences);



#endif
