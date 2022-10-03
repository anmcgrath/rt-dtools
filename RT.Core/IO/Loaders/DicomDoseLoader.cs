using RT.Core.Dose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicom;
using System.IO;
using RT.Core.Geometry;
using Dicom.StructuredReport;

namespace RT.Core.IO.Loaders
{
    public class DicomDoseLoader:BaseDicomLoader
    {
        public void Load(DicomFile[] files, DicomDoseObject dicomObject, IProgress<double> progress)
        {
            //Load standard DICOM stuff.
            base.Load(files, dicomObject, progress);

            DicomFile file0 = files[0];
            Load(file0, dicomObject,progress);
        }

        public void Load(DicomFile file, DicomDoseObject dicomObject, IProgress<double> progress)
        {
            var gridLoader = new GridBasedStructureDicomLoader();
            dicomObject.SerriesNumber = file.Dataset.GetSingleValueOrDefault<string>(DicomTag.SeriesNumber, "");
            dicomObject.SerriesDescription = file.Dataset.GetSingleValueOrDefault<string>(DicomTag.SeriesDescription, "");
            dicomObject.ClassUID = file.Dataset.GetSingleValueOrDefault<string>(DicomTag.SOPClassUID, "");
            dicomObject.InstanceUID = file.Dataset.GetSingleValueOrDefault<string>(DicomTag.SOPInstanceUID, ""); 
            String tempstring = file.Dataset.GetSingleValueOrDefault<string>(DicomTag.ReferencedRTPlanSequence, "");

            DicomReferencedSOP tt = file.Dataset.GetReferencedSOP(DicomTag.ReferencedRTPlanSequence);

            if(tt != null)
            {
                dicomObject.ReferencedClassUID = tt.Class.UID;
                dicomObject.ReferencedInstanceUID = tt.Instance.UID;
            }
            
            dicomObject.Grid = gridLoader.Load(file);
            dicomObject.Grid.Scaling = file.Dataset.GetSingleValueOrDefault<float>(DicomTag.DoseGridScaling, 1.0f);
            dicomObject.Grid.ValueUnit = unitFromString(file.Dataset.GetSingleValueOrDefault<string>(DicomTag.DoseUnits, "Relative"));
            dicomObject.Grid.Name = Path.GetFileNameWithoutExtension(file.File.Name);
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
