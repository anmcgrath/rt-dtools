using Dicom;
using RT.Core.IO.Loaders;
using RT.Core.DICOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace RT.Core.ROIs
{
    public class StructureSet:DicomObject
    {
        public Hashtable ROIs { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public StructureSet() 
        {
            ROIs = new Hashtable();
        }
    }
}
