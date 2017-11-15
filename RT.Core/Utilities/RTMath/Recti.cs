using System;
using System.Collections.Generic;
using System.Text;

namespace RT.Core.Utilities.RTMath
{
    public class Recti
    {
        /// <summary>
        /// The X coordinate of the rectangle
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// The Y coordinate of the rectangle
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// The rectangle width
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// The rectangle height
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Creates a Rectd with at (x,y) with a specified width and height
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="heigt"></param>
        public Recti(int x, int y, int width, int height)
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
        public Recti(Point2d point1, Point2d point2)
        {
            var minX = (int)Math.Round(Math.Min(point1.X, point2.X));
            var minY = (int)Math.Round(Math.Min(point1.Y, point2.Y));
            var maxX = (int)Math.Round(Math.Max(point1.X, point2.X));
            var maxY = (int)Math.Round(Math.Max(point1.Y, point2.Y));
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
        public Recti Intersect(Recti rect)
        {
            int x1 = X + Width;
            int x2 = rect.X + rect.Width;
            int y1 = Y + Height;
            int y2 = rect.Y + rect.Height;

            int xL = Math.Max(X, rect.X);
            int xR = Math.Min(x1, x2);
            if (xR <= xL)
                return null;
            else
            {
                int yT = Math.Max(Y, rect.Y);
                int yB = Math.Min(y1, y2);
                if (yB <= yT)
                    return null;
                else
                    return new Recti(xL, yT, xR - xL, yB - yT);
            }
        }

        /// <summary>
        /// Returns the Rectangle that is the intersection of this rectangle with another. Returns null if there is no intersection
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public Recti Intersect(Rectd rect)
        {
            int x1 = X + Width;
            int x2 = (int)Math.Round(rect.X + rect.Width);
            int y1 = Y - Height;
            int y2 = (int)Math.Round(rect.Y - Height);

            int xL = (int)Math.Round(Math.Max(X, rect.X));
            int xR = Math.Min(x1, x2);
            if (xR <= xL)
                return null;
            else
            {
                int yT = (int)Math.Max(Y, rect.Y);
                int yB = (int)Math.Round((double)Math.Min(y1, y2));
                if (yB <= yT)
                    return null;
                else
                    return new Recti(xL, yT, xR - xL, yB - yT);
            }
        }

        /// <summary>
        /// Returns whether (x, y) is inside the rectangle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Contains(int x, int y)
        {
            return x >= X && x <= X + Width && y >= Y && y <= Y + Height;
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
