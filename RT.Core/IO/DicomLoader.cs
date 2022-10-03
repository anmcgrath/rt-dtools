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

        public static DicomDoseObject LoadDicomDoseSync(DicomFile dicomFile)
        {
            var loader = new DicomDoseLoader();
            var dose = new DicomDoseObject();
            loader.Load(dicomFile, dose, null);
            return dose;
        }

        public static async Task<DicomPlanObject> LoadDicomPlanAsync(string[] fileNames)
        {
            var files = await getFilesAsync(fileNames);
            var loader = new PlanLoader();
            var plan = new DicomPlanObject();
            loader.Load(files, plan);
            return plan;
        }

        public static DicomPlanObject LoadDicomPlanAsync(DicomFile dicomFile)
        {
            var loader = new PlanLoader();
            var plan = new DicomPlanObject();
            loader.Load(dicomFile, plan);
            return plan;
        }
        

        public static async Task<DicomPlanObject> LoadDicomPlanAsync(string fileName)
        {
            return await LoadDicomPlanAsync(new string[] { fileName });
        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public static async Task<EgsDoseObject> LoadEgsObjectAsync(string file, IProgress<double> progress)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
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

        public static StructureSet LoadStructureSetSync(DicomFile dicomFile, IProgress<double> progress)
        {
            var loader = new ROILoader();
            var structureSet = new StructureSet();
            loader.Load(dicomFile, structureSet, progress);
            return structureSet;
        }

        public static async Task<StructureSet> LoadStructureSetAsync(string fileName, IProgress<double> progress)
        {
            return await LoadStructureSetAsync(new string[] { fileName }, progress);
        }

        private static async Task<DicomFile[]> getFilesAsync(string[] fileNames)
        {
            List<DicomFile> files = new List<DicomFile>();
            Stack<string> validDicomFiles = new Stack<string>();
            foreach(string fileName in fileNames)
            {
                if (DicomFile.HasValidHeader(fileName))
                {
                    files.Add(await DicomFile.OpenAsync(fileName));
                }
                else if(DicomFile.IsDICOMFile(fileName))
                {
                    files.Add(await DicomFile.OpenAsync(fileName));
                }                    
            }
            return files.ToArray();
        }
    }
}
