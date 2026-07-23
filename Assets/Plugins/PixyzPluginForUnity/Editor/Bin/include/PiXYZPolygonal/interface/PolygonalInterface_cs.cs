#pragma warning disable CA2101

using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Security;
using System.Collections.Concurrent;
using PiXYZAPI = Pixyz.API.Native.NativeInterface;

namespace Pixyz.Polygonal.Native {

	public static partial class NativeInterface {

		static NativeInterface()
		{
			_ = PiXYZAPI.GetLastError();
		}
		[DllImport(PiXYZAPI.memcpy_dll, EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false), SuppressUnmanagedCodeSecurity]
		private static unsafe extern void* memcpy(void* dest, void* src, ulong count);

		private static unsafe T[] CopyMemory<T>(IntPtr pointer, int length) {
			T[] managedArray = new T[length];
			GCHandle handle = GCHandle.Alloc(managedArray, GCHandleType.Pinned);
			IntPtr ptr = handle.AddrOfPinnedObject();
			void* nativePtr = pointer.ToPointer();
			memcpy(ptr.ToPointer(), nativePtr, (ulong)(length * Marshal.SizeOf(typeof(T))));
			handle.Free();
			return managedArray;
		}

		private static unsafe String ConvertValue(IntPtr s) {
			return new string((sbyte*)s);
		}

		private static IntPtr ConvertValue(string s) {
			return Marshal.StringToHGlobalAnsi(s);
		}

		private static bool ConvertValue(int b) {
			return (b != 0);
		}

		private static int ConvertValue(bool b) {
			return b ? 1 : 0;
		}

