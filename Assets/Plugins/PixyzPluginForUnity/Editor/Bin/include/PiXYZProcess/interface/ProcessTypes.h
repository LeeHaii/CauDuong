// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_PROCESS_INTERFACE_PROCESSTYPES_H_
#define _PXZ_PROCESS_INTERFACE_PROCESSTYPES_H_

#include <PiXYZCore/interface/InterfaceBase.h>
#include <PiXYZAlgo/interface/AlgoTypes.h>
#include <PiXYZCore/interface/CoreTypes.h>
#include <PiXYZIO/interface/IOTypes.h>
#include <PiXYZScene/interface/SceneTypes.h>
#include <functional> 

PXZ_MODULE_BEGIN(ProcessI)

struct BakeDiffuseOptions {
   CoreI::Int mapResolution;
   CoreI::Int padding;
   BakeDiffuseOptions(CoreI::Int _mapResolution = 1024, CoreI::Int _padding = 1) : mapResolution(_mapResolution), padding(_padding) {}
};

struct BakeOptions {
   CoreI::Int resolution;
   CoreI::Int padding;
   AlgoI::BakeMaps textures;
   BakeOptions(CoreI::Int _resolution = 1024, CoreI::Int _padding = 1, AlgoI::BakeMaps _textures = AlgoI::BakeMaps()) : resolution(_resolution), padding(_padding), textures(_textures) {}
};

typedef CoreI::None NoBaking;

struct BakeOptionSelector {
   typedef enum {
      UNKNOWN=0,
      YES,
      NO
   } Type;
   Type _type; 
   BakeOptions Yes;
   NoBaking No;
   BakeOptionSelector() : _type(UNKNOWN) {}
   BakeOptionSelector(const BakeOptions & v) : _type(YES), Yes(v) {}
   BakeOptionSelector(const NoBaking & v) : _type(NO), No(v) {}
};
struct Orientation {
   CoreI::Boolean zUP;
   CoreI::Boolean leftHanded;
   Orientation(CoreI::Boolean _zUP = false, CoreI::Boolean _leftHanded = false) : zUP(_zUP), leftHanded(_leftHanded) {}
};

struct OrientationSelect {
   typedef enum {
      UNKNOWN=0,
      AUTOMATICORIENTATION,
      FIXORIENTATION
   } Type;
   Type _type; 
   CoreI::None automaticOrientation;
   Orientation fixOrientation;
   OrientationSelect() : _type(UNKNOWN) {}
   OrientationSelect(const CoreI::None & v) : _type(AUTOMATICORIENTATION), automaticOrientation(v) {}
   OrientationSelect(const Orientation & v) : _type(FIXORIENTATION), fixOrientation(v) {}
};
struct ScaleSelect {
   typedef enum {
      UNKNOWN=0,
      AUTOMATICSCALE,
      FIXSCALE
   } Type;
   Type _type; 
   CoreI::None automaticScale;
   CoreI::Double fixScale;
   ScaleSelect() : _type(UNKNOWN) {}
   ScaleSelect(const CoreI::None & v) : _type(AUTOMATICSCALE), automaticScale(v) {}
   ScaleSelect(const CoreI::Double & v) : _type(FIXSCALE), fixScale(v) {}
};
struct CoordinateSystemOptions {
   //! Defines how the imported model's orientation (or coordinate system) is evaluated, to be correctly converted to match Pixyz Studio's own system (right-handed, Y is the Up-axis).<br>If 'Automatic Orientation' is enabled, Pixyz Studio performs an automatic conversion.<br>Use 'Fix Orientation' if the automatic detection does not work: fill in the characteristics about the imported file's coordinate system to help Studio convert the file correctly.
   OrientationSelect orientation;
   //! Defines how the imported models' scale and units are evaluated, to be correctly converted to match Pixyz Studio's own units (mm).<br>If 'Automatic scale' is enabled, Pixyz Studio performs an automatic units conversion, and applies a corrective scale if required.<br>Use 'Fix Scale' if the automatic detection does not work: for example set a 0.1 scale value to convert a file in centimeters to millimeters.
   ScaleSelect scale;
   //! Automatically snaps/moves the imported model onto the ground of the scene (on the grid).<br>This parameter changes the imported model's original position relatively to the origin, in the upward direction (Y-axis).<br>Do not use to keep model's original absolute position.
   CoreI::Boolean snapToGround;
   //! Automatically centers the imported model to the origin of the scene.<br>This parameter changes the imported model's original position relatively to the origin, in all directions.<br>Do not use to keep model's original absolute position.
   CoreI::Boolean centerToOrigin;
   CoordinateSystemOptions(OrientationSelect _orientation = OrientationSelect(), ScaleSelect _scale = ScaleSelect(), CoreI::Boolean _snapToGround = false, CoreI::Boolean _centerToOrigin = false) : orientation(_orientation), scale(_scale), snapToGround(_snapToGround), centerToOrigin(_centerToOrigin) {}
};

