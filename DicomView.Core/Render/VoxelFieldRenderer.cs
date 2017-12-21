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
                double gridSpacing = 5;

                var offX = initPosn.X % (1.0 / (gridSpacing * 2 * camera.Scale / camera.GetFOV().X));
                var offY = initPosn.Y % (1.0 / (gridSpacing * 2 * camera.Scale / camera.GetFOV().Y));
                initPosn.X -= offX;
                initPosn.Y -= offY;

                var rows = (int)Math.Round((boundingRect.Height * (camera.GetFOV().Y) / gridSpacing)) + 4;
                var cols = (int)Math.Round((boundingRect.Width * (camera.GetFOV().X) / gridSpacing)) + 4;

                var vertices = new List<double>();

                var cnorm = camera.ColDir.Length();
                var rnorm = camera.RowDir.Length();
                var cx = (cnorm * camera.ColDir.X) * gridSpacing / camera.Scale;
                var cy = (cnorm * camera.ColDir.Y) * gridSpacing / camera.Scale;
                var cz = (cnorm * camera.ColDir.Z) * gridSpacing / camera.Scale;
                var rx = (rnorm * camera.RowDir.X) * gridSpacing / camera.Scale;
                var ry = (rnorm * camera.RowDir.Y) * gridSpacing / camera.Scale;
                var rz = (rnorm * camera.RowDir.Z) * gridSpacing / camera.Scale;

                for (int i = 0; i < cols; i+=2)
                {
                    for (int j = 0; j < rows; j+=2)
                    {
                        // The additional cx, rx, cy, ry etc. are because the marching squares
                        // offsets the grid during calculation... trust me future self.
                        worldCoord.X = initPosn.X + cx * i + rx * j + cx + rx;
                        worldCoord.Y = initPosn.Y + cy * i + ry * j + cy + ry;
                        worldCoord.Z = initPosn.Z + cz * i + rz * j + cz + rz;

                        vertices.Add(worldCoord.X);
                        vertices.Add(worldCoord.Y);
                        vertices.Add(worldCoord.Z);

                        vertices.Add(vectorField.X.Interpolate(worldCoord).Value + worldCoord.X);
                        vertices.Add(vectorField.Y.Interpolate(worldCoord).Value + worldCoord.Y);
                        vertices.Add(vectorField.Z.Interpolate(worldCoord).Value + worldCoord.Z);
                    }
                }

                double[] screenVertices = getScreenVertices(vertices, camera);
                context.DrawLines(screenVertices, DicomColors.Red);

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
