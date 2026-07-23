// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_CAD_INTERFACE_CADINTERFACE_H_
#define _PXZ_CAD_INTERFACE_CADINTERFACE_H_

#include "CADTypes.h"

PXZ_MODULE_BEGIN(CADI)

class PXZ_EXPORTED CADInterface
{
public:
   //! Set the CAD precision
   /*!
     \param precision CAD precision
   */
   static void setPrecision(const GeomI::Distance & precision);


   /**
    * \defgroup Boolean Operators CSG Boolean operator
    * @{
    */
   //! perform boolean operation intersection on two bodies (A ^ B)
   /*!
     \param A The first body
     \param B The second body
     \return result List of resulting bodies
   */
   static BodyList solidIntersection(const Body & A, const Body & B);

   //! perform boolean operation substract on two bodies (A - B)
   /*!
     \param A The first body
     \param B The second body
     \return result List of resulting bodies
   */
   static BodyList solidSubstraction(const Body & A, const Body & B);

   //! perform boolean operation union on two bodies (A + B)
   /*!
     \param A The first body
     \param B The second body
     \return result List of resulting bodies
   */
   static BodyList solidUnion(const Body & A, const Body & B);

   /**@}*/

   /**
    * \defgroup curves Curve creation functions
    * @{
    */
   //! Create a Bezier curve
   /*!
     \param poles Poles list
     \return BezierCurve Bezier curve
   */
   static Curve createBezierCurve(const GeomI::Point3List & poles);

   //! Create a bounded curve from a curve
   /*!
     \param curve Curve to bound
     \param minBound Minimum bound parameter
     \param maxBound Maximum bound parameter
     \return boundedCurve Curve bounded with given bounds
   */
   static LimitedCurve createBoundedCurve(const Curve & curve, const CoreI::Double & minBound, const CoreI::Double & maxBound);

   //! Create a new circle
   /*!
     \param radius Circle radius
     \param matrix Transformation matrix
     \return circleCurve Generated circle curve
   */
   static Curve createCircleCurve(const GeomI::Distance & radius, const GeomI::Matrix4 & matrix);

   //! Create a composite curve from a list of limited curves
   /*!
     \param limitedCurveList List of limited curves
     \return compositeCurve Composite curve created from the list of limited curves
   */
   static LimitedCurve createCompositeCurve(const LimitedCurveList & limitedCurveList);

   //! Create an ellipse curve
   /*!
     \param URadius Ellipse radius in u direction
     \param VRadius Ellipse radius in v direction
     \param matrix Transformation matrix
     \return ellipseCurve Ellipse curve
   */
   static Curve createEllipseCurve(const GeomI::Distance & URadius, const GeomI::Distance & VRadius, const GeomI::Matrix4 & matrix);

   //! Create an helix curve
   /*!
     \param radius Radius of the helix
     \param pitch Height of one revolution
     \param matrix Transformation matrix
     \param trigonometrixOrientation Orientation of the rotation
     \return helixCurve Helix curve
   */
   static Curve createHelixCurve(const GeomI::Distance & radius, const GeomI::Distance & pitch, const GeomI::Matrix4 & matrix, const CoreI::Boolean & trigonometrixOrientation = true);

   //! Create a Hermite Curve
   /*!
     \param FirstPoint Starting point of the curve
     \param FirstTangent Tangent of the starting point
     \param SecondPoint Ending point of the curve
     \param SecondTangent Tangent of the ending point
     \return HermiteCurve Hermite curve
   */
   static LimitedCurve createHermiteCurve(const GeomI::Point3 & FirstPoint, const GeomI::Point3 & FirstTangent, const GeomI::Point3 & SecondPoint, const GeomI::Point3 & SecondTangent);

   //! Create an hyperBola curve
   /*!
     \param URadius Hyperbola radius in u direction
     \param VRadius Hyperbola radius in v direction
     \param matrix Transformation matrix
     \return parabolaCurve Hyperbola curve
   */
   static Curve createHyperbolaCurve(const CoreI::Double & URadius, const CoreI::Double & VRadius, const GeomI::Matrix4 & matrix);

