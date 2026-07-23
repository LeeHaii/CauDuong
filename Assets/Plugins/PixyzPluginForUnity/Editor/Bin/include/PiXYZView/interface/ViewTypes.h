// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_VIEW_INTERFACE_VIEWTYPES_H_
#define _PXZ_VIEW_INTERFACE_VIEWTYPES_H_

#include <PiXYZCore/interface/InterfaceBase.h>
#include <PiXYZCore/interface/CoreTypes.h>
#include <PiXYZGeom/interface/GeomTypes.h>
#include <PiXYZScene/interface/SceneTypes.h>
#include <functional> 

PXZ_MODULE_BEGIN(ViewI)

typedef CoreI::Ident D3D11Interop;

struct DrawPrimitives {
   CoreI::Boolean polygons;
   CoreI::Boolean breps;
   CoreI::Boolean wireframe;
   CoreI::Boolean points;
   CoreI::Boolean freeLines;
   CoreI::Boolean patchBoundaries;
   DrawPrimitives(CoreI::Boolean _polygons = false, CoreI::Boolean _breps = false, CoreI::Boolean _wireframe = false, CoreI::Boolean _points = false, CoreI::Boolean _freeLines = false, CoreI::Boolean _patchBoundaries = false) : polygons(_polygons), breps(_breps), wireframe(_wireframe), points(_points), freeLines(_freeLines), patchBoundaries(_patchBoundaries) {}
};

typedef CoreI::Ptr ID3D11Device;

typedef CoreI::Ptr ID3D11RenderTargetView;

typedef CoreI::Ptr ID3D11Resource;

typedef CoreI::Ptr ID3D11ShaderResourceView;

typedef CoreI::FilePath ImportFilePath;

struct PickResult {
   SceneI::OccurrenceList occurrences;
   GeomI::Point3List positions;
   PickResult(SceneI::OccurrenceList _occurrences = SceneI::OccurrenceList(), GeomI::Point3List _positions = GeomI::Point3List()) : occurrences(_occurrences), positions(_positions) {}
};

typedef CoreI::Int TextureHandle;

typedef CoreI::Ident ViewSession;

class ViewSessionTextureType {
public:
   typedef enum {
   COLOR = 0,
   DEPTH = 1
   } Type;
private:
   Type _value;
public:
   ViewSessionTextureType(Type val = COLOR) : _value(val) {}
   ViewSessionTextureType & operator=(Type val) { _value = val; return *this; }
   ViewSessionTextureType(const CoreI::Int& val) : _value((Type)val) {}
   ViewSessionTextureType & operator=(const CoreI::Int& val) { _value = (Type)val; return *this; }
   Type value() const { return _value; }
   operator Type() const { return _value; }
};
struct ViewSessionTexture {
   ViewSessionTextureType type;
   CoreI::Ptr texture;
   ViewSessionTexture(ViewSessionTextureType _type = ViewSessionTextureType::COLOR, CoreI::Ptr _texture = nullptr) : type(_type), texture(_texture) {}
};

typedef CoreI::List<ViewSessionTexture> ViewSessionTextureList;

typedef CoreI::Int Viewer;

typedef struct {
   GeomI::Matrix4List views;
   GeomI::Matrix4List projs;
   GeomI::Vector2 clipping;
} getViewerMatricesReturn;

typedef struct {
   CoreI::Int width;
   CoreI::Int height;
} getViewerSizeReturn;

typedef struct {
   SceneI::Occurrence occurrence;
   GeomI::Point3 position;
} pickReturn;


PXZ_MODULE_END

#endif
