using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Toolbox
{
    public class ToolBase
    {
        public bool IsSelected { get; set; }
        public bool IsActivatable { get; set; }
        public bool IsActivated { get; set; }

        public virtual void Select()
        {
            IsSelected = true;
        }

        public virtual void Unselect()
        {
            IsSelected = false;
        }

        public virtual void Activate()
        {
            IsActivated = true;
        }

        public virtual void Deactivate()
        {
            IsActivated = false;
        }
    }
}
