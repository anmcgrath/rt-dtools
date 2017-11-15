using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.Geometry
{
    public class Range
    {
        /// <summary>
        /// The range minimum
        /// </summary>
        public double Minimum { get; set; } = double.MaxValue;
        /// <summary>
        /// The range maximum
        /// </summary>
        public double Maximum { get; set; } = double.MinValue;
        /// <summary>
        /// The entire length of the range
        /// </summary>
        public double Length { get { return Maximum - Minimum; } }

        public Range() { }

        /// <summary>
        /// Initiates a range and sets the minimum to Min(num1,num2) and the maximum to Max(num1,num2)
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        public Range(double num1, double num2) : this(num1, num2, true) { }

        /// <summary>
        /// Initiates a range
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        /// <param name="autoRange">Whether or not to automatically automatically set max/min based on the actual max/min numbers</param>
        public Range(double num1, double num2, bool autoRange)
        {
            if (autoRange)
            {
                Minimum = Math.Min(num1, num2);
                Maximum = Math.Max(num1, num2);
            }
            else
            {
                Minimum = num1;
                Maximum = num2;
            }
        }

        /// <summary>
        /// Returns whether a number is surrounded (contained) by the range
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public bool Contains(double number)
        {
            return number >= Minimum && number <= Maximum;
        }

        /// <summary>
        /// Returns whether the entire range is less than another range
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public bool LessThan(Range range)
        {
            return range.Minimum > range.Maximum;
        }

        /// <summary>
        /// Returns whether the entire range is greater than another range
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public bool GreaterThan(Range range)
        {
            return Minimum > range.Maximum;
        }

        public bool Intersects(Range range)
        {
            return range.Contains(Minimum) || range.Contains(Maximum);
        }

        public double GetCentre()
        {
            return (Minimum + Maximum) / 2;
        }
    }
}
