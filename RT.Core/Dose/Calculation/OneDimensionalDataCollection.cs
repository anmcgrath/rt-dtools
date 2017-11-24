using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace RT.Core.Dose.Calculation
{
    public class OneDimensionalDataCollection
    {
        public List<float> X { get; set; }
        public List<float> Y { get; set; }

        public float Interpolate(float x)
        {
            int i = BinaryMath.BinarySearchClosest<float>(x, X);
            float x1 = X[i - 1];
            float x2 = X[i];
            float y1 = Y[i - 1];
            float y2 = Y[i];
            return ((x - x1) / (x2 - x1)) * (y2 - y1) + y1;
        }
    }
}
