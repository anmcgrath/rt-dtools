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

        public EgsDoseObject Load(string fileName, EgsDoseObject doseObject, IProgress<double> progress)
        {
            GridBasedVoxelDataStructure grid = new GridBasedVoxelDataStructure();
            grid.Name = Path.GetFileName(fileName);
            double max = 0;

            int SizeX, SizeY, SizeZ;
            using (TextReader reader = File.OpenText(fileName))
            {
                SizeX = (int)ReadDouble(reader);
                SizeY = (int)ReadDouble(reader);
                SizeZ = (int)ReadDouble(reader);

                grid.XCoords = new float[SizeX];
                grid.YCoords = new float[SizeY];
                grid.ZCoords = new float[SizeZ];
                grid.Data = new float[SizeX * SizeY * SizeZ];

                fillCoords(grid.XCoords, SizeX, reader);
                fillCoords(grid.YCoords, SizeY, reader);
                fillCoords(grid.ZCoords, SizeZ, reader);

                int Size = SizeX * SizeY * SizeZ;
                for (int i = 0; i < Size; i++)
                {
                    int indexX = i % SizeX;
                    int indexZ = (int)(i / (SizeX * SizeY));
                    int indexY = (int)(i / SizeX) - indexZ * (SizeY);
                    float data = ReadFloat(reader);
                    grid.SetVoxelByIndices(indexX, indexY, indexZ, data);

                    if(data > grid.MaxVoxel.Value)
                    {
                        grid.MaxVoxel.Value = data;
                        grid.MaxVoxel.Position.X = grid.XCoords[indexX];
                        grid.MaxVoxel.Position.Y = grid.XCoords[indexY];
                        grid.MaxVoxel.Position.Z = grid.XCoords[indexZ];
                    }
                    if (data < grid.MinVoxel.Value)
                    {
                        grid.MinVoxel.Value = data;
                        grid.MinVoxel.Position.X = grid.XCoords[indexX];
                        grid.MinVoxel.Position.Y = grid.XCoords[indexY];
                        grid.MinVoxel.Position.Z = grid.XCoords[indexZ];
                    }
                    //only report progress every 5%.
                    if(i%(Size/20)==0)
                        progress.Report(100*(double)i / (double)(Size));
                }
            }
            grid.XRange = new Range((double)grid.XCoords[0], (double)grid.XCoords[grid.XCoords.Length-1]);
            grid.YRange = new Range(grid.YCoords[0], grid.YCoords[grid.YCoords.Length - 1]);
            grid.ZRange = new Range(grid.ZCoords[0], grid.ZCoords[grid.ZCoords.Length - 1]);

            doseObject.Grid = grid;
            doseObject.Grid.ValueUnit = Unit.Egs;
            //doseObject.Grid.ComputeMax();
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

        private void fillCoords(float[] coords, int size, TextReader reader)
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
                coords[i - 1] = (float)(10 * (prevNumber + number) / 2);
                if (i != size)
                {
                    prevNumber = number;
                    number = ReadDouble(reader);
                }
            }
        }
    }
}
