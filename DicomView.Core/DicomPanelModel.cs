using RT.Core.Dose;
using RT.Core.Imaging;
using RT.Core.Planning;
using RT.Core.ROIs;
using DicomPanel.Core.Render;
using DicomPanel.Core.Render.Overlays;
using DicomPanel.Core.Toolbox;
using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;
using RT.Core.Imaging.LUT;
using RT.Core.Geometry;
using DicomPanel.Core.Render.Blending;
using RT.Core.DICOM;
using System.Diagnostics;

namespace DicomPanel.Core
{
    public partial class DicomPanelModel
    {
        public Camera Camera { get; set; }

        private IRenderContext ImageRenderContext { get; set; }
        private IRenderContext DoseRenderContext { get; set; }
        private IRenderContext RoiRenderContext { get; set; }
        private IRenderContext OverlayContext { get; set; }

        public bool UseSpyGlass = false;
        public Rectd SpyGlass { get; set; }

        public ImageRenderer ImageRenderer { get; set; }
        public DoseRenderer DoseRenderer { get; set; }
        public ROIRenderer ROIRenderer { get; set; }
        public BeamRenderer BeamRenderer { get; set; }

        public DicomImageObject PrimaryImage { get; set; }
        public DicomImageObject SecondaryImage { get; set; }
        public List<RenderableImage> AdditionalImages { get; set; }

        public List<IDoseObject> ContouredDoses { get; set; }
        
        private List<RegionOfInterest> ROIs { get; set; }
        private List<PointOfInterest> POIs { get; set; }
        private List<Beam> Beams { get; set; }

        public double PrimarySecondaryImageSplitLocation = 0.5;

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
            POIs = new List<PointOfInterest>();
            ContouredDoses = new List<IDoseObject>();
            BeamRenderer = new BeamRenderer();
            Beams = new List<Beam>();
            OrthogonalModels = new List<DicomPanelModel>();
            Overlays = new List<IOverlay>();
            ToolBox = new ToolBox();
            Overlays.Add(new ScaleOverlay());
            SpyGlass = new Rectd(0, 0, 1, 1);
            AdditionalImages = new List<RenderableImage>();
        }

        /// <summary>
        /// Invalidates the View model, causing rendering of all rendering contexts
        /// </summary>
        public void Invalidate()
        {
            Stopwatch sw = Stopwatch.StartNew();
            ImageRenderContext?.BeginRender();
            DoseRenderContext?.BeginRender();
            RoiRenderContext?.BeginRender();
            OverlayContext?.BeginRender();

            RenderImages(ImageRenderContext);
            RenderDoses(ImageRenderContext);
            RenderROIs(ImageRenderContext);
            RenderBeams(ImageRenderContext);
            RenderOverlays(OverlayContext);

            ImageRenderContext?.EndRender();
            DoseRenderContext?.EndRender();
            RoiRenderContext?.EndRender();
            OverlayContext?.EndRender();

            OverlayContext?.DrawString("" + sw.ElapsedMilliseconds + " ms", 0, 0, 12, DicomColors.Yellow);
            sw.Stop();

        }

        private void RenderImages(IRenderContext context)
        {
            var primaryImgRect = new Rectd(0, 0, 1, 1);
            var secondaryImgRect = new Rectd(0, 0, 1, 1);

            if (ImageRenderContext != null)
            {
                ImageRenderer?.BeginRender(primaryImgRect, context);
                ImageRenderer?.Render(FromImage(PrimaryImage, BlendMode.None, new Rectd(0,0,1,1)), Camera, context);
                ImageRenderer?.Render(FromImage(SecondaryImage, BlendMode.OverWhereNonZero, SpyGlass), Camera, context);
                foreach (var img in AdditionalImages)
                    ImageRenderer?.Render(img, Camera, context);
                    
                ImageRenderer?.EndRender(primaryImgRect, context);
                //ImageRenderContext?.DrawString("" + ImageRenderer.iy, 0, 0, 12, DicomColors.Yellow);

            }
        }

