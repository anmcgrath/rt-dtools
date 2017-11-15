using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.Utilities.RTMath
{
    public class Matrix3d
    {
        private double[] data;
        public double A00 { get { return data[0]; } set { data[0] = value; } }
        public double A10 { get { return data[1]; } set { data[1] = value; } }
        public double A20 { get { return data[2]; } set { data[2] = value; } }
        public double A01 { get { return data[3]; } set { data[3] = value; } }
        public double A11 { get { return data[4]; } set { data[4] = value; } }
        public double A21 { get { return data[5]; } set { data[5] = value; } }
        public double A02 { get { return data[6]; } set { data[6] = value; } }
        public double A12 { get { return data[7]; } set { data[7] = value; } }
        public double A22 { get { return data[8]; } set { data[8] = value; } }

        public Matrix3d()
        {
            data = new double[9];
        }

        public Matrix3d(double[] data)
        {
            this.data = data;
        }

        /// <summary>
        /// Left multiplies a matrix with this matrix and returns a new matrix, i.e result = this * matrix3d;
        /// </summary>
        /// <param name="matrix3d"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public Matrix3d LeftMultiply(Matrix3d matrix3d)
        {
            Matrix3d result = new Matrix3d();
            LeftMultiply(matrix3d, result);
            return result;
        }

        /// <summary>
        /// Left multiplies a matrix with this matrix and stores into result, i.e result = this * matrix3d;
        /// </summary>
        /// <param name="matrix3d"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public Matrix3d LeftMultiply(Matrix3d matrix3d, Matrix3d result)
        {
            //First column
            result.data[0] = data[0] * matrix3d.data[0] + data[3] * matrix3d.data[1] + data[6] * matrix3d.data[2];
            result.data[3] = data[0] * matrix3d.data[3] + data[3] * matrix3d.data[4] + data[6] * matrix3d.data[5];
            result.data[6] = data[0] * matrix3d.data[6] + data[3] * matrix3d.data[7] + data[6] * matrix3d.data[8];

            //Second column
            result.data[1] = data[1] * matrix3d.data[0] + data[4] * matrix3d.data[1] + data[7] * matrix3d.data[2];
            result.data[4] = data[1] * matrix3d.data[3] + data[4] * matrix3d.data[4] + data[7] * matrix3d.data[5];
            result.data[7] = data[1] * matrix3d.data[6] + data[4] * matrix3d.data[7] + data[7] * matrix3d.data[8];

            //Third column
            result.data[2] = data[2] * matrix3d.data[0] + data[5] * matrix3d.data[1] + data[8] * matrix3d.data[2];
            result.data[5] = data[2] * matrix3d.data[3] + data[5] * matrix3d.data[4] + data[8] * matrix3d.data[5];
            result.data[8] = data[2] * matrix3d.data[6] + data[5] * matrix3d.data[7] + data[8] * matrix3d.data[8];

            return result;
        }


        /// <summary>
        /// Left multiply a point by this matrix and return a new point as the result
        /// </summary>
        /// <param name="point3d"></param>
        /// <returns></returns>
        public Point3d LeftMultiply(Point3d point3d)
        {
            Point3d result = new Point3d();
            LeftMultiply(point3d, result);
            return result;
        }

        /// <summary>
        /// Left multiply a point by this matrix and store the result into result
        /// </summary>
        /// <param name="point3d"></param>
        /// <param name="result"></param>
        public void LeftMultiply(Point3d point3d, Point3d result)
        {
            result.X = data[0] * point3d.X + data[3] * point3d.Y + data[6] * point3d.Z;
            result.Y = data[1] * point3d.X + data[4] * point3d.Y + data[7] * point3d.Z;
            result.Z = data[2] * point3d.X + data[5] * point3d.Y + data[8] * point3d.Z;
        }

        public static Matrix3d operator *(Matrix3d m1, Matrix3d m2)
        {
            return m1.LeftMultiply(m2);
        }

        public static Point3d operator *(Matrix3d m1, Point3d p1)
        {
            return m1.LeftMultiply(p1);
        }

        /// <summary>
        /// Get the rotation Matrix to rotate a vector thetax degrees around the x-axis.
        /// </summary>
        /// <param name="thetax"></param>
        /// <returns></returns>
        public static Matrix3d GetRotationX(double thetax)
        {
            double r_thetax = thetax * Math.PI / 180;
            Matrix3d Rx = new Matrix3d();
            Rx.A00 = 1; Rx.A01 = 0; Rx.A02 = 0;
            Rx.A10 = 0; Rx.A11 = Math.Cos(r_thetax); Rx.A12 = -Math.Sin(r_thetax);
            Rx.A20 = 0; Rx.A21 = Math.Sin(r_thetax); Rx.A22 = Math.Cos(r_thetax);
            return Rx;
        }

        /// <summary>
        /// Get the rotation Matrix to rotate a vector thetax degrees around the y-axis.
        /// </summary>
        /// <param name="thetay"></param>
        /// <returns></returns>
        public static Matrix3d GetRotationY(double thetay)
        {
            double r_thetay = thetay * Math.PI / 180;
            Matrix3d Ry = new Matrix3d();
            Ry.A00 = Math.Cos(r_thetay); Ry.A01 = 0; Ry.A02 = Math.Sin(r_thetay);
            Ry.A10 = 0; Ry.A11 = 1; Ry.A12 = 0;
            Ry.A20 = -Math.Sin(r_thetay); Ry.A21 = 0; Ry.A22 = Math.Cos(r_thetay);
            return Ry;
        }

        /// <summary>
        /// Get the rotation Matrix to rotate a vector thetax degrees around the z-axis.
        /// </summary>
        /// <param name="thetaz"></param>
        /// <returns></returns>
        public static Matrix3d GetRotationZ(double thetaz)
        {
            double r_thetaz = thetaz * Math.PI / 180;
            Matrix3d Rz = new Matrix3d();
            Rz.A00 = Math.Cos(r_thetaz); Rz.A01 = Math.Sin(r_thetaz); Rz.A02 = 0;
            Rz.A10 = Math.Sin(r_thetaz); Rz.A11 = Math.Cos(r_thetaz); Rz.A12 = 0;
            Rz.A20 = 0; Rz.A21 = 0; Rz.A22 = 1;
            return Rz;
        }

        /// <summary>
        /// Get the rotation Matrix to rotate a vector theta degrees around the vector u
        /// </summary>
        /// <param name="theta"></param>
        /// <param name="uvec"></param>
        /// <returns></returns>
        public static Matrix3d GetRotation(double theta, Point3d uvec)
        {
            theta = theta * Math.PI / 180;
            var u = new Point3d();
            uvec.CopyTo(u);
            u /= u.Length();
            var cost = Math.Cos(theta);
            var sint = Math.Sin(theta);

            Matrix3d mat = new Matrix3d(new double[]
            {

                cost + u.X*u.X*(1-cost),
                u.Y*u.X*(1-cost)+u.Z*sint,
                u.Z*u.X*(1-cost)-u.Y*sint,
                u.X*u.Y*(1-cost)-u.Z*sint,
                cost+u.Y*u.Y*(1-cost),
                u.Z*u.Y*(1-cost)+u.X*sint,
                u.X*u.Z*(1-cost)+u.Y*sint,
                u.Y*u.Z*(1-cost)-u.X*sint,
                cost+u.Z*u.Z*(1-cost)

            });
            return mat;
        }
    }
}
