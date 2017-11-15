using RT.Core.Dose;
using RT.Core.ROIs;
using System;
using System.Collections.Generic;
using System.Text;

namespace RT.Core.DVH
{
    public class DoseVolumeHistogram
    {
        public float[] Dose { get; set; }
        public float[] DifferentialVolume { get; set; }
        public float[] CumulativeVolume { get; set; }
        public IDoseObject DoseObject { get; set; }
        public RegionOfInterest ROIObject { get; set; }
        public float TotalVolume { get; set; }
        public int NBins { get; set; }

        public DoseVolumeHistogram(IDoseObject dose, RegionOfInterest roi) : this(dose, roi, 100) { }
        public DoseVolumeHistogram(IDoseObject dose, RegionOfInterest roi, int nbins)
        {
            DoseObject = dose;
            ROIObject = roi;
            NBins = nbins;
        }

        public void Compute()
        {
            ComputeDifferential();
            ComputeCumulative();
        }

        private void ComputeDifferential()
        {
            Dose = new float[NBins];
            DifferentialVolume = new float[NBins];

            float dDose = DoseObject.Grid.MaxVoxel.Value / NBins;

            for (int i = 0; i < Dose.Length; i++)
                Dose[i] = i * dDose;

            float dx = 1, dy = 1, dz = 1;
            for (double z = ROIObject.ZRange.Minimum - dz; z <= ROIObject.ZRange.Maximum + dz; z += dz)
            {
                for (double x = ROIObject.XRange.Minimum - dx; x <= ROIObject.XRange.Maximum + dx; x += dx)
                {
                    for (double y = ROIObject.XRange.Minimum - dx; y <= ROIObject.YRange.Maximum + dy; y += dy)
                    {
                        bool pointInside = ROIObject.ContainsPointNonInterpolated(x, y, z);
                        if (pointInside)
                        {
                            float dose = DoseObject.Grid.Interpolate(x, y, z).Value;
                            DifferentialVolume[(int)(dose / dDose)] += dx * dy * dz;
                            TotalVolume += dx * dy * dz;
                        }
                    }
                }
            }

        }

        private void ComputeCumulative()
        {
            CumulativeVolume = new float[DifferentialVolume.Length];

            for (int i = 0; i < CumulativeVolume.Length; i++)
            {
                CumulativeVolume[i] = 0;
                for (int j = i; j < DifferentialVolume.Length; j++)
                {
                    CumulativeVolume[i] += DifferentialVolume[j];
                }
            }
        }
    }
}
