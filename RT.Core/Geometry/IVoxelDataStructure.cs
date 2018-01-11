using RT.Core.Dose;
using RT.Core.Planning;
using RT.Core.Utilities.RTMath;
using System.Collections;

namespace RT.Core.Geometry
{
    public interface IVoxelDataStructure : IEnumerable
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

        float this[Point3d point] { get; }

        double NormalisationPercent { get; set; }
        NormalisationType NormalisationType { get; set; }
        RelativeNormalisationOption RelativeNormalisationOption { get; set; }
        PointOfInterest NormalisationPOI { get; set; }

        float GetNormalisationAmount();


    }
}
