using System;
using System.Collections.Generic;
using System.Text;

namespace RT.Core.Utilities.RTMath
{
    /// <summary>
    /// Class that represents a histogram for storing floats
    /// </summary>
    public class Histogramf
    {
        public int[] Counts;
        private float Min { get; set; }
        private float Max { get; set; }

        public Histogramf(float min, float max, int numberOfBins)
        {
            init(min, max, numberOfBins);
        }

        private void init(float min, float max, int numberOfBins)
        {
            Counts = new int[numberOfBins];
            Min = min;
            Max = max;
        }

        public void CreateFromData(IEnumerable<float> data, float min, float max, int numberOfBins)
        {
            init(min, max, numberOfBins);

            foreach (var dataPoint in data)
            {
                AddDataPoint(dataPoint);
            }
        }

        public float[] GetBinLabels()
        {
            float[] binLabels = new float[Counts.Length];
            float StepSize = (Max - Min) / Counts.Length;
            for(int i = 0; i < Counts.Length; i++)
            {
                binLabels[i] = Min + i * StepSize;
            }
            return binLabels;
        }

        public void AddDataPoint(float dataPoint)
        {
            if (dataPoint >= Min && dataPoint <= Max)
                Counts[getBinNumber(dataPoint)]++;
        }

        private int getBinNumber(float dataPoint)
        {
            return (int)(((dataPoint - Min) / (Max - Min)) * (Counts.Length - 1));
        }
}
}
