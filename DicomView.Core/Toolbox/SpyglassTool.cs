using System;
using System.Collections.Generic;
using System.Text;
using RT.Core.Utilities.RTMath;

namespace DicomPanel.Core.Toolbox
{
    public class SpyglassTool:ToolBase,ITool
    {

        public string Name => "Spyglass";

        public string Id => "spyglass";

        public SpyglassTool()
        {
            IsActivatable = true;
        }

        public void HandleMouseDown(DicomPanelModel model, Point3d worldPoint)
        {
            
        }

        public void HandleMouseLeave(DicomPanelModel model, Point3d worldPoint)
        {
            model.UseSpyGlass = false;
        }

        public void HandleMouseMove(DicomPanelModel model, Point3d worldPoint)
        {
            model.UseSpyGlass = true;
            var scrn = model.Camera.ConvertWorldToScreenCoords(worldPoint);
            var width = .25;
            var height = .25;
            model.SpyGlass.X = scrn.X - width / 2;
            model.SpyGlass.Y = scrn.Y - height / 2;
            model.SpyGlass.Width = width;
            model.SpyGlass.Height = height;
        }

        public void HandleMouseScroll(DicomPanelModel model, Point3d worldPoint)
        {
            
        }

        public void HandleMouseUp(DicomPanelModel model, Point3d worldPoint)
        {
            
        }

        public void HandleToolCancel(DicomPanelModel model, Point3d worldPoint)
        {
            
        }
    }
}