   //! Create a Intersection Curve
   /*!
     \param firstSurface First surface of the intersection curve
     \param secondSurface Second surface of the intersection curve
     \param chart Direction curve of the intersection curve
     \param minBounds Minimum value of the bounds of the intersection curve
     \param maxBounds Maximum value of the bounds of the intersection curve
     \return IntersectionCurve Intersection curve
   */
   static LimitedCurve createIntersectionCurve(const Surface & firstSurface, const Surface & secondSurface, const PolylineCurve & chart, const CoreI::Double & minBounds, const CoreI::Double & maxBounds);

   //! Create a Line Curve
   /*!
     \param OriginPt Orinin point of the line curve
     \param DirectionPt Direction vector of the line curve
     \return LineCurve Line curve
   */
   static Curve createLineCurve(const GeomI::Point3 & OriginPt, const GeomI::Point3 & DirectionPt);

   //! Create a NURBS curve
   /*!
     \param degree Degree of the curve
     \param knots Knots of the curve
     \param poles Poles list
     \param weights Weight list
     \return NURBSCurve NURBS curve
   */
   static Curve createNURBSCurve(const CoreI::Int & degree, const CoreI::DoubleList & knots, const GeomI::Point3List & poles, const CoreI::DoubleList & weights = CoreI::DoubleList(0));

   //! Create an parabola curve
   /*!
     \param focalLength Focal lecngth of the parabola
     \param matrix Transformation matrix
     \return parabolaCurve Parabola curve
   */
   static Curve createParabolaCurve(const CoreI::Double & focalLength, const GeomI::Matrix4 & matrix);

   //! Create a Polyline curve
   /*!
     \param points Points of polyline curve
     \return polylineCurve Polyline curve
   */
   static Curve createPolylineCurve(const GeomI::Point3List & points);

   //! Create a segment curve from two given points
   /*!
     \param firstPoint First point
     \param secondPoint Second point
     \return segmentCurve Segment curve created from the two given points
   */
   static LimitedCurve createSegmentCurve(const GeomI::Point3 & firstPoint, const GeomI::Point3 & secondPoint);

   //! Create a curve from a surface
   /*!
     \param surface Surface to bound
     \param curve2D Curve to project
     \return surfacicCurve Curve projected on given surface
   */
   static LimitedCurve createSurfacicCurve(const Surface & surface, const LimitedCurve & curve2D);

   //! Create a curve from a surface
   /*!
     \param curve Curve to transform
     \param matrix Matrix of the transformation
     \return transformedCurve Curve transformed by the given matrix
   */
   static LimitedCurve createTransformedCurve(const LimitedCurve & curve, const GeomI::Matrix4 & matrix);

   //! Invert a curve parametricaly
   /*!
     \param curve The curve to invert
     \param precision The precision used to invert the curve
     \return invertedCurve The inverted curve
   */
   static Curve invertCurve(const Curve & curve, const CoreI::Double & precision);

   /**@}*/

   /**
    * \defgroup material 
    * @{
    */
   //! get the material on a face
   /*!
     \param face The face
     \return material The material
   */
   static MaterialI::Material getFaceMaterial(const Face & face);

   //! set the material on a face
   /*!
     \param face The face
     \param material The material
   */
   static void setFaceMaterial(const Face & face, const MaterialI::Material & material);

   /**@}*/

   /**
    * \defgroup model management CAD model management functions
    * @{
    */
   //! Add shape to the model
   /*!
     \param shape Shape added to the model
     \param model Model
   */
   static void addToModel(const Shape & shape, const Model & model);

   //! Build faces from a surface and a set of loop
   /*!
     \param surface Surface used to build the faces
     \param loopList List of Loops used to build the faces
     \return domain The created domain (face or openShell)
     \return splittingInfo Map between the old edges and the new ones
   */
   static buildFacesReturn buildFaces(const Surface & surface, const LoopList & loopList);

   //! Create a new model
   /*!
     \return model The created model
   */
   static Model createModel();

   //! Get all the face of a model recursively
   /*!
     \param model Model
     \return faces List of faces in the given model
   */
   static FaceList getAllModelFaces(const Model & model);

   //! Get the list of bodies contained in a model
   /*!
     \param model Model
     \return bodies List of bodies contained in the given model
   */
   static BodyList getModelBodies(const Model & model);

   //! Get boundary edges of a model grouped by cycles
   /*!
     \param model Model
     \return boundaries List of boundary edges grouped by cycles in the given model
   */
   static EdgeListList getModelBoundaries(const Model & model);

