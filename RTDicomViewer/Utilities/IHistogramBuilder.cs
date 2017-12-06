using RT.Core.Geometry;
using RT.Core.ROIs;
using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Utilities
{
    public interface IHistogramBuilder
    {
        Task<List<Histogramf>> FromGrids(IEnumerable<IVoxelDataStructure> grids);
        bool UseROI { get; set; }
        RegionOfInterest ROI { get; set; }
    }
}
