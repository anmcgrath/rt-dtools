using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Toolbox
{
    public interface ITool
    {
        string Name { get; }
        string Id { get; }
        bool IsSelected { get; set; }
        bool IsActivatable { get; set; }
        bool IsActivated { get; set; }
        void HandleMouseDown(DicomPanelModel model, Point3d worldPoint);
        void HandleMouseUp(DicomPanelModel model, Point3d worldPoint);
        void HandleMouseLeave(DicomPanelModel model, Point3d worldPoint);
        void HandleMouseScroll(DicomPanelModel model, Point3d worldPoint);
        void HandleToolCancel(DicomPanelModel model, Point3d worldPoint);
        void HandleMouseMove(DicomPanelModel model, Point3d worldPoint);
        void Select();
        void Unselect();
        void Activate();
        void Deactivate();
    }
}
