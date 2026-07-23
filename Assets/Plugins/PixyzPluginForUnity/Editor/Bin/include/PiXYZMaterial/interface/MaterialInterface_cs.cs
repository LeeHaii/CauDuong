#pragma warning disable CA2101

using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Security;
using System.Collections.Concurrent;
using PiXYZAPI = Pixyz.API.Native.NativeInterface;

namespace Pixyz.Material.Native {

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
		internal static extern void Material_Texture_init(ref Texture_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_Texture_free(ref Texture_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_ColorOrTexture_init(ref ColorOrTexture_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_ColorOrTexture_free(ref ColorOrTexture_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_ImageDefinition_init(ref ImageDefinition_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_ImageDefinition_free(ref ImageDefinition_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_ImageDefinitionList_init(ref ImageDefinitionList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_ImageDefinitionList_free(ref ImageDefinitionList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_CoeffOrTexture_init(ref CoeffOrTexture_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_CoeffOrTexture_free(ref CoeffOrTexture_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_MaterialDefinition_init(ref MaterialDefinition_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_MaterialDefinition_free(ref MaterialDefinition_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_MaterialDefinitionList_init(ref MaterialDefinitionList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_MaterialDefinitionList_free(ref MaterialDefinitionList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_ImpostorMaterialInfos_init(ref ImpostorMaterialInfos_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_ImpostorMaterialInfos_free(ref ImpostorMaterialInfos_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_ColorMaterialInfos_init(ref ColorMaterialInfos_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_ColorMaterialInfos_free(ref ColorMaterialInfos_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_ImageList_init(ref ImageList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_ImageList_free(ref ImageList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_StandardMaterialInfos_init(ref StandardMaterialInfos_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_StandardMaterialInfos_free(ref StandardMaterialInfos_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_PBRMaterialInfos_init(ref PBRMaterialInfos_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_PBRMaterialInfos_free(ref PBRMaterialInfos_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_MaterialList_init(ref MaterialList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_MaterialList_free(ref MaterialList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_UnlitTextureMaterialInfos_init(ref UnlitTextureMaterialInfos_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Material_UnlitTextureMaterialInfos_free(ref UnlitTextureMaterialInfos_c str);

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

	public static Texture ConvertValue(ref Texture_c s) {
		Texture ss = new Texture();
		ss.image = (System.UInt32)s.image;
		ss.channel = (System.Int32)s.channel;
		ss.offset = Geom.Native.NativeInterface.ConvertValue(ref s.offset);
		ss.tilling = Geom.Native.NativeInterface.ConvertValue(ref s.tilling);
		return ss;
	}

	public static Texture_c ConvertValue(Texture s, ref Texture_c ss) {
		Material.Native.NativeInterface.Material_Texture_init(ref ss);
		ss.image = (System.UInt32)s.image;
		ss.channel = (Int32)s.channel;
		Geom.Native.NativeInterface.ConvertValue(s.offset, ref ss.offset);
		Geom.Native.NativeInterface.ConvertValue(s.tilling, ref ss.tilling);
		return ss;
	}

	public static ColorOrTexture ConvertValue(ref ColorOrTexture_c s) {
		ColorOrTexture ss = new ColorOrTexture();
		ss._type = (ColorOrTexture.Type)s._type;
		switch(ss._type) {
			case ColorOrTexture.Type.UNKNOWN: break;
			case ColorOrTexture.Type.COLOR: ss.color = ConvertValue(ref s.color); break;
			case ColorOrTexture.Type.TEXTURE: ss.texture = ConvertValue(ref s.texture); break;
		}
		return ss;
	}

	public static ColorOrTexture_c ConvertValue(ColorOrTexture s, ref ColorOrTexture_c ss) {
		Material.Native.NativeInterface.Material_ColorOrTexture_init(ref ss);
		ss._type = (int)s._type;
		switch (ss._type) {
			case 0: break;
			case 1: ConvertValue(s.color, ref ss.color); break;
			case 2: ConvertValue(s.texture, ref ss.texture); break;
		}
		return ss;
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

	public static ImageDefinition ConvertValue(ref ImageDefinition_c s) {
		ImageDefinition ss = new ImageDefinition();
		ss.id = (System.UInt32)s.id;
		ss.name = ConvertValue(s.name);
		ss.width = (System.Int32)s.width;
		ss.height = (System.Int32)s.height;
		ss.bitsPerComponent = (System.Int32)s.bitsPerComponent;
		ss.componentsCount = (System.Int32)s.componentsCount;
		ss.data = Core.Native.NativeInterface.ConvertValue(ref s.data);
		return ss;
	}

	public static ImageDefinition_c ConvertValue(ImageDefinition s, ref ImageDefinition_c ss) {
		Material.Native.NativeInterface.Material_ImageDefinition_init(ref ss);
		ss.id = (System.UInt32)s.id;
		ss.name = ConvertValue(s.name);
		ss.width = (Int32)s.width;
		ss.height = (Int32)s.height;
		ss.bitsPerComponent = (Int32)s.bitsPerComponent;
		ss.componentsCount = (Int32)s.componentsCount;
		Core.Native.NativeInterface.ConvertValue(s.data, ref ss.data);
		return ss;
	}

	public static ImageDefinitionList ConvertValue(ref ImageDefinitionList_c s) {
		ImageDefinitionList list = new ImageDefinitionList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(ImageDefinition_c)));
			ImageDefinition_c value = (ImageDefinition_c)Marshal.PtrToStructure(p, typeof(ImageDefinition_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static ImageDefinitionList_c ConvertValue(ImageDefinitionList s, ref ImageDefinitionList_c list) {
		Material.Native.NativeInterface.Material_ImageDefinitionList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			ImageDefinition_c elt = new ImageDefinition_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(ImageDefinition_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static CoeffOrTexture ConvertValue(ref CoeffOrTexture_c s) {
		CoeffOrTexture ss = new CoeffOrTexture();
		ss._type = (CoeffOrTexture.Type)s._type;
		switch(ss._type) {
			case CoeffOrTexture.Type.UNKNOWN: break;
			case CoeffOrTexture.Type.COEFF: ss.coeff = (System.Double)s.coeff; break;
			case CoeffOrTexture.Type.TEXTURE: ss.texture = ConvertValue(ref s.texture); break;
		}
		return ss;
	}

	public static CoeffOrTexture_c ConvertValue(CoeffOrTexture s, ref CoeffOrTexture_c ss) {
		Material.Native.NativeInterface.Material_CoeffOrTexture_init(ref ss);
		ss._type = (int)s._type;
		switch (ss._type) {
			case 0: break;
			case 1: ss.coeff = (System.Double)s.coeff; break;
			case 2: ConvertValue(s.texture, ref ss.texture); break;
		}
		return ss;
	}

	public static MaterialDefinition ConvertValue(ref MaterialDefinition_c s) {
		MaterialDefinition ss = new MaterialDefinition();
		ss.name = ConvertValue(s.name);
		ss.id = (System.UInt32)s.id;
		ss.albedo = ConvertValue(ref s.albedo);
		ss.normal = ConvertValue(ref s.normal);
		ss.metallic = ConvertValue(ref s.metallic);
		ss.roughness = ConvertValue(ref s.roughness);
		ss.ao = ConvertValue(ref s.ao);
		ss.opacity = ConvertValue(ref s.opacity);
		ss.emissive = ConvertValue(ref s.emissive);
		return ss;
	}

	public static MaterialDefinition_c ConvertValue(MaterialDefinition s, ref MaterialDefinition_c ss) {
		Material.Native.NativeInterface.Material_MaterialDefinition_init(ref ss);
		ss.name = ConvertValue(s.name);
		ss.id = (System.UInt32)s.id;
		ConvertValue(s.albedo, ref ss.albedo);
		ConvertValue(s.normal, ref ss.normal);
		ConvertValue(s.metallic, ref ss.metallic);
		ConvertValue(s.roughness, ref ss.roughness);
		ConvertValue(s.ao, ref ss.ao);
		ConvertValue(s.opacity, ref ss.opacity);
		ConvertValue(s.emissive, ref ss.emissive);
		return ss;
	}

	public static MaterialDefinitionList ConvertValue(ref MaterialDefinitionList_c s) {
		MaterialDefinitionList list = new MaterialDefinitionList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(MaterialDefinition_c)));
			MaterialDefinition_c value = (MaterialDefinition_c)Marshal.PtrToStructure(p, typeof(MaterialDefinition_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static MaterialDefinitionList_c ConvertValue(MaterialDefinitionList s, ref MaterialDefinitionList_c list) {
		Material.Native.NativeInterface.Material_MaterialDefinitionList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			MaterialDefinition_c elt = new MaterialDefinition_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(MaterialDefinition_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static ImpostorMaterialInfos ConvertValue(ref ImpostorMaterialInfos_c s) {
		ImpostorMaterialInfos ss = new ImpostorMaterialInfos();
		ss.name = ConvertValue(s.name);
		ss.albedo = ConvertValue(ref s.albedo);
		ss.normal = ConvertValue(ref s.normal);
		ss.roughness = ConvertValue(ref s.roughness);
		ss.ao = ConvertValue(ref s.ao);
		ss.depth = ConvertValue(ref s.depth);
		ss.nbFrames = (System.Int32)s.nbFrames;
		ss.fullOcta = ConvertValue(s.fullOcta);
		ss.octaDiameter = (System.Double)s.octaDiameter;
		ss.linearSteps = (System.Int32)s.linearSteps;
		ss.binarySteps = (System.Int32)s.binarySteps;
		return ss;
	}

	public static ImpostorMaterialInfos_c ConvertValue(ImpostorMaterialInfos s, ref ImpostorMaterialInfos_c ss) {
		Material.Native.NativeInterface.Material_ImpostorMaterialInfos_init(ref ss);
		ss.name = ConvertValue(s.name);
		ConvertValue(s.albedo, ref ss.albedo);
		ConvertValue(s.normal, ref ss.normal);
		ConvertValue(s.roughness, ref ss.roughness);
		ConvertValue(s.ao, ref ss.ao);
		ConvertValue(s.depth, ref ss.depth);
		ss.nbFrames = (Int32)s.nbFrames;
		ss.fullOcta = ConvertValue(s.fullOcta);
		ss.octaDiameter = (System.Double)s.octaDiameter;
		ss.linearSteps = (Int32)s.linearSteps;
		ss.binarySteps = (Int32)s.binarySteps;
		return ss;
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

	public static ColorMaterialInfos ConvertValue(ref ColorMaterialInfos_c s) {
		ColorMaterialInfos ss = new ColorMaterialInfos();
		ss.name = ConvertValue(s.name);
		ss.color = Core.Native.NativeInterface.ConvertValue(ref s.color);
		return ss;
	}

	public static ColorMaterialInfos_c ConvertValue(ColorMaterialInfos s, ref ColorMaterialInfos_c ss) {
		Material.Native.NativeInterface.Material_ColorMaterialInfos_init(ref ss);
		ss.name = ConvertValue(s.name);
		Core.Native.NativeInterface.ConvertValue(s.color, ref ss.color);
		return ss;
	}

	public static ImageList ConvertValue(ref ImageList_c s) {
		ImageList list = new ImageList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static ImageList_c ConvertValue(ImageList s, ref ImageList_c list) {
		Material.Native.NativeInterface.Material_ImageList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static StandardMaterialInfos ConvertValue(ref StandardMaterialInfos_c s) {
		StandardMaterialInfos ss = new StandardMaterialInfos();
		ss.name = ConvertValue(s.name);
		ss.diffuse = ConvertValue(ref s.diffuse);
		ss.specular = ConvertValue(ref s.specular);
		ss.ambient = ConvertValue(ref s.ambient);
		ss.emissive = ConvertValue(ref s.emissive);
		ss.shininess = (System.Double)s.shininess;
		ss.transparency = (System.Double)s.transparency;
		return ss;
	}

	public static StandardMaterialInfos_c ConvertValue(StandardMaterialInfos s, ref StandardMaterialInfos_c ss) {
		Material.Native.NativeInterface.Material_StandardMaterialInfos_init(ref ss);
		ss.name = ConvertValue(s.name);
		ConvertValue(s.diffuse, ref ss.diffuse);
		ConvertValue(s.specular, ref ss.specular);
		ConvertValue(s.ambient, ref ss.ambient);
		ConvertValue(s.emissive, ref ss.emissive);
		ss.shininess = (System.Double)s.shininess;
		ss.transparency = (System.Double)s.transparency;
		return ss;
	}

	public static PBRMaterialInfos ConvertValue(ref PBRMaterialInfos_c s) {
		PBRMaterialInfos ss = new PBRMaterialInfos();
		ss.name = ConvertValue(s.name);
		ss.albedo = ConvertValue(ref s.albedo);
		ss.normal = ConvertValue(ref s.normal);
		ss.metallic = ConvertValue(ref s.metallic);
		ss.roughness = ConvertValue(ref s.roughness);
		ss.ao = ConvertValue(ref s.ao);
		ss.opacity = ConvertValue(ref s.opacity);
		return ss;
	}

	public static PBRMaterialInfos_c ConvertValue(PBRMaterialInfos s, ref PBRMaterialInfos_c ss) {
		Material.Native.NativeInterface.Material_PBRMaterialInfos_init(ref ss);
		ss.name = ConvertValue(s.name);
		ConvertValue(s.albedo, ref ss.albedo);
		ConvertValue(s.normal, ref ss.normal);
		ConvertValue(s.metallic, ref ss.metallic);
		ConvertValue(s.roughness, ref ss.roughness);
		ConvertValue(s.ao, ref ss.ao);
		ConvertValue(s.opacity, ref ss.opacity);
		return ss;
	}

	public static MaterialList ConvertValue(ref MaterialList_c s) {
		MaterialList list = new MaterialList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static MaterialList_c ConvertValue(MaterialList s, ref MaterialList_c list) {
		Material.Native.NativeInterface.Material_MaterialList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		int[] tab = new int[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = (int)(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static UnlitTextureMaterialInfos ConvertValue(ref UnlitTextureMaterialInfos_c s) {
		UnlitTextureMaterialInfos ss = new UnlitTextureMaterialInfos();
		ss.name = ConvertValue(s.name);
		ss.texture = ConvertValue(ref s.texture);
		return ss;
	}

	public static UnlitTextureMaterialInfos_c ConvertValue(UnlitTextureMaterialInfos s, ref UnlitTextureMaterialInfos_c ss) {
		Material.Native.NativeInterface.Material_UnlitTextureMaterialInfos_init(ref ss);
		ss.name = ConvertValue(s.name);
		ConvertValue(s.texture, ref ss.texture);
		return ss;
	}

	public static getImageSizeReturn ConvertValue(ref getImageSizeReturn_c s) {
		getImageSizeReturn ss = new getImageSizeReturn();
		ss.width = (System.Int32)s.width;
		ss.height = (System.Int32)s.height;
		return ss;
	}

	public static getImageSizeReturn_c ConvertValue(getImageSizeReturn s, ref getImageSizeReturn_c ss) {
		ss.width = (Int32)s.width;
		ss.height = (Int32)s.height;
		return ss;
	}

		#endregion

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Material_getLastError();

		public static string GetLastError()
		{
			return ConvertValue(Material_getLastError());
		}

		#region 

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Material_addUniformProperty(System.UInt32 pattern, string name, Int32 type);
		/// <summary>
		/// Add a shader uniform parameter to the given custom pattern
		/// </summary>
		/// <param name="pattern">The custom pattern to edit</param>
		/// <param name="name">Name of the new property</param>
		/// <param name="type">Type of the new uniform</param>
		public static void AddUniformProperty(System.UInt32 pattern, System.String name, ShaderUniformType type) {
			Material_addUniformProperty(pattern, name, (int)type);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Material_clearAllMaterials();
		/// <summary>
		/// Remove and delete all the materials
		/// </summary>
		public static void ClearAllMaterials() {
			Material_clearAllMaterials();
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Material_convertAllMaterialsToColors();
		/// <summary>
		/// Converts all the material in the scene to color materials
		/// </summary>
		public static void ConvertAllMaterialsToColors() {
			Material_convertAllMaterialsToColors();
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Material_convertHeightMapToNormalMap(System.UInt32 hmap, System.Double height);
		/// <summary>
		/// Convert a height map to a normal map
		/// </summary>
		/// <param name="hmap">Height map reference</param>
		/// <param name="height">Maximum height</param>
		public static System.UInt32 ConvertHeightMapToNormalMap(System.UInt32 hmap, System.Double height) {
			var ret = Material_convertHeightMapToNormalMap(hmap, height);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Material_convertNormalMapToHeightMap(System.UInt32 nmap, System.Double height);
		/// <summary>
		/// Convert a normal map to a height map
		/// </summary>
		/// <param name="nmap">Normal map reference</param>
		/// <param name="height">Maximum height</param>
		public static System.UInt32 ConvertNormalMapToHeightMap(System.UInt32 nmap, System.Double height) {
			var ret = Material_convertNormalMapToHeightMap(nmap, height);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Material_createCustomMaterialPattern(string name);
		/// <summary>
		/// Create a new custom material pattern
		/// </summary>
		/// <param name="name">Name of the pattern</param>
		public static System.UInt32 CreateCustomMaterialPattern(System.String name) {
			var ret = Material_createCustomMaterialPattern(name);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Material_createImageFromDefinition(ImageDefinition_c imageDefinition);
		/// <summary>
		/// Import an image from its raw data
		/// </summary>
		/// <param name="imageDefinition">The image definition</param>
		public static System.UInt32 CreateImageFromDefinition(ImageDefinition imageDefinition) {
			var imageDefinition_c = new Material.Native.ImageDefinition_c();
			ConvertValue(imageDefinition, ref imageDefinition_c);
			var ret = Material_createImageFromDefinition(imageDefinition_c);
			Material.Native.NativeInterface.Material_ImageDefinition_free(ref imageDefinition_c);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern ImageList_c Material_createImagesFromDefinitions(ImageDefinitionList_c imageDefinitions);
		/// <summary>
		/// Import images from their raw data
		/// </summary>
		/// <param name="imageDefinitions">The image definitions</param>
		public static ImageList CreateImagesFromDefinitions(ImageDefinitionList imageDefinitions) {
			var imageDefinitions_c = new Material.Native.ImageDefinitionList_c();
			ConvertValue(imageDefinitions, ref imageDefinitions_c);
			var ret = Material_createImagesFromDefinitions(imageDefinitions_c);
			Material.Native.NativeInterface.Material_ImageDefinitionList_free(ref imageDefinitions_c);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Material.Native.NativeInterface.Material_ImageList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Material_createMaterial(string name, string pattern);
		/// <summary>
		/// Create a new material from pattern
		/// </summary>
		/// <param name="name">Name of the material</param>
		/// <param name="pattern">Name of the pattern</param>
		public static System.UInt32 CreateMaterial(System.String name, System.String pattern) {
			var ret = Material_createMaterial(name, pattern);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Material_createMaterialFromDefinition(MaterialDefinition_c materialDefinition);
		/// <summary>
		/// Create PBR material from a material definition
		/// </summary>
		/// <param name="materialDefinition">The structure containing all the PBR material informations</param>
		public static System.UInt32 CreateMaterialFromDefinition(MaterialDefinition materialDefinition) {
			var materialDefinition_c = new Material.Native.MaterialDefinition_c();
			ConvertValue(materialDefinition, ref materialDefinition_c);
			var ret = Material_createMaterialFromDefinition(materialDefinition_c);
			Material.Native.NativeInterface.Material_MaterialDefinition_free(ref materialDefinition_c);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern MaterialList_c Material_createMaterialsFromDefinitions(MaterialDefinitionList_c materialDefinitions);
		/// <summary>
		/// Create PBR materials from material definitions
		/// </summary>
		/// <param name="materialDefinitions">Material definitions containing properties for each given material</param>
		public static MaterialList CreateMaterialsFromDefinitions(MaterialDefinitionList materialDefinitions) {
			var materialDefinitions_c = new Material.Native.MaterialDefinitionList_c();
			ConvertValue(materialDefinitions, ref materialDefinitions_c);
			var ret = Material_createMaterialsFromDefinitions(materialDefinitions_c);
			Material.Native.NativeInterface.Material_MaterialDefinitionList_free(ref materialDefinitions_c);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Material.Native.NativeInterface.Material_MaterialList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Material_createMaterialsFromMaps(string directory);
		/// <summary>
		/// Automatically creates PBR materials when importing PBR texture maps from a folder
		/// </summary>
		/// <param name="directory">Directory path</param>
		public static void CreateMaterialsFromMaps(System.String directory) {
			Material_createMaterialsFromMaps(directory);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Material_exportImage(System.UInt32 image, string filename);
		/// <summary>
		/// Export an image
		/// </summary>
		/// <param name="image">Identifier of the image to export</param>
		/// <param name="filename">Filename of the file to export</param>
		public static void ExportImage(System.UInt32 image, System.String filename) {
			Material_exportImage(image, filename);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Material_findCustomMaterialPatternByName(string name);
		/// <summary>
		/// Returns the material pattern which has the given name
		/// </summary>
		/// <param name="name">The name of the material pattern</param>
		public static System.UInt32 FindCustomMaterialPatternByName(System.String name) {
			var ret = Material_findCustomMaterialPatternByName(name);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern MaterialList_c Material_findMaterialsByPattern(string pattern);
		/// <summary>
		/// Returns all materials using the given pattern
		/// </summary>
		/// <param name="pattern">A material pattern</param>
		public static MaterialList FindMaterialsByPattern(System.String pattern) {
			var ret = Material_findMaterialsByPattern(pattern);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Material.Native.NativeInterface.Material_MaterialList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern MaterialList_c Material_findMaterialsByProperty(string propertyName, string propertyValue);
		/// <summary>
		/// Returns all materials which match a given property value
		/// </summary>
		/// <param name="propertyName">Name of the property to match</param>
		/// <param name="propertyValue">Regular expression to match for the property value</param>
		public static MaterialList FindMaterialsByProperty(System.String propertyName, System.String propertyValue) {
			var ret = Material_findMaterialsByProperty(propertyName, propertyValue);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Material.Native.NativeInterface.Material_MaterialList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern ImageList_c Material_getAllImages();
		/// <summary>
		/// Returns all the images loaded in the current session
		/// </summary>
		public static ImageList GetAllImages() {
			var ret = Material_getAllImages();
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Material.Native.NativeInterface.Material_ImageList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Core.Native.StringList_c Material_getAllMaterialPatterns();
		/// <summary>
		/// Returns all the material patterns in the current session
		/// </summary>
		public static Core.Native.StringList GetAllMaterialPatterns() {
			var ret = Material_getAllMaterialPatterns();
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Core.Native.NativeInterface.ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_StringList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern MaterialList_c Material_getAllMaterials();
		/// <summary>
		/// Retrieve the list of all the materials in the material library
		/// </summary>
		public static MaterialList GetAllMaterials() {
			var ret = Material_getAllMaterials();
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Material.Native.NativeInterface.Material_MaterialList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern ColorMaterialInfos_c Material_getColorMaterialInfos(System.UInt32 material);
		/// <summary>
		/// Get color material properties
		/// </summary>
		/// <param name="material">The material to get properties</param>
		public static ColorMaterialInfos GetColorMaterialInfos(System.UInt32 material) {
			var ret = Material_getColorMaterialInfos(material);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Material.Native.NativeInterface.Material_ColorMaterialInfos_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Material_getCustomMaterialPattern(System.UInt32 material);
		/// <summary>
		/// Returns the custom material pattern associated to the custom material
		/// </summary>
		/// <param name="material">Custom material to get the pattern from</param>
		public static System.UInt32 GetCustomMaterialPattern(System.UInt32 material) {
			var ret = Material_getCustomMaterialPattern(material);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern ImageDefinition_c Material_getImageDefinition(System.UInt32 image);
		/// <summary>
		/// Returns the raw data of an image
		/// </summary>
		/// <param name="image">Image's definition</param>
		public static ImageDefinition GetImageDefinition(System.UInt32 image) {
			var ret = Material_getImageDefinition(image);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Material.Native.NativeInterface.Material_ImageDefinition_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern ImageDefinitionList_c Material_getImageDefinitions(ImageList_c images);
		/// <summary>
		/// Returns the raw data of a set of images
		/// </summary>
		/// <param name="images">The images</param>
		public static ImageDefinitionList GetImageDefinitions(ImageList images) {
			var images_c = new Material.Native.ImageList_c();
			ConvertValue(images, ref images_c);
			var ret = Material_getImageDefinitions(images_c);
			Material.Native.NativeInterface.Material_ImageList_free(ref images_c);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Material.Native.NativeInterface.Material_ImageDefinitionList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getImageSizeReturn_c Material_getImageSize(System.UInt32 image);
		/// <summary>
		/// Returns the size of an image
		/// </summary>
		/// <param name="image">The image to get the size from</param>
		public static Material.Native.getImageSizeReturn GetImageSize(System.UInt32 image) {
			var ret = Material_getImageSize(image);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			Material.Native.getImageSizeReturn retStruct = new Material.Native.getImageSizeReturn();
			retStruct.width = (System.Int32)ret.width;
			retStruct.height = (System.Int32)ret.height;
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern ImpostorMaterialInfos_c Material_getImpostorMaterialInfos(System.UInt32 material);
		/// <summary>
		/// Get impostor texture material properties
		/// </summary>
		/// <param name="material">The material to get properties</param>
		public static ImpostorMaterialInfos GetImpostorMaterialInfos(System.UInt32 material) {
			var ret = Material_getImpostorMaterialInfos(material);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Material.Native.NativeInterface.Material_ImpostorMaterialInfos_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern MaterialDefinition_c Material_getMaterialDefinition(System.UInt32 material);
		/// <summary>
		/// Returns the properties of a PBR Material
		/// </summary>
		/// <param name="material">The PBR Material</param>
		public static MaterialDefinition GetMaterialDefinition(System.UInt32 material) {
			var ret = Material_getMaterialDefinition(material);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Material.Native.NativeInterface.Material_MaterialDefinition_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern MaterialDefinitionList_c Material_getMaterialDefinitions(MaterialList_c materials);
		/// <summary>
		/// Returns the properties of a set of PBR Materials
		/// </summary>
		/// <param name="materials">The PBR Materials</param>
		public static MaterialDefinitionList GetMaterialDefinitions(MaterialList materials) {
			var materials_c = new Material.Native.MaterialList_c();
			ConvertValue(materials, ref materials_c);
			var ret = Material_getMaterialDefinitions(materials_c);
			Material.Native.NativeInterface.Material_MaterialList_free(ref materials_c);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Material.Native.NativeInterface.Material_MaterialDefinitionList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Material_getMaterialPatternType(System.UInt32 material);
		/// <summary>
		/// Returns the MaterialPatternType name of the material
		/// </summary>
		/// <param name="material">The material to find the pattern</param>
		public static MaterialPatternType GetMaterialPatternType(System.UInt32 material) {
			var ret = Material_getMaterialPatternType(material);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (MaterialPatternType)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern PBRMaterialInfos_c Material_getPBRMaterialInfos(System.UInt32 material);
		/// <summary>
		/// Get PBR  material properties
		/// </summary>
		/// <param name="material">The material to get properties</param>
		public static PBRMaterialInfos GetPBRMaterialInfos(System.UInt32 material) {
			var ret = Material_getPBRMaterialInfos(material);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Material.Native.NativeInterface.Material_PBRMaterialInfos_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern StandardMaterialInfos_c Material_getStandardMaterialInfos(System.UInt32 material);
		/// <summary>
		/// Get standard material properties
		/// </summary>
		/// <param name="material">The material to get properties</param>
		public static StandardMaterialInfos GetStandardMaterialInfos(System.UInt32 material) {
			var ret = Material_getStandardMaterialInfos(material);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Material.Native.NativeInterface.Material_StandardMaterialInfos_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Material_getUniformPropertyType(System.UInt32 pattern, string name);
		/// <summary>
		/// Get a shader uniform shader property type
		/// </summary>
		/// <param name="pattern">The custom pattern</param>
		/// <param name="name">Name of the property to get the type from</param>
		public static ShaderUniformType GetUniformPropertyType(System.UInt32 pattern, System.String name) {
			var ret = Material_getUniformPropertyType(pattern, name);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (ShaderUniformType)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern UnlitTextureMaterialInfos_c Material_getUnlitTextureMaterialInfos(System.UInt32 material);
		/// <summary>
		/// Get unlit texture material properties
		/// </summary>
		/// <param name="material">The material to get properties</param>
		public static UnlitTextureMaterialInfos GetUnlitTextureMaterialInfos(System.UInt32 material) {
			var ret = Material_getUnlitTextureMaterialInfos(material);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			Material.Native.NativeInterface.Material_UnlitTextureMaterialInfos_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Material_importImage(string filename);
		/// <summary>
		/// Import an image
		/// </summary>
		/// <param name="filename">Filename of the image to import</param>
		public static System.UInt32 ImportImage(System.String filename) {
			var ret = Material_importImage(filename);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Material_makeMaterialNamesUnique();
		/// <summary>
		/// Rename materials to have a unique name for each one
		/// </summary>
		public static void MakeMaterialNamesUnique() {
			Material_makeMaterialNamesUnique();
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Material_resizeImage(System.UInt32 image, Int32 width, Int32 height);
		/// <summary>
		/// Resize an image
		/// </summary>
		/// <param name="image">Image to be resize</param>
		/// <param name="width">New image width</param>
		/// <param name="height">New image height</param>
		public static void ResizeImage(System.UInt32 image, System.Int32 width, System.Int32 height) {
			Material_resizeImage(image, width, height);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Material_setFragmentShader(System.UInt32 pattern, string code);
		/// <summary>
		/// Set the fragment shader of a custom pattern
		/// </summary>
		/// <param name="pattern">The custom pattern to edit</param>
		/// <param name="code">The GLSL code of the fragment shader</param>
		public static void SetFragmentShader(System.UInt32 pattern, System.String code) {
			Material_setFragmentShader(pattern, code);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Material_setMaterialMainColor(System.UInt32 material, Core.Native.ColorAlpha_c color);
		/// <summary>
		/// Set the main color on any material pattern type
		/// </summary>
		/// <param name="material">The material to apply the color on</param>
		/// <param name="color">The color to apply</param>
		public static void SetMaterialMainColor(System.UInt32 material, Core.Native.ColorAlpha color) {
			var color_c = new Core.Native.ColorAlpha_c();
			Core.Native.NativeInterface.ConvertValue(color, ref color_c);
			Material_setMaterialMainColor(material, color_c);
			Core.Native.NativeInterface.Core_ColorAlpha_free(ref color_c);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Material_setMaterialPattern(System.UInt32 material, string pattern);
		/// <summary>
		/// Sets the MaterialPattern name of the material
		/// </summary>
		/// <param name="material">The material to find the pattern</param>
		/// <param name="pattern">The pattern of the material</param>
		public static void SetMaterialPattern(System.UInt32 material, System.String pattern) {
			Material_setMaterialPattern(material, pattern);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Material_setPBRMaterialInfos(System.UInt32 material, PBRMaterialInfos_c infos);
		/// <summary>
		/// Set PBR  material properties
		/// </summary>
		/// <param name="material">The material to set properties</param>
		/// <param name="infos">The PBRMaterialInfos properties</param>
		public static void SetPBRMaterialInfos(System.UInt32 material, PBRMaterialInfos infos) {
			var infos_c = new Material.Native.PBRMaterialInfos_c();
			ConvertValue(infos, ref infos_c);
			Material_setPBRMaterialInfos(material, infos_c);
			Material.Native.NativeInterface.Material_PBRMaterialInfos_free(ref infos_c);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Material_setVertexShader(System.UInt32 pattern, string code);
		/// <summary>
		/// Set the vertex shader of a custom pattern
		/// </summary>
		/// <param name="pattern">The custom pattern to edit</param>
		/// <param name="code">The GLSL code of the vertex shader</param>
		public static void SetVertexShader(System.UInt32 pattern, System.String code) {
			Material_setVertexShader(pattern, code);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Material_updateImageFromDefinition(System.UInt32 image, ImageDefinition_c imageDefinition);
		/// <summary>
		/// Update an image from its raw data
		/// </summary>
		/// <param name="image">The image to update</param>
		/// <param name="imageDefinition">The new data to apply</param>
		public static void UpdateImageFromDefinition(System.UInt32 image, ImageDefinition imageDefinition) {
			var imageDefinition_c = new Material.Native.ImageDefinition_c();
			ConvertValue(imageDefinition, ref imageDefinition_c);
			Material_updateImageFromDefinition(image, imageDefinition_c);
			Material.Native.NativeInterface.Material_ImageDefinition_free(ref imageDefinition_c);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Material_updateImagesFromDefinitions(ImageList_c image, ImageDefinitionList_c imageDefinitions);
		/// <summary>
		/// Update images from their raw data
		/// </summary>
		/// <param name="image">The image to update</param>
		/// <param name="imageDefinitions">The new data to apply</param>
		public static void UpdateImagesFromDefinitions(ImageList image, ImageDefinitionList imageDefinitions) {
			var image_c = new Material.Native.ImageList_c();
			ConvertValue(image, ref image_c);
			var imageDefinitions_c = new Material.Native.ImageDefinitionList_c();
			ConvertValue(imageDefinitions, ref imageDefinitions_c);
			Material_updateImagesFromDefinitions(image_c, imageDefinitions_c);
			Material.Native.NativeInterface.Material_ImageList_free(ref image_c);
			Material.Native.NativeInterface.Material_ImageDefinitionList_free(ref imageDefinitions_c);
			System.String err = ConvertValue(Material_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

	}
}
