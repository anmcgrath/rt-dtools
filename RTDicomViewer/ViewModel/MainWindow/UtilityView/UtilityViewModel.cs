using GalaSoft.MvvmLight;
using RTDicomViewer.Message;
using RTDicomViewer.View.MainWindow.UtilityView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RTDicomViewer.ViewModel.MainWindow.UtilityView
{
    public class UtilityViewModel:ViewModelBase
    {
        private DVHView dvhView = new DVHView();
        private HistogramView histogramView = new HistogramView();
        public UserControl ActiveView { get { return _activeView; } set { Set("ActiveView", ref _activeView, value); } }
        private UserControl _activeView;
        public UtilityViewModel()
        {
            MessengerInstance.Register<AddDVHMessage>(this, x => ActiveView = dvhView);
            MessengerInstance.Register<AddHistogramsMessage>(this, x => ActiveView = histogramView);
        }
    }
}
