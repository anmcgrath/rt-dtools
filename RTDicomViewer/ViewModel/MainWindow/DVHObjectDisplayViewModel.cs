using DicomPanel.Core.Radiotherapy.Dose;
using DicomPanel.Core.Radiotherapy.DVH;
using DicomPanel.Core.Radiotherapy.ROIs;
using GalaSoft.MvvmLight;
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
    public class DVHObjectDisplayViewModel:ViewModelBase
    {
        public ObservableCollection<SelectableObject<RegionOfInterest, DoseVolumeHistogram>> RegionOfInterests { get; set; }
        public List<IDoseObject> Doses { get; set; }

        public DVHObjectDisplayViewModel()
        {
            RegionOfInterests = new ObservableCollection<SelectableObject<RegionOfInterest, DoseVolumeHistogram>>();
            Doses = new List<IDoseObject>();

            MessengerInstance.Register<RTObjectLoadedMessage<StructureSet>>(this,
                x => AddStructureSet(x.Value));

            MessengerInstance.Register<RTObjectLoadedMessage<DicomDoseObject>>(this,
                x => AddDoseObject(x.Value));

            MessengerInstance.Register<RTObjectLoadedMessage<EgsDoseObject>>(this,
                x => AddDoseObject(x.Value));
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
                selectableRegionOfInterest.Children.Add(new SelectableObject<DoseVolumeHistogram>(dvh));
            }
        }

        private void SelectableStructureSet_ChildrenObjectsSelectionChanged(object sender, MultiSelectableObjectEventArgs<DoseVolumeHistogram> e)
        {
            var selectedDVhs = e.SelectedObjects;
            var unselectedDVHs = e.UnselectedObjects;
        }
    }
}
