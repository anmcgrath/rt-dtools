using RT.Core.Dose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicom;
using System.IO;
using RT.Core.Geometry;

namespace RT.Core.IO.Loaders
{
    public class DicomDoseLoader
    {
        public void Load(DicomFile[] files, DicomDoseObject dicomObject)
        {
            DicomFile file0 = files[0];
            var gridLoader = new GridBasedStructureDicomLoader();
            dicomObject.Grid = gridLoader.Load(files);
            dicomObject.Name = Path.GetFileNameWithoutExtension(file0.File.Name);
            dicomObject.Grid.Scaling = file0.Dataset.Get<float>(DicomTag.DoseGridScaling, 1.0f);
            dicomObject.Grid.ValueUnit = unitFromString(file0.Dataset.Get<string>(DicomTag.DoseUnits, "Relative"));
            dicomObject.Grid.Name = Path.GetFileNameWithoutExtension(file0.File.Name);
        }

        private Unit unitFromString(string unit)
        {
            switch(unit.ToLower())
            {
                case "gy": return Unit.Gy;
                case "relative":return Unit.Percent;
            }
            return Unit.Percent;
        }
    }
}
