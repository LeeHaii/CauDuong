#pragma warning disable CA2101

using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Security;
using System.Collections.Concurrent;
using PiXYZAPI = Pixyz.API.Native.NativeInterface;

namespace Pixyz.Process.Native {

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
		internal static extern void Process_BakeDiffuseOptions_init(ref BakeDiffuseOptions_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_BakeDiffuseOptions_free(ref BakeDiffuseOptions_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_GenerateDiffuseMap_init(ref GenerateDiffuseMap_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_GenerateDiffuseMap_free(ref GenerateDiffuseMap_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_DecimateParameters_init(ref DecimateParameters_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_DecimateParameters_free(ref DecimateParameters_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_DecimateParametersList_init(ref DecimateParametersList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_DecimateParametersList_free(ref DecimateParametersList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_ScaleSelect_init(ref ScaleSelect_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_ScaleSelect_free(ref ScaleSelect_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_Orientation_init(ref Orientation_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_Orientation_free(ref Orientation_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_OrientationSelect_init(ref OrientationSelect_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_OrientationSelect_free(ref OrientationSelect_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_CoordinateSystemOptions_init(ref CoordinateSystemOptions_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_CoordinateSystemOptions_free(ref CoordinateSystemOptions_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_BakeOptions_init(ref BakeOptions_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_BakeOptions_free(ref BakeOptions_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_BakeOptionSelector_init(ref BakeOptionSelector_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_BakeOptionSelector_free(ref BakeOptionSelector_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_TessellationQuality_init(ref TessellationQuality_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_TessellationQuality_free(ref TessellationQuality_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_TessellationSettings_init(ref TessellationSettings_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_TessellationSettings_free(ref TessellationSettings_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_ImportOptions_init(ref ImportOptions_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Process_ImportOptions_free(ref ImportOptions_c str);

	public static BakeDiffuseOptions ConvertValue(ref BakeDiffuseOptions_c s) {
		BakeDiffuseOptions ss = new BakeDiffuseOptions();
		ss.mapResolution = (System.Int32)s.mapResolution;
		ss.padding = (System.Int32)s.padding;
		return ss;
	}

	public static BakeDiffuseOptions_c ConvertValue(BakeDiffuseOptions s, ref BakeDiffuseOptions_c ss) {
		Process.Native.NativeInterface.Process_BakeDiffuseOptions_init(ref ss);
		ss.mapResolution = (Int32)s.mapResolution;
		ss.padding = (Int32)s.padding;
		return ss;
	}

	public static GenerateDiffuseMap ConvertValue(ref GenerateDiffuseMap_c s) {
		GenerateDiffuseMap ss = new GenerateDiffuseMap();
		ss._type = (GenerateDiffuseMap.Type)s._type;
		switch(ss._type) {
			case GenerateDiffuseMap.Type.UNKNOWN: break;
			case GenerateDiffuseMap.Type.YES: ss.yes = ConvertValue(ref s.yes); break;
			case GenerateDiffuseMap.Type.NO: ss.no = (Int32)s.no; break;
		}
		return ss;
	}

	public static GenerateDiffuseMap_c ConvertValue(GenerateDiffuseMap s, ref GenerateDiffuseMap_c ss) {
		Process.Native.NativeInterface.Process_GenerateDiffuseMap_init(ref ss);
		ss._type = (int)s._type;
		switch (ss._type) {
			case 0: break;
			case 1: ConvertValue(s.yes, ref ss.yes); break;
			case 2: ss.no = (Int32)s.no; break;
		}
		return ss;
	}

	public static DecimateParameters ConvertValue(ref DecimateParameters_c s) {
		DecimateParameters ss = new DecimateParameters();
		ss.surfacicTolerance = (System.Double)s.surfacicTolerance;
		ss.linearTolerance = (System.Double)s.linearTolerance;
		ss.normalTolerance = (System.Double)s.normalTolerance;
		return ss;
	}

	public static DecimateParameters_c ConvertValue(DecimateParameters s, ref DecimateParameters_c ss) {
		Process.Native.NativeInterface.Process_DecimateParameters_init(ref ss);
		ss.surfacicTolerance = (System.Double)s.surfacicTolerance;
		ss.linearTolerance = (System.Double)s.linearTolerance;
		ss.normalTolerance = (System.Double)s.normalTolerance;
		return ss;
	}

	public static DecimateParametersList ConvertValue(ref DecimateParametersList_c s) {
		DecimateParametersList list = new DecimateParametersList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<DecimateParameters>(s.ptr, (int)s.size);
		return list;
	}

	public static DecimateParametersList_c ConvertValue(DecimateParametersList s, ref DecimateParametersList_c list) {
		Process.Native.NativeInterface.Process_DecimateParametersList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			DecimateParameters_c elt = new DecimateParameters_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(DecimateParameters_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static ScaleSelect ConvertValue(ref ScaleSelect_c s) {
		ScaleSelect ss = new ScaleSelect();
		ss._type = (ScaleSelect.Type)s._type;
		switch(ss._type) {
			case ScaleSelect.Type.UNKNOWN: break;
			case ScaleSelect.Type.AUTOMATICSCALE: ss.automaticScale = (Int32)s.automaticScale; break;
			case ScaleSelect.Type.FIXSCALE: ss.fixScale = (System.Double)s.fixScale; break;
		}
		return ss;
	}

	public static ScaleSelect_c ConvertValue(ScaleSelect s, ref ScaleSelect_c ss) {
		Process.Native.NativeInterface.Process_ScaleSelect_init(ref ss);
		ss._type = (int)s._type;
		switch (ss._type) {
			case 0: break;
			case 1: ss.automaticScale = (Int32)s.automaticScale; break;
			case 2: ss.fixScale = (System.Double)s.fixScale; break;
		}
		return ss;
	}

	public static Orientation ConvertValue(ref Orientation_c s) {
		Orientation ss = new Orientation();
		ss.zUP = ConvertValue(s.zUP);
		ss.leftHanded = ConvertValue(s.leftHanded);
		return ss;
	}

	public static Orientation_c ConvertValue(Orientation s, ref Orientation_c ss) {
		Process.Native.NativeInterface.Process_Orientation_init(ref ss);
		ss.zUP = ConvertValue(s.zUP);
		ss.leftHanded = ConvertValue(s.leftHanded);
		return ss;
	}

	public static OrientationSelect ConvertValue(ref OrientationSelect_c s) {
		OrientationSelect ss = new OrientationSelect();
		ss._type = (OrientationSelect.Type)s._type;
		switch(ss._type) {
			case OrientationSelect.Type.UNKNOWN: break;
			case OrientationSelect.Type.AUTOMATICORIENTATION: ss.automaticOrientation = (Int32)s.automaticOrientation; break;
			case OrientationSelect.Type.FIXORIENTATION: ss.fixOrientation = ConvertValue(ref s.fixOrientation); break;
		}
		return ss;
	}

	public static OrientationSelect_c ConvertValue(OrientationSelect s, ref OrientationSelect_c ss) {
		Process.Native.NativeInterface.Process_OrientationSelect_init(ref ss);
		ss._type = (int)s._type;
		switch (ss._type) {
			case 0: break;
			case 1: ss.automaticOrientation = (Int32)s.automaticOrientation; break;
			case 2: ConvertValue(s.fixOrientation, ref ss.fixOrientation); break;
		}
		return ss;
	}

	public static CoordinateSystemOptions ConvertValue(ref CoordinateSystemOptions_c s) {
		CoordinateSystemOptions ss = new CoordinateSystemOptions();
		ss.orientation = ConvertValue(ref s.orientation);
		ss.scale = ConvertValue(ref s.scale);
		ss.snapToGround = ConvertValue(s.snapToGround);
		ss.centerToOrigin = ConvertValue(s.centerToOrigin);
		return ss;
	}

	public static CoordinateSystemOptions_c ConvertValue(CoordinateSystemOptions s, ref CoordinateSystemOptions_c ss) {
		Process.Native.NativeInterface.Process_CoordinateSystemOptions_init(ref ss);
		ConvertValue(s.orientation, ref ss.orientation);
		ConvertValue(s.scale, ref ss.scale);
		ss.snapToGround = ConvertValue(s.snapToGround);
		ss.centerToOrigin = ConvertValue(s.centerToOrigin);
		return ss;
	}

	public static Algo.Native.BakeMaps ConvertValue(ref Algo.Native.BakeMaps_c s) {
		Algo.Native.BakeMaps ss = new Algo.Native.BakeMaps();
		ss.diffuse = ConvertValue(s.diffuse);
		ss.normal = ConvertValue(s.normal);
		ss.roughness = ConvertValue(s.roughness);
		ss.metallic = ConvertValue(s.metallic);
		ss.opacity = ConvertValue(s.opacity);
		ss.ambientOcclusion = ConvertValue(s.ambientOcclusion);
		ss.emissive = ConvertValue(s.emissive);
		return ss;
	}

	public static Algo.Native.BakeMaps_c ConvertValue(Algo.Native.BakeMaps s, ref Algo.Native.BakeMaps_c ss) {
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

	public static BakeOptions ConvertValue(ref BakeOptions_c s) {
		BakeOptions ss = new BakeOptions();
		ss.resolution = (System.Int32)s.resolution;
		ss.padding = (System.Int32)s.padding;
		ss.textures = Algo.Native.NativeInterface.ConvertValue(ref s.textures);
		return ss;
	}

	public static BakeOptions_c ConvertValue(BakeOptions s, ref BakeOptions_c ss) {
		Process.Native.NativeInterface.Process_BakeOptions_init(ref ss);
		ss.resolution = (Int32)s.resolution;
		ss.padding = (Int32)s.padding;
		Algo.Native.NativeInterface.ConvertValue(s.textures, ref ss.textures);
		return ss;
	}

	public static BakeOptionSelector ConvertValue(ref BakeOptionSelector_c s) {
		BakeOptionSelector ss = new BakeOptionSelector();
		ss._type = (BakeOptionSelector.Type)s._type;
		switch(ss._type) {
			case BakeOptionSelector.Type.UNKNOWN: break;
			case BakeOptionSelector.Type.YES: ss.Yes = ConvertValue(ref s.Yes); break;
			case BakeOptionSelector.Type.NO: ss.No = (Int32)s.No; break;
		}
		return ss;
	}

	public static BakeOptionSelector_c ConvertValue(BakeOptionSelector s, ref BakeOptionSelector_c ss) {
		Process.Native.NativeInterface.Process_BakeOptionSelector_init(ref ss);
		ss._type = (int)s._type;
		switch (ss._type) {
			case 0: break;
			case 1: ConvertValue(s.Yes, ref ss.Yes); break;
			case 2: ss.No = (Int32)s.No; break;
		}
		return ss;
	}

	public static TessellationQuality ConvertValue(ref TessellationQuality_c s) {
		TessellationQuality ss = new TessellationQuality();
		ss.maxSag = (System.Double)s.maxSag;
		ss.maxLength = (System.Double)s.maxLength;
		ss.maxAngle = (System.Double)s.maxAngle;
		return ss;
	}

	public static TessellationQuality_c ConvertValue(TessellationQuality s, ref TessellationQuality_c ss) {
		Process.Native.NativeInterface.Process_TessellationQuality_init(ref ss);
		ss.maxSag = (System.Double)s.maxSag;
		ss.maxLength = (System.Double)s.maxLength;
		ss.maxAngle = (System.Double)s.maxAngle;
		return ss;
	}

	public static TessellationSettings ConvertValue(ref TessellationSettings_c s) {
		TessellationSettings ss = new TessellationSettings();
		ss._type = (TessellationSettings.Type)s._type;
		switch(ss._type) {
			case TessellationSettings.Type.UNKNOWN: break;
			case TessellationSettings.Type.USEPRESET: ss.usePreset = (QualityPreset)s.usePreset; break;
			case TessellationSettings.Type.USECUSTOMVALUES: ss.useCustomValues = ConvertValue(ref s.useCustomValues); break;
		}
		return ss;
	}

	public static TessellationSettings_c ConvertValue(TessellationSettings s, ref TessellationSettings_c ss) {
		Process.Native.NativeInterface.Process_TessellationSettings_init(ref ss);
		ss._type = (int)s._type;
		switch (ss._type) {
			case 0: break;
			case 1: ss.usePreset = (Int32)s.usePreset; break;
			case 2: ConvertValue(s.useCustomValues, ref ss.useCustomValues); break;
		}
		return ss;
	}

	public static ImportOptions ConvertValue(ref ImportOptions_c s) {
		ImportOptions ss = new ImportOptions();
		ss.orientFaces = ConvertValue(s.orientFaces);
		ss.preserveOriginalUVs = ConvertValue(s.preserveOriginalUVs);
		ss.removeDuplicatedMeshes = ConvertValue(s.removeDuplicatedMeshes);
		return ss;
	}

	public static ImportOptions_c ConvertValue(ImportOptions s, ref ImportOptions_c ss) {
		Process.Native.NativeInterface.Process_ImportOptions_init(ref ss);
		ss.orientFaces = ConvertValue(s.orientFaces);
		ss.preserveOriginalUVs = ConvertValue(s.preserveOriginalUVs);
		ss.removeDuplicatedMeshes = ConvertValue(s.removeDuplicatedMeshes);
		return ss;
	}

		#endregion

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Process_getLastError();

		public static string GetLastError()
		{
			return ConvertValue(Process_getLastError());
		}

		#region 

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Process_decimateTargetBake(Scene.Native.OccurrenceList_c occurrences, Algo.Native.DecimateOptionsSelector_c decimationTargetType, BakeOptions_c bakingOptions, Int32 overrideExistingUV);
		/// <summary>
		/// Automatically decimates a selection of meshes, using as a target a triangle count or a ratio (reduction percentage), and bakes Normals information into a texture (plus other textures).
		/// </summary>
		/// <param name="occurrences">Occurrences to process</param>
		/// <param name="decimationTargetType"></param>
		/// <param name="bakingOptions">Option maps baking</param>
		/// <param name="overrideExistingUV">Override the original UV or not</param>
		public static void DecimateTargetBake(Scene.Native.OccurrenceList occurrences, Algo.Native.DecimateOptionsSelector decimationTargetType, BakeOptions bakingOptions, System.Boolean overrideExistingUV) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var decimationTargetType_c = new Algo.Native.DecimateOptionsSelector_c();
			Algo.Native.NativeInterface.ConvertValue(decimationTargetType, ref decimationTargetType_c);
			var bakingOptions_c = new Process.Native.BakeOptions_c();
			ConvertValue(bakingOptions, ref bakingOptions_c);
			Process_decimateTargetBake(occurrences_c, decimationTargetType_c, bakingOptions_c, overrideExistingUV ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Algo.Native.NativeInterface.Algo_DecimateOptionsSelector_free(ref decimationTargetType_c);
			Process.Native.NativeInterface.Process_BakeOptions_free(ref bakingOptions_c);
			System.String err = ConvertValue(Process_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Process_generateLODChain(Scene.Native.OccurrenceList_c occurrences, DecimateParametersList_c decimateParametersList);
		/// <summary>
		/// Automatically generates 3 LODs for the current selection.
		/// </summary>
		/// <param name="occurrences">Scene paths of components to process</param>
		/// <param name="decimateParametersList">The list of all LOD decimate parameters</param>
		public static void GenerateLODChain(Scene.Native.OccurrenceList occurrences, DecimateParametersList decimateParametersList) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var decimateParametersList_c = new Process.Native.DecimateParametersList_c();
			ConvertValue(decimateParametersList, ref decimateParametersList_c);
			Process_generateLODChain(occurrences_c, decimateParametersList_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Process.Native.NativeInterface.Process_DecimateParametersList_free(ref decimateParametersList_c);
			System.String err = ConvertValue(Process_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Process_generatePhantomMesh(Scene.Native.OccurrenceList_c occurrences, GenerateDiffuseMap_c generateDiffuseMap);
		/// <summary>
		/// Automatically generates one unique optimized mesh out of the models in the scene, with material(s).
		/// </summary>
		/// <param name="occurrences">Occurrences to process</param>
		/// <param name="generateDiffuseMap"></param>
		public static void GeneratePhantomMesh(Scene.Native.OccurrenceList occurrences, GenerateDiffuseMap generateDiffuseMap) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var generateDiffuseMap_c = new Process.Native.GenerateDiffuseMap_c();
			ConvertValue(generateDiffuseMap, ref generateDiffuseMap_c);
			Process_generatePhantomMesh(occurrences_c, generateDiffuseMap_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Process.Native.NativeInterface.Process_GenerateDiffuseMap_free(ref generateDiffuseMap_c);
			System.String err = ConvertValue(Process_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Scene.Native.OccurrenceList_c Process_guidedImport(IO.Native.ImportFilePathList_c fileNames, CoordinateSystemOptions_c coordinateSystem, TessellationSettings_c tessellation, ImportOptions_c otherOptions, Int32 importLines, Int32 importPoints, Int32 importHidden, Int32 importPMI, Int32 importVariants, Int32 useAlternativeImporter);
		/// <summary>
		/// The Guided import automatically converts and readies your 3D model(s) with guided parameters.
		/// </summary>
		/// <param name="fileNames">List of files to import</param>
		/// <param name="coordinateSystem">"Position and Scale" parameters are meant to properly position, orient and scale the imported model, to match Pixyz Studio's own coordinate sytem (right-handed, Y is the Up-axis) and units (mm).<br></param>
		/// <param name="tessellation">Tessellation is the process of converting CAD models into meshes, usable by any 3D app (check out "About Tessellation" in the documentation for more information).<br>With 'Use Preset', Pixyz Studio automatically sets tessellation values to reach a predefined mesh quality.<br>With 'Use custom values', tessellation values can be set by the user.<br>Note that the Tessellation process is ignored when importing files containing only meshes (polygonal or 3D printing models for example), or Point clouds: this Tessellation process does not affect or change them.</param>
		/// <param name="otherOptions">Use the following parameters to fine-tune the import process</param>
		/// <param name="importLines">Use to import the lines (which can be CAD curves or polylines) included in the imported model, that are important to import and preserve.</param>
		/// <param name="importPoints">Use to import the points (which can be CAD point or free points, used as references for example) included in the imported model, that are important to import and preserve.<br>No need to enable this parameter when importing Point clouds.</param>
		/// <param name="importHidden">Use to import the parts that are originally hidden in the imported model.<br>They are imported but not made visible though.</param>
		/// <param name="importPMI">Use to import the PMI attached to the imported model (applicable for CAD models only)</param>
		/// <param name="importVariants">Use to import the variants attached to the imported model (applicable for CAD models only)</param>
		/// <param name="useAlternativeImporter">Some 3D/CAD formats handled by Pixyz Studio can be imported through 2 different importing technologies (or libraries).<br>When one file/model is not properly imported, or not imported at all, using this parameter allows to test importing through an alternative importer and having a better luck (providing the format offers an alternative importer).</param>
		public static Scene.Native.OccurrenceList GuidedImport(IO.Native.ImportFilePathList fileNames, CoordinateSystemOptions coordinateSystem, TessellationSettings tessellation, ImportOptions otherOptions, System.Boolean importLines, System.Boolean importPoints, System.Boolean importHidden, System.Boolean importPMI, System.Boolean importVariants, System.Boolean useAlternativeImporter) {
			var fileNames_c = new IO.Native.ImportFilePathList_c();
			IO.Native.NativeInterface.ConvertValue(fileNames, ref fileNames_c);
			var coordinateSystem_c = new Process.Native.CoordinateSystemOptions_c();
			ConvertValue(coordinateSystem, ref coordinateSystem_c);
			var tessellation_c = new Process.Native.TessellationSettings_c();
			ConvertValue(tessellation, ref tessellation_c);
			var otherOptions_c = new Process.Native.ImportOptions_c();
			ConvertValue(otherOptions, ref otherOptions_c);
			var ret = Process_guidedImport(fileNames_c, coordinateSystem_c, tessellation_c, otherOptions_c, importLines ? 1 : 0, importPoints ? 1 : 0, importHidden ? 1 : 0, importPMI ? 1 : 0, importVariants ? 1 : 0, useAlternativeImporter ? 1 : 0);
			IO.Native.NativeInterface.IO_ImportFilePathList_free(ref fileNames_c);
			Process.Native.NativeInterface.Process_CoordinateSystemOptions_free(ref coordinateSystem_c);
			Process.Native.NativeInterface.Process_TessellationSettings_free(ref tessellation_c);
			Process.Native.NativeInterface.Process_ImportOptions_free(ref otherOptions_c);
			System.String err = ConvertValue(Process_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Scene.Native.NativeInterface.ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Process_proxyFromMeshes(Scene.Native.OccurrenceList_c occurrences, Int32 gridResolution, BakeOptionSelector_c generateTextures, Int32 transferAnimations, Int32 keepOriginalMesh);
		/// <summary>
		/// Automatically generates a Proxy Mesh out of a selection of meshes, with optional automatic textures generation.
		/// </summary>
		/// <param name="occurrences">Occurrences to process</param>
		/// <param name="gridResolution">Resolution of the voxel grid used to generated the proxy</param>
		/// <param name="generateTextures">Option maps baking</param>
		/// <param name="transferAnimations">If false, skinned animations will be lost</param>
		/// <param name="keepOriginalMesh">If true, does not delete original mesh at the end of the process</param>
		public static System.UInt32 ProxyFromMeshes(Scene.Native.OccurrenceList occurrences, System.Int32 gridResolution, BakeOptionSelector generateTextures, System.Boolean transferAnimations, System.Boolean keepOriginalMesh) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var generateTextures_c = new Process.Native.BakeOptionSelector_c();
			ConvertValue(generateTextures, ref generateTextures_c);
			var ret = Process_proxyFromMeshes(occurrences_c, gridResolution, generateTextures_c, transferAnimations ? 1 : 0, keepOriginalMesh ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Process.Native.NativeInterface.Process_BakeOptionSelector_free(ref generateTextures_c);
			System.String err = ConvertValue(Process_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Process_proxyFromPointCloud(Scene.Native.OccurrenceList_c occurrences, Int32 gridResolution, GenerateDiffuseMap_c generateDiffuseTexture, Int32 keepOriginalPointCloud);
		/// <summary>
		/// Automatically generates a Proxy Mesh out of a selection of Point Cloud, with optional automatic diffuse texture generation.
		/// </summary>
		/// <param name="occurrences">Occurrences to process</param>
		/// <param name="gridResolution">Resolution of the voxel grid used to generated the proxy</param>
		/// <param name="generateDiffuseTexture">Baking options</param>
		/// <param name="keepOriginalPointCloud">If true, does not delete original point cloud at the end of the process</param>
		public static System.UInt32 ProxyFromPointCloud(Scene.Native.OccurrenceList occurrences, System.Int32 gridResolution, GenerateDiffuseMap generateDiffuseTexture, System.Boolean keepOriginalPointCloud) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			var generateDiffuseTexture_c = new Process.Native.GenerateDiffuseMap_c();
			ConvertValue(generateDiffuseTexture, ref generateDiffuseTexture_c);
			var ret = Process_proxyFromPointCloud(occurrences_c, gridResolution, generateDiffuseTexture_c, keepOriginalPointCloud ? 1 : 0);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			Process.Native.NativeInterface.Process_GenerateDiffuseMap_free(ref generateDiffuseTexture_c);
			System.String err = ConvertValue(Process_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Process_runGenericProcess(Int32 overrideExistingUVs, Int32 repackUVs);
		/// <summary>
		/// Automatically creates optimized meshes out of any 3D model (mesh or CAD).
		/// </summary>
		/// <param name="overrideExistingUVs">If True, UVs for channel 0 will be recreated (algo GenerateUVonAABB , size=100)</param>
		/// <param name="repackUVs">If True, UVs for Channel 1 will be automatically repacked. Do not use if the original UV layout is fine (meshes already properly UVed).</param>
		public static void RunGenericProcess(System.Boolean overrideExistingUVs, System.Boolean repackUVs) {
			Process_runGenericProcess(overrideExistingUVs ? 1 : 0, repackUVs ? 1 : 0);
			System.String err = ConvertValue(Process_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

	}
}