		#region Types Init/Free Methods

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_TopologyCategoryMask_init(ref TopologyCategoryMask_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_TopologyCategoryMask_free(ref TopologyCategoryMask_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_EdgeList_init(ref EdgeList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_EdgeList_free(ref EdgeList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_EdgeListList_init(ref EdgeListList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_EdgeListList_free(ref EdgeListList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_PolygonList_init(ref PolygonList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_PolygonList_free(ref PolygonList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_StylizedLine_init(ref StylizedLine_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_StylizedLine_free(ref StylizedLine_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_StylizedLineList_init(ref StylizedLineList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_StylizedLineList_free(ref StylizedLineList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_ElementList_init(ref ElementList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_ElementList_free(ref ElementList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_PolygonListList_init(ref PolygonListList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_PolygonListList_free(ref PolygonListList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_TessellationList_init(ref TessellationList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_TessellationList_free(ref TessellationList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_VertexList_init(ref VertexList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_VertexList_free(ref VertexList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_VertexListList_init(ref VertexListList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_VertexListList_free(ref VertexListList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_DressedPoly_init(ref DressedPoly_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_DressedPoly_free(ref DressedPoly_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_DressedPolyList_init(ref DressedPolyList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_DressedPolyList_free(ref DressedPolyList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_PlaceholderJointList_init(ref PlaceholderJointList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_PlaceholderJointList_free(ref PlaceholderJointList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_JointList_init(ref JointList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_JointList_free(ref JointList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_MeshDefinition_init(ref MeshDefinition_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_MeshDefinition_free(ref MeshDefinition_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_UVCoordList_init(ref UVCoordList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_UVCoordList_free(ref UVCoordList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_PatchList_init(ref PatchList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_PatchList_free(ref PatchList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_MeshDefinitionList_init(ref MeshDefinitionList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_MeshDefinitionList_free(ref MeshDefinitionList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_MeshList_init(ref MeshList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Polygonal_MeshList_free(ref MeshList_c list);

	public static TopologyCategoryMask ConvertValue(ref TopologyCategoryMask_c s) {
		TopologyCategoryMask ss = new TopologyCategoryMask();
		ss.dimension = (TopologyDimensionMask)s.dimension;
		ss.connectivity = (TopologyConnectivityMask)s.connectivity;
		return ss;
	}

	public static TopologyCategoryMask_c ConvertValue(TopologyCategoryMask s, ref TopologyCategoryMask_c ss) {
		Polygonal.Native.NativeInterface.Polygonal_TopologyCategoryMask_init(ref ss);
		ss.dimension = (Int32)s.dimension;
		ss.connectivity = (Int32)s.connectivity;
		return ss;
	}

	public static EdgeList ConvertValue(ref EdgeList_c s) {
		EdgeList list = new EdgeList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static EdgeList_c ConvertValue(EdgeList s, ref EdgeList_c list) {
		Polygonal.Native.NativeInterface.Polygonal_EdgeList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static EdgeListList ConvertValue(ref EdgeListList_c s) {
		EdgeListList list = new EdgeListList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(EdgeList_c)));
			EdgeList_c value = (EdgeList_c)Marshal.PtrToStructure(p, typeof(EdgeList_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static EdgeListList_c ConvertValue(EdgeListList s, ref EdgeListList_c list) {
		Polygonal.Native.NativeInterface.Polygonal_EdgeListList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			EdgeList_c elt = new EdgeList_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(EdgeList_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static PolygonList ConvertValue(ref PolygonList_c s) {
		PolygonList list = new PolygonList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static PolygonList_c ConvertValue(PolygonList s, ref PolygonList_c list) {
		Polygonal.Native.NativeInterface.Polygonal_PolygonList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static Core.Native.IntList ConvertValue(ref Core.Native.IntList_c s) {
		Core.Native.IntList list = new Core.Native.IntList((int)s.size);
		if (s.size > 0)
			Marshal.Copy(s.ptr, list.list, 0, (int)s.size);
		return list;
	}

	public static Core.Native.IntList_c ConvertValue(Core.Native.IntList s, ref Core.Native.IntList_c list) {
		Core.Native.NativeInterface.Core_IntList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size > 0)
			Marshal.Copy(s.list, 0, list.ptr, (int)list.size);
		return list;
	}

	public static Core.Native.ColorAlpha ConvertValue(ref Core.Native.ColorAlpha_c s) {
		Core.Native.ColorAlpha ss = new Core.Native.ColorAlpha();
		ss.r = (System.Double)s.r;
		ss.g = (System.Double)s.g;
		ss.b = (System.Double)s.b;
		ss.a = (System.Double)s.a;
		return ss;
	}

	public static Core.Native.ColorAlpha_c ConvertValue(Core.Native.ColorAlpha s, ref Core.Native.ColorAlpha_c ss) {
		Core.Native.NativeInterface.Core_ColorAlpha_init(ref ss);
		ss.r = (System.Double)s.r;
		ss.g = (System.Double)s.g;
		ss.b = (System.Double)s.b;
		ss.a = (System.Double)s.a;
		return ss;
	}

	public static StylizedLine ConvertValue(ref StylizedLine_c s) {
		StylizedLine ss = new StylizedLine();
		ss.lines = Core.Native.NativeInterface.ConvertValue(ref s.lines);
		ss.width = (System.Double)s.width;
		ss.type = (StyleType)s.type;
		ss.pattern = (System.Int32)s.pattern;
		ss.color = Core.Native.NativeInterface.ConvertValue(ref s.color);
		ss.externalId = (System.UInt32)s.externalId;
		return ss;
	}

	public static StylizedLine_c ConvertValue(StylizedLine s, ref StylizedLine_c ss) {
		Polygonal.Native.NativeInterface.Polygonal_StylizedLine_init(ref ss);
		Core.Native.NativeInterface.ConvertValue(s.lines, ref ss.lines);
		ss.width = (System.Double)s.width;
		ss.type = (Int32)s.type;
		ss.pattern = (Int32)s.pattern;
		Core.Native.NativeInterface.ConvertValue(s.color, ref ss.color);
		ss.externalId = (System.UInt32)s.externalId;
		return ss;
	}

	public static StylizedLineList ConvertValue(ref StylizedLineList_c s) {
		StylizedLineList list = new StylizedLineList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(StylizedLine_c)));
			StylizedLine_c value = (StylizedLine_c)Marshal.PtrToStructure(p, typeof(StylizedLine_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static StylizedLineList_c ConvertValue(StylizedLineList s, ref StylizedLineList_c list) {
		Polygonal.Native.NativeInterface.Polygonal_StylizedLineList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			StylizedLine_c elt = new StylizedLine_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(StylizedLine_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static ElementList ConvertValue(ref ElementList_c s) {
		ElementList list = new ElementList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static ElementList_c ConvertValue(ElementList s, ref ElementList_c list) {
		Polygonal.Native.NativeInterface.Polygonal_ElementList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static PolygonListList ConvertValue(ref PolygonListList_c s) {
		PolygonListList list = new PolygonListList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(PolygonList_c)));
			PolygonList_c value = (PolygonList_c)Marshal.PtrToStructure(p, typeof(PolygonList_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static PolygonListList_c ConvertValue(PolygonListList s, ref PolygonListList_c list) {
		Polygonal.Native.NativeInterface.Polygonal_PolygonListList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			PolygonList_c elt = new PolygonList_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(PolygonList_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static TessellationList ConvertValue(ref TessellationList_c s) {
		TessellationList list = new TessellationList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static TessellationList_c ConvertValue(TessellationList s, ref TessellationList_c list) {
		Polygonal.Native.NativeInterface.Polygonal_TessellationList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static VertexList ConvertValue(ref VertexList_c s) {
		VertexList list = new VertexList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static VertexList_c ConvertValue(VertexList s, ref VertexList_c list) {
		Polygonal.Native.NativeInterface.Polygonal_VertexList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static VertexListList ConvertValue(ref VertexListList_c s) {
		VertexListList list = new VertexListList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(VertexList_c)));
			VertexList_c value = (VertexList_c)Marshal.PtrToStructure(p, typeof(VertexList_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static VertexListList_c ConvertValue(VertexListList s, ref VertexListList_c list) {
		Polygonal.Native.NativeInterface.Polygonal_VertexListList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			VertexList_c elt = new VertexList_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(VertexList_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static DressedPoly ConvertValue(ref DressedPoly_c s) {
		DressedPoly ss = new DressedPoly();
		ss.material = (System.UInt32)s.material;
		ss.firstTri = (System.Int32)s.firstTri;
		ss.triCount = (System.Int32)s.triCount;
		ss.firstQuad = (System.Int32)s.firstQuad;
		ss.quadCount = (System.Int32)s.quadCount;
		ss.externalId = (System.UInt32)s.externalId;
		return ss;
	}

	public static DressedPoly_c ConvertValue(DressedPoly s, ref DressedPoly_c ss) {
		Polygonal.Native.NativeInterface.Polygonal_DressedPoly_init(ref ss);
		ss.material = (System.UInt32)s.material;
		ss.firstTri = (Int32)s.firstTri;
		ss.triCount = (Int32)s.triCount;
		ss.firstQuad = (Int32)s.firstQuad;
		ss.quadCount = (Int32)s.quadCount;
		ss.externalId = (System.UInt32)s.externalId;
		return ss;
	}

	public static DressedPolyList ConvertValue(ref DressedPolyList_c s) {
		DressedPolyList list = new DressedPolyList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<DressedPoly>(s.ptr, (int)s.size);
		return list;
	}

	public static DressedPolyList_c ConvertValue(DressedPolyList s, ref DressedPolyList_c list) {
		Polygonal.Native.NativeInterface.Polygonal_DressedPolyList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			DressedPoly_c elt = new DressedPoly_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(DressedPoly_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static PlaceholderJointList ConvertValue(ref PlaceholderJointList_c s) {
		PlaceholderJointList list = new PlaceholderJointList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static PlaceholderJointList_c ConvertValue(PlaceholderJointList s, ref PlaceholderJointList_c list) {
		Polygonal.Native.NativeInterface.Polygonal_PlaceholderJointList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static Geom.Native.Point3 ConvertValue(ref Geom.Native.Point3_c s) {
		Geom.Native.Point3 ss = new Geom.Native.Point3();
		ss.x = (System.Double)s.x;
		ss.y = (System.Double)s.y;
		ss.z = (System.Double)s.z;
		return ss;
	}

	public static Geom.Native.Point3_c ConvertValue(Geom.Native.Point3 s, ref Geom.Native.Point3_c ss) {
		Geom.Native.NativeInterface.Geom_Point3_init(ref ss);
		ss.x = (System.Double)s.x;
		ss.y = (System.Double)s.y;
		ss.z = (System.Double)s.z;
		return ss;
	}

	public static Geom.Native.Point3List ConvertValue(ref Geom.Native.Point3List_c s) {
		Geom.Native.Point3List list = new Geom.Native.Point3List((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<Geom.Native.Point3>(s.ptr, (int)s.size);
		return list;
	}

	public static Geom.Native.Point3List_c ConvertValue(Geom.Native.Point3List s, ref Geom.Native.Point3List_c list) {
		Geom.Native.NativeInterface.Geom_Point3List_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			Geom.Native.Point3_c elt = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Geom.Native.Point3_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static Geom.Native.Vector3List ConvertValue(ref Geom.Native.Vector3List_c s) {
		Geom.Native.Vector3List list = new Geom.Native.Vector3List((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<Geom.Native.Vector3>(s.ptr, (int)s.size);
		return list;
	}

	public static Geom.Native.Vector3List_c ConvertValue(Geom.Native.Vector3List s, ref Geom.Native.Vector3List_c list) {
		Geom.Native.NativeInterface.Geom_Vector3List_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			Geom.Native.Point3_c elt = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Geom.Native.Point3_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static Geom.Native.Vector4List ConvertValue(ref Geom.Native.Vector4List_c s) {
		Geom.Native.Vector4List list = new Geom.Native.Vector4List((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<Geom.Native.Vector4>(s.ptr, (int)s.size);
		return list;
	}

	public static Geom.Native.Vector4List_c ConvertValue(Geom.Native.Vector4List s, ref Geom.Native.Vector4List_c list) {
		Geom.Native.NativeInterface.Geom_Vector4List_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			Geom.Native.Point4_c elt = new Geom.Native.Point4_c();
			Geom.Native.NativeInterface.ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Geom.Native.Point4_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static Geom.Native.Point2 ConvertValue(ref Geom.Native.Point2_c s) {
		Geom.Native.Point2 ss = new Geom.Native.Point2();
		ss.x = (System.Double)s.x;
		ss.y = (System.Double)s.y;
		return ss;
	}

	public static Geom.Native.Point2_c ConvertValue(Geom.Native.Point2 s, ref Geom.Native.Point2_c ss) {
		Geom.Native.NativeInterface.Geom_Point2_init(ref ss);
		ss.x = (System.Double)s.x;
		ss.y = (System.Double)s.y;
		return ss;
	}

	public static Geom.Native.Point2List ConvertValue(ref Geom.Native.Point2List_c s) {
		Geom.Native.Point2List list = new Geom.Native.Point2List((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<Geom.Native.Point2>(s.ptr, (int)s.size);
		return list;
	}

	public static Geom.Native.Point2List_c ConvertValue(Geom.Native.Point2List s, ref Geom.Native.Point2List_c list) {
		Geom.Native.NativeInterface.Geom_Point2List_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			Geom.Native.Point2_c elt = new Geom.Native.Point2_c();
			Geom.Native.NativeInterface.ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Geom.Native.Point2_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static Geom.Native.Point2ListList ConvertValue(ref Geom.Native.Point2ListList_c s) {
		Geom.Native.Point2ListList list = new Geom.Native.Point2ListList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Geom.Native.Point2List_c)));
			Geom.Native.Point2List_c value = (Geom.Native.Point2List_c)Marshal.PtrToStructure(p, typeof(Geom.Native.Point2List_c));
			list.list[i] = Geom.Native.NativeInterface.ConvertValue(ref value);
		}
		return list;
	}

	public static Geom.Native.Point2ListList_c ConvertValue(Geom.Native.Point2ListList s, ref Geom.Native.Point2ListList_c list) {
		Geom.Native.NativeInterface.Geom_Point2ListList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			Geom.Native.Point2List_c elt = new Geom.Native.Point2List_c();
			Geom.Native.NativeInterface.ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Geom.Native.Point2List_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static Core.Native.ColorAlphaList ConvertValue(ref Core.Native.ColorAlphaList_c s) {
		Core.Native.ColorAlphaList list = new Core.Native.ColorAlphaList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<Core.Native.ColorAlpha>(s.ptr, (int)s.size);
		return list;
	}

	public static Core.Native.ColorAlphaList_c ConvertValue(Core.Native.ColorAlphaList s, ref Core.Native.ColorAlphaList_c list) {
		Core.Native.NativeInterface.Core_ColorAlphaList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			Core.Native.ColorAlpha_c elt = new Core.Native.ColorAlpha_c();
			Core.Native.NativeInterface.ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Core.Native.ColorAlpha_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static Geom.Native.Curvatures ConvertValue(ref Geom.Native.Curvatures_c s) {
		Geom.Native.Curvatures ss = new Geom.Native.Curvatures();
		ss.k1 = (System.Double)s.k1;
		ss.k2 = (System.Double)s.k2;
		ss.v1 = Geom.Native.NativeInterface.ConvertValue(ref s.v1);
		ss.v2 = Geom.Native.NativeInterface.ConvertValue(ref s.v2);
		return ss;
	}

	public static Geom.Native.Curvatures_c ConvertValue(Geom.Native.Curvatures s, ref Geom.Native.Curvatures_c ss) {
		Geom.Native.NativeInterface.Geom_Curvatures_init(ref ss);
		ss.k1 = (System.Double)s.k1;
		ss.k2 = (System.Double)s.k2;
		Geom.Native.NativeInterface.ConvertValue(s.v1, ref ss.v1);
		Geom.Native.NativeInterface.ConvertValue(s.v2, ref ss.v2);
		return ss;
	}

	public static Geom.Native.CurvaturesList ConvertValue(ref Geom.Native.CurvaturesList_c s) {
		Geom.Native.CurvaturesList list = new Geom.Native.CurvaturesList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Geom.Native.Curvatures_c)));
			Geom.Native.Curvatures_c value = (Geom.Native.Curvatures_c)Marshal.PtrToStructure(p, typeof(Geom.Native.Curvatures_c));
			list.list[i] = Geom.Native.NativeInterface.ConvertValue(ref value);
		}
		return list;
	}

	public static Geom.Native.CurvaturesList_c ConvertValue(Geom.Native.CurvaturesList s, ref Geom.Native.CurvaturesList_c list) {
		Geom.Native.NativeInterface.Geom_CurvaturesList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			Geom.Native.Curvatures_c elt = new Geom.Native.Curvatures_c();
			Geom.Native.NativeInterface.ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Geom.Native.Curvatures_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static JointList ConvertValue(ref JointList_c s) {
		JointList list = new JointList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static JointList_c ConvertValue(JointList s, ref JointList_c list) {
		Polygonal.Native.NativeInterface.Polygonal_JointList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static Geom.Native.Array4 ConvertValue(ref Geom.Native.Array4_c arr) {
		Geom.Native.Array4 ss = new Geom.Native.Array4();
		System.Double[] tab = new System.Double[4];
		Marshal.Copy(arr.tab, tab, 0, 4);
		for (int i = 0; i < 4; ++i)
			ss.tab[i] = tab[i];
		return ss;
	}

	public static Geom.Native.Array4_c ConvertValue(Geom.Native.Array4 s, ref Geom.Native.Array4_c list) {
		Geom.Native.NativeInterface.Geom_Array4_init(ref list, (System.UInt64)4);
		var tab = new System.Double[4];
		for (int i=0; i < 4; ++i)
			tab[i] = s.tab[i];
		Marshal.Copy(tab, 0, list.tab, 4);
		return list;
	}

	public static Geom.Native.Matrix4 ConvertValue(ref Geom.Native.Matrix4_c arr) {
		Geom.Native.Matrix4 ss = new Geom.Native.Matrix4();
		for (int i = 0; i < 4; ++i) {
			IntPtr p = new IntPtr(arr.tab.ToInt64() + i * Marshal.SizeOf(typeof(Geom.Native.Array4_c)));
			Geom.Native.Array4_c value = (Geom.Native.Array4_c)Marshal.PtrToStructure(p, typeof(Geom.Native.Array4_c));
			ss.tab[i] = Geom.Native.NativeInterface.ConvertValue(ref value);
		}
		return ss;
	}

	public static Geom.Native.Matrix4_c ConvertValue(Geom.Native.Matrix4 s, ref Geom.Native.Matrix4_c list) {
		Geom.Native.NativeInterface.Geom_Matrix4_init(ref list, (System.UInt64)4);
		for (int i = 0; i < 4; ++i) {
			Geom.Native.Array4_c elt = new Geom.Native.Array4_c();
			Geom.Native.NativeInterface.ConvertValue(s.tab[i], ref elt);
			IntPtr p = new IntPtr(list.tab.ToInt64() + i * Marshal.SizeOf(typeof(Geom.Native.Array4_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static Geom.Native.Matrix4List ConvertValue(ref Geom.Native.Matrix4List_c s) {
		Geom.Native.Matrix4List list = new Geom.Native.Matrix4List((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Geom.Native.Matrix4_c)));
			Geom.Native.Matrix4_c value = (Geom.Native.Matrix4_c)Marshal.PtrToStructure(p, typeof(Geom.Native.Matrix4_c));
			list.list[i] = Geom.Native.NativeInterface.ConvertValue(ref value);
		}
		return list;
	}

	public static Geom.Native.Matrix4List_c ConvertValue(Geom.Native.Matrix4List s, ref Geom.Native.Matrix4List_c list) {
		Geom.Native.NativeInterface.Geom_Matrix4List_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			Geom.Native.Matrix4_c elt = new Geom.Native.Matrix4_c();
			Geom.Native.NativeInterface.ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Geom.Native.Matrix4_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static Geom.Native.Vector4I ConvertValue(ref Geom.Native.Vector4I_c s) {
		Geom.Native.Vector4I ss = new Geom.Native.Vector4I();
		ss.x = (System.Int32)s.x;
		ss.y = (System.Int32)s.y;
		ss.z = (System.Int32)s.z;
		ss.w = (System.Int32)s.w;
		return ss;
	}

	public static Geom.Native.Vector4I_c ConvertValue(Geom.Native.Vector4I s, ref Geom.Native.Vector4I_c ss) {
		Geom.Native.NativeInterface.Geom_Vector4I_init(ref ss);
		ss.x = (Int32)s.x;
		ss.y = (Int32)s.y;
		ss.z = (Int32)s.z;
		ss.w = (Int32)s.w;
		return ss;
	}

	public static Geom.Native.Vector4IList ConvertValue(ref Geom.Native.Vector4IList_c s) {
		Geom.Native.Vector4IList list = new Geom.Native.Vector4IList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<Geom.Native.Vector4I>(s.ptr, (int)s.size);
		return list;
	}

	public static Geom.Native.Vector4IList_c ConvertValue(Geom.Native.Vector4IList s, ref Geom.Native.Vector4IList_c list) {
		Geom.Native.NativeInterface.Geom_Vector4IList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			Geom.Native.Vector4I_c elt = new Geom.Native.Vector4I_c();
			Geom.Native.NativeInterface.ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Geom.Native.Vector4I_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static MeshDefinition ConvertValue(ref MeshDefinition_c s) {
		MeshDefinition ss = new MeshDefinition();
		ss.id = (System.UInt32)s.id;
		ss.externalId = (System.UInt32)s.externalId;
		ss.vertices = Geom.Native.NativeInterface.ConvertValue(ref s.vertices);
		ss.normals = Geom.Native.NativeInterface.ConvertValue(ref s.normals);
		ss.tangents = Geom.Native.NativeInterface.ConvertValue(ref s.tangents);
		ss.uvChannels = Core.Native.NativeInterface.ConvertValue(ref s.uvChannels);
		ss.uvs = Geom.Native.NativeInterface.ConvertValue(ref s.uvs);
		ss.vertexColors = Core.Native.NativeInterface.ConvertValue(ref s.vertexColors);
		ss.curvatures = Geom.Native.NativeInterface.ConvertValue(ref s.curvatures);
		ss.triangles = Core.Native.NativeInterface.ConvertValue(ref s.triangles);
		ss.quadrangles = Core.Native.NativeInterface.ConvertValue(ref s.quadrangles);
		ss.vertexMerged = Core.Native.NativeInterface.ConvertValue(ref s.vertexMerged);
		ss.dressedPolys = ConvertValue(ref s.dressedPolys);
		ss.linesVertices = Geom.Native.NativeInterface.ConvertValue(ref s.linesVertices);
		ss.lines = ConvertValue(ref s.lines);
		ss.points = Geom.Native.NativeInterface.ConvertValue(ref s.points);
		ss.pointsColors = Geom.Native.NativeInterface.ConvertValue(ref s.pointsColors);
		ss.joints = ConvertValue(ref s.joints);
		ss.inverseBindMatrices = Geom.Native.NativeInterface.ConvertValue(ref s.inverseBindMatrices);
		ss.jointWeights = Geom.Native.NativeInterface.ConvertValue(ref s.jointWeights);
		ss.jointIndices = Geom.Native.NativeInterface.ConvertValue(ref s.jointIndices);
		return ss;
	}

	public static MeshDefinition_c ConvertValue(MeshDefinition s, ref MeshDefinition_c ss) {
		Polygonal.Native.NativeInterface.Polygonal_MeshDefinition_init(ref ss);
		ss.id = (System.UInt32)s.id;
		ss.externalId = (System.UInt32)s.externalId;
		Geom.Native.NativeInterface.ConvertValue(s.vertices, ref ss.vertices);
		Geom.Native.NativeInterface.ConvertValue(s.normals, ref ss.normals);
		Geom.Native.NativeInterface.ConvertValue(s.tangents, ref ss.tangents);
		Core.Native.NativeInterface.ConvertValue(s.uvChannels, ref ss.uvChannels);
		Geom.Native.NativeInterface.ConvertValue(s.uvs, ref ss.uvs);
		Core.Native.NativeInterface.ConvertValue(s.vertexColors, ref ss.vertexColors);
		Geom.Native.NativeInterface.ConvertValue(s.curvatures, ref ss.curvatures);
		Core.Native.NativeInterface.ConvertValue(s.triangles, ref ss.triangles);
		Core.Native.NativeInterface.ConvertValue(s.quadrangles, ref ss.quadrangles);
		Core.Native.NativeInterface.ConvertValue(s.vertexMerged, ref ss.vertexMerged);
		ConvertValue(s.dressedPolys, ref ss.dressedPolys);
		Geom.Native.NativeInterface.ConvertValue(s.linesVertices, ref ss.linesVertices);
		ConvertValue(s.lines, ref ss.lines);
		Geom.Native.NativeInterface.ConvertValue(s.points, ref ss.points);
		Geom.Native.NativeInterface.ConvertValue(s.pointsColors, ref ss.pointsColors);
		ConvertValue(s.joints, ref ss.joints);
		Geom.Native.NativeInterface.ConvertValue(s.inverseBindMatrices, ref ss.inverseBindMatrices);
		Geom.Native.NativeInterface.ConvertValue(s.jointWeights, ref ss.jointWeights);
		Geom.Native.NativeInterface.ConvertValue(s.jointIndices, ref ss.jointIndices);
		return ss;
	}

	public static UVCoordList ConvertValue(ref UVCoordList_c s) {
		UVCoordList list = new UVCoordList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static UVCoordList_c ConvertValue(UVCoordList s, ref UVCoordList_c list) {
		Polygonal.Native.NativeInterface.Polygonal_UVCoordList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static PatchList ConvertValue(ref PatchList_c s) {
		PatchList list = new PatchList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static PatchList_c ConvertValue(PatchList s, ref PatchList_c list) {
		Polygonal.Native.NativeInterface.Polygonal_PatchList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static MeshDefinitionList ConvertValue(ref MeshDefinitionList_c s) {
		MeshDefinitionList list = new MeshDefinitionList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(MeshDefinition_c)));
			MeshDefinition_c value = (MeshDefinition_c)Marshal.PtrToStructure(p, typeof(MeshDefinition_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static MeshDefinitionList_c ConvertValue(MeshDefinitionList s, ref MeshDefinitionList_c list) {
		Polygonal.Native.NativeInterface.Polygonal_MeshDefinitionList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			MeshDefinition_c elt = new MeshDefinition_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(MeshDefinition_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static MeshList ConvertValue(ref MeshList_c s) {
		MeshList list = new MeshList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static MeshList_c ConvertValue(MeshList s, ref MeshList_c list) {
		Polygonal.Native.NativeInterface.Polygonal_MeshList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static Core.Native.ByteList ConvertValue(ref Core.Native.ByteList_c s) {
		Core.Native.ByteList list = new Core.Native.ByteList((int)s.size);
		if (s.size > 0)
			Marshal.Copy(s.ptr, list.list, 0, (int)s.size);
		return list;
	}

	public static Core.Native.ByteList_c ConvertValue(Core.Native.ByteList s, ref Core.Native.ByteList_c list) {
		Core.Native.NativeInterface.Core_ByteList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size > 0)
			Marshal.Copy(s.list, 0, list.ptr, (int)list.size);
		return list;
	}

	public static dracoEncodeReturn ConvertValue(ref dracoEncodeReturn_c s) {
		dracoEncodeReturn ss = new dracoEncodeReturn();
		ss.buffer = Core.Native.NativeInterface.ConvertValue(ref s.buffer);
		ss.jointIndicesId = (System.Int32)s.jointIndicesId;
		ss.jointWeightsId = (System.Int32)s.jointWeightsId;
		return ss;
	}

	public static dracoEncodeReturn_c ConvertValue(dracoEncodeReturn s, ref dracoEncodeReturn_c ss) {
		Core.Native.NativeInterface.ConvertValue(s.buffer, ref ss.buffer);
		ss.jointIndicesId = (Int32)s.jointIndicesId;
		ss.jointWeightsId = (Int32)s.jointWeightsId;
		return ss;
	}

	public static getVisiblePolygonsReturn ConvertValue(ref getVisiblePolygonsReturn_c s) {
		getVisiblePolygonsReturn ss = new getVisiblePolygonsReturn();
		ss.polygons = ConvertValue(ref s.polygons);
		ss.pixelCounts = Core.Native.NativeInterface.ConvertValue(ref s.pixelCounts);
		return ss;
	}

	public static getVisiblePolygonsReturn_c ConvertValue(getVisiblePolygonsReturn s, ref getVisiblePolygonsReturn_c ss) {
		ConvertValue(s.polygons, ref ss.polygons);
		Core.Native.NativeInterface.ConvertValue(s.pixelCounts, ref ss.pixelCounts);
		return ss;
	}

	public static getMeshSkinningReturn ConvertValue(ref getMeshSkinningReturn_c s) {
		getMeshSkinningReturn ss = new getMeshSkinningReturn();
		ss.joints = ConvertValue(ref s.joints);
		ss.IBMs = Geom.Native.NativeInterface.ConvertValue(ref s.IBMs);
		return ss;
	}

	public static getMeshSkinningReturn_c ConvertValue(getMeshSkinningReturn s, ref getMeshSkinningReturn_c ss) {
		ConvertValue(s.joints, ref ss.joints);
		Geom.Native.NativeInterface.ConvertValue(s.IBMs, ref ss.IBMs);
		return ss;
	}

		#endregion

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Polygonal_getLastError();

		public static string GetLastError()
		{
			return ConvertValue(Polygonal_getLastError());
		}

		#region checksum

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Polygonal_computeMeshTopoChecksum(System.UInt32 mesh);
		/// <summary>
		/// Compute a checksum of the mesh topology, connectivity
		/// </summary>
		/// <param name="mesh">The mesh</param>
		public static System.String ComputeMeshTopoChecksum(System.UInt32 mesh) {
			var ret = Polygonal_computeMeshTopoChecksum(mesh);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Polygonal_computeMeshVertexPositionsChecksum(System.UInt32 mesh, Int32 precisionFloat);
		/// <summary>
		/// Compute a checksum of the mesh vertices positions
		/// </summary>
		/// <param name="mesh">The mesh</param>
		/// <param name="precisionFloat">Floating point precision [1..24], number of significant numbers kept. -1 means no rounded will be applied</param>
		public static System.String ComputeMeshVertexPositionsChecksum(System.UInt32 mesh, System.Int32 precisionFloat) {
			var ret = Polygonal_computeMeshVertexPositionsChecksum(mesh, precisionFloat);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Polygonal_computeUVTopoChecksum(System.UInt32 mesh, Int32 uvChannel);
		/// <summary>
		/// Compute a checksum of the uvs topology, connectivity
		/// </summary>
		/// <param name="mesh">The mesh</param>
		/// <param name="uvChannel">The uv channel</param>
		public static System.String ComputeUVTopoChecksum(System.UInt32 mesh, System.Int32 uvChannel) {
			var ret = Polygonal_computeUVTopoChecksum(mesh, uvChannel);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Polygonal_computeUVVertexPositionsChecksum(System.UInt32 mesh, Int32 uvChannel, Int32 precisionFloat);
		/// <summary>
		/// Compute a checksum of the vertices positions in uv space
		/// </summary>
		/// <param name="mesh">The mesh</param>
		/// <param name="uvChannel">The uv channel</param>
		/// <param name="precisionFloat">Floating point precision [1..24], number of significant numbers kept. -1 means no rounded will be applied</param>
		public static System.String ComputeUVVertexPositionsChecksum(System.UInt32 mesh, System.Int32 uvChannel, System.Int32 precisionFloat) {
			var ret = Polygonal_computeUVVertexPositionsChecksum(mesh, uvChannel, precisionFloat);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		#endregion

		#region draco

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Polygonal_dracoDecode(Core.Native.ByteList_c buffer, Int32 jointIndicesId, Int32 jointWeightsId);
		/// <summary>
		/// decode a mesh using draco
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="jointIndicesId">Unique ID of Generic attribute encoding joint indices</param>
		/// <param name="jointWeightsId">Unique ID of Generic attribute encoding joint weights</param>
		public static System.UInt32 DracoDecode(Core.Native.ByteList buffer, System.Int32 jointIndicesId, System.Int32 jointWeightsId) {
			var buffer_c = new Core.Native.ByteList_c();
			Core.Native.NativeInterface.ConvertValue(buffer, ref buffer_c);
			var ret = Polygonal_dracoDecode(buffer_c, jointIndicesId, jointWeightsId);
			Core.Native.NativeInterface.Core_ByteList_free(ref buffer_c);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern dracoEncodeReturn_c Polygonal_dracoEncode(System.UInt32 mesh, Int32 compressionLevel, Int32 quantizationPosition, Int32 quantizationNormal, Int32 quantizationTexCoord);
		/// <summary>
		/// encode a mesh using draco
		/// </summary>
		/// <param name="mesh"></param>
		/// <param name="compressionLevel">0=faster but the worst compression, 10=slower but the best compression</param>
		/// <param name="quantizationPosition">Number of quantization bits used for position attributes</param>
		/// <param name="quantizationNormal">Number of quantization bits used for normal attributes</param>
		/// <param name="quantizationTexCoord">Number of quantization bits used for texture coordinates attributes</param>
		public static Polygonal.Native.dracoEncodeReturn DracoEncode(System.UInt32 mesh, System.Int32 compressionLevel, System.Int32 quantizationPosition, System.Int32 quantizationNormal, System.Int32 quantizationTexCoord) {
			var ret = Polygonal_dracoEncode(mesh, compressionLevel, quantizationPosition, quantizationNormal, quantizationTexCoord);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			Polygonal.Native.dracoEncodeReturn retStruct = new Polygonal.Native.dracoEncodeReturn();
			retStruct.buffer = ConvertValue(ref ret.buffer);
			Core.Native.NativeInterface.Core_ByteList_free(ref ret.buffer);
			retStruct.jointIndicesId = (System.Int32)ret.jointIndicesId;
			retStruct.jointWeightsId = (System.Int32)ret.jointWeightsId;
			return retStruct;
		}

		#endregion

		#region element attributes access

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Geom.Native.Point3_c Polygonal_getNormal(System.UInt32 Polygon, System.UInt32 Vertex);
		/// <summary>
		/// Return the normal attribute of a polygon at a specified vertex
		/// </summary>
		/// <param name="Polygon">The polygon</param>
		/// <param name="Vertex">The vertex</param>
		public static Geom.Native.Point3 GetNormal(System.UInt32 Polygon, System.UInt32 Vertex) {
			var ret = Polygonal_getNormal(Polygon, Vertex);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Geom.Native.NativeInterface.ConvertValue(ref ret);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern UVCoordList_c Polygonal_getTextureCoordinates(System.UInt32 tessellation);
		/// <summary>
		/// Return the texture coordinates attribute of all the polygons from the tessellation
		/// </summary>
		/// <param name="tessellation">The tessellation of the wanted polygons</param>
		public static UVCoordList GetTextureCoordinates(System.UInt32 tessellation) {
			var ret = Polygonal_getTextureCoordinates(tessellation);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Polygonal.Native.NativeInterface.Polygonal_UVCoordList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getVisiblePolygonsReturn_c Polygonal_getVisiblePolygons(System.UInt32 tessellation);
		/// <summary>
		/// return the visible polygons from the Visibility attributes (see algo.createVisibilityAttributes)
		/// </summary>
		/// <param name="tessellation">The tessellation of the wanted polygons</param>
		public static Polygonal.Native.getVisiblePolygonsReturn GetVisiblePolygons(System.UInt32 tessellation) {
			var ret = Polygonal_getVisiblePolygons(tessellation);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			Polygonal.Native.getVisiblePolygonsReturn retStruct = new Polygonal.Native.getVisiblePolygonsReturn();
			retStruct.polygons = ConvertValue(ref ret.polygons);
			Polygonal.Native.NativeInterface.Polygonal_PolygonList_free(ref ret.polygons);
			retStruct.pixelCounts = ConvertValue(ref ret.pixelCounts);
			Core.Native.NativeInterface.Core_IntList_free(ref ret.pixelCounts);
			return retStruct;
		}

		#endregion

		#region geometry access

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern PlaceholderJointList_c Polygonal_createJointPlaceholders(Core.Native.ULongList_c data, Geom.Native.Matrix4List_c worldMatrices);
		/// <summary>
		/// Create fake joint to store in mesh definitions. Thus we can retrieve stored data from getJointPlaceholders
		/// </summary>
		/// <param name="data">Create as much joints as there are data, each joint store one data</param>
		/// <param name="worldMatrices">World matrix for each joints</param>
		public static PlaceholderJointList CreateJointPlaceholders(Core.Native.ULongList data, Geom.Native.Matrix4List worldMatrices) {
			var data_c = new Core.Native.ULongList_c();
			Core.Native.NativeInterface.ConvertValue(data, ref data_c);
			var worldMatrices_c = new Geom.Native.Matrix4List_c();
			Geom.Native.NativeInterface.ConvertValue(worldMatrices, ref worldMatrices_c);
			var ret = Polygonal_createJointPlaceholders(data_c, worldMatrices_c);
			Core.Native.NativeInterface.Core_ULongList_free(ref data_c);
			Geom.Native.NativeInterface.Geom_Matrix4List_free(ref worldMatrices_c);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Polygonal.Native.NativeInterface.Polygonal_PlaceholderJointList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Polygonal_createMeshFromDefinition(MeshDefinition_c meshDefinition);
		/// <summary>
		/// Create a new mesh with the given MeshDefinition
		/// </summary>
		/// <param name="meshDefinition">Mesh definition</param>
		public static System.UInt32 CreateMeshFromDefinition(MeshDefinition meshDefinition) {
			var meshDefinition_c = new Polygonal.Native.MeshDefinition_c();
			ConvertValue(meshDefinition, ref meshDefinition_c);
			var ret = Polygonal_createMeshFromDefinition(meshDefinition_c);
			Polygonal.Native.NativeInterface.Polygonal_MeshDefinition_free(ref meshDefinition_c);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern MeshList_c Polygonal_createMeshesFromDefinitions(MeshDefinitionList_c meshDefinitions);
		/// <summary>
		/// Create new meshes with the given MeshDefinitions
		/// </summary>
		/// <param name="meshDefinitions">The MeshDefinitions</param>
		public static MeshList CreateMeshesFromDefinitions(MeshDefinitionList meshDefinitions) {
			var meshDefinitions_c = new Polygonal.Native.MeshDefinitionList_c();
			ConvertValue(meshDefinitions, ref meshDefinitions_c);
			var ret = Polygonal_createMeshesFromDefinitions(meshDefinitions_c);
			Polygonal.Native.NativeInterface.Polygonal_MeshDefinitionList_free(ref meshDefinitions_c);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Polygonal.Native.NativeInterface.Polygonal_MeshList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern PolygonList_c Polygonal_getEdgePolygons(System.UInt32 edge);
		/// <summary>
		/// Returns the polygons connected to an edge
		/// </summary>
		/// <param name="edge">The edge</param>
		public static PolygonList GetEdgePolygons(System.UInt32 edge) {
			var ret = Polygonal_getEdgePolygons(edge);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Polygonal.Native.NativeInterface.Polygonal_PolygonList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern VertexList_c Polygonal_getEdgeVertices(System.UInt32 edge);
		/// <summary>
		/// Returns the vertices of an edge
		/// </summary>
		/// <param name="edge">The edge</param>
		public static VertexList GetEdgeVertices(System.UInt32 edge) {
			var ret = Polygonal_getEdgeVertices(edge);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Polygonal.Native.NativeInterface.Polygonal_VertexList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern EdgeList_c Polygonal_getEdges(System.UInt32 tessellation, TopologyCategoryMask_c category);
		/// <summary>
		/// Returns the edges corresponding to the given connectivity
		/// </summary>
		/// <param name="tessellation">The tessellation</param>
		/// <param name="category">Category mask of the wanted edges</param>
		public static EdgeList GetEdges(System.UInt32 tessellation, TopologyCategoryMask category) {
			var category_c = new Polygonal.Native.TopologyCategoryMask_c();
			ConvertValue(category, ref category_c);
			var ret = Polygonal_getEdges(tessellation, category_c);
			Polygonal.Native.NativeInterface.Polygonal_TopologyCategoryMask_free(ref category_c);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Polygonal.Native.NativeInterface.Polygonal_EdgeList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern EdgeList_c Polygonal_getFreeEdges(System.UInt32 tessellation);
		/// <summary>
		/// Returns the free edges of a tessellation
		/// </summary>
		/// <param name="tessellation">The tessellation</param>
		public static EdgeList GetFreeEdges(System.UInt32 tessellation) {
			var ret = Polygonal_getFreeEdges(tessellation);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Polygonal.Native.NativeInterface.Polygonal_EdgeList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern VertexList_c Polygonal_getFreeVertices(System.UInt32 tessellation);
		/// <summary>
		/// Returns the free vertices of a tessellation
		/// </summary>
		/// <param name="tessellation">The tessellation</param>
		public static VertexList GetFreeVertices(System.UInt32 tessellation) {
			var ret = Polygonal_getFreeVertices(tessellation);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Polygonal.Native.NativeInterface.Polygonal_VertexList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Core.Native.ULongList_c Polygonal_getJointPlaceholders(PlaceholderJointList_c joints);
		/// <summary>
		/// Get data stored in joint placeholders
		/// </summary>
		/// <param name="joints">Placeholder joints to get data from</param>
		public static Core.Native.ULongList GetJointPlaceholders(PlaceholderJointList joints) {
			var joints_c = new Polygonal.Native.PlaceholderJointList_c();
			ConvertValue(joints, ref joints_c);
			var ret = Polygonal_getJointPlaceholders(joints_c);
			Polygonal.Native.NativeInterface.Polygonal_PlaceholderJointList_free(ref joints_c);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Core.Native.NativeInterface.ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_ULongList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern MeshDefinition_c Polygonal_getMeshDefinition(System.UInt32 mesh);
		/// <summary>
		/// Returns the definition
		/// </summary>
		/// <param name="mesh">The mesh to get definition</param>
		public static MeshDefinition GetMeshDefinition(System.UInt32 mesh) {
			var ret = Polygonal_getMeshDefinition(mesh);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Polygonal.Native.NativeInterface.Polygonal_MeshDefinition_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern MeshDefinitionList_c Polygonal_getMeshDefinitions(MeshList_c meshes);
		/// <summary>
		/// Returns the definition
		/// </summary>
		/// <param name="meshes">The meshes to get definitions</param>
		public static MeshDefinitionList GetMeshDefinitions(MeshList meshes) {
			var meshes_c = new Polygonal.Native.MeshList_c();
			ConvertValue(meshes, ref meshes_c);
			var ret = Polygonal_getMeshDefinitions(meshes_c);
			Polygonal.Native.NativeInterface.Polygonal_MeshList_free(ref meshes_c);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Polygonal.Native.NativeInterface.Polygonal_MeshDefinitionList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getMeshSkinningReturn_c Polygonal_getMeshSkinning(System.UInt32 mesh);
		/// <summary>
		/// Returns the joints/IBMs list of a given mesh (those referenced by jointIndices)
		/// </summary>
		/// <param name="mesh"></param>
		public static Polygonal.Native.getMeshSkinningReturn GetMeshSkinning(System.UInt32 mesh) {
			var ret = Polygonal_getMeshSkinning(mesh);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			Polygonal.Native.getMeshSkinningReturn retStruct = new Polygonal.Native.getMeshSkinningReturn();
			retStruct.joints = ConvertValue(ref ret.joints);
			Polygonal.Native.NativeInterface.Polygonal_JointList_free(ref ret.joints);
			retStruct.IBMs = ConvertValue(ref ret.IBMs);
			Geom.Native.NativeInterface.Geom_Matrix4List_free(ref ret.IBMs);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern PatchList_c Polygonal_getPatches(System.UInt32 tessellation);
		/// <summary>
		/// Returns the patches of a tessellation
		/// </summary>
		/// <param name="tessellation">The tessellation</param>
		public static PatchList GetPatches(System.UInt32 tessellation) {
			var ret = Polygonal_getPatches(tessellation);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Polygonal.Native.NativeInterface.Polygonal_PatchList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern EdgeList_c Polygonal_getPolygonEdges(System.UInt32 polygon);
		/// <summary>
		/// Returns the edges of a a polygon
		/// </summary>
		/// <param name="polygon">The polygon</param>
		public static EdgeList GetPolygonEdges(System.UInt32 polygon) {
			var ret = Polygonal_getPolygonEdges(polygon);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Polygonal.Native.NativeInterface.Polygonal_EdgeList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern VertexList_c Polygonal_getPolygonVertices(System.UInt32 polygon);
		/// <summary>
		/// Returns the vertices of a a polygon
		/// </summary>
		/// <param name="polygon">The polygon</param>
		public static VertexList GetPolygonVertices(System.UInt32 polygon) {
			var ret = Polygonal_getPolygonVertices(polygon);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Polygonal.Native.NativeInterface.Polygonal_VertexList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern PolygonList_c Polygonal_getPolygons(System.UInt32 tessellation);
		/// <summary>
		/// Returns the polygons of a tessellation
		/// </summary>
		/// <param name="tessellation">The tessellation</param>
		public static PolygonList GetPolygons(System.UInt32 tessellation) {
			var ret = Polygonal_getPolygons(tessellation);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Polygonal.Native.NativeInterface.Polygonal_PolygonList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern EdgeListList_c Polygonal_getTessellationBoundaries(System.UInt32 tessellation);
		/// <summary>
		/// Get boundary edges of a tessellation grouped by cycles
		/// </summary>
		/// <param name="tessellation">The Tessellation</param>
		public static EdgeListList GetTessellationBoundaries(System.UInt32 tessellation) {
			var ret = Polygonal_getTessellationBoundaries(tessellation);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Polygonal.Native.NativeInterface.Polygonal_EdgeListList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Geom.Native.Point3_c Polygonal_getVertexCoordinates(System.UInt32 vertex);
		/// <summary>
		/// Returns the vertex coordinates in the tessellation local space
		/// </summary>
		/// <param name="vertex">The vertex</param>
		public static Geom.Native.Point3 GetVertexCoordinates(System.UInt32 vertex) {
			var ret = Polygonal_getVertexCoordinates(vertex);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Geom.Native.NativeInterface.ConvertValue(ref ret);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern EdgeList_c Polygonal_getVertexEdges(System.UInt32 vertex);
		/// <summary>
		/// Returns the edges connected to a vertex
		/// </summary>
		/// <param name="vertex">The vertex</param>
		public static EdgeList GetVertexEdges(System.UInt32 vertex) {
			var ret = Polygonal_getVertexEdges(vertex);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Polygonal.Native.NativeInterface.Polygonal_EdgeList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern PolygonList_c Polygonal_getVertexPolygons(System.UInt32 vertex);
		/// <summary>
		/// Returns the polygons connected to a vertex
		/// </summary>
		/// <param name="vertex">The vertex</param>
		public static PolygonList GetVertexPolygons(System.UInt32 vertex) {
			var ret = Polygonal_getVertexPolygons(vertex);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Polygonal.Native.NativeInterface.Polygonal_PolygonList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern VertexList_c Polygonal_getVertices(System.UInt32 tessellation, TopologyCategoryMask_c category);
		/// <summary>
		/// Returns the vertices of a tessellation
		/// </summary>
		/// <param name="tessellation">The tessellation</param>
		/// <param name="category">Category mask of the wanted edges</param>
		public static VertexList GetVertices(System.UInt32 tessellation, TopologyCategoryMask category) {
			var category_c = new Polygonal.Native.TopologyCategoryMask_c();
			ConvertValue(category, ref category_c);
			var ret = Polygonal_getVertices(tessellation, category_c);
			Polygonal.Native.NativeInterface.Polygonal_TopologyCategoryMask_free(ref category_c);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Polygonal.Native.NativeInterface.Polygonal_VertexList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Polygonal_setMeshSkinning(System.UInt32 mesh, JointList_c joints, Geom.Native.Matrix4List_c IBMs);
		/// <summary>
		/// Set/Replace the list of joints/IBMs of a given mesh (those referenced by jointIndices)
		/// </summary>
		/// <param name="mesh"></param>
		/// <param name="joints"></param>
		/// <param name="IBMs">Inverse Bind Matrices</param>
		public static void SetMeshSkinning(System.UInt32 mesh, JointList joints, Geom.Native.Matrix4List IBMs) {
			var joints_c = new Polygonal.Native.JointList_c();
			ConvertValue(joints, ref joints_c);
			var IBMs_c = new Geom.Native.Matrix4List_c();
			Geom.Native.NativeInterface.ConvertValue(IBMs, ref IBMs_c);
			Polygonal_setMeshSkinning(mesh, joints_c, IBMs_c);
			Polygonal.Native.NativeInterface.Polygonal_JointList_free(ref joints_c);
			Geom.Native.NativeInterface.Geom_Matrix4List_free(ref IBMs_c);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region modification

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Polygonal_destroyElements(ElementList_c elements);
		/// <summary>
		/// Destroy all the given elements
		/// </summary>
		/// <param name="elements">List of elements to destroy</param>
		public static void DestroyElements(ElementList elements) {
			var elements_c = new Polygonal.Native.ElementList_c();
			ConvertValue(elements, ref elements_c);
			Polygonal_destroyElements(elements_c);
			Polygonal.Native.NativeInterface.Polygonal_ElementList_free(ref elements_c);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Polygonal_invertElements(ElementList_c elements);
		/// <summary>
		/// Invert the orientation of all the given elements
		/// </summary>
		/// <param name="elements">List of elements to invert</param>
		public static void InvertElements(ElementList elements) {
			var elements_c = new Polygonal.Native.ElementList_c();
			ConvertValue(elements, ref elements_c);
			Polygonal_invertElements(elements_c);
			Polygonal.Native.NativeInterface.Polygonal_ElementList_free(ref elements_c);
			System.String err = ConvertValue(Polygonal_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

	}
}
