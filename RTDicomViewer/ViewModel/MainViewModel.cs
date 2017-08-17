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

namespace RTDicomViewer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private DicomPanelModel _mainPanelModel;
        public DicomPanelModel MainPanelModel
        {
            get { return _mainPanelModel; }
            set
            {
                _mainPanelModel = value;
                RaisePropertyChanged(() => MainPanelModel);
            }
        }

        public RelayCommand OpenImageCommand => new RelayCommand(()=> { OpenFile<DicomImageObject>(true, "Open DICOM Images");});
        public RelayCommand OpenDicomDoseCommand => new RelayCommand(() => OpenFile<DicomDoseObject>(false, "Open DICOM Dose"));
        public RelayCommand OpenEgsDoseCommand => new RelayCommand(() => OpenFile<DicomDoseObject>(false, "Open 3DDose Dose"));

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            MainPanelModel = new DicomPanelModel();
            MainPanelModel.Camera.SetSagittal();

            //When we load a new image, render it
            MessengerInstance.Register<RTObjectLoadedMessage<DicomImageObject>>(this, x => {
                MainPanelModel.SetImage(x.Value);
            });
            //When a dose is selected, render it
            MessengerInstance.Register<DoseObjectRenderMessage>(this, x =>
            {
                MainPanelModel.SetDose(x.DoseObject);
            });
        }

        public async void OpenFile<T>(bool multipleFiles, string dialogTitle)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = multipleFiles;
            dialog.Title = dialogTitle;
            if(dialog.ShowDialog() == true)
            {
                try
                {
                    var obj = await DicomLoader.LoadAsync<T>(dialog.FileNames);
                    MessengerInstance.Send(new RTObjectLoadedMessage<T>(obj));
                }catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
    }
}