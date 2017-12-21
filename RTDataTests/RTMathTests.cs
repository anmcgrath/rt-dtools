using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RT.Core.Utilities.RTMath;

namespace RTDataTests
{
    [TestClass]
    public class RTMathTests
    {
        [TestMethod]
        public void Matrix4DMultiplicationTest()
        {
            double[] data1 = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            double[] data2 = new double[] { 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
            Matrix4d matrix1 = new Matrix4d(data1);
            Matrix4d matrix2 = new Matrix4d(data2);
            var m = matrix1 * matrix2;
            bool correct =
                m.A00 == 386 &&
                m.A10 == 444 &&
                m.A20 == 502 &&
                m.A30 == 560 &&

                m.A01 == 274 &&
                m.A11 == 316 &&
                m.A21 == 358 &&
                m.A31 == 400 &&

                m.A02 == 162 &&
                m.A12 == 188 &&
                m.A22 == 214 &&
                m.A32 == 240 &&

                m.A03 == 50 &&
                m.A13 == 60 &&
                m.A23 == 70 &&
                m.A33 == 80;

            Assert.IsTrue(correct);

            //Now left multiply a point4d
            var p = new Point4d(1, 2, 3, 4);
            var p2 = matrix1 * p;
            Assert.AreEqual(p2.X, 90);
            Assert.AreEqual(p2.Y, 100);
            Assert.AreEqual(p2.Z, 110);
            Assert.AreEqual(p2.T, 120);
            
        }

        [TestMethod]
        public void Matrix4DInversionTest()
        {
            double[] data = new double[] { 1, -2, 3, 4, 1, 2, -3, 4, 1, 2, 3, -4, 1, 2, 3, 4 };
            Matrix4d m = new Matrix4d(data);
            var inv = m.Inverse();
            var result = inv * m;

            Assert.IsTrue(result.IsIdentity());
        }

        [TestMethod]
        public void Matrix4DTestIsIdentity()
        {
            Matrix4d m = new Matrix4d(
                new double[] { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 });
            Matrix4d m2 = new Matrix4d(
                new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
            Assert.IsTrue(m.IsIdentity());
            Assert.IsFalse(m2.IsIdentity());
        }

        [TestMethod]
        public void Matrix3DMultiplication()
        {
            Matrix3d m1 = new Matrix3d(new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            Matrix3d m2 = new Matrix3d(new double[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 });
            var m = m1 * m2;
            bool correct =
                m.A00 == 90 &&
                m.A10 == 114 &&
                m.A20 == 138 &&

                m.A01 == 54 &&
                m.A11 == 69 &&
                m.A21 == 84 &&

                m.A02 == 18 &&
                m.A12 == 24 &&
                m.A22 == 30;
            Assert.IsTrue(correct);

            var p = new Point3d(1, 2, 3);
            var p2 = m1 * p;

            Assert.AreEqual(p2.X, 30);
            Assert.AreEqual(p2.Y, 36);
            Assert.AreEqual(p2.Z, 42);
        }

        [TestMethod]
        public void Point3dCrossTest()
        {
            var p1 = new Point3d(1, 2, 3);
            var p2 = new Point3d(3, 2, 1);
            var p = p1.Cross(p2);
            Assert.AreEqual(p.X, -4);
            Assert.AreEqual(p.Y, 8);
            Assert.AreEqual(p.Z, -4);
        }

        [TestMethod]
        public void Point3dDotProductTest()
        {
            var p1 = new Point3d(1, 2, 3);
            var p2 = new Point3d(3, 2, 1);
            var r = p1.Dot(p2);
            Assert.AreEqual(r, 10);
        }

        [TestMethod]
        public void Matrix3dDeterminantTest()
        {
            Matrix3d m = new Matrix3d();
            m.A00 = 6;
            m.A10 = 4;
            m.A20 = 2;
            m.A01 = 1;
            m.A11 = -2;
            m.A21 = 8;
            m.A02 = 1;
            m.A12 = 5;
            m.A22 = 7;
            Assert.AreEqual(-306,m.Determinate());
        }

    }
}
