using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pixyz.Plugin4Unity
{
    /// <summary>
    /// Static class for formats related information.
    /// </summary>
    public static class Formats
    {
        public static readonly HashSet<string> UnitySupportedFormats = new HashSet<string> { ".fbx", ".skp", ".obj", ".3ds", ".dwg", ".dae", ".dxf", ".pdf", ".gltf", ".glb" };

        /// <summary>
        /// Returns true if the given file is supported, otherwise, returns false. Format is .*
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool IsFileSupported(string file, bool liveSync = false)
        {
            return Regex.IsMatch(file.ToLower(), SupportedFormatsRegex);
        }

        public static bool IsPXZ(string file)
        {
            return Path.GetExtension(file.ToLower()) == ".pxz";
        }

        readonly static char[] SPLT_SLASH = new[] { '/', '\\' };
        readonly static char[] SPLT_DOT = new[] { '.' };
        /// <summary>
        /// Returns of the extension of a given file path.
        /// Format is of type ".xxx" where xxx is lowercase.
        /// This function, unlike Path.GetExtention, handles versionned extensions such as ".prt.2". In such cases, it returns ".prt".
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetExtension2(string file)
        {
            var split = file.Split(SPLT_SLASH);
            var split2 = split[split.Length - 1].Split(SPLT_DOT);
            if (split2.Length == 1)
                return null;
            else {
                int v;
                bool isV = int.TryParse(split2[split2.Length - 1], out v);
                if (isV) {
                    return '.' + split2[split2.Length - 2].ToLower();
                } else {
                    return '.' + split2[split2.Length - 1].ToLower();
                }
            }
        }

        /// <summary>
        /// Returns a collection of all extensions formatted as : . + lowercase extension.
        /// </summary>
        public static string[] SupportedFormatsScriptedImporter {
            get {
                
                if (_SupportedFormatsScriptedImporter == null)
                {
                    Pixyz.IO.Native.FormatList formats = Pixyz.IO.Native.NativeInterface.GetImportFormats();
                    HashSet<string> extensions = new HashSet<string>();
                    for (int i = 0; i < formats.length; i++)
                    {
                        Pixyz.IO.Native.Format format = formats[i];
                        for (int j = 0; j < format.extensions.length; j++)
                        {
                            extensions.Add(format.extensions[j].Length > 1 ? format.extensions[j].Substring(1) : format.extensions[j]);
                        }
                    }
                    foreach (var unitySupportedFormat in UnitySupportedFormats)
                    {
                        extensions.Remove(unitySupportedFormat);
                    }
                    
                    _SupportedFormatsScriptedImporter = extensions.ToArray();
                }

                return _SupportedFormatsScriptedImporter;
            }
        }
        private static string[] _SupportedFormatsScriptedImporter;

        /// <summary>
        /// Returns a collection of all extensions formatted in a Regex.
        /// </summary>
        public static string SupportedFormatsRegex {
            get {
                if (_SupportedFormatsRegex == null) {
                    Pixyz.IO.Native.FormatList formats = Pixyz.IO.Native.NativeInterface.GetImportFormats();
                    StringBuilder strbldr = new StringBuilder();
                    for (int i = 0; i < formats.length; i++)
                    {
                        Pixyz.IO.Native.Format format = formats[i];
                        for (int j = 0; j < format.extensions.length; j++)
                        {
                            strbldr.Append("|" + format.extensions[j].Substring(1).ToLower());
                        }
                    }

                    strbldr[0] = '(';
                    strbldr.Append(")$");
                    strbldr.Replace(".", @"\.");
                    strbldr.Replace("*", @".*");
                    _SupportedFormatsRegex = strbldr.ToString();
                }
                return _SupportedFormatsRegex;
            }
        }
        private static string _SupportedFormatsRegex;

        /// <summary>
        /// Return an array of all supported formats, specially formatted for FileBrowsers.
        /// </summary>
        public static string[] SupportedFormatsForFileBrowser {
            get {
                if (_SupportedFormatsForFileBrowser == null)
                {
                    Pixyz.IO.Native.FormatList formats = Pixyz.IO.Native.NativeInterface.GetImportFormats();
                    _SupportedFormatsForFileBrowser = new string[(formats.length + 1) * 2];

                    StringBuilder strbldr = new StringBuilder();
                    for (int i = 0; i < formats.length; i++)
                    {
                        _SupportedFormatsForFileBrowser[(i + 1) * 2] = formats[i].name;
                        _SupportedFormatsForFileBrowser[(i + 1) * 2 + 1] = string.Join(",", formats[i].extensions.list).Replace("*.", "");
                        strbldr.Append(_SupportedFormatsForFileBrowser[(i + 1) * 2 + 1]);
                        strbldr.Append(",");
                    }
                    _SupportedFormatsForFileBrowser[0] = "All Pixyz files";
                    _SupportedFormatsForFileBrowser[1] = strbldr.ToString();
                }
                return _SupportedFormatsForFileBrowser;
            }
        }
        private static string[] _SupportedFormatsForFileBrowser;
    }
}
