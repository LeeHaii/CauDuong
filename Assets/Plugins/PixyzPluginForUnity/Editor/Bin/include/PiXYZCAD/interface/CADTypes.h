// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_CAD_INTERFACE_CADTYPES_H_
#define _PXZ_CAD_INTERFACE_CADTYPES_H_

#include <PiXYZCore/interface/InterfaceBase.h>
#include <PiXYZCore/interface/CoreTypes.h>
#include <PiXYZGeom/interface/GeomTypes.h>
#include <PiXYZMaterial/interface/MaterialTypes.h>
#include <functional> 

PXZ_MODULE_BEGIN(CADI)

typedef GeomI::GeomEntity CADEntity;

typedef CADEntity Shape;

typedef Shape Body;

typedef CoreI::List<Body> BodyList;

typedef CADEntity Curve;

typedef Curve LimitedCurve;

typedef LimitedCurve BoundedCurve;

struct Bounds1D {
   CoreI::Double min;
   CoreI::Double max;
   Bounds1D(CoreI::Double _min = 1, CoreI::Double _max = -1) : min(_min), max(_max) {}
};

struct Bounds2D {
   Bounds1D u;
   Bounds1D v;
   Bounds2D(Bounds1D _u = Bounds1D(), Bounds1D _v = Bounds1D()) : u(_u), v(_v) {}
};

typedef Curve CircleCurve;

typedef CADEntity ClosedShell;

typedef CoreI::List<ClosedShell> ClosedShellList;

typedef CADEntity CoEdge;

typedef CoreI::List<CoEdge> CoEdgeList;

typedef LimitedCurve CompositeCurve;

typedef CADEntity Surface;

typedef Surface LimitedSurface;

typedef LimitedSurface ConeSurface;

typedef LimitedSurface CurveExtrusionSurface;

typedef CoreI::List<Curve> CurveList;

typedef LimitedSurface CylinderSurface;

typedef Shape Domain;

typedef CoreI::List<Domain> DomainList;

typedef Shape Edge;

typedef CoreI::List<Edge> EdgeList;

typedef CoreI::List<EdgeList> EdgeListList;

typedef Curve EllipseCurve;

typedef LimitedSurface EllipticConeSurface;

typedef Domain Face;

typedef CoreI::List<Face> FaceList;

typedef Curve HelixCurve;

typedef LimitedCurve HermiteCurve;

typedef Curve HyperbolaCurve;

typedef LimitedCurve IntersectionCurve;

typedef CoreI::List<LimitedCurve> LimitedCurveList;

typedef Curve LineCurve;

typedef CADEntity Loop;

typedef CoreI::List<Loop> LoopList;

typedef CADEntity Model;

typedef CoreI::List<Model> ModelList;

typedef LimitedCurve NURBSCurve;

typedef LimitedSurface NURBSSurface;

typedef LimitedCurve OffsetCurve;

typedef LimitedSurface OffsetSurface;

typedef Domain OpenShell;

typedef CoreI::List<OpenShell> OpenShellList;

struct OrientedDomain {
   Domain domain;
   GeomI::Orientation orientation;
   OrientedDomain(Domain _domain = 0, GeomI::Orientation _orientation = true) : domain(_domain), orientation(_orientation) {}
};

typedef CoreI::List<OrientedDomain> OrientedDomainList;

struct OrientedEdge {
   Edge edge;
   GeomI::Orientation orientation;
   OrientedEdge(Edge _edge = 0, GeomI::Orientation _orientation = true) : edge(_edge), orientation(_orientation) {}
};

typedef CoreI::List<OrientedEdge> OrientedEdgeList;

typedef Curve ParabolaCurve;

typedef Surface PlaneSurface;

typedef LimitedCurve PolylineCurve;

typedef LimitedSurface RevolutionSurface;

typedef LimitedSurface RuledSurface;

typedef LimitedCurve SegmentCurve;

typedef LimitedSurface SphereSurface;

struct SplittedEdge {
   Edge oldEdge;
   EdgeList newEdges;
   SplittedEdge(Edge _oldEdge = 0, EdgeList _newEdges = EdgeList()) : oldEdge(_oldEdge), newEdges(_newEdges) {}
};

typedef CoreI::List<SplittedEdge> SplittedEdgeList;

typedef LimitedCurve SurfacicCurve;

typedef LimitedSurface TabulatedCylinderSurface;

typedef LimitedSurface TorusSurface;

typedef LimitedCurve TransformedCurve;

typedef Shape Vertex;

typedef CoreI::List<Vertex> VertexList;

typedef struct {
   Domain domain;
   SplittedEdgeList splittingInfo;
} buildFacesReturn;

typedef struct {
   GeomI::Point3 d0;
   GeomI::Point3 du;
   GeomI::Point3 d2u;
} evalOnCurveReturn;

typedef struct {
   GeomI::Point3 d0;
   GeomI::Point3 du;
   GeomI::Point3 dv;
   GeomI::Point3 d2u;
   GeomI::Point3 d2v;
   GeomI::Point3 duv;
} evalOnSurfaceReturn;

typedef struct {
   Curve curve;
   Bounds1D bounds;
} getBoundedCurveDefinitionReturn;

typedef struct {
   CoreI::Double radius;
   GeomI::Matrix4 matrix;
} getCircleCurveDefinitionReturn;

