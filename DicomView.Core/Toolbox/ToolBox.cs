using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Toolbox
{
    public class ToolBox
    {
        public ITool SelectedTool { get; set; }
        public List<ITool> Tools { get; set; }

        public ToolBox()
        {
            Tools = new List<ITool>();
            initTools();
        }

        private void initTools()
        {
            Tools = new List<ITool>()
            {
                new PanTool(),
                new ZoomTool(),
                new WindowLevelTool(),
                new PointInfoTool(),
                new RotateTool(),
            };
            SelectedTool = Tools[0];
        }

        public void SelectTool(ITool tool)
        {
            SelectedTool?.Unselect();
            SelectedTool = tool;
            SelectedTool.Select();
        }

        public void SelectTool(string toolId)
        {
            foreach (ITool tool in Tools)
            {
                if (tool.Id == toolId)
                    SelectTool(tool);
            }
        }
    }
}
