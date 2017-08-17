using DicomPanel.Core.Radiotherapy.Dose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicom;

namespace DicomPanel.Core.IO.Loaders
{
    public class DicomDoseLoader
    {
        public void Load(DicomFile[] files, DicomDoseObject dicomObject)
        {
            DicomFile file0 = files[0];
            dicomObject.DoseGridScaling = file0.Dataset.Get<double>(DicomTag.DoseGridScaling, 1.0d);
            dicomObject.Units = file0.Dataset.Get<string>(DicomTag.DoseUnits, "None");
            var gridLoader = new GridBasedStructureDicomLoader();
            dicomObject.Grid = gridLoader.Load(files);
        }
    }
}
