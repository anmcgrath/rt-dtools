using DicomPanel.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Radiotherapy.Planning
{
    public class PointOfInterest
    {
        public Point3d Position { get; set; }
        public PointOfInterest(Point3d position)
        {
            this.Position = position;
        }
        public PointOfInterest()
        {
            this.Position = new Point3d();
        }

    }
}
