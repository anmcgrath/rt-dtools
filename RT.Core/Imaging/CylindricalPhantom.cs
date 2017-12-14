using RT.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace RT.Core.Imaging
{
    public class CylindricalPhantom:DicomImageObject
    {
        public CylindricalPhantom()
        {

        }
        public void Create(int radius, int length, int xSpacing, int ySpacing, int zSpacing)
        {
            int xRows = ( radius * 2) / xSpacing;
            int yRows = (radius * 2) / ySpacing;
            int zRows = length / zSpacing;

            var grid = new GridBasedVoxelDataStructure();
            grid.DefaultPhysicalValue = -1000;
            grid.ConstantGridSpacing = true;
            grid.Data = new float[xRows * yRows * zRows];
            grid.XCoords = new float[xRows];
            grid.YCoords = new float[yRows];
            grid.ZCoords = new float[zRows];
            grid.XRange = new Range(-xRows * xSpacing / 2, xRows * xSpacing / 2);
            grid.YRange = new Range(-yRows * ySpacing / 2, yRows * ySpacing / 2);
            grid.ZRange = new Range(-zRows * zSpacing / 2, zRows * zSpacing / 2);
            for (int i = 0; i < grid.XCoords.Length; i++)
                grid.XCoords[i] = (float)grid.XRange.Minimum + i * xSpacing;
            for (int i = 0; i < grid.YCoords.Length; i++)
                grid.YCoords[i] = (float)grid.YRange.Minimum + i * ySpacing;
            for (int i = 0; i < grid.ZCoords.Length; i++)
                grid.ZCoords[i] = (float)grid.ZRange.Minimum + i * zSpacing;
            grid.GridSpacing = new Utilities.RTMath.Point3d();
            grid.GridSpacing.X = xSpacing;
            grid.GridSpacing.Y = ySpacing;
            grid.GridSpacing.Z = zSpacing;
            

            for(int i = 0; i < zRows; i++)
            {
                for(int j = 0; j < xRows; j++)
                {
                    for(int k = 0; k < yRows; k++)
                    {
                        if(grid.XCoords[j]*grid.XCoords[j] + grid.YCoords[k] * grid.YCoords[k] < radius * radius)
                        {
                            grid.SetVoxelByIndices(i, j, k, 0);
                        }else
                        {
                            grid.SetVoxelByIndices(i, j, j, 0);
                        }
                    }
                }
            }

            this.Grid = grid;
        }
    }
}
