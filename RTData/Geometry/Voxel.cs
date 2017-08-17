using RTData.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTData.Geometry
{
    public class Voxel
    {
        public Point3d Position { get; set; }
        public float Value { get; set; }

        public Voxel()
        {
            Position = new Point3d();
        }
    }
}
