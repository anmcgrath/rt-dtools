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

namespace DicomPanel.Core.Render
{
    public class ImageRenderer
    {
        public void Render(IVoxelDataStructure image, Camera camera, IRenderContext context, Rectd screenRect, ILUT lut, double alpha)
        {
            Render(new List<IVoxelDataStructure>() { image },
                camera,
                context,
                screenRect,
                new List<ILUT>() { lut },
                new List<double>() { alpha });
        }
        public void Render(List<IVoxelDataStructure> images, Camera camera, IRenderContext context, Rectd screenRect, List<ILUT> luts, List<double> alphas)
        {
            if (context == null)
                return;

            // This render function loops through each pixel in the camera's FOV,
            // converts the location to a location in the patient coordinate system,
            // then store in a byte array to render to the image layer.
            int rows = (int)(screenRect.Height * context.Height);
            int cols = (int)(screenRect.Width * context.Width);

            int bytespp = 4;

            // Get the direction we move in each iteration from the top left of the camera
            Point3d initPosn = camera.ConvertScreenToWorldCoords(screenRect.Y, screenRect.X);
            Point3d posn = camera.ConvertScreenToWorldCoords(screenRect.Y, screenRect.X);

            Queue<byte[]> byteArrays = new Queue<byte[]>();
            //Construct a list of parameters, we will later pass one to each thread.
            List<ImageRendererThreadParameters> threadParams = new List<ImageRendererThreadParameters>();
            for(int i = 0; i < images.Count; i++)
            {
                if (images[i] != null)
                {
                    ImageRendererThreadParameters param = new ImageRendererThreadParameters()
                    {
                        ix = initPosn.X,
                        iy = initPosn.Y,
                        iz = initPosn.Z,
                        px = initPosn.X,
                        py = initPosn.Y,
                        pz = initPosn.Z,
                        cx = screenRect.Width * camera.ColDir.X * camera.GetFOV().X / (cols * camera.Scale * camera.MMPerPixel),
                        cy = screenRect.Width * camera.ColDir.Y * camera.GetFOV().X / (cols * camera.Scale * camera.MMPerPixel),
                        cz = screenRect.Width * camera.ColDir.Z * camera.GetFOV().X / (cols * camera.Scale * camera.MMPerPixel),
                        rx = screenRect.Height * camera.RowDir.X * camera.GetFOV().Y / (rows * camera.Scale * camera.MMPerPixel),
                        ry = screenRect.Height * camera.RowDir.Y * camera.GetFOV().Y / (rows * camera.Scale * camera.MMPerPixel),
                        rz = screenRect.Height * camera.RowDir.Z * camera.GetFOV().Y / (rows * camera.Scale * camera.MMPerPixel),
                        alpha = (float)alphas[i],
                        byteArrays = byteArrays,
                        cols = cols,
                        rows = rows,
                        grid = images[i],
                        lut = luts[i],
                    };
                    threadParams.Add(param);
                }
            }

            List<Thread> threads = new List<Thread>();
            foreach(var param in threadParams)
            {
                /*var threadStart = new ParameterizedThreadStart(Compute);
                var thread = new Thread(threadStart);
                threads.Add(thread);
                thread.Start(param);*/
                Compute(param);
            }
            foreach(var thread in threads)
            {
                thread.Join();
            }

            byte[] result = new byte[rows * cols * bytespp];
            byte[][] arrays = byteArrays.ToArray();
            if (arrays.Length == 1)
            {
                result = arrays[0];
            }
            else
            {
                for (int i = 0; i < result.Length; i+=4)
                {
                    for (int j = 1; j < arrays.Length; j++)
                    {
                        double val1 = arrays[j - 1][i] * alphas[j - 1] + arrays[j][i] * alphas[j];
                        double val2 = arrays[j - 1][i+1] * alphas[j - 1] + arrays[j][i+1] * alphas[j];
                        double val3 = arrays[j - 1][i+2] * alphas[j - 1] + arrays[j][i+2] * alphas[j];

                        if (val1 > 255)
                            val1 = 255;
                        if (val2 > 255)
                            val2 = 255;
                        if (val3 > 255)
                            val3 = 255;
                        
                        result[i] = (byte)val1;
                        result[i + 1] = (byte)val2;
                        result[i + 2] = (byte)val3;
                        result[i + 3] = 255;
                    }
                }
            }

            if (result.Length > 0)
            {
                context.FillPixels(result, screenRect);
            }

            result = null;
        }

        private void Compute(object p)
        {
            ImageRendererThreadParameters param = (ImageRendererThreadParameters)p;
            int bytespp = 4;
            byte[] bytearray = new byte[param.rows * param.cols * bytespp];
            int k = 0;
            float value;
            byte blue, green, red;
            byte[] bgr = new byte[3];
            Voxel interpolatedVoxel = new Voxel();

            for (int r = 0; r < param.rows; r += 1)
            {
                param.px = param.ix + param.rx * r; param.py = param.iy + param.ry * r; param.pz = param.iz + param.rz * r;
                for (int c = 0; c < param.cols; c += 1)
                {
                    param.grid.Interpolate(param.px, param.py, param.pz, interpolatedVoxel);
                    value = interpolatedVoxel.Value;
                    param.lut.Compute(value, bgr);
                    blue = (byte)(bgr[0]);
                    green = (byte)(bgr[1]);
                    red = (byte)(bgr[2]);

                    bytearray[k] = blue;
                    bytearray[k + 1] = green;
                    bytearray[k + 2] = red;
                    bytearray[k + 3] = 255;
                    k += bytespp;
                    param.px += param.cx; param.py += param.cy; param.pz += param.cz;
                }
            }

            param.byteArrays.Enqueue(bytearray);
        }
    }

    internal class ImageRendererThreadParameters
    {
        internal double ix, iy, iz;
        internal double px, py, pz;
        internal double cx, cy, cz;
        internal double rx, ry, rz;
        internal int rows, cols;
        internal IVoxelDataStructure grid;
        internal ILUT lut;
        internal float alpha;
        internal Queue<byte[]> byteArrays;
    }
}
