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

namespace RTDicomViewer.IO
{
    public class FileOpener
    {
        /// <summary>
        /// Opens a file dialog window and opens a DICOM file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public async void BeginOpenDicomAsync<T>(bool multipleFiles, string dialogTitle)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = multipleFiles;
            openFileDialog.Title = dialogTitle;
            
            if(openFileDialog.ShowDialog() == true)
            {
                T openedObject = default(T);
                Messenger.Default.Send<ProgressMessage>(new ProgressMessage(this,Progress.Begin,"Loading " + typeof(T) + "..." ));

                await Task.Run(async () =>
                {
                    try
                    {
                        openedObject = await DicomLoader.LoadAsync<T>(openFileDialog.FileNames);
                    }catch(Exception e)
                    {
                        Messenger.Default.Send(new NotificationMessage("Could not open file: " + e.Message));
                    }
                });
                if (openedObject != null)
                    Messenger.Default.Send(new RTDicomViewer.Message.RTObjectLoadedMessage<T>(openedObject));

                Messenger.Default.Send<ProgressMessage>(new ProgressMessage(this,Progress.End, "Loading Complete."));
            }
        }

        public async void BeginOpenEgsAsync()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "Open 3D Dose File";
            if (openFileDialog.ShowDialog() == true)
            {
                EgsDoseObject openedObject = null;
                await Task.Run(async () =>
                {
                    try
                    {
                        openedObject = new EgsDoseObject(openFileDialog.FileName);
                    }
                    catch (Exception e)
                    {
                        Messenger.Default.Send(new NotificationMessage("Could not open file: " + e.Message));
                    }
                });
                if (openedObject != null)
                    Messenger.Default.Send(new RTDicomViewer.Message.RTObjectLoadedMessage<EgsDoseObject>(openedObject));
            }
        }

    }
}
