using DicomPanel.Core.Radiotherapy.Dose;
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

namespace RTDicomViewer.ViewModel.MainWindow
{
    public class DoseObjectDisplayViewModel:ViewModelBase
    {
        public ObservableCollection<SelectableObject<IDoseObject>> DoseObjects { get; set; }

        public DoseObjectDisplayViewModel()
        {
            DoseObjects = new ObservableCollection<SelectableObject<IDoseObject>>();
            MessengerInstance.Register<RTObjectLoadedMessage<DicomDoseObject>>(this, x => DoseLoadedMessageReceive(x));
            MessengerInstance.Register<RTObjectLoadedMessage<EgsDoseObject>>(this, x => DoseLoadedMessageReceive(x));
        }

        //When we receive a global message telling us a dicom dose object is loaded, we handle it
        public void DoseLoadedMessageReceive(RTObjectLoadedMessage<DicomDoseObject> message)
        {
            DoseLoaded(message.Value);
        }

        //When we receive a global message telling us an egs dose object is loaded, we handle it
        public void DoseLoadedMessageReceive(RTObjectLoadedMessage<EgsDoseObject> message)
        {
            DoseLoaded(message.Value);
        }

        public void AddNewDoseObject(SelectableObject<IDoseObject> selectableDoseObject)
        {
            selectableDoseObject.ObjectSelectionChanged += Dose_ObjectSelectionChanged;
            DoseObjects.Add(selectableDoseObject);
        }

        public void RemoveDoseObject(SelectableObject<IDoseObject> selectableDoseObject)
        {
            selectableDoseObject.ObjectSelectionChanged -= Dose_ObjectSelectionChanged;
            DoseObjects.Remove(selectableDoseObject);
        }

        public object DoseLoaded(IDoseObject dose)
        {
            var newSelectableObject = new SelectableObject<IDoseObject>(dose);
            AddNewDoseObject(newSelectableObject);
            SelectDose(dose);

            return null;
        }

        //When a dose object is selected, we handle the event that is fired
        private void Dose_ObjectSelectionChanged(object sender, SelectableObjectEventArgs e)
        {
            SelectableObject<IDoseObject> selectableDoseObject = (SelectableObject<IDoseObject>)sender;
            //Let everyone know we now want to render this dose object.
            if (e.IsNowSelected)
            {
                SelectDose(selectableDoseObject.Value);
            }
            else
            {
                UnselectDose(selectableDoseObject.Value);
            }
        }

        private void SelectDose(IDoseObject dose)
        {
            foreach (var doseObj in DoseObjects)
            {
                if (doseObj.Value != dose)
                {
                    doseObj.FireSelectionEvent = false;
                    doseObj.IsSelected = false;
                    doseObj.FireSelectionEvent = true;
                } else
                {
                    doseObj.FireSelectionEvent = false;
                    doseObj.IsSelected = true;
                    doseObj.FireSelectionEvent = true;
                }
            }
            MessengerInstance.Send(new DoseObjectRenderMessage(dose));
        }

        private void UnselectDose(IDoseObject dose)
        {
            MessengerInstance.Send(new DoseObjectRenderMessage(null));
        }

    }
}
