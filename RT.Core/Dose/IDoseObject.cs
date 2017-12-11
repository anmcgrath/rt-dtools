using RT.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.Dose
{
    public interface IDoseObject
    {
        IVoxelDataStructure Grid { get; set; }
    }
}
