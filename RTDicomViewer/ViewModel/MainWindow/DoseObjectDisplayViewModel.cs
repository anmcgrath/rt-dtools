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

namespace RTDicomViewer.ViewModel.MainWindow
{
    public class DoseObjectDisplayViewModel : ViewModelBase
    {
        public ObservableCollection<DoseGridWrapper> Doses { get; set; }
        public DoseGridWrapper SelectedDose { get { return _selectedDose; } set { _selectedDose = value; RaisePropertyChanged("SelectedDose"); } }
        private DoseGridWrapper _selectedDose;

        public ObservableCollection<LUTType> LUTTypes { get; set; }

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
                Scaling = doseWrapper.Dose.Scaling,
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
                    heatLUT.Level = (float)(dose.Grid.MaxVoxel.Value / 2) + .1f * dose.Grid.MaxVoxel.Value;
                    heatLUT.Window = dose.Grid.MaxVoxel.Value * .8f; ;
                    return heatLUT;
            }
            return null;
        }
    }
}
