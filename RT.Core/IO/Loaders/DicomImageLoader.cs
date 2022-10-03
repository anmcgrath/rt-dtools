using RT.Core.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicom;

namespace RT.Core.IO.Loaders
{
    public class DicomImageLoader:BaseDicomLoader
    {
        public void Load(DicomFile[] files, DicomImageObject dicomObject, IProgress<double> progress)
        {
            base.Load(files, dicomObject, progress);

            var gridLoader = new GridBasedStructureDicomLoader();
            dicomObject.Grid = gridLoader.Load(files, progress);
            dicomObject.Grid.DefaultPhysicalValue = -1024;
            dicomObject.Grid.Scaling = 1;
            dicomObject.Grid.ValueUnit = Geometry.Unit.HU;
            dicomObject.Grid.Name = dicomObject.Modality + ": " + dicomObject.PatientName;

            try
            {
                dicomObject.LUT.Window = files[0].Dataset.GetSingleValueOrDefault<int>(DicomTag.WindowWidth,2048);
                dicomObject.LUT.Level = files[0].Dataset.GetSingleValueOrDefault<int>(DicomTag.WindowCenter,1024);
#pragma warning disable CS0168 // The variable 'e' is declared but never used
            }catch(Exception e)
#pragma warning restore CS0168 // The variable 'e' is declared but never used
            {
                //Here we should try to set to the median pixel or something
            }
        }
    }
}
