using Dicom;
using Dicom.Imaging;
using RTData.Geometry;
using RTData.Radiotherapy;
using RTData.Radiotherapy.DICOM;
using RTData.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTData.IO.Loaders
{
    public class SliceBasedStructureDicomLoader
    {
        public SliceBasedVoxelDataStructure Load(DicomFile[] files)
        {
            SliceBasedVoxelDataStructure grid = new SliceBasedVoxelDataStructure();
            files = sort(files);

            PixelDataInfo[] headerInfos = GetHeaderInfo(files);

            if (files.Length > 1)
            {
                grid.MatrixA = getMatrixA(files.First(), files.Last(), files.Length);
            }
            else
            {
                grid.MatrixA = getSingleMatrixA(files[0]);
            }

            for (int i = 0; i < files.Length; i++)
            {
                PixelDataInfo headerInfo = headerInfos[i];
                DicomPixelData dicomPixelData = DicomPixelData.Create(files[i].Dataset);
                
                for (int j = 0; j < headerInfo.GridFrameOffsetVector.Length; j++)
                {
                    byte[] framePixelData = dicomPixelData.GetFrame(j).Data;
                    float[] dataArray = GetDataArray(framePixelData, dicomPixelData.BitsAllocated, headerInfo.PixelRepresentation);
                    //rescale to proper values
                    for(int k = 0; k < dataArray.Length; k++)
                    {
                        dataArray[k] = dataArray[k]*headerInfo.RescaleSlope + headerInfo.RescaleIntercept;
                    }

                    grid.AddSlice(dataArray, headerInfo.Rows, headerInfo.Columns, headerInfo.PixelSpacing[0], headerInfo.PixelSpacing[1], headerInfo.ImagePositionPatient.X, headerInfo.ImagePositionPatient.Y, headerInfo.ImagePositionPatient.Z + headerInfo.GridFrameOffsetVector[j], headerInfo.RowDir.X, headerInfo.RowDir.Y, headerInfo.RowDir.Z, headerInfo.ColDir.X, headerInfo.ColDir.Y, headerInfo.ColDir.Z);
                }
                files[i] = null;
            }

            //grid.Window = (int)grid.GlobalMax.Value ;
            //grid.Level = (int)(grid.GlobalMax.Value / 2);

            return grid;
        }

        /// <summary>
        /// A maps the the (i,j,s) index to the patient coordinate position where i,j,s are the x y and z indices respectively.
        /// See http://nipy.org/nibabel/dicom/dicom_orientation.html
        /// </summary>
        /// <param name="firstFile"></param>
        /// <param name="lastFile"></param>
        /// <param name="N"></param>
        /// <returns></returns>
        private  Matrix4d getMatrixA(DicomFile firstFile, DicomFile lastFile, int N)
        {
            double[] i = firstFile.Dataset.Get<double[]>(DicomTag.ImageOrientationPatient);
            double[,] F =
            new double[,]
            {
                { i[3], i[0] },
                { i[4], i[1] },
                { i[5], i[2] }
            };

            double[] T1 = firstFile.Dataset.Get<double[]>(DicomTag.ImagePositionPatient);
            double[] TN = lastFile.Dataset.Get<double[]>(DicomTag.ImagePositionPatient);
            double[] px = firstFile.Dataset.Get<double[]>(DicomTag.PixelSpacing);
            double dr = px[0];
            double dc = px[1];
            double k1 = (T1[0] - TN[0]) / (1 - N);
            double k2 = (T1[1] - TN[1]) / (1 - N);
            double k3 = (T1[2] - TN[2]) / (1 - N);
            Matrix4d A = new Matrix4d(
                new double[] { F[0, 0] * dr, F[1, 0] * dr, F[2, 0] * dr, 0, F[0, 1] * dc, F[1, 1] * dc, F[2, 1] * dc, 0, k1, k2, k3, 0, T1[0], T1[1], T1[2], 1 });
            return A;
        }

        private  Matrix4d getSingleMatrixA(DicomFile file)
        {
            double[] i = file.Dataset.Get<double[]>(DicomTag.ImageOrientationPatient);
            double[,] F =
                new double[,]
                {
                    { i[3], i[0] },
                    { i[4], i[1] },
                    { i[5], i[2] }
                };

            Point3d F0 = new Point3d(F[0, 0], F[1, 0], F[2, 0]);
            Point3d F1 = new Point3d(F[1, 1], F[1, 1], F[2, 1]);
            var n = F0.Cross(F1);
            double[] T1 = file.Dataset.Get<double[]>(DicomTag.ImagePositionPatient);
            double[] TN = file.Dataset.Get<double[]>(DicomTag.ImagePositionPatient);
            double[] px = file.Dataset.Get<double[]>(DicomTag.PixelSpacing);
            double dr = px[0];
            double dc = px[1];


            double k1 = n.X * 2;
            double k2 = n.Y * 2;
            double k3 = n.Z * 2;

            Matrix4d A = new Matrix4d(
                new double[] { F[0, 0] * dr, F[1, 0] * dr, F[2, 0] * dr, 0, F[0, 1] * dc, F[1, 1] * dc, F[2, 1] * dc, 0, k1, k2, k3, 0, T1[0], T1[1], T1[2], 1 });
            return A;
        }

        /// <summary>
        /// Sort the files based on the offset position from the first (sorted) file
        /// See http://nipy.org/nibabel/dicom/dicom_orientation.html
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private  DicomFile[] sort(DicomFile[] files)
        {
            if (files.Length == 1)
                return files;

            if (files[0].Dataset.Get<double>(DicomTag.SliceLocation, double.MaxValue) != double.MaxValue)
            {
                return sortBySliceLocation(files);
            }

            DicomFile[] sortedArray = new DicomFile[files.Length];
            //d should be an offset from the first (sorted) file.
            double[] d = new double[files.Length];
            double ds = 1;

            for (int j = 0; j < files.Length; j++)
            {
                double[] i = files[j].Dataset.Get<double[]>(DicomTag.ImageOrientationPatient);
                double[] Tj = files[j].Dataset.Get<double[]>(DicomTag.ImagePositionPatient);

                Point3d F0 = new Point3d(i[3], i[4], i[5]);
                Point3d F1 = new Point3d(i[0], i[1], i[2]);
                var n = F0.Cross(F1) * ds;
                d[j] = Tj[0] * n.X + Tj[1] * n.Y + Tj[2] * n.Z;
            }

            List<DicomFile> sortedList = new List<DicomFile>();
            List<double> sortedListD = new List<double>();
            sortedList.Add(files[0]);
            sortedListD.Add(d[0]);

            for (int i = 1; i < files.Length; i++)
            {
                for (int j = 0; j < sortedList.Count; j++)
                {
                    if (sortedList.Count == 1)
                    {
                        if (sortedListD[j] > d[i])
                        {
                            sortedList.Add(files[i]);
                            sortedListD.Add(d[i]);
                            break;
                        }
                        else
                        {
                            sortedList.Insert(0, files[i]);
                            sortedListD.Insert(0, d[i]);
                            break;
                        }
                    }
                    else if (j < sortedList.Count - 1)
                    {
                        if (d[i] >= sortedListD[j] && d[i] <= sortedListD[j + 1])
                        {
                            sortedList.Insert(j + 1, files[i]);
                            sortedListD.Insert(j + 1, d[i]);
                            break;
                        }
                    }
                    else
                    {
                        sortedListD.Add(d[i]);
                        sortedList.Add(files[i]);
                        break;
                    }
                }
            }

            return sortedList.ToArray();
        }

        private  PixelDataInfo[] GetHeaderInfo(DicomFile[] files)
        {
            PixelDataInfo[] headerInfo = new PixelDataInfo[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                headerInfo[i] = new PixelDataInfo(files[i]);
            }
            return headerInfo;
        }

        private  DicomFile[] sortBySliceLocation(DicomFile[] files)
        {
            List<DicomFile> sortedList = files.ToList();
            sortedList.Sort(delegate (DicomFile x, DicomFile y)
            {
                var slicelocationx = x.Dataset.Get<double>(DicomTag.SliceLocation, 0.0);
                var slicelocationy = y.Dataset.Get<double>(DicomTag.SliceLocation, 0.0);
                if (slicelocationx == slicelocationy) return 0;
                else if (slicelocationx < slicelocationy) return -1;
                else if (slicelocationx > slicelocationy) return 1;
                else return 0;
            });
            return sortedList.ToArray();
        }

        private  float[] GetDataArray(byte[] bytes, int bitsAllocated, int pixelRepresentation)
        {
            float[] dataArray = new float[bytes.Length / (bitsAllocated / 8)];
            for (int j = 0, p = 0; j < (bytes.Length / (bitsAllocated / 8)); ++j, p += (bitsAllocated / 8))
            {
                if (pixelRepresentation == 0)
                {
                    if (bitsAllocated == 16)
                        dataArray[j] = (int)BitConverter.ToUInt16(bytes, p);
                    else if (bitsAllocated == 32)
                        dataArray[j] = (int)BitConverter.ToUInt32(bytes, p);
                }
                else
                {
                    if (bitsAllocated == 16)
                        dataArray[j] = (int)BitConverter.ToInt16(bytes, p);
                    else if (bitsAllocated == 32)
                        dataArray[j] = (int)BitConverter.ToInt32(bytes, p);
                }
            }
            return dataArray;
        }
    }
}
