using RT.Core.Planning;
using GalaSoft.MvvmLight.Messaging;
using RTDicomViewer.Message;
using RTDicomViewer.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.ViewModel.MainWindow
{
    public class POIObjectDisplayViewModel
    {
        public ObservableCollection<SelectableObject<PointOfInterest>> PointsOfInterest { get; set; }
        public POIObjectDisplayViewModel()
        {
            PointsOfInterest = new ObservableCollection<SelectableObject<PointOfInterest>>();
            Messenger.Default.Register<RTObjectLoadedMessage<PointOfInterest>>(this, x => AddPoi(x.Value));
        }

        public void AddPoi(PointOfInterest poi)
        {
            var so = new SelectableObject<PointOfInterest>(poi);
            so.ObjectSelectionChanged += So_ObjectSelectionChanged;
            PointsOfInterest.Add(so);
            so.IsSelected = true;
        }

        private void So_ObjectSelectionChanged(object sender, SelectableObjectEventArgs e)
        {
            
        }
    }
}
