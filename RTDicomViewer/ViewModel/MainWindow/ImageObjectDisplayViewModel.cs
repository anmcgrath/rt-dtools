using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RT.Core.Imaging;
using RTDicomViewer.Message;
using RTDicomViewer.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RTDicomViewer.ViewModel.MainWindow
{
    public class ImageObjectDisplayViewModel:ViewModelBase
    {
        public ObservableCollection<SelectableObject<ImagePreviewObject>> Images { get; set; }

        public RelayCommand<DicomImageObject> DeleteImageCommand => new RelayCommand<DicomImageObject>(x =>
        {
        if (MessageBox.Show("Are you sure you wish to delete the image set " + x.Name + "?", "Delete?", System.Windows.MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
                sendDeleteImageMessage(x);
        },y=>
        {
            return true;
        });

        public RelayCommand<DicomImageObject> SetPrimaryCommand => new RelayCommand<DicomImageObject>(x =>
        {
            Workspace.Workspace.Current.Axial.SetPrimaryImage(x);
            Workspace.Workspace.Current.Sagittal.SetPrimaryImage(x);
            Workspace.Workspace.Current.Coronal.SetPrimaryImage(x);
        }, y =>
        {
            return y != Workspace.Workspace.Current.Axial.PrimaryImage;
        });

        public RelayCommand<DicomImageObject> SetSecondaryCommand => new RelayCommand<DicomImageObject>(x =>
        {
            Workspace.Workspace.Current.Axial.SetSecondaryImage(x);
            Workspace.Workspace.Current.Sagittal.SetSecondaryImage(x);
            Workspace.Workspace.Current.Coronal.SetSecondaryImage(x);
        }, y =>
        {
            return true;
        });

        public ImageObjectDisplayViewModel()
        {
            Images = new ObservableCollection<SelectableObject<ImagePreviewObject>>();
            MessengerInstance.Register<RTObjectAddedMessage<DicomImageObject>>(this, x => addImage(x.Value));
            MessengerInstance.Register<RTObjectDeletedMessage<DicomImageObject>>(this, x => removeImage(x.Value));
        }

        private void addImage(DicomImageObject img)
        {
            Images.Add(new SelectableObject<ImagePreviewObject>(new ImagePreviewObject(img)));
        }

        private void removeImage(DicomImageObject img)
        {
            for(int i = 0; i< Images.Count; i++)
            {
                if (Images[i].Value.Image == img)
                    Images.RemoveAt(i);
            }
        }

        private void sendDeleteImageMessage(DicomImageObject obj)
        {
            RTObjectDeletedMessage<DicomImageObject> msg = new RTObjectDeletedMessage<DicomImageObject>(obj);
            this.MessengerInstance.Send<RTObjectDeletedMessage<DicomImageObject>>(msg);
            GC.Collect();
        }


    }
}
