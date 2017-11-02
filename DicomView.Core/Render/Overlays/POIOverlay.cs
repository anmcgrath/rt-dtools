using DicomPanel.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Render.Overlays
{
    public class POIOverlay : IOverlay
    {
        public Point3d Position { get; set; }
        public bool KeepSameSize { get; set; }
        public void Render(DicomPanelModel model, IRenderContext context)
        {

            //var dist = (model.Camera.ConvertScreenToWorldCoords(model.Camera.ConvertWorldToScreenCoords(Position)) - Position).Length();

            //if (dist < 5)
            //{
            //var frac = dist / 5;
            double scale = 1;
            if (KeepSameSize)
                scale = model.Camera.Scale;
            var w = 5 / scale;
                var p1 = model.Camera.ConvertWorldToScreenCoords(Position - model.Camera.ColDir * w);
                var p2 = model.Camera.ConvertWorldToScreenCoords(Position + model.Camera.ColDir * w);
                var p3 = model.Camera.ConvertWorldToScreenCoords(Position - model.Camera.RowDir * w);
                var p4 = model.Camera.ConvertWorldToScreenCoords(Position + model.Camera.RowDir * w);

                context.DrawLine(p1.X, p1.Y, p2.X, p2.Y, DicomColor.FromRgb(255, 0, 0));
                context.DrawLine(p3.X, p3.Y, p4.X, p4.Y, DicomColor.FromRgb(255, 0, 0));
            //}
        }
    }
}
