using Pixyz.Plugin4Unity;
using Pixyz.Commons.UI.Editor;
using System.Collections.Generic;
using UnityEngine;

namespace Pixyz.RuleEngine.Editor
{
    public class FilterOnSize : ActionInOut<IList<GameObject>, IList<GameObject>>
    {

        [UserParameter(displayName: "Max Bounding Box")]
        public float boundingBoxSize = 1f;

        public override int id => 749128909;
        public override string menuPathRuleEngine => "Filter/On Size";
        public override string menuPathToolbox => null;
        public override string tooltip { get { return "Filter meshes by their bounding box's maximal diagonal size. Size unit is by default in meters"; } }

        public override IList<GameObject> run(IList<GameObject> input)
        {
            if (!Configuration.CheckLicense()) throw new NoValidLicenseException();

            List<GameObject> output = new List<GameObject>();

            foreach (GameObject gameObject in input)
            {
                MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
                if (renderer == null)
                    continue;

                Bounds bounds = renderer.bounds;
                if (bounds.size.magnitude <= boundingBoxSize)
                    output.Add(gameObject);
            }

            return output;
        }
    }
}