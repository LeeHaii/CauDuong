using CauDuong.IfcStreaming;
using NUnit.Framework;
using UnityEngine;

public sealed class IfcVisibilityPlannerTests
{
    [Test]
    public void DetailPreload_KeepsOverviewVisibleUntilDetailIsReady()
    {
        var loading = IfcVisibilityPlanner.EvaluateRepresentation(
            IfcRepresentationState.OverviewOnly,
            true,
            false,
            true);

        Assert.That(loading.State, Is.EqualTo(IfcRepresentationState.PreloadingDetail));
        Assert.That(loading.ShowOverview, Is.True);
        Assert.That(loading.ShowDetail, Is.False);
        Assert.That(loading.KeepOverviewResident, Is.True);

        var ready = IfcVisibilityPlanner.EvaluateRepresentation(
            loading.State,
            true,
            true,
            true);

        Assert.That(ready.State, Is.EqualTo(IfcRepresentationState.DetailOnly));
        Assert.That(ready.ShowOverview, Is.False);
        Assert.That(ready.ShowDetail, Is.True);
    }

    [Test]
    public void ZoomOut_KeepsDetailVisibleUntilOverviewIsReady()
    {
        var loading = IfcVisibilityPlanner.EvaluateRepresentation(
            IfcRepresentationState.DetailOnly,
            false,
            true,
            false);

        Assert.That(loading.State, Is.EqualTo(IfcRepresentationState.PreloadingOverview));
        Assert.That(loading.ShowOverview, Is.False);
        Assert.That(loading.ShowDetail, Is.True);
        Assert.That(loading.KeepOverviewResident, Is.True);

        var ready = IfcVisibilityPlanner.EvaluateRepresentation(
            loading.State,
            false,
            true,
            true);

        Assert.That(ready.State, Is.EqualTo(IfcRepresentationState.OverviewOnly));
        Assert.That(ready.ShowOverview, Is.True);
        Assert.That(ready.ShowDetail, Is.False);
    }

    [Test]
    public void DetailOnly_RemainsVisibleWhileAdditionalCellsStream()
    {
        var decision = IfcVisibilityPlanner.EvaluateRepresentation(
            IfcRepresentationState.DetailOnly,
            true,
            false,
            false);

        Assert.That(decision.State, Is.EqualTo(IfcRepresentationState.DetailOnly));
        Assert.That(decision.ShowDetail, Is.True);
        Assert.That(decision.ShowOverview, Is.False);
    }

    [Test]
    public void VisibleAndPredictedCells_OutrankOffscreenCells()
    {
        var visible = IfcVisibilityPlanner.CalculateCellPriority(
            400f, false, true, true, 400f);
        var predicted = IfcVisibilityPlanner.CalculateCellPriority(
            100f, false, false, true, 25f);
        var offscreen = IfcVisibilityPlanner.CalculateCellPriority(
            10f, false, false, false, 10f);

        Assert.That(visible, Is.LessThan(predicted));
        Assert.That(predicted, Is.LessThan(offscreen));
    }

    [Test]
    public void ForcedCell_AlwaysHasHighestPriority()
    {
        var forced = IfcVisibilityPlanner.CalculateCellPriority(
            1_000_000f, true, false, false, 1_000_000f);
        var visible = IfcVisibilityPlanner.CalculateCellPriority(
            0f, false, true, true, 0f);

        Assert.That(forced, Is.LessThan(visible));
    }

    [Test]
    public void Eviction_ProtectsForcedAndHandoffCells()
    {
        Assert.That(
            IfcVisibilityPlanner.CanEvict(100f, 10f, true, false),
            Is.False);
        Assert.That(
            IfcVisibilityPlanner.CanEvict(100f, 10f, false, true),
            Is.False);
        Assert.That(
            IfcVisibilityPlanner.CanEvict(100f, 10f, false, false),
            Is.True);
        Assert.That(
            IfcVisibilityPlanner.CanEvict(1f, 10f, false, false),
            Is.False);
    }

    [Test]
    public void ForwardCone_RejectsCellsBehindCamera()
    {
        Assert.That(
            IfcVisibilityPlanner.IsInsideForwardCone(
                Vector3.zero, Vector3.forward, Vector3.forward * 10f, 0.5f),
            Is.True);
        Assert.That(
            IfcVisibilityPlanner.IsInsideForwardCone(
                Vector3.zero, Vector3.forward, Vector3.back * 10f, 0.5f),
            Is.False);
    }
}
