#pragma warning disable CA2101

using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Security;
using System.Collections.Concurrent;
using PiXYZAPI = Pixyz.API.Native.NativeInterface;

namespace Pixyz.IO.Native {

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
		internal static extern void IO_OwnCloudAccess_init(ref OwnCloudAccess_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_OwnCloudAccess_free(ref OwnCloudAccess_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_WebDAVAccess_init(ref WebDAVAccess_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_WebDAVAccess_free(ref WebDAVAccess_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_LocalDirectoryAccess_init(ref LocalDirectoryAccess_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_LocalDirectoryAccess_free(ref LocalDirectoryAccess_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_OpenStackAccess_init(ref OpenStackAccess_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_OpenStackAccess_free(ref OpenStackAccess_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_DropBoxAccess_init(ref DropBoxAccess_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_DropBoxAccess_free(ref DropBoxAccess_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_CloudDirectory_init(ref CloudDirectory_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_CloudDirectory_free(ref CloudDirectory_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_Format_init(ref Format_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_Format_free(ref Format_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_LocalFileAccess_init(ref LocalFileAccess_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_LocalFileAccess_free(ref LocalFileAccess_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_CloudFile_init(ref CloudFile_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_CloudFile_free(ref CloudFile_c sel);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_FormatList_init(ref FormatList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_FormatList_free(ref FormatList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_ImportFilePathList_init(ref ImportFilePathList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_ImportFilePathList_free(ref ImportFilePathList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_FilesList_init(ref FilesList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_FilesList_free(ref FilesList_c list);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_ExportFilePathList_init(ref ExportFilePathList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void IO_ExportFilePathList_free(ref ExportFilePathList_c list);

	public static OwnCloudAccess ConvertValue(ref OwnCloudAccess_c s) {
		OwnCloudAccess ss = new OwnCloudAccess();
		ss.path = ConvertValue(s.path);
		ss.authUrl = ConvertValue(s.authUrl);
		ss.user = ConvertValue(s.user);
		ss.password = ConvertValue(s.password);
		return ss;
	}

	public static OwnCloudAccess_c ConvertValue(OwnCloudAccess s, ref OwnCloudAccess_c ss) {
		IO.Native.NativeInterface.IO_OwnCloudAccess_init(ref ss);
		ss.path = ConvertValue(s.path);
		ss.authUrl = ConvertValue(s.authUrl);
		ss.user = ConvertValue(s.user);
		ss.password = ConvertValue(s.password);
		return ss;
	}

	public static WebDAVAccess ConvertValue(ref WebDAVAccess_c s) {
		WebDAVAccess ss = new WebDAVAccess();
		ss.path = ConvertValue(s.path);
		ss.host = ConvertValue(s.host);
		ss.port = (System.UInt16)s.port;
		ss.protocol = (Protocol)s.protocol;
		ss.user = ConvertValue(s.user);
		ss.password = ConvertValue(s.password);
		ss.url = ConvertValue(s.url);
		return ss;
	}

	public static WebDAVAccess_c ConvertValue(WebDAVAccess s, ref WebDAVAccess_c ss) {
		IO.Native.NativeInterface.IO_WebDAVAccess_init(ref ss);
		ss.path = ConvertValue(s.path);
		ss.host = ConvertValue(s.host);
		ss.port = (System.UInt16)s.port;
		ss.protocol = (Int32)s.protocol;
		ss.user = ConvertValue(s.user);
		ss.password = ConvertValue(s.password);
		ss.url = ConvertValue(s.url);
		return ss;
	}

	public static LocalDirectoryAccess ConvertValue(ref LocalDirectoryAccess_c s) {
		LocalDirectoryAccess ss = new LocalDirectoryAccess();
		ss.path = ConvertValue(s.path);
		return ss;
	}

	public static LocalDirectoryAccess_c ConvertValue(LocalDirectoryAccess s, ref LocalDirectoryAccess_c ss) {
		IO.Native.NativeInterface.IO_LocalDirectoryAccess_init(ref ss);
		ss.path = ConvertValue(s.path);
		return ss;
	}

	public static OpenStackAccess ConvertValue(ref OpenStackAccess_c s) {
		OpenStackAccess ss = new OpenStackAccess();
		ss.path = ConvertValue(s.path);
		ss.authUrl = ConvertValue(s.authUrl);
		ss.region = ConvertValue(s.region);
		ss.user = ConvertValue(s.user);
		ss.password = ConvertValue(s.password);
		ss.domainId = ConvertValue(s.domainId);
		ss.projectId = ConvertValue(s.projectId);
		ss.container = ConvertValue(s.container);
		ss.objectStore = ConvertValue(s.objectStore);
		return ss;
	}

	public static OpenStackAccess_c ConvertValue(OpenStackAccess s, ref OpenStackAccess_c ss) {
		IO.Native.NativeInterface.IO_OpenStackAccess_init(ref ss);
		ss.path = ConvertValue(s.path);
		ss.authUrl = ConvertValue(s.authUrl);
		ss.region = ConvertValue(s.region);
		ss.user = ConvertValue(s.user);
		ss.password = ConvertValue(s.password);
		ss.domainId = ConvertValue(s.domainId);
		ss.projectId = ConvertValue(s.projectId);
		ss.container = ConvertValue(s.container);
		ss.objectStore = ConvertValue(s.objectStore);
		return ss;
	}

	public static DropBoxAccess ConvertValue(ref DropBoxAccess_c s) {
		DropBoxAccess ss = new DropBoxAccess();
		ss.path = ConvertValue(s.path);
		ss.token = ConvertValue(s.token);
		return ss;
	}

	public static DropBoxAccess_c ConvertValue(DropBoxAccess s, ref DropBoxAccess_c ss) {
		IO.Native.NativeInterface.IO_DropBoxAccess_init(ref ss);
		ss.path = ConvertValue(s.path);
		ss.token = ConvertValue(s.token);
		return ss;
	}

	public static CloudDirectory ConvertValue(ref CloudDirectory_c s) {
		CloudDirectory ss = new CloudDirectory();
		ss._type = (CloudDirectory.Type)s._type;
		switch(ss._type) {
			case CloudDirectory.Type.UNKNOWN: break;
			case CloudDirectory.Type.LOCAL: ss.local = ConvertValue(ref s.local); break;
			case CloudDirectory.Type.OPENSTACK: ss.openStack = ConvertValue(ref s.openStack); break;
			case CloudDirectory.Type.DROPBOX: ss.dropBox = ConvertValue(ref s.dropBox); break;
			case CloudDirectory.Type.OWNCLOUD: ss.ownCloud = ConvertValue(ref s.ownCloud); break;
			case CloudDirectory.Type.WEBDAV: ss.webDAV = ConvertValue(ref s.webDAV); break;
		}
		return ss;
	}

	public static CloudDirectory_c ConvertValue(CloudDirectory s, ref CloudDirectory_c ss) {
		IO.Native.NativeInterface.IO_CloudDirectory_init(ref ss);
		ss._type = (int)s._type;
		switch (ss._type) {
			case 0: break;
			case 1: ConvertValue(s.local, ref ss.local); break;
			case 2: ConvertValue(s.openStack, ref ss.openStack); break;
			case 3: ConvertValue(s.dropBox, ref ss.dropBox); break;
			case 4: ConvertValue(s.ownCloud, ref ss.ownCloud); break;
			case 5: ConvertValue(s.webDAV, ref ss.webDAV); break;
		}
		return ss;
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

	public static Format ConvertValue(ref Format_c s) {
		Format ss = new Format();
		ss.name = ConvertValue(s.name);
		ss.extensions = Core.Native.NativeInterface.ConvertValue(ref s.extensions);
		return ss;
	}

	public static Format_c ConvertValue(Format s, ref Format_c ss) {
		IO.Native.NativeInterface.IO_Format_init(ref ss);
		ss.name = ConvertValue(s.name);
		Core.Native.NativeInterface.ConvertValue(s.extensions, ref ss.extensions);
		return ss;
	}

	public static LocalFileAccess ConvertValue(ref LocalFileAccess_c s) {
		LocalFileAccess ss = new LocalFileAccess();
		ss.path = ConvertValue(s.path);
		return ss;
	}

	public static LocalFileAccess_c ConvertValue(LocalFileAccess s, ref LocalFileAccess_c ss) {
		IO.Native.NativeInterface.IO_LocalFileAccess_init(ref ss);
		ss.path = ConvertValue(s.path);
		return ss;
	}

	public static CloudFile ConvertValue(ref CloudFile_c s) {
		CloudFile ss = new CloudFile();
		ss._type = (CloudFile.Type)s._type;
		switch(ss._type) {
			case CloudFile.Type.UNKNOWN: break;
			case CloudFile.Type.LOCAL: ss.local = ConvertValue(ref s.local); break;
			case CloudFile.Type.OPENSTACK: ss.openStack = ConvertValue(ref s.openStack); break;
			case CloudFile.Type.DROPBOX: ss.dropBox = ConvertValue(ref s.dropBox); break;
			case CloudFile.Type.OWNCLOUD: ss.ownCloud = ConvertValue(ref s.ownCloud); break;
			case CloudFile.Type.WEBDAV: ss.webDAV = ConvertValue(ref s.webDAV); break;
		}
		return ss;
	}

	public static CloudFile_c ConvertValue(CloudFile s, ref CloudFile_c ss) {
		IO.Native.NativeInterface.IO_CloudFile_init(ref ss);
		ss._type = (int)s._type;
		switch (ss._type) {
			case 0: break;
			case 1: ConvertValue(s.local, ref ss.local); break;
			case 2: ConvertValue(s.openStack, ref ss.openStack); break;
			case 3: ConvertValue(s.dropBox, ref ss.dropBox); break;
			case 4: ConvertValue(s.ownCloud, ref ss.ownCloud); break;
			case 5: ConvertValue(s.webDAV, ref ss.webDAV); break;
		}
		return ss;
	}

	public static FormatList ConvertValue(ref FormatList_c s) {
		FormatList list = new FormatList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Format_c)));
			Format_c value = (Format_c)Marshal.PtrToStructure(p, typeof(Format_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static FormatList_c ConvertValue(FormatList s, ref FormatList_c list) {
		IO.Native.NativeInterface.IO_FormatList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			Format_c elt = new Format_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(Format_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
		return list;
	}

	public static ImportFilePathList ConvertValue(ref ImportFilePathList_c s) {
		ImportFilePathList list = new ImportFilePathList((int)s.size);
		if (s.size==0) return list;
		IntPtr[] tab = new IntPtr[s.size];
		Marshal.Copy(s.ptr, tab, 0, (int)s.size);
		for (int i = 0; i < (int)s.size; ++i) {
			list.list[i] = ConvertValue(tab[i]);
		}
		return list;
	}

	public static ImportFilePathList_c ConvertValue(ImportFilePathList s, ref ImportFilePathList_c list) {
		IO.Native.NativeInterface.IO_ImportFilePathList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		IntPtr[] tab = new IntPtr[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = ConvertValue(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static FilesList ConvertValue(ref FilesList_c s) {
		FilesList list = new FilesList((int)s.size);
		if (s.size==0) return list;
		IntPtr[] tab = new IntPtr[s.size];
		Marshal.Copy(s.ptr, tab, 0, (int)s.size);
		for (int i = 0; i < (int)s.size; ++i) {
			list.list[i] = ConvertValue(tab[i]);
		}
		return list;
	}

	public static FilesList_c ConvertValue(FilesList s, ref FilesList_c list) {
		IO.Native.NativeInterface.IO_FilesList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		IntPtr[] tab = new IntPtr[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = ConvertValue(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

	public static ExportFilePathList ConvertValue(ref ExportFilePathList_c s) {
		ExportFilePathList list = new ExportFilePathList((int)s.size);
		if (s.size==0) return list;
		IntPtr[] tab = new IntPtr[s.size];
		Marshal.Copy(s.ptr, tab, 0, (int)s.size);
		for (int i = 0; i < (int)s.size; ++i) {
			list.list[i] = ConvertValue(tab[i]);
		}
		return list;
	}

	public static ExportFilePathList_c ConvertValue(ExportFilePathList s, ref ExportFilePathList_c list) {
		IO.Native.NativeInterface.IO_ExportFilePathList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		IntPtr[] tab = new IntPtr[list.size];
		for (int i = 0; i < (int)list.size; ++i)
			tab[i] = ConvertValue(s.list[i]);
		Marshal.Copy(tab, 0, list.ptr, (int)list.size);
		return list;
	}

		#endregion

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr IO_getLastError();

		public static string GetLastError()
		{
			return ConvertValue(IO_getLastError());
		}

		#region Cloud

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void IO_exportSceneToReflect(System.UInt32 root, string sourceName, string uid, Int32 keepHierarchy, string configFile);
		/// <summary>
		/// Export current scene to a reflect project
		/// </summary>
		/// <param name="root">Identifier of the destination occurrence</param>
		/// <param name="sourceName">Push source name</param>
		/// <param name="uid">UID of the push, overwrite old push if it's same UID</param>
		/// <param name="keepHierarchy">Keep hierarchy or rake tree</param>
		/// <param name="configFile">Use existing JSON config file, discard reflect UI prompt</param>
		public static void ExportSceneToReflect(System.UInt32 root, System.String sourceName, System.String uid, System.Boolean keepHierarchy, System.String configFile) {
			IO_exportSceneToReflect(root, sourceName, uid, keepHierarchy ? 1 : 0, configFile);
			System.String err = ConvertValue(IO_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region Debug

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void IO_debug(CloudFile_c cloudFile);
		/// <summary>
		/// for debugging purpose only
		/// </summary>
		/// <param name="cloudFile">File to import</param>
		public static void Debug(CloudFile cloudFile) {
			var cloudFile_c = new IO.Native.CloudFile_c();
			ConvertValue(cloudFile, ref cloudFile_c);
			IO_debug(cloudFile_c);
			IO.Native.NativeInterface.IO_CloudFile_free(ref cloudFile_c);
			System.String err = ConvertValue(IO_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void IO_importDebugTessellation(string filename);
		/// <summary>
		/// for debugging purpose only
		/// </summary>
		/// <param name="filename">Path of the file to import</param>
		public static void ImportDebugTessellation(System.String filename) {
			IO_importDebugTessellation(filename);
			System.String err = ConvertValue(IO_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region Import/Export

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void IO_exportScene(string fileName, System.UInt32 root);
		/// <summary>
		/// Export a file
		/// </summary>
		/// <param name="fileName">Path of the file to export</param>
		/// <param name="root">Identifier of the root occurrence to export</param>
		public static void ExportScene(System.String fileName, System.UInt32 root) {
			IO_exportScene(fileName, root);
			System.String err = ConvertValue(IO_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void IO_exportSelection(string fileName, Int32 keepIntermediaryNodes);
		/// <summary>
		/// Export the selection to a file
		/// </summary>
		/// <param name="fileName">Path of the file to export</param>
		/// <param name="keepIntermediaryNodes">If true, intermerdiary hierarchy is kept</param>
		public static void ExportSelection(System.String fileName, System.Boolean keepIntermediaryNodes) {
			IO_exportSelection(fileName, keepIntermediaryNodes ? 1 : 0);
			System.String err = ConvertValue(IO_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern FormatList_c IO_getExportFormats();
		/// <summary>
		/// Give all the format name and their extensions that can be exported in Pixyz
		/// </summary>
		public static FormatList GetExportFormats() {
			var ret = IO_getExportFormats();
			System.String err = ConvertValue(IO_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			IO.Native.NativeInterface.IO_FormatList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern FormatList_c IO_getImportFormats();
		/// <summary>
		/// Give all the format name and their extensions that can be imported in Pixyz
		/// </summary>
		public static FormatList GetImportFormats() {
			var ret = IO_getImportFormats();
			System.String err = ConvertValue(IO_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			IO.Native.NativeInterface.IO_FormatList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Scene.Native.OccurrenceList_c IO_importFiles(FilesList_c fileNames, System.UInt32 root);
		/// <summary>
		/// Import files
		/// </summary>
		/// <param name="fileNames">List of files's paths to import</param>
		/// <param name="root">Identifier of the destination occurrence</param>
		public static Scene.Native.OccurrenceList ImportFiles(FilesList fileNames, System.UInt32 root) {
			var fileNames_c = new IO.Native.FilesList_c();
			ConvertValue(fileNames, ref fileNames_c);
			var ret = IO_importFiles(fileNames_c, root);
			IO.Native.NativeInterface.IO_FilesList_free(ref fileNames_c);
			System.String err = ConvertValue(IO_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Scene.Native.NativeInterface.ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 IO_importPicture(string filename, System.UInt32 root);
		/// <summary>
		/// Import a picture
		/// </summary>
		/// <param name="filename">Path of the file to import</param>
		/// <param name="root">Identifier of the destination occurrence</param>
		public static System.UInt32 ImportPicture(System.String filename, System.UInt32 root) {
			var ret = IO_importPicture(filename, root);
			System.String err = ConvertValue(IO_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 IO_importScene(string fileName, System.UInt32 root);
		/// <summary>
		/// Import a file
		/// </summary>
		/// <param name="fileName">Path of the file to import</param>
		/// <param name="root">Identifier of the destination occurrence</param>
		public static System.UInt32 ImportScene(System.String fileName, System.UInt32 root) {
			var ret = IO_importScene(fileName, root);
			System.String err = ConvertValue(IO_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		#endregion

	}
}
