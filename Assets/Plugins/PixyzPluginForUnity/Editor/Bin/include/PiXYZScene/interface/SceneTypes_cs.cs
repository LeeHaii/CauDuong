#pragma warning disable CA2101

using System;
using System.Security;
using System.Runtime.InteropServices;

namespace Pixyz.Scene.Native {

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class AlternativeTreeList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public AlternativeTreeList() {}
		public AlternativeTreeList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](AlternativeTreeList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public AlternativeTreeList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct AlternativeTreeList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class AnimChannelList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public AnimChannelList() {}
		public AnimChannelList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](AnimChannelList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public AnimChannelList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct AnimChannelList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class AnimationList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public AnimationList() {}
		public AnimationList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](AnimationList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public AnimationList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct AnimationList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class AnnotationGroupList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public AnnotationGroupList() {}
		public AnnotationGroupList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](AnnotationGroupList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public AnnotationGroupList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct AnnotationGroupList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class AnnotationList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public AnnotationList() {}
		public AnnotationList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](AnnotationList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public AnnotationList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct AnnotationList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class Camera
	{
		public Camera() {}
		public Camera(Camera o) {
			this.position = o.position;
			this.direction = o.direction;
			this.up = o.up;
			this.fov = o.fov;
		}
		public Geom.Native.Point3 position;
		public Geom.Native.Point3 direction;
		public Geom.Native.Point3 up;
		public System.Double fov;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Camera_c
	{
		public Geom.Native.Point3_c position;
		public Geom.Native.Point3_c direction;
		public Geom.Native.Point3_c up;
		public System.Double fov;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class ComponentList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public ComponentList() {}
		public ComponentList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](ComponentList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public ComponentList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ComponentList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	public enum ComponentType
	{
		Part = 0,
		PMI = 1,
		Light = 2,
		VisualBehavior = 3,
		InteractionBehavior = 4,
		Metadata = 5,
		Variant = 6,
		Animation = 7,
		Joint = 8,
		Widget = 9,
		OoCComponent = 10,
		LODComponent = 11,
		ExternalDataComponent = 12,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Filter
	{
		public Filter(Filter o) {
			this.id = o.id;
			this.name = o.name;
			this.expr = o.expr;
		}
		public System.UInt32 id;
		public System.String name;
		public System.String expr;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Filter_c
	{
		public System.UInt32 id;
		public IntPtr name;
		public IntPtr expr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class FilterList {
		public Scene.Native.Filter[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public FilterList(Scene.Native.Filter[] tab) { list = tab; }
		public static implicit operator Scene.Native.Filter[](FilterList o) { return o.list; }
		public Scene.Native.Filter this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public FilterList(int size) { list = new Scene.Native.Filter[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct FilterList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class KeyframeList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public KeyframeList() {}
		public KeyframeList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](KeyframeList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public KeyframeList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct KeyframeList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class LODComponentList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public LODComponentList() {}
		public LODComponentList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](LODComponentList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public LODComponentList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct LODComponentList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class LODList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public LODList() {}
		public LODList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](LODList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public LODList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct LODList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct MergeByRegionsStrategy
	{
		public enum Type
		{
			UNKNOWN = 0,
			NUMBEROFREGIONS = 1,
			SIZEOFREGIONS = 2,
		}
		public System.Int32 NumberOfRegions;
		public System.Double SizeOfRegions;
		public Type _type;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MergeByRegionsStrategy_c
	{
		public Int32 NumberOfRegions;
		public System.Double SizeOfRegions;
		public int _type;
	}

	public enum MergeHiddenPartsMode
	{
		Destroy = 0,
		MakeVisible = 1,
		MergeSeparately = 2,
	}

	public enum MergeStrategy
	{
		MergeParts = 0,
		MergeByMaterials = 1,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct PropertyValue
	{
		public PropertyValue(PropertyValue o) {
			this.name = o.name;
			this.value = o.value;
		}
		public System.String name;
		public System.String value;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PropertyValue_c
	{
		public IntPtr name;
		public IntPtr value;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class PropertyValueList {
		public Scene.Native.PropertyValue[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public PropertyValueList(Scene.Native.PropertyValue[] tab) { list = tab; }
		public static implicit operator Scene.Native.PropertyValue[](PropertyValueList o) { return o.list; }
		public Scene.Native.PropertyValue this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public PropertyValueList(int size) { list = new Scene.Native.PropertyValue[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PropertyValueList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct MetadataDefinition {
		public MetadataDefinition(PropertyValueList value) { this._base = value; }
		public static implicit operator PropertyValueList(MetadataDefinition v) { return v._base; }
		public static implicit operator MetadataDefinition(PropertyValueList v) { return new MetadataDefinition(v); }
		public PropertyValueList _base;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class MetadataDefinitionList {
		public Scene.Native.MetadataDefinition[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public MetadataDefinitionList(Scene.Native.MetadataDefinition[] tab) { list = tab; }
		public static implicit operator Scene.Native.MetadataDefinition[](MetadataDefinitionList o) { return o.list; }
		public Scene.Native.MetadataDefinition this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public MetadataDefinitionList(int size) { list = new Scene.Native.MetadataDefinition[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MetadataDefinitionList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class MetadataList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public MetadataList() {}
		public MetadataList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](MetadataList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public MetadataList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MetadataList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class OccurrenceList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public OccurrenceList() {}
		public OccurrenceList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](OccurrenceList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public OccurrenceList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct OccurrenceList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class OccurrenceListList {
		public Scene.Native.OccurrenceList[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public OccurrenceListList(Scene.Native.OccurrenceList[] tab) { list = tab; }
		public static implicit operator Scene.Native.OccurrenceList[](OccurrenceListList o) { return o.list; }
		public Scene.Native.OccurrenceList this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public OccurrenceListList(int size) { list = new Scene.Native.OccurrenceList[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct OccurrenceListList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class PackedTree
	{
		public PackedTree() {}
		public PackedTree(PackedTree o) {
			this.occurrences = o.occurrences;
			this.parents = o.parents;
			this.names = o.names;
			this.visibles = o.visibles;
			this.materials = o.materials;
			this.transformIndices = o.transformIndices;
			this.transformMatrices = o.transformMatrices;
			this.customProperties = o.customProperties;
		}
		public OccurrenceList occurrences;
		public Core.Native.IntList parents;
		public Core.Native.StringList names;
		public Core.Native.InheritableBoolList visibles;
		public Material.Native.MaterialList materials;
		public Core.Native.IntList transformIndices;
		public Geom.Native.Matrix4List transformMatrices;
		public Core.Native.StringPairListList customProperties;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PackedTree_c
	{
		public OccurrenceList_c occurrences;
		public Core.Native.IntList_c parents;
		public Core.Native.StringList_c names;
		public Core.Native.InheritableBoolList_c visibles;
		public Material.Native.MaterialList_c materials;
		public Core.Native.IntList_c transformIndices;
		public Geom.Native.Matrix4List_c transformMatrices;
		public Core.Native.StringPairListList_c customProperties;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class PartList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public PartList() {}
		public PartList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](PartList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public PartList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PartList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	public enum Primitive_Type
	{
		CUBE = 0,
		SPHERE = 1,
		PLAN = 2,
		CONE = 3,
		ARROW = 4,
		CYLINDER = 5,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class ProberInfo
	{
		public ProberInfo() {}
		public ProberInfo(ProberInfo o) {
			this.occurrence = o.occurrence;
			this.position = o.position;
		}
		public System.UInt32 occurrence;
		public Geom.Native.Point3 position;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ProberInfo_c
	{
		public System.UInt32 occurrence;
		public Geom.Native.Point3_c position;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class RayHit
	{
		public RayHit() {}
		public RayHit(RayHit o) {
			this.rayParam = o.rayParam;
			this.occurrence = o.occurrence;
			this.triangleIndex = o.triangleIndex;
			this.triangleParam = o.triangleParam;
		}
		public System.Double rayParam;
		public System.UInt32 occurrence;
		public System.Int32 triangleIndex;
		public Geom.Native.Point2 triangleParam;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct RayHit_c
	{
		public System.Double rayParam;
		public System.UInt32 occurrence;
		public Int32 triangleIndex;
		public Geom.Native.Point2_c triangleParam;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class RayHitList {
		public Scene.Native.RayHit[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public RayHitList(Scene.Native.RayHit[] tab) { list = tab; }
		public static implicit operator Scene.Native.RayHit[](RayHitList o) { return o.list; }
		public Scene.Native.RayHit this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public RayHitList(int size) { list = new Scene.Native.RayHit[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct RayHitList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct ResizeByMaximumSizeOptions
	{
		public ResizeByMaximumSizeOptions(ResizeByMaximumSizeOptions o) {
			this.TextureSize = o.TextureSize;
			this.KeepTextureRatio = o.KeepTextureRatio;
		}
		public System.Int32 TextureSize;
		public System.Boolean KeepTextureRatio;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ResizeByMaximumSizeOptions_c
	{
		public Int32 TextureSize;
		public Int32 KeepTextureRatio;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct ResizeByTexturesOptions
	{
		public enum Type
		{
			UNKNOWN = 0,
			ALLTEXTURES = 1,
			SELECTION = 2,
		}
		public System.Int32 AllTextures;
		public Material.Native.ImageList Selection;
		public Type _type;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ResizeByTexturesOptions_c
	{
		public Int32 AllTextures;
		public Material.Native.ImageList_c Selection;
		public int _type;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct ResizeTexturesInputMode
	{
		public enum Type
		{
			UNKNOWN = 0,
			OCCURRENCES = 1,
			TEXTURES = 2,
		}
		public OccurrenceList Occurrences;
		public ResizeByTexturesOptions Textures;
		public Type _type;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ResizeTexturesInputMode_c
	{
		public OccurrenceList_c Occurrences;
		public ResizeByTexturesOptions_c Textures;
		public int _type;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct ResizeTexturesResizeMode
	{
		public enum Type
		{
			UNKNOWN = 0,
			RATIO = 1,
			MAXIMUMSIZE = 2,
		}
		public System.Double Ratio;
		public ResizeByMaximumSizeOptions MaximumSize;
		public Type _type;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ResizeTexturesResizeMode_c
	{
		public System.Double Ratio;
		public ResizeByMaximumSizeOptions_c MaximumSize;
		public int _type;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class VariantComponentList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public VariantComponentList() {}
		public VariantComponentList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](VariantComponentList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public VariantComponentList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct VariantComponentList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class VariantDefinition
	{
		public VariantDefinition() {}
		public VariantDefinition(VariantDefinition o) {
			this.variant = o.variant;
			this.overridedProperties = o.overridedProperties;
		}
		public System.UInt32 variant;
		public PropertyValueList overridedProperties;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct VariantDefinition_c
	{
		public System.UInt32 variant;
		public PropertyValueList_c overridedProperties;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class VariantDefinitionList {
		public Scene.Native.VariantDefinition[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public VariantDefinitionList(Scene.Native.VariantDefinition[] tab) { list = tab; }
		public static implicit operator Scene.Native.VariantDefinition[](VariantDefinitionList o) { return o.list; }
		public Scene.Native.VariantDefinition this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public VariantDefinitionList(int size) { list = new Scene.Native.VariantDefinition[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct VariantDefinitionList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class VariantDefinitionListList {
		public Scene.Native.VariantDefinitionList[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public VariantDefinitionListList(Scene.Native.VariantDefinitionList[] tab) { list = tab; }
		public static implicit operator Scene.Native.VariantDefinitionList[](VariantDefinitionListList o) { return o.list; }
		public Scene.Native.VariantDefinitionList this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public VariantDefinitionListList(int size) { list = new Scene.Native.VariantDefinitionList[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct VariantDefinitionListList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class VariantList {
		public System.UInt32[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public VariantList() {}
		public VariantList(System.UInt32[] tab) { list = tab; }
		public static implicit operator System.UInt32[](VariantList o) { return o.list; }
		public System.UInt32 this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public VariantList(int size) { list = new System.UInt32[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct VariantList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	public enum VisibilityMode
	{
		Inherited = 0,
		Hide = 1,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct getSubTreeStatsReturn
	{
		public getSubTreeStatsReturn(getSubTreeStatsReturn o) {
			this.partCount = o.partCount;
			this.partOccurrenceCount = o.partOccurrenceCount;
			this.triangleCount = o.triangleCount;
			this.triangleOccurrenceCount = o.triangleOccurrenceCount;
			this.vertexCount = o.vertexCount;
			this.vertexOccurrenceCount = o.vertexOccurrenceCount;
		}
		public System.Int32 partCount;
		public System.Int32 partOccurrenceCount;
		public System.Int32 triangleCount;
		public System.Int32 triangleOccurrenceCount;
		public System.Int32 vertexCount;
		public System.Int32 vertexOccurrenceCount;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getSubTreeStatsReturn_c
	{
		public Int32 partCount;
		public Int32 partOccurrenceCount;
		public Int32 triangleCount;
		public Int32 triangleOccurrenceCount;
		public Int32 vertexCount;
		public Int32 vertexOccurrenceCount;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getViewpointsFromCavitiesReturn
	{
		public getViewpointsFromCavitiesReturn() {}
		public getViewpointsFromCavitiesReturn(getViewpointsFromCavitiesReturn o) {
			this.positions = o.positions;
			this.directions = o.directions;
		}
		public Geom.Native.Point3List positions;
		public Geom.Native.Point3List directions;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getViewpointsFromCavitiesReturn_c
	{
		public Geom.Native.Point3List_c positions;
		public Geom.Native.Point3List_c directions;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct getOoCConfigurationReturn
	{
		public getOoCConfigurationReturn(getOoCConfigurationReturn o) {
			this.implementationType = o.implementationType;
			this.implementationParameters = o.implementationParameters;
		}
		public System.String implementationType;
		public System.String implementationParameters;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getOoCConfigurationReturn_c
	{
		public IntPtr implementationType;
		public IntPtr implementationParameters;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct getBRepInfosReturn
	{
		public getBRepInfosReturn(getBRepInfosReturn o) {
			this.partCount = o.partCount;
			this.totalPartCount = o.totalPartCount;
			this.vertexCount = o.vertexCount;
			this.totalVertexCount = o.totalVertexCount;
			this.edgeCount = o.edgeCount;
			this.totalEdgeCount = o.totalEdgeCount;
			this.domainCount = o.domainCount;
			this.totalDomainCount = o.totalDomainCount;
			this.bodyCount = o.bodyCount;
			this.totalBodyCount = o.totalBodyCount;
			this.area2Dsum = o.area2Dsum;
			this.boundaryCount = o.boundaryCount;
			this.boundaryEdgeCount = o.boundaryEdgeCount;
		}
		public System.Int32 partCount;
		public System.Int32 totalPartCount;
		public System.Int32 vertexCount;
		public System.Int32 totalVertexCount;
		public System.Int32 edgeCount;
		public System.Int32 totalEdgeCount;
		public System.Int32 domainCount;
		public System.Int32 totalDomainCount;
		public System.Int32 bodyCount;
		public System.Int32 totalBodyCount;
		public System.Double area2Dsum;
		public System.Int32 boundaryCount;
		public System.Int32 boundaryEdgeCount;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getBRepInfosReturn_c
	{
		public Int32 partCount;
		public Int32 totalPartCount;
		public Int32 vertexCount;
		public Int32 totalVertexCount;
		public Int32 edgeCount;
		public Int32 totalEdgeCount;
		public Int32 domainCount;
		public Int32 totalDomainCount;
		public Int32 bodyCount;
		public Int32 totalBodyCount;
		public System.Double area2Dsum;
		public Int32 boundaryCount;
		public Int32 boundaryEdgeCount;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct getTessellationInfosReturn
	{
		public getTessellationInfosReturn(getTessellationInfosReturn o) {
			this.partCount = o.partCount;
			this.totalPartCount = o.totalPartCount;
			this.vertexCount = o.vertexCount;
			this.totalVertexCount = o.totalVertexCount;
			this.edgeCount = o.edgeCount;
			this.totalEdgeCount = o.totalEdgeCount;
			this.polygonCount = o.polygonCount;
			this.totalPolygonCount = o.totalPolygonCount;
			this.patchCount = o.patchCount;
			this.totalPatchCount = o.totalPatchCount;
			this.boundaryCount = o.boundaryCount;
			this.boundaryEdgeCount = o.boundaryEdgeCount;
		}
		public System.Int32 partCount;
		public System.Int32 totalPartCount;
		public System.Int32 vertexCount;
		public System.Int32 totalVertexCount;
		public System.Int32 edgeCount;
		public System.Int32 totalEdgeCount;
		public System.Int32 polygonCount;
		public System.Int32 totalPolygonCount;
		public System.Int32 patchCount;
		public System.Int32 totalPatchCount;
		public System.Int32 boundaryCount;
		public System.Int32 boundaryEdgeCount;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getTessellationInfosReturn_c
	{
		public Int32 partCount;
		public Int32 totalPartCount;
		public Int32 vertexCount;
		public Int32 totalVertexCount;
		public Int32 edgeCount;
		public Int32 totalEdgeCount;
		public Int32 polygonCount;
		public Int32 totalPolygonCount;
		public Int32 patchCount;
		public Int32 totalPatchCount;
		public Int32 boundaryCount;
		public Int32 boundaryEdgeCount;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class evaluateExpressionOnSubTreeReturn
	{
		public evaluateExpressionOnSubTreeReturn() {}
		public evaluateExpressionOnSubTreeReturn(evaluateExpressionOnSubTreeReturn o) {
			this.occurrences = o.occurrences;
			this.evaluations = o.evaluations;
		}
		public OccurrenceList occurrences;
		public Core.Native.StringList evaluations;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct evaluateExpressionOnSubTreeReturn_c
	{
		public OccurrenceList_c occurrences;
		public Core.Native.StringList_c evaluations;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getPartsTransformsIndexedReturn
	{
		public getPartsTransformsIndexedReturn() {}
		public getPartsTransformsIndexedReturn(getPartsTransformsIndexedReturn o) {
			this.indices = o.indices;
			this.transforms = o.transforms;
		}
		public Core.Native.IntList indices;
		public Geom.Native.Matrix4List transforms;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getPartsTransformsIndexedReturn_c
	{
		public Core.Native.IntList_c indices;
		public Geom.Native.Matrix4List_c transforms;
	}

}
