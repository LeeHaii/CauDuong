using System;
using UnityEngine;

public static class IfcInteractionRaycaster
{
    private const float PointerTolerancePixels = 8f;
    private const float RayTriangleEpsilon = 0.000001f;

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

            var foundPhysicsHit = TrySelectNearestIfc(
                ray,
                hits,
                out selectedHit,
                out metadata);
            var foundOverviewHit = IfcStreamedModel.TryRaycastSurfaceOverview(
                ray,
                out var overviewHit);
            if (foundOverviewHit &&
                (!foundPhysicsHit || overviewHit.Distance < selectedHit.distance))
            {
                selectedHit = default;
                selectedHit.point = overviewHit.Point;
                selectedHit.normal = overviewHit.Normal;
                selectedHit.distance = overviewHit.Distance;
                metadata = overviewHit.Metadata;
                return true;
            }

            if (foundPhysicsHit)
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
            return TrySelectNearestIfc(ray, hits, out selectedHit, out metadata);
        }
        finally
        {
            Physics.queriesHitBackfaces = previousBackfaceSetting;
        }
    }

    private static bool TrySelectNearestIfc(
        Ray ray,
        RaycastHit[] hits,
        out RaycastHit selectedHit,
        out IfcElementMetadata metadata)
    {
        selectedHit = default;
        metadata = null;
        Array.Sort(hits, (left, right) => left.distance.CompareTo(right.distance));

        var nearestDistance = float.PositiveInfinity;
        foreach (var hit in hits)
        {
            var candidate = hit.transform.GetComponentInParent<IfcElementMetadata>();
            if (candidate == null)
            {
                continue;
            }

            if (hit.collider is MeshCollider)
            {
                if (hit.distance >= nearestDistance)
                {
                    continue;
                }

                selectedHit = hit;
                metadata = candidate;
                nearestDistance = hit.distance;
                continue;
            }

            // Large IFC meshes retain a cheap BoxCollider for broad-phase lookup.
            // A bounds hit is not a surface hit: roads, arches, and terrain can
            // leave most of that box empty. Confirm it against the readable render
            // mesh before allowing the element to be selected.
            if (hit.collider is not BoxCollider ||
                !TryRaycastReadableMesh(
                    ray,
                    hit.collider,
                    out var surfacePoint,
                    out var surfaceNormal,
                    out var surfaceDistance) ||
                surfaceDistance >= nearestDistance)
            {
                continue;
            }

            selectedHit = hit;
            selectedHit.point = surfacePoint;
            selectedHit.normal = surfaceNormal;
            selectedHit.distance = surfaceDistance;
            metadata = candidate;
            nearestDistance = surfaceDistance;
        }

        return metadata != null;
    }

    private static bool TryRaycastReadableMesh(
        Ray worldRay,
        Collider boundsCollider,
        out Vector3 worldPoint,
        out Vector3 worldNormal,
        out float worldDistance)
    {
        worldPoint = default;
        worldNormal = default;
        worldDistance = float.PositiveInfinity;
        if (!boundsCollider.TryGetComponent<MeshFilter>(out var meshFilter) ||
            meshFilter.sharedMesh == null ||
            !meshFilter.sharedMesh.isReadable)
        {
            return false;
        }

        var mesh = meshFilter.sharedMesh;
        var meshTransform = meshFilter.transform;
        var localOrigin = meshTransform.InverseTransformPoint(worldRay.origin);
        var localDirection = meshTransform.InverseTransformDirection(worldRay.direction);
        var vertices = mesh.vertices;
        var triangles = mesh.triangles;
        var found = false;
        for (var index = 0; index + 2 < triangles.Length; index += 3)
        {
            if (!TryIntersectTriangle(
                    localOrigin,
                    localDirection,
                    vertices[triangles[index]],
                    vertices[triangles[index + 1]],
                    vertices[triangles[index + 2]],
                    out var localDistance,
                    out var localNormal))
            {
                continue;
            }

            var candidatePoint = meshTransform.TransformPoint(
                localOrigin + localDirection * localDistance);
            var candidateDistance = Vector3.Distance(worldRay.origin, candidatePoint);
            if (candidateDistance >= worldDistance)
            {
                continue;
            }

            worldPoint = candidatePoint;
            worldNormal = meshTransform.TransformDirection(localNormal).normalized;
            if (Vector3.Dot(worldNormal, worldRay.direction) > 0f)
            {
                worldNormal = -worldNormal;
            }

            worldDistance = candidateDistance;
            found = true;
        }

        return found;
    }

    private static bool TryIntersectTriangle(
        Vector3 rayOrigin,
        Vector3 rayDirection,
        Vector3 vertex0,
        Vector3 vertex1,
        Vector3 vertex2,
        out float distance,
        out Vector3 normal)
    {
        distance = 0f;
        normal = default;
        var edge1 = vertex1 - vertex0;
        var edge2 = vertex2 - vertex0;
        var cross = Vector3.Cross(rayDirection, edge2);
        var determinant = Vector3.Dot(edge1, cross);
        if (Mathf.Abs(determinant) < RayTriangleEpsilon)
        {
            return false;
        }

        var inverseDeterminant = 1f / determinant;
        var originOffset = rayOrigin - vertex0;
        var u = Vector3.Dot(originOffset, cross) * inverseDeterminant;
        if (u < 0f || u > 1f)
        {
            return false;
        }

        var secondCross = Vector3.Cross(originOffset, edge1);
        var v = Vector3.Dot(rayDirection, secondCross) * inverseDeterminant;
        if (v < 0f || u + v > 1f)
        {
            return false;
        }

        distance = Vector3.Dot(edge2, secondCross) * inverseDeterminant;
        if (distance < 0f)
        {
            return false;
        }

        normal = Vector3.Cross(edge1, edge2).normalized;
        return true;
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
