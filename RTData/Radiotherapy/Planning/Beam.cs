using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTData.Radiotherapy.Planning
{
    public class Beam
    {
        public double SAD { get; set; }
        public double MU { get; set; }
        public string Name { get; set; }
        public List<ControlPoint> ControlPoints { get; set; }
        public string TreatmentMachineName { get; set; }
        public double GantryStop { get; set; }
        public double GantryStart { get; set; }

        public Beam()
        {
            ControlPoints = new List<ControlPoint>();
        }
    }
}
