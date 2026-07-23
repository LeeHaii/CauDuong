// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_SCENE_INTERFACE_SCENEINTERFACE_C_H_
#define _PXZ_SCENE_INTERFACE_SCENEINTERFACE_C_H_

#include "SceneTypes_c.h"

PXZ_EXPORTED char * Scene_getLastError();

// Add a component to an occurrence
PXZ_EXPORTED Scene_Component Scene_addComponent(Scene_Occurrence occurrence, Scene_ComponentType componentType);
// Add a light component to an occurrence
PXZ_EXPORTED Scene_Component Scene_addLightComponent(Scene_Occurrence occurrence);
// Add a new metadata property to a metadata component
PXZ_EXPORTED void Scene_addMetadata(Scene_Metadata metadata, Core_String name, Core_String value);
// Add a new metadata property to a metadata component
PXZ_EXPORTED void Scene_addMetadataBlock(Scene_Metadata metadata, Core_StringList names, Core_StringList values);
// Remove unused images from texture library
PXZ_EXPORTED Core_Int Scene_cleanUnusedImages();
// Remove unused materials from material library
PXZ_EXPORTED Core_Int Scene_cleanUnusedMaterials(Core_Boolean cleanImages);
// Compute the checksum of a sub-tree
PXZ_EXPORTED Core_String Scene_computeSubTreeChecksum(Scene_Occurrence root);
// Create a complete scene tree
PXZ_EXPORTED Scene_OccurrenceList Scene_createCompleteTree(Scene_PackedTree tree, Scene_Occurrence root, Core_Bool replaceRoot);
// Create a new cone
PXZ_EXPORTED Scene_Occurrence Scene_createCone(Core_Double bottomRadius, Core_Double height, Core_Int sides, Core_Boolean generateUV);
// Create a new cube
PXZ_EXPORTED Scene_Occurrence Scene_createCube(Core_Double sizeX, Core_Double sizeY, Core_Double sizeZ, Core_Int subdivision, Core_Boolean generateUV);
// Create a new cylinder
PXZ_EXPORTED Scene_Occurrence Scene_createCylinder(Core_Double radius, Core_Double height, Core_Int sides, Core_Boolean generateUV);
// Create a new directional light
PXZ_EXPORTED Scene_DirectionalLight Scene_createDirectionalLight(Core_Color color, Core_Double power, Geom_Vector3 direction);
// Create a new bagel klein
PXZ_EXPORTED Scene_Occurrence Scene_createImmersion(Core_Double radius, Core_Int subdivisionX, Core_Int subdivisionY);
// Create Metadata components from definitions
PXZ_EXPORTED Scene_MetadataList Scene_createMetadatasFromDefinitions(Scene_OccurrenceList occurrences, Scene_MetadataDefinitionList definitions);
PXZ_EXPORTED Scene_Occurrence Scene_createOBBMesh(Scene_Occurrence occurrence);
// Create a new occurrence
PXZ_EXPORTED Scene_Occurrence Scene_createOccurrence(Core_String name, Scene_Occurrence parent);
// Create a new occurrence and add the given occurrences as children
PXZ_EXPORTED Scene_Occurrence Scene_createOccurrenceFromSelection(Core_String name, Scene_OccurrenceList children, Scene_Occurrence parent, Core_Boolean keepMaterialAssignment);
// Creates an occurrence from string
PXZ_EXPORTED Scene_Occurrence Scene_createOccurrenceFromText(Core_String text, Core_String font, Core_Int fontSize, Core_ColorAlpha color);
// Create one new occurrence under each given parent
PXZ_EXPORTED Scene_OccurrenceList Scene_createOccurrences(Core_String name, Scene_OccurrenceList parents);
// Create a set of Parts given meshes and occurrences
PXZ_EXPORTED Scene_PartList Scene_createPartsFromMeshes(Scene_OccurrenceList occurrences, Polygonal_MeshList meshes);
// Create a  new plane
PXZ_EXPORTED Scene_Occurrence Scene_createPlane(Core_Double sizeY, Core_Double sizeX, Core_Int subdivisionX, Core_Int subdivisionY, Core_Boolean generateUV);
// Create a new positional light
PXZ_EXPORTED Scene_PositionalLight Scene_createPositionalLight(Core_Color color, Core_Double power, Geom_Point3 position);
// Creates a ray prober
PXZ_EXPORTED Core_Ident Scene_createRayProber();
// Create a scene tree with a list of meshes, all meshes becomes part occurrences with the same root. The same mesh Id can be used several times to handle create instances (prototypes)
PXZ_EXPORTED Scene_Occurrence Scene_createSceneFromMeshes(Polygonal_MeshList meshes, Geom_Matrix4List matrices, Core_Boolean centerPartPivots);
// Create a new shpere
PXZ_EXPORTED Scene_Occurrence Scene_createSphere(Core_Double radius, Core_Int subdivisionLatitude, Core_Int subdivisionLongitude, Core_Boolean generateUV);
// Creates a sphere prober
PXZ_EXPORTED Core_Ident Scene_createSphereProber();
// Create a new spot light
PXZ_EXPORTED Scene_SpotLight Scene_createSpotLight(Core_Color color, Core_Double power, Geom_Point3 position, Geom_Vector3 direction, Geom_Angle cutoff);
// Create a new torus
PXZ_EXPORTED Scene_Occurrence Scene_createTorus(Core_Double majorRadius, Core_Double minorRadius, Core_Int subdivisionLatitude, Core_Int subdivisionLongitude);
// Delete component from type
PXZ_EXPORTED void Scene_deleteComponentByType(Scene_ComponentType componentType, Scene_Occurrence occurrence, Core_Bool followPrototypes);
// Delete all components on subtree from type
PXZ_EXPORTED void Scene_deleteComponentsByType(Scene_ComponentType componentType, Scene_Occurrence rootOccurrence);
// Delete all empty assemblies
PXZ_EXPORTED void Scene_deleteEmptyOccurrences(Scene_Occurrence root);
// Delete a liste of occurrences
PXZ_EXPORTED void Scene_deleteOccurrences(Scene_OccurrenceList occurrences);
// Find all part occurrence with a given material as active material (i.e. as seen in the rendering)
PXZ_EXPORTED Scene_OccurrenceList Scene_findByActiveMaterial(Material_Material material, Scene_OccurrenceList roots);
// Returns all occurrences which a metadata property value matches the given regular expression (ECMAScript)
PXZ_EXPORTED Scene_OccurrenceList Scene_findByMetadata(Core_String property, Core_Regex regex, Scene_OccurrenceList roots);
// Returns all occurrences which a property value matches the given regular expression (ECMAScript)
PXZ_EXPORTED Scene_OccurrenceList Scene_findByProperty(Core_String property, Core_Regex regex, Scene_OccurrenceList roots);
// find part occurences in the scene in a given axis aligned bounding box
PXZ_EXPORTED Scene_OccurrenceList Scene_findPartOccurrencesInAABB(Geom_AABB aabb);
PXZ_EXPORTED Scene_Occurrence Scene_generateOctaViews(Core_Double radius, Core_Int XFrames, Core_Int YFrames, Core_Bool hemi);
// Returns the axis aligned bounding box of a list of scene paths
PXZ_EXPORTED Geom_AABB Scene_getAABB(Scene_OccurrenceList occurrences);
// Get the active material on occurrence
PXZ_EXPORTED Material_Material Scene_getActiveMaterial(Scene_Occurrence occurrence);
// Get the value of a property on the first parent that own it
PXZ_EXPORTED Core_String Scene_getActivePropertyValue(Scene_Occurrence occurrence, Core_String propertyName, Core_Boolean cacheProperty);
// Get the value of a property on the first parent that own it for each given occurrence
PXZ_EXPORTED Core_StringList Scene_getActivePropertyValues(Scene_OccurrenceList occurrences, Core_String propertyName, Core_Boolean cacheProperty);
// Returns the list of the AnnotationGroup from a PMIComponent
PXZ_EXPORTED Scene_AnnotationGroupList Scene_getAnnotationGroups(Scene_Component pmiComponent);
// Returns the list of the Annotation from a AnnotationGroup
PXZ_EXPORTED Scene_AnnotationList Scene_getAnnotations(Scene_AnnotationGroup group);
// Get the children of an occurrence
PXZ_EXPORTED Scene_OccurrenceList Scene_getChildren(Scene_Occurrence occurrence);
// Returns a packed version of the whole scene tree
PXZ_EXPORTED Scene_PackedTree Scene_getCompleteTree(Scene_Occurrence root, Scene_VisibilityMode visibilityMode);
// Returns a component on an occurrence
PXZ_EXPORTED Scene_Component Scene_getComponent(Scene_Occurrence occurrence, Scene_ComponentType componentType, Core_Bool followPrototypes);
// Returns one component of the specified type by occurrence if it exists
PXZ_EXPORTED Scene_ComponentList Scene_getComponentByOccurrence(Scene_OccurrenceList occurrences, Scene_ComponentType componentType, Core_Bool followPrototypes);
// Get the occurrence that own a component
PXZ_EXPORTED Scene_Occurrence Scene_getComponentOccurrence(Scene_Component component);
// Get the type of a component
PXZ_EXPORTED Scene_ComponentType Scene_getComponentType(Scene_Component component);
// Returns the global matrix on an occurrence
PXZ_EXPORTED Geom_Matrix4 Scene_getGlobalMatrix(Scene_Occurrence occurrence);
// Returns the global visibility of a given occurrence
PXZ_EXPORTED Core_Boolean Scene_getGlobalVisibility(Scene_Occurrence occurrence);
// Returns the local matrix on an occurrence
PXZ_EXPORTED Geom_Matrix4 Scene_getLocalMatrix(Scene_Occurrence occurrence);
// Returns the Minimum Bounding Box of a list of scene paths
PXZ_EXPORTED Geom_MBB Scene_getMBB(Scene_OccurrenceList occurrences);
// Get a metadata property value from a metadata component
PXZ_EXPORTED Core_String Scene_getMetadata(Scene_Metadata metadata, Core_String name);
// Returns definition of Metadata components
PXZ_EXPORTED Scene_MetadataDefinitionList Scene_getMetadatasDefinitions(Scene_MetadataList metadatas);
// Returns the name of an occurrence
PXZ_EXPORTED Core_String Scene_getNodeName(Scene_Occurrence occurrence);
// Returns the Oriented Bounding Box of a list of scene paths (works only on meshes, fast method, not the Minimum Volume Box)
PXZ_EXPORTED Geom_OBB Scene_getOBB(Scene_OccurrenceList occurrences);
// Returns the active material on a given occurrence
PXZ_EXPORTED Material_Material Scene_getOccurrenceActiveMaterial(Scene_Occurrence occurrence);
// Get the parent of an occurrence
PXZ_EXPORTED Scene_Occurrence Scene_getParent(Scene_Occurrence occurrence);
// Returns the active shape of a part
PXZ_EXPORTED Scene_Shape Scene_getPartActiveShape(Scene_Part part);
// Recursively get all the occurrences containing a part component
PXZ_EXPORTED Scene_OccurrenceList Scene_getPartOccurrences(Scene_Occurrence from);
// Returns the number of polygon in the parts meshes
PXZ_EXPORTED Core_Int Scene_getPolygonCount(Scene_OccurrenceList occurrences, Core_Bool asTriangleCount, Core_Bool countOnceEachInstance, Core_Bool countHidden);
// Returns all the occurrences prototyping the given occurrence
PXZ_EXPORTED Scene_OccurrenceList Scene_getReferencers(Scene_Occurrence prototype);
// Get the root occurrence of the product structure
PXZ_EXPORTED Scene_Occurrence Scene_getRoot();
// Returns some stats of a sub tree
typedef struct {
   Core_Int partCount;
   Core_Int partOccurrenceCount;
   Core_Int triangleCount;
   Core_Int triangleOccurrenceCount;
   Core_Int vertexCount;
   Core_Int vertexOccurrenceCount;
} Scene_getSubTreeStatsReturn;
PXZ_EXPORTED Scene_getSubTreeStatsReturn Scene_getSubTreeStats(Scene_Occurrence root);
// Returns the number of vertices in the parts meshes
PXZ_EXPORTED Core_Int Scene_getVertexCount(Scene_OccurrenceList occurrences, Core_Bool countOnceEachInstance, Core_Bool countHidden, Core_Bool countPoints, Core_Bool countMergedVertices);
// Returns viewpoints from model cavities
typedef struct {
   Geom_Point3List positions;
   Geom_Point3List directions;
} Scene_getViewpointsFromCavitiesReturn;
PXZ_EXPORTED Scene_getViewpointsFromCavitiesReturn Scene_getViewpointsFromCavities(Geom_Distance voxelSize, Geom_Distance minCavityVolume);
// Returns True if the given occurrence has the given component type
PXZ_EXPORTED Core_Boolean Scene_hasComponent(Scene_Occurrence occurrence, Scene_ComponentType componentType, Core_Bool followPrototypes);
// Hide the given occurrence
PXZ_EXPORTED void Scene_hide(Scene_Occurrence occurrence);
// Create the default light
PXZ_EXPORTED void Scene_insertDefaultLightsInTree();
// List all components on a type on the whole tree
PXZ_EXPORTED Scene_ComponentList Scene_listComponent(Scene_ComponentType componentType);
// List all components on an occurrence
PXZ_EXPORTED Scene_ComponentList Scene_listComponents(Scene_Occurrence occurrence, Core_Bool followPrototypes);
// list all the materials used in the part shape
PXZ_EXPORTED Material_MaterialList Scene_listPartSubMaterials(Scene_Part part);
// Merge all equivalent images (i.e. with same pixels)
PXZ_EXPORTED Core_Int Scene_mergeImages();
// Merge all equivalent materials (i.e. with same appearance)
PXZ_EXPORTED Core_Int Scene_mergeMaterials();
// Move an occurrence, adjusting the transformation to keep objects at the same place in the world space
PXZ_EXPORTED void Scene_moveOccurrences(Scene_OccurrenceList occurrences, Scene_Occurrence destination);
// Remove a property from a metadata
PXZ_EXPORTED void Scene_removeMetadata(Scene_Metadata metadata, Core_String name);
// truncate names of occurrence with too long names
PXZ_EXPORTED void Scene_renameLongOccurrenceName(Core_Int maxLength);
// Replace a material by another everywhere it is used
PXZ_EXPORTED void Scene_replaceMaterial(Material_Material originalMaterial, Material_Material newMaterial, Scene_OccurrenceList occurrences);
// Resizes the textures from a selection of occurrences (resizes all textures used by these occurrences), or from a selection of textures
PXZ_EXPORTED void Scene_resizeTextures(Scene_ResizeTexturesInputMode inputMode, Scene_ResizeTexturesResizeMode resizeMode, Core_Bool replaceTextures);
// Selects occurrences for which the property "Material" is the given material
PXZ_EXPORTED void Scene_selectByMaterial(Material_Material material);
// Selects parts for which the given material is visible in the viewer
PXZ_EXPORTED void Scene_selectByVisibleMaterial(Material_Material material);
// find part occurences in the scene in a given box and add them to the selection
PXZ_EXPORTED void Scene_selectPartOccurrencesInBox(Geom_ExtendedBox box, Core_Boolean strictlyIncludes);
// Move a component to an occurrence
PXZ_EXPORTED void Scene_setComponentOccurrence(Scene_Component component, Scene_Occurrence occurrence);
// Set the default variant
PXZ_EXPORTED void Scene_setDefaultVariant();
// Set the material on a occurrence
PXZ_EXPORTED void Scene_setOccurrenceMaterial(Scene_Occurrence occurrence, Material_Material material);
// Set the parent of an occurrence
PXZ_EXPORTED void Scene_setParent(Scene_Occurrence occurrence, Scene_Occurrence parent, Core_Boolean addInParentInstances, Scene_Occurrence insertBefore);
// Show the given occurrence
PXZ_EXPORTED void Scene_show(Scene_Occurrence occurrence);
// Show only the given occurrence
PXZ_EXPORTED void Scene_showOnly(Scene_Occurrence occurrence);
// Updates the designed ray prober
PXZ_EXPORTED void Scene_updateRayProber(Core_Ident proberID, Geom_Matrix4 matrix);
// Updates the designed sphere prober
PXZ_EXPORTED void Scene_updateSphereProber(Core_Ident proberID, Geom_Vector3 sphereCenter, Core_Double sphereRadius);

