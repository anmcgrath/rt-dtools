using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Toolbox
{
    public class ToolSelectedEventArgs
    {
        public ITool OldTool { get; set; }
        public ITool NewTool { get; set; }
        public ToolSelectedEventArgs(ITool oldTool, ITool newTool)
        {
            OldTool = oldTool;
            NewTool = newTool;
        }
    }
}
