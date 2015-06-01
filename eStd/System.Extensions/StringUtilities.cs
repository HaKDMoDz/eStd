using System;
using System.Text;

namespace System.Extensions
{
    /// <summary>
    /// Contains various string-related tools and utilities.
    /// </summary>
    public class StringUtilities
    {
        /// <summary>
        /// Returns a random string of the desired size.
        /// </summary>
        /// <param name="size">Size of the random string to return</param>
        /// <returns>The resulting string</returns>
        public static string GetRandomString(int size)
        {
            Random random = new Random();

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < size; i++)
            {
                sb.Append(Convert.ToChar(Convert.ToInt32(Math.Floor((26 * random.NextDouble()) + 65))));
            }

            return sb.ToString();
        }
    }
}