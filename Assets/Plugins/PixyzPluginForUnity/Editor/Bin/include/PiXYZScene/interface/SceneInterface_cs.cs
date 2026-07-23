#pragma warning disable CA2101

using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Security;
using System.Collections.Concurrent;
using PiXYZAPI = Pixyz.API.Native.NativeInterface;

namespace Pixyz.Scene.Native {

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
		internal static extern void Scene_AnnotationGroupList_init(ref AnnotationGroupList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_AnnotationGroupList_free(ref AnnotationGroupList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_AlternativeTreeList_init(ref AlternativeTreeList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_AlternativeTreeList_free(ref AlternativeTreeList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_VariantList_init(ref VariantList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_VariantList_free(ref VariantList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_ComponentList_init(ref ComponentList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_ComponentList_free(ref ComponentList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_OccurrenceList_init(ref OccurrenceList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_OccurrenceList_free(ref OccurrenceList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_OccurrenceListList_init(ref OccurrenceListList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_OccurrenceListList_free(ref OccurrenceListList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_KeyframeList_init(ref KeyframeList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_KeyframeList_free(ref KeyframeList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_AnimationList_init(ref AnimationList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_AnimationList_free(ref AnimationList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_AnimChannelList_init(ref AnimChannelList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_AnimChannelList_free(ref AnimChannelList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_LODList_init(ref LODList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_LODList_free(ref LODList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_LODComponentList_init(ref LODComponentList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_LODComponentList_free(ref LODComponentList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_AnnotationList_init(ref AnnotationList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_AnnotationList_free(ref AnnotationList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_Camera_init(ref Camera_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_Camera_free(ref Camera_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_MetadataList_init(ref MetadataList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_MetadataList_free(ref MetadataList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_PartList_init(ref PartList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_PartList_free(ref PartList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_MetadataDefinitionList_init(ref MetadataDefinitionList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_MetadataDefinitionList_free(ref MetadataDefinitionList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_PropertyValue_init(ref PropertyValue_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_PropertyValue_free(ref PropertyValue_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_PropertyValueList_init(ref PropertyValueList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_PropertyValueList_free(ref PropertyValueList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_VariantDefinition_init(ref VariantDefinition_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_VariantDefinition_free(ref VariantDefinition_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_Filter_init(ref Filter_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_Filter_free(ref Filter_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_PackedTree_init(ref PackedTree_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_PackedTree_free(ref PackedTree_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_RayHit_init(ref RayHit_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_RayHit_free(ref RayHit_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_RayHitList_init(ref RayHitList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_RayHitList_free(ref RayHitList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_ProberInfo_init(ref ProberInfo_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_ProberInfo_free(ref ProberInfo_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_ResizeByTexturesOptions_init(ref ResizeByTexturesOptions_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_ResizeByTexturesOptions_free(ref ResizeByTexturesOptions_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_VariantDefinitionList_init(ref VariantDefinitionList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_VariantDefinitionList_free(ref VariantDefinitionList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_VariantDefinitionListList_init(ref VariantDefinitionListList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_VariantDefinitionListList_free(ref VariantDefinitionListList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_ResizeByMaximumSizeOptions_init(ref ResizeByMaximumSizeOptions_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_ResizeByMaximumSizeOptions_free(ref ResizeByMaximumSizeOptions_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_MergeByRegionsStrategy_init(ref MergeByRegionsStrategy_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_MergeByRegionsStrategy_free(ref MergeByRegionsStrategy_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_FilterList_init(ref FilterList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_FilterList_free(ref FilterList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_ResizeTexturesResizeMode_init(ref ResizeTexturesResizeMode_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_ResizeTexturesResizeMode_free(ref ResizeTexturesResizeMode_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_ResizeTexturesInputMode_init(ref ResizeTexturesInputMode_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_ResizeTexturesInputMode_free(ref ResizeTexturesInputMode_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_VariantComponentList_init(ref VariantComponentList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Scene_VariantComponentList_free(ref VariantComponentList_c list);

	public static AnnotationGroupList ConvertValue(ref AnnotationGroupList_c s) {
		AnnotationGroupList list = new AnnotationGroupList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static AnnotationGroupList_c ConvertValue(AnnotationGroupList s, ref AnnotationGroupList_c list) {
		Scene.Native.NativeInterface.Scene_AnnotationGroupList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static AlternativeTreeList ConvertValue(ref AlternativeTreeList_c s) {
		AlternativeTreeList list = new AlternativeTreeList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static AlternativeTreeList_c ConvertValue(AlternativeTreeList s, ref AlternativeTreeList_c list) {
		Scene.Native.NativeInterface.Scene_AlternativeTreeList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static VariantList ConvertValue(ref VariantList_c s) {
		VariantList list = new VariantList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static VariantList_c ConvertValue(VariantList s, ref VariantList_c list) {
		Scene.Native.NativeInterface.Scene_VariantList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static ComponentList ConvertValue(ref ComponentList_c s) {
		ComponentList list = new ComponentList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static ComponentList_c ConvertValue(ComponentList s, ref ComponentList_c list) {
		Scene.Native.NativeInterface.Scene_ComponentList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static OccurrenceList ConvertValue(ref OccurrenceList_c s) {
		OccurrenceList list = new OccurrenceList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static OccurrenceList_c ConvertValue(OccurrenceList s, ref OccurrenceList_c list) {
		Scene.Native.NativeInterface.Scene_OccurrenceList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static OccurrenceListList ConvertValue(ref OccurrenceListList_c s) {
		OccurrenceListList list = new OccurrenceListList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(OccurrenceList_c)));
			OccurrenceList_c value = (OccurrenceList_c)Marshal.PtrToStructure(p, typeof(OccurrenceList_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static OccurrenceListList_c ConvertValue(OccurrenceListList s, ref OccurrenceListList_c list) {
		Scene.Native.NativeInterface.Scene_OccurrenceListList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			OccurrenceList_c elt = new OccurrenceList_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(OccurrenceList_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static KeyframeList ConvertValue(ref KeyframeList_c s) {
		KeyframeList list = new KeyframeList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static KeyframeList_c ConvertValue(KeyframeList s, ref KeyframeList_c list) {
		Scene.Native.NativeInterface.Scene_KeyframeList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static AnimationList ConvertValue(ref AnimationList_c s) {
		AnimationList list = new AnimationList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static AnimationList_c ConvertValue(AnimationList s, ref AnimationList_c list) {
		Scene.Native.NativeInterface.Scene_AnimationList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static AnimChannelList ConvertValue(ref AnimChannelList_c s) {
		AnimChannelList list = new AnimChannelList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static AnimChannelList_c ConvertValue(AnimChannelList s, ref AnimChannelList_c list) {
		Scene.Native.NativeInterface.Scene_AnimChannelList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static LODList ConvertValue(ref LODList_c s) {
		LODList list = new LODList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static LODList_c ConvertValue(LODList s, ref LODList_c list) {
		Scene.Native.NativeInterface.Scene_LODList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static LODComponentList ConvertValue(ref LODComponentList_c s) {
		LODComponentList list = new LODComponentList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static LODComponentList_c ConvertValue(LODComponentList s, ref LODComponentList_c list) {
		Scene.Native.NativeInterface.Scene_LODComponentList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static AnnotationList ConvertValue(ref AnnotationList_c s) {
		AnnotationList list = new AnnotationList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static AnnotationList_c ConvertValue(AnnotationList s, ref AnnotationList_c list) {
		Scene.Native.NativeInterface.Scene_AnnotationList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
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

	public static Camera ConvertValue(ref Camera_c s) {
		Camera ss = new Camera();
		ss.position = Geom.Native.NativeInterface.ConvertValue(ref s.position);
		ss.direction = Geom.Native.NativeInterface.ConvertValue(ref s.direction);
		ss.up = Geom.Native.NativeInterface.ConvertValue(ref s.up);
		ss.fov = (System.Double)s.fov;
		return ss;
	}

	public static Camera_c ConvertValue(Camera s, ref Camera_c ss) {
		Scene.Native.NativeInterface.Scene_Camera_init(ref ss);
		Geom.Native.NativeInterface.ConvertValue(s.position, ref ss.position);
		Geom.Native.NativeInterface.ConvertValue(s.direction, ref ss.direction);
		Geom.Native.NativeInterface.ConvertValue(s.up, ref ss.up);
		ss.fov = (System.Double)s.fov;
		return ss;
	}

	public static MetadataList ConvertValue(ref MetadataList_c s) {
		MetadataList list = new MetadataList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static MetadataList_c ConvertValue(MetadataList s, ref MetadataList_c list) {
		Scene.Native.NativeInterface.Scene_MetadataList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static PartList ConvertValue(ref PartList_c s) {
		PartList list = new PartList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static PartList_c ConvertValue(PartList s, ref PartList_c list) {
		Scene.Native.NativeInterface.Scene_PartList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static MetadataDefinitionList ConvertValue(ref MetadataDefinitionList_c s) {
		MetadataDefinitionList list = new MetadataDefinitionList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(PropertyValueList_c)));
			PropertyValueList_c value = (PropertyValueList_c)Marshal.PtrToStructure(p, typeof(PropertyValueList_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static MetadataDefinitionList_c ConvertValue(MetadataDefinitionList s, ref MetadataDefinitionList_c list) {
		Scene.Native.NativeInterface.Scene_MetadataDefinitionList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			PropertyValueList_c elt = new PropertyValueList_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(PropertyValueList_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static PropertyValue ConvertValue(ref PropertyValue_c s) {
		PropertyValue ss = new PropertyValue();
		ss.name = ConvertValue(s.name);
		ss.value = ConvertValue(s.value);
		return ss;
	}

	public static PropertyValue_c ConvertValue(PropertyValue s, ref PropertyValue_c ss) {
		Scene.Native.NativeInterface.Scene_PropertyValue_init(ref ss);
		ss.name = ConvertValue(s.name);
		ss.value = ConvertValue(s.value);
		return ss;
	}

	public static PropertyValueList ConvertValue(ref PropertyValueList_c s) {
		PropertyValueList list = new PropertyValueList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(PropertyValue_c)));
			PropertyValue_c value = (PropertyValue_c)Marshal.PtrToStructure(p, typeof(PropertyValue_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static PropertyValueList_c ConvertValue(PropertyValueList s, ref PropertyValueList_c list) {
		Scene.Native.NativeInterface.Scene_PropertyValueList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			PropertyValue_c elt = new PropertyValue_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(PropertyValue_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static VariantDefinition ConvertValue(ref VariantDefinition_c s) {
		VariantDefinition ss = new VariantDefinition();
		ss.variant = (System.UInt32)s.variant;
		ss.overridedProperties = ConvertValue(ref s.overridedProperties);
		return ss;
	}

	public static VariantDefinition_c ConvertValue(VariantDefinition s, ref VariantDefinition_c ss) {
		Scene.Native.NativeInterface.Scene_VariantDefinition_init(ref ss);
		ss.variant = (System.UInt32)s.variant;
		ConvertValue(s.overridedProperties, ref ss.overridedProperties);
		return ss;
	}

	public static Filter ConvertValue(ref Filter_c s) {
		Filter ss = new Filter();
		ss.id = (System.UInt32)s.id;
		ss.name = ConvertValue(s.name);
		ss.expr = ConvertValue(s.expr);
		return ss;
	}

	public static Filter_c ConvertValue(Filter s, ref Filter_c ss) {
		Scene.Native.NativeInterface.Scene_Filter_init(ref ss);
		ss.id = (System.UInt32)s.id;
		ss.name = ConvertValue(s.name);
		ss.expr = ConvertValue(s.expr);
		return ss;
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

	public static Core.Native.StringList ConvertValue(ref Core.Native.StringList_c s) {
		Core.Native.StringList list = new Core.Native.StringList((int)s.size);
		if (s.size==0) return list;
		IntPtr[] tab = new IntPtr[s.size];
		Marshal.Copy(s.ptr, tab, 0, (int)s.size);
		for (int i = 0; i < (int)s.size; ++i) {
			list.list[i] = ConvertValue(tab[i]);
		}
		return list;
	}

	public static Core.Native.StringList_c ConvertValue(Core.Native.StringList s, ref Core.Native.StringList_c list) {
		Core.Native.NativeInterface.Core_StringList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		IntPtr[] tab = new IntPtr[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = ConvertValue(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static Core.Native.InheritableBoolList ConvertValue(ref Core.Native.InheritableBoolList_c s) {
		Core.Native.InheritableBoolList list = new Core.Native.InheritableBoolList((int)s.size);
		if (s.size==0) return list;
		int[] tab = new int[s.size];
		Marshal.Copy(s.ptr, tab, 0, (int)s.size);
		for (int i = 0; i < (int)s.size; ++i) {
			list.list[i] = (Core.Native.InheritableBool)tab[i];
		}
		return list;
	}

	public static Core.Native.InheritableBoolList_c ConvertValue(Core.Native.InheritableBoolList s, ref Core.Native.InheritableBoolList_c list) {
		Core.Native.NativeInterface.Core_InheritableBoolList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)s.list[i];
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static Material.Native.MaterialList ConvertValue(ref Material.Native.MaterialList_c s) {
		Material.Native.MaterialList list = new Material.Native.MaterialList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static Material.Native.MaterialList_c ConvertValue(Material.Native.MaterialList s, ref Material.Native.MaterialList_c list) {
		Material.Native.NativeInterface.Material_MaterialList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
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

	public static Core.Native.StringPair ConvertValue(ref Core.Native.StringPair_c s) {
		Core.Native.StringPair ss = new Core.Native.StringPair();
		ss.key = ConvertValue(s.key);
		ss.value = ConvertValue(s.value);
		return ss;
	}

	public static Core.Native.StringPair_c ConvertValue(Core.Native.StringPair s, ref Core.Native.StringPair_c ss) {
		Core.Native.NativeInterface.Core_StringPair_init(ref ss);
		ss.key = ConvertValue(s.key);
		ss.value = ConvertValue(s.value);
		return ss;
	}

	public static Core.Native.StringPairList ConvertValue(ref Core.Native.StringPairList_c s) {
		Core.Native.StringPairList list = new Core.Native.StringPairList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Core.Native.StringPair_c)));
			Core.Native.StringPair_c value = (Core.Native.StringPair_c)Marshal.PtrToStructure(p, typeof(Core.Native.StringPair_c));
			list.list[i] = Core.Native.NativeInterface.ConvertValue(ref value);
		}
		return list;
	}

	public static Core.Native.StringPairList_c ConvertValue(Core.Native.StringPairList s, ref Core.Native.StringPairList_c list) {
		Core.Native.NativeInterface.Core_StringPairList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			Core.Native.StringPair_c elt = new Core.Native.StringPair_c();
			Core.Native.NativeInterface.ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Core.Native.StringPair_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static Core.Native.StringPairListList ConvertValue(ref Core.Native.StringPairListList_c s) {
		Core.Native.StringPairListList list = new Core.Native.StringPairListList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Core.Native.StringPairList_c)));
			Core.Native.StringPairList_c value = (Core.Native.StringPairList_c)Marshal.PtrToStructure(p, typeof(Core.Native.StringPairList_c));
			list.list[i] = Core.Native.NativeInterface.ConvertValue(ref value);
		}
		return list;
	}

	public static Core.Native.StringPairListList_c ConvertValue(Core.Native.StringPairListList s, ref Core.Native.StringPairListList_c list) {
		Core.Native.NativeInterface.Core_StringPairListList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			Core.Native.StringPairList_c elt = new Core.Native.StringPairList_c();
			Core.Native.NativeInterface.ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Core.Native.StringPairList_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static PackedTree ConvertValue(ref PackedTree_c s) {
		PackedTree ss = new PackedTree();
		ss.occurrences = ConvertValue(ref s.occurrences);
		ss.parents = Core.Native.NativeInterface.ConvertValue(ref s.parents);
		ss.names = Core.Native.NativeInterface.ConvertValue(ref s.names);
		ss.visibles = Core.Native.NativeInterface.ConvertValue(ref s.visibles);
		ss.materials = Material.Native.NativeInterface.ConvertValue(ref s.materials);
		ss.transformIndices = Core.Native.NativeInterface.ConvertValue(ref s.transformIndices);
		ss.transformMatrices = Geom.Native.NativeInterface.ConvertValue(ref s.transformMatrices);
		ss.customProperties = Core.Native.NativeInterface.ConvertValue(ref s.customProperties);
		return ss;
	}

	public static PackedTree_c ConvertValue(PackedTree s, ref PackedTree_c ss) {
		Scene.Native.NativeInterface.Scene_PackedTree_init(ref ss);
		ConvertValue(s.occurrences, ref ss.occurrences);
		Core.Native.NativeInterface.ConvertValue(s.parents, ref ss.parents);
		Core.Native.NativeInterface.ConvertValue(s.names, ref ss.names);
		Core.Native.NativeInterface.ConvertValue(s.visibles, ref ss.visibles);
		Material.Native.NativeInterface.ConvertValue(s.materials, ref ss.materials);
		Core.Native.NativeInterface.ConvertValue(s.transformIndices, ref ss.transformIndices);
		Geom.Native.NativeInterface.ConvertValue(s.transformMatrices, ref ss.transformMatrices);
		Core.Native.NativeInterface.ConvertValue(s.customProperties, ref ss.customProperties);
		return ss;
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

	public static RayHit ConvertValue(ref RayHit_c s) {
		RayHit ss = new RayHit();
		ss.rayParam = (System.Double)s.rayParam;
		ss.occurrence = (System.UInt32)s.occurrence;
		ss.triangleIndex = (System.Int32)s.triangleIndex;
		ss.triangleParam = Geom.Native.NativeInterface.ConvertValue(ref s.triangleParam);
		return ss;
	}

	public static RayHit_c ConvertValue(RayHit s, ref RayHit_c ss) {
		Scene.Native.NativeInterface.Scene_RayHit_init(ref ss);
		ss.rayParam = (System.Double)s.rayParam;
		ss.occurrence = (System.UInt32)s.occurrence;
		ss.triangleIndex = (Int32)s.triangleIndex;
		Geom.Native.NativeInterface.ConvertValue(s.triangleParam, ref ss.triangleParam);
		return ss;
	}

	public static RayHitList ConvertValue(ref RayHitList_c s) {
		RayHitList list = new RayHitList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(RayHit_c)));
			RayHit_c value = (RayHit_c)Marshal.PtrToStructure(p, typeof(RayHit_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static RayHitList_c ConvertValue(RayHitList s, ref RayHitList_c list) {
		Scene.Native.NativeInterface.Scene_RayHitList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			RayHit_c elt = new RayHit_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(RayHit_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static ProberInfo ConvertValue(ref ProberInfo_c s) {
		ProberInfo ss = new ProberInfo();
		ss.occurrence = (System.UInt32)s.occurrence;
		ss.position = Geom.Native.NativeInterface.ConvertValue(ref s.position);
		return ss;
	}

	public static ProberInfo_c ConvertValue(ProberInfo s, ref ProberInfo_c ss) {
		Scene.Native.NativeInterface.Scene_ProberInfo_init(ref ss);
		ss.occurrence = (System.UInt32)s.occurrence;
		Geom.Native.NativeInterface.ConvertValue(s.position, ref ss.position);
		return ss;
	}

	public static Material.Native.ImageList ConvertValue(ref Material.Native.ImageList_c s) {
		Material.Native.ImageList list = new Material.Native.ImageList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static Material.Native.ImageList_c ConvertValue(Material.Native.ImageList s, ref Material.Native.ImageList_c list) {
		Material.Native.NativeInterface.Material_ImageList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static ResizeByTexturesOptions ConvertValue(ref ResizeByTexturesOptions_c s) {
		ResizeByTexturesOptions ss = new ResizeByTexturesOptions();
		ss._type = (ResizeByTexturesOptions.Type)s._type;
		switch(ss._type) {
			case ResizeByTexturesOptions.Type.UNKNOWN: break;
			case ResizeByTexturesOptions.Type.ALLTEXTURES: ss.AllTextures = (Int32)s.AllTextures; break;
			case ResizeByTexturesOptions.Type.SELECTION: ss.Selection = ConvertValue(ref s.Selection); break;
		}
		return ss;
	}

	public static ResizeByTexturesOptions_c ConvertValue(ResizeByTexturesOptions s, ref ResizeByTexturesOptions_c ss) {
		Scene.Native.NativeInterface.Scene_ResizeByTexturesOptions_init(ref ss);
		ss._type = (int)s._type;
		switch (ss._type) {
			case 0: break;
			case 1: ss.AllTextures = (Int32)s.AllTextures; break;
			case 2: ConvertValue(s.Selection, ref ss.Selection); break;
		}
		return ss;
	}

	public static VariantDefinitionList ConvertValue(ref VariantDefinitionList_c s) {
		VariantDefinitionList list = new VariantDefinitionList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(VariantDefinition_c)));
			VariantDefinition_c value = (VariantDefinition_c)Marshal.PtrToStructure(p, typeof(VariantDefinition_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static VariantDefinitionList_c ConvertValue(VariantDefinitionList s, ref VariantDefinitionList_c list) {
		Scene.Native.NativeInterface.Scene_VariantDefinitionList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			VariantDefinition_c elt = new VariantDefinition_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(VariantDefinition_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static VariantDefinitionListList ConvertValue(ref VariantDefinitionListList_c s) {
		VariantDefinitionListList list = new VariantDefinitionListList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(VariantDefinitionList_c)));
			VariantDefinitionList_c value = (VariantDefinitionList_c)Marshal.PtrToStructure(p, typeof(VariantDefinitionList_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static VariantDefinitionListList_c ConvertValue(VariantDefinitionListList s, ref VariantDefinitionListList_c list) {
		Scene.Native.NativeInterface.Scene_VariantDefinitionListList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			VariantDefinitionList_c elt = new VariantDefinitionList_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(VariantDefinitionList_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static ResizeByMaximumSizeOptions ConvertValue(ref ResizeByMaximumSizeOptions_c s) {
		ResizeByMaximumSizeOptions ss = new ResizeByMaximumSizeOptions();
		ss.TextureSize = (System.Int32)s.TextureSize;
		ss.KeepTextureRatio = ConvertValue(s.KeepTextureRatio);
		return ss;
	}

	public static ResizeByMaximumSizeOptions_c ConvertValue(ResizeByMaximumSizeOptions s, ref ResizeByMaximumSizeOptions_c ss) {
		Scene.Native.NativeInterface.Scene_ResizeByMaximumSizeOptions_init(ref ss);
		ss.TextureSize = (Int32)s.TextureSize;
		ss.KeepTextureRatio = ConvertValue(s.KeepTextureRatio);
		return ss;
	}

	public static MergeByRegionsStrategy ConvertValue(ref MergeByRegionsStrategy_c s) {
		MergeByRegionsStrategy ss = new MergeByRegionsStrategy();
		ss._type = (MergeByRegionsStrategy.Type)s._type;
		switch(ss._type) {
			case MergeByRegionsStrategy.Type.UNKNOWN: break;
			case MergeByRegionsStrategy.Type.NUMBEROFREGIONS: ss.NumberOfRegions = (Int32)s.NumberOfRegions; break;
			case MergeByRegionsStrategy.Type.SIZEOFREGIONS: ss.SizeOfRegions = (System.Double)s.SizeOfRegions; break;
		}
		return ss;
	}

	public static MergeByRegionsStrategy_c ConvertValue(MergeByRegionsStrategy s, ref MergeByRegionsStrategy_c ss) {
		Scene.Native.NativeInterface.Scene_MergeByRegionsStrategy_init(ref ss);
		ss._type = (int)s._type;
		switch (ss._type) {
			case 0: break;
			case 1: ss.NumberOfRegions = (Int32)s.NumberOfRegions; break;
			case 2: ss.SizeOfRegions = (System.Double)s.SizeOfRegions; break;
		}
		return ss;
	}

	public static FilterList ConvertValue(ref FilterList_c s) {
		FilterList list = new FilterList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Filter_c)));
			Filter_c value = (Filter_c)Marshal.PtrToStructure(p, typeof(Filter_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static FilterList_c ConvertValue(FilterList s, ref FilterList_c list) {
		Scene.Native.NativeInterface.Scene_FilterList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			Filter_c elt = new Filter_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Filter_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static ResizeTexturesResizeMode ConvertValue(ref ResizeTexturesResizeMode_c s) {
		ResizeTexturesResizeMode ss = new ResizeTexturesResizeMode();
		ss._type = (ResizeTexturesResizeMode.Type)s._type;
		switch(ss._type) {
			case ResizeTexturesResizeMode.Type.UNKNOWN: break;
			case ResizeTexturesResizeMode.Type.RATIO: ss.Ratio = (System.Double)s.Ratio; break;
			case ResizeTexturesResizeMode.Type.MAXIMUMSIZE: ss.MaximumSize = ConvertValue(ref s.MaximumSize); break;
		}
		return ss;
	}

	public static ResizeTexturesResizeMode_c ConvertValue(ResizeTexturesResizeMode s, ref ResizeTexturesResizeMode_c ss) {
		Scene.Native.NativeInterface.Scene_ResizeTexturesResizeMode_init(ref ss);
		ss._type = (int)s._type;
		switch (ss._type) {
			case 0: break;
			case 1: ss.Ratio = (System.Double)s.Ratio; break;
			case 2: ConvertValue(s.MaximumSize, ref ss.MaximumSize); break;
		}
		return ss;
	}

	public static ResizeTexturesInputMode ConvertValue(ref ResizeTexturesInputMode_c s) {
		ResizeTexturesInputMode ss = new ResizeTexturesInputMode();
		ss._type = (ResizeTexturesInputMode.Type)s._type;
		switch(ss._type) {
			case ResizeTexturesInputMode.Type.UNKNOWN: break;
			case ResizeTexturesInputMode.Type.OCCURRENCES: ss.Occurrences = ConvertValue(ref s.Occurrences); break;
			case ResizeTexturesInputMode.Type.TEXTURES: ss.Textures = ConvertValue(ref s.Textures); break;
		}
		return ss;
	}

	public static ResizeTexturesInputMode_c ConvertValue(ResizeTexturesInputMode s, ref ResizeTexturesInputMode_c ss) {
		Scene.Native.NativeInterface.Scene_ResizeTexturesInputMode_init(ref ss);
		ss._type = (int)s._type;
		switch (ss._type) {
			case 0: break;
			case 1: ConvertValue(s.Occurrences, ref ss.Occurrences); break;
			case 2: ConvertValue(s.Textures, ref ss.Textures); break;
		}
		return ss;
	}

	public static VariantComponentList ConvertValue(ref VariantComponentList_c s) {
		VariantComponentList list = new VariantComponentList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static VariantComponentList_c ConvertValue(VariantComponentList s, ref VariantComponentList_c list) {
		Scene.Native.NativeInterface.Scene_VariantComponentList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static getSubTreeStatsReturn ConvertValue(ref getSubTreeStatsReturn_c s) {
		getSubTreeStatsReturn ss = new getSubTreeStatsReturn();
		ss.partCount = (System.Int32)s.partCount;
		ss.partOccurrenceCount = (System.Int32)s.partOccurrenceCount;
		ss.triangleCount = (System.Int32)s.triangleCount;
		ss.triangleOccurrenceCount = (System.Int32)s.triangleOccurrenceCount;
		ss.vertexCount = (System.Int32)s.vertexCount;
		ss.vertexOccurrenceCount = (System.Int32)s.vertexOccurrenceCount;
		return ss;
	}

	public static getSubTreeStatsReturn_c ConvertValue(getSubTreeStatsReturn s, ref getSubTreeStatsReturn_c ss) {
		ss.partCount = (Int32)s.partCount;
		ss.partOccurrenceCount = (Int32)s.partOccurrenceCount;
		ss.triangleCount = (Int32)s.triangleCount;
		ss.triangleOccurrenceCount = (Int32)s.triangleOccurrenceCount;
		ss.vertexCount = (Int32)s.vertexCount;
		ss.vertexOccurrenceCount = (Int32)s.vertexOccurrenceCount;
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

	public static getViewpointsFromCavitiesReturn ConvertValue(ref getViewpointsFromCavitiesReturn_c s) {
		getViewpointsFromCavitiesReturn ss = new getViewpointsFromCavitiesReturn();
		ss.positions = Geom.Native.NativeInterface.ConvertValue(ref s.positions);
		ss.directions = Geom.Native.NativeInterface.ConvertValue(ref s.directions);
		return ss;
	}

	public static getViewpointsFromCavitiesReturn_c ConvertValue(getViewpointsFromCavitiesReturn s, ref getViewpointsFromCavitiesReturn_c ss) {
		Geom.Native.NativeInterface.ConvertValue(s.positions, ref ss.positions);
		Geom.Native.NativeInterface.ConvertValue(s.directions, ref ss.directions);
		return ss;
	}

	public static getOoCConfigurationReturn ConvertValue(ref getOoCConfigurationReturn_c s) {
		getOoCConfigurationReturn ss = new getOoCConfigurationReturn();
		ss.implementationType = ConvertValue(s.implementationType);
		ss.implementationParameters = ConvertValue(s.implementationParameters);
		return ss;
	}

	public static getOoCConfigurationReturn_c ConvertValue(getOoCConfigurationReturn s, ref getOoCConfigurationReturn_c ss) {
		ss.implementationType = ConvertValue(s.implementationType);
		ss.implementationParameters = ConvertValue(s.implementationParameters);
		return ss;
	}

	public static getBRepInfosReturn ConvertValue(ref getBRepInfosReturn_c s) {
		getBRepInfosReturn ss = new getBRepInfosReturn();
		ss.partCount = (System.Int32)s.partCount;
		ss.totalPartCount = (System.Int32)s.totalPartCount;
		ss.vertexCount = (System.Int32)s.vertexCount;
		ss.totalVertexCount = (System.Int32)s.totalVertexCount;
		ss.edgeCount = (System.Int32)s.edgeCount;
		ss.totalEdgeCount = (System.Int32)s.totalEdgeCount;
		ss.domainCount = (System.Int32)s.domainCount;
		ss.totalDomainCount = (System.Int32)s.totalDomainCount;
		ss.bodyCount = (System.Int32)s.bodyCount;
		ss.totalBodyCount = (System.Int32)s.totalBodyCount;
		ss.area2Dsum = (System.Double)s.area2Dsum;
		ss.boundaryCount = (System.Int32)s.boundaryCount;
		ss.boundaryEdgeCount = (System.Int32)s.boundaryEdgeCount;
		return ss;
	}

	public static getBRepInfosReturn_c ConvertValue(getBRepInfosReturn s, ref getBRepInfosReturn_c ss) {
		ss.partCount = (Int32)s.partCount;
		ss.totalPartCount = (Int32)s.totalPartCount;
		ss.vertexCount = (Int32)s.vertexCount;
		ss.totalVertexCount = (Int32)s.totalVertexCount;
		ss.edgeCount = (Int32)s.edgeCount;
		ss.totalEdgeCount = (Int32)s.totalEdgeCount;
		ss.domainCount = (Int32)s.domainCount;
		ss.totalDomainCount = (Int32)s.totalDomainCount;
		ss.bodyCount = (Int32)s.bodyCount;
		ss.totalBodyCount = (Int32)s.totalBodyCount;
		ss.area2Dsum = (System.Double)s.area2Dsum;
		ss.boundaryCount = (Int32)s.boundaryCount;
		ss.boundaryEdgeCount = (Int32)s.boundaryEdgeCount;
		return ss;
	}

	public static getTessellationInfosReturn ConvertValue(ref getTessellationInfosReturn_c s) {
		getTessellationInfosReturn ss = new getTessellationInfosReturn();
		ss.partCount = (System.Int32)s.partCount;
		ss.totalPartCount = (System.Int32)s.totalPartCount;
		ss.vertexCount = (System.Int32)s.vertexCount;
		ss.totalVertexCount = (System.Int32)s.totalVertexCount;
		ss.edgeCount = (System.Int32)s.edgeCount;
		ss.totalEdgeCount = (System.Int32)s.totalEdgeCount;
		ss.polygonCount = (System.Int32)s.polygonCount;
		ss.totalPolygonCount = (System.Int32)s.totalPolygonCount;
		ss.patchCount = (System.Int32)s.patchCount;
		ss.totalPatchCount = (System.Int32)s.totalPatchCount;
		ss.boundaryCount = (System.Int32)s.boundaryCount;
		ss.boundaryEdgeCount = (System.Int32)s.boundaryEdgeCount;
		return ss;
	}

	public static getTessellationInfosReturn_c ConvertValue(getTessellationInfosReturn s, ref getTessellationInfosReturn_c ss) {
		ss.partCount = (Int32)s.partCount;
		ss.totalPartCount = (Int32)s.totalPartCount;
		ss.vertexCount = (Int32)s.vertexCount;
		ss.totalVertexCount = (Int32)s.totalVertexCount;
		ss.edgeCount = (Int32)s.edgeCount;
		ss.totalEdgeCount = (Int32)s.totalEdgeCount;
		ss.polygonCount = (Int32)s.polygonCount;
		ss.totalPolygonCount = (Int32)s.totalPolygonCount;
		ss.patchCount = (Int32)s.patchCount;
		ss.totalPatchCount = (Int32)s.totalPatchCount;
		ss.boundaryCount = (Int32)s.boundaryCount;
		ss.boundaryEdgeCount = (Int32)s.boundaryEdgeCount;
		return ss;
	}

	public static evaluateExpressionOnSubTreeReturn ConvertValue(ref evaluateExpressionOnSubTreeReturn_c s) {
		evaluateExpressionOnSubTreeReturn ss = new evaluateExpressionOnSubTreeReturn();
		ss.occurrences = ConvertValue(ref s.occurrences);
		ss.evaluations = Core.Native.NativeInterface.ConvertValue(ref s.evaluations);
		return ss;
	}

	public static evaluateExpressionOnSubTreeReturn_c ConvertValue(evaluateExpressionOnSubTreeReturn s, ref evaluateExpressionOnSubTreeReturn_c ss) {
		ConvertValue(s.occurrences, ref ss.occurrences);
		Core.Native.NativeInterface.ConvertValue(s.evaluations, ref ss.evaluations);
		return ss;
	}

	public static getPartsTransformsIndexedReturn ConvertValue(ref getPartsTransformsIndexedReturn_c s) {
		getPartsTransformsIndexedReturn ss = new getPartsTransformsIndexedReturn();
		ss.indices = Core.Native.NativeInterface.ConvertValue(ref s.indices);
		ss.transforms = Geom.Native.NativeInterface.ConvertValue(ref s.transforms);
		return ss;
	}

	public static getPartsTransformsIndexedReturn_c ConvertValue(getPartsTransformsIndexedReturn s, ref getPartsTransformsIndexedReturn_c ss) {
		Core.Native.NativeInterface.ConvertValue(s.indices, ref ss.indices);
		Geom.Native.NativeInterface.ConvertValue(s.transforms, ref ss.transforms);
		return ss;
	}

		#endregion

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Scene_getLastError();

		public static string GetLastError()
		{
			return ConvertValue(Scene_getLastError());
		}

		#region 

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_addComponent(System.UInt32 occurrence, Int32 componentType);
		/// <summary>
		/// Add a component to an occurrence
		/// </summary>
		/// <param name="occurrence">The occurrence to add the new component</param>
		/// <param name="componentType">Type of the component</param>
		public static System.UInt32 AddComponent(System.UInt32 occurrence, ComponentType componentType) {
			var ret = Scene_addComponent(occurrence, (int)componentType);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_addLightComponent(System.UInt32 occurrence);
		/// <summary>
		/// Add a light component to an occurrence
		/// </summary>
		/// <param name="occurrence">The occurrence to add the new component</param>
		public static System.UInt32 AddLightComponent(System.UInt32 occurrence) {
			var ret = Scene_addLightComponent(occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_addMetadata(System.UInt32 metadata, string name, string value);
		/// <summary>
		/// Add a new metadata property to a metadata component
		/// </summary>
		/// <param name="metadata">The metadata component</param>
		/// <param name="name">The new property name</param>
		/// <param name="value">The new property value</param>
		public static void AddMetadata(System.UInt32 metadata, System.String name, System.String value) {
			Scene_addMetadata(metadata, name, value);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_addMetadataBlock(System.UInt32 metadata, Core.Native.StringList_c names, Core.Native.StringList_c values);
		/// <summary>
		/// Add a new metadata property to a metadata component
		/// </summary>
		/// <param name="metadata">The metadata component</param>
		/// <param name="names">The new properties names</param>
		/// <param name="values">The new properties values</param>
		public static void AddMetadataBlock(System.UInt32 metadata, Core.Native.StringList names, Core.Native.StringList values) {
			var names_c = new Core.Native.StringList_c();
			Core.Native.NativeInterface.ConvertValue(names, ref names_c);
			var values_c = new Core.Native.StringList_c();
			Core.Native.NativeInterface.ConvertValue(values, ref values_c);
			Scene_addMetadataBlock(metadata, names_c, values_c);
			Core.Native.NativeInterface.Core_StringList_free(ref names_c);
			Core.Native.NativeInterface.Core_StringList_free(ref values_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Scene_cleanUnusedImages();
		/// <summary>
		/// Remove unused images from texture library
		/// </summary>
		public static System.Int32 CleanUnusedImages() {
			var ret = Scene_cleanUnusedImages();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Int32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Scene_cleanUnusedMaterials(Int32 cleanImages);
		/// <summary>
		/// Remove unused materials from material library
		/// </summary>
		/// <param name="cleanImages">Call cleanUnusedImages if true</param>
		public static System.Int32 CleanUnusedMaterials(System.Boolean cleanImages) {
			var ret = Scene_cleanUnusedMaterials(cleanImages ? 1 : 0);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Int32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Scene_computeSubTreeChecksum(System.UInt32 root);
		/// <summary>
		/// Compute the checksum of a sub-tree
		/// </summary>
		/// <param name="root">Occurrence to compute</param>
		public static System.String ComputeSubTreeChecksum(System.UInt32 root) {
			var ret = Scene_computeSubTreeChecksum(root);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern OccurrenceList_c Scene_createCompleteTree(PackedTree_c tree, System.UInt32 root, Int32 replaceRoot);
		/// <summary>
		/// Create a complete scene tree
		/// </summary>
		/// <param name="tree"></param>
		/// <param name="root">Specify the root occurrence of the scene</param>
		/// <param name="replaceRoot">If true, the root occurrence will be replaced by the root of the given tree, else it will be added as a child</param>
		public static OccurrenceList CreateCompleteTree(PackedTree tree, System.UInt32 root, System.Boolean replaceRoot) {
			var tree_c = new Scene.Native.PackedTree_c();
			ConvertValue(tree, ref tree_c);
			var ret = Scene_createCompleteTree(tree_c, root, replaceRoot ? 1 : 0);
			Scene.Native.NativeInterface.Scene_PackedTree_free(ref tree_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_createCone(System.Double bottomRadius, System.Double height, Int32 sides, Int32 generateUV);
		/// <summary>
		/// Create a new cone
		/// </summary>
		/// <param name="bottomRadius">Radius of the bottom of the cone </param>
		/// <param name="height">Height of thes cone</param>
		/// <param name="sides">Number of sides of the cone</param>
		/// <param name="generateUV">Generation of the UV</param>
		public static System.UInt32 CreateCone(System.Double bottomRadius, System.Double height, System.Int32 sides, System.Boolean generateUV) {
			var ret = Scene_createCone(bottomRadius, height, sides, generateUV ? 1 : 0);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_createCube(System.Double sizeX, System.Double sizeY, System.Double sizeZ, Int32 subdivision, Int32 generateUV);
		/// <summary>
		/// Create a new cube
		/// </summary>
		/// <param name="sizeX">Size of the Cube on the x axis</param>
		/// <param name="sizeY">Size of the Cube on the y axis</param>
		/// <param name="sizeZ">Size of the Cube on the z axis</param>
		/// <param name="subdivision">Subdivision of the Cube on all the axis</param>
		/// <param name="generateUV">Generation of the UV</param>
		public static System.UInt32 CreateCube(System.Double sizeX, System.Double sizeY, System.Double sizeZ, System.Int32 subdivision, System.Boolean generateUV) {
			var ret = Scene_createCube(sizeX, sizeY, sizeZ, subdivision, generateUV ? 1 : 0);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_createCylinder(System.Double radius, System.Double height, Int32 sides, Int32 generateUV);
		/// <summary>
		/// Create a new cylinder
		/// </summary>
		/// <param name="radius">Radius of the Cylinder</param>
		/// <param name="height">Height of the Cylinder</param>
		/// <param name="sides">Number of Sides of the Cylinder</param>
		/// <param name="generateUV">Generation of the UV</param>
		public static System.UInt32 CreateCylinder(System.Double radius, System.Double height, System.Int32 sides, System.Boolean generateUV) {
			var ret = Scene_createCylinder(radius, height, sides, generateUV ? 1 : 0);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_createDirectionalLight(Core.Native.Color_c color, System.Double power, Geom.Native.Point3_c direction);
		/// <summary>
		/// Create a new directional light
		/// </summary>
		/// <param name="color">Color of the light</param>
		/// <param name="power">Intensity of the light</param>
		/// <param name="direction">Relative direction of the light</param>
		public static System.UInt32 CreateDirectionalLight(Core.Native.Color color, System.Double power, Geom.Native.Point3 direction) {
			var color_c = new Core.Native.Color_c();
			Core.Native.NativeInterface.ConvertValue(color, ref color_c);
			var direction_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(direction, ref direction_c);
			var ret = Scene_createDirectionalLight(color_c, power, direction_c);
			Core.Native.NativeInterface.Core_Color_free(ref color_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref direction_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_createImmersion(System.Double radius, Int32 subdivisionX, Int32 subdivisionY);
		/// <summary>
		/// Create a new bagel klein
		/// </summary>
		/// <param name="radius">Radius of the Immersion</param>
		/// <param name="subdivisionX">Subdivision of the Immersion on the Latitude</param>
		/// <param name="subdivisionY">Subdivision of the Immersion on the Longitude</param>
		public static System.UInt32 CreateImmersion(System.Double radius, System.Int32 subdivisionX, System.Int32 subdivisionY) {
			var ret = Scene_createImmersion(radius, subdivisionX, subdivisionY);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern MetadataList_c Scene_createMetadatasFromDefinitions(OccurrenceList_c occurrences, MetadataDefinitionList_c definitions);
		/// <summary>
		/// Create Metadata components from definitions
		/// </summary>
		/// <param name="occurrences">List of occurrences to add the metadata components</param>
		/// <param name="definitions">List of metadata definition</param>
		public static MetadataList CreateMetadatasFromDefinitions(OccurrenceList occurrences, MetadataDefinitionList definitions) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			var definitions_c = new Scene.Native.MetadataDefinitionList_c();
			ConvertValue(definitions, ref definitions_c);
			var ret = Scene_createMetadatasFromDefinitions(occurrences_c, definitions_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Scene.Native.NativeInterface.Scene_MetadataDefinitionList_free(ref definitions_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_MetadataList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_createOBBMesh(System.UInt32 occurrence);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="occurrence"></param>
		public static System.UInt32 CreateOBBMesh(System.UInt32 occurrence) {
			var ret = Scene_createOBBMesh(occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_createOccurrence(string name, System.UInt32 parent);
		/// <summary>
		/// Create a new occurrence
		/// </summary>
		/// <param name="name">Name of the new occurrence</param>
		/// <param name="parent">Create the occurrence as a child of parent, if not set the parent will be root</param>
		public static System.UInt32 CreateOccurrence(System.String name, System.UInt32 parent) {
			var ret = Scene_createOccurrence(name, parent);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_createOccurrenceFromSelection(string name, OccurrenceList_c children, System.UInt32 parent, Int32 keepMaterialAssignment);
		/// <summary>
		/// Create a new occurrence and add the given occurrences as children
		/// </summary>
		/// <param name="name">Name of the new occurrence</param>
		/// <param name="children">Add given occurrence as children (if any)</param>
		/// <param name="parent">If defined, the new occurrence will be created as a child of this parent. Else if children are defined, the first common parent of children will be used as a parent for this new occurrence. Last resort will be to use the root as parent</param>
		/// <param name="keepMaterialAssignment">If defined, material assignation will be updated to keep the visual same aspect</param>
		public static System.UInt32 CreateOccurrenceFromSelection(System.String name, OccurrenceList children, System.UInt32 parent, System.Boolean keepMaterialAssignment) {
			var children_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(children, ref children_c);
			var ret = Scene_createOccurrenceFromSelection(name, children_c, parent, keepMaterialAssignment ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref children_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_createOccurrenceFromText(string text, string font, Int32 fontSize, Core.Native.ColorAlpha_c color);
		/// <summary>
		/// Creates an occurrence from string
		/// </summary>
		/// <param name="text">The occurrence's name</param>
		/// <param name="font">The font to use</param>
		/// <param name="fontSize">The font size</param>
		/// <param name="color">The occurrence color</param>
		public static System.UInt32 CreateOccurrenceFromText(System.String text, System.String font, System.Int32 fontSize, Core.Native.ColorAlpha color) {
			var color_c = new Core.Native.ColorAlpha_c();
			Core.Native.NativeInterface.ConvertValue(color, ref color_c);
			var ret = Scene_createOccurrenceFromText(text, font, fontSize, color_c);
			Core.Native.NativeInterface.Core_ColorAlpha_free(ref color_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern OccurrenceList_c Scene_createOccurrences(string name, OccurrenceList_c parents);
		/// <summary>
		/// Create one new occurrence under each given parent
		/// </summary>
		/// <param name="name">Name of the new occurrence</param>
		/// <param name="parents">Create the occurrences as a child of each parent. If empty, one occurrence will be created with root as parent</param>
		public static OccurrenceList CreateOccurrences(System.String name, OccurrenceList parents) {
			var parents_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(parents, ref parents_c);
			var ret = Scene_createOccurrences(name, parents_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref parents_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern PartList_c Scene_createPartsFromMeshes(OccurrenceList_c occurrences, Polygonal.Native.MeshList_c meshes);
		/// <summary>
		/// Create a set of Parts given meshes and occurrences
		/// </summary>
		/// <param name="occurrences">The occurrence which will contains the part component of the mesh at the same index</param>
		/// <param name="meshes">List of mesh to create part, if the mesh is invalid (e.g 0) no part will be created and 0 will be returned in the parts list at this index</param>
		public static PartList CreatePartsFromMeshes(OccurrenceList occurrences, Polygonal.Native.MeshList meshes) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			var meshes_c = new Polygonal.Native.MeshList_c();
			Polygonal.Native.NativeInterface.ConvertValue(meshes, ref meshes_c);
			var ret = Scene_createPartsFromMeshes(occurrences_c, meshes_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Polygonal.Native.NativeInterface.Polygonal_MeshList_free(ref meshes_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_PartList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_createPlane(System.Double sizeY, System.Double sizeX, Int32 subdivisionX, Int32 subdivisionY, Int32 generateUV);
		/// <summary>
		/// Create a  new plane
		/// </summary>
		/// <param name="sizeY">Size of the Plane on the y axis</param>
		/// <param name="sizeX">Size of the Plane on the x axis</param>
		/// <param name="subdivisionX">Subdivision of the Plane on the x axis</param>
		/// <param name="subdivisionY">Subdivision of the Plane on the y axis</param>
		/// <param name="generateUV">Generation of the UV</param>
		public static System.UInt32 CreatePlane(System.Double sizeY, System.Double sizeX, System.Int32 subdivisionX, System.Int32 subdivisionY, System.Boolean generateUV) {
			var ret = Scene_createPlane(sizeY, sizeX, subdivisionX, subdivisionY, generateUV ? 1 : 0);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_createPositionalLight(Core.Native.Color_c color, System.Double power, Geom.Native.Point3_c position);
		/// <summary>
		/// Create a new positional light
		/// </summary>
		/// <param name="color">Color of the light</param>
		/// <param name="power">Intensity of the light</param>
		/// <param name="position">Relative position of the light</param>
		public static System.UInt32 CreatePositionalLight(Core.Native.Color color, System.Double power, Geom.Native.Point3 position) {
			var color_c = new Core.Native.Color_c();
			Core.Native.NativeInterface.ConvertValue(color, ref color_c);
			var position_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(position, ref position_c);
			var ret = Scene_createPositionalLight(color_c, power, position_c);
			Core.Native.NativeInterface.Core_Color_free(ref color_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref position_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_createRayProber();
		/// <summary>
		/// Creates a ray prober
		/// </summary>
		public static System.UInt32 CreateRayProber() {
			var ret = Scene_createRayProber();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_createSceneFromMeshes(Polygonal.Native.MeshList_c meshes, Geom.Native.Matrix4List_c matrices, Int32 centerPartPivots);
		/// <summary>
		/// Create a scene tree with a list of meshes, all meshes becomes part occurrences with the same root. The same mesh Id can be used several times to handle create instances (prototypes)
		/// </summary>
		/// <param name="meshes">List of input meshes</param>
		/// <param name="matrices">List of matrices of input meshes (if empty Identity will be used)</param>
		/// <param name="centerPartPivots">If True, the input meshes will be centered in their local coordinate system and the translation will be set as part matrix. If you want to rollback the meshes to their initial pivots use 'resetPartTransform' function</param>
		public static System.UInt32 CreateSceneFromMeshes(Polygonal.Native.MeshList meshes, Geom.Native.Matrix4List matrices, System.Boolean centerPartPivots) {
			var meshes_c = new Polygonal.Native.MeshList_c();
			Polygonal.Native.NativeInterface.ConvertValue(meshes, ref meshes_c);
			var matrices_c = new Geom.Native.Matrix4List_c();
			Geom.Native.NativeInterface.ConvertValue(matrices, ref matrices_c);
			var ret = Scene_createSceneFromMeshes(meshes_c, matrices_c, centerPartPivots ? 1 : 0);
			Polygonal.Native.NativeInterface.Polygonal_MeshList_free(ref meshes_c);
			Geom.Native.NativeInterface.Geom_Matrix4List_free(ref matrices_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_createSphere(System.Double radius, Int32 subdivisionLatitude, Int32 subdivisionLongitude, Int32 generateUV);
		/// <summary>
		/// Create a new shpere
		/// </summary>
		/// <param name="radius">Radius of the Sphere</param>
		/// <param name="subdivisionLatitude">Subdivision of the Sphere on the Latitude</param>
		/// <param name="subdivisionLongitude">Subdivision of the Sphere on the Longitude</param>
		/// <param name="generateUV">Generation of the UV</param>
		public static System.UInt32 CreateSphere(System.Double radius, System.Int32 subdivisionLatitude, System.Int32 subdivisionLongitude, System.Boolean generateUV) {
			var ret = Scene_createSphere(radius, subdivisionLatitude, subdivisionLongitude, generateUV ? 1 : 0);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_createSphereProber();
		/// <summary>
		/// Creates a sphere prober
		/// </summary>
		public static System.UInt32 CreateSphereProber() {
			var ret = Scene_createSphereProber();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_createSpotLight(Core.Native.Color_c color, System.Double power, Geom.Native.Point3_c position, Geom.Native.Point3_c direction, System.Double cutoff);
		/// <summary>
		/// Create a new spot light
		/// </summary>
		/// <param name="color">Color of the light</param>
		/// <param name="power">Intensity of the light</param>
		/// <param name="position">Relative position of the light</param>
		/// <param name="direction">Relative direction of the light</param>
		/// <param name="cutoff">Cutoff angle of the spot light</param>
		public static System.UInt32 CreateSpotLight(Core.Native.Color color, System.Double power, Geom.Native.Point3 position, Geom.Native.Point3 direction, System.Double cutoff) {
			var color_c = new Core.Native.Color_c();
			Core.Native.NativeInterface.ConvertValue(color, ref color_c);
			var position_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(position, ref position_c);
			var direction_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(direction, ref direction_c);
			var ret = Scene_createSpotLight(color_c, power, position_c, direction_c, cutoff);
			Core.Native.NativeInterface.Core_Color_free(ref color_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref position_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref direction_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_createTorus(System.Double majorRadius, System.Double minorRadius, Int32 subdivisionLatitude, Int32 subdivisionLongitude);
		/// <summary>
		/// Create a new torus
		/// </summary>
		/// <param name="majorRadius">Major Radius</param>
		/// <param name="minorRadius">Minor Radius</param>
		/// <param name="subdivisionLatitude">Subdivision of the Torus on the Latitude</param>
		/// <param name="subdivisionLongitude">Subdivision of the Torus on the Longitude</param>
		public static System.UInt32 CreateTorus(System.Double majorRadius, System.Double minorRadius, System.Int32 subdivisionLatitude, System.Int32 subdivisionLongitude) {
			var ret = Scene_createTorus(majorRadius, minorRadius, subdivisionLatitude, subdivisionLongitude);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_deleteComponentByType(Int32 componentType, System.UInt32 occurrence, Int32 followPrototypes);
		/// <summary>
		/// Delete component from type
		/// </summary>
		/// <param name="componentType">Type of the component</param>
		/// <param name="occurrence">The occurrence to remove components from</param>
		/// <param name="followPrototypes">If true and if the component is not set on the occurrence, try to find it on its prototyping chain</param>
		public static void DeleteComponentByType(ComponentType componentType, System.UInt32 occurrence, System.Boolean followPrototypes) {
			Scene_deleteComponentByType((int)componentType, occurrence, followPrototypes ? 1 : 0);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_deleteComponentsByType(Int32 componentType, System.UInt32 rootOccurrence);
		/// <summary>
		/// Delete all components on subtree from type
		/// </summary>
		/// <param name="componentType">Type of the component</param>
		/// <param name="rootOccurrence">The root occurrence to remove components from</param>
		public static void DeleteComponentsByType(ComponentType componentType, System.UInt32 rootOccurrence) {
			Scene_deleteComponentsByType((int)componentType, rootOccurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_deleteEmptyOccurrences(System.UInt32 root);
		/// <summary>
		/// Delete all empty assemblies
		/// </summary>
		/// <param name="root">Root occurrence for the process</param>
		public static void DeleteEmptyOccurrences(System.UInt32 root) {
			Scene_deleteEmptyOccurrences(root);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_deleteOccurrences(OccurrenceList_c occurrences);
		/// <summary>
		/// Delete a liste of occurrences
		/// </summary>
		/// <param name="occurrences">Occurrences to delete</param>
		public static void DeleteOccurrences(OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			Scene_deleteOccurrences(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern OccurrenceList_c Scene_findByActiveMaterial(System.UInt32 material, OccurrenceList_c roots);
		/// <summary>
		/// Find all part occurrence with a given material as active material (i.e. as seen in the rendering)
		/// </summary>
		/// <param name="material">A material</param>
		/// <param name="roots">If specified, restrict the search from the given roots</param>
		public static OccurrenceList FindByActiveMaterial(System.UInt32 material, OccurrenceList roots) {
			var roots_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(roots, ref roots_c);
			var ret = Scene_findByActiveMaterial(material, roots_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref roots_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern OccurrenceList_c Scene_findByMetadata(string property, string regex, OccurrenceList_c roots);
		/// <summary>
		/// Returns all occurrences which a metadata property value matches the given regular expression (ECMAScript)
		/// </summary>
		/// <param name="property">Property name</param>
		/// <param name="regex">Regular expression (ECMAScript)</param>
		/// <param name="roots">If specified, restrict the search from the given roots</param>
		public static OccurrenceList FindByMetadata(System.String property, System.String regex, OccurrenceList roots) {
			var roots_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(roots, ref roots_c);
			var ret = Scene_findByMetadata(property, regex, roots_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref roots_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern OccurrenceList_c Scene_findByProperty(string property, string regex, OccurrenceList_c roots);
		/// <summary>
		/// Returns all occurrences which a property value matches the given regular expression (ECMAScript)
		/// </summary>
		/// <param name="property">Property name</param>
		/// <param name="regex">Regular expression (ECMAScript)</param>
		/// <param name="roots">If specified, restrict the search from the given roots</param>
		public static OccurrenceList FindByProperty(System.String property, System.String regex, OccurrenceList roots) {
			var roots_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(roots, ref roots_c);
			var ret = Scene_findByProperty(property, regex, roots_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref roots_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern OccurrenceList_c Scene_findPartOccurrencesInAABB(Geom.Native.AABB_c aabb);
		/// <summary>
		/// find part occurences in the scene in a given axis aligned bounding box
		/// </summary>
		/// <param name="aabb">The axis aligned bounding box</param>
		public static OccurrenceList FindPartOccurrencesInAABB(Geom.Native.AABB aabb) {
			var aabb_c = new Geom.Native.AABB_c();
			Geom.Native.NativeInterface.ConvertValue(aabb, ref aabb_c);
			var ret = Scene_findPartOccurrencesInAABB(aabb_c);
			Geom.Native.NativeInterface.Geom_AABB_free(ref aabb_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_generateOctaViews(System.Double radius, Int32 XFrames, Int32 YFrames, Int32 hemi);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="radius"></param>
		/// <param name="XFrames"></param>
		/// <param name="YFrames"></param>
		/// <param name="hemi"></param>
		public static System.UInt32 GenerateOctaViews(System.Double radius, System.Int32 XFrames, System.Int32 YFrames, System.Boolean hemi) {
			var ret = Scene_generateOctaViews(radius, XFrames, YFrames, hemi ? 1 : 0);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Geom.Native.AABB_c Scene_getAABB(OccurrenceList_c occurrences);
		/// <summary>
		/// Returns the axis aligned bounding box of a list of scene paths
		/// </summary>
		/// <param name="occurrences">List of occurrences to retrieve the AABB</param>
		public static Geom.Native.AABB GetAABB(OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			var ret = Scene_getAABB(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Geom.Native.NativeInterface.ConvertValue(ref ret);
			Geom.Native.NativeInterface.Geom_AABB_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getActiveMaterial(System.UInt32 occurrence);
		/// <summary>
		/// Get the active material on occurrence
		/// </summary>
		/// <param name="occurrence">The occurrence</param>
		public static System.UInt32 GetActiveMaterial(System.UInt32 occurrence) {
			var ret = Scene_getActiveMaterial(occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Scene_getActivePropertyValue(System.UInt32 occurrence, string propertyName, Int32 cacheProperty);
		/// <summary>
		/// Get the value of a property on the first parent that own it
		/// </summary>
		/// <param name="occurrence">An occurrence</param>
		/// <param name="propertyName">Property name</param>
		/// <param name="cacheProperty">If true, the property will be copied on all ancestor of occurrence below the property owner to speed up future calls</param>
		public static System.String GetActivePropertyValue(System.UInt32 occurrence, System.String propertyName, System.Boolean cacheProperty) {
			var ret = Scene_getActivePropertyValue(occurrence, propertyName, cacheProperty ? 1 : 0);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Core.Native.StringList_c Scene_getActivePropertyValues(OccurrenceList_c occurrences, string propertyName, Int32 cacheProperty);
		/// <summary>
		/// Get the value of a property on the first parent that own it for each given occurrence
		/// </summary>
		/// <param name="occurrences">List of occurrences</param>
		/// <param name="propertyName">Property name</param>
		/// <param name="cacheProperty">If true, the property will be copied on all ancestor of occurrence below the property owner to speed up future calls</param>
		public static Core.Native.StringList GetActivePropertyValues(OccurrenceList occurrences, System.String propertyName, System.Boolean cacheProperty) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			var ret = Scene_getActivePropertyValues(occurrences_c, propertyName, cacheProperty ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Core.Native.NativeInterface.ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_StringList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern AnnotationGroupList_c Scene_getAnnotationGroups(System.UInt32 pmiComponent);
		/// <summary>
		/// Returns the list of the AnnotationGroup from a PMIComponent
		/// </summary>
		/// <param name="pmiComponent">The pmi component</param>
		public static AnnotationGroupList GetAnnotationGroups(System.UInt32 pmiComponent) {
			var ret = Scene_getAnnotationGroups(pmiComponent);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_AnnotationGroupList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern AnnotationList_c Scene_getAnnotations(System.UInt32 group);
		/// <summary>
		/// Returns the list of the Annotation from a AnnotationGroup
		/// </summary>
		/// <param name="group">The AnnotationGroup</param>
		public static AnnotationList GetAnnotations(System.UInt32 group) {
			var ret = Scene_getAnnotations(group);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_AnnotationList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern OccurrenceList_c Scene_getChildren(System.UInt32 occurrence);
		/// <summary>
		/// Get the children of an occurrence
		/// </summary>
		/// <param name="occurrence">The occurrence</param>
		public static OccurrenceList GetChildren(System.UInt32 occurrence) {
			var ret = Scene_getChildren(occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern PackedTree_c Scene_getCompleteTree(System.UInt32 root, Int32 visibilityMode);
		/// <summary>
		/// Returns a packed version of the whole scene tree
		/// </summary>
		/// <param name="root">Specify the root of the returned scene</param>
		/// <param name="visibilityMode">The visibility mode</param>
		public static PackedTree GetCompleteTree(System.UInt32 root, VisibilityMode visibilityMode) {
			var ret = Scene_getCompleteTree(root, (int)visibilityMode);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_PackedTree_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getComponent(System.UInt32 occurrence, Int32 componentType, Int32 followPrototypes);
		/// <summary>
		/// Returns a component on an occurrence
		/// </summary>
		/// <param name="occurrence">The occurrence</param>
		/// <param name="componentType">Type of the component</param>
		/// <param name="followPrototypes">If true and if the component is not set on the occurrence, try to find it on its prototyping chain</param>
		public static System.UInt32 GetComponent(System.UInt32 occurrence, ComponentType componentType, System.Boolean followPrototypes) {
			var ret = Scene_getComponent(occurrence, (int)componentType, followPrototypes ? 1 : 0);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern ComponentList_c Scene_getComponentByOccurrence(OccurrenceList_c occurrences, Int32 componentType, Int32 followPrototypes);
		/// <summary>
		/// Returns one component of the specified type by occurrence if it exists
		/// </summary>
		/// <param name="occurrences">The occurrences list</param>
		/// <param name="componentType">Type of the component</param>
		/// <param name="followPrototypes">If true and if the component is not set on the occurrence, try to find it on its prototyping chain</param>
		public static ComponentList GetComponentByOccurrence(OccurrenceList occurrences, ComponentType componentType, System.Boolean followPrototypes) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			var ret = Scene_getComponentByOccurrence(occurrences_c, (int)componentType, followPrototypes ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_ComponentList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getComponentOccurrence(System.UInt32 component);
		/// <summary>
		/// Get the occurrence that own a component
		/// </summary>
		/// <param name="component">The component</param>
		public static System.UInt32 GetComponentOccurrence(System.UInt32 component) {
			var ret = Scene_getComponentOccurrence(component);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Scene_getComponentType(System.UInt32 component);
		/// <summary>
		/// Get the type of a component
		/// </summary>
		/// <param name="component">The component</param>
		public static ComponentType GetComponentType(System.UInt32 component) {
			var ret = Scene_getComponentType(component);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (ComponentType)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Geom.Native.Matrix4_c Scene_getGlobalMatrix(System.UInt32 occurrence);
		/// <summary>
		/// Returns the global matrix on an occurrence
		/// </summary>
		/// <param name="occurrence">Occurrence to get the global matrix</param>
		public static Geom.Native.Matrix4 GetGlobalMatrix(System.UInt32 occurrence) {
			var ret = Scene_getGlobalMatrix(occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Geom.Native.NativeInterface.ConvertValue(ref ret);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Scene_getGlobalVisibility(System.UInt32 occurrence);
		/// <summary>
		/// Returns the global visibility of a given occurrence
		/// </summary>
		/// <param name="occurrence">Occurrence to get the global visibility</param>
		public static System.Boolean GetGlobalVisibility(System.UInt32 occurrence) {
			var ret = Scene_getGlobalVisibility(occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Boolean)(0 != ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Geom.Native.Matrix4_c Scene_getLocalMatrix(System.UInt32 occurrence);
		/// <summary>
		/// Returns the local matrix on an occurrence
		/// </summary>
		/// <param name="occurrence">Node to get the local matrix</param>
		public static Geom.Native.Matrix4 GetLocalMatrix(System.UInt32 occurrence) {
			var ret = Scene_getLocalMatrix(occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Geom.Native.NativeInterface.ConvertValue(ref ret);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Geom.Native.OBB_c Scene_getMBB(OccurrenceList_c occurrences);
		/// <summary>
		/// Returns the Minimum Bounding Box of a list of scene paths
		/// </summary>
		/// <param name="occurrences">List of occurrences to retrieve the AABB</param>
		public static Geom.Native.OBB GetMBB(OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			var ret = Scene_getMBB(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Geom.Native.NativeInterface.ConvertValue(ref ret);
			Geom.Native.NativeInterface.Geom_OBB_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Scene_getMetadata(System.UInt32 metadata, string name);
		/// <summary>
		/// Get a metadata property value from a metadata component
		/// </summary>
		/// <param name="metadata">The metadata component</param>
		/// <param name="name">The metadata property name</param>
		public static System.String GetMetadata(System.UInt32 metadata, System.String name) {
			var ret = Scene_getMetadata(metadata, name);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern MetadataDefinitionList_c Scene_getMetadatasDefinitions(MetadataList_c metadatas);
		/// <summary>
		/// Returns definition of Metadata components
		/// </summary>
		/// <param name="metadatas">List of metadata component to retrieve definition</param>
		public static MetadataDefinitionList GetMetadatasDefinitions(MetadataList metadatas) {
			var metadatas_c = new Scene.Native.MetadataList_c();
			ConvertValue(metadatas, ref metadatas_c);
			var ret = Scene_getMetadatasDefinitions(metadatas_c);
			Scene.Native.NativeInterface.Scene_MetadataList_free(ref metadatas_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_MetadataDefinitionList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Scene_getNodeName(System.UInt32 occurrence);
		/// <summary>
		/// Returns the name of an occurrence
		/// </summary>
		/// <param name="occurrence">The occurrence to get the name</param>
		public static System.String GetNodeName(System.UInt32 occurrence) {
			var ret = Scene_getNodeName(occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Geom.Native.OBB_c Scene_getOBB(OccurrenceList_c occurrences);
		/// <summary>
		/// Returns the Oriented Bounding Box of a list of scene paths (works only on meshes, fast method, not the Minimum Volume Box)
		/// </summary>
		/// <param name="occurrences">List of occurrences to retrieve the AABB</param>
		public static Geom.Native.OBB GetOBB(OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			var ret = Scene_getOBB(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Geom.Native.NativeInterface.ConvertValue(ref ret);
			Geom.Native.NativeInterface.Geom_OBB_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getOccurrenceActiveMaterial(System.UInt32 occurrence);
		/// <summary>
		/// Returns the active material on a given occurrence
		/// </summary>
		/// <param name="occurrence">Occurrence to get the active material</param>
		public static System.UInt32 GetOccurrenceActiveMaterial(System.UInt32 occurrence) {
			var ret = Scene_getOccurrenceActiveMaterial(occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getParent(System.UInt32 occurrence);
		/// <summary>
		/// Get the parent of an occurrence
		/// </summary>
		/// <param name="occurrence">The occurrence</param>
		public static System.UInt32 GetParent(System.UInt32 occurrence) {
			var ret = Scene_getParent(occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getPartActiveShape(System.UInt32 part);
		/// <summary>
		/// Returns the active shape of a part
		/// </summary>
		/// <param name="part">The part</param>
		public static System.UInt32 GetPartActiveShape(System.UInt32 part) {
			var ret = Scene_getPartActiveShape(part);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern OccurrenceList_c Scene_getPartOccurrences(System.UInt32 from);
		/// <summary>
		/// Recursively get all the occurrences containing a part component
		/// </summary>
		/// <param name="from">Source occurrence of the recursion</param>
		public static OccurrenceList GetPartOccurrences(System.UInt32 from) {
			var ret = Scene_getPartOccurrences(from);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Scene_getPolygonCount(OccurrenceList_c occurrences, Int32 asTriangleCount, Int32 countOnceEachInstance, Int32 countHidden);
		/// <summary>
		/// Returns the number of polygon in the parts meshes
		/// </summary>
		/// <param name="occurrences">The part occurrences</param>
		/// <param name="asTriangleCount">If true count the equivalent of triangles for each polygon</param>
		/// <param name="countOnceEachInstance">If true ignore multiple instance of each tessellation</param>
		/// <param name="countHidden">If true, also count hidden components</param>
		public static System.Int32 GetPolygonCount(OccurrenceList occurrences, System.Boolean asTriangleCount, System.Boolean countOnceEachInstance, System.Boolean countHidden) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			var ret = Scene_getPolygonCount(occurrences_c, asTriangleCount ? 1 : 0, countOnceEachInstance ? 1 : 0, countHidden ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Int32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern OccurrenceList_c Scene_getReferencers(System.UInt32 prototype);
		/// <summary>
		/// Returns all the occurrences prototyping the given occurrence
		/// </summary>
		/// <param name="prototype">The prototype occurrence</param>
		public static OccurrenceList GetReferencers(System.UInt32 prototype) {
			var ret = Scene_getReferencers(prototype);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getRoot();
		/// <summary>
		/// Get the root occurrence of the product structure
		/// </summary>
		public static System.UInt32 GetRoot() {
			var ret = Scene_getRoot();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getSubTreeStatsReturn_c Scene_getSubTreeStats(System.UInt32 root);
		/// <summary>
		/// Returns some stats of a sub tree
		/// </summary>
		/// <param name="root">The root of the sub tree</param>
		public static Scene.Native.getSubTreeStatsReturn GetSubTreeStats(System.UInt32 root) {
			var ret = Scene_getSubTreeStats(root);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			Scene.Native.getSubTreeStatsReturn retStruct = new Scene.Native.getSubTreeStatsReturn();
			retStruct.partCount = (System.Int32)ret.partCount;
			retStruct.partOccurrenceCount = (System.Int32)ret.partOccurrenceCount;
			retStruct.triangleCount = (System.Int32)ret.triangleCount;
			retStruct.triangleOccurrenceCount = (System.Int32)ret.triangleOccurrenceCount;
			retStruct.vertexCount = (System.Int32)ret.vertexCount;
			retStruct.vertexOccurrenceCount = (System.Int32)ret.vertexOccurrenceCount;
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Scene_getVertexCount(OccurrenceList_c occurrences, Int32 countOnceEachInstance, Int32 countHidden, Int32 countPoints, Int32 countMergedVertices);
		/// <summary>
		/// Returns the number of vertices in the parts meshes
		/// </summary>
		/// <param name="occurrences">The part occurrences</param>
		/// <param name="countOnceEachInstance">If true ignore multiple instance of each tessellation</param>
		/// <param name="countHidden">If true, also count hidden components</param>
		/// <param name="countPoints">If true, also count points (for points cloud)</param>
		/// <param name="countMergedVertices">If true count all merged vertices in each tessellation</param>
		public static System.Int32 GetVertexCount(OccurrenceList occurrences, System.Boolean countOnceEachInstance, System.Boolean countHidden, System.Boolean countPoints, System.Boolean countMergedVertices) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			var ret = Scene_getVertexCount(occurrences_c, countOnceEachInstance ? 1 : 0, countHidden ? 1 : 0, countPoints ? 1 : 0, countMergedVertices ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Int32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getViewpointsFromCavitiesReturn_c Scene_getViewpointsFromCavities(System.Double voxelSize, System.Double minCavityVolume);
		/// <summary>
		/// Returns viewpoints from model cavities
		/// </summary>
		/// <param name="voxelSize">Precision for cavities detection</param>
		/// <param name="minCavityVolume">Minimum volume for a cavity to be returned</param>
		public static Scene.Native.getViewpointsFromCavitiesReturn GetViewpointsFromCavities(System.Double voxelSize, System.Double minCavityVolume) {
			var ret = Scene_getViewpointsFromCavities(voxelSize, minCavityVolume);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			Scene.Native.getViewpointsFromCavitiesReturn retStruct = new Scene.Native.getViewpointsFromCavitiesReturn();
			retStruct.positions = ConvertValue(ref ret.positions);
			Geom.Native.NativeInterface.Geom_Point3List_free(ref ret.positions);
			retStruct.directions = ConvertValue(ref ret.directions);
			Geom.Native.NativeInterface.Geom_Point3List_free(ref ret.directions);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Scene_hasComponent(System.UInt32 occurrence, Int32 componentType, Int32 followPrototypes);
		/// <summary>
		/// Returns True if the given occurrence has the given component type
		/// </summary>
		/// <param name="occurrence">The occurrence</param>
		/// <param name="componentType">Type of the component</param>
		/// <param name="followPrototypes">If true and if the component is not set on the occurrence, try to find it on its prototyping chain</param>
		public static System.Boolean HasComponent(System.UInt32 occurrence, ComponentType componentType, System.Boolean followPrototypes) {
			var ret = Scene_hasComponent(occurrence, (int)componentType, followPrototypes ? 1 : 0);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Boolean)(0 != ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_hide(System.UInt32 occurrence);
		/// <summary>
		/// Hide the given occurrence
		/// </summary>
		/// <param name="occurrence">The occurrence to hide</param>
		public static void Hide(System.UInt32 occurrence) {
			Scene_hide(occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_insertDefaultLightsInTree();
		/// <summary>
		/// Create the default light
		/// </summary>
		public static void InsertDefaultLightsInTree() {
			Scene_insertDefaultLightsInTree();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern ComponentList_c Scene_listComponent(Int32 componentType);
		/// <summary>
		/// List all components on a type on the whole tree
		/// </summary>
		/// <param name="componentType">The component type</param>
		public static ComponentList ListComponent(ComponentType componentType) {
			var ret = Scene_listComponent((int)componentType);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_ComponentList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern ComponentList_c Scene_listComponents(System.UInt32 occurrence, Int32 followPrototypes);
		/// <summary>
		/// List all components on an occurrence
		/// </summary>
		/// <param name="occurrence">The occurrence to list the components</param>
		/// <param name="followPrototypes">If true list also components owned by the prototype</param>
		public static ComponentList ListComponents(System.UInt32 occurrence, System.Boolean followPrototypes) {
			var ret = Scene_listComponents(occurrence, followPrototypes ? 1 : 0);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_ComponentList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Material.Native.MaterialList_c Scene_listPartSubMaterials(System.UInt32 part);
		/// <summary>
		/// list all the materials used in the part shape
		/// </summary>
		/// <param name="part">The part which contains sub materials</param>
		public static Material.Native.MaterialList ListPartSubMaterials(System.UInt32 part) {
			var ret = Scene_listPartSubMaterials(part);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Material.Native.NativeInterface.ConvertValue(ref ret);
			Material.Native.NativeInterface.Material_MaterialList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Scene_mergeImages();
		/// <summary>
		/// Merge all equivalent images (i.e. with same pixels)
		/// </summary>
		public static System.Int32 MergeImages() {
			var ret = Scene_mergeImages();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Int32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Scene_mergeMaterials();
		/// <summary>
		/// Merge all equivalent materials (i.e. with same appearance)
		/// </summary>
		public static System.Int32 MergeMaterials() {
			var ret = Scene_mergeMaterials();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Int32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_moveOccurrences(OccurrenceList_c occurrences, System.UInt32 destination);
		/// <summary>
		/// Move an occurrence, adjusting the transformation to keep objects at the same place in the world space
		/// </summary>
		/// <param name="occurrences">The occurrences to move</param>
		/// <param name="destination">Destination occurrence (the new parent)</param>
		public static void MoveOccurrences(OccurrenceList occurrences, System.UInt32 destination) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			Scene_moveOccurrences(occurrences_c, destination);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_removeMetadata(System.UInt32 metadata, string name);
		/// <summary>
		/// Remove a property from a metadata
		/// </summary>
		/// <param name="metadata">The occurrence</param>
		/// <param name="name">The name of the property</param>
		public static void RemoveMetadata(System.UInt32 metadata, System.String name) {
			Scene_removeMetadata(metadata, name);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_renameLongOccurrenceName(Int32 maxLength);
		/// <summary>
		/// truncate names of occurrence with too long names
		/// </summary>
		/// <param name="maxLength">Maximum name length</param>
		public static void RenameLongOccurrenceName(System.Int32 maxLength) {
			Scene_renameLongOccurrenceName(maxLength);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_replaceMaterial(System.UInt32 originalMaterial, System.UInt32 newMaterial, OccurrenceList_c occurrences);
		/// <summary>
		/// Replace a material by another everywhere it is used
		/// </summary>
		/// <param name="originalMaterial">The material to replace everywhere</param>
		/// <param name="newMaterial">The new material to set in place of originalMaterial</param>
		/// <param name="occurrences">The occurrences on which replacing the materials</param>
		public static void ReplaceMaterial(System.UInt32 originalMaterial, System.UInt32 newMaterial, OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			Scene_replaceMaterial(originalMaterial, newMaterial, occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_resizeTextures(ResizeTexturesInputMode_c inputMode, ResizeTexturesResizeMode_c resizeMode, Int32 replaceTextures);
		/// <summary>
		/// Resizes the textures from a selection of occurrences (resizes all textures used by these occurrences), or from a selection of textures
		/// </summary>
		/// <param name="inputMode">Defines if the textures to resize are textures used by a selection of Occurrences, or a selection among the textures available in the scene</param>
		/// <param name="resizeMode">Defines if the textures are resized following a ratio or following a maximum size/resolution (only textures above the defined maximum are downsized)</param>
		/// <param name="replaceTextures">If True, overwrites textures from the selection</param>
		public static void ResizeTextures(ResizeTexturesInputMode inputMode, ResizeTexturesResizeMode resizeMode, System.Boolean replaceTextures) {
			var inputMode_c = new Scene.Native.ResizeTexturesInputMode_c();
			ConvertValue(inputMode, ref inputMode_c);
			var resizeMode_c = new Scene.Native.ResizeTexturesResizeMode_c();
			ConvertValue(resizeMode, ref resizeMode_c);
			Scene_resizeTextures(inputMode_c, resizeMode_c, replaceTextures ? 1 : 0);
			Scene.Native.NativeInterface.Scene_ResizeTexturesInputMode_free(ref inputMode_c);
			Scene.Native.NativeInterface.Scene_ResizeTexturesResizeMode_free(ref resizeMode_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_selectByMaterial(System.UInt32 material);
		/// <summary>
		/// Selects occurrences for which the property "Material" is the given material
		/// </summary>
		/// <param name="material">A material</param>
		public static void SelectByMaterial(System.UInt32 material) {
			Scene_selectByMaterial(material);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_selectByVisibleMaterial(System.UInt32 material);
		/// <summary>
		/// Selects parts for which the given material is visible in the viewer
		/// </summary>
		/// <param name="material">A material</param>
		public static void SelectByVisibleMaterial(System.UInt32 material) {
			Scene_selectByVisibleMaterial(material);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_selectPartOccurrencesInBox(Geom.Native.ExtendedBox_c box, Int32 strictlyIncludes);
		/// <summary>
		/// find part occurences in the scene in a given box and add them to the selection
		/// </summary>
		/// <param name="box">The extension box</param>
		/// <param name="strictlyIncludes">If false, parts only need to intersect the box to be selected</param>
		public static void SelectPartOccurrencesInBox(Geom.Native.ExtendedBox box, System.Boolean strictlyIncludes) {
			var box_c = new Geom.Native.ExtendedBox_c();
			Geom.Native.NativeInterface.ConvertValue(box, ref box_c);
			Scene_selectPartOccurrencesInBox(box_c, strictlyIncludes ? 1 : 0);
			Geom.Native.NativeInterface.Geom_ExtendedBox_free(ref box_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_setComponentOccurrence(System.UInt32 component, System.UInt32 occurrence);
		/// <summary>
		/// Move a component to an occurrence
		/// </summary>
		/// <param name="component">The component</param>
		/// <param name="occurrence">The occurrence</param>
		public static void SetComponentOccurrence(System.UInt32 component, System.UInt32 occurrence) {
			Scene_setComponentOccurrence(component, occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_setDefaultVariant();
		/// <summary>
		/// Set the default variant
		/// </summary>
		public static void SetDefaultVariant() {
			Scene_setDefaultVariant();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_setOccurrenceMaterial(System.UInt32 occurrence, System.UInt32 material);
		/// <summary>
		/// Set the material on a occurrence
		/// </summary>
		/// <param name="occurrence">Occurrence to set the material</param>
		/// <param name="material">The new occurrence material</param>
		public static void SetOccurrenceMaterial(System.UInt32 occurrence, System.UInt32 material) {
			Scene_setOccurrenceMaterial(occurrence, material);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_setParent(System.UInt32 occurrence, System.UInt32 parent, Int32 addInParentInstances, System.UInt32 insertBefore);
		/// <summary>
		/// Set the parent of an occurrence
		/// </summary>
		/// <param name="occurrence">The occurrence</param>
		/// <param name="parent">The parent occurrence</param>
		/// <param name="addInParentInstances">If True, each occurrence whose prototype is the target parent will generate a child whose prototype is the occurrence itself</param>
		/// <param name="insertBefore">Add before this child occurrence in the children list of the parent occurrence</param>
		public static void SetParent(System.UInt32 occurrence, System.UInt32 parent, System.Boolean addInParentInstances, System.UInt32 insertBefore) {
			Scene_setParent(occurrence, parent, addInParentInstances ? 1 : 0, insertBefore);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_show(System.UInt32 occurrence);
		/// <summary>
		/// Show the given occurrence
		/// </summary>
		/// <param name="occurrence">The occurrence to show</param>
		public static void Show(System.UInt32 occurrence) {
			Scene_show(occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_showOnly(System.UInt32 occurrence);
		/// <summary>
		/// Show only the given occurrence
		/// </summary>
		/// <param name="occurrence">The occurrence to show</param>
		public static void ShowOnly(System.UInt32 occurrence) {
			Scene_showOnly(occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_updateRayProber(System.UInt32 proberID, Geom.Native.Matrix4_c matrix);
		/// <summary>
		/// Updates the designed ray prober
		/// </summary>
		/// <param name="proberID">The ray prober Id</param>
		/// <param name="matrix">The new ray prober matrix</param>
		public static void UpdateRayProber(System.UInt32 proberID, Geom.Native.Matrix4 matrix) {
			var matrix_c = new Geom.Native.Matrix4_c();
			Geom.Native.NativeInterface.ConvertValue(matrix, ref matrix_c);
			Scene_updateRayProber(proberID, matrix_c);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref matrix_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_updateSphereProber(System.UInt32 proberID, Geom.Native.Point3_c sphereCenter, System.Double sphereRadius);
		/// <summary>
		/// Updates the designed sphere prober
		/// </summary>
		/// <param name="proberID">The sphere prober Id</param>
		/// <param name="sphereCenter">The new prober center</param>
		/// <param name="sphereRadius">The new prober radius</param>
		public static void UpdateSphereProber(System.UInt32 proberID, Geom.Native.Point3 sphereCenter, System.Double sphereRadius) {
			var sphereCenter_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(sphereCenter, ref sphereCenter_c);
			Scene_updateSphereProber(proberID, sphereCenter_c, sphereRadius);
			Geom.Native.NativeInterface.Geom_Point3_free(ref sphereCenter_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region LODs

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_addLOD(System.UInt32 component, System.UInt32 occurrence, System.Double distance);
		/// <summary>
		/// Add a LOD to a LODComponent
		/// </summary>
		/// <param name="component">The LODComponent where to add the LOD</param>
		/// <param name="occurrence">The occurrence referenced by this LOD (if the occurrence is already used in another LOD, an exception will be thrown)</param>
		/// <param name="distance">The distance of activation of this LOD</param>
		public static System.UInt32 AddLOD(System.UInt32 component, System.UInt32 occurrence, System.Double distance) {
			var ret = Scene_addLOD(component, occurrence, distance);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_createHierarchicalClusters(System.UInt32 root, Int32 maxDepth, System.Double minFitting);
		/// <summary>
		/// Reorganize a sub tree with hierarchical clustering
		/// </summary>
		/// <param name="root">Root of the sub-tree to reorganize</param>
		/// <param name="maxDepth">Minimal fitting coefficient to allow 2 nodes to be merged/clustered</param>
		/// <param name="minFitting">Minimal fitting coefficient to allow 2 nodes to be merged/clustered</param>
		public static void CreateHierarchicalClusters(System.UInt32 root, System.Int32 maxDepth, System.Double minFitting) {
			Scene_createHierarchicalClusters(root, maxDepth, minFitting);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getLODComponentOfLOD(System.UInt32 lod);
		/// <summary>
		/// Retrieve the LOD component that use a given LOD
		/// </summary>
		/// <param name="lod">A LOD</param>
		public static System.UInt32 GetLODComponentOfLOD(System.UInt32 lod) {
			var ret = Scene_getLODComponentOfLOD(lod);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern LODComponentList_c Scene_getLODComponents();
		/// <summary>
		/// Get all LODComponent of the scene
		/// </summary>
		public static LODComponentList GetLODComponents() {
			var ret = Scene_getLODComponents();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_LODComponentList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern LODList_c Scene_getLODs(System.UInt32 component);
		/// <summary>
		/// Get all LODs of a LODComponent
		/// </summary>
		/// <param name="component">The LODComponent where to add the LOD</param>
		public static LODList GetLODs(System.UInt32 component) {
			var ret = Scene_getLODs(component);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_LODList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getOccurrenceLOD(System.UInt32 occurrence);
		/// <summary>
		/// Retrieve the LOD that references a given occurrence (if any)
		/// </summary>
		/// <param name="occurrence">An occurrence referenced by a LOD</param>
		public static System.UInt32 GetOccurrenceLOD(System.UInt32 occurrence) {
			var ret = Scene_getOccurrenceLOD(occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		#endregion

		#region OoC

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_configureOoC(System.UInt32 ooc, string implementationType, string implementationParameters);
		/// <summary>
		/// Set and configure an Out of Core component implementation
		/// </summary>
		/// <param name="ooc">An OoC component</param>
		/// <param name="implementationType">An implementation type (see listOoCImplementations)</param>
		/// <param name="implementationParameters">Depends of implementation type</param>
		public static void ConfigureOoC(System.UInt32 ooc, System.String implementationType, System.String implementationParameters) {
			Scene_configureOoC(ooc, implementationType, implementationParameters);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_dumpOoC(System.UInt32 ooc);
		/// <summary>
		/// Ask an Out of Core component to save its sub-scene
		/// </summary>
		/// <param name="ooc">An OoC component</param>
		public static void DumpOoC(System.UInt32 ooc) {
			Scene_dumpOoC(ooc);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getOoCConfigurationReturn_c Scene_getOoCConfiguration(System.UInt32 ooc);
		/// <summary>
		/// Get the current configuration of an Out of Core component
		/// </summary>
		/// <param name="ooc">An OoC component</param>
		public static Scene.Native.getOoCConfigurationReturn GetOoCConfiguration(System.UInt32 ooc) {
			var ret = Scene_getOoCConfiguration(ooc);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			Scene.Native.getOoCConfigurationReturn retStruct = new Scene.Native.getOoCConfigurationReturn();
			retStruct.implementationType = ConvertValue(ret.implementationType);
			retStruct.implementationParameters = ConvertValue(ret.implementationParameters);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Core.Native.StringList_c Scene_listOoCImplementations();
		/// <summary>
		/// 
		/// </summary>
		public static Core.Native.StringList ListOoCImplementations() {
			var ret = Scene_listOoCImplementations();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Core.Native.NativeInterface.ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_StringList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_loadOoC(System.UInt32 ooc);
		/// <summary>
		/// Ask an Out of Core component to load its sub-scene
		/// </summary>
		/// <param name="ooc">An OoC component</param>
		public static void LoadOoC(System.UInt32 ooc) {
			Scene_loadOoC(ooc);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_unloadOoC(System.UInt32 ooc);
		/// <summary>
		/// Ask an Out of Core component to unload its sub-scene
		/// </summary>
		/// <param name="ooc">An OoC component</param>
		public static void UnloadOoC(System.UInt32 ooc) {
			Scene_unloadOoC(ooc);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region alternative trees

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_createAlternativeTree(string name, System.UInt32 root);
		/// <summary>
		/// Create a new alternative tree
		/// </summary>
		/// <param name="name">The name of the new alternative tree</param>
		/// <param name="root">The root occurrence</param>
		public static System.UInt32 CreateAlternativeTree(System.String name, System.UInt32 root) {
			var ret = Scene_createAlternativeTree(name, root);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getAlternativeTreeRoot(System.UInt32 tree);
		/// <summary>
		/// Returns the root occurrence associated with the given AlternativeTree
		/// </summary>
		/// <param name="tree">Targeted alternative tree</param>
		public static System.UInt32 GetAlternativeTreeRoot(System.UInt32 tree) {
			var ret = Scene_getAlternativeTreeRoot(tree);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern AlternativeTreeList_c Scene_listAlternativeTrees();
		/// <summary>
		/// Returns all the available alternative trees
		/// </summary>
		public static AlternativeTreeList ListAlternativeTrees() {
			var ret = Scene_listAlternativeTrees();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_AlternativeTreeList_free(ref ret);
			return convRet;
		}

		#endregion

		#region animations

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_addKeyframe(System.UInt32 channel, System.Int64 time, System.Double value);
		/// <summary>
		/// Adds a keyframe in the curve
		/// </summary>
		/// <param name="channel">The channel one wants to add a keyframe in</param>
		/// <param name="time">The time</param>
		/// <param name="value">The value</param>
		public static System.UInt32 AddKeyframe(System.UInt32 channel, System.Int64 time, System.Double value) {
			var ret = Scene_addKeyframe(channel, time, value);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_addKeyframeFromCurrentPosition(System.UInt32 channel, System.Int64 time);
		/// <summary>
		/// Adds keyframes in a given AnimChannel based on current position
		/// </summary>
		/// <param name="channel">The channel one wants to add a keyframe in</param>
		/// <param name="time">The time</param>
		public static void AddKeyframeFromCurrentPosition(System.UInt32 channel, System.Int64 time) {
			Scene_addKeyframeFromCurrentPosition(channel, time);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Scene_animatesThisOccurrence(System.UInt32 animation, System.UInt32 occurrence);
		/// <summary>
		/// Does this Animation animates this Occurrence - or one of its parents (thus animating it indirectly) ?
		/// </summary>
		/// <param name="animation">The Animation</param>
		/// <param name="occurrence">The supposingly animated occurrence</param>
		public static System.Boolean AnimatesThisOccurrence(System.UInt32 animation, System.UInt32 occurrence) {
			var ret = Scene_animatesThisOccurrence(animation, occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Boolean)(0 != ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_bakeAnimation(System.UInt32 animation, System.UInt32 occurrence, System.UInt32 end, System.Int64 intervall);
		/// <summary>
		/// Baking soda
		/// </summary>
		/// <param name="animation">The Animation</param>
		/// <param name="occurrence">The occurrence</param>
		/// <param name="end">The parent occurrence</param>
		/// <param name="intervall">The intervall</param>
		public static void BakeAnimation(System.UInt32 animation, System.UInt32 occurrence, System.UInt32 end, System.Int64 intervall) {
			Scene_bakeAnimation(animation, occurrence, end, intervall);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_createAnimation(string name);
		/// <summary>
		/// Creates an animation
		/// </summary>
		/// <param name="name">Name of the animation</param>
		public static System.UInt32 CreateAnimation(System.String name) {
			var ret = Scene_createAnimation(name);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_createSkeletonMesh(System.UInt32 root);
		/// <summary>
		/// Create a skeleton mesh from a joint component tree
		/// </summary>
		/// <param name="root">Root joint component node</param>
		public static void CreateSkeletonMesh(System.UInt32 root) {
			Scene_createSkeletonMesh(root);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_debugAnimation();
		/// <summary>
		/// h4xx_h3re
		/// </summary>
		public static void DebugAnimation() {
			Scene_debugAnimation();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_decimateAnimChannelBySegment(System.UInt32 channel, System.Double precision);
		/// <summary>
		/// Decimates by segment a given AnimChannel
		/// </summary>
		/// <param name="channel">The channel</param>
		/// <param name="precision">The precision</param>
		public static void DecimateAnimChannelBySegment(System.UInt32 channel, System.Double precision) {
			Scene_decimateAnimChannelBySegment(channel, precision);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_deleteAnimation(System.UInt32 animation);
		/// <summary>
		/// Deletes an animation
		/// </summary>
		/// <param name="animation">The created animation</param>
		public static void DeleteAnimation(System.UInt32 animation) {
			Scene_deleteAnimation(animation);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_displayAllKeyframesFromAnimChannel(System.UInt32 channel);
		/// <summary>
		/// Displays info on the selected AnimChannel
		/// </summary>
		/// <param name="channel">The channel</param>
		public static void DisplayAllKeyframesFromAnimChannel(System.UInt32 channel) {
			Scene_displayAllKeyframesFromAnimChannel(channel);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_displayAllKeyframesFromAnimation(System.UInt32 animation);
		/// <summary>
		/// Displays info on the selected animation
		/// </summary>
		/// <param name="animation">The animation</param>
		public static void DisplayAllKeyframesFromAnimation(System.UInt32 animation) {
			Scene_displayAllKeyframesFromAnimation(animation);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_displayValueFromAnimChannelAtTime(System.UInt32 channel, System.Int64 time, Int32 defaultValue);
		/// <summary>
		/// Displays the value
		/// </summary>
		/// <param name="channel">The channel</param>
		/// <param name="time">The time</param>
		/// <param name="defaultValue">Show default instead ?</param>
		public static void DisplayValueFromAnimChannelAtTime(System.UInt32 channel, System.Int64 time, System.Boolean defaultValue) {
			Scene_displayValueFromAnimChannelAtTime(channel, time, defaultValue ? 1 : 0);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getAnimChannelIfExists(System.UInt32 animation, System.UInt32 occurrence);
		/// <summary>
		/// Returns the main AnimChannel of an Occurrence according to a given Animation
		/// </summary>
		/// <param name="animation">The Animation</param>
		/// <param name="occurrence">The Occurrence</param>
		public static System.UInt32 GetAnimChannelIfExists(System.UInt32 animation, System.UInt32 occurrence) {
			var ret = Scene_getAnimChannelIfExists(animation, occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getAnimChannelOccurrence(System.UInt32 channel);
		/// <summary>
		/// Returns the Occurrence related to a given AnimChannel
		/// </summary>
		/// <param name="channel">The channel</param>
		public static System.UInt32 GetAnimChannelOccurrence(System.UInt32 channel) {
			var ret = Scene_getAnimChannelOccurrence(channel);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getKeyframeParentAnimChannel(System.UInt32 keyframe);
		/// <summary>
		/// Returns the parent AnimChannel of a given Keyframe
		/// </summary>
		/// <param name="keyframe">The keyframe one wants the parent of</param>
		public static System.UInt32 GetKeyframeParentAnimChannel(System.UInt32 keyframe) {
			var ret = Scene_getKeyframeParentAnimChannel(keyframe);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern KeyframeList_c Scene_getKeyframes(System.UInt32 channel);
		/// <summary>
		/// Returns a list of all keyframes of a simple animChannel
		/// </summary>
		/// <param name="channel">The channel one wants to extract the keyframs from</param>
		public static KeyframeList GetKeyframes(System.UInt32 channel) {
			var ret = Scene_getKeyframes(channel);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_KeyframeList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getMainChannel(System.UInt32 channel);
		/// <summary>
		/// Returns the main AnimChannel of a given AnimChannel
		/// </summary>
		/// <param name="channel">The channel one wants the main of</param>
		public static System.UInt32 GetMainChannel(System.UInt32 channel) {
			var ret = Scene_getMainChannel(channel);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getOccurrenceJoint(System.UInt32 occurrence);
		/// <summary>
		/// Returns the Joint assigned to an occurrence if any
		/// </summary>
		/// <param name="occurrence">The occurrence</param>
		public static System.UInt32 GetOccurrenceJoint(System.UInt32 occurrence) {
			var ret = Scene_getOccurrenceJoint(occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getParentChannel(System.UInt32 channel);
		/// <summary>
		/// Returns (if exists) the parent AnimChannel of a given AnimChannel
		/// </summary>
		/// <param name="channel">The channel one wants the parent of</param>
		public static System.UInt32 GetParentChannel(System.UInt32 channel) {
			var ret = Scene_getParentChannel(channel);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getSubChannel(System.UInt32 channel, string name);
		/// <summary>
		/// Returns the subchannel of a given name from an AnimChannel
		/// </summary>
		/// <param name="channel">The channel one wants the subchannel of</param>
		/// <param name="name">The name of the subchannel</param>
		public static System.UInt32 GetSubChannel(System.UInt32 channel, System.String name) {
			var ret = Scene_getSubChannel(channel, name);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern AnimChannelList_c Scene_getSubChannels(System.UInt32 channel);
		/// <summary>
		/// Returns all the sub channel of an AnimChannel
		/// </summary>
		/// <param name="channel">The channel one wants the subchannel of</param>
		public static AnimChannelList GetSubChannels(System.UInt32 channel) {
			var ret = Scene_getSubChannels(channel);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_AnimChannelList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_linkPropertyToAnimation(System.UInt32 animation, System.UInt32 entity, string propertyName);
		/// <summary>
		/// Creates a Binder in an Animation stack to animate an entity's property
		/// </summary>
		/// <param name="animation">The Animation stack where to put a animated property</param>
		/// <param name="entity">The entity object to animate</param>
		/// <param name="propertyName">The name of the property to animate</param>
		public static System.UInt32 LinkPropertyToAnimation(System.UInt32 animation, System.UInt32 entity, System.String propertyName) {
			var ret = Scene_linkPropertyToAnimation(animation, entity, propertyName);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern AnimationList_c Scene_listAnimations();
		/// <summary>
		/// List all Animations from the scene
		/// </summary>
		public static AnimationList ListAnimations() {
			var ret = Scene_listAnimations();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_AnimationList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern AnimChannelList_c Scene_listMainChannels(System.UInt32 animation);
		/// <summary>
		/// List all main AnimChannel from a given Animation
		/// </summary>
		/// <param name="animation">The Animation one wants to list the channels from</param>
		public static AnimChannelList ListMainChannels(System.UInt32 animation) {
			var ret = Scene_listMainChannels(animation);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_AnimChannelList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_makeDefaultKeyframe(System.UInt32 channel);
		/// <summary>
		/// Creates keyframes with the default values of the channel at time 0
		/// </summary>
		/// <param name="channel">The channel</param>
		public static void MakeDefaultKeyframe(System.UInt32 channel) {
			Scene_makeDefaultKeyframe(channel);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_moveAnimation(System.UInt32 animation, System.UInt32 target, System.UInt32 newParent, System.Int64 intervall);
		/// <summary>
		/// Moving animation
		/// </summary>
		/// <param name="animation">The Animation</param>
		/// <param name="target">The target occurrence</param>
		/// <param name="newParent">The new parent occurrence</param>
		/// <param name="intervall">The intervall</param>
		public static void MoveAnimation(System.UInt32 animation, System.UInt32 target, System.UInt32 newParent, System.Int64 intervall) {
			Scene_moveAnimation(animation, target, newParent, intervall);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_removeKeyframe(System.UInt32 channel, System.Int64 time);
		/// <summary>
		/// Removes a keyframe in the curve
		/// </summary>
		/// <param name="channel">The channel one wants to remove a keyframe from</param>
		/// <param name="time">The time</param>
		public static void RemoveKeyframe(System.UInt32 channel, System.Int64 time) {
			Scene_removeKeyframe(channel, time);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_unlinkPropertyToAnimation(System.UInt32 animation, System.UInt32 entity, string propertyName);
		/// <summary>
		/// Unlinks a binder
		/// </summary>
		/// <param name="animation">The Animation stack where to put a animated property</param>
		/// <param name="entity">The entity object to animate</param>
		/// <param name="propertyName">The name of the property to animate</param>
		public static void UnlinkPropertyToAnimation(System.UInt32 animation, System.UInt32 entity, System.String propertyName) {
			Scene_unlinkPropertyToAnimation(animation, entity, propertyName);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region debug

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getBRepInfosReturn_c Scene_getBRepInfos();
		/// <summary>
		/// 
		/// </summary>
		public static Scene.Native.getBRepInfosReturn GetBRepInfos() {
			var ret = Scene_getBRepInfos();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			Scene.Native.getBRepInfosReturn retStruct = new Scene.Native.getBRepInfosReturn();
			retStruct.partCount = (System.Int32)ret.partCount;
			retStruct.totalPartCount = (System.Int32)ret.totalPartCount;
			retStruct.vertexCount = (System.Int32)ret.vertexCount;
			retStruct.totalVertexCount = (System.Int32)ret.totalVertexCount;
			retStruct.edgeCount = (System.Int32)ret.edgeCount;
			retStruct.totalEdgeCount = (System.Int32)ret.totalEdgeCount;
			retStruct.domainCount = (System.Int32)ret.domainCount;
			retStruct.totalDomainCount = (System.Int32)ret.totalDomainCount;
			retStruct.bodyCount = (System.Int32)ret.bodyCount;
			retStruct.totalBodyCount = (System.Int32)ret.totalBodyCount;
			retStruct.area2Dsum = (System.Double)ret.area2Dsum;
			retStruct.boundaryCount = (System.Int32)ret.boundaryCount;
			retStruct.boundaryEdgeCount = (System.Int32)ret.boundaryEdgeCount;
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getTessellationInfosReturn_c Scene_getTessellationInfos();
		/// <summary>
		/// 
		/// </summary>
		public static Scene.Native.getTessellationInfosReturn GetTessellationInfos() {
			var ret = Scene_getTessellationInfos();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			Scene.Native.getTessellationInfosReturn retStruct = new Scene.Native.getTessellationInfosReturn();
			retStruct.partCount = (System.Int32)ret.partCount;
			retStruct.totalPartCount = (System.Int32)ret.totalPartCount;
			retStruct.vertexCount = (System.Int32)ret.vertexCount;
			retStruct.totalVertexCount = (System.Int32)ret.totalVertexCount;
			retStruct.edgeCount = (System.Int32)ret.edgeCount;
			retStruct.totalEdgeCount = (System.Int32)ret.totalEdgeCount;
			retStruct.polygonCount = (System.Int32)ret.polygonCount;
			retStruct.totalPolygonCount = (System.Int32)ret.totalPolygonCount;
			retStruct.patchCount = (System.Int32)ret.patchCount;
			retStruct.totalPatchCount = (System.Int32)ret.totalPatchCount;
			retStruct.boundaryCount = (System.Int32)ret.boundaryCount;
			retStruct.boundaryEdgeCount = (System.Int32)ret.boundaryEdgeCount;
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_print(System.UInt32 root);
		/// <summary>
		/// Print an occurrence tree on log
		/// </summary>
		/// <param name="root">Occurrence tree root</param>
		public static void Print(System.UInt32 root) {
			Scene_print(root);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region events

		private delegate void onRayProbeDelegate(System.IntPtr userdata, System.UInt32 proberID, ProberInfo_c proberInfo);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_addonRayProbeCallback(onRayProbeDelegate callback, System.IntPtr userdata);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_removeonRayProbeCallback(System.UInt32 id);

		private static System.UInt32 addonRayProbeCallback(onRayProbeDelegate callback, System.IntPtr userdata) {
			return Scene_addonRayProbeCallback(callback, userdata);
		}

		private static void removeonRayProbeCallback(System.UInt32 id) {
			Scene_removeonRayProbeCallback(id);
		}

		public class onRayProbeTask : PiXYZAPI.PixyzCallbackTask
		{
			public struct Result
			{
				public System.UInt32 proberID;
				public ProberInfo proberInfo;

				public Result(System.UInt32 proberID, ProberInfo proberInfo)
				{
					this.proberID = proberID;
					this.proberInfo = proberInfo;
				}
			}

			public class Execution : BaseTaskExecution<Result>
			{
				public Execution(CancellationTokenSource cancelTokenSource = null) : base(cancelTokenSource)
				{
				}

				protected override void AttachCallback()
				{
					onRayProbeDelegate callback = Updated;

					_delegatePtr = GCHandle.Alloc(callback, GCHandleType.Pinned);
					_callbackId = addonRayProbeCallback(callback, IntPtr.Zero);

				}

				protected override void DetachCallback()
				{
					if (_callbackId == 0)
						return;

					removeonRayProbeCallback(_callbackId);
					_callbackId = 0;
				}

				private void Updated(System.IntPtr userdata, System.UInt32 proberID, ProberInfo_c proberInfo)
				{
					try
					{
						_results.Enqueue(new Result(proberID, ConvertValue(ref proberInfo)));

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
			public static Task<Result> WaitonRayProbe(CancellationTokenSource cancelSource = null)
			{
				return Task.Run(new Execution(cancelSource).RunOnce);
			}
		}
		private delegate void onSphereProbeDelegate(System.IntPtr userdata, System.UInt32 proberID, ProberInfo_c proberInfo);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_addonSphereProbeCallback(onSphereProbeDelegate callback, System.IntPtr userdata);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_removeonSphereProbeCallback(System.UInt32 id);

		private static System.UInt32 addonSphereProbeCallback(onSphereProbeDelegate callback, System.IntPtr userdata) {
			return Scene_addonSphereProbeCallback(callback, userdata);
		}

		private static void removeonSphereProbeCallback(System.UInt32 id) {
			Scene_removeonSphereProbeCallback(id);
		}

		public class onSphereProbeTask : PiXYZAPI.PixyzCallbackTask
		{
			public struct Result
			{
				public System.UInt32 proberID;
				public ProberInfo proberInfo;

				public Result(System.UInt32 proberID, ProberInfo proberInfo)
				{
					this.proberID = proberID;
					this.proberInfo = proberInfo;
				}
			}

			public class Execution : BaseTaskExecution<Result>
			{
				public Execution(CancellationTokenSource cancelTokenSource = null) : base(cancelTokenSource)
				{
				}

				protected override void AttachCallback()
				{
					onSphereProbeDelegate callback = Updated;

					_delegatePtr = GCHandle.Alloc(callback, GCHandleType.Pinned);
					_callbackId = addonSphereProbeCallback(callback, IntPtr.Zero);

				}

				protected override void DetachCallback()
				{
					if (_callbackId == 0)
						return;

					removeonSphereProbeCallback(_callbackId);
					_callbackId = 0;
				}

				private void Updated(System.IntPtr userdata, System.UInt32 proberID, ProberInfo_c proberInfo)
				{
					try
					{
						_results.Enqueue(new Result(proberID, ConvertValue(ref proberInfo)));

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
			public static Task<Result> WaitonSphereProbe(CancellationTokenSource cancelSource = null)
			{
				return Task.Run(new Execution(cancelSource).RunOnce);
			}
		}
		#endregion

		#region filters

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_addFilterToLibrary(string name, string expr);
		/// <summary>
		/// Add a filter to the filters library
		/// </summary>
		/// <param name="name">Name of the filter</param>
		/// <param name="expr">The filter expression</param>
		public static System.UInt32 AddFilterToLibrary(System.String name, System.String expr) {
			var ret = Scene_addFilterToLibrary(name, expr);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Scene_evaluateExpression(string filter);
		/// <summary>
		/// Evaluate the given filter expression
		/// </summary>
		/// <param name="filter">The filter expression</param>
		public static System.String EvaluateExpression(System.String filter) {
			var ret = Scene_evaluateExpression(filter);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Core.Native.StringList_c Scene_evaluateExpressionOnOccurrences(OccurrenceList_c occurrences, string filter);
		/// <summary>
		/// evaluate the given filter expression on all occurrences under the given occurrence and returns the result
		/// </summary>
		/// <param name="occurrences">Occurrences on which to evaluate the expression</param>
		/// <param name="filter">The filter expression</param>
		public static Core.Native.StringList EvaluateExpressionOnOccurrences(OccurrenceList occurrences, System.String filter) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			var ret = Scene_evaluateExpressionOnOccurrences(occurrences_c, filter);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Core.Native.NativeInterface.ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_StringList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern evaluateExpressionOnSubTreeReturn_c Scene_evaluateExpressionOnSubTree(string filter, System.UInt32 from);
		/// <summary>
		/// evaluate the given filter expression on all occurrences under the given occurrence and returns the result
		/// </summary>
		/// <param name="filter">The filter expression</param>
		/// <param name="from">Source occurrence of the recursion</param>
		public static Scene.Native.evaluateExpressionOnSubTreeReturn EvaluateExpressionOnSubTree(System.String filter, System.UInt32 from) {
			var ret = Scene_evaluateExpressionOnSubTree(filter, from);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			Scene.Native.evaluateExpressionOnSubTreeReturn retStruct = new Scene.Native.evaluateExpressionOnSubTreeReturn();
			retStruct.occurrences = ConvertValue(ref ret.occurrences);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret.occurrences);
			retStruct.evaluations = ConvertValue(ref ret.evaluations);
			Core.Native.NativeInterface.Core_StringList_free(ref ret.evaluations);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_exportFilterLibrary(string file);
		/// <summary>
		/// Export filters from a given file
		/// </summary>
		/// <param name="file">File path to export</param>
		public static void ExportFilterLibrary(System.String file) {
			Scene_exportFilterLibrary(file);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Filter_c Scene_findFilterByName(string name);
		/// <summary>
		/// Returns the first filter in the filter library with the given name
		/// </summary>
		/// <param name="name">Name of the filter to retrieve (case sensitive)</param>
		public static Filter FindFilterByName(System.String name) {
			var ret = Scene_findFilterByName(name);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_Filter_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Scene_getFilterExpression(System.UInt32 filterId);
		/// <summary>
		/// Returns the filter expression (string) from a filter id stored in the library
		/// </summary>
		/// <param name="filterId">Identifier of the filter to fetch</param>
		public static System.String GetFilterExpression(System.UInt32 filterId) {
			var ret = Scene_getFilterExpression(filterId);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Filter_c Scene_getFilterFromLibrary(System.UInt32 filterId);
		/// <summary>
		/// Retrieve a filter from the library with its identifier
		/// </summary>
		/// <param name="filterId">Identifier of the filter to retrieve</param>
		public static Filter GetFilterFromLibrary(System.UInt32 filterId) {
			var ret = Scene_getFilterFromLibrary(filterId);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_Filter_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern OccurrenceList_c Scene_getFilteredOccurrences(string filter, System.UInt32 from);
		/// <summary>
		/// Recursively get all the occurrences validating the given filter expression
		/// </summary>
		/// <param name="filter">The filter expression</param>
		/// <param name="from">Source occurrence of the recursion</param>
		public static OccurrenceList GetFilteredOccurrences(System.String filter, System.UInt32 from) {
			var ret = Scene_getFilteredOccurrences(filter, from);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_importFilterLibrary(string file);
		/// <summary>
		/// Import filters from a given file
		/// </summary>
		/// <param name="file">File containing the filter library</param>
		public static void ImportFilterLibrary(System.String file) {
			Scene_importFilterLibrary(file);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern FilterList_c Scene_listFilterLibrary();
		/// <summary>
		/// Returns all the filter stored in the filter library
		/// </summary>
		public static FilterList ListFilterLibrary() {
			var ret = Scene_listFilterLibrary();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_FilterList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_removeFilterFromLibrary(System.UInt32 filterId);
		/// <summary>
		/// Remove a filter from the filters library
		/// </summary>
		/// <param name="filterId">Identifier of the filter to remove</param>
		public static void RemoveFilterFromLibrary(System.UInt32 filterId) {
			Scene_removeFilterFromLibrary(filterId);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Scene_rewriteFilterExpression(string expr);
		/// <summary>
		/// Parse the given expression, and regenerate it from the internal representation structure
		/// </summary>
		/// <param name="expr">Input filter expression</param>
		public static System.String RewriteFilterExpression(System.String expr) {
			var ret = Scene_rewriteFilterExpression(expr);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		#endregion

		#region isolate

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_isolate(OccurrenceList_c occurrences);
		/// <summary>
		/// Enter isolate mode by isolating a subset of the scene for process, export, viewer, ...
		/// </summary>
		/// <param name="occurrences">Occurrences to isolate</param>
		public static void Isolate(OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			Scene_isolate(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_unisolate();
		/// <summary>
		/// Exit the isolate mode
		/// </summary>
		public static void Unisolate() {
			Scene_unisolate();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region merging

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern OccurrenceList_c Scene_mergeByRegions(OccurrenceList_c roots, MergeByRegionsStrategy_c mergeBy, Int32 strategy);
		/// <summary>
		/// Merge all parts within the same area.
		/// </summary>
		/// <param name="roots">Roots occurrences for the process (will not be removed)</param>
		/// <param name="mergeBy">Number: number of output parts (or regions of parts)\nSize: diagonal size of output regions</param>
		/// <param name="strategy">Choose the regions merging strategy</param>
		public static OccurrenceList MergeByRegions(OccurrenceList roots, MergeByRegionsStrategy mergeBy, MergeStrategy strategy) {
			var roots_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(roots, ref roots_c);
			var mergeBy_c = new Scene.Native.MergeByRegionsStrategy_c();
			ConvertValue(mergeBy, ref mergeBy_c);
			var ret = Scene_mergeByRegions(roots_c, mergeBy_c, (int)strategy);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref roots_c);
			Scene.Native.NativeInterface.Scene_MergeByRegionsStrategy_free(ref mergeBy_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_mergeByTreeLevel(OccurrenceList_c partOccurrences, Int32 maxLevel, Int32 mergeHiddenPartsMode);
		/// <summary>
		/// Merge all parts over maxLevel level
		/// </summary>
		/// <param name="partOccurrences">Occurrence of the parts to merge</param>
		/// <param name="maxLevel">Maximum tree level</param>
		/// <param name="mergeHiddenPartsMode">Hidden parts handling mode, Destroy them, make visible or merge separately</param>
		public static void MergeByTreeLevel(OccurrenceList partOccurrences, System.Int32 maxLevel, MergeHiddenPartsMode mergeHiddenPartsMode) {
			var partOccurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(partOccurrences, ref partOccurrences_c);
			Scene_mergeByTreeLevel(partOccurrences_c, maxLevel, (int)mergeHiddenPartsMode);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref partOccurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_mergeFinalLevel(OccurrenceList_c roots, Int32 mergeHiddenPartsMode, Int32 CollapseToParent);
		/// <summary>
		/// Merge final level (occurrences with only occurrence with part component as children)
		/// </summary>
		/// <param name="roots">Roots occurrences for the process (will not be removed)</param>
		/// <param name="mergeHiddenPartsMode">Hidden parts handling mode, Destroy them, make visible or merge separately</param>
		/// <param name="CollapseToParent">If true, final level unique merged part will replace it's parent</param>
		public static void MergeFinalLevel(OccurrenceList roots, MergeHiddenPartsMode mergeHiddenPartsMode, System.Boolean CollapseToParent) {
			var roots_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(roots, ref roots_c);
			Scene_mergeFinalLevel(roots_c, (int)mergeHiddenPartsMode, CollapseToParent ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref roots_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern OccurrenceList_c Scene_mergeParts(OccurrenceList_c partOccurrences, Int32 mergeHiddenPartsMode);
		/// <summary>
		/// Merge a set of parts
		/// </summary>
		/// <param name="partOccurrences">Occurrence of the parts to merge</param>
		/// <param name="mergeHiddenPartsMode">Hidden parts handling mode, Destroy them, make visible or merge separately</param>
		public static OccurrenceList MergeParts(OccurrenceList partOccurrences, MergeHiddenPartsMode mergeHiddenPartsMode) {
			var partOccurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(partOccurrences, ref partOccurrences_c);
			var ret = Scene_mergeParts(partOccurrences_c, (int)mergeHiddenPartsMode);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref partOccurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_mergePartsByAssemblies(OccurrenceList_c roots, Int32 mergeHiddenPartsMode);
		/// <summary>
		/// Merge all parts under each assembly together
		/// </summary>
		/// <param name="roots">Roots occurrences for the process (will not be removed)</param>
		/// <param name="mergeHiddenPartsMode">Hidden parts handling mode, Destroy them, make visible or merge separately</param>
		public static void MergePartsByAssemblies(OccurrenceList roots, MergeHiddenPartsMode mergeHiddenPartsMode) {
			var roots_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(roots, ref roots_c);
			Scene_mergePartsByAssemblies(roots_c, (int)mergeHiddenPartsMode);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref roots_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern OccurrenceList_c Scene_mergePartsByMaterials(OccurrenceList_c partOccurrences, Int32 mergeNoMaterials, Int32 mergeHiddenPartsMode, Int32 combineMeshes);
		/// <summary>
		/// Merge a set of parts by materials
		/// </summary>
		/// <param name="partOccurrences">Occurrence of the parts to merge</param>
		/// <param name="mergeNoMaterials">If true, merge all parts with no active material together, else do not merge them</param>
		/// <param name="mergeHiddenPartsMode">Hidden parts handling mode, Destroy them, make visible or merge separately</param>
		/// <param name="combineMeshes">If true, explode and remerge the input parts by visible materials</param>
		public static OccurrenceList MergePartsByMaterials(OccurrenceList partOccurrences, System.Boolean mergeNoMaterials, MergeHiddenPartsMode mergeHiddenPartsMode, System.Boolean combineMeshes) {
			var partOccurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(partOccurrences, ref partOccurrences_c);
			var ret = Scene_mergePartsByMaterials(partOccurrences_c, mergeNoMaterials ? 1 : 0, (int)mergeHiddenPartsMode, combineMeshes ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref partOccurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_mergePartsByName(System.UInt32 root, Int32 mergeHiddenPartsMode);
		/// <summary>
		/// Merge all parts by occurences names
		/// </summary>
		/// <param name="root">Root occurrence of the subtree to process</param>
		/// <param name="mergeHiddenPartsMode">Hidden parts handling mode, Destroy them, make visible or merge separately</param>
		public static void MergePartsByName(System.UInt32 root, MergeHiddenPartsMode mergeHiddenPartsMode) {
			Scene_mergePartsByName(root, (int)mergeHiddenPartsMode);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_transferCADMaterialsOnPartOccurrences(System.UInt32 rootOccurrence);
		/// <summary>
		/// Set all materials on part occurrences
		/// </summary>
		/// <param name="rootOccurrence">Root occurrence</param>
		public static void TransferCADMaterialsOnPartOccurrences(System.UInt32 rootOccurrence) {
			Scene_transferCADMaterialsOnPartOccurrences(rootOccurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_transferMaterialsOnPatches(System.UInt32 rootOccurrence);
		/// <summary>
		/// Take the first instance material and set it one the mesh patches
		/// </summary>
		/// <param name="rootOccurrence">Root occurrence</param>
		public static void TransferMaterialsOnPatches(System.UInt32 rootOccurrence) {
			Scene_transferMaterialsOnPatches(rootOccurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region modification

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_applyTransformation(System.UInt32 occurrence, Geom.Native.Matrix4_c matrix);
		/// <summary>
		/// apply a transformation to the local matrix of an occurrence
		/// </summary>
		/// <param name="occurrence">Occurrence to apply the matrix on</param>
		/// <param name="matrix">Transformation to matrix</param>
		public static void ApplyTransformation(System.UInt32 occurrence, Geom.Native.Matrix4 matrix) {
			var matrix_c = new Geom.Native.Matrix4_c();
			Geom.Native.NativeInterface.ConvertValue(matrix, ref matrix_c);
			Scene_applyTransformation(occurrence, matrix_c);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref matrix_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_createSymmetry(OccurrenceList_c occurrences, Int32 plane);
		/// <summary>
		/// Create symmetries from selection
		/// </summary>
		/// <param name="occurrences">Selection of occurrences</param>
		/// <param name="plane">Symmetry plane</param>
		public static void CreateSymmetry(OccurrenceList occurrences, Geom.Native.AxisPlane plane) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			Scene_createSymmetry(occurrences_c, (int)plane);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_rotate(System.UInt32 occurrence, Geom.Native.Point3_c axis, System.Double angle);
		/// <summary>
		/// Modify the local matrix of the scene node to apply a rotation
		/// </summary>
		/// <param name="occurrence">Occurrence to rotate</param>
		/// <param name="axis">Axis of rotation</param>
		/// <param name="angle">Angle of rotation</param>
		public static void Rotate(System.UInt32 occurrence, Geom.Native.Point3 axis, System.Double angle) {
			var axis_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(axis, ref axis_c);
			Scene_rotate(occurrence, axis_c, angle);
			Geom.Native.NativeInterface.Geom_Point3_free(ref axis_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_setLocalMatrix(System.UInt32 occurrence, Geom.Native.Matrix4_c matrix);
		/// <summary>
		/// change the local matrix on an occurrence
		/// </summary>
		/// <param name="occurrence">Occurrence to set the local matrix</param>
		/// <param name="matrix">The new occurrence local matrix</param>
		public static void SetLocalMatrix(System.UInt32 occurrence, Geom.Native.Matrix4 matrix) {
			var matrix_c = new Geom.Native.Matrix4_c();
			Geom.Native.NativeInterface.ConvertValue(matrix, ref matrix_c);
			Scene_setLocalMatrix(occurrence, matrix_c);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref matrix_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region part

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getPartMesh(System.UInt32 part);
		/// <summary>
		/// Return the mesh of the TesselatedShape
		/// </summary>
		/// <param name="part">The part component</param>
		public static System.UInt32 GetPartMesh(System.UInt32 part) {
			var ret = Scene_getPartMesh(part);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getPartModel(System.UInt32 part);
		/// <summary>
		/// Return the model of the BRepShape
		/// </summary>
		/// <param name="part">The part component</param>
		public static System.UInt32 GetPartModel(System.UInt32 part) {
			var ret = Scene_getPartModel(part);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Polygonal.Native.MeshList_c Scene_getPartsMeshes(PartList_c parts);
		/// <summary>
		/// Return the meshes of the TesselatedShape for each given parts if any
		/// </summary>
		/// <param name="parts">The list of part component</param>
		public static Polygonal.Native.MeshList GetPartsMeshes(PartList parts) {
			var parts_c = new Scene.Native.PartList_c();
			ConvertValue(parts, ref parts_c);
			var ret = Scene_getPartsMeshes(parts_c);
			Scene.Native.NativeInterface.Scene_PartList_free(ref parts_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Polygonal.Native.NativeInterface.ConvertValue(ref ret);
			Polygonal.Native.NativeInterface.Polygonal_MeshList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern CAD.Native.ModelList_c Scene_getPartsModels(PartList_c parts);
		/// <summary>
		/// Return the models of the BRepShape for each given parts if any
		/// </summary>
		/// <param name="parts">The list of part component</param>
		public static CAD.Native.ModelList GetPartsModels(PartList parts) {
			var parts_c = new Scene.Native.PartList_c();
			ConvertValue(parts, ref parts_c);
			var ret = Scene_getPartsModels(parts_c);
			Scene.Native.NativeInterface.Scene_PartList_free(ref parts_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = CAD.Native.NativeInterface.ConvertValue(ref ret);
			CAD.Native.NativeInterface.CAD_ModelList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Geom.Native.Matrix4List_c Scene_getPartsTransforms(PartList_c parts);
		/// <summary>
		/// Returns the transform matrix of each given parts
		/// </summary>
		/// <param name="parts">The parts to retrieve transform</param>
		public static Geom.Native.Matrix4List GetPartsTransforms(PartList parts) {
			var parts_c = new Scene.Native.PartList_c();
			ConvertValue(parts, ref parts_c);
			var ret = Scene_getPartsTransforms(parts_c);
			Scene.Native.NativeInterface.Scene_PartList_free(ref parts_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Geom.Native.NativeInterface.ConvertValue(ref ret);
			Geom.Native.NativeInterface.Geom_Matrix4List_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getPartsTransformsIndexedReturn_c Scene_getPartsTransformsIndexed(PartList_c parts);
		/// <summary>
		/// Returns the transform matrix of each given parts (indexed mode)
		/// </summary>
		/// <param name="parts">The parts to retrieve transform</param>
		public static Scene.Native.getPartsTransformsIndexedReturn GetPartsTransformsIndexed(PartList parts) {
			var parts_c = new Scene.Native.PartList_c();
			ConvertValue(parts, ref parts_c);
			var ret = Scene_getPartsTransformsIndexed(parts_c);
			Scene.Native.NativeInterface.Scene_PartList_free(ref parts_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			Scene.Native.getPartsTransformsIndexedReturn retStruct = new Scene.Native.getPartsTransformsIndexedReturn();
			retStruct.indices = ConvertValue(ref ret.indices);
			Core.Native.NativeInterface.Core_IntList_free(ref ret.indices);
			retStruct.transforms = ConvertValue(ref ret.transforms);
			Geom.Native.NativeInterface.Geom_Matrix4List_free(ref ret.transforms);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_setPartMesh(System.UInt32 part, System.UInt32 mesh);
		/// <summary>
		/// Add a mesh to a part (create a TessellatedShape on the part)
		/// </summary>
		/// <param name="part">The part component</param>
		/// <param name="mesh">The mesh to add to the part</param>
		public static void SetPartMesh(System.UInt32 part, System.UInt32 mesh) {
			Scene_setPartMesh(part, mesh);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_setPartModel(System.UInt32 part, System.UInt32 model);
		/// <summary>
		/// Add a model to a part (create a BRepShape on the part)
		/// </summary>
		/// <param name="part">The part component</param>
		/// <param name="model">The model to add to the part</param>
		public static void SetPartModel(System.UInt32 part, System.UInt32 model) {
			Scene_setPartModel(part, model);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_setPartsTransforms(PartList_c parts, Geom.Native.Matrix4List_c transforms);
		/// <summary>
		/// Set the transform matrix of each given parts
		/// </summary>
		/// <param name="parts">The parts to retrieve transform</param>
		/// <param name="transforms">The transform matrix of each part</param>
		public static void SetPartsTransforms(PartList parts, Geom.Native.Matrix4List transforms) {
			var parts_c = new Scene.Native.PartList_c();
			ConvertValue(parts, ref parts_c);
			var transforms_c = new Geom.Native.Matrix4List_c();
			Geom.Native.NativeInterface.ConvertValue(transforms, ref transforms_c);
			Scene_setPartsTransforms(parts_c, transforms_c);
			Scene.Native.NativeInterface.Scene_PartList_free(ref parts_c);
			Geom.Native.NativeInterface.Geom_Matrix4List_free(ref transforms_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_setPartsTransformsIndexed(PartList_c parts, Core.Native.IntList_c indices, Geom.Native.Matrix4List_c transforms);
		/// <summary>
		/// Set the transform matrix of each given parts (indexed mode)
		/// </summary>
		/// <param name="parts">The parts to retrieve transform</param>
		/// <param name="indices">The transform matrix index for each parts</param>
		/// <param name="transforms">The list of transform matrices</param>
		public static void SetPartsTransformsIndexed(PartList parts, Core.Native.IntList indices, Geom.Native.Matrix4List transforms) {
			var parts_c = new Scene.Native.PartList_c();
			ConvertValue(parts, ref parts_c);
			var indices_c = new Core.Native.IntList_c();
			Core.Native.NativeInterface.ConvertValue(indices, ref indices_c);
			var transforms_c = new Geom.Native.Matrix4List_c();
			Geom.Native.NativeInterface.ConvertValue(transforms, ref transforms_c);
			Scene_setPartsTransformsIndexed(parts_c, indices_c, transforms_c);
			Scene.Native.NativeInterface.Scene_PartList_free(ref parts_c);
			Core.Native.NativeInterface.Core_IntList_free(ref indices_c);
			Geom.Native.NativeInterface.Geom_Matrix4List_free(ref transforms_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region pivots

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_alignPivotPointToWorld(OccurrenceList_c occurrences, Int32 applyToChildren);
		/// <summary>
		/// Re-orient the Pivot Point straight to world origin (the grid)
		/// </summary>
		/// <param name="occurrences">The occurrences to modify</param>
		/// <param name="applyToChildren">If True, all the pivot of the descending occurrences from occurrence will be affected</param>
		public static void AlignPivotPointToWorld(OccurrenceList occurrences, System.Boolean applyToChildren) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			Scene_alignPivotPointToWorld(occurrences_c, applyToChildren ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_movePivotPointToOccurrenceCenter(OccurrenceList_c occurrences, Int32 applyToChildren);
		/// <summary>
		/// Move the pivot point of each occurrence listed in the function input, to the center of its bounding box (and of its children if the parameter is True)
		/// </summary>
		/// <param name="occurrences">Occurrences (or the roots occurrences if recursively=True)</param>
		/// <param name="applyToChildren">If True, all the pivot of the descending occurrences from occurrence will be affected</param>
		public static void MovePivotPointToOccurrenceCenter(OccurrenceList occurrences, System.Boolean applyToChildren) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			Scene_movePivotPointToOccurrenceCenter(occurrences_c, applyToChildren ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_movePivotPointToOrigin(System.UInt32 occurrence, Int32 applyToChildren);
		/// <summary>
		/// Move the pivot point of an occurrence (and its descendants if recursively) to the origin (0,0,0)
		/// </summary>
		/// <param name="occurrence">The occurrence (or the root occurrence if recursively=True)</param>
		/// <param name="applyToChildren">If True, all the pivot of the descending occurrences from occurrence will be affected</param>
		public static void MovePivotPointToOrigin(System.UInt32 occurrence, System.Boolean applyToChildren) {
			Scene_movePivotPointToOrigin(occurrence, applyToChildren ? 1 : 0);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_movePivotPointToSelectionCenter(OccurrenceList_c occurrences);
		/// <summary>
		/// Move the pivot point of all given occurrences to the center of all occurrences
		/// </summary>
		/// <param name="occurrences">The occurrences to modify</param>
		public static void MovePivotPointToSelectionCenter(OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			Scene_movePivotPointToSelectionCenter(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_movePivotPointToTargetedOccurrenceCenter(OccurrenceList_c occurrences, System.UInt32 target, Int32 applyToChildren);
		/// <summary>
		/// Move the pivot point of each occurrence listed in the function input, to the center of the targeted occurrence Center (and of its children if the parameter is True)
		/// </summary>
		/// <param name="occurrences">The occurrence (or the root occurrence if recursively=True)</param>
		/// <param name="target">The target occurrence</param>
		/// <param name="applyToChildren">If True, all the pivot of the descending occurrences from occurrence will be affected</param>
		public static void MovePivotPointToTargetedOccurrenceCenter(OccurrenceList occurrences, System.UInt32 target, System.Boolean applyToChildren) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			Scene_movePivotPointToTargetedOccurrenceCenter(occurrences_c, target, applyToChildren ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_setPivotOnly(System.UInt32 occurrence, Geom.Native.Matrix4_c pivot);
		/// <summary>
		/// Set the pivot of an occurrence to the given transformation matrix, the geometry will not be moved (warning: do not confuse with property Transform which actually move the occurrence)
		/// </summary>
		/// <param name="occurrence">The occurrence to move the pivot</param>
		/// <param name="pivot">The new transformation matrix for the occurrence (pivot)</param>
		public static void SetPivotOnly(System.UInt32 occurrence, Geom.Native.Matrix4 pivot) {
			var pivot_c = new Geom.Native.Matrix4_c();
			Geom.Native.NativeInterface.ConvertValue(pivot, ref pivot_c);
			Scene_setPivotOnly(occurrence, pivot_c);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref pivot_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region prototype

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getPrototype(System.UInt32 occurrence);
		/// <summary>
		/// Returns the prototype of an occurrence
		/// </summary>
		/// <param name="occurrence">The occurrence</param>
		public static System.UInt32 GetPrototype(System.UInt32 occurrence) {
			var ret = Scene_getPrototype(occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_prototypeSubTree(System.UInt32 prototype);
		/// <summary>
		/// Create occurrences that prototype the given occurrence and all its subtree
		/// </summary>
		/// <param name="prototype">The root occurrence of the sub-tree to prototype</param>
		public static System.UInt32 PrototypeSubTree(System.UInt32 prototype) {
			var ret = Scene_prototypeSubTree(prototype);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_setPrototype(System.UInt32 occurrence, System.UInt32 prototype);
		/// <summary>
		/// Sets the prototype of an occurrence
		/// </summary>
		/// <param name="occurrence">The occurrence</param>
		/// <param name="prototype">The prototype</param>
		public static void SetPrototype(System.UInt32 occurrence, System.UInt32 prototype) {
			Scene_setPrototype(occurrence, prototype);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region selection

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_clearSelection();
		/// <summary>
		/// Clear the current selection
		/// </summary>
		public static void ClearSelection() {
			Scene_clearSelection();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_deleteSelection();
		/// <summary>
		/// Delete all selected occurrences, and/or sub-occurrence elements
		/// </summary>
		public static void DeleteSelection() {
			Scene_deleteSelection();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_explodeSelection();
		/// <summary>
		/// For each occurrence, create a new occurrence with the selected sub-occurrence elements and remove them from the original occurrence
		/// </summary>
		public static void ExplodeSelection() {
			Scene_explodeSelection();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern OccurrenceList_c Scene_getSelectedOccurrences();
		/// <summary>
		/// Returns all the selected occurrences
		/// </summary>
		public static OccurrenceList GetSelectedOccurrences() {
			var ret = Scene_getSelectedOccurrences();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_invertOrientationSelection();
		/// <summary>
		/// Invert the orientation of each selected item (occurrences and/or sub-occurrence elements
		/// </summary>
		public static void InvertOrientationSelection() {
			Scene_invertOrientationSelection();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_invertSelection();
		/// <summary>
		/// Replace the selection by all unselected part occurrences
		/// </summary>
		public static void InvertSelection() {
			Scene_invertSelection();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_removeMaterials();
		/// <summary>
		/// Remove all materials appplied to the selection
		/// </summary>
		public static void RemoveMaterials() {
			Scene_removeMaterials();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_select(OccurrenceList_c occurrences);
		/// <summary>
		/// Add occurrences to selection
		/// </summary>
		/// <param name="occurrences">Occurrences to add to the selection</param>
		public static void Select(OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			Scene_select(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_selectAllPartOccurrences();
		/// <summary>
		/// Select all part occurrences
		/// </summary>
		public static void SelectAllPartOccurrences() {
			Scene_selectAllPartOccurrences();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_separateSelection();
		/// <summary>
		/// Seperate all polygones form their original parts into a new one
		/// </summary>
		public static System.UInt32 SeparateSelection() {
			var ret = Scene_separateSelection();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_unselect(OccurrenceList_c occurrence);
		/// <summary>
		/// Remove occurrences to selection
		/// </summary>
		/// <param name="occurrence">Occurrences to remove from the selection</param>
		public static void Unselect(OccurrenceList occurrence) {
			var occurrence_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrence, ref occurrence_c);
			Scene_unselect(occurrence_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrence_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region simplification

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_compress(System.UInt32 occurrence);
		/// <summary>
		/// Compress a sub-tree by removing occurrence containing only one Child or empty, and by removing useless instances (see removeUselessInstances)
		/// </summary>
		/// <param name="occurrence">Root occurrence for the process</param>
		public static System.UInt32 Compress(System.UInt32 occurrence) {
			var ret = Scene_compress(occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_convertToOldSchoolVisibility(System.UInt32 root);
		/// <summary>
		/// Modify the visible properties of the sub-tree to look like old school visibility (only hidden/inherited)
		/// </summary>
		/// <param name="root">Root occurrence</param>
		public static void ConvertToOldSchoolVisibility(System.UInt32 root) {
			Scene_convertToOldSchoolVisibility(root);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern OccurrenceList_c Scene_getDuplicatedParts(System.UInt32 root, System.Double acceptVolumeRatio, System.Double acceptPolycountRatio, System.Double acceptAABBAxisRatio, System.Double acceptAABBCenterDistance);
		/// <summary>
		/// Get duplicated parts
		/// </summary>
		/// <param name="root">Root occurrence for the process</param>
		/// <param name="acceptVolumeRatio">If the ratio of volumes of two part is lower than acceptVolumeRatio, they will be considered duplicated</param>
		/// <param name="acceptPolycountRatio">If the ratio of polygon counts of two part is lower than acceptPolycountRatio, they will be considered duplicated</param>
		/// <param name="acceptAABBAxisRatio">If the ratio of AABB axis of two part is lower than acceptAABBAxisRatio, they will be considered duplicated</param>
		/// <param name="acceptAABBCenterDistance">If the ratio of AABB centers of two part is lower than acceptAABBCenterRatio, they will be considered duplicated</param>
		public static OccurrenceList GetDuplicatedParts(System.UInt32 root, System.Double acceptVolumeRatio, System.Double acceptPolycountRatio, System.Double acceptAABBAxisRatio, System.Double acceptAABBCenterDistance) {
			var ret = Scene_getDuplicatedParts(root, acceptVolumeRatio, acceptPolycountRatio, acceptAABBAxisRatio, acceptAABBCenterDistance);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_identifyInstances(Int32 minOccurrenceCount);
		/// <summary>
		/// Identify parts with more than one occurrence on the scene
		/// </summary>
		/// <param name="minOccurrenceCount">Min occurrence count</param>
		public static void IdentifyInstances(System.Int32 minOccurrenceCount) {
			Scene_identifyInstances(minOccurrenceCount);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_makeInstanceUnique(OccurrenceList_c occurrences);
		/// <summary>
		/// Singularize all instances on the sub-tree of an occurrence
		/// </summary>
		/// <param name="occurrences">Root occurrence for the process</param>
		public static void MakeInstanceUnique(OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(occurrences, ref occurrences_c);
			Scene_makeInstanceUnique(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_rake(System.UInt32 occurrence, Int32 keepInstances);
		/// <summary>
		/// Set the same parent to all descending parts (all parts will be singularized)
		/// </summary>
		/// <param name="occurrence">Root occurrence for the process</param>
		/// <param name="keepInstances">If false, the part will be singularized</param>
		public static void Rake(System.UInt32 occurrence, System.Boolean keepInstances) {
			Scene_rake(occurrence, keepInstances ? 1 : 0);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_removeSymmetryMatrices(System.UInt32 occurrence);
		/// <summary>
		/// Remove symmetry matrices (apply matrices on geometries on nodes under an occurrence with a symmetry matrix
		/// </summary>
		/// <param name="occurrence">Root occurrence for the process</param>
		public static void RemoveSymmetryMatrices(System.UInt32 occurrence) {
			Scene_removeSymmetryMatrices(occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_removeUselessInstances(System.UInt32 occurrence);
		/// <summary>
		/// Remove instances where they are not needed (prototype referenced once, ...)
		/// </summary>
		/// <param name="occurrence">Root occurrence for the process</param>
		public static void RemoveUselessInstances(System.UInt32 occurrence) {
			Scene_removeUselessInstances(occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_resetPartTransform(System.UInt32 root);
		/// <summary>
		/// Set all part transformation matrices to identity in a sub-tree, transformation will be applied to the shapes
		/// </summary>
		/// <param name="root">Root occurrence for the process</param>
		public static void ResetPartTransform(System.UInt32 root) {
			Scene_resetPartTransform(root);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_resetTransform(System.UInt32 root, Int32 recursive, Int32 keepInstantiation, Int32 keepPartTransform);
		/// <summary>
		/// Set all transformation matrices to identity in a sub-tree.
		/// </summary>
		/// <param name="root">Root occurrence for the process</param>
		/// <param name="recursive">If False, transformation will be applied only on the root and its components</param>
		/// <param name="keepInstantiation">If False, all occurrences will be singularized</param>
		/// <param name="keepPartTransform">If False, transformation will be applied to the shapes (BRepShape points or TessellatedShape vertices)</param>
		public static void ResetTransform(System.UInt32 root, System.Boolean recursive, System.Boolean keepInstantiation, System.Boolean keepPartTransform) {
			Scene_resetTransform(root, recursive ? 1 : 0, keepInstantiation ? 1 : 0, keepPartTransform ? 1 : 0);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_selectByMaximumSize(OccurrenceList_c roots, System.Double maxDiagLength, System.Double maxSize, Int32 selectHidden);
		/// <summary>
		/// Select all parts meeting the criteria
		/// </summary>
		/// <param name="roots">Roots occurrences for the process</param>
		/// <param name="maxDiagLength">If the diagonal axis of the bouding box is less than maxDiagLength, part will be selected. -1 to ignore</param>
		/// <param name="maxSize">If the longer axis of the box is less than maxLength, part will be selected. -1 to ignore</param>
		/// <param name="selectHidden">If true, hidden parts meeting the criteria will be selected as well</param>
		public static void SelectByMaximumSize(OccurrenceList roots, System.Double maxDiagLength, System.Double maxSize, System.Boolean selectHidden) {
			var roots_c = new Scene.Native.OccurrenceList_c();
			ConvertValue(roots, ref roots_c);
			Scene_selectByMaximumSize(roots_c, maxDiagLength, maxSize, selectHidden ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref roots_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_selectDuplicated(System.Double acceptVolumeRatio, System.Double acceptPolycountRatio, System.Double acceptAABBAxisRatio, System.Double acceptAABBCenterDistance);
		/// <summary>
		/// Select duplicated parts
		/// </summary>
		/// <param name="acceptVolumeRatio">If the ratio of volumes of two part is lower than acceptVolumeRatio, they will be considered duplicated</param>
		/// <param name="acceptPolycountRatio">If the ratio of polygon counts of two part is lower than acceptPolycountRatio, they will be considered duplicated</param>
		/// <param name="acceptAABBAxisRatio">If the ratio of AABB axis of two part is lower than acceptAABBAxisRatio, they will be considered duplicated</param>
		/// <param name="acceptAABBCenterDistance">If the ratio of AABB centers of two part is lower than acceptAABBCenterRatio, they will be considered duplicated</param>
		public static void SelectDuplicated(System.Double acceptVolumeRatio, System.Double acceptPolycountRatio, System.Double acceptAABBAxisRatio, System.Double acceptAABBCenterDistance) {
			Scene_selectDuplicated(acceptVolumeRatio, acceptPolycountRatio, acceptAABBAxisRatio, acceptAABBCenterDistance);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_selectInstances(System.UInt32 occurrence);
		/// <summary>
		/// Select occurrences sharing the same prototype as the given one
		/// </summary>
		/// <param name="occurrence">Reference part occurrence</param>
		public static void SelectInstances(System.UInt32 occurrence) {
			Scene_selectInstances(occurrence);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_selectPartsFromNoShow();
		/// <summary>
		/// Select hidden parts
		/// </summary>
		public static void SelectPartsFromNoShow() {
			Scene_selectPartsFromNoShow();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region spatialRequest

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern RayHit_c Scene_rayCast(Geom.Native.Ray_c ray, System.UInt32 root);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ray">The ray to cast</param>
		/// <param name="root">The root occurrence to cast from</param>
		public static RayHit RayCast(Geom.Native.Ray ray, System.UInt32 root) {
			var ray_c = new Geom.Native.Ray_c();
			Geom.Native.NativeInterface.ConvertValue(ray, ref ray_c);
			var ret = Scene_rayCast(ray_c, root);
			Geom.Native.NativeInterface.Geom_Ray_free(ref ray_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_RayHit_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern RayHitList_c Scene_rayCastAll(Geom.Native.Ray_c ray, System.UInt32 root);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ray">The ray to cast</param>
		/// <param name="root">The root occurrence to cast from</param>
		public static RayHitList RayCastAll(Geom.Native.Ray ray, System.UInt32 root) {
			var ray_c = new Geom.Native.Ray_c();
			Geom.Native.NativeInterface.ConvertValue(ray, ref ray_c);
			var ret = Scene_rayCastAll(ray_c, root);
			Geom.Native.NativeInterface.Geom_Ray_free(ref ray_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_RayHitList_free(ref ret);
			return convRet;
		}

		#endregion

		#region variant

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_addVariant(string name);
		/// <summary>
		/// Create a new variant
		/// </summary>
		/// <param name="name">The name of the new variant</param>
		public static System.UInt32 AddVariant(System.String name) {
			var ret = Scene_addVariant(name);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_duplicateVariant(System.UInt32 variant, string name);
		/// <summary>
		/// Create a new variant which is a copy of an existing variant
		/// </summary>
		/// <param name="variant">The variant to duplicated</param>
		/// <param name="name">Name of the new variant</param>
		public static System.UInt32 DuplicateVariant(System.UInt32 variant, System.String name) {
			var ret = Scene_duplicateVariant(variant, name);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern VariantDefinitionListList_c Scene_getVariantComponentsDefinitions(VariantComponentList_c variantComponents);
		/// <summary>
		/// Returns the definitions of multiple variant components
		/// </summary>
		/// <param name="variantComponents">The list of variant components to retrieve definitions</param>
		public static VariantDefinitionListList GetVariantComponentsDefinitions(VariantComponentList variantComponents) {
			var variantComponents_c = new Scene.Native.VariantComponentList_c();
			ConvertValue(variantComponents, ref variantComponents_c);
			var ret = Scene_getVariantComponentsDefinitions(variantComponents_c);
			Scene.Native.NativeInterface.Scene_VariantComponentList_free(ref variantComponents_c);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_VariantDefinitionListList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Scene_getVariantTree(System.UInt32 variant);
		/// <summary>
		/// Get the alternative tree used by this variant
		/// </summary>
		/// <param name="variant">The variant</param>
		public static System.UInt32 GetVariantTree(System.UInt32 variant) {
			var ret = Scene_getVariantTree(variant);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern VariantList_c Scene_listVariants();
		/// <summary>
		/// Returns all the available variants
		/// </summary>
		public static VariantList ListVariants() {
			var ret = Scene_listVariants();
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_VariantList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_removeVariant(System.UInt32 variant);
		/// <summary>
		/// Remove a variant
		/// </summary>
		/// <param name="variant">The variant to remove</param>
		public static void RemoveVariant(System.UInt32 variant) {
			Scene_removeVariant(variant);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_setCurrentVariant(System.UInt32 variant);
		/// <summary>
		/// Change the current variant used
		/// </summary>
		/// <param name="variant">The variant to enable (can be null)</param>
		public static void SetCurrentVariant(System.UInt32 variant) {
			Scene_setCurrentVariant(variant);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Scene_setVariantTree(System.UInt32 variant, System.UInt32 tree);
		/// <summary>
		/// Set the alternative tree to use for this variant
		/// </summary>
		/// <param name="variant">The variant to modify</param>
		/// <param name="tree">The alternative tree to use for this variant</param>
		public static void SetVariantTree(System.UInt32 variant, System.UInt32 tree) {
			Scene_setVariantTree(variant, tree);
			System.String err = ConvertValue(Scene_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

	}
}
