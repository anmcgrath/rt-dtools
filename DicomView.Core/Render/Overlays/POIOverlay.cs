using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;
using RT.Core.DICOM;

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
            Render(model.Camera, context);
        }
        public void Render(Camera camera, IRenderContext context)
        {
            var dist = (camera.ConvertScreenToWorldCoords(camera.ConvertWorldToScreenCoords(Position)) - Position).Length();

            if (dist != 0)
                Fraction = (SizeInMM - Math.Abs(dist/2)) / SizeInMM;
            if (Fraction < 0.2)
                Fraction = 0;
            if (Fraction > 1)
                Fraction = 1;

            double scale = 1;
            if (KeepSameSizeOnScreen)
            {
                scale = camera.Scale;
            }
            var w = SizeInMM * Fraction / scale;

                var p1 = camera.ConvertWorldToScreenCoords(Position - camera.ColDir * w);
                var p2 = camera.ConvertWorldToScreenCoords(Position + camera.ColDir * w);
                var p3 = camera.ConvertWorldToScreenCoords(Position - camera.RowDir * w);
                var p4 = camera.ConvertWorldToScreenCoords(Position + camera.RowDir * w);

            context.DrawLine(p1.X, p1.Y, p2.X, p2.Y, DicomColors.Red, LineType.Normal);
            context.DrawLine(p3.X, p3.Y, p4.X, p4.Y, DicomColors.Red, LineType.Normal);

            if(RenderCircle)
            {
                var center = camera.ConvertWorldToScreenCoords(Position);
                var wx = (2 * Fraction * SizeInMM / camera.GetFOV().X);
                var wy = (2 * Fraction * SizeInMM / camera.GetFOV().Y);
                if(!KeepSameSizeOnScreen)
                {
                    wx *= camera.Scale;
                    wy *= camera.Scale;
                }
                context.DrawEllipse(center.X, center.Y, wx,wy, DicomColors.Red);
            }
            //}
        }
    }
}
