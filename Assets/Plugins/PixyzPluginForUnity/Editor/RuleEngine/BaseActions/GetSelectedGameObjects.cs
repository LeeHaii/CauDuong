using Pixyz.Toolbox.Editor;
using System.Collections.Generic;
using UnityEngine;
using Pixyz.Commons.UI.Editor;
using Pixyz.Commons.Extensions;

namespace Pixyz.RuleEngine.Editor
{
    public class GetSelectedGameObjects : ActionOut<IList<GameObject>> {

        public override int id => 64811152;
        public override string menuPathRuleEngine => "Get/Selected GameObjects";
        public override string menuPathToolbox => null;
        public override string tooltip => "Get GameObjects selected in the Unity Editor";

        public override IList<GameObject> run() {

            IList<GameObject> gameObjects;

#if UNITY_EDITOR
            gameObjects = UnityEditor.Selection.gameObjects;
#else
            gameObjects = new GameObject[0];
#endif

#if UNITY_EDITOR
            UnityEditor.Undo.RegisterFullObjectHierarchyUndo(new List<GameObject>(gameObjects).GetHighestAncestor(), "RuleEngine Entry Point");
#endif

            return gameObjects;
        }
    }
}