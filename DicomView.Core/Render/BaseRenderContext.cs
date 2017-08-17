using DicomPanel.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Render
{
    public class BaseRenderContext
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Recti ScreenRect { get { return new Recti(0, 0, Width, Height); } }
    }
}
