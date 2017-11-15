using RT.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.ROIs
{
    /// <summary>
    /// Represents a single slice of the ROI which may contain multiple polygons
    /// </summary>
    public class RegionOfInterestSlice
    {
        public List<PlanarPolygon> Polygons { get; set; }
        public BinaryMask BinaryMask { get; set; }
        public double ZCoord { get; set; }
        public RegionOfInterestSlice()
        {
            Polygons = new List<PlanarPolygon>();
            BinaryMask = new BinaryMask();
        }

        public void AddPolygon(PlanarPolygon polygon)
        {
            Polygons.Add(polygon);
        }

        public void ComputeBinaryMask()
        {
            if (Polygons.Count > 1)
            {

            }
            for (int i = 0; i < Polygons.Count; i++)
            {
                BinaryMask.AddPolygon(Polygons[i]);
            }
        }

        public bool ContainsPoint(double x, double y)
        {
            return BinaryMask.ContainsPoint(x, y);
        }
    }
}
