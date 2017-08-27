using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomPanel.Core.Geometry
{
    class DicomSlice
    {
        public DicomSlice(int rows, int columns)
        {
            this.Rows = rows;
            this.Columns = columns;
            Data = new float[Columns * Rows];
        }

        /// <summary>
        /// The slice data
        /// </summary>
        public float[] Data { get; set; }
        /// <summary>
        /// Projection of rows onto x
        /// </summary>
        public double Xx { get; set; }
        /// <summary>
        /// Projection of rows onto y
        /// </summary>
        public double Xy { get; set; }
        /// <summary>
        /// Projection of rows onto z
        /// </summary>
        public double Xz { get; set; }
        /// <summary>
        /// Prohjection of columns onto x
        /// </summary>
        public double Yx { get; set; }
        /// <summary>
        /// Projection of columns onto y
        /// </summary>
        public double Yy { get; set; }
        /// <summary>
        /// Projection of columns onto z
        /// </summary>
        public double Yz { get; set; }
        /// <summary>
        /// Image position patient X
        /// </summary>
        public double Sx { get; set; }
        /// <summary>
        /// Image position patient Y
        /// </summary>
        public double Sy { get; set; }
        /// <summary>
        /// Image position patient Z
        /// </summary>
        public double Sz { get; set; }
        /// <summary>
        /// Row spacing
        /// </summary>
        public double Dr { get; set; }
        /// <summary>
        /// Column spacing
        /// </summary>
        public double Dc { get; set; }
        /// <summary>
        /// The number of rows in the image slice
        /// </summary>
        public int Rows { get; set; }
        /// <summary>
        /// The number of columns in the image slice
        /// </summary>
        public int Columns { get; set; }

        public float Get(int row, int column)
        {
            if (row < 0 || column < 0 || row > Rows - 1 || column > Columns - 1)
                return -1000;
            else
                return Data[row * (Columns) + column];
        }

        public void Set(int row, int column, float value)
        {
            if (row < 0 || column < 0 || row > Rows - 1 || column > Columns - 1)
                return;
            else
                Data[row * (Columns) + column] = value;
        }

        public double ComputePx(int row, int column)
        {
            return Yx * Dc * column + Xx * Dr * row + Sx;
        }

        public double ComputePy(int row, int column)
        {
            return Yy * Dc * column + Xy * Dr * row + Sy;
        }

        public double ComputePz(int row, int column)
        {
            return Yz * Dc * column + Xz * Dc * row + Sz;
        }

        public Range XRange
        {
            get
            {
                if (_xRange == null)
                {
                    _xRange = new Range(Sx, ComputePx(Rows, Columns));
                }
                return _xRange;
            }
        }
        private Range _xRange;

        public Range YRange
        {
            get
            {
                if (_yRange == null)
                {
                    _yRange = new Range(Sy, ComputePy(Rows, Columns));
                }
                return _yRange;
            }
        }
        private Range _yRange;

        public Range ZRange
        {
            get
            {
                if (_zRange == null)
                {
                    _zRange = new Range(Sz, ComputePz(Rows, Columns));
                }
                return _zRange;
            }
        }
        private Range _zRange;
    }
}
