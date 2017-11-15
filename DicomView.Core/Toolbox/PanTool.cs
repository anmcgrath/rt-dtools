using System;
using System.Collections.Generic;
using System.Text;
using RT.Core.Utilities.RTMath;

namespace DicomPanel.Core.Toolbox
{
    public class PanTool : ToolBase, ITool
    {
        public string Name { get => "Pan Tool"; }
        public string Id { get => "pan"; }

        private bool mouseDown = false;
        private Point3d initPoint = new Point3d();

        public PanTool() { }

        public void HandleMouseDown(DicomPanelModel model, Point3d worldPoint)
        {
            mouseDown = true;
            initPoint = worldPoint;
        }

        public void HandleMouseLeave(DicomPanelModel model, Point3d worldPoint)
        {
            mouseDown = false;
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

        public void HandleMouseMove(DicomPanelModel model, Point3d worldPoint)
        {
            if(mouseDown)
            {
                var diff = initPoint - worldPoint;
                model.Camera.Move(diff);
                model.Invalidate();
            }
        }
    }
}
