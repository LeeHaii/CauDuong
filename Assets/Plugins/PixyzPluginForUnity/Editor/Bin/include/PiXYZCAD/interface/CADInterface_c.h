// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_CAD_INTERFACE_CADINTERFACE_C_H_
#define _PXZ_CAD_INTERFACE_CADINTERFACE_C_H_

#include "CADTypes_c.h"

PXZ_EXPORTED char * CAD_getLastError();

// Set the CAD precision
PXZ_EXPORTED void CAD_setPrecision(Geom_Distance precision);

// ----------------------------------------------------
// Boolean Operators
// CSG Boolean operator
// ----------------------------------------------------

// perform boolean operation intersection on two bodies (A ^ B)
PXZ_EXPORTED CAD_BodyList CAD_solidIntersection(CAD_Body A, CAD_Body B);
// perform boolean operation substract on two bodies (A - B)
PXZ_EXPORTED CAD_BodyList CAD_solidSubstraction(CAD_Body A, CAD_Body B);
// perform boolean operation union on two bodies (A + B)
PXZ_EXPORTED CAD_BodyList CAD_solidUnion(CAD_Body A, CAD_Body B);

// ----------------------------------------------------
// curves
// Curve creation functions
// ----------------------------------------------------

// Create a Bezier curve
PXZ_EXPORTED CAD_Curve CAD_createBezierCurve(Geom_Point3List poles);
// Create a bounded curve from a curve
PXZ_EXPORTED CAD_LimitedCurve CAD_createBoundedCurve(CAD_Curve curve, Core_Double minBound, Core_Double maxBound);
// Create a new circle
PXZ_EXPORTED CAD_Curve CAD_createCircleCurve(Geom_Distance radius, Geom_Matrix4 matrix);
// Create a composite curve from a list of limited curves
PXZ_EXPORTED CAD_LimitedCurve CAD_createCompositeCurve(CAD_LimitedCurveList limitedCurveList);
// Create an ellipse curve
PXZ_EXPORTED CAD_Curve CAD_createEllipseCurve(Geom_Distance URadius, Geom_Distance VRadius, Geom_Matrix4 matrix);
// Create an helix curve
PXZ_EXPORTED CAD_Curve CAD_createHelixCurve(Geom_Distance radius, Geom_Distance pitch, Geom_Matrix4 matrix, Core_Boolean trigonometrixOrientation);
// Create a Hermite Curve
PXZ_EXPORTED CAD_LimitedCurve CAD_createHermiteCurve(Geom_Point3 FirstPoint, Geom_Point3 FirstTangent, Geom_Point3 SecondPoint, Geom_Point3 SecondTangent);
// Create an hyperBola curve
PXZ_EXPORTED CAD_Curve CAD_createHyperbolaCurve(Core_Double URadius, Core_Double VRadius, Geom_Matrix4 matrix);
// Create a Intersection Curve
PXZ_EXPORTED CAD_LimitedCurve CAD_createIntersectionCurve(CAD_Surface firstSurface, CAD_Surface secondSurface, CAD_PolylineCurve chart, Core_Double minBounds, Core_Double maxBounds);
// Create a Line Curve
PXZ_EXPORTED CAD_Curve CAD_createLineCurve(Geom_Point3 OriginPt, Geom_Point3 DirectionPt);
// Create a NURBS curve
PXZ_EXPORTED CAD_Curve CAD_createNURBSCurve(Core_Int degree, Core_DoubleList knots, Geom_Point3List poles, Core_DoubleList weights);
// Create an parabola curve
PXZ_EXPORTED CAD_Curve CAD_createParabolaCurve(Core_Double focalLength, Geom_Matrix4 matrix);
// Create a Polyline curve
PXZ_EXPORTED CAD_Curve CAD_createPolylineCurve(Geom_Point3List points);
// Create a segment curve from two given points
PXZ_EXPORTED CAD_LimitedCurve CAD_createSegmentCurve(Geom_Point3 firstPoint, Geom_Point3 secondPoint);
// Create a curve from a surface
PXZ_EXPORTED CAD_LimitedCurve CAD_createSurfacicCurve(CAD_Surface surface, CAD_LimitedCurve curve2D);
// Create a curve from a surface
PXZ_EXPORTED CAD_LimitedCurve CAD_createTransformedCurve(CAD_LimitedCurve curve, Geom_Matrix4 matrix);
// Invert a curve parametricaly
PXZ_EXPORTED CAD_Curve CAD_invertCurve(CAD_Curve curve, Core_Double precision);

