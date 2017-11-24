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
        public BeamRenderer BeamRenderer { get; set; }

        public DicomImageObject Image { get; set; }
        public DicomImageObject SecondaryImage { get; set; }
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

            var primaryImgRect = new Rectd(0, 0, 1, 1);
            var secondaryImgRect = new Rectd(0, 0, 1, 1);
            if(SecondaryImage != null)
            {
                //primaryImgRect.Width = PrimarySecondaryImageSplitLocation;
                //secondaryImgRect.X = PrimarySecondaryImageSplitLocation;
                //secondaryImgRect.Width = (1 - PrimarySecondaryImageSplitLocation);
                //OverlayContext?.DrawRect(PrimarySecondaryImageSplitLocation - .01, 0, PrimarySecondaryImageSplitLocation + .01, .01, new RT.Core.DICOM.DicomColor(255, 255, 0, 0));
            }
            if(ImageRenderContext != null)
            {
                List<IVoxelDataStructure> imgs = new List<IVoxelDataStructure>();
                List<ILUT> luts = new List<ILUT>();
                List<double> alphas = new List<double>();
                if(Image != null)
                {
                    imgs.Add(Image.Grid);
                    luts.Add(Image.LUT);
                    alphas.Add(.5);
                }
                if(SecondaryImage != null)
                {
                    imgs.Add(SecondaryImage.Grid);
                    luts.Add(SecondaryImage.LUT);
                    alphas.Add(.5);
                }
                ImageRenderer?.Render(imgs, Camera, ImageRenderContext, primaryImgRect, luts, alphas);
            }

            if (DoseRenderContext != null)
            {
                for (int i = 0; i < ContouredDoses.Count; i++)
                {
                    DoseRenderer?.Render(ContouredDoses[i], Camera, ImageRenderContext, new Rectd(0, 0, 1, 1),(LineType)i);
                }

                for(int i = 0; i < Beams.Count; i++)
                {
                    BeamRenderer?.Render(Beams[i], Camera, ImageRenderContext, new Rectd(0, 0, 1, 1), (LineType)i);
                }
            }

            if (ContouredDoses.Count > 0)
            {
                int k = 0;
                double initY = 5.0 / (double)OverlayContext.Height;
                double initX = 5.0 / (double)OverlayContext.Width;
                double spacing = 17 / (double)OverlayContext.Height;
                foreach (var contourInfo in DoseRenderer.ContourInfo)
                {
                    OverlayContext.DrawString("" + contourInfo.Threshold, initX, initY + k * spacing, 12, contourInfo.Color);
                    k++;
                }
            }

            if (RoiRenderContext != null)
            {
                ROIRenderer?.Render(ROIs, Camera, ImageRenderContext, new Rectd(0, 0, 1, 1));
                ROIRenderer?.Render(POIs, Camera, ImageRenderContext, new Rectd(0, 0, 1, 1));
            }

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
            //if (ImageRenderContext != null)
                //ImageRenderer?.Render(Image, Camera, ImageRenderContext, new Rectd(0, 0, 1, 1), new GrayScaleLUT(0,0));
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
