#pragma warning disable CA2101

using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Security;
using System.Collections.Concurrent;
using PiXYZAPI = Pixyz.API.Native.NativeInterface;

namespace Pixyz.Cloud.Native {

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

		#endregion

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Cloud_getLastError();

		public static string GetLastError()
		{
			return ConvertValue(Cloud_getLastError());
		}

		#region AWS

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Cloud_downloadDirectoryFromS3(string bucketName, string region, string directoryPath, string directory);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bucketName">S3 bucket to download from</param>
		/// <param name="region">AWS region</param>
		/// <param name="directoryPath">Remote path to directory</param>
		/// <param name="directory">Local directory to dowmload in</param>
		public static void DownloadDirectoryFromS3(System.String bucketName, System.String region, System.String directoryPath, System.String directory) {
			Cloud_downloadDirectoryFromS3(bucketName, region, directoryPath, directory);
			System.String err = ConvertValue(Cloud_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Cloud_downloadFileFromS3(string bucketName, string region, string filePath, string directory);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bucketName">S3 bucket to download from</param>
		/// <param name="region">AWS region</param>
		/// <param name="filePath">Remote path to file</param>
		/// <param name="directory">Local directory to dowmload in</param>
		public static void DownloadFileFromS3(System.String bucketName, System.String region, System.String filePath, System.String directory) {
			Cloud_downloadFileFromS3(bucketName, region, filePath, directory);
			System.String err = ConvertValue(Cloud_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Core.Native.StringList_c Cloud_listFilesFromS3(string bucketName, string region, string prefix);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bucketName">S3 bucket to list</param>
		/// <param name="region">AWS region</param>
		/// <param name="prefix">Prefix for filtering</param>
		public static Core.Native.StringList ListFilesFromS3(System.String bucketName, System.String region, System.String prefix) {
			var ret = Cloud_listFilesFromS3(bucketName, region, prefix);
			System.String err = ConvertValue(Cloud_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Core.Native.NativeInterface.ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_StringList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Cloud_uploadDirectoryToS3(string bucketName, string region, string directoryPath, string directory);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bucketName">S3 bucket to upload in</param>
		/// <param name="region">AWS region</param>
		/// <param name="directoryPath">Local directory</param>
		/// <param name="directory">Remote path to upload in</param>
		public static void UploadDirectoryToS3(System.String bucketName, System.String region, System.String directoryPath, System.String directory) {
			Cloud_uploadDirectoryToS3(bucketName, region, directoryPath, directory);
			System.String err = ConvertValue(Cloud_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Cloud_uploadFileToS3(string bucketName, string region, string filePath, string directory);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bucketName">S3 bucket to upload in</param>
		/// <param name="region">AWS region</param>
		/// <param name="filePath">Local file</param>
		/// <param name="directory">Remote path to upload in</param>
		public static void UploadFileToS3(System.String bucketName, System.String region, System.String filePath, System.String directory) {
			Cloud_uploadFileToS3(bucketName, region, filePath, directory);
			System.String err = ConvertValue(Cloud_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

	}
}
