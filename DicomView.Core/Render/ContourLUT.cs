using DicomPanel.Core.Render.Contouring;
using RT.Core.Imaging.LUT;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Render
{
    public class ContourLUT : ILUT
    {
        public float Window { get; set; }
        public float Level { get; set; }
        public float Max { get; set; } = 1000;
        private byte[] red = new byte[256];
        private byte[] green = new byte[256];
        private byte[] blue = new byte[256];

        public void Compute(float value, byte[] output)
        {
            var val = (int)((value / Max) * 255);
            output[0] = blue[val];
            output[1] = green[val];
            output[2] = red[val];
        }

        public void Create(List<ContourInfo> contours, float max)
        {
            Max = max;
            for (int j = 0; j < 256; j++)
            {
                for (int i = contours.Count - 1; i >= 0; i--)
                {
                    double thr = (double)j / 256.0;
                    if(thr >= (contours[i].Threshold/100))
                    {
                        red[j] = (byte)contours[i].Color.R;
                        green[j] = (byte)contours[i].Color.G;
                        blue[j] = (byte)contours[i].Color.B;
                    }
                }
            }
        }
    }
}
