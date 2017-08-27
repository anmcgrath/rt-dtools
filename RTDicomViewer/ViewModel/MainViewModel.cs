using GalaSoft.MvvmLight;
using DicomPanel.Core.Geometry;
using DicomPanel.Core.IO;
using DicomPanel.Core.Radiotherapy.Dose;
using DicomPanel.Core.Radiotherapy.Imaging;
using DicomPanel.Core.Radiotherapy.Planning;
using DicomPanel.Core.Radiotherapy.ROIs;
using System.IO;
using System.Windows;
using DicomPanel.Core;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using RTDicomViewer.Message;
using System;
using System.Threading.Tasks;
using System.Threading;
using RTDicomViewer.IO;

namespace RTDicomViewer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private DicomPanelModel _axialPanelModel;
        public DicomPanelModel AxialPanelModel { get; set; }
        public DicomPanelModel CoronalPanelModel { get; set; }
        public DicomPanelModel SagittalPanelModel { get; set; }

        public RelayCommand OpenImageCommand => new RelayCommand(
            ()=> { FileOpener.BeginOpenAsync<DicomImageObject>(true, "Open DICOM Images");});

        public RelayCommand OpenDicomDoseCommand => new RelayCommand(
            () => FileOpener.BeginOpenAsync<DicomDoseObject>(false, "Open DICOM Dose"));

        public RelayCommand OpenEgsDoseCommand => new RelayCommand(
            () => FileOpener.BeginOpenAsync<DicomDoseObject>(false, "Open 3DDose Dose"));

        public RelayCommand OpenStructureSetCommand => new RelayCommand(
            () => FileOpener.BeginOpenAsync<StructureSet>(false, "Open Structure Set"));

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            AxialPanelModel = new DicomPanelModel();
            AxialPanelModel.Camera.SetAxial();

            SagittalPanelModel = new DicomPanelModel();
            SagittalPanelModel.Camera.SetSagittal();

            CoronalPanelModel = new DicomPanelModel();
            CoronalPanelModel.Camera.SetCoronal();

            //When we load a new image, render it
            MessengerInstance.Register<RTObjectLoadedMessage<DicomImageObject>>(this, x => {
                AxialPanelModel.SetImage(x.Value);
                SagittalPanelModel.SetImage(x.Value);
                CoronalPanelModel.SetImage(x.Value);
            });
            //When a dose is selected, render it
            MessengerInstance.Register<DoseObjectRenderMessage>(this, x =>
            {
                AxialPanelModel.SetDose(x.DoseObject);
                SagittalPanelModel.SetDose(x.DoseObject);
                CoronalPanelModel.SetDose(x.DoseObject);
            });
            MessengerInstance.Register<ROIsObjectRenderMessage>(this, x =>
            {
                AxialPanelModel.AddROIs(x.AddedRois);
                AxialPanelModel.RemoveROIs(x.RemovedRois);
                SagittalPanelModel.AddROIs(x.AddedRois);
                SagittalPanelModel.RemoveROIs(x.RemovedRois);
                CoronalPanelModel.AddROIs(x.AddedRois);
                CoronalPanelModel.RemoveROIs(x.RemovedRois);
            });
        }
    }

}