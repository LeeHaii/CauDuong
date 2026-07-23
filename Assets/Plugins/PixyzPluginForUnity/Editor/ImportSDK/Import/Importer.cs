using Pixyz.Commons.Extensions;
using Pixyz.Commons.Utilities;
using Pixyz.OptimizeSDK.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Reflection;
using Pixyz.LODTools.Editor;
using Pixyz.LODTools;
using Pixyz.OptimizeSDK.Editor.MeshProcessing;
using Pixyz.Commons.Extensions.Editor;
using UnityEditor;
using Pixyz.Plugin4Unity;
using Pixyz.Plugin4Unity.Core;
using Pixyz.OptimizeSDK.Runtime;

namespace Pixyz.ImportSDK {

    public delegate void ExceptionHandler(Exception exception);

    /// <summary>
    /// Single-use class for importing 3D data to Unity.<br/>
    /// This class can be used in the editor as well as in runtime, if the license requirements are met.
    /// </summary>
    public sealed class Importer {

        public static Importer RunningInstance;

        private static ImportStamp _LatestModelImportedObject;
        /// <summary>
        /// The GameObject reference to the latest imported model. Returns null if no model was imported during this session.
        /// </summary>
        public static ImportStamp LatestModelImportedObject {
            get {
                if (_LatestModelImportedObject == null) {
                    _LatestModelImportedObject = GameObject.FindObjectsOfType<ImportStamp>().OrderByDescending(x => x.importTime).FirstOrDefault();
                }
                return _LatestModelImportedObject;
            }
        }

        /// <summary>
        /// The file path to the latest imported model. Returns null if no model was imported during this session.
        /// </summary>
        public static string LatestModelImportedPath { get; private set; }

        private static Dictionary<string, ImportSettingsTemplate> _SettingsTemplate = new Dictionary<string, ImportSettingsTemplate>();
        /// <summary>
        /// Add a pre-process action to run for a specific file format.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="preprocessingAction"></param>
        public static void AddOrSetTemplate(string format, ImportSettingsTemplate template) {
            format = format.ToLower();
            if (_SettingsTemplate.ContainsKey(format.ToLower())) {
                _SettingsTemplate[format] = template;
            } else {
                _SettingsTemplate.Add(format, template);
            }
        }
        
        private static Dictionary<string, SubProcess> _Preprocesses = new Dictionary<string, SubProcess>();
        /// <summary>
        /// Add a pre-process action to run for a specific file format.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="preprocessingAction"></param>
        public static void AddOrSetPreprocess(string format, SubProcess preprocessingAction) {
            format = format.ToLower();
            if (_Preprocesses.ContainsKey(format.ToLower())) {
                _Preprocesses[format] = preprocessingAction;
            } else {
                _Preprocesses.Add(format, preprocessingAction);
            }
        }

        private static Dictionary<string, SubProcess> _Postprocesses = new Dictionary<string, SubProcess>();
        /// <summary>
        /// Add a post-process action to run for a specific file format.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="postprocessingAction"></param>
        public static void AddOrSetPostprocess(string format, SubProcess postprocessingAction) {
            format = format.ToLower();
            if (_Postprocesses.ContainsKey(format.ToLower())) {
                _Postprocesses[format] = postprocessingAction;
            } else {
                _Postprocesses.Add(format, postprocessingAction);
            }
        }
        
        public static ImportSettingsTemplate GetSettingsTemplate(string filePath) {
            ImportSettingsTemplate template;
            if (!string.IsNullOrEmpty(filePath) && _SettingsTemplate.TryGetValue(Path.GetExtension(filePath).ToLower(), out template)) {
                return template;
            } else {
                return ImportSettingsTemplate.Default;
            }
        }

        
        public static SubProcess GetPreprocess(string filePath) {
            SubProcess preprocess;
            if (!string.IsNullOrEmpty(filePath) && _Preprocesses.TryGetValue(Path.GetExtension(filePath).ToLower(), out preprocess)) {
                return preprocess;
            } else {
                return null;
            }
        }

        public static SubProcess GetPostprocess(string filePath) {
            SubProcess postprocess;
            if (!string.IsNullOrEmpty(filePath) && _Postprocesses.TryGetValue(Path.GetExtension(filePath).ToLower(), out postprocess)) {
                return postprocess;
            } else {
                return null;
            }
        }
        
        /// <summary>
        /// Callback function triggered everytime the importer has progressed.
        /// Always occurs in the main thread.
        /// </summary>
        public event ProgressHandler progressed;

        /// <summary>
        /// Callback function trigerred when the import failed
        /// </summary>
        public event ExceptionHandler failed;

        private System.Diagnostics.Stopwatch _stopwatch;
        /// <summary>
        /// Elasped ticks since the begining of the import.
        /// </summary>
        public long elaspedTicks { get { return _stopwatch.ElapsedTicks; } }

