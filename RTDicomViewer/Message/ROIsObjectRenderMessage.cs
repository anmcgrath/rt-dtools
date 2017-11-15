using RT.Core.ROIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Message
{
    public class ROIsObjectRenderMessage
    {
        public List<RegionOfInterest> AddedRois { get; set; }
        public List<RegionOfInterest> RemovedRois { get; set; }
        public ROIsObjectRenderMessage(List<RegionOfInterest> addedRois, List<RegionOfInterest> removedRois)
        {
            AddedRois = addedRois;
            RemovedRois = removedRois;
        }
    }
}
