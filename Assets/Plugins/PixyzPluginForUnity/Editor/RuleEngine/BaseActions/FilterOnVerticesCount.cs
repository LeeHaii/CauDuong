using System.Collections.Generic;
using UnityEngine;
using Pixyz.Plugin4Unity;
using Pixyz.Commons.UI.Editor;

namespace Pixyz.RuleEngine.Editor
{
    public class FilterOnVerticesCount : ActionInOut<IList<GameObject>, IList<GameObject>>
    {

        [UserParameter(displayName:"Min")]
        public int minVertexCount = 1;

        [UserParameter(displayName: "Max")]
        public int maxVertexCount = 1;
        public override int id => 1526631;
        public override string menuPathRuleEngine => "Filter/On Vertices count";
        public override string menuPathToolbox => null;
        public override string tooltip => "Filter meshes by their number of vertices";
        public override IList<GameObject> run(IList<GameObject> input)
        {
            if (!Configuration.CheckLicense()) throw new NoValidLicenseException();

            List<GameObject> output = new List<GameObject>();

            foreach (GameObject gameObject in input)
            {
                MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
                if (renderer == null)
                    continue;

                MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
                if (meshFilter == null)
                    continue;

                int vertextCount = meshFilter.mesh.vertexCount;
                if (vertextCount >= minVertexCount && vertextCount <= maxVertexCount)
                    output.Add(gameObject);
            }

                return output;
        }
    }

}