using Dicom;
using RT.Core.IO.Loaders;
using RT.Core.DICOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace RT.Core.Planning
{
    public class DicomPlanObject:DicomObject
    {
        public bool IsBrachy { get; set; }

        public string Name { get; set; }
        public string ClassUID { get; set; }
        public string InstanceUID { get; set; }
        public string ReferencedClassUID { get; set; }
        public string ReferencedInstanceUID { get; set; }
        public string Label { get; set; }

        public double RxDose { get; set; }
        public double TotalMU { get { double total = 0; foreach (Beam beam in Beams.Values) { total += beam.MU; } return total; } }

        public double TotalRefAirKerma { get; set; }
        public double TotalTime { get { double total = 0; foreach (BrachyChannel chanel in Channels.Values) { total += chanel.ChannelTotalTime; } return total; } }

        public Hashtable Beams { get; set; }
        public Hashtable Sources { get; set; }
        public Hashtable Channels { get; set; }

        public DicomPlanObject()
        {
            Beams = new Hashtable();
            Sources = new Hashtable();
            Channels = new Hashtable();
        }

        public void Clear()
        {
            Beams.Clear();
            Sources.Clear();
            Channels.Clear();
        }
    }
}