// ----------------------------------------------------
// LODs
// Levels of detail management related functions
// ----------------------------------------------------


// ----------------------------------------------------
// OoC
// Out of Core related functions
// ----------------------------------------------------


// ----------------------------------------------------
// alternative trees
// AlternativeTree related functions
// ----------------------------------------------------

// Create a new alternative tree
PXZ_EXPORTED Scene_AlternativeTree Scene_createAlternativeTree(Core_String name, Scene_Occurrence root);
// Returns the root occurrence associated with the given AlternativeTree
PXZ_EXPORTED Scene_Occurrence Scene_getAlternativeTreeRoot(Scene_AlternativeTree tree);
// Returns all the available alternative trees
PXZ_EXPORTED Scene_AlternativeTreeList Scene_listAlternativeTrees();

// ----------------------------------------------------
// animations
// Animations related functions
// ----------------------------------------------------

// Adds a keyframe in the curve
PXZ_EXPORTED Scene_Keyframe Scene_addKeyframe(Scene_AnimChannel channel, Scene_AnimationTime time, Core_Double value);
// Adds keyframes in a given AnimChannel based on current position
PXZ_EXPORTED void Scene_addKeyframeFromCurrentPosition(Scene_AnimChannel channel, Scene_AnimationTime time);
// Does this Animation animates this Occurrence - or one of its parents (thus animating it indirectly) ?
PXZ_EXPORTED Core_Bool Scene_animatesThisOccurrence(Scene_Animation animation, Scene_Occurrence occurrence);
// Baking soda
PXZ_EXPORTED void Scene_bakeAnimation(Scene_Animation animation, Scene_Occurrence occurrence, Scene_Occurrence end, Scene_AnimationTime intervall);
// Creates an animation
PXZ_EXPORTED Scene_Animation Scene_createAnimation(Core_String name);
// Create a skeleton mesh from a joint component tree
PXZ_EXPORTED void Scene_createSkeletonMesh(Scene_Occurrence root);
// Decimates by segment a given AnimChannel
PXZ_EXPORTED void Scene_decimateAnimChannelBySegment(Scene_AnimChannel channel, Core_Double precision);
// Deletes an animation
PXZ_EXPORTED void Scene_deleteAnimation(Scene_Animation animation);
// Displays info on the selected AnimChannel
PXZ_EXPORTED void Scene_displayAllKeyframesFromAnimChannel(Scene_AnimChannel channel);
// Displays info on the selected animation
PXZ_EXPORTED void Scene_displayAllKeyframesFromAnimation(Scene_Animation animation);
// Displays the value
PXZ_EXPORTED void Scene_displayValueFromAnimChannelAtTime(Scene_AnimChannel channel, Scene_AnimationTime time, Core_Bool defaultValue);
// Returns the main AnimChannel of an Occurrence according to a given Animation
PXZ_EXPORTED Scene_AnimChannel Scene_getAnimChannelIfExists(Scene_Animation animation, Scene_Occurrence occurrence);
// Returns the Occurrence related to a given AnimChannel
PXZ_EXPORTED Scene_Occurrence Scene_getAnimChannelOccurrence(Scene_AnimChannel channel);
// Returns the parent AnimChannel of a given Keyframe
PXZ_EXPORTED Scene_AnimChannel Scene_getKeyframeParentAnimChannel(Scene_Keyframe keyframe);
// Returns a list of all keyframes of a simple animChannel
PXZ_EXPORTED Scene_KeyframeList Scene_getKeyframes(Scene_AnimChannel channel);
// Returns the main AnimChannel of a given AnimChannel
PXZ_EXPORTED Scene_AnimChannel Scene_getMainChannel(Scene_AnimChannel channel);
// Returns the Joint assigned to an occurrence if any
PXZ_EXPORTED Polygonal_Joint Scene_getOccurrenceJoint(Scene_Occurrence occurrence);
// Returns (if exists) the parent AnimChannel of a given AnimChannel
PXZ_EXPORTED Scene_AnimChannel Scene_getParentChannel(Scene_AnimChannel channel);
// Returns the subchannel of a given name from an AnimChannel
PXZ_EXPORTED Scene_AnimChannel Scene_getSubChannel(Scene_AnimChannel channel, Core_String name);
// Returns all the sub channel of an AnimChannel
PXZ_EXPORTED Scene_AnimChannelList Scene_getSubChannels(Scene_AnimChannel channel);
// Creates a Binder in an Animation stack to animate an entity's property
PXZ_EXPORTED Scene_AnimChannel Scene_linkPropertyToAnimation(Scene_Animation animation, Core_Entity entity, Core_String propertyName);
// List all Animations from the scene
PXZ_EXPORTED Scene_AnimationList Scene_listAnimations();
// List all main AnimChannel from a given Animation
PXZ_EXPORTED Scene_AnimChannelList Scene_listMainChannels(Scene_Animation animation);
// Creates keyframes with the default values of the channel at time 0
PXZ_EXPORTED void Scene_makeDefaultKeyframe(Scene_AnimChannel channel);
// Moving animation
PXZ_EXPORTED void Scene_moveAnimation(Scene_Animation animation, Scene_Occurrence target, Scene_Occurrence newParent, Scene_AnimationTime intervall);
// Removes a keyframe in the curve
PXZ_EXPORTED void Scene_removeKeyframe(Scene_AnimChannel channel, Scene_AnimationTime time);
// Unlinks a binder
PXZ_EXPORTED void Scene_unlinkPropertyToAnimation(Scene_Animation animation, Core_Entity entity, Core_String propertyName);

