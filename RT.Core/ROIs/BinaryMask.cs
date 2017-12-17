using RT.Core.Geometry;
using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.ROIs
{
    public class BinaryMask
    {
        /// <summary>
        /// InsideBinaryData holds the binary mask with true representing a point inside and false representing a point outside.
        /// </summary>
        private bool[] InsideBinaryData;
        private bool hasPolygon;

        public Range XRange { get; set; }
        public Range YRange { get; set; }
        public double GridSpacing { get; set; } = .8; //mm

        public int Rows { get; set; }
        public int Columns { get; set; }

        public BinaryMask() { }
        public BinaryMask(Range xrange, Range yrange)
        {
            XRange = xrange;
            YRange = yrange;
            Rows = (int)(YRange.Length / GridSpacing);
            Columns = (int)(XRange.Length / GridSpacing);
            InsideBinaryData = new bool[Rows * Columns];
        }

        public void AddPolygon(PlanarPolygon polygon)
        {
            if (!hasPolygon)
            {
                XRange = polygon.XRange;
                YRange = polygon.YRange;
                Rows = (int)(polygon.YRange.Length / GridSpacing);
                Columns = (int)(polygon.XRange.Length / GridSpacing);

                // +2 to rows and cols to allow buffer of one row of false on top, left, bottom right.
                InsideBinaryData = new bool[(Rows) * (Columns)];
            }

            // Draw the new polygon to a boolean array
            bool[] polygonData = drawPolygon2(polygon);
            int numTrue = polygonData.Count(x => x == true);
            int numOrigTrue = InsideBinaryData.Count(x => x == true);

            // Add the new polygon to the current inside binary data
            InsideBinaryData = BinaryMath.Xor(polygonData, InsideBinaryData);

            hasPolygon = true;
        }

        /// <summary>
        /// Draw a polygon onto a bool array using a scanline algorithm
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        private bool[] drawPolygon2(PlanarPolygon polygon)
        {
            bool[] data = new bool[Rows * Columns];
            // loop through each row and find intersecting x coords to draw between
            for (int row = 0; row < Rows; row++)
            {
                double[] xcoords = polygon.GetFixedYLineIntersections(GetCoordinate(row, YRange));
                if (xcoords.Length > 0)
                {
                    for (int i = 0; i < xcoords.Length - 1; i += 2)
                    {
                        int xi1 = GetIndex(xcoords[i], XRange);
                        int xi2 = GetIndex(xcoords[i + 1], XRange);
                        for (int col = xi1; col <= xi2; col++)
                        {
                            SetByRowCol(row, col, true, data);
                        }
                    }
                }
            }
            return data;
        }

        private double GetCoordinate(int index, Range range)
        {
            return (index) * GridSpacing + range.Minimum;
        }

        private int GetIndex(double value, Range range)
        {
            return (int)(((value - range.Minimum) / GridSpacing));
        }

        public void SetByRowCol(int row, int column, bool value, bool[] array)
        {
            if (row < Rows && column < Columns)
                array[column + Columns * row] = value;
        }

        public bool ContainsPoint(double x, double y)
        {
            int ix = GetIndex(x, XRange);
            int iy = GetIndex(y, YRange);
            return ContainsIndex(iy, ix);
        }

        public bool ContainsIndex(int row, int column)
        {
            if (row < 0 || row > Rows - 1 || column < 0 || column > Columns - 1)
                return false;
            return InsideBinaryData[column + Columns * row];
        }

        /// <summary>
        /// Interpolates between two binary masks.
        /// See Schenk et. al Efficient Semiautomatic Segmentation of 3D objects in Medical Images
        /// Note this DOES NOT CURRENTLY WORK. Needs tweaking
        /// </summary>
        /// <param name="mask2"></param>
        /// <param name="frac">0 is all this mask, 1 is all mask 2</param>
        /// <returns></returns>
        public BinaryMask InterpolateWith(BinaryMask mask2, double frac)
        {
            BinaryMask newMask = new BinaryMask(XRange, YRange);
            float[] m1distance = BinaryMath.DistanceTransform(InsideBinaryData);
            float[] m2distance = BinaryMath.DistanceTransform(mask2.InsideBinaryData);
            newMask.InsideBinaryData = new bool[InsideBinaryData.Length];

            for (int i = 0; i < m1distance.Length; i++)
            {
                var dist = m1distance[i] * (1 - frac) + m2distance[i] * (frac);
                if (dist <= 0)
                {
                    newMask.InsideBinaryData[i] = true;
                }
            }
            return newMask;
        }
    }
}