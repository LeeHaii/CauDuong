#pragma warning disable CA2101

using System;
using System.Security;
using System.Runtime.InteropServices;

namespace Pixyz.Process.Native {

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct BakeDiffuseOptions
	{
		public BakeDiffuseOptions(BakeDiffuseOptions o) {
			this.mapResolution = o.mapResolution;
			this.padding = o.padding;
		}
		public System.Int32 mapResolution;
		public System.Int32 padding;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct BakeDiffuseOptions_c
	{
		public Int32 mapResolution;
		public Int32 padding;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class BakeOptions
	{
		public BakeOptions() {}
		public BakeOptions(BakeOptions o) {
			this.resolution = o.resolution;
			this.padding = o.padding;
			this.textures = o.textures;
		}
		public System.Int32 resolution;
		public System.Int32 padding;
		public Algo.Native.BakeMaps textures;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct BakeOptions_c
	{
		public Int32 resolution;
		public Int32 padding;
		public Algo.Native.BakeMaps_c textures;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct BakeOptionSelector
	{
		public enum Type
		{
			UNKNOWN = 0,
			YES = 1,
			NO = 2,
		}
		public BakeOptions Yes;
		public System.Int32 No;
		public Type _type;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct BakeOptionSelector_c
	{
		public BakeOptions_c Yes;
		public Int32 No;
		public int _type;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Orientation
	{
		public Orientation(Orientation o) {
			this.zUP = o.zUP;
			this.leftHanded = o.leftHanded;
		}
		public System.Boolean zUP;
		public System.Boolean leftHanded;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Orientation_c
	{
		public Int32 zUP;
		public Int32 leftHanded;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct OrientationSelect
	{
		public enum Type
		{
			UNKNOWN = 0,
			AUTOMATICORIENTATION = 1,
			FIXORIENTATION = 2,
		}
		public System.Int32 automaticOrientation;
		public Orientation fixOrientation;
		public Type _type;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct OrientationSelect_c
	{
		public Int32 automaticOrientation;
		public Orientation_c fixOrientation;
		public int _type;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct ScaleSelect
	{
		public enum Type
		{
			UNKNOWN = 0,
			AUTOMATICSCALE = 1,
			FIXSCALE = 2,
		}
		public System.Int32 automaticScale;
		public System.Double fixScale;
		public Type _type;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ScaleSelect_c
	{
		public Int32 automaticScale;
		public System.Double fixScale;
		public int _type;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class CoordinateSystemOptions
	{
		public CoordinateSystemOptions() {}
		public CoordinateSystemOptions(CoordinateSystemOptions o) {
			this.orientation = o.orientation;
			this.scale = o.scale;
			this.snapToGround = o.snapToGround;
			this.centerToOrigin = o.centerToOrigin;
		}
		public OrientationSelect orientation;
		public ScaleSelect scale;
		public System.Boolean snapToGround;
		public System.Boolean centerToOrigin;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct CoordinateSystemOptions_c
	{
		public OrientationSelect_c orientation;
		public ScaleSelect_c scale;
		public Int32 snapToGround;
		public Int32 centerToOrigin;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct DecimateParameters
	{
		public DecimateParameters(DecimateParameters o) {
			this.surfacicTolerance = o.surfacicTolerance;
			this.linearTolerance = o.linearTolerance;
			this.normalTolerance = o.normalTolerance;
		}
		public System.Double surfacicTolerance;
		public System.Double linearTolerance;
		public System.Double normalTolerance;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DecimateParameters_c
	{
		public System.Double surfacicTolerance;
		public System.Double linearTolerance;
		public System.Double normalTolerance;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class DecimateParametersList {
		public Process.Native.DecimateParameters[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public DecimateParametersList(Process.Native.DecimateParameters[] tab) { list = tab; }
		public static implicit operator Process.Native.DecimateParameters[](DecimateParametersList o) { return o.list; }
		public Process.Native.DecimateParameters this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public DecimateParametersList(int size) { list = new Process.Native.DecimateParameters[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DecimateParametersList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct GenerateDiffuseMap
	{
		public enum Type
		{
			UNKNOWN = 0,
			YES = 1,
			NO = 2,
		}
		public BakeDiffuseOptions yes;
		public System.Int32 no;
		public Type _type;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct GenerateDiffuseMap_c
	{
		public BakeDiffuseOptions_c yes;
		public Int32 no;
		public int _type;
	}

	public enum HierarchyMode
	{
		Full = 0,
		Compress = 1,
		Rake = 2,
		MergeAll = 3,
		MergeFinalLevel = 4,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct ImportOptions
	{
		public ImportOptions(ImportOptions o) {
			this.orientFaces = o.orientFaces;
			this.preserveOriginalUVs = o.preserveOriginalUVs;
			this.removeDuplicatedMeshes = o.removeDuplicatedMeshes;
		}
		public System.Boolean orientFaces;
		public System.Boolean preserveOriginalUVs;
		public System.Boolean removeDuplicatedMeshes;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ImportOptions_c
	{
		public Int32 orientFaces;
		public Int32 preserveOriginalUVs;
		public Int32 removeDuplicatedMeshes;
	}

	public enum QualityPreset
	{
		VeryHigh = 0,
		High = 1,
		Medium = 2,
		Low = 3,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct TessellationQuality
	{
		public TessellationQuality(TessellationQuality o) {
			this.maxSag = o.maxSag;
			this.maxLength = o.maxLength;
			this.maxAngle = o.maxAngle;
		}
		public System.Double maxSag;
		public System.Double maxLength;
		public System.Double maxAngle;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct TessellationQuality_c
	{
		public System.Double maxSag;
		public System.Double maxLength;
		public System.Double maxAngle;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct TessellationSettings
	{
		public enum Type
		{
			UNKNOWN = 0,
			USEPRESET = 1,
			USECUSTOMVALUES = 2,
		}
		public QualityPreset usePreset;
		public TessellationQuality useCustomValues;
		public Type _type;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct TessellationSettings_c
	{
		public Int32 usePreset;
		public TessellationQuality_c useCustomValues;
		public int _type;
	}

}
