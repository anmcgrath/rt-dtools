using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTData.Utilities.RTMath
{
    public class Matrix4d
    {
        private double[] data;
        public double A00 { get { return data[0]; } set { data[0] = value; } }
        public double A10 { get { return data[1]; } set { data[1] = value; } }
        public double A20 { get { return data[2]; } set { data[2] = value; } }
        public double A30 { get { return data[3]; } set { data[3] = value; } }
        public double A01 { get { return data[4]; } set { data[4] = value; } }
        public double A11 { get { return data[5]; } set { data[5] = value; } }
        public double A21 { get { return data[6]; } set { data[6] = value; } }
        public double A31 { get { return data[7]; } set { data[7] = value; } }
        public double A02 { get { return data[8]; } set { data[8] = value; } }
        public double A12 { get { return data[9]; } set { data[9] = value; } }
        public double A22 { get { return data[10]; } set { data[10] = value; } }
        public double A32 { get { return data[11]; } set { data[11] = value; } }
        public double A03 { get { return data[12]; } set { data[12] = value; } }
        public double A13 { get { return data[13]; } set { data[13] = value; } }
        public double A23 { get { return data[14]; } set { data[14] = value; } }
        public double A33 { get { return data[15]; } set { data[15] = value; } }

        public Matrix4d()
        {
            data = new double[16];
        }

        public Matrix4d(double[] data)
        {
            this.data = data;
        }

        /// <summary>
        /// Left multiplies a matrix with this matrix and returns a new matrix, i.e result = this * matrix3d;
        /// </summary>
        /// <param name="matrix4d"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public Matrix4d LeftMultiply(Matrix4d matrix4d)
        {
            Matrix4d result = new Matrix4d();
            LeftMultiply(matrix4d, result);
            return result;
        }

        /// <summary>
        /// Left multiplies a matrix with this matrix and stores into result, i.e result = this * matrix3d;
        /// </summary>
        /// <param name="matrix4d"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public Matrix4d LeftMultiply(Matrix4d matrix4d, Matrix4d result)
        {
            //First column
            result.data[0] = data[0] * matrix4d.data[0] + data[4] * matrix4d.data[1] + data[8] * matrix4d.data[2] + data[12] * matrix4d.data[3];
            result.data[4] = data[0] * matrix4d.data[4] + data[4] * matrix4d.data[5] + data[8] * matrix4d.data[6] + data[12] * matrix4d.data[7];
            result.data[8] = data[0] * matrix4d.data[8] + data[4] * matrix4d.data[9] + data[8] * matrix4d.data[10] + data[12] * matrix4d.data[11];
            result.data[12] = data[0] * matrix4d.data[12] + data[4] * matrix4d.data[13] + data[8] * matrix4d.data[14] + data[12] * matrix4d.data[15];

            //Second column
            result.data[1] = data[1] * matrix4d.data[0] + data[5] * matrix4d.data[1] + data[9] * matrix4d.data[2] + data[13] * matrix4d.data[3];
            result.data[5] = data[1] * matrix4d.data[4] + data[5] * matrix4d.data[5] + data[9] * matrix4d.data[6] + data[13] * matrix4d.data[7];
            result.data[9] = data[1] * matrix4d.data[8] + data[5] * matrix4d.data[9] + data[9] * matrix4d.data[10] + data[13] * matrix4d.data[11];
            result.data[13] = data[1] * matrix4d.data[12] + data[5] * matrix4d.data[13] + data[9] * matrix4d.data[14] + data[13] * matrix4d.data[15];

            //Third column
            result.data[2] = data[2] * matrix4d.data[0] + data[6] * matrix4d.data[1] + data[10] * matrix4d.data[2] + data[14] * matrix4d.data[3];
            result.data[6] = data[2] * matrix4d.data[4] + data[6] * matrix4d.data[5] + data[10] * matrix4d.data[6] + data[14] * matrix4d.data[7];
            result.data[10] = data[2] * matrix4d.data[8] + data[6] * matrix4d.data[9] + data[10] * matrix4d.data[10] + data[14] * matrix4d.data[11];
            result.data[14] = data[2] * matrix4d.data[12] + data[6] * matrix4d.data[13] + data[10] * matrix4d.data[14] + data[14] * matrix4d.data[15];

            //Fourth column
            result.data[3] = data[3] * matrix4d.data[0] + data[7] * matrix4d.data[1] + data[11] * matrix4d.data[2] + data[15] * matrix4d.data[3];
            result.data[7] = data[3] * matrix4d.data[4] + data[7] * matrix4d.data[5] + data[11] * matrix4d.data[6] + data[15] * matrix4d.data[7];
            result.data[11] = data[3] * matrix4d.data[8] + data[7] * matrix4d.data[9] + data[11] * matrix4d.data[10] + data[15] * matrix4d.data[11];
            result.data[15] = data[3] * matrix4d.data[12] + data[7] * matrix4d.data[13] + data[11] * matrix4d.data[14] + data[15] * matrix4d.data[15];

            return result;
        }

        /// <summary>
        /// Returns the inverse of a 4x4 matrix
        /// </summary>
        /// <returns></returns>
        public Matrix4d Inverse()
        {
            var s0 = A00 * A11 - A10 * A01;
            var s1 = A00 * A12 - A10 * A02;
            var s2 = A00 * A13 - A10 * A03;
            var s3 = A01 * A12 - A11 * A02;
            var s4 = A01 * A13 - A11 * A03;
            var s5 = A02 * A13 - A12 * A03;

            var c5 = A22 * A33 - A32 * A23;
            var c4 = A21 * A33 - A31 * A23;
            var c3 = A21 * A32 - A31 * A22;
            var c2 = A20 * A33 - A30 * A23;
            var c1 = A20 * A32 - A30 * A22;
            var c0 = A20 * A31 - A30 * A21;

            // Should check for 0 determinant
            var invdet = 1.0 / (s0 * c5 - s1 * c4 + s2 * c3 + s3 * c2 - s4 * c1 + s5 * c0);

            Matrix4d newMatrix = new Matrix4d();

            newMatrix.A00 = (A11 * c5 - A12 * c4 + A13 * c3) * invdet;
            newMatrix.A01 = (-A01 * c5 + A02 * c4 - A03 * c3) * invdet;
            newMatrix.A02 = (A31 * s5 - A32 * s4 + A33 * s3) * invdet;
            newMatrix.A03 = (-A21 * s5 + A22 * s4 - A23 * s3) * invdet;

            newMatrix.A10 = (-A10 * c5 + A12 * c2 - A13 * c1) * invdet;
            newMatrix.A11 = (A00 * c5 - A02 * c2 + A03 * c1) * invdet;
            newMatrix.A12 = (-A30 * s5 + A32 * s2 - A33 * s1) * invdet;
            newMatrix.A13 = (A20 * s5 - A22 * s2 + A23 * s1) * invdet;

            newMatrix.A20 = (A10 * c4 - A11 * c2 + A13 * c0) * invdet;
            newMatrix.A21 = (-A00 * c4 + A01 * c2 - A03 * c0) * invdet;
            newMatrix.A22 = (A30 * s4 - A31 * s2 + A33 * s0) * invdet;
            newMatrix.A23 = (-A20 * s4 + A21 * s2 - A23 * s0) * invdet;

            newMatrix.A30 = (-A10 * c3 + A11 * c1 - A12 * c0) * invdet;
            newMatrix.A31 = (A00 * c3 - A01 * c1 + A02 * c0) * invdet;
            newMatrix.A32 = (-A30 * s3 + A31 * s1 - A32 * s0) * invdet;
            newMatrix.A33 = (A20 * s3 - A21 * s1 + A22 * s0) * invdet;

            return newMatrix;
        }

        public bool IsIdentity()
        {
            bool isIdentity = true;
            for(int i = 0; i < data.Length; i++)
            {
                if (i % 5 == 0) // on diagonal
                    isIdentity = isIdentity && (data[i] == 1);
                else
                    isIdentity = isIdentity && (data[i] == 0);
            }
            return isIdentity;
        }


        /// <summary>
        /// Left multiply a point by this matrix and return a new point as the result
        /// </summary>
        /// <param name="point4d"></param>
        /// <returns></returns>
        public Point4d LeftMultiply(Point4d point4d)
        {
            Point4d result = new Point4d();
            LeftMultiply(point4d, result);
            return result;
        }

        /// <summary>
        /// Left multiply a point by this matrix and store the result into result
        /// </summary>
        /// <param name="point4d"></param>
        /// <param name="result"></param>
        public void LeftMultiply(Point4d point4d, Point4d result)
        {
            result.X = data[0] * point4d.X + data[4] * point4d.Y + data[8] * point4d.Z + data[12] * point4d.T;
            result.Y = data[1] * point4d.X + data[5] * point4d.Y + data[9] * point4d.Z + data[13] * point4d.T;
            result.Z = data[2] * point4d.X + data[6] * point4d.Y + data[10] * point4d.Z + data[14] * point4d.T;
            result.T = data[3] * point4d.X + data[7] * point4d.Y + data[11] * point4d.Z + data[15] * point4d.T;
        }

        public static Matrix4d operator *(Matrix4d m1, Matrix4d m2)
        {
            return m1.LeftMultiply(m2);
        }

        public static Point4d operator *(Matrix4d m, Point4d p)
        {
            return m.LeftMultiply(p);
        }
    }
}
