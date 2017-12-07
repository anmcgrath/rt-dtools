using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using RT.Core.Geometry;
using RT.Core.ROIs;
using RT.Core.Utilities.RTMath;
using RTDicomViewer.Message;
using RTDicomViewer.ViewModel.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RTDicomViewer.Utilities
{
    public class HistogramBuilder:ViewModelBase,IHistogramBuilder
    {
        public bool AutomaticMinMax { get; set; } = true;
        public int BinCount { get; set; } = 15;
        public float Max { get { return _max; } set { _max = value; RaisePropertyChanged("Max"); } }
        private float _max = 10;
        public float Min { get { return _min; } set { _min = value; RaisePropertyChanged("Min"); } }
        private float _min = 0;
        public RegionOfInterest ROI { get; set; }
        public bool UseROI { get; set; }
        private IProgressService progressService;

        public HistogramBuilder(IProgressService progress)
        {
            progressService = progress;
        }

        public async Task<List<Histogramf>> FromGrids(IEnumerable<IVoxelDataStructure> grids)
        {
            //Messenger.Default.Send<ProgressMessage>(new ProgressMessage(this, Progress.Begin, 0, false, "Creating Histogram"));
            var progressItem = progressService.CreateNew("Creating Histogram(s)...", false);

            var progress = new Progress<int>(x => { progressItem.ProgressAmount = (x); });
            List<Histogramf> histograms = new List<Histogramf>();
            await Task.Run(()
            =>
            {
                histograms = buildHistograms(grids, progress);
            });
            progressService.End(progressItem);

            return histograms;
        }

        private List<Histogramf> buildHistograms(IEnumerable<IVoxelDataStructure> grids, IProgress<int> progress)
        {
            if (AutomaticMinMax)
                SetMinMax(grids);

            List<Histogramf> histograms = new List<Histogramf>();
            //Grid number solely for reporting progress

            foreach (var grid in grids)
            {
                histograms.Add(buildHistogram(grid, progress));
            }
            
            return histograms;
        }

        private Histogramf buildHistogram(IVoxelDataStructure grid, IProgress<int> progress)
        {
            Histogramf histogram = new Histogramf(Min, Max, BinCount);

            int numberOfVoxels = grid.NumberOfVoxels;
            //Only report every 5%
            int updateNumber = numberOfVoxels / 20;
            int voxelNum = 0;
            foreach(Voxel voxel in grid)
            {
                voxelNum++;
                if (!(grid.ValueUnit == Unit.Gamma && voxel.Value == -1))
                {
                    if (UseROI && ROI.ContainsPointNonInterpolated(voxel.Position))
                        histogram.AddDataPoint(voxel.Value * grid.Scaling);
                    else if (!UseROI)
                        histogram.AddDataPoint(voxel.Value * grid.Scaling);
                }
                if (voxelNum % updateNumber == 0)
                {
                    progress.Report((int)(100 * ((double)voxelNum / (double)numberOfVoxels)));
                }
            }
            return histogram;
        }

        private void SetMinMax(IEnumerable<IVoxelDataStructure> grids)
        {
            if (AutomaticMinMax)
            {
                Max = float.MinValue;
                Min = float.MaxValue;
            }
            
            foreach(var grid in grids)
            {
                foreach(Voxel voxel in grid)
                {
                    if (!(grid.ValueUnit == Unit.Gamma && voxel.Value == -1))
                    {
                        if (UseROI && ROI.ContainsPointNonInterpolated(voxel.Position.X, voxel.Position.Y, voxel.Position.Z))
                        {
                            CompareAndSetMax(voxel.Value * grid.Scaling);
                            CompareAndSetMin(voxel.Value * grid.Scaling);
                        }
                        else if (!UseROI)
                        {
                            CompareAndSetMax(voxel.Value * grid.Scaling);
                            CompareAndSetMin(voxel.Value * grid.Scaling);
                        }
                    }
                }
            }
        }

        private void CompareAndSetMax(float value)
        {
            if (value > Max)
                Max = value;
        }
        private void CompareAndSetMin(float value)
        {
            if (value < Min)
                Min = value;
        }
    }
}
