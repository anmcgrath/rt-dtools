using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTData.Utilities.RTMath
{
    public class Point4d
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double T { get; set; }

        public Point4d() { }

        public Point4d(double x, double y, double z, double t)
        {
            X = x;
            Y = y;
            Z = z;
            T = t;
        }

        /// <summary>
        /// Adds a point to this point
        /// </summary>
        /// <param name="point3d"></param>
        public void Add(Point4d point4d)
        {
            X += point4d.X;
            Y += point4d.Y;
            Z += point4d.Z;
            T += point4d.T;
        }

        /// <summary>
        /// Adds a point to this point and stores in result
        /// </summary>
        /// <param name="point3d"></param>
        /// <param name="result"></param>
        public void Add(Point4d point4d, Point4d result)
        {
            this.CopyTo(result);
            result.Add(point4d);
        }

        /// <summary>
        /// Subtracts a point from this point
        /// </summary>
        /// <param name="point3d"></param>
        public void Subtract(Point4d point4d)
        {
            X -= point4d.X;
            Y -= point4d.Y;
            Z -= point4d.Z;
            T -= point4d.T;
        }

        /// <summary>
        /// Subtracts a point from this point and stores in result
        /// </summary>
        /// <param name="point3d"></param>
        /// <param name="result"></param>
        public void Subtract(Point4d point4d, Point4d result)
        {
            this.CopyTo(result);
            result.Subtract(point4d);
        }


        /// <summary>
        /// Returns the dot product of this point with another point
        /// </summary>
        /// <param name="point3d"></param>
        /// <returns></returns>
        public double Dot(Point4d point4d)
        {
            return X * point4d.X + Y * point4d.Y + Z * point4d.Z + T * point4d.T;
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
            return X * X + Y * Y + Z * Z + T * T;
        }

        /// <summary>
        /// Copies this vector into another vector
        /// </summary>
        /// <param name="point3f"></param>
        public void CopyTo(Point4d point4d)
        {
            point4d.X = X;
            point4d.Y = Y;
            point4d.Z = Z;
            point4d.T = T;
        }

        public static Point4d operator +(Point4d p1, Point4d p2)
        {
            Point4d result = new Point4d();
            p1.CopyTo(result);
            result.Add(p2);
            return result;
        }

        public static Point4d operator -(Point4d p1, Point4d p2)
        {
            Point4d result = new Point4d();
            p1.CopyTo(result);
            result.Add(p2);
            return result;
        }
    }
}
