using RTDicomViewer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Message
{
    public class DoseRenderQualityChanged
    {
        public DoseRenderOptions Options { get; set; }
        public DoseRenderQualityChanged(DoseRenderOptions options)
        {
            Options = options;
        }
    }
}