        private RenderableImage FromImage(DicomImageObject image, BlendMode blendMode, Rectd screenRect)
        {
            if (image == null)
                return new RenderableImage();
            Random r = new Random();

            return new RenderableImage()
            {
                Grid = image.Grid,
                Alpha = 0.5f,
                BlendMode = blendMode,
                Name = image.Name,
                LUT = image.LUT,
                Scaling = 1,
                Units = "HU",
                ScreenRect = screenRect
            };
        }

        private void RenderDoses(IRenderContext context)
        {
            if (context != null)
            {
                for (int i = 0; i < ContouredDoses.Count; i++)
                {
                    DoseRenderer?.Render(ContouredDoses[i], Camera, context, new Rectd(0, 0, 1, 1), (LineType)i);
                }
            }
        }

        private void RenderROIs(IRenderContext context)
        {
            if (context != null)
            {
                ROIRenderer?.Render(ROIs, Camera, context, new Rectd(0, 0, 1, 1));
                ROIRenderer?.Render(POIs, Camera, context, new Rectd(0, 0, 1, 1));
            }
        }

        private void RenderBeams(IRenderContext context)
        {
            if(context != null)
            {
                for (int i = 0; i < Beams.Count; i++)
                {
                    BeamRenderer?.Render(Beams[i], Camera, context, new Rectd(0, 0, 1, 1), (LineType)i);
                }
            }
        }

        private void RenderOverlays(IRenderContext context)
        {
            if (ContouredDoses.Count > 0)
            {
                int k = 0;
                double initY = 5.0 / (double)OverlayContext.Height;
                double initX = 5.0 / (double)OverlayContext.Width;
                double spacing = 17 / (double)OverlayContext.Height;
                foreach (var contourInfo in DoseRenderer.ContourInfo)
                {
                    context.DrawString("" + contourInfo.Threshold, initX, initY + k * spacing, 12, contourInfo.Color);
                    k++;
                }
            }

            if (context != null)
            {
                foreach (IOverlay overlay in Overlays)
                {
                    overlay.Render(this, context);
                }
            }

            if(Camera.IsAxial)
            {
                context?.DrawString("Z: " + Math.Round(Camera.Position.Z, 2) + " mm", 0, .9, 12, DicomColors.Yellow);
            }
        }

        public void InvalidateOverlays()
        {
            OverlayContext.BeginRender();
            RenderOverlays(OverlayContext);
            OverlayContext.EndRender();
        }

        public void SetPrimaryImage(DicomImageObject image)
        {
            PrimaryImage = image;
            ResetCameraToImageCentre();
            Invalidate();
        }

        public void SetSecondaryImage(DicomImageObject image)
        {
            SecondaryImage = image;
            Invalidate();
        }

        public void AddDose(IDoseObject dose)
        {
            if (!ContouredDoses.Contains(dose))
                ContouredDoses.Add(dose);
            Invalidate();
        }

        public void RemoveDose(IDoseObject dose)
        {
            if (ContouredDoses.Contains(dose))
                ContouredDoses.Remove(dose);
            Invalidate();
        }

        public void AddImage(RenderableImage image)
        {
            if(!AdditionalImages.Contains(image))
                AdditionalImages.Add(image);
            Invalidate();
        }

        public void RemoveImage(RenderableImage image)
        {
            if(AdditionalImages.Contains(image))
                AdditionalImages.Remove(image);
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

        public void AddPOI(PointOfInterest poi)
        {
            POIs.Add(poi);
            Invalidate();
        }

        public void RemovePOI(PointOfInterest poi)
        {
            POIs.Remove(poi);
            Invalidate();
        }

        public void AddBeam(Beam beam)
        {
            Beams.Add(beam);
            Invalidate();
        }

        public void RemoveBeam(Beam beam)
        {
            Beams.Remove(beam);
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
            if(PrimaryImage?.Grid != null)
                Camera.MoveTo(PrimaryImage.Grid.XRange.GetCentre(), PrimaryImage.Grid.YRange.GetCentre(), PrimaryImage.Grid.ZRange.GetCentre());
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
