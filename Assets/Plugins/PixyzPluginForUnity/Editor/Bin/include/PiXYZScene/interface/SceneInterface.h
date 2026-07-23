// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_SCENE_INTERFACE_SCENEINTERFACE_H_
#define _PXZ_SCENE_INTERFACE_SCENEINTERFACE_H_

#include "SceneTypes.h"

PXZ_MODULE_BEGIN(SceneI)

class PXZ_EXPORTED SceneInterface
{
public:
   //! Add a component to an occurrence
   /*!
     \param occurrence The occurrence to add the new component
     \param componentType Type of the component
     \return component The new component added to occurrence
   */
   static Component addComponent(const Occurrence & occurrence, const ComponentType & componentType);

   //! Add a light component to an occurrence
   /*!
     \param occurrence The occurrence to add the new component
     \return component The new component added to occurrence
   */
   static Component addLightComponent(const Occurrence & occurrence);

   //! Add a new metadata property to a metadata component
   /*!
     \param metadata The metadata component
     \param name The new property name
     \param value The new property value
   */
   static void addMetadata(const Metadata & metadata, const CoreI::String & name, const CoreI::String & value);

   //! Add a new metadata property to a metadata component
   /*!
     \param metadata The metadata component
     \param names The new properties names
     \param values The new properties values
   */
   static void addMetadataBlock(const Metadata & metadata, const CoreI::StringList & names, const CoreI::StringList & values);

   //! Remove unused images from texture library
   /*!
     \return nbTextureDeleted The number of texture deleted by the clean
   */
   static CoreI::Int cleanUnusedImages();

   //! Remove unused materials from material library
   /*!
     \param cleanImages Call cleanUnusedImages if true
     \return nbMaterialDeleted The number of material deleted by the clean
   */
   static CoreI::Int cleanUnusedMaterials(const CoreI::Boolean & cleanImages = false);

   //! Compute the checksum of a sub-tree
   /*!
     \param root Occurrence to compute
     \return checksum 
   */
   static CoreI::String computeSubTreeChecksum(const Occurrence & root = Occurrence());

   //! Create a complete scene tree
   /*!
     \param tree 
     \param root Specify the root occurrence of the scene
     \param replaceRoot If true, the root occurrence will be replaced by the root of the given tree, else it will be added as a child
     \return occurrences Create occurrences (equivalent to tree.occurrences)
   */
   static OccurrenceList createCompleteTree(const PackedTree & tree, const Occurrence & root = 0, const CoreI::Bool & replaceRoot = true);

   //! Create a new cone
   /*!
     \param bottomRadius Radius of the bottom of the cone 
     \param height Height of thes cone
     \param sides Number of sides of the cone
     \param generateUV Generation of the UV
     \return occurrence The created Occurrence
   */
   static Occurrence createCone(const CoreI::Double & bottomRadius, const CoreI::Double & height, const CoreI::Int & sides = 16, const CoreI::Boolean & generateUV = true);

   //! Create a new cube
   /*!
     \param sizeX Size of the Cube on the x axis
     \param sizeY Size of the Cube on the y axis
     \param sizeZ Size of the Cube on the z axis
     \param subdivision Subdivision of the Cube on all the axis
     \param generateUV Generation of the UV
     \return occurrence The created Occurrence
   */
   static Occurrence createCube(const CoreI::Double & sizeX, const CoreI::Double & sizeY, const CoreI::Double & sizeZ, const CoreI::Int & subdivision = 1, const CoreI::Boolean & generateUV = true);

   //! Create a new cylinder
   /*!
     \param radius Radius of the Cylinder
     \param height Height of the Cylinder
     \param sides Number of Sides of the Cylinder
     \param generateUV Generation of the UV
     \return occurrence The created Occurrence
   */
   static Occurrence createCylinder(const CoreI::Double & radius, const CoreI::Double & height, const CoreI::Int & sides = 16, const CoreI::Boolean & generateUV = true);

   //! Create a new directional light
   /*!
     \param color Color of the light
     \param power Intensity of the light
     \param direction Relative direction of the light
     \return light The created directional light
   */
   static DirectionalLight createDirectionalLight(const CoreI::Color & color, const CoreI::Double & power, const GeomI::Vector3 & direction = GeomI::Point3(0, 0, 1));

   //! Create a new bagel klein
   /*!
     \param radius Radius of the Immersion
     \param subdivisionX Subdivision of the Immersion on the Latitude
     \param subdivisionY Subdivision of the Immersion on the Longitude
     \return occurrence The created Occurrence
   */
   static Occurrence createImmersion(const CoreI::Double & radius, const CoreI::Int & subdivisionX, const CoreI::Int & subdivisionY);

   //! Create Metadata components from definitions
   /*!
     \param occurrences List of occurrences to add the metadata components
     \param definitions List of metadata definition
     \return metadatas List of metadata components created from definitions (if definition is empty no component is created)
   */
   static MetadataList createMetadatasFromDefinitions(const OccurrenceList & occurrences, const MetadataDefinitionList & definitions);

   /*!
     \param occurrence 
     \return obbOccurrence The create occurrence
   */
   static Occurrence createOBBMesh(const Occurrence & occurrence);

   //! Create a new occurrence
   /*!
     \param name Name of the new occurrence
     \param parent Create the occurrence as a child of parent, if not set the parent will be root
     \return occurrence Created occurrence
   */
   static Occurrence createOccurrence(const CoreI::String & name, const Occurrence & parent = Occurrence());

   //! Create a new occurrence and add the given occurrences as children
   /*!
     \param name Name of the new occurrence
     \param children Add given occurrence as children (if any)
     \param parent If defined, the new occurrence will be created as a child of this parent. Else if children are defined, the first common parent of children will be used as a parent for this new occurrence. Last resort will be to use the root as parent
     \param keepMaterialAssignment If defined, material assignation will be updated to keep the visual same aspect
     \return occurrence Created occurrence
   */
   static Occurrence createOccurrenceFromSelection(const CoreI::String & name, const OccurrenceList & children, const Occurrence & parent, const CoreI::Boolean & keepMaterialAssignment = true);

   //! Creates an occurrence from string
   /*!
     \param text The occurrence's name
     \param font The font to use
     \param fontSize The font size
     \param color The occurrence color
     \return occurence 
   */
   static Occurrence createOccurrenceFromText(const CoreI::String & text, const CoreI::String & font = "ChicFont", const CoreI::Int & fontSize = 64, const CoreI::ColorAlpha & color = CoreI::ColorAlpha());

   //! Create one new occurrence under each given parent
   /*!
     \param name Name of the new occurrence
     \param parents Create the occurrences as a child of each parent. If empty, one occurrence will be created with root as parent
     \return occurrences Created occurrences
   */
   static OccurrenceList createOccurrences(const CoreI::String & name, const OccurrenceList & parents = OccurrenceList(0));

   //! Create a set of Parts given meshes and occurrences
   /*!
     \param occurrences The occurrence which will contains the part component of the mesh at the same index
     \param meshes List of mesh to create part, if the mesh is invalid (e.g 0) no part will be created and 0 will be returned in the parts list at this index
     \return parts List of created parts components, if there is no mesh at an index, no part is created and the identifier is 0
   */
   static PartList createPartsFromMeshes(const OccurrenceList & occurrences, const PolygonalI::MeshList & meshes);

