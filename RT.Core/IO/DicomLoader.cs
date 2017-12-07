using Dicom;
using RT.Core.IO.Loaders;
using RT.Core;
using RT.Core.DICOM;
using RT.Core.Dose;
using RT.Core.Imaging;
using RT.Core.Planning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Core.ROIs;

namespace RT.Core.IO
{
    public class DicomLoader
    {
        public static async Task<DicomDoseObject> LoadDicomDoseAsync(string[] fileNames, IProgress<double> progress)
        {
            var files = await getFilesAsync(fileNames);
            var loader = new DicomDoseLoader();
            var dose = new DicomDoseObject();
            loader.Load(files, dose, progress);
            return dose;
        }

        public static async Task<DicomDoseObject> LoadDicomDoseAsync(string fileName, IProgress<double> progress)
        {
            return await LoadDicomDoseAsync(new string[] { fileName }, progress);
        }

        public static async Task<EgsDoseObject> LoadEgsObjectAsync(string file, IProgress<double> progress)
        {
            var loader = new EgsDoseLoader();
            var dose = new EgsDoseObject();
            loader.Load(file, dose, progress);
            return dose;
        }

        public static async Task<DicomImageObject> LoadDicomImageAsync(string[] fileNames, IProgress<double> progress)
        {
            var files = await getFilesAsync(fileNames);
            var loader = new DicomImageLoader();
            var img = new DicomImageObject();
            loader.Load(files, img, progress);
            return img;
        }

        public static async Task<DicomImageObject> LoadDicomImageAsync(string fileName, IProgress<double> progress)
        {
            return await LoadDicomImageAsync(new string[] { fileName }, progress);
        }

        public static async Task<StructureSet> LoadStructureSetAsync(string[] fileNames, IProgress<double> progress)
        {
            var files = await getFilesAsync(fileNames);
            var loader = new ROILoader();
            var structureSet = new StructureSet();
            loader.Load(files, structureSet, progress);
            return structureSet;
        }

        public static async Task<DicomDoseObject> LoadStructureSetAsync(string fileName, IProgress<double> progress)
        {
            return await LoadDicomDoseAsync(new string[] { fileName }, progress);
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
