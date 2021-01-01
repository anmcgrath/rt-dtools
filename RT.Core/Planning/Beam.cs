using RT.Core.Imaging;
using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.Planning
{
    public class Beam
    {
        public double SAD { get; set; } = 1000;
        public double SSD { get; set; }
        public double MU { get; set; }
        public double DoseRateSet { get; set; }
        public double FinalCumulativeMetersetWeight { get; set; } = 1.0;
        public string Name { get; set; }
        public List<ControlPoint> ControlPoints { get; set; }
        public string TreatmentMachineName { get; set; }
        public double GantryStop { get; set; }
        public double GantryStart { get; set; }
        public double CouchAngle { get; set; }
        public double CollimatorAngle { get; set; }
        public Jaw XJaw { get; set; }
        public Jaw YJaw { get; set; }
        
        public PointOfInterest Isocenter { get; set; }

        public List<Block> Blocks { get; set; }
        public Beam()
        {
            ControlPoints = new List<ControlPoint>();
            XJaw = new Jaw();
            XJaw.NegativeJawPosition = -100;
            XJaw.PositiveJawPosition = 100;
            YJaw = new Jaw();
            YJaw.NegativeJawPosition = -100;
            YJaw.PositiveJawPosition = 100;
        }

        public void CalculateSSD(DicomImageObject img, float threshold)
        {
            var p1 = new Point3d(Isocenter.Position.X, Isocenter.Position.Y - SAD, Isocenter.Position.Z);
            RTCoordinateTransform T = new RTCoordinateTransform();
            T.CollimatorAngle = CollimatorAngle;
            T.CouchAngle = CouchAngle;
            T.GantryAngle = GantryStart;

            Point3d sourcePosn = new Point3d();
            T.Transform(p1, Isocenter.Position, sourcePosn);

            //Unit vector in direction from source position to isocentre with a length of sampleLength mm
            double sampleLength = 2;
            double totalLength = (Isocenter.Position - sourcePosn).Length();
            int n = (int)totalLength / (int)sampleLength; // the number of steps to sample in the image
            var u = sampleLength * ((Isocenter.Position - sourcePosn) / totalLength);
            double ssd = 0;
            for(int i = 0; i < n; i++)
            {
                var p = sourcePosn + u * i;
                var hu = img.Grid.Interpolate(p);
                if(hu.Value > threshold)
                {
                    ssd = i * sampleLength;
                    break;
                }
            }
            this.SSD = ssd;
        }
    }
}
