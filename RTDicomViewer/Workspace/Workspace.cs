using RT.Core;
using RT.Core.Dose;
using RT.Core.Imaging;
using RT.Core.Planning;
using RT.Core.ROIs;
using GalaSoft.MvvmLight.Messaging;
using RTDicomViewer.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DicomPanel.Core;
using DicomPanel.Core.Render.Contouring;
using RT.Core.DICOM;

namespace RTDicomViewer.Workspace
{
    public class Workspace
    {
        public static void Init() { _current = new Workspace(); }

        public static Workspace Current { get { if (_current == null) _current = new Workspace(); return _current; } }
        private static Workspace _current;

        public DicomPanelModel Axial { get; set; }
        public DicomPanelModel Coronal { get; set; }
        public DicomPanelModel Sagittal { get; set; }

        public List<ContourInfo> ContourInfo = new List<DicomPanel.Core.Render.Contouring.ContourInfo>()
            {
                new ContourInfo(DicomColors.Black,99),
                new ContourInfo(DicomColors.OrangeRed,90),
                new ContourInfo(DicomColors.Orange,80),
                new ContourInfo(DicomColors.Yellow,70),
                new ContourInfo(DicomColors.White,60),
                new ContourInfo(DicomColors.Green,50),
                new ContourInfo(DicomColors.Blue,40),
                new ContourInfo(DicomColors.LightBlue,30),
                new ContourInfo(DicomColors.LightSkyBlue,20),
            };

        public WorkspaceItemCollection<DicomDoseObject> Doses { get; set; }
        public WorkspaceItemCollection<DicomImageObject> Images { get; set; }
        public WorkspaceItemCollection<StructureSet> StructureSets { get; set; }
        public WorkspaceItemCollection<PointOfInterest> Points { get; set; }

        public Workspace()
        {
            Doses = new WorkspaceItemCollection<DicomDoseObject>();
            Images = new WorkspaceItemCollection<DicomImageObject>();
            StructureSets = new WorkspaceItemCollection<StructureSet>();
            Points = new WorkspaceItemCollection<PointOfInterest>();

            Messenger.Default.Register<RTObjectAddedMessage<DicomDoseObject>>(this, x => Doses.Add(x.Value,x.Value.Grid.Name));
            Messenger.Default.Register<RTObjectAddedMessage<PointOfInterest>>(this, 
                x => Points.Add(x.Value, x.Value.Name));
            Messenger.Default.Register<RTObjectAddedMessage<DicomImageObject>>(this, 
                x => Images.Add(x.Value, x.Value.SeriesUid));
            Messenger.Default.Register<RTObjectAddedMessage<StructureSet>>(this, x => StructureSets.Add(x.Value, x.Value.Name));

            Messenger.Default.Register<RTObjectDeletedMessage<DicomImageObject>>(this,
                x => Images.Remove(x.Value));
        }
    }
}