   //! Create a  new plane
   /*!
     \param sizeY Size of the Plane on the y axis
     \param sizeX Size of the Plane on the x axis
     \param subdivisionX Subdivision of the Plane on the x axis
     \param subdivisionY Subdivision of the Plane on the y axis
     \param generateUV Generation of the UV
     \return occurrence The created Occurrence
   */
   static Occurrence createPlane(const CoreI::Double & sizeY, const CoreI::Double & sizeX, const CoreI::Int & subdivisionX = 1, const CoreI::Int & subdivisionY = 1, const CoreI::Boolean & generateUV = true);

   //! Create a new positional light
   /*!
     \param color Color of the light
     \param power Intensity of the light
     \param position Relative position of the light
     \return light The created positional light
   */
   static PositionalLight createPositionalLight(const CoreI::Color & color, const CoreI::Double & power, const GeomI::Point3 & position = GeomI::Point3());

   //! Creates a ray prober
   /*!
     \return id 
   */
   static CoreI::Ident createRayProber();

   //! Create a scene tree with a list of meshes, all meshes becomes part occurrences with the same root. The same mesh Id can be used several times to handle create instances (prototypes)
   /*!
     \param meshes List of input meshes
     \param matrices List of matrices of input meshes (if empty Identity will be used)
     \param centerPartPivots If True, the input meshes will be centered in their local coordinate system and the translation will be set as part matrix. If you want to rollback the meshes to their initial pivots use 'resetPartTransform' function
     \return root The created root occurrence
   */
   static Occurrence createSceneFromMeshes(const PolygonalI::MeshList & meshes, const GeomI::Matrix4List & matrices, const CoreI::Boolean & centerPartPivots = true);

   //! Create a new shpere
   /*!
     \param radius Radius of the Sphere
     \param subdivisionLatitude Subdivision of the Sphere on the Latitude
     \param subdivisionLongitude Subdivision of the Sphere on the Longitude
     \param generateUV Generation of the UV
     \return occurrence The created Occurrence
   */
   static Occurrence createSphere(const CoreI::Double & radius, const CoreI::Int & subdivisionLatitude = 16, const CoreI::Int & subdivisionLongitude = 16, const CoreI::Boolean & generateUV = true);

   //! Creates a sphere prober
   /*!
     \return id 
   */
   static CoreI::Ident createSphereProber();

   //! Create a new spot light
   /*!
     \param color Color of the light
     \param power Intensity of the light
     \param position Relative position of the light
     \param direction Relative direction of the light
     \param cutoff Cutoff angle of the spot light
     \return light The created spot light
   */
   static SpotLight createSpotLight(const CoreI::Color & color, const CoreI::Double & power, const GeomI::Point3 & position = GeomI::Point3(), const GeomI::Vector3 & direction = GeomI::Point3(0, 0, 1), const GeomI::Angle & cutoff = 20);

   //! Create a new torus
   /*!
     \param majorRadius Major Radius
     \param minorRadius Minor Radius
     \param subdivisionLatitude Subdivision of the Torus on the Latitude
     \param subdivisionLongitude Subdivision of the Torus on the Longitude
     \return occurrence The created Occurrence
   */
   static Occurrence createTorus(const CoreI::Double & majorRadius, const CoreI::Double & minorRadius, const CoreI::Int & subdivisionLatitude, const CoreI::Int & subdivisionLongitude);

   //! Delete component from type
   /*!
     \param componentType Type of the component
     \param occurrence The occurrence to remove components from
     \param followPrototypes If true and if the component is not set on the occurrence, try to find it on its prototyping chain
   */
   static void deleteComponentByType(const ComponentType & componentType, const Occurrence & occurrence, const CoreI::Bool & followPrototypes = true);

   //! Delete all components on subtree from type
   /*!
     \param componentType Type of the component
     \param rootOccurrence The root occurrence to remove components from
   */
   static void deleteComponentsByType(const ComponentType & componentType, const Occurrence & rootOccurrence = Occurrence());

   //! Delete all empty assemblies
   /*!
     \param root Root occurrence for the process
   */
   static void deleteEmptyOccurrences(const Occurrence & root = 0);

   //! Delete a liste of occurrences
   /*!
     \param occurrences Occurrences to delete
   */
   static void deleteOccurrences(const OccurrenceList & occurrences);

   //! Find all part occurrence with a given material as active material (i.e. as seen in the rendering)
   /*!
     \param material A material
     \param roots If specified, restrict the search from the given roots
     \return occurrence Occurrences of parts with the given material as active material
   */
   static OccurrenceList findByActiveMaterial(const MaterialI::Material & material, const OccurrenceList & roots = OccurrenceList(0));

   //! Returns all occurrences which a metadata property value matches the given regular expression (ECMAScript)
   /*!
     \param property Property name
     \param regex Regular expression (ECMAScript)
     \param roots If specified, restrict the search from the given roots
     \return occurrences Occurrences which matches the given regular expression
   */
   static OccurrenceList findByMetadata(const CoreI::String & property, const CoreI::Regex & regex, const OccurrenceList & roots = OccurrenceList(0));

   //! Returns all occurrences which a property value matches the given regular expression (ECMAScript)
   /*!
     \param property Property name
     \param regex Regular expression (ECMAScript)
     \param roots If specified, restrict the search from the given roots
     \return occurrences Occurrences which matches the given regular expression
   */
   static OccurrenceList findByProperty(const CoreI::String & property, const CoreI::Regex & regex, const OccurrenceList & roots = OccurrenceList(0));

   //! find part occurences in the scene in a given axis aligned bounding box
   /*!
     \param aabb The axis aligned bounding box
     \return occurrences Occurrences found in the given axis aligned bounded box
   */
   static OccurrenceList findPartOccurrencesInAABB(const GeomI::AABB & aabb);

   /*!
     \param radius 
     \param XFrames 
     \param YFrames 
     \param hemi 
     \return occurrence The created Occurrence
   */
   static Occurrence generateOctaViews(const CoreI::Double & radius, const CoreI::Int & XFrames, const CoreI::Int & YFrames, const CoreI::Bool & hemi = false);

   //! Returns the axis aligned bounding box of a list of scene paths
   /*!
     \param occurrences List of occurrences to retrieve the AABB
     \return aabb The axis aligned bounding box of all given occurrences
   */
   static GeomI::AABB getAABB(const OccurrenceList & occurrences);

   //! Get the active material on occurrence
   /*!
     \param occurrence The occurrence
     \return material The material
   */
   static MaterialI::Material getActiveMaterial(const Occurrence & occurrence);

   //! Get the value of a property on the first parent that own it
   /*!
     \param occurrence An occurrence
     \param propertyName Property name
     \param cacheProperty If true, the property will be copied on all ancestor of occurrence below the property owner to speed up future calls
     \return value Property value
   */
   static CoreI::String getActivePropertyValue(const Occurrence & occurrence, const CoreI::String & propertyName, const CoreI::Boolean & cacheProperty = false);

   //! Get the value of a property on the first parent that own it for each given occurrence
   /*!
     \param occurrences List of occurrences
     \param propertyName Property name
     \param cacheProperty If true, the property will be copied on all ancestor of occurrence below the property owner to speed up future calls
     \return values Property value for each occurrence
   */
   static CoreI::StringList getActivePropertyValues(const OccurrenceList & occurrences, const CoreI::String & propertyName, const CoreI::Boolean & cacheProperty = false);

   //! Returns the list of the AnnotationGroup from a PMIComponent
   /*!
     \param pmiComponent The pmi component
     \return groups List of AnnotationGroup
   */
   static AnnotationGroupList getAnnotationGroups(const Component & pmiComponent);

