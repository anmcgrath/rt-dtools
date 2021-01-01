using Dicom;
using RT.Core;
using RT.Core.DICOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.IO.Loaders
{
    public class BaseDicomLoader
    {
        public BaseDicomLoader() { }
        protected void Load(DicomFile[] files, DicomObject dicomObject, IProgress<double> progress)
        {
            try
            {
                if (files.Length > 0)
                {
                    dicomObject.FileNames = new string[files.Length];
                    for (int i = 0; i < files.Length; i++)
                    {
                        dicomObject.FileNames[i] = files[i].File.Name;
                    }
                    dicomObject.PatientId = files[0].Dataset.GetSingleValueOrDefault(DicomTag.PatientID, "");
                    dicomObject.PatientName = files[0].Dataset.GetSingleValueOrDefault(DicomTag.PatientName, "");
                    dicomObject.SeriesUid = files[0].Dataset.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, "");
                    dicomObject.Modality = files[0].Dataset.GetSingleValueOrDefault(DicomTag.Modality, "");
                    dicomObject.SeriesDescription = files[0].Dataset.GetSingleValueOrDefault(DicomTag.SeriesDescription, "");
                    dicomObject.StudyDescription = files[0].Dataset.GetSingleValueOrDefault(DicomTag.StudyDescription, "");
                }
            }
            catch (Exception e)
            {
                //Messenger.Default.Send(new NotificationMessage(e.Message));
            }
        }
    }
}
