// <copyright file="NumberExtensions.cs" company="Edge Extensions Project">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Kevin Nessland</author>
// <email>kevinnessland@gmail.com</email>
// <date>2009-07-08</date>
// <summary>Contains Integer-related extension methods.</summary>

using System;

namespace Creek.Extensions
{
    /// <summary>
    /// Contains various number-related extensions.
    /// </summary>
    public static class NumberExtensions
    {
        /// <summary>
        /// Determines if a number lies between two numbers.  Non-inclusive of the low- and high-bound numbers.
        /// </summary>
        /// <param name="i">Integer to use in comparison.</param>
        /// <param name="lowerBound">The low-bound number.</param>
        /// <param name="upperBound">The high-bound number.</param>
        /// <returns>Results of the comparison.</returns>
        public static bool IsBetween(this int i, int lowerBound, int upperBound)
        {
            return i.IsBetween(lowerBound, upperBound, false);
        }

        /// <summary>
        /// Determines if a number lies between two numbers.
        /// </summary>
        /// <param name="i">Integer to use in comparison.</param>
        /// <param name="lowerBound">The low-bound number.</param>
        /// <param name="upperBound">The high-bound number.</param>
        /// <param name="includeBounds">If true, includes low- and high-bound numbers in the comparison.</param>
        /// <returns>Results of the comparison.</returns>
        public static bool IsBetween(this int i, int lowerBound, int upperBound, bool includeBounds)
        {
            if (includeBounds)
            {
                return i >= lowerBound && i <= upperBound;
            }
            else
            {
                return i > lowerBound && i < upperBound;
            }
        }

        /// <summary>
        /// Determines if the number is prime.
        /// </summary>
        /// <param name="i">Integer to inspect.</param>
        /// <returns>Results of the determination.</returns>
        public static bool IsPrime(this int i)
        {
            if ((i % 2) == 0)
            {
                return i == 2;
            }

            int sqrt = (int)Math.Sqrt(i);

            for (int t = 3; t <= sqrt; t = t + 2)
            {
                if (i % t == 0)
                {
                    return false;
                }
            }

            return i != 1;
        }

        /// <summary>
        /// Calculates a value's percentage of another value.
        /// </summary>
        /// <param name="value">Value to compare.</param>
        /// <param name="totalValue">Total value to compare original to.</param>
        /// <returns>Integer percentage value.</returns>
        public static int PercentageOf(this double value, double totalValue)
        {
            return Convert.ToInt32(value * 100 / totalValue);
        }

        /// <summary>
        /// Calculates a value's percentage of another value.
        /// </summary>
        /// <param name="value">Value to compare.</param>
        /// <param name="totalValue">Total value to compare original to.</param>
        /// <returns>Integer percentage value.</returns>
        public static int PercentageOf(this long value, long totalValue)
        {
            return Convert.ToInt32(value * 100 / totalValue);
        }

        /// <summary>
        /// Rounds a double value.
        /// </summary>
        /// <param name="value">Value to round.</param>
        /// <returns>The results of the rounding operation.</returns>
        public static long Round(this double value)
        {
            if (value >= 0)
            {
                return (long)Math.Floor(value);
            }

            return (long)Math.Ceiling(value);
        }

        /// <summary>
        /// Rounds the supplied decimal to the specified amount of decimal points.
        /// </summary>
        /// <param name="val">The decimal to round.</param>
        /// <param name="decimalPoints">The number of decimal points to round the output value to.</param>
        /// <returns>A rounded decimal.</returns>
        public static decimal RoundDecimalPoints(this decimal val, int decimalPoints)
        {
            return Math.Round(val, decimalPoints);
        }

        /// <summary>
        /// Rounds the supplied decimal value to two decimal points.
        /// </summary>
        /// <param name="val">The decimal to round.</param>
        /// <returns>A decimal value rounded to two decimal points.</returns>
        public static decimal RoundToTwoDecimalPoints(this decimal val)
        {
            return Math.Round(val, 2);
        }

        /// <summary>
        /// Returns the specified number squared.
        /// </summary>
        /// <param name="i">Number to square.</param>
        /// <returns>The number squared.</returns>
        public static int Squared(this int i)
        {
            return i * i;
        }

        /// <summary>
        /// Formats a decimal to a currency string.
        /// </summary>
        /// <param name="value">Decimal value to format.</param>
        /// <returns>A currency string (e.g. 1,234.43).</returns>
        public static string ToCurrencyString(this decimal value)
        {
            return value.ToString("N");

        }
    }
}
