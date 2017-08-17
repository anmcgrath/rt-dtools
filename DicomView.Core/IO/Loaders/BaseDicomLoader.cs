using Dicom;
using DicomPanel.Core.Radiotherapy;
using DicomPanel.Core.Radiotherapy.DICOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomPanel.Core.IO.Loaders
{
    public class BaseDicomLoader
    {
        public BaseDicomLoader() { }
        public void Load(DicomFile[] files, DicomObject dicomObject)
        {
            dicomObject.FileNames = new string[files.Length];
            for(int i = 0; i < files.Length; i++)
            {
                dicomObject.FileNames[i] = files[i].File.Name;
            }
            if(files.Length > 0)
            {
                dicomObject.PatientId = files[0].Dataset.Get<string>(DicomTag.PatientID, "");
                dicomObject.PatientName = files[0].Dataset.Get<string>(DicomTag.PatientName, "");
                dicomObject.SeriesUid = files[0].Dataset.Get<string>(DicomTag.SeriesInstanceUID, "");
                dicomObject.Modality = files[0].Dataset.Get<string>(DicomTag.Modality, "");
            }
        }
    }
}