typedef struct {
   Edge edge;
   GeomI::Orientation edgeOrientation;
   Loop loop;
   Surface surface;
   LimitedSurface parametricCurve;
} getCoEdgeDefinitionReturn;

typedef struct {
   LimitedCurveList curves;
   CoreI::DoubleList parameters;
} getCompositeCurveDefinitionReturn;

typedef struct {
   CoreI::Double radius;
   CoreI::Double semiAngle;
   GeomI::Matrix4 matrix;
} getConeSurfaceDefinitionReturn;

typedef struct {
   LimitedCurve generatrixCurve;
   LimitedCurve directrixCruve;
   Surface surfaceReference;
} getCurveExtrusionSurfaceDefinitionReturn;

typedef struct {
   CoreI::Double radius;
   GeomI::Matrix4 matrix;
} getCylinderSurfaceDefinitionReturn;

typedef struct {
   Vertex vertex1;
   Vertex vertex2;
   Curve curve;
   Bounds1D bounds;
} getEdgeDefinitionReturn;

typedef struct {
   CoreI::Double radius1;
   CoreI::Double radius2;
   GeomI::Matrix4 matrix;
} getEllipseCurveDefinitionReturn;

typedef struct {
   CoreI::Double radius1;
   CoreI::Double radius2;
   CoreI::Double semiAngle;
   GeomI::Matrix4 matrix;
} getEllipticConeSurfaceDefinitionReturn;

typedef struct {
   Surface surface;
   LoopList loops;
   GeomI::Orientation orientation;
   Bounds2D limits;
} getFaceDefinitionReturn;

typedef struct {
   CoreI::Double radius;
   GeomI::Matrix4 matrix;
   CoreI::Boolean trigonometricOrientation;
} getHelixCurveDefinitionReturn;

typedef struct {
   GeomI::Point3 firstPoint;
   GeomI::Point3 secondPoint;
   GeomI::Point3 firstTangent;
   GeomI::Point3 secondTangent;
} getHermiteCurveDefinitionReturn;

typedef struct {
   CoreI::Double radius1;
   CoreI::Double radius2;
   GeomI::Matrix4 matrix;
} getHyperbolaCurveDefinitionReturn;

typedef struct {
   Surface surface1;
   Surface surface2;
   PolylineCurve chart;
   Bounds1D bounds;
} getIntersectionCurveDefinitionReturn;

typedef struct {
   GeomI::Point3 origin;
   GeomI::Point3 direction;
} getLineCurveDefinitionReturn;

typedef struct {
   CoreI::Int degree;
   CoreI::DoubleList knots;
   GeomI::Point3List poles;
   CoreI::DoubleList weights;
} getNURBSCurveDefinitionReturn;

typedef struct {
   CoreI::Int degreeU;
   CoreI::Int degreeV;
   CoreI::DoubleList knotsU;
   CoreI::DoubleList knotsV;
   GeomI::Point3ListList poles;
   CoreI::DoubleListList weights;
} getNURBSSurfaceDefinitionReturn;

typedef struct {
   LimitedCurve curve;
   GeomI::Point3 direction;
   CoreI::Double distance;
   Surface surfaceReference;
} getOffsetCurveDefinitionReturn;

typedef struct {
   Surface baseSurface;
   CoreI::Double distance;
} getOffsetSurfaceDefinitionReturn;

typedef struct {
   CoreI::Double focalLength;
   GeomI::Matrix4 matrix;
} getParabolaCurveDefinitionReturn;

typedef struct {
   GeomI::Point3List points;
   CoreI::DoubleList parameters;
} getPolylineCurveDefinitionReturn;

typedef struct {
   LimitedCurve generatricCurve;
   GeomI::Point3 axisOrigin;
   GeomI::Point3 axisDirection;
   CoreI::Double startAngle;
   CoreI::Double endAngle;
} getRevolutionSurfaceDefinitionReturn;

typedef struct {
   LimitedCurve firstCurve;
   LimitedCurve secondCurve;
} getRuledSurfaceDefinitionReturn;

typedef struct {
   GeomI::Point3 startPoint;
   GeomI::Point3 endPoint;
} getSegmentCurveDefinitionReturn;

typedef struct {
   CoreI::Double radius;
   GeomI::Matrix4 matrix;
} getSphereSurfaceDefinitionReturn;

typedef struct {
   Surface surface;
   LimitedCurve curve2D;
} getSurfacicCurveDefinitionReturn;

typedef struct {
   LimitedCurve directrixCurve;
   GeomI::Point3 generatrixLine;
   Bounds1D range;
} getTabulatedCylinderSurfaceDefinitionReturn;

typedef struct {
   CoreI::Double majorRadius;
   CoreI::Double minorRadius;
   GeomI::Matrix4 matrix;
} getTorusSurfaceDefinitionReturn;

typedef struct {
   LimitedCurve curve;
   GeomI::Matrix4 matrix;
} getTransformedCurveDefinitionReturn;

typedef struct {
   CoreI::Boolean periodic;
   CoreI::Double period;
} isCurvePeriodicReturn;

typedef struct {
   CoreI::Boolean closedU;
   CoreI::Boolean closedV;
} isSurfaceClosedReturn;

typedef struct {
   CoreI::Boolean periodicU;
   CoreI::Boolean periodicV;
   CoreI::Double periodU;
   CoreI::Double periodV;
} isSurfacePeriodicReturn;


PXZ_MODULE_END

#endif
