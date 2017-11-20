using DicomPanel.Core.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Core.DICOM;
using RT.Core.Utilities.RTMath;

namespace DicomPanel.Wpf.Rendering
{
    public class OpenGlRenderContext : IRenderContext
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public double RelativeScale { get; set; } = 1;


        public void BeginRender()
        {
            
        }

        public void DrawEllipse(double x0, double y0, double radiusX, double radiusY, DicomColor color)
        {
            
        }

        public void DrawLine(double x0, double y0, double x1, double y1, DicomColor color, LineType lineType)
        {
            
        }

        public void DrawLine(double x0, double y0, double x1, double y1, DicomColor color)
        {
            
        }

        public void DrawRect(double x0, double y0, double x1, double y1, DicomColor color)
        {
            
        }

        public void DrawString(string text, double x, double y, double size, DicomColor color)
        {
            
        }

        public void EndRender()
        {
            
        }

        public void FillPixels(byte[] byteArray, Rectd destRect)
        {
            
        }

        public void FillRect(double x0, double y0, double x1, double y1, DicomColor fill, DicomColor stroke)
        {
            
        }
    }
}
