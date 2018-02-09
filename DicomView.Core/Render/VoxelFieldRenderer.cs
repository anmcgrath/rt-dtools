using RT.Core.DICOM;
using RT.Core.Geometry;
using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Render
{
    public class VoxelFieldRenderer
    {
        private Point3d worldCoord = new Point3d();
        private Point2d screenCoords1 = new Point2d();
        private Point2d screenCoords2 = new Point2d();

        public void Render(VectorField vectorField, Camera camera, IRenderContext context, Rectd screenRect)
        {
            var boundingRect = camera.GetBoundingScreenRect(vectorField.X.XRange, vectorField.X.YRange, vectorField.X.ZRange, screenRect);

            if (boundingRect != null)
            {
                var initPosn = camera.ConvertScreenToWorldCoords(boundingRect.Y, boundingRect.X);

                double rows = 80;
                double cols = 80;
                double gridSpacingRows = rows / camera.GetFOV().Y;
                double gridSpacingCols = cols / camera.GetFOV().X;
                double gridSpacing = Math.Min(gridSpacingRows, gridSpacingCols);

                var vertices = new List<double>();

                var cnorm = camera.ColDir.Length();
                var rnorm = camera.RowDir.Length();
                var cx = (cnorm * camera.ColDir.X) * gridSpacingCols;
                var cy = (cnorm * camera.ColDir.Y) * gridSpacingCols;
                var cz = (cnorm * camera.ColDir.Z) * gridSpacingCols;
                var rx = (rnorm * camera.RowDir.X) * gridSpacingRows;
                var ry = (rnorm * camera.RowDir.Y) * gridSpacingRows;
                var rz = (rnorm * camera.RowDir.Z) * gridSpacingRows;

                for (int i = 0; i < cols; i ++)
                {
                    for (int j = 0; j < rows; j ++)
                    {
                        worldCoord.X = initPosn.X + cx * i + rx * j + cx + rx;
                        worldCoord.Y = initPosn.Y + cy * i + ry * j + cy + ry;
                        worldCoord.Z = initPosn.Z + cz * i + rz * j + cz + rz;

                        var x = vectorField.X.Interpolate(worldCoord).Value;
                        var y = vectorField.Y.Interpolate(worldCoord).Value;
                        var z = vectorField.Z.Interpolate(worldCoord).Value;

                        float len = (float)Math.Sqrt(x * x + y * y + z * z);

                        if (len <= 0.01)
                            continue;

                        // The additional cx, rx, cy, ry etc. are because the marching squares
                        // offsets the grid during calculation... trust me future self.

                        vertices.Add(worldCoord.X);
                        vertices.Add(worldCoord.Y);
                        vertices.Add(worldCoord.Z);
                        
                        x /= len;
                        y /= len;
                        z /= len;

                        vertices.Add(x*gridSpacing + worldCoord.X);
                        vertices.Add(y*gridSpacing + worldCoord.Y);
                        vertices.Add(z*gridSpacing + worldCoord.Z);
                    }
                }

                double[] screenVertices = getScreenVertices(vertices, camera);

                for(int i = 0; i < screenVertices.Length; i+=4)
                {
                    double x0 = screenVertices[i];
                    double y0 = screenVertices[i + 1];
                    double x1 = screenVertices[i + 2];
                    double y1 = screenVertices[i + 3];
                    context.DrawArrow(x0, y0, x1, y1, DicomColors.Red);
                    
                }
                //context.DrawLines(screenVertices, DicomColors.Red);

            }
        }

        private double[] getScreenVertices(List<double> vertices, Camera camera)
        {
            double[] screenVertices = new double[2 * vertices.Count / 3];

            for (int i = 0, j = 0; i < vertices.Count; i += 6, j += 4)
            {
                camera.ConvertWorldToScreenCoords(vertices[i + 0], vertices[i + 1], vertices[i + 2], screenCoords1);
                camera.ConvertWorldToScreenCoords(vertices[i + 3], vertices[i + 4], vertices[i + 5], screenCoords2);
                screenVertices[j] = screenCoords1.X;
                screenVertices[j + 1] = screenCoords1.Y;
                screenVertices[j + 2] = screenCoords2.X;
                screenVertices[j + 3] = screenCoords2.Y;
            }
            return screenVertices;
        }
    }
}