// ----------------------------------------------------
// debug
// Debug functions
// ----------------------------------------------------

typedef struct {
   Core_Int partCount;
   Core_Int totalPartCount;
   Core_Int vertexCount;
   Core_Int totalVertexCount;
   Core_Int edgeCount;
   Core_Int totalEdgeCount;
   Core_Int domainCount;
   Core_Int totalDomainCount;
   Core_Int bodyCount;
   Core_Int totalBodyCount;
   Core_Double area2Dsum;
   Core_Int boundaryCount;
   Core_Int boundaryEdgeCount;
} Scene_getBRepInfosReturn;
PXZ_EXPORTED Scene_getBRepInfosReturn Scene_getBRepInfos();
typedef struct {
   Core_Int partCount;
   Core_Int totalPartCount;
   Core_Int vertexCount;
   Core_Int totalVertexCount;
   Core_Int edgeCount;
   Core_Int totalEdgeCount;
   Core_Int polygonCount;
   Core_Int totalPolygonCount;
   Core_Int patchCount;
   Core_Int totalPatchCount;
   Core_Int boundaryCount;
   Core_Int boundaryEdgeCount;
} Scene_getTessellationInfosReturn;
PXZ_EXPORTED Scene_getTessellationInfosReturn Scene_getTessellationInfos();
// Print an occurrence tree on log
PXZ_EXPORTED void Scene_print(Scene_Occurrence root);

