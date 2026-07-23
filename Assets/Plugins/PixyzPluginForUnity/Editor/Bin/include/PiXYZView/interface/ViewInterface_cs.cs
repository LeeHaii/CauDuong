#pragma warning disable CA2101

using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Security;
using System.Collections.Concurrent;
using PiXYZAPI = Pixyz.API.Native.NativeInterface;

namespace Pixyz.View.Native {

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
		internal static extern void View_DrawPrimitives_init(ref DrawPrimitives_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void View_DrawPrimitives_free(ref DrawPrimitives_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void View_ViewSessionTexture_init(ref ViewSessionTexture_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void View_ViewSessionTexture_free(ref ViewSessionTexture_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void View_PickResult_init(ref PickResult_c str);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void View_PickResult_free(ref PickResult_c str);

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void View_ViewSessionTextureList_init(ref ViewSessionTextureList_c list, UInt64 size);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		internal static extern void View_ViewSessionTextureList_free(ref ViewSessionTextureList_c list);

	public static DrawPrimitives ConvertValue(ref DrawPrimitives_c s) {
		DrawPrimitives ss = new DrawPrimitives();
		ss.polygons = ConvertValue(s.polygons);
		ss.breps = ConvertValue(s.breps);
		ss.wireframe = ConvertValue(s.wireframe);
		ss.points = ConvertValue(s.points);
		ss.freeLines = ConvertValue(s.freeLines);
		ss.patchBoundaries = ConvertValue(s.patchBoundaries);
		return ss;
	}

	public static DrawPrimitives_c ConvertValue(DrawPrimitives s, ref DrawPrimitives_c ss) {
		View.Native.NativeInterface.View_DrawPrimitives_init(ref ss);
		ss.polygons = ConvertValue(s.polygons);
		ss.breps = ConvertValue(s.breps);
		ss.wireframe = ConvertValue(s.wireframe);
		ss.points = ConvertValue(s.points);
		ss.freeLines = ConvertValue(s.freeLines);
		ss.patchBoundaries = ConvertValue(s.patchBoundaries);
		return ss;
	}

	public static ViewSessionTexture ConvertValue(ref ViewSessionTexture_c s) {
		ViewSessionTexture ss = new ViewSessionTexture();
		ss.type = (ViewSessionTextureType)s.type;
		ss.texture = (System.IntPtr)s.texture;
		return ss;
	}

	public static ViewSessionTexture_c ConvertValue(ViewSessionTexture s, ref ViewSessionTexture_c ss) {
		View.Native.NativeInterface.View_ViewSessionTexture_init(ref ss);
		ss.type = (Int32)s.type;
		ss.texture = (System.IntPtr)s.texture;
		return ss;
	}

	public static Scene.Native.OccurrenceList ConvertValue(ref Scene.Native.OccurrenceList_c s) {
		Scene.Native.OccurrenceList list = new Scene.Native.OccurrenceList((int)s.size);
		if (s.size==0) return list;
			list.list = CopyMemory<System.UInt32>(s.ptr, (int)s.size);
		return list;
	}

	public static Scene.Native.OccurrenceList_c ConvertValue(Scene.Native.OccurrenceList s, ref Scene.Native.OccurrenceList_c list) {
		Scene.Native.NativeInterface.Scene_OccurrenceList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
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

	public static PickResult ConvertValue(ref PickResult_c s) {
		PickResult ss = new PickResult();
		ss.occurrences = Scene.Native.NativeInterface.ConvertValue(ref s.occurrences);
		ss.positions = Geom.Native.NativeInterface.ConvertValue(ref s.positions);
		return ss;
	}

	public static PickResult_c ConvertValue(PickResult s, ref PickResult_c ss) {
		View.Native.NativeInterface.View_PickResult_init(ref ss);
		Scene.Native.NativeInterface.ConvertValue(s.occurrences, ref ss.occurrences);
		Geom.Native.NativeInterface.ConvertValue(s.positions, ref ss.positions);
		return ss;
	}

	public static ViewSessionTextureList ConvertValue(ref ViewSessionTextureList_c s) {
		ViewSessionTextureList list = new ViewSessionTextureList((int)s.size);
		if (s.size==0) return list;
		for (int i = 0; i < (int)s.size; ++i) {
			IntPtr p = new IntPtr(s.ptr.ToInt64() + i * Marshal.SizeOf(typeof(ViewSessionTexture_c)));
			ViewSessionTexture_c value = (ViewSessionTexture_c)Marshal.PtrToStructure(p, typeof(ViewSessionTexture_c));
			list.list[i] = ConvertValue(ref value);
		}
		return list;
	}

	public static ViewSessionTextureList_c ConvertValue(ViewSessionTextureList s, ref ViewSessionTextureList_c list) {
		View.Native.NativeInterface.View_ViewSessionTextureList_init(ref list, s == null ? 0 : (System.UInt64)s.length);
		if(list.size == 0) return list;
		for(int i = 0; i < (int)list.size; ++i) {
			ViewSessionTexture_c elt = new ViewSessionTexture_c();
			ConvertValue(s.list[i], ref elt);
			IntPtr p = new IntPtr(list.ptr.ToInt64() + i * Marshal.SizeOf(typeof(ViewSessionTexture_c)));
			Marshal.StructureToPtr(elt, p, true);
		}
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

	public static getViewerMatricesReturn ConvertValue(ref getViewerMatricesReturn_c s) {
		getViewerMatricesReturn ss = new getViewerMatricesReturn();
		ss.views = Geom.Native.NativeInterface.ConvertValue(ref s.views);
		ss.projs = Geom.Native.NativeInterface.ConvertValue(ref s.projs);
		ss.clipping = Geom.Native.NativeInterface.ConvertValue(ref s.clipping);
		return ss;
	}

	public static getViewerMatricesReturn_c ConvertValue(getViewerMatricesReturn s, ref getViewerMatricesReturn_c ss) {
		Geom.Native.NativeInterface.ConvertValue(s.views, ref ss.views);
		Geom.Native.NativeInterface.ConvertValue(s.projs, ref ss.projs);
		Geom.Native.NativeInterface.ConvertValue(s.clipping, ref ss.clipping);
		return ss;
	}

	public static getViewerSizeReturn ConvertValue(ref getViewerSizeReturn_c s) {
		getViewerSizeReturn ss = new getViewerSizeReturn();
		ss.width = (System.Int32)s.width;
		ss.height = (System.Int32)s.height;
		return ss;
	}

	public static getViewerSizeReturn_c ConvertValue(getViewerSizeReturn s, ref getViewerSizeReturn_c ss) {
		ss.width = (Int32)s.width;
		ss.height = (Int32)s.height;
		return ss;
	}

	public static pickReturn ConvertValue(ref pickReturn_c s) {
		pickReturn ss = new pickReturn();
		ss.occurrence = (System.UInt32)s.occurrence;
		ss.position = Geom.Native.NativeInterface.ConvertValue(ref s.position);
		return ss;
	}

	public static pickReturn_c ConvertValue(pickReturn s, ref pickReturn_c ss) {
		ss.occurrence = (System.UInt32)s.occurrence;
		Geom.Native.NativeInterface.ConvertValue(s.position, ref ss.position);
		return ss;
	}

		#endregion

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr View_getLastError();

		public static string GetLastError()
		{
			return ConvertValue(View_getLastError());
		}

		#region 

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_addNotification(string text, System.UInt32 time);
		/// <summary>
		/// Add a notification
		/// </summary>
		/// <param name="text">Notification text</param>
		/// <param name="time">Time to stay</param>
		public static void AddNotification(System.String text, System.UInt32 time) {
			View_addNotification(text, time);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_addRootToMainViewer(System.UInt32 occurrence);
		/// <summary>
		/// Add a root to the main viewer
		/// </summary>
		/// <param name="occurrence">Occurrence to add</param>
		public static void AddRootToMainViewer(System.UInt32 occurrence) {
			View_addRootToMainViewer(occurrence);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_fit(Scene.Native.OccurrenceList_c occurrences);
		/// <summary>
		/// Fit the main camera to the given parts
		/// </summary>
		/// <param name="occurrences">Parts to fit</param>
		public static void Fit(Scene.Native.OccurrenceList occurrences) {
			var occurrences_c = new Scene.Native.OccurrenceList_c();
			Scene.Native.NativeInterface.ConvertValue(occurrences, ref occurrences_c);
			View_fit(occurrences_c);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref occurrences_c);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_setCameraPosition(Geom.Native.Point3_c newPosition);
		/// <summary>
		/// Set the camera position
		/// </summary>
		/// <param name="newPosition">New camera position</param>
		public static void SetCameraPosition(Geom.Native.Point3 newPosition) {
			var newPosition_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(newPosition, ref newPosition_c);
			View_setCameraPosition(newPosition_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref newPosition_c);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_toOrigin();
		/// <summary>
		/// Place the camera to origin
		/// </summary>
		public static void ToOrigin() {
			View_toOrigin();
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		private delegate void AfterFramebufferCreateDelegate(System.IntPtr userdata);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 View_addAfterFramebufferCreateCallback(AfterFramebufferCreateDelegate callback, System.IntPtr userdata);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_removeAfterFramebufferCreateCallback(System.UInt32 id);

		private static System.UInt32 addAfterFramebufferCreateCallback(AfterFramebufferCreateDelegate callback, System.IntPtr userdata) {
			return View_addAfterFramebufferCreateCallback(callback, userdata);
		}

		private static void removeAfterFramebufferCreateCallback(System.UInt32 id) {
			View_removeAfterFramebufferCreateCallback(id);
		}

		public class AfterFramebufferCreateTask : PiXYZAPI.PixyzCallbackTask
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
					AfterFramebufferCreateDelegate callback = Updated;

					_delegatePtr = GCHandle.Alloc(callback, GCHandleType.Pinned);
					_callbackId = addAfterFramebufferCreateCallback(callback, IntPtr.Zero);

				}

				protected override void DetachCallback()
				{
					if (_callbackId == 0)
						return;

					removeAfterFramebufferCreateCallback(_callbackId);
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
			public static Task<Result> WaitAfterFramebufferCreate(CancellationTokenSource cancelSource = null)
			{
				return Task.Run(new Execution(cancelSource).RunOnce);
			}
		}
		private delegate void BeforeFramebufferDeleteDelegate(System.IntPtr userdata);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 View_addBeforeFramebufferDeleteCallback(BeforeFramebufferDeleteDelegate callback, System.IntPtr userdata);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_removeBeforeFramebufferDeleteCallback(System.UInt32 id);

		private static System.UInt32 addBeforeFramebufferDeleteCallback(BeforeFramebufferDeleteDelegate callback, System.IntPtr userdata) {
			return View_addBeforeFramebufferDeleteCallback(callback, userdata);
		}

		private static void removeBeforeFramebufferDeleteCallback(System.UInt32 id) {
			View_removeBeforeFramebufferDeleteCallback(id);
		}

		public class BeforeFramebufferDeleteTask : PiXYZAPI.PixyzCallbackTask
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
					BeforeFramebufferDeleteDelegate callback = Updated;

					_delegatePtr = GCHandle.Alloc(callback, GCHandleType.Pinned);
					_callbackId = addBeforeFramebufferDeleteCallback(callback, IntPtr.Zero);

				}

				protected override void DetachCallback()
				{
					if (_callbackId == 0)
						return;

					removeBeforeFramebufferDeleteCallback(_callbackId);
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
			public static Task<Result> WaitBeforeFramebufferDelete(CancellationTokenSource cancelSource = null)
			{
				return Task.Run(new Execution(cancelSource).RunOnce);
			}
		}
		private delegate void BeforeRefreshDelegate(System.IntPtr userdata, System.UInt64 globalTimeMillisecond);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 View_addBeforeRefreshCallback(BeforeRefreshDelegate callback, System.IntPtr userdata);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_removeBeforeRefreshCallback(System.UInt32 id);

		private static System.UInt32 addBeforeRefreshCallback(BeforeRefreshDelegate callback, System.IntPtr userdata) {
			return View_addBeforeRefreshCallback(callback, userdata);
		}

		private static void removeBeforeRefreshCallback(System.UInt32 id) {
			View_removeBeforeRefreshCallback(id);
		}

		public class BeforeRefreshTask : PiXYZAPI.PixyzCallbackTask
		{
			public struct Result
			{
				public System.UInt64 globalTimeMillisecond;

				public Result(System.UInt64 globalTimeMillisecond)
				{
					this.globalTimeMillisecond = globalTimeMillisecond;
				}
			}

			public class Execution : BaseTaskExecution<Result>
			{
				public Execution(CancellationTokenSource cancelTokenSource = null) : base(cancelTokenSource)
				{
				}

				protected override void AttachCallback()
				{
					BeforeRefreshDelegate callback = Updated;

					_delegatePtr = GCHandle.Alloc(callback, GCHandleType.Pinned);
					_callbackId = addBeforeRefreshCallback(callback, IntPtr.Zero);

				}

				protected override void DetachCallback()
				{
					if (_callbackId == 0)
						return;

					removeBeforeRefreshCallback(_callbackId);
					_callbackId = 0;
				}

				private void Updated(System.IntPtr userdata, System.UInt64 globalTimeMillisecond)
				{
					try
					{
						_results.Enqueue(new Result(globalTimeMillisecond));

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
			public static Task<Result> WaitBeforeRefresh(CancellationTokenSource cancelSource = null)
			{
				return Task.Run(new Execution(cancelSource).RunOnce);
			}
		}
		#endregion

		#region Animation Player

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_pauseAnimation();
		/// <summary>
		/// pauses an animation
		/// </summary>
		public static void PauseAnimation() {
			View_pauseAnimation();
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_playAnimation(System.UInt32 animation, System.Double speed, Int32 times);
		/// <summary>
		/// plays an animation
		/// </summary>
		/// <param name="animation">Animation to play</param>
		/// <param name="speed">Speed</param>
		/// <param name="times">Number of loops</param>
		public static void PlayAnimation(System.UInt32 animation, System.Double speed, System.Int32 times) {
			View_playAnimation(animation, speed, times);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_stopAnimation();
		/// <summary>
		/// stops an animation
		/// </summary>
		public static void StopAnimation() {
			View_stopAnimation();
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region Occurrence

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 View_createOccurrenceOnCameraTarget(string name, System.UInt32 parent);
		/// <summary>
		/// Create an occurrence on the target position of the camera (mid-click)
		/// </summary>
		/// <param name="name">Name of the new occurrence</param>
		/// <param name="parent">If defined, the new occurrence will be added as a child of the parent. Else the new parent will be the root of the current variant</param>
		public static System.UInt32 CreateOccurrenceOnCameraTarget(System.String name, System.UInt32 parent) {
			var ret = View_createOccurrenceOnCameraTarget(name, parent);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		#endregion

		#region OverrideMaterial

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_disableOverrideMaterial();
		/// <summary>
		/// 
		/// </summary>
		public static void DisableOverrideMaterial() {
			View_disableOverrideMaterial();
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_enableOverrideMaterial(System.UInt32 material);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="material">The material to enable as override material</param>
		public static void EnableOverrideMaterial(System.UInt32 material) {
			View_enableOverrideMaterial(material);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region Rendering Events

		private delegate void PickResultDelegate(System.IntPtr userdata, PickResult_c result, System.UInt32 session);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 View_addPickResultCallback(PickResultDelegate callback, System.IntPtr userdata);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_removePickResultCallback(System.UInt32 id);

		private static System.UInt32 addPickResultCallback(PickResultDelegate callback, System.IntPtr userdata) {
			return View_addPickResultCallback(callback, userdata);
		}

		private static void removePickResultCallback(System.UInt32 id) {
			View_removePickResultCallback(id);
		}

		public class PickResultTask : PiXYZAPI.PixyzCallbackTask
		{
			public struct Result
			{
				public PickResult result;
				public System.UInt32 session;

				public Result(PickResult result, System.UInt32 session)
				{
					this.result = result;
					this.session = session;
				}
			}

			public class Execution : BaseTaskExecution<Result>
			{
				public Execution(CancellationTokenSource cancelTokenSource = null) : base(cancelTokenSource)
				{
				}

				protected override void AttachCallback()
				{
					PickResultDelegate callback = Updated;

					_delegatePtr = GCHandle.Alloc(callback, GCHandleType.Pinned);
					_callbackId = addPickResultCallback(callback, IntPtr.Zero);

				}

				protected override void DetachCallback()
				{
					if (_callbackId == 0)
						return;

					removePickResultCallback(_callbackId);
					_callbackId = 0;
				}

				private void Updated(System.IntPtr userdata, PickResult_c result, System.UInt32 session)
				{
					try
					{
						_results.Enqueue(new Result(ConvertValue(ref result), session));

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
			public static Task<Result> WaitPickResult(CancellationTokenSource cancelSource = null)
			{
				return Task.Run(new Execution(cancelSource).RunOnce);
			}
		}
		private delegate void ViewSessionResizedDelegate(System.IntPtr userdata, System.UInt32 session);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 View_addViewSessionResizedCallback(ViewSessionResizedDelegate callback, System.IntPtr userdata);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_removeViewSessionResizedCallback(System.UInt32 id);

		private static System.UInt32 addViewSessionResizedCallback(ViewSessionResizedDelegate callback, System.IntPtr userdata) {
			return View_addViewSessionResizedCallback(callback, userdata);
		}

		private static void removeViewSessionResizedCallback(System.UInt32 id) {
			View_removeViewSessionResizedCallback(id);
		}

		public class ViewSessionResizedTask : PiXYZAPI.PixyzCallbackTask
		{
			public struct Result
			{
				public System.UInt32 session;

				public Result(System.UInt32 session)
				{
					this.session = session;
				}
			}

			public class Execution : BaseTaskExecution<Result>
			{
				public Execution(CancellationTokenSource cancelTokenSource = null) : base(cancelTokenSource)
				{
				}

				protected override void AttachCallback()
				{
					ViewSessionResizedDelegate callback = Updated;

					_delegatePtr = GCHandle.Alloc(callback, GCHandleType.Pinned);
					_callbackId = addViewSessionResizedCallback(callback, IntPtr.Zero);

				}

				protected override void DetachCallback()
				{
					if (_callbackId == 0)
						return;

					removeViewSessionResizedCallback(_callbackId);
					_callbackId = 0;
				}

				private void Updated(System.IntPtr userdata, System.UInt32 session)
				{
					try
					{
						_results.Enqueue(new Result(session));

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
			public static Task<Result> WaitViewSessionResized(CancellationTokenSource cancelSource = null)
			{
				return Task.Run(new Execution(cancelSource).RunOnce);
			}
		}
		#endregion

		#region View Session

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_addViewSessionRoot(System.UInt32 session, System.UInt32 root);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="root"></param>
		public static void AddViewSessionRoot(System.UInt32 session, System.UInt32 root) {
			View_addViewSessionRoot(session, root);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 View_isViewSessionExist(System.UInt32 session);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		public static System.Boolean IsViewSessionExist(System.UInt32 session) {
			var ret = View_isViewSessionExist(session);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Boolean)(0 != ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_pickOccurrences(System.UInt32 session, Int32 x, Int32 y);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public static void PickOccurrences(System.UInt32 session, System.Int32 x, System.Int32 y) {
			View_pickOccurrences(session, x, y);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_removeViewSessionRoot(System.UInt32 session, System.UInt32 root);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="root"></param>
		public static void RemoveViewSessionRoot(System.UInt32 session, System.UInt32 root) {
			View_removeViewSessionRoot(session, root);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_resizeViewSession(System.UInt32 session, Int32 width, Int32 height, ViewSessionTextureList_c textures);
		/// <summary>
		/// call this function when the texture has to be resized. the resize will take effect during the next render
		/// </summary>
		/// <param name="session"></param>
		/// <param name="width">Viewer width</param>
		/// <param name="height">Viewer height</param>
		/// <param name="textures">Textures that need to be resized</param>
		public static void ResizeViewSession(System.UInt32 session, System.Int32 width, System.Int32 height, ViewSessionTextureList textures) {
			var textures_c = new View.Native.ViewSessionTextureList_c();
			ConvertValue(textures, ref textures_c);
			View_resizeViewSession(session, width, height, textures_c);
			View.Native.NativeInterface.View_ViewSessionTextureList_free(ref textures_c);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_setViewSessionViewerProperty(System.UInt32 session, string name, string value);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public static void SetViewSessionViewerProperty(System.UInt32 session, System.String name, System.String value) {
			View_setViewSessionViewerProperty(session, name, value);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		private delegate void ViewSessionInitializedDelegate(System.IntPtr userdata, System.UInt32 session);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 View_addViewSessionInitializedCallback(ViewSessionInitializedDelegate callback, System.IntPtr userdata);
		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_removeViewSessionInitializedCallback(System.UInt32 id);

		private static System.UInt32 addViewSessionInitializedCallback(ViewSessionInitializedDelegate callback, System.IntPtr userdata) {
			return View_addViewSessionInitializedCallback(callback, userdata);
		}

		private static void removeViewSessionInitializedCallback(System.UInt32 id) {
			View_removeViewSessionInitializedCallback(id);
		}

		public class ViewSessionInitializedTask : PiXYZAPI.PixyzCallbackTask
		{
			public struct Result
			{
				public System.UInt32 session;

				public Result(System.UInt32 session)
				{
					this.session = session;
				}
			}

			public class Execution : BaseTaskExecution<Result>
			{
				public Execution(CancellationTokenSource cancelTokenSource = null) : base(cancelTokenSource)
				{
				}

				protected override void AttachCallback()
				{
					ViewSessionInitializedDelegate callback = Updated;

					_delegatePtr = GCHandle.Alloc(callback, GCHandleType.Pinned);
					_callbackId = addViewSessionInitializedCallback(callback, IntPtr.Zero);

				}

				protected override void DetachCallback()
				{
					if (_callbackId == 0)
						return;

					removeViewSessionInitializedCallback(_callbackId);
					_callbackId = 0;
				}

				private void Updated(System.IntPtr userdata, System.UInt32 session)
				{
					try
					{
						_results.Enqueue(new Result(session));

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
			public static Task<Result> WaitViewSessionInitialized(CancellationTokenSource cancelSource = null)
			{
				return Task.Run(new Execution(cancelSource).RunOnce);
			}
		}
		#endregion

		#region View Session Settings

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern DrawPrimitives_c View_getViewSessionDrawPrimitives(System.UInt32 session);
		/// <summary>
		/// Returns ViewSession viewer's draw properties
		/// </summary>
		/// <param name="session"></param>
		public static DrawPrimitives GetViewSessionDrawPrimitives(System.UInt32 session) {
			var ret = View_getViewSessionDrawPrimitives(session);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			View.Native.NativeInterface.View_DrawPrimitives_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_setViewSessionDrawPrimitives(System.UInt32 session, DrawPrimitives_c primitives);
		/// <summary>
		/// Set ViewSession viewer's draw properties
		/// </summary>
		/// <param name="session"></param>
		/// <param name="primitives"></param>
		public static void SetViewSessionDrawPrimitives(System.UInt32 session, DrawPrimitives primitives) {
			var primitives_c = new View.Native.DrawPrimitives_c();
			ConvertValue(primitives, ref primitives_c);
			View_setViewSessionDrawPrimitives(session, primitives_c);
			View.Native.NativeInterface.View_DrawPrimitives_free(ref primitives_c);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_updateExplodeView(System.UInt32 session, Int32 enabled, System.Double factor, Int32 X, Int32 Y, Int32 Z);
		/// <summary>
		/// Update explode settings for the viewer associated to the ViewSession
		/// </summary>
		/// <param name="session"></param>
		/// <param name="enabled"></param>
		/// <param name="factor"></param>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <param name="Z"></param>
		public static void UpdateExplodeView(System.UInt32 session, System.Boolean enabled, System.Double factor, System.Boolean X, System.Boolean Y, System.Boolean Z) {
			View_updateExplodeView(session, enabled ? 1 : 0, factor, X ? 1 : 0, Y ? 1 : 0, Z ? 1 : 0);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_updateViewSessionCuttingPlane(System.UInt32 session, Int32 enabled, Geom.Native.Matrix4_c matrix);
		/// <summary>
		/// Update cut plane for the viewer associated to the ViewSession
		/// </summary>
		/// <param name="session"></param>
		/// <param name="enabled"></param>
		/// <param name="matrix"></param>
		public static void UpdateViewSessionCuttingPlane(System.UInt32 session, System.Boolean enabled, Geom.Native.Matrix4 matrix) {
			var matrix_c = new Geom.Native.Matrix4_c();
			Geom.Native.NativeInterface.ConvertValue(matrix, ref matrix_c);
			View_updateViewSessionCuttingPlane(session, enabled ? 1 : 0, matrix_c);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref matrix_c);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region elements

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_showBReps(Int32 show, Int32 viewer);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="show">True to enable, False to disable</param>
		/// <param name="viewer"></param>
		public static void ShowBReps(System.Boolean show, System.Int32 viewer) {
			View_showBReps(show ? 1 : 0, viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_showEdges(Int32 show, Int32 viewer);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="show">True to enable, False to disable</param>
		/// <param name="viewer"></param>
		public static void ShowEdges(System.Boolean show, System.Int32 viewer) {
			View_showEdges(show ? 1 : 0, viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_showHidden(Int32 enable, Int32 viewer);
		/// <summary>
		/// Switch between show hidden and show visible mode
		/// </summary>
		/// <param name="enable">True to enable, False to disable</param>
		/// <param name="viewer"></param>
		public static void ShowHidden(System.Boolean enable, System.Int32 viewer) {
			View_showHidden(enable ? 1 : 0, viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_showLines(Int32 show, Int32 viewer);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="show">True to enable, False to disable</param>
		/// <param name="viewer"></param>
		public static void ShowLines(System.Boolean show, System.Int32 viewer) {
			View_showLines(show ? 1 : 0, viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_showPatchesBorders(Int32 show, Int32 viewer);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="show">True to enable, False to disable</param>
		/// <param name="viewer"></param>
		public static void ShowPatchesBorders(System.Boolean show, System.Int32 viewer) {
			View_showPatchesBorders(show ? 1 : 0, viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_showPoints(Int32 show, Int32 viewer);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="show">True to enable, False to disable</param>
		/// <param name="viewer"></param>
		public static void ShowPoints(System.Boolean show, System.Int32 viewer) {
			View_showPoints(show ? 1 : 0, viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_showPolygons(Int32 show, Int32 viewer);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="show">True to enable, False to disable</param>
		/// <param name="viewer"></param>
		public static void ShowPolygons(System.Boolean show, System.Int32 viewer) {
			View_showPolygons(show ? 1 : 0, viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_showSkeleton(Int32 show, Int32 viewer);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="show">True to enable, False to disable</param>
		/// <param name="viewer"></param>
		public static void ShowSkeleton(System.Boolean show, System.Int32 viewer) {
			View_showSkeleton(show ? 1 : 0, viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region interop

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_addSharedD3D11Texture(System.UInt32 interop, Int32 pxzTexture, System.IntPtr dxTexture);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="interop"></param>
		/// <param name="pxzTexture"></param>
		/// <param name="dxTexture"></param>
		public static void AddSharedD3D11Texture(System.UInt32 interop, System.Int32 pxzTexture, System.IntPtr dxTexture) {
			View_addSharedD3D11Texture(interop, pxzTexture, dxTexture);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 View_createD3D11Interop(Int32 viewer, System.IntPtr device);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="viewer"></param>
		/// <param name="device"></param>
		public static System.UInt32 CreateD3D11Interop(System.Int32 viewer, System.IntPtr device) {
			var ret = View_createD3D11Interop(viewer, device);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_deleteD3D11Interop(System.UInt32 interop);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="interop"></param>
		public static void DeleteD3D11Interop(System.UInt32 interop) {
			View_deleteD3D11Interop(interop);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 View_isD3D11InteropLocked(System.UInt32 interop);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="interop"></param>
		public static System.Boolean IsD3D11InteropLocked(System.UInt32 interop) {
			var ret = View_isD3D11InteropLocked(interop);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Boolean)(0 != ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_lockD3D11Interop(System.UInt32 interop);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="interop"></param>
		public static void LockD3D11Interop(System.UInt32 interop) {
			View_lockD3D11Interop(interop);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_removeSharedD3D11Texture(System.UInt32 interop, Int32 pxzTexture);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="interop"></param>
		/// <param name="pxzTexture"></param>
		public static void RemoveSharedD3D11Texture(System.UInt32 interop, System.Int32 pxzTexture) {
			View_removeSharedD3D11Texture(interop, pxzTexture);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_unlockD3D11Interop(System.UInt32 interop);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="interop"></param>
		public static void UnlockD3D11Interop(System.UInt32 interop) {
			View_unlockD3D11Interop(interop);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region offscreen

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_addRoot(System.UInt32 root, Int32 viewer);
		/// <summary>
		/// Add a viewer root
		/// </summary>
		/// <param name="root">Occurrence to add</param>
		/// <param name="viewer">Viewer to modify</param>
		public static void AddRoot(System.UInt32 root, System.Int32 viewer) {
			View_addRoot(root, viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 View_createViewer(Int32 width, Int32 height, Int32 handleSelection);
		/// <summary>
		/// Create a new viewer
		/// </summary>
		/// <param name="width">Width of the viewer framebuffer</param>
		/// <param name="height">Height of the viewer framebuffer</param>
		/// <param name="handleSelection">Does the viewer handle selection of occurrences</param>
		public static System.Int32 CreateViewer(System.Int32 width, System.Int32 height, System.Boolean handleSelection) {
			var ret = View_createViewer(width, height, handleSelection ? 1 : 0);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Int32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_destroyViewer(Int32 viewer);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="viewer">Viewer to destroy</param>
		public static void DestroyViewer(System.Int32 viewer) {
			View_destroyViewer(viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_fitView(Int32 viewer);
		/// <summary>
		/// Fit scene to viewer
		/// </summary>
		/// <param name="viewer">Viewer to modify</param>
		public static void FitView(System.Int32 viewer) {
			View_fitView(viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 View_getColorTextureHandle(Int32 viewer, Int32 index);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="viewer"></param>
		/// <param name="index"></param>
		public static System.Int32 GetColorTextureHandle(System.Int32 viewer, System.Int32 index) {
			var ret = View_getColorTextureHandle(viewer, index);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Int32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr View_getCuttingPlaneProperty(string propertyName, Int32 viewer);
		/// <summary>
		/// Get a viewer property value
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="viewer"></param>
		public static System.String GetCuttingPlaneProperty(System.String propertyName, System.Int32 viewer) {
			var ret = View_getCuttingPlaneProperty(propertyName, viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 View_getDepthTextureHandle(Int32 viewer);
		/// <summary>
		/// Get depth texture handle
		/// </summary>
		/// <param name="viewer">Targeted viewer</param>
		public static System.Int32 GetDepthTextureHandle(System.Int32 viewer) {
			var ret = View_getDepthTextureHandle(viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Int32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr View_getExplodeViewProperty(string propertyName, Int32 viewer);
		/// <summary>
		/// Get a viewer property value
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="viewer"></param>
		public static System.String GetExplodeViewProperty(System.String propertyName, System.Int32 viewer) {
			var ret = View_getExplodeViewProperty(propertyName, viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 View_getFXAATextureHandle(Int32 viewer);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="viewer"></param>
		public static System.Int32 GetFXAATextureHandle(System.Int32 viewer) {
			var ret = View_getFXAATextureHandle(viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Int32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Int32 View_getFinalTextureHandle(Int32 viewer);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="viewer"></param>
		public static System.Int32 GetFinalTextureHandle(System.Int32 viewer) {
			var ret = View_getFinalTextureHandle(viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.Int32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern DrawPrimitives_c View_getViewerDrawPrimitives(Int32 viewer);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="viewer"></param>
		public static DrawPrimitives GetViewerDrawPrimitives(System.Int32 viewer) {
			var ret = View_getViewerDrawPrimitives(viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = ConvertValue(ref ret);
			View.Native.NativeInterface.View_DrawPrimitives_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getViewerMatricesReturn_c View_getViewerMatrices(Int32 viewer);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="viewer"></param>
		public static View.Native.getViewerMatricesReturn GetViewerMatrices(System.Int32 viewer) {
			var ret = View_getViewerMatrices(viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			View.Native.getViewerMatricesReturn retStruct = new View.Native.getViewerMatricesReturn();
			retStruct.views = ConvertValue(ref ret.views);
			Geom.Native.NativeInterface.Geom_Matrix4List_free(ref ret.views);
			retStruct.projs = ConvertValue(ref ret.projs);
			Geom.Native.NativeInterface.Geom_Matrix4List_free(ref ret.projs);
			retStruct.clipping = ConvertValue(ref ret.clipping);
			Geom.Native.NativeInterface.Geom_Point2_free(ref ret.clipping);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr View_getViewerProperty(string propertyName, Int32 viewer);
		/// <summary>
		/// Get a viewer property value
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="viewer"></param>
		public static System.String GetViewerProperty(System.String propertyName, System.Int32 viewer) {
			var ret = View_getViewerProperty(propertyName, viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern getViewerSizeReturn_c View_getViewerSize(Int32 viewer);
		/// <summary>
		/// Retrieve the viewport size of a viewer
		/// </summary>
		/// <param name="viewer"></param>
		public static View.Native.getViewerSizeReturn GetViewerSize(System.Int32 viewer) {
			var ret = View_getViewerSize(viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			View.Native.getViewerSizeReturn retStruct = new View.Native.getViewerSizeReturn();
			retStruct.width = (System.Int32)ret.width;
			retStruct.height = (System.Int32)ret.height;
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Core.Native.StringList_c View_listViewerProperties(Int32 viewer);
		/// <summary>
		/// Get the list of viewer properties
		/// </summary>
		/// <param name="viewer"></param>
		public static Core.Native.StringList ListViewerProperties(System.Int32 viewer) {
			var ret = View_listViewerProperties(viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Core.Native.NativeInterface.ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_StringList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_makeCurrent(Int32 viewer);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="viewer">Targeted viewer</param>
		public static void MakeCurrent(System.Int32 viewer) {
			View_makeCurrent(viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern pickReturn_c View_pick(Int32 x, Int32 y, Int32 viewer);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="viewer"></param>
		public static View.Native.pickReturn Pick(System.Int32 x, System.Int32 y, System.Int32 viewer) {
			var ret = View_pick(x, y, viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			View.Native.pickReturn retStruct = new View.Native.pickReturn();
			retStruct.occurrence = (System.UInt32)ret.occurrence;
			retStruct.position = ConvertValue(ref ret.position);
			Geom.Native.NativeInterface.Geom_Point3_free(ref ret.position);
			return retStruct;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_refreshViewer(Int32 viewer, Int32 frameCount, Int32 forceUpdate);
		/// <summary>
		/// Refresh the viewer
		/// </summary>
		/// <param name="viewer">Viewer to refresh</param>
		/// <param name="frameCount">Number of frames to render</param>
		/// <param name="forceUpdate">Force the viewer to update pending modification on the geometry. By default this is disabled while running process</param>
		public static void RefreshViewer(System.Int32 viewer, System.Int32 frameCount, System.Boolean forceUpdate) {
			View_refreshViewer(viewer, frameCount, forceUpdate ? 1 : 0);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_removeRoot(System.UInt32 root, Int32 viewer);
		/// <summary>
		/// Remove a viewer root
		/// </summary>
		/// <param name="root">Occurrence to remove</param>
		/// <param name="viewer">Viewer to modify</param>
		public static void RemoveRoot(System.UInt32 root, System.Int32 viewer) {
			View_removeRoot(root, viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_resizeViewer(Int32 width, Int32 height, Int32 viewer);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="width">Width of the viewer framebuffer</param>
		/// <param name="height">Height of the viewer framebuffer</param>
		/// <param name="viewer"></param>
		public static void ResizeViewer(System.Int32 width, System.Int32 height, System.Int32 viewer) {
			View_resizeViewer(width, height, viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_setCuttingPlaneProperty(string propertyName, string propertyValue, Int32 viewer);
		/// <summary>
		/// Set a viewer property value
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="propertyValue"></param>
		/// <param name="viewer"></param>
		public static void SetCuttingPlaneProperty(System.String propertyName, System.String propertyValue, System.Int32 viewer) {
			View_setCuttingPlaneProperty(propertyName, propertyValue, viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_setExplodeViewProperty(string propertyName, string propertyValue, Int32 viewer);
		/// <summary>
		/// Set a viewer property value
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="propertyValue"></param>
		/// <param name="viewer"></param>
		public static void SetExplodeViewProperty(System.String propertyName, System.String propertyValue, System.Int32 viewer) {
			View_setExplodeViewProperty(propertyName, propertyValue, viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_setViewerDrawPrimitives(DrawPrimitives_c primitivies, Int32 viewer);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="primitivies"></param>
		/// <param name="viewer"></param>
		public static void SetViewerDrawPrimitives(DrawPrimitives primitivies, System.Int32 viewer) {
			var primitivies_c = new View.Native.DrawPrimitives_c();
			ConvertValue(primitivies, ref primitivies_c);
			View_setViewerDrawPrimitives(primitivies_c, viewer);
			View.Native.NativeInterface.View_DrawPrimitives_free(ref primitivies_c);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_setViewerMatrices(Geom.Native.Matrix4List_c views, Geom.Native.Matrix4List_c projs, Geom.Native.Point2_c clipping, Int32 viewer);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="views"></param>
		/// <param name="projs"></param>
		/// <param name="clipping"></param>
		/// <param name="viewer"></param>
		public static void SetViewerMatrices(Geom.Native.Matrix4List views, Geom.Native.Matrix4List projs, Geom.Native.Point2 clipping, System.Int32 viewer) {
			var views_c = new Geom.Native.Matrix4List_c();
			Geom.Native.NativeInterface.ConvertValue(views, ref views_c);
			var projs_c = new Geom.Native.Matrix4List_c();
			Geom.Native.NativeInterface.ConvertValue(projs, ref projs_c);
			var clipping_c = new Geom.Native.Point2_c();
			Geom.Native.NativeInterface.ConvertValue(clipping, ref clipping_c);
			View_setViewerMatrices(views_c, projs_c, clipping_c, viewer);
			Geom.Native.NativeInterface.Geom_Matrix4List_free(ref views_c);
			Geom.Native.NativeInterface.Geom_Matrix4List_free(ref projs_c);
			Geom.Native.NativeInterface.Geom_Point2_free(ref clipping_c);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_setViewerProperty(string propertyName, string propertyValue, Int32 viewer);
		/// <summary>
		/// Set a viewer property value
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="propertyValue"></param>
		/// <param name="viewer"></param>
		public static void SetViewerProperty(System.String propertyName, System.String propertyValue, System.Int32 viewer) {
			View_setViewerProperty(propertyName, propertyValue, viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_takeScreenshot(string filename, Int32 viewer);
		/// <summary>
		/// Take a screenshot
		/// </summary>
		/// <param name="filename">File path to save at</param>
		/// <param name="viewer">Targeted viewer</param>
		public static void TakeScreenshot(System.String filename, System.Int32 viewer) {
			View_takeScreenshot(filename, viewer);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		#region viewer

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Scene.Native.Camera_c View_getCurrentCamera();
		/// <summary>
		/// Returns the information of the current camera (used in the main viewer)
		/// </summary>
		public static Scene.Native.Camera GetCurrentCamera() {
			var ret = View_getCurrentCamera();
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Scene.Native.NativeInterface.ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_Camera_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr View_getMainViewerCuttingPlaneProperty(string propertyName);
		/// <summary>
		/// Get a viewer property value
		/// </summary>
		/// <param name="propertyName">Property name</param>
		public static System.String GetMainViewerCuttingPlaneProperty(System.String propertyName) {
			var ret = View_getMainViewerCuttingPlaneProperty(propertyName);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr View_getMainViewerExplodeViewProperty(string propertyName);
		/// <summary>
		/// Get a viewer property value
		/// </summary>
		/// <param name="propertyName">Property name</param>
		public static System.String GetMainViewerExplodeViewProperty(System.String propertyName) {
			var ret = View_getMainViewerExplodeViewProperty(propertyName);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr View_getMainViewerProperty(string propertyName);
		/// <summary>
		/// Get a viewer property value
		/// </summary>
		/// <param name="propertyName">Property name</param>
		public static System.String GetMainViewerProperty(System.String propertyName) {
			var ret = View_getMainViewerProperty(propertyName);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Geom.Native.Matrix4_c View_getProjectionMatrix();
		/// <summary>
		/// Returns the main viewer current view matrix
		/// </summary>
		public static Geom.Native.Matrix4 GetProjectionMatrix() {
			var ret = View_getProjectionMatrix();
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Geom.Native.NativeInterface.ConvertValue(ref ret);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Geom.Native.Matrix4_c View_getViewMatrix();
		/// <summary>
		/// Returns the main viewer current view matrix
		/// </summary>
		public static Geom.Native.Matrix4 GetViewMatrix() {
			var ret = View_getViewMatrix();
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Geom.Native.NativeInterface.ConvertValue(ref ret);
			Geom.Native.NativeInterface.Geom_Matrix4_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Core.Native.StringList_c View_listMainViewerCuttingPlaneProperties();
		/// <summary>
		/// Get the list of viewer properties
		/// </summary>
		public static Core.Native.StringList ListMainViewerCuttingPlaneProperties() {
			var ret = View_listMainViewerCuttingPlaneProperties();
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Core.Native.NativeInterface.ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_StringList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Core.Native.StringList_c View_listMainViewerExplodeViewProperty();
		/// <summary>
		/// Get the list of viewer properties
		/// </summary>
		public static Core.Native.StringList ListMainViewerExplodeViewProperty() {
			var ret = View_listMainViewerExplodeViewProperty();
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Core.Native.NativeInterface.ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_StringList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Core.Native.StringList_c View_listMainViewerProperties();
		/// <summary>
		/// Get the list of viewer properties
		/// </summary>
		public static Core.Native.StringList ListMainViewerProperties() {
			var ret = View_listMainViewerProperties();
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Core.Native.NativeInterface.ConvertValue(ref ret);
			Core.Native.NativeInterface.Core_StringList_free(ref ret);
			return convRet;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_pauseViewer();
		/// <summary>
		/// Pause the viewer
		/// </summary>
		public static void PauseViewer() {
			View_pauseViewer();
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_resumeViewer();
		/// <summary>
		/// Pause the viewer
		/// </summary>
		public static void ResumeViewer() {
			View_resumeViewer();
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_setMainViewerCuttingPlaneProperty(string propertyName, string propertyValue);
		/// <summary>
		/// Set a viewer property value
		/// </summary>
		/// <param name="propertyName">Property name</param>
		/// <param name="propertyValue">Property value</param>
		public static void SetMainViewerCuttingPlaneProperty(System.String propertyName, System.String propertyValue) {
			View_setMainViewerCuttingPlaneProperty(propertyName, propertyValue);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_setMainViewerExplodeViewProperty(string propertyName, string propertyValue);
		/// <summary>
		/// Set a viewer property value
		/// </summary>
		/// <param name="propertyName">Property name</param>
		/// <param name="propertyValue">Property value</param>
		public static void SetMainViewerExplodeViewProperty(System.String propertyName, System.String propertyValue) {
			View_setMainViewerExplodeViewProperty(propertyName, propertyValue);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern IntPtr View_setMainViewerProperty(string propertyName, string propertyValue);
		/// <summary>
		/// Set a viewer property value
		/// </summary>
		/// <param name="propertyName">Property name</param>
		/// <param name="propertyValue">Property value</param>
		public static System.String SetMainViewerProperty(System.String propertyName, System.String propertyValue) {
			var ret = View_setMainViewerProperty(propertyName, propertyValue);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return ConvertValue(ret);
		}

		#endregion

		#region visibility

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern System.UInt32 View_beginVisibilitySession(Int32 width, Int32 height);
		/// <summary>
		/// Start a new visibility session
		/// </summary>
		/// <param name="width">Width of the renderer used for the visibility session</param>
		/// <param name="height">Width of the renderer used for the visibility session</param>
		public static System.UInt32 BeginVisibilitySession(System.Int32 width, System.Int32 height) {
			var ret = View_beginVisibilitySession(width, height);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			return (System.UInt32)ret;
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_drawCappingPlane();
		/// <summary>
		/// Create a mesh capping the cutting plane and display it
		/// </summary>
		public static void DrawCappingPlane() {
			View_drawCappingPlane();
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_endVisibilitySession(System.UInt32 visibilitySession);
		/// <summary>
		/// Terminate a visibility session
		/// </summary>
		/// <param name="visibilitySession">Identifier of the visibility session</param>
		public static void EndVisibilitySession(System.UInt32 visibilitySession) {
			View_endVisibilitySession(visibilitySession);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_setVisibilitySessionCamera(System.UInt32 visibilitySession, Geom.Native.Point3_c position, Geom.Native.Point3_c view, Geom.Native.Point3_c up, System.Double fovx);
		/// <summary>
		/// Place the camera of a given visibility session
		/// </summary>
		/// <param name="visibilitySession">Identifier of the visibility session</param>
		/// <param name="position">Position of the camera</param>
		/// <param name="view">View direction of the camera</param>
		/// <param name="up">Up direction of the camera</param>
		/// <param name="fovx">Horizontal field of view (in degree)</param>
		public static void SetVisibilitySessionCamera(System.UInt32 visibilitySession, Geom.Native.Point3 position, Geom.Native.Point3 view, Geom.Native.Point3 up, System.Double fovx) {
			var position_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(position, ref position_c);
			var view_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(view, ref view_c);
			var up_c = new Geom.Native.Point3_c();
			Geom.Native.NativeInterface.ConvertValue(up, ref up_c);
			View_setVisibilitySessionCamera(visibilitySession, position_c, view_c, up_c, fovx);
			Geom.Native.NativeInterface.Geom_Point3_free(ref position_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref view_c);
			Geom.Native.NativeInterface.Geom_Point3_free(ref up_c);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern void View_test();
		/// <summary>
		/// 
		/// </summary>
		public static void Test() {
			View_test();
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		[DllImport(PiXYZAPI.PiXYZAPI_dll)]
		private static extern Scene.Native.OccurrenceList_c View_visibilityShoot(System.UInt32 visibilitySession, Int32 parts, Int32 patches, Int32 polygons, Int32 countOnce);
		/// <summary>
		/// Render one frame of the visibility session
		/// </summary>
		/// <param name="visibilitySession">Identifier of the visibility session</param>
		/// <param name="parts">If false, optimize when parts seen are not wanted</param>
		/// <param name="patches">If false, optimize when patches seen are not wanted</param>
		/// <param name="polygons">If false, optimize when polygons seen are not wanted</param>
		/// <param name="countOnce">Optimize when it is not needed to count the numbers of pixels seen during the session</param>
		public static Scene.Native.OccurrenceList VisibilityShoot(System.UInt32 visibilitySession, System.Boolean parts, System.Boolean patches, System.Boolean polygons, System.Boolean countOnce) {
			var ret = View_visibilityShoot(visibilitySession, parts ? 1 : 0, patches ? 1 : 0, polygons ? 1 : 0, countOnce ? 1 : 0);
			System.String err = ConvertValue(View_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
			var convRet = Scene.Native.NativeInterface.ConvertValue(ref ret);
			Scene.Native.NativeInterface.Scene_OccurrenceList_free(ref ret);
			return convRet;
		}

		#endregion

	}
}
