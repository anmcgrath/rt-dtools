using DicomPanel.Core.Render;
using DicomPanel.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DicomPanel.Wpf.Rendering
{
    public class WriteableBitmapRenderContext : BaseRenderContext, IRenderContext
    {
        private WriteableBitmap _bitmap;
        public double RelativeScale { get; set; }

        public WriteableBitmapRenderContext(WriteableBitmap bitmap)
        {
            _bitmap = bitmap;
        }

        public void Resize(WriteableBitmap bitmap, int width, int height)
        {
            _bitmap = bitmap;
            this.Width = width;
            this.Height = height;
        }

        public void DrawRect(double x0, double y0, double x1, double y1, DicomColor color)
        {
            _bitmap.DrawRectangle((int)x0, (int)y0, (int)x1, (int)y1, DicomColorConverter.FromDicomColor(color));
        }

        public void DrawLine(double x0, double y0, double x1, double y1, DicomColor color)
        {
            _bitmap.DrawLine((int)x0, (int)y0, (int)x1, (int)y1, DicomColorConverter.FromDicomColor(color));
        }

        public void FillPixels(byte[] byteArray, Recti destRect)
        {
            _bitmap.WritePixels(new System.Windows.Int32Rect(0, 0, destRect.Width, destRect.Height), byteArray, _bitmap.BackBufferStride, destRect.X,destRect.Y);
        }
    }
}