// ----------------------------------------------------
// events
// ----------------------------------------------------

   /*!
     \param proberID The ray propber ID
     \param proberInfo The prober's info
   */
PXZ_EXPORTED Core_Ident Scene_addonRayProbeCallback(void(*fp)(void *, Core_Ident, Scene_ProberInfo), void * userdata = nullptr);
PXZ_EXPORTED void Scene_removeonRayProbeCallback(Core_Ident id);

   /*!
     \param proberID The sphere propber ID
     \param proberInfo The prober's info
   */
PXZ_EXPORTED Core_Ident Scene_addonSphereProbeCallback(void(*fp)(void *, Core_Ident, Scene_ProberInfo), void * userdata = nullptr);
PXZ_EXPORTED void Scene_removeonSphereProbeCallback(Core_Ident id);


// ----------------------------------------------------
// filters
// Scene filtering functions
// ----------------------------------------------------

// Add a filter to the filters library
PXZ_EXPORTED Core_Ident Scene_addFilterToLibrary(Core_String name, Scene_FilterExpression expr);
// Evaluate the given filter expression
PXZ_EXPORTED Core_String Scene_evaluateExpression(Scene_FilterExpression filter);
// evaluate the given filter expression on all occurrences under the given occurrence and returns the result
PXZ_EXPORTED Core_StringList Scene_evaluateExpressionOnOccurrences(Scene_OccurrenceList occurrences, Scene_FilterExpression filter);
// evaluate the given filter expression on all occurrences under the given occurrence and returns the result
typedef struct {
   Scene_OccurrenceList occurrences;
   Core_StringList evaluations;
} Scene_evaluateExpressionOnSubTreeReturn;
PXZ_EXPORTED Scene_evaluateExpressionOnSubTreeReturn Scene_evaluateExpressionOnSubTree(Scene_FilterExpression filter, Scene_Occurrence from);
// Export filters from a given file
PXZ_EXPORTED void Scene_exportFilterLibrary(Core_FilePath file);
// Returns the first filter in the filter library with the given name
PXZ_EXPORTED Scene_Filter Scene_findFilterByName(Core_String name);
// Returns the filter expression (string) from a filter id stored in the library
PXZ_EXPORTED Scene_FilterExpression Scene_getFilterExpression(Core_Ident filterId);
// Retrieve a filter from the library with its identifier
PXZ_EXPORTED Scene_Filter Scene_getFilterFromLibrary(Core_Ident filterId);
// Recursively get all the occurrences validating the given filter expression
PXZ_EXPORTED Scene_OccurrenceList Scene_getFilteredOccurrences(Scene_FilterExpression filter, Scene_Occurrence from);
// Import filters from a given file
PXZ_EXPORTED void Scene_importFilterLibrary(Core_FilePath file);
// Returns all the filter stored in the filter library
PXZ_EXPORTED Scene_FilterList Scene_listFilterLibrary();
// Remove a filter from the filters library
PXZ_EXPORTED void Scene_removeFilterFromLibrary(Core_Ident filterId);

