// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_VIEW_INTERFACE_VIEWTYPES_C_H_
#define _PXZ_VIEW_INTERFACE_VIEWTYPES_C_H_

#include <string.h>
#include <PiXYZCore/interface/InterfaceBase_c.h>
#include <PiXYZCore/interface/CoreTypes_c.h>
#include <PiXYZGeom/interface/GeomTypes_c.h>
#include <PiXYZScene/interface/SceneTypes_c.h>


typedef Core_Ident View_D3D11Interop;

typedef struct {
   Core_Boolean polygons;
   Core_Boolean breps;
   Core_Boolean wireframe;
   Core_Boolean points;
   Core_Boolean freeLines;
   Core_Boolean patchBoundaries;
} View_DrawPrimitives;

PXZ_EXPORTED void View_DrawPrimitives_init(View_DrawPrimitives * str);
PXZ_EXPORTED void View_DrawPrimitives_free(View_DrawPrimitives * str);
typedef Core_Ptr View_ID3D11Device;

typedef Core_Ptr View_ID3D11RenderTargetView;

typedef Core_Ptr View_ID3D11Resource;

typedef Core_Ptr View_ID3D11ShaderResourceView;

typedef Core_FilePath View_ImportFilePath;

typedef struct {
   Scene_OccurrenceList occurrences;
   Geom_Point3List positions;
} View_PickResult;

PXZ_EXPORTED void View_PickResult_init(View_PickResult * str);
PXZ_EXPORTED void View_PickResult_free(View_PickResult * str);
typedef Core_Int View_TextureHandle;

typedef Core_Ident View_ViewSession;

typedef enum {
 View_ViewSessionTextureType_COLOR = 0,
 View_ViewSessionTextureType_DEPTH = 1,
} View_ViewSessionTextureType;

typedef struct {
   View_ViewSessionTextureType type;
   Core_Ptr texture;
} View_ViewSessionTexture;

PXZ_EXPORTED void View_ViewSessionTexture_init(View_ViewSessionTexture * str);
PXZ_EXPORTED void View_ViewSessionTexture_free(View_ViewSessionTexture * str);
typedef struct {
   size_t size;
   View_ViewSessionTexture * list;
} View_ViewSessionTextureList;

PXZ_EXPORTED void View_ViewSessionTextureList_init(View_ViewSessionTextureList * list, size_t size = 0);
PXZ_EXPORTED void View_ViewSessionTextureList_free(View_ViewSessionTextureList * list);
typedef Core_Int View_Viewer;



#endif
