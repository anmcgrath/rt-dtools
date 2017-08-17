using DicomPanel.Core.Geometry;
using DicomPanel.Core.Radiotherapy.Dose;
using DicomPanel.Core.Render.Contouring;
using DicomPanel.Core.Utilities.RTMath;
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
        public int MaxNumberOfGridPoints { get; set; } = 50;

        public DoseRenderer()
        {
            ContourInfo = new List<Contouring.ContourInfo>()
            {
                new Contouring.ContourInfo(DicomColor.FromRgb(255,0,0),90),
                new Contouring.ContourInfo(DicomColor.FromRgb(128,0,0),80),
                new Contouring.ContourInfo(DicomColor.FromRgb(0,0,255),80),
            };
        }

        public void Render(IDoseObject doseObject, Camera camera, IRenderContext context, Recti screenRect)
        {
            if (doseObject == null || doseObject.Grid == null)
                return;

            //Translates screen to world points and vice versa
            var t = camera.CreateTranslator(context);
            var ms = new MarchingSquares();
            List<PlanarPolygon> polygons = new List<PlanarPolygon>();

            foreach(ContourInfo contourInfo in ContourInfo)
            {
                var interpolatedDoseGrid = new InterpolatedDoseGrid(doseObject, MaxNumberOfGridPoints, t, screenRect);
                var contour = ms.GetContour(interpolatedDoseGrid.Data, interpolatedDoseGrid.Rows, interpolatedDoseGrid.Columns, interpolatedDoseGrid.Coords, contourInfo.Threshold, contourInfo.Color);
                var polygon = contour.ToPlanarPolygon(t);
                for (int i = 0; i < polygon.Vertices.Length - 4; i += 4)
                {
                    context.DrawLine(
                        polygon.Vertices[i + 0],
                        polygon.Vertices[i + 1],
                        polygon.Vertices[i + 2],
                        polygon.Vertices[i + 3],
                        contour.Color
                        );
                }
            }
        }
    }
}