// ----------------------------------------------------
// isolate
// Isolate functions
// ----------------------------------------------------

// Enter isolate mode by isolating a subset of the scene for process, export, viewer, ...
PXZ_EXPORTED void Scene_isolate(Scene_OccurrenceList occurrences);
// Exit the isolate mode
PXZ_EXPORTED void Scene_unisolate();

// ----------------------------------------------------
// merging
// Part merging related functions
// ----------------------------------------------------

// Merge all parts within the same area.
PXZ_EXPORTED Scene_OccurrenceList Scene_mergeByRegions(Scene_OccurrenceList roots, Scene_MergeByRegionsStrategy mergeBy, Scene_MergeStrategy strategy);
// Merge all parts over maxLevel level
PXZ_EXPORTED void Scene_mergeByTreeLevel(Scene_OccurrenceList partOccurrences, Core_Int maxLevel, Scene_MergeHiddenPartsMode mergeHiddenPartsMode);
// Merge final level (occurrences with only occurrence with part component as children)
PXZ_EXPORTED void Scene_mergeFinalLevel(Scene_OccurrenceList roots, Scene_MergeHiddenPartsMode mergeHiddenPartsMode, Core_Boolean CollapseToParent);
// Merge a set of parts
PXZ_EXPORTED Scene_OccurrenceList Scene_mergeParts(Scene_OccurrenceList partOccurrences, Scene_MergeHiddenPartsMode mergeHiddenPartsMode);
// Merge all parts under each assembly together
PXZ_EXPORTED void Scene_mergePartsByAssemblies(Scene_OccurrenceList roots, Scene_MergeHiddenPartsMode mergeHiddenPartsMode);
// Merge a set of parts by materials
PXZ_EXPORTED Scene_OccurrenceList Scene_mergePartsByMaterials(Scene_OccurrenceList partOccurrences, Core_Boolean mergeNoMaterials, Scene_MergeHiddenPartsMode mergeHiddenPartsMode, Core_Boolean combineMeshes);
// Merge all parts by occurences names
PXZ_EXPORTED void Scene_mergePartsByName(Scene_Occurrence root, Scene_MergeHiddenPartsMode mergeHiddenPartsMode);
// Set all materials on part occurrences
PXZ_EXPORTED void Scene_transferCADMaterialsOnPartOccurrences(Scene_Occurrence rootOccurrence);
// Take the first instance material and set it one the mesh patches
PXZ_EXPORTED void Scene_transferMaterialsOnPatches(Scene_Occurrence rootOccurrence);

