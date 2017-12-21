using System;
using System.Collections.Generic;
using System.Text;

namespace RT.Core.Geometry
{
    public class VectorField
    {
        public IVoxelDataStructure X { get; set; }
        public IVoxelDataStructure Y { get; set; }
        public IVoxelDataStructure Z { get; set; }
        public VectorField()
        {

        }
    }
}
