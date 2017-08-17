using System;
using System.Collections.Generic;
using System.Text;

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