   //! Get the list of domains (Face or OpenShell) contained in a model
   /*!
     \param model Model
     \return domains List of domains contained in the given model
   */
   static DomainList getModelDomains(const Model & model);

   //! Get the list of free edges contained in a model
   /*!
     \param model Model
     \return edges List of edges contained in the given model
   */
   static EdgeList getModelEdges(const Model & model);

   //! Get the list of free vertices contained in a model
   /*!
     \param model Model
     \return vertices List of vertices contained in the given model
   */
   static VertexList getModelVertices(const Model & model);

   //! Returns the entities referencing a given CAD entity
   /*!
     \param entity CAD entity to get the referencers
     \return referencers List of CAD entities referencing the given entity
   */
   static CoreI::EntityList getReferencers(const CADEntity & entity);

   /**@}*/

   /**
    * \defgroup structure access CAD structure access functions
    * @{
    */
   //! evaluate curvature on a curve
   /*!
     \param curve The curve
     \param parameter Parameter to evaluate
     \return curvature Curvature on curve at parameter
   */
   static CoreI::Double evalCurvatureOnCurve(const Curve & curve, const CoreI::Double & parameter);

   //! evaluate main curvatures on a surface
   /*!
     \param surface The surface
     \param parameter Parameter to evaluate
     \return curvatures Main curvatures on surface at parameter
   */
   static GeomI::Curvatures evalCurvatureOnSurface(const Surface & surface, const GeomI::Point2 & parameter);

   //! evaluate a point and derivatives on a curve
   /*!
     \param curve The curve
     \param parameter Parameter to evaluate
     \param derivation Derivation level (0,1,2)
     \return d0 D0
     \return du Du
     \return d2u D2u
   */
   static evalOnCurveReturn evalOnCurve(const Curve & curve, const CoreI::Double & parameter, const CoreI::Int & derivation = 0);

   //! evaluate a point and derivatives on a surface
   /*!
     \param surface The surface
     \param parameter Parameter to evaluate
     \param derivation Derivation level (0,1,2)
     \return d0 D0
     \return du Du
     \return dv Dv
     \return d2u D2u
     \return d2v D2v
     \return duv Duv
   */
   static evalOnSurfaceReturn evalOnSurface(const Surface & surface, const GeomI::Point2 & parameter, const CoreI::Int & derivation = 0);

   //! get all closedShells contain in the body
   /*!
     \param body The body
     \return closedShells The closedShells contain within the body
   */
   static ClosedShellList getBodyClosedShells(const Body & body);

   //! get all parameters contained in the boundedCurve
   /*!
     \param boundedCurve The boundedCurve
     \return curve The curve of the boundedCurve
     \return bounds The boudns of the boundedCurve
   */
   static getBoundedCurveDefinitionReturn getBoundedCurveDefinition(const BoundedCurve & boundedCurve);

   //! get all parameters contained in the circleCurve
   /*!
     \param circleCurve The circleCurve
     \return radius The radius of the circle
     \return matrix The matrix transformation of the circle
   */
   static getCircleCurveDefinitionReturn getCircleCurveDefinition(const CircleCurve & circleCurve);

   //! get all orienteDomains contain in the closedShell
   /*!
     \param closedShell The closedShell
     \return orientedDomains The orientedDomains contain within the closedShell
   */
   static OrientedDomainList getClosedShellOrientedDomains(const ClosedShell & closedShell);

   //! get all parameters contained in the coEdge
   /*!
     \param coEdge The coEdge
     \return edge The edge of the coEdge
     \return edgeOrientation Orientation of the edge
     \return loop The loop containing the coEdge
     \return surface The surface of the coEdge
     \return parametricCurve The parametricCurve of the coEdge
   */
   static getCoEdgeDefinitionReturn getCoEdgeDefinition(const CoEdge & coEdge);

   //! get all parameters contained in the compositeCurve
   /*!
     \param compositeCurve The compositeCurve
     \return curves The curves of the compositeCurve
     \return parameters The parameters of the compositeCurve
   */
   static getCompositeCurveDefinitionReturn getCompositeCurveDefinition(const CompositeCurve & compositeCurve);

   //! get all parameters contained in the coneSurface
   /*!
     \param coneSurface The coneSurface
     \return radius The radius of the coneSurface
     \return semiAngle The semiAngle of coneSurface
     \return matrix The transformation matrix of coneSurface
   */
   static getConeSurfaceDefinitionReturn getConeSurfaceDefinition(const ConeSurface & coneSurface);

