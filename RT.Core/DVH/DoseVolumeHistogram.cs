using RT.Core.Dose;
using RT.Core.ROIs;
using System;
using System.Collections.Generic;
using System.Text;

namespace RT.Core.DVH
{
    public class DoseVolumeHistogram
    {
        public double[] Dose { get; set; }
        public double[] DifferentialVolume { get; set; }
        public double[] CumulativeVolume { get; set; }
        public IDoseObject DoseObject { get; set; }
        public RegionOfInterest ROIObject { get; set; }
        public double TotalVolume { get; set; }
        public int NBins { get; set; }
        public bool IsComputed { get; set; }

        public DoseVolumeHistogram(IDoseObject dose, RegionOfInterest roi) : this(dose, roi, 200) { }
        public DoseVolumeHistogram(IDoseObject dose, RegionOfInterest roi, int nbins)
        {
            DoseObject = dose;
            ROIObject = roi;
            NBins = nbins;

            Dose = new double[NBins];
            for(int i = 0; i < Dose.Length; i++)
            {
                Dose[i] = i;
            }
            CumulativeVolume = new double[NBins];
            for(int i = 0; i < Dose.Length; i++)
            {
                CumulativeVolume[i] = 1;
            }
        }

        public void Compute()
        {
            if (!IsComputed)
            {
                ComputeDifferential();
                ComputeCumulative();
                IsComputed = true;
            }
        }

        private void ComputeDifferential()
        {
            Dose = new double[NBins];
            DifferentialVolume = new double[NBins];

            float dDose = DoseObject.Grid.MaxVoxel.Value * DoseObject.Grid.Scaling / NBins;

            for (int i = 0; i < Dose.Length; i++)
                Dose[i] = i * dDose;

            double dx = 1, dy = 1, dz = 1;
            for (double z = ROIObject.ZRange.Minimum -1; z <= ROIObject.ZRange.Maximum +1; z += dz)
            {
                for (double x = ROIObject.XRange.Minimum - dx; x <= ROIObject.XRange.Maximum + dx; x += dx)
                {
                    for (double y = ROIObject.YRange.Minimum - dy; y <= ROIObject.YRange.Maximum + dy; y += dy)
                    {
                        bool pointInside = ROIObject.ContainsPointNonInterpolated(x, y, z);
                        if (pointInside)
                        {
                            float dose = DoseObject.Grid.Interpolate(x, y, z).Value * DoseObject.Grid.Scaling;
                            double voxelVol = dx * dy * dz;
                            DifferentialVolume[(int)(dose / dDose)] += voxelVol;
                            TotalVolume += voxelVol;

                            double s = sum(DifferentialVolume);
                        }
                    }
                }
            }
        }

        private double sum(double[] array)
        {
            double result = 0;
            foreach (var val in array)
                result += val;
            return result;
        }

        private void ComputeCumulative()
        {
            CumulativeVolume = new double[DifferentialVolume.Length];

            for (int i = 0; i < CumulativeVolume.Length; i++)
            {
                CumulativeVolume[i] = 0;
                for (int j = i; j < DifferentialVolume.Length; j++)
                {
                    CumulativeVolume[i] += DifferentialVolume[j];
                }
            }
        }

        public override string ToString()
        {
            string str = "";
            for(int i = 0; i < CumulativeVolume.Length; i++)
            {
                str += Dose[i] + "\t" + 100 * (CumulativeVolume[i] / TotalVolume) + "\n";
            }
            return str;
        }
    }
}
