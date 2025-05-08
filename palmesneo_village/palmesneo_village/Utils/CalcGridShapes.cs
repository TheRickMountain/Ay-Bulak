using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public static class CalcGridShapes
    {
        public static bool IsPointInsideCircle(Point center, Point tile, float radius)
        {
            float dx = center.X - tile.X;
            float dy = center.Y - tile.Y;
            float distance_squared = dx * dx + dy * dy;
            return distance_squared <= radius * radius;
        }

        public static List<Point> GetFilledCirclePoints(Point center, float radius)
        {
            List<Point> points = new List<Point>();

            if (radius <= 0) return points;

            int top = (int)Math.Ceiling((double)center.Y - (double)radius);
            int bottom = (int)Math.Floor((double)center.Y + (double)radius);
            int left = (int)Math.Ceiling((double)center.X - (double)radius);
            int right = (int)Math.Floor((double)center.X + (double)radius);

            for (int y = top; y <= bottom; y++)
            {
                for (int x = left; x <= right; x++)
                {
                    Point tile = new Point(x, y);

                    if (IsPointInsideCircle(center, tile, radius))
                    {
                        points.Add(tile);
                    }
                }
            }

            return points;
        }
    }
}
