using DicomPanel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOfPanel
{
    public class MainViewModel
    {
        public DicomPanelModel DicomModel { get; set; }

        public MainViewModel()
        {
            DicomModel = new DicomPanelModel();
        }
    }
}
