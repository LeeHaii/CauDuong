using System;
using UnityEngine;

public static class IfcInteractionRaycaster
{
    private const float PointerTolerancePixels = 8f;

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
            viewingCamera = Camera.main;
        }

        if (viewingCamera == null)
        {
            viewingCamera = UnityEngine.Object.FindFirstObjectByType<Camera>();
        }

        if (viewingCamera == null)
        {
            return false;
        }

        Physics.SyncTransforms();
        var previousBackfaceSetting = Physics.queriesHitBackfaces;
        var ray = viewingCamera.ScreenPointToRay(screenPosition);
        RaycastHit[] hits;
        try
        {
            // IFC surfaces are rendered double-sided, so their colliders must be
            // queryable from either winding direction as well.
            Physics.queriesHitBackfaces = true;
            hits = Physics.RaycastAll(
                ray,
                viewingCamera.farClipPlane,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);

            if (TrySelectNearestIfc(hits, out selectedHit, out metadata))
            {
                return true;
            }

            var radius = GetPointerToleranceRadius(viewingCamera, hits);
            hits = Physics.SphereCastAll(
                ray,
                radius,
                viewingCamera.farClipPlane,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);
            return TrySelectNearestIfc(hits, out selectedHit, out metadata);
        }
        finally
        {
            Physics.queriesHitBackfaces = previousBackfaceSetting;
        }
    }

    private static bool TrySelectNearestIfc(
        RaycastHit[] hits,
        out RaycastHit selectedHit,
        out IfcElementMetadata metadata)
    {
        selectedHit = default;
        metadata = null;
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

    private static float GetPointerToleranceRadius(
        Camera viewingCamera,
        RaycastHit[] referenceHits)
    {
        var referenceDistance = Mathf.Min(500f, viewingCamera.farClipPlane * 0.1f);
        foreach (var hit in referenceHits)
        {
            if (hit.distance > 0f)
            {
                referenceDistance = Mathf.Min(referenceDistance, hit.distance);
            }
        }

        var worldUnitsPerPixel = viewingCamera.orthographic
            ? viewingCamera.orthographicSize * 2f / Mathf.Max(1f, Screen.height)
            : 2f * referenceDistance *
              Mathf.Tan(viewingCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) /
              Mathf.Max(1f, Screen.height);
        return Mathf.Clamp(
            worldUnitsPerPixel * PointerTolerancePixels,
            0.05f,
            5f);
    }
}
