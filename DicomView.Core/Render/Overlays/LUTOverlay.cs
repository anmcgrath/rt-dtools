using RT.Core.Geometry;
using RT.Core.Imaging.LUT;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Render.Overlays
{
    public class LUTOverlay : IOverlay
    {
        
        public LUTOverlay(ILUT lut, IVoxelDataStructure grid)
        {

        }

        public void Render(DicomPanelModel model, IRenderContext context)
        {
            throw new NotImplementedException();
        }
    }
}
