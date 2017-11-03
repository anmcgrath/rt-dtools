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
            int y0i = transyi(y0);
            int y1i = transyi(y1);

            _bitmap.DrawRectangle(transxi(x0), transyi(y0), transxi(x1), transyi(y1), DicomColorConverter.FromDicomColor(color));
        }

        public void FillRect(double x0, double y0, double x1, double y1, DicomColor fill, DicomColor stroke)
        {
            int y0i = transyi(y0);
            int y1i = transyi(y1);

            _bitmap.FillRectangle(transxi(x0), transyi(y0), transxi(x1), transyi(y1), DicomColorConverter.FromDicomColor(fill));
            DrawRect(x0, y0, x1, y1, stroke);
        }

        public void DrawLine(double x0, double y0, double x1, double y1, DicomColor color)
        {
            _bitmap.DrawLine(transxi(x0), transyi(y0), transxi(x1), transyi(y1), DicomColorConverter.FromDicomColor(color));
        }

        public void FillPixels(byte[] byteArray, Rectd destRect)
        {
            var screenRect = new Recti(transxi(destRect.X), transyi(destRect.Y),
                (int)(Width * (destRect.Width)), (int)(Height * (destRect.Height)));

            int stride = 4 * ((screenRect.Width * (_bitmap.Format.BitsPerPixel/8) + 3) / 4);

            _bitmap.WritePixels(new System.Windows.Int32Rect(0, 0, screenRect.Width, screenRect.Height), byteArray, stride, screenRect.X, screenRect.Y);
        }

        public void DrawString(string text, double x, double y, double size, DicomColor color)
        {
            _bitmap.DrawString(transxi(x), transyi(y), DicomColorConverter.FromDicomColor(color), new PortableFontDesc(), text);
        }

        public void BeginRender()
        {
            this._bitmap.Clear(new Color() { A = 0, R = 0, G = 0, B = 0 });
        }

        public void EndRender()
        {
        }

        public void DrawEllipse(double x0, double y0, double radiusX, double radiusY, DicomColor color)
        {
            _bitmap.DrawEllipseCentered(transxi(x0), transyi(y0), transxi(radiusX), transyi(radiusY), DicomColorConverter.FromDicomColor(color));
        }
    }
}
