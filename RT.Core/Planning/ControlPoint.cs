using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.Planning
{
    public class ControlPoint
    {
        public Jaw XJaw { get; set; }
        public Jaw YJaw { get; set; }
        public int Index { get; set; }
        public double CumulativeMetersetWeight { get; set; }
        public int NominalBeamEnergy { get; set; }

        public double GantryAngle { get; set; }
        public double CollimatorAngle { get; set; }
        public double CouchRotation { get; set; }
        public double CouchAngle { get; set; }
        public double[] Isocentre { get; set; }
    }
}
