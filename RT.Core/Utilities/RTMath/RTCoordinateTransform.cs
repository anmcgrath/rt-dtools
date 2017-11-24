using System;
using System.Collections.Generic;
using System.Text;

namespace RT.Core.Utilities.RTMath
{
    public class RTCoordinateTransform
    {
        private Matrix3d RCol = Matrix3d.GetRotationY(0);
        private Matrix3d RCouch = Matrix3d.GetRotationY(0);
        private Matrix3d RGantry = Matrix3d.GetRotationZ(0);
        private Matrix3d R = Matrix3d.GetRotationX(0);
        //Instantiate a point here to use in calculations
        private Point3d v = new Point3d();
        private Point3d u = new Point3d();

        public double GantryAngle { get { return _gantryAngle; } set { _gantryAngle = value; RGantry = Matrix3d.GetRotationZ(-value); calculateR(); } }
        private double _gantryAngle;

        public double CollimatorAngle { get { return _collimatorAngle; } set { _collimatorAngle = value; RCol = Matrix3d.GetRotationY(value); calculateR(); } }
        private double _collimatorAngle;

        public double CouchAngle { get { return _couchAngle; } set { _couchAngle = value; RCouch = Matrix3d.GetRotationY(value); calculateR(); } }
        private double _couchAngle;

        /// <summary>
        /// Calculate the rotation matrix for a point
        /// </summary>
        private void calculateR()
        {
            R = RCouch * RGantry * RCol;
        }

        public void Transform(Point3d point, Point3d isocentre, Point3d result)
        {
            point.Subtract(isocentre, v);
            R.LeftMultiply(v, u);
            u.Add(isocentre, result);
        }
    }
}
