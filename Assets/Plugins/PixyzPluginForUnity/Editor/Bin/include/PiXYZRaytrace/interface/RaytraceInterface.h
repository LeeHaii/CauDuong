// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_RAYTRACE_INTERFACE_RAYTRACEINTERFACE_H_
#define _PXZ_RAYTRACE_INTERFACE_RAYTRACEINTERFACE_H_

#include "RaytraceTypes.h"

PXZ_MODULE_BEGIN(RaytraceI)

class PXZ_EXPORTED RaytraceInterface
{
public:
   /*!
     \param width 
     \param height 
     \param camera 
     \param outputImagePath 
   */
   static void renderImage(const CoreI::Int & width, const CoreI::Int & height, const Camera & camera, const CoreI::OutputFilePath & outputImagePath);


};

PXZ_MODULE_END



#endif
