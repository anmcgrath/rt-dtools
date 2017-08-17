using RTData.Geometry;
using RTData.Radiotherapy.Dose;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTData.IO.Loaders
{
    public class EgsDoseLoader
    {
        public EgsDoseObject Load(string fileName)
        {
            EgsDoseObject dose = new EgsDoseObject();
            GridBasedVoxelDataStructure grid = new GridBasedVoxelDataStructure();
            dose.Name = Path.GetFileName(fileName);
            string text = File.ReadAllText(fileName);
            string[] numbers = text.Split(new char[] { '\n', ' ', '\t', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int SizeX = Int32.Parse(numbers[0]);
            int SizeY = Int32.Parse(numbers[1]);
            int SizeZ = Int32.Parse(numbers[2]);
            grid.XCoords = new double[SizeX];
            grid.YCoords = new double[SizeY];
            grid.ZCoords = new double[SizeZ];
            grid.Data = new float[SizeX, SizeY, SizeZ];

            int offset = 3;

            for (int i = 0; i < SizeX; i++)
            {
                grid.XCoords[i] = 10 * (Double.Parse(numbers[offset + i]) + Double.Parse(numbers[offset + i + 1])) / 2;
            }
            offset += SizeX + 1;
            for (int i = 0; i < SizeY; i++)
            {
                grid.YCoords[i] = 10 * (Double.Parse(numbers[offset + i]) + Double.Parse(numbers[offset + i + 1])) / 2;
            }
            offset += SizeY + 1;
            for (int i = 0; i < SizeZ; i++)
            {
                grid.ZCoords[i] = 10 * (Double.Parse(numbers[offset + i]) + Double.Parse(numbers[offset + i + 1])) / 2;
            }
            offset += SizeZ + 1;

            for (int i = 0; i < SizeX * SizeY * SizeZ; i++)
            {
                int indexX = i % SizeX;
                int indexZ = (int)(i / (SizeX * SizeY));
                int indexY = (int)(i / SizeX) - indexZ * (SizeY);
                grid.Data[indexX, indexY, indexZ] = float.Parse(numbers[offset + i]);
            }

            foreach(Voxel voxel in grid.Voxels)
            {
                if (voxel.Value > grid.GlobalMax.Value)
                    grid.GlobalMax = voxel;
            }
            return dose;
        }
    }
}
