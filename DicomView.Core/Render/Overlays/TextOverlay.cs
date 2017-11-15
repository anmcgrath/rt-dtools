using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;
using RT.Core.DICOM;

namespace DicomPanel.Core.Render.Overlays
{
    public class TextOverlay:IOverlay
    {
        public string Text { get; set; }
        public Point3d Position { get; set; }
        /// <summary>
        /// Text opacity value from 0 to 1
        /// </summary>
        public double Opacity = 1;
        public TextOverlay(string text)
        {
            Position = new Point3d();
            Text = text;
        }

        public void Render(DicomPanelModel model, IRenderContext context)
        {
            var s = model.Camera.ConvertWorldToScreenCoords(Position);
            var r1 = new Point3d(Position.X - 2/model.Camera.Scale, Position.Y - 2/model.Camera.Scale, Position.Z);
            var r2 = new Point3d(Position.X, Position.Y, Position.Z);
            r2 += model.Camera.ColDir * 48 / model.Camera.Scale;
            r2 += model.Camera.RowDir * 16 / model.Camera.Scale;
            var sr1 = model.Camera.ConvertWorldToScreenCoords(r1);
            var sr2 = model.Camera.ConvertWorldToScreenCoords(r2);

            context.FillRect(sr1.X,sr1.Y,sr2.X,sr2.Y, DicomColor.FromArgb((int)(128 * Opacity), 0, 0, 0), DicomColor.FromArgb((int)(255*Opacity), 100, 100, 100));
            context.DrawString(Text, s.X, s.Y, 12, DicomColor.FromArgb((int)(255*Opacity), 0, 255, 255));
        }
    }
}
