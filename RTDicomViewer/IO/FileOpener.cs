using DicomPanel.Core.IO;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
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
        public static async void BeginOpenAsync<T>(bool multipleFiles, string dialogTitle)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = multipleFiles;
            openFileDialog.Title = dialogTitle;
            
            if(openFileDialog.ShowDialog() == true)
            {
                T openedObject = default(T);
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
            }
        }
    }
}
