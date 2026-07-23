// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_CAD_INTERFACE_CADTYPES_C_H_
#define _PXZ_CAD_INTERFACE_CADTYPES_C_H_

#include <string.h>
#include <PiXYZCore/interface/InterfaceBase_c.h>
#include <PiXYZCore/interface/CoreTypes_c.h>
#include <PiXYZGeom/interface/GeomTypes_c.h>
#include <PiXYZMaterial/interface/MaterialTypes_c.h>


typedef Geom_GeomEntity CAD_CADEntity;

typedef CAD_CADEntity CAD_Shape;

typedef CAD_Shape CAD_Body;

typedef struct {
   size_t size;
   CAD_Body * list;
} CAD_BodyList;

PXZ_EXPORTED void CAD_BodyList_init(CAD_BodyList * list, size_t size = 0);
PXZ_EXPORTED void CAD_BodyList_free(CAD_BodyList * list);
typedef CAD_CADEntity CAD_Curve;

typedef CAD_Curve CAD_LimitedCurve;

typedef CAD_LimitedCurve CAD_BoundedCurve;

typedef struct {
   Core_Double min;
   Core_Double max;
} CAD_Bounds1D;

PXZ_EXPORTED void CAD_Bounds1D_init(CAD_Bounds1D * str);
PXZ_EXPORTED void CAD_Bounds1D_free(CAD_Bounds1D * str);
typedef struct {
   CAD_Bounds1D u;
   CAD_Bounds1D v;
} CAD_Bounds2D;

PXZ_EXPORTED void CAD_Bounds2D_init(CAD_Bounds2D * str);
PXZ_EXPORTED void CAD_Bounds2D_free(CAD_Bounds2D * str);
typedef CAD_Curve CAD_CircleCurve;

typedef CAD_CADEntity CAD_ClosedShell;

typedef struct {
   size_t size;
   CAD_ClosedShell * list;
} CAD_ClosedShellList;

PXZ_EXPORTED void CAD_ClosedShellList_init(CAD_ClosedShellList * list, size_t size = 0);
PXZ_EXPORTED void CAD_ClosedShellList_free(CAD_ClosedShellList * list);
typedef CAD_CADEntity CAD_CoEdge;

typedef struct {
   size_t size;
   CAD_CoEdge * list;
} CAD_CoEdgeList;

PXZ_EXPORTED void CAD_CoEdgeList_init(CAD_CoEdgeList * list, size_t size = 0);
PXZ_EXPORTED void CAD_CoEdgeList_free(CAD_CoEdgeList * list);
typedef CAD_LimitedCurve CAD_CompositeCurve;

typedef CAD_CADEntity CAD_Surface;

typedef CAD_Surface CAD_LimitedSurface;

typedef CAD_LimitedSurface CAD_ConeSurface;

typedef CAD_LimitedSurface CAD_CurveExtrusionSurface;

typedef struct {
   size_t size;
   CAD_Curve * list;
} CAD_CurveList;

PXZ_EXPORTED void CAD_CurveList_init(CAD_CurveList * list, size_t size = 0);
PXZ_EXPORTED void CAD_CurveList_free(CAD_CurveList * list);
typedef CAD_LimitedSurface CAD_CylinderSurface;

typedef CAD_Shape CAD_Domain;

typedef struct {
   size_t size;
   CAD_Domain * list;
} CAD_DomainList;

PXZ_EXPORTED void CAD_DomainList_init(CAD_DomainList * list, size_t size = 0);
PXZ_EXPORTED void CAD_DomainList_free(CAD_DomainList * list);
typedef CAD_Shape CAD_Edge;

typedef struct {
   size_t size;
   CAD_Edge * list;
} CAD_EdgeList;

PXZ_EXPORTED void CAD_EdgeList_init(CAD_EdgeList * list, size_t size = 0);
PXZ_EXPORTED void CAD_EdgeList_free(CAD_EdgeList * list);
typedef struct {
   size_t size;
   CAD_EdgeList * list;
} CAD_EdgeListList;

PXZ_EXPORTED void CAD_EdgeListList_init(CAD_EdgeListList * list, size_t size = 0);
PXZ_EXPORTED void CAD_EdgeListList_free(CAD_EdgeListList * list);
typedef CAD_Curve CAD_EllipseCurve;

typedef CAD_LimitedSurface CAD_EllipticConeSurface;

typedef CAD_Domain CAD_Face;

typedef struct {
   size_t size;
   CAD_Face * list;
} CAD_FaceList;

PXZ_EXPORTED void CAD_FaceList_init(CAD_FaceList * list, size_t size = 0);
PXZ_EXPORTED void CAD_FaceList_free(CAD_FaceList * list);
typedef CAD_Curve CAD_HelixCurve;

typedef CAD_LimitedCurve CAD_HermiteCurve;

typedef CAD_Curve CAD_HyperbolaCurve;

typedef CAD_LimitedCurve CAD_IntersectionCurve;