        /// <summary>
        /// Callback function triggered when the importer has finished importing.
        /// In Async mode, this callback is triggered only when everything is finished.
        /// Always occurs in the main thread.
        /// </summary>
        public event GameObjectToVoidHandler completed;

        private string _file;
        /// <summary>
        /// The file to import
        /// </summary>
        public string filePath {
            get { return _file; }
            set { if (_hasStarted) { throw new AccessViolationException("Can only set Importer properties before it runs"); } _file = value; } }

        private bool _isAsynchronous = true;
        /// <summary>
        /// [Default is True]
        /// If set to true, the import process will run as much as it can on different threads than the main one, so that it won't freeze the Editor/Application and performances are kept to the maximum.
        /// In Asynchronous mode, it is recommended to use callback methods to get information on the import status.
        /// </summary>
        public bool isAsynchronous {
            get { return _isAsynchronous; }
            set { if (_hasStarted) { throw new AccessViolationException("Can only set Importer properties before it runs"); } _isAsynchronous = value; } }

        private bool _printMessageOnCompletion = false;
        /// <summary>
        /// [Default is True]
        /// If set to true, the importer will print a message in the console on import completion.
        /// </summary>
        public bool printMessageOnCompletion {
            get { return _printMessageOnCompletion; }
            set { if (_hasStarted) { throw new AccessViolationException("Can only set Importer properties before it runs"); } _printMessageOnCompletion = value; }
        }

        private ImportSettings _importSettings;
        /// <summary>
        /// Returns the ImportSettings reference 
        /// </summary>
        public ImportSettings importSettings { get { return _importSettings; } set { if (_hasStarted) { throw new AccessViolationException("Can only set Importer properties before it runs"); } _importSettings = value; } }
        
        public SubProcess preprocess => GetPreprocess(filePath);

        public SubProcess postprocess => GetPostprocess(filePath);
        

        private ImportStamp _importStamp;
        public ImportStamp importedModel => _importStamp;

        private ImportSettings _importSettingsCopy;

        private Scene.Native.PackedTree _sceneTree;

        private SceneTreeConverter _sceneTreeConverter;

        private bool _hasStarted = false;
        private string _seed;
        UInt32 _root;
        public UInt32 root { get { return _root; } set { _root = value; } }

        const string CANCEL_MESSAGE = "Import has been canceled.";

        private static bool _isPointCloud = false;
        
        MeshData lodMeshData;
        LODBuilder lodBuilder;
        LODProcess lodProcess;
        GameObject imported;

        float prefSagRatio;
        float prefMaxSag;

        public Importer(string file, ImportSettings importSettings) {

            filePath = file;
            prefSagRatio = Preferences.CustomImportSagRatio;
            prefMaxSag = Preferences.CustomImportMaxSag;
            if (importSettings)
                this.importSettings = importSettings;
            else
                this.importSettings = ScriptableObject.CreateInstance<ImportSettings>();

            if (!File.Exists(file)) {
                throw new Exception($"File '{file}' does not exist");
            }
            if (!Plugin4Unity.Formats.IsFileSupported(file)) {
                throw new Exception($"File '{file}' is not supported by Pixyz");
            }
        }

        /// <summary>
        /// Starts importing the file. Can be executed only once per Importer instance.
        /// </summary>
        public void run() {
            try {
                if(!Core.Native.NativeInterface.CheckLicense())
                    throw new NoValidLicenseException();
                
                if (!OnImportStart())
                    return;

                RunningInstance = this;
                Dispatcher.StartCoroutine(runCoreCommands());
            } catch (Exception exception) {
                reportImportProgressed(1f, "Exception");
                invokeFailed(exception);
                Debug.LogException(exception);
            }
        }
        internal static bool OnImportStart()
        {
            bool isCallerIdentified = false;
            var stackTrace = new System.Diagnostics.StackTrace();
            try
            {
                for (int i = 2; i < 10; i++)
                {
                    MethodBase methodBase = stackTrace.GetFrame(i).GetMethod();
                    string typeName = methodBase.DeclaringType.Name;
                    string methodName = methodBase.Name;
                    //UnityEngine.Debug.Log($"TYPE:{typeName}, METHOD:{methodName}");
                    if (isCallerIdentified = typeName == "ScriptedImporter" && methodName == "OnImportAsset")
                        break;
                    if (isCallerIdentified = typeName == "ImportWindow" && methodName == "OnImportClicked")
                        break;
                }
            }
            catch { }
            if (!isCallerIdentified)
            {
                Debug.LogError(new OutOfTermsException().Message);
                return false;
            }

            return true;
        }

        delegate uint ImportFileDelegate(string file);

        private bool checkIfCanceled() {
            if (_isStopped) {
                Core.Native.NativeInterface.ResetSession();
                reportImportProgressed(1f, CANCEL_MESSAGE);
                Debug.LogWarning(CANCEL_MESSAGE);
                return true;
            }
            return false;
        }
                
