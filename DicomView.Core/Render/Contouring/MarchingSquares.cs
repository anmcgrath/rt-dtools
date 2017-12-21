using System;
using System.Collections.Generic;
using System.Text;
using RT.Core.DICOM;

namespace DicomPanel.Core.Render.Contouring
{
    /// <summary>
    /// Performs a margins squares procedure to get a contour from a grid
    /// </summary>
    public class MarchingSquares
    {
        private double[] vTopLeft = new double[3];
        private double[] vTopRight = new double[3];
        private double[] vBottomLeft = new double[3];
        private double[] vBottomRight = new double[3];
        public MarchingSquares()
        {

        }

        public Contour GetContour(float[][] grid, int rows, int columns, double[][][] coords, double thresholds, DicomColor color)
        {
            Contour contour = new Contour(GetVertices(grid, rows, columns, coords, thresholds), color);
            return contour;
        }

        /// <summary>
        /// Returns a list of lines in the form [x0,y0,z0,x1,y1,z1]
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="grid">The data grid</param>
        /// <param name="coords">Coordinates corresponding 1:1 to the data grid</param>
        /// <returns></returns>
        public double[] GetVertices(float[][] grid, int rows, int columns, double[][][] coords, double threshold)
        {
            List<double> vertices = new List<double>();
            float A, B, C, D;
            for (int row = 1; row < rows; row++)
            {
                for (int col = 1; col < columns; col++)
                {
                    A = grid[row - 1][col - 1];
                    B = grid[row - 1][col];
                    C = grid[row][col - 1];
                    D = grid[row][col];
                    checkSquare(A, B, C, D, row, col, threshold, vertices, coords);
                }
            }

            return vertices.ToArray();
        }

        /// <summary>
        /// Returns a list of lines in the form [x0,y0,z0,x1,y1,z1]
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="grid">The data grid</param>
        /// <param name="coords">Coordinates corresponding 1:1 to the data grid</param>
        /// <returns></returns>
        public double[] GetVertices(bool[] grid, int rows, int columns, double[][][] coords)
        {
            List<double> vertices = new List<double>();
            bool A, B, C, D;
            for (int row = 1; row < rows; row++)
            {
                for (int col = 1; col < columns; col++)
                {
                    A = getBoolValue(grid, row - 1, col - 1, rows, columns);
                    B = getBoolValue(grid, row - 1, col, rows, columns);
                    C = getBoolValue(grid, row, col - 1, rows, columns);
                    D = getBoolValue(grid, row, col, rows, columns);
                    checkSquare(A, B, C, D, row, col, true, vertices, coords);
                }
            }

            return vertices.ToArray();
        }

        private bool getBoolValue(bool[] grid, int row, int col, int rows, int cols)
        {
            if (row > rows - 1 || row < 0 || col < 0 || col > cols - 1)
                return false;
            return grid[row * cols + col];
        }

        /// <summary>
        /// Returns a list of lines in the form [x0,y0,z0,x1,y1,z1]
        /// </summary>
        /// <returns></returns>
        public double[] GetVertices(bool[] grid, int rows, int columns, double x0, double y0, double z0, double rdx, double rdy, double rdz, double cdx, double cdy, double cdz)
        {
            List<double> vertices = new List<double>();
            bool A, B, C, D;
            double x, y, z;
            // Start from -1 and finish at rows so that we avoid edge artefacts
            for (int i = -1; i < rows + 1; i++)
            {
                for (int j = -1; j < columns + 1; j++)
                {
                    var col = j;
                    var row = i;

                    A = getBoolValue(grid, row - 1, col - 1, rows, columns);
                    B = getBoolValue(grid, row - 1, col, rows, columns);
                    C = getBoolValue(grid, row, col - 1, rows, columns);
                    D = getBoolValue(grid, row, col, rows, columns);

                    x = x0 + rdx * row + cdx * col;
                    y = y0 + rdy * row + cdy * col;
                    z = z0 + rdz * row + cdz * col;
                    checkSquare(A, B, C, D, x, y, z, rdx, rdy, rdz, cdx, cdy, cdz, vertices);
                }
            }

            return vertices.ToArray();
        }

