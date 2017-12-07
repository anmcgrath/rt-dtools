using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.IO
{
    public interface IFileOpener
    {
        void BeginOpenDicomDoseAsync();
        void BeginOpenEgsDoseAsync();
        void BeginOpenStructuresAsync();
        void BeginOpenImagesAsync();
        void BeginOpenDicomPlanAsync();
    }
}
