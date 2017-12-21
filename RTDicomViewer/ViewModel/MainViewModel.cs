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
using GalaSoft.MvvmLight.Ioc;
using RTDicomViewer.Utilities.Testing;

namespace RTDicomViewer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private DicomPanelModel _axialPanelModel;
        public DicomPanelModel AxialPanelModel { get; set; }
        public DicomPanelModel CoronalPanelModel { get; set; }
        public DicomPanelModel SagittalPanelModel { get; set; }
        private IFileOpener FileOpener { get; set; }

        public ToolBox ToolBox = new ToolBox();

        public RelayCommand OpenImageCommand => new RelayCommand(
            () => { FileOpener.BeginOpenImagesAsync(); });

        public RelayCommand OpenDicomDoseCommand => new RelayCommand(
            () => { FileOpener.BeginOpenDicomDoseAsync(); });

        public RelayCommand OpenEgsDoseCommand => new RelayCommand(
            () => { FileOpener.BeginOpenEgsDoseAsync(); });

        public RelayCommand OpenStructureSetCommand => new RelayCommand(
            () => { FileOpener.BeginOpenStructuresAsync(); });

        public RelayCommand OpenDoseStatisticsCommand => new RelayCommand(
            () => { OpenDoseStatistics(); });

        public RelayCommand CreateCubePhantomCommand => new RelayCommand(
            () => { CreateCubePhantom(); });

        public RelayCommand OpenDicomPlanCommand => new RelayCommand(
            () => { FileOpener.BeginOpenDicomPlanAsync(); });

        public RelayCommand CreateNewPOICommand => new RelayCommand(
            () => { CreateNewPOI(); });

        public RelayCommand CreateNewBeamCommand => new RelayCommand(
            () => { CreateNewBeam(); });

        public RelayCommand OpenHistogramWindowCommand => new RelayCommand(
            () => { var window = new HistogramCreatorWindowView(); window.Owner = Application.Current.MainWindow; window.Show(); });


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IFileOpener fileOpener)
        {
            Workspace.Workspace.Init();
            FileOpener = fileOpener;

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
                InvalidateAll();
            });

            MessengerInstance.Register<RTObjectDeletedMessage<DicomImageObject>>(this, x =>
            {
                AxialPanelModel.SetPrimaryImage(null);
                SagittalPanelModel.SetPrimaryImage(null);
                CoronalPanelModel.SetPrimaryImage(null);
                InvalidateAll();

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

            Workspace.Workspace.Current.Axial = AxialPanelModel;
            Workspace.Workspace.Current.Coronal = CoronalPanelModel;
            Workspace.Workspace.Current.Sagittal = SagittalPanelModel;

            SimpleIoc.Default.GetInstance<IProgressView>().DataContext = SimpleIoc.Default.GetInstance<IProgressService>();
        }

        private void InvalidateAll()
        {
            AxialPanelModel.Invalidate();
            SagittalPanelModel.Invalidate();
            CoronalPanelModel.Invalidate();
        }

        public void OnClose()
        {
            MessengerInstance.Unregister(this);
        }

        public void OpenDoseStatistics()
        {
            var doseStatsWindow = new DVHObjectDisplayView();
            doseStatsWindow.Owner = Application.Current.MainWindow;
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
                dose.Grid.NormalisationType = options.NormalisationType;
                dose.Grid.NormalisationPercent = options.NormalisationIsodose;
                dose.Grid.NormalisationPOI = options.POI;
                dose.Grid.RelativeNormalisationOption = options.RelativeNormalisationOption;
            }
            AxialPanelModel.DoseRenderer.MaxNumberOfGridPoints = options.RenderQuality;
            CoronalPanelModel.DoseRenderer.MaxNumberOfGridPoints = options.RenderQuality;
            SagittalPanelModel.DoseRenderer.MaxNumberOfGridPoints = options.RenderQuality;

            AxialPanelModel.DoseRenderer.ContourInfo = options.ContourInfo.ToList();
            CoronalPanelModel.DoseRenderer.ContourInfo = options.ContourInfo.ToList();
            SagittalPanelModel.DoseRenderer.ContourInfo = options.ContourInfo.ToList();

            InvalidateAll();
        }
    }

}