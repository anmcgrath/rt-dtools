using Dicom;
using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.DICOM
{
    /// <summary>
    /// Stores Info on the PixelData for a DICOM Object
    /// </summary>
    public class PixelDataInfo
    {
        public Point3d RowDir { get; set; }
        public Point3d ColDir { get; set; }
        public Point3d ImagePositionPatient { get; set; }
        public double[] PixelSpacing { get; set; }
        public string ImageType { get; set; }
        public double SliceThickness { get; set; }
        public double SpacingBetweenSlices { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int BitsAllocated { get; set; }
        public int PixelRepresentation { get; set; }
        public int NumberOfFrames { get; set; }
        public float RescaleSlope { get; set; }
        public float RescaleIntercept { get; set; }
        public double SliceLocation { get; set; }
        public bool HasSliceLocation { get; set; }
        public double[] GridFrameOffsetVector { get; set; }

        public PixelDataInfo(DicomFile file)
        {
            var imgOrientationPatient = new double[] { 0, 0, 0, 0, 0, 0 };
            if (file.Dataset.TryGetValues<double>(DicomTag.ImageOrientationPatient, out double[] tmp))
            {
                imgOrientationPatient = tmp;
            }

            RowDir = new Point3d(imgOrientationPatient[0], imgOrientationPatient[1], imgOrientationPatient[2]);
            ColDir = new Point3d(imgOrientationPatient[3], imgOrientationPatient[4], imgOrientationPatient[5]);

            var imgPositionPatient = new double[] { 0, 0, 0 };
            if (file.Dataset.TryGetValues<double>(DicomTag.ImagePositionPatient, out tmp))
            {
                imgPositionPatient = tmp;
            }

            ImagePositionPatient = new Point3d(imgPositionPatient[0], imgPositionPatient[1], imgPositionPatient[2]);

            SliceThickness = file.Dataset.GetSingleValueOrDefault<double>(DicomTag.SliceThickness, 0.0);

            //winson add 2020.9.2 - Inconsistent 'Slice Thickness' and 'Spacing Between Slices'
            SpacingBetweenSlices = file.Dataset.GetSingleValueOrDefault<double>(DicomTag.SpacingBetweenSlices, 0.0);
            if (SpacingBetweenSlices > 0.0)
            {
                SliceThickness = SpacingBetweenSlices;
            }

            Rows = file.Dataset.GetSingleValue<int>(DicomTag.Rows);
            Columns = file.Dataset.GetSingleValue<int>(DicomTag.Columns);
            BitsAllocated = file.Dataset.GetSingleValueOrDefault<int>(DicomTag.BitsAllocated, 0);
            PixelRepresentation = file.Dataset.GetSingleValueOrDefault<int>(DicomTag.PixelRepresentation, 0);
            RescaleSlope = file.Dataset.GetSingleValueOrDefault<float>(DicomTag.RescaleSlope, 1.0f);
            RescaleIntercept = file.Dataset.GetSingleValueOrDefault<float>(DicomTag.RescaleIntercept, 0.0f);

            if (file.Dataset.TryGetValues<double>(DicomTag.PixelSpacing, out tmp))
            {
                PixelSpacing = tmp;
            }
            else
            {
                PixelSpacing =  new double[] { 1, 1, 1 };
            }            

            try
            {
                SliceLocation = file.Dataset.GetSingleValueOrDefault<double>(DicomTag.SliceLocation, 0.0);
                HasSliceLocation = true;
            }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
            catch (Exception e)
#pragma warning restore CS0168 // The variable 'e' is declared but never used
            {
                HasSliceLocation = false;
            }

            if (file.Dataset.TryGetValues<double>(DicomTag.GridFrameOffsetVector, out tmp))//All Z-offsets
            {
                GridFrameOffsetVector = tmp;
            }
            else
            {
                GridFrameOffsetVector = new double[1];
            }
        }
    }
}
