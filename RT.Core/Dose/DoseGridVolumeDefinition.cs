using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace RT.Core.Dose
{
    public class DoseGridVolumeDefinition
    {
        /// <summary>
        /// The spacing, in mm, in the x-direction
        /// </summary>
        public double Dx { get; set; } = 3;
        /// <summary>
        /// The spacing, in mm, in the y-direction
        /// </summary>
        public double Dy { get; set; } = 3;
        /// <summary>
        /// The spacing, in mm, in the z-direction
        /// </summary>
        public double Dz { get; set; } = 3;

        /// <summary>
        /// The first point defining the cube
        /// </summary>
        public Point3d P1 { get; set; }

        /// <summary>
        /// The second point defining the cube
        /// </summary>
        public Point3d P2 { get; set; }

        /// <summary>
        /// Defines a cubic volume in which dose calculation should be performed
        /// </summary>
        public DoseGridVolumeDefinition(Point3d p1, Point3d p2)
        {
            P1 = p1;
            P2 = p2;
        }
    }
}
