using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DicomPanel.Core.Geometry;
using Dicom;
using DicomPanel.Core.IO.Loaders;

namespace DicomPanel.Core.Radiotherapy.Dose
{
    public class EgsDoseObject : IDoseObject
    {
        public string Name { get; set; }
        public string FileName { get; set; }
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
