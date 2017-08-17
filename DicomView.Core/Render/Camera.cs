using DicomPanel.Core.Geometry;
using DicomPanel.Core.Render;
using DicomPanel.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DicomPanel.Core.Render
{
    /// The camera represents a plane inside a 3D grid with ColDir and RowDir running on the plane. The posiiton is supposed to represent the "center" of the collimated view plane.
    /// </summary>
    public class Camera
    {
        public Point3d Position { get; set; }
        public Point3d ColDir { get; private set; }
        private double colDirLength { get; set; }
        public Point3d RowDir { get; private set; }
        private double rowDirLength { get; set; }
        public Point3d Normal { get; private set; }
       
        public double Scale { get; set; }
        public double MMPerPixel { get; set; }

        public bool IsAxial { get { return (Normal.Z != 0 && Normal.X == 0 && Normal.Y == 0); } }

        public Camera()
        {
            ColDir = new Point3d();
            RowDir = new Point3d();
            Normal = new Point3d();
            Position = new Point3d();
            Scale = 1;
            MMPerPixel = 1;
            SetAxial();
        }

        /// <summary>
        /// Moves the camera by the specified amount
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Move(double x, double y, double z)
        {
            Position.X += x;
            Position.Y += y;
            Position.Z += z;
        }

        /// <summary>
        /// Moves the camera by the specified amount
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Move(Point3d amount)
        {
            Position.Add(amount);
        }

        public void Zoom(double amount)
        {
            Scale *= amount;
        }

        public void ZoomIn()
        {
            Scale += .05;
        }

        public void ZoomOut()
        {
            Scale -= .05;
        }

        /// <summary>
        /// Move to a position in the patient coordinate system
        /// </summary>
        /// <param name="x">The x position (mm)</param>
        /// <param name="y">The y position (mm)</param>
        /// <param name="z">The z position (mm)</param>
        public void MoveTo(double x, double y, double z)
        {
            Position.X = x;
            Position.Y = y;
            Position.Z = z;
        }

        public void SetAxial()
        {
            ColDir.X = 1;
            ColDir.Y = 0;
            ColDir.Z = 0;
            RowDir.X = 0;
            RowDir.Y = 1;
            RowDir.Z = 0;
            Normal = ColDir.Cross(RowDir);
        }
        public void SetSagittal()
        {
            ColDir.X = 0;
            ColDir.Y = -1;
            ColDir.Z = 0;
            RowDir.X = 0;
            RowDir.Y = 0;
            RowDir.Z = -1;
            Normal = ColDir.Cross(RowDir);
        }
        public void SetCoronal()
        {
            ColDir.X = 1;
            ColDir.Y = 0;
            ColDir.Z = 0;
            RowDir.X = 0;
            RowDir.Y = 0;
            RowDir.Z = -1;
            Normal = ColDir.Cross(RowDir);
        }

        /// <summary>
        /// Scrolls through patient slices in the direciton of the camera normal vector
        /// </summary>
        /// <param name="direction">Positive or negative value indicating direction with respect to the normal vector</param>
        public void Scroll(double direction, double amount)
        {
            if (direction == 0)
                return;

            Point3d movDir = new Point3d();
            Normal.CopyTo(movDir);
            movDir *= (direction / Math.Abs(direction));
            Position += movDir * amount;
            Normal = ColDir.Cross(RowDir);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="thetax">Angle around the x axis in degrees</param>
        /// <param name="thetay">Angle around the y axis in degrees</param>
        /// <param name="thetaz">Angle around the z axis in degrees</param>
        public void Rotate(double thetax, double thetay, double thetaz)
        {
            double r_thetax = thetax * Math.PI / 180;
            double r_thetay = thetay * Math.PI / 180;
            double r_thetaz = thetaz * Math.PI / 180;
            var Rx = Matrix3d.GetRotationX(thetax);
            var Ry = Matrix3d.GetRotationY(thetay);
            var Rz = Matrix3d.GetRotationZ(thetaz);
            ColDir = Rx * Ry * Rz * ColDir;
            RowDir = Rx * Ry * Rz * RowDir;
            colDirLength = ColDir.Length();
            rowDirLength = RowDir.Length();
            Normal = ColDir.Cross(RowDir);
        }

        public WorldPointTranslator CreateTranslator(IRenderContext context)
        {
            return WorldPointTranslator.Create(this, context);
        }
    }
}
