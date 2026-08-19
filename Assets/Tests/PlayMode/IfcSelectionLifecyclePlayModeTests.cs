using System.Collections;
using CauDuong.IfcStreaming;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class IfcSelectionLifecyclePlayModeTests
{
    private const int IfcSelectionLayer = 8;

    [UnityTest]
    public IEnumerator BvhPath_KeepsBroadPhaseColliderAndReleasesMeshCpuCopy()
    {
        var owner = new GameObject("IFC selection fixture")
        {
            layer = IfcSelectionLayer
        };
        var mesh = CreateTriangleMesh();
        var collider = owner.AddComponent<BoxCollider>();
        var bvh = new IfcTriangleBvh(
            mesh.vertices,
            new[] { mesh.triangles });

        try
        {
            var decision = IfcSelectionPolicy.Evaluate(
                generateBoundsColliders: true,
                hasValidBounds: IfcSelectionPolicy.HasValidBounds(mesh.bounds),
                hasSelectionGeometry: bvh != null,
                releaseCpuMeshData: true,
                resident: true,
                elementVisible: true,
                detailVisible: true);
            collider.center = mesh.bounds.center;
            collider.size = IfcSelectionPolicy.WithMinimumThickness(
                mesh.bounds.size,
                0.001f);
            collider.enabled = decision.ShouldBePickable;
            if (decision.ShouldReleaseCpuMeshData)
            {
                mesh.UploadMeshData(true);
            }

            yield return new WaitForFixedUpdate();

            Assert.That(collider.enabled, Is.True);
            Assert.That(mesh.isReadable, Is.False);
            Assert.That(
                Physics.Raycast(
                    new Ray(Vector3.zero, Vector3.forward),
                    out var broadPhaseHit,
                    20f,
                    1 << IfcSelectionLayer,
                    QueryTriggerInteraction.Ignore),
                Is.True);
            Assert.That(broadPhaseHit.collider, Is.SameAs(collider));
            Assert.That(
                bvh.Raycast(
                    new Ray(Vector3.zero, Vector3.forward),
                    out var exactDistance,
                    out _),
                Is.True);
            Assert.That(exactDistance, Is.EqualTo(5f).Within(0.0001f));

            var hiddenDecision = IfcSelectionPolicy.Evaluate(
                generateBoundsColliders: true,
                hasValidBounds: true,
                hasSelectionGeometry: true,
                releaseCpuMeshData: true,
                resident: true,
                elementVisible: false,
                detailVisible: true);
            collider.enabled = hiddenDecision.ShouldBePickable;
            yield return new WaitForFixedUpdate();

            Assert.That(collider.enabled, Is.False);
            Assert.That(
                Physics.Raycast(
                    new Ray(Vector3.zero, Vector3.forward),
                    20f,
                    1 << IfcSelectionLayer,
                    QueryTriggerInteraction.Ignore),
                Is.False);
        }
        finally
        {
            Object.Destroy(owner);
            Object.Destroy(mesh);
        }
    }

    [Test]
    public void MissingBvh_KeepsReadableMeshForExactFallback()
    {
        var mesh = CreateTriangleMesh();
        try
        {
            var decision = IfcSelectionPolicy.Evaluate(
                generateBoundsColliders: true,
                hasValidBounds: true,
                hasSelectionGeometry: false,
                releaseCpuMeshData: true,
                resident: true,
                elementVisible: true,
                detailVisible: true);

            Assert.That(decision.ShouldEnableBoundsCollider, Is.True);
            Assert.That(decision.MustKeepRenderMeshReadable, Is.True);
            Assert.That(decision.ShouldReleaseCpuMeshData, Is.False);
            Assert.That(mesh.isReadable, Is.True);
        }
        finally
        {
            Object.DestroyImmediate(mesh);
        }
    }

    private static Mesh CreateTriangleMesh()
    {
        var mesh = new Mesh { name = "IFC selection triangle" };
        mesh.vertices = new[]
        {
            new Vector3(-1f, -1f, 5f),
            new Vector3(1f, -1f, 5f),
            new Vector3(0f, 1f, 5f)
        };
        mesh.triangles = new[] { 0, 1, 2 };
        mesh.RecalculateBounds();
        return mesh;
    }
}
