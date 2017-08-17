using DicomPanel.Core.Geometry;
using DicomPanel.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Render.Contouring
{
    public class Contour
    {
        /// <summary>
        /// Successive line segments in the form of (x1, y1, z2, x2, y2, z2)
        /// </summary>
        public double[] Vertices;
        public DicomColor Color;

        /// <summary>
        /// Holds line segments in the 3D world and a color
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="color"></param>
        public Contour(double[] vertices, DicomColor color)
        {
            Vertices = vertices;
            Color = color;
        }

        public PlanarPolygon ToPlanarPolygon(WorldPointTranslator translator)
        {
            double[] vertices = new double[(int)(Vertices.Length * 0.75)];
            Point2d screenPoint = new Point2d();
            int verticesIndex = 0;
            double xmin = double.MaxValue;
            double xmax = double.MinValue;
            double ymin = double.MaxValue;
            double ymax = double.MinValue;
            for(int i = 0; i < Vertices.Length; i+=3, verticesIndex+=2)
            {
                translator.ConvertWorldToScreenCoords(Vertices[i + 0], Vertices[i + 1], Vertices[i + 1], screenPoint);
                vertices[verticesIndex + 0] = screenPoint.X;
                vertices[verticesIndex + 1] = screenPoint.Y;
                xmin = Math.Min(xmin, screenPoint.X);
                xmax = Math.Max(xmax, screenPoint.X);
                ymax = Math.Max(ymax, screenPoint.Y);
                ymin = Math.Min(ymin, screenPoint.Y);
            }
            return new PlanarPolygon()
            {
                Vertices = vertices,
                XRange = new Range(xmin, xmax),
                YRange = new Range(ymin, ymax),
            };
            
        }
    }
}
