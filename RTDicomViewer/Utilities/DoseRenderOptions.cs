using DicomPanel.Core.Render.Contouring;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Utilities
{
    public class DoseRenderOptions
    {
        public ObservableCollection<ContourInfo> ContourInfo { get; set; }
        public double NormalisationIsodose { get; set; }
        public int RenderQuality { get; set; }
    }
}