struct DecimateParameters {
   GeomI::Distance surfacicTolerance;
   GeomI::Distance linearTolerance;
   GeomI::Angle normalTolerance;
   DecimateParameters(GeomI::Distance _surfacicTolerance = 1, GeomI::Distance _linearTolerance = 0.1, GeomI::Angle _normalTolerance = 5) : surfacicTolerance(_surfacicTolerance), linearTolerance(_linearTolerance), normalTolerance(_normalTolerance) {}
};

typedef CoreI::List<DecimateParameters> DecimateParametersList;

struct GenerateDiffuseMap {
   typedef enum {
      UNKNOWN=0,
      YES,
      NO
   } Type;
   Type _type; 
   BakeDiffuseOptions yes;
   NoBaking no;
   GenerateDiffuseMap() : _type(UNKNOWN) {}
   GenerateDiffuseMap(const BakeDiffuseOptions & v) : _type(YES), yes(v) {}
   GenerateDiffuseMap(const NoBaking & v) : _type(NO), no(v) {}
};
class HierarchyMode {
public:
   typedef enum {
   Full = 0,
   Compress = 1,
   Rake = 2,
   MergeAll = 3,
   MergeFinalLevel = 4
   } Type;
private:
   Type _value;
public:
   HierarchyMode(Type val = Full) : _value(val) {}
   HierarchyMode & operator=(Type val) { _value = val; return *this; }
   HierarchyMode(const CoreI::Int& val) : _value((Type)val) {}
   HierarchyMode & operator=(const CoreI::Int& val) { _value = (Type)val; return *this; }
   Type value() const { return _value; }
   operator Type() const { return _value; }
};
struct ImportOptions {
   //! If enabled, Pixyz Studio tries to orient model's faces (normals) consistently so they face the proper direction.<br>Do not use if the original's model faces orientation is already correct, as the process may destroy it.<br>If the model is not 100% properly oriented after import, use the functions from the 'Mesh' menu to fix faces orientation.
   CoreI::Boolean orientFaces;
   //! UVs applied to a mesh are used to properly display materials using texture maps.<br>If 'Preserve Original UVs' is enabled, Pixyz Studio imports the UVs included in the imported model (on any channel), and creates UVs on meshes that do not have any UVs (by a 400x400mm BoundingBox projection on channel 0).<br>If the parameter is disabled, all UVs (from any channel) are destroyed ans new ones are created (same BoundingBox projection).
   CoreI::Boolean preserveOriginalUVs;
   //! Use to remove duplicated/superimposed meshes (= parts).<br>Do not use if the imported model includes different configurations of the same model, resulting in intentionnaly duplicated parts.
   CoreI::Boolean removeDuplicatedMeshes;
   ImportOptions(CoreI::Boolean _orientFaces = false, CoreI::Boolean _preserveOriginalUVs = true, CoreI::Boolean _removeDuplicatedMeshes = true) : orientFaces(_orientFaces), preserveOriginalUVs(_preserveOriginalUVs), removeDuplicatedMeshes(_removeDuplicatedMeshes) {}
};

class QualityPreset {
public:
   typedef enum {
   VeryHigh = 0,
   High = 1,
   Medium = 2,
   Low = 3
   } Type;
private:
   Type _value;
public:
   QualityPreset(Type val = VeryHigh) : _value(val) {}
   QualityPreset & operator=(Type val) { _value = val; return *this; }
   QualityPreset(const CoreI::Int& val) : _value((Type)val) {}
   QualityPreset & operator=(const CoreI::Int& val) { _value = (Type)val; return *this; }
   Type value() const { return _value; }
   operator Type() const { return _value; }
};
struct TessellationQuality {
   //! Maximum distance between the geometry and the tessellation
   GeomI::Distance maxSag;
   //! Maximum length of elements
   GeomI::Distance maxLength;
   //! Maximum angle between normals of two adjacent elements
   GeomI::Angle maxAngle;
   TessellationQuality(GeomI::Distance _maxSag = 0.1, GeomI::Distance _maxLength = -1, GeomI::Angle _maxAngle = -1) : maxSag(_maxSag), maxLength(_maxLength), maxAngle(_maxAngle) {}
};

struct TessellationSettings {
   typedef enum {
      UNKNOWN=0,
      USEPRESET,
      USECUSTOMVALUES
   } Type;
   Type _type; 
   QualityPreset usePreset;
   TessellationQuality useCustomValues;
   TessellationSettings() : _type(UNKNOWN) {}
   TessellationSettings(const QualityPreset & v) : _type(USEPRESET), usePreset(v) {}
   TessellationSettings(const TessellationQuality & v) : _type(USECUSTOMVALUES), useCustomValues(v) {}
};

PXZ_MODULE_END

#endif
