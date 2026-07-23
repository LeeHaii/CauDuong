using Pixyz.Commons.UI.Editor;
using System.Collections.Generic;
using UnityEngine;
using Pixyz.Commons.Extensions;

namespace Pixyz.RuleEngine.Editor
{
    public class GetAllGameObjects : ActionOut<IList<GameObject>> {

        public override int id => 887274667;
        public override string menuPathRuleEngine => "Get/All GameObjects";
        public override string menuPathToolbox => null;
        public override string tooltip => "Get all GameObjects in the current Scene, at any level.";

        public override IList<GameObject> run() {

            var gameObjects = GameObject.FindObjectsOfType<GameObject>();

#if UNITY_EDITOR
            UnityEditor.Undo.RegisterFullObjectHierarchyUndo(new List<GameObject>(gameObjects).GetHighestAncestor(), "RuleEngine Entry Point");
#endif

            return gameObjects;
        }
    }
}