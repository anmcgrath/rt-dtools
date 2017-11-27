using DicomPanel.Core.Render.Blending;
using RT.Core.Geometry;
using RT.Core.Imaging.LUT;
using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Render
{
    public class RenderableImage
    {
        public IVoxelDataStructure Grid { get; set; }
        public string Name { get; set; }
        public string Units { get; set; }
        public float Scaling { get; set; }
        public ILUT LUT { get; set; }
        public BlendMode BlendMode { get; set; }
        public float Alpha { get; set; }
        public Rectd ScreenRect { get; set; }
    }
}
