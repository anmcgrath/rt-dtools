using Dicom;
using DicomPanel.Core.Geometry;
using DicomPanel.Core.IO.Loaders;
using DicomPanel.Core.Radiotherapy.DICOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomPanel.Core.Radiotherapy.Dose
{
    public class DicomDoseObject:DicomObject, IDoseObject
    {
        public IVoxelDataStructure Grid { get; set; }
        public double DoseGridScaling { get; set; }
        public string Units { get; set; }
        public string Name { get; set; }

        public DicomDoseObject(params DicomFile[] files):base(files)
        {
            var loader = new DicomDoseLoader();
            loader.Load(files, this);
        }

        public DicomDoseObject() { }

        public float GetNormalisationAmount()
        {
            return Grid.MaxVoxel.Value;
        }
    }
}
