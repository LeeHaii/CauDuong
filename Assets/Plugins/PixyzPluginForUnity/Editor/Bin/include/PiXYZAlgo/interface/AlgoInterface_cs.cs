#pragma warning disable CA2101

using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Security;
using System.Collections.Concurrent;
using PiXYZAPI = Pixyz.API.Native.NativeInterface;

namespace Pixyz.Algo.Native {

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
		internal static extern void Algo_BakeMap_init(ref BakeMap_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_BakeMap_free(ref BakeMap_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_MapTypeList_init(ref MapTypeList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_MapTypeList_free(ref MapTypeList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_BakeMapList_init(ref BakeMapList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_BakeMapList_free(ref BakeMapList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_PlaneParameters_init(ref PlaneParameters_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_PlaneParameters_free(ref PlaneParameters_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_CylinderParameters_init(ref CylinderParameters_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_CylinderParameters_free(ref CylinderParameters_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_ConeParameters_init(ref ConeParameters_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_ConeParameters_free(ref ConeParameters_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_BoxParameters_init(ref BoxParameters_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_BoxParameters_free(ref BoxParameters_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_SphereParameters_init(ref SphereParameters_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_SphereParameters_free(ref SphereParameters_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_HexahedronParameters_init(ref HexahedronParameters_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_HexahedronParameters_free(ref HexahedronParameters_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_FeatureInput_init(ref FeatureInput_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_FeatureInput_free(ref FeatureInput_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_FeatureInputList_init(ref FeatureInputList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_FeatureInputList_free(ref FeatureInputList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_Feature_init(ref Feature_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_Feature_free(ref Feature_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_FeatureList_init(ref FeatureList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_FeatureList_free(ref FeatureList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_Box_init(ref Box_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_Box_free(ref Box_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_BakeMaps_init(ref BakeMaps_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_BakeMaps_free(ref BakeMaps_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_BakeOption_init(ref BakeOption_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_BakeOption_free(ref BakeOption_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_DecimateOptionsSelector_init(ref DecimateOptionsSelector_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_DecimateOptionsSelector_free(ref DecimateOptionsSelector_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_ReplaceByOccurrenceOptions_init(ref ReplaceByOccurrenceOptions_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_ReplaceByOccurrenceOptions_free(ref ReplaceByOccurrenceOptions_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_OctahedralImpostor_init(ref OctahedralImpostor_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_OctahedralImpostor_free(ref OctahedralImpostor_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_OccurrenceFeatures_init(ref OccurrenceFeatures_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_OccurrenceFeatures_free(ref OccurrenceFeatures_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_OccurrenceFeaturesList_init(ref OccurrenceFeaturesList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_OccurrenceFeaturesList_free(ref OccurrenceFeaturesList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_CustomBakeMap_init(ref CustomBakeMap_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_CustomBakeMap_free(ref CustomBakeMap_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_CustomBakeMapList_init(ref CustomBakeMapList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_CustomBakeMapList_free(ref CustomBakeMapList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_Plane_init(ref Plane_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_Plane_free(ref Plane_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_Cylinder_init(ref Cylinder_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_Cylinder_free(ref Cylinder_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_getPixelValueList_init(ref getPixelValueList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_getPixelValueList_free(ref getPixelValueList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_UseColorOption_init(ref UseColorOption_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_UseColorOption_free(ref UseColorOption_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_PrimitiveShapeParameters_init(ref PrimitiveShapeParameters_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_PrimitiveShapeParameters_free(ref PrimitiveShapeParameters_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_ReplaceByPrimitiveOptions_init(ref ReplaceByPrimitiveOptions_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_ReplaceByPrimitiveOptions_free(ref ReplaceByPrimitiveOptions_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_Sphere_init(ref Sphere_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_Sphere_free(ref Sphere_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_ReplaceByOption_init(ref ReplaceByOption_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Algo_ReplaceByOption_free(ref ReplaceByOption_c sel);

	public static Scene.Native.PropertyValue ConvertValue(ref Scene.Native.PropertyValue_c s) {
		Scene.Native.PropertyValue ss = new Scene.Native.PropertyValue();
		ss.name = ConvertValue(s.name);
		ss.value = ConvertValue(s.value);
		return ss;
	}

	public static Scene.Native.PropertyValue_c ConvertValue(Scene.Native.PropertyValue s, ref Scene.Native.PropertyValue_c ss) {
		Scene.Native.NativeInterface.Scene_PropertyValue_init(ref ss);
		ss.name = ConvertValue(s.name);
		ss.value = ConvertValue(s.value);
		return ss;
	}

	public static Scene.Native.PropertyValueList ConvertValue(ref Scene.Native.PropertyValueList_c s) {
		Scene.Native.PropertyValueList list = new Scene.Native.PropertyValueList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Scene.Native.PropertyValue_c)));
			Scene.Native.PropertyValue_c value = (Scene.Native.PropertyValue_c)Marshal.PtrToStructure(p, typeof(Scene.Native.PropertyValue_c));
			list.list[i] = Scene.Native.NativeInterface.ConvertValue(ref value);
		}
		return list;
	}

	public static Scene.Native.PropertyValueList_c ConvertValue(Scene.Native.PropertyValueList s, ref Scene.Native.PropertyValueList_c list) {
		Scene.Native.NativeInterface.Scene_PropertyValueList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			Scene.Native.PropertyValue_c elt = new Scene.Native.PropertyValue_c();
			Scene.Native.NativeInterface.ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Scene.Native.PropertyValue_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static BakeMap ConvertValue(ref BakeMap_c s) {
		BakeMap ss = new BakeMap();
		ss.type = (MapType)s.type;
		ss.properties = Scene.Native.NativeInterface.ConvertValue(ref s.properties);
		return ss;
	}

	public static BakeMap_c ConvertValue(BakeMap s, ref BakeMap_c ss) {
		Algo.Native.NativeInterface.Algo_BakeMap_init(ref ss);
		ss.type = (Int32)s.type;
		Scene.Native.NativeInterface.ConvertValue(s.properties, ref ss.properties);
		return ss;
	}

	public static MapTypeList ConvertValue(ref MapTypeList_c s) {
		MapTypeList list = new MapTypeList((int)s.size);
		if (s.size==0) return list;
		int[] tab = new int[s.size];
		Marshal.Copy(s.ptr, tab, 0, (int)s.size);
		for (int i = 0; i < (int)s.size; ++i) {
			list.list[i] = (MapType)tab[i];
		}
		return list;
	}

	public static MapTypeList_c ConvertValue(MapTypeList s, ref MapTypeList_c list) {
		Algo.Native.NativeInterface.Algo_MapTypeList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)s.list[i];
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static BakeMapList ConvertValue(ref BakeMapList_c s) {
		BakeMapList list = new BakeMapList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(BakeMap_c)));
			BakeMap_c value = (BakeMap_c)Marshal.PtrToStructure(p, typeof(BakeMap_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static BakeMapList_c ConvertValue(BakeMapList s, ref BakeMapList_c list) {
		Algo.Native.NativeInterface.Algo_BakeMapList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			BakeMap_c elt = new BakeMap_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(BakeMap_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static PlaneParameters ConvertValue(ref PlaneParameters_c s) {
		PlaneParameters ss = new PlaneParameters();
		ss.SizeX = (System.Double)s.SizeX;
		ss.SizeY = (System.Double)s.SizeY;
		ss.SubdivisionX = (System.Int32)s.SubdivisionX;
		ss.SubdivisionY = (System.Int32)s.SubdivisionY;
		return ss;
	}

	public static PlaneParameters_c ConvertValue(PlaneParameters s, ref PlaneParameters_c ss) {
		Algo.Native.NativeInterface.Algo_PlaneParameters_init(ref ss);
		ss.SizeX = (System.Double)s.SizeX;
		ss.SizeY = (System.Double)s.SizeY;
		ss.SubdivisionX = (Int32)s.SubdivisionX;
		ss.SubdivisionY = (Int32)s.SubdivisionY;
		return ss;
	}

	public static CylinderParameters ConvertValue(ref CylinderParameters_c s) {
		CylinderParameters ss = new CylinderParameters();
		ss.Radius = (System.Double)s.Radius;
		ss.Height = (System.Double)s.Height;
		ss.Sides = (System.Int32)s.Sides;
		return ss;
	}

	public static CylinderParameters_c ConvertValue(CylinderParameters s, ref CylinderParameters_c ss) {
		Algo.Native.NativeInterface.Algo_CylinderParameters_init(ref ss);
		ss.Radius = (System.Double)s.Radius;
		ss.Height = (System.Double)s.Height;
		ss.Sides = (Int32)s.Sides;
		return ss;
	}

	public static ConeParameters ConvertValue(ref ConeParameters_c s) {
		ConeParameters ss = new ConeParameters();
		ss.BottomRadius = (System.Double)s.BottomRadius;
		ss.Height = (System.Double)s.Height;
		ss.Sides = (System.Int32)s.Sides;
		return ss;
	}

	public static ConeParameters_c ConvertValue(ConeParameters s, ref ConeParameters_c ss) {
		Algo.Native.NativeInterface.Algo_ConeParameters_init(ref ss);
		ss.BottomRadius = (System.Double)s.BottomRadius;
		ss.Height = (System.Double)s.Height;
		ss.Sides = (Int32)s.Sides;
		return ss;
	}

	public static BoxParameters ConvertValue(ref BoxParameters_c s) {
		BoxParameters ss = new BoxParameters();
		ss.SizeX = (System.Double)s.SizeX;
		ss.SizeY = (System.Double)s.SizeY;
		ss.SizeZ = (System.Double)s.SizeZ;
		ss.Subdivision = (System.Int32)s.Subdivision;
		return ss;
	}

	public static BoxParameters_c ConvertValue(BoxParameters s, ref BoxParameters_c ss) {
		Algo.Native.NativeInterface.Algo_BoxParameters_init(ref ss);
		ss.SizeX = (System.Double)s.SizeX;
		ss.SizeY = (System.Double)s.SizeY;
		ss.SizeZ = (System.Double)s.SizeZ;
		ss.Subdivision = (Int32)s.Subdivision;
		return ss;
	}

	public static SphereParameters ConvertValue(ref SphereParameters_c s) {
		SphereParameters ss = new SphereParameters();
		ss.Radius = (System.Double)s.Radius;
		ss.Latitude = (System.Int32)s.Latitude;
		ss.Longitude = (System.Int32)s.Longitude;
		return ss;
	}

	public static SphereParameters_c ConvertValue(SphereParameters s, ref SphereParameters_c ss) {
		Algo.Native.NativeInterface.Algo_SphereParameters_init(ref ss);
		ss.Radius = (System.Double)s.Radius;
		ss.Latitude = (Int32)s.Latitude;
		ss.Longitude = (Int32)s.Longitude;
		return ss;
	}

	public static HexahedronParameters ConvertValue(ref HexahedronParameters_c s) {
		HexahedronParameters ss = new HexahedronParameters();
		ss.XLength = (System.Double)s.XLength;
		ss.YLength = (System.Double)s.YLength;
		ss.ZLength = (System.Double)s.ZLength;
		return ss;
	}

	public static HexahedronParameters_c ConvertValue(HexahedronParameters s, ref HexahedronParameters_c ss) {
		Algo.Native.NativeInterface.Algo_HexahedronParameters_init(ref ss);
		ss.XLength = (System.Double)s.XLength;
		ss.YLength = (System.Double)s.YLength;
		ss.ZLength = (System.Double)s.ZLength;
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

	public static FeatureInput ConvertValue(ref FeatureInput_c s) {
		FeatureInput ss = new FeatureInput();
		ss.position = Geom.Native.NativeInterface.ConvertValue(ref s.position);
		ss.direction = Geom.Native.NativeInterface.ConvertValue(ref s.direction);
		ss.diameter = (System.Double)s.diameter;
		return ss;
	}

	public static FeatureInput_c ConvertValue(FeatureInput s, ref FeatureInput_c ss) {
		Algo.Native.NativeInterface.Algo_FeatureInput_init(ref ss);
		Geom.Native.NativeInterface.ConvertValue(s.position, ref ss.position);
		Geom.Native.NativeInterface.ConvertValue(s.direction, ref ss.direction);
		ss.diameter = (System.Double)s.diameter;
		return ss;
	}

	public static FeatureInputList ConvertValue(ref FeatureInputList_c s) {
		FeatureInputList list = new FeatureInputList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(FeatureInput_c)));
			FeatureInput_c value = (FeatureInput_c)Marshal.PtrToStructure(p, typeof(FeatureInput_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static FeatureInputList_c ConvertValue(FeatureInputList s, ref FeatureInputList_c list) {
		Algo.Native.NativeInterface.Algo_FeatureInputList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			FeatureInput_c elt = new FeatureInput_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(FeatureInput_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static Feature ConvertValue(ref Feature_c s) {
		Feature ss = new Feature();
		ss.type = (FeatureType)s.type;
		ss.inputs = ConvertValue(ref s.inputs);
		return ss;
	}

	public static Feature_c ConvertValue(Feature s, ref Feature_c ss) {
		Algo.Native.NativeInterface.Algo_Feature_init(ref ss);
		ss.type = (Int32)s.type;
		ConvertValue(s.inputs, ref ss.inputs);
		return ss;
	}

	public static FeatureList ConvertValue(ref FeatureList_c s) {
		FeatureList list = new FeatureList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Feature_c)));
			Feature_c value = (Feature_c)Marshal.PtrToStructure(p, typeof(Feature_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static FeatureList_c ConvertValue(FeatureList s, ref FeatureList_c list) {
		Algo.Native.NativeInterface.Algo_FeatureList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			Feature_c elt = new Feature_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Feature_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static Geom.Native.Affine ConvertValue(ref Geom.Native.Affine_c s) {
		Geom.Native.Affine ss = new Geom.Native.Affine();
		ss.origin = Geom.Native.NativeInterface.ConvertValue(ref s.origin);
		ss.xAxis = Geom.Native.NativeInterface.ConvertValue(ref s.xAxis);
		ss.yAxis = Geom.Native.NativeInterface.ConvertValue(ref s.yAxis);
		ss.zAxis = Geom.Native.NativeInterface.ConvertValue(ref s.zAxis);
		return ss;
	}

	public static Geom.Native.Affine_c ConvertValue(Geom.Native.Affine s, ref Geom.Native.Affine_c ss) {
		Geom.Native.NativeInterface.Geom_Affine_init(ref ss);
		Geom.Native.NativeInterface.ConvertValue(s.origin, ref ss.origin);
		Geom.Native.NativeInterface.ConvertValue(s.xAxis, ref ss.xAxis);
		Geom.Native.NativeInterface.ConvertValue(s.yAxis, ref ss.yAxis);
		Geom.Native.NativeInterface.ConvertValue(s.zAxis, ref ss.zAxis);
		return ss;
	}

	public static Box ConvertValue(ref Box_c s) {
		Box ss = new Box();
		ss.position = Geom.Native.NativeInterface.ConvertValue(ref s.position);
		ss.length = (System.Double)s.length;
		ss.height = (System.Double)s.height;
		ss.depth = (System.Double)s.depth;
		return ss;
	}

	public static Box_c ConvertValue(Box s, ref Box_c ss) {
		Algo.Native.NativeInterface.Algo_Box_init(ref ss);
		Geom.Native.NativeInterface.ConvertValue(s.position, ref ss.position);
		ss.length = (System.Double)s.length;
		ss.height = (System.Double)s.height;
		ss.depth = (System.Double)s.depth;
		return ss;
	}

	public static BakeMaps ConvertValue(ref BakeMaps_c s) {
		BakeMaps ss = new BakeMaps();
		ss.diffuse = ConvertValue(s.diffuse);
		ss.normal = ConvertValue(s.normal);
		ss.roughness = ConvertValue(s.roughness);
		ss.metallic = ConvertValue(s.metallic);
		ss.opacity = ConvertValue(s.opacity);
		ss.ambientOcclusion = ConvertValue(s.ambientOcclusion);
		ss.emissive = ConvertValue(s.emissive);
		return ss;
	}

	public static BakeMaps_c ConvertValue(BakeMaps s, ref BakeMaps_c ss) {
		Algo.Native.NativeInterface.Algo_BakeMaps_init(ref ss);
		ss.diffuse = ConvertValue(s.diffuse);
		ss.normal = ConvertValue(s.normal);
		ss.roughness = ConvertValue(s.roughness);
		ss.metallic = ConvertValue(s.metallic);
		ss.opacity = ConvertValue(s.opacity);
		ss.ambientOcclusion = ConvertValue(s.ambientOcclusion);
		ss.emissive = ConvertValue(s.emissive);
		return ss;
	}

	public static BakeOption ConvertValue(ref BakeOption_c s) {
		BakeOption ss = new BakeOption();
		ss.bakingMethod = (BakingMethod)s.bakingMethod;
		ss.resolution = (System.Int32)s.resolution;
		ss.padding = (System.Int32)s.padding;
		ss.textures = ConvertValue(ref s.textures);
		return ss;
	}

	public static BakeOption_c ConvertValue(BakeOption s, ref BakeOption_c ss) {
		Algo.Native.NativeInterface.Algo_BakeOption_init(ref ss);
		ss.bakingMethod = (Int32)s.bakingMethod;
		ss.resolution = (Int32)s.resolution;
		ss.padding = (Int32)s.padding;
		ConvertValue(s.textures, ref ss.textures);
		return ss;
	}

	public static DecimateOptionsSelector ConvertValue(ref DecimateOptionsSelector_c s) {
		DecimateOptionsSelector ss = new DecimateOptionsSelector();
		ss._type = (DecimateOptionsSelector.Type)s._type;
		switch(ss._type) {
			case DecimateOptionsSelector.Type.UNKNOWN: break;
			case DecimateOptionsSelector.Type.TRIANGLECOUNT: ss.triangleCount = (Int32)s.triangleCount; break;
			case DecimateOptionsSelector.Type.RATIO: ss.ratio = (System.Double)s.ratio; break;
		}
		return ss;
	}

	public static DecimateOptionsSelector_c ConvertValue(DecimateOptionsSelector s, ref DecimateOptionsSelector_c ss) {
		Algo.Native.NativeInterface.Algo_DecimateOptionsSelector_init(ref ss);
		ss._type = (int)s._type;
		switch (ss._type) {
			case 0: break;
			case 1: ss.triangleCount = (Int32)s.triangleCount; break;
			case 2: ss.ratio = (System.Double)s.ratio; break;
		}
		return ss;
	}

	public static ReplaceByOccurrenceOptions ConvertValue(ref ReplaceByOccurrenceOptions_c s) {
		ReplaceByOccurrenceOptions ss = new ReplaceByOccurrenceOptions();
		ss.Occurrence = (System.UInt32)s.Occurrence;
		ss.Aligned = ConvertValue(s.Aligned);
		return ss;
	}

	public static ReplaceByOccurrenceOptions_c ConvertValue(ReplaceByOccurrenceOptions s, ref ReplaceByOccurrenceOptions_c ss) {
		Algo.Native.NativeInterface.Algo_ReplaceByOccurrenceOptions_init(ref ss);
		ss.Occurrence = (System.UInt32)s.Occurrence;
		ss.Aligned = ConvertValue(s.Aligned);
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

	public static OctahedralImpostor ConvertValue(ref OctahedralImpostor_c s) {
		OctahedralImpostor ss = new OctahedralImpostor();
		ss.OctaTransform = Geom.Native.NativeInterface.ConvertValue(ref s.OctaTransform);
		ss.Radius = (System.Double)s.Radius;
		ss.NormalMap = (System.UInt32)s.NormalMap;
		ss.DepthMap = (System.UInt32)s.DepthMap;
		ss.DiffuseMap = (System.UInt32)s.DiffuseMap;
		ss.MetallicMap = (System.UInt32)s.MetallicMap;
		ss.AOMap = (System.UInt32)s.AOMap;
		ss.RoughnessMap = (System.UInt32)s.RoughnessMap;
		return ss;
	}

	public static OctahedralImpostor_c ConvertValue(OctahedralImpostor s, ref OctahedralImpostor_c ss) {
		Algo.Native.NativeInterface.Algo_OctahedralImpostor_init(ref ss);
		Geom.Native.NativeInterface.ConvertValue(s.OctaTransform, ref ss.OctaTransform);
		ss.Radius = (System.Double)s.Radius;
		ss.NormalMap = (System.UInt32)s.NormalMap;
		ss.DepthMap = (System.UInt32)s.DepthMap;
		ss.DiffuseMap = (System.UInt32)s.DiffuseMap;
		ss.MetallicMap = (System.UInt32)s.MetallicMap;
		ss.AOMap = (System.UInt32)s.AOMap;
		ss.RoughnessMap = (System.UInt32)s.RoughnessMap;
		return ss;
	}

	public static OccurrenceFeatures ConvertValue(ref OccurrenceFeatures_c s) {
		OccurrenceFeatures ss = new OccurrenceFeatures();
		ss.occurrence = (System.UInt32)s.occurrence;
		ss.features = ConvertValue(ref s.features);
		return ss;
	}

	public static OccurrenceFeatures_c ConvertValue(OccurrenceFeatures s, ref OccurrenceFeatures_c ss) {
		Algo.Native.NativeInterface.Algo_OccurrenceFeatures_init(ref ss);
		ss.occurrence = (System.UInt32)s.occurrence;
		ConvertValue(s.features, ref ss.features);
		return ss;
	}

	public static OccurrenceFeaturesList ConvertValue(ref OccurrenceFeaturesList_c s) {
		OccurrenceFeaturesList list = new OccurrenceFeaturesList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(OccurrenceFeatures_c)));
			OccurrenceFeatures_c value = (OccurrenceFeatures_c)Marshal.PtrToStructure(p, typeof(OccurrenceFeatures_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static OccurrenceFeaturesList_c ConvertValue(OccurrenceFeaturesList s, ref OccurrenceFeaturesList_c list) {
		Algo.Native.NativeInterface.Algo_OccurrenceFeaturesList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			OccurrenceFeatures_c elt = new OccurrenceFeatures_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(OccurrenceFeatures_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static CustomBakeMap ConvertValue(ref CustomBakeMap_c s) {
		CustomBakeMap ss = new CustomBakeMap();
		ss.name = ConvertValue(s.name);
		ss.component = (System.Int32)s.component;
		return ss;
	}

	public static CustomBakeMap_c ConvertValue(CustomBakeMap s, ref CustomBakeMap_c ss) {
		Algo.Native.NativeInterface.Algo_CustomBakeMap_init(ref ss);
		ss.name = ConvertValue(s.name);
		ss.component = (Int32)s.component;
		return ss;
	}

	public static CustomBakeMapList ConvertValue(ref CustomBakeMapList_c s) {
		CustomBakeMapList list = new CustomBakeMapList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(CustomBakeMap_c)));
			CustomBakeMap_c value = (CustomBakeMap_c)Marshal.PtrToStructure(p, typeof(CustomBakeMap_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static CustomBakeMapList_c ConvertValue(CustomBakeMapList s, ref CustomBakeMapList_c list) {
		Algo.Native.NativeInterface.Algo_CustomBakeMapList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			CustomBakeMap_c elt = new CustomBakeMap_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(CustomBakeMap_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static Plane ConvertValue(ref Plane_c s) {
		Plane ss = new Plane();
		ss.position = Geom.Native.NativeInterface.ConvertValue(ref s.position);
		ss.length = (System.Double)s.length;
		ss.height = (System.Double)s.height;
		return ss;
	}

	public static Plane_c ConvertValue(Plane s, ref Plane_c ss) {
		Algo.Native.NativeInterface.Algo_Plane_init(ref ss);
		Geom.Native.NativeInterface.ConvertValue(s.position, ref ss.position);
		ss.length = (System.Double)s.length;
		ss.height = (System.Double)s.height;
		return ss;
	}

	public static Cylinder ConvertValue(ref Cylinder_c s) {
		Cylinder ss = new Cylinder();
		ss.position = Geom.Native.NativeInterface.ConvertValue(ref s.position);
		ss.radius = (System.Double)s.radius;
		ss.length = (System.Double)s.length;
		return ss;
	}

	public static Cylinder_c ConvertValue(Cylinder s, ref Cylinder_c ss) {
		Algo.Native.NativeInterface.Algo_Cylinder_init(ref ss);
		Geom.Native.NativeInterface.ConvertValue(s.position, ref ss.position);
		ss.radius = (System.Double)s.radius;
		ss.length = (System.Double)s.length;
		return ss;
	}

	private static Core.Native.ColorAlpha_c getPixelValue_CS(Geom.Native.Point3_c fromPos, Geom.Native.Point2_c param, System.UInt64 polygonIndex, System.UInt32 occurrence, getPixelValue callback) {

		var retC = new Core.Native.ColorAlpha_c();
		var retCS = callback(Geom.Native.NativeInterface.ConvertValue(ref fromPos), Geom.Native.NativeInterface.ConvertValue(ref param), (System.UInt64)(polygonIndex), (System.UInt32)(occurrence));
		Core.Native.NativeInterface.ConvertValue(retCS, ref retC);
		return retC;
	}

	public static getPixelValueList ConvertValue(ref getPixelValueList_c s) {
		getPixelValueList list = new getPixelValueList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			//TODO;
		}
		return list;
	}

	public static getPixelValueList_c ConvertValue(getPixelValueList s, ref getPixelValueList_c list) {
		Algo.Native.NativeInterface.Algo_getPixelValueList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		IntPtr[] tab = new IntPtr[list.size];
		for(int i = 0; i < (int)list.size; ++i) {
			int index = i;
			getPixelValue_c callbackC = (fromPos, param, polygonIndex, occurrence) => {
				return getPixelValue_CS(fromPos, param, polygonIndex, occurrence, s.list[index]); 

			};
			tab[i] = Marshal.GetFunctionPointerForDelegate(callbackC);
		}
			Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static Core.Native.Color ConvertValue(ref Core.Native.Color_c s) {
		Core.Native.Color ss = new Core.Native.Color();
		ss.r = (System.Double)s.r;
		ss.g = (System.Double)s.g;
		ss.b = (System.Double)s.b;
		return ss;
	}

	public static Core.Native.Color_c ConvertValue(Core.Native.Color s, ref Core.Native.Color_c ss) {
		Core.Native.NativeInterface.Core_Color_init(ref ss);
		ss.r = (System.Double)s.r;
		ss.g = (System.Double)s.g;
		ss.b = (System.Double)s.b;
		return ss;
	}

	public static UseColorOption ConvertValue(ref UseColorOption_c s) {
		UseColorOption ss = new UseColorOption();
		ss._type = (UseColorOption.Type)s._type;
		switch(ss._type) {
			case UseColorOption.Type.UNKNOWN: break;
			case UseColorOption.Type.USELINESCOLOR: ss.UseLinesColor = (Int32)s.UseLinesColor; break;
			case UseColorOption.Type.CHOOSECOLOR: ss.ChooseColor = ConvertValue(ref s.ChooseColor); break;
		}
		return ss;
	}

	public static UseColorOption_c ConvertValue(UseColorOption s, ref UseColorOption_c ss) {
		Algo.Native.NativeInterface.Algo_UseColorOption_init(ref ss);
		ss._type = (int)s._type;
		switch (ss._type) {
			case 0: break;
			case 1: ss.UseLinesColor = (Int32)s.UseLinesColor; break;
			case 2: ConvertValue(s.ChooseColor, ref ss.ChooseColor); break;
		}
		return ss;
	}

	public static PrimitiveShapeParameters ConvertValue(ref PrimitiveShapeParameters_c s) {
		PrimitiveShapeParameters ss = new PrimitiveShapeParameters();
		ss._type = (PrimitiveShapeParameters.Type)s._type;
		switch(ss._type) {
			case PrimitiveShapeParameters.Type.UNKNOWN: break;
			case PrimitiveShapeParameters.Type.BOX: ss.Box = ConvertValue(ref s.Box); break;
			case PrimitiveShapeParameters.Type.PLANE: ss.Plane = ConvertValue(ref s.Plane); break;
			case PrimitiveShapeParameters.Type.SPHERE: ss.Sphere = ConvertValue(ref s.Sphere); break;
			case PrimitiveShapeParameters.Type.CYLINDER: ss.Cylinder = ConvertValue(ref s.Cylinder); break;
			case PrimitiveShapeParameters.Type.CONE: ss.Cone = ConvertValue(ref s.Cone); break;
		}
		return ss;
	}

	public static PrimitiveShapeParameters_c ConvertValue(PrimitiveShapeParameters s, ref PrimitiveShapeParameters_c ss) {
		Algo.Native.NativeInterface.Algo_PrimitiveShapeParameters_init(ref ss);
		ss._type = (int)s._type;
		switch (ss._type) {
			case 0: break;
			case 1: ConvertValue(s.Box, ref ss.Box); break;
			case 2: ConvertValue(s.Plane, ref ss.Plane); break;
			case 3: ConvertValue(s.Sphere, ref ss.Sphere); break;
			case 4: ConvertValue(s.Cylinder, ref ss.Cylinder); break;
			case 5: ConvertValue(s.Cone, ref ss.Cone); break;
		}
		return ss;
	}

	public static ReplaceByPrimitiveOptions ConvertValue(ref ReplaceByPrimitiveOptions_c s) {
		ReplaceByPrimitiveOptions ss = new ReplaceByPrimitiveOptions();
		ss.Type = ConvertValue(ref s.Type);
		ss.Aligned = ConvertValue(s.Aligned);
		ss.GenerateUV = ConvertValue(s.GenerateUV);
		return ss;
	}

	public static ReplaceByPrimitiveOptions_c ConvertValue(ReplaceByPrimitiveOptions s, ref ReplaceByPrimitiveOptions_c ss) {
		Algo.Native.NativeInterface.Algo_ReplaceByPrimitiveOptions_init(ref ss);
		ConvertValue(s.Type, ref ss.Type);
		ss.Aligned = ConvertValue(s.Aligned);
		ss.GenerateUV = ConvertValue(s.GenerateUV);
		return ss;
	}

	public static Sphere ConvertValue(ref Sphere_c s) {
		Sphere ss = new Sphere();
		ss.position = Geom.Native.NativeInterface.ConvertValue(ref s.position);
		ss.radius = (System.Double)s.radius;
		return ss;
	}

	public static Sphere_c ConvertValue(Sphere s, ref Sphere_c ss) {
		Algo.Native.NativeInterface.Algo_Sphere_init(ref ss);
		Geom.Native.NativeInterface.ConvertValue(s.position, ref ss.position);
		ss.radius = (System.Double)s.radius;
		return ss;
	}

	public static ReplaceByOption ConvertValue(ref ReplaceByOption_c s) {
		ReplaceByOption ss = new ReplaceByOption();
		ss._type = (ReplaceByOption.Type)s._type;
		switch(ss._type) {
			case ReplaceByOption.Type.UNKNOWN: break;
			case ReplaceByOption.Type.OCCURRENCE: ss.Occurrence = ConvertValue(ref s.Occurrence); break;
			case ReplaceByOption.Type.BOUNDINGBOX: ss.BoundingBox = (ReplaceByBoxType)s.BoundingBox; break;
			case ReplaceByOption.Type.CONVEXHULL: ss.ConvexHull = (Int32)s.ConvexHull; break;
			case ReplaceByOption.Type.PRIMITIVE: ss.Primitive = ConvertValue(ref s.Primitive); break;
		}
		return ss;
	}

	public static ReplaceByOption_c ConvertValue(ReplaceByOption s, ref ReplaceByOption_c ss) {
		Algo.Native.NativeInterface.Algo_ReplaceByOption_init(ref ss);
		ss._type = (int)s._type;
		switch (ss._type) {
			case 0: break;
			case 1: ConvertValue(s.Occurrence, ref ss.Occurrence); break;
			case 2: ss.BoundingBox = (Int32)s.BoundingBox; break;
			case 3: ss.ConvexHull = (Int32)s.ConvexHull; break;
			case 4: ConvertValue(s.Primitive, ref ss.Primitive); break;
		}
		return ss;
	}

	public static getVisibilityStatsReturn ConvertValue(ref getVisibilityStatsReturn_c s) {
		getVisibilityStatsReturn ss = new getVisibilityStatsReturn();
		ss.visibleCountFront = (System.Int32)s.visibleCountFront;
		ss.visibleCountBack = (System.Int32)s.visibleCountBack;
		return ss;
	}

	public static getVisibilityStatsReturn_c ConvertValue(getVisibilityStatsReturn s, ref getVisibilityStatsReturn_c ss) {
		ss.visibleCountFront = (Int32)s.visibleCountFront;
		ss.visibleCountBack = (Int32)s.visibleCountBack;
		return ss;
	}

		#endregion

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Algo_getLastError();

		public static string GetLastError()
		{
			return ConvertValue(Algo_getLastError());
		}

		#region 

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_assembleCAD(Scene.Native.OccurrenceList_c occurrences, System.Double tolerance, Int32 removeDuplicatedFaces);
		/// <summary>
		/// Assemble faces of CAD shapes
		/// </summary>
		/// <param name="occurrences">Occurrences of components to assemble</param>
		/// <param name="tolerance">Assembling tolerance</param>
		/// <param name="removeDuplicatedFaces">If True, duplicated faces will be removed</param>
		public static void AssembleCAD(Scene.Native.OccurrenceList occurrences, System.Double tolerance, System.Boolean removeDuplicatedFaces) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_assembleCAD(occurrences_c, tolerance, removeDuplicatedFaces ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_backToInitialBRep(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Create the BRep shape from a Tessellated shape with Domain Patch Attributes (after tessellate)
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		public static void BackToInitialBRep(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_backToInitialBRep(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern OctahedralImpostor_c Algo_bakeImpostor(System.UInt32 occurrence, Int32 XFrames, Int32 YFrames, Int32 hemi, Int32 resolution, Int32 padding, Int32 roughness, Int32 metallic, Int32 ao);
		/// <summary>
		/// bakes impostors textures
		/// </summary>
		/// <param name="occurrence"></param>
		/// <param name="XFrames"></param>
		/// <param name="YFrames"></param>
		/// <param name="hemi"></param>
		/// <param name="resolution"></param>
		/// <param name="padding"></param>
		/// <param name="roughness"></param>
		/// <param name="metallic"></param>
		/// <param name="ao"></param>
		public static OctahedralImpostor BakeImpostor(System.UInt32 occurrence, System.Int32 XFrames, System.Int32 YFrames, System.Boolean hemi, System.Int32 resolution, System.Int32 padding, System.Boolean roughness, System.Boolean metallic, System.Boolean ao) {
			var ret = Algo_bakeImpostor(occurrence, XFrames, YFrames, hemi ? 1 : 0, resolution, padding, roughness ? 1 : 0, metallic ? 1 : 0, ao ? 1 : 0);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Algo.Native.NativeInterface.Algo_OctahedralImpostor_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_bakeVertexAttributes(Scene.Native.OccurrenceList_c destinationOccurrences, Scene.Native.OccurrenceList_c sourceOccurrences, Int32 skinnedMesh, Int32 positions, Int32 useCurrentPositionAsTPose);
		/// <summary>
		/// Bake vertex attributes on meshes from other meshes
		/// </summary>
		/// <param name="destinationOccurrences">Occurrences of the meshes where to store the baked vertex attributes</param>
		/// <param name="sourceOccurrences">Occurrences of components from which to bake vertex attributes</param>
		/// <param name="skinnedMesh">Enabling skinned mesh baking (joint assignation for animation transfert)</param>
		/// <param name="positions">Enabling vertex position baking</param>
		/// <param name="useCurrentPositionAsTPose">Use the current position as the T-Pose</param>
		public static void BakeVertexAttributes(Scene.Native.OccurrenceList destinationOccurrences, Scene.Native.OccurrenceList sourceOccurrences, System.Boolean skinnedMesh, System.Boolean positions, System.Boolean useCurrentPositionAsTPose) {
			var destinationOccurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(destinationOccurrences, ref destinationOccurrences_c);
			var sourceOccurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(sourceOccurrences, ref sourceOccurrences_c);
			Algo_bakeVertexAttributes(destinationOccurrences_c, sourceOccurrences_c, skinnedMesh ? 1 : 0, positions ? 1 : 0, useCurrentPositionAsTPose ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref destinationOccurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref sourceOccurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Algo_basicRetopologize(Scene.Native.OccurrenceList_c occurrences, Int32 targetTriangleCount, Int32 pureQuad, Int32 pointCloud);
		/// <summary>
		/// Replace the tessellations of the selected parts by a retopology
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="targetTriangleCount">Target triangle count</param>
		/// <param name="pureQuad">Retopologize to a pure quad mesh if True, else the resulting mesh will be quad dominant but can contains triangles</param>
		/// <param name="pointCloud">Set to true if occurrences are point cloud, else False</param>
		public static System.UInt32 BasicRetopologize(Scene.Native.OccurrenceList occurrences, System.Int32 targetTriangleCount, System.Boolean pureQuad, System.Boolean pointCloud) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_basicRetopologize(occurrences_c, targetTriangleCount, pureQuad ? 1 : 0, pointCloud ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_calculateNormalsInPointClouds(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// calculate the normal of each point of a Point Cloud
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		public static void CalculateNormalsInPointClouds(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_calculateNormalsInPointClouds(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Algo_checkTessellationFlags(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="occurrences">Occurrences of components to check</param>
		public static System.Boolean CheckTessellationFlags(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_checkTessellationFlags(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Boolean)(0 != ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Scene.Native.OccurrenceList_c Algo_combineMeshesByMaterials(Scene.Native.OccurrenceList_c occurrences, Int32 mergeNoMaterials, Int32 mergeHiddenPartsMode);
		/// <summary>
		/// Explode and (re)merge a set of mesh parts by visible materials
		/// </summary>
		/// <param name="occurrences">Occurrences of the parts to merge</param>
		/// <param name="mergeNoMaterials">If true, merge all parts with no active material together, else do not merge them</param>
		/// <param name="mergeHiddenPartsMode">Hidden parts handling mode, Destroy them, make visible or merge separately</param>
		public static Scene.Native.OccurrenceList CombineMeshesByMaterials(Scene.Native.OccurrenceList occurrences, System.Boolean mergeNoMaterials, Scene.Native.MergeHiddenPartsMode mergeHiddenPartsMode) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_combineMeshesByMaterials(occurrences_c, mergeNoMaterials ? 1 : 0, (int)mergeHiddenPartsMode);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Scene.Native.NativeInterface.ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_convertCADtoNURBS(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Convert all CAD curves/surfaces to NURBS definition
		/// </summary>
		/// <param name="occurrences">Occurrences of components to convert</param>
		public static void ConvertCADtoNURBS(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_convertCADtoNURBS(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_convertSimilarOccurencesToInstances(Scene.Native.OccurrenceList_c occurrences, Int32 checkMeshTopo, Int32 checkVertexPositions, Int32 vertexPositionPrecision, Int32 checkUVTopo, Int32 checkUVVertexPositions, Int32 UVPositionprecision, Int32 keepExistingPrototypes);
		/// <summary>
		/// Create instances when there are similar parts.
		/// </summary>
		/// <param name="occurrences">Occurrence for which we want to find similar parts and create instances using prototypes.</param>
		/// <param name="checkMeshTopo"></param>
		/// <param name="checkVertexPositions"></param>
		/// <param name="vertexPositionPrecision"></param>
		/// <param name="checkUVTopo"></param>
		/// <param name="checkUVVertexPositions"></param>
		/// <param name="UVPositionprecision"></param>
		/// <param name="keepExistingPrototypes"></param>
		public static void ConvertSimilarOccurencesToInstances(Scene.Native.OccurrenceList occurrences, System.Boolean checkMeshTopo, System.Boolean checkVertexPositions, System.Int32 vertexPositionPrecision, System.Boolean checkUVTopo, System.Boolean checkUVVertexPositions, System.Int32 UVPositionprecision, System.Boolean keepExistingPrototypes) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_convertSimilarOccurencesToInstances(occurrences_c, checkMeshTopo ? 1 : 0, checkVertexPositions ? 1 : 0, vertexPositionPrecision, checkUVTopo ? 1 : 0, checkUVVertexPositions ? 1 : 0, UVPositionprecision, keepExistingPrototypes ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_convexDecomposition(Scene.Native.OccurrenceList_c occurrences, Int32 maxCount, Int32 vertexCount, Int32 approximate, Int32 resolution, System.Double concavity);
		/// <summary>
		/// Explode each mesh to approximated convex decomposition
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="maxCount">Maximum number of convex hull to generated</param>
		/// <param name="vertexCount">Maximum number of vertices per convex hull</param>
		/// <param name="approximate">Approximate method</param>
		/// <param name="resolution">Resolution</param>
		/// <param name="concavity">Concavity</param>
		public static void ConvexDecomposition(Scene.Native.OccurrenceList occurrences, System.Int32 maxCount, System.Int32 vertexCount, System.Boolean approximate, System.Int32 resolution, System.Double concavity) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_convexDecomposition(occurrences_c, maxCount, vertexCount, approximate ? 1 : 0, resolution, concavity);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_crackEdges(Scene.Native.OccurrenceList_c occurrences, Int32 useAttributesFilter, Int32 useSharpEdgeFilter, System.Double sharpAngleFilter, Int32 useNonManifoldFilter);
		/// <summary>
		/// crack polygonal edges according to given criteria
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="useAttributesFilter">Filters by attribute</param>
		/// <param name="useSharpEdgeFilter">Filters by edge sharpness.</param>
		/// <param name="sharpAngleFilter">Sharp angle, if negative the default sharp angle value is used</param>
		/// <param name="useNonManifoldFilter">Filters by manifold-ness.</param>
		public static void CrackEdges(Scene.Native.OccurrenceList occurrences, System.Boolean useAttributesFilter, System.Boolean useSharpEdgeFilter, System.Double sharpAngleFilter, System.Boolean useNonManifoldFilter) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_crackEdges(occurrences_c, useAttributesFilter ? 1 : 0, useSharpEdgeFilter ? 1 : 0, sharpAngleFilter, useNonManifoldFilter ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_createFreeEdgesFromPatches(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Create free edges from patch borders
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		public static void CreateFreeEdgesFromPatches(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_createFreeEdgesFromPatches(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_createIdentifiedPatchesFromPatches(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Create identified patch from existing patch (this is usefull before cloning for baking)
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		public static void CreateIdentifiedPatchesFromPatches(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_createIdentifiedPatchesFromPatches(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_createInstancesBySimilarity(Scene.Native.OccurrenceList_c occurrences, System.Double dimensionsSimilarity, System.Double polycountSimilarity, Int32 ignoreSymmetry, Int32 keepExistingPrototypes, Int32 createNewOccurrencesForPrototypes);
		/// <summary>
		/// Create instances when there are similar parts. This can be used to repair instances or to simplify a model that has similar parts that could be instanciated instead to reduce the number of unique meshes (reduces drawcalls, GPU memory usage and file size). Using 1.0 (100%) in all similarity criterias is non destructive. Using lower values will help finding more similar parts, even if their polycount or dimensions varies a bit.
		/// </summary>
		/// <param name="occurrences">Occurrence for which we want to find similar parts and create instances using prototypes.</param>
		/// <param name="dimensionsSimilarity">The percentage of similarity on dimensions. A value of 1.0 (100%) will find parts that have exactly the same dimensions. A lower value will increase the likelyhood to find similar parts, at the cost of precision.</param>
		/// <param name="polycountSimilarity">The percentage of similarity on polycount. A value of 1.0 (100%) will find parts that have exactly the same polycount. A lower value will increase the likelyhood to find similar parts, at the cost of precision.</param>
		/// <param name="ignoreSymmetry">If True, symmetries will be ignored, otherwise negative scaling will be applied in the occurrence transformation.</param>
		/// <param name="keepExistingPrototypes">If True, existing prototypes will be kept. Otherwise, the selection will be singularized and instanced will be created from scratch.</param>
		/// <param name="createNewOccurrencesForPrototypes">If True, a new occurrence will be created for each prototype. Those occurrences won't appear in the hierarchy, and so deleting one of the part in the scene has no risks of singularizing. If set to False, an arbitrary occurrence will be used as the prototype for other similar occurrences, which is less safe but will result in less occurrences.</param>
		public static void CreateInstancesBySimilarity(Scene.Native.OccurrenceList occurrences, System.Double dimensionsSimilarity, System.Double polycountSimilarity, System.Boolean ignoreSymmetry, System.Boolean keepExistingPrototypes, System.Boolean createNewOccurrencesForPrototypes) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_createInstancesBySimilarity(occurrences_c, dimensionsSimilarity, polycountSimilarity, ignoreSymmetry ? 1 : 0, keepExistingPrototypes ? 1 : 0, createNewOccurrencesForPrototypes ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_createNormals(Scene.Native.OccurrenceList_c occurrences, System.Double sharpEdge, Int32 overriding, Int32 useAreaWeighting);
		/// <summary>
		/// Create normal attributes on tessellations
		/// </summary>
		/// <param name="occurrences">Occurrences of components to create attributes</param>
		/// <param name="sharpEdge">Edges with an angle between their polygons greater than sharpEdge will be considered sharp (default use the Pixyz sharpAngle parameter)</param>
		/// <param name="overriding">If true, override existing normals, else only create normals on meshes without normals</param>
		/// <param name="useAreaWeighting">If true, normal computation will be weighted using polygon areas</param>
		public static void CreateNormals(Scene.Native.OccurrenceList occurrences, System.Double sharpEdge, System.Boolean overriding, System.Boolean useAreaWeighting) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_createNormals(occurrences_c, sharpEdge, overriding ? 1 : 0, useAreaWeighting ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Algo_createOcclusionMesh(Scene.Native.OccurrenceList_c occurrences, Int32 type, System.Double voxelSize, Int32 gap);
		/// <summary>
		/// Compute an occluder or an occludee with the occurrences selected
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="type">Type of what we create</param>
		/// <param name="voxelSize">Size of voxels</param>
		/// <param name="gap">Dilation iterations on the voxel grid</param>
		public static System.UInt32 CreateOcclusionMesh(Scene.Native.OccurrenceList occurrences, CreateOccluder type, System.Double voxelSize, System.Int32 gap) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_createOcclusionMesh(occurrences_c, (int)type, voxelSize, gap);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_createTangents(Scene.Native.OccurrenceList_c occurrences, System.Double sharpEdge, Int32 uvChannel, Int32 overriding);
		/// <summary>
		/// Create tangent attributes on tessellations
		/// </summary>
		/// <param name="occurrences">Occurrences of components to create attributes</param>
		/// <param name="sharpEdge">Edges with an angle between their polygons greater than sharpEdge will be considered sharp (default use the Pixyz sharpAngle parameter)</param>
		/// <param name="uvChannel">UV channel to use for the tangents creation</param>
		/// <param name="overriding">If true, override existing tangents, else only create tangents on meshes without tangents</param>
		public static void CreateTangents(Scene.Native.OccurrenceList occurrences, System.Double sharpEdge, System.Int32 uvChannel, System.Boolean overriding) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_createTangents(occurrences_c, sharpEdge, uvChannel, overriding ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_createVisibilityPatchesFromPatch(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Create visibility patches from existing patches
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		public static void CreateVisibilityPatchesFromPatch(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_createVisibilityPatchesFromPatch(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_debug(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Debug function
		/// </summary>
		/// <param name="occurrences">Occurrences of components to check</param>
		public static void Debug(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_debug(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_decimate(Scene.Native.OccurrenceList_c occurrences, System.Double surfacicTolerance, System.Double lineicTolerance, System.Double normalTolerance, System.Double texCoordTolerance, Int32 releaseConstraintOnSmallArea);
		/// <summary>
		/// reduce the polygon count by removing some vertices
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="surfacicTolerance">Maximum distance between surfacic vertices and resulting simplified surfaces</param>
		/// <param name="lineicTolerance">Maximum distance between lineic vertices and resulting simplified lines</param>
		/// <param name="normalTolerance">Maximum angle between original normals and those interpolated on the simplified surface</param>
		/// <param name="texCoordTolerance">Maximum distance (in UV space) between original texcoords and those interpolated on the simplified surface</param>
		/// <param name="releaseConstraintOnSmallArea">If True, release constraint of normal and/or texcoord tolerance on small areas (according to surfacicTolerance)</param>
		public static void Decimate(Scene.Native.OccurrenceList occurrences, System.Double surfacicTolerance, System.Double lineicTolerance, System.Double normalTolerance, System.Double texCoordTolerance, System.Boolean releaseConstraintOnSmallArea) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_decimate(occurrences_c, surfacicTolerance, lineicTolerance, normalTolerance, texCoordTolerance, releaseConstraintOnSmallArea ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_decimateEdgeCollapse(Scene.Native.OccurrenceList_c occurrences, System.Double surfacicTolerance, System.Double boundaryWeight, System.Double normalWeight, System.Double UVWeight, System.Double sharpNormalWeight, System.Double UVSeamWeight, System.Double normalMaxDeviation, Int32 forbidUVOverlaps, System.Double UVMaxDeviation, System.Double UVSeamMaxDeviation, Int32 protectTopology, Int32 qualityTradeoff);
		/// <summary>
		/// reduce the polygon count by collapsing some edges to obtain an simplified mesh
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="surfacicTolerance">Error max between the simplified mesh et the old one</param>
		/// <param name="boundaryWeight">Boundary importance during the decimation</param>
		/// <param name="normalWeight">Normal importance during the decimation</param>
		/// <param name="UVWeight">UV importance during the decimation</param>
		/// <param name="sharpNormalWeight">Importance of sharp edges during the decimation</param>
		/// <param name="UVSeamWeight">Importance of UV seams during the decimation</param>
		/// <param name="normalMaxDeviation">Constraint the normals deviation on decimated model</param>
		/// <param name="forbidUVOverlaps">Forbid UV to fold over and overlap during the decimation</param>
		/// <param name="UVMaxDeviation">Constraint the uv deviation on decimated model</param>
		/// <param name="UVSeamMaxDeviation">Constraint the uv seams deviation on decimated model</param>
		/// <param name="protectTopology">If false, the topology of the mesh can change and some edges can become non-manifold. But the visual quality will be better on model with complex topology</param>
		/// <param name="qualityTradeoff">For big models it is recommanded to choose PreferSpeed tradeoff. In PreferSpeed mode, quadrics are computed only on position (and not on other vertex attributes)</param>
		public static void DecimateEdgeCollapse(Scene.Native.OccurrenceList occurrences, System.Double surfacicTolerance, System.Double boundaryWeight, System.Double normalWeight, System.Double UVWeight, System.Double sharpNormalWeight, System.Double UVSeamWeight, System.Double normalMaxDeviation, System.Boolean forbidUVOverlaps, System.Double UVMaxDeviation, System.Double UVSeamMaxDeviation, System.Boolean protectTopology, QualitySpeedTradeoff qualityTradeoff) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_decimateEdgeCollapse(occurrences_c, surfacicTolerance, boundaryWeight, normalWeight, UVWeight, sharpNormalWeight, UVSeamWeight, normalMaxDeviation, forbidUVOverlaps ? 1 : 0, UVMaxDeviation, UVSeamMaxDeviation, protectTopology ? 1 : 0, (int)qualityTradeoff);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_decimatePointClouds(Scene.Native.OccurrenceList_c occurrences, System.Double tolerance);
		/// <summary>
		/// decimate Point Cloud Occurences according to tolerance 
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="tolerance">Avarage distance between points</param>
		public static void DecimatePointClouds(Scene.Native.OccurrenceList occurrences, System.Double tolerance) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_decimatePointClouds(occurrences_c, tolerance);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_decimateTarget(Scene.Native.OccurrenceList_c occurrences, DecimateOptionsSelector_c targetStrategy, Int32 UVImportance, Int32 protectTopology, Int32 iterativeThreshold);
		/// <summary>
		/// reduce the polygon count by collapsing some edges to obtain a target triangle count (iterative version that use less memory)
		/// </summary>
		/// <param name="occurrences">List of occurrences to process</param>
		/// <param name="targetStrategy">Select between targetCount or ratio to define the number of triangles left after the decimation process</param>
		/// <param name="UVImportance">Select importance of texture coordinates</param>
		/// <param name="protectTopology">If False, the topology of the mesh can change and some edges can become non-manifold</param>
		/// <param name="iterativeThreshold">Number of triangles above which the iterative algorithm is used to limit the memory usage</param>
		public static void DecimateTarget(Scene.Native.OccurrenceList occurrences, DecimateOptionsSelector targetStrategy, UVImportanceEnum UVImportance, System.Boolean protectTopology, System.Int32 iterativeThreshold) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var targetStrategy_c = new Algo.Native.DecimateOptionsSelector_c();
			ConvertValue(targetStrategy, ref targetStrategy_c);
			Algo_decimateTarget(occurrences_c, targetStrategy_c, (int)UVImportance, protectTopology ? 1 : 0, iterativeThreshold);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Algo.Native.NativeInterface.Algo_DecimateOptionsSelector_free(ref targetStrategy_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_decimateTargetLODChain(Scene.Native.OccurrenceList_c occurrences, Core.Native.IntList_c targetTriangleCounts, Int32 useVertexWeigths, System.Double boundaryWeight, System.Double normalWeight, System.Double UVWeight, System.Double sharpNormalWeight, System.Double UVSeamWeight, Int32 forbidUVFoldovers, Int32 protectTopology, Int32 qualityTradeoff);
		/// <summary>
		/// Create LOD using decimate target
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="targetTriangleCounts">Target triangle count for each LOD</param>
		/// <param name="useVertexWeigths">Use vertex importance weights if any</param>
		/// <param name="boundaryWeight">Boundary importance during the decimation</param>
		/// <param name="normalWeight">Normal importance during the decimation</param>
		/// <param name="UVWeight">UV importance during the decimation</param>
		/// <param name="sharpNormalWeight">Importance of sharp edges during the decimation</param>
		/// <param name="UVSeamWeight">Importance of UV seams during the decimation</param>
		/// <param name="forbidUVFoldovers">Forbid UV to fold over and overlap during the decimation</param>
		/// <param name="protectTopology">If false, the topology of the mesh can change and some edges can become non-manifold. But the visual quality will be better on model with complex topology</param>
		/// <param name="qualityTradeoff">For big models it is recommanded to choose PreferSpeed tradeoff. In PreferSpeed mode, quadrics are computed only on position (and not on other vertex attributes)</param>
		public static void DecimateTargetLODChain(Scene.Native.OccurrenceList occurrences, Core.Native.IntList targetTriangleCounts, System.Boolean useVertexWeigths, System.Double boundaryWeight, System.Double normalWeight, System.Double UVWeight, System.Double sharpNormalWeight, System.Double UVSeamWeight, System.Boolean forbidUVFoldovers, System.Boolean protectTopology, QualitySpeedTradeoff qualityTradeoff) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var targetTriangleCounts_c = new Core.Native.IntList_c();
			Core.Native.NativeInterface.ConvertValue(targetTriangleCounts, ref targetTriangleCounts_c);
			Algo_decimateTargetLODChain(occurrences_c, targetTriangleCounts_c, useVertexWeigths ? 1 : 0, boundaryWeight, normalWeight, UVWeight, sharpNormalWeight, UVSeamWeight, forbidUVFoldovers ? 1 : 0, protectTopology ? 1 : 0, (int)qualityTradeoff);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Core.Native.NativeInterface.Core_IntList_free(ref targetTriangleCounts_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_decimateTargetOldSchool(Scene.Native.OccurrenceList_c occurrences, DecimateOptionsSelector_c TargetStrategy, System.Double boundaryWeight, System.Double normalWeight, System.Double UVWeight, System.Double sharpNormalWeight, System.Double UVSeamWeight, Int32 forbidUVFoldovers, Int32 protectTopology, Int32 qualityTradeoff, Int32 useStatic);
		/// <summary>
		/// reduce the polygon count by collapsing some edges to obtain a target triangle count
		/// </summary>
		/// <param name="occurrences">List of occurrences to process</param>
		/// <param name="TargetStrategy">Select between targetCount or ratio to define the number of triangles left after the decimation process</param>
		/// <param name="boundaryWeight">Defines how important the edges defining the mesh boundaries (free edges) are during the decimation process, to preserve them from distortion</param>
		/// <param name="normalWeight">Defines how important vertex normals are during the decimation process, to preserve the smoothing of the mesh from being damaged</param>
		/// <param name="UVWeight">Defines how important UVs (texture coordinates) are during the decimation process, to preserve them from being distorted (along with the textures using the UVs)</param>
		/// <param name="sharpNormalWeight">Defines how important sharp edges (or hard edges) are during the decimation process, to preserve them from being distorted</param>
		/// <param name="UVSeamWeight">Defines how important UV seams (UV islands contours) are during the decimation process, to preserve them from being distorted (along with the textures using the UVs)</param>
		/// <param name="forbidUVFoldovers">Forbids UVs to fold over and overlap each other during the decimation</param>
		/// <param name="protectTopology">If False, the topology of the mesh can change and some edges can become non-manifold; but the visual quality will be better on model with complex topology</param>
		/// <param name="qualityTradeoff">For big models it is recommanded to choose PreferSpeed tradeoff. In PreferSpeed mode, quadrics are computed only on position (and not on other vertex attributes)</param>
		/// <param name="useStatic">Use static mesh method</param>
		public static void DecimateTargetOldSchool(Scene.Native.OccurrenceList occurrences, DecimateOptionsSelector TargetStrategy, System.Double boundaryWeight, System.Double normalWeight, System.Double UVWeight, System.Double sharpNormalWeight, System.Double UVSeamWeight, System.Boolean forbidUVFoldovers, System.Boolean protectTopology, QualitySpeedTradeoff qualityTradeoff, System.Boolean useStatic) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var TargetStrategy_c = new Algo.Native.DecimateOptionsSelector_c();
			ConvertValue(TargetStrategy, ref TargetStrategy_c);
			Algo_decimateTargetOldSchool(occurrences_c, TargetStrategy_c, boundaryWeight, normalWeight, UVWeight, sharpNormalWeight, UVSeamWeight, forbidUVFoldovers ? 1 : 0, protectTopology ? 1 : 0, (int)qualityTradeoff, useStatic ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Algo.Native.NativeInterface.Algo_DecimateOptionsSelector_free(ref TargetStrategy_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_defeatureCAD(Scene.Native.OccurrenceList_c occurrences, System.Double maxDiameter);
		/// <summary>
		/// Remove some features from brep
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="maxDiameter">Maximum diameter of the holes to be removed (-1=no max diameter)</param>
		public static void DefeatureCAD(Scene.Native.OccurrenceList occurrences, System.Double maxDiameter) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_defeatureCAD(occurrences_c, maxDiameter);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_deleteAttibute(System.UInt32 occurrence, Int32 type);
		/// <summary>
		/// Delete designed attribute on tessellations
		/// </summary>
		/// <param name="occurrence">Occurrence to detele attribute from</param>
		/// <param name="type">Attribute type</param>
		public static void DeleteAttibute(System.UInt32 occurrence, AttributType type) {
			Algo_deleteAttibute(occurrence, (int)type);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_deleteBRepShapes(Scene.Native.OccurrenceList_c occurrences, Int32 onlyTessellated);
		/// <summary>
		/// Delete BRep representation on parts
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="onlyTessellated">If True, delete only BRep represensation on part with a tessellated shape</param>
		public static void DeleteBRepShapes(Scene.Native.OccurrenceList occurrences, System.Boolean onlyTessellated) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_deleteBRepShapes(occurrences_c, onlyTessellated ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_deleteFreeVertices(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Delete all free vertices of the mesh of given parts
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		public static void DeleteFreeVertices(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_deleteFreeVertices(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_deleteLines(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Delete all free line of the mesh of given parts
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		public static void DeleteLines(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_deleteLines(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_deleteNormals(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Remove normal attributes on tessellations
		/// </summary>
		/// <param name="occurrences">Occurrences of components to delete</param>
		public static void DeleteNormals(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_deleteNormals(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_deletePatches(Scene.Native.OccurrenceList_c occurrences, Int32 keepOnePatchByMaterial);
		/// <summary>
		/// Delete patches attributes on tessellations
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="keepOnePatchByMaterial">If set, one patch by material will be kept, else all patches will be deleted and materials on patches will be lost</param>
		public static void DeletePatches(Scene.Native.OccurrenceList occurrences, System.Boolean keepOnePatchByMaterial) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_deletePatches(occurrences_c, keepOnePatchByMaterial ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_deletePolygons(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Delete all polygons of the mesh of given parts
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		public static void DeletePolygons(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_deletePolygons(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_deleteTangents(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Remove tangent attributes on tessellations
		/// </summary>
		/// <param name="occurrences">Occurrences of components to delete</param>
		public static void DeleteTangents(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_deleteTangents(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_deleteTextureCoordinates(Scene.Native.OccurrenceList_c occurrences, Int32 channel);
		/// <summary>
		/// Delete texture coordinates on tessellations
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="channel">Choose Texture coordinates channel to delete (-1 for all channels)</param>
		public static void DeleteTextureCoordinates(Scene.Native.OccurrenceList occurrences, System.Int32 channel) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_deleteTextureCoordinates(occurrences_c, channel);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_deleteVisibilityPatches(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// delete the visibility patches of given occurrences
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		public static void DeleteVisibilityPatches(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_deleteVisibilityPatches(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.Double Algo_evalDecimateErrorForTarget(Scene.Native.OccurrenceList_c occurrences, DecimateOptionsSelector_c TargetStrategy, System.Double boundaryWeight, System.Double normalWeight, System.Double UVWeight, System.Double sharpNormalWeight, System.Double UVSeamWeight, Int32 forbidUVFoldovers, Int32 protectTopology);
		/// <summary>
		/// returns the max error to set to reach a given target
		/// </summary>
		/// <param name="occurrences">List of occurrences to process</param>
		/// <param name="TargetStrategy">Select between targetCount or ratio to define the number of triangles left after the decimation process</param>
		/// <param name="boundaryWeight">Defines how important the edges defining the mesh boundaries (free edges) are during the decimation process, to preserve them from distortion</param>
		/// <param name="normalWeight">Defines how important vertex normals are during the decimation process, to preserve the smoothing of the mesh from being damaged</param>
		/// <param name="UVWeight">Defines how important UVs (texture coordinates) are during the decimation process, to preserve them from being distorted (along with the textures using the UVs)</param>
		/// <param name="sharpNormalWeight">Defines how important sharp edges (or hard edges) are during the decimation process, to preserve them from being distorted</param>
		/// <param name="UVSeamWeight">Defines how important UV seams (UV islands contours) are during the decimation process, to preserve them from being distorted (along with the textures using the UVs)</param>
		/// <param name="forbidUVFoldovers">Forbids UVs to fold over and overlap each other during the decimation</param>
		/// <param name="protectTopology">If False, the topology of the mesh can change and some edges can become non-manifold; but the visual quality will be better on model with complex topology</param>
		public static System.Double EvalDecimateErrorForTarget(Scene.Native.OccurrenceList occurrences, DecimateOptionsSelector TargetStrategy, System.Double boundaryWeight, System.Double normalWeight, System.Double UVWeight, System.Double sharpNormalWeight, System.Double UVSeamWeight, System.Boolean forbidUVFoldovers, System.Boolean protectTopology) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var TargetStrategy_c = new Algo.Native.DecimateOptionsSelector_c();
			ConvertValue(TargetStrategy, ref TargetStrategy_c);
			var ret = Algo_evalDecimateErrorForTarget(occurrences_c, TargetStrategy_c, boundaryWeight, normalWeight, UVWeight, sharpNormalWeight, UVSeamWeight, forbidUVFoldovers ? 1 : 0, protectTopology ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Algo.Native.NativeInterface.Algo_DecimateOptionsSelector_free(ref TargetStrategy_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Double)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_explodeBodies(Scene.Native.OccurrenceList_c occurrences, Int32 groupOpenShells);
		/// <summary>
		/// Explode all CAD Parts by body
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="groupOpenShells">Group all open shells in one part</param>
		public static void ExplodeBodies(Scene.Native.OccurrenceList occurrences, System.Boolean groupOpenShells) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_explodeBodies(occurrences_c, groupOpenShells ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_explodeConnectedMeshes(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Explode connected set of polygons to parts
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		public static void ExplodeConnectedMeshes(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_explodeConnectedMeshes(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_explodePartByMaterials(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Explode all parts by material
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		public static void ExplodePartByMaterials(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_explodePartByMaterials(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_explodePatches(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Explode all parts by patch
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		public static void ExplodePatches(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_explodePatches(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_explodeVertexCount(Scene.Native.OccurrenceList_c occurrences, Int32 maxVertexCount, Int32 maxTriangleCount, Int32 countMergedVerticesOnce);
		/// <summary>
		/// Explode parts to respect a maximum vertex count
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="maxVertexCount">The maximum number of vertices by part</param>
		/// <param name="maxTriangleCount">The maximum number of triangles by part (quadrangles count twice)</param>
		/// <param name="countMergedVerticesOnce">If true, one vertex used in several triangles with different normals will be counted once (for Unity must be False)</param>
		public static void ExplodeVertexCount(Scene.Native.OccurrenceList occurrences, System.Int32 maxVertexCount, System.Int32 maxTriangleCount, System.Boolean countMergedVerticesOnce) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_explodeVertexCount(occurrences_c, maxVertexCount, maxTriangleCount, countMergedVerticesOnce ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_explodeVoxel(Scene.Native.OccurrenceList_c occurrences, System.Double voxelSize);
		/// <summary>
		/// Explode parts by voxel
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="voxelSize">Voxel size</param>
		public static void ExplodeVoxel(Scene.Native.OccurrenceList occurrences, System.Double voxelSize) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_explodeVoxel(occurrences_c, voxelSize);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_extractNeutralAxis(Scene.Native.OccurrenceList_c occurrences, System.Double maxDiameter, Int32 removeOriginalMesh);
		/// <summary>
		/// Extract neutral axis from tessellations
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="maxDiameter">Maximum diameter of beams</param>
		/// <param name="removeOriginalMesh">Remove or not the original mesh at the end of the algorithm</param>
		public static void ExtractNeutralAxis(Scene.Native.OccurrenceList occurrences, System.Double maxDiameter, System.Boolean removeOriginalMesh) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_extractNeutralAxis(occurrences_c, maxDiameter, removeOriginalMesh ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Polygonal.Native.TessellationList_c Algo_getTessellations(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// returns all the tessellation of the given occurrences (only returns editable mesh, see algo.toEditableMesh)
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		public static Polygonal.Native.TessellationList GetTessellations(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_getTessellations(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Polygonal.Native.NativeInterface.ConvertValue(ref ret);
			Polygonal.Native.NativeInterface.Polygonal_TessellationList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getVisibilityStatsReturn_c Algo_getVisibilityStats(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// returns the visibility statistics for some occurrences
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		public static Algo.Native.getVisibilityStatsReturn GetVisibilityStats(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_getVisibilityStats(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			Algo.Native.getVisibilityStatsReturn retStruct = new Algo.Native.getVisibilityStatsReturn();
			retStruct.visibleCountFront = (System.Int32)ret.visibleCountFront;
			retStruct.visibleCountBack = (System.Int32)ret.visibleCountBack;
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Algo_getVoxelTessellation(Core.Native.DoubleList_c voxels, System.Double threshold);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="voxels"></param>
		/// <param name="threshold"></param>
		public static System.UInt32 GetVoxelTessellation(Core.Native.DoubleList voxels, System.Double threshold) {
			var voxels_c = new Core.Native.DoubleList_c();
			Core.Native.NativeInterface.ConvertValue(voxels, ref voxels_c);
			var ret = Algo_getVoxelTessellation(voxels_c, threshold);
			Core.Native.NativeInterface.Core_DoubleList_free(ref voxels_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_identifyPatches(Scene.Native.OccurrenceList_c occurrences, Int32 useAttributesFilter, Int32 useSharpEdgeFilter, System.Double sharpAngle, Int32 useBoundaryFilter, Int32 useNonManifoldFilter, Int32 useLineEdgeFilter, Int32 useQuadLineFilter);
		/// <summary>
		/// Create cad patches on tessellation (needed by some functions)
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="useAttributesFilter">Filters by attributes</param>
		/// <param name="useSharpEdgeFilter">Filters by edge sharpness</param>
		/// <param name="sharpAngle">Sharp angle in degree, if negative the default sharp angle value is used</param>
		/// <param name="useBoundaryFilter">Filters by boundaries</param>
		/// <param name="useNonManifoldFilter">Filters by manifold-ness</param>
		/// <param name="useLineEdgeFilter">Filters by edge</param>
		/// <param name="useQuadLineFilter">Filters by quad lines</param>
		public static void IdentifyPatches(Scene.Native.OccurrenceList occurrences, System.Boolean useAttributesFilter, System.Boolean useSharpEdgeFilter, System.Double sharpAngle, System.Boolean useBoundaryFilter, System.Boolean useNonManifoldFilter, System.Boolean useLineEdgeFilter, System.Boolean useQuadLineFilter) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_identifyPatches(occurrences_c, useAttributesFilter ? 1 : 0, useSharpEdgeFilter ? 1 : 0, sharpAngle, useBoundaryFilter ? 1 : 0, useNonManifoldFilter ? 1 : 0, useLineEdgeFilter ? 1 : 0, useQuadLineFilter ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_lineToCylinderCAD(Scene.Native.OccurrenceList_c occurrences, System.Double radius);
		/// <summary>
		/// Change lines into cylinders
		/// </summary>
		/// <param name="occurrences">Occurrence of components to reverse</param>
		/// <param name="radius">Size of the radius of the cylinders</param>
		public static void LineToCylinderCAD(Scene.Native.OccurrenceList occurrences, System.Double radius) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_lineToCylinderCAD(occurrences_c, radius);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_lineToTexture(Scene.Native.OccurrenceList_c lines, UseColorOption_c useColor, Int32 resolution, Int32 thickness);
		/// <summary>
		/// Generate a textured quadrangle over an existing mesh of coplanar lines
		/// </summary>
		/// <param name="lines">Lines to select</param>
		/// <param name="useColor">Set color policy.</param>
		/// <param name="resolution">Texture resolution</param>
		/// <param name="thickness">The thickness of the lines in pixels</param>
		public static void LineToTexture(Scene.Native.OccurrenceList lines, UseColorOption useColor, System.Int32 resolution, System.Int32 thickness) {
			var lines_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(lines, ref lines_c);
			var useColor_c = new Algo.Native.UseColorOption_c();
			ConvertValue(useColor, ref useColor_c);
			Algo_lineToTexture(lines_c, useColor_c, resolution, thickness);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref lines_c);
			Algo.Native.NativeInterface.Algo_UseColorOption_free(ref useColor_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern OccurrenceFeaturesList_c Algo_listFeatures(Scene.Native.OccurrenceList_c occurrences, Int32 throughHoles, Int32 blindHoles, System.Double maxDiameter);
		/// <summary>
		/// List features from tessellations
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="throughHoles">List through holes</param>
		/// <param name="blindHoles">List blind holes</param>
		/// <param name="maxDiameter">Maximum diameter of the holes to be list (-1=no max diameter)</param>
		public static OccurrenceFeaturesList ListFeatures(Scene.Native.OccurrenceList occurrences, System.Boolean throughHoles, System.Boolean blindHoles, System.Double maxDiameter) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_listFeatures(occurrences_c, throughHoles ? 1 : 0, blindHoles ? 1 : 0, maxDiameter);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Algo.Native.NativeInterface.Algo_OccurrenceFeaturesList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Algo_marchingCubes(Scene.Native.OccurrenceList_c occurrences, System.Double voxelSize, Int32 elements, Int32 dilation, Int32 surfacic);
		/// <summary>
		/// Replace the tessellations of the selected parts by a marching cube representation
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="voxelSize">Size of voxels</param>
		/// <param name="elements">Type of elements used to detect the voxels, polygons or points or hybrid</param>
		/// <param name="dilation">Dilation iterations on the voxel grid (only if surfacic=false)</param>
		/// <param name="surfacic">Prefer this mode if the source is surfacic, the result is not guaranteed to be watertight</param>
		public static System.UInt32 MarchingCubes(Scene.Native.OccurrenceList occurrences, System.Double voxelSize, ElementFilter elements, System.Int32 dilation, System.Boolean surfacic) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_marchingCubes(occurrences_c, voxelSize, (int)elements, dilation, surfacic ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_mergeVertices(Scene.Native.OccurrenceList_c occurrences, System.Double maxDistance, Polygonal.Native.TopologyCategoryMask_c mask);
		/// <summary>
		/// merge near vertices according to the given distance
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="maxDistance">Maximum distance between two vertex to merge</param>
		/// <param name="mask">Topological category of the vertices to merge</param>
		public static void MergeVertices(Scene.Native.OccurrenceList occurrences, System.Double maxDistance, Polygonal.Native.TopologyCategoryMask mask) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var mask_c = new Polygonal.Native.TopologyCategoryMask_c();
			Polygonal.Native.NativeInterface.ConvertValue(mask, ref mask_c);
			Algo_mergeVertices(occurrences_c, maxDistance, mask_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Polygonal.Native.NativeInterface.Polygonal_TopologyCategoryMask_free(ref mask_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_noiseMesh(Scene.Native.OccurrenceList_c occurrences, System.Double maxAmplitude);
		/// <summary>
		/// Apply noise to vertex positions along their normals
		/// </summary>
		/// <param name="occurrences">Occurrences of parts to noise</param>
		/// <param name="maxAmplitude">Maximum distance between original vertex and noisy vertex</param>
		public static void NoiseMesh(Scene.Native.OccurrenceList occurrences, System.Double maxAmplitude) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_noiseMesh(occurrences_c, maxAmplitude);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_optimizeCADLoops(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Optimize CAD Face loops by merging useless loop edges
		/// </summary>
		/// <param name="occurrences">Occurrences of components to optimize</param>
		public static void OptimizeCADLoops(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_optimizeCADLoops(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_optimizeForRendering(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Optimize mesh for rendering (lossless, only reindexing)
		/// </summary>
		/// <param name="occurrences">Occurrences of components to optimize</param>
		public static void OptimizeForRendering(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_optimizeForRendering(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_optimizeSubMeshes(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Sort sub meshes by materials
		/// </summary>
		/// <param name="occurrences">Occurrences of parts to process</param>
		public static void OptimizeSubMeshes(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_optimizeSubMeshes(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_optimizeTextureSize(System.UInt32 root, System.Double texelPerMm);
		/// <summary>
		/// Resizes scene textures based on a number of texels per 3D space units (e.g: mm)
		/// </summary>
		/// <param name="root">Root from which texture resizing will process</param>
		/// <param name="texelPerMm">Number of texel per millimeter in a 3D space</param>
		public static void OptimizeTextureSize(System.UInt32 root, System.Double texelPerMm) {
			Algo_optimizeTextureSize(root, texelPerMm);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_patchToSurface(Scene.Native.OccurrenceList_c occurrences, Int32 quality, Int32 useNormals, Int32 useGuides);
		/// <summary>
		/// Create a CAD representation from the tessellated representation patches for each given part
		/// </summary>
		/// <param name="occurrences">Occurrences of components to reverse</param>
		/// <param name="quality">Method to use</param>
		/// <param name="useNormals">Use normals to approximate</param>
		/// <param name="useGuides">Use guides to approximate</param>
		public static void PatchToSurface(Scene.Native.OccurrenceList occurrences, ComputingQuality quality, System.Boolean useNormals, System.Boolean useGuides) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_patchToSurface(occurrences_c, (int)quality, useNormals ? 1 : 0, useGuides ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Algo_proxyMesh(Scene.Native.OccurrenceList_c occurrences, System.Double voxelSize, Int32 elements, Int32 dilation, Int32 surfacic);
		/// <summary>
		/// Replace the tessellations of the selected parts by a proxy mesh based on a voxelization
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="voxelSize">Size of voxels</param>
		/// <param name="elements">Type of elements used to detect the voxels, polygons or points or hybrid</param>
		/// <param name="dilation">Dilation iterations on the voxel grid (only if surfacic=false)</param>
		/// <param name="surfacic">Prefer this mode if the source is surfacic, the result is not guaranteed to be watertight</param>
		public static System.UInt32 ProxyMesh(Scene.Native.OccurrenceList occurrences, System.Double voxelSize, ElementFilter elements, System.Int32 dilation, System.Boolean surfacic) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_proxyMesh(occurrences_c, voxelSize, (int)elements, dilation, surfacic ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_removeHoles(Scene.Native.OccurrenceList_c occurrences, Int32 throughHoles, Int32 blindHoles, Int32 surfacicHoles, System.Double maxDiameter, System.UInt32 fillWithMaterial);
		/// <summary>
		/// Remove some features from tessellations
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="throughHoles">Remove through holes</param>
		/// <param name="blindHoles">Remove blind holes</param>
		/// <param name="surfacicHoles">Remove surfacic holes</param>
		/// <param name="maxDiameter">Maximum diameter of the holes to be removed (-1=no max diameter)</param>
		/// <param name="fillWithMaterial">If set, the given material will be used to fill the holes</param>
		public static void RemoveHoles(Scene.Native.OccurrenceList occurrences, System.Boolean throughHoles, System.Boolean blindHoles, System.Boolean surfacicHoles, System.Double maxDiameter, System.UInt32 fillWithMaterial) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_removeHoles(occurrences_c, throughHoles ? 1 : 0, blindHoles ? 1 : 0, surfacicHoles ? 1 : 0, maxDiameter, fillWithMaterial);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_repairCAD(Scene.Native.OccurrenceList_c occurrences, System.Double tolerance, Int32 orient);
		/// <summary>
		/// Repair CAD shapes, assemble faces, remove duplicated faces, optimize loops and repair topology
		/// </summary>
		/// <param name="occurrences">Occurrences of components to clean</param>
		/// <param name="tolerance">Tolerance</param>
		/// <param name="orient">If true reorient the model</param>
		public static void RepairCAD(Scene.Native.OccurrenceList occurrences, System.Double tolerance, System.Boolean orient) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_repairCAD(occurrences_c, tolerance, orient ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_replaceBy(Scene.Native.OccurrenceList_c occurrences, ReplaceByOption_c replaceBy);
		/// <summary>
		/// Replace geometries by other shapes, or primitives
		/// </summary>
		/// <param name="occurrences">Occurrences of components to replace</param>
		/// <param name="replaceBy">Shape replacement option</param>
		public static void ReplaceBy(Scene.Native.OccurrenceList occurrences, ReplaceByOption replaceBy) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var replaceBy_c = new Algo.Native.ReplaceByOption_c();
			ConvertValue(replaceBy, ref replaceBy_c);
			Algo_replaceBy(occurrences_c, replaceBy_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Algo.Native.NativeInterface.Algo_ReplaceByOption_free(ref replaceBy_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_replaceByBox(Scene.Native.OccurrenceList_c occurrences, Int32 boxType);
		/// <summary>
		/// Replace objects by a bounding box
		/// </summary>
		/// <param name="occurrences">Occurrences of components to replace</param>
		/// <param name="boxType">Bounding box type, oriented, axis-aligned, ...</param>
		public static void ReplaceByBox(Scene.Native.OccurrenceList occurrences, ReplaceByBoxType boxType) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_replaceByBox(occurrences_c, (int)boxType);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_replaceByConvexHull(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Replace objects by convex hull
		/// </summary>
		/// <param name="occurrences">Occurrences of components to replace</param>
		public static void ReplaceByConvexHull(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_replaceByConvexHull(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_replaceByPrimitive(Scene.Native.OccurrenceList_c occurrences, PrimitiveShapeParameters_c primitive, Int32 generateUV);
		/// <summary>
		/// Replace objects by a primitive shapes
		/// </summary>
		/// <param name="occurrences">Occurrences of components to replace</param>
		/// <param name="primitive">Primitive type and parameters</param>
		/// <param name="generateUV">Primitive type and parameters</param>
		public static void ReplaceByPrimitive(Scene.Native.OccurrenceList occurrences, PrimitiveShapeParameters primitive, System.Boolean generateUV) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var primitive_c = new Algo.Native.PrimitiveShapeParameters_c();
			ConvertValue(primitive, ref primitive_c);
			Algo_replaceByPrimitive(occurrences_c, primitive_c, generateUV ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Algo.Native.NativeInterface.Algo_PrimitiveShapeParameters_free(ref primitive_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_retessellate(Scene.Native.OccurrenceList_c occurrences, System.Double maxSag, System.Double maxLength, System.Double maxAngle, Int32 createNormals, Int32 uvMode, Int32 uvChannel, System.Double uvPadding, Int32 createTangents, Int32 createFreeEdges);
		/// <summary>
		/// Update the tessellated representation of each CAD part with new tessellation parameters
		/// </summary>
		/// <param name="occurrences">Occurrences of components to tessellate</param>
		/// <param name="maxSag">Maximum distance between the geometry and the tessellation</param>
		/// <param name="maxLength">Maximum length of elements</param>
		/// <param name="maxAngle">Maximum angle between normals of two adjacent elements</param>
		/// <param name="createNormals">If true, normals will be generated</param>
		/// <param name="uvMode">Select the texture coordinates generation mode</param>
		/// <param name="uvChannel">The UV channel of the generated texture coordinates (if any)</param>
		/// <param name="uvPadding">The UV padding between UV island in UV coordinate space (between 0-1). This parameter is handled as an heuristic so it might not be respected</param>
		/// <param name="createTangents">If true, tangents will be generated</param>
		/// <param name="createFreeEdges">If true, free edges will be created for each patch borders</param>
		public static void Retessellate(Scene.Native.OccurrenceList occurrences, System.Double maxSag, System.Double maxLength, System.Double maxAngle, System.Boolean createNormals, UVGenerationMode uvMode, System.Int32 uvChannel, System.Double uvPadding, System.Boolean createTangents, System.Boolean createFreeEdges) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_retessellate(occurrences_c, maxSag, maxLength, maxAngle, createNormals ? 1 : 0, (int)uvMode, uvChannel, uvPadding, createTangents ? 1 : 0, createFreeEdges ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Algo_retopologize(Scene.Native.OccurrenceList_c occurrences, Int32 targetTriangleCount, Int32 pureQuad, Int32 pointCloud, System.Double precision);
		/// <summary>
		/// Replace the tessellations of the selected parts by a retopology of the external hull
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="targetTriangleCount">Target triangle count</param>
		/// <param name="pureQuad">Retopologize to a pure quad mesh if True, else the resulting mesh will be quad dominant but can contains triangles</param>
		/// <param name="pointCloud">Set to true if occurrences are point cloud, else False</param>
		/// <param name="precision">If set, define the precision of the features to preserve</param>
		public static System.UInt32 Retopologize(Scene.Native.OccurrenceList occurrences, System.Int32 targetTriangleCount, System.Boolean pureQuad, System.Boolean pointCloud, System.Double precision) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_retopologize(occurrences_c, targetTriangleCount, pureQuad ? 1 : 0, pointCloud ? 1 : 0, precision);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Algo_retopologizeWithClusterQuadrics(Scene.Native.OccurrenceList_c occurrences, System.Double voxelSize);
		/// <summary>
		/// Replace the tessellations of the selected parts by a retopology base on cluster quadrics
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="voxelSize">Size of voxels</param>
		public static System.UInt32 RetopologizeWithClusterQuadrics(Scene.Native.OccurrenceList occurrences, System.Double voxelSize) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_retopologizeWithClusterQuadrics(occurrences_c, voxelSize);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_segmentByDistorsion(Scene.Native.OccurrenceList_c occurrences, System.Double localThreshold, System.Double globalThreshold);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="localThreshold"></param>
		/// <param name="globalThreshold"></param>
		public static void SegmentByDistorsion(Scene.Native.OccurrenceList occurrences, System.Double localThreshold, System.Double globalThreshold) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_segmentByDistorsion(occurrences_c, localThreshold, globalThreshold);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_selectSimilar(Scene.Native.OccurrenceList_c occurrences, System.Double dimensionsSimilarity, System.Double polycountSimilarity, Int32 ignoreSymmetry);
		/// <summary>
		/// Selects occurrences in the whole scene that are similar to the selected occurrences. If several occurrences are selected, the selection afterwards will contain similar parts for each input occurrence.
		/// </summary>
		/// <param name="occurrences">Occurrences for which we want to find similar occurrences in the scene.</param>
		/// <param name="dimensionsSimilarity">The percentage of similarity on dimensions. A value of 1.0 (100%) will find parts that have exactly the same dimensions. A lower value will increase the likelyhood to find similar parts, at the cost of precision.</param>
		/// <param name="polycountSimilarity">The percentage of similarity on polycount. A value of 1.0 (100%) will find parts that have exactly the same polycount. A lower value will increase the likelyhood to find similar parts, at the cost of precision.</param>
		/// <param name="ignoreSymmetry">If True, symmetries will be ignored, otherwise negative scaling will be applied in the occurrence transformation.</param>
		public static void SelectSimilar(Scene.Native.OccurrenceList occurrences, System.Double dimensionsSimilarity, System.Double polycountSimilarity, System.Boolean ignoreSymmetry) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_selectSimilar(occurrences_c, dimensionsSimilarity, polycountSimilarity, ignoreSymmetry ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_smoothMesh(Scene.Native.OccurrenceList_c occurrences, Int32 mode, Int32 maxIterations, Int32 lockSignificantEdges);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="mode">Computation of cost</param>
		/// <param name="maxIterations">Maximum number of swapping iteration</param>
		/// <param name="lockSignificantEdges">Forbid to swap significant edges (e.g. UV seams, sharp edges, patch borders, ...)</param>
		public static void SmoothMesh(Scene.Native.OccurrenceList occurrences, CostEvaluation mode, System.Int32 maxIterations, System.Boolean lockSignificantEdges) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_smoothMesh(occurrences_c, (int)mode, maxIterations, lockSignificantEdges ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_sweep(Scene.Native.OccurrenceList_c occurrences, System.Double radius, Int32 sides, Int32 createNormals, Int32 keepLines, Int32 generateUV);
		/// <summary>
		/// Extrudes a circular section along an underlying polyline (curve)
		/// </summary>
		/// <param name="occurrences">Occurrences of components to check</param>
		/// <param name="radius">Radius of cylinders</param>
		/// <param name="sides">Number of points to create cylinders</param>
		/// <param name="createNormals"></param>
		/// <param name="keepLines"></param>
		/// <param name="generateUV"></param>
		public static void Sweep(Scene.Native.OccurrenceList occurrences, System.Double radius, System.Int32 sides, System.Boolean createNormals, System.Boolean keepLines, System.Boolean generateUV) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_sweep(occurrences_c, radius, sides, createNormals ? 1 : 0, keepLines ? 1 : 0, generateUV ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_tessellate(Scene.Native.OccurrenceList_c occurrences, System.Double maxSag, System.Double maxLength, System.Double maxAngle, Int32 createNormals, Int32 uvMode, Int32 uvChannel, System.Double uvPadding, Int32 createTangents, Int32 createFreeEdges, Int32 keepBRepShape, Int32 overrideExistingTessellation);
		/// <summary>
		/// Create a tessellated representation from a CAD representation for each given part
		/// </summary>
		/// <param name="occurrences">Occurrences of components to tessellate</param>
		/// <param name="maxSag">Maximum distance between the geometry and the tessellation</param>
		/// <param name="maxLength">Maximum length of elements</param>
		/// <param name="maxAngle">Maximum angle between normals of two adjacent elements</param>
		/// <param name="createNormals">If true, normals will be generated</param>
		/// <param name="uvMode">Select the texture coordinates generation mode</param>
		/// <param name="uvChannel">The UV channel of the generated texture coordinates (if any)</param>
		/// <param name="uvPadding">The UV padding between UV island in UV coordinate space (between 0-1). This parameter is handled as an heuristic so it might not be respected</param>
		/// <param name="createTangents">If true, tangents will be generated</param>
		/// <param name="createFreeEdges">If true, free edges will be created for each patch borders</param>
		/// <param name="keepBRepShape">If true, BRep shapes will be kept for Back to Brep or Retessellate</param>
		/// <param name="overrideExistingTessellation">If true, already tessellated parts will be re-tessellated</param>
		public static void Tessellate(Scene.Native.OccurrenceList occurrences, System.Double maxSag, System.Double maxLength, System.Double maxAngle, System.Boolean createNormals, UVGenerationMode uvMode, System.Int32 uvChannel, System.Double uvPadding, System.Boolean createTangents, System.Boolean createFreeEdges, System.Boolean keepBRepShape, System.Boolean overrideExistingTessellation) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_tessellate(occurrences_c, maxSag, maxLength, maxAngle, createNormals ? 1 : 0, (int)uvMode, uvChannel, uvPadding, createTangents ? 1 : 0, createFreeEdges ? 1 : 0, keepBRepShape ? 1 : 0, overrideExistingTessellation ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_tessellateRelativelyToAABB(Scene.Native.OccurrenceList_c occurrences, System.Double maxSag, System.Double sagRatio, System.Double maxLength, System.Double maxAngle, Int32 createNormals, Int32 uvMode, Int32 uvChannel, System.Double uvPadding, Int32 createTangents, Int32 createFreeEdges, Int32 keepBRepShape, Int32 overrideExistingTessellation);
		/// <summary>
		/// Creates a tessellated representation from a CAD representation for each given part. It multiplies the length of the diagonal of the bounding box by the sagRatio. If the output value is above maxSag, then maxSag is used as tessellation value. Else if the output value is below maxSag, it is used as tessellation value.
		/// </summary>
		/// <param name="occurrences">Occurrences of components to tessellate</param>
		/// <param name="maxSag">Maximum distance between the geometry and the tessellation</param>
		/// <param name="sagRatio">Maximum ratio distance between the geometry and the tessellation</param>
		/// <param name="maxLength">Maximum length of elements</param>
		/// <param name="maxAngle">Maximum angle between normals of two adjacent elements</param>
		/// <param name="createNormals">If true, normals will be generated</param>
		/// <param name="uvMode">Select the texture coordinates generation mode</param>
		/// <param name="uvChannel">The UV channel of the generated texture coordinates (if any)</param>
		/// <param name="uvPadding">The UV padding between UV island in UV coordinate space (between 0-1). This parameter is handled as an heuristic so it might not be respected</param>
		/// <param name="createTangents">If true, tangents will be generated</param>
		/// <param name="createFreeEdges">If true, free edges will be created for each patch borders</param>
		/// <param name="keepBRepShape">If true, BRep shapes will be kept for Back to Brep or Retessellate</param>
		/// <param name="overrideExistingTessellation">If true, already tessellated parts will be re-tessellated</param>
		public static void TessellateRelativelyToAABB(Scene.Native.OccurrenceList occurrences, System.Double maxSag, System.Double sagRatio, System.Double maxLength, System.Double maxAngle, System.Boolean createNormals, UVGenerationMode uvMode, System.Int32 uvChannel, System.Double uvPadding, System.Boolean createTangents, System.Boolean createFreeEdges, System.Boolean keepBRepShape, System.Boolean overrideExistingTessellation) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_tessellateRelativelyToAABB(occurrences_c, maxSag, sagRatio, maxLength, maxAngle, createNormals ? 1 : 0, (int)uvMode, uvChannel, uvPadding, createTangents ? 1 : 0, createFreeEdges ? 1 : 0, keepBRepShape ? 1 : 0, overrideExistingTessellation ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Algo_voxelize(Scene.Native.OccurrenceList_c occurrences, System.Double voxelSize, Int32 elements, Int32 dilation, Int32 useCurrentAnimationPosition);
		/// <summary>
		/// Replace the tessellations of the selected parts by a voxelization of the external skin
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="voxelSize">Size of voxels</param>
		/// <param name="elements">Type of elements used to detect the voxels, polygons or points or hybrid</param>
		/// <param name="dilation">Dilation iterations on the voxel grid</param>
		/// <param name="useCurrentAnimationPosition">Use the current animation position instead of the t-pose</param>
		public static System.UInt32 Voxelize(Scene.Native.OccurrenceList occurrences, System.Double voxelSize, ElementFilter elements, System.Int32 dilation, System.Boolean useCurrentAnimationPosition) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_voxelize(occurrences_c, voxelSize, (int)elements, dilation, useCurrentAnimationPosition ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_voxelizePointClouds(Scene.Native.OccurrenceList_c occurrences, System.Double voxelSize);
		/// <summary>
		/// Explode point clouds to voxels
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="voxelSize">Size of voxels</param>
		public static void VoxelizePointClouds(Scene.Native.OccurrenceList occurrences, System.Double voxelSize) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_voxelizePointClouds(occurrences_c, voxelSize);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region Baking

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_bakeUV(System.UInt32 source, System.UInt32 destination, Int32 sourceChannel, Int32 destinationChannel, System.Double tolerance);
		/// <summary>
		/// Bake UV from a mesh to another mesh
		/// </summary>
		/// <param name="source">Occurrence of the source mesh</param>
		/// <param name="destination">Occurrence of the destination mesh</param>
		/// <param name="sourceChannel">Source UV channel to bake</param>
		/// <param name="destinationChannel">Destination UV channel to bake to</param>
		/// <param name="tolerance">Tolerance when point is projected on seam (if the model come from a decimation it is recommanded to use the lineic tolerance here)</param>
		public static void BakeUV(System.UInt32 source, System.UInt32 destination, System.Int32 sourceChannel, System.Int32 destinationChannel, System.Double tolerance) {
			Algo_bakeUV(source, destination, sourceChannel, destinationChannel, tolerance);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Algo_combineMeshes(Scene.Native.OccurrenceList_c occurrences, BakeOption_c bakingOptions, Int32 overrideExistingUVs);
		/// <summary>
		/// Combine all given meshes to one mesh with one material (baked)
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="bakingOptions">Baking options</param>
		/// <param name="overrideExistingUVs">If True, overide existing UVs on channel</param>
		public static System.UInt32 CombineMeshes(Scene.Native.OccurrenceList occurrences, BakeOption bakingOptions, System.Boolean overrideExistingUVs) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var bakingOptions_c = new Algo.Native.BakeOption_c();
			ConvertValue(bakingOptions, ref bakingOptions_c);
			var ret = Algo_combineMeshes(occurrences_c, bakingOptions_c, overrideExistingUVs ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Algo.Native.NativeInterface.Algo_BakeOption_free(ref bakingOptions_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		#endregion

		#region Hidden Detection

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_createVisibilityInformation(Scene.Native.OccurrenceList_c occurrences, Int32 level, Int32 resolution, Int32 sphereCount, System.Double fovX, Int32 considerTransparentOpaque);
		/// <summary>
		/// Create visilibity information on parts viewed from a set of camera automatically placed on a sphere around the scene
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="level">Level of parts to remove : Parts, Patches or Polygons</param>
		/// <param name="resolution">Resolution of the visibility viewer</param>
		/// <param name="sphereCount">Segmentation of the sphere sphereCount x sphereCount</param>
		/// <param name="fovX">Horizontal field of view (in degree)</param>
		/// <param name="considerTransparentOpaque">If True, Parts, Patches or Polygons with a transparent appearance are considered as opaque</param>
		public static void CreateVisibilityInformation(Scene.Native.OccurrenceList occurrences, SelectionLevel level, System.Int32 resolution, System.Int32 sphereCount, System.Double fovX, System.Boolean considerTransparentOpaque) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_createVisibilityInformation(occurrences_c, (int)level, resolution, sphereCount, fovX, considerTransparentOpaque ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_createVisibilityInformationFromViewPoints(Scene.Native.OccurrenceList_c occurrences, Geom.Native.Point3List_c cameraPositions, Geom.Native.Point3List_c cameraDirections, Geom.Native.Point3List_c cameraUps, Int32 resolution, System.Double fovX, Int32 considerTransparentOpaque);
		/// <summary>
		/// Create visilibity information on parts viewed from a given set of camera
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="cameraPositions">List of camera positions</param>
		/// <param name="cameraDirections">List of camera directions</param>
		/// <param name="cameraUps">List of camera up vectors</param>
		/// <param name="resolution">Resolution of the visibility viewer</param>
		/// <param name="fovX">Horizontal field of view (in degree)</param>
		/// <param name="considerTransparentOpaque">If True, Parts, Patches or Polygons with a transparent appearance are considered as opaque</param>
		public static void CreateVisibilityInformationFromViewPoints(Scene.Native.OccurrenceList occurrences, Geom.Native.Point3List cameraPositions, Geom.Native.Point3List cameraDirections, Geom.Native.Point3List cameraUps, System.Int32 resolution, System.Double fovX, System.Boolean considerTransparentOpaque) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var cameraPositions_c = new Geom.Native.Point3List_c();
			Geom.Native.NativeInterface.ConvertValue(cameraPositions, ref cameraPositions_c);
			var cameraDirections_c = new Geom.Native.Point3List_c();
			Geom.Native.NativeInterface.ConvertValue(cameraDirections, ref cameraDirections_c);
			var cameraUps_c = new Geom.Native.Point3List_c();
			Geom.Native.NativeInterface.ConvertValue(cameraUps, ref cameraUps_c);
			Algo_createVisibilityInformationFromViewPoints(occurrences_c, cameraPositions_c, cameraDirections_c, cameraUps_c, resolution, fovX, considerTransparentOpaque ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Geom.Native.NativeInterface.Geom_Point3List_free(ref cameraPositions_c);
			Geom.Native.NativeInterface.Geom_Point3List_free(ref cameraDirections_c);
			Geom.Native.NativeInterface.Geom_Point3List_free(ref cameraUps_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Scene.Native.OccurrenceList_c Algo_getHiddenOccurrences(Scene.Native.OccurrenceList_c occurrences, Int32 resolution, Int32 sphereCount, System.Double fovX, Int32 considerTransparentOpaque);
		/// <summary>
		/// Return parts occurrences not viewed from a sphere around the scene
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="resolution">Resolution of the visibility viewer</param>
		/// <param name="sphereCount">Segmentation of the sphere sphereCount x sphereCount</param>
		/// <param name="fovX">Horizontal field of view (in degree)</param>
		/// <param name="considerTransparentOpaque">If True, Parts, Patches or Polygons with a transparent appearance are considered as opaque</param>
		public static Scene.Native.OccurrenceList GetHiddenOccurrences(Scene.Native.OccurrenceList occurrences, System.Int32 resolution, System.Int32 sphereCount, System.Double fovX, System.Boolean considerTransparentOpaque) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_getHiddenOccurrences(occurrences_c, resolution, sphereCount, fovX, considerTransparentOpaque ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Scene.Native.NativeInterface.ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Core.Native.BoolList_c Algo_hiddenRemoval(Scene.Native.OccurrenceList_c occurrences, Int32 level, Int32 resolution, Int32 sphereCount, System.Double fovX, Int32 considerTransparentOpaque, Int32 adjacencyDepth);
		/// <summary>
		/// Delete parts, patches or polygons not viewed from a sphere around the scene
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="level">Level of parts to remove : Parts, Patches or Polygons</param>
		/// <param name="resolution">Resolution of the visibility viewer</param>
		/// <param name="sphereCount">Segmentation of the sphere sphereCount x sphereCount</param>
		/// <param name="fovX">Horizontal field of view (in degree)</param>
		/// <param name="considerTransparentOpaque">If True, Parts, Patches or Polygons with a transparent appearance are considered as opaque</param>
		/// <param name="adjacencyDepth">Mark neighbors polygons as visible</param>
		public static Core.Native.BoolList HiddenRemoval(Scene.Native.OccurrenceList occurrences, SelectionLevel level, System.Int32 resolution, System.Int32 sphereCount, System.Double fovX, System.Boolean considerTransparentOpaque, System.Int32 adjacencyDepth) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_hiddenRemoval(occurrences_c, (int)level, resolution, sphereCount, fovX, considerTransparentOpaque ? 1 : 0, adjacencyDepth);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Core.Native.NativeInterface.ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_BoolList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Core.Native.BoolList_c Algo_hiddenRemovalCamera(Scene.Native.OccurrenceList_c occurrences, Int32 level, Geom.Native.Point3List_c cameraPositions, Int32 resolution, Int32 sphereCount, System.Double fovX, Int32 considerTransparentOpaque, Int32 adjacencyDepth);
		/// <summary>
		/// Delete parts, patches or polygons not viewed from spheres generated with a set of camera position
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="level">Level of parts to remove : Parts, Patches or Polygons</param>
		/// <param name="cameraPositions">List of camera positions</param>
		/// <param name="resolution">Resolution of the visibility viewer</param>
		/// <param name="sphereCount">Segmentation of the sphere sphereCount x sphereCount</param>
		/// <param name="fovX">Horizontal field of view (in degree)</param>
		/// <param name="considerTransparentOpaque">If True, Parts, Patches or Polygons with a transparent appearance are considered as opaque</param>
		/// <param name="adjacencyDepth">Mark neighbors polygons as visible</param>
		public static Core.Native.BoolList HiddenRemovalCamera(Scene.Native.OccurrenceList occurrences, SelectionLevel level, Geom.Native.Point3List cameraPositions, System.Int32 resolution, System.Int32 sphereCount, System.Double fovX, System.Boolean considerTransparentOpaque, System.Int32 adjacencyDepth) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var cameraPositions_c = new Geom.Native.Point3List_c();
			Geom.Native.NativeInterface.ConvertValue(cameraPositions, ref cameraPositions_c);
			var ret = Algo_hiddenRemovalCamera(occurrences_c, (int)level, cameraPositions_c, resolution, sphereCount, fovX, considerTransparentOpaque ? 1 : 0, adjacencyDepth);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Geom.Native.NativeInterface.Geom_Point3List_free(ref cameraPositions_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Core.Native.NativeInterface.ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_BoolList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Core.Native.BoolList_c Algo_hiddenRemovalViewPoints(Scene.Native.OccurrenceList_c occurrences, Int32 level, Geom.Native.Point3List_c cameraPositions, Geom.Native.Point3List_c cameraDirections, Geom.Native.Point3List_c cameraUps, Int32 resolution, System.Double fovX, Int32 considerTransparentOpaque, Int32 adjacencyDepth);
		/// <summary>
		/// Delete parts, patches or polygons not viewed from a set of camera position/orientation
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="level">Level of parts to remove : Parts, Patches or Polygons</param>
		/// <param name="cameraPositions">List of camera positions</param>
		/// <param name="cameraDirections">List of camera directions</param>
		/// <param name="cameraUps">List of camera up vectors</param>
		/// <param name="resolution">Resolution of the visibility viewer</param>
		/// <param name="fovX">Horizontal field of view (in degree)</param>
		/// <param name="considerTransparentOpaque">If True, Parts, Patches or Polygons with a transparent appearance are considered as opaque</param>
		/// <param name="adjacencyDepth">Mark neighbors polygons as visible</param>
		public static Core.Native.BoolList HiddenRemovalViewPoints(Scene.Native.OccurrenceList occurrences, SelectionLevel level, Geom.Native.Point3List cameraPositions, Geom.Native.Point3List cameraDirections, Geom.Native.Point3List cameraUps, System.Int32 resolution, System.Double fovX, System.Boolean considerTransparentOpaque, System.Int32 adjacencyDepth) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var cameraPositions_c = new Geom.Native.Point3List_c();
			Geom.Native.NativeInterface.ConvertValue(cameraPositions, ref cameraPositions_c);
			var cameraDirections_c = new Geom.Native.Point3List_c();
			Geom.Native.NativeInterface.ConvertValue(cameraDirections, ref cameraDirections_c);
			var cameraUps_c = new Geom.Native.Point3List_c();
			Geom.Native.NativeInterface.ConvertValue(cameraUps, ref cameraUps_c);
			var ret = Algo_hiddenRemovalViewPoints(occurrences_c, (int)level, cameraPositions_c, cameraDirections_c, cameraUps_c, resolution, fovX, considerTransparentOpaque ? 1 : 0, adjacencyDepth);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Geom.Native.NativeInterface.Geom_Point3List_free(ref cameraPositions_c);
			Geom.Native.NativeInterface.Geom_Point3List_free(ref cameraDirections_c);
			Geom.Native.NativeInterface.Geom_Point3List_free(ref cameraUps_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Core.Native.NativeInterface.ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_BoolList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_hiddenSelection(Scene.Native.OccurrenceList_c occurrences, Int32 resolution, Int32 sphereCount, System.Double fovX, Int32 considerTransparentOpaque);
		/// <summary>
		/// Select parts not viewed from a sphere around the scene
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="resolution">Resolution of the visibility viewer</param>
		/// <param name="sphereCount">Segmentation of the sphere sphereCount x sphereCount</param>
		/// <param name="fovX">Horizontal field of view (in degree)</param>
		/// <param name="considerTransparentOpaque">If True, Parts, Patches or Polygons with a transparent appearance are considered as opaque</param>
		public static void HiddenSelection(Scene.Native.OccurrenceList occurrences, System.Int32 resolution, System.Int32 sphereCount, System.Double fovX, System.Boolean considerTransparentOpaque) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_hiddenSelection(occurrences_c, resolution, sphereCount, fovX, considerTransparentOpaque ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_smartHiddenCreateVisibilityInformation(Scene.Native.OccurrenceList_c occurrences, System.Double voxelSize, System.Double minimumCavityVolume, Int32 resolution, Int32 mode, Int32 considerTransparentOpaque);
		/// <summary>
		/// Create visilibity information on parts viewed from a set of camera automatically generated
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="voxelSize">Size of the voxels in mm (smaller it is, more viewpoints there are)</param>
		/// <param name="minimumCavityVolume">Minimum volume of a cavity in cubic meter (smaller it is, more viewpoints there are)</param>
		/// <param name="resolution">Resolution of the visibility viewer</param>
		/// <param name="mode">Select where to place camera (all cavities, only outer or only inner cavities)</param>
		/// <param name="considerTransparentOpaque">If True, Parts, Patches or Polygons with a transparent appearance are considered as opaque</param>
		public static void SmartHiddenCreateVisibilityInformation(Scene.Native.OccurrenceList occurrences, System.Double voxelSize, System.Double minimumCavityVolume, System.Int32 resolution, SmartHiddenType mode, System.Boolean considerTransparentOpaque) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_smartHiddenCreateVisibilityInformation(occurrences_c, voxelSize, minimumCavityVolume, resolution, (int)mode, considerTransparentOpaque ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Core.Native.BoolList_c Algo_smartHiddenRemoval(Scene.Native.OccurrenceList_c occurrences, Int32 level, System.Double voxelSize, System.Double minimumCavityVolume, Int32 resolution, Int32 mode, Int32 considerTransparentOpaque, Int32 adjacencyDepth);
		/// <summary>
		/// Delete parts, patches or polygons not viewed from a set of camera automatically generated
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="level">Level of parts to remove : Parts, Patches or Polygons</param>
		/// <param name="voxelSize">Size of the voxels in mm (smaller it is, more viewpoints there are)</param>
		/// <param name="minimumCavityVolume">Minimum volume of a cavity in cubic meter (smaller it is, more viewpoints there are)</param>
		/// <param name="resolution">Resolution of the visibility viewer</param>
		/// <param name="mode">Select where to place camera (all cavities, only outer or only inner cavities)</param>
		/// <param name="considerTransparentOpaque">If True, Parts, Patches or Polygons with a transparent appearance are considered as opaque</param>
		/// <param name="adjacencyDepth">Mark neighbors polygons as visible</param>
		public static Core.Native.BoolList SmartHiddenRemoval(Scene.Native.OccurrenceList occurrences, SelectionLevel level, System.Double voxelSize, System.Double minimumCavityVolume, System.Int32 resolution, SmartHiddenType mode, System.Boolean considerTransparentOpaque, System.Int32 adjacencyDepth) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_smartHiddenRemoval(occurrences_c, (int)level, voxelSize, minimumCavityVolume, resolution, (int)mode, considerTransparentOpaque ? 1 : 0, adjacencyDepth);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Core.Native.NativeInterface.ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_BoolList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_smartHiddenSelection(Scene.Native.OccurrenceList_c occurrences, System.Double voxelSize, System.Double minimumCavityVolume, Int32 resolution, Int32 mode, Int32 considerTransparentOpaque);
		/// <summary>
		/// Select parts not viewed from a set of camera automatically generated
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="voxelSize">Size of the voxels in mm (smaller it is, more viewpoints there are)</param>
		/// <param name="minimumCavityVolume">Minimum volume of a cavity in cubic meter (smaller it is, more viewpoints there are)</param>
		/// <param name="resolution">Resolution of the visibility viewer</param>
		/// <param name="mode">Select where to place camera (all cavities, only outer or only inner cavities)</param>
		/// <param name="considerTransparentOpaque">If True, Parts, Patches or Polygons with a transparent appearance are considered as opaque</param>
		public static void SmartHiddenSelection(Scene.Native.OccurrenceList occurrences, System.Double voxelSize, System.Double minimumCavityVolume, System.Int32 resolution, SmartHiddenType mode, System.Boolean considerTransparentOpaque) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_smartHiddenSelection(occurrences_c, voxelSize, minimumCavityVolume, resolution, (int)mode, considerTransparentOpaque ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region Sawing

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_sawWithAABB(Scene.Native.OccurrenceList_c occurrences, Geom.Native.AABB_c aabb, Int32 mode, string innerSuffix, string outerSuffix);
		/// <summary>
		/// Saw the mesh with an axis-aligned bounding box
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="aabb">Axis-Aligned Bounding Box</param>
		/// <param name="mode">The sawing mode</param>
		/// <param name="innerSuffix">Only if mode is set to SawAndSplit, set the suffix of the inner part</param>
		/// <param name="outerSuffix">Only if mode is set to SawAndSplit, set the suffix of the outer part</param>
		public static void SawWithAABB(Scene.Native.OccurrenceList occurrences, Geom.Native.AABB aabb, SawingMode mode, System.String innerSuffix, System.String outerSuffix) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var aabb_c = new Geom.Native.AABB_c();
			Geom.Native.NativeInterface.ConvertValue(aabb, ref aabb_c);
			Algo_sawWithAABB(occurrences_c, aabb_c, (int)mode, innerSuffix, outerSuffix);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Geom.Native.NativeInterface.Geom_AABB_free(ref aabb_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_sawWithOBB(Scene.Native.OccurrenceList_c occurrences, Geom.Native.OBB_c obb, Int32 mode, string innerSuffix, string outerSuffix);
		/// <summary>
		/// Saw the mesh with an oriented bounding box
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="obb">Minimum Bounding Box</param>
		/// <param name="mode">The sawing mode</param>
		/// <param name="innerSuffix">Only if mode is set to SawAndSplit, set the suffix of the inner part</param>
		/// <param name="outerSuffix">Only if mode is set to SawAndSplit, set the suffix of the outer part</param>
		public static void SawWithOBB(Scene.Native.OccurrenceList occurrences, Geom.Native.OBB obb, SawingMode mode, System.String innerSuffix, System.String outerSuffix) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var obb_c = new Geom.Native.OBB_c();
			Geom.Native.NativeInterface.ConvertValue(obb, ref obb_c);
			Algo_sawWithOBB(occurrences_c, obb_c, (int)mode, innerSuffix, outerSuffix);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Geom.Native.NativeInterface.Geom_OBB_free(ref obb_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_sawWithPlane(Scene.Native.OccurrenceList_c occurrences, Geom.Native.Point3_c planeOrigin, Geom.Native.Point3_c planeNormal, Int32 mode, string innerSuffix, string outerSuffix);
		/// <summary>
		/// Saw the mesh with a plane
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="planeOrigin">The plane origin</param>
		/// <param name="planeNormal">The plane normal</param>
		/// <param name="mode">The sawing mode</param>
		/// <param name="innerSuffix">Only if mode is set to SawAndSplit, set the suffix of the inner part</param>
		/// <param name="outerSuffix">Only if mode is set to SawAndSplit, set the suffix of the outer part</param>
		public static void SawWithPlane(Scene.Native.OccurrenceList occurrences, Geom.Native.Point3 planeOrigin, Geom.Native.Point3 planeNormal, SawingMode mode, System.String innerSuffix, System.String outerSuffix) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var planeOrigin_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(planeOrigin, ref planeOrigin_c);
			var planeNormal_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(planeNormal, ref planeNormal_c);
			Algo_sawWithPlane(occurrences_c, planeOrigin_c, planeNormal_c, (int)mode, innerSuffix, outerSuffix);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref planeOrigin_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref planeNormal_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region UV Mapping

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_applyUvTransform(Scene.Native.OccurrenceList_c occurrences, Geom.Native.Matrix4_c matrix, Int32 channel);
		/// <summary>
		/// Apply a transformation matrix on texture coordinates
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="matrix">Transformationmatrix</param>
		/// <param name="channel">UV channel to transform</param>
		public static void ApplyUvTransform(Scene.Native.OccurrenceList occurrences, Geom.Native.Matrix4 matrix, System.Int32 channel) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var matrix_c = new Geom.Native.Matrix4_c();
			Geom.Native.NativeInterface.ConvertValue(matrix, ref matrix_c);
			Algo_applyUvTransform(occurrences_c, matrix_c, channel);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref matrix_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_automaticUVMapping(Scene.Native.OccurrenceList_c occurrences, Int32 channel, System.Double maxAngleDistorsion, System.Double maxAreaDistorsion, Int32 sharpToSeam, Int32 forbidOverlapping);
		/// <summary>
		/// Generates the texture coordinates and automatically cut
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="channel">The UV channel which will contains the texture coordinates</param>
		/// <param name="maxAngleDistorsion">Maximum angle distorsion |2PI-SumVtxAng|/2PI</param>
		/// <param name="maxAreaDistorsion">Maximum area distorsion before scale to 1. |2DArea-3DArea|/3DArea </param>
		/// <param name="sharpToSeam">If enabled, sharp edges are automatically considered as UV seams</param>
		/// <param name="forbidOverlapping">If enabled, UV cannot overlap</param>
		public static void AutomaticUVMapping(Scene.Native.OccurrenceList occurrences, System.Int32 channel, System.Double maxAngleDistorsion, System.Double maxAreaDistorsion, System.Boolean sharpToSeam, System.Boolean forbidOverlapping) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_automaticUVMapping(occurrences_c, channel, maxAngleDistorsion, maxAreaDistorsion, sharpToSeam ? 1 : 0, forbidOverlapping ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_copyUV(Scene.Native.OccurrenceList_c occurrences, Int32 sourceChannel, Int32 destinationChannel);
		/// <summary>
		/// Copy an UV channel to another UV channel
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="sourceChannel">The source UV channel to copy</param>
		/// <param name="destinationChannel">The destination UV channel to copy into</param>
		public static void CopyUV(Scene.Native.OccurrenceList occurrences, System.Int32 sourceChannel, System.Int32 destinationChannel) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_copyUV(occurrences_c, sourceChannel, destinationChannel);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_deleteUnfoldWeightAttribute(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Delete Unfold Weight attributes on tessellations
		/// </summary>
		/// <param name="occurrences">Occurrences of components to delete attributes</param>
		public static void DeleteUnfoldWeightAttribute(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_deleteUnfoldWeightAttribute(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_mapUvOnAABB(Scene.Native.OccurrenceList_c occurrences, Int32 useLocalAABB, System.Double uv3dSize, Int32 channel, Int32 overrideExistingUVs);
		/// <summary>
		/// Generate texture coordinates using the projection on object Axis Aligned Bounding Box
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="useLocalAABB">If enabled, uses part own bounding box, else use global one</param>
		/// <param name="uv3dSize">3D size of the UV space [0-1]</param>
		/// <param name="channel">The UV channel which will contains the texture coordinates</param>
		/// <param name="overrideExistingUVs">If True, overide existing UVs on channel</param>
		public static void MapUvOnAABB(Scene.Native.OccurrenceList occurrences, System.Boolean useLocalAABB, System.Double uv3dSize, System.Int32 channel, System.Boolean overrideExistingUVs) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_mapUvOnAABB(occurrences_c, useLocalAABB ? 1 : 0, uv3dSize, channel, overrideExistingUVs ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_mapUvOnBox(Scene.Native.OccurrenceList_c occurrences, Box_c box, Int32 channel, Int32 overrideExistingUVs);
		/// <summary>
		/// Generate texture coordinates using the projection on a box
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="box">Box definition</param>
		/// <param name="channel">The UV channel which will contains the texture coordinates</param>
		/// <param name="overrideExistingUVs">If True, overide existing UVs on channel</param>
		public static void MapUvOnBox(Scene.Native.OccurrenceList occurrences, Box box, System.Int32 channel, System.Boolean overrideExistingUVs) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var box_c = new Algo.Native.Box_c();
			ConvertValue(box, ref box_c);
			Algo_mapUvOnBox(occurrences_c, box_c, channel, overrideExistingUVs ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Algo.Native.NativeInterface.Algo_Box_free(ref box_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_mapUvOnCubicAABB(Scene.Native.OccurrenceList_c occurrences, System.Double uv3dSize, Int32 channel, Int32 overrideExistingUVs);
		/// <summary>
		/// Generate texture coordinates using the projection on object AABB, with same scale on each axis
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="uv3dSize">3D size of the UV space [0-1]</param>
		/// <param name="channel">The UV channel which will contains the texture coordinates</param>
		/// <param name="overrideExistingUVs">If True, overide existing UVs on channel</param>
		public static void MapUvOnCubicAABB(Scene.Native.OccurrenceList occurrences, System.Double uv3dSize, System.Int32 channel, System.Boolean overrideExistingUVs) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_mapUvOnCubicAABB(occurrences_c, uv3dSize, channel, overrideExistingUVs ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_mapUvOnCustomAABB(Scene.Native.OccurrenceList_c occurrences, Geom.Native.AABB_c aabb, System.Double uv3dSize, Int32 channel, Int32 overrideExistingUVs);
		/// <summary>
		/// Generate texture coordinates using the projection on custom AABB
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="aabb">Axis aligned bounding box to project on</param>
		/// <param name="uv3dSize">3D size of the UV space [0-1]</param>
		/// <param name="channel">The UV channel which will contains the texture coordinates</param>
		/// <param name="overrideExistingUVs">If True, overide existing UVs on channel</param>
		public static void MapUvOnCustomAABB(Scene.Native.OccurrenceList occurrences, Geom.Native.AABB aabb, System.Double uv3dSize, System.Int32 channel, System.Boolean overrideExistingUVs) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var aabb_c = new Geom.Native.AABB_c();
			Geom.Native.NativeInterface.ConvertValue(aabb, ref aabb_c);
			Algo_mapUvOnCustomAABB(occurrences_c, aabb_c, uv3dSize, channel, overrideExistingUVs ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Geom.Native.NativeInterface.Geom_AABB_free(ref aabb_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_mapUvOnCylinder(Scene.Native.OccurrenceList_c occurrences, Cylinder_c cylinder, Int32 channel, Int32 overrideExistingUVs);
		/// <summary>
		/// Generate texture coordinates using the projection on a cylinder
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="cylinder">Cylinder definition</param>
		/// <param name="channel">The UV channel which will contains the texture coordinates</param>
		/// <param name="overrideExistingUVs">If True, overide existing UVs on channel</param>
		public static void MapUvOnCylinder(Scene.Native.OccurrenceList occurrences, Cylinder cylinder, System.Int32 channel, System.Boolean overrideExistingUVs) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var cylinder_c = new Algo.Native.Cylinder_c();
			ConvertValue(cylinder, ref cylinder_c);
			Algo_mapUvOnCylinder(occurrences_c, cylinder_c, channel, overrideExistingUVs ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Algo.Native.NativeInterface.Algo_Cylinder_free(ref cylinder_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_mapUvOnFittingCylinder(Scene.Native.OccurrenceList_c occurrences, Int32 channel, Int32 overrideExistingUVs, Int32 useAABB);
		/// <summary>
		/// Generate texture coordinates using the projection on a fitting cylinder
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="channel">The UV channel which will contains the texture coordinates</param>
		/// <param name="overrideExistingUVs">If True, overide existing UVs on channel</param>
		/// <param name="useAABB">If true use for the fitting the global Axis Aligned Bounding Box (AABB), else use a Minimum Bounding Box (MBB only based on transformed AABB of occurrences)</param>
		public static void MapUvOnFittingCylinder(Scene.Native.OccurrenceList occurrences, System.Int32 channel, System.Boolean overrideExistingUVs, System.Boolean useAABB) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_mapUvOnFittingCylinder(occurrences_c, channel, overrideExistingUVs ? 1 : 0, useAABB ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_mapUvOnFittingSphere(Scene.Native.OccurrenceList_c occurrences, Int32 channel, Int32 overrideExistingUVs, Int32 useAABB);
		/// <summary>
		/// Generate texture coordinates using the projection on a fitting sphere
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="channel">The UV channel which will contains the texture coordinates</param>
		/// <param name="overrideExistingUVs">If True, overide existing UVs on channel</param>
		/// <param name="useAABB">If true use for the fitting the global Axis Aligned Bounding Box (AABB), else use a Minimum Bounding Box (MBB only based on transformed AABB of occurrences)</param>
		public static void MapUvOnFittingSphere(Scene.Native.OccurrenceList occurrences, System.Int32 channel, System.Boolean overrideExistingUVs, System.Boolean useAABB) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_mapUvOnFittingSphere(occurrences_c, channel, overrideExistingUVs ? 1 : 0, useAABB ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_mapUvOnMBB(Scene.Native.OccurrenceList_c occurrences, Int32 useLocalMBB, System.Double uv3dSize, Int32 channel, Int32 overrideExistingUVs);
		/// <summary>
		/// Generate texture coordinates using the projection on object Minimum Bounding Box
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="useLocalMBB">If enabled, uses part own bounding box, else use global one</param>
		/// <param name="uv3dSize">3D size of the UV space [0-1]</param>
		/// <param name="channel">The UV channel which will contains the texture coordinates</param>
		/// <param name="overrideExistingUVs">If True, overide existing UVs on channel</param>
		public static void MapUvOnMBB(Scene.Native.OccurrenceList occurrences, System.Boolean useLocalMBB, System.Double uv3dSize, System.Int32 channel, System.Boolean overrideExistingUVs) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_mapUvOnMBB(occurrences_c, useLocalMBB ? 1 : 0, uv3dSize, channel, overrideExistingUVs ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_mapUvOnPlane(Scene.Native.OccurrenceList_c occurrences, Plane_c plane, Int32 channel, Int32 overrideExistingUVs);
		/// <summary>
		/// Generate texture coordinates using the projection on a plane
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="plane">Plane definition</param>
		/// <param name="channel">The UV channel which will contains the texture coordinates</param>
		/// <param name="overrideExistingUVs">If True, overide existing UVs on channel</param>
		public static void MapUvOnPlane(Scene.Native.OccurrenceList occurrences, Plane plane, System.Int32 channel, System.Boolean overrideExistingUVs) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var plane_c = new Algo.Native.Plane_c();
			ConvertValue(plane, ref plane_c);
			Algo_mapUvOnPlane(occurrences_c, plane_c, channel, overrideExistingUVs ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Algo.Native.NativeInterface.Algo_Plane_free(ref plane_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_mapUvOnSphere(Scene.Native.OccurrenceList_c occurrences, Sphere_c sphere, Int32 channel, Int32 overrideExistingUVs);
		/// <summary>
		/// Generate texture coordinates using the projection on a sphere
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="sphere">Sphere definition</param>
		/// <param name="channel">The UV channel which will contains the texture coordinates</param>
		/// <param name="overrideExistingUVs">If True, overide existing UVs on channel</param>
		public static void MapUvOnSphere(Scene.Native.OccurrenceList occurrences, Sphere sphere, System.Int32 channel, System.Boolean overrideExistingUVs) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var sphere_c = new Algo.Native.Sphere_c();
			ConvertValue(sphere, ref sphere_c);
			Algo_mapUvOnSphere(occurrences_c, sphere_c, channel, overrideExistingUVs ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Algo.Native.NativeInterface.Algo_Sphere_free(ref sphere_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_normalizeUV(Scene.Native.OccurrenceList_c occurrences, Int32 sourceUVChannel, Int32 destinationUVChannel, Int32 uniform, Int32 sharedUVSpace, Int32 ignoreNullIslands);
		/// <summary>
		/// Normalize UVs to fit in the [0-1] uv space
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="sourceUVChannel">UV Channel to normalize</param>
		/// <param name="destinationUVChannel">UV channel to store the normalized UV (if -1, sourceUVChannel will be replaced)</param>
		/// <param name="uniform">If true, the scale will be uniform. Else UV can be deformed with a non-uniform scale</param>
		/// <param name="sharedUVSpace">If true, all parts will be processed as if they were merged to avoid overlapping of their UV coordinates</param>
		/// <param name="ignoreNullIslands">If true, islands with null height and width will be ignored and their UV coordinates will be set to [0,0] (Slower if enabled)</param>
		public static void NormalizeUV(Scene.Native.OccurrenceList occurrences, System.Int32 sourceUVChannel, System.Int32 destinationUVChannel, System.Boolean uniform, System.Boolean sharedUVSpace, System.Boolean ignoreNullIslands) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_normalizeUV(occurrences_c, sourceUVChannel, destinationUVChannel, uniform ? 1 : 0, sharedUVSpace ? 1 : 0, ignoreNullIslands ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_relaxUV(Scene.Native.OccurrenceList_c occurrences, Int32 method, Int32 iterations, Int32 channel);
		/// <summary>
		/// Relax UVs
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="method">What the method used to relax is based on</param>
		/// <param name="iterations">Number of relax iterations</param>
		/// <param name="channel">The UV channel to repack</param>
		public static void RelaxUV(Scene.Native.OccurrenceList occurrences, RelaxUVMethod method, System.Int32 iterations, System.Int32 channel) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_relaxUV(occurrences_c, (int)method, iterations, channel);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_removeUV(Scene.Native.OccurrenceList_c occurrences, Int32 channel);
		/// <summary>
		/// Remove one or all UV channel(s)
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="channel">The UV channel to remove (all if channel=-1)</param>
		public static void RemoveUV(Scene.Native.OccurrenceList occurrences, System.Int32 channel) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_removeUV(occurrences_c, channel);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Scene.Native.OccurrenceList_c Algo_repackUV(Scene.Native.OccurrenceList_c occurrences, Int32 channel, Int32 shareMap, Int32 resolution, Int32 padding, Int32 uniformRatio, Int32 iterations, Int32 removeOverlaps);
		/// <summary>
		/// Pack existing UV (create atlas)
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="channel">The UV channel to repack</param>
		/// <param name="shareMap">If True, the UV of all given parts will be packed together</param>
		/// <param name="resolution">Resolution wanted for the final map</param>
		/// <param name="padding">Set the padding (in pixels) between UV islands</param>
		/// <param name="uniformRatio">If true, UV of different part will have the same ratio</param>
		/// <param name="iterations">Fitting iterations</param>
		/// <param name="removeOverlaps">Remove overlaps to avoid multiple triangles UVs to share the same pixel</param>
		public static Scene.Native.OccurrenceList RepackUV(Scene.Native.OccurrenceList occurrences, System.Int32 channel, System.Boolean shareMap, System.Int32 resolution, System.Int32 padding, System.Boolean uniformRatio, System.Int32 iterations, System.Boolean removeOverlaps) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_repackUV(occurrences_c, channel, shareMap ? 1 : 0, resolution, padding, uniformRatio ? 1 : 0, iterations, removeOverlaps ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Scene.Native.NativeInterface.ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_scaleUV(Scene.Native.OccurrenceList_c occurrences, System.Double scaleU, System.Double scaleV, Int32 channel);
		/// <summary>
		/// Apply a scale on texture coordinates
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="scaleU">Scale to apply to U coordinate</param>
		/// <param name="scaleV">Scale to apply to V coordinate</param>
		/// <param name="channel">UV channel to transform</param>
		public static void ScaleUV(Scene.Native.OccurrenceList occurrences, System.Double scaleU, System.Double scaleV, System.Int32 channel) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_scaleUV(occurrences_c, scaleU, scaleV, channel);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_segmentDiskFront(Scene.Native.OccurrenceList_c occurrences, System.Double threshold, Int32 channel);
		/// <summary>
		/// Create UV patches with disk-like topology
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="threshold">Threshold of the front's Gaussian Curvature</param>
		/// <param name="channel">The UV channel to repack</param>
		public static void SegmentDiskFront(Scene.Native.OccurrenceList occurrences, System.Double threshold, System.Int32 channel) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_segmentDiskFront(occurrences_c, threshold, channel);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_smoothUV(Scene.Native.OccurrenceList_c occurrences, Int32 iterations, Int32 channel);
		/// <summary>
		/// Smooth texture coordinates
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="iterations">Number of smooth iterations</param>
		/// <param name="channel">The UV channel which will contains the texture coordinates to smooth</param>
		public static void SmoothUV(Scene.Native.OccurrenceList occurrences, System.Int32 iterations, System.Int32 channel) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_smoothUV(occurrences_c, iterations, channel);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_stitchUV(Scene.Native.OccurrenceList_c occurrences, Int32 channel);
		/// <summary>
		/// Try to stitch existing UV islands
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="channel">The UV channel to repack</param>
		public static void StitchUV(Scene.Native.OccurrenceList occurrences, System.Int32 channel) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_stitchUV(occurrences_c, channel);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_swapUvChannels(Scene.Native.OccurrenceList_c occurrences, Int32 firstChannel, Int32 secondChannel);
		/// <summary>
		/// Swap two UV channels
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		/// <param name="firstChannel">First UV Channel to swap</param>
		/// <param name="secondChannel">Second UV Channel to swap</param>
		public static void SwapUvChannels(Scene.Native.OccurrenceList occurrences, System.Int32 firstChannel, System.Int32 secondChannel) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_swapUvChannels(occurrences_c, firstChannel, secondChannel);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_transferVisibilityToUnfoldWeight(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Set Unfold Weight Attribute from Visibility Attribute
		/// </summary>
		/// <param name="occurrences">Occurrences of part to process</param>
		public static void TransferVisibilityToUnfoldWeight(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_transferVisibilityToUnfoldWeight(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region fitting

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Geom.Native.Affine_c Algo_getFittingCylinder(Scene.Native.OccurrenceList_c occurrences, Int32 useAABB);
		/// <summary>
		/// Returns the fitting cylinder of a set of occurrences (based on MBB)
		/// </summary>
		/// <param name="occurrences">Occurrences to fit</param>
		/// <param name="useAABB">If true use the global Axis Aligned Bounding Box (AABB), else use a Minimum Bounding Box MBB only based on transformed AABB of occurrences)</param>
		public static Geom.Native.Affine GetFittingCylinder(Scene.Native.OccurrenceList occurrences, System.Boolean useAABB) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_getFittingCylinder(occurrences_c, useAABB ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Geom.Native.NativeInterface.ConvertValue(ref ret);
			Geom.Native.NativeInterface.Geom_Affine_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Geom.Native.Affine_c Algo_getFittingSphere(Scene.Native.OccurrenceList_c occurrences, Int32 useAABB);
		/// <summary>
		/// Returns the fitting sphere of a set of occurrences
		/// </summary>
		/// <param name="occurrences">Occurrences to fit</param>
		/// <param name="useAABB">If true use the global Axis Aligned Bounding Box (AABB), else use a Minimum Bounding Box (MBB only based on transformed AABB of occurrences)</param>
		public static Geom.Native.Affine GetFittingSphere(Scene.Native.OccurrenceList occurrences, System.Boolean useAABB) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_getFittingSphere(occurrences_c, useAABB ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Geom.Native.NativeInterface.ConvertValue(ref ret);
			Geom.Native.NativeInterface.Geom_Affine_free(ref ret);
			return convRet;
		}

		#endregion

		#region map_generation

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Material.Native.ImageList_c Algo_bakeMaps(Scene.Native.OccurrenceList_c destinationOccurrences, Scene.Native.OccurrenceList_c sourceOccurrences, BakeMapList_c mapsToBake, Int32 channel, Int32 resolution, Int32 padding, Int32 shareMaps, string mapSuffix, CustomBakeMapList_c additionalCustomMaps, System.Double tolerance, Int32 method, System.Double opacityThreshold, Int32 useCurrentPosition, System.Double offset, getPixelValueList_c callbackList);
		/// <summary>
		/// Bake texture maps on meshes from self or other meshes
		/// </summary>
		/// <param name="destinationOccurrences">Occurrences of the meshes where to store the baked map</param>
		/// <param name="sourceOccurrences">Occurrences of components from which to bake maps (if empty use destination)</param>
		/// <param name="mapsToBake">List of map to generate (Normal, Diffuse, ...)</param>
		/// <param name="channel">UV channel of destOccurrence to use for the map generation</param>
		/// <param name="resolution">Map resolution</param>
		/// <param name="padding">Add padding to the map</param>
		/// <param name="shareMaps">If true, all the destinationOccurrences will share the same maps</param>
		/// <param name="mapSuffix">Add a suffix to the map names</param>
		/// <param name="additionalCustomMaps">Additional custom maps to bake</param>
		/// <param name="tolerance">Tolerance of projection for baking from source to destination</param>
		/// <param name="method">Method to find source color if source occurrences are different than destination occurrences (Prefer ProjOnly for point clouds and RayOnly for meshes)</param>
		/// <param name="opacityThreshold">If the opacity is under this threshold, considers as fully transparent and store the color behind the intersection</param>
		/// <param name="useCurrentPosition">Use the current position instead of the T-Pose of the input occurrence</param>
		/// <param name="offset">Offset from mesh</param>
		/// <param name="callbackList">Callbacks that returns a color</param>
		public static Material.Native.ImageList BakeMaps(Scene.Native.OccurrenceList destinationOccurrences, Scene.Native.OccurrenceList sourceOccurrences, BakeMapList mapsToBake, System.Int32 channel, System.Int32 resolution, System.Int32 padding, System.Boolean shareMaps, System.String mapSuffix, CustomBakeMapList additionalCustomMaps, System.Double tolerance, BakingMethod method, System.Double opacityThreshold, System.Boolean useCurrentPosition, System.Double offset, getPixelValueList callbackList) {
			var destinationOccurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(destinationOccurrences, ref destinationOccurrences_c);
			var sourceOccurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(sourceOccurrences, ref sourceOccurrences_c);
			var mapsToBake_c = new Algo.Native.BakeMapList_c();
			ConvertValue(mapsToBake, ref mapsToBake_c);
			var additionalCustomMaps_c = new Algo.Native.CustomBakeMapList_c();
			ConvertValue(additionalCustomMaps, ref additionalCustomMaps_c);
			var callbackList_c = new Algo.Native.getPixelValueList_c();
			ConvertValue(callbackList, ref callbackList_c);
			var ret = Algo_bakeMaps(destinationOccurrences_c, sourceOccurrences_c, mapsToBake_c, channel, resolution, padding, shareMaps ? 1 : 0, mapSuffix, additionalCustomMaps_c, tolerance, (int)method, opacityThreshold, useCurrentPosition ? 1 : 0, offset, callbackList_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref destinationOccurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref sourceOccurrences_c);
			Algo.Native.NativeInterface.Algo_BakeMapList_free(ref mapsToBake_c);
			Algo.Native.NativeInterface.Algo_CustomBakeMapList_free(ref additionalCustomMaps_c);
			Algo.Native.NativeInterface.Algo_getPixelValueList_free(ref callbackList_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Material.Native.NativeInterface.ConvertValue(ref ret);
			Material.Native.NativeInterface.Material_ImageList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Algo_convertNormalMap(Scene.Native.OccurrenceList_c occurrences, System.UInt32 normalMap, Int32 uvChannel, Int32 sourceIsObjectSpace, Int32 destinationIsObjectSpace, Int32 sourceIsRightHanded, Int32 destinationIsRightHanded, Int32 replaceMap, Int32 resolution, Int32 padding);
		/// <summary>
		/// Convert an existing normal map between Object-space and Tangent-space
		/// </summary>
		/// <param name="occurrences">Occurrences of components using the given map</param>
		/// <param name="normalMap">Source normal map to convert</param>
		/// <param name="uvChannel">UV channel used on the given map</param>
		/// <param name="sourceIsObjectSpace">If True, consider the given normalMap in Object-space representation, else Tangent-space</param>
		/// <param name="destinationIsObjectSpace">If True, convert the given normalMap to Object-space representation, else Tangent-space</param>
		/// <param name="sourceIsRightHanded">Considers source normal map as part of a right-handed coordinates system</param>
		/// <param name="destinationIsRightHanded">Generate destination normal map as part of a right-handed coordinates system</param>
		/// <param name="replaceMap">If true, the given normalMap will be replaced by the converted one</param>
		/// <param name="resolution">New map resolution (if replaceMap=false), if resolution=-1, the input resolution will be used</param>
		/// <param name="padding">Number of pixels to add for padding</param>
		public static System.UInt32 ConvertNormalMap(Scene.Native.OccurrenceList occurrences, System.UInt32 normalMap, System.Int32 uvChannel, System.Boolean sourceIsObjectSpace, System.Boolean destinationIsObjectSpace, System.Boolean sourceIsRightHanded, System.Boolean destinationIsRightHanded, System.Boolean replaceMap, System.Int32 resolution, System.Int32 padding) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_convertNormalMap(occurrences_c, normalMap, uvChannel, sourceIsObjectSpace ? 1 : 0, destinationIsObjectSpace ? 1 : 0, sourceIsRightHanded ? 1 : 0, destinationIsRightHanded ? 1 : 0, replaceMap ? 1 : 0, resolution, padding);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Algo_createBillboard(Scene.Native.OccurrenceList_c occurrences, Int32 resolution, Int32 XPositive, Int32 XNegative, Int32 YPositive, Int32 YNegative, Int32 ZPositive, Int32 ZNegative, Int32 moveFacesToCenter, Int32 leftHandedNormalMap);
		/// <summary>
		/// Create a billboard imposter
		/// </summary>
		/// <param name="occurrences">Occurrences to bake in the billboard</param>
		/// <param name="resolution">Total resolution of the billboard (contains all wanted faces)</param>
		/// <param name="XPositive">Bake face facing X+</param>
		/// <param name="XNegative">Bake face facing X-</param>
		/// <param name="YPositive">Bake face facing Y+</param>
		/// <param name="YNegative">Bake face facing Y-</param>
		/// <param name="ZPositive">Bake face facing Z+</param>
		/// <param name="ZNegative">Bake face facing Z-</param>
		/// <param name="moveFacesToCenter">If true, all face are moved to the center of the AABB of the occurrences, else it will shape an AABB</param>
		/// <param name="leftHandedNormalMap">If true, a left handed normal map will be generated</param>
		public static System.UInt32 CreateBillboard(Scene.Native.OccurrenceList occurrences, System.Int32 resolution, System.Boolean XPositive, System.Boolean XNegative, System.Boolean YPositive, System.Boolean YNegative, System.Boolean ZPositive, System.Boolean ZNegative, System.Boolean moveFacesToCenter, System.Boolean leftHandedNormalMap) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_createBillboard(occurrences_c, resolution, XPositive ? 1 : 0, XNegative ? 1 : 0, YPositive ? 1 : 0, YNegative ? 1 : 0, ZPositive ? 1 : 0, ZNegative ? 1 : 0, moveFacesToCenter ? 1 : 0, leftHandedNormalMap ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_orientNormalMap(System.UInt32 normalMap);
		/// <summary>
		/// Orient a tangent space normal map (all Z positive)
		/// </summary>
		/// <param name="normalMap">Normal map to orient</param>
		public static void OrientNormalMap(System.UInt32 normalMap) {
			Algo_orientNormalMap(normalMap);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region repair

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_barySmooth(Scene.Native.OccurrenceList_c occurrences, Int32 iteration);
		/// <summary>
		/// Smooth the tessellations by moving the vertices to the barycenter of their neighbors
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="iteration">Number of iterations</param>
		public static void BarySmooth(Scene.Native.OccurrenceList occurrences, System.Int32 iteration) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_barySmooth(occurrences_c, iteration);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_crackMoebiusStrips(Scene.Native.OccurrenceList_c occurrences, Int32 maxEdgeCount);
		/// <summary>
		/// Remove moebius strip by topologically cracking them (make it orientable)
		/// </summary>
		/// <param name="occurrences">Occurrences of components to repair</param>
		/// <param name="maxEdgeCount">Maximum number of edges to crack to remove one moebius strip</param>
		public static void CrackMoebiusStrips(Scene.Native.OccurrenceList occurrences, System.Int32 maxEdgeCount) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_crackMoebiusStrips(occurrences_c, maxEdgeCount);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_crackNonManifoldVertices(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Splits non-manifold vertices
		/// </summary>
		/// <param name="occurrences">Occurrences of components to repair</param>
		public static void CrackNonManifoldVertices(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_crackNonManifoldVertices(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Algo_createCavityOccurrences(Scene.Native.OccurrenceList_c occurrences, System.Double voxelSize, System.Double minimumCavityVolume, Int32 mode, System.UInt32 parent);
		/// <summary>
		/// Identify cavities and create occurrences to show them
		/// </summary>
		/// <param name="occurrences">Occurrences set to identify cavities</param>
		/// <param name="voxelSize">Size of the voxels in mm</param>
		/// <param name="minimumCavityVolume">Minimum volume of a cavity in cubic meter</param>
		/// <param name="mode">Select where to place camera (all cavities, only outer or only inner cavities)</param>
		/// <param name="parent">The create occurrence root will be added under the parent if given, else it will be added under the deeper parent of given occurrences</param>
		public static System.UInt32 CreateCavityOccurrences(Scene.Native.OccurrenceList occurrences, System.Double voxelSize, System.Double minimumCavityVolume, SmartHiddenType mode, System.UInt32 parent) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_createCavityOccurrences(occurrences_c, voxelSize, minimumCavityVolume, (int)mode, parent);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_invertOrientation(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Invert the orientation of tessellation elements
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		public static void InvertOrientation(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_invertOrientation(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_moebiusCracker(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Splits moebius ring
		/// </summary>
		/// <param name="occurrences">Occurrences of components to repair</param>
		public static void MoebiusCracker(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_moebiusCracker(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_orient(Scene.Native.OccurrenceList_c occurrences, Int32 makeOrientable, Int32 useArea, Int32 orientStrategy);
		/// <summary>
		/// Orient tessellation elements
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="makeOrientable">Crack moebius strips to make the model orientable</param>
		/// <param name="useArea">Use the area instead of counting the number of triangle</param>
		/// <param name="orientStrategy">Strategy to adopt with this algorithm</param>
		public static void Orient(Scene.Native.OccurrenceList occurrences, System.Boolean makeOrientable, System.Boolean useArea, OrientStrategy orientStrategy) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_orient(occurrences_c, makeOrientable ? 1 : 0, useArea ? 1 : 0, (int)orientStrategy);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_orientFromCamera(Scene.Native.OccurrenceList_c occurrences, Geom.Native.Point3_c cameraPosition, Geom.Native.Point3_c cameraDirection, Geom.Native.Point3_c cameraUp, Int32 resolution, System.Double fovX);
		/// <summary>
		/// Properly orient all polygons in the same direction, using a specified viewpoint
		/// </summary>
		/// <param name="occurrences">Occurrences to orient</param>
		/// <param name="cameraPosition">Camera position</param>
		/// <param name="cameraDirection">Camera direction</param>
		/// <param name="cameraUp">Camera up vector</param>
		/// <param name="resolution">Resolution of the visibility viewer</param>
		/// <param name="fovX">Horizontal field of view (in degree)</param>
		public static void OrientFromCamera(Scene.Native.OccurrenceList occurrences, Geom.Native.Point3 cameraPosition, Geom.Native.Point3 cameraDirection, Geom.Native.Point3 cameraUp, System.Int32 resolution, System.Double fovX) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var cameraPosition_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(cameraPosition, ref cameraPosition_c);
			var cameraDirection_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(cameraDirection, ref cameraDirection_c);
			var cameraUp_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(cameraUp, ref cameraUp_c);
			Algo_orientFromCamera(occurrences_c, cameraPosition_c, cameraDirection_c, cameraUp_c, resolution, fovX);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref cameraPosition_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref cameraDirection_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref cameraUp_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_orientFromFace();
		/// <summary>
		/// Orient all connect polygones in the same orientation of the polygon selectionned
		/// </summary>
		public static void OrientFromFace() {
			Algo_orientFromFace();
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_orientNormals(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Orient existing normal according to the polygons clockwise
		/// </summary>
		/// <param name="occurrences">Occurrences of components to orient normals</param>
		public static void OrientNormals(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_orientNormals(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_remeshSurfacicHoles(Scene.Native.OccurrenceList_c occurrences, System.Double maxDiameter);
		/// <summary>
		/// Resmesh surfacic holes of tessellations
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="maxDiameter">Maximum surfacic holes diameters</param>
		public static void RemeshSurfacicHoles(Scene.Native.OccurrenceList occurrences, System.Double maxDiameter) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_remeshSurfacicHoles(occurrences_c, maxDiameter);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_removeDegeneratedPolygons(Scene.Native.OccurrenceList_c occurrences, System.Double tolerance);
		/// <summary>
		/// Remove some kinds of degenerated polygons
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="tolerance">Degenerated tolerance</param>
		public static void RemoveDegeneratedPolygons(Scene.Native.OccurrenceList occurrences, System.Double tolerance) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_removeDegeneratedPolygons(occurrences_c, tolerance);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_removeMultiplePolygon(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Remove multiple polygon
		/// </summary>
		/// <param name="occurrences">Occurrences of components to repair</param>
		public static void RemoveMultiplePolygon(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_removeMultiplePolygon(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.Double Algo_removeZFighting(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Remove Z-fighting (surfaces overlaping) by slightly shrinking the selected parts' surfaces
		/// </summary>
		/// <param name="occurrences">Occurrences to process</param>
		public static System.Double RemoveZFighting(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var ret = Algo_removeZFighting(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Double)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_repairMesh(Scene.Native.OccurrenceList_c occurrences, System.Double tolerance, Int32 crackNonManifold, Int32 orient);
		/// <summary>
		/// Launch the repair process to repair a disconnected or not clean tessellation
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="tolerance">Connection tolerance</param>
		/// <param name="crackNonManifold">At the end of the repair process, crack resulting non-manifold edges</param>
		/// <param name="orient">If true reorient the model</param>
		public static void RepairMesh(Scene.Native.OccurrenceList occurrences, System.Double tolerance, System.Boolean crackNonManifold, System.Boolean orient) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_repairMesh(occurrences_c, tolerance, crackNonManifold ? 1 : 0, orient ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_repairNullNormals(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Create normal on an existing normal set when normal is null (polygons appears black)
		/// </summary>
		/// <param name="occurrences">Occurrences of components to repair null normals</param>
		public static void RepairNullNormals(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_repairNullNormals(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_separateToManifold(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Remove non manifold edges and try to reconnect manifold groups of triangles
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		public static void SeparateToManifold(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_separateToManifold(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_sewBoundary(Scene.Native.OccurrenceList_c occurrences, System.Double maxDistance);
		/// <summary>
		/// Sew boundaries between them
		/// </summary>
		/// <param name="occurrences">Occurrences of components to repair</param>
		/// <param name="maxDistance">Maximum distance between bundaries</param>
		public static void SewBoundary(Scene.Native.OccurrenceList occurrences, System.Double maxDistance) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_sewBoundary(occurrences_c, maxDistance);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_smartOrient(Scene.Native.OccurrenceList_c occurrences, System.Double voxelSize, System.Double minimumCavityVolume, Int32 resolution, Int32 mode, Int32 considerTransparentOpaque, Int32 orientStrategy);
		/// <summary>
		/// Properly orient all polygons in the same direction, using visibility attributes
		/// </summary>
		/// <param name="occurrences">Occurrences to orient</param>
		/// <param name="voxelSize">Size of the voxels in mm (smaller it is, more viewpoints there are)</param>
		/// <param name="minimumCavityVolume">Minimum volume of a cavity in cubic meter (smaller it is, more viewpoints there are)</param>
		/// <param name="resolution">Resolution of the visibility viewer</param>
		/// <param name="mode">Select where to place camera (all cavities, only outer or only inner cavities)</param>
		/// <param name="considerTransparentOpaque">If True, Parts, Patches or Polygons with a transparent appearance are considered as opaque</param>
		/// <param name="orientStrategy">Strategy to adopt with this algorithm</param>
		public static void SmartOrient(Scene.Native.OccurrenceList occurrences, System.Double voxelSize, System.Double minimumCavityVolume, System.Int32 resolution, SmartHiddenType mode, System.Boolean considerTransparentOpaque, SmartOrientStrategy orientStrategy) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_smartOrient(occurrences_c, voxelSize, minimumCavityVolume, resolution, (int)mode, considerTransparentOpaque ? 1 : 0, (int)orientStrategy);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_vertexOffset(Scene.Native.OccurrenceList_c occurrences, System.Double offset);
		/// <summary>
		/// Move the vertices by the offsset along their normal
		/// </summary>
		/// <param name="occurrences">Occurrences to process</param>
		/// <param name="offset">Displacement</param>
		public static void VertexOffset(Scene.Native.OccurrenceList occurrences, System.Double offset) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_vertexOffset(occurrences_c, offset);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region tessellation conversion

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_equilateralize(Scene.Native.OccurrenceList_c occurrences, Int32 maxIterations);
		/// <summary>
		/// Sswap edges to make triangles more equilateral
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="maxIterations">Maximum number of swapping iteration</param>
		public static void Equilateralize(Scene.Native.OccurrenceList occurrences, System.Int32 maxIterations) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_equilateralize(occurrences_c, maxIterations);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_quadify(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Merge all triangle polygons in the meshes to quadrangles
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		public static void Quadify(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_quadify(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_requadify(Scene.Native.OccurrenceList_c occurrences, Int32 forceFullQuad);
		/// <summary>
		/// Advanced function to requadify a triangle tessellation coming from full quad mesh
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="forceFullQuad">Force the results to be only full quad. It it's impossible, nothing is done</param>
		public static void Requadify(Scene.Native.OccurrenceList occurrences, System.Boolean forceFullQuad) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_requadify(occurrences_c, forceFullQuad ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_toEditableMesh(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Convert all static mesh to editable mesh
		/// </summary>
		/// <param name="occurrences">Occurrences to convert to editable mesh</param>
		public static void ToEditableMesh(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_toEditableMesh(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_toStaticMesh(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Convert all editable mesh to static mesh
		/// </summary>
		/// <param name="occurrences">Occurrences to convert to static mesh</param>
		public static void ToStaticMesh(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_toStaticMesh(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_triangularize(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Split all non-triangle polygons in the meshes to triangles
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		public static void Triangularize(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_triangularize(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region vertex weights

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_createVertexWeightsFromVertexColors(Scene.Native.OccurrenceList_c occurrences, System.Double offset, System.Double scale);
		/// <summary>
		/// Use vertex colors attributes on meshes of the given occurrence to create vertex weights attributes used by the decimation functions, the finals weights will be computed with w = offset + (red - blue) * scale
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="offset">Offset value for weight computation</param>
		/// <param name="scale">Scale value for weight computation</param>
		public static void CreateVertexWeightsFromVertexColors(Scene.Native.OccurrenceList occurrences, System.Double offset, System.Double scale) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_createVertexWeightsFromVertexColors(occurrences_c, offset, scale);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_createVertexWeightsFromVisibilityAttributes(Scene.Native.OccurrenceList_c occurrences, System.Double offset, System.Double scale);
		/// <summary>
		/// Use visibility attributes on meshes of the given occurrence to create vertex weights attributes used by the decimation functions. The finals weights will be computed with w = offset + (visibility/maxVisibility) * scale
		/// </summary>
		/// <param name="occurrences">Occurrences of components to process</param>
		/// <param name="offset">Offset value for weight computation</param>
		/// <param name="scale">Scale value for weight computation</param>
		public static void CreateVertexWeightsFromVisibilityAttributes(Scene.Native.OccurrenceList occurrences, System.Double offset, System.Double scale) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_createVertexWeightsFromVisibilityAttributes(occurrences_c, offset, scale);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region visibility

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_createVisibilityAttributes(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Create visibility attributes on tessellations
		/// </summary>
		/// <param name="occurrences">Occurrences of components to create attributes</param>
		public static void CreateVisibilityAttributes(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_createVisibilityAttributes(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_deleteVisibilityAttributes(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Delete visibility attributes on tessellations
		/// </summary>
		/// <param name="occurrences">Occurrences of components to delete attributes</param>
		public static void DeleteVisibilityAttributes(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_deleteVisibilityAttributes(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Algo_flagVisibilityAttributesOnTransparents(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Add one count to all visiblility attributes (poly and patch) on transparent patches
		/// </summary>
		/// <param name="occurrences">Occurrences of components to create attributes</param>
		public static void FlagVisibilityAttributesOnTransparents(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			Algo_flagVisibilityAttributesOnTransparents(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Algo_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

	}
}
