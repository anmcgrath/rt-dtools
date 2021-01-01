using Dicom;
using RT.Core.Geometry;
using RT.Core.IO.Loaders;
using RT.Core.DICOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.Dose
{
    public class DicomDoseObject:DicomObject, IDoseObject
    {
        public string SerriesNumber { get; set; }
        public string SerriesDescription { get; set; }

        public string ClassUID { get; set; }
        public string InstanceUID { get; set; }
        public string ReferencedClassUID { get; set; }
        public string ReferencedInstanceUID { get; set; }

        public IVoxelDataStructure Grid { get; set; }
        public DicomDoseObject() { }
    }
}
