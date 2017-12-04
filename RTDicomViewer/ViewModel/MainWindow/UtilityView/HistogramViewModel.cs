using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using RT.Core.Utilities.RTMath;
using RTDicomViewer.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.ViewModel.MainWindow.UtilityView
{
    public class HistogramViewModel:ViewModelBase
    {
        public PlotModel OxyPlotModel { get; set; }

        private Dictionary<Histogramf, ColumnSeries> HistogramColumnSeries;
        public HistogramViewModel()
        {
            OxyPlotModel = new PlotModel();
            OxyPlotModel.Background = OxyColors.Black;
            OxyPlotModel.TextColor = OxyColors.White;

            HistogramColumnSeries = new Dictionary<Histogramf, ColumnSeries>();
            MessengerInstance.Register<AddHistogramsMessage>(this, x => AddHistograms(x.Histograms));
        }

        public void AddHistograms(List<Histogramf> histograms)
        {
            OxyPlotModel.Series.Clear();

            foreach (var histogram in histograms)
            {
                OxyPlotModel.Series.Add(createColumnSeries(histogram));
                OxyPlotModel.InvalidatePlot(true);
            }
        }

        private ColumnSeries createColumnSeries(Histogramf histogram)
        {
            ColumnSeries ColumnSeries = new ColumnSeries();
            CategoryAxis axis = new CategoryAxis();
            var labels = histogram.GetBinLabels();
            foreach (var label in labels)
                axis.Labels.Add("" + Math.Round(label, 2));
            OxyPlotModel.Axes.Clear();
            OxyPlotModel.Axes.Add(axis);

            for(int i = 0; i < histogram.Counts.Length; i++)
            {
                ColumnSeries.Items.Add(new ColumnItem() { Value = histogram.Counts[i], CategoryIndex = i });
            }
            return ColumnSeries;
        }
    }
}
