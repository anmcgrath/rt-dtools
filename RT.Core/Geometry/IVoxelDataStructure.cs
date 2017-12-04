using RT.Core.ROIs;
using RT.Core.Utilities.RTMath;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.Geometry
{
    public interface IVoxelDataStructure:IEnumerable
    {
        Voxel Interpolate(double x, double y, double z);
        Voxel Interpolate(Point3d position);
        void Interpolate(double x, double y, double z, Voxel voxel);
        void Interpolate(Point3d position, Voxel voxel);

        Range XRange { get; set; }
        Range YRange { get; set; }
        Range ZRange { get; set; }
        bool ContainsPoint(Point3d point);
        bool ContainsPoint(double x, double y, double z);

        float DefaultPhysicalValue { get; set; }
        Unit ValueUnit { get; set; }
        float Scaling { get; set; }

        Voxel MaxVoxel { get; set; }
        Voxel MinVoxel { get; set; }
        void ComputeMax();

        int NumberOfVoxels { get; }
        string Name { get; set; }

        //Histogramf CreateHistogram(RegionOfInterest roi);
        //Histogramf CreateHistogram();

        
    }
}