   //! get all parameters contained in the curveExtrusionSurface
   /*!
     \param curveExtrusionSurface The curveExtrusionSurface
     \return generatrixCurve The generatrix curve of the curveExtrusionSurface
     \return directrixCruve The directrix curve of the curveExtrusionSurface
     \return surfaceReference The reference surface of curveExtrusionSurface
   */
   static getCurveExtrusionSurfaceDefinitionReturn getCurveExtrusionSurfaceDefinition(const CurveExtrusionSurface & curveExtrusionSurface);

   //! get the parametric space limits of a curve
   /*!
     \param curve The curve
     \return limits Curve limits
   */
   static Bounds1D getCurveLimits(const LimitedCurve & curve);

   //! get all parameters contained in the cylinderSurface
   /*!
     \param cylinderSurface The cylinderSurface
     \return radius The radius of the cylinderSurface
     \return matrix The transformation matrix of cylinderSurface
   */
   static getCylinderSurfaceDefinitionReturn getCylinderSurfaceDefinition(const CylinderSurface & cylinderSurface);

   //! get all parameters contained in the edge
   /*!
     \param edge The edge
     \return vertex1 The first vertex of the edge
     \return vertex2 The second vertex of the edge
     \return curve The curve of the edge
     \return bounds The bounds of the edge
   */
   static getEdgeDefinitionReturn getEdgeDefinition(const Edge & edge);

   //! get all parameters contained in the ellipseCurve
   /*!
     \param ellipseCurve The ellipseCurve
     \return radius1 The radius on x of the ellipse
     \return radius2 The radius on y of the ellipse
     \return matrix The transformation matrix of the ellipse
   */
   static getEllipseCurveDefinitionReturn getEllipseCurveDefinition(const EllipseCurve & ellipseCurve);

   //! get all parameters contained in the ellipticConeSurface
   /*!
     \param ellipticConeSurface The EllipticConeSurface
     \return radius1 The radius on X of the coneSurface
     \return radius2 The radius on Y of the coneSurface
     \return semiAngle The semiAngle of coneSurface
     \return matrix The transformation matrix of coneSurface
   */
   static getEllipticConeSurfaceDefinitionReturn getEllipticConeSurfaceDefinition(const EllipticConeSurface & ellipticConeSurface);

   //! get all parameters contain in the face
   /*!
     \param face The face
     \return surface The surface contain within the face
     \return loops The loops contain within the face
     \return orientation Relative orientation of the surface
     \return limits Face limits on surfaces
   */
   static getFaceDefinitionReturn getFaceDefinition(const Face & face);

   //! get parametric definition of each face loop
   /*!
     \param face The face
     \return boundaries The parametric boundaries
   */
   static GeomI::Point2ListList getFaceParametricBoundaries(const Face & face);

   //! get all parameters contained in the helixCurve
   /*!
     \param helixCurve The helixCurve
     \return radius The radius of the helixCurve
     \return matrix The matrix of the helixCurve
     \return trigonometricOrientation The trigonometricOrientation of the helixCurve
   */
   static getHelixCurveDefinitionReturn getHelixCurveDefinition(const HelixCurve & helixCurve);

   //! get all parameters contained in the hermiteCurve
   /*!
     \param hermiteCurve The HermiteCurve
     \return firstPoint The first point of the hermiteCurve
     \return secondPoint The second point of the hermiteCurve
     \return firstTangent The first tangent of the hermiteCurve
     \return secondTangent The second tangent of the hermiteCurve
   */
   static getHermiteCurveDefinitionReturn getHermiteCurveDefinition(const HermiteCurve & hermiteCurve);

   //! get all parameters contained in the hyperbolaCurve
   /*!
     \param hyperbolaCurve The hyperbolaCurve
     \return radius1 The radius on x of the hyperbola
     \return radius2 The radius on y of the hyperbola
     \return matrix The transformation matrix of the hyperbola
   */
   static getHyperbolaCurveDefinitionReturn getHyperbolaCurveDefinition(const HyperbolaCurve & hyperbolaCurve);

