// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_MATERIAL_INTERFACE_MATERIALINTERFACE_H_
#define _PXZ_MATERIAL_INTERFACE_MATERIALINTERFACE_H_

#include "MaterialTypes.h"

PXZ_MODULE_BEGIN(MaterialI)

class PXZ_EXPORTED MaterialInterface
{
public:
   //! Add a shader uniform parameter to the given custom pattern
   /*!
     \param pattern The custom pattern to edit
     \param name Name of the new property
     \param type Type of the new uniform
   */
   static void addUniformProperty(const CustomMaterialPattern & pattern, const CoreI::String & name, const ShaderUniformType & type);

   //! Remove and delete all the materials
   static void clearAllMaterials();

   //! Converts all the material in the scene to color materials
   static void convertAllMaterialsToColors();

   //! Convert a height map to a normal map
   /*!
     \param hmap Height map reference
     \param height Maximum height
     \return nmap Normal map
   */
   static Image convertHeightMapToNormalMap(const Image & hmap, const CoreI::Double & height = 0.5);

   //! Create a new custom material pattern
   /*!
     \param name Name of the pattern
     \return pattern The created material pattern
   */
   static CustomMaterialPattern createCustomMaterialPattern(const CoreI::String & name);

   //! Import an image from its raw data
   /*!
     \param imageDefinition The image definition
     \return image The created image
   */
   static Image createImageFromDefinition(const ImageDefinition & imageDefinition);

   //! Import images from their raw data
   /*!
     \param imageDefinitions The image definitions
     \return images The created images
   */
   static ImageList createImagesFromDefinitions(const ImageDefinitionList & imageDefinitions);

   //! Create a new material from pattern
   /*!
     \param name Name of the material
     \param pattern Name of the pattern
     \return material The created material
   */
   static Material createMaterial(const CoreI::String & name, const CoreI::String & pattern);

   //! Create PBR material from a material definition
   /*!
     \param materialDefinition The structure containing all the PBR material informations
     \return material The created material
   */
   static Material createMaterialFromDefinition(const MaterialDefinition & materialDefinition);

   //! Create PBR materials from material definitions
   /*!
     \param materialDefinitions Material definitions containing properties for each given material
     \return materials The created materials
   */
   static MaterialList createMaterialsFromDefinitions(const MaterialDefinitionList & materialDefinitions);

   //! Automatically creates PBR materials when importing PBR texture maps from a folder
   /*!
     \param directory Directory path
   */
   static void createMaterialsFromMaps(const CoreI::DirectoryPath & directory);

   //! Export an image
   /*!
     \param image Identifier of the image to export
     \param filename Filename of the file to export
   */
   static void exportImage(const Image & image, const CoreI::OutputFilePath & filename);

   //! Returns the material pattern which has the given name
   /*!
     \param name The name of the material pattern
     \return pattern The material pattern
   */
   static CustomMaterialPattern findCustomMaterialPatternByName(const CoreI::String & name);

   //! Returns all materials using the given pattern
   /*!
     \param pattern A material pattern
     \return materials Materials using the pattern
   */
   static MaterialList findMaterialsByPattern(const CoreI::String & pattern);

   //! Returns all materials which match a given property value
   /*!
     \param propertyName Name of the property to match
     \param propertyValue Regular expression to match for the property value
     \return materials Materials matching the property value
   */
   static MaterialList findMaterialsByProperty(const CoreI::String & propertyName, const CoreI::Regex & propertyValue);

   //! Returns all the images loaded in the current session
   /*!
     \return images A list containing all images identifiers
   */
   static ImageList getAllImages();

   //! Returns all the material patterns in the current session
   /*!
     \return shaders A list containing all material patterns
   */
   static CoreI::StringList getAllMaterialPatterns();

   //! Retrieve the list of all the materials in the material library
   /*!
     \return materials List of materials in the material library
   */
   static MaterialList getAllMaterials();

   //! Get color material properties
   /*!
     \param material The material to get properties
     \return infos The ColorMaterialInfos properties
   */
   static ColorMaterialInfos getColorMaterialInfos(const Material & material);

   //! Returns the custom material pattern associated to the custom material
   /*!
     \param material Custom material to get the pattern from
     \return pattern The custom material pattern
   */
   static CustomMaterialPattern getCustomMaterialPattern(const Material & material);

