using Stride.Core.Mathematics;
using System.Collections.Generic;

namespace _2D.Geometry
{
    public class Utils
    {
        public static bool PolygonContainsPoint(Vector2 point, List<Vector2> polygon)
        {
            int numPolygons = polygon.Count;
            bool isInsidePolygon = false;

            var i = 0;
            int j = numPolygons - 1;

            for (; i < numPolygons; j = i++)
            {
                if (((polygon[i].Y > point.Y) !=
                    (polygon[j].Y > point.Y)) &&
                    (point.X < (polygon[j].X - polygon[i].X) *
                    (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) +
                    polygon[i].X))
                {
                    isInsidePolygon = !isInsidePolygon;
                }
            }
            return isInsidePolygon;
        }
    }
}