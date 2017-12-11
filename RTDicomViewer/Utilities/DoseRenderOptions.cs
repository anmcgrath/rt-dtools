using DicomPanel.Core.Render.Contouring;
using GalaSoft.MvvmLight;
using RT.Core.Dose;
using RT.Core.Planning;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Utilities
{
    public class DoseRenderOptions:ViewModelBase
    {
        public ObservableCollection<ContourInfo> ContourInfo { get; set; }
        public double NormalisationIsodose { get { return _normalisationIsodose; } set { _normalisationIsodose = value; RaisePropertyChanged("NormalisationIsodose"); } }
        private double _normalisationIsodose;
        public int RenderQuality { get; set; } = 80;
        public RelativeNormalisationOption RelativeNormalisationOption { get { return _relativeNormalisationOption; } set { _relativeNormalisationOption = value; RaisePropertyChanged("RelativeNormalisationOption"); } }
        private RelativeNormalisationOption _relativeNormalisationOption = RelativeNormalisationOption.Max;
        public NormalisationType NormalisationType { get; set; }
        public PointOfInterest POI { get; set; }
    }
}
