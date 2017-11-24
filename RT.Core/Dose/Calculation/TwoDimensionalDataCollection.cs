using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace RT.Core.Dose.Calculation
{
    public class TwoDimensionalDataCollection
    {
        public List<float> X { get; set; }
        public List<float> Y { get; set; }
        public List<List<float>> XY { get; set; }

        public float Interpolate(float x, float y)
        {
            int xi = BinaryMath.BinarySearchClosest<float>(x, X);
            float x1 = X[xi - 1];
            float x2 = X[xi];
            float y1 = Y[xi - 1];
            float y2 = Y[xi];
            return ((x - x1) / (x2 - x1)) * (y2 - y1) + y1;
        }
    }
}