// ----------------------------------------------------
// material
// ----------------------------------------------------

// get the material on a face
PXZ_EXPORTED Material_Material CAD_getFaceMaterial(CAD_Face face);
// set the material on a face
PXZ_EXPORTED void CAD_setFaceMaterial(CAD_Face face, Material_Material material);

// ----------------------------------------------------
// model management
// CAD model management functions
// ----------------------------------------------------

// Add shape to the model
PXZ_EXPORTED void CAD_addToModel(CAD_Shape shape, CAD_Model model);
// Build faces from a surface and a set of loop
typedef struct {
   CAD_Domain domain;
   CAD_SplittedEdgeList splittingInfo;
} CAD_buildFacesReturn;
PXZ_EXPORTED CAD_buildFacesReturn CAD_buildFaces(CAD_Surface surface, CAD_LoopList loopList);
// Create a new model
PXZ_EXPORTED CAD_Model CAD_createModel();
// Get all the face of a model recursively
PXZ_EXPORTED CAD_FaceList CAD_getAllModelFaces(CAD_Model model);
// Get the list of bodies contained in a model
PXZ_EXPORTED CAD_BodyList CAD_getModelBodies(CAD_Model model);
// Get boundary edges of a model grouped by cycles
PXZ_EXPORTED CAD_EdgeListList CAD_getModelBoundaries(CAD_Model model);
// Get the list of domains (Face or OpenShell) contained in a model
PXZ_EXPORTED CAD_DomainList CAD_getModelDomains(CAD_Model model);
// Get the list of free edges contained in a model
PXZ_EXPORTED CAD_EdgeList CAD_getModelEdges(CAD_Model model);
// Get the list of free vertices contained in a model
PXZ_EXPORTED CAD_VertexList CAD_getModelVertices(CAD_Model model);
// Returns the entities referencing a given CAD entity
PXZ_EXPORTED Core_EntityList CAD_getReferencers(CAD_CADEntity entity);

// ----------------------------------------------------
// structure access
// CAD structure access functions
// ----------------------------------------------------

