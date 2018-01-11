using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Core.Utilities.RTMath;
using RT.Core.ROIs;

namespace RT.Core.Geometry
{
    public partial class GridBasedVoxelDataStructure : VoxelDataStructureBase, IVoxelDataStructure
    {
        /// <summary>
        /// X Coordinates (LR orientation)
        /// </summary>
        public float[] XCoords { get; set; }
        /// <summary>
        /// Y Coordinates (AP orientation)
        /// </summary>
        public float[] YCoords { get; set; }
        /// <summary>
        /// X Coordinates (Inf-Sup orientation)
        /// </summary>
        public float[] ZCoords { get; set; }
        /// <summary>
        /// The voxel data, access through Data[x + length(y) * (y + (length(z) * z)]
        /// </summary>
        public float[] Data { get; set; }
        /// <summary>
        /// Whether the grid spacing is constant in each direction
        /// </summary>
        public bool ConstantGridSpacing { get; set; }
        /// <summary>
        /// Only set if the grid has constant pixel spacing
        /// </summary>
        public Point3d GridSpacing { get; set; }

        /// <summary>
        /// Returned when the interpolation cannot find a value at the point
        /// </summary>
        public float DefaultPhysicalValue { get; set; }
        public int NumberOfVoxels { get { return XCoords.Length * YCoords.Length * ZCoords.Length; } }

        public GridBasedVoxelDataStructure()
        {
        }

        public void SetVoxelByCoords(float x, float y, float z, float value)
        {
            if (ConstantGridSpacing)
            {
                int indexX = (int)((x - XCoords[0]) / GridSpacing.X);
                int indexY = (int)((y - YCoords[0]) / GridSpacing.Y);
                int indexZ = (int)((z - ZCoords[0]) / GridSpacing.Z);
                Data[getIndex(indexX, indexY, indexZ)] = value;
            }
        }

        public void SetVoxelByIndices(int i, int j, int k, float value)
        {
            Data[getIndex(i, j, k)] = value;
        }

        private int getIndex(int ix, int iy, int iz)
        {
            return ix + XCoords.Length * (iy + (YCoords.Length * iz));
        }

        float x0, x1, y0, y1, z0, z1;
        int ix0, ix1, iy0, iy1, iz0, iz1;
        public override void Interpolate(Point3d position, Voxel voxel)
        {
            voxel.Position.X = position.X;
            voxel.Position.Y = position.Y;
            voxel.Position.Z = position.Z;

            if (!XRange.Contains(position.X) || !YRange.Contains(position.Y) || !ZRange.Contains(position.Z))
            {
                voxel.Value = DefaultPhysicalValue;
            }
            else
            {
                //float x0 = 0, x1 = 0, y0 = 0, y1 = 0, z0 = 0, z1 = 0;
                //int ix0 = 0, ix1 = 0, iy0 = 0, iy1 = 0, iz0 = 0, iz1 = 0;

                if (ConstantGridSpacing)
                {
                    ix0 = (int)((position.X - XCoords[0]) / GridSpacing.X);
                    if (ix0 == XCoords.Length - 1)
                        ix1 = ix0;
                    else
                        ix1 = ix0 + 1;
                    x0 = XCoords[ix0];
                    x1 = XCoords[ix1];
                    iy0 = (int)((position.Y - YCoords[0]) / GridSpacing.Y);
                    if (iy0 == YCoords.Length - 1)
                        iy1 = iy0;
                    else
                        iy1 = iy0 + 1;
                    y0 = YCoords[iy0];
                    y1 = YCoords[iy1];
                    if (GridSpacing.Z != 0)
                    {
                        iz0 = (int)((position.Z - ZCoords[0]) / GridSpacing.Z);
                        if (iz0 == ZCoords.Length - 1)
                            iz1 = iz0;
                        else
                            iz1 = iz0 + 1;
                        z0 = ZCoords[iz0];
                        z1 = ZCoords[iz1];
                    }
                    else
                    {
                        iz0 = 0; iz1 = 0;
                        z0 = ZCoords[iz0];
                        z1 = ZCoords[iz1];
                    }
                }
                else
                {
                    var xt = binarySearchForSurroundingCoords(position.X, XCoords);
                    x0 = (float)xt.Item1;
                    x1 = (float)xt.Item2;
                    ix0 = xt.Item3;
                    ix1 = xt.Item4;
                    var yt = binarySearchForSurroundingCoords(position.Y, YCoords);
                    y0 = (float)yt.Item1;
                    y1 = (float)yt.Item2;
                    iy0 = yt.Item3;
                    iy1 = yt.Item4;
                    var zt = binarySearchForSurroundingCoords(position.Z, ZCoords);
                    z0 = (float)zt.Item1;
                    z1 = (float)zt.Item2;
                    iz0 = zt.Item3;
                    iz1 = zt.Item4;
                }

                voxel.Value = Interpolation.TrilinearInterpolate(
                    (float)position.X, (float)position.Y, (float)position.Z,
                    x0, y0, z0,
                    x1, y1, z1,
                    Data[getIndex(ix0, iy0, iz0)],
                    Data[getIndex(ix1, iy0, iz0)],
                    Data[getIndex(ix0, iy0, iz1)],
                    Data[getIndex(ix1, iy0, iz1)],
                    Data[getIndex(ix0, iy1, iz0)],
                    Data[getIndex(ix1, iy1, iz0)],
                    Data[getIndex(ix0, iy1, iz1)],
                    Data[getIndex(ix1, iy1, iz1)]);
            }
        }

