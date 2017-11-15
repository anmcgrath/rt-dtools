using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.Geometry
{
    public interface IVoxelDataStructure
    {
        Voxel Interpolate(double x, double y, double z);
        Voxel Interpolate(Point3d position);
        void Interpolate(double x, double y, double z, Voxel voxel);
        void Interpolate(Point3d position, Voxel voxel);

        Range XRange { get; set; }
        Range YRange { get; set; }
        Range ZRange { get; set; }

        float DefaultPhysicalValue { get; set; }

        Voxel MaxVoxel { get; set; }
        void ComputeMax();
    }
}
