// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_CLOUD_INTERFACE_CLOUDINTERFACE_C_H_
#define _PXZ_CLOUD_INTERFACE_CLOUDINTERFACE_C_H_

#include "CloudTypes_c.h"

PXZ_EXPORTED char * Cloud_getLastError();

// ----------------------------------------------------
// AWS
// ----------------------------------------------------

PXZ_EXPORTED void Cloud_downloadDirectoryFromS3(Core_String bucketName, Core_String region, Core_String directoryPath, Core_DirectoryPath directory);
PXZ_EXPORTED void Cloud_downloadFileFromS3(Core_String bucketName, Core_String region, Core_String filePath, Core_DirectoryPath directory);
PXZ_EXPORTED Core_StringList Cloud_listFilesFromS3(Core_String bucketName, Core_String region, Core_String prefix);
PXZ_EXPORTED void Cloud_uploadDirectoryToS3(Core_String bucketName, Core_String region, Core_DirectoryPath directoryPath, Core_String directory);
PXZ_EXPORTED void Cloud_uploadFileToS3(Core_String bucketName, Core_String region, Core_FilePath filePath, Core_String directory);



#endif
