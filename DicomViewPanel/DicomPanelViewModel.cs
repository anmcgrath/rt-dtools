using DicomPanel.Core;
using DicomPanel.Core.Render;
using DicomPanel.Wpf.Rendering;
using DicomPanel.Wpf.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DicomPanel.Wpf
{
    public class DicomPanelViewModel:BindableItem
    {

        public DicomPanelModel Model { get; set; }

        public WriteableBitmap ImageBase { get { return _imageBase; } set { SetField(ref _imageBase, value); } }
        private WriteableBitmap _imageBase;

        public WriteableBitmap ImageTop { get { return _imageTop; } set { SetField(ref _imageTop, value); } }
        private WriteableBitmap _imageTop;

        public WriteableBitmapRenderContext ImageBaseRenderContext { get; set; }
        public WriteableBitmapRenderContext ImageTopRenderContext { get; set; }

        public DicomPanelViewModel()
        {
            ImageBaseRenderContext = new WriteableBitmapRenderContext(ImageBase);
            ImageTopRenderContext = new WriteableBitmapRenderContext(ImageTop);
        }

        public void SetModel(DicomPanelModel model)
        {
            Model = model;
            Model.SetImageRenderContext(ImageBaseRenderContext);
            Model.SetDoseRenderContext(ImageTopRenderContext);
            Model.SetRoiRenderContext(ImageTopRenderContext);
            Model.SetOverlayContext(ImageTopRenderContext);
            Model?.Invalidate();
        }

        public void OnSizeChanged(double newWidth, double newHeight)
        {
            createRenderTargets((int)Math.Round(newWidth), (int)Math.Round(newHeight));
            Model?.Invalidate();
        }

        private void createRenderTargets(int width, int height)
        {
            int imgWidth = Math.Min(width, 512);
            int imgHeight = Math.Min(height, 512);
            ImageBase = new WriteableBitmap(imgWidth, imgHeight, 96, 96, PixelFormats.Bgr32, null);
            ImageTop = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
            ImageBaseRenderContext.Resize(ImageBase, imgWidth, imgHeight);
            ImageTopRenderContext.Resize(ImageTop, width, height);
        }
    }
}
