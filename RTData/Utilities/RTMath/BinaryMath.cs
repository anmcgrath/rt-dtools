using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTData.Utilities.RTMath
{
    class BinaryMath
    {
        /// <summary>
        /// Returns a bool grid with the same size but opposite boolean values
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool[] Not(bool[] data)
        {
            bool[] not = new bool[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                not[i] = !data[i];
            }
            return not;
        }

        /// <summary>
        /// Applies an xor with grid2 to the grid "data"
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        public static bool[] Xor(bool[] grid1, bool[] grid2)
        {
            bool[] xor = new bool[grid1.Length];
            for (int i = 0; i < xor.Length; i++)
            {
                xor[i] = grid1[i] ^ grid2[i];
            }
            return xor;
        }

        /// <summary>
        /// Binary search for the index of a value. If the value is in the array, the index is returned,
        /// otherwise the returned index is the index of the next highest value.
        /// </summary>
        /// <param name="value">The value to search for</param>
        /// <param name="array">The array to search in</param>
        /// <returns></returns>
        public static int BinarySearchClosest<T>(T value, List<T> array) where T : IComparable
        {
            int index = array.BinarySearch(value);
            if (index < 0)
                return ~index;
            return index;
        }

        /// <summary>
        /// Performs a distance transform of a binary mask, with a positive value if mask point is true and negative otherwises.
        /// Adapted from https://cs.brown.edu/~pff/dt/
        /// </summary>
        /// <param name="dataInput"></param>
        /// <param name="dataOutput"></param>
        public static float[] DistanceTransform(bool[] input)
        {
            int n = input.Length;
            float[] f = new float[n];
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i])
                    f[i] = float.PositiveInfinity;
                else
                    f[i] = 0;

            }
            float[] d = new float[n];
            int[] v = new int[n];
            float[] z = new float[n + 1];
            int k = 0;
            v[0] = 0;
            z[0] = float.NegativeInfinity;
            z[1] = +float.PositiveInfinity;
            for (int q = 1; q <= n - 1; q++)
            {
                float s = ((f[q] + q * q) - (f[v[k]] + v[k] * v[k])) / (2 * q - 2 * v[k]);
                while (s <= z[k])
                {
                    k--;
                    s = ((f[q] + q * q) - (f[v[k]] + v[k] * v[k])) / (2 * q - 2 * v[k]);
                }
                k++;
                v[k] = q;
                z[k] = s;
                z[k + 1] = float.PositiveInfinity;
            }

            k = 0;
            for (int q = 0; q <= n - 1; q++)
            {
                while (z[k + 1] < q)
                    k++;
                d[q] = (q - v[k]) * (q - v[k]) + f[v[k]];
            }

            for (int i = 0; i < n; i++)
            {
                if (input[i])
                    d[i] = -Math.Abs(d[i]);
                else
                    d[i] = Math.Abs(d[i]);
            }
            return d;
        }
    }
}
