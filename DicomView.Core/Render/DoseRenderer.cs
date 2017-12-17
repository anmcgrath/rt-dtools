using RT.Core.Geometry;
using RT.Core.Dose;
using DicomPanel.Core.Render.Contouring;
using RT.Core.Utilities.RTMath;
using RT.Core.DICOM;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Render
{
    public class DoseRenderer
    {
        private Point2d screenCoords1 = new Point2d();
        private Point2d screenCoords2 = new Point2d();

        public List<ContourInfo> ContourInfo { get; set; }
        /// <summary>
        /// The maximum number of grid points to use when interpolating dose for the marching squares algorithm
        /// </summary>
        public int MaxNumberOfGridPoints { get; set; } = 80;

        public DoseRenderer()
        {
            ContourInfo = new List<Contouring.ContourInfo>()
            {
                new Contouring.ContourInfo(DicomColors.Red,99),
                new Contouring.ContourInfo(DicomColors.OrangeRed,90),
                new Contouring.ContourInfo(DicomColors.Orange,80),
                new Contouring.ContourInfo(DicomColors.Yellow,70),
                new Contouring.ContourInfo(DicomColors.White,60),
                new Contouring.ContourInfo(DicomColors.Green,50),
                new Contouring.ContourInfo(DicomColors.Blue,40),
                new Contouring.ContourInfo(DicomColors.LightBlue,30),
                new Contouring.ContourInfo(DicomColors.LightSkyBlue,20),
            };
        }

        public void Render(IDoseObject doseObject, Camera camera, IRenderContext context, Rectd screenRect, LineType lineType)
        {
            if (doseObject == null || doseObject.Grid == null)
                return;

            if (doseObject.Grid.GetNormalisationAmount() == 0)
                return;

            //Translates screen to world points and vice versa
            var ms = new MarchingSquares();
            List<PlanarPolygon> polygons = new List<PlanarPolygon>();
            var interpolatedDoseGrid = new InterpolatedDoseGrid(doseObject, MaxNumberOfGridPoints, camera, screenRect);

            foreach (ContourInfo contourInfo in ContourInfo)
            {
                var contour = ms.GetContour(interpolatedDoseGrid.Data, interpolatedDoseGrid.Rows, interpolatedDoseGrid.Columns, interpolatedDoseGrid.Coords, contourInfo.Threshold, contourInfo.Color);
                //var polygon = contour.ToPlanarPolygon(camera);

                Point2d screenPoint1 = new Point2d();
                Point2d screenPoint2 = new Point2d();
                Point3d worldPoint1 = new Point3d();
                Point3d worldPoint2 = new Point3d();

                var screenVertices = getScreenVertices(contour.Vertices, camera);

                context.DrawLines(screenVertices, contourInfo.Color);
            }
        }

        private double[] getScreenVertices(double[] vertices, Camera camera)
        {
            double[] screenVertices = new double[2 * vertices.Length / 3];

            for (int i = 0, j = 0; i < vertices.Length; i += 6, j += 4)
            {
                camera.ConvertWorldToScreenCoords(vertices[i + 0], vertices[i + 1], vertices[i + 2], screenCoords1);
                camera.ConvertWorldToScreenCoords(vertices[i + 3], vertices[i + 4], vertices[i + 5], screenCoords2);
                screenVertices[j] = screenCoords1.X;
                screenVertices[j + 1] = screenCoords1.Y;
                screenVertices[j + 2] = screenCoords2.X;
                screenVertices[j + 3] = screenCoords2.Y;
            }
            return screenVertices;
        }
    }
}
