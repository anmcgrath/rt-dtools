using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Core.Geometry;
using Dicom;
using RT.Core.IO.Loaders;

namespace RT.Core.Dose
{
    public class EgsDoseObject : IDoseObject
    {
        public string FileName { get; set; }
        public string Name { get; set; }
        public IVoxelDataStructure Grid { get; set; }

        public EgsDoseObject(string file)
        {
            var loader = new EgsDoseLoader();
            loader.Load(file, this);
        }

        public float GetNormalisationAmount()
        {
            return Grid.MaxVoxel.Value;
        }
    }
}
