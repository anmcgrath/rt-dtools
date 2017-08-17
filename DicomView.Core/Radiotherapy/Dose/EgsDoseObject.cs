using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DicomPanel.Core.Geometry;

namespace DicomPanel.Core.Radiotherapy.Dose
{
    public class EgsDoseObject : IDoseObject
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public IVoxelDataStructure Grid { get; set; }

        public float GetNormalisationAmount()
        {
            return Grid.MaxVoxel.Value;
        }
    }
}
