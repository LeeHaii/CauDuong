using System;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(XbimIfcLoader))]
public sealed class RuntimeIfcLoader : MonoBehaviour
{
    [SerializeField] private XbimIfcLoader loader;
    [SerializeField] private TextMeshProUGUI statusText;

    private void Awake()
    {
        if (loader == null)
        {
            loader = GetComponent<XbimIfcLoader>();
        }
    }

    private void OnEnable()
    {
        if (loader != null)
        {
            loader.StatusChanged += SetStatus;
        }
    }

    private void OnDisable()
    {
        if (loader != null)
        {
            loader.StatusChanged -= SetStatus;
        }
    }

    public void BrowseIFC()
    {
#if UNITY_EDITOR_WIN
        var path = UnityEditor.EditorUtility.OpenFilePanel("Open IFC Model", string.Empty, "ifc");
#elif UNITY_STANDALONE_WIN
        var path = WindowsFileDialog.OpenIfcFile();
#else
        SetStatus("Runtime IFC import is currently supported on Windows only.");
        return;
#endif

        if (!string.IsNullOrWhiteSpace(path))
        {
            LoadIFC(path);
        }
    }

    public void LoadIFC(string path)
    {
        if (loader == null)
        {
            SetStatus("XbimIfcLoader is not assigned.");
            return;
        }

        loader.LoadIFC(path);
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }

        Debug.Log(message);
    }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    private static class WindowsFileDialog
    {
        private const int MaxPathCharacters = 4096;
        private const int FileMustExist = 0x00001000;
        private const int PathMustExist = 0x00000800;
        private const int ExplorerStyle = 0x00080000;
        private const int DoNotChangeDirectory = 0x00000008;

        [DllImport(
            "comdlg32.dll",
            CharSet = CharSet.Unicode,
            EntryPoint = "GetOpenFileNameW",
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetOpenFileName(ref OpenFileNameData data);

        public static string OpenIfcFile()
        {
            var fileBuffer = Marshal.AllocHGlobal(MaxPathCharacters * sizeof(char));
            var filter = Marshal.StringToHGlobalUni(
                "IFC files (*.ifc)\0*.ifc\0All files (*.*)\0*.*\0");
            var title = Marshal.StringToHGlobalUni("Open IFC Model");

            try
            {
                Marshal.WriteInt16(fileBuffer, 0);

                var data = new OpenFileNameData
                {
                    structSize = Marshal.SizeOf<OpenFileNameData>(),
                    filter = filter,
                    filterIndex = 1,
                    file = fileBuffer,
                    maxFile = MaxPathCharacters,
                    title = title,
                    flags = FileMustExist | PathMustExist | ExplorerStyle | DoNotChangeDirectory
                };

                return GetOpenFileName(ref data)
                    ? Marshal.PtrToStringUni(fileBuffer)
                    : null;
            }
            finally
            {
                Marshal.FreeHGlobal(title);
                Marshal.FreeHGlobal(filter);
                Marshal.FreeHGlobal(fileBuffer);
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct OpenFileNameData
        {
            public int structSize;
            public IntPtr owner;
            public IntPtr instance;
            public IntPtr filter;
            public IntPtr customFilter;
            public int maxCustomFilter;
            public int filterIndex;
            public IntPtr file;
            public int maxFile;
            public IntPtr fileTitle;
            public int maxFileTitle;
            public IntPtr initialDirectory;
            public IntPtr title;
            public int flags;
            public short fileOffset;
            public short fileExtension;
            public IntPtr defaultExtension;
            public IntPtr customData;
            public IntPtr hook;
            public IntPtr templateName;
            public IntPtr reserved;
            public int reservedValue;
            public int flagsExtended;
        }
    }
#endif
}
