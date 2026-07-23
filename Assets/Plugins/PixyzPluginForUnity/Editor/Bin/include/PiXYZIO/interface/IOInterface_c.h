// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_IO_INTERFACE_IOINTERFACE_C_H_
#define _PXZ_IO_INTERFACE_IOINTERFACE_C_H_

#include "IOTypes_c.h"

PXZ_EXPORTED char * IO_getLastError();

// ----------------------------------------------------
// Cloud
// ----------------------------------------------------

// Export current scene to a reflect project
PXZ_EXPORTED void IO_exportSceneToReflect(Scene_Occurrence root, Core_String sourceName, Core_String uid, Core_Boolean keepHierarchy, Core_FilePath configFile);

// ----------------------------------------------------
// Debug
// ----------------------------------------------------


// ----------------------------------------------------
// Import/Export
// external format import/export
// ----------------------------------------------------

// Export a file
PXZ_EXPORTED void IO_exportScene(Core_OutputFilePath fileName, Scene_Occurrence root);
// Export the selection to a file
PXZ_EXPORTED void IO_exportSelection(Core_OutputFilePath fileName, Core_Boolean keepIntermediaryNodes);
// Give all the format name and their extensions that can be exported in Pixyz
PXZ_EXPORTED IO_FormatList IO_getExportFormats();
// Give all the format name and their extensions that can be imported in Pixyz
PXZ_EXPORTED IO_FormatList IO_getImportFormats();
// Import files
PXZ_EXPORTED Scene_OccurrenceList IO_importFiles(IO_FilesList fileNames, Scene_Occurrence root);
// Import a picture
PXZ_EXPORTED Scene_Occurrence IO_importPicture(Core_FilePath filename, Scene_Occurrence root);
// Import a file
PXZ_EXPORTED Scene_Occurrence IO_importScene(Core_FilePath fileName, Scene_Occurrence root);



#endif
