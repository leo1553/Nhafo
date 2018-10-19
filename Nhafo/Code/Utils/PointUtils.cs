using System;
using System.Windows;

namespace Nhafo.Code.Utils {
    public static class PointUtils {
        public static readonly Point Zero = new Point(0, 0);

        public static Point Sum(this Point point, Point other) {
            return new Point(point.X + other.X, point.Y + other.Y);
        }
        public static Point Sum(this Point point, Size size) {
            return new Point(point.X + size.Width, point.Y + size.Height);
        }

        public static Point Sub(this Point point, Point other) {
            return new Point(point.X - other.X, point.Y - other.Y);
        }

        public static Point Clamp(this Point point, Point min, Point max) {
            double x = point.X;
            double y = point.Y;

            if(x < min.X)
                x = min.X;
            else if(x > max.X)
                x = max.X;

            if(y < min.Y)
                y = min.Y;
            else if(y > max.Y)
                y = max.Y;

            return new Point(x, y);
        }

        public static double Distance(this Point point, Point other) {
            return Math.Sqrt(Math.Pow(point.X - other.X, 2) + Math.Pow(point.Y - other.Y, 2));
        }
    }
}
