using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RT.MonteCarlo.Phantom
{
    public class EgsPhantomCreator
    {
        public void Create(EgsPhantomCreatorOptions options)
        {
            StreamWriter sw;

            using (sw = new StreamWriter(new FileStream(options.OutputFileName, FileMode.Create)))
            {
                WriteMaterials(options, sw);
                WriteVoxelRanges(options, sw);
                WriteVoxelValues(options, sw);
            }
        }

        private void WriteMaterials(EgsPhantomCreatorOptions options, StreamWriter sw)
        {

        }

        private void WriteVoxelValues(EgsPhantomCreatorOptions options, StreamWriter sw)
        {
            for (double z = options.ZRange.Minimum; z < options.ZRange.Maximum; z += options.Dz)
            {
                for (double y = options.YRange.Minimum; y < options.YRange.Maximum; y += options.Dy)
                {
                    for (double x = options.XRange.Minimum; x < options.XRange.Maximum; x += options.Dx)
                    {
                        sw.Write("{0}\t",options.Grid.Interpolate(x,y,z).Value*options.Grid.Scaling);
                    }
                    sw.Write('\n');
                }
            }
        }

        private void WriteVoxelRanges(EgsPhantomCreatorOptions options, StreamWriter sw)
        {
            int rows = (int)(options.YRange.Length / options.Dy);
            int cols = (int)(options.XRange.Length / options.Dx);
            int slices = (int)(options.ZRange.Length / options.Dz);

            sw.WriteLine("{0} {1} {2}", cols, rows, slices);

            for (double x = options.XRange.Minimum; x < options.XRange.Maximum; x += options.Dz)
                sw.Write("{0}\t", x);

            for (double y = options.YRange.Minimum; y < options.YRange.Maximum; y += options.Dy)
                sw.Write("{0}\t", y);

            for (double z = options.ZRange.Minimum; z < options.ZRange.Maximum; z++)
                sw.Write("{0}\t", z);
        }
    }
}
