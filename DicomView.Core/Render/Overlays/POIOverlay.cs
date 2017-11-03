using DicomPanel.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Render.Overlays
{
    public class POIOverlay : IOverlay
    {
        public Point3d Position { get; set; }
        public bool KeepSameSizeOnScreen { get; set; }
        public bool RenderCircle { get; set; }
        public double SizeInMM { get; set; } = 5;
        /// <summary>
        /// Fraction of total size. this gets updated as the distance from the point changes.
        /// </summary>
        public double Fraction { get; set; } = 1;
        public void Render(DicomPanelModel model, IRenderContext context)
        {
            var dist = (model.Camera.ConvertScreenToWorldCoords(model.Camera.ConvertWorldToScreenCoords(Position)) - Position).Length();

            if (dist != 0)
                Fraction = (SizeInMM - Math.Abs(dist/2)) / SizeInMM;
            if (Fraction < 0.2)
                Fraction = 0;
            if (Fraction > 1)
                Fraction = 1;

            double scale = 1;
            if (KeepSameSizeOnScreen)
            {
                scale = model.Camera.Scale;
            }
            var w = SizeInMM * Fraction / scale;

                var p1 = model.Camera.ConvertWorldToScreenCoords(Position - model.Camera.ColDir * w);
                var p2 = model.Camera.ConvertWorldToScreenCoords(Position + model.Camera.ColDir * w);
                var p3 = model.Camera.ConvertWorldToScreenCoords(Position - model.Camera.RowDir * w);
                var p4 = model.Camera.ConvertWorldToScreenCoords(Position + model.Camera.RowDir * w);

            context.DrawLine(p1.X, p1.Y, p2.X, p2.Y, DicomColors.Red);
            context.DrawLine(p3.X, p3.Y, p4.X, p4.Y, DicomColors.Red);

            if(RenderCircle)
            {
                var center = model.Camera.ConvertWorldToScreenCoords(Position);
                var wx = (2 * Fraction * SizeInMM / model.Camera.GetFOV().X);
                var wy = (2 * Fraction * SizeInMM / model.Camera.GetFOV().Y);
                if(!KeepSameSizeOnScreen)
                {
                    wx *= model.Camera.Scale;
                    wy *= model.Camera.Scale;
                }
                context.DrawEllipse(center.X, center.Y, wx,wy, DicomColors.Red);
            }
            //}
        }
    }
}
