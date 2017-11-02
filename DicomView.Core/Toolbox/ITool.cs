using DicomPanel.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Toolbox
{
    public interface ITool
    {
        string Name { get; }
        string Id { get; }
        bool IsActive { get; set; }
        void HandleMouseDown(DicomPanelModel model, Point3d worldPoint);
        void HandleMouseUp(DicomPanelModel model, Point3d worldPoint);
        void HandleMouseLeave(DicomPanelModel model, Point3d worldPoint);
        void HandleMouseScroll(DicomPanelModel model, Point3d worldPoint);
        void HandleToolCancel(DicomPanelModel model, Point3d worldPoint);
        void HandleMouseMove(DicomPanelModel model, Point3d worldPoint);
        void Select();
        void Unselect();
    }
}
