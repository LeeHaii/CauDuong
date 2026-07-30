using System;
using UnityEngine;

public static class IfcInteractionRaycaster
{
    public static bool TryRaycast(
        Camera viewingCamera,
        Vector2 screenPosition,
        out RaycastHit selectedHit,
        out IfcElementMetadata metadata)
    {
        selectedHit = default;
        metadata = null;
        if (viewingCamera == null)
        {
            return false;
        }

        var hits = Physics.RaycastAll(
            viewingCamera.ScreenPointToRay(screenPosition),
            viewingCamera.farClipPlane,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore);
        Array.Sort(hits, (left, right) => left.distance.CompareTo(right.distance));

        foreach (var hit in hits)
        {
            var candidate = hit.transform.GetComponentInParent<IfcElementMetadata>();
            if (candidate == null)
            {
                continue;
            }

            selectedHit = hit;
            metadata = candidate;
            return true;
        }

        return false;
    }
}
