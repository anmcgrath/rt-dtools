using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTDicomViewer.Message;
using DicomPanel.Core.Radiotherapy.ROIs;
using System.Collections.ObjectModel;
using RTDicomViewer.Utilities;

namespace RTDicomViewer.ViewModel.MainWindow
{
    public class ROIObjectDisplayViewModel : ViewModelBase
    {
        public ObservableCollection<SelectableObject<StructureSet,RegionOfInterest>> StructureSets { get; set; }

        public ROIObjectDisplayViewModel()
        {
            StructureSets = new ObservableCollection<SelectableObject<StructureSet,RegionOfInterest>>();

            MessengerInstance.Register<RTObjectLoadedMessage<StructureSet>>(this,
                x => AddStructureSet(x.Value));
        }

        private void AddStructureSet(StructureSet structureSet)
        {
            var selectableStructureSet = new SelectableObject<StructureSet, RegionOfInterest>(structureSet);
            selectableStructureSet.ChildrenObjectsSelectionChanged += SelectableStructureSet_ChildrenObjectsSelectionChanged;
            foreach(RegionOfInterest roi in structureSet.ROIs)
            {
                selectableStructureSet.AddChild(new SelectableObject<RegionOfInterest>(roi));
            }
            StructureSets.Add(selectableStructureSet);
        }

        private void SelectableStructureSet_ChildrenObjectsSelectionChanged(object sender, MultiSelectableObjectEventArgs<RegionOfInterest> e)
        {
            List<RegionOfInterest> selectedRois = new List<RegionOfInterest>();
            List<RegionOfInterest> unselectedRois = new List<RegionOfInterest>();
            foreach (var selectedRoi in e.SelectedObjects)
                selectedRois.Add(selectedRoi.Value);
            foreach (var unselectedRoi in e.UnselectedObjects)
                unselectedRois.Add(unselectedRoi.Value);

            MessengerInstance.Send(new ROIsObjectRenderMessage(selectedRois,unselectedRois));
        }
    }
}
