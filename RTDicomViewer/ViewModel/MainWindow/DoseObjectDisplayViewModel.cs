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
    public class DoseObjectDisplayViewModel:ViewModelBase
    {
        /// <summary>
        /// Renderable Images of the dose can be rendered as a color wash etc.
        /// </summary>
        public Dictionary<SelectableObject<IDoseObject,DoseRenderType>, RenderableImage> RenderableImages;
        public RenderableImage SelectedRenderableImage { get { return _selectedRenderableImage; } set { _selectedRenderableImage = value; RaisePropertyChanged("SelectedRenderableImage"); } }
        private RenderableImage _selectedRenderableImage;

        public ObservableCollection<SelectableObject<IDoseObject, DoseRenderType>> DoseObjects { get; set; }
        /// <summary>
        /// Selected as in highlighted in the list, not as in rendered
        /// </summary>
        public SelectableObject<IDoseObject,DoseRenderType> SelectedDose { get { return _selectedDose; } set { _selectedDose = value; RaisePropertyChanged("SelectedDose"); SelectedRenderableImage = RenderableImages[SelectedDose]; } }
        private SelectableObject<IDoseObject, DoseRenderType> _selectedDose { get; set; }

        private ContourLUT contourLUT = new ContourLUT();
        private List<ContourInfo> contourInfo;

        public ObservableCollection<ILUT> LUTs { get; set; }

        public DoseObjectDisplayViewModel()
        {
            DoseObjects = new ObservableCollection<SelectableObject<IDoseObject, DoseRenderType>>();
            MessengerInstance.Register<RTObjectAddedMessage<DicomDoseObject>>(this, x => DoseLoadedMessageReceive(x));
            MessengerInstance.Register<RTObjectAddedMessage<EgsDoseObject>>(this, x => DoseLoadedMessageReceive(x));
            RenderableImages = new Dictionary<SelectableObject<IDoseObject, DoseRenderType>, RenderableImage>();
            LUTs = new ObservableCollection<ILUT>()
            {
                contourLUT,
                new HeatLUT(),
            };
            MessengerInstance.Register<DoseRenderQualityChanged>(this, x =>
             {
                 List<ContourInfo> newList = new List<ContourInfo>();
                 contourInfo = x.Options.ContourInfo.ToList();

                 if(SelectedDose != null)
                    contourLUT.Create(newList, SelectedDose.Value.Grid.MaxVoxel.Value);
             }
            );
        }

        //When we receive a global message telling us a dicom dose object is loaded, we handle it
        public void DoseLoadedMessageReceive(RTObjectAddedMessage<DicomDoseObject> message)
        {
            DoseLoaded(message.Value);
        }

        //When we receive a global message telling us an egs dose object is loaded, we handle it
        public void DoseLoadedMessageReceive(RTObjectAddedMessage<EgsDoseObject> message)
        {
            DoseLoaded(message.Value);
        }

        public void AddNewDoseObject(SelectableObject<IDoseObject, DoseRenderType> selectableDoseObject)
        {
            selectableDoseObject.ObjectSelectionChanged += Dose_ObjectSelectionChanged;
            selectableDoseObject.ChildrenObjectsSelectionChanged += SelectableDoseObject_ChildrenObjectsSelectionChanged;
            DoseObjects.Add(selectableDoseObject);
            RenderableImages.Add(selectableDoseObject, new RenderableImage()
            {
                LUT = LUTs.First(),
                Alpha = 0.7f,
                BlendMode = DicomPanel.Core.Render.Blending.BlendMode.OverWhereNonZero,
                Grid = selectableDoseObject.Value.Grid,
                Scaling = selectableDoseObject.Value.Scaling,
                Name = "",
                ScreenRect = new RT.Core.Utilities.RTMath.Rectd(0,0,1,1),
                Units = "",
            });
        }

        private void SelectableDoseObject_ChildrenObjectsSelectionChanged(object sender, MultiSelectableObjectEventArgs<DoseRenderType> e)
        {
            SelectableObject<IDoseObject, DoseRenderType> dose = (SelectableObject<IDoseObject, DoseRenderType>)sender;
            foreach (var unselectedObject in e.UnselectedObjects)
            {
                if (unselectedObject.Value == DoseRenderType.Lines)
                {
                    Workspace.Workspace.Current.Axial.RemoveDose(dose.Value);
                    Workspace.Workspace.Current.Coronal.RemoveDose(dose.Value);
                    Workspace.Workspace.Current.Sagittal.RemoveDose(dose.Value);
                }else
                {
                    Workspace.Workspace.Current.Axial.RemoveImage(RenderableImages[dose]);
                    Workspace.Workspace.Current.Coronal.RemoveImage(RenderableImages[dose]);
                    Workspace.Workspace.Current.Sagittal.RemoveImage(RenderableImages[dose]);
                }
            }
            foreach (var unselectedObject in e.SelectedObjects)
            {
                if (unselectedObject.Value == DoseRenderType.Lines)
                {
                    Workspace.Workspace.Current.Axial.AddDose(dose.Value);
                    Workspace.Workspace.Current.Coronal.AddDose(dose.Value);
                    Workspace.Workspace.Current.Sagittal.AddDose(dose.Value);
                }
                else
                {
                    Workspace.Workspace.Current.Axial.AddImage(RenderableImages[dose]);
                    Workspace.Workspace.Current.Coronal.AddImage(RenderableImages[dose]);
                    Workspace.Workspace.Current.Sagittal.AddImage(RenderableImages[dose]);
                }
            }
        }

        public void RemoveDoseObject(SelectableObject<IDoseObject, DoseRenderType> selectableDoseObject)
        {
            selectableDoseObject.ObjectSelectionChanged -= Dose_ObjectSelectionChanged;
            DoseObjects.Remove(selectableDoseObject);
            RenderableImages.Remove(selectableDoseObject);
        }

        public object DoseLoaded(IDoseObject dose)
        {
            var newSelectableObject = new SelectableObject<IDoseObject, DoseRenderType>(dose);
            newSelectableObject.AddChild(new SelectableObject<DoseRenderType>(DoseRenderType.Lines));
            newSelectableObject.AddChild(new SelectableObject<DoseRenderType>(DoseRenderType.Wash));
            contourLUT.Create(contourInfo, dose.Grid.MaxVoxel.Value);

            AddNewDoseObject(newSelectableObject);
            SelectedDose = newSelectableObject;
            SelectDose(dose);
            return null;
        }

        //When a dose object is selected, we handle the event that is fired
        private void Dose_ObjectSelectionChanged(object sender, SelectableObjectEventArgs e)
        {
            SelectableObject<IDoseObject,DoseRenderType> selectableDoseObject = (SelectableObject<IDoseObject, DoseRenderType>)sender;
            //Let everyone know we now want to render this dose object.
            if (e.IsNowSelected)
            {
                foreach(var child in selectableDoseObject.Children)
                {
                    child.IsSelected = true;
                }
            }
            else
            {
                foreach (var child in selectableDoseObject.Children)
                {
                    child.IsSelected = false;
                }
            }
        }

        private void SelectDose(IDoseObject dose)
        {
            foreach (var doseObj in DoseObjects)
            {
                if (doseObj.Value == dose)
                {
                    doseObj.IsSelected = true;
                }
            }
        }

        private void UnselectDose(IDoseObject dose)
        {
            foreach (var doseObj in DoseObjects)
            {
                if (doseObj.Value == dose)
                {
                    doseObj.IsSelected = false;
                }
            }
        }

    }
}
