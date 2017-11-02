using DicomPanel.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Render.Overlays
{
    public class PointInfoOverlay:IOverlay
    {
        public Point3d Position { get; set; }
        private POIOverlay poiOverlay = new POIOverlay();
        private TextOverlay textOverlay = new TextOverlay("");
        public PointInfoOverlay()
        {
            Position = new Point3d();
            poiOverlay = new POIOverlay();
            poiOverlay.KeepSameSize = true;
        }

        public void Render(DicomPanelModel model, IRenderContext context)
        {
            poiOverlay.Position = Position;
            Position.CopyTo(textOverlay.Position);
            textOverlay.Position = Position - (model.Camera.RowDir * 25 / model.Camera.Scale);
            var HU = model?.Image?.Grid?.Interpolate(Position);

            if(HU != null)
                textOverlay.Text = Math.Round(HU.Value) + " HU"; 

            poiOverlay.Render(model, context);
            textOverlay.Render(model, context);
        }
    }
}
