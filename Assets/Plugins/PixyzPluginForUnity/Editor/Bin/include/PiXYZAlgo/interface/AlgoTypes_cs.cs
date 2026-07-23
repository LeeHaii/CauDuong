#pragma warning disable CA2101

using System;
using System.Security;
using System.Runtime.InteropServices;

namespace Pixyz.Algo.Native {

	public enum AttributType
	{
		Patches = 4020,
		PatchMaterials = 4021,
		PatchBoundaries = 4022,
		UVs = 4035,
		Normals = 4036,
		Tangents = 4037,
		Binormals = 4038,
	}

	public enum MapType
	{
		Diffuse = 0,
		Normal = 1,
		Opacity = 2,
		Roughness = 3,
		Specular = 4,
		Metallic = 5,
		AO = 6,
		Emissive = 7,
		PartId = 8,
		MaterialId = 9,
		ComputeAO = 10,
		Bent = 11,
		UV = 12,
		Displacement = 13,
		LocalPosition = 14,
		GlobalPosition = 15,
		Depth = 16,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class BakeMap
	{
		public BakeMap() {}
		public BakeMap(BakeMap o) {
			this.type = o.type;
			this.properties = o.properties;
		}
		public MapType type;
		public Scene.Native.PropertyValueList properties;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct BakeMap_c
	{
		public Int32 type;
		public Scene.Native.PropertyValueList_c properties;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class BakeMapList {
		public Algo.Native.BakeMap[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public BakeMapList(Algo.Native.BakeMap[] tab) { list = tab; }
		public static implicit operator Algo.Native.BakeMap[](BakeMapList o) { return o.list; }
		public Algo.Native.BakeMap this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public BakeMapList(int size) { list = new Algo.Native.BakeMap[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct BakeMapList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct BakeMaps
	{
		public BakeMaps(BakeMaps o) {
			this.diffuse = o.diffuse;
			this.normal = o.normal;
			this.roughness = o.roughness;
			this.metallic = o.metallic;
			this.opacity = o.opacity;
			this.ambientOcclusion = o.ambientOcclusion;
			this.emissive = o.emissive;
		}
		public System.Boolean diffuse;
		public System.Boolean normal;
		public System.Boolean roughness;
		public System.Boolean metallic;
		public System.Boolean opacity;
		public System.Boolean ambientOcclusion;
		public System.Boolean emissive;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct BakeMaps_c
	{
		public Int32 diffuse;
		public Int32 normal;
		public Int32 roughness;
		public Int32 metallic;
		public Int32 opacity;
		public Int32 ambientOcclusion;
		public Int32 emissive;
	}

	public enum BakingMethod
	{
		RayOnly = 0,
		ProjOnly = 1,
		RayAndProj = 2,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class BakeOption
	{
		public BakeOption() {}
		public BakeOption(BakeOption o) {
			this.bakingMethod = o.bakingMethod;
			this.resolution = o.resolution;
			this.padding = o.padding;
			this.textures = o.textures;
		}
		public BakingMethod bakingMethod;
		public System.Int32 resolution;
		public System.Int32 padding;
		public BakeMaps textures;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct BakeOption_c
	{
		public Int32 bakingMethod;
		public Int32 resolution;
		public Int32 padding;
		public BakeMaps_c textures;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class Box
	{
		public Box() {}
		public Box(Box o) {
			this.position = o.position;
			this.length = o.length;
			this.height = o.height;
			this.depth = o.depth;
		}
		public Geom.Native.Affine position;
		public System.Double length;
		public System.Double height;
		public System.Double depth;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Box_c
	{
		public Geom.Native.Affine_c position;
		public System.Double length;
		public System.Double height;
		public System.Double depth;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct BoxParameters
	{
		public BoxParameters(BoxParameters o) {
			this.SizeX = o.SizeX;
			this.SizeY = o.SizeY;
			this.SizeZ = o.SizeZ;
			this.Subdivision = o.Subdivision;
		}
		public System.Double SizeX;
		public System.Double SizeY;
		public System.Double SizeZ;
		public System.Int32 Subdivision;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct BoxParameters_c
	{
		public System.Double SizeX;
		public System.Double SizeY;
		public System.Double SizeZ;
		public Int32 Subdivision;
	}

	public enum ComputingQuality
	{
		High = 0,
		Medium = 1,
		Low = 2,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct ConeParameters
	{
		public ConeParameters(ConeParameters o) {
			this.BottomRadius = o.BottomRadius;
			this.Height = o.Height;
			this.Sides = o.Sides;
		}
		public System.Double BottomRadius;
		public System.Double Height;
		public System.Int32 Sides;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ConeParameters_c
	{
		public System.Double BottomRadius;
		public System.Double Height;
		public Int32 Sides;
	}

	public enum ConvexityConstraint
	{
		REMOVE_CONVEX_ONLY = 0,
		REMOVE_CONCAVE_ONLY = 1,
		REMOVE_BOTH = 2,
	}

	public enum CostEvaluation
	{
		SumEvaluation = 0,
		MaxEvaluation = 1,
		AverageEvaluation = 2,
	}

	public enum CreateOccluder
	{
		Occludee = 0,
		Occluder = 1,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct CustomBakeMap
	{
		public CustomBakeMap(CustomBakeMap o) {
			this.name = o.name;
			this.component = o.component;
		}
		public System.String name;
		public System.Int32 component;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct CustomBakeMap_c
	{
		public IntPtr name;
		public Int32 component;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class CustomBakeMapList {
		public Algo.Native.CustomBakeMap[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public CustomBakeMapList(Algo.Native.CustomBakeMap[] tab) { list = tab; }
		public static implicit operator Algo.Native.CustomBakeMap[](CustomBakeMapList o) { return o.list; }
		public Algo.Native.CustomBakeMap this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public CustomBakeMapList(int size) { list = new Algo.Native.CustomBakeMap[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct CustomBakeMapList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class Cylinder
	{
		public Cylinder() {}
		public Cylinder(Cylinder o) {
			this.position = o.position;
			this.radius = o.radius;
			this.length = o.length;
		}
		public Geom.Native.Affine position;
		public System.Double radius;
		public System.Double length;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Cylinder_c
	{
		public Geom.Native.Affine_c position;
		public System.Double radius;
		public System.Double length;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct CylinderParameters
	{
		public CylinderParameters(CylinderParameters o) {
			this.Radius = o.Radius;
			this.Height = o.Height;
			this.Sides = o.Sides;
		}
		public System.Double Radius;
		public System.Double Height;
		public System.Int32 Sides;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct CylinderParameters_c
	{
		public System.Double Radius;
		public System.Double Height;
		public Int32 Sides;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct DecimateOptionsSelector
	{
		public enum Type
		{
			UNKNOWN = 0,
			TRIANGLECOUNT = 1,
			RATIO = 2,
		}
		public System.Int32 triangleCount;
		public System.Double ratio;
		public Type _type;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DecimateOptionsSelector_c
	{
		public Int32 triangleCount;
		public System.Double ratio;
		public int _type;
	}

	public enum ElementFilter
	{
		Polygons = 0,
		Points = 1,
		Hybrid = 2,
	}

	public enum FeatureType
	{
		Unknown = 0,
		ThroughHole = 1,
		BlindHole = 2,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class FeatureInput
	{
		public FeatureInput() {}
		public FeatureInput(FeatureInput o) {
			this.position = o.position;
			this.direction = o.direction;
			this.diameter = o.diameter;
		}
		public Geom.Native.Point3 position;
		public Geom.Native.Point3 direction;
		public System.Double diameter;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct FeatureInput_c
	{
		public Geom.Native.Point3_c position;
		public Geom.Native.Point3_c direction;
		public System.Double diameter;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class FeatureInputList {
		public Algo.Native.FeatureInput[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public FeatureInputList(Algo.Native.FeatureInput[] tab) { list = tab; }
		public static implicit operator Algo.Native.FeatureInput[](FeatureInputList o) { return o.list; }
		public Algo.Native.FeatureInput this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public FeatureInputList(int size) { list = new Algo.Native.FeatureInput[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct FeatureInputList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class Feature
	{
		public Feature() {}
		public Feature(Feature o) {
			this.type = o.type;
			this.inputs = o.inputs;
		}
		public FeatureType type;
		public FeatureInputList inputs;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Feature_c
	{
		public Int32 type;
		public FeatureInputList_c inputs;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class FeatureList {
		public Algo.Native.Feature[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public FeatureList(Algo.Native.Feature[] tab) { list = tab; }
		public static implicit operator Algo.Native.Feature[](FeatureList o) { return o.list; }
		public Algo.Native.Feature this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public FeatureList(int size) { list = new Algo.Native.Feature[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct FeatureList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	public enum FlatteningStopCondition
	{
		MEAN_DEFORMATION = 0,
		ABSOLUTE_DEFORMATION = 1,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct HexahedronParameters
	{
		public HexahedronParameters(HexahedronParameters o) {
			this.XLength = o.XLength;
			this.YLength = o.YLength;
			this.ZLength = o.ZLength;
		}
		public System.Double XLength;
		public System.Double YLength;
		public System.Double ZLength;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct HexahedronParameters_c
	{
		public System.Double XLength;
		public System.Double YLength;
		public System.Double ZLength;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class MapTypeList {
		public Algo.Native.MapType[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public MapTypeList(Algo.Native.MapType[] tab) { list = tab; }
		public static implicit operator Algo.Native.MapType[](MapTypeList o) { return o.list; }
		public Algo.Native.MapType this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public MapTypeList(int size) { list = new Algo.Native.MapType[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MapTypeList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class OccurrenceFeatures
	{
		public OccurrenceFeatures() {}
		public OccurrenceFeatures(OccurrenceFeatures o) {
			this.occurrence = o.occurrence;
			this.features = o.features;
		}
		public System.UInt32 occurrence;
		public FeatureList features;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct OccurrenceFeatures_c
	{
		public System.UInt32 occurrence;
		public FeatureList_c features;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class OccurrenceFeaturesList {
		public Algo.Native.OccurrenceFeatures[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public OccurrenceFeaturesList(Algo.Native.OccurrenceFeatures[] tab) { list = tab; }
		public static implicit operator Algo.Native.OccurrenceFeatures[](OccurrenceFeaturesList o) { return o.list; }
		public Algo.Native.OccurrenceFeatures this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public OccurrenceFeaturesList(int size) { list = new Algo.Native.OccurrenceFeatures[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct OccurrenceFeaturesList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class OctahedralImpostor
	{
		public OctahedralImpostor() {}
		public OctahedralImpostor(OctahedralImpostor o) {
			this.OctaTransform = o.OctaTransform;
			this.Radius = o.Radius;
			this.NormalMap = o.NormalMap;
			this.DepthMap = o.DepthMap;
			this.DiffuseMap = o.DiffuseMap;
			this.MetallicMap = o.MetallicMap;
			this.AOMap = o.AOMap;
			this.RoughnessMap = o.RoughnessMap;
		}
		public Geom.Native.Matrix4 OctaTransform;
		public System.Double Radius;
		public System.UInt32 NormalMap;
		public System.UInt32 DepthMap;
		public System.UInt32 DiffuseMap;
		public System.UInt32 MetallicMap;
		public System.UInt32 AOMap;
		public System.UInt32 RoughnessMap;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct OctahedralImpostor_c
	{
		public Geom.Native.Matrix4_c OctaTransform;
		public System.Double Radius;
		public System.UInt32 NormalMap;
		public System.UInt32 DepthMap;
		public System.UInt32 DiffuseMap;
		public System.UInt32 MetallicMap;
		public System.UInt32 AOMap;
		public System.UInt32 RoughnessMap;
	}

	public enum OrientStrategy
	{
		ExteriorOnly = 0,
		VisibilityOrExterior = 1,
		VisibilityByConnected = 2,
		VisibilityByPolygon = 3,
		ConformToMajority = 4,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class Plane
	{
		public Plane() {}
		public Plane(Plane o) {
			this.position = o.position;
			this.length = o.length;
			this.height = o.height;
		}
		public Geom.Native.Affine position;
		public System.Double length;
		public System.Double height;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Plane_c
	{
		public Geom.Native.Affine_c position;
		public System.Double length;
		public System.Double height;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct PlaneParameters
	{
		public PlaneParameters(PlaneParameters o) {
			this.SizeX = o.SizeX;
			this.SizeY = o.SizeY;
			this.SubdivisionX = o.SubdivisionX;
			this.SubdivisionY = o.SubdivisionY;
		}
		public System.Double SizeX;
		public System.Double SizeY;
		public System.Int32 SubdivisionX;
		public System.Int32 SubdivisionY;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PlaneParameters_c
	{
		public System.Double SizeX;
		public System.Double SizeY;
		public Int32 SubdivisionX;
		public Int32 SubdivisionY;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct SphereParameters
	{
		public SphereParameters(SphereParameters o) {
			this.Radius = o.Radius;
			this.Latitude = o.Latitude;
			this.Longitude = o.Longitude;
		}
		public System.Double Radius;
		public System.Int32 Latitude;
		public System.Int32 Longitude;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SphereParameters_c
	{
		public System.Double Radius;
		public Int32 Latitude;
		public Int32 Longitude;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct PrimitiveShapeParameters
	{
		public enum Type
		{
			UNKNOWN = 0,
			BOX = 1,
			PLANE = 2,
			SPHERE = 3,
			CYLINDER = 4,
			CONE = 5,
		}
		public BoxParameters Box;
		public PlaneParameters Plane;
		public SphereParameters Sphere;
		public CylinderParameters Cylinder;
		public ConeParameters Cone;
		public Type _type;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PrimitiveShapeParameters_c
	{
		public BoxParameters_c Box;
		public PlaneParameters_c Plane;
		public SphereParameters_c Sphere;
		public CylinderParameters_c Cylinder;
		public ConeParameters_c Cone;
		public int _type;
	}

	public enum QualityMemoryTradeoff
	{
		PreferQuality = 0,
		PreferMemory = 1,
	}

	public enum QualitySpeedTradeoff
	{
		PreferQuality = 0,
		PreferSpeed = 1,
	}

	public enum RelaxUVMethod
	{
		Angle = 0,
		Edge = 1,
	}

	public enum ReplaceByBoxType
	{
		Minimum = 0,
		LocallyAligned = 1,
	}

	public enum ReplaceByMode
	{
		ByOccurrence = 0,
		All = 1,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct ReplaceByOccurrenceOptions
	{
		public ReplaceByOccurrenceOptions(ReplaceByOccurrenceOptions o) {
			this.Occurrence = o.Occurrence;
			this.Aligned = o.Aligned;
		}
		public System.UInt32 Occurrence;
		public System.Boolean Aligned;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ReplaceByOccurrenceOptions_c
	{
		public System.UInt32 Occurrence;
		public Int32 Aligned;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class ReplaceByPrimitiveOptions
	{
		public ReplaceByPrimitiveOptions() {}
		public ReplaceByPrimitiveOptions(ReplaceByPrimitiveOptions o) {
			this.Type = o.Type;
			this.Aligned = o.Aligned;
			this.GenerateUV = o.GenerateUV;
		}
		public PrimitiveShapeParameters Type;
		public System.Boolean Aligned;
		public System.Boolean GenerateUV;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ReplaceByPrimitiveOptions_c
	{
		public PrimitiveShapeParameters_c Type;
		public Int32 Aligned;
		public Int32 GenerateUV;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct ReplaceByOption
	{
		public enum Type
		{
			UNKNOWN = 0,
			OCCURRENCE = 1,
			BOUNDINGBOX = 2,
			CONVEXHULL = 3,
			PRIMITIVE = 4,
		}
		public ReplaceByOccurrenceOptions Occurrence;
		public ReplaceByBoxType BoundingBox;
		public System.Int32 ConvexHull;
		public ReplaceByPrimitiveOptions Primitive;
		public Type _type;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ReplaceByOption_c
	{
		public ReplaceByOccurrenceOptions_c Occurrence;
		public Int32 BoundingBox;
		public Int32 ConvexHull;
		public ReplaceByPrimitiveOptions_c Primitive;
		public int _type;
	}

	public enum SawingMode
	{
		SawOnly = 0,
		SawAndSplit = 1,
		KeepInside = 2,
		KeepOutside = 3,
	}

	public enum SelectionLevel
	{
		Parts = 0,
		Patches = 1,
		Polygons = 2,
	}

	public enum SmartHiddenType
	{
		All = 0,
		OnlyOuter = 1,
		OnlyInners = 2,
	}

	public enum SmartOrientStrategy
	{
		VisibilityByConnected = 0,
		VisibilityByPolygon = 1,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class Sphere
	{
		public Sphere() {}
		public Sphere(Sphere o) {
			this.position = o.position;
			this.radius = o.radius;
		}
		public Geom.Native.Affine position;
		public System.Double radius;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Sphere_c
	{
		public Geom.Native.Affine_c position;
		public System.Double radius;
	}

	public enum UVGenerationMode
	{
		NoUV = 0,
		IntrinsicUV = 1,
		ConformalScaledUV = 2,
	}

	public enum UVImportanceEnum
	{
		PreserveSeamsAndReduceDeformation = 0,
		PreserveSeams = 1,
		IgnoreUV = 2,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct UseColorOption
	{
		public enum Type
		{
			UNKNOWN = 0,
			USELINESCOLOR = 1,
			CHOOSECOLOR = 2,
		}
		public System.Int32 UseLinesColor;
		public Core.Native.Color ChooseColor;
		public Type _type;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct UseColorOption_c
	{
		public Int32 UseLinesColor;
		public Core.Native.Color_c ChooseColor;
		public int _type;
	}

	//C# delegate
	public delegate Core.Native.ColorAlpha getPixelValue(Geom.Native.Point3 fromPos, Geom.Native.Point2 param, System.UInt64 polygonIndex, System.UInt32 occurrence);

	//C delegate
	public delegate Core.Native.ColorAlpha_c getPixelValue_c(Geom.Native.Point3_c fromPos, Geom.Native.Point2_c param, System.UInt64 polygonIndex, System.UInt32 occurrence);

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getPixelValueList {
		public Algo.Native.getPixelValue[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public getPixelValueList(Algo.Native.getPixelValue[] tab) { list = tab; }
		public static implicit operator Algo.Native.getPixelValue[](getPixelValueList o) { return o.list; }
		public Algo.Native.getPixelValue this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public getPixelValueList(int size) { list = new Algo.Native.getPixelValue[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getPixelValueList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct getVisibilityStatsReturn
	{
		public getVisibilityStatsReturn(getVisibilityStatsReturn o) {
			this.visibleCountFront = o.visibleCountFront;
			this.visibleCountBack = o.visibleCountBack;
		}
		public System.Int32 visibleCountFront;
		public System.Int32 visibleCountBack;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getVisibilityStatsReturn_c
	{
		public Int32 visibleCountFront;
		public Int32 visibleCountBack;
	}

}
