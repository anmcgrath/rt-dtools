using GalaSoft.MvvmLight;
using RT.Core.Geometry;
using RT.Core.IO;
using RT.Core.Dose;
using RT.Core.Imaging;
using RT.Core.Planning;
using RT.Core.ROIs;
using System.IO;
using System.Windows;
using RT.Core;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using RTDicomViewer.Message;
using System;
using System.Threading.Tasks;
using System.Threading;
using RTDicomViewer.IO;
using DicomPanel.Core.Toolbox;
using RTDicomViewer.ViewModel.Dialogs;
using RTDicomViewer.View.Dialogs;
using RTDicomViewer.View.MainWindow;
using DicomPanel.Core;

namespace RTDicomViewer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public double NormalisationIsodose { get; set; } = 100.0;
        public int RenderQuality { get; set; } = 50;
        private DicomPanelModel _axialPanelModel;
        public DicomPanelModel AxialPanelModel { get; set; }
        public DicomPanelModel CoronalPanelModel { get; set; }
        public DicomPanelModel SagittalPanelModel { get; set; }

        public ToolBox ToolBox = new ToolBox();

        public RelayCommand OpenImageCommand => new RelayCommand(
            () => { var fo = new FileOpener(); fo.BeginOpenDicomAsync<DicomImageObject>(true, "Open DICOM Images"); });

        public RelayCommand OpenDicomDoseCommand => new RelayCommand(
            () => { var fo = new FileOpener(); fo.BeginOpenDicomAsync<DicomDoseObject>(false, "Open DICOM Dose"); });

        public RelayCommand OpenEgsDoseCommand => new RelayCommand(
            () => { var fo = new FileOpener(); fo.BeginOpenEgsAsync(); });

        public RelayCommand OpenStructureSetCommand => new RelayCommand(
            () => { var fo = new FileOpener(); fo.BeginOpenDicomAsync<StructureSet>(false, "Open Structure Set"); });

        public RelayCommand OpenDoseStatisticsCommand => new RelayCommand(
            () => { OpenDoseStatistics(); });

        public RelayCommand CreateCubePhantomCommand => new RelayCommand(
            () => { CreateCubePhantom(); });

        public RelayCommand OpenDicomPlanCommand => new RelayCommand(
            () => { var fo = new FileOpener(); fo.BeginOpenDicomAsync<DicomPlanObject>(false, "Open RT Plan"); });

        public RelayCommand CreateNewPOICommand => new RelayCommand(
            () => { CreateNewPOI(); });

        public RelayCommand ApplyNormalisationCommand => new RelayCommand(
            () => { ApplyNormalisation(); });

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Workspace.Workspace.Init();

            AxialPanelModel = new DicomPanelModel();
            AxialPanelModel.Camera.SetAxial();
            AxialPanelModel.SetToolBox(ToolBox);

            SagittalPanelModel = new DicomPanelModel();
            SagittalPanelModel.Camera.SetSagittal();
            SagittalPanelModel.SetToolBox(ToolBox);

            CoronalPanelModel = new DicomPanelModel();
            CoronalPanelModel.Camera.SetCoronal();
            CoronalPanelModel.SetToolBox(ToolBox);

            AxialPanelModel.OrthogonalModels.Add(SagittalPanelModel);
            AxialPanelModel.OrthogonalModels.Add(CoronalPanelModel);
            SagittalPanelModel.OrthogonalModels.Add(AxialPanelModel);
            SagittalPanelModel.OrthogonalModels.Add(CoronalPanelModel);
            CoronalPanelModel.OrthogonalModels.Add(AxialPanelModel);
            CoronalPanelModel.OrthogonalModels.Add(SagittalPanelModel);

            //When we load a new image, render it
            MessengerInstance.Register<RTObjectLoadedMessage<DicomImageObject>>(this, x => {
                AxialPanelModel.SetImage(x.Value);
                SagittalPanelModel.SetImage(x.Value);
                CoronalPanelModel.SetImage(x.Value);
            });

            MessengerInstance.Register<RTObjectLoadedMessage<DicomDoseObject>>(this, x => {
                x.Value.NormalisationIsodose = this.NormalisationIsodose;
            });

            //When a dose is selected, render it
            MessengerInstance.Register<DoseObjectRenderMessage>(this, x =>
            {
                if (x.RemoveDose)
                {
                    AxialPanelModel.RemoveDose(x.DoseObject);
                    SagittalPanelModel.RemoveDose(x.DoseObject);
                    CoronalPanelModel.RemoveDose(x.DoseObject);
                }
                else
                {
                    AxialPanelModel.AddDose(x.DoseObject);
                    SagittalPanelModel.AddDose(x.DoseObject);
                    CoronalPanelModel.AddDose(x.DoseObject);
                }
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

            //Handle showing or closing loading dialog windows below
            MessengerInstance.Register<ProgressMessage>(this, HandleProgressMessage);
        }

        private ProgressDialogViewModel progressDialogViewModel = new ProgressDialogViewModel();
        public ProgressDialogView progressDialogView = new ProgressDialogView();
        public void HandleProgressMessage(ProgressMessage message)
        {
            if (progressDialogView.DataContext != progressDialogViewModel)
                progressDialogView.DataContext = progressDialogViewModel;

            progressDialogViewModel.Apply(message);

            if(message.ProgressType == Progress.Begin)
            {
                progressDialogView.Show();
            }
            if (message.ProgressType == Progress.End && progressDialogViewModel.ObjectProgressStatuses.Count == 0)
            {
                progressDialogView.Hide();
            }
        }

        public void OnClose()
        {
            MessengerInstance.Unregister(this);
            progressDialogView.Close();
            progressDialogView = null;
        }

        public void OpenDoseStatistics()
        {
            var doseStatsWindow = new DVHObjectDisplayView();
            doseStatsWindow.Show();
        }

        public void CreateCubePhantom()
        {
            var cubePhantom = new CubePhantom();
            cubePhantom.Create(200, 200, 300, 1, 1, 1);
            MessengerInstance.Send<RTObjectLoadedMessage<DicomImageObject>>(new RTObjectLoadedMessage<DicomImageObject>(cubePhantom));
        }

        public void CreateNewPOI()
        {
            var poi = new PointOfInterest();
            poi.Name = "New POI";
            MessengerInstance.Send(new RTObjectLoadedMessage<PointOfInterest>(poi));
            AxialPanelModel.AddPOI(poi);
            CoronalPanelModel.AddPOI(poi);
            SagittalPanelModel.AddPOI(poi);
        }

        public void ApplyNormalisation()
        {
            foreach(var dose in Workspace.Workspace.Current.Doses.GetList())
            {
                dose.NormalisationIsodose = this.NormalisationIsodose;
            }
            AxialPanelModel.DoseRenderer.MaxNumberOfGridPoints = RenderQuality;
            CoronalPanelModel.DoseRenderer.MaxNumberOfGridPoints = RenderQuality;
            SagittalPanelModel.DoseRenderer.MaxNumberOfGridPoints = RenderQuality;

            AxialPanelModel.Invalidate();
            CoronalPanelModel.Invalidate();
            SagittalPanelModel.Invalidate();
        }
    }

}