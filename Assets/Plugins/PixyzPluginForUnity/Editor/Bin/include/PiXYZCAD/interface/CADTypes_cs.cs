#pragma warning disable CA2101

using System;
using System.Security;
using System.Runtime.InteropServices;

namespace Pixyz.CAD.Native {

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class BodyList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public BodyList() {}
		public BodyList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](BodyList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public BodyList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct BodyList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Bounds1D
	{
		public Bounds1D(Bounds1D o) {
			this.min = o.min;
			this.max = o.max;
		}
		public System.Double min;
		public System.Double max;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Bounds1D_c
	{
		public System.Double min;
		public System.Double max;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class Bounds2D
	{
		public Bounds2D() {}
		public Bounds2D(Bounds2D o) {
			this.u = o.u;
			this.v = o.v;
		}
		public Bounds1D u;
		public Bounds1D v;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Bounds2D_c
	{
		public Bounds1D_c u;
		public Bounds1D_c v;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class ClosedShellList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public ClosedShellList() {}
		public ClosedShellList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](ClosedShellList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public ClosedShellList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ClosedShellList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class CoEdgeList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public CoEdgeList() {}
		public CoEdgeList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](CoEdgeList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public CoEdgeList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct CoEdgeList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class CurveList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public CurveList() {}
		public CurveList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](CurveList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public CurveList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct CurveList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class DomainList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public DomainList() {}
		public DomainList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](DomainList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public DomainList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DomainList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class EdgeList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public EdgeList() {}
		public EdgeList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](EdgeList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public EdgeList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct EdgeList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class EdgeListList {
		public CAD.Native.EdgeList[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public EdgeListList(CAD.Native.EdgeList[] tab) { list = tab; }
		public static implicit operator CAD.Native.EdgeList[](EdgeListList o) { return o.list; }
		public CAD.Native.EdgeList this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public EdgeListList(int size) { list = new CAD.Native.EdgeList[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct EdgeListList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class FaceList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public FaceList() {}
		public FaceList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](FaceList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public FaceList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct FaceList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class LimitedCurveList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public LimitedCurveList() {}
		public LimitedCurveList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](LimitedCurveList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public LimitedCurveList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct LimitedCurveList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class LoopList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public LoopList() {}
		public LoopList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](LoopList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public LoopList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct LoopList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class ModelList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public ModelList() {}
		public ModelList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](ModelList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public ModelList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ModelList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class OpenShellList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public OpenShellList() {}
		public OpenShellList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](OpenShellList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public OpenShellList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct OpenShellList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct OrientedDomain
	{
		public OrientedDomain(OrientedDomain o) {
			this.domain = o.domain;
			this.orientation = o.orientation;
		}
		public System.UInt32 domain;
		public System.Boolean orientation;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct OrientedDomain_c
	{
		public System.UInt32 domain;
		public Int32 orientation;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class OrientedDomainList {
		public CAD.Native.OrientedDomain[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public OrientedDomainList(CAD.Native.OrientedDomain[] tab) { list = tab; }
		public static implicit operator CAD.Native.OrientedDomain[](OrientedDomainList o) { return o.list; }
		public CAD.Native.OrientedDomain this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public OrientedDomainList(int size) { list = new CAD.Native.OrientedDomain[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct OrientedDomainList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct OrientedEdge
	{
		public OrientedEdge(OrientedEdge o) {
			this.edge = o.edge;
			this.orientation = o.orientation;
		}
		public System.UInt32 edge;
		public System.Boolean orientation;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct OrientedEdge_c
	{
		public System.UInt32 edge;
		public Int32 orientation;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class OrientedEdgeList {
		public CAD.Native.OrientedEdge[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public OrientedEdgeList(CAD.Native.OrientedEdge[] tab) { list = tab; }
		public static implicit operator CAD.Native.OrientedEdge[](OrientedEdgeList o) { return o.list; }
		public CAD.Native.OrientedEdge this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public OrientedEdgeList(int size) { list = new CAD.Native.OrientedEdge[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct OrientedEdgeList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class SplittedEdge
	{
		public SplittedEdge() {}
		public SplittedEdge(SplittedEdge o) {
			this.oldEdge = o.oldEdge;
			this.newEdges = o.newEdges;
		}
		public System.UInt32 oldEdge;
		public EdgeList newEdges;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SplittedEdge_c
	{
		public System.UInt32 oldEdge;
		public EdgeList_c newEdges;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class SplittedEdgeList {
		public CAD.Native.SplittedEdge[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public SplittedEdgeList(CAD.Native.SplittedEdge[] tab) { list = tab; }
		public static implicit operator CAD.Native.SplittedEdge[](SplittedEdgeList o) { return o.list; }
		public CAD.Native.SplittedEdge this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public SplittedEdgeList(int size) { list = new CAD.Native.SplittedEdge[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SplittedEdgeList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class VertexList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public VertexList() {}
		public VertexList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](VertexList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public VertexList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct VertexList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class buildFacesReturn
	{
		public buildFacesReturn() {}
		public buildFacesReturn(buildFacesReturn o) {
			this.domain = o.domain;
			this.splittingInfo = o.splittingInfo;
		}
		public System.UInt32 domain;
		public SplittedEdgeList splittingInfo;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct buildFacesReturn_c
	{
		public System.UInt32 domain;
		public SplittedEdgeList_c splittingInfo;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class evalOnCurveReturn
	{
		public evalOnCurveReturn() {}
		public evalOnCurveReturn(evalOnCurveReturn o) {
			this.d0 = o.d0;
			this.du = o.du;
			this.d2u = o.d2u;
		}
		public Geom.Native.Point3 d0;
		public Geom.Native.Point3 du;
		public Geom.Native.Point3 d2u;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct evalOnCurveReturn_c
	{
		public Geom.Native.Point3_c d0;
		public Geom.Native.Point3_c du;
		public Geom.Native.Point3_c d2u;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class evalOnSurfaceReturn
	{
		public evalOnSurfaceReturn() {}
		public evalOnSurfaceReturn(evalOnSurfaceReturn o) {
			this.d0 = o.d0;
			this.du = o.du;
			this.dv = o.dv;
			this.d2u = o.d2u;
			this.d2v = o.d2v;
			this.duv = o.duv;
		}
		public Geom.Native.Point3 d0;
		public Geom.Native.Point3 du;
		public Geom.Native.Point3 dv;
		public Geom.Native.Point3 d2u;
		public Geom.Native.Point3 d2v;
		public Geom.Native.Point3 duv;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct evalOnSurfaceReturn_c
	{
		public Geom.Native.Point3_c d0;
		public Geom.Native.Point3_c du;
		public Geom.Native.Point3_c dv;
		public Geom.Native.Point3_c d2u;
		public Geom.Native.Point3_c d2v;
		public Geom.Native.Point3_c duv;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getBoundedCurveDefinitionReturn
	{
		public getBoundedCurveDefinitionReturn() {}
		public getBoundedCurveDefinitionReturn(getBoundedCurveDefinitionReturn o) {
			this.curve = o.curve;
			this.bounds = o.bounds;
		}
		public System.UInt32 curve;
		public Bounds1D bounds;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getBoundedCurveDefinitionReturn_c
	{
		public System.UInt32 curve;
		public Bounds1D_c bounds;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getCircleCurveDefinitionReturn
	{
		public getCircleCurveDefinitionReturn() {}
		public getCircleCurveDefinitionReturn(getCircleCurveDefinitionReturn o) {
			this.radius = o.radius;
			this.matrix = o.matrix;
		}
		public System.Double radius;
		public Geom.Native.Matrix4 matrix;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getCircleCurveDefinitionReturn_c
	{
		public System.Double radius;
		public Geom.Native.Matrix4_c matrix;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct getCoEdgeDefinitionReturn
	{
		public getCoEdgeDefinitionReturn(getCoEdgeDefinitionReturn o) {
			this.edge = o.edge;
			this.edgeOrientation = o.edgeOrientation;
			this.loop = o.loop;
			this.surface = o.surface;
			this.parametricCurve = o.parametricCurve;
		}
		public System.UInt32 edge;
		public System.Boolean edgeOrientation;
		public System.UInt32 loop;
		public System.UInt32 surface;
		public System.UInt32 parametricCurve;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getCoEdgeDefinitionReturn_c
	{
		public System.UInt32 edge;
		public Int32 edgeOrientation;
		public System.UInt32 loop;
		public System.UInt32 surface;
		public System.UInt32 parametricCurve;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getCompositeCurveDefinitionReturn
	{
		public getCompositeCurveDefinitionReturn() {}
		public getCompositeCurveDefinitionReturn(getCompositeCurveDefinitionReturn o) {
			this.curves = o.curves;
			this.parameters = o.parameters;
		}
		public LimitedCurveList curves;
		public Core.Native.DoubleList parameters;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getCompositeCurveDefinitionReturn_c
	{
		public LimitedCurveList_c curves;
		public Core.Native.DoubleList_c parameters;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getConeSurfaceDefinitionReturn
	{
		public getConeSurfaceDefinitionReturn() {}
		public getConeSurfaceDefinitionReturn(getConeSurfaceDefinitionReturn o) {
			this.radius = o.radius;
			this.semiAngle = o.semiAngle;
			this.matrix = o.matrix;
		}
		public System.Double radius;
		public System.Double semiAngle;
		public Geom.Native.Matrix4 matrix;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getConeSurfaceDefinitionReturn_c
	{
		public System.Double radius;
		public System.Double semiAngle;
		public Geom.Native.Matrix4_c matrix;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct getCurveExtrusionSurfaceDefinitionReturn
	{
		public getCurveExtrusionSurfaceDefinitionReturn(getCurveExtrusionSurfaceDefinitionReturn o) {
			this.generatrixCurve = o.generatrixCurve;
			this.directrixCruve = o.directrixCruve;
			this.surfaceReference = o.surfaceReference;
		}
		public System.UInt32 generatrixCurve;
		public System.UInt32 directrixCruve;
		public System.UInt32 surfaceReference;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getCurveExtrusionSurfaceDefinitionReturn_c
	{
		public System.UInt32 generatrixCurve;
		public System.UInt32 directrixCruve;
		public System.UInt32 surfaceReference;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getCylinderSurfaceDefinitionReturn
	{
		public getCylinderSurfaceDefinitionReturn() {}
		public getCylinderSurfaceDefinitionReturn(getCylinderSurfaceDefinitionReturn o) {
			this.radius = o.radius;
			this.matrix = o.matrix;
		}
		public System.Double radius;
		public Geom.Native.Matrix4 matrix;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getCylinderSurfaceDefinitionReturn_c
	{
		public System.Double radius;
		public Geom.Native.Matrix4_c matrix;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getEdgeDefinitionReturn
	{
		public getEdgeDefinitionReturn() {}
		public getEdgeDefinitionReturn(getEdgeDefinitionReturn o) {
			this.vertex1 = o.vertex1;
			this.vertex2 = o.vertex2;
			this.curve = o.curve;
			this.bounds = o.bounds;
		}
		public System.UInt32 vertex1;
		public System.UInt32 vertex2;
		public System.UInt32 curve;
		public Bounds1D bounds;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getEdgeDefinitionReturn_c
	{
		public System.UInt32 vertex1;
		public System.UInt32 vertex2;
		public System.UInt32 curve;
		public Bounds1D_c bounds;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getEllipseCurveDefinitionReturn
	{
		public getEllipseCurveDefinitionReturn() {}
		public getEllipseCurveDefinitionReturn(getEllipseCurveDefinitionReturn o) {
			this.radius1 = o.radius1;
			this.radius2 = o.radius2;
			this.matrix = o.matrix;
		}
		public System.Double radius1;
		public System.Double radius2;
		public Geom.Native.Matrix4 matrix;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getEllipseCurveDefinitionReturn_c
	{
		public System.Double radius1;
		public System.Double radius2;
		public Geom.Native.Matrix4_c matrix;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getEllipticConeSurfaceDefinitionReturn
	{
		public getEllipticConeSurfaceDefinitionReturn() {}
		public getEllipticConeSurfaceDefinitionReturn(getEllipticConeSurfaceDefinitionReturn o) {
			this.radius1 = o.radius1;
			this.radius2 = o.radius2;
			this.semiAngle = o.semiAngle;
			this.matrix = o.matrix;
		}
		public System.Double radius1;
		public System.Double radius2;
		public System.Double semiAngle;
		public Geom.Native.Matrix4 matrix;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getEllipticConeSurfaceDefinitionReturn_c
	{
		public System.Double radius1;
		public System.Double radius2;
		public System.Double semiAngle;
		public Geom.Native.Matrix4_c matrix;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getFaceDefinitionReturn
	{
		public getFaceDefinitionReturn() {}
		public getFaceDefinitionReturn(getFaceDefinitionReturn o) {
			this.surface = o.surface;
			this.loops = o.loops;
			this.orientation = o.orientation;
			this.limits = o.limits;
		}
		public System.UInt32 surface;
		public LoopList loops;
		public System.Boolean orientation;
		public Bounds2D limits;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getFaceDefinitionReturn_c
	{
		public System.UInt32 surface;
		public LoopList_c loops;
		public Int32 orientation;
		public Bounds2D_c limits;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getHelixCurveDefinitionReturn
	{
		public getHelixCurveDefinitionReturn() {}
		public getHelixCurveDefinitionReturn(getHelixCurveDefinitionReturn o) {
			this.radius = o.radius;
			this.matrix = o.matrix;
			this.trigonometricOrientation = o.trigonometricOrientation;
		}
		public System.Double radius;
		public Geom.Native.Matrix4 matrix;
		public System.Boolean trigonometricOrientation;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getHelixCurveDefinitionReturn_c
	{
		public System.Double radius;
		public Geom.Native.Matrix4_c matrix;
		public Int32 trigonometricOrientation;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getHermiteCurveDefinitionReturn
	{
		public getHermiteCurveDefinitionReturn() {}
		public getHermiteCurveDefinitionReturn(getHermiteCurveDefinitionReturn o) {
			this.firstPoint = o.firstPoint;
			this.secondPoint = o.secondPoint;
			this.firstTangent = o.firstTangent;
			this.secondTangent = o.secondTangent;
		}
		public Geom.Native.Point3 firstPoint;
		public Geom.Native.Point3 secondPoint;
		public Geom.Native.Point3 firstTangent;
		public Geom.Native.Point3 secondTangent;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getHermiteCurveDefinitionReturn_c
	{
		public Geom.Native.Point3_c firstPoint;
		public Geom.Native.Point3_c secondPoint;
		public Geom.Native.Point3_c firstTangent;
		public Geom.Native.Point3_c secondTangent;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getHyperbolaCurveDefinitionReturn
	{
		public getHyperbolaCurveDefinitionReturn() {}
		public getHyperbolaCurveDefinitionReturn(getHyperbolaCurveDefinitionReturn o) {
			this.radius1 = o.radius1;
			this.radius2 = o.radius2;
			this.matrix = o.matrix;
		}
		public System.Double radius1;
		public System.Double radius2;
		public Geom.Native.Matrix4 matrix;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getHyperbolaCurveDefinitionReturn_c
	{
		public System.Double radius1;
		public System.Double radius2;
		public Geom.Native.Matrix4_c matrix;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getIntersectionCurveDefinitionReturn
	{
		public getIntersectionCurveDefinitionReturn() {}
		public getIntersectionCurveDefinitionReturn(getIntersectionCurveDefinitionReturn o) {
			this.surface1 = o.surface1;
			this.surface2 = o.surface2;
			this.chart = o.chart;
			this.bounds = o.bounds;
		}
		public System.UInt32 surface1;
		public System.UInt32 surface2;
		public System.UInt32 chart;
		public Bounds1D bounds;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getIntersectionCurveDefinitionReturn_c
	{
		public System.UInt32 surface1;
		public System.UInt32 surface2;
		public System.UInt32 chart;
		public Bounds1D_c bounds;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getLineCurveDefinitionReturn
	{
		public getLineCurveDefinitionReturn() {}
		public getLineCurveDefinitionReturn(getLineCurveDefinitionReturn o) {
			this.origin = o.origin;
			this.direction = o.direction;
		}
		public Geom.Native.Point3 origin;
		public Geom.Native.Point3 direction;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getLineCurveDefinitionReturn_c
	{
		public Geom.Native.Point3_c origin;
		public Geom.Native.Point3_c direction;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getNURBSCurveDefinitionReturn
	{
		public getNURBSCurveDefinitionReturn() {}
		public getNURBSCurveDefinitionReturn(getNURBSCurveDefinitionReturn o) {
			this.degree = o.degree;
			this.knots = o.knots;
			this.poles = o.poles;
			this.weights = o.weights;
		}
		public System.Int32 degree;
		public Core.Native.DoubleList knots;
		public Geom.Native.Point3List poles;
		public Core.Native.DoubleList weights;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getNURBSCurveDefinitionReturn_c
	{
		public Int32 degree;
		public Core.Native.DoubleList_c knots;
		public Geom.Native.Point3List_c poles;
		public Core.Native.DoubleList_c weights;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getNURBSSurfaceDefinitionReturn
	{
		public getNURBSSurfaceDefinitionReturn() {}
		public getNURBSSurfaceDefinitionReturn(getNURBSSurfaceDefinitionReturn o) {
			this.degreeU = o.degreeU;
			this.degreeV = o.degreeV;
			this.knotsU = o.knotsU;
			this.knotsV = o.knotsV;
			this.poles = o.poles;
			this.weights = o.weights;
		}
		public System.Int32 degreeU;
		public System.Int32 degreeV;
		public Core.Native.DoubleList knotsU;
		public Core.Native.DoubleList knotsV;
		public Geom.Native.Point3ListList poles;
		public Core.Native.DoubleListList weights;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getNURBSSurfaceDefinitionReturn_c
	{
		public Int32 degreeU;
		public Int32 degreeV;
		public Core.Native.DoubleList_c knotsU;
		public Core.Native.DoubleList_c knotsV;
		public Geom.Native.Point3ListList_c poles;
		public Core.Native.DoubleListList_c weights;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getOffsetCurveDefinitionReturn
	{
		public getOffsetCurveDefinitionReturn() {}
		public getOffsetCurveDefinitionReturn(getOffsetCurveDefinitionReturn o) {
			this.curve = o.curve;
			this.direction = o.direction;
			this.distance = o.distance;
			this.surfaceReference = o.surfaceReference;
		}
		public System.UInt32 curve;
		public Geom.Native.Point3 direction;
		public System.Double distance;
		public System.UInt32 surfaceReference;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getOffsetCurveDefinitionReturn_c
	{
		public System.UInt32 curve;
		public Geom.Native.Point3_c direction;
		public System.Double distance;
		public System.UInt32 surfaceReference;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct getOffsetSurfaceDefinitionReturn
	{
		public getOffsetSurfaceDefinitionReturn(getOffsetSurfaceDefinitionReturn o) {
			this.baseSurface = o.baseSurface;
			this.distance = o.distance;
		}
		public System.UInt32 baseSurface;
		public System.Double distance;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getOffsetSurfaceDefinitionReturn_c
	{
		public System.UInt32 baseSurface;
		public System.Double distance;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getParabolaCurveDefinitionReturn
	{
		public getParabolaCurveDefinitionReturn() {}
		public getParabolaCurveDefinitionReturn(getParabolaCurveDefinitionReturn o) {
			this.focalLength = o.focalLength;
			this.matrix = o.matrix;
		}
		public System.Double focalLength;
		public Geom.Native.Matrix4 matrix;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getParabolaCurveDefinitionReturn_c
	{
		public System.Double focalLength;
		public Geom.Native.Matrix4_c matrix;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getPolylineCurveDefinitionReturn
	{
		public getPolylineCurveDefinitionReturn() {}
		public getPolylineCurveDefinitionReturn(getPolylineCurveDefinitionReturn o) {
			this.points = o.points;
			this.parameters = o.parameters;
		}
		public Geom.Native.Point3List points;
		public Core.Native.DoubleList parameters;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getPolylineCurveDefinitionReturn_c
	{
		public Geom.Native.Point3List_c points;
		public Core.Native.DoubleList_c parameters;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getRevolutionSurfaceDefinitionReturn
	{
		public getRevolutionSurfaceDefinitionReturn() {}
		public getRevolutionSurfaceDefinitionReturn(getRevolutionSurfaceDefinitionReturn o) {
			this.generatricCurve = o.generatricCurve;
			this.axisOrigin = o.axisOrigin;
			this.axisDirection = o.axisDirection;
			this.startAngle = o.startAngle;
			this.endAngle = o.endAngle;
		}
		public System.UInt32 generatricCurve;
		public Geom.Native.Point3 axisOrigin;
		public Geom.Native.Point3 axisDirection;
		public System.Double startAngle;
		public System.Double endAngle;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getRevolutionSurfaceDefinitionReturn_c
	{
		public System.UInt32 generatricCurve;
		public Geom.Native.Point3_c axisOrigin;
		public Geom.Native.Point3_c axisDirection;
		public System.Double startAngle;
		public System.Double endAngle;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct getRuledSurfaceDefinitionReturn
	{
		public getRuledSurfaceDefinitionReturn(getRuledSurfaceDefinitionReturn o) {
			this.firstCurve = o.firstCurve;
			this.secondCurve = o.secondCurve;
		}
		public System.UInt32 firstCurve;
		public System.UInt32 secondCurve;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getRuledSurfaceDefinitionReturn_c
	{
		public System.UInt32 firstCurve;
		public System.UInt32 secondCurve;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getSegmentCurveDefinitionReturn
	{
		public getSegmentCurveDefinitionReturn() {}
		public getSegmentCurveDefinitionReturn(getSegmentCurveDefinitionReturn o) {
			this.startPoint = o.startPoint;
			this.endPoint = o.endPoint;
		}
		public Geom.Native.Point3 startPoint;
		public Geom.Native.Point3 endPoint;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getSegmentCurveDefinitionReturn_c
	{
		public Geom.Native.Point3_c startPoint;
		public Geom.Native.Point3_c endPoint;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getSphereSurfaceDefinitionReturn
	{
		public getSphereSurfaceDefinitionReturn() {}
		public getSphereSurfaceDefinitionReturn(getSphereSurfaceDefinitionReturn o) {
			this.radius = o.radius;
			this.matrix = o.matrix;
		}
		public System.Double radius;
		public Geom.Native.Matrix4 matrix;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getSphereSurfaceDefinitionReturn_c
	{
		public System.Double radius;
		public Geom.Native.Matrix4_c matrix;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct getSurfacicCurveDefinitionReturn
	{
		public getSurfacicCurveDefinitionReturn(getSurfacicCurveDefinitionReturn o) {
			this.surface = o.surface;
			this.curve2D = o.curve2D;
		}
		public System.UInt32 surface;
		public System.UInt32 curve2D;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getSurfacicCurveDefinitionReturn_c
	{
		public System.UInt32 surface;
		public System.UInt32 curve2D;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getTabulatedCylinderSurfaceDefinitionReturn
	{
		public getTabulatedCylinderSurfaceDefinitionReturn() {}
		public getTabulatedCylinderSurfaceDefinitionReturn(getTabulatedCylinderSurfaceDefinitionReturn o) {
			this.directrixCurve = o.directrixCurve;
			this.generatrixLine = o.generatrixLine;
			this.range = o.range;
		}
		public System.UInt32 directrixCurve;
		public Geom.Native.Point3 generatrixLine;
		public Bounds1D range;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getTabulatedCylinderSurfaceDefinitionReturn_c
	{
		public System.UInt32 directrixCurve;
		public Geom.Native.Point3_c generatrixLine;
		public Bounds1D_c range;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getTorusSurfaceDefinitionReturn
	{
		public getTorusSurfaceDefinitionReturn() {}
		public getTorusSurfaceDefinitionReturn(getTorusSurfaceDefinitionReturn o) {
			this.majorRadius = o.majorRadius;
			this.minorRadius = o.minorRadius;
			this.matrix = o.matrix;
		}
		public System.Double majorRadius;
		public System.Double minorRadius;
		public Geom.Native.Matrix4 matrix;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getTorusSurfaceDefinitionReturn_c
	{
		public System.Double majorRadius;
		public System.Double minorRadius;
		public Geom.Native.Matrix4_c matrix;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getTransformedCurveDefinitionReturn
	{
		public getTransformedCurveDefinitionReturn() {}
		public getTransformedCurveDefinitionReturn(getTransformedCurveDefinitionReturn o) {
			this.curve = o.curve;
			this.matrix = o.matrix;
		}
		public System.UInt32 curve;
		public Geom.Native.Matrix4 matrix;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getTransformedCurveDefinitionReturn_c
	{
		public System.UInt32 curve;
		public Geom.Native.Matrix4_c matrix;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct isCurvePeriodicReturn
	{
		public isCurvePeriodicReturn(isCurvePeriodicReturn o) {
			this.periodic = o.periodic;
			this.period = o.period;
		}
		public System.Boolean periodic;
		public System.Double period;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct isCurvePeriodicReturn_c
	{
		public Int32 periodic;
		public System.Double period;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct isSurfaceClosedReturn
	{
		public isSurfaceClosedReturn(isSurfaceClosedReturn o) {
			this.closedU = o.closedU;
			this.closedV = o.closedV;
		}
		public System.Boolean closedU;
		public System.Boolean closedV;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct isSurfaceClosedReturn_c
	{
		public Int32 closedU;
		public Int32 closedV;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct isSurfacePeriodicReturn
	{
		public isSurfacePeriodicReturn(isSurfacePeriodicReturn o) {
			this.periodicU = o.periodicU;
			this.periodicV = o.periodicV;
			this.periodU = o.periodU;
			this.periodV = o.periodV;
		}
		public System.Boolean periodicU;
		public System.Boolean periodicV;
		public System.Double periodU;
		public System.Double periodV;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct isSurfacePeriodicReturn_c
	{
		public Int32 periodicU;
		public Int32 periodicV;
		public System.Double periodU;
		public System.Double periodV;
	}

}
