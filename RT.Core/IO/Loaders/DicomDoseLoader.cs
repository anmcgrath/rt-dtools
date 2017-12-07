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
    public class DicomDoseLoader:BaseDicomLoader
    {
        public void Load(DicomFile[] files, DicomDoseObject dicomObject, IProgress<double> progress)
        {
            //Load standard DICOM stuff.
            base.Load(files, dicomObject, progress);

            DicomFile file0 = files[0];
            var gridLoader = new GridBasedStructureDicomLoader();
            dicomObject.Grid = gridLoader.Load(files, progress);
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
