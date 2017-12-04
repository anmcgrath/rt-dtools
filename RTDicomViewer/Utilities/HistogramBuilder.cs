using GalaSoft.MvvmLight.Messaging;
using RT.Core.Geometry;
using RT.Core.ROIs;
using RT.Core.Utilities.RTMath;
using RTDicomViewer.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RTDicomViewer.Utilities
{
    public class HistogramBuilder
    {
        public bool AutomaticMinMax { get; set; } = true;
        public int BinCount { get; set; } = 15;
        public float Max { get; set; } = 10;
        public float Min { get; set; } = 0;
        public RegionOfInterest ROI { get; set; }
        public bool UseROI { get; set; }

        public async Task<List<Histogramf>> FromGrids(IEnumerable<IVoxelDataStructure> grids)
        {
            Messenger.Default.Send<ProgressMessage>(new ProgressMessage(this, Progress.Begin, 0, true, "Creating Histogram"));
            List<Histogramf> histograms = new List<Histogramf>();
            await Task.Run(()
            =>
            {
                histograms = buildHistograms(grids);
            });
            Messenger.Default.Send<ProgressMessage>(new ProgressMessage(this, Progress.End, "Creating Histogram"));
            return histograms;
        }

        private List<Histogramf> buildHistograms(IEnumerable<IVoxelDataStructure> grids)
        {
            if (AutomaticMinMax)
                SetMinMax(grids);

            List<Histogramf> histograms = new List<Histogramf>();
            foreach (var grid in grids)
                histograms.Add(buildHistogram(grid));
            
            return histograms;
        }

        private Histogramf buildHistogram(IVoxelDataStructure grid)
        {
            Histogramf histogram = new Histogramf(Min, Max, BinCount);

            foreach(Voxel voxel in grid)
            {
                if (!(grid.ValueUnit == Unit.Gamma && voxel.Value == -1))
                {
                    if (UseROI && ROI.ContainsPointNonInterpolated(voxel.Position))
                        histogram.AddDataPoint(voxel.Value * grid.Scaling);
                    else if (!UseROI)
                        histogram.AddDataPoint(voxel.Value * grid.Scaling);
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
