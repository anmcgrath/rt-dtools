using System;
using System.Collections.Generic;
using System.Text;

namespace RT.Core.Utilities.RTMath
{
    public class Histogram<T> where T:IComparable
    {
        public Histogram()
        {
            
        }

        public void Create(IEnumerable<T> data, T max, T min)
        {
            foreach(var dataPoint in data)
            {

            }
        }

        public void Add(T dataPoint)
        {

        }

    }
}
