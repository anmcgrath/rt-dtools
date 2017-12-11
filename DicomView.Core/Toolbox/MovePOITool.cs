using System;
using System.Collections.Generic;
using System.Text;
using RT.Core.Utilities.RTMath;
using RT.Core.Planning;

namespace DicomPanel.Core.Toolbox
{
    public class MovePOITool : ToolBase, ITool
    {
        public string Name => "Move POI";

        public string Id => "movepoi";

        public PointOfInterest Target { get; set; }
        private bool mouseDown = false;

        public void HandleMouseDown(DicomPanelModel model, Point3d worldPoint)
        {
            mouseDown = true;
            MoveTarget(model, worldPoint);
        }

        private void MoveTarget(DicomPanelModel model, Point3d worldPoint)
        {
            if (Target != null)
            {
                Target.Position = worldPoint;
                model.Invalidate();
                foreach (var orthogModel in model.OrthogonalModels)
                {
                    orthogModel.Invalidate();
                }
            }
        }

        public void HandleMouseLeave(DicomPanelModel model, Point3d worldPoint)
        {
            mouseDown = false;
        }

        public void HandleMouseMove(DicomPanelModel model, Point3d worldPoint)
        {
            if (mouseDown)
                MoveTarget(model, worldPoint);
        }

        public void HandleMouseScroll(DicomPanelModel model, Point3d worldPoint)
        {
            if (mouseDown)
                MoveTarget(model, worldPoint);
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
