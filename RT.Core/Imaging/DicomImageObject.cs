using Dicom;
using RT.Core.Geometry;
using RT.Core.IO.Loaders;
using RT.Core.DICOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Core.Imaging.LUT;

namespace RT.Core.Imaging
{
    public class DicomImageObject:DicomObject
    {
        public IVoxelDataStructure Grid { get; set; }
        public ILUT LUT { get; set; } = new GrayScaleLUT();

        public DicomImageObject() { }
    }
}
