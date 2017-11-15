using RT.Core.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicom;

namespace RT.Core.IO.Loaders
{
    public class DicomImageLoader
    {
        public void Load(DicomFile[] files, DicomImageObject dicomObject)
        {
            var gridLoader = new GridBasedStructureDicomLoader();
            dicomObject.Grid = gridLoader.Load(files);
            dicomObject.Grid.DefaultPhysicalValue = -1024;

            try
            {
                dicomObject.Window = files[0].Dataset.Get<int>(DicomTag.WindowWidth);
                dicomObject.Level = files[0].Dataset.Get<int>(DicomTag.WindowCenter);
            }catch(Exception e)
            {
                //Here we should try to set to the median pixel or something
                dicomObject.Window = 400;
                dicomObject.Level = 1;
            }
        }
    }
}
