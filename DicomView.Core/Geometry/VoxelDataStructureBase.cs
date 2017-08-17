using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DicomPanel.Core.Utilities.RTMath;

namespace DicomPanel.Core.Geometry
{
    public abstract class VoxelDataStructureBase
    {
        public Range XRange { get; set; }
        public Range YRange { get; set; }
        public Range ZRange { get; set; }
        public Voxel MaxVoxel { get; set; }
        private Point3d positionCache;

        public Voxels Voxels { get; protected set; }

        public VoxelDataStructureBase()
        {
            XRange = new Range();
            YRange = new Range();
            ZRange = new Range();
            positionCache = new Point3d();
            MaxVoxel = new Voxel() { Value = float.MinValue };
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

        public abstract void Interpolate(Point3d position, Voxel voxel);

        public void ComputeMax()
        {
            foreach(Voxel voxel in Voxels)
            {
                if(voxel.Value > MaxVoxel.Value)
                {
                    MaxVoxel = voxel;
                }
            }
        }

    }
}
