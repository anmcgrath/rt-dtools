using DicomPanel.Core.Radiotherapy.Dose;
using DicomPanel.Core.Radiotherapy.Imaging;
using DicomPanel.Core.Radiotherapy.ROIs;
using DicomPanel.Core.Render;
using DicomPanel.Core.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core
{
    public partial class DicomPanelModel
    {
        public Camera Camera { get; set; }

        private IRenderContext ImageRenderContext { get; set; }
        private IRenderContext DoseRenderContext { get; set; }
        private IRenderContext RoiRenderContext { get; set; }
        private IRenderContext OverlayContext { get; set; }

        public ImageRenderer ImageRenderer { get; set; }
        public DoseRenderer DoseRenderer { get; set; }
        public ROIRenderer ROIRenderer { get; set; }

        public DicomImageObject Image { get; set; }
        public IDoseObject Dose { get; set; }
        private List<RegionOfInterest> ROIs { get; set; }

        public DicomPanelGroup Group { get; set; }

        public ITool SelectedTool { get; set; }

        public List<ITool> Tools { get; set; }

        public DicomPanelModel()
        {
            Camera = new Camera();
            ImageRenderer = new ImageRenderer();
            DoseRenderer = new DoseRenderer();
            ROIRenderer = new ROIRenderer();
            ROIs = new List<RegionOfInterest>();
        }



        /// <summary>
        /// Invalidates the View model, causing rendering of all rendering contexts
        /// </summary>
        public void Invalidate()
        {
            ImageRenderContext.BeginRender();
            DoseRenderContext.BeginRender();
            RoiRenderContext.BeginRender();
            OverlayContext.BeginRender();

            if (ImageRenderContext != null)
                ImageRenderer?.Render(Image, Camera, ImageRenderContext, new Utilities.RTMath.Rectd(0, 0, 1, 1));

            if (DoseRenderContext != null)
                DoseRenderer?.Render(Dose, Camera, OverlayContext, new Utilities.RTMath.Rectd(0, 0, 1, 1));

            if (RoiRenderContext != null)
                ROIRenderer?.Render(ROIs, Camera, ImageRenderContext, new Utilities.RTMath.Rectd(0, 0, 1, 1));

            OverlayContext.DrawRect(0, 0, 1.0, 1.0 , DicomColor.FromRgb(128, 0, 128));

            ImageRenderContext.EndRender();
            DoseRenderContext.EndRender();
            RoiRenderContext.EndRender();
            OverlayContext.EndRender();

        }

        public void SetImage(DicomImageObject image)
        {
            Image = image;
            ResetCameraToImageCentre();
            Invalidate();
        }

        public void SetDose(IDoseObject dose)
        {
            Dose = dose;
            Invalidate();
        }

        public void AddROIs(IEnumerable<RegionOfInterest> rois)
        {
            bool hasROIsToAdd = false;
            foreach(var roi in rois)
            {
                hasROIsToAdd = true;
                if (!ROIs.Contains(roi))
                    ROIs.Add(roi);
            }
            if(hasROIsToAdd)
                Invalidate();
        }

        public void RemoveROIs(IEnumerable<RegionOfInterest> rois)
        {
            bool hasROIsToRemove = false;
            foreach(var roi in rois)
            {
                hasROIsToRemove = true;
                if (ROIs.Contains(roi))
                    ROIs.Remove(roi);
            }
            if(hasROIsToRemove)
                Invalidate();
        }

        public void ResetCameraToImageCentre()
        {
            if(Image?.Grid != null)
                Camera.MoveTo(Image.Grid.XRange.GetCentre(), Image.Grid.YRange.GetCentre(), Image.Grid.ZRange.GetCentre());
        }

        public void SetImageRenderContext(IRenderContext context)
        {
            ImageRenderContext = context;
        }

        public void SetDoseRenderContext(IRenderContext context)
        {
            DoseRenderContext = context;
        }

        public void SetRoiRenderContext(IRenderContext context)
        {
            RoiRenderContext = context;
        }

        public void SetOverlayContext(IRenderContext context)
        {
            OverlayContext = context;
        }

        private void initTools()
        {
            Tools = new List<ITool>()
            {
                new PanTool(this),
            };
        }

        private void SelectTool(ITool tool)
        {
            SelectedTool?.Unselect();
            SelectedTool = tool;
            SelectedTool.Select();
        }

        private void SelectTool(string toolId)
        {
            foreach(ITool tool in Tools)
            {
                if (tool.Id == toolId)
                    SelectTool(tool);
            }
        }
    }
}