   //! get all parameters contained in the intersectionCurve
   /*!
     \param intersectionCurve The intersectionCurve
     \return surface1 The first surface of the intersectionCurve
     \return surface2 The second surface of the intersectionCurve
     \return chart The chart of the intersectionCurve
     \return bounds The boudns of the intersectionCurve
   */
   static getIntersectionCurveDefinitionReturn getIntersectionCurveDefinition(const IntersectionCurve & intersectionCurve);

   //! get all parameters contain in the lineCurve
   /*!
     \param lineCurve The lineCurve
     \return origin The origin of the lineCurve
     \return direction The direction of the lineCurve
   */
   static getLineCurveDefinitionReturn getLineCurveDefinition(const LineCurve & lineCurve);

   //! get all coEdges contain in the loop
   /*!
     \param loop The loop
     \return coEdges The coEdges contain within the loop
   */
   static CoEdgeList getLoopCoEdges(const Loop & loop);

   //! get all parameters contained in the nurbsCurve
   /*!
     \param nurbsCurve The nurbsCurve
     \return degree The degree of the nurbsCurve
     \return knots The knots of the nurbsCurve
     \return poles The poles of the nurbsCurve
     \return weights The weights of the poles of the nurbsCurve
   */
   static getNURBSCurveDefinitionReturn getNURBSCurveDefinition(const NURBSCurve & nurbsCurve);

   //! get all parameters contained in the nurbsSurface
   /*!
     \param nurbsSurface The nurbsSurface
     \return degreeU The degree on U of the nurbsSurface
     \return degreeV The degree on V of the nurbsSurface
     \return knotsU The knots on U of the nurbsSurface
     \return knotsV The knots on V of the nurbsSurface
     \return poles The poles of nurbsSurface
     \return weights The weights of the poles of nurbsSurface
   */
   static getNURBSSurfaceDefinitionReturn getNURBSSurfaceDefinition(const NURBSSurface & nurbsSurface);

   //! get all parameters contained in the offsetCurve
   /*!
     \param offsetCurve The offsetCurve
     \return curve The curve of the offsetCurve
     \return direction The direction of the offset
     \return distance The distance of the offset
     \return surfaceReference The surfaceReference of the offsetCurve
   */
   static getOffsetCurveDefinitionReturn getOffsetCurveDefinition(const OffsetCurve & offsetCurve);

   //! get all parameters contained in the offsetSurface
   /*!
     \param offsetSurface The offsetSurface
     \return baseSurface The initial surface
     \return distance The distance offset
   */
   static getOffsetSurfaceDefinitionReturn getOffsetSurfaceDefinition(const OffsetSurface & offsetSurface);

   //! get all orienteDomains contain in the openShell
   /*!
     \param openShell The openShell
     \return orientedDomains The orientedDomains contain within the openShell
   */
   static OrientedDomainList getOpenShellOrientedDomains(const OpenShell & openShell);

   //! get all parameters contained in the parabolaCurve
   /*!
     \param parabolaCurve The parabolaCurve
     \return focalLength The radius of the hyperbola
     \return matrix The transformation matrix of the hyperbola
   */
   static getParabolaCurveDefinitionReturn getParabolaCurveDefinition(const ParabolaCurve & parabolaCurve);

   //! get all parameters contained in the planeSurface
   /*!
     \param planeSurface The planeSurface
     \return matrix The transformation matrix of planeSurface
   */
   static GeomI::Matrix4 getPlaneSurfaceDefinition(const PlaneSurface & planeSurface);

   //! get all parameters contained in the polylinCurve
   /*!
     \param polylineCurve The polylineCurve
     \return points The points of the polylineCurve
     \return parameters The parameters of the polylineCurve
   */
   static getPolylineCurveDefinitionReturn getPolylineCurveDefinition(const PolylineCurve & polylineCurve);

   //! get all parameters contained in the revolutionSurface
   /*!
     \param revolutionSurface The revolutionSurface
     \return generatricCurve Thegeneratrix curve of the revolutionSurface
     \return axisOrigin The origin of the axis of the revolutionSurface
     \return axisDirection The direction of the axis of the revolutionSurface
     \return startAngle The starting angle of the revolutionSurface
     \return endAngle The ending angle of the revolutionSurface
   */
   static getRevolutionSurfaceDefinitionReturn getRevolutionSurfaceDefinition(const RevolutionSurface & revolutionSurface);