   //! Returns the list of the Annotation from a AnnotationGroup
   /*!
     \param group The AnnotationGroup
     \return annotations List of Annotation
   */
   static AnnotationList getAnnotations(const AnnotationGroup & group);

   //! Get the children of an occurrence
   /*!
     \param occurrence The occurrence
     \return children Children occurrences
   */
   static OccurrenceList getChildren(const Occurrence & occurrence);

   //! Returns a packed version of the whole scene tree
   /*!
     \param root Specify the root of the returned scene
     \param visibilityMode The visibility mode
     \return tree 
   */
   static PackedTree getCompleteTree(const Occurrence & root = 0, const VisibilityMode & visibilityMode = VisibilityMode::Hide);

   //! Returns a component on an occurrence
   /*!
     \param occurrence The occurrence
     \param componentType Type of the component
     \param followPrototypes If true and if the component is not set on the occurrence, try to find it on its prototyping chain
     \return component The component
   */
   static Component getComponent(const Occurrence & occurrence, const ComponentType & componentType, const CoreI::Bool & followPrototypes = true);

   //! Returns one component of the specified type by occurrence if it exists
   /*!
     \param occurrences The occurrences list
     \param componentType Type of the component
     \param followPrototypes If true and if the component is not set on the occurrence, try to find it on its prototyping chain
     \return components List of component synchronized with occurrences
   */
   static ComponentList getComponentByOccurrence(const OccurrenceList & occurrences, const ComponentType & componentType, const CoreI::Bool & followPrototypes = true);

   //! Get the occurrence that own a component
   /*!
     \param component The component
     \return occurrence The occurrence
   */
   static Occurrence getComponentOccurrence(const Component & component);

   //! Get the type of a component
   /*!
     \param component The component
     \return componentType Type of the component
   */
   static ComponentType getComponentType(const Component & component);

   //! Returns the global matrix on an occurrence
   /*!
     \param occurrence Occurrence to get the global matrix
     \return matrix The global matrix of the occurrence
   */
   static GeomI::Matrix4 getGlobalMatrix(const Occurrence & occurrence);

   //! Returns the global visibility of a given occurrence
   /*!
     \param occurrence Occurrence to get the global visibility
     \return visible True if the occurrence is visible, else False
   */
   static CoreI::Boolean getGlobalVisibility(const Occurrence & occurrence);

   //! Returns the local matrix on an occurrence
   /*!
     \param occurrence Node to get the local matrix
     \return matrix The node local matrix
   */
   static GeomI::Matrix4 getLocalMatrix(const Occurrence & occurrence);

   //! Returns the Minimum Bounding Box of a list of scene paths
   /*!
     \param occurrences List of occurrences to retrieve the AABB
     \return mbb The minimum bounding box of all given occurrences
   */
   static GeomI::MBB getMBB(const OccurrenceList & occurrences);

   //! Get a metadata property value from a metadata component
   /*!
     \param metadata The metadata component
     \param name The metadata property name
     \return component The property value
   */
   static CoreI::String getMetadata(const Metadata & metadata, const CoreI::String & name);

   //! Returns definition of Metadata components
   /*!
     \param metadatas List of metadata component to retrieve definition
     \return definitions List of metadata definition for each given metadata component
   */
   static MetadataDefinitionList getMetadatasDefinitions(const MetadataList & metadatas);

   //! Returns the name of an occurrence
   /*!
     \param occurrence The occurrence to get the name
     \return name The occurrence name
   */
   static CoreI::String getNodeName(const Occurrence & occurrence);

   //! Returns the Oriented Bounding Box of a list of scene paths (works only on meshes, fast method, not the Minimum Volume Box)
   /*!
     \param occurrences List of occurrences to retrieve the AABB
     \return obb The oriented bounding box of all given occurrences
   */
   static GeomI::OBB getOBB(const OccurrenceList & occurrences);

   //! Returns the active material on a given occurrence
   /*!
     \param occurrence Occurrence to get the active material
     \return material The active material of the occurrence
   */
   static MaterialI::Material getOccurrenceActiveMaterial(const Occurrence & occurrence);

   //! Get the parent of an occurrence
   /*!
     \param occurrence The occurrence
     \return parent The parent occurrence
   */
   static Occurrence getParent(const Occurrence & occurrence);

   //! Returns the active shape of a part
   /*!
     \param part The part
     \return shape The active shape of a part
   */
   static Shape getPartActiveShape(const Part & part);

   //! Recursively get all the occurrences containing a part component
   /*!
     \param from Source occurrence of the recursion
     \return occurrences Result occurrences
   */
   static OccurrenceList getPartOccurrences(const Occurrence & from = Occurrence());

   //! Returns the number of polygon in the parts meshes
   /*!
     \param occurrences The part occurrences
     \param asTriangleCount If true count the equivalent of triangles for each polygon
     \param countOnceEachInstance If true ignore multiple instance of each tessellation
     \param countHidden If true, also count hidden components
     \return polygonCount The number of polygons
   */
   static CoreI::Int getPolygonCount(const OccurrenceList & occurrences, const CoreI::Bool & asTriangleCount = false, const CoreI::Bool & countOnceEachInstance = false, const CoreI::Bool & countHidden = false);

   //! Returns all the occurrences prototyping the given occurrence
   /*!
     \param prototype The prototype occurrence
     \return referencers The referencers occurrences
   */
   static OccurrenceList getReferencers(const Occurrence & prototype);

   //! Get the root occurrence of the product structure
   /*!
     \return root The root occurrence
   */
   static Occurrence getRoot();

   //! Returns some stats of a sub tree
   /*!
     \param root The root of the sub tree
     \return partCount Number of parts in the sub-tree (instances are counted once)
     \return partOccurrenceCount Number of part occurrence in the sub-tree (instances are counted multiple times)
     \return triangleCount Number of triangles in the sub-tree (instances are counted once, quadrangle count for 2 triangles)
     \return triangleOccurrenceCount Number of triangles in the sub-tree (instances are counted multiples times, quadrangle count for 2 triangles)
     \return vertexCount Number of surfacic vertices in the sub-tree (instances are counted once)
     \return vertexOccurrenceCount Number of surfacic vertices in the sub-tree (instances are counted multiples times)
   */
   static getSubTreeStatsReturn getSubTreeStats(const Occurrence & root);

   //! Returns the number of vertices in the parts meshes
   /*!
     \param occurrences The part occurrences
     \param countOnceEachInstance If true ignore multiple instance of each tessellation
     \param countHidden If true, also count hidden components
     \param countPoints If true, also count points (for points cloud)
     \param countMergedVertices If true count all merged vertices in each tessellation
     \return vertexCount The number of vertices
   */
   static CoreI::Int getVertexCount(const OccurrenceList & occurrences, const CoreI::Bool & countOnceEachInstance = false, const CoreI::Bool & countHidden = false, const CoreI::Bool & countPoints = false, const CoreI::Bool & countMergedVertices = false);

   //! Returns viewpoints from model cavities
   /*!
     \param voxelSize Precision for cavities detection
     \param minCavityVolume Minimum volume for a cavity to be returned
     \return positions List of viewpoint positions
     \return directions List of viewpoint directions
   */
   static getViewpointsFromCavitiesReturn getViewpointsFromCavities(const GeomI::Distance & voxelSize, const GeomI::Distance & minCavityVolume);

