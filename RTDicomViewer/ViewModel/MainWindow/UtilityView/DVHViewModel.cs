using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Series;
using RT.Core.DVH;
using RTDicomViewer.Message;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.ViewModel.MainWindow.UtilityView
{
    public class DVHViewModel:ViewModelBase
    {
        public PlotModel OxyPlotModel { get; set; }

        private ObservableCollection<DoseVolumeHistogram> DVHs { get; set; }

        public DVHViewModel()
        {
            OxyPlotModel = new PlotModel();
            OxyPlotModel.Background = OxyColors.Black;
            OxyPlotModel.TextColor = OxyColors.White;

            MessengerInstance.Register<AddDVHMessage>(this, x => AddDVHs(x.DVHs));

            DVHs = new ObservableCollection<DoseVolumeHistogram>();
        }

        public void AddDVHs(List<DoseVolumeHistogram> dvhs)
        {
            OxyPlotModel.Series.Clear();
            foreach(var dvh in dvhs)
            {
                OxyPlotModel.Series.Add(createLineSeries(dvh));
            }
            OxyPlotModel.InvalidatePlot(true);
        }

        private LineSeries createLineSeries(DoseVolumeHistogram dvh)
        {
            LineSeries series = new LineSeries();

            /*series.Color = OxyPlot.OxyColor.FromArgb(
                (byte)dvh.ROIObject.Color.A, 
                (byte)dvh.ROIObject.Color.R, 
                (byte)dvh.ROIObject.Color.G, 
                (byte)dvh.ROIObject.Color.B);*/
            for(int i = 0; i < dvh.Dose.Length; i++)
            {
                series.Points.Add(new DataPoint(dvh.Dose[i], dvh.CumulativeVolume[i]));
            }
            return series;
        }
    }
}
