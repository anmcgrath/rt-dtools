using RT.Core.IO;
using RT.Core.Dose;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using RTDicomViewer.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using RTDicomViewer.ViewModel.Dialogs;
using RT.Core.Imaging;
using RT.Core.ROIs;

namespace RTDicomViewer.IO
{
    public class FileOpener:IFileOpener
    {
        private IProgressService ProgressService;
        public FileOpener(IProgressService progressService)
        {
            ProgressService = progressService;
        }

        public async void BeginOpenDicomDoseAsync()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "Open Dicom Dose";
            if (openFileDialog.ShowDialog() == true)
            {
                var progressItem = ProgressService.CreateNew("Loading Dose File...", false);
                var progress = new Progress<double>(x => { progressItem.ProgressAmount = (int)x; });
                DicomDoseObject openedObject = null;
                await Task.Run(async () =>
                {
                    try
                    {
                        openedObject = await DicomLoader.LoadDicomDoseAsync(openFileDialog.FileName, progress);
                    }
                    catch (Exception e)
                    {
                        Messenger.Default.Send(new NotificationMessage("Could not open file: " + e.Message));
                    }
                });
                if (openedObject != null)
                    Messenger.Default.Send(new RTDicomViewer.Message.RTObjectAddedMessage<DicomDoseObject>(openedObject));

                ProgressService.End(progressItem);
            }
        }

        public void BeginOpenDicomPlanAsync()
        {
            
        }

        public async void BeginOpenEgsDoseAsync()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "Open 3D Dose File";
            if (openFileDialog.ShowDialog() == true)
            {
                var progressItem = ProgressService.CreateNew("Loading 3DDose File...", false);
                var progress = new Progress<double>(x => { progressItem.ProgressAmount = (int)x; });
                EgsDoseObject openedObject = null;
                await Task.Run(async () =>
                {
                    try
                    {
                        openedObject = await DicomLoader.LoadEgsObjectAsync(openFileDialog.FileName, progress);
                    }
                    catch (Exception e)
                    {
                        Messenger.Default.Send(new NotificationMessage("Could not open file: " + e.Message));
                    }
                });
                if (openedObject != null)
                    Messenger.Default.Send(new RTDicomViewer.Message.RTObjectAddedMessage<EgsDoseObject>(openedObject));

                ProgressService.End(progressItem);
            }
        }

        public async void BeginOpenImagesAsync()
        {
            string[] files;
            if((files = getFileNames("Open Dicom Image(s)",true)) != null)
            {
                var pi = ProgressService.CreateNew("Loading Dicom Image(s)...", false);
                var progress = new Progress<double>(x => { pi.ProgressAmount = (int)x; });

                DicomImageObject openedObject = null;
                await Task.Run(async () =>
                {
                    try
                    {
                        openedObject = await DicomLoader.LoadDicomImageAsync(files, progress);
                    }
                    catch (Exception e)
                    {
                        Messenger.Default.Send(new NotificationMessage("Could not open file: " + e.Message));
                    }
                });
                if (openedObject != null)
                    Messenger.Default.Send(new RTDicomViewer.Message.RTObjectAddedMessage<DicomImageObject>(openedObject));

                ProgressService.End(pi);
            }

        }

        public async void BeginOpenStructuresAsync()
        {
            string[] files;
            if ((files = getFileNames("Open Structure Set", true)) != null)
            {
                var pi = ProgressService.CreateNew("Loading Structure Set...", false);
                var progress = new Progress<double>(x => { pi.ProgressAmount = (int)x; });

                StructureSet openedObject = null;
                await Task.Run(async () =>
                {
                    try
                    {
                        openedObject = await DicomLoader.LoadStructureSetAsync(files, progress);
                    }
                    catch (Exception e)
                    {
                        Messenger.Default.Send(new NotificationMessage("Could not open file: " + e.Message));
                    }
                });
                if (openedObject != null)
                    Messenger.Default.Send(new RTDicomViewer.Message.RTObjectAddedMessage<StructureSet>(openedObject));

                ProgressService.End(pi);
            }
        }

        private string[] getFileNames(string title, bool allowMultiple)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = allowMultiple;
            openFileDialog.Title = title;
            if(openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileNames;
            }else
            {
                return null;
            }
        }

    }
}
