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
        DicomPanelModel Model { get; set; }
        bool IsActive { get; set; }
        void HandleMouseDown(Point3d worldPoint);
        void HandleMouseUp(Point3d worldPoint);
        void HandleMouseLeave(Point3d worldPoint);
        void HandleMouseScroll(Point3d worldPoint);
        void HandleToolCancel(Point3d worldPoint);
        void HandleMouseMove(Point3d worldPoint);
        void Select();
        void Unselect();
    }
}
