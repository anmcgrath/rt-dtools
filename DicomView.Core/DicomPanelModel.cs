using DicomPanel.Core.Radiotherapy.Dose;
using DicomPanel.Core.Radiotherapy.Imaging;
using DicomPanel.Core.Render;
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

        public DicomImageObject Image { get; set; }
        public IDoseObject Dose { get; set; }

        public DicomPanelModel()
        {
            Camera = new Camera();
            ImageRenderer = new ImageRenderer();
            DoseRenderer = new DoseRenderer();
        }

        /// <summary>
        /// Invalidates the View model, causing rendering of all rendering contexts
        /// </summary>
        public void Invalidate()
        {
            if(ImageRenderContext != null)
                ImageRenderer?.Render(Image, Camera, ImageRenderContext, new Utilities.RTMath.Recti(0, 0, ImageRenderContext.Width, ImageRenderContext.Height));
            if (DoseRenderContext != null)
                DoseRenderer?.Render(Dose, Camera, ImageRenderContext, new Utilities.RTMath.Recti(0, 0, DoseRenderContext.Width, DoseRenderContext.Height));

            ImageRenderContext.DrawRect(0, 0, ImageRenderContext.Width, ImageRenderContext.Height, DicomColor.FromRgb(0, 255, 0));
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
    }
}
