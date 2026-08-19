using UnityEngine;

namespace CauDuong.IfcStreaming
{
    public enum IfcRepresentationState
    {
        OverviewOnly,
        PreloadingDetail,
        DetailOnly,
        PreloadingOverview
    }

    public readonly struct IfcRepresentationDecision
    {
        public IfcRepresentationState State { get; }
        public bool KeepOverviewResident { get; }
        public bool ShowOverview { get; }
        public bool ShowDetail { get; }

        public IfcRepresentationDecision(
            IfcRepresentationState state,
            bool keepOverviewResident,
            bool showOverview,
            bool showDetail)
        {
            State = state;
            KeepOverviewResident = keepOverviewResident;
            ShowOverview = showOverview;
            ShowDetail = showDetail;
        }
    }

    public static class IfcVisibilityPlanner
    {
        public const float ForcedPriority = -1_000_000f;
        public const float FrustumPriorityBias = -100_000f;
        public const float ForwardConePriorityBias = -10_000f;

        public static IfcRepresentationDecision EvaluateRepresentation(
            IfcRepresentationState current,
            bool wantsDetail,
            bool detailReady,
            bool overviewReady)
        {
            var next = current;
            if (wantsDetail)
            {
                var alreadyShowingDetail =
                    current == IfcRepresentationState.DetailOnly ||
                    current == IfcRepresentationState.PreloadingOverview;
                next = alreadyShowingDetail || detailReady
                    ? IfcRepresentationState.DetailOnly
                    : IfcRepresentationState.PreloadingDetail;
            }
            else
            {
                if (overviewReady || current == IfcRepresentationState.OverviewOnly)
                {
                    next = IfcRepresentationState.OverviewOnly;
                }
                else
                {
                    next = IfcRepresentationState.PreloadingOverview;
                }
            }

            // A representation is only hidden after its replacement is ready.
            // Preload states therefore keep the previously visible side active.
            var showOverview = next == IfcRepresentationState.OverviewOnly ||
                               next == IfcRepresentationState.PreloadingDetail;
            var showDetail = next == IfcRepresentationState.DetailOnly ||
                             next == IfcRepresentationState.PreloadingOverview;

            // A cold start near the model has no previous visible detail. Keep
            // the overview requested until detail can be activated atomically.
            var keepOverviewResident = showOverview ||
                                       next == IfcRepresentationState.PreloadingOverview;
            return new IfcRepresentationDecision(
                next,
                keepOverviewResident,
                showOverview,
                showDetail);
        }

        public static bool IsInsideForwardCone(
            Vector3 cameraPosition,
            Vector3 cameraForward,
            Vector3 boundsCenter,
            float cosineHalfAngle)
        {
            var offset = boundsCenter - cameraPosition;
            if (offset.sqrMagnitude <= Mathf.Epsilon)
            {
                return true;
            }

            return Vector3.Dot(cameraForward.normalized, offset.normalized) >=
                   Mathf.Clamp(cosineHalfAngle, -1f, 1f);
        }

        public static float CalculateCellPriority(
            float squaredDistance,
            bool forced,
            bool insideFrustum,
            bool insideForwardCone,
            float predictedSquaredDistance)
        {
            if (forced)
            {
                return ForcedPriority;
            }

            var priority = Mathf.Min(
                Mathf.Max(0f, squaredDistance),
                Mathf.Max(0f, predictedSquaredDistance));
            if (insideFrustum)
            {
                priority += FrustumPriorityBias;
            }
            else if (insideForwardCone)
            {
                priority += ForwardConePriorityBias;
            }

            return priority;
        }

        public static bool CanEvict(
            float victimPriority,
            float incomingPriority,
            bool forced,
            bool requiredForHandoff)
        {
            return !forced &&
                   !requiredForHandoff &&
                   victimPriority >= incomingPriority;
        }
    }
}
