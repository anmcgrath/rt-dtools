using System;
using System.Collections.Generic;
using System.Text;
using DicomPanel.Core.Utilities.RTMath;

namespace DicomPanel.Core.Toolbox
{
    public class WindowLevelTool : ToolBase, ITool
    {
        public string Name => "Window/Level";
        public string Id => "windowlevel";
        private bool mouseDown;

        public void HandleMouseDown(DicomPanelModel model, Point3d worldPoint)
        {
            mouseDown = true;
        }

        public void HandleMouseLeave(DicomPanelModel model, Point3d worldPoint)
        {
            mouseDown = false;
        }

        public void HandleMouseMove(DicomPanelModel model, Point3d worldPoint)
        {
            var screenPoint = model.Camera.ConvertWorldToScreenCoords(worldPoint);
            if(mouseDown && model?.Image?.Grid?.MaxVoxel != null)
            {
                int window = (int)(screenPoint.X * (model.Image.Grid.MaxVoxel.Value+1000))-1000;
                int level = (int)(screenPoint.Y * (model.Image.Grid.MaxVoxel.Value+1000))-1000;
                model.Image.Window = window;
                model.Image.Level = level;
                model.InvalidateImage();

                foreach(DicomPanelModel orthoModel in model.OrthogonalModels)
                {
                    orthoModel.InvalidateImage();
                }
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
