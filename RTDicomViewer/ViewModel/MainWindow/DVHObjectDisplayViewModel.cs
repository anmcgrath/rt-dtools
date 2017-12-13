using RT.Core.Dose;
using RT.Core.DVH;
using RT.Core.ROIs;
using GalaSoft.MvvmLight;
using RTDicomViewer.Message;
using RTDicomViewer.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RTDicomViewer.ViewModel.MainWindow
{
    public class DVHObjectDisplayViewModel:ViewModelBase
    {
        public ObservableCollection<SelectableObject<RegionOfInterest, DoseVolumeHistogram>> RegionOfInterests { get; set; }
        public List<IDoseObject> Doses { get; set; }
        private DVHBuilder DVHBuilder;

        public DVHObjectDisplayViewModel(IDVHBuilder dvhBuilder)
        {
            DVHBuilder = DVHBuilder;

            RegionOfInterests = new ObservableCollection<SelectableObject<RegionOfInterest, DoseVolumeHistogram>>();
            Doses = new List<IDoseObject>();

            MessengerInstance.Register<RTObjectAddedMessage<StructureSet>>(this,
                x => AddStructureSet(x.Value));

            MessengerInstance.Register<RTObjectAddedMessage<DicomDoseObject>>(this,
                x => AddDoseObject(x.Value));

            MessengerInstance.Register<RTObjectAddedMessage<EgsDoseObject>>(this,
                x => AddDoseObject(x.Value));

            foreach(var ss in Workspace.Workspace.Current.StructureSets.GetList())
            {
                AddStructureSet(ss);
            }
            foreach(var dose in Workspace.Workspace.Current.Doses.GetList())
            {
                AddDoseObject(dose);
            }
        }

        private void RemoveStructureSet(StructureSet structureSet)
        {
        }

        private void AddStructureSet(StructureSet structureSet)
        {
            foreach (RegionOfInterest roi in structureSet.ROIs)
            {
                var selectableRegionOfInterest = new SelectableObject<RegionOfInterest, DoseVolumeHistogram>(roi);
                selectableRegionOfInterest.ChildrenObjectsSelectionChanged += SelectableStructureSet_ChildrenObjectsSelectionChanged;

                foreach (IDoseObject doseObject in Doses)
                {
                    var dvh = new DoseVolumeHistogram(doseObject, roi);
                    selectableRegionOfInterest.AddChild(new SelectableObject<DoseVolumeHistogram>(dvh));
                }

                RegionOfInterests.Add(selectableRegionOfInterest);
            }
            
        }

        private void AddDoseObject(IDoseObject doseObject)
        {
            Doses.Add(doseObject);
            foreach(SelectableObject<RegionOfInterest, DoseVolumeHistogram> selectableRegionOfInterest in RegionOfInterests)
            {
                var dvh = new DoseVolumeHistogram(doseObject, selectableRegionOfInterest.Value);
                selectableRegionOfInterest.AddChild(new SelectableObject<DoseVolumeHistogram>(dvh));
            }
        }

        private void SelectableStructureSet_ChildrenObjectsSelectionChanged(object sender, MultiSelectableObjectEventArgs<DoseVolumeHistogram> e)
        {
            var selectedDVhs = e.SelectedObjects;
            var unselectedDVHs = e.UnselectedObjects;

            var firstDVH = selectedDVhs.FirstOrDefault()?.Value;
            var dvhsToAdd = new List<DoseVolumeHistogram>();
            foreach(var roi in RegionOfInterests)
            {
                foreach(var dvh in roi.Children)
                {
                    if (dvh.IsSelected)
                    {
                        dvh.Value.Compute();
                        Clipboard.SetText(dvh.Value.ToString());
                        dvhsToAdd.Add(dvh.Value);
                    }
                }
            }
            MessengerInstance.Send<AddDVHMessage>(new AddDVHMessage(dvhsToAdd));
        }
    }
}
