using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.Patient
{
    /// <summary>
    /// DICOM 病人基本信息类
    /// </summary>
    public class Patient
    {
        public string PatientName { get; set; }
        public string PatientSex { get; set; }
        public string PatientBirthdayDate { get; set; }
        public string PatientID { get; set; }
        public string PatientBirthdayTime { get; set; }
        public int StudyCount { get; set; }
        public int BeamCount { get; set; }
        public int SurfaceCount { get; set; }
    }
}
