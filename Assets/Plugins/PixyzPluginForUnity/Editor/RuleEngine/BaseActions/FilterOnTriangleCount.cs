using System.Collections.Generic;
using UnityEngine;
using Pixyz.Plugin4Unity;
using Pixyz.Commons.UI.Editor;
using Pixyz.Commons.Extensions;

namespace Pixyz.RuleEngine.Editor
{
    public class FilterOnTriangleCount : ActionInOut<IList<GameObject>, IList<GameObject>>
    {

        [UserParameter(displayName: "Min")]
        public int minTriangleCount = 1;

        [UserParameter(displayName: "Max")]
        public int maxTriangleCount=1;

        public override int id => 273604424;
        public override string menuPathRuleEngine => "Filter/On Triangles Count";
        public override string menuPathToolbox => null;
        public override string tooltip { get { return "Filter meshes by their number of triangles"; } }

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

                int triangleCount = meshFilter.mesh.GetPolycount();
                if (triangleCount >= minTriangleCount && triangleCount <= maxTriangleCount)
                    output.Add(gameObject);
            }

            return output;
        }
    }
}