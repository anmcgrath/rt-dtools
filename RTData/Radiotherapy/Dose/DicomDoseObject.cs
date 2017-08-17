using Dicom;
using RTData.Geometry;
using RTData.IO.Loaders;
using RTData.Radiotherapy.DICOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTData.Radiotherapy.Dose
{
    public class DicomDoseObject:DicomObject, IDoseObject
    {
        public IVoxelDataStructure Grid { get; set; }
        public double DoseGridScaling { get; set; }
        public string Units { get; set; }

        public DicomDoseObject(params DicomFile[] files):base(files)
        {
            var loader = new DicomDoseLoader();
            loader.Load(files, this);
        }

        public DicomDoseObject() { }
    }
}
