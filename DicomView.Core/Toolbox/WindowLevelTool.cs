using System;
using System.Collections.Generic;
using System.Text;
using RT.Core.Utilities.RTMath;
using RT.Core.Imaging.LUT;

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
                model.SecondaryImage.LUT = new HeatLUT();
                model.SecondaryImage.LUT.Window = window;
                model.SecondaryImage.LUT.Level = level;
                model.Invalidate();

                foreach(DicomPanelModel orthoModel in model.OrthogonalModels)
                {
                    orthoModel.Invalidate();
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
