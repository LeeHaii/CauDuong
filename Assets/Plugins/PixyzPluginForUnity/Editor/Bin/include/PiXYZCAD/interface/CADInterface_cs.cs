#pragma warning disable CA2101

using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Security;
using System.Collections.Concurrent;
using PiXYZAPI = Pixyz.API.Native.NativeInterface;

namespace Pixyz.CAD.Native {

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
		internal static extern void CAD_ClosedShellList_init(ref ClosedShellList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_ClosedShellList_free(ref ClosedShellList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_OpenShellList_init(ref OpenShellList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_OpenShellList_free(ref OpenShellList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_OrientedEdge_init(ref OrientedEdge_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_OrientedEdge_free(ref OrientedEdge_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_OrientedEdgeList_init(ref OrientedEdgeList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_OrientedEdgeList_free(ref OrientedEdgeList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_CurveList_init(ref CurveList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_CurveList_free(ref CurveList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_OrientedDomain_init(ref OrientedDomain_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_OrientedDomain_free(ref OrientedDomain_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_Bounds1D_init(ref Bounds1D_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_Bounds1D_free(ref Bounds1D_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_Bounds2D_init(ref Bounds2D_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_Bounds2D_free(ref Bounds2D_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_EdgeList_init(ref EdgeList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_EdgeList_free(ref EdgeList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_SplittedEdge_init(ref SplittedEdge_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_SplittedEdge_free(ref SplittedEdge_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_OrientedDomainList_init(ref OrientedDomainList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_OrientedDomainList_free(ref OrientedDomainList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_DomainList_init(ref DomainList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_DomainList_free(ref DomainList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_EdgeListList_init(ref EdgeListList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_EdgeListList_free(ref EdgeListList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_BodyList_init(ref BodyList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_BodyList_free(ref BodyList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_FaceList_init(ref FaceList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_FaceList_free(ref FaceList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_VertexList_init(ref VertexList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_VertexList_free(ref VertexList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_SplittedEdgeList_init(ref SplittedEdgeList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_SplittedEdgeList_free(ref SplittedEdgeList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_LimitedCurveList_init(ref LimitedCurveList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_LimitedCurveList_free(ref LimitedCurveList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_ModelList_init(ref ModelList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_ModelList_free(ref ModelList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_CoEdgeList_init(ref CoEdgeList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_CoEdgeList_free(ref CoEdgeList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_LoopList_init(ref LoopList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void CAD_LoopList_free(ref LoopList_c list);

	public static ClosedShellList ConvertValue(ref ClosedShellList_c s) {
		ClosedShellList list = new ClosedShellList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static ClosedShellList_c ConvertValue(ClosedShellList s, ref ClosedShellList_c list) {
		CAD.Native.NativeInterface.CAD_ClosedShellList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static OpenShellList ConvertValue(ref OpenShellList_c s) {
		OpenShellList list = new OpenShellList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static OpenShellList_c ConvertValue(OpenShellList s, ref OpenShellList_c list) {
		CAD.Native.NativeInterface.CAD_OpenShellList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static OrientedEdge ConvertValue(ref OrientedEdge_c s) {
		OrientedEdge ss = new OrientedEdge();
		ss.edge = (System.UInt32)s.edge;
		ss.orientation = ConvertValue(s.orientation);
		return ss;
	}

	public static OrientedEdge_c ConvertValue(OrientedEdge s, ref OrientedEdge_c ss) {
		CAD.Native.NativeInterface.CAD_OrientedEdge_init(ref ss);
		ss.edge = (System.UInt32)s.edge;
		ss.orientation = ConvertValue(s.orientation);
		return ss;
	}

	public static OrientedEdgeList ConvertValue(ref OrientedEdgeList_c s) {
		OrientedEdgeList list = new OrientedEdgeList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<OrientedEdge>(s.ptr, (int)s.size);
		return list;
	}

	public static OrientedEdgeList_c ConvertValue(OrientedEdgeList s, ref OrientedEdgeList_c list) {
		CAD.Native.NativeInterface.CAD_OrientedEdgeList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			OrientedEdge_c elt = new OrientedEdge_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(OrientedEdge_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static CurveList ConvertValue(ref CurveList_c s) {
		CurveList list = new CurveList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static CurveList_c ConvertValue(CurveList s, ref CurveList_c list) {
		CAD.Native.NativeInterface.CAD_CurveList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static OrientedDomain ConvertValue(ref OrientedDomain_c s) {
		OrientedDomain ss = new OrientedDomain();
		ss.domain = (System.UInt32)s.domain;
		ss.orientation = ConvertValue(s.orientation);
		return ss;
	}

	public static OrientedDomain_c ConvertValue(OrientedDomain s, ref OrientedDomain_c ss) {
		CAD.Native.NativeInterface.CAD_OrientedDomain_init(ref ss);
		ss.domain = (System.UInt32)s.domain;
		ss.orientation = ConvertValue(s.orientation);
		return ss;
	}

	public static Bounds1D ConvertValue(ref Bounds1D_c s) {
		Bounds1D ss = new Bounds1D();
		ss.min = (System.Double)s.min;
		ss.max = (System.Double)s.max;
		return ss;
	}

	public static Bounds1D_c ConvertValue(Bounds1D s, ref Bounds1D_c ss) {
		CAD.Native.NativeInterface.CAD_Bounds1D_init(ref ss);
		ss.min = (System.Double)s.min;
		ss.max = (System.Double)s.max;
		return ss;
	}

	public static Bounds2D ConvertValue(ref Bounds2D_c s) {
		Bounds2D ss = new Bounds2D();
		ss.u = ConvertValue(ref s.u);
		ss.v = ConvertValue(ref s.v);
		return ss;
	}

	public static Bounds2D_c ConvertValue(Bounds2D s, ref Bounds2D_c ss) {
		CAD.Native.NativeInterface.CAD_Bounds2D_init(ref ss);
		ConvertValue(s.u, ref ss.u);
		ConvertValue(s.v, ref ss.v);
		return ss;
	}

	public static EdgeList ConvertValue(ref EdgeList_c s) {
		EdgeList list = new EdgeList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static EdgeList_c ConvertValue(EdgeList s, ref EdgeList_c list) {
		CAD.Native.NativeInterface.CAD_EdgeList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static SplittedEdge ConvertValue(ref SplittedEdge_c s) {
		SplittedEdge ss = new SplittedEdge();
		ss.oldEdge = (System.UInt32)s.oldEdge;
		ss.newEdges = ConvertValue(ref s.newEdges);
		return ss;
	}

	public static SplittedEdge_c ConvertValue(SplittedEdge s, ref SplittedEdge_c ss) {
		CAD.Native.NativeInterface.CAD_SplittedEdge_init(ref ss);
		ss.oldEdge = (System.UInt32)s.oldEdge;
		ConvertValue(s.newEdges, ref ss.newEdges);
		return ss;
	}

	public static OrientedDomainList ConvertValue(ref OrientedDomainList_c s) {
		OrientedDomainList list = new OrientedDomainList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<OrientedDomain>(s.ptr, (int)s.size);
		return list;
	}

	public static OrientedDomainList_c ConvertValue(OrientedDomainList s, ref OrientedDomainList_c list) {
		CAD.Native.NativeInterface.CAD_OrientedDomainList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			OrientedDomain_c elt = new OrientedDomain_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(OrientedDomain_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static DomainList ConvertValue(ref DomainList_c s) {
		DomainList list = new DomainList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static DomainList_c ConvertValue(DomainList s, ref DomainList_c list) {
		CAD.Native.NativeInterface.CAD_DomainList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
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
		CAD.Native.NativeInterface.CAD_EdgeListList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			EdgeList_c elt = new EdgeList_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(EdgeList_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static BodyList ConvertValue(ref BodyList_c s) {
		BodyList list = new BodyList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static BodyList_c ConvertValue(BodyList s, ref BodyList_c list) {
		CAD.Native.NativeInterface.CAD_BodyList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static FaceList ConvertValue(ref FaceList_c s) {
		FaceList list = new FaceList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static FaceList_c ConvertValue(FaceList s, ref FaceList_c list) {
		CAD.Native.NativeInterface.CAD_FaceList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
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
		CAD.Native.NativeInterface.CAD_VertexList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static SplittedEdgeList ConvertValue(ref SplittedEdgeList_c s) {
		SplittedEdgeList list = new SplittedEdgeList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(SplittedEdge_c)));
			SplittedEdge_c value = (SplittedEdge_c)Marshal.PtrToStructure(p, typeof(SplittedEdge_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static SplittedEdgeList_c ConvertValue(SplittedEdgeList s, ref SplittedEdgeList_c list) {
		CAD.Native.NativeInterface.CAD_SplittedEdgeList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			SplittedEdge_c elt = new SplittedEdge_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(SplittedEdge_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static LimitedCurveList ConvertValue(ref LimitedCurveList_c s) {
		LimitedCurveList list = new LimitedCurveList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static LimitedCurveList_c ConvertValue(LimitedCurveList s, ref LimitedCurveList_c list) {
		CAD.Native.NativeInterface.CAD_LimitedCurveList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static ModelList ConvertValue(ref ModelList_c s) {
		ModelList list = new ModelList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static ModelList_c ConvertValue(ModelList s, ref ModelList_c list) {
		CAD.Native.NativeInterface.CAD_ModelList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static CoEdgeList ConvertValue(ref CoEdgeList_c s) {
		CoEdgeList list = new CoEdgeList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static CoEdgeList_c ConvertValue(CoEdgeList s, ref CoEdgeList_c list) {
		CAD.Native.NativeInterface.CAD_CoEdgeList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static LoopList ConvertValue(ref LoopList_c s) {
		LoopList list = new LoopList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static LoopList_c ConvertValue(LoopList s, ref LoopList_c list) {
		CAD.Native.NativeInterface.CAD_LoopList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static buildFacesReturn ConvertValue(ref buildFacesReturn_c s) {
		buildFacesReturn ss = new buildFacesReturn();
		ss.domain = (System.UInt32)s.domain;
		ss.splittingInfo = ConvertValue(ref s.splittingInfo);
		return ss;
	}

	public static buildFacesReturn_c ConvertValue(buildFacesReturn s, ref buildFacesReturn_c ss) {
		ss.domain = (System.UInt32)s.domain;
		ConvertValue(s.splittingInfo, ref ss.splittingInfo);
		return ss;
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

	public static evalOnCurveReturn ConvertValue(ref evalOnCurveReturn_c s) {
		evalOnCurveReturn ss = new evalOnCurveReturn();
		ss.d0 = Geom.Native.NativeInterface.ConvertValue(ref s.d0);
		ss.du = Geom.Native.NativeInterface.ConvertValue(ref s.du);
		ss.d2u = Geom.Native.NativeInterface.ConvertValue(ref s.d2u);
		return ss;
	}

	public static evalOnCurveReturn_c ConvertValue(evalOnCurveReturn s, ref evalOnCurveReturn_c ss) {
		Geom.Native.NativeInterface.ConvertValue(s.d0, ref ss.d0);
		Geom.Native.NativeInterface.ConvertValue(s.du, ref ss.du);
		Geom.Native.NativeInterface.ConvertValue(s.d2u, ref ss.d2u);
		return ss;
	}

	public static evalOnSurfaceReturn ConvertValue(ref evalOnSurfaceReturn_c s) {
		evalOnSurfaceReturn ss = new evalOnSurfaceReturn();
		ss.d0 = Geom.Native.NativeInterface.ConvertValue(ref s.d0);
		ss.du = Geom.Native.NativeInterface.ConvertValue(ref s.du);
		ss.dv = Geom.Native.NativeInterface.ConvertValue(ref s.dv);
		ss.d2u = Geom.Native.NativeInterface.ConvertValue(ref s.d2u);
		ss.d2v = Geom.Native.NativeInterface.ConvertValue(ref s.d2v);
		ss.duv = Geom.Native.NativeInterface.ConvertValue(ref s.duv);
		return ss;
	}

	public static evalOnSurfaceReturn_c ConvertValue(evalOnSurfaceReturn s, ref evalOnSurfaceReturn_c ss) {
		Geom.Native.NativeInterface.ConvertValue(s.d0, ref ss.d0);
		Geom.Native.NativeInterface.ConvertValue(s.du, ref ss.du);
		Geom.Native.NativeInterface.ConvertValue(s.dv, ref ss.dv);
		Geom.Native.NativeInterface.ConvertValue(s.d2u, ref ss.d2u);
		Geom.Native.NativeInterface.ConvertValue(s.d2v, ref ss.d2v);
		Geom.Native.NativeInterface.ConvertValue(s.duv, ref ss.duv);
		return ss;
	}

	public static getBoundedCurveDefinitionReturn ConvertValue(ref getBoundedCurveDefinitionReturn_c s) {
		getBoundedCurveDefinitionReturn ss = new getBoundedCurveDefinitionReturn();
		ss.curve = (System.UInt32)s.curve;
		ss.bounds = ConvertValue(ref s.bounds);
		return ss;
	}

	public static getBoundedCurveDefinitionReturn_c ConvertValue(getBoundedCurveDefinitionReturn s, ref getBoundedCurveDefinitionReturn_c ss) {
		ss.curve = (System.UInt32)s.curve;
		ConvertValue(s.bounds, ref ss.bounds);
		return ss;
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

	public static getCircleCurveDefinitionReturn ConvertValue(ref getCircleCurveDefinitionReturn_c s) {
		getCircleCurveDefinitionReturn ss = new getCircleCurveDefinitionReturn();
		ss.radius = (System.Double)s.radius;
		ss.matrix = Geom.Native.NativeInterface.ConvertValue(ref s.matrix);
		return ss;
	}

	public static getCircleCurveDefinitionReturn_c ConvertValue(getCircleCurveDefinitionReturn s, ref getCircleCurveDefinitionReturn_c ss) {
		ss.radius = (System.Double)s.radius;
		Geom.Native.NativeInterface.ConvertValue(s.matrix, ref ss.matrix);
		return ss;
	}

	public static getCoEdgeDefinitionReturn ConvertValue(ref getCoEdgeDefinitionReturn_c s) {
		getCoEdgeDefinitionReturn ss = new getCoEdgeDefinitionReturn();
		ss.edge = (System.UInt32)s.edge;
		ss.edgeOrientation = ConvertValue(s.edgeOrientation);
		ss.loop = (System.UInt32)s.loop;
		ss.surface = (System.UInt32)s.surface;
		ss.parametricCurve = (System.UInt32)s.parametricCurve;
		return ss;
	}

	public static getCoEdgeDefinitionReturn_c ConvertValue(getCoEdgeDefinitionReturn s, ref getCoEdgeDefinitionReturn_c ss) {
		ss.edge = (System.UInt32)s.edge;
		ss.edgeOrientation = ConvertValue(s.edgeOrientation);
		ss.loop = (System.UInt32)s.loop;
		ss.surface = (System.UInt32)s.surface;
		ss.parametricCurve = (System.UInt32)s.parametricCurve;
		return ss;
	}

	public static Core.Native.DoubleList ConvertValue(ref Core.Native.DoubleList_c s) {
		Core.Native.DoubleList list = new Core.Native.DoubleList((int)s.size);
		if (s.size > 0)
			Marshal.Copy(s.ptr, list.list, 0, (int)s.size);
		return list;
	}

	public static Core.Native.DoubleList_c ConvertValue(Core.Native.DoubleList s, ref Core.Native.DoubleList_c list) {
		Core.Native.NativeInterface.Core_DoubleList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size > 0)
			Marshal.Copy(s.list, 0, list.ptr, (int)list.size);
		return list;
	}

	public static getCompositeCurveDefinitionReturn ConvertValue(ref getCompositeCurveDefinitionReturn_c s) {
		getCompositeCurveDefinitionReturn ss = new getCompositeCurveDefinitionReturn();
		ss.curves = ConvertValue(ref s.curves);
		ss.parameters = Core.Native.NativeInterface.ConvertValue(ref s.parameters);
		return ss;
	}

	public static getCompositeCurveDefinitionReturn_c ConvertValue(getCompositeCurveDefinitionReturn s, ref getCompositeCurveDefinitionReturn_c ss) {
		ConvertValue(s.curves, ref ss.curves);
		Core.Native.NativeInterface.ConvertValue(s.parameters, ref ss.parameters);
		return ss;
	}

	public static getConeSurfaceDefinitionReturn ConvertValue(ref getConeSurfaceDefinitionReturn_c s) {
		getConeSurfaceDefinitionReturn ss = new getConeSurfaceDefinitionReturn();
		ss.radius = (System.Double)s.radius;
		ss.semiAngle = (System.Double)s.semiAngle;
		ss.matrix = Geom.Native.NativeInterface.ConvertValue(ref s.matrix);
		return ss;
	}

	public static getConeSurfaceDefinitionReturn_c ConvertValue(getConeSurfaceDefinitionReturn s, ref getConeSurfaceDefinitionReturn_c ss) {
		ss.radius = (System.Double)s.radius;
		ss.semiAngle = (System.Double)s.semiAngle;
		Geom.Native.NativeInterface.ConvertValue(s.matrix, ref ss.matrix);
		return ss;
	}

	public static getCurveExtrusionSurfaceDefinitionReturn ConvertValue(ref getCurveExtrusionSurfaceDefinitionReturn_c s) {
		getCurveExtrusionSurfaceDefinitionReturn ss = new getCurveExtrusionSurfaceDefinitionReturn();
		ss.generatrixCurve = (System.UInt32)s.generatrixCurve;
		ss.directrixCruve = (System.UInt32)s.directrixCruve;
		ss.surfaceReference = (System.UInt32)s.surfaceReference;
		return ss;
	}

	public static getCurveExtrusionSurfaceDefinitionReturn_c ConvertValue(getCurveExtrusionSurfaceDefinitionReturn s, ref getCurveExtrusionSurfaceDefinitionReturn_c ss) {
		ss.generatrixCurve = (System.UInt32)s.generatrixCurve;
		ss.directrixCruve = (System.UInt32)s.directrixCruve;
		ss.surfaceReference = (System.UInt32)s.surfaceReference;
		return ss;
	}

	public static getCylinderSurfaceDefinitionReturn ConvertValue(ref getCylinderSurfaceDefinitionReturn_c s) {
		getCylinderSurfaceDefinitionReturn ss = new getCylinderSurfaceDefinitionReturn();
		ss.radius = (System.Double)s.radius;
		ss.matrix = Geom.Native.NativeInterface.ConvertValue(ref s.matrix);
		return ss;
	}

	public static getCylinderSurfaceDefinitionReturn_c ConvertValue(getCylinderSurfaceDefinitionReturn s, ref getCylinderSurfaceDefinitionReturn_c ss) {
		ss.radius = (System.Double)s.radius;
		Geom.Native.NativeInterface.ConvertValue(s.matrix, ref ss.matrix);
		return ss;
	}

	public static getEdgeDefinitionReturn ConvertValue(ref getEdgeDefinitionReturn_c s) {
		getEdgeDefinitionReturn ss = new getEdgeDefinitionReturn();
		ss.vertex1 = (System.UInt32)s.vertex1;
		ss.vertex2 = (System.UInt32)s.vertex2;
		ss.curve = (System.UInt32)s.curve;
		ss.bounds = ConvertValue(ref s.bounds);
		return ss;
	}

	public static getEdgeDefinitionReturn_c ConvertValue(getEdgeDefinitionReturn s, ref getEdgeDefinitionReturn_c ss) {
		ss.vertex1 = (System.UInt32)s.vertex1;
		ss.vertex2 = (System.UInt32)s.vertex2;
		ss.curve = (System.UInt32)s.curve;
		ConvertValue(s.bounds, ref ss.bounds);
		return ss;
	}

	public static getEllipseCurveDefinitionReturn ConvertValue(ref getEllipseCurveDefinitionReturn_c s) {
		getEllipseCurveDefinitionReturn ss = new getEllipseCurveDefinitionReturn();
		ss.radius1 = (System.Double)s.radius1;
		ss.radius2 = (System.Double)s.radius2;
		ss.matrix = Geom.Native.NativeInterface.ConvertValue(ref s.matrix);
		return ss;
	}

	public static getEllipseCurveDefinitionReturn_c ConvertValue(getEllipseCurveDefinitionReturn s, ref getEllipseCurveDefinitionReturn_c ss) {
		ss.radius1 = (System.Double)s.radius1;
		ss.radius2 = (System.Double)s.radius2;
		Geom.Native.NativeInterface.ConvertValue(s.matrix, ref ss.matrix);
		return ss;
	}

	public static getEllipticConeSurfaceDefinitionReturn ConvertValue(ref getEllipticConeSurfaceDefinitionReturn_c s) {
		getEllipticConeSurfaceDefinitionReturn ss = new getEllipticConeSurfaceDefinitionReturn();
		ss.radius1 = (System.Double)s.radius1;
		ss.radius2 = (System.Double)s.radius2;
		ss.semiAngle = (System.Double)s.semiAngle;
		ss.matrix = Geom.Native.NativeInterface.ConvertValue(ref s.matrix);
		return ss;
	}

	public static getEllipticConeSurfaceDefinitionReturn_c ConvertValue(getEllipticConeSurfaceDefinitionReturn s, ref getEllipticConeSurfaceDefinitionReturn_c ss) {
		ss.radius1 = (System.Double)s.radius1;
		ss.radius2 = (System.Double)s.radius2;
		ss.semiAngle = (System.Double)s.semiAngle;
		Geom.Native.NativeInterface.ConvertValue(s.matrix, ref ss.matrix);
		return ss;
	}

	public static getFaceDefinitionReturn ConvertValue(ref getFaceDefinitionReturn_c s) {
		getFaceDefinitionReturn ss = new getFaceDefinitionReturn();
		ss.surface = (System.UInt32)s.surface;
		ss.loops = ConvertValue(ref s.loops);
		ss.orientation = ConvertValue(s.orientation);
		ss.limits = ConvertValue(ref s.limits);
		return ss;
	}

	public static getFaceDefinitionReturn_c ConvertValue(getFaceDefinitionReturn s, ref getFaceDefinitionReturn_c ss) {
		ss.surface = (System.UInt32)s.surface;
		ConvertValue(s.loops, ref ss.loops);
		ss.orientation = ConvertValue(s.orientation);
		ConvertValue(s.limits, ref ss.limits);
		return ss;
	}

	public static getHelixCurveDefinitionReturn ConvertValue(ref getHelixCurveDefinitionReturn_c s) {
		getHelixCurveDefinitionReturn ss = new getHelixCurveDefinitionReturn();
		ss.radius = (System.Double)s.radius;
		ss.matrix = Geom.Native.NativeInterface.ConvertValue(ref s.matrix);
		ss.trigonometricOrientation = ConvertValue(s.trigonometricOrientation);
		return ss;
	}

	public static getHelixCurveDefinitionReturn_c ConvertValue(getHelixCurveDefinitionReturn s, ref getHelixCurveDefinitionReturn_c ss) {
		ss.radius = (System.Double)s.radius;
		Geom.Native.NativeInterface.ConvertValue(s.matrix, ref ss.matrix);
		ss.trigonometricOrientation = ConvertValue(s.trigonometricOrientation);
		return ss;
	}

	public static getHermiteCurveDefinitionReturn ConvertValue(ref getHermiteCurveDefinitionReturn_c s) {
		getHermiteCurveDefinitionReturn ss = new getHermiteCurveDefinitionReturn();
		ss.firstPoint = Geom.Native.NativeInterface.ConvertValue(ref s.firstPoint);
		ss.secondPoint = Geom.Native.NativeInterface.ConvertValue(ref s.secondPoint);
		ss.firstTangent = Geom.Native.NativeInterface.ConvertValue(ref s.firstTangent);
		ss.secondTangent = Geom.Native.NativeInterface.ConvertValue(ref s.secondTangent);
		return ss;
	}

	public static getHermiteCurveDefinitionReturn_c ConvertValue(getHermiteCurveDefinitionReturn s, ref getHermiteCurveDefinitionReturn_c ss) {
		Geom.Native.NativeInterface.ConvertValue(s.firstPoint, ref ss.firstPoint);
		Geom.Native.NativeInterface.ConvertValue(s.secondPoint, ref ss.secondPoint);
		Geom.Native.NativeInterface.ConvertValue(s.firstTangent, ref ss.firstTangent);
		Geom.Native.NativeInterface.ConvertValue(s.secondTangent, ref ss.secondTangent);
		return ss;
	}

	public static getHyperbolaCurveDefinitionReturn ConvertValue(ref getHyperbolaCurveDefinitionReturn_c s) {
		getHyperbolaCurveDefinitionReturn ss = new getHyperbolaCurveDefinitionReturn();
		ss.radius1 = (System.Double)s.radius1;
		ss.radius2 = (System.Double)s.radius2;
		ss.matrix = Geom.Native.NativeInterface.ConvertValue(ref s.matrix);
		return ss;
	}

	public static getHyperbolaCurveDefinitionReturn_c ConvertValue(getHyperbolaCurveDefinitionReturn s, ref getHyperbolaCurveDefinitionReturn_c ss) {
		ss.radius1 = (System.Double)s.radius1;
		ss.radius2 = (System.Double)s.radius2;
		Geom.Native.NativeInterface.ConvertValue(s.matrix, ref ss.matrix);
		return ss;
	}

	public static getIntersectionCurveDefinitionReturn ConvertValue(ref getIntersectionCurveDefinitionReturn_c s) {
		getIntersectionCurveDefinitionReturn ss = new getIntersectionCurveDefinitionReturn();
		ss.surface1 = (System.UInt32)s.surface1;
		ss.surface2 = (System.UInt32)s.surface2;
		ss.chart = (System.UInt32)s.chart;
		ss.bounds = ConvertValue(ref s.bounds);
		return ss;
	}

	public static getIntersectionCurveDefinitionReturn_c ConvertValue(getIntersectionCurveDefinitionReturn s, ref getIntersectionCurveDefinitionReturn_c ss) {
		ss.surface1 = (System.UInt32)s.surface1;
		ss.surface2 = (System.UInt32)s.surface2;
		ss.chart = (System.UInt32)s.chart;
		ConvertValue(s.bounds, ref ss.bounds);
		return ss;
	}

	public static getLineCurveDefinitionReturn ConvertValue(ref getLineCurveDefinitionReturn_c s) {
		getLineCurveDefinitionReturn ss = new getLineCurveDefinitionReturn();
		ss.origin = Geom.Native.NativeInterface.ConvertValue(ref s.origin);
		ss.direction = Geom.Native.NativeInterface.ConvertValue(ref s.direction);
		return ss;
	}

	public static getLineCurveDefinitionReturn_c ConvertValue(getLineCurveDefinitionReturn s, ref getLineCurveDefinitionReturn_c ss) {
		Geom.Native.NativeInterface.ConvertValue(s.origin, ref ss.origin);
		Geom.Native.NativeInterface.ConvertValue(s.direction, ref ss.direction);
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

	public static getNURBSCurveDefinitionReturn ConvertValue(ref getNURBSCurveDefinitionReturn_c s) {
		getNURBSCurveDefinitionReturn ss = new getNURBSCurveDefinitionReturn();
		ss.degree = (System.Int32)s.degree;
		ss.knots = Core.Native.NativeInterface.ConvertValue(ref s.knots);
		ss.poles = Geom.Native.NativeInterface.ConvertValue(ref s.poles);
		ss.weights = Core.Native.NativeInterface.ConvertValue(ref s.weights);
		return ss;
	}

	public static getNURBSCurveDefinitionReturn_c ConvertValue(getNURBSCurveDefinitionReturn s, ref getNURBSCurveDefinitionReturn_c ss) {
		ss.degree = (Int32)s.degree;
		Core.Native.NativeInterface.ConvertValue(s.knots, ref ss.knots);
		Geom.Native.NativeInterface.ConvertValue(s.poles, ref ss.poles);
		Core.Native.NativeInterface.ConvertValue(s.weights, ref ss.weights);
		return ss;
	}

	public static Geom.Native.Point3ListList ConvertValue(ref Geom.Native.Point3ListList_c s) {
		Geom.Native.Point3ListList list = new Geom.Native.Point3ListList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Geom.Native.Point3List_c)));
			Geom.Native.Point3List_c value = (Geom.Native.Point3List_c)Marshal.PtrToStructure(p, typeof(Geom.Native.Point3List_c));
			list.list[i] = Geom.Native.NativeInterface.ConvertValue(ref value);
		}
		return list;
	}

	public static Geom.Native.Point3ListList_c ConvertValue(Geom.Native.Point3ListList s, ref Geom.Native.Point3ListList_c list) {
		Geom.Native.NativeInterface.Geom_Point3ListList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			Geom.Native.Point3List_c elt = new Geom.Native.Point3List_c();
			Geom.Native.NativeInterface.ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Geom.Native.Point3List_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static Core.Native.DoubleListList ConvertValue(ref Core.Native.DoubleListList_c s) {
		Core.Native.DoubleListList list = new Core.Native.DoubleListList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Core.Native.DoubleList_c)));
			Core.Native.DoubleList_c value = (Core.Native.DoubleList_c)Marshal.PtrToStructure(p, typeof(Core.Native.DoubleList_c));
			list.list[i] = Core.Native.NativeInterface.ConvertValue(ref value);
		}
		return list;
	}

	public static Core.Native.DoubleListList_c ConvertValue(Core.Native.DoubleListList s, ref Core.Native.DoubleListList_c list) {
		Core.Native.NativeInterface.Core_DoubleListList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			Core.Native.DoubleList_c elt = new Core.Native.DoubleList_c();
			Core.Native.NativeInterface.ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Core.Native.DoubleList_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static getNURBSSurfaceDefinitionReturn ConvertValue(ref getNURBSSurfaceDefinitionReturn_c s) {
		getNURBSSurfaceDefinitionReturn ss = new getNURBSSurfaceDefinitionReturn();
		ss.degreeU = (System.Int32)s.degreeU;
		ss.degreeV = (System.Int32)s.degreeV;
		ss.knotsU = Core.Native.NativeInterface.ConvertValue(ref s.knotsU);
		ss.knotsV = Core.Native.NativeInterface.ConvertValue(ref s.knotsV);
		ss.poles = Geom.Native.NativeInterface.ConvertValue(ref s.poles);
		ss.weights = Core.Native.NativeInterface.ConvertValue(ref s.weights);
		return ss;
	}

	public static getNURBSSurfaceDefinitionReturn_c ConvertValue(getNURBSSurfaceDefinitionReturn s, ref getNURBSSurfaceDefinitionReturn_c ss) {
		ss.degreeU = (Int32)s.degreeU;
		ss.degreeV = (Int32)s.degreeV;
		Core.Native.NativeInterface.ConvertValue(s.knotsU, ref ss.knotsU);
		Core.Native.NativeInterface.ConvertValue(s.knotsV, ref ss.knotsV);
		Geom.Native.NativeInterface.ConvertValue(s.poles, ref ss.poles);
		Core.Native.NativeInterface.ConvertValue(s.weights, ref ss.weights);
		return ss;
	}

	public static getOffsetCurveDefinitionReturn ConvertValue(ref getOffsetCurveDefinitionReturn_c s) {
		getOffsetCurveDefinitionReturn ss = new getOffsetCurveDefinitionReturn();
		ss.curve = (System.UInt32)s.curve;
		ss.direction = Geom.Native.NativeInterface.ConvertValue(ref s.direction);
		ss.distance = (System.Double)s.distance;
		ss.surfaceReference = (System.UInt32)s.surfaceReference;
		return ss;
	}

	public static getOffsetCurveDefinitionReturn_c ConvertValue(getOffsetCurveDefinitionReturn s, ref getOffsetCurveDefinitionReturn_c ss) {
		ss.curve = (System.UInt32)s.curve;
		Geom.Native.NativeInterface.ConvertValue(s.direction, ref ss.direction);
		ss.distance = (System.Double)s.distance;
		ss.surfaceReference = (System.UInt32)s.surfaceReference;
		return ss;
	}

	public static getOffsetSurfaceDefinitionReturn ConvertValue(ref getOffsetSurfaceDefinitionReturn_c s) {
		getOffsetSurfaceDefinitionReturn ss = new getOffsetSurfaceDefinitionReturn();
		ss.baseSurface = (System.UInt32)s.baseSurface;
		ss.distance = (System.Double)s.distance;
		return ss;
	}

	public static getOffsetSurfaceDefinitionReturn_c ConvertValue(getOffsetSurfaceDefinitionReturn s, ref getOffsetSurfaceDefinitionReturn_c ss) {
		ss.baseSurface = (System.UInt32)s.baseSurface;
		ss.distance = (System.Double)s.distance;
		return ss;
	}

	public static getParabolaCurveDefinitionReturn ConvertValue(ref getParabolaCurveDefinitionReturn_c s) {
		getParabolaCurveDefinitionReturn ss = new getParabolaCurveDefinitionReturn();
		ss.focalLength = (System.Double)s.focalLength;
		ss.matrix = Geom.Native.NativeInterface.ConvertValue(ref s.matrix);
		return ss;
	}

	public static getParabolaCurveDefinitionReturn_c ConvertValue(getParabolaCurveDefinitionReturn s, ref getParabolaCurveDefinitionReturn_c ss) {
		ss.focalLength = (System.Double)s.focalLength;
		Geom.Native.NativeInterface.ConvertValue(s.matrix, ref ss.matrix);
		return ss;
	}

	public static getPolylineCurveDefinitionReturn ConvertValue(ref getPolylineCurveDefinitionReturn_c s) {
		getPolylineCurveDefinitionReturn ss = new getPolylineCurveDefinitionReturn();
		ss.points = Geom.Native.NativeInterface.ConvertValue(ref s.points);
		ss.parameters = Core.Native.NativeInterface.ConvertValue(ref s.parameters);
		return ss;
	}

	public static getPolylineCurveDefinitionReturn_c ConvertValue(getPolylineCurveDefinitionReturn s, ref getPolylineCurveDefinitionReturn_c ss) {
		Geom.Native.NativeInterface.ConvertValue(s.points, ref ss.points);
		Core.Native.NativeInterface.ConvertValue(s.parameters, ref ss.parameters);
		return ss;
	}

	public static getRevolutionSurfaceDefinitionReturn ConvertValue(ref getRevolutionSurfaceDefinitionReturn_c s) {
		getRevolutionSurfaceDefinitionReturn ss = new getRevolutionSurfaceDefinitionReturn();
		ss.generatricCurve = (System.UInt32)s.generatricCurve;
		ss.axisOrigin = Geom.Native.NativeInterface.ConvertValue(ref s.axisOrigin);
		ss.axisDirection = Geom.Native.NativeInterface.ConvertValue(ref s.axisDirection);
		ss.startAngle = (System.Double)s.startAngle;
		ss.endAngle = (System.Double)s.endAngle;
		return ss;
	}

	public static getRevolutionSurfaceDefinitionReturn_c ConvertValue(getRevolutionSurfaceDefinitionReturn s, ref getRevolutionSurfaceDefinitionReturn_c ss) {
		ss.generatricCurve = (System.UInt32)s.generatricCurve;
		Geom.Native.NativeInterface.ConvertValue(s.axisOrigin, ref ss.axisOrigin);
		Geom.Native.NativeInterface.ConvertValue(s.axisDirection, ref ss.axisDirection);
		ss.startAngle = (System.Double)s.startAngle;
		ss.endAngle = (System.Double)s.endAngle;
		return ss;
	}

	public static getRuledSurfaceDefinitionReturn ConvertValue(ref getRuledSurfaceDefinitionReturn_c s) {
		getRuledSurfaceDefinitionReturn ss = new getRuledSurfaceDefinitionReturn();
		ss.firstCurve = (System.UInt32)s.firstCurve;
		ss.secondCurve = (System.UInt32)s.secondCurve;
		return ss;
	}

	public static getRuledSurfaceDefinitionReturn_c ConvertValue(getRuledSurfaceDefinitionReturn s, ref getRuledSurfaceDefinitionReturn_c ss) {
		ss.firstCurve = (System.UInt32)s.firstCurve;
		ss.secondCurve = (System.UInt32)s.secondCurve;
		return ss;
	}

	public static getSegmentCurveDefinitionReturn ConvertValue(ref getSegmentCurveDefinitionReturn_c s) {
		getSegmentCurveDefinitionReturn ss = new getSegmentCurveDefinitionReturn();
		ss.startPoint = Geom.Native.NativeInterface.ConvertValue(ref s.startPoint);
		ss.endPoint = Geom.Native.NativeInterface.ConvertValue(ref s.endPoint);
		return ss;
	}

	public static getSegmentCurveDefinitionReturn_c ConvertValue(getSegmentCurveDefinitionReturn s, ref getSegmentCurveDefinitionReturn_c ss) {
		Geom.Native.NativeInterface.ConvertValue(s.startPoint, ref ss.startPoint);
		Geom.Native.NativeInterface.ConvertValue(s.endPoint, ref ss.endPoint);
		return ss;
	}

	public static getSphereSurfaceDefinitionReturn ConvertValue(ref getSphereSurfaceDefinitionReturn_c s) {
		getSphereSurfaceDefinitionReturn ss = new getSphereSurfaceDefinitionReturn();
		ss.radius = (System.Double)s.radius;
		ss.matrix = Geom.Native.NativeInterface.ConvertValue(ref s.matrix);
		return ss;
	}

	public static getSphereSurfaceDefinitionReturn_c ConvertValue(getSphereSurfaceDefinitionReturn s, ref getSphereSurfaceDefinitionReturn_c ss) {
		ss.radius = (System.Double)s.radius;
		Geom.Native.NativeInterface.ConvertValue(s.matrix, ref ss.matrix);
		return ss;
	}

	public static getSurfacicCurveDefinitionReturn ConvertValue(ref getSurfacicCurveDefinitionReturn_c s) {
		getSurfacicCurveDefinitionReturn ss = new getSurfacicCurveDefinitionReturn();
		ss.surface = (System.UInt32)s.surface;
		ss.curve2D = (System.UInt32)s.curve2D;
		return ss;
	}

	public static getSurfacicCurveDefinitionReturn_c ConvertValue(getSurfacicCurveDefinitionReturn s, ref getSurfacicCurveDefinitionReturn_c ss) {
		ss.surface = (System.UInt32)s.surface;
		ss.curve2D = (System.UInt32)s.curve2D;
		return ss;
	}

	public static getTabulatedCylinderSurfaceDefinitionReturn ConvertValue(ref getTabulatedCylinderSurfaceDefinitionReturn_c s) {
		getTabulatedCylinderSurfaceDefinitionReturn ss = new getTabulatedCylinderSurfaceDefinitionReturn();
		ss.directrixCurve = (System.UInt32)s.directrixCurve;
		ss.generatrixLine = Geom.Native.NativeInterface.ConvertValue(ref s.generatrixLine);
		ss.range = ConvertValue(ref s.range);
		return ss;
	}

	public static getTabulatedCylinderSurfaceDefinitionReturn_c ConvertValue(getTabulatedCylinderSurfaceDefinitionReturn s, ref getTabulatedCylinderSurfaceDefinitionReturn_c ss) {
		ss.directrixCurve = (System.UInt32)s.directrixCurve;
		Geom.Native.NativeInterface.ConvertValue(s.generatrixLine, ref ss.generatrixLine);
		ConvertValue(s.range, ref ss.range);
		return ss;
	}

	public static getTorusSurfaceDefinitionReturn ConvertValue(ref getTorusSurfaceDefinitionReturn_c s) {
		getTorusSurfaceDefinitionReturn ss = new getTorusSurfaceDefinitionReturn();
		ss.majorRadius = (System.Double)s.majorRadius;
		ss.minorRadius = (System.Double)s.minorRadius;
		ss.matrix = Geom.Native.NativeInterface.ConvertValue(ref s.matrix);
		return ss;
	}

	public static getTorusSurfaceDefinitionReturn_c ConvertValue(getTorusSurfaceDefinitionReturn s, ref getTorusSurfaceDefinitionReturn_c ss) {
		ss.majorRadius = (System.Double)s.majorRadius;
		ss.minorRadius = (System.Double)s.minorRadius;
		Geom.Native.NativeInterface.ConvertValue(s.matrix, ref ss.matrix);
		return ss;
	}

	public static getTransformedCurveDefinitionReturn ConvertValue(ref getTransformedCurveDefinitionReturn_c s) {
		getTransformedCurveDefinitionReturn ss = new getTransformedCurveDefinitionReturn();
		ss.curve = (System.UInt32)s.curve;
		ss.matrix = Geom.Native.NativeInterface.ConvertValue(ref s.matrix);
		return ss;
	}

	public static getTransformedCurveDefinitionReturn_c ConvertValue(getTransformedCurveDefinitionReturn s, ref getTransformedCurveDefinitionReturn_c ss) {
		ss.curve = (System.UInt32)s.curve;
		Geom.Native.NativeInterface.ConvertValue(s.matrix, ref ss.matrix);
		return ss;
	}

	public static isCurvePeriodicReturn ConvertValue(ref isCurvePeriodicReturn_c s) {
		isCurvePeriodicReturn ss = new isCurvePeriodicReturn();
		ss.periodic = ConvertValue(s.periodic);
		ss.period = (System.Double)s.period;
		return ss;
	}

	public static isCurvePeriodicReturn_c ConvertValue(isCurvePeriodicReturn s, ref isCurvePeriodicReturn_c ss) {
		ss.periodic = ConvertValue(s.periodic);
		ss.period = (System.Double)s.period;
		return ss;
	}

	public static isSurfaceClosedReturn ConvertValue(ref isSurfaceClosedReturn_c s) {
		isSurfaceClosedReturn ss = new isSurfaceClosedReturn();
		ss.closedU = ConvertValue(s.closedU);
		ss.closedV = ConvertValue(s.closedV);
		return ss;
	}

	public static isSurfaceClosedReturn_c ConvertValue(isSurfaceClosedReturn s, ref isSurfaceClosedReturn_c ss) {
		ss.closedU = ConvertValue(s.closedU);
		ss.closedV = ConvertValue(s.closedV);
		return ss;
	}

	public static isSurfacePeriodicReturn ConvertValue(ref isSurfacePeriodicReturn_c s) {
		isSurfacePeriodicReturn ss = new isSurfacePeriodicReturn();
		ss.periodicU = ConvertValue(s.periodicU);
		ss.periodicV = ConvertValue(s.periodicV);
		ss.periodU = (System.Double)s.periodU;
		ss.periodV = (System.Double)s.periodV;
		return ss;
	}

	public static isSurfacePeriodicReturn_c ConvertValue(isSurfacePeriodicReturn s, ref isSurfacePeriodicReturn_c ss) {
		ss.periodicU = ConvertValue(s.periodicU);
		ss.periodicV = ConvertValue(s.periodicV);
		ss.periodU = (System.Double)s.periodU;
		ss.periodV = (System.Double)s.periodV;
		return ss;
	}

		#endregion

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr CAD_getLastError();

		public static string GetLastError()
		{
			return ConvertValue(CAD_getLastError());
		}

		#region 

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void CAD_setPrecision(System.Double precision);
		/// <summary>
		/// Set the CAD precision
		/// </summary>
		/// <param name="precision">CAD precision</param>
		public static void SetPrecision(System.Double precision) {
			CAD_setPrecision(precision);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void CAD_test();
		/// <summary>
		/// 
		/// </summary>
		public static void Test() {
			CAD_test();
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region Boolean Operators

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern BodyList_c CAD_solidIntersection(System.UInt32 A, System.UInt32 B);
		/// <summary>
		/// perform boolean operation intersection on two bodies (A ^ B)
		/// </summary>
		/// <param name="A">The first body</param>
		/// <param name="B">The second body</param>
		public static BodyList SolidIntersection(System.UInt32 A, System.UInt32 B) {
			var ret = CAD_solidIntersection(A, B);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			CAD.Native.NativeInterface.CAD_BodyList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern BodyList_c CAD_solidSubstraction(System.UInt32 A, System.UInt32 B);
		/// <summary>
		/// perform boolean operation substract on two bodies (A - B)
		/// </summary>
		/// <param name="A">The first body</param>
		/// <param name="B">The second body</param>
		public static BodyList SolidSubstraction(System.UInt32 A, System.UInt32 B) {
			var ret = CAD_solidSubstraction(A, B);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			CAD.Native.NativeInterface.CAD_BodyList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern BodyList_c CAD_solidUnion(System.UInt32 A, System.UInt32 B);
		/// <summary>
		/// perform boolean operation union on two bodies (A + B)
		/// </summary>
		/// <param name="A">The first body</param>
		/// <param name="B">The second body</param>
		public static BodyList SolidUnion(System.UInt32 A, System.UInt32 B) {
			var ret = CAD_solidUnion(A, B);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			CAD.Native.NativeInterface.CAD_BodyList_free(ref ret);
			return convRet;
		}

		#endregion

		#region curves

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_convertCurveIntoNurbs(System.UInt32 curve);
		/// <summary>
		/// Convert a curve into a nurbs
		/// </summary>
		/// <param name="curve">Curve to convert</param>
		public static System.UInt32 ConvertCurveIntoNurbs(System.UInt32 curve) {
			var ret = CAD_convertCurveIntoNurbs(curve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createBezierCurve(Geom.Native.Point3List_c poles);
		/// <summary>
		/// Create a Bezier curve
		/// </summary>
		/// <param name="poles">Poles list</param>
		public static System.UInt32 CreateBezierCurve(Geom.Native.Point3List poles) {
			var poles_c = new Geom.Native.Point3List_c();
			Geom.Native.NativeInterface.ConvertValue(poles, ref poles_c);
			var ret = CAD_createBezierCurve(poles_c);
			Geom.Native.NativeInterface.Geom_Point3List_free(ref poles_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createBoundedCurve(System.UInt32 curve, System.Double minBound, System.Double maxBound);
		/// <summary>
		/// Create a bounded curve from a curve
		/// </summary>
		/// <param name="curve">Curve to bound</param>
		/// <param name="minBound">Minimum bound parameter</param>
		/// <param name="maxBound">Maximum bound parameter</param>
		public static System.UInt32 CreateBoundedCurve(System.UInt32 curve, System.Double minBound, System.Double maxBound) {
			var ret = CAD_createBoundedCurve(curve, minBound, maxBound);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createCircleCurve(System.Double radius, Geom.Native.Matrix4_c matrix);
		/// <summary>
		/// Create a new circle
		/// </summary>
		/// <param name="radius">Circle radius</param>
		/// <param name="matrix">Transformation matrix</param>
		public static System.UInt32 CreateCircleCurve(System.Double radius, Geom.Native.Matrix4 matrix) {
			var matrix_c = new Geom.Native.Matrix4_c();
			Geom.Native.NativeInterface.ConvertValue(matrix, ref matrix_c);
			var ret = CAD_createCircleCurve(radius, matrix_c);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref matrix_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createCompositeCurve(LimitedCurveList_c limitedCurveList);
		/// <summary>
		/// Create a composite curve from a list of limited curves
		/// </summary>
		/// <param name="limitedCurveList">List of limited curves</param>
		public static System.UInt32 CreateCompositeCurve(LimitedCurveList limitedCurveList) {
			var limitedCurveList_c = new CAD.Native.LimitedCurveList_c();
			ConvertValue(limitedCurveList, ref limitedCurveList_c);
			var ret = CAD_createCompositeCurve(limitedCurveList_c);
			CAD.Native.NativeInterface.CAD_LimitedCurveList_free(ref limitedCurveList_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createEllipseCurve(System.Double URadius, System.Double VRadius, Geom.Native.Matrix4_c matrix);
		/// <summary>
		/// Create an ellipse curve
		/// </summary>
		/// <param name="URadius">Ellipse radius in u direction</param>
		/// <param name="VRadius">Ellipse radius in v direction</param>
		/// <param name="matrix">Transformation matrix</param>
		public static System.UInt32 CreateEllipseCurve(System.Double URadius, System.Double VRadius, Geom.Native.Matrix4 matrix) {
			var matrix_c = new Geom.Native.Matrix4_c();
			Geom.Native.NativeInterface.ConvertValue(matrix, ref matrix_c);
			var ret = CAD_createEllipseCurve(URadius, VRadius, matrix_c);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref matrix_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createHelixCurve(System.Double radius, System.Double pitch, Geom.Native.Matrix4_c matrix, Int32 trigonometrixOrientation);
		/// <summary>
		/// Create an helix curve
		/// </summary>
		/// <param name="radius">Radius of the helix</param>
		/// <param name="pitch">Height of one revolution</param>
		/// <param name="matrix">Transformation matrix</param>
		/// <param name="trigonometrixOrientation">Orientation of the rotation</param>
		public static System.UInt32 CreateHelixCurve(System.Double radius, System.Double pitch, Geom.Native.Matrix4 matrix, System.Boolean trigonometrixOrientation) {
			var matrix_c = new Geom.Native.Matrix4_c();
			Geom.Native.NativeInterface.ConvertValue(matrix, ref matrix_c);
			var ret = CAD_createHelixCurve(radius, pitch, matrix_c, trigonometrixOrientation ? 1 : 0);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref matrix_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createHermiteCurve(Geom.Native.Point3_c FirstPoint, Geom.Native.Point3_c FirstTangent, Geom.Native.Point3_c SecondPoint, Geom.Native.Point3_c SecondTangent);
		/// <summary>
		/// Create a Hermite Curve
		/// </summary>
		/// <param name="FirstPoint">Starting point of the curve</param>
		/// <param name="FirstTangent">Tangent of the starting point</param>
		/// <param name="SecondPoint">Ending point of the curve</param>
		/// <param name="SecondTangent">Tangent of the ending point</param>
		public static System.UInt32 CreateHermiteCurve(Geom.Native.Point3 FirstPoint, Geom.Native.Point3 FirstTangent, Geom.Native.Point3 SecondPoint, Geom.Native.Point3 SecondTangent) {
			var FirstPoint_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(FirstPoint, ref FirstPoint_c);
			var FirstTangent_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(FirstTangent, ref FirstTangent_c);
			var SecondPoint_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(SecondPoint, ref SecondPoint_c);
			var SecondTangent_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(SecondTangent, ref SecondTangent_c);
			var ret = CAD_createHermiteCurve(FirstPoint_c, FirstTangent_c, SecondPoint_c, SecondTangent_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref FirstPoint_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref FirstTangent_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref SecondPoint_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref SecondTangent_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createHyperbolaCurve(System.Double URadius, System.Double VRadius, Geom.Native.Matrix4_c matrix);
		/// <summary>
		/// Create an hyperBola curve
		/// </summary>
		/// <param name="URadius">Hyperbola radius in u direction</param>
		/// <param name="VRadius">Hyperbola radius in v direction</param>
		/// <param name="matrix">Transformation matrix</param>
		public static System.UInt32 CreateHyperbolaCurve(System.Double URadius, System.Double VRadius, Geom.Native.Matrix4 matrix) {
			var matrix_c = new Geom.Native.Matrix4_c();
			Geom.Native.NativeInterface.ConvertValue(matrix, ref matrix_c);
			var ret = CAD_createHyperbolaCurve(URadius, VRadius, matrix_c);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref matrix_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createIntersectionCurve(System.UInt32 firstSurface, System.UInt32 secondSurface, System.UInt32 chart, System.Double minBounds, System.Double maxBounds);
		/// <summary>
		/// Create a Intersection Curve
		/// </summary>
		/// <param name="firstSurface">First surface of the intersection curve</param>
		/// <param name="secondSurface">Second surface of the intersection curve</param>
		/// <param name="chart">Direction curve of the intersection curve</param>
		/// <param name="minBounds">Minimum value of the bounds of the intersection curve</param>
		/// <param name="maxBounds">Maximum value of the bounds of the intersection curve</param>
		public static System.UInt32 CreateIntersectionCurve(System.UInt32 firstSurface, System.UInt32 secondSurface, System.UInt32 chart, System.Double minBounds, System.Double maxBounds) {
			var ret = CAD_createIntersectionCurve(firstSurface, secondSurface, chart, minBounds, maxBounds);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createLineCurve(Geom.Native.Point3_c OriginPt, Geom.Native.Point3_c DirectionPt);
		/// <summary>
		/// Create a Line Curve
		/// </summary>
		/// <param name="OriginPt">Orinin point of the line curve</param>
		/// <param name="DirectionPt">Direction vector of the line curve</param>
		public static System.UInt32 CreateLineCurve(Geom.Native.Point3 OriginPt, Geom.Native.Point3 DirectionPt) {
			var OriginPt_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(OriginPt, ref OriginPt_c);
			var DirectionPt_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(DirectionPt, ref DirectionPt_c);
			var ret = CAD_createLineCurve(OriginPt_c, DirectionPt_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref OriginPt_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref DirectionPt_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createNURBSCurve(Int32 degree, Core.Native.DoubleList_c knots, Geom.Native.Point3List_c poles, Core.Native.DoubleList_c weights);
		/// <summary>
		/// Create a NURBS curve
		/// </summary>
		/// <param name="degree">Degree of the curve</param>
		/// <param name="knots">Knots of the curve</param>
		/// <param name="poles">Poles list</param>
		/// <param name="weights">Weight list</param>
		public static System.UInt32 CreateNURBSCurve(System.Int32 degree, Core.Native.DoubleList knots, Geom.Native.Point3List poles, Core.Native.DoubleList weights) {
			var knots_c = new Core.Native.DoubleList_c();
			Core.Native.NativeInterface.ConvertValue(knots, ref knots_c);
			var poles_c = new Geom.Native.Point3List_c();
			Geom.Native.NativeInterface.ConvertValue(poles, ref poles_c);
			var weights_c = new Core.Native.DoubleList_c();
			Core.Native.NativeInterface.ConvertValue(weights, ref weights_c);
			var ret = CAD_createNURBSCurve(degree, knots_c, poles_c, weights_c);
			Core.Native.NativeInterface.Core_DoubleList_free(ref knots_c);
			Geom.Native.NativeInterface.Geom_Point3List_free(ref poles_c);
			Core.Native.NativeInterface.Core_DoubleList_free(ref weights_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createParabolaCurve(System.Double focalLength, Geom.Native.Matrix4_c matrix);
		/// <summary>
		/// Create an parabola curve
		/// </summary>
		/// <param name="focalLength">Focal lecngth of the parabola</param>
		/// <param name="matrix">Transformation matrix</param>
		public static System.UInt32 CreateParabolaCurve(System.Double focalLength, Geom.Native.Matrix4 matrix) {
			var matrix_c = new Geom.Native.Matrix4_c();
			Geom.Native.NativeInterface.ConvertValue(matrix, ref matrix_c);
			var ret = CAD_createParabolaCurve(focalLength, matrix_c);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref matrix_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createPolylineCurve(Geom.Native.Point3List_c points);
		/// <summary>
		/// Create a Polyline curve
		/// </summary>
		/// <param name="points">Points of polyline curve</param>
		public static System.UInt32 CreatePolylineCurve(Geom.Native.Point3List points) {
			var points_c = new Geom.Native.Point3List_c();
			Geom.Native.NativeInterface.ConvertValue(points, ref points_c);
			var ret = CAD_createPolylineCurve(points_c);
			Geom.Native.NativeInterface.Geom_Point3List_free(ref points_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createSegmentCurve(Geom.Native.Point3_c firstPoint, Geom.Native.Point3_c secondPoint);
		/// <summary>
		/// Create a segment curve from two given points
		/// </summary>
		/// <param name="firstPoint">First point</param>
		/// <param name="secondPoint">Second point</param>
		public static System.UInt32 CreateSegmentCurve(Geom.Native.Point3 firstPoint, Geom.Native.Point3 secondPoint) {
			var firstPoint_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(firstPoint, ref firstPoint_c);
			var secondPoint_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(secondPoint, ref secondPoint_c);
			var ret = CAD_createSegmentCurve(firstPoint_c, secondPoint_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref firstPoint_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref secondPoint_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createSurfacicCurve(System.UInt32 surface, System.UInt32 curve2D);
		/// <summary>
		/// Create a curve from a surface
		/// </summary>
		/// <param name="surface">Surface to bound</param>
		/// <param name="curve2D">Curve to project</param>
		public static System.UInt32 CreateSurfacicCurve(System.UInt32 surface, System.UInt32 curve2D) {
			var ret = CAD_createSurfacicCurve(surface, curve2D);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createTransformedCurve(System.UInt32 curve, Geom.Native.Matrix4_c matrix);
		/// <summary>
		/// Create a curve from a surface
		/// </summary>
		/// <param name="curve">Curve to transform</param>
		/// <param name="matrix">Matrix of the transformation</param>
		public static System.UInt32 CreateTransformedCurve(System.UInt32 curve, Geom.Native.Matrix4 matrix) {
			var matrix_c = new Geom.Native.Matrix4_c();
			Geom.Native.NativeInterface.ConvertValue(matrix, ref matrix_c);
			var ret = CAD_createTransformedCurve(curve, matrix_c);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref matrix_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_invertCurve(System.UInt32 curve, System.Double precision);
		/// <summary>
		/// Invert a curve parametricaly
		/// </summary>
		/// <param name="curve">The curve to invert</param>
		/// <param name="precision">The precision used to invert the curve</param>
		public static System.UInt32 InvertCurve(System.UInt32 curve, System.Double precision) {
			var ret = CAD_invertCurve(curve, precision);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		#endregion

		#region material

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_getFaceMaterial(System.UInt32 face);
		/// <summary>
		/// get the material on a face
		/// </summary>
		/// <param name="face">The face</param>
		public static System.UInt32 GetFaceMaterial(System.UInt32 face) {
			var ret = CAD_getFaceMaterial(face);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void CAD_setFaceMaterial(System.UInt32 face, System.UInt32 material);
		/// <summary>
		/// set the material on a face
		/// </summary>
		/// <param name="face">The face</param>
		/// <param name="material">The material</param>
		public static void SetFaceMaterial(System.UInt32 face, System.UInt32 material) {
			CAD_setFaceMaterial(face, material);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region model management

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void CAD_addToModel(System.UInt32 shape, System.UInt32 model);
		/// <summary>
		/// Add shape to the model
		/// </summary>
		/// <param name="shape">Shape added to the model</param>
		/// <param name="model">Model</param>
		public static void AddToModel(System.UInt32 shape, System.UInt32 model) {
			CAD_addToModel(shape, model);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern buildFacesReturn_c CAD_buildFaces(System.UInt32 surface, LoopList_c loopList);
		/// <summary>
		/// Build faces from a surface and a set of loop
		/// </summary>
		/// <param name="surface">Surface used to build the faces</param>
		/// <param name="loopList">List of Loops used to build the faces</param>
		public static CAD.Native.buildFacesReturn BuildFaces(System.UInt32 surface, LoopList loopList) {
			var loopList_c = new CAD.Native.LoopList_c();
			ConvertValue(loopList, ref loopList_c);
			var ret = CAD_buildFaces(surface, loopList_c);
			CAD.Native.NativeInterface.CAD_LoopList_free(ref loopList_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.buildFacesReturn retStruct = new CAD.Native.buildFacesReturn();
			retStruct.domain = (System.UInt32)ret.domain;
			retStruct.splittingInfo = ConvertValue(ref ret.splittingInfo);
			CAD.Native.NativeInterface.CAD_SplittedEdgeList_free(ref ret.splittingInfo);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createModel();
		/// <summary>
		/// Create a new model
		/// </summary>
		public static System.UInt32 CreateModel() {
			var ret = CAD_createModel();
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern FaceList_c CAD_getAllModelFaces(System.UInt32 model);
		/// <summary>
		/// Get all the face of a model recursively
		/// </summary>
		/// <param name="model">Model</param>
		public static FaceList GetAllModelFaces(System.UInt32 model) {
			var ret = CAD_getAllModelFaces(model);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			CAD.Native.NativeInterface.CAD_FaceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.Double CAD_getFace2DArea(System.UInt32 face);
		/// <summary>
		/// Returns the 2D domain area of a face
		/// </summary>
		/// <param name="face">The face</param>
		public static System.Double GetFace2DArea(System.UInt32 face) {
			var ret = CAD_getFace2DArea(face);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Double)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern BodyList_c CAD_getModelBodies(System.UInt32 model);
		/// <summary>
		/// Get the list of bodies contained in a model
		/// </summary>
		/// <param name="model">Model</param>
		public static BodyList GetModelBodies(System.UInt32 model) {
			var ret = CAD_getModelBodies(model);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			CAD.Native.NativeInterface.CAD_BodyList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern EdgeListList_c CAD_getModelBoundaries(System.UInt32 model);
		/// <summary>
		/// Get boundary edges of a model grouped by cycles
		/// </summary>
		/// <param name="model">Model</param>
		public static EdgeListList GetModelBoundaries(System.UInt32 model) {
			var ret = CAD_getModelBoundaries(model);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			CAD.Native.NativeInterface.CAD_EdgeListList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern DomainList_c CAD_getModelDomains(System.UInt32 model);
		/// <summary>
		/// Get the list of domains (Face or OpenShell) contained in a model
		/// </summary>
		/// <param name="model">Model</param>
		public static DomainList GetModelDomains(System.UInt32 model) {
			var ret = CAD_getModelDomains(model);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			CAD.Native.NativeInterface.CAD_DomainList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern EdgeList_c CAD_getModelEdges(System.UInt32 model);
		/// <summary>
		/// Get the list of free edges contained in a model
		/// </summary>
		/// <param name="model">Model</param>
		public static EdgeList GetModelEdges(System.UInt32 model) {
			var ret = CAD_getModelEdges(model);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			CAD.Native.NativeInterface.CAD_EdgeList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern VertexList_c CAD_getModelVertices(System.UInt32 model);
		/// <summary>
		/// Get the list of free vertices contained in a model
		/// </summary>
		/// <param name="model">Model</param>
		public static VertexList GetModelVertices(System.UInt32 model) {
			var ret = CAD_getModelVertices(model);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			CAD.Native.NativeInterface.CAD_VertexList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Core.Native.EntityList_c CAD_getReferencers(System.UInt32 entity);
		/// <summary>
		/// Returns the entities referencing a given CAD entity
		/// </summary>
		/// <param name="entity">CAD entity to get the referencers</param>
		public static Core.Native.EntityList GetReferencers(System.UInt32 entity) {
			var ret = CAD_getReferencers(entity);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Core.Native.NativeInterface.ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_EntityList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Core.Native.EntityList_c CAD_getUnreferencedEntities();
		/// <summary>
		/// Returns all the unreferenced CAD entities
		/// </summary>
		public static Core.Native.EntityList GetUnreferencedEntities() {
			var ret = CAD_getUnreferencedEntities();
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Core.Native.NativeInterface.ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_EntityList_free(ref ret);
			return convRet;
		}

		#endregion

		#region structure access

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.Double CAD_evalCurvatureOnCurve(System.UInt32 curve, System.Double parameter);
		/// <summary>
		/// evaluate curvature on a curve
		/// </summary>
		/// <param name="curve">The curve</param>
		/// <param name="parameter">Parameter to evaluate</param>
		public static System.Double EvalCurvatureOnCurve(System.UInt32 curve, System.Double parameter) {
			var ret = CAD_evalCurvatureOnCurve(curve, parameter);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Double)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Geom.Native.Curvatures_c CAD_evalCurvatureOnSurface(System.UInt32 surface, Geom.Native.Point2_c parameter);
		/// <summary>
		/// evaluate main curvatures on a surface
		/// </summary>
		/// <param name="surface">The surface</param>
		/// <param name="parameter">Parameter to evaluate</param>
		public static Geom.Native.Curvatures EvalCurvatureOnSurface(System.UInt32 surface, Geom.Native.Point2 parameter) {
			var parameter_c = new Geom.Native.Point2_c();
			Geom.Native.NativeInterface.ConvertValue(parameter, ref parameter_c);
			var ret = CAD_evalCurvatureOnSurface(surface, parameter_c);
			Geom.Native.NativeInterface.Geom_Point2_free(ref parameter_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Geom.Native.NativeInterface.ConvertValue(ref ret);
			Geom.Native.NativeInterface.Geom_Curvatures_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern evalOnCurveReturn_c CAD_evalOnCurve(System.UInt32 curve, System.Double parameter, Int32 derivation);
		/// <summary>
		/// evaluate a point and derivatives on a curve
		/// </summary>
		/// <param name="curve">The curve</param>
		/// <param name="parameter">Parameter to evaluate</param>
		/// <param name="derivation">Derivation level (0,1,2)</param>
		public static CAD.Native.evalOnCurveReturn EvalOnCurve(System.UInt32 curve, System.Double parameter, System.Int32 derivation) {
			var ret = CAD_evalOnCurve(curve, parameter, derivation);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.evalOnCurveReturn retStruct = new CAD.Native.evalOnCurveReturn();
			retStruct.d0 = ConvertValue(ref ret.d0);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.d0);
			retStruct.du = ConvertValue(ref ret.du);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.du);
			retStruct.d2u = ConvertValue(ref ret.d2u);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.d2u);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern evalOnSurfaceReturn_c CAD_evalOnSurface(System.UInt32 surface, Geom.Native.Point2_c parameter, Int32 derivation);
		/// <summary>
		/// evaluate a point and derivatives on a surface
		/// </summary>
		/// <param name="surface">The surface</param>
		/// <param name="parameter">Parameter to evaluate</param>
		/// <param name="derivation">Derivation level (0,1,2)</param>
		public static CAD.Native.evalOnSurfaceReturn EvalOnSurface(System.UInt32 surface, Geom.Native.Point2 parameter, System.Int32 derivation) {
			var parameter_c = new Geom.Native.Point2_c();
			Geom.Native.NativeInterface.ConvertValue(parameter, ref parameter_c);
			var ret = CAD_evalOnSurface(surface, parameter_c, derivation);
			Geom.Native.NativeInterface.Geom_Point2_free(ref parameter_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.evalOnSurfaceReturn retStruct = new CAD.Native.evalOnSurfaceReturn();
			retStruct.d0 = ConvertValue(ref ret.d0);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.d0);
			retStruct.du = ConvertValue(ref ret.du);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.du);
			retStruct.dv = ConvertValue(ref ret.dv);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.dv);
			retStruct.d2u = ConvertValue(ref ret.d2u);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.d2u);
			retStruct.d2v = ConvertValue(ref ret.d2v);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.d2v);
			retStruct.duv = ConvertValue(ref ret.duv);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.duv);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern ClosedShellList_c CAD_getBodyClosedShells(System.UInt32 body);
		/// <summary>
		/// get all closedShells contain in the body
		/// </summary>
		/// <param name="body">The body</param>
		public static ClosedShellList GetBodyClosedShells(System.UInt32 body) {
			var ret = CAD_getBodyClosedShells(body);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			CAD.Native.NativeInterface.CAD_ClosedShellList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getBoundedCurveDefinitionReturn_c CAD_getBoundedCurveDefinition(System.UInt32 boundedCurve);
		/// <summary>
		/// get all parameters contained in the boundedCurve
		/// </summary>
		/// <param name="boundedCurve">The boundedCurve</param>
		public static CAD.Native.getBoundedCurveDefinitionReturn GetBoundedCurveDefinition(System.UInt32 boundedCurve) {
			var ret = CAD_getBoundedCurveDefinition(boundedCurve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getBoundedCurveDefinitionReturn retStruct = new CAD.Native.getBoundedCurveDefinitionReturn();
			retStruct.curve = (System.UInt32)ret.curve;
			retStruct.bounds = ConvertValue(ref ret.bounds);
			CAD.Native.NativeInterface.CAD_Bounds1D_free(ref ret.bounds);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getCircleCurveDefinitionReturn_c CAD_getCircleCurveDefinition(System.UInt32 circleCurve);
		/// <summary>
		/// get all parameters contained in the circleCurve
		/// </summary>
		/// <param name="circleCurve">The circleCurve</param>
		public static CAD.Native.getCircleCurveDefinitionReturn GetCircleCurveDefinition(System.UInt32 circleCurve) {
			var ret = CAD_getCircleCurveDefinition(circleCurve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getCircleCurveDefinitionReturn retStruct = new CAD.Native.getCircleCurveDefinitionReturn();
			retStruct.radius = (System.Double)ret.radius;
			retStruct.matrix = ConvertValue(ref ret.matrix);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref ret.matrix);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern OrientedDomainList_c CAD_getClosedShellOrientedDomains(System.UInt32 closedShell);
		/// <summary>
		/// get all orienteDomains contain in the closedShell
		/// </summary>
		/// <param name="closedShell">The closedShell</param>
		public static OrientedDomainList GetClosedShellOrientedDomains(System.UInt32 closedShell) {
			var ret = CAD_getClosedShellOrientedDomains(closedShell);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			CAD.Native.NativeInterface.CAD_OrientedDomainList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getCoEdgeDefinitionReturn_c CAD_getCoEdgeDefinition(System.UInt32 coEdge);
		/// <summary>
		/// get all parameters contained in the coEdge
		/// </summary>
		/// <param name="coEdge">The coEdge</param>
		public static CAD.Native.getCoEdgeDefinitionReturn GetCoEdgeDefinition(System.UInt32 coEdge) {
			var ret = CAD_getCoEdgeDefinition(coEdge);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getCoEdgeDefinitionReturn retStruct = new CAD.Native.getCoEdgeDefinitionReturn();
			retStruct.edge = (System.UInt32)ret.edge;
			retStruct.edgeOrientation = (0 != ret.edgeOrientation);
			retStruct.loop = (System.UInt32)ret.loop;
			retStruct.surface = (System.UInt32)ret.surface;
			retStruct.parametricCurve = (System.UInt32)ret.parametricCurve;
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getCompositeCurveDefinitionReturn_c CAD_getCompositeCurveDefinition(System.UInt32 compositeCurve);
		/// <summary>
		/// get all parameters contained in the compositeCurve
		/// </summary>
		/// <param name="compositeCurve">The compositeCurve</param>
		public static CAD.Native.getCompositeCurveDefinitionReturn GetCompositeCurveDefinition(System.UInt32 compositeCurve) {
			var ret = CAD_getCompositeCurveDefinition(compositeCurve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getCompositeCurveDefinitionReturn retStruct = new CAD.Native.getCompositeCurveDefinitionReturn();
			retStruct.curves = ConvertValue(ref ret.curves);
			CAD.Native.NativeInterface.CAD_LimitedCurveList_free(ref ret.curves);
			retStruct.parameters = ConvertValue(ref ret.parameters);
			Core.Native.NativeInterface.Core_DoubleList_free(ref ret.parameters);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getConeSurfaceDefinitionReturn_c CAD_getConeSurfaceDefinition(System.UInt32 coneSurface);
		/// <summary>
		/// get all parameters contained in the coneSurface
		/// </summary>
		/// <param name="coneSurface">The coneSurface</param>
		public static CAD.Native.getConeSurfaceDefinitionReturn GetConeSurfaceDefinition(System.UInt32 coneSurface) {
			var ret = CAD_getConeSurfaceDefinition(coneSurface);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getConeSurfaceDefinitionReturn retStruct = new CAD.Native.getConeSurfaceDefinitionReturn();
			retStruct.radius = (System.Double)ret.radius;
			retStruct.semiAngle = (System.Double)ret.semiAngle;
			retStruct.matrix = ConvertValue(ref ret.matrix);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref ret.matrix);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getCurveExtrusionSurfaceDefinitionReturn_c CAD_getCurveExtrusionSurfaceDefinition(System.UInt32 curveExtrusionSurface);
		/// <summary>
		/// get all parameters contained in the curveExtrusionSurface
		/// </summary>
		/// <param name="curveExtrusionSurface">The curveExtrusionSurface</param>
		public static CAD.Native.getCurveExtrusionSurfaceDefinitionReturn GetCurveExtrusionSurfaceDefinition(System.UInt32 curveExtrusionSurface) {
			var ret = CAD_getCurveExtrusionSurfaceDefinition(curveExtrusionSurface);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getCurveExtrusionSurfaceDefinitionReturn retStruct = new CAD.Native.getCurveExtrusionSurfaceDefinitionReturn();
			retStruct.generatrixCurve = (System.UInt32)ret.generatrixCurve;
			retStruct.directrixCruve = (System.UInt32)ret.directrixCruve;
			retStruct.surfaceReference = (System.UInt32)ret.surfaceReference;
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Bounds1D_c CAD_getCurveLimits(System.UInt32 curve);
		/// <summary>
		/// get the parametric space limits of a curve
		/// </summary>
		/// <param name="curve">The curve</param>
		public static Bounds1D GetCurveLimits(System.UInt32 curve) {
			var ret = CAD_getCurveLimits(curve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			CAD.Native.NativeInterface.CAD_Bounds1D_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getCylinderSurfaceDefinitionReturn_c CAD_getCylinderSurfaceDefinition(System.UInt32 cylinderSurface);
		/// <summary>
		/// get all parameters contained in the cylinderSurface
		/// </summary>
		/// <param name="cylinderSurface">The cylinderSurface</param>
		public static CAD.Native.getCylinderSurfaceDefinitionReturn GetCylinderSurfaceDefinition(System.UInt32 cylinderSurface) {
			var ret = CAD_getCylinderSurfaceDefinition(cylinderSurface);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getCylinderSurfaceDefinitionReturn retStruct = new CAD.Native.getCylinderSurfaceDefinitionReturn();
			retStruct.radius = (System.Double)ret.radius;
			retStruct.matrix = ConvertValue(ref ret.matrix);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref ret.matrix);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getEdgeDefinitionReturn_c CAD_getEdgeDefinition(System.UInt32 edge);
		/// <summary>
		/// get all parameters contained in the edge
		/// </summary>
		/// <param name="edge">The edge</param>
		public static CAD.Native.getEdgeDefinitionReturn GetEdgeDefinition(System.UInt32 edge) {
			var ret = CAD_getEdgeDefinition(edge);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getEdgeDefinitionReturn retStruct = new CAD.Native.getEdgeDefinitionReturn();
			retStruct.vertex1 = (System.UInt32)ret.vertex1;
			retStruct.vertex2 = (System.UInt32)ret.vertex2;
			retStruct.curve = (System.UInt32)ret.curve;
			retStruct.bounds = ConvertValue(ref ret.bounds);
			CAD.Native.NativeInterface.CAD_Bounds1D_free(ref ret.bounds);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getEllipseCurveDefinitionReturn_c CAD_getEllipseCurveDefinition(System.UInt32 ellipseCurve);
		/// <summary>
		/// get all parameters contained in the ellipseCurve
		/// </summary>
		/// <param name="ellipseCurve">The ellipseCurve</param>
		public static CAD.Native.getEllipseCurveDefinitionReturn GetEllipseCurveDefinition(System.UInt32 ellipseCurve) {
			var ret = CAD_getEllipseCurveDefinition(ellipseCurve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getEllipseCurveDefinitionReturn retStruct = new CAD.Native.getEllipseCurveDefinitionReturn();
			retStruct.radius1 = (System.Double)ret.radius1;
			retStruct.radius2 = (System.Double)ret.radius2;
			retStruct.matrix = ConvertValue(ref ret.matrix);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref ret.matrix);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getEllipticConeSurfaceDefinitionReturn_c CAD_getEllipticConeSurfaceDefinition(System.UInt32 ellipticConeSurface);
		/// <summary>
		/// get all parameters contained in the ellipticConeSurface
		/// </summary>
		/// <param name="ellipticConeSurface">The EllipticConeSurface</param>
		public static CAD.Native.getEllipticConeSurfaceDefinitionReturn GetEllipticConeSurfaceDefinition(System.UInt32 ellipticConeSurface) {
			var ret = CAD_getEllipticConeSurfaceDefinition(ellipticConeSurface);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getEllipticConeSurfaceDefinitionReturn retStruct = new CAD.Native.getEllipticConeSurfaceDefinitionReturn();
			retStruct.radius1 = (System.Double)ret.radius1;
			retStruct.radius2 = (System.Double)ret.radius2;
			retStruct.semiAngle = (System.Double)ret.semiAngle;
			retStruct.matrix = ConvertValue(ref ret.matrix);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref ret.matrix);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getFaceDefinitionReturn_c CAD_getFaceDefinition(System.UInt32 face);
		/// <summary>
		/// get all parameters contain in the face
		/// </summary>
		/// <param name="face">The face</param>
		public static CAD.Native.getFaceDefinitionReturn GetFaceDefinition(System.UInt32 face) {
			var ret = CAD_getFaceDefinition(face);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getFaceDefinitionReturn retStruct = new CAD.Native.getFaceDefinitionReturn();
			retStruct.surface = (System.UInt32)ret.surface;
			retStruct.loops = ConvertValue(ref ret.loops);
			CAD.Native.NativeInterface.CAD_LoopList_free(ref ret.loops);
			retStruct.orientation = (0 != ret.orientation);
			retStruct.limits = ConvertValue(ref ret.limits);
			CAD.Native.NativeInterface.CAD_Bounds2D_free(ref ret.limits);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Geom.Native.Point2ListList_c CAD_getFaceParametricBoundaries(System.UInt32 face);
		/// <summary>
		/// get parametric definition of each face loop
		/// </summary>
		/// <param name="face">The face</param>
		public static Geom.Native.Point2ListList GetFaceParametricBoundaries(System.UInt32 face) {
			var ret = CAD_getFaceParametricBoundaries(face);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Geom.Native.NativeInterface.ConvertValue(ref ret);
			Geom.Native.NativeInterface.Geom_Point2ListList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getHelixCurveDefinitionReturn_c CAD_getHelixCurveDefinition(System.UInt32 helixCurve);
		/// <summary>
		/// get all parameters contained in the helixCurve
		/// </summary>
		/// <param name="helixCurve">The helixCurve</param>
		public static CAD.Native.getHelixCurveDefinitionReturn GetHelixCurveDefinition(System.UInt32 helixCurve) {
			var ret = CAD_getHelixCurveDefinition(helixCurve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getHelixCurveDefinitionReturn retStruct = new CAD.Native.getHelixCurveDefinitionReturn();
			retStruct.radius = (System.Double)ret.radius;
			retStruct.matrix = ConvertValue(ref ret.matrix);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref ret.matrix);
			retStruct.trigonometricOrientation = (0 != ret.trigonometricOrientation);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getHermiteCurveDefinitionReturn_c CAD_getHermiteCurveDefinition(System.UInt32 hermiteCurve);
		/// <summary>
		/// get all parameters contained in the hermiteCurve
		/// </summary>
		/// <param name="hermiteCurve">The HermiteCurve</param>
		public static CAD.Native.getHermiteCurveDefinitionReturn GetHermiteCurveDefinition(System.UInt32 hermiteCurve) {
			var ret = CAD_getHermiteCurveDefinition(hermiteCurve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getHermiteCurveDefinitionReturn retStruct = new CAD.Native.getHermiteCurveDefinitionReturn();
			retStruct.firstPoint = ConvertValue(ref ret.firstPoint);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.firstPoint);
			retStruct.secondPoint = ConvertValue(ref ret.secondPoint);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.secondPoint);
			retStruct.firstTangent = ConvertValue(ref ret.firstTangent);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.firstTangent);
			retStruct.secondTangent = ConvertValue(ref ret.secondTangent);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.secondTangent);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getHyperbolaCurveDefinitionReturn_c CAD_getHyperbolaCurveDefinition(System.UInt32 hyperbolaCurve);
		/// <summary>
		/// get all parameters contained in the hyperbolaCurve
		/// </summary>
		/// <param name="hyperbolaCurve">The hyperbolaCurve</param>
		public static CAD.Native.getHyperbolaCurveDefinitionReturn GetHyperbolaCurveDefinition(System.UInt32 hyperbolaCurve) {
			var ret = CAD_getHyperbolaCurveDefinition(hyperbolaCurve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getHyperbolaCurveDefinitionReturn retStruct = new CAD.Native.getHyperbolaCurveDefinitionReturn();
			retStruct.radius1 = (System.Double)ret.radius1;
			retStruct.radius2 = (System.Double)ret.radius2;
			retStruct.matrix = ConvertValue(ref ret.matrix);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref ret.matrix);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getIntersectionCurveDefinitionReturn_c CAD_getIntersectionCurveDefinition(System.UInt32 intersectionCurve);
		/// <summary>
		/// get all parameters contained in the intersectionCurve
		/// </summary>
		/// <param name="intersectionCurve">The intersectionCurve</param>
		public static CAD.Native.getIntersectionCurveDefinitionReturn GetIntersectionCurveDefinition(System.UInt32 intersectionCurve) {
			var ret = CAD_getIntersectionCurveDefinition(intersectionCurve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getIntersectionCurveDefinitionReturn retStruct = new CAD.Native.getIntersectionCurveDefinitionReturn();
			retStruct.surface1 = (System.UInt32)ret.surface1;
			retStruct.surface2 = (System.UInt32)ret.surface2;
			retStruct.chart = (System.UInt32)ret.chart;
			retStruct.bounds = ConvertValue(ref ret.bounds);
			CAD.Native.NativeInterface.CAD_Bounds1D_free(ref ret.bounds);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getLineCurveDefinitionReturn_c CAD_getLineCurveDefinition(System.UInt32 lineCurve);
		/// <summary>
		/// get all parameters contain in the lineCurve
		/// </summary>
		/// <param name="lineCurve">The lineCurve</param>
		public static CAD.Native.getLineCurveDefinitionReturn GetLineCurveDefinition(System.UInt32 lineCurve) {
			var ret = CAD_getLineCurveDefinition(lineCurve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getLineCurveDefinitionReturn retStruct = new CAD.Native.getLineCurveDefinitionReturn();
			retStruct.origin = ConvertValue(ref ret.origin);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.origin);
			retStruct.direction = ConvertValue(ref ret.direction);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.direction);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern CoEdgeList_c CAD_getLoopCoEdges(System.UInt32 loop);
		/// <summary>
		/// get all coEdges contain in the loop
		/// </summary>
		/// <param name="loop">The loop</param>
		public static CoEdgeList GetLoopCoEdges(System.UInt32 loop) {
			var ret = CAD_getLoopCoEdges(loop);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			CAD.Native.NativeInterface.CAD_CoEdgeList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getNURBSCurveDefinitionReturn_c CAD_getNURBSCurveDefinition(System.UInt32 nurbsCurve);
		/// <summary>
		/// get all parameters contained in the nurbsCurve
		/// </summary>
		/// <param name="nurbsCurve">The nurbsCurve</param>
		public static CAD.Native.getNURBSCurveDefinitionReturn GetNURBSCurveDefinition(System.UInt32 nurbsCurve) {
			var ret = CAD_getNURBSCurveDefinition(nurbsCurve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getNURBSCurveDefinitionReturn retStruct = new CAD.Native.getNURBSCurveDefinitionReturn();
			retStruct.degree = (System.Int32)ret.degree;
			retStruct.knots = ConvertValue(ref ret.knots);
			Core.Native.NativeInterface.Core_DoubleList_free(ref ret.knots);
			retStruct.poles = ConvertValue(ref ret.poles);
			Geom.Native.NativeInterface.Geom_Point3List_free(ref ret.poles);
			retStruct.weights = ConvertValue(ref ret.weights);
			Core.Native.NativeInterface.Core_DoubleList_free(ref ret.weights);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getNURBSSurfaceDefinitionReturn_c CAD_getNURBSSurfaceDefinition(System.UInt32 nurbsSurface);
		/// <summary>
		/// get all parameters contained in the nurbsSurface
		/// </summary>
		/// <param name="nurbsSurface">The nurbsSurface</param>
		public static CAD.Native.getNURBSSurfaceDefinitionReturn GetNURBSSurfaceDefinition(System.UInt32 nurbsSurface) {
			var ret = CAD_getNURBSSurfaceDefinition(nurbsSurface);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getNURBSSurfaceDefinitionReturn retStruct = new CAD.Native.getNURBSSurfaceDefinitionReturn();
			retStruct.degreeU = (System.Int32)ret.degreeU;
			retStruct.degreeV = (System.Int32)ret.degreeV;
			retStruct.knotsU = ConvertValue(ref ret.knotsU);
			Core.Native.NativeInterface.Core_DoubleList_free(ref ret.knotsU);
			retStruct.knotsV = ConvertValue(ref ret.knotsV);
			Core.Native.NativeInterface.Core_DoubleList_free(ref ret.knotsV);
			retStruct.poles = ConvertValue(ref ret.poles);
			Geom.Native.NativeInterface.Geom_Point3ListList_free(ref ret.poles);
			retStruct.weights = ConvertValue(ref ret.weights);
			Core.Native.NativeInterface.Core_DoubleListList_free(ref ret.weights);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getOffsetCurveDefinitionReturn_c CAD_getOffsetCurveDefinition(System.UInt32 offsetCurve);
		/// <summary>
		/// get all parameters contained in the offsetCurve
		/// </summary>
		/// <param name="offsetCurve">The offsetCurve</param>
		public static CAD.Native.getOffsetCurveDefinitionReturn GetOffsetCurveDefinition(System.UInt32 offsetCurve) {
			var ret = CAD_getOffsetCurveDefinition(offsetCurve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getOffsetCurveDefinitionReturn retStruct = new CAD.Native.getOffsetCurveDefinitionReturn();
			retStruct.curve = (System.UInt32)ret.curve;
			retStruct.direction = ConvertValue(ref ret.direction);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.direction);
			retStruct.distance = (System.Double)ret.distance;
			retStruct.surfaceReference = (System.UInt32)ret.surfaceReference;
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getOffsetSurfaceDefinitionReturn_c CAD_getOffsetSurfaceDefinition(System.UInt32 offsetSurface);
		/// <summary>
		/// get all parameters contained in the offsetSurface
		/// </summary>
		/// <param name="offsetSurface">The offsetSurface</param>
		public static CAD.Native.getOffsetSurfaceDefinitionReturn GetOffsetSurfaceDefinition(System.UInt32 offsetSurface) {
			var ret = CAD_getOffsetSurfaceDefinition(offsetSurface);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getOffsetSurfaceDefinitionReturn retStruct = new CAD.Native.getOffsetSurfaceDefinitionReturn();
			retStruct.baseSurface = (System.UInt32)ret.baseSurface;
			retStruct.distance = (System.Double)ret.distance;
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern OrientedDomainList_c CAD_getOpenShellOrientedDomains(System.UInt32 openShell);
		/// <summary>
		/// get all orienteDomains contain in the openShell
		/// </summary>
		/// <param name="openShell">The openShell</param>
		public static OrientedDomainList GetOpenShellOrientedDomains(System.UInt32 openShell) {
			var ret = CAD_getOpenShellOrientedDomains(openShell);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			CAD.Native.NativeInterface.CAD_OrientedDomainList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getParabolaCurveDefinitionReturn_c CAD_getParabolaCurveDefinition(System.UInt32 parabolaCurve);
		/// <summary>
		/// get all parameters contained in the parabolaCurve
		/// </summary>
		/// <param name="parabolaCurve">The parabolaCurve</param>
		public static CAD.Native.getParabolaCurveDefinitionReturn GetParabolaCurveDefinition(System.UInt32 parabolaCurve) {
			var ret = CAD_getParabolaCurveDefinition(parabolaCurve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getParabolaCurveDefinitionReturn retStruct = new CAD.Native.getParabolaCurveDefinitionReturn();
			retStruct.focalLength = (System.Double)ret.focalLength;
			retStruct.matrix = ConvertValue(ref ret.matrix);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref ret.matrix);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Geom.Native.Matrix4_c CAD_getPlaneSurfaceDefinition(System.UInt32 planeSurface);
		/// <summary>
		/// get all parameters contained in the planeSurface
		/// </summary>
		/// <param name="planeSurface">The planeSurface</param>
		public static Geom.Native.Matrix4 GetPlaneSurfaceDefinition(System.UInt32 planeSurface) {
			var ret = CAD_getPlaneSurfaceDefinition(planeSurface);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Geom.Native.NativeInterface.ConvertValue(ref ret);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getPolylineCurveDefinitionReturn_c CAD_getPolylineCurveDefinition(System.UInt32 polylineCurve);
		/// <summary>
		/// get all parameters contained in the polylinCurve
		/// </summary>
		/// <param name="polylineCurve">The polylineCurve</param>
		public static CAD.Native.getPolylineCurveDefinitionReturn GetPolylineCurveDefinition(System.UInt32 polylineCurve) {
			var ret = CAD_getPolylineCurveDefinition(polylineCurve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getPolylineCurveDefinitionReturn retStruct = new CAD.Native.getPolylineCurveDefinitionReturn();
			retStruct.points = ConvertValue(ref ret.points);
			Geom.Native.NativeInterface.Geom_Point3List_free(ref ret.points);
			retStruct.parameters = ConvertValue(ref ret.parameters);
			Core.Native.NativeInterface.Core_DoubleList_free(ref ret.parameters);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getRevolutionSurfaceDefinitionReturn_c CAD_getRevolutionSurfaceDefinition(System.UInt32 revolutionSurface);
		/// <summary>
		/// get all parameters contained in the revolutionSurface
		/// </summary>
		/// <param name="revolutionSurface">The revolutionSurface</param>
		public static CAD.Native.getRevolutionSurfaceDefinitionReturn GetRevolutionSurfaceDefinition(System.UInt32 revolutionSurface) {
			var ret = CAD_getRevolutionSurfaceDefinition(revolutionSurface);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getRevolutionSurfaceDefinitionReturn retStruct = new CAD.Native.getRevolutionSurfaceDefinitionReturn();
			retStruct.generatricCurve = (System.UInt32)ret.generatricCurve;
			retStruct.axisOrigin = ConvertValue(ref ret.axisOrigin);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.axisOrigin);
			retStruct.axisDirection = ConvertValue(ref ret.axisDirection);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.axisDirection);
			retStruct.startAngle = (System.Double)ret.startAngle;
			retStruct.endAngle = (System.Double)ret.endAngle;
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getRuledSurfaceDefinitionReturn_c CAD_getRuledSurfaceDefinition(System.UInt32 ruledSurface);
		/// <summary>
		/// get all parameters contained in the ruledSurface
		/// </summary>
		/// <param name="ruledSurface">The ruledSurface</param>
		public static CAD.Native.getRuledSurfaceDefinitionReturn GetRuledSurfaceDefinition(System.UInt32 ruledSurface) {
			var ret = CAD_getRuledSurfaceDefinition(ruledSurface);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getRuledSurfaceDefinitionReturn retStruct = new CAD.Native.getRuledSurfaceDefinitionReturn();
			retStruct.firstCurve = (System.UInt32)ret.firstCurve;
			retStruct.secondCurve = (System.UInt32)ret.secondCurve;
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getSegmentCurveDefinitionReturn_c CAD_getSegmentCurveDefinition(System.UInt32 segmentCurve);
		/// <summary>
		/// get all parameters contained in the segmentCurve
		/// </summary>
		/// <param name="segmentCurve">The segmentCurve</param>
		public static CAD.Native.getSegmentCurveDefinitionReturn GetSegmentCurveDefinition(System.UInt32 segmentCurve) {
			var ret = CAD_getSegmentCurveDefinition(segmentCurve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getSegmentCurveDefinitionReturn retStruct = new CAD.Native.getSegmentCurveDefinitionReturn();
			retStruct.startPoint = ConvertValue(ref ret.startPoint);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.startPoint);
			retStruct.endPoint = ConvertValue(ref ret.endPoint);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.endPoint);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getSphereSurfaceDefinitionReturn_c CAD_getSphereSurfaceDefinition(System.UInt32 sphereSurface);
		/// <summary>
		/// get all parameters contained in the sphereSurface
		/// </summary>
		/// <param name="sphereSurface">The sphereSurface</param>
		public static CAD.Native.getSphereSurfaceDefinitionReturn GetSphereSurfaceDefinition(System.UInt32 sphereSurface) {
			var ret = CAD_getSphereSurfaceDefinition(sphereSurface);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getSphereSurfaceDefinitionReturn retStruct = new CAD.Native.getSphereSurfaceDefinitionReturn();
			retStruct.radius = (System.Double)ret.radius;
			retStruct.matrix = ConvertValue(ref ret.matrix);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref ret.matrix);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Bounds2D_c CAD_getSurfaceLimits(System.UInt32 surface);
		/// <summary>
		/// get the parametric space limits of a surface
		/// </summary>
		/// <param name="surface">The surface</param>
		public static Bounds2D GetSurfaceLimits(System.UInt32 surface) {
			var ret = CAD_getSurfaceLimits(surface);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			CAD.Native.NativeInterface.CAD_Bounds2D_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getSurfacicCurveDefinitionReturn_c CAD_getSurfacicCurveDefinition(System.UInt32 surfacicCurve);
		/// <summary>
		/// get all parameters contained in the surfacicCurve
		/// </summary>
		/// <param name="surfacicCurve">The surfacicCurve</param>
		public static CAD.Native.getSurfacicCurveDefinitionReturn GetSurfacicCurveDefinition(System.UInt32 surfacicCurve) {
			var ret = CAD_getSurfacicCurveDefinition(surfacicCurve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getSurfacicCurveDefinitionReturn retStruct = new CAD.Native.getSurfacicCurveDefinitionReturn();
			retStruct.surface = (System.UInt32)ret.surface;
			retStruct.curve2D = (System.UInt32)ret.curve2D;
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getTabulatedCylinderSurfaceDefinitionReturn_c CAD_getTabulatedCylinderSurfaceDefinition(System.UInt32 tabulatedCylinderSurface);
		/// <summary>
		/// get all parameters contained in the TabulatedCylinderSurface
		/// </summary>
		/// <param name="tabulatedCylinderSurface">The tabulatedCylinderSurface</param>
		public static CAD.Native.getTabulatedCylinderSurfaceDefinitionReturn GetTabulatedCylinderSurfaceDefinition(System.UInt32 tabulatedCylinderSurface) {
			var ret = CAD_getTabulatedCylinderSurfaceDefinition(tabulatedCylinderSurface);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getTabulatedCylinderSurfaceDefinitionReturn retStruct = new CAD.Native.getTabulatedCylinderSurfaceDefinitionReturn();
			retStruct.directrixCurve = (System.UInt32)ret.directrixCurve;
			retStruct.generatrixLine = ConvertValue(ref ret.generatrixLine);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.generatrixLine);
			retStruct.range = ConvertValue(ref ret.range);
			CAD.Native.NativeInterface.CAD_Bounds1D_free(ref ret.range);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getTorusSurfaceDefinitionReturn_c CAD_getTorusSurfaceDefinition(System.UInt32 torusSurface);
		/// <summary>
		/// get all parameters contained in the torusSurface
		/// </summary>
		/// <param name="torusSurface">The torusSurface</param>
		public static CAD.Native.getTorusSurfaceDefinitionReturn GetTorusSurfaceDefinition(System.UInt32 torusSurface) {
			var ret = CAD_getTorusSurfaceDefinition(torusSurface);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getTorusSurfaceDefinitionReturn retStruct = new CAD.Native.getTorusSurfaceDefinitionReturn();
			retStruct.majorRadius = (System.Double)ret.majorRadius;
			retStruct.minorRadius = (System.Double)ret.minorRadius;
			retStruct.matrix = ConvertValue(ref ret.matrix);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref ret.matrix);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getTransformedCurveDefinitionReturn_c CAD_getTransformedCurveDefinition(System.UInt32 transformedCurve);
		/// <summary>
		/// get all parameters contained in the transformedCurve
		/// </summary>
		/// <param name="transformedCurve">The transformedCurve</param>
		public static CAD.Native.getTransformedCurveDefinitionReturn GetTransformedCurveDefinition(System.UInt32 transformedCurve) {
			var ret = CAD_getTransformedCurveDefinition(transformedCurve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.getTransformedCurveDefinitionReturn retStruct = new CAD.Native.getTransformedCurveDefinitionReturn();
			retStruct.curve = (System.UInt32)ret.curve;
			retStruct.matrix = ConvertValue(ref ret.matrix);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref ret.matrix);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Geom.Native.Point3_c CAD_getVertexPosition(System.UInt32 vertex);
		/// <summary>
		/// get the position of the vertex
		/// </summary>
		/// <param name="vertex">The vertex</param>
		public static Geom.Native.Point3 GetVertexPosition(System.UInt32 vertex) {
			var ret = CAD_getVertexPosition(vertex);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Geom.Native.NativeInterface.ConvertValue(ref ret);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 CAD_isCurveClosed(System.UInt32 curve);
		/// <summary>
		/// if the curve is closed, return true, return false otherwise
		/// </summary>
		/// <param name="curve">The curve</param>
		public static System.Boolean IsCurveClosed(System.UInt32 curve) {
			var ret = CAD_isCurveClosed(curve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Boolean)(0 != ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern isCurvePeriodicReturn_c CAD_isCurvePeriodic(System.UInt32 curve);
		/// <summary>
		/// if the curve is periodic return true, return false otherwise
		/// </summary>
		/// <param name="curve">The curve</param>
		public static CAD.Native.isCurvePeriodicReturn IsCurvePeriodic(System.UInt32 curve) {
			var ret = CAD_isCurvePeriodic(curve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.isCurvePeriodicReturn retStruct = new CAD.Native.isCurvePeriodicReturn();
			retStruct.periodic = (0 != ret.periodic);
			retStruct.period = (System.Double)ret.period;
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern isSurfaceClosedReturn_c CAD_isSurfaceClosed(System.UInt32 surface);
		/// <summary>
		/// return if the surface is closed on U or on V
		/// </summary>
		/// <param name="surface">The surface</param>
		public static CAD.Native.isSurfaceClosedReturn IsSurfaceClosed(System.UInt32 surface) {
			var ret = CAD_isSurfaceClosed(surface);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.isSurfaceClosedReturn retStruct = new CAD.Native.isSurfaceClosedReturn();
			retStruct.closedU = (0 != ret.closedU);
			retStruct.closedV = (0 != ret.closedV);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern isSurfacePeriodicReturn_c CAD_isSurfacePeriodic(System.UInt32 surface);
		/// <summary>
		/// return if the surface is periodic on U or on V
		/// </summary>
		/// <param name="surface">The surface</param>
		public static CAD.Native.isSurfacePeriodicReturn IsSurfacePeriodic(System.UInt32 surface) {
			var ret = CAD_isSurfacePeriodic(surface);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			CAD.Native.isSurfacePeriodicReturn retStruct = new CAD.Native.isSurfacePeriodicReturn();
			retStruct.periodicU = (0 != ret.periodicU);
			retStruct.periodicV = (0 != ret.periodicV);
			retStruct.periodU = (System.Double)ret.periodU;
			retStruct.periodV = (System.Double)ret.periodV;
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.Double CAD_projectOnCurve(System.UInt32 curve, Geom.Native.Point3_c point, System.Double precision);
		/// <summary>
		/// project a point to a curve
		/// </summary>
		/// <param name="curve">The curve</param>
		/// <param name="point">The point to project</param>
		/// <param name="precision">Projection precision</param>
		public static System.Double ProjectOnCurve(System.UInt32 curve, Geom.Native.Point3 point, System.Double precision) {
			var point_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(point, ref point_c);
			var ret = CAD_projectOnCurve(curve, point_c, precision);
			Geom.Native.NativeInterface.Geom_Point3_free(ref point_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Double)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Geom.Native.Point2_c CAD_projectOnSurface(System.UInt32 surface, Geom.Native.Point3_c point, System.Double precision);
		/// <summary>
		/// project a point to a surface
		/// </summary>
		/// <param name="surface">The surface</param>
		/// <param name="point">The point to project</param>
		/// <param name="precision">Projection precision</param>
		public static Geom.Native.Point2 ProjectOnSurface(System.UInt32 surface, Geom.Native.Point3 point, System.Double precision) {
			var point_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(point, ref point_c);
			var ret = CAD_projectOnSurface(surface, point_c, precision);
			Geom.Native.NativeInterface.Geom_Point3_free(ref point_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Geom.Native.NativeInterface.ConvertValue(ref ret);
			Geom.Native.NativeInterface.Geom_Point2_free(ref ret);
			return convRet;
		}

		#endregion

		#region structure creation

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createBody(System.UInt32 outerShell, ClosedShellList_c innerShells);
		/// <summary>
		/// Create a body from a surface
		/// </summary>
		/// <param name="outerShell">ClosedShell used to create the body</param>
		/// <param name="innerShells">List of closedShell used to create the body</param>
		public static System.UInt32 CreateBody(System.UInt32 outerShell, ClosedShellList innerShells) {
			var innerShells_c = new CAD.Native.ClosedShellList_c();
			ConvertValue(innerShells, ref innerShells_c);
			var ret = CAD_createBody(outerShell, innerShells_c);
			CAD.Native.NativeInterface.CAD_ClosedShellList_free(ref innerShells_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createClosedShell(DomainList_c domains, Geom.Native.OrientationList_c orientations);
		/// <summary>
		/// Create a closedShell from a set of domains of a set of orientations
		/// </summary>
		/// <param name="domains">List of domains composing the closedShell</param>
		/// <param name="orientations">List of orientations for each domain</param>
		public static System.UInt32 CreateClosedShell(DomainList domains, Geom.Native.OrientationList orientations) {
			var domains_c = new CAD.Native.DomainList_c();
			ConvertValue(domains, ref domains_c);
			var orientations_c = new Geom.Native.OrientationList_c();
			Geom.Native.NativeInterface.ConvertValue(orientations, ref orientations_c);
			var ret = CAD_createClosedShell(domains_c, orientations_c);
			CAD.Native.NativeInterface.CAD_DomainList_free(ref domains_c);
			Geom.Native.NativeInterface.Geom_OrientationList_free(ref orientations_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createCoEdge(System.UInt32 edge, Int32 orientation, System.UInt32 surface, System.UInt32 curve2D);
		/// <summary>
		/// Create an coEdge with a edge and an orientation
		/// </summary>
		/// <param name="edge">Edge used to create the coEdge</param>
		/// <param name="orientation">Orientation of the edge regarding the loop</param>
		/// <param name="surface">The surface trimmed by the edge</param>
		/// <param name="curve2D">Surfacic curve of the edge on the surface trimmed</param>
		public static System.UInt32 CreateCoEdge(System.UInt32 edge, System.Boolean orientation, System.UInt32 surface, System.UInt32 curve2D) {
			var ret = CAD_createCoEdge(edge, orientation ? 1 : 0, surface, curve2D);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createEdge(System.UInt32 curve, System.UInt32 startVertex, System.UInt32 endVertex);
		/// <summary>
		/// Create an edge with a curve an extremity vertices
		/// </summary>
		/// <param name="curve">Curve used to create the edge</param>
		/// <param name="startVertex">The start vertex</param>
		/// <param name="endVertex">The end vertex</param>
		public static System.UInt32 CreateEdge(System.UInt32 curve, System.UInt32 startVertex, System.UInt32 endVertex) {
			var ret = CAD_createEdge(curve, startVertex, endVertex);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createEdgeFromCurve(System.UInt32 curve);
		/// <summary>
		/// Create an edge from a limited curve
		/// </summary>
		/// <param name="curve">Limited curve used to create the edge</param>
		public static System.UInt32 CreateEdgeFromCurve(System.UInt32 curve) {
			var ret = CAD_createEdgeFromCurve(curve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createFace(System.UInt32 surface, LoopList_c loopList, Int32 useSurfaceOrientation);
		/// <summary>
		/// Create a face from a surface
		/// </summary>
		/// <param name="surface">Surface used to create the face</param>
		/// <param name="loopList">List of Loops used to create the face</param>
		/// <param name="useSurfaceOrientation">If True, the face will have the same orientation than the surface and loops will be inverted if they are inconsistent</param>
		public static System.UInt32 CreateFace(System.UInt32 surface, LoopList loopList, System.Boolean useSurfaceOrientation) {
			var loopList_c = new CAD.Native.LoopList_c();
			ConvertValue(loopList, ref loopList_c);
			var ret = CAD_createFace(surface, loopList_c, useSurfaceOrientation ? 1 : 0);
			CAD.Native.NativeInterface.CAD_LoopList_free(ref loopList_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createLoop(CoEdgeList_c coEdges, Int32 check);
		/// <summary>
		/// Create a loop from a set of edges of a set of orientations
		/// </summary>
		/// <param name="coEdges">List of coEdges composing the loop</param>
		/// <param name="check">If true, the loop check if edges are well connected or not</param>
		public static System.UInt32 CreateLoop(CoEdgeList coEdges, System.Boolean check) {
			var coEdges_c = new CAD.Native.CoEdgeList_c();
			ConvertValue(coEdges, ref coEdges_c);
			var ret = CAD_createLoop(coEdges_c, check ? 1 : 0);
			CAD.Native.NativeInterface.CAD_CoEdgeList_free(ref coEdges_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createOpenShell(DomainList_c domains, Geom.Native.OrientationList_c orientations, LoopList_c loopList);
		/// <summary>
		/// Create a openShell from a set of domains of a set of orientations and set of loops
		/// </summary>
		/// <param name="domains">List of domains composing the openShell</param>
		/// <param name="orientations">List of orientations for each domain</param>
		/// <param name="loopList">List of loops restricted the openShell</param>
		public static System.UInt32 CreateOpenShell(DomainList domains, Geom.Native.OrientationList orientations, LoopList loopList) {
			var domains_c = new CAD.Native.DomainList_c();
			ConvertValue(domains, ref domains_c);
			var orientations_c = new Geom.Native.OrientationList_c();
			Geom.Native.NativeInterface.ConvertValue(orientations, ref orientations_c);
			var loopList_c = new CAD.Native.LoopList_c();
			ConvertValue(loopList, ref loopList_c);
			var ret = CAD_createOpenShell(domains_c, orientations_c, loopList_c);
			CAD.Native.NativeInterface.CAD_DomainList_free(ref domains_c);
			Geom.Native.NativeInterface.Geom_OrientationList_free(ref orientations_c);
			CAD.Native.NativeInterface.CAD_LoopList_free(ref loopList_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createVertex(Geom.Native.Point3_c position);
		/// <summary>
		/// Create a vertex from a position
		/// </summary>
		/// <param name="position">Vertex position</param>
		public static System.UInt32 CreateVertex(Geom.Native.Point3 position) {
			var position_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(position, ref position_c);
			var ret = CAD_createVertex(position_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref position_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		#endregion

		#region surfaces

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_convertSurfaceIntoNurbs(System.UInt32 surface, System.Double minBoundsU, System.Double maxBoundsU, System.Double minBoundsV, System.Double maxBoundsV);
		/// <summary>
		/// Convert a surface into a nurbs
		/// </summary>
		/// <param name="surface">Surface to convert</param>
		/// <param name="minBoundsU">Minimum bound on the U axis parameter </param>
		/// <param name="maxBoundsU">Maximum bound on the U axis parameter </param>
		/// <param name="minBoundsV">Minimum bound on the V axis parameter </param>
		/// <param name="maxBoundsV">Maximum bound on the V axis parameter </param>
		public static System.UInt32 ConvertSurfaceIntoNurbs(System.UInt32 surface, System.Double minBoundsU, System.Double maxBoundsU, System.Double minBoundsV, System.Double maxBoundsV) {
			var ret = CAD_convertSurfaceIntoNurbs(surface, minBoundsU, maxBoundsU, minBoundsV, maxBoundsV);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createBezierSurface(Int32 degreeU, Int32 degreeV, Geom.Native.Point3List_c poles);
		/// <summary>
		/// Create a new bezier surface
		/// </summary>
		/// <param name="degreeU">U degree</param>
		/// <param name="degreeV">V degree</param>
		/// <param name="poles">Poles list</param>
		public static System.UInt32 CreateBezierSurface(System.Int32 degreeU, System.Int32 degreeV, Geom.Native.Point3List poles) {
			var poles_c = new Geom.Native.Point3List_c();
			Geom.Native.NativeInterface.ConvertValue(poles, ref poles_c);
			var ret = CAD_createBezierSurface(degreeU, degreeV, poles_c);
			Geom.Native.NativeInterface.Geom_Point3List_free(ref poles_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createConeSurface(System.Double radius, System.Double semiAngle, Geom.Native.Matrix4_c matrix);
		/// <summary>
		/// Create a new cone surface
		/// </summary>
		/// <param name="radius">Radius of the cone at origin</param>
		/// <param name="semiAngle">Semi-angle of the cone</param>
		/// <param name="matrix">Positionning matrix of the cone</param>
		public static System.UInt32 CreateConeSurface(System.Double radius, System.Double semiAngle, Geom.Native.Matrix4 matrix) {
			var matrix_c = new Geom.Native.Matrix4_c();
			Geom.Native.NativeInterface.ConvertValue(matrix, ref matrix_c);
			var ret = CAD_createConeSurface(radius, semiAngle, matrix_c);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref matrix_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createCurveExtrusionSurface(System.UInt32 generatrixCurve, System.UInt32 directrixCurve, System.UInt32 refSurface, System.Double precision);
		/// <summary>
		/// Create a new curveExtrusion surface
		/// </summary>
		/// <param name="generatrixCurve">The generatrix curve</param>
		/// <param name="directrixCurve">The directrix curve</param>
		/// <param name="refSurface">The reference surface</param>
		/// <param name="precision">The precision for the evaluation of points</param>
		public static System.UInt32 CreateCurveExtrusionSurface(System.UInt32 generatrixCurve, System.UInt32 directrixCurve, System.UInt32 refSurface, System.Double precision) {
			var ret = CAD_createCurveExtrusionSurface(generatrixCurve, directrixCurve, refSurface, precision);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createCylinderSurface(System.Double radius, Geom.Native.Matrix4_c matrix);
		/// <summary>
		/// Create a new cylinder surface
		/// </summary>
		/// <param name="radius">Radius of the cylinder</param>
		/// <param name="matrix">Positionning matrix of the cylinder</param>
		public static System.UInt32 CreateCylinderSurface(System.Double radius, Geom.Native.Matrix4 matrix) {
			var matrix_c = new Geom.Native.Matrix4_c();
			Geom.Native.NativeInterface.ConvertValue(matrix, ref matrix_c);
			var ret = CAD_createCylinderSurface(radius, matrix_c);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref matrix_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createEllipticConeSurface(System.Double radius1, System.Double radius2, System.Double semiAngle, Geom.Native.Matrix4_c matrix);
		/// <summary>
		/// Create a new elliptic cone surface
		/// </summary>
		/// <param name="radius1">Radius of the cone at origin on the X axis</param>
		/// <param name="radius2">Radius of the cone at origin on the Y axis</param>
		/// <param name="semiAngle">Semi-angle of the cone</param>
		/// <param name="matrix">Positionning matrix of the cone</param>
		public static System.UInt32 CreateEllipticConeSurface(System.Double radius1, System.Double radius2, System.Double semiAngle, Geom.Native.Matrix4 matrix) {
			var matrix_c = new Geom.Native.Matrix4_c();
			Geom.Native.NativeInterface.ConvertValue(matrix, ref matrix_c);
			var ret = CAD_createEllipticConeSurface(radius1, radius2, semiAngle, matrix_c);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref matrix_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createNURBSSurface(Int32 degreeU, Int32 degreeV, Core.Native.DoubleList_c knotsU, Core.Native.DoubleList_c knotsV, Geom.Native.Point3List_c poles, Core.Native.DoubleList_c weights);
		/// <summary>
		/// Create a new NURBS surface
		/// </summary>
		/// <param name="degreeU">U degree</param>
		/// <param name="degreeV">V degree</param>
		/// <param name="knotsU">Knots on U</param>
		/// <param name="knotsV">Knots on V</param>
		/// <param name="poles">Poles list</param>
		/// <param name="weights">Weights list</param>
		public static System.UInt32 CreateNURBSSurface(System.Int32 degreeU, System.Int32 degreeV, Core.Native.DoubleList knotsU, Core.Native.DoubleList knotsV, Geom.Native.Point3List poles, Core.Native.DoubleList weights) {
			var knotsU_c = new Core.Native.DoubleList_c();
			Core.Native.NativeInterface.ConvertValue(knotsU, ref knotsU_c);
			var knotsV_c = new Core.Native.DoubleList_c();
			Core.Native.NativeInterface.ConvertValue(knotsV, ref knotsV_c);
			var poles_c = new Geom.Native.Point3List_c();
			Geom.Native.NativeInterface.ConvertValue(poles, ref poles_c);
			var weights_c = new Core.Native.DoubleList_c();
			Core.Native.NativeInterface.ConvertValue(weights, ref weights_c);
			var ret = CAD_createNURBSSurface(degreeU, degreeV, knotsU_c, knotsV_c, poles_c, weights_c);
			Core.Native.NativeInterface.Core_DoubleList_free(ref knotsU_c);
			Core.Native.NativeInterface.Core_DoubleList_free(ref knotsV_c);
			Geom.Native.NativeInterface.Geom_Point3List_free(ref poles_c);
			Core.Native.NativeInterface.Core_DoubleList_free(ref weights_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createOffsetSurface(System.UInt32 baseSurface, System.Double distance);
		/// <summary>
		/// Create a new offset surface
		/// </summary>
		/// <param name="baseSurface">The base surface</param>
		/// <param name="distance">The offset distance</param>
		public static System.UInt32 CreateOffsetSurface(System.UInt32 baseSurface, System.Double distance) {
			var ret = CAD_createOffsetSurface(baseSurface, distance);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createPlaneSurface(Geom.Native.Matrix4_c matrix);
		/// <summary>
		/// Create a new plane surface
		/// </summary>
		/// <param name="matrix">Positionning matrix of the plane</param>
		public static System.UInt32 CreatePlaneSurface(Geom.Native.Matrix4 matrix) {
			var matrix_c = new Geom.Native.Matrix4_c();
			Geom.Native.NativeInterface.ConvertValue(matrix, ref matrix_c);
			var ret = CAD_createPlaneSurface(matrix_c);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref matrix_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createRevolutionSurface(System.UInt32 generatrixCurve, Geom.Native.Point3_c axisOrigin, Geom.Native.Point3_c axisDirection, System.Double startAngle, System.Double endAngle);
		/// <summary>
		/// Create a new revolution surface
		/// </summary>
		/// <param name="generatrixCurve">Generatrix curve rotated to create the revolution surface</param>
		/// <param name="axisOrigin">Axis origin point</param>
		/// <param name="axisDirection">Axis direction vector</param>
		/// <param name="startAngle">Start angle of the revolution surface</param>
		/// <param name="endAngle">End angle of the revolution surface</param>
		public static System.UInt32 CreateRevolutionSurface(System.UInt32 generatrixCurve, Geom.Native.Point3 axisOrigin, Geom.Native.Point3 axisDirection, System.Double startAngle, System.Double endAngle) {
			var axisOrigin_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(axisOrigin, ref axisOrigin_c);
			var axisDirection_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(axisDirection, ref axisDirection_c);
			var ret = CAD_createRevolutionSurface(generatrixCurve, axisOrigin_c, axisDirection_c, startAngle, endAngle);
			Geom.Native.NativeInterface.Geom_Point3_free(ref axisOrigin_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref axisDirection_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createRuledSurface(System.UInt32 firstCurve, System.UInt32 secondCurve);
		/// <summary>
		/// Create a new ruled surface
		/// </summary>
		/// <param name="firstCurve">First Curve</param>
		/// <param name="secondCurve">Seconde Curve</param>
		public static System.UInt32 CreateRuledSurface(System.UInt32 firstCurve, System.UInt32 secondCurve) {
			var ret = CAD_createRuledSurface(firstCurve, secondCurve);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createSphereSurface(System.Double radius, Geom.Native.Matrix4_c matrix);
		/// <summary>
		/// Create a new sphere surface
		/// </summary>
		/// <param name="radius">Radius of the sphere</param>
		/// <param name="matrix">Positionning matrix of the sphere</param>
		public static System.UInt32 CreateSphereSurface(System.Double radius, Geom.Native.Matrix4 matrix) {
			var matrix_c = new Geom.Native.Matrix4_c();
			Geom.Native.NativeInterface.ConvertValue(matrix, ref matrix_c);
			var ret = CAD_createSphereSurface(radius, matrix_c);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref matrix_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createTabulatedCylinderSurface(System.UInt32 directrixCurve, Geom.Native.Point3_c GeneratixLine, System.Double minRange, System.Double maxRange);
		/// <summary>
		/// Create a new tabulated cylinder surface
		/// </summary>
		/// <param name="directrixCurve">Directrix Curve</param>
		/// <param name="GeneratixLine">Generatrix Line</param>
		/// <param name="minRange">Minimimum value of the range</param>
		/// <param name="maxRange">Maximum value of the range</param>
		public static System.UInt32 CreateTabulatedCylinderSurface(System.UInt32 directrixCurve, Geom.Native.Point3 GeneratixLine, System.Double minRange, System.Double maxRange) {
			var GeneratixLine_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(GeneratixLine, ref GeneratixLine_c);
			var ret = CAD_createTabulatedCylinderSurface(directrixCurve, GeneratixLine_c, minRange, maxRange);
			Geom.Native.NativeInterface.Geom_Point3_free(ref GeneratixLine_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 CAD_createTorusSurface(System.Double radiusMax, System.Double radiusMin, Geom.Native.Matrix4_c matrix);
		/// <summary>
		/// Create a new torus surface
		/// </summary>
		/// <param name="radiusMax">Major radius </param>
		/// <param name="radiusMin">Minor radius </param>
		/// <param name="matrix">Positionning matrix of the sphere</param>
		public static System.UInt32 CreateTorusSurface(System.Double radiusMax, System.Double radiusMin, Geom.Native.Matrix4 matrix) {
			var matrix_c = new Geom.Native.Matrix4_c();
			Geom.Native.NativeInterface.ConvertValue(matrix, ref matrix_c);
			var ret = CAD_createTorusSurface(radiusMax, radiusMin, matrix_c);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref matrix_c);
			System.String err = ConvertValue(CAD_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		#endregion

	}
}
