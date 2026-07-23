#pragma warning disable CA2101

using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Security;
using System.Collections.Concurrent;
using PiXYZAPI = Pixyz.API.Native.NativeInterface;

namespace Pixyz.Raytrace.Native {

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
		internal static extern void Raytrace_Camera_init(ref Camera_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void Raytrace_Camera_free(ref Camera_c str);

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
		Raytrace.Native.NativeInterface.Raytrace_Camera_init(ref ss);
		Geom.Native.NativeInterface.ConvertValue(s.position, ref ss.position);
		Geom.Native.NativeInterface.ConvertValue(s.direction, ref ss.direction);
		Geom.Native.NativeInterface.ConvertValue(s.up, ref ss.up);
		ss.fov = (System.Double)s.fov;
		return ss;
	}

		#endregion

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr Raytrace_getLastError();

		public static string GetLastError()
		{
			return ConvertValue(Raytrace_getLastError());
		}

		#region 

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void Raytrace_renderImage(Int32 width, Int32 height, Camera_c camera, string outputImagePath);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="camera"></param>
		/// <param name="outputImagePath"></param>
		public static void RenderImage(System.Int32 width, System.Int32 height, Camera camera, System.String outputImagePath) {
			var camera_c = new Raytrace.Native.Camera_c();
			ConvertValue(camera, ref camera_c);
			Raytrace_renderImage(width, height, camera_c, outputImagePath);
			Raytrace.Native.NativeInterface.Raytrace_Camera_free(ref camera_c);
			System.String err = ConvertValue(Raytrace_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

	}
}