        private void checkSquare(bool A, bool B, bool C, bool D, double x, double y, double z, double rdx, double rdy, double rdz, double cdx, double cdy, double cdz, List<double> vertices)
        {
            byte swb = 0, nwb = 0, neb = 0, seb = 0;
            if (C)
                swb = 0b0000_0001;
            if (D)
                seb = 0b0000_0010;
            if (B)
                neb = 0b0000_0100;
            if (A)
                nwb = 0b0000_1000;
            byte lineType = swb;
            lineType |= seb;
            lineType |= neb;
            lineType |= nwb;

            // The corner vectors
            vTopLeft[0] = x; vTopLeft[1] = y; vTopLeft[2] = z;
            vTopRight[0] = x + cdx; vTopRight[1] = y + cdy; vTopRight[2] = z + cdz;
            vBottomRight[0] = x + cdx + rdx; vBottomRight[1] = y + cdy + rdy; vBottomRight[2] = z + cdz + rdz;
            vBottomLeft[0] = x + rdx; vBottomLeft[1] = y + rdy; vBottomLeft[2] = z + rdz;

            addVertices(vertices, lineType, A ? 10 : 0, B ? 10 : 0, C ? 10 : 0, D ? 10 : 0, 5);
        }


        private void checkSquare(bool A, bool B, bool C, bool D, int row, int col, bool threshold, List<double> vertices, double[][][] coords)
        {
            byte swb = 0, nwb = 0, neb = 0, seb = 0;
            if (C  == threshold)
                swb = 0b0000_0001;
            if (D == threshold)
                seb = 0b0000_0010;
            if (B == threshold)
                neb = 0b0000_0100;
            if (A == threshold)
                nwb = 0b0000_1000;
            byte lineType = swb;
            lineType |= seb;
            lineType |= neb;
            lineType |= nwb;

            // The corner vectors
            vTopLeft = coords[row - 1][col - 1];
            vTopRight = coords[row - 1][col];
            vBottomRight = coords[row][col];
            vBottomLeft = coords[row][col - 1];
            addVertices(vertices, lineType, A?10:0, B?10:0, C?10:0, D?10:0, 5);
        }


        private void checkSquare(float A, float B, float C, float D, int row, int col, double threshold, List<double> vertices, double[][][] coords)
        {
            byte swb = 0, nwb = 0, neb = 0, seb = 0;
            if (C > threshold)
                swb = 0b0000_0001;
            if (D > threshold)
                seb = 0b0000_0010;
            if (B > threshold)
                neb = 0b0000_0100;
            if (A > threshold)
                nwb = 0b0000_1000;
            byte lineType = swb;
            lineType |= seb;
            lineType |= neb;
            lineType |= nwb;

            // The corner vectors
            vTopLeft = coords[row - 1][col - 1];
            vTopRight = coords[row - 1][col];
            vBottomRight = coords[row][col];
            vBottomLeft = coords[row][col - 1];
            addVertices(vertices, lineType, A, B, C, D, threshold);
        }

