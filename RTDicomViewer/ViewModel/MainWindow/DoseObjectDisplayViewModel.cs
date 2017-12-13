using RT.Core.Dose;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RTDicomViewer.Message;
using RTDicomViewer.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DicomPanel.Core.Render;
using RT.Core.Imaging.LUT;
using DicomPanel.Core.Render.Contouring;
using RT.Core.Eval;
using RT.Core.Geometry;
using RT.Core.ROIs;
using System.Windows;
using RTDicomViewer.ViewModel.Dialogs;
using RTDicomViewer.View.Dialogs;

namespace RTDicomViewer.ViewModel.MainWindow
{
    public class DoseObjectDisplayViewModel : ViewModelBase
    {
        public ObservableCollection<DoseGridWrapper> Doses { get; set; }
        public DoseGridWrapper SelectedDose { get { return _selectedDose; } set { _selectedDose = value; RaisePropertyChanged("SelectedDose");  } }
        private DoseGridWrapper _selectedDose;

        public RelayCommand<IDoseObject> DeleteDoseCommand => new RelayCommand<IDoseObject>(x =>
        {
            //TODO: Can't delete if it's involved in a calculation..
            var result = MessageBox.Show("Are you sure you wish to delete this dose object?");
            if (result == MessageBoxResult.Yes)
            {
                if (x is EgsDoseObject)
                {
                    MessengerInstance.Send(new RTObjectDeletedMessage<EgsDoseObject>((EgsDoseObject)x));
                }else if(x is DicomDoseObject)
                {
                    MessengerInstance.Send(new RTObjectAddedMessage<DicomDoseObject>((DicomDoseObject)x));
                }
            }
        });

        public RelayCommand GammaCommand => new RelayCommand(() =>
        {
            var gammaWindow = new GammaWindowView();
            gammaWindow.Owner = Application.Current.MainWindow;
            gammaWindow.ShowDialog();
        });

        public ObservableCollection<LUTType> LUTTypes { get; set; }
        private IProgressService progressVm;

        public DoseObjectDisplayViewModel(IProgressService progressVm)
        {
            this.progressVm = progressVm;
            Doses = new ObservableCollection<DoseGridWrapper>();
            LUTTypes = new ObservableCollection<LUTType>() { LUTType.Contour, LUTType.Heat };
            MessengerInstance.Register<RTObjectAddedMessage<EgsDoseObject>>(this, x => AddNewDose(x.Value));
            MessengerInstance.Register<RTObjectAddedMessage<DicomDoseObject>>(this, x => AddNewDose(x.Value));
            MessengerInstance.Register<RTObjectDeletedMessage<EgsDoseObject>>(this, x => RemoveDose(x.Value));
            MessengerInstance.Register<DoseRenderQualityChanged>(this, x => OnContourListChanged(x.Options.ContourInfo.ToList()));
        }

        public void RemoveDose(IDoseObject dose)
        {
            DoseGridWrapper wrapper = Doses.Where(b => b.Dose == dose).FirstOrDefault();
            if (wrapper != null)
            {
                wrapper.PropertyChanged -= Wrapper_PropertyChanged;
                Doses.Remove(wrapper);
            }
        }

        public void AddNewDose(IDoseObject dose)
        {
            var wrapper = new DoseGridWrapper();
            wrapper.Dose = dose;
            wrapper.LUTType = LUTType.Contour;
            wrapper.LUT = getNewLUT(wrapper.LUTType, dose);
            wrapper.RenderDoseWash = false;
            wrapper.RenderLines = true;

            Doses.Add(wrapper);
            SelectedDose = wrapper;
            OnRenderDoseChanged(wrapper);

            wrapper.PropertyChanged += Wrapper_PropertyChanged;
        }

        private void Wrapper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var doseWrapper = (DoseGridWrapper)sender;
            if (e.PropertyName == "RenderDoseWash")
                OnRenderDoseChanged(doseWrapper);
            else if (e.PropertyName == "RenderLines")
                OnRenderDoseChanged(doseWrapper);
            else if (e.PropertyName == "LUTType")
                OnLUTTypeChanged(doseWrapper);
        }

        public void OnContourListChanged(List<ContourInfo> contourList)
        {
            foreach (var doseWrapper in Doses)
            {
                if (doseWrapper.LUTType == LUTType.Contour)
                    doseWrapper.LUT = getNewLUT(LUTType.Contour, doseWrapper.Dose, contourList);
                if (doseWrapper.RenderLines || doseWrapper.LUTType == LUTType.Contour)
                    OnRenderDoseChanged(doseWrapper);
            }
        }

