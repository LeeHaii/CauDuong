using System;
using System.Collections.Generic;
using System.IO;
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
        var paths = SelectIfcFiles();

        foreach (var path in paths)
        {
            LoadIFC(path);
        }
    }

    public IReadOnlyList<string> SelectIfcFiles()
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        return WindowsFileDialog.OpenFiles(
            "IFC files (*.ifc)\0*.ifc\0All files (*.*)\0*.*\0",
            "Chọn mô hình IFC",
            true);
#else
        SetStatus("Runtime IFC import is currently supported on Windows only.");
        return Array.Empty<string>();
#endif
    }

    public string SelectImageFile()
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        var paths = WindowsFileDialog.OpenFiles(
            "Image files (*.png;*.jpg;*.jpeg)\0*.png;*.jpg;*.jpeg\0" +
            "All files (*.*)\0*.*\0",
            "Chọn ảnh hiện trường",
            false);
        return paths.Count > 0 ? paths[0] : string.Empty;
#else
        SetStatus("Image selection is currently supported on Windows only.");
        return string.Empty;
#endif
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

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    private static class WindowsFileDialog
    {
        private const int MaxPathCharacters = 4096;
        private const int FileMustExist = 0x00001000;
        private const int PathMustExist = 0x00000800;
        private const int ExplorerStyle = 0x00080000;
        private const int DoNotChangeDirectory = 0x00000008;
        private const int AllowMultiSelect = 0x00000200;

        [DllImport(
            "comdlg32.dll",
            CharSet = CharSet.Unicode,
            EntryPoint = "GetOpenFileNameW",
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetOpenFileName(ref OpenFileNameData data);

        public static IReadOnlyList<string> OpenFiles(
            string filterText,
            string titleText,
            bool allowMultiSelect)
        {
            var fileBuffer = Marshal.AllocHGlobal(MaxPathCharacters * sizeof(char));
            var filter = Marshal.StringToHGlobalUni(filterText);
            var title = Marshal.StringToHGlobalUni(titleText);

            try
            {
                // The native dialog may only write a single trailing null for one
                // selected file. Clear the complete buffer first so the parser is
                // guaranteed to encounter a double-null terminator instead of stale
                // unmanaged memory.
                Marshal.Copy(
                    new byte[MaxPathCharacters * sizeof(char)],
                    0,
                    fileBuffer,
                    MaxPathCharacters * sizeof(char));

                var data = new OpenFileNameData
                {
                    structSize = Marshal.SizeOf<OpenFileNameData>(),
                    filter = filter,
                    filterIndex = 1,
                    file = fileBuffer,
                    maxFile = MaxPathCharacters,
                    title = title,
                    flags = FileMustExist |
                            PathMustExist |
                            ExplorerStyle |
                            DoNotChangeDirectory |
                            (allowMultiSelect ? AllowMultiSelect : 0)
                };

                if (!GetOpenFileName(ref data))
                {
                    return Array.Empty<string>();
                }

                var parts = ReadMultiString(fileBuffer);
                if (parts.Length == 0)
                {
                    return Array.Empty<string>();
                }

                // GetOpenFileName only guarantees a second null-delimited value
                // when multi-select is enabled. Reading beyond the first value for
                // a single selection can pick up uninitialized buffer contents and
                // feed illegal characters into Path.Combine.
                if (!allowMultiSelect || parts.Length == 1)
                {
                    return new[] { parts[0] };
                }

                var directory = parts[0];
                var paths = new string[parts.Length - 1];
                for (var index = 1; index < parts.Length; index++)
                {
                    paths[index - 1] = Path.Combine(directory, parts[index]);
                }

                return paths;
            }
            finally
            {
                Marshal.FreeHGlobal(title);
                Marshal.FreeHGlobal(filter);
                Marshal.FreeHGlobal(fileBuffer);
            }
        }

        private static string[] ReadMultiString(IntPtr buffer)
        {
            var values = new List<string>();
            var current = new System.Text.StringBuilder();

            for (var index = 0; index < MaxPathCharacters; index++)
            {
                var character = (char)Marshal.ReadInt16(
                    buffer,
                    index * sizeof(char));
                if (character != '\0')
                {
                    current.Append(character);
                    continue;
                }

                if (current.Length == 0)
                {
                    break;
                }

                values.Add(current.ToString());
                current.Clear();
            }

            return values.ToArray();
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