typedef struct {
   size_t size;
   CAD_LimitedCurve * list;
} CAD_LimitedCurveList;

PXZ_EXPORTED void CAD_LimitedCurveList_init(CAD_LimitedCurveList * list, size_t size = 0);
PXZ_EXPORTED void CAD_LimitedCurveList_free(CAD_LimitedCurveList * list);
typedef CAD_Curve CAD_LineCurve;

typedef CAD_CADEntity CAD_Loop;

typedef struct {
   size_t size;
   CAD_Loop * list;
} CAD_LoopList;

PXZ_EXPORTED void CAD_LoopList_init(CAD_LoopList * list, size_t size = 0);
PXZ_EXPORTED void CAD_LoopList_free(CAD_LoopList * list);
typedef CAD_CADEntity CAD_Model;

typedef struct {
   size_t size;
   CAD_Model * list;
} CAD_ModelList;

PXZ_EXPORTED void CAD_ModelList_init(CAD_ModelList * list, size_t size = 0);
PXZ_EXPORTED void CAD_ModelList_free(CAD_ModelList * list);
typedef CAD_LimitedCurve CAD_NURBSCurve;

typedef CAD_LimitedSurface CAD_NURBSSurface;

typedef CAD_LimitedCurve CAD_OffsetCurve;

typedef CAD_LimitedSurface CAD_OffsetSurface;

typedef CAD_Domain CAD_OpenShell;

typedef struct {
   size_t size;
   CAD_OpenShell * list;
} CAD_OpenShellList;

PXZ_EXPORTED void CAD_OpenShellList_init(CAD_OpenShellList * list, size_t size = 0);
PXZ_EXPORTED void CAD_OpenShellList_free(CAD_OpenShellList * list);
typedef struct {
   CAD_Domain domain;
   Geom_Orientation orientation;
} CAD_OrientedDomain;

PXZ_EXPORTED void CAD_OrientedDomain_init(CAD_OrientedDomain * str);
PXZ_EXPORTED void CAD_OrientedDomain_free(CAD_OrientedDomain * str);
typedef struct {
   size_t size;
   CAD_OrientedDomain * list;
} CAD_OrientedDomainList;

PXZ_EXPORTED void CAD_OrientedDomainList_init(CAD_OrientedDomainList * list, size_t size = 0);
PXZ_EXPORTED void CAD_OrientedDomainList_free(CAD_OrientedDomainList * list);
typedef struct {
   CAD_Edge edge;
   Geom_Orientation orientation;
} CAD_OrientedEdge;

PXZ_EXPORTED void CAD_OrientedEdge_init(CAD_OrientedEdge * str);
PXZ_EXPORTED void CAD_OrientedEdge_free(CAD_OrientedEdge * str);
typedef struct {
   size_t size;
   CAD_OrientedEdge * list;
} CAD_OrientedEdgeList;

PXZ_EXPORTED void CAD_OrientedEdgeList_init(CAD_OrientedEdgeList * list, size_t size = 0);
PXZ_EXPORTED void CAD_OrientedEdgeList_free(CAD_OrientedEdgeList * list);
typedef CAD_Curve CAD_ParabolaCurve;

typedef CAD_Surface CAD_PlaneSurface;

typedef CAD_LimitedCurve CAD_PolylineCurve;

typedef CAD_LimitedSurface CAD_RevolutionSurface;

typedef CAD_LimitedSurface CAD_RuledSurface;

typedef CAD_LimitedCurve CAD_SegmentCurve;

typedef CAD_LimitedSurface CAD_SphereSurface;

typedef struct {
   CAD_Edge oldEdge;
   CAD_EdgeList newEdges;
} CAD_SplittedEdge;

PXZ_EXPORTED void CAD_SplittedEdge_init(CAD_SplittedEdge * str);
PXZ_EXPORTED void CAD_SplittedEdge_free(CAD_SplittedEdge * str);
typedef struct {
   size_t size;
   CAD_SplittedEdge * list;
} CAD_SplittedEdgeList;

PXZ_EXPORTED void CAD_SplittedEdgeList_init(CAD_SplittedEdgeList * list, size_t size = 0);
PXZ_EXPORTED void CAD_SplittedEdgeList_free(CAD_SplittedEdgeList * list);
typedef CAD_LimitedCurve CAD_SurfacicCurve;

typedef CAD_LimitedSurface CAD_TabulatedCylinderSurface;

typedef CAD_LimitedSurface CAD_TorusSurface;

typedef CAD_LimitedCurve CAD_TransformedCurve;

typedef CAD_Shape CAD_Vertex;

typedef struct {
   size_t size;
   CAD_Vertex * list;
} CAD_VertexList;

PXZ_EXPORTED void CAD_VertexList_init(CAD_VertexList * list, size_t size = 0);
PXZ_EXPORTED void CAD_VertexList_free(CAD_VertexList * list);


#endif