   //! Returns True if the given occurrence has the given component type
   /*!
     \param occurrence The occurrence
     \param componentType Type of the component
     \param followPrototypes If true and if the component is not set on the occurrence, try to find it on its prototyping chain
     \return hasComp 
   */
   static CoreI::Boolean hasComponent(const Occurrence & occurrence, const ComponentType & componentType, const CoreI::Bool & followPrototypes = true);

   //! Hide the given occurrence
   /*!
     \param occurrence The occurrence to hide
   */
   static void hide(const Occurrence & occurrence);

   //! Create the default light
   static void insertDefaultLightsInTree();

   //! List all components on a type on the whole tree
   /*!
     \param componentType The component type
     \return components The component list of the type chosen
   */
   static ComponentList listComponent(const ComponentType & componentType);

   //! List all components on an occurrence
   /*!
     \param occurrence The occurrence to list the components
     \param followPrototypes If true list also components owned by the prototype
     \return components The components owned by the occurrence
   */
   static ComponentList listComponents(const Occurrence & occurrence, const CoreI::Bool & followPrototypes = true);

   //! list all the materials used in the part shape
   /*!
     \param part The part which contains sub materials
     \return materials Used materials
   */
   static MaterialI::MaterialList listPartSubMaterials(const Part & part);

   //! Merge all equivalent images (i.e. with same pixels)
   /*!
     \return nbTexture The number of texture after the merge
   */
   static CoreI::Int mergeImages();

   //! Merge all equivalent materials (i.e. with same appearance)
   /*!
     \return nbMaterial The number of material after the merge
   */
   static CoreI::Int mergeMaterials();

   //! Move an occurrence, adjusting the transformation to keep objects at the same place in the world space
   /*!
     \param occurrences The occurrences to move
     \param destination Destination occurrence (the new parent)
   */
   static void moveOccurrences(const OccurrenceList & occurrences, const Occurrence & destination);

   //! Remove a property from a metadata
   /*!
     \param metadata The occurrence
     \param name The name of the property
   */
   static void removeMetadata(const Metadata & metadata, const CoreI::String & name);

   //! truncate names of occurrence with too long names
   /*!
     \param maxLength Maximum name length
   */
   static void renameLongOccurrenceName(const CoreI::Int & maxLength);

   //! Replace a material by another everywhere it is used
   /*!
     \param originalMaterial The material to replace everywhere
     \param newMaterial The new material to set in place of originalMaterial
     \param occurrences The occurrences on which replacing the materials
   */
   static void replaceMaterial(const MaterialI::Material & originalMaterial, const MaterialI::Material & newMaterial, const OccurrenceList & occurrences = OccurrenceList(0));

   //! Resizes the textures from a selection of occurrences (resizes all textures used by these occurrences), or from a selection of textures
   /*!
     \param inputMode Defines if the textures to resize are textures used by a selection of Occurrences, or a selection among the textures available in the scene
     \param resizeMode Defines if the textures are resized following a ratio or following a maximum size/resolution (only textures above the defined maximum are downsized)
     \param replaceTextures If True, overwrites textures from the selection
   */
   static void resizeTextures(const ResizeTexturesInputMode & inputMode, const ResizeTexturesResizeMode & resizeMode, const CoreI::Bool & replaceTextures);

   //! Selects occurrences for which the property "Material" is the given material
   /*!
     \param material A material
   */
   static void selectByMaterial(const MaterialI::Material & material);

   //! Selects parts for which the given material is visible in the viewer
   /*!
     \param material A material
   */
   static void selectByVisibleMaterial(const MaterialI::Material & material);

   //! find part occurences in the scene in a given box and add them to the selection
   /*!
     \param box The extension box
     \param strictlyIncludes If false, parts only need to intersect the box to be selected
   */
   static void selectPartOccurrencesInBox(const GeomI::ExtendedBox & box, const CoreI::Boolean & strictlyIncludes);

   //! Move a component to an occurrence
   /*!
     \param component The component
     \param occurrence The occurrence
   */
   static void setComponentOccurrence(const Component & component, const Occurrence & occurrence);

   //! Set the default variant
   static void setDefaultVariant();

   //! Set the material on a occurrence
   /*!
     \param occurrence Occurrence to set the material
     \param material The new occurrence material
   */
   static void setOccurrenceMaterial(const Occurrence & occurrence, const MaterialI::Material & material);

   //! Set the parent of an occurrence
   /*!
     \param occurrence The occurrence
     \param parent The parent occurrence
     \param addInParentInstances If True, each occurrence whose prototype is the target parent will generate a child whose prototype is the occurrence itself
     \param insertBefore Add before this child occurrence in the children list of the parent occurrence
   */
   static void setParent(const Occurrence & occurrence, const Occurrence & parent, const CoreI::Boolean & addInParentInstances = false, const Occurrence & insertBefore = Occurrence());

   //! Show the given occurrence
   /*!
     \param occurrence The occurrence to show
   */
   static void show(const Occurrence & occurrence);

   //! Show only the given occurrence
   /*!
     \param occurrence The occurrence to show
   */
   static void showOnly(const Occurrence & occurrence);

   //! Updates the designed ray prober
   /*!
     \param proberID The ray prober Id
     \param matrix The new ray prober matrix
   */
   static void updateRayProber(const CoreI::Ident & proberID, const GeomI::Matrix4 & matrix);

   //! Updates the designed sphere prober
   /*!
     \param proberID The sphere prober Id
     \param sphereCenter The new prober center
     \param sphereRadius The new prober radius
   */
   static void updateSphereProber(const CoreI::Ident & proberID, const GeomI::Vector3 & sphereCenter, const CoreI::Double & sphereRadius);


   /**
    * \defgroup LODs Levels of detail management related functions
    * @{
    */
   /**@}*/

   /**
    * \defgroup OoC Out of Core related functions
    * @{
    */
   /**@}*/

   /**
    * \defgroup alternative trees AlternativeTree related functions
    * @{
    */
   //! Create a new alternative tree
   /*!
     \param name The name of the new alternative tree
     \param root The root occurrence
     \return tree The new alternative tree
   */
   static AlternativeTree createAlternativeTree(const CoreI::String & name, const Occurrence & root = Occurrence());

   //! Returns the root occurrence associated with the given AlternativeTree
   /*!
     \param tree Targeted alternative tree
     \return root The root occurrence
   */
   static Occurrence getAlternativeTreeRoot(const AlternativeTree & tree);

   //! Returns all the available alternative trees
   /*!
     \return trees All alternative trees
   */
   static AlternativeTreeList listAlternativeTrees();

   /**@}*/

   /**
    * \defgroup animations Animations related functions
    * @{
    */
   //! Adds a keyframe in the curve
   /*!
     \param channel The channel one wants to add a keyframe in
     \param time The time
     \param value The value
     \return keyframe The corresponding keyframe
   */
   static Keyframe addKeyframe(const AnimChannel & channel, const AnimationTime & time, const CoreI::Double & value);

   //! Adds keyframes in a given AnimChannel based on current position
   /*!
     \param channel The channel one wants to add a keyframe in
     \param time The time
   */
   static void addKeyframeFromCurrentPosition(const AnimChannel & channel, const AnimationTime & time);

   //! Does this Animation animates this Occurrence - or one of its parents (thus animating it indirectly) ?
   /*!
     \param animation The Animation
     \param occurrence The supposingly animated occurrence
     \return isAnimated The answer to this question
   */
   static CoreI::Bool animatesThisOccurrence(const Animation & animation, const Occurrence & occurrence);

   //! Baking soda
   /*!
     \param animation The Animation
     \param occurrence The occurrence
     \param end The parent occurrence
     \param intervall The intervall
   */
   static void bakeAnimation(const Animation & animation, const Occurrence & occurrence, const Occurrence & end, const AnimationTime & intervall);

