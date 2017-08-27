using DicomPanel.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core
{
    public partial class DicomPanelModel
    {
        public bool mouseDown = false;
		public void OnMouseScroll(Point3d worldPoint, double delta)
        {
            if (delta > 0)
                Camera.Move(Camera.Normal * 2);
            else
                Camera.Move(Camera.Normal * -2);

            Invalidate();
        }

        public void OnMouseDown(Point3d worldPoint)
        {
            mouseDown = true;
            initPoint = worldPoint;
        }

        private Point3d initPoint = new Point3d();
        public void OnMouseMove(Point3d worldPoint)
        {
            if(mouseDown)
            {
                var wp = Camera.ConvertWorldToScreenCoords(worldPoint);

                var diff = worldPoint - initPoint;
                Camera.Move(-diff.X, -diff.Y, -diff.Z);
                
                Invalidate();
                ImageRenderContext.DrawString(wp.X + ", " + wp.Y, 0, 0.5, 12);
            }
        }

        public void OnMouseExit(Point3d worldPoint)
        {
            mouseDown = false;
        }

        public void OnMouseEnter(Point3d worldPoint)
        {
            
        }
        
        public void OnMouseUp(Point3d worldPoint)
        {
            mouseDown = false;
        }
        
        public void OnResize(double width, double height)
        {
            Camera.SetFOV(width * Camera.MMPerPixel, height * Camera.MMPerPixel);
        }
    }
}