   //! Returns the raw data of an image
   /*!
     \param image Image's definition
     \return imageDefinition Definition of the image
   */
   static ImageDefinition getImageDefinition(const Image & image);

   //! Returns the raw data of a set of images
   /*!
     \param images The images
     \return imageDefinitions Images's definitions
   */
   static ImageDefinitionList getImageDefinitions(const ImageList & images);

   //! Returns the size of an image
   /*!
     \param image The image to get the size from
     \return width The width of the image in pixels
     \return height The height of the image in pixels
   */
   static getImageSizeReturn getImageSize(const Image & image);

   //! Get impostor texture material properties
   /*!
     \param material The material to get properties
     \return infos The getImpostorMaterialInfos properties
   */
   static ImpostorMaterialInfos getImpostorMaterialInfos(const Material & material);

   //! Returns the properties of a PBR Material
   /*!
     \param material The PBR Material
     \return materialDefinition The PBR material definition
   */
   static MaterialDefinition getMaterialDefinition(const Material & material);

   //! Returns the properties of a set of PBR Materials
   /*!
     \param materials The PBR Materials
     \return materialDefinitions The PBR Material definitions
   */
   static MaterialDefinitionList getMaterialDefinitions(const MaterialList & materials);

   //! Returns the MaterialPatternType name of the material
   /*!
     \param material The material to find the pattern
     \return patternType The pattern type of the material
   */
   static MaterialPatternType getMaterialPatternType(const Material & material);

   //! Get PBR  material properties
   /*!
     \param material The material to get properties
     \return infos The PBRMaterialInfos properties
   */
   static PBRMaterialInfos getPBRMaterialInfos(const Material & material);

   //! Get standard material properties
   /*!
     \param material The material to get properties
     \return infos The StandardMaterialInfos properties
   */
   static StandardMaterialInfos getStandardMaterialInfos(const Material & material);

   //! Get a shader uniform shader property type
   /*!
     \param pattern The custom pattern
     \param name Name of the property to get the type from
     \return type Type of the uniform property
   */
   static ShaderUniformType getUniformPropertyType(const CustomMaterialPattern & pattern, const CoreI::String & name);

   //! Get unlit texture material properties
   /*!
     \param material The material to get properties
     \return infos The UnlitTextureMaterialInfos properties
   */
   static UnlitTextureMaterialInfos getUnlitTextureMaterialInfos(const Material & material);

   //! Import an image
   /*!
     \param filename Filename of the image to import
     \return image Identifier of the imported image
   */
   static Image importImage(const CoreI::FilePath & filename);

   //! Rename materials to have a unique name for each one
   static void makeMaterialNamesUnique();

   //! Resize an image
   /*!
     \param image Image to be resize
     \param width New image width
     \param height New image height
   */
   static void resizeImage(const Image & image, const CoreI::Int & width, const CoreI::Int & height);

   //! Set the fragment shader of a custom pattern
   /*!
     \param pattern The custom pattern to edit
     \param code The GLSL code of the fragment shader
   */
   static void setFragmentShader(const CustomMaterialPattern & pattern, const CoreI::String & code);

   //! Set the main color on any material pattern type
   /*!
     \param material The material to apply the color on
     \param color The color to apply
   */
   static void setMaterialMainColor(const Material & material, const CoreI::ColorAlpha & color);

   //! Sets the MaterialPattern name of the material
   /*!
     \param material The material to find the pattern
     \param pattern The pattern of the material
   */
   static void setMaterialPattern(const Material & material, const CoreI::String & pattern);

   //! Set PBR  material properties
   /*!
     \param material The material to set properties
     \param infos The PBRMaterialInfos properties
   */
   static void setPBRMaterialInfos(const Material & material, const PBRMaterialInfos & infos);

   //! Set the vertex shader of a custom pattern
   /*!
     \param pattern The custom pattern to edit
     \param code The GLSL code of the vertex shader
   */
   static void setVertexShader(const CustomMaterialPattern & pattern, const CoreI::String & code);

   //! Update an image from its raw data
   /*!
     \param image The image to update
     \param imageDefinition The new data to apply
   */
   static void updateImageFromDefinition(const Image & image, const ImageDefinition & imageDefinition);

   //! Update images from their raw data
   /*!
     \param image The image to update
     \param imageDefinitions The new data to apply
   */
   static void updateImagesFromDefinitions(const ImageList & image, const ImageDefinitionList & imageDefinitions);


};

PXZ_MODULE_END



#endif
