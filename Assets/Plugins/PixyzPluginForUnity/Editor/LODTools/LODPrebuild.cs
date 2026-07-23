using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Pixyz.OptimizeSDK;
using Pixyz.OptimizeSDK.Editor.MeshProcessing;
using Pixyz.Commons.Extensions.Editor;
using Pixyz.Commons.Utilities.Editor;
using Pixyz.Plugin4Unity;
namespace Pixyz.LODTools.Editor
{
    [System.Serializable]
    public class LODPreBuild : IDisposable
    {
        public bool foldout = false;

        public LODProcessInspector processEditor = null;
        public LODGenerationData pastGeneration = null;
        public LODProcess buildProcess = null;
        public LODBuilder builder = new LODBuilder();

        public List<GameObject> gos = new List<GameObject>();
        public List<Renderer> srcRenderers = new List<Renderer>();

        public LODPreBuild(GameObject go)
        {
            gos = new List<GameObject>() { go };
            buildProcess = LODProcess.CreateInstance(true);
        }

        public LODPreBuild(LODGenerationData data)
        {
            pastGeneration = data;
            buildProcess = data.ProcessUsed;
        }

        public LODPreBuild()
        {
            gos = new List<GameObject>();
        }

        public void Dispose()
        {
            if (!AssetDatabase.IsMainAsset(buildProcess) && pastGeneration == null)
                GameObject.DestroyImmediate(buildProcess);
        }
    }
}
