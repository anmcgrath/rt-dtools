using DicomPanel.Core.Radiotherapy.Dose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Message
{
    public class DoseObjectRenderMessage
    {
        public IDoseObject DoseObject { get; set; }
        public DoseObjectRenderMessage(IDoseObject doseObject)
        {
            DoseObject = doseObject;
        }
    }
}
