using System.IO;
using UnityEditor;
using UnityEngine;

namespace Pixyz.API.Native
{
    [InitializeOnLoad]
    public static partial class NativeInterface
    {

#if UNITY_EDITOR && PXZ_CUSTOM_DLL_PATH
#if UNITY_EDITOR_LINUX
        internal const string PiXYZAPI_dll = "libPiXYZAPI.so";
        internal const string memcpy_dll = "libc.so.6";
#elif UNITY_EDITOR_WIN
        internal const string PiXYZAPI_dll = "PiXYZAPI";
        internal const string memcpy_dll = "msvcrt.dll";
#endif
#elif PXZ_CUSTOM_DLL_PATH
#if UNITY_STANDALONE_LINUX
        internal const string PiXYZAPI_dll = "undefined";
        internal const string memcpy_dll = "undefined";
#elif UNITY_STANDALONE_WIN
        internal const string PiXYZAPI_dll = "undefined";
        internal const string memcpy_dll = "undefined";
#endif
#endif

        static NativeInterface()
        {
            if (Application.isBatchMode || !Application.isEditor || !UnityEditorInternal.InternalEditorUtility.isHumanControllingUs)
                return;

            try
            {
                Initialize("PiXYZ4Unity", "2048f7530f418970e73ccca54ecb4675213b25113f6c5a6456451e12868de5d5e0f5def5", "");
            }
            catch (System.Exception e)
            {
                try
                {
                    if (!Core.Native.NativeInterface.CheckLicense())
                    {
                        Debug.LogWarning("The Pixyz Plugin for Unity requires a valid License.\nPlease install yours via the License Manager or visit www.pixyz-software.com to get one");
                    }
                    else
                        Debug.LogError("Exception while initializing Pixyz plugin 1 : " + e.Message);
                    return;
                }
                catch
                {
                    Debug.LogError("Exception while initializing Pixyz plugin 2 : " + e.Message);
                }
            }
        }
    }

}