// ----------------------------------------------------
// modification
// ----------------------------------------------------

// apply a transformation to the local matrix of an occurrence
PXZ_EXPORTED void Scene_applyTransformation(Scene_Occurrence occurrence, Geom_Matrix4 matrix);
// Create symmetries from selection
PXZ_EXPORTED void Scene_createSymmetry(Scene_OccurrenceList occurrences, Geom_AxisPlane plane);
// Modify the local matrix of the scene node to apply a rotation
PXZ_EXPORTED void Scene_rotate(Scene_Occurrence occurrence, Geom_Vector3 axis, Geom_Angle angle);
// change the local matrix on an occurrence
PXZ_EXPORTED void Scene_setLocalMatrix(Scene_Occurrence occurrence, Geom_Matrix4 matrix);

// ----------------------------------------------------
// part
// Part component functions
// ----------------------------------------------------

// Return the mesh of the TesselatedShape
PXZ_EXPORTED Polygonal_Mesh Scene_getPartMesh(Scene_Part part);
// Return the model of the BRepShape
PXZ_EXPORTED CAD_Model Scene_getPartModel(Scene_Part part);
// Return the meshes of the TesselatedShape for each given parts if any
PXZ_EXPORTED Polygonal_MeshList Scene_getPartsMeshes(Scene_PartList parts);
// Return the models of the BRepShape for each given parts if any
PXZ_EXPORTED CAD_ModelList Scene_getPartsModels(Scene_PartList parts);
// Returns the transform matrix of each given parts
PXZ_EXPORTED Geom_Matrix4List Scene_getPartsTransforms(Scene_PartList parts);
// Returns the transform matrix of each given parts (indexed mode)
typedef struct {
   Core_IntList indices;
   Geom_Matrix4List transforms;
} Scene_getPartsTransformsIndexedReturn;
PXZ_EXPORTED Scene_getPartsTransformsIndexedReturn Scene_getPartsTransformsIndexed(Scene_PartList parts);
// Add a mesh to a part (create a TessellatedShape on the part)
PXZ_EXPORTED void Scene_setPartMesh(Scene_Part part, Polygonal_Mesh mesh);
// Add a model to a part (create a BRepShape on the part)
PXZ_EXPORTED void Scene_setPartModel(Scene_Part part, CAD_Model model);
// Set the transform matrix of each given parts
PXZ_EXPORTED void Scene_setPartsTransforms(Scene_PartList parts, Geom_Matrix4List transforms);
// Set the transform matrix of each given parts (indexed mode)
PXZ_EXPORTED void Scene_setPartsTransformsIndexed(Scene_PartList parts, Core_IntList indices, Geom_Matrix4List transforms);

