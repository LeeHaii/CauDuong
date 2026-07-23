// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_CLOUD_INTERFACE_CLOUDINTERFACE_H_
#define _PXZ_CLOUD_INTERFACE_CLOUDINTERFACE_H_

#include "CloudTypes.h"

PXZ_MODULE_BEGIN(CloudI)

class PXZ_EXPORTED CloudInterface
{
public:
   /**
    * \defgroup AWS 
    * @{
    */
   /*!
     \param bucketName S3 bucket to download from
     \param region AWS region
     \param directoryPath Remote path to directory
     \param directory Local directory to dowmload in
   */
   static void downloadDirectoryFromS3(const CoreI::String & bucketName, const CoreI::String & region, const CoreI::String & directoryPath, const CoreI::DirectoryPath & directory);

   /*!
     \param bucketName S3 bucket to download from
     \param region AWS region
     \param filePath Remote path to file
     \param directory Local directory to dowmload in
   */
   static void downloadFileFromS3(const CoreI::String & bucketName, const CoreI::String & region, const CoreI::String & filePath, const CoreI::DirectoryPath & directory);

   /*!
     \param bucketName S3 bucket to list
     \param region AWS region
     \param prefix Prefix for filtering
     \return fileList List of files
   */
   static CoreI::StringList listFilesFromS3(const CoreI::String & bucketName, const CoreI::String & region, const CoreI::String & prefix);

   /*!
     \param bucketName S3 bucket to upload in
     \param region AWS region
     \param directoryPath Local directory
     \param directory Remote path to upload in
   */
   static void uploadDirectoryToS3(const CoreI::String & bucketName, const CoreI::String & region, const CoreI::DirectoryPath & directoryPath, const CoreI::String & directory);

   /*!
     \param bucketName S3 bucket to upload in
     \param region AWS region
     \param filePath Local file
     \param directory Remote path to upload in
   */
   static void uploadFileToS3(const CoreI::String & bucketName, const CoreI::String & region, const CoreI::FilePath & filePath, const CoreI::String & directory);

   /**@}*/

};

PXZ_MODULE_END



#endif