   //! get all parameters contained in the ruledSurface
   /*!
     \param ruledSurface The ruledSurface
     \return firstCurve The first curve of the ruledSurface
     \return secondCurve The second curve of the ruledSurface
   */
   static getRuledSurfaceDefinitionReturn getRuledSurfaceDefinition(const RuledSurface & ruledSurface);

   //! get all parameters contained in the segmentCurve
   /*!
     \param segmentCurve The segmentCurve
     \return startPoint The first point of the segmentCurve
     \return endPoint The second point of the segmentCurve
   */
   static getSegmentCurveDefinitionReturn getSegmentCurveDefinition(const SegmentCurve & segmentCurve);

   //! get all parameters contained in the sphereSurface
   /*!
     \param sphereSurface The sphereSurface
     \return radius The radius of the sphereSurface
     \return matrix The transformation matrix of sphereSurface
   */
   static getSphereSurfaceDefinitionReturn getSphereSurfaceDefinition(const SphereSurface & sphereSurface);

   //! get the parametric space limits of a surface
   /*!
     \param surface The surface
     \return limits Surface limits
   */
   static Bounds2D getSurfaceLimits(const LimitedSurface & surface);

   //! get all parameters contained in the surfacicCurve
   /*!
     \param surfacicCurve The surfacicCurve
     \return surface The surface of the surfacicCurve
     \return curve2D The 2D curve of the surfacicCurve
   */
   static getSurfacicCurveDefinitionReturn getSurfacicCurveDefinition(const SurfacicCurve & surfacicCurve);

   //! get all parameters contained in the TabulatedCylinderSurface
   /*!
     \param tabulatedCylinderSurface The tabulatedCylinderSurface
     \return directrixCurve The directrix curve of the tabulatedCylinderSurface
     \return generatrixLine The generatrix line of the tabulatedCylinderSurface
     \return range The range of the tabulatedCylinderSurface
   */
   static getTabulatedCylinderSurfaceDefinitionReturn getTabulatedCylinderSurfaceDefinition(const TabulatedCylinderSurface & tabulatedCylinderSurface);

   //! get all parameters contained in the torusSurface
   /*!
     \param torusSurface The torusSurface
     \return majorRadius The major radius of the torusSurface
     \return minorRadius The minor radius of the torusSurface
     \return matrix The transformation matrix of torusSurface
   */
   static getTorusSurfaceDefinitionReturn getTorusSurfaceDefinition(const TorusSurface & torusSurface);

   //! get all parameters contained in the transformedCurve
   /*!
     \param transformedCurve The transformedCurve
     \return curve The initial curve
     \return matrix The transformation matrix
   */
   static getTransformedCurveDefinitionReturn getTransformedCurveDefinition(const TransformedCurve & transformedCurve);

   //! get the position of the vertex
   /*!
     \param vertex The vertex
     \return position The position of the vertex
   */
   static GeomI::Point3 getVertexPosition(const Vertex & vertex);

   //! if the curve is closed, return true, return false otherwise
   /*!
     \param curve The curve
     \return closed The value
   */
   static CoreI::Boolean isCurveClosed(const Curve & curve);

   //! if the curve is periodic return true, return false otherwise
   /*!
     \param curve The curve
     \return periodic The value
     \return period If th curve is periodic, this value is equal to the period value, equal to 0 otherwise
   */
   static isCurvePeriodicReturn isCurvePeriodic(const Curve & curve);

   //! return if the surface is closed on U or on V
   /*!
     \param surface The surface
     \return closedU The value on U
     \return closedV The value on V
   */
   static isSurfaceClosedReturn isSurfaceClosed(const Surface & surface);

   //! return if the surface is periodic on U or on V
   /*!
     \param surface The surface
     \return periodicU The value on U
     \return periodicV The value on V
     \return periodU If th curve is periodic on U, this value is equal to the period value, equal to 0 otherwise
     \return periodV If th curve is periodic on V, this value is equal to the period value, equal to 0 otherwise
   */
   static isSurfacePeriodicReturn isSurfacePeriodic(const Surface & surface);

   //! project a point to a curve
   /*!
     \param curve The curve
     \param point The point to project
     \param precision Projection precision
     \return projectionParameter The projection parameter on the curve
   */
   static CoreI::Double projectOnCurve(const Curve & curve, const GeomI::Point3 & point, const CoreI::Double & precision = -1);

