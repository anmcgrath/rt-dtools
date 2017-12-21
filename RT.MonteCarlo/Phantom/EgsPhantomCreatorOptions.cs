using RT.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace RT.MonteCarlo.Phantom
{
    public class EgsPhantomCreatorOptions
    {
        public string OutputFileName { get; set; }
        public double Dx { get; set; }
        public double Dy { get; set; }
        public double Dz { get; set; }
        public Range XRange { get; set; }
        public Range YRange { get; set; }
        public Range ZRange { get; set; }
        public IVoxelDataStructure Grid { get; set; }

        public EgsPhantomCreatorOptions()
        {
            XRange = new Range();
            YRange = new Range();
            ZRange = new Range();
        }
    }
}
