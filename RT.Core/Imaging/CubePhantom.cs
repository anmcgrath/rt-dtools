using RT.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace RT.Core.Imaging
{
    public class CubePhantom:DicomImageObject
    {
        public CubePhantom()
        {
            this.PatientName = "CUBE^PHANTOM";
            this.Name = "Cube Phantom";
            this.Modality = "CT";
        }

        public void Create(int xWidth, int yWidth, int zWidth, double xSpacing, double ySpacing, double zSpacing)
        {
            int xRows = (int)(xWidth / xSpacing)+1;
            int yRows = (int)(yWidth / ySpacing)+1;
            int zRows = (int)(zWidth / zSpacing)+1;
            var grid = new GridBasedVoxelDataStructure();
            grid.DefaultPhysicalValue = -1000;
            grid.ConstantGridSpacing = true;
            grid.Data = new float[xRows, yRows, zRows];
            grid.XCoords = new double[xRows];
            grid.YCoords = new double[yRows];
            grid.ZCoords = new double[zRows];
            grid.XRange = new Range(-xWidth / 2, xWidth / 2);
            grid.YRange = new Range(-yWidth / 2, yWidth / 2);
            grid.ZRange = new Range(-zWidth / 2, zWidth/ 2);
            for (int i = 0; i < grid.XCoords.Length; i++)
                grid.XCoords[i] = grid.XRange.Minimum + i * xSpacing;
            for (int i = 0; i < grid.YCoords.Length; i++)
                grid.YCoords[i] = grid.YRange.Minimum + i * ySpacing;
            for (int i = 0; i < grid.ZCoords.Length; i++)
                grid.ZCoords[i] = grid.ZRange.Minimum + i * zSpacing;
            grid.GridSpacing = new Utilities.RTMath.Point3d(xSpacing, ySpacing, zSpacing);
            this.Grid = grid;

        }
    }
}
