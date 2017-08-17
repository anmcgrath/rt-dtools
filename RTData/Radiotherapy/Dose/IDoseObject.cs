using RTData.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTData.Radiotherapy.Dose
{
    public interface IDoseObject
    {
        IVoxelDataStructure Grid { get; set; }
    }
}
