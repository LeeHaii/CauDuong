using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class RuntimeSaveFileDialog
{
    private const int MaxPathCharacters = 4096;
    private const int OverwritePrompt = 0x00000002;
    private const int DoNotChangeDirectory = 0x00000008;
    private const int PathMustExist = 0x00000800;
    private const int ExplorerStyle = 0x00080000;

    public static bool TryGetSavePath(
        string title,
        string defaultFileName,
        string extension,
        string filterDescription,
        out string path)
    {
#if UNITY_EDITOR
        path = UnityEditor.EditorUtility.SaveFilePanel(
            title,
            string.Empty,
            defaultFileName,
            extension);
        return !string.IsNullOrWhiteSpace(path);
#elif UNITY_STANDALONE_WIN
        return TryGetWindowsSavePath(
            title,
            defaultFileName,
            extension,
            filterDescription,
            out path);
#else
        path = System.IO.Path.Combine(Application.persistentDataPath, defaultFileName);
        return true;
#endif
    }

#if UNITY_STANDALONE_WIN
    [DllImport(
        "comdlg32.dll",
        CharSet = CharSet.Unicode,
        EntryPoint = "GetSaveFileNameW",
        SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetSaveFileName(ref SaveFileNameData data);

    private static bool TryGetWindowsSavePath(
        string title,
        string defaultFileName,
        string extension,
        string filterDescription,
        out string path)
    {
        path = string.Empty;
        var fileBuffer = Marshal.AllocHGlobal(MaxPathCharacters * sizeof(char));
        var filter = Marshal.StringToHGlobalUni(
            $"{filterDescription} (*.{extension})\0*.{extension}\0" +
            "All files (*.*)\0*.*\0\0");
        var titlePointer = Marshal.StringToHGlobalUni(title);
        var extensionPointer = Marshal.StringToHGlobalUni(extension);

        try
        {
            for (var index = 0; index < MaxPathCharacters; index++)
            {
                Marshal.WriteInt16(fileBuffer, index * sizeof(char), 0);
            }

            var initialCharacters = defaultFileName.ToCharArray();
            Marshal.Copy(
                initialCharacters,
                0,
                fileBuffer,
                Math.Min(initialCharacters.Length, MaxPathCharacters - 1));

            var data = new SaveFileNameData
            {
                structSize = Marshal.SizeOf<SaveFileNameData>(),
                filter = filter,
                filterIndex = 1,
                file = fileBuffer,
                maxFile = MaxPathCharacters,
                title = titlePointer,
                flags = OverwritePrompt |
                        DoNotChangeDirectory |
                        PathMustExist |
                        ExplorerStyle,
                defaultExtension = extensionPointer
            };

            if (!GetSaveFileName(ref data))
            {
                return false;
            }

            path = Marshal.PtrToStringUni(fileBuffer) ?? string.Empty;
            return !string.IsNullOrWhiteSpace(path);
        }
        finally
        {
            Marshal.FreeHGlobal(extensionPointer);
            Marshal.FreeHGlobal(titlePointer);
            Marshal.FreeHGlobal(filter);
            Marshal.FreeHGlobal(fileBuffer);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct SaveFileNameData
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
#endif
}
