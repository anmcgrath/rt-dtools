using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;
using RT.Core.DICOM;

namespace DicomPanel.Core.Render.Overlays
{
    public class ScaleOverlay : IOverlay
    {
        public void Render(DicomPanelModel model, IRenderContext context)
        {
            var p1_screen = new Point2d(.05, .95);
            var p1_world = model.Camera.ConvertScreenToWorldCoords(p1_screen);
            var col_dir_norm = model.Camera.ColDir / model.Camera.ColDir.Length();
            var p2_world = p1_world + col_dir_norm * 10;

            var p2_screen = model.Camera.ConvertWorldToScreenCoords(p2_world);
            try
            {
                context.DrawLine(p1_screen.X, p1_screen.Y, p2_screen.X, p2_screen.Y, DicomColors.Yellow);
                Point2d p_scr = new Point2d();
                for(int i = 0; i < 11; i++)
                {
                    p_scr = model.Camera.ConvertWorldToScreenCoords(p1_world + col_dir_norm * i);
                    context.DrawLine(p_scr.X, p1_screen.Y, p_scr.X, p_scr.Y - .02,DicomColors.Yellow);
                }
                context.DrawString("10 mm", p_scr.X + .02, p_scr.Y - .03, 10, DicomColors.Yellow);
            }catch(Exception e)
            {

            }
        }
    }
}
