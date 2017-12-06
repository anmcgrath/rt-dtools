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

namespace RTDicomViewer.ViewModel.MainWindow
{
    public class DoseObjectDisplayViewModel : ViewModelBase
    {
        public ObservableCollection<DoseGridWrapper> Doses { get; set; }
        public DoseGridWrapper SelectedDose { get { return _selectedDose; } set { _selectedDose = value; RaisePropertyChanged("SelectedDose");  } }
        private DoseGridWrapper _selectedDose;

        public DoseGridWrapper SelectedMathDose1 { get { return _selectedMathDose1; } set { _selectedMathDose1 = value; RaisePropertyChanged("SelectedMathDose1"); RaisePropertyChanged("SubtractCommand"); } }
        private DoseGridWrapper _selectedMathDose1;

        public DoseGridWrapper SelectedMathDose2 { get { return _selectedMathDose2; } set { _selectedMathDose2 = value; RaisePropertyChanged("SelectedMathDose2"); RaisePropertyChanged("SubtractCommand"); } }
        private DoseGridWrapper _selectedMathDose2;

        public bool IsComputing { get { return _isComputing; } set { _isComputing = value; RaisePropertyChanged("SubtractCommand"); } }
        private bool _isComputing;

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

        public ObservableCollection<LUTType> LUTTypes { get; set; }
        private IProgressService progressVm;

        public RelayCommand SubtractCommand => new RelayCommand(() =>
        {
            SubtractDoses();
        },
            ()=>
            {
                return !IsComputing && !(SelectedMathDose1 == null) && !(SelectedMathDose2 == null);
            });

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
                Doses.Remove(wrapper);
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
                Workspace.Workspace.Current.Axial.Invalidate();
                Workspace.Workspace.Current.Sagittal.Invalidate();
                Workspace.Workspace.Current.Coronal.Invalidate();
                doseWrapper.RenderableImage = null;
            }
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
                    contourLUT.Create(contourList, dose.Grid.MaxVoxel.Value);
                    return contourLUT;
                case LUTType.Heat:
                    var heatLUT = new HeatLUT();
                    if (dose.Grid.ValueUnit == Unit.Gamma)
                    {
                        heatLUT.Level = .5f;
                        heatLUT.Window = 1;
                    }else
                    {
                        heatLUT.Level = 0.6f * dose.Grid.MaxVoxel.Value;
                        heatLUT.Window = 0.8f * dose.Grid.MaxVoxel.Value;
                    }
                    return heatLUT;
            }
            return null;
        }

        public async void SubtractDoses()
        {
            if (SelectedMathDose1 == null || SelectedMathDose2 == null)
                return;
            DicomDoseObject newDoseObject = new DicomDoseObject();
            var math = new GridMath();
            
            IsComputing = true;
            var progressItem = progressVm.CreateNew("Performing Gamma Calculation...", false);
            var progress = new Progress<int>(x => { progressItem.ProgressAmount = x; });
            await Task.Run(() =>
            {
                newDoseObject.Grid = math.Gamma(SelectedMathDose1.Dose.Grid, SelectedMathDose2.Dose.Grid, progress);
                //newDoseObject.Grid = math.Subtract(SelectedMathDose1.Dose.Grid, SelectedMathDose2.Dose.Grid);
            });
            progressVm.End(progressItem);

            newDoseObject.Grid.ValueUnit = Unit.Gamma;
            newDoseObject.Grid.Name = "Gamma Result";
            IsComputing = false;
            MessengerInstance.Send<RTObjectAddedMessage<DicomDoseObject>>(new RTObjectAddedMessage<DicomDoseObject>(newDoseObject));
        }
    }
}
