using System;
using System.Collections.Generic;
using System.Text;
using RT.Core.Utilities.RTMath;

namespace DicomPanel.Core.Toolbox
{
    public class RotateTool : ToolBase, ITool
    {
        public string Name => "Rotate";
        public string Id => "rotate";
        private bool mouseDown;
        private double initAngle;

        public void HandleMouseDown(DicomPanelModel model, Point3d worldPoint)
        {
            mouseDown = true;
            var sp = model.Camera.ConvertWorldToScreenCoords(worldPoint);
            initAngle = Math.Atan2(sp.Y - 0.5, sp.X - 0.5);
        }

        public void HandleMouseLeave(DicomPanelModel model, Point3d worldPoint)
        {
            mouseDown = false;
        }

        public void HandleMouseMove(DicomPanelModel model, Point3d worldPoint)
        {
            if(mouseDown)
            {
                var sp = model.Camera.ConvertWorldToScreenCoords(worldPoint);
                var angle = Math.Atan2(sp.Y - 0.5, sp.X - 0.5);
                var dtheta = -(angle - initAngle);
                var d_dtheta = dtheta * 180 / Math.PI; //theta in degrees

                model.Camera.Rotate(d_dtheta, model.Camera.Normal);
                model.Invalidate();

                foreach(DicomPanelModel orthogonalModel in model.OrthogonalModels)
                {
                    orthogonalModel.Camera.Rotate(d_dtheta, model.Camera.Normal);
                    orthogonalModel.Invalidate();
                }

                initAngle = angle;

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
