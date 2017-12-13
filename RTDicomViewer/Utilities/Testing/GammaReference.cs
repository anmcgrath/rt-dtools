using RT.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Utilities.Testing
{
    public class GammaReference:GridBasedVoxelDataStructure
    {
        public GammaReference()
        {
            Name = "Reference";
            XCoords = new float[144];
            YCoords = new float[201];
            ZCoords = new float[127];
            XRange = new Range(-72, 71);
            YRange = new Range(-100, 100);
            ZRange = new Range(-63, 63);
            Data = new float[144, 201, 127];
            GridSpacing = new RT.Core.Utilities.RTMath.Point3d(1, 1, 1);
            for (int i = 0; i < XCoords.Length; i++)
                XCoords[i] = (float)(XRange.Minimum + i * GridSpacing.X);
            for (int i = 0; i < YCoords.Length; i++)
                YCoords[i] = (float)(YRange.Minimum + i * GridSpacing.Y);
            for (int i = 0; i < ZCoords.Length; i++)
                ZCoords[i] = (float)(ZRange.Minimum + i * GridSpacing.Z);

            this.MinVoxel.Value = 0;
            this.MaxVoxel.Value = 1.98f;

            ConstantGridSpacing = true;

            this.Scaling = 1;
            SetVoxel(0, 0, 0, 2);
        }
    }
}
