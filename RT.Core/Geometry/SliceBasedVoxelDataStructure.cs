using RT.Core.Utilities.RTMath;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.Geometry
{
    public class SliceBasedVoxelDataStructure : VoxelDataStructureBase, IVoxelDataStructure
    {
        /// <summary>
        /// The matrix A converts (i,j,k) (voxel indices) to patient coordinate positions
        /// </summary>
        public Matrix4d MatrixA { get { return _matrixA; } set { _matrixA = value; MatrixAInv = _matrixA.Inverse();
            } }
        private Matrix4d _matrixA;

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

        public float DefaultPhysicalValue { get; set; }

        public SliceBasedVoxelDataStructure():base()
        {
            _positionCache = new Point4d(0,0,0,1);
            _indexCache = new Point4d(0, 0, 0, 1);
            _slices = new List<DicomSlice>();
        }

        private Point3d getPositionFromIndex(int i, int j, int k)
        {
            Point4d pt4 = MatrixA.LeftMultiply(new Point4d(i, j, k, 1));
            return new Point3d(pt4.X, pt4.Y, pt4.Z);
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

            int ic0 = (int)Math.Round(_indexCache.X);
            int ir0 = (int)Math.Round(_indexCache.Y);
            int ik0 = (int)Math.Round(_indexCache.Z);

            voxel.Value = getVoxel(ic0, ir0, ik0);

            /*var p0 = getPositionFromIndex(ic0, ir0, ik0);
            int ic1 = ic0 + 1, ir1 = ir0 + 1, ik1 = ik0 + 1;

            var p1 = getPositionFromIndex(ic1, ir0, ik1);

            voxel.Value = Interpolation.TrilinearInterpolate(
                (float)position.X, (float)position.Y, (float)position.Z,
                (float)p0.X, (float)p0.Y, (float)p0.Z,
                (float)p1.X, (float)p1.Y, (float)p1.Z,
                getVoxel(ic0, ir0, ik0),
                getVoxel(ic1, ir0, ik0),
                getVoxel(ic0, ir0, ik1),
                getVoxel(ic1, ir0, ik1),
                getVoxel(ic0, ir1, ik0),
                getVoxel(ic1, ir1, ik0),
                getVoxel(ic0, ir1, ik1),
                getVoxel(ic1, ir1, ik1));*/
        }

        private float getVoxel(int ic, int ir, int ik)
        {
            if (ik > _slices.Count - 1 || ik < 0)
                return DefaultPhysicalValue;
            else
            {
                if (ic < _slices[ik].Columns && ir < _slices[ik].Columns && ic > -1 && ir > -1)
                    return _slices[ik].Get(ic, ir);
                else
                    return DefaultPhysicalValue;
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

            _slices.Add(slice);
            Voxel sliceMax = slice.ComputeMax();
            if (sliceMax.Value > this.MaxVoxel.Value)
                MaxVoxel = sliceMax;
            Voxel sliceMin = slice.ComputeMin();
            if (sliceMin.Value < this.MinVoxel.Value)
                MinVoxel = sliceMin;
        }

        public void ComputeMax()
        {
            var max = new Voxel() { Value = float.MinValue };
            for(int i = 0; i < _slices.Count; i++)
            {
                var sliceMax = _slices[i].ComputeMax();
                if (sliceMax.Value > max.Value)
                    max = sliceMax;
            }
            MaxVoxel = max;
        }
    }
    
}
