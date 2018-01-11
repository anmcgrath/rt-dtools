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
        private int bins = 65000;
        private byte[] red = new byte[65000];
        private byte[] green = new byte[65000];
        private byte[] blue = new byte[65000];
        public float Max;
        public float Norm;

        public void Compute(float value, byte[] output)
        {
            var val = (int)(Math.Round(((value / Max) * bins)));
            if (val < 0)
                val = 0;
            if (val > bins - 1)
                val = bins - 1;
            output[0] = blue[val];
            output[1] = green[val];
            output[2] = red[val];
        }


        public void Create(List<ContourInfo> contours, float max, float norm)
        {
            Norm = norm;
            Max = max;
            for (int j = 0; j < bins; j++)
            {
                for (int i = contours.Count - 1; i >= 0; i--)
                {
                    double thr = (double)(j * (Max / Norm)) / (bins + 1);
                    if (thr >= (contours[i].Threshold))
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
