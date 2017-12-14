using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.Geometry
{
    /// <summary>
    /// Represents a planar polygon, with all vertices in the same plane
    /// </summary>
    public class PlanarPolygon
    {
        public Range XRange { get; set; }
        public Range YRange { get; set; }
        /// <summary>
        /// Polygon vertices in the form [x0,y0,x1,y1... ] etc
        /// </summary>
        public double[] Vertices { get; set; }

        /// <summary>
        /// Returns a SORTED list of x coordinates that define intersection points with the line defined at y0. The line is perpendicular to the y-axis.
        /// </summary>
        /// <param name="y0">The y coordinate of the line perpendicular to the y-axis</param>
        /// <returns></returns>
        public double[] GetFixedYLineIntersections(double y)
        {
            List<double> intersectingXCoords = new List<double>();
            int j = Vertices.Length - 2;
            double x0, x1, y0, y1, m;
            for (int i = 0; i < Vertices.Length; i += 2, j = i - 2)
            {
                x0 = Vertices[j];
                y0 = Vertices[j + 1];
                x1 = Vertices[i];
                y1 = Vertices[i + 1];
                if (y0 < y && y1 < y || y0 > y && y1 > y)
                    continue;
                if (y1 == y)
                    continue;

                // handle the case that the edge is a straight line parallel to our line
                if (y0 == y1)
                {
                    intersectingXCoords.Add(x0);
                    intersectingXCoords.Add(x1);
                }
                else if (x0 == x1)
                {
                    // we add the x coord because we know the y values of the edge cross the line
                    intersectingXCoords.Add(x0);
                }
                else
                {
                    m = (y1 - y0) / (x1 - x0);
                    intersectingXCoords.Add((y - y0) / m + x0);
                }
            }

            intersectingXCoords.Sort();

            if (intersectingXCoords.Count % 2 == 0)
                return intersectingXCoords.ToArray();
            else
                return intersectingXCoords.Distinct().ToArray(); // Remove repeating x coords when we return as an array
        }
    }
}
