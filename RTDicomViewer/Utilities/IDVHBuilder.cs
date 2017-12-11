using RT.Core.Dose;
using RT.Core.DVH;
using RT.Core.ROIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Utilities
{
    public interface IDVHBuilder
    {
        Task<DoseVolumeHistogram> Build(IDoseObject dose, RegionOfInterest roi);
    }
}