// ----------------------------------------------------
// pivots
// Pivot moving functions
// ----------------------------------------------------

// Re-orient the Pivot Point straight to world origin (the grid)
PXZ_EXPORTED void Scene_alignPivotPointToWorld(Scene_OccurrenceList occurrences, Core_Bool applyToChildren);
// Move the pivot point of each occurrence listed in the function input, to the center of its bounding box (and of its children if the parameter is True)
PXZ_EXPORTED void Scene_movePivotPointToOccurrenceCenter(Scene_OccurrenceList occurrences, Core_Bool applyToChildren);
// Move the pivot point of an occurrence (and its descendants if recursively) to the origin (0,0,0)
PXZ_EXPORTED void Scene_movePivotPointToOrigin(Scene_Occurrence occurrence, Core_Bool applyToChildren);
// Move the pivot point of all given occurrences to the center of all occurrences
PXZ_EXPORTED void Scene_movePivotPointToSelectionCenter(Scene_OccurrenceList occurrences);
// Move the pivot point of each occurrence listed in the function input, to the center of the targeted occurrence Center (and of its children if the parameter is True)
PXZ_EXPORTED void Scene_movePivotPointToTargetedOccurrenceCenter(Scene_OccurrenceList occurrences, Scene_Occurrence target, Core_Bool applyToChildren);
// Set the pivot of an occurrence to the given transformation matrix, the geometry will not be moved (warning: do not confuse with property Transform which actually move the occurrence)
PXZ_EXPORTED void Scene_setPivotOnly(Scene_Occurrence occurrence, Geom_Matrix4 pivot);

// ----------------------------------------------------
// prototype
// Prototyping related functions
// ----------------------------------------------------

// Returns the prototype of an occurrence
PXZ_EXPORTED Scene_Occurrence Scene_getPrototype(Scene_Occurrence occurrence);
// Create occurrences that prototype the given occurrence and all its subtree
PXZ_EXPORTED Scene_Occurrence Scene_prototypeSubTree(Scene_Occurrence prototype);
// Sets the prototype of an occurrence
PXZ_EXPORTED void Scene_setPrototype(Scene_Occurrence occurrence, Scene_Occurrence prototype);

// ----------------------------------------------------
// selection
// Selection related functions
// ----------------------------------------------------

// Clear the current selection
PXZ_EXPORTED void Scene_clearSelection();
// Delete all selected occurrences, and/or sub-occurrence elements
PXZ_EXPORTED void Scene_deleteSelection();
// For each occurrence, create a new occurrence with the selected sub-occurrence elements and remove them from the original occurrence
PXZ_EXPORTED void Scene_explodeSelection();
// Returns all the selected occurrences
PXZ_EXPORTED Scene_OccurrenceList Scene_getSelectedOccurrences();
// Invert the orientation of each selected item (occurrences and/or sub-occurrence elements
PXZ_EXPORTED void Scene_invertOrientationSelection();
// Replace the selection by all unselected part occurrences
PXZ_EXPORTED void Scene_invertSelection();
// Remove all materials appplied to the selection
PXZ_EXPORTED void Scene_removeMaterials();
// Add occurrences to selection
PXZ_EXPORTED void Scene_select(Scene_OccurrenceList occurrences);
// Select all part occurrences
PXZ_EXPORTED void Scene_selectAllPartOccurrences();
// Seperate all polygones form their original parts into a new one
PXZ_EXPORTED Scene_Occurrence Scene_separateSelection();
// Remove occurrences to selection
PXZ_EXPORTED void Scene_unselect(Scene_OccurrenceList occurrence);

