using DicomPanel.Core.Geometry;
using DicomPanel.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Render
{
    public partial class Camera
    {
        /// <summary>
        /// Some cached doubles for computing world to screen points
        /// </summary>
        private double tlx;
        private double tly;
        private double tlz;

        CoordinateTransform t = new CoordinateTransform();

        private Matrix3d _screenToWorldMatrix { get; set; }

        private void cacheWorldToScreenVariables()
        {
            tlx = Position.X - (ColDir.X * FOV.X * MMPerPixel / Scale + RowDir.X * FOV.Y * MMPerPixel / Scale) / 2;
            tly = Position.Y - (ColDir.Y * FOV.X * MMPerPixel / Scale + RowDir.Y * FOV.Y * MMPerPixel / Scale) / 2;
            tlz = Position.Z - (ColDir.Z * FOV.X * MMPerPixel / Scale + RowDir.Z * FOV.Y * MMPerPixel / Scale) / 2;
        }

        private void createScreenToWorldMatrix()
        {
            _screenToWorldMatrix.A00 = ColDir.X / Scale;
            _screenToWorldMatrix.A01 = RowDir.X / Scale;
            _screenToWorldMatrix.A02 = Position.X - (ColDir.X * FOV.X * MMPerPixel / Scale + RowDir.X * FOV.Y * MMPerPixel / Scale) / 2;

            _screenToWorldMatrix.A10 = ColDir.Y / Scale;
            _screenToWorldMatrix.A11 = RowDir.Y / Scale;
            _screenToWorldMatrix.A12 = Position.Y - (ColDir.Y * FOV.X * MMPerPixel / Scale + RowDir.Y * FOV.Y * MMPerPixel / Scale) / 2;

            _screenToWorldMatrix.A20 = ColDir.Z / Scale;
            _screenToWorldMatrix.A21 = RowDir.Z / Scale;
            _screenToWorldMatrix.A22 = Position.Z - (ColDir.Z * FOV.X * MMPerPixel / Scale + RowDir.Z * FOV.Y * MMPerPixel / Scale) / 2;
        }

        public void ConvertScreenToWorldCoords(double y, double x, Point3d screenCoords)
        {
            _screenCoordsCache.X = x;
            _screenCoordsCache.Y = y;
            _screenCoordsCache.Z = 1;
            ConvertScreenToWorldCoords(_screenCoordsCache, screenCoords);
        }

        /// <summary>
        /// Converts screen coordinates (i.e pixel coordinates) to patient position
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public Point3d ConvertScreenToWorldCoords(double y, double x)
        {
            _screenCoordsCache.X = x;
            _screenCoordsCache.Y = y;
            Point3d worldCoords = new Point3d();
            ConvertScreenToWorldCoords(_screenCoordsCache, worldCoords);
            return worldCoords;
        }

        /// <summary>
        /// Converts screen coordinates (i.e pixel coordinates) to patient position
        /// </summary>
        /// <param name="screenCoords"></param>
        /// <returns></returns>
        public Point3d ConvertScreenToWorldCoords(Point2d screenCoords)
        {
            return ConvertScreenToWorldCoords(screenCoords.Y, screenCoords.X);
        }

        public void ConvertScreenToWorldCoords(Point3d screenCoords, Point3d worldCoords)
        {
            screenCoords.Z = 1;
            //Convert normalised device coordinates where x:[-1,1] and y:[-1,1] to camera coords (mm) where x:[cx-FOV.X/2,cx+FOV.X/2]
            screenCoords.X = t.Ndc2x(screenCoords.X, FOV.X);
            screenCoords.Y = t.Ndc2y(screenCoords.Y, FOV.Y); 
            _screenToWorldMatrix.LeftMultiply(screenCoords, worldCoords);
        }

        /// <summary>
        /// Converts patient cordinates to screen coordinates, 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Point2d ConvertWorldToScreenCoords(double x, double y, double z)
        {
            Point2d screenCoords = new Point2d();
            ConvertWorldToScreenCoords(x, y, z, screenCoords);
            return screenCoords;
        }

        public Point2d ConvertWorldToScreenCoords(Point3d worldCoords)
        {
            Point2d screenCoords = new Point2d();
            ConvertWorldToScreenCoords(worldCoords.X, worldCoords.Y, worldCoords.Z, screenCoords);
            return screenCoords;
        }

        public void ConvertWorldToScreenCoords(Point3d worldCoords, Point2d screenCoords)
        {
            ConvertWorldToScreenCoords(worldCoords.X, worldCoords.Y, worldCoords.Z, screenCoords);
        }

        /// <summary>
        /// Returns a rectangle on the screen which bounds the 3D object defined by the three ranges
        /// </summary>
        /// <param name="xrange">The xrange of the object</param>
        /// <param name="yrange">The yrange of the object</param>
        /// <param name="zrange">The zrange of the object</param>
        /// <returns></returns>
        public Rectd GetBoundingScreenRect(Range xrange, Range yrange, Range zrange, Rectd normDeviceRect)
        {
            Point2d minPoint = new Point2d(double.MaxValue, double.MaxValue);
            Point2d maxPoint = new Point2d(double.MinValue, double.MinValue);
            // Project each vertex of the 3d cube onto the screen
            // and find the minimum rect surrounding those points.
            Point2d[] projectedVertices = new Point2d[8];
            projectedVertices[0] = ConvertWorldToScreenCoords(xrange.Minimum, yrange.Minimum, zrange.Minimum);
            projectedVertices[1] = ConvertWorldToScreenCoords(xrange.Minimum, yrange.Minimum, zrange.Maximum);
            projectedVertices[2] = ConvertWorldToScreenCoords(xrange.Maximum, yrange.Minimum, zrange.Maximum);
            projectedVertices[3] = ConvertWorldToScreenCoords(xrange.Maximum, yrange.Minimum, zrange.Minimum);
            projectedVertices[4] = ConvertWorldToScreenCoords(xrange.Minimum, yrange.Maximum, zrange.Minimum);
            projectedVertices[5] = ConvertWorldToScreenCoords(xrange.Minimum, yrange.Maximum, zrange.Maximum);
            projectedVertices[6] = ConvertWorldToScreenCoords(xrange.Maximum, yrange.Maximum, zrange.Maximum);
            projectedVertices[7] = ConvertWorldToScreenCoords(xrange.Maximum, yrange.Maximum, zrange.Minimum);

            foreach (var projectedVertex in projectedVertices)
            {
                if (projectedVertex.X < minPoint.X)
                    minPoint.X = projectedVertex.X;
                if (projectedVertex.X > maxPoint.X)
                    maxPoint.X = projectedVertex.X;
                if (projectedVertex.Y < minPoint.Y)
                    minPoint.Y = projectedVertex.Y;
                if (projectedVertex.Y > maxPoint.Y)
                    maxPoint.Y = projectedVertex.Y;
            }
            Rectd boundingRect = new Rectd(minPoint, maxPoint);
            boundingRect = boundingRect.Intersect(normDeviceRect);
            return boundingRect;
        }

        public void ConvertWorldToScreenCoords(double x, double y, double z, Point2d screenCoords)
        {
            //get w1 and w2 in screen coordinates (including z) from the top left
            //s1 = (w1 - tl) * Scale * MMPerPixel;
            double s1x = (x - tlx) * Scale * MMPerPixel;
            double s1y = (y - tly) * Scale * MMPerPixel;
            double s1z = (z - tlz) * Scale * MMPerPixel;
            screenCoords.X = (s1x * ColDir.X + s1y * ColDir.Y + s1z * ColDir.Z) * colDirLength;
            screenCoords.Y = (s1x * RowDir.X + s1y * RowDir.Y + s1z * RowDir.Z) * rowDirLength;
            var dot = ColDir.Dot(RowDir);
            screenCoords.X = t.X2Ndc(screenCoords.X, FOV.X);
            screenCoords.Y = t.Y2Ndc(screenCoords.Y, FOV.Y);
        }
    }
}
