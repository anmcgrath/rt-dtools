using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Core.Dose;
using RT.Core.ROIs;
using RTDicomViewer.ViewModel.Dialogs;
using RT.Core.DVH;

namespace RTDicomViewer.Utilities
{
    public class DVHBuilder : IDVHBuilder
    {
        private IProgressService ProgressService;
        public DVHBuilder(IProgressService progressService)
        {
            ProgressService = progressService;
        }
        public async Task<DoseVolumeHistogram> Build(IDoseObject dose, RegionOfInterest roi)
        {
            var pi = ProgressService.CreateNew("Building DVH... ", false);
            DoseVolumeHistogram dvh = null;
            await Task.Run(() =>
                {
                    dvh = new DoseVolumeHistogram(dose, roi);
                    dvh.Compute();
                }
            );
            ProgressService.End(pi);
            return dvh;
        }
    }
}
