using Dicom;
using DicomPanel.Core.Geometry;
using DicomPanel.Core.IO.Loaders;
using DicomPanel.Core.Radiotherapy.DICOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomPanel.Core.Radiotherapy.Imaging
{
    public class DicomImageObject:DicomObject
    {
        public IVoxelDataStructure Grid { get; set; }
        public int Window { get; set; } = 400;
        public int Level { get; set; } = 0;

        public DicomImageObject() { }
        public DicomImageObject(params DicomFile[] files):base(files)
        {
            var loader = new DicomImageLoader();
            loader.Load(files,this);
        }
    }
}
