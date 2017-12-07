using Dicom;
using Dicom.Imaging;
using RT.Core.Geometry;
using RT.Core.DICOM;
using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.IO.Loaders
{
    public class GridBasedStructureDicomLoader
    {
        public GridBasedVoxelDataStructure Load(DicomFile[] dicomFiles, IProgress<double> progress)
        {
            GridBasedVoxelDataStructure grid = new GridBasedVoxelDataStructure();
            DicomFile[] files = SortOnZIncreasing(dicomFiles);
            PixelDataInfo DicomHeader = new PixelDataInfo(files[0]);
            Point3d size = GetSize(files);

            grid.ConstantGridSpacing = true;
            grid.GridSpacing = GetSpacing(files);
            grid.XCoords = GetCoords(files, (int)size.X, DicomHeader.RowDir.X, DicomHeader.ColDir.X, DicomHeader.ImagePositionPatient.X, 1);
            grid.YCoords = GetCoords(files, (int)size.Y, DicomHeader.RowDir.Y, DicomHeader.ColDir.Y, DicomHeader.ImagePositionPatient.Y, 1);
            grid.ZCoords = GetCoords(files, (int)size.Z, DicomHeader.RowDir.Z, DicomHeader.ColDir.Z, DicomHeader.ImagePositionPatient.Z, 1);
            grid.Data = new float[(int)size.X / 1, (int)size.Y / 1, (int)size.Z / 1];

            int currYIndex = 0;
            int currZIndex = 0;
            int currXIndex = 0;

            for (int i = 0; i < files.Length; i++)
            {
                DicomPixelData dicomPixelData = DicomPixelData.Create(files[i].Dataset);
                PixelDataInfo tempHeaderInfo = new PixelDataInfo(files[i]);
                for (int j = 0; j < tempHeaderInfo.GridFrameOffsetVector.Length; j++)
                {
                    byte[] framePixelData = dicomPixelData.GetFrame(j).Data;
                    float[] dataArray = GetDataArray(framePixelData, tempHeaderInfo.BitsAllocated, tempHeaderInfo.PixelRepresentation);
                    for (int k = 0; k < dataArray.Length; k++)
                    {
                        int currRow = k % tempHeaderInfo.Columns;
                        int currCol = (int)Math.Floor((double)(k / (tempHeaderInfo.Columns)));
                        currXIndex = (int)Math.Abs(tempHeaderInfo.RowDir.X) * currRow + (int)Math.Abs(tempHeaderInfo.ColDir.X) * currCol
                            + (1 - (int)Math.Abs(tempHeaderInfo.RowDir.X) - (int)Math.Abs(tempHeaderInfo.ColDir.X)) * (i + j); ;
                        currYIndex = (int)Math.Abs(tempHeaderInfo.RowDir.Y) * currRow + (int)Math.Abs(tempHeaderInfo.ColDir.Y) * currCol
                            + (1 - (int)Math.Abs(tempHeaderInfo.RowDir.Y) - (int)Math.Abs(tempHeaderInfo.ColDir.Y)) * (i + j); ;
                        currZIndex = (int)Math.Abs(tempHeaderInfo.RowDir.Z) * currRow + (int)Math.Abs(tempHeaderInfo.ColDir.Z) * currCol
                            + (1 - (int)Math.Abs(tempHeaderInfo.RowDir.Z) - (int)Math.Abs(tempHeaderInfo.ColDir.Z)) * (i + j);

                        //Rescale back to actual HUs
                        dataArray[k] = dataArray[k] * tempHeaderInfo.RescaleSlope + tempHeaderInfo.RescaleIntercept;
                        grid.Data[currXIndex, currYIndex, currZIndex] = dataArray[k];

                        if (dataArray[k] > grid.MaxVoxel.Value)
                        {
                            grid.MaxVoxel.Value = dataArray[k];
                            grid.MaxVoxel.Position.X = grid.XCoords[currXIndex];
                            grid.MaxVoxel.Position.Y = grid.YCoords[currYIndex];
                            grid.MaxVoxel.Position.Z = grid.ZCoords[currZIndex];
                        }
                        if(dataArray[k] < grid.MinVoxel.Value)
                        {
                            grid.MinVoxel.Value = dataArray[k];
                            grid.MinVoxel.Position.X = grid.XCoords[currXIndex];
                            grid.MinVoxel.Position.Y = grid.YCoords[currYIndex];
                            grid.MinVoxel.Position.Z = grid.ZCoords[currZIndex];
                        }

                    }

                    progress.Report(100 * ((((double)j / (double)tempHeaderInfo.GridFrameOffsetVector.Length) / files.Length) + ((double)i / (double)files.Length)));
                }
            }
            grid.XRange = new Range(grid.XCoords[0], grid.XCoords[grid.XCoords.Length - 1]);
            grid.YRange = new Range(grid.YCoords[0], grid.YCoords[grid.YCoords.Length - 1]);
            grid.ZRange = new Range(grid.ZCoords[0], grid.ZCoords[grid.ZCoords.Length - 1]);
            
            //grid.ComputeMax();
            return grid;
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

        private  double[] GetCoords(DicomFile[] files, int size, double rowDir, double colDir, double position, int scale)
        {
            PixelDataInfo tempHeader = new PixelDataInfo(files[0]);
            double[] coords = new double[size];
            if (rowDir != 0 || colDir != 0)
            {
                for (int i = 0; i < size; i++)
                {
                    coords[i] = position + i * rowDir * tempHeader.PixelSpacing[1] + i * colDir * tempHeader.PixelSpacing[0];
                }
            }
            else
            {
                if (tempHeader.GridFrameOffsetVector.Length > 1)
                {
                    for (int i = 0; i < size; i++)
                    {
                        coords[i] = position + tempHeader.GridFrameOffsetVector[i];
                    }
                }
                for (int i = 0; i < files.Length; i++)
                {
                    coords[i] = position + i * tempHeader.SliceThickness;
                }
            }
            return coords;
        }

        private  Point3d GetSize(DicomFile[] files)
        {
            Point3d size = new Point3d(0, 0, 0);
            if (files.Length == 1)
                return GetSize(files[0]);
            else
            {
                PixelDataInfo fileHeader = new PixelDataInfo(files[0]);
                size.X += Math.Abs(fileHeader.RowDir.X) * fileHeader.Columns + Math.Abs(fileHeader.ColDir.X) * fileHeader.Rows;
                size.Y += Math.Abs(fileHeader.RowDir.Y) * fileHeader.Columns + Math.Abs(fileHeader.ColDir.Y) * fileHeader.Rows;
                size.Z += Math.Abs(fileHeader.RowDir.Z) * fileHeader.Columns + Math.Abs(fileHeader.ColDir.Z) * fileHeader.Rows;
                size.X += (1 - Math.Abs(fileHeader.RowDir.X) - Math.Abs(fileHeader.ColDir.X)) * files.Length;
                size.Y += (1 - Math.Abs(fileHeader.RowDir.Y) - Math.Abs(fileHeader.ColDir.Y)) * files.Length;
                size.Z += (1 - Math.Abs(fileHeader.RowDir.Z) - Math.Abs(fileHeader.ColDir.Z)) * files.Length;
            }
            return size;
        }

        private  Point3d GetSize(DicomFile file)
        {
            PixelDataInfo fileHeader = new PixelDataInfo(file);
            Point3d size = new Point3d();
            size.X += Math.Abs(fileHeader.RowDir.X) * fileHeader.Columns + Math.Abs(fileHeader.ColDir.X) * fileHeader.Rows;
            size.Y += Math.Abs(fileHeader.RowDir.Y) * fileHeader.Columns + Math.Abs(fileHeader.ColDir.Y) * fileHeader.Rows;
            size.Z += Math.Abs(fileHeader.RowDir.Z) * fileHeader.Columns + Math.Abs(fileHeader.ColDir.Z) * fileHeader.Rows;
            size.X += (1 - Math.Abs(fileHeader.RowDir.X) - Math.Abs(fileHeader.ColDir.X)) * fileHeader.GridFrameOffsetVector.Length;
            size.Y += (1 - Math.Abs(fileHeader.RowDir.Y) - Math.Abs(fileHeader.ColDir.Y)) * fileHeader.GridFrameOffsetVector.Length;
            size.Z += (1 - Math.Abs(fileHeader.RowDir.Z) - Math.Abs(fileHeader.ColDir.Z)) * fileHeader.GridFrameOffsetVector.Length;
            return size;
        }

        private  Point3d GetSpacing(DicomFile[] files)
        {
            Point3d spacing = new Point3d(0, 0, 0);
            if (files.Length == 1)
                return GetSpacing(files[0]);
            else
            {
                PixelDataInfo fileHeader = new PixelDataInfo(files[0]);
                double thirdDimension = 0;
                if (fileHeader.SliceThickness != 0)
                    thirdDimension = fileHeader.SliceThickness;
                else if (fileHeader.GridFrameOffsetVector.Length > 1)
                    thirdDimension = fileHeader.GridFrameOffsetVector[1] - fileHeader.GridFrameOffsetVector[0];

                spacing.X += Math.Abs(fileHeader.RowDir.X) * fileHeader.PixelSpacing[1] + Math.Abs(fileHeader.ColDir.X) * fileHeader.PixelSpacing[0];
                spacing.Y += Math.Abs(fileHeader.RowDir.Y) * fileHeader.PixelSpacing[1] + Math.Abs(fileHeader.ColDir.Y) * fileHeader.PixelSpacing[0];
                spacing.Z += Math.Abs(fileHeader.RowDir.Z) * fileHeader.PixelSpacing[1] + Math.Abs(fileHeader.ColDir.Z) * fileHeader.PixelSpacing[0];
                spacing.X += (1 - Math.Abs(fileHeader.RowDir.X) - Math.Abs(fileHeader.ColDir.X)) * thirdDimension;
                spacing.Y += (1 - Math.Abs(fileHeader.RowDir.Y) - Math.Abs(fileHeader.ColDir.Y)) * thirdDimension;
                spacing.Z += (1 - Math.Abs(fileHeader.RowDir.Z) - Math.Abs(fileHeader.ColDir.Z)) * thirdDimension;
            }
            return spacing;
        }

        private  Point3d GetSpacing(DicomFile file)
        {
            PixelDataInfo fileHeader = new PixelDataInfo(file);
            Point3d spacing = new Point3d();

            double thirdDimension = 0;
            if (fileHeader.SliceThickness != 0)
                thirdDimension = fileHeader.SliceThickness;
            else if (fileHeader.GridFrameOffsetVector.Length > 1)
                thirdDimension = fileHeader.GridFrameOffsetVector[1] - fileHeader.GridFrameOffsetVector[0];

            spacing.X += Math.Abs(fileHeader.RowDir.X) * fileHeader.PixelSpacing[1] + Math.Abs(fileHeader.ColDir.X) * fileHeader.PixelSpacing[0];
            spacing.Y += Math.Abs(fileHeader.RowDir.Y) * fileHeader.PixelSpacing[1] + Math.Abs(fileHeader.ColDir.Y) * fileHeader.PixelSpacing[0];
            spacing.Z += Math.Abs(fileHeader.RowDir.Z) * fileHeader.PixelSpacing[1] + Math.Abs(fileHeader.ColDir.Z) * fileHeader.PixelSpacing[0];
            spacing.X += (1 - Math.Abs(fileHeader.RowDir.X) - Math.Abs(fileHeader.ColDir.X)) * thirdDimension;
            spacing.Y += (1 - Math.Abs(fileHeader.RowDir.Y) - Math.Abs(fileHeader.ColDir.Y)) * thirdDimension;
            spacing.Z += (1 - Math.Abs(fileHeader.RowDir.Z) - Math.Abs(fileHeader.ColDir.Z)) * thirdDimension;
            return spacing;
        }

        /// <summary>
        /// Sorts a list of dicom files by their Z values
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private  DicomFile[] SortOnZIncreasing(DicomFile[] files)
        {
            bool swapped;
            do
            {
                swapped = false;
                for (int i = 1; i < files.Length; i++)
                {
                    if (files[i - 1].Dataset.Get<double[]>(DicomTag.ImagePositionPatient)[2] >
                        files[i].Dataset.Get<double[]>(DicomTag.ImagePositionPatient)[2])
                    {
                        swapped = true;
                        DicomFile temp = files[i];
                        files[i] = files[i - 1];
                        files[i - 1] = temp;
                    }
                }
            } while (swapped);
            return files;
        }
    }
}
