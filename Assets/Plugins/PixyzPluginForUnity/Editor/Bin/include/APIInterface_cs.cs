#pragma warning disable CA2101

using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Security;
using System.Collections.Concurrent;

namespace Pixyz.API.Native {

	public static partial class NativeInterface {

#if !PXZ_CUSTOM_DLL_PATH
	#if PXZ_OS_LINUX
		internal const string PiXYZAPI_dll = "libPiXYZAPI";
		internal const string memcpy_dll = "libc.so.6";
	#elif PXZ_OS_WIN32
		internal const string PiXYZAPI_dll = "PiXYZAPI";
		internal const string memcpy_dll = "msvcrt.dll";
	#else
		internal const string PiXYZAPI_dll = "PiXYZAPI";
		internal const string memcpy_dll = "msvcrt.dll_undefined_platform";
	#endif

#endif

		[DllImport(memcpy_dll, EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false), SuppressUnmanagedCodeSecurity]
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

		[DllImport(PiXYZAPI_dll)]
		private static extern IntPtr API_getLastError();

		public static string GetLastError()
		{
			return ConvertValue(API_getLastError());
		}

		#region 

		[DllImport(PiXYZAPI_dll)]
		private static extern void API_initialize(string productKey, string validationKey, string license);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="productKey"></param>
		/// <param name="validationKey"></param>
		/// <param name="license"></param>
		public static void Initialize(System.String productKey, System.String validationKey, System.String license) {
			API_initialize(productKey, validationKey, license);
			System.String err = ConvertValue(API_getLastError());
			if(!System.String.IsNullOrEmpty(err))
				throw new Exception(err);
		}

		#endregion

		public class PixyzCallbackTask
		{
			public abstract class BaseTaskExecution<T> : IDisposable where T : struct
			{
				protected ConcurrentQueue<T> _results;
				protected volatile SemaphoreSlim _isCompleted = null;
				protected volatile SemaphoreSlim _isUpdated = null;
				protected volatile uint _callbackId;
				protected volatile bool _isDisposed = false;
				protected volatile bool _isContiniuous = false;
				protected GCHandle _delegatePtr;

				private volatile CancellationTokenSource _cancelTokenSource = null;
				private CancellationToken _cancelToken;

				public BaseTaskExecution(CancellationTokenSource cancelTokenSource = null)
				{
					_isCompleted = new SemaphoreSlim(0, 1);
					_isUpdated = new SemaphoreSlim(0);
					_results = new ConcurrentQueue<T>();
					if (_cancelTokenSource != null)
					{
						_cancelToken = cancelTokenSource.Token;
						_cancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancelTokenSource.Token);
					}
					else
					{
						_cancelTokenSource = new CancellationTokenSource();
						_cancelToken = _cancelTokenSource.Token;
					}
				}

				protected abstract void AttachCallback();

				protected abstract void DetachCallback();

				protected void Run(bool continiuous)
				{
					if (_isDisposed)
						throw new Exception("This TaskExecution was disposed, you can't run it.");

					_isContiniuous = continiuous;
					try
					{
						AttachCallback();
						_isCompleted.Wait(_cancelToken);

						Dispose();
					}
					catch (OperationCanceledException)
					{
						Dispose();
					}
				}

				public Task RunContiniuous()
				{
					return Task.Run(() => { Run(true); });
				}

				public Task<T> RunOnce()
				{
					if (_isDisposed)
						throw new Exception("This TaskExecution was disposed, you can't run it.");

					try
					{
						return Task.Run(() =>
						{
							Run(false);
							T result = new T();
							_results.TryDequeue(out result);
							return result;
						});
					}
					catch (OperationCanceledException)
					{
						Dispose();
						return Task.FromResult(new T());
					}
				}

				public void Stop()
				{
					if (!_cancelTokenSource.IsCancellationRequested)
						_cancelTokenSource.Cancel();
				}

				public async Task<T> WaitNewValue()
				{
					T result = new T();
					do
					{
						bool success = _results.TryDequeue(out result);
						if(success)
							return result;
						else
							await _isUpdated.WaitAsync(_cancelToken);

					} while(!_results.IsEmpty && !_cancelToken.IsCancellationRequested);

					return result;
				}

				public void Dispose()
				{
					if (_isDisposed)
						return;

					_isDisposed = true;
					Stop();
					DetachCallback();

					if (_delegatePtr.IsAllocated)
						_delegatePtr.Free();

					_isCompleted.Dispose();
					_isUpdated.Dispose();
					_cancelTokenSource.Dispose();
					_isCompleted = null;
					_isUpdated = null;
				}
			}
		}
	}
}
