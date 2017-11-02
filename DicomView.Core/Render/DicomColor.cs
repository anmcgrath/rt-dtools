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

        public static DicomColor FromUInt32(uint color)
        {
            var a = (byte)(color >> 24);
            var r = (byte)(color >> 16);
            var g = (byte)(color >> 8);
            var b = (byte)(color >> 0);
            return FromArgb(a, r, g, b);
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
