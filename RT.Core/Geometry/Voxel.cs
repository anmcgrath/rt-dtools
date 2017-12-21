using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.Geometry
{
    public class Voxel<T>
    {
        public Point3d Position { get; set; }
        public T Value { get; set; }

        public Voxel()
        {
            Position = new Point3d();
        }
    }

    public class Voxel : Voxel<float> { }
}
