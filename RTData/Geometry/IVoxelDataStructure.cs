﻿using RTData.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTData.Geometry
{
    public interface IVoxelDataStructure:IEnumerator<Voxel>
    {
        Voxel Interpolate(double x, double y, double z);
        Voxel Interpolate(Point3d position);
        void Interpolate(double x, double y, double z, Voxel voxel);
        void Interpolate(Point3d position, Voxel voxel);
        Voxels Voxels { get; }

        Range XRange { get; set; }
        Range YRange { get; set; }
        Range ZRange { get; set; }
    }
}
