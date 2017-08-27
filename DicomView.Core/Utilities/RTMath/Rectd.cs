using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Utilities.RTMath
{
    public class Rectd
    {
        /// <summary>
        /// The X coordinate of the rectangle
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// The Y coordinate of the rectangle
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// The rectangle width
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// The rectangle height
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Creates a Rectd with at (x,y) with a specified width and height
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="heigt"></param>
        public Rectd(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Creates a Rectd of the smallest size that contains both point1 and point2
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        public Rectd(Point2d point1, Point2d point2)
        {
            var minX = Math.Min(point1.X, point2.X);
            var minY = Math.Min(point1.Y, point2.Y);
            var maxX = Math.Max(point1.X, point2.X);
            var maxY = Math.Max(point1.Y, point2.Y);
            X = minX;
            Y = minY;
            Width = maxX - minX;
            Height = maxY - minY;
        }

        /// <summary>
        /// Returns the Rectangle that is the intersection of this rectangle with another. Returns null if there is no intersection
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public Rectd Intersect(Rectd rect)
        {
            double x1 = X + Width;
            double x2 = rect.X + rect.Width;
            double y1 = Y + Height;
            double y2 = rect.Y + rect.Height;

            double xL = Math.Max(X, rect.X);
            double xR = Math.Min(x1, x2);
            if (xR <= xL)
                return null;
            else
            {
                double yB = Math.Min(y1, y2);
                double yT = Math.Max(Y, rect.Y);
                if (yB <= yT)
                    return null;
                else
                    return new Rectd(xL, yT, xR - xL, yB - yT);
            }
        }

        public Rectd Intersect(Recti rect)
        {
            double x1 = X + Width;
            double x2 = rect.X + rect.Width;
            double y1 = Y + Height;
            double y2 = rect.Y + rect.Height;

            double xL = Math.Max(X, rect.X);
            double xR = Math.Min(x1, x2);
            if (xR <= xL)
                return null;
            else
            {
                double yB = Math.Min(y1, y2);
                double yT = Math.Max(Y, rect.Y);
                if (yB <= yT)
                    return null;
                else
                    return new Rectd(xL, yT, xR - xL, yB - yT);
            }
        }

        /// <summary>
        /// Returns whether (x, y) is inside the rectangle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Contains(double x, double y)
        {
            return x >= X && x <= X + Width && y >= Y && y <= Y + Height;
        }

        /// <summary>
        /// Returns whether a point is inside the rectangle
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Point2d point)
        {
            return Contains(point.X, point.Y);
        }
    }
}
