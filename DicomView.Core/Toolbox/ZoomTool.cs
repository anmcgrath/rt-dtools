using System;
using System.Collections.Generic;
using System.Text;
using RT.Core.Utilities.RTMath;

namespace DicomPanel.Core.Toolbox
{
    public class ZoomTool : ToolBase, ITool
    {
        public string Name => "Zoom";
        public string Id => "zoom";
        private bool mouseDown = false;
        private Point2d initPosn;

        public void HandleMouseDown(DicomPanelModel model, Point3d worldPoint)
        {
            mouseDown = true;
            initPosn = model.Camera.ConvertWorldToScreenCoords(worldPoint);
        }

        public void HandleMouseLeave(DicomPanelModel model, Point3d worldPoint)
        {
            mouseDown = false;
        }

        public void HandleMouseMove(DicomPanelModel model, Point3d worldPoint)
        {
            if(mouseDown)
            {
                var newPoint = model.Camera.ConvertWorldToScreenCoords(worldPoint);
                var diff = newPoint - initPosn;

                double zoomInContribution = 0;
                double zoomOutContribution = 0;
                if (diff.Y > 0)
                    zoomInContribution += Math.Abs(diff.Y);
                else
                    zoomOutContribution += Math.Abs(diff.Y);

                if (diff.X < 0)
                    zoomInContribution += Math.Abs(diff.X);
                else
                    zoomOutContribution += Math.Abs(diff.X);

                if (zoomInContribution > zoomOutContribution)
                    model.Camera.Zoom(0.95);
                else
                    model.Camera.Zoom(1.05);

                initPosn = newPoint;
                model.Invalidate();
            }
        }

        public void HandleMouseScroll(DicomPanelModel model, Point3d worldPoint)
        {
            
        }

        public void HandleMouseUp(DicomPanelModel model, Point3d worldPoint)
        {
            mouseDown = false;
        }

        public void HandleToolCancel(DicomPanelModel model, Point3d worldPoint)
        {
            mouseDown = false;
        }
    }
}