    /// <summary>
    /// Read the file in the Pixyz Core native assemblies
    /// </summary>
    private IEnumerator runCoreCommands() {

//#if UNITY_EDITOR
//            UnityEditor.EditorPrefs.SetBool("kAutoRefresh", false);
//#endif

            if (_hasStarted) {
                throw new Exception("An Importer instance can only import once. Please create a new Importer instance to import another file.");
            }

            _hasStarted = true;
            _stopwatch = new System.Diagnostics.Stopwatch();
            _stopwatch.Start();

            _seed = UnityEngine.Random.Range(0, int.MaxValue).ToString();

            Profiling.Start("Importer " + _seed);
            reportImportProgressed(0f, "Initializing...");

            /// Freezing the ImportSettings, and eventually change some settings depending on context
            /// For example,
            /// - Ensures it's not making LODs and doing some tree processing if file is .pxz
            /// - Ensures it's not loading metadata if there is some tree processing
            /// - Loads default shader if not override shader is specified
            _importSettingsCopy = UnityEngine.Object.Instantiate(importSettings).Verify(GetSettingsTemplate(filePath));
            _importSettingsCopy.name = importSettings.name;

            if (_importSettingsCopy.shader == null) {
                _importSettingsCopy.shader = ShaderUtilities.GetDefaultShader();
            }

            if (isAsynchronous)
                yield return Dispatcher.GoThreadPool();

            /// This part runs in another thread to keep the Editor/App smooth and improve performances.
            try {
                
                /// Assembly
                reportImportProgressed(0.10f, "Reading file...");
                Core.Native.NativeInterface.SetCurrentThreadAsProcessThread();
                Core.Native.NativeInterface.ResetSession();

                if (checkIfCanceled())
                    yield break;

                //Import File
                _root = 0;
                string ext = Path.GetExtension(filePath).ToLower();

                // Set IO preferences
                Core.Native.NativeInterface.SetModuleProperty("IO", "FlipCoordinateSystem", (importSettings.orientation == Orientation.AUTOMATIC).ToString());
                if (ext != ".vpb")
                    Core.Native.NativeInterface.SetModuleProperty("IO", "PreferLoadMesh", "false");
                
                if (importSettings.orientation == Orientation.AUTOMATIC)
                {
                    // Orientation is set to automatic. Model will be correctly oriented inside Pixyz (y-up / right-handed).
                    _importSettingsCopy.isLeftHanded = false;
                    _importSettingsCopy.isZUp = false;
                }

                Core.Native.NativeInterface.PushAnalytic("ImportFormat", ext + ";" +importSettings.qualities.quality.quality.ToString());
                _root = IO.Native.NativeInterface.ImportScene(filePath, _root);

                if (checkIfCanceled())
                    yield break;

                
                if (preprocess != null) {
                    try {
                        reportImportProgressed(0.30f, "Pre-processing...");
                        preprocess.run(this);
                    } catch (Exception exception) {
                        Debug.LogError("An exception occurred in the pre-process : " + exception);
                        yield break;
                    }
                }

                reportImportProgressed(0.40f, "Processing...");

                //Run automatic process
                _root = AutomaticImportProcess(_root, _importSettingsCopy);

                if (checkIfCanceled())
                    yield break;

                /// Scene
                reportImportProgressed(0.60f, "Extracting...");
                _sceneTree = Scene.Native.NativeInterface.GetCompleteTree(_root, Scene.Native.VisibilityMode.Inherited);                

            } catch (Exception coreException) {
                /// An exception has occured in the Core
                Core.Native.NativeInterface.ResetSession();
                reportImportProgressed(1f, "Core exception");
                invokeFailed(coreException, true);
                Debug.LogException(coreException);
                yield break;
            }

            if (_isStopped) {
                reportImportProgressed(1f, CANCEL_MESSAGE);
                Debug.LogWarning(CANCEL_MESSAGE + " before creating objects");
                yield break;
            }

            reportImportProgressed(0.70f, "Converting...");
            if (isAsynchronous)
                yield return Dispatcher.GoMainThread();

            Core.Native.NativeInterface.SetCurrentThreadAsProcessThread();

            /// Running this Part in the main Unity thread.
            reportImportProgressed(0.80f, "Creating objects...");
            try {
                /// Converting the Scene from Core data to Unity data structure
                _sceneTreeConverter = new SceneTreeConverter(_sceneTree, filePath, _importSettingsCopy, finalize);
                _sceneTreeConverter.Convert();
                
            } catch (Exception sceneConversionException) {
                /// An exception has occured while trying to convert the Scene
                reportImportProgressed(1f, "Extraction exception");
                invokeFailed(sceneConversionException);
                Debug.LogException(sceneConversionException);
            }
        }

