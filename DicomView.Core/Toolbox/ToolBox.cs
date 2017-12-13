using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Toolbox
{
    public class ToolBox
    {
        public ITool SelectedTool { get; set; }
        public List<ITool> Tools { get; set; }
        public delegate void EventHandler(object sender, ToolSelectedEventArgs args);
        public event EventHandler ToolSelected;

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
            ITool oldTool = null;
            ITool newTool = null;

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
                oldTool = SelectedTool;

                SelectedTool?.Unselect();
                SelectedTool = tool;
                SelectedTool.Select();

                newTool = SelectedTool;
            }

            ToolSelected?.Invoke(this, new ToolSelectedEventArgs(oldTool, newTool));
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
