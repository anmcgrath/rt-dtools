using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Toolbox
{
    public class ToolBase
    {
        public bool IsActive { get; set; }

        public virtual void Select()
        {
            IsActive = true;
        }

        public virtual void Unselect()
        {
            IsActive = false;
        }
    }
}
