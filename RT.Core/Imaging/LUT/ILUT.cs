using System;
using System.Collections.Generic;
using System.Text;

namespace RT.Core.Imaging.LUT
{
    public interface ILUT
    {
        void Compute(float value, byte[] output);
        float Window { get; set; }
        float Level { get; set; }
    }
}
