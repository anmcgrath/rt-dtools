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

namespace RTDicomViewer.ViewModel.MainWindow
{
    public class AnalyseDisplayViewModel:ViewModelBase
    {
        public double NormalisationIsodose { get; set; } = 100.0;
        public int RenderQuality { get; set; } = 50;

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
            var dcs = new DicomColors();
            var fields = typeof(DicomColors).GetFields();
            foreach (var field in fields)
                AvailableColors.Add((DicomColor)field.GetValue(dcs));
              
            
            RenderOptions = new DoseRenderOptions();
            RenderOptions.ContourInfo = new ObservableCollection<ContourInfo>(Workspace.Workspace.Current.ContourInfo);
            RenderOptions.NormalisationIsodose = 100;
            RenderOptions.RenderQuality = 50;

            //Since this class manages normalisation, when a dose object is imported into the program set the relevant normalisation type
            MessengerInstance.Register<RTObjectAddedMessage<DicomDoseObject>>(this, x => {
                x.Value.NormalisationIsodose = NormalisationIsodose;
            });

            MessengerInstance.Send<DoseRenderQualityChanged>(new DoseRenderQualityChanged(RenderOptions));
        }
    }
}
