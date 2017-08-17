using Dicom;
using RTData.IO.Loaders;
using RTData.Radiotherapy;
using RTData.Radiotherapy.DICOM;
using RTData.Radiotherapy.Dose;
using RTData.Radiotherapy.Imaging;
using RTData.Radiotherapy.Planning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTData.IO
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
