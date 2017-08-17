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
            foreach (var obj in DoseObjects)
                obj.IsSelected = false;
            var newSelectableObject = new SelectableObject<IDoseObject>(dose);
            AddNewDoseObject(newSelectableObject);

            return null;
        }

        //When a dose object is selected, we handle the event that is fired
        private void Dose_ObjectSelectionChanged(object sender, SelectableObjectEventArgs e)
        {
            //Let everyone know we now want to render this dose object.
            if(e.IsNowSelected)
            {
                SelectableObject<IDoseObject> selectableDoseObject = (SelectableObject<IDoseObject>)sender;
                MessengerInstance.Send(new DoseObjectRenderMessage(selectableDoseObject.Value));
            }else
            {
                SelectableObject<IDoseObject> selectableDoseObject = (SelectableObject<IDoseObject>)sender;
                MessengerInstance.Send(new DoseObjectRenderMessage(null));
            }
        }

    }
}
