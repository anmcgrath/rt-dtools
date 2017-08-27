using System;
using System.Collections.Generic;
using System.Text;
using DicomPanel.Core.Utilities.RTMath;

namespace DicomPanel.Core.Toolbox
{
    public class PanTool : ToolBase, ITool
    {
        public string Name { get => "Pan Tool"; }
        public string Id { get => "pan"; }
        public DicomPanelModel Model { get; set; }

        public PanTool(DicomPanelModel model)
        {
            this.Model = model; 
        }

        private bool mouseDown = false;
        private Point3d initPoint = new Point3d();

        public void HandleMouseDown(Point3d worldPoint)
        {
            mouseDown = true;
            initPoint = worldPoint;
        }

        public void HandleMouseLeave(Point3d worldPoint)
        {
            mouseDown = false;
        }

        public void HandleMouseScroll(Point3d worldPoint)
        {
            
        }

        public void HandleMouseUp(Point3d worldPoint)
        {
            mouseDown = false;
        }

        public void HandleToolCancel(Point3d worldPoint)
        {
            mouseDown = false;
        }

        public void HandleMouseMove(Point3d worldPoint)
        {
            if(mouseDown)
            {
                var diff = initPoint - worldPoint;
                Model.Camera.Move(diff);
                Model.Invalidate();
            }
        }
    }
}
