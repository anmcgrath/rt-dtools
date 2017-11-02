using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Render.Overlays
{
    public interface IOverlay
    {
        void Render(DicomPanelModel model, IRenderContext context);
    }
}
