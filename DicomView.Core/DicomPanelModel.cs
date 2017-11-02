using DicomPanel.Core.Radiotherapy.Dose;
using DicomPanel.Core.Radiotherapy.Imaging;
using DicomPanel.Core.Radiotherapy.ROIs;
using DicomPanel.Core.Render;
using DicomPanel.Core.Render.Overlays;
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

        public List<IOverlay> Overlays { get; set; }
        public List<DicomPanelModel> OrthogonalModels { get; set; }

        public ToolBox ToolBox { get; set; }

        public DicomPanelModel()
        {
            Camera = new Camera();
            ImageRenderer = new ImageRenderer();
            DoseRenderer = new DoseRenderer();
            ROIRenderer = new ROIRenderer();
            ROIs = new List<RegionOfInterest>();
            OrthogonalModels = new List<DicomPanelModel>();
            Overlays = new List<IOverlay>();
            ToolBox = new ToolBox();
        }

        /// <summary>
        /// Invalidates the View model, causing rendering of all rendering contexts
        /// </summary>
        public void Invalidate()
        {
            ImageRenderContext?.BeginRender();
            DoseRenderContext?.BeginRender();
            RoiRenderContext?.BeginRender();
            OverlayContext?.BeginRender();

            if (ImageRenderContext != null)
                ImageRenderer?.Render(Image, Camera, ImageRenderContext, new Utilities.RTMath.Rectd(0, 0, 1, 1));

            if (DoseRenderContext != null)
                DoseRenderer?.Render(Dose, Camera, DoseRenderContext, new Utilities.RTMath.Rectd(0, 0, 1, 1));

            if (RoiRenderContext != null)
                ROIRenderer?.Render(ROIs, Camera, ImageRenderContext, new Utilities.RTMath.Rectd(0, 0, 1, 1));

            if (OverlayContext != null)
            {
                foreach (IOverlay overlay in Overlays)
                {
                    overlay.Render(this, OverlayContext);
                }
            }

            ImageRenderContext?.EndRender();
            DoseRenderContext?.EndRender();
            RoiRenderContext?.EndRender();
            OverlayContext?.EndRender();

        }

        public void InvalidateImage()
        {
            ImageRenderContext?.BeginRender();
            if (ImageRenderContext != null)
                ImageRenderer?.Render(Image, Camera, ImageRenderContext, new Utilities.RTMath.Rectd(0, 0, 1, 1));
            ImageRenderContext?.EndRender();
        }

        public void InvalidateOverlays()
        {
            OverlayContext.BeginRender();
            foreach (IOverlay overlay in Overlays)
                overlay.Render(this, OverlayContext);
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

        public void SetToolBox(ToolBox toolBox)
        {
            ToolBox = toolBox;
        }
    }
}
