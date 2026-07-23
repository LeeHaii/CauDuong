// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_MATERIAL_INTERFACE_MATERIALINTERFACE_C_H_
#define _PXZ_MATERIAL_INTERFACE_MATERIALINTERFACE_C_H_

#include "MaterialTypes_c.h"

PXZ_EXPORTED char * Material_getLastError();

// Add a shader uniform parameter to the given custom pattern
PXZ_EXPORTED void Material_addUniformProperty(Material_CustomMaterialPattern pattern, Core_String name, Material_ShaderUniformType type);
// Remove and delete all the materials
PXZ_EXPORTED void Material_clearAllMaterials();
// Converts all the material in the scene to color materials
PXZ_EXPORTED void Material_convertAllMaterialsToColors();
// Convert a height map to a normal map
PXZ_EXPORTED Material_Image Material_convertHeightMapToNormalMap(Material_Image hmap, Core_Double height);
// Create a new custom material pattern
PXZ_EXPORTED Material_CustomMaterialPattern Material_createCustomMaterialPattern(Core_String name);
// Import an image from its raw data
PXZ_EXPORTED Material_Image Material_createImageFromDefinition(Material_ImageDefinition imageDefinition);
// Import images from their raw data
PXZ_EXPORTED Material_ImageList Material_createImagesFromDefinitions(Material_ImageDefinitionList imageDefinitions);
// Create a new material from pattern
PXZ_EXPORTED Material_Material Material_createMaterial(Core_String name, Core_String pattern);
// Create PBR material from a material definition
PXZ_EXPORTED Material_Material Material_createMaterialFromDefinition(Material_MaterialDefinition materialDefinition);
// Create PBR materials from material definitions
PXZ_EXPORTED Material_MaterialList Material_createMaterialsFromDefinitions(Material_MaterialDefinitionList materialDefinitions);
// Automatically creates PBR materials when importing PBR texture maps from a folder
PXZ_EXPORTED void Material_createMaterialsFromMaps(Core_DirectoryPath directory);
// Export an image
PXZ_EXPORTED void Material_exportImage(Material_Image image, Core_OutputFilePath filename);
// Returns the material pattern which has the given name
PXZ_EXPORTED Material_CustomMaterialPattern Material_findCustomMaterialPatternByName(Core_String name);
// Returns all materials using the given pattern
PXZ_EXPORTED Material_MaterialList Material_findMaterialsByPattern(Core_String pattern);
// Returns all materials which match a given property value
PXZ_EXPORTED Material_MaterialList Material_findMaterialsByProperty(Core_String propertyName, Core_Regex propertyValue);
// Returns all the images loaded in the current session
PXZ_EXPORTED Material_ImageList Material_getAllImages();
// Returns all the material patterns in the current session
PXZ_EXPORTED Core_StringList Material_getAllMaterialPatterns();
// Retrieve the list of all the materials in the material library
PXZ_EXPORTED Material_MaterialList Material_getAllMaterials();
// Get color material properties
PXZ_EXPORTED Material_ColorMaterialInfos Material_getColorMaterialInfos(Material_Material material);
// Returns the custom material pattern associated to the custom material
PXZ_EXPORTED Material_CustomMaterialPattern Material_getCustomMaterialPattern(Material_Material material);
// Returns the raw data of an image
PXZ_EXPORTED Material_ImageDefinition Material_getImageDefinition(Material_Image image);
// Returns the raw data of a set of images
PXZ_EXPORTED Material_ImageDefinitionList Material_getImageDefinitions(Material_ImageList images);
// Returns the size of an image
typedef struct {
   Core_Int width;
   Core_Int height;
} Material_getImageSizeReturn;
PXZ_EXPORTED Material_getImageSizeReturn Material_getImageSize(Material_Image image);
// Get impostor texture material properties
PXZ_EXPORTED Material_ImpostorMaterialInfos Material_getImpostorMaterialInfos(Material_Material material);
// Returns the properties of a PBR Material
PXZ_EXPORTED Material_MaterialDefinition Material_getMaterialDefinition(Material_Material material);
// Returns the properties of a set of PBR Materials
PXZ_EXPORTED Material_MaterialDefinitionList Material_getMaterialDefinitions(Material_MaterialList materials);
// Returns the MaterialPatternType name of the material
PXZ_EXPORTED Material_MaterialPatternType Material_getMaterialPatternType(Material_Material material);
// Get PBR  material properties
PXZ_EXPORTED Material_PBRMaterialInfos Material_getPBRMaterialInfos(Material_Material material);
// Get standard material properties
PXZ_EXPORTED Material_StandardMaterialInfos Material_getStandardMaterialInfos(Material_Material material);
// Get a shader uniform shader property type
PXZ_EXPORTED Material_ShaderUniformType Material_getUniformPropertyType(Material_CustomMaterialPattern pattern, Core_String name);
// Get unlit texture material properties
PXZ_EXPORTED Material_UnlitTextureMaterialInfos Material_getUnlitTextureMaterialInfos(Material_Material material);
// Import an image
PXZ_EXPORTED Material_Image Material_importImage(Core_FilePath filename);
// Rename materials to have a unique name for each one
PXZ_EXPORTED void Material_makeMaterialNamesUnique();
// Resize an image
PXZ_EXPORTED void Material_resizeImage(Material_Image image, Core_Int width, Core_Int height);
// Set the fragment shader of a custom pattern
PXZ_EXPORTED void Material_setFragmentShader(Material_CustomMaterialPattern pattern, Core_String code);
// Set the main color on any material pattern type
PXZ_EXPORTED void Material_setMaterialMainColor(Material_Material material, Core_ColorAlpha color);
// Sets the MaterialPattern name of the material
PXZ_EXPORTED void Material_setMaterialPattern(Material_Material material, Core_String pattern);
// Set PBR  material properties
PXZ_EXPORTED void Material_setPBRMaterialInfos(Material_Material material, Material_PBRMaterialInfos infos);
// Set the vertex shader of a custom pattern
PXZ_EXPORTED void Material_setVertexShader(Material_CustomMaterialPattern pattern, Core_String code);
// Update an image from its raw data
PXZ_EXPORTED void Material_updateImageFromDefinition(Material_Image image, Material_ImageDefinition imageDefinition);
// Update images from their raw data
PXZ_EXPORTED void Material_updateImagesFromDefinitions(Material_ImageList image, Material_ImageDefinitionList imageDefinitions);



#endif
