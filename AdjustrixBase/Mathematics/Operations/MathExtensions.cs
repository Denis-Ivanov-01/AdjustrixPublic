using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdjustrixBase.Mathematics.Operations
{
    public static class MathExtensions
    {
        /// <summary>
        /// Computes the square root of a decimal value using Newton's (Babylonian) method.
        /// </summary>
        /// <param name="value">The decimal number to find the square root of.</param>
        /// <param name="tolerance">The precision tolerance (default: 1e-28m for high precision).</param>
        /// <returns>The square root of the given decimal number.</returns>
        public static decimal Sqrt(decimal value, decimal tolerance = 1e-28m)
        {
            if (value < 0)
                throw new ArgumentException("Cannot compute square root of a negative number.");

            if (value == 0 || value == 1)
                return value; // Quick return for 0 and 1

            decimal x = value; // Initial guess (start with the value itself)
            decimal lastX;

            do
            {
                lastX = x;
                x = (x + value / x) / 2; // Newton's iteration step
            }
            while (Math.Abs(x - lastX) > tolerance);

            return x;
        }
    }
}
