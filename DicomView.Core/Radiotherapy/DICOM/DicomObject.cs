using Dicom;
using DicomPanel.Core.IO.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomPanel.Core.Radiotherapy.DICOM
{
    /// <summary>
    /// Represents a basic DICOM object
    /// </summary>
    public class DicomObject
    {
        public string[] FileNames { get; set; }
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public string SeriesUid { get; set; }
        public string Modality { get; set; }

        public DicomObject() { }
        public DicomObject(params DicomFile[] files)
        {
            var loader = new BaseDicomLoader();
            loader.Load(files, this);
        }
    }
}
