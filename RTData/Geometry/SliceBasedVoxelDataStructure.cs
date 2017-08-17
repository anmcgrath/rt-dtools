using RTData.Utilities.RTMath;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTData.Geometry
{
    public class SliceBasedVoxelDataStructure : VoxelDataStructureBase, IVoxelDataStructure
    {
        /// <summary>
        /// The matrix A converts (i,j,k) (voxel indices) to patient coordinate positions
        /// </summary>
        public Matrix4d MatrixA { get { return _matrixA; } set { _matrixA = value; MatrixAInv = _matrixA.Inverse(); } }
        private Matrix4d _matrixA;

        public Voxels Voxels { get; private set; }

        /// <summary>
        /// The inverse A matrix converts patient coordinate positions to (i,j,k) voxel indices
        /// </summary>
        private Matrix4d MatrixAInv { get; set; }

        /// <summary>
        /// Every DicomSlice loaded from a dicom file
        /// </summary>
        private List<DicomSlice> _slices;

        /// <summary>
        /// Used so that a new vector does not have to be constantly created
        /// </summary>
        private Point4d _positionCache;
        private Point4d _indexCache;

        public SliceBasedVoxelDataStructure():base()
        {
            _positionCache = new Point4d(0,0,0,1);
            _indexCache = new Point4d(0, 0, 0, 1);
            _slices = new List<DicomSlice>();
            GlobalMax = new Voxel() { Value = float.MinValue };
            Voxels = new Voxels(this);
        }

        public override void Interpolate(Point3d position, Voxel voxel)
        {
            voxel.Position.X = position.X;
            voxel.Position.Y = position.Y;
            voxel.Position.Z = position.Z;

            _positionCache.X = position.X;
            _positionCache.Y = position.Y;
            _positionCache.Z = position.Z;
            MatrixAInv.LeftMultiply(_positionCache, _indexCache);

            int ic = (int)Math.Round(_indexCache.X);
            int ir = (int)Math.Round(_indexCache.Y);
            int iz = (int)Math.Round(_indexCache.Z);

            if (iz > _slices.Count - 1 || iz < 0)
                voxel.Value = 0;
            else
            {
                if (ic < _slices[iz].Columns && ir < _slices[iz].Columns && ic > -1 && ir > -1)
                    voxel.Value = _slices[iz].Get(ic, ir);
            }
        }

        public void AddSlice(float[] sliceData, int rows, int columns, double dr, double dc, double sx, double sy, double sz, double xx, double xy, double xz, double yx, double yy, double yz)
        {
            DicomSlice slice = new DicomSlice(rows, columns);
            slice.Data = sliceData;

            slice.Sx = sx;
            slice.Sy = sy;
            slice.Sz = sz;
            slice.Xx = xx;
            slice.Xy = xy;
            slice.Xz = xz;
            slice.Yx = yx;
            slice.Yy = yy;
            slice.Yz = yz;
            slice.Dc = dc;
            slice.Dr = dr;

            if (slice.XRange.Minimum < XRange.Minimum)
                XRange.Minimum = slice.XRange.Minimum;
            if (slice.XRange.Maximum > XRange.Maximum)
                XRange.Maximum = slice.XRange.Maximum;
            if (slice.YRange.Minimum < YRange.Minimum)
                YRange.Minimum = slice.YRange.Minimum;
            if (slice.YRange.Maximum > YRange.Maximum)
                YRange.Maximum = slice.YRange.Maximum;
            if (slice.ZRange.Minimum < ZRange.Minimum)
                ZRange.Minimum = slice.ZRange.Minimum;
            if (slice.ZRange.Maximum > ZRange.Maximum)
                ZRange.Maximum = slice.ZRange.Maximum;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (slice.Get(row, col) > GlobalMax.Value)
                    {
                        GlobalMax.Value = slice.Get(row, col);
                        GlobalMax.Position.X = slice.ComputePx(row, col);
                        GlobalMax.Position.Y = slice.ComputePy(row, col);
                        GlobalMax.Position.Z = slice.ComputePz(row, col);
                    }
                }
            }

            _slices.Add(slice);
        }

        private int _currentVoxelRow = 0;
        private int _currentVoxelCol = 0;
        private int _currentSliceIndex = 0;
        private Voxel _current = new Voxel();

        Voxel Current
        {
            get
            {
                var slice = _slices[_currentSliceIndex];
                _current.Position.X = slice.ComputePx(_currentVoxelRow, _currentVoxelCol);
                _current.Position.Y = slice.ComputePy(_currentVoxelRow, _currentVoxelCol);
                _current.Position.Z = slice.ComputePz(_currentVoxelRow, _currentVoxelCol);
                _current.Value = _slices[_currentSliceIndex].Get(_currentVoxelRow, _currentVoxelCol);
                return _current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        Voxel IEnumerator<Voxel>.Current
        {
            get
            {
                return Current;
            }
        }

        public void Dispose() { }

        public void Reset()
        {
            //_currentVoxelRow = 0;
            //_currentSliceIndex = 0;
            //_currentVoxelCol = 0;
        }

        public bool MoveNext()
        {
            var slice = _slices[_currentSliceIndex];
            _currentVoxelCol++;
            if(_currentVoxelCol > slice.Columns-1)
            {
                _currentVoxelCol = 0;
                _currentVoxelRow++;
            }
            if(_currentVoxelRow > slice.Rows-1)
            {
                _currentVoxelRow = 0;
                _currentSliceIndex++;
            }
            return _currentSliceIndex < _slices.Count;
        }



    }



    
}