// evaluate curvature on a curve
PXZ_EXPORTED Core_Double CAD_evalCurvatureOnCurve(CAD_Curve curve, Core_Double parameter);
// evaluate main curvatures on a surface
PXZ_EXPORTED Geom_Curvatures CAD_evalCurvatureOnSurface(CAD_Surface surface, Geom_Point2 parameter);
// evaluate a point and derivatives on a curve
typedef struct {
   Geom_Point3 d0;
   Geom_Point3 du;
   Geom_Point3 d2u;
} CAD_evalOnCurveReturn;
PXZ_EXPORTED CAD_evalOnCurveReturn CAD_evalOnCurve(CAD_Curve curve, Core_Double parameter, Core_Int derivation);
// evaluate a point and derivatives on a surface
typedef struct {
   Geom_Point3 d0;
   Geom_Point3 du;
   Geom_Point3 dv;
   Geom_Point3 d2u;
   Geom_Point3 d2v;
   Geom_Point3 duv;
} CAD_evalOnSurfaceReturn;
PXZ_EXPORTED CAD_evalOnSurfaceReturn CAD_evalOnSurface(CAD_Surface surface, Geom_Point2 parameter, Core_Int derivation);
// get all closedShells contain in the body
PXZ_EXPORTED CAD_ClosedShellList CAD_getBodyClosedShells(CAD_Body body);
// get all parameters contained in the boundedCurve
typedef struct {
   CAD_Curve curve;
   CAD_Bounds1D bounds;
} CAD_getBoundedCurveDefinitionReturn;
PXZ_EXPORTED CAD_getBoundedCurveDefinitionReturn CAD_getBoundedCurveDefinition(CAD_BoundedCurve boundedCurve);
// get all parameters contained in the circleCurve
typedef struct {
   Core_Double radius;
   Geom_Matrix4 matrix;
} CAD_getCircleCurveDefinitionReturn;
PXZ_EXPORTED CAD_getCircleCurveDefinitionReturn CAD_getCircleCurveDefinition(CAD_CircleCurve circleCurve);
// get all orienteDomains contain in the closedShell
PXZ_EXPORTED CAD_OrientedDomainList CAD_getClosedShellOrientedDomains(CAD_ClosedShell closedShell);
// get all parameters contained in the coEdge
typedef struct {
   CAD_Edge edge;
   Geom_Orientation edgeOrientation;
   CAD_Loop loop;
   CAD_Surface surface;
   CAD_LimitedSurface parametricCurve;
} CAD_getCoEdgeDefinitionReturn;
PXZ_EXPORTED CAD_getCoEdgeDefinitionReturn CAD_getCoEdgeDefinition(CAD_CoEdge coEdge);
// get all parameters contained in the compositeCurve
typedef struct {
   CAD_LimitedCurveList curves;
   Core_DoubleList parameters;
} CAD_getCompositeCurveDefinitionReturn;
PXZ_EXPORTED CAD_getCompositeCurveDefinitionReturn CAD_getCompositeCurveDefinition(CAD_CompositeCurve compositeCurve);
// get all parameters contained in the coneSurface
typedef struct {
   Core_Double radius;
   Core_Double semiAngle;
   Geom_Matrix4 matrix;
} CAD_getConeSurfaceDefinitionReturn;
PXZ_EXPORTED CAD_getConeSurfaceDefinitionReturn CAD_getConeSurfaceDefinition(CAD_ConeSurface coneSurface);
// get all parameters contained in the curveExtrusionSurface
typedef struct {
   CAD_LimitedCurve generatrixCurve;
   CAD_LimitedCurve directrixCruve;
   CAD_Surface surfaceReference;
} CAD_getCurveExtrusionSurfaceDefinitionReturn;
PXZ_EXPORTED CAD_getCurveExtrusionSurfaceDefinitionReturn CAD_getCurveExtrusionSurfaceDefinition(CAD_CurveExtrusionSurface curveExtrusionSurface);
// get the parametric space limits of a curve
PXZ_EXPORTED CAD_Bounds1D CAD_getCurveLimits(CAD_LimitedCurve curve);
// get all parameters contained in the cylinderSurface
typedef struct {
   Core_Double radius;
   Geom_Matrix4 matrix;
} CAD_getCylinderSurfaceDefinitionReturn;
PXZ_EXPORTED CAD_getCylinderSurfaceDefinitionReturn CAD_getCylinderSurfaceDefinition(CAD_CylinderSurface cylinderSurface);
// get all parameters contained in the edge
typedef struct {
   CAD_Vertex vertex1;
   CAD_Vertex vertex2;
   CAD_Curve curve;
   CAD_Bounds1D bounds;
} CAD_getEdgeDefinitionReturn;
PXZ_EXPORTED CAD_getEdgeDefinitionReturn CAD_getEdgeDefinition(CAD_Edge edge);
// get all parameters contained in the ellipseCurve
typedef struct {
   Core_Double radius1;
   Core_Double radius2;
   Geom_Matrix4 matrix;
} CAD_getEllipseCurveDefinitionReturn;
PXZ_EXPORTED CAD_getEllipseCurveDefinitionReturn CAD_getEllipseCurveDefinition(CAD_EllipseCurve ellipseCurve);
// get all parameters contained in the ellipticConeSurface
typedef struct {
   Core_Double radius1;
   Core_Double radius2;
   Core_Double semiAngle;
   Geom_Matrix4 matrix;
} CAD_getEllipticConeSurfaceDefinitionReturn;
PXZ_EXPORTED CAD_getEllipticConeSurfaceDefinitionReturn CAD_getEllipticConeSurfaceDefinition(CAD_EllipticConeSurface ellipticConeSurface);
// get all parameters contain in the face
typedef struct {
   CAD_Surface surface;
   CAD_LoopList loops;
   Geom_Orientation orientation;
   CAD_Bounds2D limits;
} CAD_getFaceDefinitionReturn;
PXZ_EXPORTED CAD_getFaceDefinitionReturn CAD_getFaceDefinition(CAD_Face face);
// get parametric definition of each face loop
PXZ_EXPORTED Geom_Point2ListList CAD_getFaceParametricBoundaries(CAD_Face face);
// get all parameters contained in the helixCurve
typedef struct {
   Core_Double radius;
   Geom_Matrix4 matrix;
   Core_Boolean trigonometricOrientation;
} CAD_getHelixCurveDefinitionReturn;
PXZ_EXPORTED CAD_getHelixCurveDefinitionReturn CAD_getHelixCurveDefinition(CAD_HelixCurve helixCurve);
// get all parameters contained in the hermiteCurve
typedef struct {
   Geom_Point3 firstPoint;
   Geom_Point3 secondPoint;
   Geom_Point3 firstTangent;
   Geom_Point3 secondTangent;
} CAD_getHermiteCurveDefinitionReturn;
PXZ_EXPORTED CAD_getHermiteCurveDefinitionReturn CAD_getHermiteCurveDefinition(CAD_HermiteCurve hermiteCurve);
// get all parameters contained in the hyperbolaCurve
typedef struct {
   Core_Double radius1;
   Core_Double radius2;
   Geom_Matrix4 matrix;
} CAD_getHyperbolaCurveDefinitionReturn;
PXZ_EXPORTED CAD_getHyperbolaCurveDefinitionReturn CAD_getHyperbolaCurveDefinition(CAD_HyperbolaCurve hyperbolaCurve);
// get all parameters contained in the intersectionCurve
typedef struct {
   CAD_Surface surface1;
   CAD_Surface surface2;
   CAD_PolylineCurve chart;
   CAD_Bounds1D bounds;
} CAD_getIntersectionCurveDefinitionReturn;
PXZ_EXPORTED CAD_getIntersectionCurveDefinitionReturn CAD_getIntersectionCurveDefinition(CAD_IntersectionCurve intersectionCurve);
// get all parameters contain in the lineCurve
typedef struct {
   Geom_Point3 origin;
   Geom_Point3 direction;
} CAD_getLineCurveDefinitionReturn;
PXZ_EXPORTED CAD_getLineCurveDefinitionReturn CAD_getLineCurveDefinition(CAD_LineCurve lineCurve);
// get all coEdges contain in the loop
PXZ_EXPORTED CAD_CoEdgeList CAD_getLoopCoEdges(CAD_Loop loop);
// get all parameters contained in the nurbsCurve
typedef struct {
   Core_Int degree;
   Core_DoubleList knots;
   Geom_Point3List poles;
   Core_DoubleList weights;
} CAD_getNURBSCurveDefinitionReturn;
PXZ_EXPORTED CAD_getNURBSCurveDefinitionReturn CAD_getNURBSCurveDefinition(CAD_NURBSCurve nurbsCurve);
// get all parameters contained in the nurbsSurface
typedef struct {
   Core_Int degreeU;
   Core_Int degreeV;
   Core_DoubleList knotsU;
   Core_DoubleList knotsV;
   Geom_Point3ListList poles;
   Core_DoubleListList weights;
} CAD_getNURBSSurfaceDefinitionReturn;
PXZ_EXPORTED CAD_getNURBSSurfaceDefinitionReturn CAD_getNURBSSurfaceDefinition(CAD_NURBSSurface nurbsSurface);
// get all parameters contained in the offsetCurve
typedef struct {
   CAD_LimitedCurve curve;
   Geom_Point3 direction;
   Core_Double distance;
   CAD_Surface surfaceReference;
} CAD_getOffsetCurveDefinitionReturn;
PXZ_EXPORTED CAD_getOffsetCurveDefinitionReturn CAD_getOffsetCurveDefinition(CAD_OffsetCurve offsetCurve);
// get all parameters contained in the offsetSurface
typedef struct {
   CAD_Surface baseSurface;
   Core_Double distance;
} CAD_getOffsetSurfaceDefinitionReturn;
PXZ_EXPORTED CAD_getOffsetSurfaceDefinitionReturn CAD_getOffsetSurfaceDefinition(CAD_OffsetSurface offsetSurface);
// get all orienteDomains contain in the openShell
PXZ_EXPORTED CAD_OrientedDomainList CAD_getOpenShellOrientedDomains(CAD_OpenShell openShell);
// get all parameters contained in the parabolaCurve
typedef struct {
   Core_Double focalLength;
   Geom_Matrix4 matrix;
} CAD_getParabolaCurveDefinitionReturn;
PXZ_EXPORTED CAD_getParabolaCurveDefinitionReturn CAD_getParabolaCurveDefinition(CAD_ParabolaCurve parabolaCurve);
// get all parameters contained in the planeSurface
PXZ_EXPORTED Geom_Matrix4 CAD_getPlaneSurfaceDefinition(CAD_PlaneSurface planeSurface);
// get all parameters contained in the polylinCurve
typedef struct {
   Geom_Point3List points;
   Core_DoubleList parameters;
} CAD_getPolylineCurveDefinitionReturn;
PXZ_EXPORTED CAD_getPolylineCurveDefinitionReturn CAD_getPolylineCurveDefinition(CAD_PolylineCurve polylineCurve);
// get all parameters contained in the revolutionSurface
typedef struct {
   CAD_LimitedCurve generatricCurve;
   Geom_Point3 axisOrigin;
   Geom_Point3 axisDirection;
   Core_Double startAngle;
   Core_Double endAngle;
} CAD_getRevolutionSurfaceDefinitionReturn;
PXZ_EXPORTED CAD_getRevolutionSurfaceDefinitionReturn CAD_getRevolutionSurfaceDefinition(CAD_RevolutionSurface revolutionSurface);
// get all parameters contained in the ruledSurface
typedef struct {
   CAD_LimitedCurve firstCurve;
   CAD_LimitedCurve secondCurve;
} CAD_getRuledSurfaceDefinitionReturn;
PXZ_EXPORTED CAD_getRuledSurfaceDefinitionReturn CAD_getRuledSurfaceDefinition(CAD_RuledSurface ruledSurface);
// get all parameters contained in the segmentCurve
typedef struct {
   Geom_Point3 startPoint;
   Geom_Point3 endPoint;
} CAD_getSegmentCurveDefinitionReturn;
PXZ_EXPORTED CAD_getSegmentCurveDefinitionReturn CAD_getSegmentCurveDefinition(CAD_SegmentCurve segmentCurve);
// get all parameters contained in the sphereSurface
typedef struct {
   Core_Double radius;
   Geom_Matrix4 matrix;
} CAD_getSphereSurfaceDefinitionReturn;
PXZ_EXPORTED CAD_getSphereSurfaceDefinitionReturn CAD_getSphereSurfaceDefinition(CAD_SphereSurface sphereSurface);
// get the parametric space limits of a surface
PXZ_EXPORTED CAD_Bounds2D CAD_getSurfaceLimits(CAD_LimitedSurface surface);
// get all parameters contained in the surfacicCurve
typedef struct {
   CAD_Surface surface;
   CAD_LimitedCurve curve2D;
} CAD_getSurfacicCurveDefinitionReturn;
PXZ_EXPORTED CAD_getSurfacicCurveDefinitionReturn CAD_getSurfacicCurveDefinition(CAD_SurfacicCurve surfacicCurve);
// get all parameters contained in the TabulatedCylinderSurface
typedef struct {
   CAD_LimitedCurve directrixCurve;
   Geom_Point3 generatrixLine;
   CAD_Bounds1D range;
} CAD_getTabulatedCylinderSurfaceDefinitionReturn;
PXZ_EXPORTED CAD_getTabulatedCylinderSurfaceDefinitionReturn CAD_getTabulatedCylinderSurfaceDefinition(CAD_TabulatedCylinderSurface tabulatedCylinderSurface);
// get all parameters contained in the torusSurface
typedef struct {
   Core_Double majorRadius;
   Core_Double minorRadius;
   Geom_Matrix4 matrix;
} CAD_getTorusSurfaceDefinitionReturn;
PXZ_EXPORTED CAD_getTorusSurfaceDefinitionReturn CAD_getTorusSurfaceDefinition(CAD_TorusSurface torusSurface);
// get all parameters contained in the transformedCurve
typedef struct {
   CAD_LimitedCurve curve;
   Geom_Matrix4 matrix;
} CAD_getTransformedCurveDefinitionReturn;
PXZ_EXPORTED CAD_getTransformedCurveDefinitionReturn CAD_getTransformedCurveDefinition(CAD_TransformedCurve transformedCurve);
// get the position of the vertex
PXZ_EXPORTED Geom_Point3 CAD_getVertexPosition(CAD_Vertex vertex);
// if the curve is closed, return true, return false otherwise
PXZ_EXPORTED Core_Boolean CAD_isCurveClosed(CAD_Curve curve);
// if the curve is periodic return true, return false otherwise
typedef struct {
   Core_Boolean periodic;
   Core_Double period;
} CAD_isCurvePeriodicReturn;
PXZ_EXPORTED CAD_isCurvePeriodicReturn CAD_isCurvePeriodic(CAD_Curve curve);
// return if the surface is closed on U or on V
typedef struct {
   Core_Boolean closedU;
   Core_Boolean closedV;
} CAD_isSurfaceClosedReturn;
PXZ_EXPORTED CAD_isSurfaceClosedReturn CAD_isSurfaceClosed(CAD_Surface surface);
// return if the surface is periodic on U or on V
typedef struct {
   Core_Boolean periodicU;
   Core_Boolean periodicV;
   Core_Double periodU;
   Core_Double periodV;
} CAD_isSurfacePeriodicReturn;
PXZ_EXPORTED CAD_isSurfacePeriodicReturn CAD_isSurfacePeriodic(CAD_Surface surface);
// project a point to a curve
PXZ_EXPORTED Core_Double CAD_projectOnCurve(CAD_Curve curve, Geom_Point3 point, Core_Double precision);
// project a point to a surface
PXZ_EXPORTED Geom_Point2 CAD_projectOnSurface(CAD_Surface surface, Geom_Point3 point, Core_Double precision);

