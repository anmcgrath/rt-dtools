using DicomPanel.Core.Render.Contouring;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RT.Core.DICOM;
using RT.Core.Dose;
using RTDicomViewer.Message;
using RTDicomViewer.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using RT.Core.Planning;

namespace RTDicomViewer.ViewModel.MainWindow
{
    public class AnalyseDisplayViewModel:ViewModelBase
    {
        public double NormalisationIsodose { get; set; } = 100.0;
        public List<RelativeNormalisationOption> RelativeNormalisationOptions { get; set; }
        public List<NormalisationType> NormalisationTypes { get; set; }
        public ObservableCollection<PointOfInterest> POIs { get; set; }

        public ObservableCollection<DicomColor> AvailableColors { get; set; } = new ObservableCollection<DicomColor>();

        public DoseRenderOptions RenderOptions { get; set; }

        public RelayCommand ApplyNormalisationCommand =>
            new RelayCommand(() =>
            {
                MessengerInstance.Send<DoseRenderQualityChanged>(new DoseRenderQualityChanged(RenderOptions));
            });

        public RelayCommand AddNewContourInfoCommand =>
            new RelayCommand((() =>
            {
                RenderOptions.ContourInfo.Add(new ContourInfo(DicomColors.Black, 100));
            }));

        public RelayCommand<ContourInfo> RemoveContourCommand =>
            new RelayCommand<ContourInfo>(x =>
            {
                RenderOptions.ContourInfo.Remove(x);
            });

        public AnalyseDisplayViewModel()
        {
            SetNormalisationChoices();
            InitRenderOptions();
            SubscribeToMessages();

            MessengerInstance.Send<DoseRenderQualityChanged>(new DoseRenderQualityChanged(RenderOptions));
        }

        private void SetNormalisationChoices()
        {
            var dcs = new DicomColors();
            var fields = typeof(DicomColors).GetFields();
            foreach (var field in fields)
                AvailableColors.Add((DicomColor)field.GetValue(dcs));

            RelativeNormalisationOptions = new List<RelativeNormalisationOption>()
            {
                RelativeNormalisationOption.Max,
                RelativeNormalisationOption.POI,
            };
            NormalisationTypes = new List<NormalisationType>()
            {
                NormalisationType.Relative,
                NormalisationType.Absolute
            };
            POIs = new ObservableCollection<PointOfInterest>();
        }

        private void InitRenderOptions()
        {
            RenderOptions = new DoseRenderOptions();
            RenderOptions.ContourInfo = new ObservableCollection<ContourInfo>(Workspace.Workspace.Current.ContourInfo);
            RenderOptions.NormalisationIsodose = 100;
            RenderOptions.RenderQuality = 80;
            RenderOptions.NormalisationType = NormalisationType.Relative;
            RenderOptions.RelativeNormalisationOption = RelativeNormalisationOption.Max;
        }

        private void SubscribeToMessages()
        {
            //Since this class manages normalisation, when a dose object is imported into the program set the relevant normalisation type
            MessengerInstance.Register<RTObjectAddedMessage<DicomDoseObject>>(this, x => {
                x.Value.Grid.NormalisationPercent = RenderOptions.NormalisationIsodose;
                x.Value.Grid.NormalisationPOI = RenderOptions.POI;
                x.Value.Grid.NormalisationType = RenderOptions.NormalisationType;
                x.Value.Grid.RelativeNormalisationOption = RenderOptions.RelativeNormalisationOption;
            });

            MessengerInstance.Register<RTObjectAddedMessage<PointOfInterest>>(this, x =>
            {
                POIs.Add(x.Value);
                if (POIs.Count == 1)
                    RenderOptions.POI = x.Value;
            });
        }
    }
}
