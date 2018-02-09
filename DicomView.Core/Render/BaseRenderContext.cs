using RT.Core.DICOM;
using RT.Core.Utilities.RTMath;
using System;

namespace DicomPanel.Core.Render
{
    public class BaseRenderContext
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Recti ScreenRect { get { return new Recti(0, 0, Width, Height); } }
        protected CoordinateTransform t = new CoordinateTransform();

        private Point2d _cachedNormDeviceCoords = new Point2d();
        /// <summary>
        /// Translate x from normalised device coordinates to an integer screen coordinate
        /// </summary>
        /// <returns></returns>
        protected int transxi(double x)
        {
            return (int)Math.Round(t.Ndc2x(x, Width));
        }

        /// <summary>
        /// Transalate y from normalised device coordinates to an interger screen coordinate
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        protected int transyi(double y)
        {
            return (int)Math.Round(t.Ndc2y(y, Height));
        }
    }
}
