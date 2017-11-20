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
        public List<ContourInfo> ContourInfo { get; set; }
        /// <summary>
        /// The maximum number of grid points to use when interpolating dose for the marching squares algorithm
        /// </summary>
        public int MaxNumberOfGridPoints { get; set; } = 100;

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

            //Translates screen to world points and vice versa
            var ms = new MarchingSquares();
            List<PlanarPolygon> polygons = new List<PlanarPolygon>();

            double textY = 0;

            foreach(ContourInfo contourInfo in ContourInfo)
            {
                var interpolatedDoseGrid = new InterpolatedDoseGrid(doseObject, MaxNumberOfGridPoints, camera, screenRect);
                var contour = ms.GetContour(interpolatedDoseGrid.Data, interpolatedDoseGrid.Rows, interpolatedDoseGrid.Columns, interpolatedDoseGrid.Coords, contourInfo.Threshold, contourInfo.Color);
                //var polygon = contour.ToPlanarPolygon(camera);

                Point2d screenPoint1 = new Point2d();
                Point2d screenPoint2 = new Point2d();
                Point3d worldPoint1 = new Point3d();
                Point3d worldPoint2 = new Point3d();

                for(int i = 0; i < contour.Vertices.Length; i+= 6)
                {
                    worldPoint1.X = contour.Vertices[i];
                    worldPoint1.Y = contour.Vertices[i + 1];
                    worldPoint1.Z = contour.Vertices[i + 2];
                    worldPoint2.X = contour.Vertices[i + 3];
                    worldPoint2.Y = contour.Vertices[i + 4];
                    worldPoint2.Z = contour.Vertices[i + 5];

                    camera.ConvertWorldToScreenCoords(worldPoint1, screenPoint1);
                    camera.ConvertWorldToScreenCoords(worldPoint2, screenPoint2);

                    context.DrawLine(screenPoint1.X, screenPoint1.Y, screenPoint2.X, screenPoint2.Y, contour.Color, lineType);
                }
            }
        }
    }
}
