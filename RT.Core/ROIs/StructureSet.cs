using Dicom;
using RT.Core.IO.Loaders;
using RT.Core.DICOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.ROIs
{
    public class StructureSet:DicomObject
    {
        public string Name { get; set; }
        public List<RegionOfInterest> ROIs { get; set; }

        public StructureSet(params DicomFile[] files):base(files)
        {
            var loader = new ROILoader();
            loader.Load(files, this);
        }
    }
}
