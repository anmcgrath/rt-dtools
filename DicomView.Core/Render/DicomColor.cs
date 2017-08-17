using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomPanel.Core.Render
{
    public class DicomColor
    {
        public int A { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        private static int MAX = 255;

        public DicomColor() { }

        public DicomColor(int a, int r, int g, int b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        public static DicomColor FromArgb(int a, int r, int g, int b)
        {
            return new DicomColor(a, r, g, b);
        }

        public static DicomColor FromRgb(int r, int g, int b)
        {
            return new DicomColor(MAX, r, g, b);
        }
    }
}