        private void addVertices(List<double> vertices, byte lineType, float topLeft, float topRight, float bottomRight, float bottomLeft, double threshold)
        {
            double x0 = 0, x1 = 0, y0 = 0, y1 = 0, z0 = 0, z1 = 0;

            if (lineType == 1 || lineType == 14)
            {
                x0 = (vBottomLeft[0] + (vTopLeft[0] - vBottomLeft[0]) * ((threshold - bottomRight) / (topLeft - bottomRight)));
                y0 = (vBottomLeft[1] + (vTopLeft[1] - vBottomLeft[1]) * ((threshold - bottomRight) / (topLeft - bottomRight)));
                z0 = (vBottomLeft[2] + (vTopLeft[2] - vBottomLeft[2]) * ((threshold - bottomRight) / (topLeft - bottomRight)));
                x1 = (vBottomLeft[0] + (vBottomRight[0] - vBottomLeft[0]) * ((threshold - bottomRight) / (bottomLeft - bottomRight)));
                y1 = (vBottomLeft[1] + (vBottomRight[1] - vBottomLeft[1]) * ((threshold - bottomRight) / (bottomLeft - bottomRight)));
                z1 = (vBottomLeft[2] + (vBottomRight[2] - vBottomLeft[2]) * ((threshold - bottomRight) / (bottomLeft - bottomRight)));
            }
            else if (lineType == 2 || lineType == 13)
            {
                x1 = (vTopRight[0] + (vBottomRight[0] - vTopRight[0]) * ((threshold - topRight) / (bottomLeft - topRight)));
                y1 = (vTopRight[1] + (vBottomRight[1] - vTopRight[1]) * ((threshold - topRight) / (bottomLeft - topRight)));
                z1 = (vTopRight[2] + (vBottomRight[2] - vTopRight[2]) * ((threshold - topRight) / (bottomLeft - topRight)));
                x0 = (vBottomLeft[0] + (vBottomRight[0] - vBottomLeft[0]) * ((threshold - bottomRight) / (bottomLeft - bottomRight)));
                y0 = (vBottomLeft[1] + (vBottomRight[1] - vBottomLeft[1]) * ((threshold - bottomRight) / (bottomLeft - bottomRight)));
                z0 = (vBottomLeft[2] + (vBottomRight[2] - vBottomLeft[2]) * ((threshold - bottomRight) / (bottomLeft - bottomRight)));
            }
            else if (lineType == 3 || lineType == 12)
            {
                x0 = (vBottomLeft[0] + (vTopLeft[0] - vBottomLeft[0]) * ((threshold - bottomRight) / (topLeft - bottomRight)));
                y0 = (vBottomLeft[1] + (vTopLeft[1] - vBottomLeft[1]) * ((threshold - bottomRight) / (topLeft - bottomRight)));
                z0 = (vBottomLeft[2] + (vTopLeft[2] - vBottomLeft[2]) * ((threshold - bottomRight) / (topLeft - bottomRight)));
                x1 = (vTopRight[0] + (vBottomRight[0] - vTopRight[0]) * ((threshold - topRight) / (bottomLeft - topRight)));
                y1 = (vTopRight[1] + (vBottomRight[1] - vTopRight[1]) * ((threshold - topRight) / (bottomLeft - topRight)));
                z1 = (vTopRight[2] + (vBottomRight[2] - vTopRight[2]) * ((threshold - topRight) / (bottomLeft - topRight)));
            }
            else if (lineType == 4 || lineType == 11)
            {
                x0 = (vTopLeft[0] + (vTopRight[0] - vTopLeft[0]) * ((threshold - topLeft) / (topRight - topLeft)));
                y0 = (vTopLeft[1] + (vTopRight[1] - vTopLeft[1]) * ((threshold - topLeft) / (topRight - topLeft)));
                z0 = (vTopLeft[2] + (vTopRight[2] - vTopLeft[2]) * ((threshold - topLeft) / (topRight - topLeft)));
                x1 = vTopRight[0] + (vBottomRight[0] - vTopRight[0]) * ((threshold - topRight) / (bottomLeft - topRight));
                y1 = vTopRight[1] + (vBottomRight[1] - vTopRight[1]) * ((threshold - topRight) / (bottomLeft - topRight));
                z1 = vTopRight[2] + (vBottomRight[2] - vTopRight[2]) * ((threshold - topRight) / (bottomLeft - topRight));
            }
            else if (lineType == 5)
            {
                x0 = (vBottomLeft[0] + (vTopLeft[0] - vBottomLeft[0]) * ((threshold - bottomRight) / (topLeft - bottomRight)));
                y0 = (vBottomLeft[1] + (vTopLeft[1] - vBottomLeft[1]) * ((threshold - bottomRight) / (topLeft - bottomRight)));
                z0 = (vBottomLeft[2] + (vTopLeft[2] - vBottomLeft[2]) * ((threshold - bottomRight) / (topLeft - bottomRight)));
                x1 = (vTopLeft[0] + (vTopRight[0] - vTopLeft[0]) * ((threshold - topLeft) / (topRight - topLeft)));
                y1 = (vTopLeft[1] + (vTopRight[1] - vTopLeft[1]) * ((threshold - topLeft) / (topRight - topLeft)));
                z1 = (vTopLeft[2] + (vTopRight[2] - vTopLeft[2]) * ((threshold - topLeft) / (topRight - topLeft)));
                vertices.AddRange(new double[] { x0, y0, z0, x1, y1, z1 });
                x0 = (vBottomLeft[0] + (vBottomRight[0] - vBottomLeft[0]) * ((threshold - bottomRight) / (bottomLeft - bottomRight)));
                y0 = (vBottomLeft[1] + (vBottomRight[1] - vBottomLeft[1]) * ((threshold - bottomRight) / (bottomLeft - bottomRight)));
                z0 = (vBottomLeft[2] + (vBottomRight[2] - vBottomLeft[2]) * ((threshold - bottomRight) / (bottomLeft - bottomRight)));
                x1 = (vTopRight[0] + (vBottomRight[0] - vTopRight[0]) * ((threshold - topRight) / (bottomLeft - topRight)));
                y1 = (vTopRight[1] + (vBottomRight[1] - vTopRight[1]) * ((threshold - topRight) / (bottomLeft - topRight)));
                z1 = (vTopRight[2] + (vBottomRight[2] - vTopRight[2]) * ((threshold - topRight) / (bottomLeft - topRight)));
            }
            else if (lineType == 6 || lineType == 9)
            {
                x0 = (vTopLeft[0] + (vTopRight[0] - vTopLeft[0]) * ((threshold - topLeft) / (topRight - topLeft)));
                y0 = (vTopLeft[1] + (vTopRight[1] - vTopLeft[1]) * ((threshold - topLeft) / (topRight - topLeft)));
                z0 = (vTopLeft[2] + (vTopRight[2] - vTopLeft[2]) * ((threshold - topLeft) / (topRight - topLeft)));
                x1 = (vBottomLeft[0] + (vBottomRight[0] - vBottomLeft[0]) * ((threshold - bottomRight) / (bottomLeft - bottomRight)));
                y1 = (vBottomLeft[1] + (vBottomRight[1] - vBottomLeft[1]) * ((threshold - bottomRight) / (bottomLeft - bottomRight)));
                z1 = (vBottomLeft[2] + (vBottomRight[2] - vBottomLeft[2]) * ((threshold - bottomRight) / (bottomLeft - bottomRight)));
            }
            else if (lineType == 7 || lineType == 8)
            {
                x0 = (vBottomLeft[0] + (vTopLeft[0] - vBottomLeft[0]) * ((threshold - bottomRight) / (topLeft - bottomRight)));
                y0 = (vBottomLeft[1] + (vTopLeft[1] - vBottomLeft[1]) * ((threshold - bottomRight) / (topLeft - bottomRight)));
                z0 = (vBottomLeft[2] + (vTopLeft[2] - vBottomLeft[2]) * ((threshold - bottomRight) / (topLeft - bottomRight)));
                x1 = (vTopLeft[0] + (vTopRight[0] - vTopLeft[0]) * ((threshold - topLeft) / (topRight - topLeft)));
                y1 = (vTopLeft[1] + (vTopRight[1] - vTopLeft[1]) * ((threshold - topLeft) / (topRight - topLeft)));
                z1 = (vTopLeft[2] + (vTopRight[2] - vTopLeft[2]) * ((threshold - topLeft) / (topRight - topLeft)));
            }
            else if (lineType == 10)
            {
                /*Py = (Cy + (Ay - Cy) * ((threshold - C) / (A - C)));
                Px = Ax;
                Qx = (Cx + (Dx - Cx) * ((threshold - C) / (D - C)));
                Qy = Cy;
                vertices.AddRange(new double[] { x0, y0, z0, x1, y1, z1 });
                Px = (Ax + (Bx - Ax) * ((threshold - A) / (B - A)));
                Py = Ay;
                Qx = Bx;
                Qy = (By + (Dy - By) * ((threshold - B) / (D - B)));*/
            }

            if (lineType != 0 && lineType != 15)
                vertices.AddRange(new double[] { x0, y0, z0, x1, y1, z1 });
        }
    }
}
