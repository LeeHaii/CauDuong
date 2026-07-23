// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_API_INTERFACE_APIINTERFACE_H_
#define _PXZ_API_INTERFACE_APIINTERFACE_H_

#include "APITypes.h"

PXZ_MODULE_BEGIN(APII)

class PXZ_EXPORTED APIInterface
{
public:
   /*!
     \param productKey 
     \param validationKey 
     \param license 
   */
   static void initialize(const CoreI::String & productKey, const CoreI::String & validationKey, const CoreI::String & license = "");


};

PXZ_MODULE_END



#endif
