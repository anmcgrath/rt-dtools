using DicomPanel.Core.Event;
using DicomPanel.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core
{
    public partial class DicomPanelModel
    {
		public void OnMouseScroll(Point3d worldPoint, double delta)
        {
            if (delta > 0)
                Camera.Move(Camera.Normal * 2);
            else
                Camera.Move(Camera.Normal * -2);

            Invalidate();
            ToolBox?.SelectedTool?.HandleMouseScroll(this, worldPoint);
        }

        public void OnMouseDown(Point3d worldPoint)
        {
            ToolBox?.SelectedTool?.HandleMouseDown(this, worldPoint);
        }

        public void OnMouseMove(Point3d worldPoint)
        {
            ToolBox?.SelectedTool?.HandleMouseMove(this, worldPoint);
        }

        public void OnMouseExit(Point3d worldPoint)
        {
            ToolBox?.SelectedTool?.HandleMouseLeave(this, worldPoint);
        }

        public void OnMouseEnter(Point3d worldPoint)
        {
            
        }
        
        public void OnMouseUp(Point3d worldPoint)
        {
            ToolBox?.SelectedTool?.HandleMouseUp(this, worldPoint);
        }
        
        public void OnResize(double width, double height)
        {
            Camera.SetFOV(width * Camera.MMPerPixel, height * Camera.MMPerPixel);
        }
    }
}
