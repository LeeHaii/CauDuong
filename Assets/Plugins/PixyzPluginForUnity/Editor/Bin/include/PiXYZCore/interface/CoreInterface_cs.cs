#pragma warning disable CA2101

using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Security;
using System.Collections.Concurrent;
using PiXYZAPI = Pixyz.API.Native.NativeInterface;

namespace Pixyz.Core.Native {

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
		internal static extern void Core_StringPair_init(ref StringPair_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_StringPair_free(ref StringPair_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_StringPairList_init(ref StringPairList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_StringPairList_free(ref StringPairList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_AutoValueReturn_init(ref AutoValueReturn_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_AutoValueReturn_free(ref AutoValueReturn_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_Color_init(ref Color_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_Color_free(ref Color_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ColorList_init(ref ColorList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ColorList_free(ref ColorList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_IdentPair_init(ref IdentPair_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_IdentPair_free(ref IdentPair_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_IdentPairList_init(ref IdentPairList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_IdentPairList_free(ref IdentPairList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ColorAlpha_init(ref ColorAlpha_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ColorAlpha_free(ref ColorAlpha_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ColorAlphaList_init(ref ColorAlphaList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ColorAlphaList_free(ref ColorAlphaList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_Date_init(ref Date_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_Date_free(ref Date_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_LicenseInfos_init(ref LicenseInfos_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_LicenseInfos_free(ref LicenseInfos_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_StringPairListList_init(ref StringPairListList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_StringPairListList_free(ref StringPairListList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_EntityList_init(ref EntityList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_EntityList_free(ref EntityList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_WebLicenseInfo_init(ref WebLicenseInfo_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_WebLicenseInfo_free(ref WebLicenseInfo_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_WebLicenseInfoList_init(ref WebLicenseInfoList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_WebLicenseInfoList_free(ref WebLicenseInfoList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_IntList_init(ref IntList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_IntList_free(ref IntList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_StringList_init(ref StringList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_StringList_free(ref StringList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_EnumTypeDesc_init(ref EnumTypeDesc_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_EnumTypeDesc_free(ref EnumTypeDesc_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ListTypeDesc_init(ref ListTypeDesc_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ListTypeDesc_free(ref ListTypeDesc_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_EntityListList_init(ref EntityListList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_EntityListList_free(ref EntityListList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_DoubleList_init(ref DoubleList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_DoubleList_free(ref DoubleList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_BaseTypeDesc_init(ref BaseTypeDesc_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_BaseTypeDesc_free(ref BaseTypeDesc_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_AliasTypeDesc_init(ref AliasTypeDesc_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_AliasTypeDesc_free(ref AliasTypeDesc_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ArrayTypeDesc_init(ref ArrayTypeDesc_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ArrayTypeDesc_free(ref ArrayTypeDesc_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_Field_init(ref Field_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_Field_free(ref Field_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_FieldList_init(ref FieldList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_FieldList_free(ref FieldList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_StructTypeDesc_init(ref StructTypeDesc_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_StructTypeDesc_free(ref StructTypeDesc_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_SelectTypeDesc_init(ref SelectTypeDesc_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_SelectTypeDesc_free(ref SelectTypeDesc_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_TypeDesc_init(ref TypeDesc_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_TypeDesc_free(ref TypeDesc_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ParameterDesc_init(ref ParameterDesc_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ParameterDesc_free(ref ParameterDesc_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ParameterDescList_init(ref ParameterDescList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ParameterDescList_free(ref ParameterDescList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_FunctionDesc_init(ref FunctionDesc_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_FunctionDesc_free(ref FunctionDesc_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_FunctionDescList_init(ref FunctionDescList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_FunctionDescList_free(ref FunctionDescList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_GroupDesc_init(ref GroupDesc_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_GroupDesc_free(ref GroupDesc_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_GroupDescList_init(ref GroupDescList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_GroupDescList_free(ref GroupDescList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ModuleDesc_init(ref ModuleDesc_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ModuleDesc_free(ref ModuleDesc_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ModuleDescList_init(ref ModuleDescList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ModuleDescList_free(ref ModuleDescList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_DoubleListList_init(ref DoubleListList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_DoubleListList_free(ref DoubleListList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_EventDesc_init(ref EventDesc_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_EventDesc_free(ref EventDesc_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ULongList_init(ref ULongList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ULongList_free(ref ULongList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_InheritableBoolList_init(ref InheritableBoolList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_InheritableBoolList_free(ref InheritableBoolList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ByteList_init(ref ByteList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_ByteList_free(ref ByteList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_IntListList_init(ref IntListList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_IntListList_free(ref IntListList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_BoolList_init(ref BoolList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_BoolList_free(ref BoolList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_FilePathList_init(ref FilePathList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Core_FilePathList_free(ref FilePathList_c list);

	public static StringPair ConvertValue(ref StringPair_c s) {
		StringPair ss = new StringPair();
		ss.key = ConvertValue(s.key);
		ss.value = ConvertValue(s.value);
		return ss;
	}

	public static StringPair_c ConvertValue(StringPair s, ref StringPair_c ss) {
		Core.Native.NativeInterface.Core_StringPair_init(ref ss);
		ss.key = ConvertValue(s.key);
		ss.value = ConvertValue(s.value);
		return ss;
	}

	public static StringPairList ConvertValue(ref StringPairList_c s) {
		StringPairList list = new StringPairList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(StringPair_c)));
			StringPair_c value = (StringPair_c)Marshal.PtrToStructure(p, typeof(StringPair_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static StringPairList_c ConvertValue(StringPairList s, ref StringPairList_c list) {
		Core.Native.NativeInterface.Core_StringPairList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			StringPair_c elt = new StringPair_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(StringPair_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static AutoValueReturn ConvertValue(ref AutoValueReturn_c s) {
		AutoValueReturn ss = new AutoValueReturn();
		ss.values = ConvertValue(ref s.values);
		ss.message = ConvertValue(s.message);
		return ss;
	}

	public static AutoValueReturn_c ConvertValue(AutoValueReturn s, ref AutoValueReturn_c ss) {
		Core.Native.NativeInterface.Core_AutoValueReturn_init(ref ss);
		ConvertValue(s.values, ref ss.values);
		ss.message = ConvertValue(s.message);
		return ss;
	}

	public static Color ConvertValue(ref Color_c s) {
		Color ss = new Color();
		ss.r = (System.Double)s.r;
		ss.g = (System.Double)s.g;
		ss.b = (System.Double)s.b;
		return ss;
	}

	public static Color_c ConvertValue(Color s, ref Color_c ss) {
		Core.Native.NativeInterface.Core_Color_init(ref ss);
		ss.r = (System.Double)s.r;
		ss.g = (System.Double)s.g;
		ss.b = (System.Double)s.b;
		return ss;
	}

	public static ColorList ConvertValue(ref ColorList_c s) {
		ColorList list = new ColorList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<Color>(s.ptr, (int)s.size);
		return list;
	}

	public static ColorList_c ConvertValue(ColorList s, ref ColorList_c list) {
		Core.Native.NativeInterface.Core_ColorList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			Color_c elt = new Color_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Color_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static IdentPair ConvertValue(ref IdentPair_c s) {
		IdentPair ss = new IdentPair();
		ss.key = (System.UInt32)s.key;
		ss.value = (System.UInt32)s.value;
		return ss;
	}

	public static IdentPair_c ConvertValue(IdentPair s, ref IdentPair_c ss) {
		Core.Native.NativeInterface.Core_IdentPair_init(ref ss);
		ss.key = (System.UInt32)s.key;
		ss.value = (System.UInt32)s.value;
		return ss;
	}

	public static IdentPairList ConvertValue(ref IdentPairList_c s) {
		IdentPairList list = new IdentPairList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<IdentPair>(s.ptr, (int)s.size);
		return list;
	}

	public static IdentPairList_c ConvertValue(IdentPairList s, ref IdentPairList_c list) {
		Core.Native.NativeInterface.Core_IdentPairList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			IdentPair_c elt = new IdentPair_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(IdentPair_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static ColorAlpha ConvertValue(ref ColorAlpha_c s) {
		ColorAlpha ss = new ColorAlpha();
		ss.r = (System.Double)s.r;
		ss.g = (System.Double)s.g;
		ss.b = (System.Double)s.b;
		ss.a = (System.Double)s.a;
		return ss;
	}

	public static ColorAlpha_c ConvertValue(ColorAlpha s, ref ColorAlpha_c ss) {
		Core.Native.NativeInterface.Core_ColorAlpha_init(ref ss);
		ss.r = (System.Double)s.r;
		ss.g = (System.Double)s.g;
		ss.b = (System.Double)s.b;
		ss.a = (System.Double)s.a;
		return ss;
	}

	public static ColorAlphaList ConvertValue(ref ColorAlphaList_c s) {
		ColorAlphaList list = new ColorAlphaList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<ColorAlpha>(s.ptr, (int)s.size);
		return list;
	}

	public static ColorAlphaList_c ConvertValue(ColorAlphaList s, ref ColorAlphaList_c list) {
		Core.Native.NativeInterface.Core_ColorAlphaList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			ColorAlpha_c elt = new ColorAlpha_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(ColorAlpha_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static Date ConvertValue(ref Date_c s) {
		Date ss = new Date();
		ss.year = (System.Int32)s.year;
		ss.month = (System.Int32)s.month;
		ss.day = (System.Int32)s.day;
		return ss;
	}

	public static Date_c ConvertValue(Date s, ref Date_c ss) {
		Core.Native.NativeInterface.Core_Date_init(ref ss);
		ss.year = (Int32)s.year;
		ss.month = (Int32)s.month;
		ss.day = (Int32)s.day;
		return ss;
	}

	public static LicenseInfos ConvertValue(ref LicenseInfos_c s) {
		LicenseInfos ss = new LicenseInfos();
		ss.version = ConvertValue(s.version);
		ss.customerName = ConvertValue(s.customerName);
		ss.customerCompany = ConvertValue(s.customerCompany);
		ss.customerEmail = ConvertValue(s.customerEmail);
		ss.startDate = ConvertValue(ref s.startDate);
		ss.endDate = ConvertValue(ref s.endDate);
		return ss;
	}

	public static LicenseInfos_c ConvertValue(LicenseInfos s, ref LicenseInfos_c ss) {
		Core.Native.NativeInterface.Core_LicenseInfos_init(ref ss);
		ss.version = ConvertValue(s.version);
		ss.customerName = ConvertValue(s.customerName);
		ss.customerCompany = ConvertValue(s.customerCompany);
		ss.customerEmail = ConvertValue(s.customerEmail);
		ConvertValue(s.startDate, ref ss.startDate);
		ConvertValue(s.endDate, ref ss.endDate);
		return ss;
	}

	public static StringPairListList ConvertValue(ref StringPairListList_c s) {
		StringPairListList list = new StringPairListList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(StringPairList_c)));
			StringPairList_c value = (StringPairList_c)Marshal.PtrToStructure(p, typeof(StringPairList_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static StringPairListList_c ConvertValue(StringPairListList s, ref StringPairListList_c list) {
		Core.Native.NativeInterface.Core_StringPairListList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			StringPairList_c elt = new StringPairList_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(StringPairList_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static EntityList ConvertValue(ref EntityList_c s) {
		EntityList list = new EntityList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static EntityList_c ConvertValue(EntityList s, ref EntityList_c list) {
		Core.Native.NativeInterface.Core_EntityList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static WebLicenseInfo ConvertValue(ref WebLicenseInfo_c s) {
		WebLicenseInfo ss = new WebLicenseInfo();
		ss.id = (System.UInt32)s.id;
		ss.product = ConvertValue(s.product);
		ss.validity = ConvertValue(ref s.validity);
		ss.count = (System.Int32)s.count;
		ss.inUse = (System.Int32)s.inUse;
		ss.onMachine = ConvertValue(s.onMachine);
		ss.current = ConvertValue(s.current);
		return ss;
	}

	public static WebLicenseInfo_c ConvertValue(WebLicenseInfo s, ref WebLicenseInfo_c ss) {
		Core.Native.NativeInterface.Core_WebLicenseInfo_init(ref ss);
		ss.id = (System.UInt32)s.id;
		ss.product = ConvertValue(s.product);
		ConvertValue(s.validity, ref ss.validity);
		ss.count = (Int32)s.count;
		ss.inUse = (Int32)s.inUse;
		ss.onMachine = ConvertValue(s.onMachine);
		ss.current = ConvertValue(s.current);
		return ss;
	}

	public static WebLicenseInfoList ConvertValue(ref WebLicenseInfoList_c s) {
		WebLicenseInfoList list = new WebLicenseInfoList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(WebLicenseInfo_c)));
			WebLicenseInfo_c value = (WebLicenseInfo_c)Marshal.PtrToStructure(p, typeof(WebLicenseInfo_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static WebLicenseInfoList_c ConvertValue(WebLicenseInfoList s, ref WebLicenseInfoList_c list) {
		Core.Native.NativeInterface.Core_WebLicenseInfoList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			WebLicenseInfo_c elt = new WebLicenseInfo_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(WebLicenseInfo_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static IntList ConvertValue(ref IntList_c s) {
		IntList list = new IntList((int)s.size);
		if (s.size > 0)
			Marshal.Copy(s.ptr, list.list, 0, (int)s.size);
		return list;
	}

	public static IntList_c ConvertValue(IntList s, ref IntList_c list) {
		Core.Native.NativeInterface.Core_IntList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size > 0)
			Marshal.Copy(s.list, 0, list.ptr, (int)list.size);
		return list;
	}

	public static StringList ConvertValue(ref StringList_c s) {
		StringList list = new StringList((int)s.size);
		if (s.size==0) return list;
		IntPtr[] tab = new IntPtr[s.size];
		Marshal.Copy(s.ptr, tab, 0, (int)s.size);
		for (int i = 0; i < (int)s.size; ++i) {
			list.list[i] = ConvertValue(tab[i]);
		}
		return list;
	}

	public static StringList_c ConvertValue(StringList s, ref StringList_c list) {
		Core.Native.NativeInterface.Core_StringList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		IntPtr[] tab = new IntPtr[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = ConvertValue(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static EnumTypeDesc ConvertValue(ref EnumTypeDesc_c s) {
		EnumTypeDesc ss = new EnumTypeDesc();
		ss.name = ConvertValue(s.name);
		ss.module = ConvertValue(s.module);
		ss.description = ConvertValue(s.description);
		ss.values = ConvertValue(ref s.values);
		ss.labels = ConvertValue(ref s.labels);
		ss.valueType = ConvertValue(s.valueType);
		return ss;
	}

	public static EnumTypeDesc_c ConvertValue(EnumTypeDesc s, ref EnumTypeDesc_c ss) {
		Core.Native.NativeInterface.Core_EnumTypeDesc_init(ref ss);
		ss.name = ConvertValue(s.name);
		ss.module = ConvertValue(s.module);
		ss.description = ConvertValue(s.description);
		ConvertValue(s.values, ref ss.values);
		ConvertValue(s.labels, ref ss.labels);
		ss.valueType = ConvertValue(s.valueType);
		return ss;
	}

	public static ListTypeDesc ConvertValue(ref ListTypeDesc_c s) {
		ListTypeDesc ss = new ListTypeDesc();
		ss.name = ConvertValue(s.name);
		ss.module = ConvertValue(s.module);
		ss.description = ConvertValue(s.description);
		ss.listType = ConvertValue(s.listType);
		return ss;
	}

	public static ListTypeDesc_c ConvertValue(ListTypeDesc s, ref ListTypeDesc_c ss) {
		Core.Native.NativeInterface.Core_ListTypeDesc_init(ref ss);
		ss.name = ConvertValue(s.name);
		ss.module = ConvertValue(s.module);
		ss.description = ConvertValue(s.description);
		ss.listType = ConvertValue(s.listType);
		return ss;
	}

	public static EntityListList ConvertValue(ref EntityListList_c s) {
		EntityListList list = new EntityListList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(EntityList_c)));
			EntityList_c value = (EntityList_c)Marshal.PtrToStructure(p, typeof(EntityList_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static EntityListList_c ConvertValue(EntityListList s, ref EntityListList_c list) {
		Core.Native.NativeInterface.Core_EntityListList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			EntityList_c elt = new EntityList_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(EntityList_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static DoubleList ConvertValue(ref DoubleList_c s) {
		DoubleList list = new DoubleList((int)s.size);
		if (s.size > 0)
			Marshal.Copy(s.ptr, list.list, 0, (int)s.size);
		return list;
	}

	public static DoubleList_c ConvertValue(DoubleList s, ref DoubleList_c list) {
		Core.Native.NativeInterface.Core_DoubleList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size > 0)
			Marshal.Copy(s.list, 0, list.ptr, (int)list.size);
		return list;
	}

	public static BaseTypeDesc ConvertValue(ref BaseTypeDesc_c s) {
		BaseTypeDesc ss = new BaseTypeDesc();
		ss.name = ConvertValue(s.name);
		ss.module = ConvertValue(s.module);
		ss.description = ConvertValue(s.description);
		return ss;
	}

	public static BaseTypeDesc_c ConvertValue(BaseTypeDesc s, ref BaseTypeDesc_c ss) {
		Core.Native.NativeInterface.Core_BaseTypeDesc_init(ref ss);
		ss.name = ConvertValue(s.name);
		ss.module = ConvertValue(s.module);
		ss.description = ConvertValue(s.description);
		return ss;
	}

	public static AliasTypeDesc ConvertValue(ref AliasTypeDesc_c s) {
		AliasTypeDesc ss = new AliasTypeDesc();
		ss.name = ConvertValue(s.name);
		ss.module = ConvertValue(s.module);
		ss.description = ConvertValue(s.description);
		ss.alias = ConvertValue(s.alias);
		return ss;
	}

	public static AliasTypeDesc_c ConvertValue(AliasTypeDesc s, ref AliasTypeDesc_c ss) {
		Core.Native.NativeInterface.Core_AliasTypeDesc_init(ref ss);
		ss.name = ConvertValue(s.name);
		ss.module = ConvertValue(s.module);
		ss.description = ConvertValue(s.description);
		ss.alias = ConvertValue(s.alias);
		return ss;
	}

	public static ArrayTypeDesc ConvertValue(ref ArrayTypeDesc_c s) {
		ArrayTypeDesc ss = new ArrayTypeDesc();
		ss.name = ConvertValue(s.name);
		ss.module = ConvertValue(s.module);
		ss.description = ConvertValue(s.description);
		ss.arrayType = ConvertValue(s.arrayType);
		return ss;
	}

	public static ArrayTypeDesc_c ConvertValue(ArrayTypeDesc s, ref ArrayTypeDesc_c ss) {
		Core.Native.NativeInterface.Core_ArrayTypeDesc_init(ref ss);
		ss.name = ConvertValue(s.name);
		ss.module = ConvertValue(s.module);
		ss.description = ConvertValue(s.description);
		ss.arrayType = ConvertValue(s.arrayType);
		return ss;
	}

	public static Field ConvertValue(ref Field_c s) {
		Field ss = new Field();
		ss.name = ConvertValue(s.name);
		ss.type = ConvertValue(s.type);
		ss.defaultValue = ConvertValue(s.defaultValue);
		ss.description = ConvertValue(s.description);
		ss.predefinedValues = ConvertValue(ref s.predefinedValues);
		return ss;
	}

	public static Field_c ConvertValue(Field s, ref Field_c ss) {
		Core.Native.NativeInterface.Core_Field_init(ref ss);
		ss.name = ConvertValue(s.name);
		ss.type = ConvertValue(s.type);
		ss.defaultValue = ConvertValue(s.defaultValue);
		ss.description = ConvertValue(s.description);
		ConvertValue(s.predefinedValues, ref ss.predefinedValues);
		return ss;
	}

	public static FieldList ConvertValue(ref FieldList_c s) {
		FieldList list = new FieldList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Field_c)));
			Field_c value = (Field_c)Marshal.PtrToStructure(p, typeof(Field_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static FieldList_c ConvertValue(FieldList s, ref FieldList_c list) {
		Core.Native.NativeInterface.Core_FieldList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			Field_c elt = new Field_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Field_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static StructTypeDesc ConvertValue(ref StructTypeDesc_c s) {
		StructTypeDesc ss = new StructTypeDesc();
		ss.name = ConvertValue(s.name);
		ss.module = ConvertValue(s.module);
		ss.description = ConvertValue(s.description);
		ss.type = (TypeDescType)s.type;
		ss.fields = ConvertValue(ref s.fields);
		return ss;
	}

	public static StructTypeDesc_c ConvertValue(StructTypeDesc s, ref StructTypeDesc_c ss) {
		Core.Native.NativeInterface.Core_StructTypeDesc_init(ref ss);
		ss.name = ConvertValue(s.name);
		ss.module = ConvertValue(s.module);
		ss.description = ConvertValue(s.description);
		ss.type = (Int32)s.type;
		ConvertValue(s.fields, ref ss.fields);
		return ss;
	}

	public static SelectTypeDesc ConvertValue(ref SelectTypeDesc_c s) {
		SelectTypeDesc ss = new SelectTypeDesc();
		ss.name = ConvertValue(s.name);
		ss.module = ConvertValue(s.module);
		ss.description = ConvertValue(s.description);
		ss.type = (TypeDescType)s.type;
		return ss;
	}

	public static SelectTypeDesc_c ConvertValue(SelectTypeDesc s, ref SelectTypeDesc_c ss) {
		Core.Native.NativeInterface.Core_SelectTypeDesc_init(ref ss);
		ss.name = ConvertValue(s.name);
		ss.module = ConvertValue(s.module);
		ss.description = ConvertValue(s.description);
		ss.type = (Int32)s.type;
		return ss;
	}

	public static TypeDesc ConvertValue(ref TypeDesc_c s) {
		TypeDesc ss = new TypeDesc();
		ss._type = (TypeDesc.Type)s._type;
		switch(ss._type) {
			case TypeDesc.Type.UNKNOWN: break;
			case TypeDesc.Type.BASETYPE: ss.baseType = ConvertValue(ref s.baseType); break;
			case TypeDesc.Type.ALIASTYPE: ss.aliasType = ConvertValue(ref s.aliasType); break;
			case TypeDesc.Type.ENUMTYPE: ss.enumType = ConvertValue(ref s.enumType); break;
			case TypeDesc.Type.LISTTYPE: ss.listType = ConvertValue(ref s.listType); break;
			case TypeDesc.Type.ARRAYTYPE: ss.arrayType = ConvertValue(ref s.arrayType); break;
			case TypeDesc.Type.STRUCTTYPE: ss.structType = ConvertValue(ref s.structType); break;
			case TypeDesc.Type.SELECTTYPE: ss.selectType = ConvertValue(ref s.selectType); break;
		}
		return ss;
	}

	public static TypeDesc_c ConvertValue(TypeDesc s, ref TypeDesc_c ss) {
		Core.Native.NativeInterface.Core_TypeDesc_init(ref ss);
		ss._type = (int)s._type;
		switch (ss._type) {
			case 0: break;
			case 1: ConvertValue(s.baseType, ref ss.baseType); break;
			case 2: ConvertValue(s.aliasType, ref ss.aliasType); break;
			case 3: ConvertValue(s.enumType, ref ss.enumType); break;
			case 4: ConvertValue(s.listType, ref ss.listType); break;
			case 5: ConvertValue(s.arrayType, ref ss.arrayType); break;
			case 6: ConvertValue(s.structType, ref ss.structType); break;
			case 7: ConvertValue(s.selectType, ref ss.selectType); break;
		}
		return ss;
	}

	public static ParameterDesc ConvertValue(ref ParameterDesc_c s) {
		ParameterDesc ss = new ParameterDesc();
		ss.name = ConvertValue(s.name);
		ss.type = ConvertValue(ref s.type);
		ss.description = ConvertValue(s.description);
		ss.optional = ConvertValue(s.optional);
		ss.defaultValue = ConvertValue(s.defaultValue);
		return ss;
	}

	public static ParameterDesc_c ConvertValue(ParameterDesc s, ref ParameterDesc_c ss) {
		Core.Native.NativeInterface.Core_ParameterDesc_init(ref ss);
		ss.name = ConvertValue(s.name);
		ConvertValue(s.type, ref ss.type);
		ss.description = ConvertValue(s.description);
		ss.optional = ConvertValue(s.optional);
		ss.defaultValue = ConvertValue(s.defaultValue);
		return ss;
	}

	public static ParameterDescList ConvertValue(ref ParameterDescList_c s) {
		ParameterDescList list = new ParameterDescList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(ParameterDesc_c)));
			ParameterDesc_c value = (ParameterDesc_c)Marshal.PtrToStructure(p, typeof(ParameterDesc_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static ParameterDescList_c ConvertValue(ParameterDescList s, ref ParameterDescList_c list) {
		Core.Native.NativeInterface.Core_ParameterDescList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			ParameterDesc_c elt = new ParameterDesc_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(ParameterDesc_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static FunctionDesc ConvertValue(ref FunctionDesc_c s) {
		FunctionDesc ss = new FunctionDesc();
		ss.name = ConvertValue(s.name);
		ss.module = ConvertValue(s.module);
		ss.parameters = ConvertValue(ref s.parameters);
		ss.returns = ConvertValue(ref s.returns);
		return ss;
	}

	public static FunctionDesc_c ConvertValue(FunctionDesc s, ref FunctionDesc_c ss) {
		Core.Native.NativeInterface.Core_FunctionDesc_init(ref ss);
		ss.name = ConvertValue(s.name);
		ss.module = ConvertValue(s.module);
		ConvertValue(s.parameters, ref ss.parameters);
		ConvertValue(s.returns, ref ss.returns);
		return ss;
	}

	public static FunctionDescList ConvertValue(ref FunctionDescList_c s) {
		FunctionDescList list = new FunctionDescList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(FunctionDesc_c)));
			FunctionDesc_c value = (FunctionDesc_c)Marshal.PtrToStructure(p, typeof(FunctionDesc_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static FunctionDescList_c ConvertValue(FunctionDescList s, ref FunctionDescList_c list) {
		Core.Native.NativeInterface.Core_FunctionDescList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			FunctionDesc_c elt = new FunctionDesc_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(FunctionDesc_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static GroupDesc ConvertValue(ref GroupDesc_c s) {
		GroupDesc ss = new GroupDesc();
		ss.name = ConvertValue(s.name);
		ss.description = ConvertValue(s.description);
		ss.functions = ConvertValue(ref s.functions);
		return ss;
	}

	public static GroupDesc_c ConvertValue(GroupDesc s, ref GroupDesc_c ss) {
		Core.Native.NativeInterface.Core_GroupDesc_init(ref ss);
		ss.name = ConvertValue(s.name);
		ss.description = ConvertValue(s.description);
		ConvertValue(s.functions, ref ss.functions);
		return ss;
	}

	public static GroupDescList ConvertValue(ref GroupDescList_c s) {
		GroupDescList list = new GroupDescList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(GroupDesc_c)));
			GroupDesc_c value = (GroupDesc_c)Marshal.PtrToStructure(p, typeof(GroupDesc_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static GroupDescList_c ConvertValue(GroupDescList s, ref GroupDescList_c list) {
		Core.Native.NativeInterface.Core_GroupDescList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			GroupDesc_c elt = new GroupDesc_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(GroupDesc_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static ModuleDesc ConvertValue(ref ModuleDesc_c s) {
		ModuleDesc ss = new ModuleDesc();
		ss.name = ConvertValue(s.name);
		ss.description = ConvertValue(s.description);
		ss.author = ConvertValue(s.author);
		ss.groups = ConvertValue(ref s.groups);
		return ss;
	}

	public static ModuleDesc_c ConvertValue(ModuleDesc s, ref ModuleDesc_c ss) {
		Core.Native.NativeInterface.Core_ModuleDesc_init(ref ss);
		ss.name = ConvertValue(s.name);
		ss.description = ConvertValue(s.description);
		ss.author = ConvertValue(s.author);
		ConvertValue(s.groups, ref ss.groups);
		return ss;
	}

	public static ModuleDescList ConvertValue(ref ModuleDescList_c s) {
		ModuleDescList list = new ModuleDescList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(ModuleDesc_c)));
			ModuleDesc_c value = (ModuleDesc_c)Marshal.PtrToStructure(p, typeof(ModuleDesc_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static ModuleDescList_c ConvertValue(ModuleDescList s, ref ModuleDescList_c list) {
		Core.Native.NativeInterface.Core_ModuleDescList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			ModuleDesc_c elt = new ModuleDesc_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(ModuleDesc_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static DoubleListList ConvertValue(ref DoubleListList_c s) {
		DoubleListList list = new DoubleListList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(DoubleList_c)));
			DoubleList_c value = (DoubleList_c)Marshal.PtrToStructure(p, typeof(DoubleList_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static DoubleListList_c ConvertValue(DoubleListList s, ref DoubleListList_c list) {
		Core.Native.NativeInterface.Core_DoubleListList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			DoubleList_c elt = new DoubleList_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(DoubleList_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static EventDesc ConvertValue(ref EventDesc_c s) {
		EventDesc ss = new EventDesc();
		ss.module = ConvertValue(s.module);
		ss.name = ConvertValue(s.name);
		ss.description = ConvertValue(s.description);
		ss.parameters = ConvertValue(ref s.parameters);
		return ss;
	}

	public static EventDesc_c ConvertValue(EventDesc s, ref EventDesc_c ss) {
		Core.Native.NativeInterface.Core_EventDesc_init(ref ss);
		ss.module = ConvertValue(s.module);
		ss.name = ConvertValue(s.name);
		ss.description = ConvertValue(s.description);
		ConvertValue(s.parameters, ref ss.parameters);
		return ss;
	}

	public static ULongList ConvertValue(ref ULongList_c s) {
		ULongList list = new ULongList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt64>(s.ptr, (int)s.size);
		return list;
	}

	public static ULongList_c ConvertValue(ULongList s, ref ULongList_c list) {
		Core.Native.NativeInterface.Core_ULongList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		long[] tab = new long[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (long)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static InheritableBoolList ConvertValue(ref InheritableBoolList_c s) {
		InheritableBoolList list = new InheritableBoolList((int)s.size);
		if (s.size==0) return list;
		int[] tab = new int[s.size];
		Marshal.Copy(s.ptr, tab, 0, (int)s.size);
		for (int i = 0; i < (int)s.size; ++i) {
			list.list[i] = (InheritableBool)tab[i];
		}
		return list;
	}

	public static InheritableBoolList_c ConvertValue(InheritableBoolList s, ref InheritableBoolList_c list) {
		Core.Native.NativeInterface.Core_InheritableBoolList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)s.list[i];
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static ByteList ConvertValue(ref ByteList_c s) {
		ByteList list = new ByteList((int)s.size);
		if (s.size > 0)
			Marshal.Copy(s.ptr, list.list, 0, (int)s.size);
		return list;
	}

	public static ByteList_c ConvertValue(ByteList s, ref ByteList_c list) {
		Core.Native.NativeInterface.Core_ByteList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size > 0)
			Marshal.Copy(s.list, 0, list.ptr, (int)list.size);
		return list;
	}

	public static IntListList ConvertValue(ref IntListList_c s) {
		IntListList list = new IntListList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(IntList_c)));
			IntList_c value = (IntList_c)Marshal.PtrToStructure(p, typeof(IntList_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static IntListList_c ConvertValue(IntListList s, ref IntListList_c list) {
		Core.Native.NativeInterface.Core_IntListList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			IntList_c elt = new IntList_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(IntList_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static BoolList ConvertValue(ref BoolList_c s) {
		BoolList list = new BoolList((int)s.size);
		if (s.size==0) return list;
		int[] tab = new int[s.size];
		Marshal.Copy(s.ptr, tab, 0, (int)s.size);
			list.list = CopyMemory<System.Boolean>(s.ptr, (int)s.size);
		return list;
	}

	public static BoolList_c ConvertValue(BoolList s, ref BoolList_c list) {
		Core.Native.NativeInterface.Core_BoolList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = ConvertValue(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static FilePathList ConvertValue(ref FilePathList_c s) {
		FilePathList list = new FilePathList((int)s.size);
		if (s.size==0) return list;
		IntPtr[] tab = new IntPtr[s.size];
		Marshal.Copy(s.ptr, tab, 0, (int)s.size);
		for (int i = 0; i < (int)s.size; ++i) {
			list.list[i] = ConvertValue(tab[i]);
		}
		return list;
	}

	public static FilePathList_c ConvertValue(FilePathList s, ref FilePathList_c list) {
		Core.Native.NativeInterface.Core_FilePathList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		IntPtr[] tab = new IntPtr[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = ConvertValue(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static getLicenseServerReturn ConvertValue(ref getLicenseServerReturn_c s) {
		getLicenseServerReturn ss = new getLicenseServerReturn();
		ss.serverHost = ConvertValue(s.serverHost);
		ss.serverPort = (System.UInt16)s.serverPort;
		ss.useFlexLM = ConvertValue(s.useFlexLM);
		return ss;
	}

	public static getLicenseServerReturn_c ConvertValue(getLicenseServerReturn s, ref getLicenseServerReturn_c ss) {
		ss.serverHost = ConvertValue(s.serverHost);
		ss.serverPort = (System.UInt16)s.serverPort;
		ss.useFlexLM = ConvertValue(s.useFlexLM);
		return ss;
	}

	public static availableMemoryReturn ConvertValue(ref availableMemoryReturn_c s) {
		availableMemoryReturn ss = new availableMemoryReturn();
		ss.availVirt = (System.Int64)s.availVirt;
		ss.totalVirt = (System.Int64)s.totalVirt;
		ss.availPhys = (System.Int64)s.availPhys;
		ss.totalPhys = (System.Int64)s.totalPhys;
		return ss;
	}

	public static availableMemoryReturn_c ConvertValue(availableMemoryReturn s, ref availableMemoryReturn_c ss) {
		ss.availVirt = (System.Int64)s.availVirt;
		ss.totalVirt = (System.Int64)s.totalVirt;
		ss.availPhys = (System.Int64)s.availPhys;
		ss.totalPhys = (System.Int64)s.totalPhys;
		return ss;
	}

	public static checkForUpdatesReturn ConvertValue(ref checkForUpdatesReturn_c s) {
		checkForUpdatesReturn ss = new checkForUpdatesReturn();
		ss.newVersionAvailable = ConvertValue(s.newVersionAvailable);
		ss.newVersion = ConvertValue(s.newVersion);
		ss.newVersionLink = ConvertValue(s.newVersionLink);
		return ss;
	}

	public static checkForUpdatesReturn_c ConvertValue(checkForUpdatesReturn s, ref checkForUpdatesReturn_c ss) {
		ss.newVersionAvailable = ConvertValue(s.newVersionAvailable);
		ss.newVersion = ConvertValue(s.newVersion);
		ss.newVersionLink = ConvertValue(s.newVersionLink);
		return ss;
	}

	public static getInterfaceLoggerConfigurationReturn ConvertValue(ref getInterfaceLoggerConfigurationReturn_c s) {
		getInterfaceLoggerConfigurationReturn ss = new getInterfaceLoggerConfigurationReturn();
		ss.functionEnabled = ConvertValue(s.functionEnabled);
		ss.parametersEnabled = ConvertValue(s.parametersEnabled);
		ss.executionTimeEnabled = ConvertValue(s.executionTimeEnabled);
		return ss;
	}

	public static getInterfaceLoggerConfigurationReturn_c ConvertValue(getInterfaceLoggerConfigurationReturn s, ref getInterfaceLoggerConfigurationReturn_c ss) {
		ss.functionEnabled = ConvertValue(s.functionEnabled);
		ss.parametersEnabled = ConvertValue(s.parametersEnabled);
		ss.executionTimeEnabled = ConvertValue(s.executionTimeEnabled);
		return ss;
	}

		#endregion

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Core_getLastError();

		public static string GetLastError()
		{
			return ConvertValue(Core_getLastError());
		}

		#region 

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Core_cloneEntity(System.UInt32 entity);
		/// <summary>
		/// Clone an entity
		/// </summary>
		/// <param name="entity">The entity to clone</param>
		public static System.UInt32 CloneEntity(System.UInt32 entity) {
			var ret = Core_cloneEntity(entity);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_crash();
		/// <summary>
		/// The only function in Pixyz to crash except undo/redo
		/// </summary>
		public static void Crash() {
			Core_crash();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_deleteEntities(EntityList_c entities);
		/// <summary>
		/// Delete a set of entities
		/// </summary>
		/// <param name="entities">List of entity to delete</param>
		public static void DeleteEntities(EntityList entities) {
			var entities_c = new Core.Native.EntityList_c();
			ConvertValue(entities, ref entities_c);
			Core_deleteEntities(entities_c);
			Core.Native.NativeInterface.Core_EntityList_free(ref entities_c);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_generateHTMLDocumentation(string output);
		/// <summary>
		/// Generate the documentation of available functions in HTML format
		/// </summary>
		/// <param name="output">Path to the output directory</param>
		public static void GenerateHTMLDocumentation(System.String output) {
			Core_generateHTMLDocumentation(output);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_resetSession();
		/// <summary>
		/// Clear all the current session (all unsaved work will be lost)
		/// </summary>
		public static void ResetSession() {
			Core_resetSession();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Core_unsavedUserChanges();
		/// <summary>
		/// Returns true if the user has made changes to the project
		/// </summary>
		public static System.Boolean UnsavedUserChanges() {
			var ret = Core_unsavedUserChanges();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Boolean)(0 != ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_updateDocumentation();
		/// <summary>
		/// Update the documentation of available functions and plugins in HTML format
		/// </summary>
		public static void UpdateDocumentation() {
			Core_updateDocumentation();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		private delegate void AtExitDelegate(System.IntPtr userdata);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Core_addAtExitCallback(AtExitDelegate callback, System.IntPtr userdata);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_removeAtExitCallback(System.UInt32 id);

		private static System.UInt32 addAtExitCallback(AtExitDelegate callback, System.IntPtr userdata) {
			return Core_addAtExitCallback(callback, userdata);
		}

		private static void removeAtExitCallback(System.UInt32 id) {
			Core_removeAtExitCallback(id);
		}

		public class AtExitTask : PiXYZAPI.PixyzCallbackTask
		{
			public struct Result
			{
			}

			public class Execution : BaseTaskExecution<Result>
			{
				public Execution(CancellationTokenSource cancelTokenSource = null) : base(cancelTokenSource)
				{
				}

				protected override void AttachCallback()
				{
					AtExitDelegate callback = Updated;

					_delegatePtr = GCHandle.Alloc(callback, GCHandleType.Pinned);
					_callbackId = addAtExitCallback(callback, IntPtr.Zero);

				}

				protected override void DetachCallback()
				{
					if (_callbackId == 0)
						return;

					removeAtExitCallback(_callbackId);
					_callbackId = 0;
				}

				private void Updated(System.IntPtr userdata)
				{
					try
					{
						_results.Enqueue(new Result());

						if (_isContiniuous)
						{
							_isUpdated.Release();
						}
						else
						{
							DetachCallback();
							_isCompleted.Release();
						}
					}
					catch(System.Exception)
					{
							DetachCallback();
						_isCompleted.Release();
					}
				}
			}
			public static Task<Result> WaitAtExit(CancellationTokenSource cancelSource = null)
			{
				return Task.Run(new Execution(cancelSource).RunOnce);
			}
		}
		private delegate void ProgressChangedDelegate(System.IntPtr userdata, System.Int32 progress);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Core_addProgressChangedCallback(ProgressChangedDelegate callback, System.IntPtr userdata);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_removeProgressChangedCallback(System.UInt32 id);

		private static System.UInt32 addProgressChangedCallback(ProgressChangedDelegate callback, System.IntPtr userdata) {
			return Core_addProgressChangedCallback(callback, userdata);
		}

		private static void removeProgressChangedCallback(System.UInt32 id) {
			Core_removeProgressChangedCallback(id);
		}

		public class ProgressChangedTask : PiXYZAPI.PixyzCallbackTask
		{
			public struct Result
			{
				public System.Int32 progress;

				public Result(System.Int32 progress)
				{
					this.progress = progress;
				}
			}

			public class Execution : BaseTaskExecution<Result>
			{
				public Execution(CancellationTokenSource cancelTokenSource = null) : base(cancelTokenSource)
				{
				}

				protected override void AttachCallback()
				{
					ProgressChangedDelegate callback = Updated;

					_delegatePtr = GCHandle.Alloc(callback, GCHandleType.Pinned);
					_callbackId = addProgressChangedCallback(callback, IntPtr.Zero);

				}

				protected override void DetachCallback()
				{
					if (_callbackId == 0)
						return;

					removeProgressChangedCallback(_callbackId);
					_callbackId = 0;
				}

				private void Updated(System.IntPtr userdata, System.Int32 progress)
				{
					try
					{
						_results.Enqueue(new Result(progress));

						if (_isContiniuous)
						{
							_isUpdated.Release();
						}
						else
						{
							DetachCallback();
							_isCompleted.Release();
						}
					}
					catch(System.Exception)
					{
							DetachCallback();
						_isCompleted.Release();
					}
				}
			}
			public static Task<Result> WaitProgressChanged(CancellationTokenSource cancelSource = null)
			{
				return Task.Run(new Execution(cancelSource).RunOnce);
			}
		}
		private delegate void ProgressStepFinishedDelegate(System.IntPtr userdata);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Core_addProgressStepFinishedCallback(ProgressStepFinishedDelegate callback, System.IntPtr userdata);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_removeProgressStepFinishedCallback(System.UInt32 id);

		private static System.UInt32 addProgressStepFinishedCallback(ProgressStepFinishedDelegate callback, System.IntPtr userdata) {
			return Core_addProgressStepFinishedCallback(callback, userdata);
		}

		private static void removeProgressStepFinishedCallback(System.UInt32 id) {
			Core_removeProgressStepFinishedCallback(id);
		}

		public class ProgressStepFinishedTask : PiXYZAPI.PixyzCallbackTask
		{
			public struct Result
			{
			}

			public class Execution : BaseTaskExecution<Result>
			{
				public Execution(CancellationTokenSource cancelTokenSource = null) : base(cancelTokenSource)
				{
				}

				protected override void AttachCallback()
				{
					ProgressStepFinishedDelegate callback = Updated;

					_delegatePtr = GCHandle.Alloc(callback, GCHandleType.Pinned);
					_callbackId = addProgressStepFinishedCallback(callback, IntPtr.Zero);

				}

				protected override void DetachCallback()
				{
					if (_callbackId == 0)
						return;

					removeProgressStepFinishedCallback(_callbackId);
					_callbackId = 0;
				}

				private void Updated(System.IntPtr userdata)
				{
					try
					{
						_results.Enqueue(new Result());

						if (_isContiniuous)
						{
							_isUpdated.Release();
						}
						else
						{
							DetachCallback();
							_isCompleted.Release();
						}
					}
					catch(System.Exception)
					{
							DetachCallback();
						_isCompleted.Release();
					}
				}
			}
			public static Task<Result> WaitProgressStepFinished(CancellationTokenSource cancelSource = null)
			{
				return Task.Run(new Execution(cancelSource).RunOnce);
			}
		}
		private delegate void ProgressStepStartDelegate(System.IntPtr userdata, System.String stepName);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Core_addProgressStepStartCallback(ProgressStepStartDelegate callback, System.IntPtr userdata);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_removeProgressStepStartCallback(System.UInt32 id);

		private static System.UInt32 addProgressStepStartCallback(ProgressStepStartDelegate callback, System.IntPtr userdata) {
			return Core_addProgressStepStartCallback(callback, userdata);
		}

		private static void removeProgressStepStartCallback(System.UInt32 id) {
			Core_removeProgressStepStartCallback(id);
		}

		public class ProgressStepStartTask : PiXYZAPI.PixyzCallbackTask
		{
			public struct Result
			{
				public System.String stepName;

				public Result(System.String stepName)
				{
					this.stepName = stepName;
				}
			}

			public class Execution : BaseTaskExecution<Result>
			{
				public Execution(CancellationTokenSource cancelTokenSource = null) : base(cancelTokenSource)
				{
				}

				protected override void AttachCallback()
				{
					ProgressStepStartDelegate callback = Updated;

					_delegatePtr = GCHandle.Alloc(callback, GCHandleType.Pinned);
					_callbackId = addProgressStepStartCallback(callback, IntPtr.Zero);

				}

				protected override void DetachCallback()
				{
					if (_callbackId == 0)
						return;

					removeProgressStepStartCallback(_callbackId);
					_callbackId = 0;
				}

				private void Updated(System.IntPtr userdata, System.String stepName)
				{
					try
					{
						_results.Enqueue(new Result(stepName));

						if (_isContiniuous)
						{
							_isUpdated.Release();
						}
						else
						{
							DetachCallback();
							_isCompleted.Release();
						}
					}
					catch(System.Exception)
					{
							DetachCallback();
						_isCompleted.Release();
					}
				}
			}
			public static Task<Result> WaitProgressStepStart(CancellationTokenSource cancelSource = null)
			{
				return Task.Run(new Execution(cancelSource).RunOnce);
			}
		}
		#endregion

		#region Desc

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern EventDesc_c Core_getEvent(string moduleName, string eventName);
		/// <summary>
		/// get EventDesc of an event
		/// </summary>
		/// <param name="moduleName">Target module name</param>
		/// <param name="eventName"></param>
		public static EventDesc GetEvent(System.String moduleName, System.String eventName) {
			var ret = Core_getEvent(moduleName, eventName);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_EventDesc_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern FunctionDesc_c Core_getFunction(string moduleName, string functionName);
		/// <summary>
		/// get FunctionDesc of a function
		/// </summary>
		/// <param name="moduleName">Target module name</param>
		/// <param name="functionName">Target function name</param>
		public static FunctionDesc GetFunction(System.String moduleName, System.String functionName) {
			var ret = Core_getFunction(moduleName, functionName);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_FunctionDesc_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern FunctionDescList_c Core_getFunctions(string moduleName, string groupName);
		/// <summary>
		/// get functions of a group
		/// </summary>
		/// <param name="moduleName">Target module name</param>
		/// <param name="groupName">Target group name</param>
		public static FunctionDescList GetFunctions(System.String moduleName, System.String groupName) {
			var ret = Core_getFunctions(moduleName, groupName);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_FunctionDescList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern GroupDesc_c Core_getGroup(string moduleName, string groupName);
		/// <summary>
		/// get a group desc from a specific module
		/// </summary>
		/// <param name="moduleName">Target module name</param>
		/// <param name="groupName">Target group name</param>
		public static GroupDesc GetGroup(System.String moduleName, System.String groupName) {
			var ret = Core_getGroup(moduleName, groupName);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_GroupDesc_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern GroupDescList_c Core_getGroups(string moduleName);
		/// <summary>
		/// get all group desc of a module
		/// </summary>
		/// <param name="moduleName">Target module name</param>
		public static GroupDescList GetGroups(System.String moduleName) {
			var ret = Core_getGroups(moduleName);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_GroupDescList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern ModuleDescList_c Core_getModules();
		/// <summary>
		/// get all modules desc
		/// </summary>
		public static ModuleDescList GetModules() {
			var ret = Core_getModules();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_ModuleDescList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern StringList_c Core_getModulesName();
		/// <summary>
		/// get all modules name
		/// </summary>
		public static StringList GetModulesName() {
			var ret = Core_getModulesName();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_StringList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_softStopAsyncEventManager();
		/// <summary>
		/// Ask the async EventManager to join the main thread, enableEventManagerAsync must be enable
		/// </summary>
		public static void SoftStopAsyncEventManager() {
			Core_softStopAsyncEventManager();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region UI

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Core_askString(string msg, string defaultValue);
		/// <summary>
		/// Invite the user to enter a string
		/// </summary>
		/// <param name="msg">Message to display</param>
		/// <param name="defaultValue">Message to display</param>
		public static System.String AskString(System.String msg, System.String defaultValue) {
			var ret = Core_askString(msg, defaultValue);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Core_askYesNo(string question, Int32 defaultValue);
		/// <summary>
		/// Ask a question which need a Yes/No answer
		/// </summary>
		/// <param name="question">Question to display</param>
		/// <param name="defaultValue">Default value (if interfactive mode is disabled)</param>
		public static System.Boolean AskYesNo(System.String question, System.Boolean defaultValue) {
			var ret = Core_askYesNo(question, defaultValue ? 1 : 0);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Boolean)(0 != ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Core_choose(string message, StringList_c values, Int32 defaultValue);
		/// <summary>
		/// Invite the user to choose one value between multiple choice
		/// </summary>
		/// <param name="message">Message to display</param>
		/// <param name="values">Possible values to choose</param>
		/// <param name="defaultValue">Default value index</param>
		public static System.Int32 Choose(System.String message, StringList values, System.Int32 defaultValue) {
			var values_c = new Core.Native.StringList_c();
			ConvertValue(values, ref values_c);
			var ret = Core_choose(message, values_c, defaultValue);
			Core.Native.NativeInterface.Core_StringList_free(ref values_c);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Int32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Core_isInteractiveMode();
		/// <summary>
		/// Returns True if the script is in interactive mode, else returns false
		/// </summary>
		public static System.Boolean IsInteractiveMode() {
			var ret = Core_isInteractiveMode();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Boolean)(0 != ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_message(string msg);
		/// <summary>
		/// Display a message (or a MessageBox in GUI)
		/// </summary>
		/// <param name="msg">Message to display</param>
		public static void Message(System.String msg) {
			Core_message(msg);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_setInteractiveMode(Int32 interactive);
		/// <summary>
		/// Switch between interactive mode and non-interactive mode, UI functions will no ask user on non-interactive mode and will return default values
		/// </summary>
		/// <param name="interactive">True if you want to enter interactive mode, else False</param>
		public static void SetInteractiveMode(System.Boolean interactive) {
			Core_setInteractiveMode(interactive ? 1 : 0);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region database

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern EntityList_c Core_getAllEntities();
		/// <summary>
		/// returns all the entities on the database
		/// </summary>
		public static EntityList GetAllEntities() {
			var ret = Core_getAllEntities();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_EntityList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Core_getEntityType(System.UInt32 entity);
		/// <summary>
		/// returns the type id of the entity
		/// </summary>
		/// <param name="entity">The wanted entity</param>
		public static System.Int32 GetEntityType(System.UInt32 entity) {
			var ret = Core_getEntityType(entity);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Int32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Core_getEntityTypeFromString(string entityTypeString);
		/// <summary>
		/// returns the type id of the entity
		/// </summary>
		/// <param name="entityTypeString">The wanted entity type</param>
		public static System.Int32 GetEntityTypeFromString(System.String entityTypeString) {
			var ret = Core_getEntityTypeFromString(entityTypeString);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Int32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Core_getEntityTypeString(System.UInt32 entity);
		/// <summary>
		/// returns the type name of the entity
		/// </summary>
		/// <param name="entity">The wanted entity</param>
		public static System.String GetEntityTypeString(System.UInt32 entity) {
			var ret = Core_getEntityTypeString(entity);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntListList_c Core_getTypeStats();
		/// <summary>
		/// Get the database stats
		/// </summary>
		public static IntListList GetTypeStats() {
			var ret = Core_getTypeStats();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_IntListList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_load(string fileName);
		/// <summary>
		/// Load a new scene
		/// </summary>
		/// <param name="fileName">Path to load the file</param>
		public static void Load(System.String fileName) {
			Core_load(fileName);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_save(string fileName);
		/// <summary>
		/// Save the scene
		/// </summary>
		/// <param name="fileName">Path to save the file</param>
		public static void Save(System.String fileName) {
			Core_save(fileName);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region function presets

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_exportPresets(string fileName);
		/// <summary>
		/// Export all presets
		/// </summary>
		/// <param name="fileName">Path to save the preset file</param>
		public static void ExportPresets(System.String fileName) {
			Core_exportPresets(fileName);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_importPresets(string fileName);
		/// <summary>
		/// Import presets from file
		/// </summary>
		/// <param name="fileName">Path to the preset file to load</param>
		public static void ImportPresets(System.String fileName) {
			Core_importPresets(fileName);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_removeAllPresets();
		/// <summary>
		/// Remove all presets
		/// </summary>
		public static void RemoveAllPresets() {
			Core_removeAllPresets();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region license

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_addWantedToken(string tokenName);
		/// <summary>
		/// Add a license token to the list of wanted optional tokens
		/// </summary>
		/// <param name="tokenName">Wanted token</param>
		public static void AddWantedToken(System.String tokenName) {
			Core_addWantedToken(tokenName);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Core_checkLicense();
		/// <summary>
		/// check the current license
		/// </summary>
		public static System.Boolean CheckLicense() {
			var ret = Core_checkLicense();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Boolean)(0 != ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_configureLicenseServer(string address, System.UInt16 port, Int32 flexLM);
		/// <summary>
		/// Configure the license server to use to get floating licenses
		/// </summary>
		/// <param name="address">Server address</param>
		/// <param name="port">Server port</param>
		/// <param name="flexLM">Enable FlexLM license server</param>
		public static void ConfigureLicenseServer(System.String address, System.UInt16 port, System.Boolean flexLM) {
			Core_configureLicenseServer(address, port, flexLM ? 1 : 0);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_generateActivationCode(string filePath);
		/// <summary>
		/// Create an activation code to generate an offline license
		/// </summary>
		/// <param name="filePath">Path to write the activation code</param>
		public static void GenerateActivationCode(System.String filePath) {
			Core_generateActivationCode(filePath);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_generateDeactivationCode(string filePath);
		/// <summary>
		/// Create an deactivation code to release the license from this machine
		/// </summary>
		/// <param name="filePath">Path to write the deactivation code</param>
		public static void GenerateDeactivationCode(System.String filePath) {
			Core_generateDeactivationCode(filePath);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern LicenseInfos_c Core_getCurrentLicenseInfos();
		/// <summary>
		/// get informations on current installed license
		/// </summary>
		public static LicenseInfos GetCurrentLicenseInfos() {
			var ret = Core_getCurrentLicenseInfos();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_LicenseInfos_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getLicenseServerReturn_c Core_getLicenseServer();
		/// <summary>
		/// Get current license server
		/// </summary>
		public static Core.Native.getLicenseServerReturn GetLicenseServer() {
			var ret = Core_getLicenseServer();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			Core.Native.getLicenseServerReturn retStruct = new Core.Native.getLicenseServerReturn();
			retStruct.serverHost = ConvertValue(ret.serverHost);
			retStruct.serverPort = (System.UInt16)ret.serverPort;
			retStruct.useFlexLM = (0 != ret.useFlexLM);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_installLicense(string licensePath);
		/// <summary>
		/// install a new license
		/// </summary>
		/// <param name="licensePath">Path of the license file</param>
		public static void InstallLicense(System.String licensePath) {
			Core_installLicense(licensePath);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Core_isFloatingLicense();
		/// <summary>
		/// Tells if license is floating
		/// </summary>
		public static System.Boolean IsFloatingLicense() {
			var ret = Core_isFloatingLicense();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Boolean)(0 != ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern StringList_c Core_listOwnedTokens();
		/// <summary>
		/// Get the list of actually owned license tokens
		/// </summary>
		public static StringList ListOwnedTokens() {
			var ret = Core_listOwnedTokens();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_StringList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern StringList_c Core_listTokens(Int32 onlyMandatory);
		/// <summary>
		/// Get the list of license tokens for this product
		/// </summary>
		/// <param name="onlyMandatory">If True, optional tokens will not be returned</param>
		public static StringList ListTokens(System.Boolean onlyMandatory) {
			var ret = Core_listTokens(onlyMandatory ? 1 : 0);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_StringList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_needToken(string tokenName);
		/// <summary>
		/// Ensure that a license token is available, usefull to be sure to own floatting licence tokens
		/// </summary>
		/// <param name="tokenName">Token name</param>
		public static void NeedToken(System.String tokenName) {
			Core_needToken(tokenName);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_releaseToken(string tokenName);
		/// <summary>
		/// Release an optional license token
		/// </summary>
		/// <param name="tokenName">Token name</param>
		public static void ReleaseToken(System.String tokenName) {
			Core_releaseToken(tokenName);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_releaseWebLicense(string login, string password, System.UInt32 id);
		/// <summary>
		/// release License owned by user WEB account
		/// </summary>
		/// <param name="login">WEB account login</param>
		/// <param name="password">WEB account password</param>
		/// <param name="id">WEB license id</param>
		public static void ReleaseWebLicense(System.String login, System.String password, System.UInt32 id) {
			Core_releaseWebLicense(login, password, id);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_removeWantedToken(string tokenName);
		/// <summary>
		/// remove a license token from the list of wanted optional tokens
		/// </summary>
		/// <param name="tokenName">Unwanted token</param>
		public static void RemoveWantedToken(System.String tokenName) {
			Core_removeWantedToken(tokenName);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_requestWebLicense(string login, string password, System.UInt32 id);
		/// <summary>
		/// request License owned by user WEB account
		/// </summary>
		/// <param name="login">WEB account login</param>
		/// <param name="password">WEB account password</param>
		/// <param name="id">WEB license id</param>
		public static void RequestWebLicense(System.String login, System.String password, System.UInt32 id) {
			Core_requestWebLicense(login, password, id);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern WebLicenseInfoList_c Core_retrieveWebLicenses(string login, string password);
		/// <summary>
		/// Retrieves License owned by user WEB account
		/// </summary>
		/// <param name="login">WEB account login</param>
		/// <param name="password">WEB account password</param>
		public static WebLicenseInfoList RetrieveWebLicenses(System.String login, System.String password) {
			var ret = Core_retrieveWebLicenses(login, password);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_WebLicenseInfoList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Core_tokenValid(string tokenName);
		/// <summary>
		/// Returns True if a token is owned by the product
		/// </summary>
		/// <param name="tokenName">Token name</param>
		public static System.Boolean TokenValid(System.String tokenName) {
			var ret = Core_tokenValid(tokenName);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Boolean)(0 != ret);
		}

		#endregion

		#region pipeline

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Core_getOutputFilePath(string fileName, string data);
		/// <summary>
		/// Return a complete output file path for Pixyz Pipeline, this function is usefull for online usage when you know where is the output directory
		/// </summary>
		/// <param name="fileName">The desired file name (suffix of the path)</param>
		/// <param name="data">Optional data assiocated with file</param>
		public static System.String GetOutputFilePath(System.String fileName, System.String data) {
			var ret = Core_getOutputFilePath(fileName, data);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		#endregion

		#region plugins

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_executeCommand(string cmd);
		/// <summary>
		/// Execute a command
		/// </summary>
		/// <param name="cmd">Command to execute</param>
		public static void ExecuteCommand(System.String cmd) {
			Core_executeCommand(cmd);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_installPlugin(string pluginFile, Int32 installForAllUsers, Int32 generateDocumentation);
		/// <summary>
		/// Install a new plugin
		/// </summary>
		/// <param name="pluginFile">Path to the plugin to be installed</param>
		/// <param name="installForAllUsers">If false only the current user will see the plugin installed</param>
		/// <param name="generateDocumentation">If false the documentation of the plugin is not generated</param>
		public static void InstallPlugin(System.String pluginFile, System.Boolean installForAllUsers, System.Boolean generateDocumentation) {
			Core_installPlugin(pluginFile, installForAllUsers ? 1 : 0, generateDocumentation ? 1 : 0);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region progress

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_popProgression();
		/// <summary>
		/// Leave current progression level
		/// </summary>
		public static void PopProgression() {
			Core_popProgression();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_pushProgression(Int32 stepCount, string progressName);
		/// <summary>
		/// Create a new progression level
		/// </summary>
		/// <param name="stepCount">Step count</param>
		/// <param name="progressName">Name of the progression step</param>
		public static void PushProgression(System.Int32 stepCount, System.String progressName) {
			Core_pushProgression(stepCount, progressName);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_stepProgression(Int32 stepCount);
		/// <summary>
		/// Add a step to current progression level
		/// </summary>
		/// <param name="stepCount">Step count</param>
		public static void StepProgression(System.Int32 stepCount) {
			Core_stepProgression(stepCount);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region properties

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_addCustomProperty(System.UInt32 entity, string name, string value);
		/// <summary>
		/// Add a custom property to an entity that support custom properties
		/// </summary>
		/// <param name="entity">An entity that support custom properties</param>
		/// <param name="name">Name of the custom property</param>
		/// <param name="value">Value of the custom property</param>
		public static void AddCustomProperty(System.UInt32 entity, System.String name, System.String value) {
			Core_addCustomProperty(entity, name, value);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Core_getModuleProperty(string module, string propertyName);
		/// <summary>
		/// Returns the value of a module property
		/// </summary>
		/// <param name="module">Name of the module</param>
		/// <param name="propertyName">The property name</param>
		public static System.String GetModuleProperty(System.String module, System.String propertyName) {
			var ret = Core_getModuleProperty(module, propertyName);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern StringList_c Core_getProperties(EntityList_c entities, string propertyName, string defaultValue);
		/// <summary>
		/// Get the property value on entities (if the property is not set on an entity, defaultValue is returned)
		/// </summary>
		/// <param name="entities">List of entities</param>
		/// <param name="propertyName">The property name</param>
		/// <param name="defaultValue">Default value to return if the property does not exist on an entity</param>
		public static StringList GetProperties(EntityList entities, System.String propertyName, System.String defaultValue) {
			var entities_c = new Core.Native.EntityList_c();
			ConvertValue(entities, ref entities_c);
			var ret = Core_getProperties(entities_c, propertyName, defaultValue);
			Core.Native.NativeInterface.Core_EntityList_free(ref entities_c);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_StringList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Core_getProperty(System.UInt32 entity, string propertyName);
		/// <summary>
		/// Get a property value as String on an entity (error if the property does not exist on the entity)
		/// </summary>
		/// <param name="entity">The entity</param>
		/// <param name="propertyName">The property name</param>
		public static System.String GetProperty(System.UInt32 entity, System.String propertyName) {
			var ret = Core_getProperty(entity, propertyName);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Core_hasProperty(System.UInt32 entity, string propertyName);
		/// <summary>
		/// Return true if the property was found on the occurrence, will not throw any exception except if the entity does not exist.
		/// </summary>
		/// <param name="entity">An entity that support properties</param>
		/// <param name="propertyName">Name of the property</param>
		public static System.Boolean HasProperty(System.UInt32 entity, System.String propertyName) {
			var ret = Core_hasProperty(entity, propertyName);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Boolean)(0 != ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern StringList_c Core_listModuleProperties(string module);
		/// <summary>
		/// Returns all the properties in the given module
		/// </summary>
		/// <param name="module">Name of the module</param>
		public static StringList ListModuleProperties(System.String module) {
			var ret = Core_listModuleProperties(module);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_StringList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern StringList_c Core_listProperties(System.UInt32 entity);
		/// <summary>
		/// Returns the name of the properties available on an entity
		/// </summary>
		/// <param name="entity">Entity to list</param>
		public static StringList ListProperties(System.UInt32 entity) {
			var ret = Core_listProperties(entity);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_StringList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_removeCustomProperty(System.UInt32 entity, string name);
		/// <summary>
		/// Remove a custom property from an entity that support custom properties
		/// </summary>
		/// <param name="entity">An entity that support custom properties</param>
		/// <param name="name">Name of the custom property</param>
		public static void RemoveCustomProperty(System.UInt32 entity, System.String name) {
			Core_removeCustomProperty(entity, name);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Core_restoreModulePropertyDefaultValue(string module, string propertyName);
		/// <summary>
		/// Restore the default value of a module property
		/// </summary>
		/// <param name="module">Name of the module</param>
		/// <param name="propertyName">The property name</param>
		public static System.String RestoreModulePropertyDefaultValue(System.String module, System.String propertyName) {
			var ret = Core_restoreModulePropertyDefaultValue(module, propertyName);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Core_setModuleProperty(string module, string propertyName, string propertyValue);
		/// <summary>
		/// Set the value of a module property
		/// </summary>
		/// <param name="module">Name of the module</param>
		/// <param name="propertyName">The property name</param>
		/// <param name="propertyValue">The property value</param>
		public static System.String SetModuleProperty(System.String module, System.String propertyName, System.String propertyValue) {
			var ret = Core_setModuleProperty(module, propertyName, propertyValue);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Core_setProperty(System.UInt32 entity, string propertyName, string propertyValue);
		/// <summary>
		/// Set a property value on an entity
		/// </summary>
		/// <param name="entity">The entity</param>
		/// <param name="propertyName">The property name</param>
		/// <param name="propertyValue">The property value</param>
		public static System.String SetProperty(System.UInt32 entity, System.String propertyName, System.String propertyValue) {
			var ret = Core_setProperty(entity, propertyName, propertyValue);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Core_supportCustomProperties(System.UInt32 entity);
		/// <summary>
		/// Return true if an entity support custom properties
		/// </summary>
		/// <param name="entity">An entity</param>
		public static System.Boolean SupportCustomProperties(System.UInt32 entity) {
			var ret = Core_supportCustomProperties(entity);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Boolean)(0 != ret);
		}

		#endregion

		#region system

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern availableMemoryReturn_c Core_availableMemory();
		/// <summary>
		/// returns available memory
		/// </summary>
		public static Core.Native.availableMemoryReturn AvailableMemory() {
			var ret = Core_availableMemory();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			Core.Native.availableMemoryReturn retStruct = new Core.Native.availableMemoryReturn();
			retStruct.availVirt = (System.Int64)ret.availVirt;
			retStruct.totalVirt = (System.Int64)ret.totalVirt;
			retStruct.availPhys = (System.Int64)ret.availPhys;
			retStruct.totalPhys = (System.Int64)ret.totalPhys;
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern checkForUpdatesReturn_c Core_checkForUpdates();
		/// <summary>
		/// check for software update
		/// </summary>
		public static Core.Native.checkForUpdatesReturn CheckForUpdates() {
			var ret = Core_checkForUpdates();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			Core.Native.checkForUpdatesReturn retStruct = new Core.Native.checkForUpdatesReturn();
			retStruct.newVersionAvailable = (0 != ret.newVersionAvailable);
			retStruct.newVersion = ConvertValue(ret.newVersion);
			retStruct.newVersionLink = ConvertValue(ret.newVersionLink);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_clearOtherTemporaryDirectories();
		/// <summary>
		/// remove all other session temporary directories (warning: make sure that no other instance of pixyz is running
		/// </summary>
		public static void ClearOtherTemporaryDirectories() {
			Core_clearOtherTemporaryDirectories();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_exportFunctionDialogScreen(string moduleName, string functionName, string OutputFilePath);
		/// <summary>
		/// Test documentation GUI dialog print
		/// </summary>
		/// <param name="moduleName">The module's name</param>
		/// <param name="functionName">The function's name</param>
		/// <param name="OutputFilePath">Where to save image</param>
		public static void ExportFunctionDialogScreen(System.String moduleName, System.String functionName, System.String OutputFilePath) {
			Core_exportFunctionDialogScreen(moduleName, functionName, OutputFilePath);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Core_getCustomVersionTag();
		/// <summary>
		/// get the Pixyz custom version tag
		/// </summary>
		public static System.String GetCustomVersionTag() {
			var ret = Core_getCustomVersionTag();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Core_getInstallationDirectory();
		/// <summary>
		/// get the Pixyz installation directory
		/// </summary>
		public static System.String GetInstallationDirectory() {
			var ret = Core_getInstallationDirectory();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.Int64 Core_getMemoryUsagePeak();
		/// <summary>
		/// Returns the memory usage peak of the current process in MB ( only available on windows yet )
		/// </summary>
		public static System.Int64 GetMemoryUsagePeak() {
			var ret = Core_getMemoryUsagePeak();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Int64)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Core_getPixyzWebsiteURL();
		/// <summary>
		/// get the Pixyz website URL
		/// </summary>
		public static System.String GetPixyzWebsiteURL() {
			var ret = Core_getPixyzWebsiteURL();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Core_getProductDocumentationURL();
		/// <summary>
		/// get the product documentation URL
		/// </summary>
		public static System.String GetProductDocumentationURL() {
			var ret = Core_getProductDocumentationURL();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Core_getTempDirectory();
		/// <summary>
		/// get the Pixyz temp directory
		/// </summary>
		public static System.String GetTempDirectory() {
			var ret = Core_getTempDirectory();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Core_getVersion();
		/// <summary>
		/// get the Pixyz product version
		/// </summary>
		public static System.String GetVersion() {
			var ret = Core_getVersion();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_pushAnalytic(string name, string data);
		/// <summary>
		/// push custom analytic event (Only for authorized products)
		/// </summary>
		/// <param name="name">Analytic event name</param>
		/// <param name="data">Analytic event data</param>
		public static void PushAnalytic(System.String name, System.String data) {
			Core_pushAnalytic(name, data);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_setCurrentThreadAsProcessThread();
		/// <summary>
		/// set thread
		/// </summary>
		public static void SetCurrentThreadAsProcessThread() {
			Core_setCurrentThreadAsProcessThread();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region undo/redo

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_clearUndoRedo();
		/// <summary>
		/// Clear undo/redo history
		/// </summary>
		public static void ClearUndoRedo() {
			Core_clearUndoRedo();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_endUndoRedoStep();
		/// <summary>
		/// End current undo/redo step
		/// </summary>
		public static void EndUndoRedoStep() {
			Core_endUndoRedoStep();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_redo(System.UInt32 count);
		/// <summary>
		/// redo some steps
		/// </summary>
		/// <param name="count"></param>
		public static void Redo(System.UInt32 count) {
			Core_redo(count);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_startUndoRedoStep(string stepName);
		/// <summary>
		/// Start a new undo/redo step
		/// </summary>
		/// <param name="stepName"></param>
		public static void StartUndoRedoStep(System.String stepName) {
			Core_startUndoRedoStep(stepName);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_toggleUndoRedo();
		/// <summary>
		/// Toggle undo/redo
		/// </summary>
		public static void ToggleUndoRedo() {
			Core_toggleUndoRedo();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_undo(System.UInt32 count);
		/// <summary>
		/// undo some steps
		/// </summary>
		/// <param name="count"></param>
		public static void Undo(System.UInt32 count) {
			Core_undo(count);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region utils

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Color_c Core_getColorFromIndex(Int32 index);
		/// <summary>
		/// Returns a unique color associated with an index
		/// </summary>
		/// <param name="index">Index of the color (index must be less than 2^24)</param>
		public static Color GetColorFromIndex(System.Int32 index) {
			var ret = Core_getColorFromIndex(index);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_Color_free(ref ret);
			return convRet;
		}

		#endregion

		#region verbose

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_addConsoleVerbose(Int32 level);
		/// <summary>
		/// add a console verbose level
		/// </summary>
		/// <param name="level">Verbose level</param>
		public static void AddConsoleVerbose(Verbose level) {
			Core_addConsoleVerbose((int)level);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_addLogFileVerbose(Int32 level);
		/// <summary>
		/// add a log file verbose level
		/// </summary>
		/// <param name="level">Verbose level</param>
		public static void AddLogFileVerbose(Verbose level) {
			Core_addLogFileVerbose((int)level);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_addSessionLogFileVerbose(Int32 level);
		/// <summary>
		/// add a session log file (lastSession.log) verbose level
		/// </summary>
		/// <param name="level">Verbose level</param>
		public static void AddSessionLogFileVerbose(Verbose level) {
			Core_addSessionLogFileVerbose((int)level);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_configureInterfaceLogger(Int32 enableFunction, Int32 enableParameters, Int32 enableExecutionTime);
		/// <summary>
		/// Set new configuration for the Interface Logger
		/// </summary>
		/// <param name="enableFunction">If true, the called function names will be print</param>
		/// <param name="enableParameters">If true, the called function parameters will be print (only if enableFunction=true too)</param>
		/// <param name="enableExecutionTime">If true, the called functions execution times will be print</param>
		public static void ConfigureInterfaceLogger(System.Boolean enableFunction, System.Boolean enableParameters, System.Boolean enableExecutionTime) {
			Core_configureInterfaceLogger(enableFunction ? 1 : 0, enableParameters ? 1 : 0, enableExecutionTime ? 1 : 0);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getInterfaceLoggerConfigurationReturn_c Core_getInterfaceLoggerConfiguration();
		/// <summary>
		/// Get the current Interface Logger configuration
		/// </summary>
		public static Core.Native.getInterfaceLoggerConfigurationReturn GetInterfaceLoggerConfiguration() {
			var ret = Core_getInterfaceLoggerConfiguration();
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			Core.Native.getInterfaceLoggerConfigurationReturn retStruct = new Core.Native.getInterfaceLoggerConfigurationReturn();
			retStruct.functionEnabled = (0 != ret.functionEnabled);
			retStruct.parametersEnabled = (0 != ret.parametersEnabled);
			retStruct.executionTimeEnabled = (0 != ret.executionTimeEnabled);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_removeConsoleVerbose(Int32 level);
		/// <summary>
		/// remove a console verbose level
		/// </summary>
		/// <param name="level">Verbose level</param>
		public static void RemoveConsoleVerbose(Verbose level) {
			Core_removeConsoleVerbose((int)level);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_removeLogFileVerbose(Int32 level);
		/// <summary>
		/// remove a log file verbose level
		/// </summary>
		/// <param name="level">Verbose level</param>
		public static void RemoveLogFileVerbose(Verbose level) {
			Core_removeLogFileVerbose((int)level);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_removeSessionLogFileVerbose(Int32 level);
		/// <summary>
		/// remove a session log file (lastSession.log) verbose level
		/// </summary>
		/// <param name="level">Verbose level</param>
		public static void RemoveSessionLogFileVerbose(Verbose level) {
			Core_removeSessionLogFileVerbose((int)level);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Core_setLogFile(string path);
		/// <summary>
		/// set the path of the log file
		/// </summary>
		/// <param name="path">Path of the log file</param>
		public static void SetLogFile(System.String path) {
			Core_setLogFile(path);
			System.String err = ConvertValue(Core_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

	}
}
