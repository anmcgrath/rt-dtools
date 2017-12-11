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
using GalaSoft.MvvmLight.Command;
using DicomPanel.Core.Toolbox;

namespace RTDicomViewer.ViewModel.MainWindow
{
    public class POIObjectDisplayViewModel
    {
        public ObservableCollection<SelectableObject<PointOfInterest>> PointsOfInterest { get; set; }
        public SelectableObject<PointOfInterest> SelectedPOI { get; set; }

        public RelayCommand MovePOICommand => new RelayCommand(() =>
        {
            if(SelectedPOI != null)
            {
                Workspace.Workspace.Current.Axial.ToolBox.SelectTool("movepoi");
                ((MovePOITool)(Workspace.Workspace.Current.Axial.ToolBox.GetTool("movepoi"))).Target = SelectedPOI.Value;
            }
        });

        public POIObjectDisplayViewModel()
        {
            PointsOfInterest = new ObservableCollection<SelectableObject<PointOfInterest>>();
            Messenger.Default.Register<RTObjectAddedMessage<PointOfInterest>>(this, x => AddPoi(x.Value));
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
