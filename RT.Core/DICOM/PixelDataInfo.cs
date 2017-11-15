using Dicom;
using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.DICOM
{
    public class PixelDataInfo
    {
        public Point3d RowDir { get; set; }
        public Point3d ColDir { get; set; }
        public Point3d ImagePositionPatient { get; set; }
        public double[] PixelSpacing { get; set; }
        public string ImageType { get; set; }
        public double SliceThickness { get; set; }
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
            var imgOrientationPatient = file.Dataset.Get<double[]>(DicomTag.ImageOrientationPatient, new double[] { 0, 0, 0, 0, 0, 0 });
            RowDir = new Point3d(imgOrientationPatient[0], imgOrientationPatient[1], imgOrientationPatient[2]);
            ColDir = new Point3d(imgOrientationPatient[3], imgOrientationPatient[4], imgOrientationPatient[5]);

            var imgPositionPatient = file.Dataset.Get<double[]>(DicomTag.ImagePositionPatient, new double[] { 0, 0, 0 });
            ImagePositionPatient = new Point3d(imgPositionPatient[0], imgPositionPatient[1], imgPositionPatient[2]);

            SliceThickness = file.Dataset.Get<double>(DicomTag.SliceThickness, 0.0);
            Rows = file.Dataset.Get<int>(DicomTag.Rows);
            Columns = file.Dataset.Get<int>(DicomTag.Columns);
            BitsAllocated = file.Dataset.Get<int>(DicomTag.BitsAllocated, 0);
            PixelRepresentation = file.Dataset.Get<int>(DicomTag.PixelRepresentation, 0);
            RescaleSlope = file.Dataset.Get<float>(DicomTag.RescaleSlope, 1.0f);
            RescaleIntercept = file.Dataset.Get<float>(DicomTag.RescaleIntercept, 0.0f);
            PixelSpacing = file.Dataset.Get<double[]>(DicomTag.PixelSpacing, new double[] {1,1,1 });

            try
            {
                SliceLocation = file.Dataset.Get<double>(DicomTag.SliceLocation, 0.0);
                HasSliceLocation = true;
            }
            catch (Exception e)
            {
                HasSliceLocation = false;
            }

            try
            {
                GridFrameOffsetVector = file.Dataset.Get<DicomDecimalString>(DicomTag.GridFrameOffsetVector).Get<double[]>(); //All Z-offsets
            }
            catch (Exception e) { GridFrameOffsetVector = new double[1]; };
        }
    }
}
