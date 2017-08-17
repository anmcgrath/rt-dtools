using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DicomPanel.Core.Render;
using DicomPanel.Core.Utilities.RTMath;

namespace DicomPanel.Wpf.Rendering
{
    public class CanvasRenderContext : BaseRenderContext, IRenderContext
    {
        public double RelativeScale { get; set; }
        private Canvas _canvas { get; set; }
        public CanvasRenderContext(Canvas canvas)
        {
            _canvas = canvas;
        }
        public void FillPixels(byte[] byteArray, Recti destRect)
        {
            //Currently don't know how to do this...
        }

        public void DrawRect(double x0, double y0, double x1, double y1, DicomColor color)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Stroke = new SolidColorBrush(DicomColorConverter.FromDicomColor(color));
            rectangle.Width = Math.Abs(x1 - x0);
            rectangle.Height = Math.Abs(y1 - y0);
            Canvas.SetLeft(rectangle, Math.Min(x0, x1));
            Canvas.SetTop(rectangle, Math.Min(y0, y1));
            _canvas.Children.Add(rectangle);
        }

        public void DrawLine(double x0, double y0, double x1, double y1, DicomColor color)
        {
            throw new NotImplementedException();
        }
    }
}
