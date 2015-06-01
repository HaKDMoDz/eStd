using System;
using System.Security.Cryptography;
using System.Text;

namespace System.Extensions
{
    /// <summary>
    /// Contains security-related extensions.
    /// </summary>
    public static class SecurityExtensions
    {
        /// <summary>
        /// Verifies a <see cref="string">string</see> against the passed MD5 hash.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to compare.</param>
        /// <param name="hash">The hash to compare against.</param>
        /// <returns>True if the input and the hash are the same, false otherwise.</returns>
        public static bool MD5Verify(this string s, string hash)
        {
            // Hash the input.
            string hashOfInput = s.ToMD5();

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// MD5 encodes the passed <see cref="string">string</see>.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to encode.</param>
        /// <returns>An encoded <see cref="string">string</see>.</returns>
        public static string ToMD5(this string s)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hasher.ComputeHash(Encoding.Default.GetBytes(s));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sb = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sb.ToString();
        }
    }
}