using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomPanel.Core.Utilities.RTMath
{
    public class Point3d
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Point3d() { }

        public Point3d(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Adds a point to this point
        /// </summary>
        /// <param name="point3d"></param>
        public void Add(Point3d point3d)
        {
            X += point3d.X;
            Y += point3d.Y;
            Z += point3d.Z;
        }

        /// <summary>
        /// Adds a point to this point and stores in result
        /// </summary>
        /// <param name="point3d"></param>
        /// <param name="result"></param>
        public void Add(Point3d point3d, Point3d result)
        {
            this.CopyTo(result);
            result.Add(point3d);
        }

        /// <summary>
        /// Subtracts a point from this point
        /// </summary>
        /// <param name="point3d"></param>
        public void Subtract(Point3d point3d)
        {
            X -= point3d.X;
            Y -= point3d.Y;
            Z -= point3d.Z;
        }

        /// <summary>
        /// Subtracts a point from this point and stores in result
        /// </summary>
        /// <param name="point3d"></param>
        /// <param name="result"></param>
        public void Subtract(Point3d point3d, Point3d result)
        {
            this.CopyTo(result);
            result.Subtract(point3d);
        }

        public void Multiply(double value)
        {
            X *= value;
            Y *= value;
            Z *= value;
        }


        /// <summary>
        /// Returns the dot product of this point with another point
        /// </summary>
        /// <param name="point3d"></param>
        /// <returns></returns>
        public double Dot(Point3d point3d)
        {
            return X * point3d.X + Y * point3d.Y + Z * point3d.Z;
        }

        /// <summary>
        /// Crosses a vector with this vector and returns the result as a new vector
        /// </summary>
        /// <param name="point3d"></param>
        public Point3d Cross(Point3d point3d)
        {
            Point3d result = new Point3d();
            Cross(point3d, result);
            return result;
        }

        /// <summary>
        /// Crosses a vector and stores the result in result
        /// </summary>
        /// <param name="point3d"></param>
        /// <param name="result"></param>
        public void Cross(Point3d point3d, Point3d result)
        {
            result.X = this.Y * point3d.Z - this.Z * point3d.Y;
            result.Y = this.Z * point3d.X - this.X * point3d.Z;
            result.Z = this.X * point3d.Y - this.Y * point3d.X;
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
            return X * X + Y * Y + Z * Z;
        }

        /// <summary>
        /// Copies this vector into another vector
        /// </summary>
        /// <param name="point3f"></param>
        public void CopyTo(Point3d point3f)
        {
            point3f.X = X;
            point3f.Y = Y;
            point3f.Z = Z;
        }

        public static Point3d operator +(Point3d p1, Point3d p2)
        {
            Point3d result = new Point3d();
            p1.CopyTo(result);
            result.Add(p2);
            return result;
        }

        public static Point3d operator -(Point3d p1, Point3d p2)
        {
            Point3d result = new Point3d();
            p1.CopyTo(result);
            result.Add(p2);
            return result;
        }

        public static Point3d operator *(Point3d point, double mult)
        {
            Point3d point2 = new Point3d();
            point.CopyTo(point2);
            point2.Multiply(mult);
            return point2;
        }

        public static Point3d operator *(double multi, Point3d point)
        {
            return point * multi;
        }

    }
}
