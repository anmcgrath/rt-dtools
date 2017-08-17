using Dicom;
using RTData.Geometry;
using RTData.IO.Loaders;
using RTData.Radiotherapy.DICOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTData.Radiotherapy.Imaging
{
    public class DicomImageObject:DicomObject
    {
        public IVoxelDataStructure Grid { get; set; }
        public int Window { get; set; }
        public int Level { get; set; }

        public DicomImageObject() { }
        public DicomImageObject(params DicomFile[] files):base(files)
        {
            var loader = new DicomImageLoader();
            loader.Load(files,this);
        }
    }
}
