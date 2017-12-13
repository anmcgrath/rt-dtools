using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.DICOM
{
    public class DicomColor
    {
        /// <summary>
        /// Alpha channel
        /// </summary>
        public int A { get; set; }
        /// <summary>
        /// Red channel
        /// </summary>
        public int R { get; set; }
        /// <summary>
        /// Green channel
        /// </summary>
        public int G { get; set; }
        /// <summary>
        /// Blue channel
        /// </summary>
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

        /// <summary>
        /// Create a DicomColor from a uint 3d
        /// </summary>
        /// <param name="color">uint value of the color, e.g 0xFF0000</param>
        /// <returns></returns>
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
