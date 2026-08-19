using System;
using System.Collections.Generic;
using UnityEngine;

namespace CauDuong.IfcStreaming
{
    public sealed class IfcTriangleBvh
    {
        private const int LeafTriangleCount = 16;

        private struct Node
        {
            public Bounds Bounds;
            public int Left;
            public int Right;
            public int Start;
            public int Count;

            public bool IsLeaf => Count > 0;
        }

        private sealed class CentroidComparer : IComparer<int>
        {
            private readonly Vector3[] vertices;
            private readonly int[] triangleIndices;
            private readonly int axis;

            public CentroidComparer(
                Vector3[] vertices,
                int[] triangleIndices,
                int axis)
            {
                this.vertices = vertices;
                this.triangleIndices = triangleIndices;
                this.axis = axis;
            }

            public int Compare(int left, int right)
            {
                return ReadCentroid(left)[axis].CompareTo(ReadCentroid(right)[axis]);
            }

            private Vector3 ReadCentroid(int triangle)
            {
                var offset = triangle * 3;
                return (vertices[triangleIndices[offset]] +
                        vertices[triangleIndices[offset + 1]] +
                        vertices[triangleIndices[offset + 2]]) / 3f;
            }
        }

        private readonly Vector3[] vertices;
        private readonly int[] triangleIndices;
        private readonly Node[] nodes;

        public int TriangleCount => triangleIndices.Length / 3;
        public int VertexCount => vertices.Length;
        public int IndexCount => triangleIndices.Length;
        public int NodeCount => nodes.Length;
        public long EstimatedBytes =>
            (long)vertices.Length * sizeof(float) * 3L +
            (long)triangleIndices.Length * sizeof(int) +
            (long)nodes.Length * (sizeof(float) * 6L + sizeof(int) * 4L);

        public IfcTriangleBvh(Vector3[] vertices, IReadOnlyList<int[]> subMeshes)
        {
            this.vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
            if (subMeshes == null)
            {
                throw new ArgumentNullException(nameof(subMeshes));
            }

            var indexCount = 0;
            foreach (var subMesh in subMeshes)
            {
                indexCount += subMesh?.Length ?? 0;
            }

            if (indexCount == 0 || indexCount % 3 != 0)
            {
                throw new ArgumentException(
                    "Selection geometry must contain complete triangles.",
                    nameof(subMeshes));
            }

            var sourceIndices = new int[indexCount];
            var writeIndex = 0;
            foreach (var subMesh in subMeshes)
            {
                if (subMesh == null)
                {
                    continue;
                }

                Array.Copy(subMesh, 0, sourceIndices, writeIndex, subMesh.Length);
                writeIndex += subMesh.Length;
            }

            var triangleCount = indexCount / 3;
            var triangleOrder = new int[triangleCount];
            for (var triangle = 0; triangle < triangleCount; triangle++)
            {
                triangleOrder[triangle] = triangle;
            }

            var nodeList = new List<Node>(Mathf.Max(1, triangleCount / 8));
            BuildNode(
                vertices,
                sourceIndices,
                triangleOrder,
                nodeList,
                0,
                triangleCount);
            nodes = nodeList.ToArray();

            // Store triangles in leaf order so the temporary order array is not
            // retained at runtime.
            triangleIndices = new int[indexCount];
            for (var destinationTriangle = 0;
                 destinationTriangle < triangleCount;
                 destinationTriangle++)
            {
                var sourceTriangle = triangleOrder[destinationTriangle];
                Array.Copy(
                    sourceIndices,
                    sourceTriangle * 3,
                    triangleIndices,
                    destinationTriangle * 3,
                    3);
            }
        }

        public bool Raycast(
            Ray localRay,
            out float distance,
            out Vector3 normal)
        {
            distance = float.PositiveInfinity;
            normal = default;
            if (nodes.Length == 0 || localRay.direction.sqrMagnitude <= Mathf.Epsilon)
            {
                return false;
            }

            var normalizedRay = new Ray(
                localRay.origin,
                localRay.direction.normalized);
            return RaycastNode(0, normalizedRay, ref distance, ref normal);
        }

