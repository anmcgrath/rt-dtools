using RT.Core.DVH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Message
{
    public class AddDVHMessage
    {
        public List<DoseVolumeHistogram> DVHs { get; set; }
        public AddDVHMessage(List<DoseVolumeHistogram> dvhs)
        {
            DVHs = dvhs;
        }
    }
}
