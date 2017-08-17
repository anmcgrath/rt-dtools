using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTData.Utilities.RTMath;

namespace RTData.Geometry
{
    public class GridBasedVoxelDataStructure : VoxelDataStructureBase, IVoxelDataStructure
    {
        /// <summary>
        /// X Coordinates (LR orientation)
        /// </summary>
        public double[] XCoords { get; set; }
        /// <summary>
        /// Y Coordinates (AP orientation)
        /// </summary>
        public double[] YCoords { get; set; }
        /// <summary>
        /// X Coordinates (Inf-Sup orientation)
        /// </summary>
        public double[] ZCoords { get; set; }
        /// <summary>
        /// The voxel data, access through Data[x,y,z]
        /// </summary>
        public float[,,] Data { get; set; }
        /// <summary>
        /// Whether the grid spacing is constant in each direction
        /// </summary>
        public bool ConstantGridSpacing { get; set; }
        /// <summary>
        /// Only set if the grid has constant pixel spacing
        /// </summary>
        public Point3d GridSpacing { get; set; }

        public Voxels Voxels { get; private set; }

        public GridBasedVoxelDataStructure()
        {
            Voxels = new Voxels(this);
            GlobalMax = new Voxel() { Value = float.MinValue };
        }


        public override void Interpolate(Point3d position, Voxel voxel)
        {
            voxel.Position.X = position.X;
            voxel.Position.Y = position.Y;
            voxel.Position.Z = position.Z;

            if (!XRange.Contains(position.X) || !YRange.Contains(position.Y) || !ZRange.Contains(position.Z))
            {
                voxel.Value = 0;
            }
            else
            {
                float x0 = 0, x1 = 0, y0 = 0, y1 = 0, z0 = 0, z1 = 0;
                int ix0 = 0, ix1 = 0, iy0 = 0, iy1 = 0, iz0 = 0, iz1 = 0;

                if (ConstantGridSpacing)
                {
                    ix0 = (int)((position.X - XCoords[0]) / GridSpacing.X);
                    ix1 = ix0 == XCoords.Length - 1 ? ix0 : ix0 + 1;
                    x0 = (float)XCoords[ix0];
                    x1 = (float)XCoords[ix1];
                    iy0 = (int)((position.Y - YCoords[0]) / GridSpacing.Y);
                    iy1 = iy0 == YCoords.Length - 1 ? iy0 : iy0 + 1;
                    y0 = (float)YCoords[iy0];
                    y1 = (float)YCoords[iy1];
                    if (GridSpacing.Z != 0)
                    {
                        iz0 = (int)((position.Z - ZCoords[0]) / GridSpacing.Z);
                        iz1 = iz0 == ZCoords.Length - 1 ? iz0 : iz0 + 1;
                        z0 = (float)ZCoords[iz0];
                        z1 = (float)ZCoords[iz1];
                    }
                    else
                    {
                        iz0 = 0; iz1 = 0;
                        z0 = (float)ZCoords[iz0];
                        z1 = (float)ZCoords[iz1];
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
                float xd;
                if (x1 == x0)
                    xd = 0;
                else
                    xd = (float)(position.X - x0) / (x1 - x0);
                double yd;
                if (y1 == y0)
                    yd = 0;
                else
                    yd = (position.Y - y0) / (y1 - y0);
                double zd;
                if (z1 == z0)
                    zd = 0;
                else
                    zd = (position.Z - z0) / (z1 - z0);
                float c00 = Data[ix0, iy0, iz0] * (1 - xd) + Data[ix1, iy0, iz0] * xd;
                float c01 = Data[ix0, iy0, iz1] * (1 - xd) + Data[ix1, iy0, iz1] * xd;
                float c10 = Data[ix0, iy1, iz0] * (1 - xd) + Data[ix1, iy1, iz0] * xd;
                float c11 = Data[ix0, iy1, iz1] * (1 - xd) + Data[ix1, iy1, iz1] * xd;
                float c0 = (float)(c00 * (1 - yd) + c10 * yd);
                float c1 = (float)(c01 * (1 - yd) + c11 * yd);
                float c = (float)(c0 * (1 - zd) + c1 * zd);

                voxel.Value = c;
            }
        }

        /// <summary>
        /// Helper function used in the interpolation
        /// </summary>
        /// <param name="value"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        protected Tuple<double, double, int, int> binarySearchForSurroundingCoords(double value, double[] array)
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

        //IEnumerator Functions For Voxels
        private int currentXIndex;
        private int currentYIndex;
        private int currentZIndex;
        private Voxel voxel = new Voxel();

        public bool MoveNext()
        {
            currentXIndex++;
            if(currentXIndex > XCoords.Length-1)
            {
                currentXIndex = 0;
                currentYIndex++;
            }
            if(currentYIndex > YCoords.Length-1)
            {
                currentYIndex = 0;
                currentZIndex++;
            }
            return !(currentZIndex > ZCoords.Length - 1);
        }

        public void Reset()
        {
            //currentXIndex = 0;
            //currentYIndex = 0;
            //currentZIndex = 0;
        }

        public void Dispose() { }

        public Voxel Current
        {
            get
            {
                voxel.Position.X = XCoords[currentXIndex];
                voxel.Position.Y = YCoords[currentYIndex];
                voxel.Position.Z = ZCoords[currentZIndex];
                voxel.Value = Data[currentXIndex, currentYIndex, currentZIndex];
                return voxel;
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
    }
}
