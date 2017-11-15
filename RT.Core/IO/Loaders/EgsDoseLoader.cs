using RT.Core.Geometry;
using RT.Core.Dose;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.IO.Loaders
{
    public class EgsDoseLoader
    {

        public EgsDoseObject Load(string fileName, EgsDoseObject doseObject)
        {
            GridBasedVoxelDataStructure grid = new GridBasedVoxelDataStructure();
            doseObject.Name = Path.GetFileName(fileName);
            double max = 0;

            int SizeX, SizeY, SizeZ;
            using (TextReader reader = File.OpenText(fileName))
            {
                SizeX = (int)ReadDouble(reader);
                SizeY = (int)ReadDouble(reader);
                SizeZ = (int)ReadDouble(reader);

                grid.XCoords = new double[SizeX];
                grid.YCoords = new double[SizeY];
                grid.ZCoords = new double[SizeZ];
                grid.Data = new float[SizeX, SizeY, SizeZ];

                fillCoords(grid.XCoords, SizeX, reader);
                fillCoords(grid.YCoords, SizeY, reader);
                fillCoords(grid.ZCoords, SizeZ, reader);

                for (int i = 0; i < SizeX * SizeY * SizeZ; i++)
                {
                    int indexX = i % SizeX;
                    int indexZ = (int)(i / (SizeX * SizeY));
                    int indexY = (int)(i / SizeX) - indexZ * (SizeY);
                    float data = ReadFloat(reader);
                    grid.Data[indexX, indexY, indexZ] = data;
                }
            }
            grid.XRange = new Range((double)grid.XCoords[0], (double)grid.XCoords[grid.XCoords.Length-1]);
            grid.YRange = new Range(grid.YCoords[0], grid.YCoords[grid.YCoords.Length - 1]);
            grid.ZRange = new Range(grid.ZCoords[0], grid.ZCoords[grid.ZCoords.Length - 1]);

            doseObject.Grid = grid;
            doseObject.Grid.ComputeMax();
            return doseObject;
        }

        private double ReadDouble(TextReader reader)
        {
            string numberString = readNumberString(reader);
            bool parseNumber = Double.TryParse(numberString, out double number);
            if (parseNumber)
                return number;
            else
                return Double.NaN;
        }

        private float ReadFloat(TextReader reader)
        {
            string numberString = readNumberString(reader);
            bool parseNumber = float.TryParse(numberString, out float number);
            if (parseNumber)
                return number;
            else
                return float.NaN;
        }

        private string readNumberString(TextReader reader)
        {
            while (Char.IsWhiteSpace((char)reader.Peek())) //Read all the whitespace until the start of the next number
                reader.Read();
            string numberString = "";
            bool inNumber = true;
            while (inNumber)
            {
                int num = reader.Read();
                char c = (char)num;
                if (Char.IsWhiteSpace(c))
                    inNumber = false;
                else
                {
                    numberString += c;
                }
            }
            return numberString;
        }

        private void fillCoords(double[] coords, int size, TextReader reader)
        {
            double prevNumber = 0;
            double number = 0;
            for (int i = 0; i < size + 1; i++)
            {
                if (i == 0)
                {
                    prevNumber = ReadDouble(reader);
                    number = ReadDouble(reader);
                    continue;
                }
                // Take the centre of the voxel as our coord location as 3ddose files list the voxel boundaries
                coords[i - 1] = 10 * (prevNumber + number) / 2;
                if (i != size)
                {
                    prevNumber = number;
                    number = ReadDouble(reader);
                }
            }
        }
    }
}
