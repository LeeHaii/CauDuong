using UnityEngine;

namespace CauDuong.IfcStreaming
{
    public readonly struct IfcSelectionPolicyDecision
    {
        public bool ShouldEnableBoundsCollider { get; }
        public bool MustKeepRenderMeshReadable { get; }
        public bool ShouldReleaseCpuMeshData { get; }
        public bool ShouldBePickable { get; }

        internal IfcSelectionPolicyDecision(
            bool shouldEnableBoundsCollider,
            bool mustKeepRenderMeshReadable,
            bool shouldReleaseCpuMeshData,
            bool shouldBePickable)
        {
            ShouldEnableBoundsCollider = shouldEnableBoundsCollider;
            MustKeepRenderMeshReadable = mustKeepRenderMeshReadable;
            ShouldReleaseCpuMeshData = shouldReleaseCpuMeshData;
            ShouldBePickable = shouldBePickable;
        }
    }

    public static class IfcSelectionPolicy
    {
        public static IfcSelectionPolicyDecision Evaluate(
            bool generateBoundsColliders,
            bool hasValidBounds,
            bool hasSelectionGeometry,
            bool releaseCpuMeshData,
            bool resident,
            bool elementVisible,
            bool detailVisible)
        {
            var shouldEnableBoundsCollider =
                generateBoundsColliders && hasValidBounds;
            var mustKeepRenderMeshReadable =
                shouldEnableBoundsCollider && !hasSelectionGeometry;
            var shouldReleaseCpuMeshData =
                releaseCpuMeshData && !mustKeepRenderMeshReadable;
            var shouldBePickable =
                shouldEnableBoundsCollider &&
                resident &&
                elementVisible &&
                detailVisible;

            return new IfcSelectionPolicyDecision(
                shouldEnableBoundsCollider,
                mustKeepRenderMeshReadable,
                shouldReleaseCpuMeshData,
                shouldBePickable);
        }

        public static bool HasValidBounds(Bounds bounds)
        {
            var center = bounds.center;
            var size = bounds.size;
            return IsFinite(center.x) &&
                   IsFinite(center.y) &&
                   IsFinite(center.z) &&
                   IsFinite(size.x) &&
                   IsFinite(size.y) &&
                   IsFinite(size.z) &&
                   size.x >= 0f &&
                   size.y >= 0f &&
                   size.z >= 0f &&
                   (size.x > 0f || size.y > 0f || size.z > 0f);
        }

        public static Vector3 WithMinimumThickness(
            Vector3 size,
            float minimumThickness)
        {
            var minimum = Mathf.Max(Mathf.Epsilon, minimumThickness);
            return new Vector3(
                Mathf.Max(size.x, minimum),
                Mathf.Max(size.y, minimum),
                Mathf.Max(size.z, minimum));
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }
}
