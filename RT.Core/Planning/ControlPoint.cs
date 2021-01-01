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

        public decimal GantryAngle { get; set; }
        public decimal CollimatorAngle { get; set; }
        public decimal CouchRotation { get; set; }
        public decimal CouchAngle { get; set; }
        public double[] Isocenter { get; set; }

        public string LeafX1 { get; set; }
        public string LeafX2 { get; set; }
    }
}
