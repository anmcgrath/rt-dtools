using DicomPanel.Core.Geometry;
using DicomPanel.Core.Radiotherapy.Dose;
using DicomPanel.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Render.Contouring
{
    public class InterpolatedDoseGrid
    {
        public float[,] Data;
        public double[][][] Coords { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

        public InterpolatedDoseGrid(IDoseObject doseObject, int maxNumberOfGrids, WorldPointTranslator translator, Recti screenRect)
        {
            var boundingRect = translator.GetBoundingScreenRect(doseObject.Grid.XRange, doseObject.Grid.YRange, doseObject.Grid.ZRange, screenRect);

            if (boundingRect == null)
                return;

            Rows = maxNumberOfGrids;
            Columns = maxNumberOfGrids;

            var dy = boundingRect.Height / maxNumberOfGrids;
            var dx = boundingRect.Width / maxNumberOfGrids;

            Point2d screenPoint = new Point2d(boundingRect.X, boundingRect.Y);
            var normalisationAmount = doseObject.GetNormalisationAmount();

            Coords = new double[Rows][][];
            Data = new float[Rows, Columns];

            for (int row = 0; row < Rows; row++)
            {
                Coords[row] = new double[Columns][];
                for (int col = 0; col < Columns; col++)
                {
                    screenPoint.X = boundingRect.X + col * dx;
                    screenPoint.Y = boundingRect.Y + row * dy;

                    var pt = translator.ConvertScreenToWorldCoords(screenPoint);
                    Coords[row][col] = new double[3] { pt.X, pt.Y, pt.Z };

                    Data[row, col] = 100 * doseObject.Grid.Interpolate(pt).Value / normalisationAmount;
                }
            }
        }
    }
}
