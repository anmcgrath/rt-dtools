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
using GalaSoft.MvvmLight;

namespace RTDicomViewer.ViewModel.MainWindow
{
    public class POIObjectDisplayViewModel:ViewModelBase
    {
        public ObservableCollection<SelectableObject<PointOfInterest>> PointsOfInterest { get; set; }
        public SelectableObject<PointOfInterest> SelectedPOI { get; set; }
        public bool ToolIsActive { get { return _toolIsActive; } set { _toolIsActive = value; RaisePropertyChanged("ToolIsActive");  } }
        private bool _toolIsActive = false;

        //Store the tool that we can go back to it.
        private ITool OldTool;

        public RelayCommand MovePOICommand => new RelayCommand(() =>
        {
            if(SelectedPOI != null)
            {
                if (!ToolIsActive)
                {
                    OldTool = Workspace.Workspace.Current.Axial.ToolBox.SelectedTool;
                    Workspace.Workspace.Current.Axial.ToolBox.SelectTool("movepoi");
                    ((MovePOITool)(Workspace.Workspace.Current.Axial.ToolBox.GetTool("movepoi"))).Target = SelectedPOI.Value;
                    ToolIsActive = true;
                    Workspace.Workspace.Current.Axial.ToolBox.ToolSelected += ToolBox_ToolSelected;
                }else if(OldTool != null)
                {
                    Workspace.Workspace.Current.Axial.ToolBox.ToolSelected -= ToolBox_ToolSelected;
                    Workspace.Workspace.Current.Axial.ToolBox.SelectTool(OldTool);
                    ToolIsActive = false;
                }
            }
        });

        private void ToolBox_ToolSelected(object sender, ToolSelectedEventArgs args)
        {
            ToolIsActive = false;
            Workspace.Workspace.Current.Axial.ToolBox.ToolSelected -= ToolBox_ToolSelected;
        }

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
