using DicomPanel.Core;
using DicomPanel.Core.Radiotherapy.Dose;
using DicomPanel.Core.Radiotherapy.Imaging;
using DicomPanel.Core.Radiotherapy.ROIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Workspace
{
    public class Workspace
    {
        public static Workspace Current { get { if (_current == null) _current = new Workspace(); return _current; } }
        private static Workspace _current;

        public DicomPanelModel Axial { get; set; }
        public DicomPanelModel Coronal { get; set; }
        public DicomPanelModel Sagittal { get; set; }

        public WorkspaceItemCollection<IDoseObject> Doses { get; set; }
        public WorkspaceItemCollection<DicomImageObject> Images { get; set; }
        public WorkspaceItemCollection<StructureSet> StructureSets { get; set; }

        public Workspace()
        {
            Doses = new WorkspaceItemCollection<IDoseObject>();
            Images = new WorkspaceItemCollection<DicomImageObject>();
            StructureSets = new WorkspaceItemCollection<StructureSet>();
        }
    }
}
