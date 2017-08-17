using Dicom;
using DicomPanel.Core.IO.Loaders;
using DicomPanel.Core.Radiotherapy;
using DicomPanel.Core.Radiotherapy.DICOM;
using DicomPanel.Core.Radiotherapy.Dose;
using DicomPanel.Core.Radiotherapy.Imaging;
using DicomPanel.Core.Radiotherapy.Planning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomPanel.Core.IO
{
    public class DicomLoader
    {
        public static async Task<T> LoadAsync<T>(string[] fileNames)
        {
            var files = await getFilesAsync(fileNames);
            return (T)Activator.CreateInstance(typeof(T), files);
        }

        public static async Task<T> LoadAsync<T>(string fileName)
        {
            return await LoadAsync<T>(new string[] { fileName });
        }

        private static async Task<DicomFile[]> getFilesAsync(string[] fileNames)
        {
            List<DicomFile> files = new List<DicomFile>();
            Stack<string> validDicomFiles = new Stack<string>();
            foreach(string fileName in fileNames)
            {
                if (DicomFile.HasValidHeader(fileName))
                    files.Add(await DicomFile.OpenAsync(fileName));
            }
            return files.ToArray();
        }
    }
}