        /// <summary>
        /// Helper function used in the interpolation
        /// </summary>
        /// <param name="value"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        protected Tuple<double, double, int, int> binarySearchForSurroundingCoords(double value, float[] array)
        {
            if (value < array[0] || value > array[array.Length - 1])
                return new Tuple<double, double, int, int>(0, 0, 0, 0);
            if (value == array[0])
                return new Tuple<double, double, int, int>(array[0], array[1], 0, 1);
            if (value == array[array.Length - 1])
                return new Tuple<double, double, int, int>(array[array.Length - 2], array[array.Length - 1], array.Length - 2, array.Length - 1);

            int low = 0; // 0 is always going to be the first element
            int high = array.Length - 1; // Find highest element
            int middle = (low + high + 1) / 2; // Find middle element
            int location = -1; // Return value -1 if not found

            do // Search for element
            {
                // if element is found at middle
                if (array[middle] <= value && array[middle + 1] >= value)
                    return new Tuple<double, double, int, int>(array[middle], array[middle + 1], middle, middle + 1);

                // middle element is too high
                else if (value < array[middle])
                    high = middle - 1; // eliminate lower half
                else // middle element is too low
                    low = middle + 1; // eleminate lower half

                middle = (low + high + 1) / 2; // recalculate the middle  
            } while ((low <= high) && (location == -1));

            return new Tuple<double, double, int, int>(0, 0, 0, 0);
        }

        public void ComputeMax()
        {
            MaxVoxel.Value = float.MinValue;
            foreach (var x in XCoords)
            {
                foreach (var y in YCoords)
                {
                    foreach (var z in ZCoords)
                    {
                        var val = Interpolate(x, y, z).Value;
                        if (val > MaxVoxel.Value)
                        {
                            MaxVoxel.Value = val;
                            MaxVoxel.Position.X = x;
                            MaxVoxel.Position.Y = y;
                            MaxVoxel.Position.Z = z;
                        }
                    }
                }
            }
        }

        private Voxel enumeratorVoxel = new Voxel();
        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < XCoords.Length; i++)
            {
                for (int j = 0; j < YCoords.Length; j++)
                {
                    for (int k = 0; k < ZCoords.Length; k++)
                    {
                        enumeratorVoxel.Position.X = XCoords[i];
                        enumeratorVoxel.Position.Y = YCoords[j];
                        enumeratorVoxel.Position.Z = ZCoords[k];
                        enumeratorVoxel.Value = Data[getIndex(i, j, k)];
                        yield return enumeratorVoxel;
                    }
                }
            }
        }

        /// <summary>
        /// Returns an empty grid of the same size
        /// </summary>
        /// <returns></returns>
        public GridBasedVoxelDataStructure CopyEmpty()
        {
            return CreateNew(XCoords.Length, YCoords.Length, ZCoords.Length);
        }

        public GridBasedVoxelDataStructure CopyWithCoords()
        {
            var grid = CreateNew(XCoords.Length, YCoords.Length, ZCoords.Length, XRange, YRange, ZRange);
            GridSpacing.CopyTo(grid.GridSpacing);
            grid.ConstantGridSpacing = ConstantGridSpacing;
            return grid;
        }

        public static GridBasedVoxelDataStructure CreateNew(int width, int height, int depth)
        {
            GridBasedVoxelDataStructure newGrid = new GridBasedVoxelDataStructure();
            newGrid.XCoords = new float[width];
            newGrid.YCoords = new float[height];
            newGrid.ZCoords = new float[depth];
            newGrid.Data = new float[width * height * depth];
            return newGrid;
        }

        /// <summary>
        /// Returns a new grid with constant grid spacing, with coords based on the ranges
        /// </summary>
        /// <param name="width">The number of x coordinates</param>
        /// <param name="height">The number of y coordinates</param>
        /// <param name="depth">The number of z coordinates</param>
        /// <param name="xrange"></param>
        /// <param name="yRange"></param>
        /// <param name="zRange"></param>
        /// <returns></returns>
        public static GridBasedVoxelDataStructure CreateNew(int width, int height, int depth, Range xrange, Range yRange, Range zRange)
        {
            GridBasedVoxelDataStructure newGrid = CreateNew(width, height, depth);
            newGrid.XRange = new Range(xrange.Minimum, xrange.Maximum);
            newGrid.YRange = new Range(yRange.Minimum, yRange.Maximum);
            newGrid.ZRange = new Range(zRange.Minimum, zRange.Maximum);
            newGrid.GridSpacing = new Point3d(xrange.Length / width, yRange.Length / height, zRange.Length / height);
            for (int i = 0; i < newGrid.XCoords.Length; i++)
                newGrid.XCoords[i] = (float)(xrange.Minimum + i * newGrid.GridSpacing.X);
            for (int i = 0; i < newGrid.YCoords.Length; i++)
                newGrid.YCoords[i] = (float)(yRange.Minimum + i * newGrid.GridSpacing.Y);
            for (int i = 0; i < newGrid.ZRange.Length; i++)
                newGrid.ZCoords[i] = (float)(zRange.Minimum + i * newGrid.GridSpacing.Z);
            
            return newGrid;
        }
    }

}
