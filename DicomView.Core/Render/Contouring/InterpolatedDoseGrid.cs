using RT.Core.Geometry;
using RT.Core.Dose;
using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Render.Contouring
{
    public class InterpolatedDoseGrid
    {
        public float[][] Data;
        public double[][][] Coords { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

        public InterpolatedDoseGrid(IDoseObject doseObject, int maxNumberOfGrids, Camera camera, Rectd normRect)
        {
            //Intersect the camera screen and cube surrounding the dose object to limit the rendering.
            var boundingRect = camera.GetBoundingScreenRect(doseObject.Grid.XRange, doseObject.Grid.YRange, doseObject.Grid.ZRange, normRect);

            if (boundingRect == null)
                return;

            Rows = maxNumberOfGrids;
            Columns = maxNumberOfGrids;

            var dy = boundingRect.Height / maxNumberOfGrids;
            var dx = boundingRect.Width / maxNumberOfGrids;

            Point2d screenPoint = new Point2d(boundingRect.X, boundingRect.Y);
            var normalisationAmount = doseObject.Grid.GetNormalisationAmount();

            Coords = new double[Rows][][];
            Data = new float[Rows][];
            Voxel voxel = new Voxel();

            for (int row = 0; row < Rows; row++)
            {
                Coords[row] = new double[Columns][];
                Data[row] = new float[Columns];
                for (int col = 0; col < Columns; col++)
                {
                    screenPoint.X = boundingRect.X + col * dx;
                    screenPoint.Y = boundingRect.Y + row * dy;

                    var pt = camera.ConvertScreenToWorldCoords(screenPoint);
                    Coords[row][col] = new double[3] { pt.X, pt.Y, pt.Z };

                    doseObject.Grid.Interpolate(pt, voxel);

                    Data[row][col] = (voxel.Value * doseObject.Grid.Scaling) / normalisationAmount;
                }
            }
        }
    }
}
