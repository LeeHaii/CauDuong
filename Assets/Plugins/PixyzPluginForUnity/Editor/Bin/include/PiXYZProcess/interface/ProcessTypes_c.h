// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_PROCESS_INTERFACE_PROCESSTYPES_C_H_
#define _PXZ_PROCESS_INTERFACE_PROCESSTYPES_C_H_

#include <string.h>
#include <PiXYZCore/interface/InterfaceBase_c.h>
#include <PiXYZAlgo/interface/AlgoTypes_c.h>
#include <PiXYZCore/interface/CoreTypes_c.h>
#include <PiXYZIO/interface/IOTypes_c.h>
#include <PiXYZScene/interface/SceneTypes_c.h>


typedef struct {
   Core_Int mapResolution;
   Core_Int padding;
} Process_BakeDiffuseOptions;

PXZ_EXPORTED void Process_BakeDiffuseOptions_init(Process_BakeDiffuseOptions * str);
PXZ_EXPORTED void Process_BakeDiffuseOptions_free(Process_BakeDiffuseOptions * str);
typedef struct {
   Core_Int resolution;
   Core_Int padding;
   Algo_BakeMaps textures;
} Process_BakeOptions;

PXZ_EXPORTED void Process_BakeOptions_init(Process_BakeOptions * str);
PXZ_EXPORTED void Process_BakeOptions_free(Process_BakeOptions * str);
typedef Core_None Process_NoBaking;

struct Process_BakeOptionSelector {
   Process_BakeOptions Yes;
   Process_NoBaking No;
   typedef enum {
      UNKNOWN=0,
      YES,
      NO
   } Type;

   Type _type; 
};

PXZ_EXPORTED void Process_BakeOptionSelector_init(Process_BakeOptionSelector * sel);
PXZ_EXPORTED void Process_BakeOptionSelector_free(Process_BakeOptionSelector * sel);

typedef struct {
   Core_Boolean zUP;
   Core_Boolean leftHanded;
} Process_Orientation;

PXZ_EXPORTED void Process_Orientation_init(Process_Orientation * str);
PXZ_EXPORTED void Process_Orientation_free(Process_Orientation * str);
struct Process_OrientationSelect {
   Core_None automaticOrientation;
   Process_Orientation fixOrientation;
   typedef enum {
      UNKNOWN=0,
      AUTOMATICORIENTATION,
      FIXORIENTATION
   } Type;

   Type _type; 
};

PXZ_EXPORTED void Process_OrientationSelect_init(Process_OrientationSelect * sel);
PXZ_EXPORTED void Process_OrientationSelect_free(Process_OrientationSelect * sel);

struct Process_ScaleSelect {
   Core_None automaticScale;
   Core_Double fixScale;
   typedef enum {
      UNKNOWN=0,
      AUTOMATICSCALE,
      FIXSCALE
   } Type;

   Type _type; 
};

PXZ_EXPORTED void Process_ScaleSelect_init(Process_ScaleSelect * sel);
PXZ_EXPORTED void Process_ScaleSelect_free(Process_ScaleSelect * sel);

typedef struct {
   Process_OrientationSelect orientation;
   Process_ScaleSelect scale;
   Core_Boolean snapToGround;
   Core_Boolean centerToOrigin;
} Process_CoordinateSystemOptions;

PXZ_EXPORTED void Process_CoordinateSystemOptions_init(Process_CoordinateSystemOptions * str);
PXZ_EXPORTED void Process_CoordinateSystemOptions_free(Process_CoordinateSystemOptions * str);
typedef struct {
   Geom_Distance surfacicTolerance;
   Geom_Distance linearTolerance;
   Geom_Angle normalTolerance;
} Process_DecimateParameters;

PXZ_EXPORTED void Process_DecimateParameters_init(Process_DecimateParameters * str);
PXZ_EXPORTED void Process_DecimateParameters_free(Process_DecimateParameters * str);
typedef struct {
   size_t size;
   Process_DecimateParameters * list;
} Process_DecimateParametersList;

PXZ_EXPORTED void Process_DecimateParametersList_init(Process_DecimateParametersList * list, size_t size = 0);
PXZ_EXPORTED void Process_DecimateParametersList_free(Process_DecimateParametersList * list);
struct Process_GenerateDiffuseMap {
   Process_BakeDiffuseOptions yes;
   Process_NoBaking no;
   typedef enum {
      UNKNOWN=0,
      YES,
      NO
   } Type;

   Type _type; 
};

PXZ_EXPORTED void Process_GenerateDiffuseMap_init(Process_GenerateDiffuseMap * sel);
PXZ_EXPORTED void Process_GenerateDiffuseMap_free(Process_GenerateDiffuseMap * sel);

typedef enum {
 Process_HierarchyMode_Full = 0,
 Process_HierarchyMode_Compress = 1,
 Process_HierarchyMode_Rake = 2,
 Process_HierarchyMode_MergeAll = 3,
 Process_HierarchyMode_MergeFinalLevel = 4,
} Process_HierarchyMode;

typedef struct {
   Core_Boolean orientFaces;
   Core_Boolean preserveOriginalUVs;
   Core_Boolean removeDuplicatedMeshes;
} Process_ImportOptions;

PXZ_EXPORTED void Process_ImportOptions_init(Process_ImportOptions * str);
PXZ_EXPORTED void Process_ImportOptions_free(Process_ImportOptions * str);
typedef enum {
 Process_QualityPreset_VeryHigh = 0,
 Process_QualityPreset_High = 1,
 Process_QualityPreset_Medium = 2,
 Process_QualityPreset_Low = 3,
} Process_QualityPreset;

typedef struct {
   Geom_Distance maxSag;
   Geom_Distance maxLength;
   Geom_Angle maxAngle;
} Process_TessellationQuality;

PXZ_EXPORTED void Process_TessellationQuality_init(Process_TessellationQuality * str);
PXZ_EXPORTED void Process_TessellationQuality_free(Process_TessellationQuality * str);
struct Process_TessellationSettings {
   Process_QualityPreset usePreset;
   Process_TessellationQuality useCustomValues;
   typedef enum {
      UNKNOWN=0,
      USEPRESET,
      USECUSTOMVALUES
   } Type;

   Type _type; 
};

PXZ_EXPORTED void Process_TessellationSettings_init(Process_TessellationSettings * sel);
PXZ_EXPORTED void Process_TessellationSettings_free(Process_TessellationSettings * sel);



#endif
