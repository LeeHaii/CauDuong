using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public static class IfcUiHitTest
{
    private const string InteractiveClass = "hud-interactive";

    public static bool IsPointerOverInteractiveUi(
        UIDocument document,
        Vector2 screenPosition)
    {
        var root = document != null ? document.rootVisualElement : null;
        if (root?.panel != null)
        {
            var panelPosition = RuntimePanelUtils.ScreenToPanel(
                root.panel,
                screenPosition);
            var picked = root.panel.Pick(panelPosition);
            for (var current = picked; current != null; current = current.parent)
            {
                if (current.ClassListContains(InteractiveClass))
                {
                    return true;
                }

                if (current == root)
                {
                    break;
                }
            }

            return false;
        }

        return EventSystem.current != null &&
               EventSystem.current.IsPointerOverGameObject();
    }
}
