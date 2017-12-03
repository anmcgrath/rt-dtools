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

        public ObservableCollection<LUTType> LUTTypes { get; set; }

        public RelayCommand SubtractCommand => new RelayCommand(() =>
        {
            SubtractDoses();
        },
            ()=>
            {
                return !IsComputing && !(SelectedMathDose1 == null) && !(SelectedMathDose2 == null);
            });

        public DoseObjectDisplayViewModel()
        {
            Doses = new ObservableCollection<DoseGridWrapper>();
            LUTTypes = new ObservableCollection<LUTType>() { LUTType.Contour, LUTType.Heat };
            MessengerInstance.Register<RTObjectAddedMessage<EgsDoseObject>>(this, x => AddNewDose(x.Value));
            MessengerInstance.Register<RTObjectAddedMessage<DicomDoseObject>>(this, x => AddNewDose(x.Value));
            MessengerInstance.Register<DoseRenderQualityChanged>(this, x => OnContourListChanged(x.Options.ContourInfo.ToList()));
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
                    heatLUT.Level = .5f;
                    heatLUT.Window = 1;
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
            await Task.Run(() =>
            {
                newDoseObject.Grid = math.Gamma(SelectedMathDose1.Dose.Grid, SelectedMathDose2.Dose.Grid);
                //newDoseObject.Grid = math.Subtract(SelectedMathDose1.Dose.Grid, SelectedMathDose2.Dose.Grid);
            });
            IsComputing = false;
            this.AddNewDose(newDoseObject);

            GridBasedVoxelDataStructure grid = (GridBasedVoxelDataStructure)newDoseObject.Grid;
            int num = 0;
            int total = 0;
            for(int i = 0; i < grid.XCoords.Length; i++)
            {
                for(int j = 0; j < grid.YCoords.Length; j++)
                {
                    for(int k = 0; k < grid.ZCoords.Length; k++)
                    {
                        //if (ptv.ContainsPointNonInterpolated(grid.XCoords[i], grid.YCoords[j], grid.ZCoords[k]))
                        //{
                        var val = grid.Data[i, j, k];
                        if(val >= 1)
                             num++;
                        if(val != -1)
                            total++;
                        //}
                    }
                }
            }
            MessageBox.Show("num: " + num + ", total:" + total);

        }
    }
}
