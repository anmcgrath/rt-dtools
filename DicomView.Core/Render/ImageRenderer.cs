using RT.Core.Imaging;
using RT.Core.Imaging.LUT;
using RT.Core.Utilities;
using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using RT.Core.Geometry;
using RT.Core.DICOM;

namespace DicomPanel.Core.Render
{
    public class ImageRenderer
    {
        private int totalRows;
        private int totalCols;
        private Rectd TotalScreenRect;
        private int bytespp = 4;
        private byte[] buffer;
        private double alpha = 1;
        //here for debugging... remove and move to local function when done
        public void BeginRender(Rectd screenRect, IRenderContext context)
        {
            if (context == null)
                return;
            totalRows = (int)(screenRect.Height * context.Height);
            totalCols = (int)(screenRect.Width * context.Width);
            TotalScreenRect = screenRect;
            buffer = new byte[totalRows * totalCols * bytespp];

        }
        public void EndRender(Rectd screenRect, IRenderContext context)
        {
            if (context == null)
                return;
            if (buffer.Length > 0)
                context.FillPixels(buffer, screenRect);
        }

        public void Render(RenderableImage image, Camera camera, IRenderContext context)
        {
            if (image.Grid == null)
                return;

            // Get the direction we move in each iteration from the top left of the camera
            Rectd screenRect = camera.GetBoundingScreenRect(image.Grid.XRange, image.Grid.YRange, image.Grid.ZRange, image.ScreenRect);
            //Rectd screenRect = new Rectd(0,0,1,1);

            if (screenRect == null)
                return;

            //align the screenRect position to a pixel to avoid shifting artefacts.
            screenRect.X -= (screenRect.X) % (1.0f / context.Width);
            screenRect.Y -= (screenRect.Y) % (1.0f / context.Height);

            Point3d initPosn = camera.ConvertScreenToWorldCoords(screenRect.Y, screenRect.X);
            Point3d posn = camera.ConvertScreenToWorldCoords(screenRect.Y, screenRect.X);

            int rows = (int)Math.Round(screenRect.Height * context.Height);
            int cols = (int)Math.Round(screenRect.Width * context.Width);
            int startingRow = (int)Math.Round((screenRect.Y / TotalScreenRect.Height) * totalRows);
            int startingCol = (int)Math.Round((screenRect.X / TotalScreenRect.Width) * totalCols);

            double ix, iy, iz, px, py, pz, cx, cy, cz, rx, ry, rz;
            ix = initPosn.X;
            iy = initPosn.Y;
            iz = initPosn.Z;
            px = initPosn.X;
            py = initPosn.Y;
            pz = initPosn.Z;
            cx = screenRect.Width * camera.ColDir.X * camera.GetFOV().X / (cols * camera.Scale * camera.MMPerPixel);
            cy = screenRect.Width * camera.ColDir.Y * camera.GetFOV().X / (cols * camera.Scale * camera.MMPerPixel);
            cz = screenRect.Width * camera.ColDir.Z * camera.GetFOV().X / (cols * camera.Scale * camera.MMPerPixel);
            rx = screenRect.Height * camera.RowDir.X * camera.GetFOV().Y / (rows * camera.Scale * camera.MMPerPixel);
            ry = screenRect.Height * camera.RowDir.Y * camera.GetFOV().Y / (rows * camera.Scale * camera.MMPerPixel);
            rz = screenRect.Height * camera.RowDir.Z * camera.GetFOV().Y / (rows * camera.Scale * camera.MMPerPixel);

            int k;
            int dr = 0;
            float value;
            byte blue, green, red;
            byte actualBlue = 0, actualGreen = 0, actualRed = 0;
            byte actualAlpha = (byte)(255 * alpha);
            byte[] bgr = new byte[3];
            Voxel interpolatedVoxel = new Voxel();
            double val1, val2, val3;
            var norm = image.Grid.GetNormalisationAmount();

            for (int r = startingRow; r < rows + startingRow; r += 1)
            {
                dr++;
                k = r * totalCols * bytespp + startingCol * bytespp;
                px = ix + rx * dr; py = iy + ry * dr; pz = iz + rz * dr;
                for (int c = startingCol; c < cols + startingCol; c += 1)
                {
                    image.Grid.Interpolate(px, py, pz, interpolatedVoxel);
                    value = interpolatedVoxel.Value * image.Grid.Scaling;
                    image.LUT.Compute(value, bgr);

                    blue = (byte)(bgr[0]);
                    green = (byte)(bgr[1]);
                    red = (byte)(bgr[2]);

                    switch (image.BlendMode)
                    {
                        case Blending.BlendMode.None:
                            actualRed = red;
                            actualGreen = green;
                            actualBlue = blue; break;
                        case Blending.BlendMode.Over:
                            val1 = buffer[k] * alpha + blue * image.Alpha;
                            val2 = buffer[k + 1] * (1 - image.Alpha) + green * image.Alpha;
                            val3 = buffer[k + 2] * (1 - image.Alpha) + red * image.Alpha;
                            if (val1 > 255) val1 = 255;
                            if (val2 > 255) val2 = 255;
                            if (val3 > 255) val3 = 255;
                            actualBlue = (byte)val1;
                            actualGreen = (byte)val2;
                            actualRed = (byte)val3;
                            break;
                        case Blending.BlendMode.OverWhereNonZero:
                            if (blue == 0)
                                actualBlue = buffer[k];
                            else
                            {
                                val1 = buffer[k] * alpha + blue * image.Alpha;
                                if (val1 > 255) val1 = 255;
                                actualBlue = (byte)val1;
                            }
                            if (green == 0)
                                actualGreen = buffer[k + 1];
                            else
                            {
                                val2 = buffer[k + 1] * alpha + green * image.Alpha;
                                if (val2 > 255) val2 = 255;
                                actualGreen = (byte)val2;
                            }

                            if (red == 0)
                                actualRed = buffer[k + 2];
                            else
                            {
                                val3 = buffer[k + 2] * alpha + red * image.Alpha;
                                if (val3 > 255) val3 = 255;
                                actualRed = (byte)val3;
                            }
                            break;
                    }

                    buffer[k] = actualBlue;
                    buffer[k + 1] = actualGreen;
                    buffer[k + 2] = actualRed;
                    buffer[k + 3] = actualAlpha;

                    k += bytespp;
                    px += cx; py += cy; pz += cz;
                }
            }
        }
    }
}