        private void finalize() {

            if (checkIfCanceled())
                return;

            try {
                GameObject gameObject =  _sceneTreeConverter.gameObject;

                if (_importSettings.hasLODs)
                    GenerateLOD(gameObject);

                /// Sets LatestModelImported (useful for RuleEngine or any other script running after an import)
                TimeSpan time = Profiling.End("Importer " + _seed);
                _stopwatch.Stop();

                /// Recreating ImportedModel stamp
                _importStamp = gameObject.AddComponent<ImportStamp>();
                _importStamp.stamp(filePath, elaspedTicks);
                _importStamp.importSettings = _importSettingsCopy;

                _LatestModelImportedObject = _importStamp;

                
                /// Post-Process
                if (postprocess != null) {
                    try {
                        postprocess.run(this);
                    } catch (Exception exception) {
                        Debug.LogError("An exception occurred in the post-process : " + exception);
                    }
                }
                
                /// Import is finished. Sets progress to 100% and runs callbackEnded.
                //reportImportProgressed(1f, "Done !");
                if (printMessageOnCompletion)
                    BaseExtensions.LogColor(UnityEngine.Color.green, $"Pixyz Import > File imported in {time.FormatNicely()}");

                invokeCompleted(gameObject);

            } catch (Exception exception) {
                reportImportProgressed(1f, "Finalization exception");
                invokeFailed(exception);
                Debug.LogException(exception);
            }

            clear();
        }

        private void invokeFailed(Exception exception, bool goMainThread = false)
        {
            RunningInstance = null;
            //#if UNITY_EDITOR
            //            UnityEditor.EditorPrefs.SetBool("kAutoRefresh", true);
            //#endif
            if(goMainThread)
            {
                Dispatcher.StartCoroutine(invokeFailedMainThread(exception));
            }
            else
            {
                failed?.Invoke(exception);
            }
        }

        private IEnumerator invokeFailedMainThread(Exception e)
        {
            yield return Dispatcher.GoMainThread();
            Core.Native.NativeInterface.SetCurrentThreadAsProcessThread();
            failed?.Invoke(e);
        }

        private void invokeCompleted(GameObject gameObject)
        {
            RunningInstance = null;
//#if UNITY_EDITOR
//            UnityEditor.EditorPrefs.SetBool("kAutoRefresh", true);
//#endif
            completed?.Invoke(gameObject);
        }

        private void reportImportProgressed(float progress, string message) {
            progressed?.Invoke(progress, message);
        }

        private void clear() {
            _sceneTree = new Scene.Native.PackedTree();
            _sceneTreeConverter = null;
        }

        private bool _isStopped = false;
        public bool isStopped => _isStopped;

        public void stop() {
            _isStopped = true;
        }
        
