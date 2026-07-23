// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_IO_INTERFACE_IOINTERFACE_H_
#define _PXZ_IO_INTERFACE_IOINTERFACE_H_

#include "IOTypes.h"

PXZ_MODULE_BEGIN(IOI)

class PXZ_EXPORTED IOInterface
{
public:
   /**
    * \defgroup Cloud 
    * @{
    */
   //! Export current scene to a reflect project
   /*!
     \param root Identifier of the destination occurrence
     \param sourceName Push source name
     \param uid UID of the push, overwrite old push if it's same UID
     \param keepHierarchy Keep hierarchy or rake tree
     \param configFile Use existing JSON config file, discard reflect UI prompt
   */
   static void exportSceneToReflect(const SceneI::Occurrence & root = 0, const CoreI::String & sourceName = "", const CoreI::String & uid = "", const CoreI::Boolean & keepHierarchy = false, const CoreI::FilePath & configFile = "");

   /**@}*/

   /**
    * \defgroup Debug 
    * @{
    */
   /**@}*/

   /**
    * \defgroup Import/Export external format import/export
    * @{
    */
   //! Export a file
   /*!
     \param fileName Path of the file to export
     \param root Identifier of the root occurrence to export
   */
   static void exportScene(const CoreI::OutputFilePath & fileName, const SceneI::Occurrence & root = 0);

   //! Export the selection to a file
   /*!
     \param fileName Path of the file to export
     \param keepIntermediaryNodes If true, intermerdiary hierarchy is kept
   */
   static void exportSelection(const CoreI::OutputFilePath & fileName, const CoreI::Boolean & keepIntermediaryNodes = false);

   //! Give all the format name and their extensions that can be exported in Pixyz
   /*!
     \return formats Formats that can be exported in Pixyz
   */
   static FormatList getExportFormats();

   //! Give all the format name and their extensions that can be imported in Pixyz
   /*!
     \return formats Formats that can be imported in Pixyz
   */
   static FormatList getImportFormats();

   //! Import files
   /*!
     \param fileNames List of files's paths to import
     \param root Identifier of the destination occurrence
     \return dest The root occurrences of each imported file
   */
   static SceneI::OccurrenceList importFiles(const FilesList & fileNames, const SceneI::Occurrence & root = 0);

   //! Import a picture
   /*!
     \param filename Path of the file to import
     \param root Identifier of the destination occurrence
     \return dest The root occurrence if defined, oterwise a new occurrence created by the importer
   */
   static SceneI::Occurrence importPicture(const CoreI::FilePath & filename, const SceneI::Occurrence & root = 0);

   //! Import a file
   /*!
     \param fileName Path of the file to import
     \param root Identifier of the destination occurrence
     \return dest The root occurrence if defined, oterwise a new occurrence created by the importer
   */
   static SceneI::Occurrence importScene(const CoreI::FilePath & fileName, const SceneI::Occurrence & root = 0);

   /**@}*/

};

PXZ_MODULE_END



#endif
