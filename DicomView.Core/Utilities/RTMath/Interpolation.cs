using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Utilities.RTMath
{
    public class Interpolation
    {
        public static float TrilinearInterpolate(float x, float y, float z, float x0, float y0, float z0, float x1, float y1, float z1, float v000, float v100, float v001, float v101, float v010, float v110, float v011, float v111)
        {
            float xd;
            if (x1 == x0)
                xd = 0;
            else
                xd = (x - x0) / (x1 - x0);
            double yd;
            if (y1 == y0)
                yd = 0;
            else
                yd = (y - y0) / (y1 - y0);
            double zd;
            if (z1 == z0)
                zd = 0;
            else
                zd = (z - z0) / (z1 - z0);
            float c00 = v000 * (1 - xd) + v100 * xd;
            float c01 = v001 * (1 - xd) + v101 * xd;
            float c10 = v010 * (1 - xd) + v110 * xd;
            float c11 = v011 * (1 - xd) + v111 * xd;
            float c0 = (float)(c00 * (1 - yd) + c10 * yd);
            float c1 = (float)(c01 * (1 - yd) + c11 * yd);
            float c = (float)(c0 * (1 - zd) + c1 * zd);

            return c;
        }
    }
}