        public void OnLUTTypeChanged(DoseGridWrapper doseWrapper)
        {
            if (doseWrapper == null)
                return;
            doseWrapper.LUT = getNewLUT(doseWrapper.LUTType, doseWrapper.Dose);
            OnRenderDoseChanged(doseWrapper);
        }

        public void OnRenderDoseChanged(DoseGridWrapper doseWrapper)
        {
            if (doseWrapper == null)
                return;

            if (doseWrapper.RenderLines)
            {
                Workspace.Workspace.Current.Axial.AddDose(doseWrapper.Dose);
                Workspace.Workspace.Current.Sagittal.AddDose(doseWrapper.Dose);
                Workspace.Workspace.Current.Coronal.AddDose(doseWrapper.Dose);
            }
            else
            {
                Workspace.Workspace.Current.Axial.RemoveDose(doseWrapper.Dose);
                Workspace.Workspace.Current.Sagittal.RemoveDose(doseWrapper.Dose);
                Workspace.Workspace.Current.Coronal.RemoveDose(doseWrapper.Dose);
            }
            if (doseWrapper.RenderDoseWash)
            {
                if (doseWrapper.RenderableImage != null)
                {
                    Workspace.Workspace.Current.Axial.RemoveImage(doseWrapper.RenderableImage);
                    Workspace.Workspace.Current.Sagittal.RemoveImage(doseWrapper.RenderableImage);
                    Workspace.Workspace.Current.Coronal.RemoveImage(doseWrapper.RenderableImage);
                    doseWrapper.RenderableImage = null;
                }
                doseWrapper.RenderableImage = getNewRenderableImage(doseWrapper);
                Workspace.Workspace.Current.Axial.AddImage(doseWrapper.RenderableImage);
                Workspace.Workspace.Current.Sagittal.AddImage(doseWrapper.RenderableImage);
                Workspace.Workspace.Current.Coronal.AddImage(doseWrapper.RenderableImage);
            }
            else if (doseWrapper.RenderableImage != null)
            {
                Workspace.Workspace.Current.Axial.RemoveImage(doseWrapper.RenderableImage);
                Workspace.Workspace.Current.Sagittal.RemoveImage(doseWrapper.RenderableImage);
                Workspace.Workspace.Current.Coronal.RemoveImage(doseWrapper.RenderableImage);
                doseWrapper.RenderableImage = null;
            }

            Workspace.Workspace.Current.Axial.Invalidate();
            Workspace.Workspace.Current.Sagittal.Invalidate();
            Workspace.Workspace.Current.Coronal.Invalidate();
        }

        private RenderableImage getNewRenderableImage(DoseGridWrapper doseWrapper)
        {
            RenderableImage renderableImage = new RenderableImage()
            {
                Alpha = 0.7f,
                BlendMode = DicomPanel.Core.Render.Blending.BlendMode.OverWhereNonZero,
                Grid = doseWrapper.Dose.Grid,
                LUT = doseWrapper.LUT,
                Name = "Dose",
                Scaling = doseWrapper.Dose.Grid.Scaling,
                ScreenRect = new RT.Core.Utilities.RTMath.Rectd(0, 0, 1, 1),
                Units = "undefined",
            };
            return renderableImage;
        }

        private ILUT getNewLUT(LUTType lutType, IDoseObject dose)
        {
            return getNewLUT(lutType, dose, Workspace.Workspace.Current.ContourInfo);
        }

        private ILUT getNewLUT(LUTType lutType, IDoseObject dose, List<ContourInfo> contourList)
        {
            switch (lutType)
            {
                case LUTType.Contour:
                    var contourLUT = new ContourLUT();
                    var norm = dose.Grid.GetNormalisationAmount();
                    contourLUT.Create(contourList, dose.Grid.MaxVoxel.Value * dose.Grid.Scaling, dose.Grid.GetNormalisationAmount());
                    return contourLUT;
                case LUTType.Heat:
                    var heatLUT = new HeatLUT();
                    if (dose.Grid.ValueUnit == Unit.Gamma)
                    {
                        heatLUT.Level = .5f;
                        heatLUT.Window = 1;
                    }else
                    {
                        heatLUT.Level = 0.6f * dose.Grid.MaxVoxel.Value * dose.Grid.Scaling;
                        heatLUT.Window = 0.8f * dose.Grid.MaxVoxel.Value * dose.Grid.Scaling;
                    }
                    return heatLUT;
            }
            return null;
        }
    }
}