   //! Creates an animation
   /*!
     \param name Name of the animation
     \return animation The created animation
   */
   static Animation createAnimation(const CoreI::String & name);

   //! Create a skeleton mesh from a joint component tree
   /*!
     \param root Root joint component node
   */
   static void createSkeletonMesh(const Occurrence & root);

   //! Decimates by segment a given AnimChannel
   /*!
     \param channel The channel
     \param precision The precision
   */
   static void decimateAnimChannelBySegment(const AnimChannel & channel, const CoreI::Double & precision);

   //! Deletes an animation
   /*!
     \param animation The created animation
   */
   static void deleteAnimation(const Animation & animation);

   //! Displays info on the selected AnimChannel
   /*!
     \param channel The channel
   */
   static void displayAllKeyframesFromAnimChannel(const AnimChannel & channel);

   //! Displays info on the selected animation
   /*!
     \param animation The animation
   */
   static void displayAllKeyframesFromAnimation(const Animation & animation);

   //! Displays the value
   /*!
     \param channel The channel
     \param time The time
     \param defaultValue Show default instead ?
   */
   static void displayValueFromAnimChannelAtTime(const AnimChannel & channel, const AnimationTime & time, const CoreI::Bool & defaultValue = false);

   //! Returns the main AnimChannel of an Occurrence according to a given Animation
   /*!
     \param animation The Animation
     \param occurrence The Occurrence
     \return channel The channel
   */
   static AnimChannel getAnimChannelIfExists(const Animation & animation, const Occurrence & occurrence);

   //! Returns the Occurrence related to a given AnimChannel
   /*!
     \param channel The channel
     \return occurrence The corresponding occurrence
   */
   static Occurrence getAnimChannelOccurrence(const AnimChannel & channel);

   //! Returns the parent AnimChannel of a given Keyframe
   /*!
     \param keyframe The keyframe one wants the parent of
     \return animChannel The Parent
   */
   static AnimChannel getKeyframeParentAnimChannel(const Keyframe & keyframe);

   //! Returns a list of all keyframes of a simple animChannel
   /*!
     \param channel The channel one wants to extract the keyframs from
     \return keyframelist The list of keyframes
   */
   static KeyframeList getKeyframes(const AnimChannel & channel);

   //! Returns the main AnimChannel of a given AnimChannel
   /*!
     \param channel The channel one wants the main of
     \return mainChannel The corresponding main Channel
   */
   static AnimChannel getMainChannel(const AnimChannel & channel);

   //! Returns the Joint assigned to an occurrence if any
   /*!
     \param occurrence The occurrence
     \return joint The joint assigned to the given occurrence
   */
   static PolygonalI::Joint getOccurrenceJoint(const Occurrence & occurrence);

   //! Returns (if exists) the parent AnimChannel of a given AnimChannel
   /*!
     \param channel The channel one wants the parent of
     \return parentChannel The corresponding parent Channel
   */
   static AnimChannel getParentChannel(const AnimChannel & channel);

   //! Returns the subchannel of a given name from an AnimChannel
   /*!
     \param channel The channel one wants the subchannel of
     \param name The name of the subchannel
     \return subChannel The corresponding subchannel
   */
   static AnimChannel getSubChannel(const AnimChannel & channel, const CoreI::String & name);

   //! Returns all the sub channel of an AnimChannel
   /*!
     \param channel The channel one wants the subchannel of
     \return subChannels The list of direct sub channels
   */
   static AnimChannelList getSubChannels(const AnimChannel & channel);

   //! Creates a Binder in an Animation stack to animate an entity's property
   /*!
     \param animation The Animation stack where to put a animated property
     \param entity The entity object to animate
     \param propertyName The name of the property to animate
     \return mainChannel The main channel of the binder
   */
   static AnimChannel linkPropertyToAnimation(const Animation & animation, const CoreI::Entity & entity, const CoreI::String & propertyName);

   //! List all Animations from the scene
   /*!
     \return animList The list containing animations
   */
   static AnimationList listAnimations();

   //! List all main AnimChannel from a given Animation
   /*!
     \param animation The Animation one wants to list the channels from
     \return channelList The list containing the AnimChannels
   */
   static AnimChannelList listMainChannels(const Animation & animation);

   //! Creates keyframes with the default values of the channel at time 0
   /*!
     \param channel The channel
   */
   static void makeDefaultKeyframe(const AnimChannel & channel);

   //! Moving animation
   /*!
     \param animation The Animation
     \param target The target occurrence
     \param newParent The new parent occurrence
     \param intervall The intervall
   */
   static void moveAnimation(const Animation & animation, const Occurrence & target, const Occurrence & newParent, const AnimationTime & intervall);

   //! Removes a keyframe in the curve
   /*!
     \param channel The channel one wants to remove a keyframe from
     \param time The time
   */
   static void removeKeyframe(const AnimChannel & channel, const AnimationTime & time);

   //! Unlinks a binder
   /*!
     \param animation The Animation stack where to put a animated property
     \param entity The entity object to animate
     \param propertyName The name of the property to animate
   */
   static void unlinkPropertyToAnimation(const Animation & animation, const CoreI::Entity & entity, const CoreI::String & propertyName);

   /**@}*/

   /**
    * \defgroup debug Debug functions
    * @{
    */
   /*!
     \return partCount The part count
     \return totalPartCount The total part count
     \return vertexCount The vertex count
     \return totalVertexCount The total vertex count
     \return edgeCount The edge count
     \return totalEdgeCount The total edge count
     \return domainCount The domain count
     \return totalDomainCount The total domain count
     \return bodyCount The body count
     \return totalBodyCount The total body count
     \return area2Dsum The 2D area sum
     \return boundaryCount The boundary count
     \return boundaryEdgeCount The boundary edge count
   */
   static getBRepInfosReturn getBRepInfos();

   /*!
     \return partCount The part count
     \return totalPartCount The total part count
     \return vertexCount The vertex count
     \return totalVertexCount The total vertex count
     \return edgeCount The edge count
     \return totalEdgeCount The total edge count
     \return polygonCount The polygon count
     \return totalPolygonCount The total polygon count
     \return patchCount The patch count
     \return totalPatchCount The total patch count
     \return boundaryCount The boundary count
     \return boundaryEdgeCount The boundary edge count
   */
   static getTessellationInfosReturn getTessellationInfos();

   //! Print an occurrence tree on log
   /*!
     \param root Occurrence tree root
   */
   static void print(const Occurrence & root = 0);

   /**@}*/

   /**
    * \defgroup events 
    * @{
    */
   /*!
     \param proberID The ray propber ID
     \param proberInfo The prober's info
   */
   static CoreI::Ident addonRayProbeCallback(void(*fp)(void *, const CoreI::Ident &, const ProberInfo &), void * userdata = nullptr);
   static void removeonRayProbeCallback(CoreI::Ident id); 

   /*!
     \param proberID The sphere propber ID
     \param proberInfo The prober's info
   */
   static CoreI::Ident addonSphereProbeCallback(void(*fp)(void *, const CoreI::Ident &, const ProberInfo &), void * userdata = nullptr);
   static void removeonSphereProbeCallback(CoreI::Ident id); 

   /**@}*/

   /**
    * \defgroup filters Scene filtering functions
    * @{
    */
   //! Add a filter to the filters library
   /*!
     \param name Name of the filter
     \param expr The filter expression
     \return filterId Identifier of the created filter
   */
   static CoreI::Ident addFilterToLibrary(const CoreI::String & name, const FilterExpression & expr);

