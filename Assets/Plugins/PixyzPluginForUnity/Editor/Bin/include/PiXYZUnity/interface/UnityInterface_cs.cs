#pragma warning disable CA2101

using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Security;
using System.Collections.Concurrent;
using PiXYZAPI = Pixyz.API.Native.NativeInterface;

namespace Pixyz.Unity.Native {

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
		private static extern IntPtr Unity_getLastError();

		public static string GetLastError()
		{
			return ConvertValue(Unity_getLastError());
		}

		#region Rendering Functions

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.IntPtr Unity_getDestroyFunction();
		/// <summary>
		/// 
		/// </summary>
		public static System.IntPtr GetDestroyFunction() {
			var ret = Unity_getDestroyFunction();
			System.String err = ConvertValue(Unity_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.IntPtr)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.IntPtr Unity_getDrawFunction();
		/// <summary>
		/// 
		/// </summary>
		public static System.IntPtr GetDrawFunction() {
			var ret = Unity_getDrawFunction();
			System.String err = ConvertValue(Unity_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.IntPtr)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.IntPtr Unity_getInitFunction();
		/// <summary>
		/// 
		/// </summary>
		public static System.IntPtr GetInitFunction() {
			var ret = Unity_getInitFunction();
			System.String err = ConvertValue(Unity_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.IntPtr)ret;
		}

		#endregion

		#region Rendering functions

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 Unity_createViewSession(Int32 width, Int32 height, View.Native.ViewSessionTextureList_c textures);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="width">Viewer width</param>
		/// <param name="height">Viewer height</param>
		/// <param name="textures">Textures that will be written on</param>
		public static System.UInt32 CreateViewSession(System.Int32 width, System.Int32 height, View.Native.ViewSessionTextureList textures) {
			var textures_c = new View.Native.ViewSessionTextureList_c();
			View.Native.NativeInterface.ConvertValue(textures, ref textures_c);
			var ret = Unity_createViewSession(width, height, textures_c);
			View.Native.NativeInterface.View_ViewSessionTextureList_free(ref textures_c);
			System.String err = ConvertValue(Unity_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		#endregion

		#region directx

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.IntPtr Unity_getD3D11Device();
		/// <summary>
		/// 
		/// </summary>
		public static System.IntPtr GetD3D11Device() {
			var ret = Unity_getD3D11Device();
			System.String err = ConvertValue(Unity_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.IntPtr)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.IntPtr Unity_getD3D11RenderTargetViewFromRenderBuffer(System.IntPtr surface);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="surface"></param>
		public static System.IntPtr GetD3D11RenderTargetViewFromRenderBuffer(System.IntPtr surface) {
			var ret = Unity_getD3D11RenderTargetViewFromRenderBuffer(surface);
			System.String err = ConvertValue(Unity_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.IntPtr)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.IntPtr Unity_getD3D11ShaderResourceViewFromNativeTexture(System.UInt32 texture);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="texture"></param>
		public static System.IntPtr GetD3D11ShaderResourceViewFromNativeTexture(System.UInt32 texture) {
			var ret = Unity_getD3D11ShaderResourceViewFromNativeTexture(texture);
			System.String err = ConvertValue(Unity_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.IntPtr)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.IntPtr Unity_getD3D11TextureFromNativeTexture(System.UInt32 texture);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="texture"></param>
		public static System.IntPtr GetD3D11TextureFromNativeTexture(System.UInt32 texture) {
			var ret = Unity_getD3D11TextureFromNativeTexture(texture);
			System.String err = ConvertValue(Unity_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.IntPtr)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.IntPtr Unity_getD3D11TextureFromRenderBuffer(System.IntPtr buffer);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		public static System.IntPtr GetD3D11TextureFromRenderBuffer(System.IntPtr buffer) {
			var ret = Unity_getD3D11TextureFromRenderBuffer(buffer);
			System.String err = ConvertValue(Unity_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.IntPtr)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Unity_lockRenderUpdate();
		/// <summary>
		/// Lock the mutex that ensure no rendering is done between lock/unlock
		/// </summary>
		public static void LockRenderUpdate() {
			Unity_lockRenderUpdate();
			System.String err = ConvertValue(Unity_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 Unity_tryLockRenderUpdate();
		/// <summary>
		/// Try to lock the mutex that ensure no rendering is done between lock/unlock, returns true if the mutex has been locked, false if it was already locked
		/// </summary>
		public static System.Boolean TryLockRenderUpdate() {
			var ret = Unity_tryLockRenderUpdate();
			System.String err = ConvertValue(Unity_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Boolean)(0 != ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Unity_unlockRenderUpdate();
		/// <summary>
		/// Unlock the mutex that ensure no rendering is done between lock/unlock
		/// </summary>
		public static void UnlockRenderUpdate() {
			Unity_unlockRenderUpdate();
			System.String err = ConvertValue(Unity_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

	}
}
