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
            ImagePreview = new WriteableBitmap(100, 100, 96, 96, PixelFormats.Bgr32, null);
            var wbContext = new WriteableBitmapRenderContext(ImagePreview);
            wbContext.Resize(ImagePreview, 100, 100);
            var imgRenderer = new ImageRenderer();
            var camera = new Camera();
            camera.SetAxial();
            camera.SetFOV(512, 512);
            camera.MoveTo(img.Grid.XRange.GetCentre(), img.Grid.YRange.GetCentre(), img.Grid.ZRange.GetCentre());
            imgRenderer.Render(img, camera, wbContext, new RT.Core.Utilities.RTMath.Rectd(0, 0, 1, 1));
        }
    }
}