        private UInt32 AutomaticImportProcess(UInt32 root, ImportSettings settings)
        {
            string ext = filePath.Substring(filePath.LastIndexOf('.')+1).ToLower();
            //Create occurrenceList
            Scene.Native.OccurrenceList rootList = new Scene.Native.OccurrenceList(1);
            rootList[0] = Scene.Native.NativeInterface.GetRoot();

            //Tolerance
            Tolerances tolerances = new Tolerances(settings.qualities.quality.quality);

            reportImportProgressed(0.5f, "Processing imported model");
            if (settings.importPoints)
            {
                if (ext == "e57" || ext == "pts" || ext == "ptx" || ext == "xyz" || ext == "rcp")
                {
                    _isPointCloud = true;

                    var bounds = Scene.Native.NativeInterface.GetAABB(rootList);

                    if (settings.voxelizeGridSize > 1)
                    {
                        double voxelGridSize = Math.Max(1, Math.Min(80, settings.voxelizeGridSize));
                        double voxelSize = Math.Pow((bounds.high.x - bounds.low.x) * (bounds.high.y - bounds.low.y) * (bounds.high.z - bounds.low.z), 1.0 / 3.0) / voxelGridSize;
                        Scene.Native.NativeInterface.MergeByTreeLevel(rootList, 1, Scene.Native.MergeHiddenPartsMode.Destroy);
                        root = Scene.Native.NativeInterface.GetChildren(rootList[0])[0];
                        Algo.Native.NativeInterface.VoxelizePointClouds(rootList, voxelSize);
                    }

                    if(settings.hasLODs && settings.qualities.lods.Length > 1)
                    {
                        List<int> qualities = new List<int>();
                        for (int i = 0; i< settings.qualities.lods.Length; i++)
                        {
                            if (settings.qualities.lods[i].quality == LODTools.LodQuality.CULLED)
                                continue;

                            qualities.Add((int)settings.qualities.lods[i].quality);
                        }
                    }
                    else if (tolerances.doDecimation)
                    {
                        //Decimating point cloud
                        double voxelSizePointCloud = Mathf.Pow(((float)bounds.high.x - (float)bounds.low.x) * ((float)bounds.high.y - (float)bounds.low.y) * ((float)bounds.high.z - (float)bounds.low.z), 1.0f / 3.0f);
                        double splatSize = 0.0035;
                        double distance =  Mathf.Max(0.002f * (float)voxelSizePointCloud, (float)(voxelSizePointCloud * splatSize)) / tolerances.pointCloudDensity;

                        Algo.Native.NativeInterface.DecimatePointClouds(rootList, distance);
                    }
                    return root;
                }
            }
            else
            {
                Algo.Native.NativeInterface.DeleteFreeVertices(rootList);
            }
                

            if(settings.stitchPatches && settings.treeProcess != TreeProcessType.MERGE_FINAL_LEVEL)
            {
                Scene.Native.NativeInterface.MergeFinalLevel(rootList, Scene.Native.MergeHiddenPartsMode.Destroy, true);
                Scene.Native.NativeInterface.DeleteEmptyOccurrences(root);
            }

            if(!settings.importLines)
            {
                Algo.Native.NativeInterface.DeleteLines(rootList);
            }

            if (settings.repair)
            {
                Algo.Native.NativeInterface.RepairCAD(rootList, 0.1, false);
                Algo.Native.NativeInterface.RepairMesh(rootList, 0.1, true, false);
            }

            if (settings.qualities.quality.quality == LodQuality.CUSTOM)
            {
                tolerances.sagRatio = prefSagRatio;
                tolerances.maxSag = prefMaxSag;
            }

            if (tolerances.doDecimation)
            {
                //Decimating (if original model has mesh data)...
                Algo.Native.NativeInterface.Decimate(rootList, tolerances.surfacicTolerance, tolerances.lineicTolerance, tolerances.normalTolerance, tolerances.uvTolerance, false);
            }

            //Tessellating (if original model has BREP data)...
            //Do not tesselate if there is existig tesselation (only with pxz file)
            //Delete all BREP info to free memory space
            Core.Native.NativeInterface.SetModuleProperty("Tessellate", "GenerateQuads", "False");
            Algo.Native.NativeInterface.TessellateRelativelyToAABB(rootList, tolerances.maxSag, tolerances.sagRatio, -1, tolerances.maxAngle, true, Algo.Native.UVGenerationMode.NoUV, 1, 0, false, false, false, false);

            //Tesselate can re-create lines from brep curves into meshes
            if (!settings.importLines)
            {
                Algo.Native.NativeInterface.DeleteLines(rootList);
            }

            if (settings.repairInstances)
            {
                Algo.Native.NativeInterface.CreateInstancesBySimilarity(rootList, 00.99, 0.99, true, true, false);
            }

            if(settings.orient)
            {
                Algo.Native.NativeInterface.RepairMesh(rootList, 0.1, true, true);
            }
            else if(settings.repair)
            {
                Algo.Native.NativeInterface.RepairMesh(rootList, 0.1, true, false);
            }
            
            bool isOccurrenceSingle = Scene.Native.NativeInterface.GetChildren(root).list.Length==0;

            Scene.Native.NativeInterface.MergeMaterials();

            switch (settings.treeProcess)
            {
                case TreeProcessType.TRANSFER_ALL_UNDER_ROOT:
                    Scene.Native.NativeInterface.Rake(root, true);
                    break;
                case TreeProcessType.CLEANUP_INTERMEDIARY_NODES:
                    var newRoot = Scene.Native.NativeInterface.CreateOccurrence(Scene.Native.NativeInterface.GetNodeName(root), Scene.Native.NativeInterface.GetParent(root));
                    var compressed = Scene.Native.NativeInterface.Compress(root);
                    if(root != compressed)
                    {
                        Scene.Native.NativeInterface.SetParent(compressed, newRoot, false, 0);
                        root = newRoot;
                    }
                    
                    break;
                case TreeProcessType.MERGE_ALL:
                    Scene.Native.NativeInterface.MergeByTreeLevel(rootList, 1, Scene.Native.MergeHiddenPartsMode.Destroy);
                    root = Scene.Native.NativeInterface.GetChildren(rootList[0])[0];
                    break;
                 case TreeProcessType.MERGE_BY_MATERIAL:
                    Scene.Native.NativeInterface.MergeByTreeLevel(rootList, 1, Scene.Native.MergeHiddenPartsMode.Destroy);
                    Algo.Native.NativeInterface.ExplodePartByMaterials(rootList);
                    root = Scene.Native.NativeInterface.GetChildren(rootList[0])[0];
                    break;
                case TreeProcessType.MERGE_FINAL_LEVEL:
                    Scene.Native.NativeInterface.MergeFinalLevel(rootList, Scene.Native.MergeHiddenPartsMode.Destroy, true);
                    root = Scene.Native.NativeInterface.GetChildren(rootList[0])[0];
                    Scene.Native.NativeInterface.DeleteEmptyOccurrences(root);
                    break;
                case TreeProcessType.MERGE_BY_HIERARCHY_LEVEL:
                    Scene.Native.NativeInterface.MergeByTreeLevel(rootList, _importSettingsCopy.treeLevel, Scene.Native.MergeHiddenPartsMode.Destroy);
                    root = Scene.Native.NativeInterface.GetChildren(rootList[0])[0];
                    break;
                case TreeProcessType.MERGE_BY_NAME:
                    Scene.Native.NativeInterface.MergePartsByName(root, Scene.Native.MergeHiddenPartsMode.Destroy);
                    break;
                case TreeProcessType.MERGE_BY_REGIONS:
                    Scene.Native.MergeByRegionsStrategy strategy = new Scene.Native.MergeByRegionsStrategy();
                    strategy.NumberOfRegions = settings.numberOfRegions;
                    strategy.SizeOfRegions = settings.sizeOfRegions * 1000f; // Multiply by 1000 to convert from Unity units to mm (real scale at import != toolbox)
                    if (settings.mergeBy == MergeByRegions.NUMBER_OF_REGIONS)
                    {
                        strategy._type = Scene.Native.MergeByRegionsStrategy.Type.NUMBEROFREGIONS;
                    } 
                    else
                    {
                        strategy._type = Scene.Native.MergeByRegionsStrategy.Type.SIZEOFREGIONS;
                    }
                    Scene.Native.NativeInterface.MergeByRegions(rootList, strategy, (Scene.Native.MergeStrategy)settings.mergeType);
                    root = rootList[0];
                    if (!settings.importPatchBorders)
                        Algo.Native.NativeInterface.DeletePatches(rootList, true);
                    break;
                default:
                    break;
                
            }

            //LOD management
            if(settings.hasLODs)
            {
                if (settings.qualities.lods.Length > 1)
                {
                    List<int> qualities = new List<int>();
                    for (int i = 0; i < settings.qualities.lods.Length; i++)
                    {
                        if (settings.qualities.lods[i].quality == LODTools.LodQuality.CULLED)
                            continue;

                        qualities.Add((int)settings.qualities.lods[i].quality);
                    }

                    //Generate LOD
                    //NativeInterface.GenerateLOD(rootList, new Core.Native.IntList(qualities.ToArray()));
                }
            }
            //---------------
            if(settings.importPatchBorders)
            {
                Algo.Native.NativeInterface.CreateFreeEdgesFromPatches(rootList);
            }

            if(settings.combinePatchesByMaterial)
            {
                Algo.Native.NativeInterface.DeletePatches(rootList, true);
            }

            Algo.Native.NativeInterface.CreateNormals(rootList, -1, false, true);

            if(settings.splitTo16BytesIndex)
            {
                Algo.Native.NativeInterface.ExplodeVertexCount(rootList, 65534, 65534, false);
            }

            if(settings.mapUV3dSize >= 0)
            {
                Algo.Native.NativeInterface.MapUvOnAABB(rootList, false, settings.mapUV3dSize, 0, true);

                Algo.Native.NativeInterface.CreateTangents(rootList, -1, 0, false);
            }

            if(settings.lightmapResolution > 0)
            {
                // Compute lightmap uvs in one UV space per part. Probably slower but more logical. However Unity scales himself lightmap uvs for each part...
                Algo.Native.NativeInterface.MapUvOnAABB(rootList, false, 1, 0, true);
                Algo.Native.NativeInterface.RepackUV(rootList, 1, false, settings.lightmapResolution, settings.uvPadding, false, 3, false);
                Algo.Native.NativeInterface.NormalizeUV(rootList, 1, -1, true, false, false);
            }

            if (settings.avoidNegativeScale)
            {
                if (!settings.isLeftHanded)
                {
                    // Apply a left handed conversion on root
                    // Then, calling NativeInterface.RemoveSymmetryMatrices applies this transformation directly in vertices
                    Geom.Native.Matrix4 matLeftHanded = Conversions.Identity();
                    matLeftHanded.tab[0] = new Geom.Native.Array4(new double[] { -1.0, 0.0, 0.0, 0.0 });
                    Scene.Native.NativeInterface.ApplyTransformation(root, matLeftHanded);
                }
                Scene.Native.NativeInterface.RemoveSymmetryMatrices(root);
            }

            Scene.Native.NativeInterface.TransferCADMaterialsOnPartOccurrences(root);

            Scene.Native.NativeInterface.ResetPartTransform(root);

            return root;

        }