   //! Evaluate the given filter expression
   /*!
     \param filter The filter expression
     \return result Result of the given expression
   */
   static CoreI::String evaluateExpression(const FilterExpression & filter);

   //! evaluate the given filter expression on all occurrences under the given occurrence and returns the result
   /*!
     \param occurrences Occurrences on which to evaluate the expression
     \param filter The filter expression
     \return evaluations The evaluation of the expression on the occurrence at the same index in given occurrences
   */
   static CoreI::StringList evaluateExpressionOnOccurrences(const OccurrenceList & occurrences, const FilterExpression & filter);

   //! evaluate the given filter expression on all occurrences under the given occurrence and returns the result
   /*!
     \param filter The filter expression
     \param from Source occurrence of the recursion
     \return occurrences 
     \return evaluations The evaluation of the expression on the occurrence at the same index in occurrences
   */
   static evaluateExpressionOnSubTreeReturn evaluateExpressionOnSubTree(const FilterExpression & filter, const Occurrence & from = Occurrence());

   //! Export filters from a given file
   /*!
     \param file File path to export
   */
   static void exportFilterLibrary(const CoreI::FilePath & file);

   //! Returns the first filter in the filter library with the given name
   /*!
     \param name Name of the filter to retrieve (case sensitive)
     \return filter The retrieved filter
   */
   static Filter findFilterByName(const CoreI::String & name);

   //! Returns the filter expression (string) from a filter id stored in the library
   /*!
     \param filterId Identifier of the filter to fetch
     \return expr Filter expression
   */
   static FilterExpression getFilterExpression(const CoreI::Ident & filterId);

   //! Retrieve a filter from the library with its identifier
   /*!
     \param filterId Identifier of the filter to retrieve
     \return filter The retrieved filter
   */
   static Filter getFilterFromLibrary(const CoreI::Ident & filterId);

   //! Recursively get all the occurrences validating the given filter expression
   /*!
     \param filter The filter expression
     \param from Source occurrence of the recursion
     \return occurrences Result occurrences
   */
   static OccurrenceList getFilteredOccurrences(const FilterExpression & filter, const Occurrence & from = Occurrence());

   //! Import filters from a given file
   /*!
     \param file File containing the filter library
   */
   static void importFilterLibrary(const CoreI::FilePath & file);

   //! Returns all the filter stored in the filter library
   /*!
     \return filters All the filters stored in the filter library
   */
   static FilterList listFilterLibrary();

   //! Remove a filter from the filters library
   /*!
     \param filterId Identifier of the filter to remove
   */
   static void removeFilterFromLibrary(const CoreI::Ident & filterId);

   /**@}*/

   /**
    * \defgroup isolate Isolate functions
    * @{
    */
   //! Enter isolate mode by isolating a subset of the scene for process, export, viewer, ...
   /*!
     \param occurrences Occurrences to isolate
   */
   static void isolate(const OccurrenceList & occurrences);

   //! Exit the isolate mode
   static void unisolate();

   /**@}*/

   /**
    * \defgroup merging Part merging related functions
    * @{
    */
   //! Merge all parts within the same area.
   /*!
     \param roots Roots occurrences for the process (will not be removed)
     \param mergeBy Number: number of output parts (or regions of parts)\nSize: diagonal size of output regions
     \param strategy Choose the regions merging strategy
     \return mergedOccurrences Resulting merged occurrences
   */
   static OccurrenceList mergeByRegions(const OccurrenceList & roots, const MergeByRegionsStrategy & mergeBy, const MergeStrategy & strategy);

   //! Merge all parts over maxLevel level
   /*!
     \param partOccurrences Occurrence of the parts to merge
     \param maxLevel Maximum tree level
     \param mergeHiddenPartsMode Hidden parts handling mode, Destroy them, make visible or merge separately
   */
   static void mergeByTreeLevel(const OccurrenceList & partOccurrences, const CoreI::Int & maxLevel, const MergeHiddenPartsMode & mergeHiddenPartsMode = MergeHiddenPartsMode::MergeSeparately);

   //! Merge final level (occurrences with only occurrence with part component as children)
   /*!
     \param roots Roots occurrences for the process (will not be removed)
     \param mergeHiddenPartsMode Hidden parts handling mode, Destroy them, make visible or merge separately
     \param CollapseToParent If true, final level unique merged part will replace it's parent
   */
   static void mergeFinalLevel(const OccurrenceList & roots = OccurrenceList(0), const MergeHiddenPartsMode & mergeHiddenPartsMode = MergeHiddenPartsMode::MergeSeparately, const CoreI::Boolean & CollapseToParent = false);

   //! Merge a set of parts
   /*!
     \param partOccurrences Occurrence of the parts to merge
     \param mergeHiddenPartsMode Hidden parts handling mode, Destroy them, make visible or merge separately
     \return mergedOccurrences Resulting merged occurrences
   */
   static OccurrenceList mergeParts(const OccurrenceList & partOccurrences, const MergeHiddenPartsMode & mergeHiddenPartsMode = MergeHiddenPartsMode::MergeSeparately);

   //! Merge all parts under each assembly together
   /*!
     \param roots Roots occurrences for the process (will not be removed)
     \param mergeHiddenPartsMode Hidden parts handling mode, Destroy them, make visible or merge separately
   */
   static void mergePartsByAssemblies(const OccurrenceList & roots = OccurrenceList(0), const MergeHiddenPartsMode & mergeHiddenPartsMode = MergeHiddenPartsMode::MergeSeparately);

   //! Merge a set of parts by materials
   /*!
     \param partOccurrences Occurrence of the parts to merge
     \param mergeNoMaterials If true, merge all parts with no active material together, else do not merge them
     \param mergeHiddenPartsMode Hidden parts handling mode, Destroy them, make visible or merge separately
     \param combineMeshes If true, explode and remerge the input parts by visible materials
     \return mergedOccurrences Resulting merged occurrences
   */
   static OccurrenceList mergePartsByMaterials(const OccurrenceList & partOccurrences, const CoreI::Boolean & mergeNoMaterials = true, const MergeHiddenPartsMode & mergeHiddenPartsMode = MergeHiddenPartsMode::MergeSeparately, const CoreI::Boolean & combineMeshes = true);

   //! Merge all parts by occurences names
   /*!
     \param root Root occurrence of the subtree to process
     \param mergeHiddenPartsMode Hidden parts handling mode, Destroy them, make visible or merge separately
   */
   static void mergePartsByName(const Occurrence & root = 0, const MergeHiddenPartsMode & mergeHiddenPartsMode = MergeHiddenPartsMode::MergeSeparately);

   //! Set all materials on part occurrences
   /*!
     \param rootOccurrence Root occurrence
   */
   static void transferCADMaterialsOnPartOccurrences(const Occurrence & rootOccurrence = Occurrence());

   //! Take the first instance material and set it one the mesh patches
   /*!
     \param rootOccurrence Root occurrence
   */
   static void transferMaterialsOnPatches(const Occurrence & rootOccurrence = Occurrence());

   /**@}*/

   /**
    * \defgroup modification 
    * @{
    */
   //! apply a transformation to the local matrix of an occurrence
   /*!
     \param occurrence Occurrence to apply the matrix on
     \param matrix Transformation to matrix
   */
   static void applyTransformation(const Occurrence & occurrence, const GeomI::Matrix4 & matrix);

