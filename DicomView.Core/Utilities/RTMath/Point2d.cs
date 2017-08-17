using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomPanel.Core.Utilities.RTMath
{
    public class Point2d
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point2d() { }

        public Point2d(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Adds a point to this point
        /// </summary>
        /// <param name="point3d"></param>
        public void Add(Point2d point)
        {
            X += point.X;
            Y += point.Y;
        }

        /// <summary>
        /// Adds a point to this point and stores in result
        /// </summary>
        /// <param name="point3d"></param>
        /// <param name="result"></param>
        public void Add(Point2d point, Point2d result)
        {
            this.CopyTo(point);
            result.Add(point);
        }

        /// <summary>
        /// Subtracts a point from this point
        /// </summary>
        /// <param name="point3d"></param>
        public void Subtract(Point2d point)
        {
            X -= point.X;
            Y -= point.Y;
        }

        /// <summary>
        /// Subtracts a point from this point and stores in result
        /// </summary>
        /// <param name="point3d"></param>
        /// <param name="result"></param>
        public void Subtract(Point2d point, Point2d result)
        {
            this.CopyTo(result);
            result.Subtract(point);
        }

        public void Multiply(double value)
        {
            X *= value;
            Y *= value;
        }


        /// <summary>
        /// Returns the dot product of this point with another point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double Dot(Point2d point)
        {
            return X * point.X + Y * point.Y;
        }

        /// <summary>
        /// Returns the Euclidean length of this vector
        /// </summary>
        public double Length()
        {
            return Math.Sqrt(LengthSquared());
        }

        /// <summary>
        /// Returns the squared Euclidian length of this vector
        /// </summary>
        /// <returns></returns>
        public double LengthSquared()
        {
            return X * X + Y * Y;
        }

        /// <summary>
        /// Copies this vector into another vector
        /// </summary>
        /// <param name="point3f"></param>
        public void CopyTo(Point2d point)
        {
            point.X = X;
            point.Y = Y;
        }

        public static Point2d operator +(Point2d p1, Point2d p2)
        {
            Point2d result = new Point2d();
            p1.CopyTo(result);
            result.Add(p2);
            return result;
        }

        public static Point2d operator -(Point2d p1, Point2d p2)
        {
            Point2d result = new Point2d();
            p1.CopyTo(result);
            result.Add(p2);
            return result;
        }

        public static Point2d operator *(Point2d point, double mult)
        {
            Point2d point2 = new Point2d();
            point.CopyTo(point2);
            point2.Multiply(mult);
            return point2;
        }

        public static Point2d operator *(double multi, Point2d point)
        {
            return point * multi;
        }

    }
}
