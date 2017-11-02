using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core
{
    public class OrthogonalDicomPanelGroup
    {
        public DicomPanelModel Transverse { get; set; }
        public DicomPanelModel Sagittal { get; set; }
        public DicomPanelModel Coronal { get; set; }
        public OrthogonalDicomPanelGroup(DicomPanelModel transverse, DicomPanelModel sagittal, DicomPanelModel coronal)
        {
            Transverse = transverse;
            Sagittal = sagittal;
            Coronal = coronal;
        }
    }
}
