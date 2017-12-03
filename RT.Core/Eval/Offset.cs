using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace RT.Core.Eval
{
    public class Offset:IComparable<Offset>
    {
        public double DistanceSquared { get; set; }
        public Point3d Displacement { get { return _displacement; } set { _displacement = value; DistanceSquared = _displacement.LengthSquared(); } }
        private Point3d _displacement;

        public int CompareTo(Offset obj)
        {
            return this.DistanceSquared.CompareTo(obj.DistanceSquared);
        }
    }
}
