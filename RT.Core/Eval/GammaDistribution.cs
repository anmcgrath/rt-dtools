using RT.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace RT.Core.Eval
{
    public class GammaDistribution
    {
        public IVoxelDataStructure Gamma { get; set; }
        public VectorField Vectors { get; set; } = new VectorField();
        public IVoxelDataStructure Jacobian { get; set; }
    }
}
