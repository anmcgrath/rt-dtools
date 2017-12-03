using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Core.Utilities.RTMath;

namespace RT.Core.Geometry
{
    public abstract class VoxelDataStructureBase
    {
        public Range XRange { get; set; }
        public Range YRange { get; set; }
        public Range ZRange { get; set; }
        public Voxel MaxVoxel { get; set; }
        public Voxel MinVoxel { get; set; }
        private Point3d positionCache;

        /// <summary>
        /// Converts actual value to value of type Unit
        /// </summary>
        public float Scaling { get; set; } = 1;

        /// <summary>
        /// The physical unit
        /// </summary>
        public Unit ValueUnit { get; set; }

        public VoxelDataStructureBase()
        {
            XRange = new Range();
            YRange = new Range();
            ZRange = new Range();
            positionCache = new Point3d();
            MaxVoxel = new Voxel() { Value = float.MinValue };
            MinVoxel = new Voxel() { Value = float.MaxValue };
        }

        public Voxel Interpolate(double x, double y, double z)
        {
            Voxel voxel = new Voxel();
            Interpolate(x, y, z, voxel);
            return voxel;
        }

        public Voxel Interpolate(Point3d position)
        {
            Voxel voxel = new Voxel();
            Interpolate(position.X, position.Y, position.Z, voxel);
            return voxel;
        }

        public void Interpolate(double x, double y, double z, Voxel voxel)
        {
            positionCache.X = x;
            positionCache.Y = y;
            positionCache.Z = z;
            Interpolate(positionCache, voxel);
        }

        public bool ContainsPoint(double x, double y, double z)
        {
            return XRange.Contains(x) && YRange.Contains(y) && ZRange.Contains(z);
        }

        public bool ContainsPoint(Point3d point)
        {
            return ContainsPoint(point.X, point.Y, point.Z);
        }

        public abstract void Interpolate(Point3d position, Voxel voxel);

    }
}