// ----------------------------------------------------
// structure creation
// CAD structure creation functions
// ----------------------------------------------------

// Create a body from a surface
PXZ_EXPORTED CAD_Body CAD_createBody(CAD_ClosedShell outerShell, CAD_ClosedShellList innerShells);
// Create a closedShell from a set of domains of a set of orientations
PXZ_EXPORTED CAD_ClosedShell CAD_createClosedShell(CAD_DomainList domains, Geom_OrientationList orientations);
// Create an coEdge with a edge and an orientation
PXZ_EXPORTED CAD_CoEdge CAD_createCoEdge(CAD_Edge edge, Geom_Orientation orientation, CAD_Surface surface, CAD_Curve curve2D);
// Create an edge with a curve an extremity vertices
PXZ_EXPORTED CAD_Edge CAD_createEdge(CAD_Curve curve, CAD_Vertex startVertex, CAD_Vertex endVertex);
// Create an edge from a limited curve
PXZ_EXPORTED CAD_Edge CAD_createEdgeFromCurve(CAD_LimitedCurve curve);
// Create a face from a surface
PXZ_EXPORTED CAD_Face CAD_createFace(CAD_Surface surface, CAD_LoopList loopList, Core_Boolean useSurfaceOrientation);
// Create a loop from a set of edges of a set of orientations
PXZ_EXPORTED CAD_Loop CAD_createLoop(CAD_CoEdgeList coEdges, Core_Boolean check);
// Create a openShell from a set of domains of a set of orientations and set of loops
PXZ_EXPORTED CAD_OpenShell CAD_createOpenShell(CAD_DomainList domains, Geom_OrientationList orientations, CAD_LoopList loopList);
// Create a vertex from a position
PXZ_EXPORTED CAD_Vertex CAD_createVertex(Geom_Point3 position);

