#pragma warning disable CA2101

using System;
using System.Security;
using System.Runtime.InteropServices;

namespace Pixyz.View.Native {

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct DrawPrimitives
	{
		public DrawPrimitives(DrawPrimitives o) {
			this.polygons = o.polygons;
			this.breps = o.breps;
			this.wireframe = o.wireframe;
			this.points = o.points;
			this.freeLines = o.freeLines;
			this.patchBoundaries = o.patchBoundaries;
		}
		public System.Boolean polygons;
		public System.Boolean breps;
		public System.Boolean wireframe;
		public System.Boolean points;
		public System.Boolean freeLines;
		public System.Boolean patchBoundaries;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DrawPrimitives_c
	{
		public Int32 polygons;
		public Int32 breps;
		public Int32 wireframe;
		public Int32 points;
		public Int32 freeLines;
		public Int32 patchBoundaries;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class PickResult
	{
		public PickResult() {}
		public PickResult(PickResult o) {
			this.occurrences = o.occurrences;
			this.positions = o.positions;
		}
		public Scene.Native.OccurrenceList occurrences;
		public Geom.Native.Point3List positions;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PickResult_c
	{
		public Scene.Native.OccurrenceList_c occurrences;
		public Geom.Native.Point3List_c positions;
	}

	public enum ViewSessionTextureType
	{
		COLOR = 0,
		DEPTH = 1,
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class ViewSessionTexture
	{
		public ViewSessionTexture() {}
		public ViewSessionTexture(ViewSessionTexture o) {
			this.type = o.type;
			this.texture = o.texture;
		}
		public ViewSessionTextureType type;
		public System.IntPtr texture;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ViewSessionTexture_c
	{
		public Int32 type;
		public System.IntPtr texture;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class ViewSessionTextureList {
		public View.Native.ViewSessionTexture[] list;
		public int length { get { return (list != null) ? list.Length : 0; } }
		public ViewSessionTextureList(View.Native.ViewSessionTexture[] tab) { list = tab; }
		public static implicit operator View.Native.ViewSessionTexture[](ViewSessionTextureList o) { return o.list; }
		public View.Native.ViewSessionTexture this[int index] {
			get { return list[index]; }
			set { list[index] = value; }
		}
		public ViewSessionTextureList(int size) { list = new View.Native.ViewSessionTexture[size]; }
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ViewSessionTextureList_c
	{
		public System.UInt64 size;
		public IntPtr ptr;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class getViewerMatricesReturn
	{
		public getViewerMatricesReturn() {}
		public getViewerMatricesReturn(getViewerMatricesReturn o) {
			this.views = o.views;
			this.projs = o.projs;
			this.clipping = o.clipping;
		}
		public Geom.Native.Matrix4List views;
		public Geom.Native.Matrix4List projs;
		public Geom.Native.Point2 clipping;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getViewerMatricesReturn_c
	{
		public Geom.Native.Matrix4List_c views;
		public Geom.Native.Matrix4List_c projs;
		public Geom.Native.Point2_c clipping;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct getViewerSizeReturn
	{
		public getViewerSizeReturn(getViewerSizeReturn o) {
			this.width = o.width;
			this.height = o.height;
		}
		public System.Int32 width;
		public System.Int32 height;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct getViewerSizeReturn_c
	{
		public Int32 width;
		public Int32 height;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class pickReturn
	{
		public pickReturn() {}
		public pickReturn(pickReturn o) {
			this.occurrence = o.occurrence;
			this.position = o.position;
		}
		public System.UInt32 occurrence;
		public Geom.Native.Point3 position;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct pickReturn_c
	{
		public System.UInt32 occurrence;
		public Geom.Native.Point3_c position;
	}

}