   //! project a point to a surface
   /*!
     \param surface The surface
     \param point The point to project
     \param precision Projection precision
     \return projectionParameter The projection parameter on the surface
   */
   static GeomI::Point2 projectOnSurface(const Surface & surface, const GeomI::Point3 & point, const CoreI::Double & precision = -1);

   /**@}*/

   /**
    * \defgroup structure creation CAD structure creation functions
    * @{
    */
   //! Create a body from a surface
   /*!
     \param outerShell ClosedShell used to create the body
     \param innerShells List of closedShell used to create the body
     \return body The created body
   */
   static Body createBody(const ClosedShell & outerShell, const ClosedShellList & innerShells = ClosedShellList(0));

   //! Create a closedShell from a set of domains of a set of orientations
   /*!
     \param domains List of domains composing the closedShell
     \param orientations List of orientations for each domain
     \return closedShell The created closedShell
   */
   static ClosedShell createClosedShell(const DomainList & domains, const GeomI::OrientationList & orientations);

   //! Create an coEdge with a edge and an orientation
   /*!
     \param edge Edge used to create the coEdge
     \param orientation Orientation of the edge regarding the loop
     \param surface The surface trimmed by the edge
     \param curve2D Surfacic curve of the edge on the surface trimmed
     \return coEdge The created edge
   */
   static CoEdge createCoEdge(const Edge & edge, const GeomI::Orientation & orientation, const Surface & surface = 0, const Curve & curve2D = 0);

   //! Create an edge with a curve an extremity vertices
   /*!
     \param curve Curve used to create the edge
     \param startVertex The start vertex
     \param endVertex The end vertex
     \return edge The created edge
   */
   static Edge createEdge(const Curve & curve, const Vertex & startVertex, const Vertex & endVertex);

   //! Create an edge from a limited curve
   /*!
     \param curve Limited curve used to create the edge
     \return edge The created edge
   */
   static Edge createEdgeFromCurve(const LimitedCurve & curve);

   //! Create a face from a surface
   /*!
     \param surface Surface used to create the face
     \param loopList List of Loops used to create the face
     \param useSurfaceOrientation If True, the face will have the same orientation than the surface and loops will be inverted if they are inconsistent
     \return face The created face
   */
   static Face createFace(const Surface & surface, const LoopList & loopList = LoopList(0), const CoreI::Boolean & useSurfaceOrientation = false);

   //! Create a loop from a set of edges of a set of orientations
   /*!
     \param coEdges List of coEdges composing the loop
     \param check If true, the loop check if edges are well connected or not
     \return loop The created loop
   */
   static Loop createLoop(const CoEdgeList & coEdges, const CoreI::Boolean & check = true);

   //! Create a openShell from a set of domains of a set of orientations and set of loops
   /*!
     \param domains List of domains composing the openShell
     \param orientations List of orientations for each domain
     \param loopList List of loops restricted the openShell
     \return openShell The created openShell
   */
   static OpenShell createOpenShell(const DomainList & domains, const GeomI::OrientationList & orientations, const LoopList & loopList = LoopList(0));

   //! Create a vertex from a position
   /*!
     \param position Vertex position
     \return vertex The created vertex
   */
   static Vertex createVertex(const GeomI::Point3 & position);

   /**@}*/

   /**
    * \defgroup surfaces Surface creation functions
    * @{
    */
   //! Create a new bezier surface
   /*!
     \param degreeU U degree
     \param degreeV V degree
     \param poles Poles list
     \return bezierSurface The new Bezier surface
   */
   static Surface createBezierSurface(const CoreI::Int & degreeU, const CoreI::Int & degreeV, const GeomI::Point3List & poles);

   //! Create a new cone surface
   /*!
     \param radius Radius of the cone at origin
     \param semiAngle Semi-angle of the cone
     \param matrix Positionning matrix of the cone
     \return coneSurface The new cone surface
   */
   static Surface createConeSurface(const GeomI::Distance & radius, const GeomI::Angle & semiAngle, const GeomI::Matrix4 & matrix = GeomI::IdentityMatrix4());

   //! Create a new curveExtrusion surface
   /*!
     \param generatrixCurve The generatrix curve
     \param directrixCurve The directrix curve
     \param refSurface The reference surface
     \param precision The precision for the evaluation of points
     \return curveExtrusionSurface The new curveExtrusion surface
   */
   static Surface createCurveExtrusionSurface(const LimitedCurve & generatrixCurve, const LimitedCurve & directrixCurve, const Surface & refSurface, const CoreI::Double & precision);