        private bool RaycastNode(
            int nodeIndex,
            Ray ray,
            ref float nearestDistance,
            ref Vector3 nearestNormal)
        {
            var node = nodes[nodeIndex];
            if (!node.Bounds.IntersectRay(ray))
            {
                return false;
            }

            if (!node.IsLeaf)
            {
                var hitLeft = RaycastNode(
                    node.Left,
                    ray,
                    ref nearestDistance,
                    ref nearestNormal);
                var hitRight = RaycastNode(
                    node.Right,
                    ray,
                    ref nearestDistance,
                    ref nearestNormal);
                return hitLeft || hitRight;
            }

            var found = false;
            for (var triangle = node.Start;
                 triangle < node.Start + node.Count;
                 triangle++)
            {
                var offset = triangle * 3;
                if (!TryIntersectTriangle(
                        ray,
                        vertices[triangleIndices[offset]],
                        vertices[triangleIndices[offset + 1]],
                        vertices[triangleIndices[offset + 2]],
                        out var hitDistance,
                        out var hitNormal) ||
                    hitDistance >= nearestDistance)
                {
                    continue;
                }

                nearestDistance = hitDistance;
                nearestNormal = hitNormal;
                found = true;
            }

            return found;
        }

        private static int BuildNode(
            Vector3[] vertices,
            int[] triangleIndices,
            int[] triangleOrder,
            List<Node> nodes,
            int start,
            int count)
        {
            var bounds = default(Bounds);
            var centroidBounds = default(Bounds);
            var hasBounds = false;
            for (var index = start; index < start + count; index++)
            {
                var triangle = triangleOrder[index];
                var offset = triangle * 3;
                var vertex0 = vertices[triangleIndices[offset]];
                var vertex1 = vertices[triangleIndices[offset + 1]];
                var vertex2 = vertices[triangleIndices[offset + 2]];
                var triangleBounds = new Bounds(vertex0, Vector3.zero);
                triangleBounds.Encapsulate(vertex1);
                triangleBounds.Encapsulate(vertex2);
                var centroid = (vertex0 + vertex1 + vertex2) / 3f;
                if (!hasBounds)
                {
                    bounds = triangleBounds;
                    centroidBounds = new Bounds(centroid, Vector3.zero);
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(triangleBounds);
                    centroidBounds.Encapsulate(centroid);
                }
            }

            var nodeIndex = nodes.Count;
            nodes.Add(default);
            if (count <= LeafTriangleCount)
            {
                nodes[nodeIndex] = new Node
                {
                    Bounds = bounds,
                    Start = start,
                    Count = count,
                    Left = -1,
                    Right = -1
                };
                return nodeIndex;
            }

            var size = centroidBounds.size;
            var axis = size.x >= size.y && size.x >= size.z
                ? 0
                : size.y >= size.z
                    ? 1
                    : 2;
            Array.Sort(
                triangleOrder,
                start,
                count,
                new CentroidComparer(vertices, triangleIndices, axis));
            var leftCount = count / 2;
            var left = BuildNode(
                vertices,
                triangleIndices,
                triangleOrder,
                nodes,
                start,
                leftCount);
            var right = BuildNode(
                vertices,
                triangleIndices,
                triangleOrder,
                nodes,
                start + leftCount,
                count - leftCount);
            nodes[nodeIndex] = new Node
            {
                Bounds = bounds,
                Left = left,
                Right = right,
                Start = 0,
                Count = 0
            };
            return nodeIndex;
        }

        private static bool TryIntersectTriangle(
            Ray ray,
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
            var firstCross = Vector3.Cross(ray.direction, edge2);
            var determinant = Vector3.Dot(edge1, firstCross);
            if (Mathf.Abs(determinant) < 1e-8f)
            {
                return false;
            }

            var inverseDeterminant = 1f / determinant;
            var originOffset = ray.origin - vertex0;
            var u = Vector3.Dot(originOffset, firstCross) * inverseDeterminant;
            if (u < 0f || u > 1f)
            {
                return false;
            }

            var secondCross = Vector3.Cross(originOffset, edge1);
            var v = Vector3.Dot(ray.direction, secondCross) * inverseDeterminant;
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
    }
}
