using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RT.Core.Dose;
using RT.Core.Geometry;
using RT.Core.Imaging;
using RT.Core.ROIs;
using RTDicomViewer.Message;
using RTDicomViewer.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.ViewModel.Dialogs
{
    public class HistogramCreaterWindowViewModel:ViewModelBase
    {
        public ObservableCollection<SelectableObject<IVoxelDataStructure>> Data { get; set; }
        public ObservableCollection<RegionOfInterest> ROIs { get; set; }

        public HistogramBuilder HistogramBuilder { get; set; }

        public RegionOfInterest SelectedROI { get { return _selectedROI; } set { _selectedROI = value; RaisePropertyChanged("SelectedROI"); } }
        private RegionOfInterest _selectedROI { get; set; }

        public List<IVoxelDataStructure> SelectedData { get; set; }

        public bool LimitHistograms { get { return _limitHistograms; } set { _limitHistograms = value; RaisePropertyChanged("LimitHistograms"); } }
        private bool _limitHistograms;

        public RelayCommand ApplyCommand => new RelayCommand(() =>
        {
            BuildHistograms();
        });


        public HistogramCreaterWindowViewModel()
        {
            Data = new ObservableCollection<SelectableObject<IVoxelDataStructure>>();
            ROIs = new ObservableCollection<RegionOfInterest>();

            SelectedData = new List<IVoxelDataStructure>();

            foreach (var item in Workspace.Workspace.Current.Doses.GetList())
                AddData(item.Grid);
            foreach (var item in Workspace.Workspace.Current.Images.GetList())
                AddData(item.Grid);
            foreach (var structureSet in Workspace.Workspace.Current.StructureSets.GetList())
                foreach (var roi in structureSet.ROIs)
                    ROIs.Add(roi);

            SelectedROI = ROIs.FirstOrDefault();

            MessengerInstance.Register<RTObjectAddedMessage<EgsDoseObject>>(this, x => AddData(x.Value.Grid));
            MessengerInstance.Register<RTObjectAddedMessage<DicomDoseObject>>(this, x => AddData(x.Value.Grid));
            MessengerInstance.Register<RTObjectAddedMessage<DicomImageObject>>(this, x => AddData(x.Value.Grid));
            MessengerInstance.Register<RTObjectAddedMessage<StructureSet>>(this, x => { foreach (var roi in x.Value.ROIs) ROIs.Add(roi); SelectedROI = x.Value.ROIs.FirstOrDefault(); });
        }

        public async void BuildHistograms()
        {
            var builder = new HistogramBuilder();
            builder.UseROI = this.LimitHistograms && SelectedROI != null;
            builder.ROI = this.SelectedROI;
            var histograms = await builder.FromGrids(SelectedData);
            MessengerInstance.Send(new AddHistogramsMessage(histograms));
        }

        private void AddData(IVoxelDataStructure data)
        {
            var selectableData = new SelectableObject<IVoxelDataStructure>(data);
            Data.Add(selectableData);
            selectableData.ObjectSelectionChanged += SelectableData_ObjectSelectionChanged;
        }

        private void SelectableData_ObjectSelectionChanged(object sender, SelectableObjectEventArgs e)
        {
            var selectedObject = (SelectableObject<IVoxelDataStructure>)sender;
            if (e.IsNowSelected)
                SelectedData.Add(selectedObject.Value);
            else
                SelectedData.Remove(selectedObject.Value);
        }
    }
}
