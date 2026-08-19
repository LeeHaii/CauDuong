using CauDuong.IfcStreaming;
using NUnit.Framework;
using UnityEngine;

public sealed class IfcSelectionPolicyTests
{
    [TestCase(false, false, true, false, false)]
    [TestCase(true, false, true, true, true)]
    [TestCase(true, true, true, true, false)]
    [TestCase(true, true, false, true, true)]
    public void Evaluate_SeparatesColliderCapabilityFromMeshReadability(
        bool generateColliders,
        bool hasSelectionBvh,
        bool releaseCpuMeshData,
        bool expectedCollider,
        bool expectedReadableMesh)
    {
        var decision = IfcSelectionPolicy.Evaluate(
            generateColliders,
            hasValidBounds: true,
            hasSelectionGeometry: hasSelectionBvh,
            releaseCpuMeshData: releaseCpuMeshData,
            resident: true,
            elementVisible: true,
            detailVisible: true);

        Assert.That(
            decision.ShouldEnableBoundsCollider,
            Is.EqualTo(expectedCollider));
        Assert.That(
            !decision.ShouldReleaseCpuMeshData,
            Is.EqualTo(expectedReadableMesh));
        Assert.That(
            decision.MustKeepRenderMeshReadable,
            Is.EqualTo(generateColliders && !hasSelectionBvh));
    }

    [TestCase(false, true, true, false)]
    [TestCase(true, false, true, false)]
    [TestCase(true, true, false, false)]
    [TestCase(true, true, true, true)]
    public void Evaluate_PickabilityRequiresResidentVisibleDetail(
        bool resident,
        bool elementVisible,
        bool detailVisible,
        bool expectedPickable)
    {
        var decision = IfcSelectionPolicy.Evaluate(
            generateBoundsColliders: true,
            hasValidBounds: true,
            hasSelectionGeometry: true,
            releaseCpuMeshData: true,
            resident: resident,
            elementVisible: elementVisible,
            detailVisible: detailVisible);

        Assert.That(decision.ShouldBePickable, Is.EqualTo(expectedPickable));
    }

    [Test]
    public void HasValidBounds_AcceptsThinSurfaceAndAddsMinimumThickness()
    {
        var bounds = new Bounds(Vector3.one, new Vector3(20f, 0f, 10f));

        Assert.That(IfcSelectionPolicy.HasValidBounds(bounds), Is.True);
        Assert.That(
            IfcSelectionPolicy.WithMinimumThickness(bounds.size, 0.01f),
            Is.EqualTo(new Vector3(20f, 0.01f, 10f)));
    }

    [Test]
    public void HasValidBounds_RejectsZeroAndNonFiniteBounds()
    {
        Assert.That(
            IfcSelectionPolicy.HasValidBounds(
                new Bounds(Vector3.zero, Vector3.zero)),
            Is.False);
        Assert.That(
            IfcSelectionPolicy.HasValidBounds(
                new Bounds(
                    new Vector3(float.NaN, 0f, 0f),
                    Vector3.one)),
            Is.False);
    }
}
