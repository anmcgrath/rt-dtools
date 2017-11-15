using System;
using System.Collections.Generic;
using System.Text;
using RT.Core.DICOM;

namespace DicomPanel.Core.Render.Contouring
{
    public class ContourInfo
    {
        public DicomColor Color { get; set; }
        public float Threshold { get; set; }

        public ContourInfo(DicomColor color, float threshold)
        {
            Color = color;
            Threshold = threshold;
        }
    }
}
