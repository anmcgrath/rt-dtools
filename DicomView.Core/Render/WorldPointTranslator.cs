using DicomPanel.Core.Geometry;
using DicomPanel.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Render
{
    public class WorldPointTranslator
    {
        private IRenderContext _context;
        private Camera _camera;
        private Point3d _screenCoordsCache { get; set; }
        private Point3d _worldCoordsCache { get; set; }
        private double _colDirLength { get; set; }
        private double _rowDirLength { get; set; }

        /// <summary>
        /// Some cached doubles for computing world to screen points
        /// </summary>
        private double tlx;
        private double tly;
        private double tlz;

        private Matrix3d _screenToWorldMatrix { get; set; }

        private WorldPointTranslator()
        {
            _screenToWorldMatrix = new Matrix3d();
            _screenCoordsCache = new Point3d(0, 0, 1);
            _worldCoordsCache = new Point3d();
        }

        public static WorldPointTranslator Create(Camera camera, IRenderContext context)
        {
            WorldPointTranslator translator = new WorldPointTranslator();
            translator._camera = camera;
            translator._context = context;
            translator.createScreenToWorldMatrix();
            translator.cacheWorldToScreenVariables();
            translator._colDirLength = camera.ColDir.Length();
            translator._rowDirLength = camera.RowDir.Length();
            return translator;
        }

        private void cacheWorldToScreenVariables()
        {
            tlx = _camera.Position.X - (_camera.ColDir.X * _context.Width * _camera.MMPerPixel / _camera.Scale + _camera.RowDir.X * _context.Height * _camera.MMPerPixel / _camera.Scale) / 2;
            tly = _camera.Position.Y - (_camera.ColDir.Y * _context.Width * _camera.MMPerPixel / _camera.Scale + _camera.RowDir.Y * _context.Height * _camera.MMPerPixel / _camera.Scale) / 2;
            tlz = _camera.Position.Z - (_camera.ColDir.Z * _context.Width * _camera.MMPerPixel / _camera.Scale + _camera.RowDir.Z * _context.Height * _camera.MMPerPixel / _camera.Scale) / 2;
        }

        private void createScreenToWorldMatrix()
        {
            _screenToWorldMatrix.A00 = _camera.ColDir.X / _camera.Scale;
            _screenToWorldMatrix.A01 = _camera.RowDir.X / _camera.Scale;
            _screenToWorldMatrix.A02 = _camera.Position.X - (_camera.ColDir.X * _context.Width * _camera.MMPerPixel / _camera.Scale + _camera.RowDir.X * _context.Height * _camera.MMPerPixel / _camera.Scale) / 2;

            _screenToWorldMatrix.A10 = _camera.ColDir.Y / _camera.Scale;
            _screenToWorldMatrix.A11 = _camera.RowDir.Y / _camera.Scale;
            _screenToWorldMatrix.A12 = _camera.Position.Y - (_camera.ColDir.Y * _context.Width * _camera.MMPerPixel / _camera.Scale + _camera.RowDir.Y * _context.Height * _camera.MMPerPixel / _camera.Scale) / 2;

            _screenToWorldMatrix.A20 = _camera.ColDir.Z / _camera.Scale;
            _screenToWorldMatrix.A21 = _camera.RowDir.Z / _camera.Scale;
            _screenToWorldMatrix.A22 = _camera.Position.Z - (_camera.ColDir.Z * _context.Width * _camera.MMPerPixel / _camera.Scale + _camera.RowDir.Z * _context.Height * _camera.MMPerPixel / _camera.Scale) / 2;
        }

        public void ConvertScreenToWorldCoords(double row, double column, Point3d screenCoords)
        {
            _screenCoordsCache.X = column;
            _screenCoordsCache.Y = row;
            _screenCoordsCache.Z = 1;
            ConvertScreenToWorldCoords(_screenCoordsCache, screenCoords);
        }

        /// <summary>
        /// Converts screen coordinates (i.e pixel coordinates) to patient position
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public Point3d ConvertScreenToWorldCoords(double row, double column)
        {
            _screenCoordsCache.X = column;
            _screenCoordsCache.Y = row;
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
        public Rectd GetBoundingScreenRect(Range xrange, Range yrange, Range zrange, Recti screenRect)
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
            boundingRect.Intersect(screenRect);
            return boundingRect;
        }

        public void ConvertWorldToScreenCoords(double x, double y, double z, Point2d screenCoords)
        {
            //get w1 and w2 in screen coordinates (including z) from the top left
            //s1 = (w1 - tl) * Scale * MMPerPixel;
            double s1x = (x - tlx) * _camera.Scale * _camera.MMPerPixel;
            double s1y = (y - tly) * _camera.Scale * _camera.MMPerPixel;
            double s1z = (z - tlz) * _camera.Scale * _camera.MMPerPixel;
            screenCoords.X = _colDirLength * (s1x * _camera.ColDir.X + s1y * _camera.ColDir.Y + s1z * _camera.ColDir.Z);
            screenCoords.Y = _rowDirLength * (s1x * _camera.RowDir.X + s1y * _camera.RowDir.Y + s1z * _camera.RowDir.Z);
        }

    }
}
