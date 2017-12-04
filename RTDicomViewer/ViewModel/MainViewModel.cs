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
using RTDicomViewer.Utilities;
using System.Linq;
using System.Collections.Generic;

namespace RTDicomViewer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
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

        public RelayCommand CreateNewBeamCommand => new RelayCommand(
            () => { CreateNewBeam(); });

        public RelayCommand OpenHistogramWindowCommand => new RelayCommand(
            () => { var window = new HistogramCreatorWindowView(); window.Owner = Application.Current.MainWindow; window.Show(); });


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
            MessengerInstance.Register<RTObjectAddedMessage<DicomImageObject>>(this, x => {
                AxialPanelModel.SetPrimaryImage(x.Value);
                SagittalPanelModel.SetPrimaryImage(x.Value);
                CoronalPanelModel.SetPrimaryImage(x.Value);
            });

            MessengerInstance.Register<RTObjectDeletedMessage<DicomImageObject>>(this, x =>
            {
                AxialPanelModel.SetPrimaryImage(null);
                SagittalPanelModel.SetPrimaryImage(null);
                CoronalPanelModel.SetPrimaryImage(null);
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

            MessengerInstance.Register<NotificationMessage>(this, x =>
             {
                 MessageBox.Show(x.Notification);
             });

            //When the dose render options are changed...
            MessengerInstance.Register<DoseRenderQualityChanged>(this, x =>
            {
                ChangeDoseRenderOptions(x.Options);
            });

            //Handle showing or closing loading dialog windows below
            MessengerInstance.Register<ProgressMessage>(this, HandleProgressMessage);

            Workspace.Workspace.Current.Axial = AxialPanelModel;
            Workspace.Workspace.Current.Coronal = CoronalPanelModel;
            Workspace.Workspace.Current.Sagittal = SagittalPanelModel;
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
            MessengerInstance.Send<RTObjectAddedMessage<DicomImageObject>>(new RTObjectAddedMessage<DicomImageObject>(cubePhantom));
        }

        public void CreateNewPOI()
        {
            var poi = new PointOfInterest();
            poi.Name = "New POI";
            MessengerInstance.Send(new RTObjectAddedMessage<PointOfInterest>(poi));
            AxialPanelModel.AddPOI(poi);
            CoronalPanelModel.AddPOI(poi);
            SagittalPanelModel.AddPOI(poi);
        }

        public void CreateNewBeam()
        {
            var beam = new Beam();
            beam.ControlPoints = new List<ControlPoint>();
            beam.GantryStart = 0;
            beam.CollimatorAngle = 0;
            beam.Name = "New Beam";
            if (Workspace.Workspace.Current.Points.GetList().Count == 0)
            {
                CreateNewPOI();
            }
            beam.Isocentre = Workspace.Workspace.Current.Points.GetList().First();
            MessengerInstance.Send(new RTObjectAddedMessage<Beam>(beam));

            AxialPanelModel.AddBeam(beam);
            CoronalPanelModel.AddBeam(beam);
            SagittalPanelModel.AddBeam(beam);
        }

        public void ChangeDoseRenderOptions(DoseRenderOptions options)
        {
            foreach(var dose in Workspace.Workspace.Current.Doses.GetList())
            {
                dose.NormalisationIsodose = options.NormalisationIsodose;
            }
            AxialPanelModel.DoseRenderer.MaxNumberOfGridPoints = options.RenderQuality;
            CoronalPanelModel.DoseRenderer.MaxNumberOfGridPoints = options.RenderQuality;
            SagittalPanelModel.DoseRenderer.MaxNumberOfGridPoints = options.RenderQuality;

            AxialPanelModel.DoseRenderer.ContourInfo = options.ContourInfo.ToList();
            CoronalPanelModel.DoseRenderer.ContourInfo = options.ContourInfo.ToList();
            SagittalPanelModel.DoseRenderer.ContourInfo = options.ContourInfo.ToList();

            AxialPanelModel.Invalidate();
            CoronalPanelModel.Invalidate();
            SagittalPanelModel.Invalidate();
        }
    }

}