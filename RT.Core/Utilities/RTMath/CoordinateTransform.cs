using System;
using System.Collections.Generic;
using System.Text;

namespace RT.Core.Utilities.RTMath
{
    public class CoordinateTransform
    {
        /// <summary>
        /// Transforms a x coordinate in the normalised device space, where (-1,1) is on the bottom left, to a space of width "width" where (0,0) is the top left, and the y direction is flipped
        /// </summary>
        /// <param name="nx"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public double Ndc2x(double nx, double width)
        {
            return width * nx;
        }

        /// <summary>
        /// Transforms a y coordinate in the normalised device space, where (-1,1) is on the bottom left, to a space of height "height" where (0,0) is the top left, and the y direction is flipped
        /// </summary>
        /// <param name="ny"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public double Ndc2y(double ny, double height)
        {
            return height * ny;
        }

        public double Y2Ndc(double y, double height)
        {
            return y / height;
        }

        public double X2Ndc(double x, double width)
        {
            return x / width;
        }
    }
}
