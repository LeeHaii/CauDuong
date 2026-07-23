#pragma warning disable CA2101

using System;
using System.Security;
using System.Runtime.InteropServices;

namespace Pixyz.Polygonal.Native {

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct DressedPoly
	{
		public DressedPoly(DressedPoly o) {
			this.material = o.material;
			this.firstTri = o.firstTri;
			this.triCount = o.triCount;
			this.firstQuad = o.firstQuad;
			this.quadCount = o.quadCount;
			this.externalId = o.externalId;
		}
		public System.UInt32 material;
		public System.Int32 firstTri;
		public System.Int32 triCount;
		public System.Int32 firstQuad;
		public System.Int32 quadCount;
		public System.UInt32 externalId;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DressedPoly_c
	{
		public System.UInt32 material;
		public Int32 firstTri;
		public Int32 triCount;
		public Int32 firstQuad;
		public Int32 quadCount;
		public System.UInt32 externalId;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class DressedPolyList {
		public Polygonal.Native.DressedPoly[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public DressedPolyList(Polygonal.Native.DressedPoly[] tab) { list = tab; }
		public static implicit operator Polygonal.Native.DressedPoly[](DressedPolyList o) { return o.list; }
		public Polygonal.Native.DressedPoly this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public DressedPolyList(int size) { list = new Polygonal.Native.DressedPoly[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DressedPolyList_c
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
		public Polygonal.Native.EdgeList[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public EdgeListList(Polygonal.Native.EdgeList[] tab) { list = tab; }
		public static implicit operator Polygonal.Native.EdgeList[](EdgeListList o) { return o.list; }
		public Polygonal.Native.EdgeList this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public EdgeListList(int size) { list = new Polygonal.Native.EdgeList[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct EdgeListList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class ElementList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public ElementList() {}
		public ElementList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](ElementList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public ElementList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ElementList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class JointList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public JointList() {}
		public JointList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](JointList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public JointList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct JointList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	public enum StyleType
	{
		NORMAL = 0,
		STIPPLE = 1,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class StylizedLine
	{
		public StylizedLine() {}
		public StylizedLine(StylizedLine o) {
			this.lines = o.lines;
			this.width = o.width;
			this.type = o.type;
			this.pattern = o.pattern;
			this.color = o.color;
			this.externalId = o.externalId;
		}
		public Core.Native.IntList lines;
		public System.Double width;
		public StyleType type;
		public System.Int32 pattern;
		public Core.Native.ColorAlpha color;
		public System.UInt32 externalId;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct StylizedLine_c
	{
		public Core.Native.IntList_c lines;
		public System.Double width;
		public Int32 type;
		public Int32 pattern;
		public Core.Native.ColorAlpha_c color;
		public System.UInt32 externalId;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class StylizedLineList {
		public Polygonal.Native.StylizedLine[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public StylizedLineList(Polygonal.Native.StylizedLine[] tab) { list = tab; }
		public static implicit operator Polygonal.Native.StylizedLine[](StylizedLineList o) { return o.list; }
		public Polygonal.Native.StylizedLine this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public StylizedLineList(int size) { list = new Polygonal.Native.StylizedLine[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct StylizedLineList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class MeshDefinition
	{
		public MeshDefinition() {}
		public MeshDefinition(MeshDefinition o) {
			this.id = o.id;
			this.externalId = o.externalId;
			this.vertices = o.vertices;
			this.normals = o.normals;
			this.tangents = o.tangents;
			this.uvChannels = o.uvChannels;
			this.uvs = o.uvs;
			this.vertexColors = o.vertexColors;
			this.curvatures = o.curvatures;
			this.triangles = o.triangles;
			this.quadrangles = o.quadrangles;
			this.vertexMerged = o.vertexMerged;
			this.dressedPolys = o.dressedPolys;
			this.linesVertices = o.linesVertices;
			this.lines = o.lines;
			this.points = o.points;
			this.pointsColors = o.pointsColors;
			this.joints = o.joints;
			this.inverseBindMatrices = o.inverseBindMatrices;
			this.jointWeights = o.jointWeights;
			this.jointIndices = o.jointIndices;
		}
		public System.UInt32 id;
		public System.UInt32 externalId;
		public Geom.Native.Point3List vertices;
		public Geom.Native.Vector3List normals;
		public Geom.Native.Vector4List tangents;
		public Core.Native.IntList uvChannels;
		public Geom.Native.Point2ListList uvs;
		public Core.Native.ColorAlphaList vertexColors;
		public Geom.Native.CurvaturesList curvatures;
		public Core.Native.IntList triangles;
		public Core.Native.IntList quadrangles;
		public Core.Native.IntList vertexMerged;
		public DressedPolyList dressedPolys;
		public Geom.Native.Point3List linesVertices;
		public StylizedLineList lines;
		public Geom.Native.Point3List points;
		public Geom.Native.Vector3List pointsColors;
		public JointList joints;
		public Geom.Native.Matrix4List inverseBindMatrices;
		public Geom.Native.Vector4List jointWeights;
		public Geom.Native.Vector4IList jointIndices;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MeshDefinition_c
	{
		public System.UInt32 id;
		public System.UInt32 externalId;
		public Geom.Native.Point3List_c vertices;
		public Geom.Native.Vector3List_c normals;
		public Geom.Native.Vector4List_c tangents;
		public Core.Native.IntList_c uvChannels;
		public Geom.Native.Point2ListList_c uvs;
		public Core.Native.ColorAlphaList_c vertexColors;
		public Geom.Native.CurvaturesList_c curvatures;
		public Core.Native.IntList_c triangles;
		public Core.Native.IntList_c quadrangles;
		public Core.Native.IntList_c vertexMerged;
		public DressedPolyList_c dressedPolys;
		public Geom.Native.Point3List_c linesVertices;
		public StylizedLineList_c lines;
		public Geom.Native.Point3List_c points;
		public Geom.Native.Vector3List_c pointsColors;
		public JointList_c joints;
		public Geom.Native.Matrix4List_c inverseBindMatrices;
		public Geom.Native.Vector4List_c jointWeights;
		public Geom.Native.Vector4IList_c jointIndices;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class MeshDefinitionList {
		public Polygonal.Native.MeshDefinition[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public MeshDefinitionList(Polygonal.Native.MeshDefinition[] tab) { list = tab; }
		public static implicit operator Polygonal.Native.MeshDefinition[](MeshDefinitionList o) { return o.list; }
		public Polygonal.Native.MeshDefinition this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public MeshDefinitionList(int size) { list = new Polygonal.Native.MeshDefinition[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MeshDefinitionList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class MeshList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public MeshList() {}
		public MeshList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](MeshList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public MeshList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MeshList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class PatchList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public PatchList() {}
		public PatchList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](PatchList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public PatchList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PatchList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class PlaceholderJointList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public PlaceholderJointList() {}
		public PlaceholderJointList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](PlaceholderJointList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public PlaceholderJointList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PlaceholderJointList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class PolygonList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public PolygonList() {}
		public PolygonList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](PolygonList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public PolygonList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PolygonList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class PolygonListList {
		public Polygonal.Native.PolygonList[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public PolygonListList(Polygonal.Native.PolygonList[] tab) { list = tab; }
		public static implicit operator Polygonal.Native.PolygonList[](PolygonListList o) { return o.list; }
		public Polygonal.Native.PolygonList this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public PolygonListList(int size) { list = new Polygonal.Native.PolygonList[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PolygonListList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class TessellationList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public TessellationList() {}
		public TessellationList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](TessellationList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public TessellationList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct TessellationList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	public enum TopologyDimensionMask
	{
		NONE = 0,
		VERTEX = 1,
		LINE = 2,
		FACE = 4,
		ALL = 7,
	}

	public enum TopologyConnectivityMask
	{
		NONE = 0,
		MANIFOLD = 1,
		BOUNDARY = 2,
		NONMANIFOLD = 4,
		BOUNDARY_NONMANIFOLD = 6,
		FREE = 8,
		ALL = 15,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class TopologyCategoryMask
	{
		public TopologyCategoryMask() {}
		public TopologyCategoryMask(TopologyCategoryMask o) {
			this.dimension = o.dimension;
			this.connectivity = o.connectivity;
		}
		public TopologyDimensionMask dimension;
		public TopologyConnectivityMask connectivity;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct TopologyCategoryMask_c
	{
		public Int32 dimension;
		public Int32 connectivity;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class UVCoordList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public UVCoordList() {}
		public UVCoordList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](UVCoordList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public UVCoordList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct UVCoordList_c
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
	public class VertexListList {
		public Polygonal.Native.VertexList[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public VertexListList(Polygonal.Native.VertexList[] tab) { list = tab; }
		public static implicit operator Polygonal.Native.VertexList[](VertexListList o) { return o.list; }
		public Polygonal.Native.VertexList this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public VertexListList(int size) { list = new Polygonal.Native.VertexList[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct VertexListList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class dracoEncodeReturn
	{
		public dracoEncodeReturn() {}
		public dracoEncodeReturn(dracoEncodeReturn o) {
			this.buffer = o.buffer;
			this.jointIndicesId = o.jointIndicesId;
			this.jointWeightsId = o.jointWeightsId;
		}
		public Core.Native.ByteList buffer;
		public System.Int32 jointIndicesId;
		public System.Int32 jointWeightsId;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct dracoEncodeReturn_c
	{
		public Core.Native.ByteList_c buffer;
		public Int32 jointIndicesId;
		public Int32 jointWeightsId;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getVisiblePolygonsReturn
	{
		public getVisiblePolygonsReturn() {}
		public getVisiblePolygonsReturn(getVisiblePolygonsReturn o) {
			this.polygons = o.polygons;
			this.pixelCounts = o.pixelCounts;
		}
		public PolygonList polygons;
		public Core.Native.IntList pixelCounts;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getVisiblePolygonsReturn_c
	{
		public PolygonList_c polygons;
		public Core.Native.IntList_c pixelCounts;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getMeshSkinningReturn
	{
		public getMeshSkinningReturn() {}
		public getMeshSkinningReturn(getMeshSkinningReturn o) {
			this.joints = o.joints;
			this.IBMs = o.IBMs;
		}
		public JointList joints;
		public Geom.Native.Matrix4List IBMs;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getMeshSkinningReturn_c
	{
		public JointList_c joints;
		public Geom.Native.Matrix4List_c IBMs;
	}

}