   //! Create a new cylinder surface
   /*!
     \param radius Radius of the cylinder
     \param matrix Positionning matrix of the cylinder
     \return cylinderSurface The new cylinder surface
   */
   static Surface createCylinderSurface(const GeomI::Distance & radius, const GeomI::Matrix4 & matrix = GeomI::IdentityMatrix4());

   //! Create a new elliptic cone surface
   /*!
     \param radius1 Radius of the cone at origin on the X axis
     \param radius2 Radius of the cone at origin on the Y axis
     \param semiAngle Semi-angle of the cone
     \param matrix Positionning matrix of the cone
     \return ellipticConeSurface The new elliptic cone surface
   */
   static Surface createEllipticConeSurface(const GeomI::Distance & radius1, const GeomI::Distance & radius2, const GeomI::Angle & semiAngle, const GeomI::Matrix4 & matrix = GeomI::IdentityMatrix4());

   //! Create a new NURBS surface
   /*!
     \param degreeU U degree
     \param degreeV V degree
     \param knotsU Knots on U
     \param knotsV Knots on V
     \param poles Poles list
     \param weights Weights list
     \return NURBSSurface The new NURBS surface
   */
   static Surface createNURBSSurface(const CoreI::Int & degreeU, const CoreI::Int & degreeV, const CoreI::DoubleList & knotsU, const CoreI::DoubleList & knotsV, const GeomI::Point3List & poles, const CoreI::DoubleList & weights = CoreI::DoubleList(0));

   //! Create a new offset surface
   /*!
     \param baseSurface The base surface
     \param distance The offset distance
     \return offsetSurface The new offset surface
   */
   static Surface createOffsetSurface(const Surface & baseSurface, const CoreI::Double & distance);

   //! Create a new plane surface
   /*!
     \param matrix Positionning matrix of the plane
     \return planeSurface The new plane surface
   */
   static Surface createPlaneSurface(const GeomI::Matrix4 & matrix = GeomI::IdentityMatrix4());

   //! Create a new revolution surface
   /*!
     \param generatrixCurve Generatrix curve rotated to create the revolution surface
     \param axisOrigin Axis origin point
     \param axisDirection Axis direction vector
     \param startAngle Start angle of the revolution surface
     \param endAngle End angle of the revolution surface
     \return revolutionSurface Revolution surface generated by rotating the given curve around the axis
   */
   static Surface createRevolutionSurface(const LimitedCurve & generatrixCurve, const GeomI::Point3 & axisOrigin, const GeomI::Vector3 & axisDirection, const GeomI::Angle & startAngle = 0, const GeomI::Angle & endAngle = 360.0);

   //! Create a new ruled surface
   /*!
     \param firstCurve First Curve
     \param secondCurve Seconde Curve
     \return ruledSurface The new ruled surface
   */
   static Surface createRuledSurface(const LimitedCurve & firstCurve, const LimitedCurve & secondCurve);

   //! Create a new sphere surface
   /*!
     \param radius Radius of the sphere
     \param matrix Positionning matrix of the sphere
     \return sphereSurface The new sphere surface
   */
   static Surface createSphereSurface(const GeomI::Distance & radius, const GeomI::Matrix4 & matrix = GeomI::IdentityMatrix4());

   //! Create a new tabulated cylinder surface
   /*!
     \param directrixCurve Directrix Curve
     \param GeneratixLine Generatrix Line
     \param minRange Minimimum value of the range
     \param maxRange Maximum value of the range
     \return tabulatedCylinderSurface The new tabulated cylinder surface
   */
   static Surface createTabulatedCylinderSurface(const LimitedCurve & directrixCurve, const GeomI::Point3 & GeneratixLine, const GeomI::Distance & minRange, const GeomI::Distance & maxRange);

   //! Create a new torus surface
   /*!
     \param radiusMax Major radius 
     \param radiusMin Minor radius 
     \param matrix Positionning matrix of the sphere
     \return torusSurface The new torus surface
   */
   static Surface createTorusSurface(const GeomI::Distance & radiusMax, const GeomI::Distance & radiusMin, const GeomI::Matrix4 & matrix = GeomI::IdentityMatrix4());

   /**@}*/

};

PXZ_MODULE_END



#endif