        private void GenerateLOD(GameObject gameObject)
        {
            if (_importSettingsCopy.qualities.lods.Length > 1)
            {
                List<int> qualities = new List<int>();
                for (int i = 0; i < _importSettingsCopy.qualities.lods.Length; i++)
                {
                    if (_importSettingsCopy.qualities.lods[i].quality == LODTools.LodQuality.CULLED)
                        continue;

                    qualities.Add((int)_importSettingsCopy.qualities.lods[i].quality);
                }

                lodBuilder = new LODBuilder();

                lodProcess = LODProcess.CreateInstance(false);

                lodProcess.name = "Default";
                
                // Don't process qualities[0] because it was already processed in import process
                for (int i = 1; i < qualities.Count; i++)
                {
                    LODRule newRule = LODRule.CreateInstance();
                    
                    if (_isPointCloud)
                    {
                        newRule.isDecimatePointCloudActivated = true;
                        lodProcess.AddRule(newRule, _importSettingsCopy.qualities.lods[i].threshold, 0);
                        lodProcess.SetLODSource(i - 1, i - 1);
                    }
                    else
                    {
                        // LOD rules are only decimations to quality at import
                        newRule.isDecimateToQualityActivated = true;
                        newRule.decimateToQualityParam = DecimateToQualityParameters.Preset((OptimizeSDK.Runtime.QualityLevel)(_importSettingsCopy.qualities.lods[i].quality - 1));
                        lodProcess.AddRule(newRule, _importSettingsCopy.qualities.lods[i].threshold, 0);
                    }
                }

                List<GameObject> objects = new List<GameObject>() { gameObject };

                //get renderers
                List<Renderer> renderers = new List<Renderer>();
                renderers.AddRange(gameObject.GetComponentsInChildren<Renderer>(true));

                lodMeshData = objects.GetMeshData();
                lodMeshData.renderers = new List<Renderer>(renderers.Count);
                lodMeshData.meshes = new List<Mesh>(renderers.Count);
                lodMeshData.materials = new List<UnityEngine.Material[]>(renderers.Count);

                for (int i = 0; i < renderers.Count; ++i)
                {
                    Renderer r = renderers[i];
                    if (r is SkinnedMeshRenderer)
                    {
                        lodMeshData.meshes.Add(((SkinnedMeshRenderer)r).sharedMesh);
                    }
                    else if (r is MeshRenderer)
                    {
                        lodMeshData.meshes.Add(r.GetComponent<MeshFilter>().sharedMesh);
                    }
                    else
                    {
                        continue;
                    }
                    lodMeshData.renderers.Add(renderers[i]);
                    lodMeshData.materials.Add(r.sharedMaterials);
                }

                PixyzContext context = new PixyzContext();
                context.UnityMeshesToPixyzCreate(lodMeshData.renderers, lodMeshData.meshes, lodMeshData.materials);
                lodBuilder.generationCompleted += LODGenerationCompleted;
                try
                {
#pragma warning disable CS4014
                    imported = gameObject;
                    lodBuilder.BuildLOD(context, lodProcess, false);
#pragma warning restore CS4014
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.Message + "\n" + e.StackTrace);
                }

            }

        }

