using RTData.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTData.Radiotherapy.Planning
{
    public class Jaw
    {
        public Direction Direction { get; set; }
        /// <summary>
        /// Position of the negative jaw, i.e X1 or Y1
        /// </summary>
        public double NegativeJawPosition { get; set; }
        /// <summary>
        /// Position of the positive jaw, i.e X2 or Y2
        /// </summary>
        public double PositiveJawPosition { get; set; }
    }
}
