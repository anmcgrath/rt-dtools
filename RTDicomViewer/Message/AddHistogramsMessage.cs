using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Message
{
    public class AddHistogramsMessage
    {
        public List<Histogramf> Histograms;
        public AddHistogramsMessage(List<Histogramf> histograms)
        {
            Histograms = histograms;
        }
    }
}
