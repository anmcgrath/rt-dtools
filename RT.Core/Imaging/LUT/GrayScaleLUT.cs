using System;
using System.Collections.Generic;
using System.Text;

namespace RT.Core.Imaging.LUT
{
    public class GrayScaleLUT : ILUT
    {
        public bool IsGrayScale => true;

        public float Window { get { return _window; } set { Create(value, _level); _window = value; } }
        private float _window = 400;
        public float Level { get { return _level; } set { Create(_window, value); } }
        private float _level = 40;

        private int maxPixel = 255;
        private float min = 0;
        private float max = 0;
        private float windowTimesMaxPixel;
        private byte gray = 0;

        public GrayScaleLUT() { Create(Window, Level); }

        public void Compute(float value, byte[] output)
        {
            gray = Compute(value);
            output[0] = gray;
            output[1] = gray;
            output[2] = gray;
        }

        public byte Compute(float value)
        {
            if (value < min)
                value = 0;
            else if (value > max)
                value = maxPixel;
            else
                value = (value - min) * windowTimesMaxPixel;
            if (value < 0)
                value = 0;
            if (value > maxPixel)
                value = maxPixel;
            return (byte)value;
        }

        public void Create(float window, float level)
        {
            min = level - window / 2;
            max = level + window / 2;
            windowTimesMaxPixel = maxPixel / window;
        }
    }
}