// ----------------------------------------------------
// simplification
// Scene structure simplification functions
// ----------------------------------------------------

// Compress a sub-tree by removing occurrence containing only one Child or empty, and by removing useless instances (see removeUselessInstances)
PXZ_EXPORTED Scene_Occurrence Scene_compress(Scene_Occurrence occurrence);
// Modify the visible properties of the sub-tree to look like old school visibility (only hidden/inherited)
PXZ_EXPORTED void Scene_convertToOldSchoolVisibility(Scene_Occurrence root);
// Get duplicated parts
PXZ_EXPORTED Scene_OccurrenceList Scene_getDuplicatedParts(Scene_Occurrence root, Core_Real acceptVolumeRatio, Core_Real acceptPolycountRatio, Core_Real acceptAABBAxisRatio, Geom_Distance acceptAABBCenterDistance);
// Identify parts with more than one occurrence on the scene
PXZ_EXPORTED void Scene_identifyInstances(Core_Int minOccurrenceCount);
// Singularize all instances on the sub-tree of an occurrence
PXZ_EXPORTED void Scene_makeInstanceUnique(Scene_OccurrenceList occurrences);
// Set the same parent to all descending parts (all parts will be singularized)
PXZ_EXPORTED void Scene_rake(Scene_Occurrence occurrence, Core_Boolean keepInstances);
// Remove symmetry matrices (apply matrices on geometries on nodes under an occurrence with a symmetry matrix
PXZ_EXPORTED void Scene_removeSymmetryMatrices(Scene_Occurrence occurrence);
// Remove instances where they are not needed (prototype referenced once, ...)
PXZ_EXPORTED void Scene_removeUselessInstances(Scene_Occurrence occurrence);
// Set all part transformation matrices to identity in a sub-tree, transformation will be applied to the shapes
PXZ_EXPORTED void Scene_resetPartTransform(Scene_Occurrence root);
// Set all transformation matrices to identity in a sub-tree.
PXZ_EXPORTED void Scene_resetTransform(Scene_Occurrence root, Core_Boolean recursive, Core_Boolean keepInstantiation, Core_Boolean keepPartTransform);
// Select all parts meeting the criteria
PXZ_EXPORTED void Scene_selectByMaximumSize(Scene_OccurrenceList roots, Geom_Distance maxDiagLength, Geom_Distance maxSize, Core_Boolean selectHidden);
// Select duplicated parts
PXZ_EXPORTED void Scene_selectDuplicated(Core_Real acceptVolumeRatio, Core_Real acceptPolycountRatio, Core_Real acceptAABBAxisRatio, Geom_Distance acceptAABBCenterDistance);
// Select occurrences sharing the same prototype as the given one
PXZ_EXPORTED void Scene_selectInstances(Scene_Occurrence occurrence);
// Select hidden parts
PXZ_EXPORTED void Scene_selectPartsFromNoShow();

// ----------------------------------------------------
// spatialRequest
// ----------------------------------------------------

PXZ_EXPORTED Scene_RayHit Scene_rayCast(Geom_Ray ray, Scene_Occurrence root);
PXZ_EXPORTED Scene_RayHitList Scene_rayCastAll(Geom_Ray ray, Scene_Occurrence root);

// ----------------------------------------------------
// variant
// variant related functions
// ----------------------------------------------------

// Create a new variant
PXZ_EXPORTED Scene_Variant Scene_addVariant(Core_String name);
// Create a new variant which is a copy of an existing variant
PXZ_EXPORTED Scene_Variant Scene_duplicateVariant(Scene_Variant variant, Core_String name);
// Returns the definitions of multiple variant components
PXZ_EXPORTED Scene_VariantDefinitionListList Scene_getVariantComponentsDefinitions(Scene_VariantComponentList variantComponents);
// Get the alternative tree used by this variant
PXZ_EXPORTED Scene_AlternativeTree Scene_getVariantTree(Scene_Variant variant);
// Returns all the available variants
PXZ_EXPORTED Scene_VariantList Scene_listVariants();
// Remove a variant
PXZ_EXPORTED void Scene_removeVariant(Scene_Variant variant);
// Change the current variant used
PXZ_EXPORTED void Scene_setCurrentVariant(Scene_Variant variant);
// Set the alternative tree to use for this variant
PXZ_EXPORTED void Scene_setVariantTree(Scene_Variant variant, Scene_AlternativeTree tree);



#endif