        private void LODGenerationCompleted(bool error)
        {
            try
            {
                double[] thresholds = new double[_importSettingsCopy.qualities.lods.Length];
                for (int i = 0; i < _importSettingsCopy.qualities.lods.Length; i++)
                {
                    thresholds[i] = _importSettingsCopy.qualities.lods[i].threshold;
                }
                    
                if (_importSettingsCopy.lodsMode == LodGroupPlacement.ROOT)
                {
                    //Set LOD group on Root
                    LODGroup group = null;
                    UnityEngine.LOD[] lods = new UnityEngine.LOD[lodBuilder.Contexts.Length];
                    group = imported.AddComponent<LODGroup>();
                    UnityEngine.LOD lod0 = lods[0];

                    List<GameObject> selection = new List<GameObject>(Selection.gameObjects);
                    selection.Add(group.gameObject);

                    Selection.objects = selection.ToArray();

                    lod0.screenRelativeTransitionHeight = (float)thresholds[0];

                    List<Renderer> newLOD0Renderers = new List<Renderer>();

                    foreach (Renderer r in lodMeshData.renderers)
                    {
                        GameObject copiedGameobject = new GameObject(r.gameObject.name+"_LOD0");
                        copiedGameobject.transform.position = r.transform.position;
                        copiedGameobject.transform.rotation = r.transform.rotation;
                        copiedGameobject.transform.localScale = r.transform.lossyScale;

                        if (r is MeshRenderer)
                        {
                            UnityEditorInternal.ComponentUtility.CopyComponent(r.GetComponent<MeshFilter>());
                            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(copiedGameobject);
                        }

                        UnityEditorInternal.ComponentUtility.CopyComponent(r);
                        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(copiedGameobject);
                        newLOD0Renderers.Add(copiedGameobject.GetComponent<Renderer>());
                        copiedGameobject.transform.parent = r.transform;

                    }

                    lod0.renderers = newLOD0Renderers.ToArray();

                    lod0.screenRelativeTransitionHeight = (float)thresholds[0];
                    
                    lods[0] = lod0;

                    for (int i = 1; i < lodBuilder.Contexts.Length; ++i)
                    {

                        PixyzContext context = lodBuilder.Contexts[i];
                        Renderer[] generatedRenderer = context.PixyzMeshToUnityRenderer(context.pixyzMeshes);

                        LOD lod = lods[i];

                        if (lod.renderers != null)
                        {
                            foreach (Renderer r in lod.renderers)
                            {
                                if (r == null)
                                    continue;

                                PrefabInstanceStatus status = PrefabUtility.GetPrefabInstanceStatus(r.transform);

                                if (status == PrefabInstanceStatus.Connected || status == PrefabInstanceStatus.MissingAsset)
                                    r.gameObject.SetActive(false);
                                else
                                    GameObject.DestroyImmediate(r.gameObject);

                            }
                        }

                        lod.screenRelativeTransitionHeight = (float)thresholds[i];

                        for (int j = 0; j < generatedRenderer.Length; ++j)
                        {

                            Renderer r = generatedRenderer[j];
                            r.gameObject.name = $"{lodMeshData.renderers[j].name}_LOD{i}";

                            Matrix4x4 transform = Conversions.ConvertMatrix(context.pixyzMatrices[j]);

                            transform.GetTRS(out Vector3 pos, out Quaternion rot, out Vector3 scale);
                            r.transform.position = pos;
                            r.transform.rotation = rot;
                            r.transform.localScale = scale;

                            r.transform.parent = lodMeshData.renderers[j].transform;

                        }
                        lod.renderers = generatedRenderer.ToArray();
                        lods[i] = lod;
                    }

                    group.SetLODs(lods);
                }
                else
                {
                    //LOD group on leaves
                    List<LODGroup> lodGroups = new List<LODGroup>();
                    List<LOD[]> lodPerRenderers = new List<LOD[]>();

                    foreach (Renderer r in lodMeshData.renderers)
                    {
                        GameObject copiedGameobject = new GameObject(r.gameObject.name + "_LOD0");
                        copiedGameobject.transform.position = r.transform.position;
                        copiedGameobject.transform.rotation = r.transform.rotation;
                        copiedGameobject.transform.localScale = r.transform.lossyScale;

                        if (r is MeshRenderer)
                        {
                            UnityEditorInternal.ComponentUtility.CopyComponent(r.GetComponent<MeshFilter>());
                            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(copiedGameobject);
                        }

                        UnityEditorInternal.ComponentUtility.CopyComponent(r);
                        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(copiedGameobject);

                        //Create new LOD Group
                        lodGroups.Add(r.gameObject.AddComponent<LODGroup>());
                        //Add lod array to this group
                        LOD[] newLods = new LOD[lodBuilder.Contexts.Length];
                        newLods[0].screenRelativeTransitionHeight = (float)thresholds[0];
                        newLods[0].renderers = new Renderer[1];
                        newLods[0].renderers[0] = copiedGameobject.GetComponent<Renderer>();
                        lodPerRenderers.Add(newLods);
                        copiedGameobject.transform.parent = r.transform;
                    }

                    for (int i = 1; i < lodBuilder.Contexts.Length; ++i)
                    {

                        PixyzContext context = lodBuilder.Contexts[i];
                        Renderer[] generatedRenderer = context.PixyzMeshToUnityRenderer(context.pixyzMeshes);

                        for (int j = 0; j < generatedRenderer.Length; ++j)
                        {
                            //Renderer j for LOD i
                            Renderer r = generatedRenderer[j];
                            r.gameObject.name = $"{lodMeshData.renderers[j].name}_LOD{i}";

                            Matrix4x4 transform = Conversions.ConvertMatrix(context.pixyzMatrices[j]);

                            transform.GetTRS(out Vector3 pos, out Quaternion rot, out Vector3 scale);
                            r.transform.position = pos;
                            r.transform.rotation = rot;
                            r.transform.localScale = scale;

                            r.transform.parent = lodMeshData.renderers[j].transform;

                            if (lodPerRenderers[j] == null)
                                lodPerRenderers[j] = new LOD[lodBuilder.Contexts.Length];
                            if (lodPerRenderers[j][i].renderers == null)
                                lodPerRenderers[j][i].renderers = new Renderer[1];

                            if (lodBuilder.meshToThresholds.TryGetValue(context.pixyzMeshes[j], out double threshold))
                            {
                                // threshold was automatically computed by the optimization process
                                lodPerRenderers[j][i - 1].screenRelativeTransitionHeight = (float)threshold; // in this case set the one before
                            }
                            else
                            {
                                lodPerRenderers[j][i].screenRelativeTransitionHeight = (float)thresholds[i];
                            }

                            lodPerRenderers[j][i].renderers[0] = r;
                        }
                    }

                    for (int i= 0;i<lodGroups.Count;i++)
                    {
                        lodGroups[i].SetLODs(lodPerRenderers[i]);
                    }
                }

                foreach (Renderer r in lodMeshData.renderers)
                {
                    Component.DestroyImmediate(r.GetComponent<MeshFilter>());
                    Component.DestroyImmediate(r);
                }

            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"[LODGenerationError] {e.Message}\n{e.StackTrace}");
            }
        }
    }
}