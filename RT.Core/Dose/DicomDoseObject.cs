using Dicom;
using RT.Core.Geometry;
using RT.Core.IO.Loaders;
using RT.Core.DICOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.Dose
{
    public class DicomDoseObject:DicomObject, IDoseObject
    {
        public IVoxelDataStructure Grid { get; set; }
        public double DoseGridScaling { get; set; }
        public string Units { get; set; }
        public string Name { get; set; }

        //Change this to proper normalisation
        public double NormalisationIsodose = 100;

        public DicomDoseObject(params DicomFile[] files):base(files)
        {
            var loader = new DicomDoseLoader();
            loader.Load(files, this);
        }

        public DicomDoseObject() { }

        public float GetNormalisationAmount()
        {
            return Grid.MaxVoxel.Value * ((float) NormalisationIsodose / 100);
        }
    }
}
