using System;
using System.Collections.Generic;
using System.Text;
using RT.Core.Utilities.RTMath;
using DicomPanel.Core.Render.Overlays;

namespace DicomPanel.Core.Toolbox
{
    public class PointInfoTool : ToolBase, ITool
    {
        public string Name => "Point Info";
        public string Id => "pointinfo";
        private bool mouseDown;
        private PointInfoOverlay pointInfoOverlay;
        private List<DicomPanelModel> activeModels { get; set; } = new List<DicomPanelModel>();

        public void HandleMouseDown(DicomPanelModel model, Point3d worldPoint)
        {
            if (!activeModels.Contains(model))
                activeModels.Add(model);

            mouseDown = true;
            if (pointInfoOverlay != null)
            {
                foreach (DicomPanelModel m in activeModels)
                {
                    if (m.Overlays.Contains(pointInfoOverlay))
                    {
                        m.Overlays.Remove(pointInfoOverlay);
                        m.InvalidateOverlays();
                    }
                }
            }

            if (pointInfoOverlay == null)
            {
                pointInfoOverlay = new PointInfoOverlay();
                
            }
            model.Overlays.Add(pointInfoOverlay);
            pointInfoOverlay.Position = worldPoint;
            model.InvalidateOverlays();
        }

        public void HandleMouseLeave(DicomPanelModel model, Point3d worldPoint)
        {
            mouseDown = false;
        }

        public void HandleMouseMove(DicomPanelModel model, Point3d worldPoint)
        {
            if (pointInfoOverlay != null && mouseDown)
            {
                pointInfoOverlay.Position = worldPoint;
                model.InvalidateOverlays();
            }

        }

        public void HandleMouseScroll(DicomPanelModel model, Point3d worldPoint)
        {
            if (pointInfoOverlay != null && mouseDown)
            {
                pointInfoOverlay.Position = worldPoint;
                model.InvalidateOverlays();
            }
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
