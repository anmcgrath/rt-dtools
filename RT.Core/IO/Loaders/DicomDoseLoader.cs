using RT.Core.Dose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicom;
using System.IO;

namespace RT.Core.IO.Loaders
{
    public class DicomDoseLoader
    {
        public void Load(DicomFile[] files, DicomDoseObject dicomObject)
        {
            DicomFile file0 = files[0];
            dicomObject.Scaling = file0.Dataset.Get<float>(DicomTag.DoseGridScaling, 1.0f);
            dicomObject.Units = file0.Dataset.Get<string>(DicomTag.DoseUnits, "None");
            dicomObject.Name = Path.GetFileNameWithoutExtension(file0.File.Name);
            var gridLoader = new GridBasedStructureDicomLoader();
            dicomObject.Grid = gridLoader.Load(files);
        }
    }
}
