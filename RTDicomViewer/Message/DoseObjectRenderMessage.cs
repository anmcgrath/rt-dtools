using RT.Core.Dose;
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
        /// <summary>
        /// Whether or not the models should remove the dose object or add it
        /// </summary>
        public bool RemoveDose { get; set; }
        public DoseObjectRenderMessage(IDoseObject doseObject, bool remove)
        {
            DoseObject = doseObject;
            RemoveDose = remove;
        }
    }
}