   //! Create symmetries from selection
   /*!
     \param occurrences Selection of occurrences
     \param plane Symmetry plane
   */
   static void createSymmetry(const OccurrenceList & occurrences, const GeomI::AxisPlane & plane);

   //! Modify the local matrix of the scene node to apply a rotation
   /*!
     \param occurrence Occurrence to rotate
     \param axis Axis of rotation
     \param angle Angle of rotation
   */
   static void rotate(const Occurrence & occurrence, const GeomI::Vector3 & axis, const GeomI::Angle & angle);

   //! change the local matrix on an occurrence
   /*!
     \param occurrence Occurrence to set the local matrix
     \param matrix The new occurrence local matrix
   */
   static void setLocalMatrix(const Occurrence & occurrence, const GeomI::Matrix4 & matrix);

   /**@}*/

   /**
    * \defgroup part Part component functions
    * @{
    */
   //! Return the mesh of the TesselatedShape
   /*!
     \param part The part component
     \return mesh The mesh of the tessellated shape of the part
   */
   static PolygonalI::Mesh getPartMesh(const Part & part);

   //! Return the model of the BRepShape
   /*!
     \param part The part component
     \return model The model of the BRep shape of the part
   */
   static CADI::Model getPartModel(const Part & part);

   //! Return the meshes of the TesselatedShape for each given parts if any
   /*!
     \param parts The list of part component
     \return meshes The list of mesh of the tessellated shape of each part
   */
   static PolygonalI::MeshList getPartsMeshes(const PartList & parts);

   //! Return the models of the BRepShape for each given parts if any
   /*!
     \param parts The list of part component
     \return models The list of models of the BRep shape of each part
   */
   static CADI::ModelList getPartsModels(const PartList & parts);

   //! Returns the transform matrix of each given parts
   /*!
     \param parts The parts to retrieve transform
     \return transforms The transform matrix of each part
   */
   static GeomI::Matrix4List getPartsTransforms(const PartList & parts);

   //! Returns the transform matrix of each given parts (indexed mode)
   /*!
     \param parts The parts to retrieve transform
     \return indices The transform matrix index for each parts (0 for Identity)
     \return transforms The list of transform matrices (the first is always Identity)
   */
   static getPartsTransformsIndexedReturn getPartsTransformsIndexed(const PartList & parts);

   //! Add a mesh to a part (create a TessellatedShape on the part)
   /*!
     \param part The part component
     \param mesh The mesh to add to the part
   */
   static void setPartMesh(const Part & part, const PolygonalI::Mesh & mesh);

   //! Add a model to a part (create a BRepShape on the part)
   /*!
     \param part The part component
     \param model The model to add to the part
   */
   static void setPartModel(const Part & part, const CADI::Model & model);

   //! Set the transform matrix of each given parts
   /*!
     \param parts The parts to retrieve transform
     \param transforms The transform matrix of each part
   */
   static void setPartsTransforms(const PartList & parts, const GeomI::Matrix4List & transforms);

   //! Set the transform matrix of each given parts (indexed mode)
   /*!
     \param parts The parts to retrieve transform
     \param indices The transform matrix index for each parts
     \param transforms The list of transform matrices
   */
   static void setPartsTransformsIndexed(const PartList & parts, const CoreI::IntList & indices, const GeomI::Matrix4List & transforms);

   /**@}*/

   /**
    * \defgroup pivots Pivot moving functions
    * @{
    */
   //! Re-orient the Pivot Point straight to world origin (the grid)
   /*!
     \param occurrences The occurrences to modify
     \param applyToChildren If True, all the pivot of the descending occurrences from occurrence will be affected
   */
   static void alignPivotPointToWorld(const OccurrenceList & occurrences, const CoreI::Bool & applyToChildren);

   //! Move the pivot point of each occurrence listed in the function input, to the center of its bounding box (and of its children if the parameter is True)
   /*!
     \param occurrences Occurrences (or the roots occurrences if recursively=True)
     \param applyToChildren If True, all the pivot of the descending occurrences from occurrence will be affected
   */
   static void movePivotPointToOccurrenceCenter(const OccurrenceList & occurrences, const CoreI::Bool & applyToChildren);

   //! Move the pivot point of an occurrence (and its descendants if recursively) to the origin (0,0,0)
   /*!
     \param occurrence The occurrence (or the root occurrence if recursively=True)
     \param applyToChildren If True, all the pivot of the descending occurrences from occurrence will be affected
   */
   static void movePivotPointToOrigin(const Occurrence & occurrence, const CoreI::Bool & applyToChildren);

   //! Move the pivot point of all given occurrences to the center of all occurrences
   /*!
     \param occurrences The occurrences to modify
   */
   static void movePivotPointToSelectionCenter(const OccurrenceList & occurrences);

   //! Move the pivot point of each occurrence listed in the function input, to the center of the targeted occurrence Center (and of its children if the parameter is True)
   /*!
     \param occurrences The occurrence (or the root occurrence if recursively=True)
     \param target The target occurrence
     \param applyToChildren If True, all the pivot of the descending occurrences from occurrence will be affected
   */
   static void movePivotPointToTargetedOccurrenceCenter(const OccurrenceList & occurrences, const Occurrence & target, const CoreI::Bool & applyToChildren);

   //! Set the pivot of an occurrence to the given transformation matrix, the geometry will not be moved (warning: do not confuse with property Transform which actually move the occurrence)
   /*!
     \param occurrence The occurrence to move the pivot
     \param pivot The new transformation matrix for the occurrence (pivot)
   */
   static void setPivotOnly(const Occurrence & occurrence, const GeomI::Matrix4 & pivot);

   /**@}*/

   /**
    * \defgroup prototype Prototyping related functions
    * @{
    */
   //! Returns the prototype of an occurrence
   /*!
     \param occurrence The occurrence
     \return prototype The prototype (if any)
   */
   static Occurrence getPrototype(const Occurrence & occurrence);

   //! Create occurrences that prototype the given occurrence and all its subtree
   /*!
     \param prototype The root occurrence of the sub-tree to prototype
     \return occurrence The root occurrence of the prototyped sub-tree
   */
   static Occurrence prototypeSubTree(const Occurrence & prototype);

   //! Sets the prototype of an occurrence
   /*!
     \param occurrence The occurrence
     \param prototype The prototype
   */
   static void setPrototype(const Occurrence & occurrence, const Occurrence & prototype);

   /**@}*/

   /**
    * \defgroup selection Selection related functions
    * @{
    */
   //! Clear the current selection
   static void clearSelection();

   //! Delete all selected occurrences, and/or sub-occurrence elements
   static void deleteSelection();

   //! For each occurrence, create a new occurrence with the selected sub-occurrence elements and remove them from the original occurrence
   static void explodeSelection();

   //! Returns all the selected occurrences
   /*!
     \return selection The list of selected occurrences
   */
   static OccurrenceList getSelectedOccurrences();

   //! Invert the orientation of each selected item (occurrences and/or sub-occurrence elements
   static void invertOrientationSelection();

   //! Replace the selection by all unselected part occurrences
   static void invertSelection();

   //! Remove all materials appplied to the selection
   static void removeMaterials();

   //! Add occurrences to selection
   /*!
     \param occurrences Occurrences to add to the selection
   */
   static void select(const OccurrenceList & occurrences);

   //! Select all part occurrences
   static void selectAllPartOccurrences();

   //! Seperate all polygones form their original parts into a new one
   /*!
     \return newOccurrence The new occurrence created
   */
   static Occurrence separateSelection();