// ----------------------------------------------------
// surfaces
// Surface creation functions
// ----------------------------------------------------

// Create a new bezier surface
PXZ_EXPORTED CAD_Surface CAD_createBezierSurface(Core_Int degreeU, Core_Int degreeV, Geom_Point3List poles);
// Create a new cone surface
PXZ_EXPORTED CAD_Surface CAD_createConeSurface(Geom_Distance radius, Geom_Angle semiAngle, Geom_Matrix4 matrix);
// Create a new curveExtrusion surface
PXZ_EXPORTED CAD_Surface CAD_createCurveExtrusionSurface(CAD_LimitedCurve generatrixCurve, CAD_LimitedCurve directrixCurve, CAD_Surface refSurface, Core_Double precision);
// Create a new cylinder surface
PXZ_EXPORTED CAD_Surface CAD_createCylinderSurface(Geom_Distance radius, Geom_Matrix4 matrix);
// Create a new elliptic cone surface
PXZ_EXPORTED CAD_Surface CAD_createEllipticConeSurface(Geom_Distance radius1, Geom_Distance radius2, Geom_Angle semiAngle, Geom_Matrix4 matrix);
// Create a new NURBS surface
PXZ_EXPORTED CAD_Surface CAD_createNURBSSurface(Core_Int degreeU, Core_Int degreeV, Core_DoubleList knotsU, Core_DoubleList knotsV, Geom_Point3List poles, Core_DoubleList weights);
// Create a new offset surface
PXZ_EXPORTED CAD_Surface CAD_createOffsetSurface(CAD_Surface baseSurface, Core_Double distance);
// Create a new plane surface
PXZ_EXPORTED CAD_Surface CAD_createPlaneSurface(Geom_Matrix4 matrix);
// Create a new revolution surface
PXZ_EXPORTED CAD_Surface CAD_createRevolutionSurface(CAD_LimitedCurve generatrixCurve, Geom_Point3 axisOrigin, Geom_Vector3 axisDirection, Geom_Angle startAngle, Geom_Angle endAngle);
// Create a new ruled surface
PXZ_EXPORTED CAD_Surface CAD_createRuledSurface(CAD_LimitedCurve firstCurve, CAD_LimitedCurve secondCurve);
// Create a new sphere surface
PXZ_EXPORTED CAD_Surface CAD_createSphereSurface(Geom_Distance radius, Geom_Matrix4 matrix);
// Create a new tabulated cylinder surface
PXZ_EXPORTED CAD_Surface CAD_createTabulatedCylinderSurface(CAD_LimitedCurve directrixCurve, Geom_Point3 GeneratixLine, Geom_Distance minRange, Geom_Distance maxRange);
// Create a new torus surface
PXZ_EXPORTED CAD_Surface CAD_createTorusSurface(Geom_Distance radiusMax, Geom_Distance radiusMin, Geom_Matrix4 matrix);



#endif
