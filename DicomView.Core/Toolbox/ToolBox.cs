using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Toolbox
{
    public class ToolBox
    {
        public ITool SelectedTool { get; set; }
        public List<ITool> Tools { get; set; }
        /// <summary>
        /// Activated tools are activated alongside selected tools.
        /// </summary>
        public List<ITool> ActivatedTools { get; set; }

        public ToolBox()
        {
            Tools = new List<ITool>();
            ActivatedTools = new List<ITool>();
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
                new SpyglassTool(),
                new MovePOITool(),
            };
            SelectedTool = Tools[0];
        }

        public void SelectTool(ITool tool)
        {
            if (tool.IsActivatable)
            {
                if (ActivatedTools.Contains(tool))
                    ActivatedTools.Remove(tool);
                else
                    ActivatedTools.Add(tool);

                tool.IsActivated = ActivatedTools.Contains(tool);
            }
            else
            {
                SelectedTool?.Unselect();
                 SelectedTool = tool;
                SelectedTool.Select();
            }
        }

        public void SelectTool(string toolId)
        {
            ITool tool;
            if ((tool = GetTool(toolId)) != null)
                SelectTool(tool);
        }

        public ITool GetTool(string toolId)
        {
            foreach (ITool tool in Tools)
            {
                if (tool.Id == toolId)
                    return tool;
            }
            return null;
        }
    }
}