   //! Remove occurrences to selection
   /*!
     \param occurrence Occurrences to remove from the selection
   */
   static void unselect(const OccurrenceList & occurrence);

   /**@}*/

   /**
    * \defgroup simplification Scene structure simplification functions
    * @{
    */
   //! Compress a sub-tree by removing occurrence containing only one Child or empty, and by removing useless instances (see removeUselessInstances)
   /*!
     \param occurrence Root occurrence for the process
     \return resultingOccurrences The resulting occurences of compression
   */
   static Occurrence compress(const Occurrence & occurrence = 0);

   //! Modify the visible properties of the sub-tree to look like old school visibility (only hidden/inherited)
   /*!
     \param root Root occurrence
   */
   static void convertToOldSchoolVisibility(const Occurrence & root = 0);

   //! Get duplicated parts
   /*!
     \param root Root occurrence for the process
     \param acceptVolumeRatio If the ratio of volumes of two part is lower than acceptVolumeRatio, they will be considered duplicated
     \param acceptPolycountRatio If the ratio of polygon counts of two part is lower than acceptPolycountRatio, they will be considered duplicated
     \param acceptAABBAxisRatio If the ratio of AABB axis of two part is lower than acceptAABBAxisRatio, they will be considered duplicated
     \param acceptAABBCenterDistance If the ratio of AABB centers of two part is lower than acceptAABBCenterRatio, they will be considered duplicated
     \return duplicatedParts Duplicated part occurrences
   */
   static OccurrenceList getDuplicatedParts(const Occurrence & root = 0, const CoreI::Real & acceptVolumeRatio = 0.01, const CoreI::Real & acceptPolycountRatio = 0.1, const CoreI::Real & acceptAABBAxisRatio = 0.01, const GeomI::Distance & acceptAABBCenterDistance = 0.1);

   //! Identify parts with more than one occurrence on the scene
   /*!
     \param minOccurrenceCount Min occurrence count
   */
   static void identifyInstances(const CoreI::Int & minOccurrenceCount);

   //! Singularize all instances on the sub-tree of an occurrence
   /*!
     \param occurrences Root occurrence for the process
   */
   static void makeInstanceUnique(const OccurrenceList & occurrences = OccurrenceList(0));

   //! Set the same parent to all descending parts (all parts will be singularized)
   /*!
     \param occurrence Root occurrence for the process
     \param keepInstances If false, the part will be singularized
   */
   static void rake(const Occurrence & occurrence = 0, const CoreI::Boolean & keepInstances = false);

   //! Remove symmetry matrices (apply matrices on geometries on nodes under an occurrence with a symmetry matrix
   /*!
     \param occurrence Root occurrence for the process
   */
   static void removeSymmetryMatrices(const Occurrence & occurrence = 0);

   //! Remove instances where they are not needed (prototype referenced once, ...)
   /*!
     \param occurrence Root occurrence for the process
   */
   static void removeUselessInstances(const Occurrence & occurrence = 0);

   //! Set all part transformation matrices to identity in a sub-tree, transformation will be applied to the shapes
   /*!
     \param root Root occurrence for the process
   */
   static void resetPartTransform(const Occurrence & root = 0);

   //! Set all transformation matrices to identity in a sub-tree.
   /*!
     \param root Root occurrence for the process
     \param recursive If False, transformation will be applied only on the root and its components
     \param keepInstantiation If False, all occurrences will be singularized
     \param keepPartTransform If False, transformation will be applied to the shapes (BRepShape points or TessellatedShape vertices)
   */
   static void resetTransform(const Occurrence & root, const CoreI::Boolean & recursive = true, const CoreI::Boolean & keepInstantiation = true, const CoreI::Boolean & keepPartTransform = false);

   //! Select all parts meeting the criteria
   /*!
     \param roots Roots occurrences for the process
     \param maxDiagLength If the diagonal axis of the bouding box is less than maxDiagLength, part will be selected. -1 to ignore
     \param maxSize If the longer axis of the box is less than maxLength, part will be selected. -1 to ignore
     \param selectHidden If true, hidden parts meeting the criteria will be selected as well
   */
   static void selectByMaximumSize(const OccurrenceList & roots, const GeomI::Distance & maxDiagLength, const GeomI::Distance & maxSize = -1, const CoreI::Boolean & selectHidden = false);

   //! Select duplicated parts
   /*!
     \param acceptVolumeRatio If the ratio of volumes of two part is lower than acceptVolumeRatio, they will be considered duplicated
     \param acceptPolycountRatio If the ratio of polygon counts of two part is lower than acceptPolycountRatio, they will be considered duplicated
     \param acceptAABBAxisRatio If the ratio of AABB axis of two part is lower than acceptAABBAxisRatio, they will be considered duplicated
     \param acceptAABBCenterDistance If the ratio of AABB centers of two part is lower than acceptAABBCenterRatio, they will be considered duplicated
   */
   static void selectDuplicated(const CoreI::Real & acceptVolumeRatio = 0.01, const CoreI::Real & acceptPolycountRatio = 0.1, const CoreI::Real & acceptAABBAxisRatio = 0.01, const GeomI::Distance & acceptAABBCenterDistance = 0.1);

   //! Select occurrences sharing the same prototype as the given one
   /*!
     \param occurrence Reference part occurrence
   */
   static void selectInstances(const Occurrence & occurrence);

   //! Select hidden parts
   static void selectPartsFromNoShow();

   /**@}*/

   /**
    * \defgroup spatialRequest 
    * @{
    */
   /*!
     \param ray The ray to cast
     \param root The root occurrence to cast from
     \return hit Information of the first ray hit
   */
   static RayHit rayCast(const GeomI::Ray & ray, const Occurrence & root);

   /*!
     \param ray The ray to cast
     \param root The root occurrence to cast from
     \return hits Information of the first ray hit
   */
   static RayHitList rayCastAll(const GeomI::Ray & ray, const Occurrence & root);

   /**@}*/

   /**
    * \defgroup variant variant related functions
    * @{
    */
   //! Create a new variant
   /*!
     \param name The name of the new variant
     \return variant The new variant
   */
   static Variant addVariant(const CoreI::String & name);

   //! Create a new variant which is a copy of an existing variant
   /*!
     \param variant The variant to duplicated
     \param name Name of the new variant
     \return newVariant The created variant
   */
   static Variant duplicateVariant(const Variant & variant, const CoreI::String & name);

   //! Returns the definitions of multiple variant components
   /*!
     \param variantComponents The list of variant components to retrieve definitions
     \return definitions For each variant component, returns one list of variant definition (one by variant)
   */
   static VariantDefinitionListList getVariantComponentsDefinitions(const VariantComponentList & variantComponents);

   //! Get the alternative tree used by this variant
   /*!
     \param variant The variant
     \return tree The alternative tree used by this variant
   */
   static AlternativeTree getVariantTree(const Variant & variant);

   //! Returns all the available variants
   /*!
     \return variants All variants
   */
   static VariantList listVariants();

   //! Remove a variant
   /*!
     \param variant The variant to remove
   */
   static void removeVariant(const Variant & variant);

   //! Change the current variant used
   /*!
     \param variant The variant to enable (can be null)
   */
   static void setCurrentVariant(const Variant & variant = Variant());

   //! Set the alternative tree to use for this variant
   /*!
     \param variant The variant to modify
     \param tree The alternative tree to use for this variant
   */
   static void setVariantTree(const Variant & variant, const AlternativeTree & tree);

   /**@}*/

};

PXZ_MODULE_END



#endif
