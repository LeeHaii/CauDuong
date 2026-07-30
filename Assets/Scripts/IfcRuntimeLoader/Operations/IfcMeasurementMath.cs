using System.Collections.Generic;
using UnityEngine;

namespace CauDuong.IfcOperations
{
    public static class IfcMeasurementMath
    {
        public static float Distance(Vector3 first, Vector3 second)
        {
            return Vector3.Distance(first, second);
        }

        public static float Height(Vector3 first, Vector3 second)
        {
            return Mathf.Abs(second.y - first.y);
        }

        public static float PolygonArea(IReadOnlyList<Vector3> points)
        {
            if (points == null || points.Count < 3)
            {
                return 0f;
            }

            var crossSum = Vector3.zero;
            for (var index = 0; index < points.Count; index++)
            {
                crossSum += Vector3.Cross(
                    points[index],
                    points[(index + 1) % points.Count]);
            }

            return crossSum.magnitude * 0.5f;
        }
    }
}
