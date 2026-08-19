using CauDuong.IfcStreaming;
using NUnit.Framework;
using UnityEngine;

public sealed class IfcTriangleBvhTests
{
    [Test]
    public void Raycast_ReturnsNearestTriangleAndNormal()
    {
        var vertices = new[]
        {
            new Vector3(-1f, -1f, 5f),
            new Vector3(1f, -1f, 5f),
            new Vector3(0f, 1f, 5f),
            new Vector3(-1f, -1f, 10f),
            new Vector3(1f, -1f, 10f),
            new Vector3(0f, 1f, 10f)
        };
        var bvh = new IfcTriangleBvh(
            vertices,
            new[] { new[] { 0, 1, 2, 3, 4, 5 } });

        Assert.That(
            bvh.Raycast(new Ray(Vector3.zero, Vector3.forward), out var distance, out var normal),
            Is.True);
        Assert.That(distance, Is.EqualTo(5f).Within(0.0001f));
        Assert.That(Mathf.Abs(normal.z), Is.EqualTo(1f).Within(0.0001f));
    }

    [Test]
    public void Raycast_MissesOutsideBounds()
    {
        var bvh = new IfcTriangleBvh(
            new[]
            {
                new Vector3(-1f, -1f, 5f),
                new Vector3(1f, -1f, 5f),
                new Vector3(0f, 1f, 5f)
            },
            new[] { new[] { 0, 1, 2 } });

        Assert.That(
            bvh.Raycast(
                new Ray(new Vector3(10f, 10f, 0f), Vector3.forward),
                out _,
                out _),
            Is.False);
    }

    [Test]
    public void Constructor_CombinesSubMeshesAndReportsOwnedMemory()
    {
        var bvh = new IfcTriangleBvh(
            new[]
            {
                new Vector3(-1f, -1f, 5f),
                new Vector3(1f, -1f, 5f),
                new Vector3(0f, 1f, 5f),
                new Vector3(-1f, -1f, 10f),
                new Vector3(1f, -1f, 10f),
                new Vector3(0f, 1f, 10f)
            },
            new[] { new[] { 0, 1, 2 }, new[] { 3, 4, 5 } });

        Assert.That(bvh.VertexCount, Is.EqualTo(6));
        Assert.That(bvh.IndexCount, Is.EqualTo(6));
        Assert.That(bvh.TriangleCount, Is.EqualTo(2));
        Assert.That(bvh.NodeCount, Is.GreaterThanOrEqualTo(1));
        Assert.That(bvh.EstimatedBytes, Is.GreaterThan(0));
    }

    [Test]
    public void Raycast_IgnoresDegenerateTriangleAndFindsValidSurface()
    {
        var bvh = new IfcTriangleBvh(
            new[]
            {
                Vector3.zero,
                Vector3.zero,
                Vector3.zero,
                new Vector3(-1f, -1f, 4f),
                new Vector3(1f, -1f, 4f),
                new Vector3(0f, 1f, 4f)
            },
            new[] { new[] { 0, 1, 2, 3, 4, 5 } });

        Assert.That(
            bvh.Raycast(
                new Ray(Vector3.zero, Vector3.forward * 5f),
                out var distance,
                out _),
            Is.True);
        Assert.That(distance, Is.EqualTo(4f).Within(0.0001f));
    }
}
