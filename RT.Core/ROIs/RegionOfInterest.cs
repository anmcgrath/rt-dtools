using RT.Core.Geometry;
using RT.Core.DICOM;
using RT.Core.Utilities;
using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.ROIs
{
    public class RegionOfInterest
    {
        public string Name { get; set; }
        public DicomColor Color { get; set; }
        public int ROINumber { get; set; }
        public int HU { get; set; }
        public Range XRange { get; set; }
        public Range YRange { get; set; }
        public Range ZRange { get; set; }
        public ContourType Type { get; set; }

        public List<RegionOfInterestSlice> RegionOfInterestSlices { get; set; }
        private Dictionary<double, RegionOfInterestSlice> sliceDictionary { get; set; }
        /// <summary>
        /// Lists all z coordinates where rois are
        /// </summary>
        private List<double> roiZCoordinates { get; set; }

        public RegionOfInterest()
        {
            RegionOfInterestSlices = new List<RegionOfInterestSlice>();
            sliceDictionary = new Dictionary<double, RegionOfInterestSlice>();
            roiZCoordinates = new List<double>();
            // Set range to extremes so we can calculate mins/maxes later
            ZRange = new Range(double.MaxValue, double.MinValue, false);
        }

        public void AddSlice(RegionOfInterestSlice slice, double coordinate)
        {
            if (!sliceDictionary.ContainsKey(coordinate))
            {
                // Add the ROIs to arrays but ensure they are sorted by z coord
                int insertIndex = BinaryMath.BinarySearchClosest(coordinate, roiZCoordinates);
                if (insertIndex < 0)
                    insertIndex = ~insertIndex;
                RegionOfInterestSlices.Insert(insertIndex, slice);
                roiZCoordinates.Insert(insertIndex, coordinate);
                sliceDictionary.Add(coordinate, slice);
            }
        }

        /// <summary>
        /// Finds whether a point is inside the ROI, not interpolating between ROI slices
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public bool ContainsPointNonInterpolated(double x, double y, double z)
        {
            RegionOfInterestSlice slice = GetClosestSlice(z);
            if (slice == null)
                return false;

            return slice.ContainsPoint(x, y);
        }

        /// <summary>
        /// Finds whether a point is inside the ROI, by interpolating between ROI slices
        /// Only use this if you are checking a SINGLE point, otheriwse use ContainsXYCoordsInterpolated
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public bool ContainsPointInterpolated(double x, double y, double z)
        {
            if (!ZRange.Contains(z))
                return false;

            BinaryMask mask = GetInterpolatedMask(z);

            return mask.ContainsPoint(x, y);
        }

        /// <summary>
        /// Bulk processes whether an array of x and y coords are inside the roi, with interpolation
        /// </summary>
        /// <param name="xy">Vertices accessed by (row,col) </param>
        /// <param name="z"></param>
        /// <returns></returns>
        public bool[,] ContainsXYCoordsInterpolated(double[,][] xy, double z)
        {
            BinaryMask mask = GetInterpolatedMask(z);
            int cols = xy.GetLength(1);
            int rows = xy.GetLength(0);
            bool[,] pointsInside = new bool[rows, cols];
            for (int col = 0; col < cols; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    pointsInside[row, col] = mask.ContainsPoint(xy[row, col][0], xy[row, col][1]);
                }
            }
            return pointsInside;
        }

        public RegionOfInterestSlice GetSlice(double z)
        {
            if (sliceDictionary.ContainsKey(z))
                return sliceDictionary[z];
            else
                return null;
        }

        private BinaryMask GetInterpolatedMask(double z)
        {
            if (sliceDictionary.ContainsKey(z))
                return sliceDictionary[z].BinaryMask;

            RegionOfInterestSlice slice1 = GetClosestSlicePrevious(z);
            RegionOfInterestSlice slice2 = GetClosestSliceNext(z);
            double z1 = slice1?.ZCoord ?? 0, z2 = slice2?.ZCoord ?? 0;
            BinaryMask mask1 = slice1?.BinaryMask;
            BinaryMask mask2 = slice2?.BinaryMask;
            if (mask1 == null)
            {
                mask1 = new BinaryMask(XRange, YRange);
                z1 = slice2 != null ? slice2.ZCoord - 2 : z - 2;
            }
            if (mask2 == null)
            {
                mask2 = new BinaryMask(XRange, YRange);
                z2 = slice2 != null ? slice2.ZCoord + 2 : z + 2;
            }
            double frac = (z - z1) / (z2 - z1);
            BinaryMask interped = mask1.InterpolateWith(mask2, frac);

            return interped;
        }

        /// <summary>
        /// Returns the closest slice with a z value less than z
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public RegionOfInterestSlice GetClosestSlicePrevious(double z)
        {
            if (!ZRange.Contains(z))
                return null;

            if (sliceDictionary.ContainsKey(z))
                return sliceDictionary[z];

            int index = BinaryMath.BinarySearchClosest(z, roiZCoordinates);
            if (!(index - 1 > RegionOfInterestSlices.Count - 1) && !(index - 1 < 0))
                return RegionOfInterestSlices[index - 1];

            return null;
        }

        /// <summary>
        /// Returns the closest slice with
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public RegionOfInterestSlice GetClosestSlice(double z)
        {
            if (!ZRange.Contains(z))
                return null;

            if (sliceDictionary.ContainsKey(z))
                return sliceDictionary[z];

            int index = BinaryMath.BinarySearchClosest(z, roiZCoordinates);
            RegionOfInterestSlice slice1 = null;
            RegionOfInterestSlice slice2 = null;

            if (!(index - 1 > RegionOfInterestSlices.Count - 1) && !(index - 1 < 0))
                slice1 = RegionOfInterestSlices[index - 1];
            if (!(index > RegionOfInterestSlices.Count - 1) && !(index < 0))
                slice2 = RegionOfInterestSlices[index];

            if (slice2 == null)
                return slice1;
            if (slice1 == null)
                return slice2;

            if (Math.Abs((slice1.ZCoord - z)) <= Math.Abs((slice2.ZCoord - z)))
                return slice1;
            else
                return slice2;
        }

        /// <summary>
        /// Returns the closest slice with a z value greater than z
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public RegionOfInterestSlice GetClosestSliceNext(double z)
        {
            if (!ZRange.Contains(z))
                return null;

            if (sliceDictionary.ContainsKey(z))
                return sliceDictionary[z];

            int index = BinaryMath.BinarySearchClosest(z, roiZCoordinates);
            if (!(index > RegionOfInterestSlices.Count - 1) && !(index < 0))
                return RegionOfInterestSlices[index];

            return null;
        }
    }
}
