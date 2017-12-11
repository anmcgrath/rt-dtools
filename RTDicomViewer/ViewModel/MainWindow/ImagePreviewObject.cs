using DicomPanel.Core.Render;
using DicomPanel.Wpf.Rendering;
using RT.Core.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RTDicomViewer.ViewModel.MainWindow
{
    public class ImagePreviewObject
    {
        public WriteableBitmap ImagePreview { get; set; }
        public DicomImageObject Image { get; set; }

        public ImagePreviewObject(DicomImageObject img)
        {
            Image = img;
            ImagePreview = new WriteableBitmap(50, 50, 96, 96, PixelFormats.Bgr32, null);
            var wbContext = new WriteableBitmapRenderContext(ImagePreview);
            wbContext.Resize(ImagePreview, 50, 50);
            var imgRenderer = new ImageRenderer();
            var camera = new Camera();
            camera.SetAxial();
            camera.SetFOV(512, 512);
            camera.MoveTo(img.Grid.XRange.GetCentre(), img.Grid.YRange.GetCentre(), img.Grid.ZRange.GetCentre());
            var renderableImage = new RenderableImage();
            renderableImage.LUT = img.LUT;
            renderableImage.ScreenRect = new RT.Core.Utilities.RTMath.Rectd(0, 0, 1, 1);
            renderableImage.Scaling = img.Grid.Scaling;
            renderableImage.Grid = img.Grid;
            renderableImage.Units = "HU";
            imgRenderer.BeginRender(renderableImage.ScreenRect, wbContext);
            imgRenderer.Render(renderableImage, camera, wbContext);
            imgRenderer.EndRender(renderableImage.ScreenRect, wbContext);
        }
    }
}
