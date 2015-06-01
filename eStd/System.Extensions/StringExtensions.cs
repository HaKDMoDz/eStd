using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Extensions
{
    /// <summary>
    /// Contains various useful string-related extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Base64 decodes a <see cref="string">string</see>.
        /// </summary>
        /// <param name="s">A base64 encoded <see cref="string">string</see>.</param>
        /// <returns>A decoded <see cref="string">string</see>.</returns>
        /// <example>
        ///   <para>The following example encodes a <see cref="string">string</see> and decodes it back to its original value.</para>
        ///   <code language="c#">
        ///     string encoded = "Hello, World!".Base64Encode();   
        ///     string decoded = encoded.Base64Decode();
        ///   </code>
        /// </example>
        public static string Base64Decode(this string s)
        {
            byte[] decbuff = Convert.FromBase64String(s);

            return System.Text.Encoding.UTF8.GetString(decbuff);
        }

        /// <summary>
        /// Base64 encodes a <see cref="string">string</see>.
        /// </summary>
        /// <param name="s">An input <see cref="string">string</see>.</param>
        /// <returns>A base64 encoded <see cref="string">string</see>.</returns>
        /// <example>
        ///   <para>The following example encodes a <see cref="string">string</see> and decodes it back to its original value.</para>
        ///   <code language="c#">
        ///     string encoded = "Hello, World!".Base64Encode();   
        ///     string decoded = encoded.Base64Decode();
        ///   </code>
        /// </example>
        public static string Base64Encode(this string s)
        {
            byte[] encbuff = System.Text.Encoding.UTF8.GetBytes(s);

            return Convert.ToBase64String(encbuff);
        }

        /// <summary>
        /// Gets the number of instances of specified character in the <see cref="string">string</see>.
        /// </summary>
        /// <param name="value">The <see cref="String">String</see> to evaulate.</param>
        /// <param name="character">The value to look for in the <paramref name="value">value</paramref>.</param>
        /// <returns>The number of instances of the specified character in the specified <see cref="string">string</see>.</returns>
        /// <example>
        ///   <code language="c#">
        ///     string value = "Hello, World!";
        ///     string character = "l";
        ///     int charCount = value.CharacterInstanceCount(character);
        ///   </code>
        /// </example>
        public static int CharacterInstanceCount(this string value, string character)
        {
            int intReturnValue = 0;

            for (int intCharacter = 0; intCharacter <= (value.Length - 1); intCharacter++)
            {
                if (value.Substring(intCharacter, 1) == character)
                {
                    intReturnValue += 1;
                }
            }

            return intReturnValue;
        }

        /// <summary>
        /// An overload of the built-in .NET String.Contains() method, this method determines whether a substring exists 
        /// within a <see cref="string">string</see> in an optionally case-insensitive way.
        /// </summary>
        /// <param name="s"><see cref="string">String</see> to search.</param>
        /// <param name="subString">Substring to match when searching.</param>
        /// <param name="caseSensitive">Determines whether or not to ignore case.</param>
        /// <returns>Indicator of substring presence within the <see cref="string">string</see>.</returns>
        /// <example>
        ///   <code language="c#">
        ///     string s = "Hello, World!";
        ///     bool valid = s.Contains("hello", true); -> returns false
        ///   </code>
        /// </example>
        public static bool Contains(this string s, string subString, bool caseSensitive)
        {
            if (caseSensitive)
            {
                return s.Contains(subString);
            }
            else
            {
                return s.ToLower().IndexOf(subString.ToLower(), 0) >= 0;
            }
        }


        /// <summary>
        /// Decryptes a <see cref="string">string</see> using the supplied key. Decoding is done using RSA encryption.
        /// </summary>
        /// <param name="s"><see cref="string">String</see> that must be decrypted.</param>
        /// <param name="key">Decryption key.</param>
        /// <returns>The decrypted <see cref="string">string</see> or null if decryption failed.</returns>
        /// <exception cref="ArgumentException">Occurs when stringToDecrypt or key is null or empty.</exception>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         string key = "mykey";
        ///         string encrypted = s.Encrypt(key);
        ///         string decrypted = encrypted.Decrypt(key);
        ///     </code>
        /// </example>
        public static string Decrypt(this string s, string key)
        {
            string result = null;

            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentException("An empty string value cannot be encrypted.");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Cannot decrypt using an empty key. Please supply a decryption key.");
            }

            CspParameters cspp = new CspParameters();
            cspp.KeyContainerName = key;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspp);
            rsa.PersistKeyInCsp = true;

            string[] decryptArray = s.Split(new string[] { "-" }, StringSplitOptions.None);
            byte[] decryptByteArray = Array.ConvertAll<string, byte>(decryptArray, (a => Convert.ToByte(byte.Parse(a, System.Globalization.NumberStyles.HexNumber))));

            byte[] bytes = rsa.Decrypt(decryptByteArray, true);

            result = System.Text.UTF8Encoding.UTF8.GetString(bytes);

            return result;
        }

        /// <summary>
        /// Encryptes a <see cref="string">string</see> using the supplied key. Encoding is done using RSA encryption.
        /// </summary>
        /// <param name="s"><see cref="string">String</see> that must be encrypted.</param>
        /// <param name="key">Encryption key.</param>
        /// <returns>A string representing a byte array separated by a minus sign.</returns>
        /// <exception cref="ArgumentException">Occurs when the <see cref="string">string</see> or key is null or empty.</exception>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         string key = "mykey";
        ///         string encrypted = s.Encrypt(key);
        ///         string decrypted = encrypted.Decrypt(key);
        ///     </code>
        /// </example>
        public static string Encrypt(this string s, string key)
        {
            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentException("An empty string value cannot be encrypted.");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Cannot encrypt using an empty key. Please supply an encryption key.");
            }

            CspParameters cspp = new CspParameters();
            cspp.KeyContainerName = key;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspp);
            rsa.PersistKeyInCsp = true;

            byte[] bytes = rsa.Encrypt(System.Text.UTF8Encoding.UTF8.GetBytes(s), true);

            return BitConverter.ToString(bytes);
        }

        /// <summary>
        /// Determines whether the ending of this instance matches any of the specified strings.  Case sensitive.
        /// </summary>
        /// <param name="s"><see cref="string">String</see> to inspect.</param>
        /// <param name="values">The <see cref="string">string</see> instances to compare.</param>
        /// <returns>Indicator of presence of any of the supplied values at the end of the <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         List&lt;string&gt; values = new List&lt;string&gt;();
        ///         values.AddRange("hello", "!", "World");
        ///         bool result = s.EndsWithAny(values);
        ///     </code>
        /// </example>
        public static bool EndsWithAny(this string s, List<string> values)
        {
            return s.EndsWithAny(values, true);
        }

        /// <summary>
        /// Determines whether the ending of this <see cref="string">string</see> instance matches any of the specified strings.  
        /// Optionally allows case sensitivity to be specified.
        /// </summary>
        /// <param name="s">String to inspect.</param>
        /// <param name="values">The System.Strings to compare.</param>
        /// <param name="ignoreCase">True to ignore case when comparing this string and value.  Otherwise false.</param>
        /// <returns>Indicator of presence of any of the supplied values at the end of the string.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         List&lt;string&gt; values = new List&lt;string&gt;();
        ///         values.AddRange("hello", "!", "World");
        ///         bool result = s.EndsWithAny(values, false);
        ///     </code>
        /// </example>
        public static bool EndsWithAny(this string s, List<string> values, bool ignoreCase)
        {
            return s.EndsWithAny(values, ignoreCase, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Determines whether the ending of this <see cref="string">string</see> instance matches any of the specified strings. 
        /// Optionally allows case sensitivity to be specified. 
        /// Optionally allows the culture to be specified.
        /// </summary>
        /// <param name="s"><see cref="string">String</see> to inspect.</param>
        /// <param name="values">The System.Strings to compare.</param>
        /// <param name="ignoreCase">True to ignore case when comparing this string and value.  Otherwise false.</param>
        /// <param name="culture">Culteral information that determines how this string and value are compared.</param>
        /// <returns>Indicator of presence of any of the supplied values at the end of the string.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         List&lt;string&gt; values = new List&lt;string&gt;();
        ///         values.AddRange("hello", "!", "World");
        ///         bool result = s.EndsWithAny(values, true, CultureInfo.CurrentCulture);
        ///     </code>
        /// </example>
        public static bool EndsWithAny(this string s, List<string> values, bool ignoreCase, CultureInfo culture)
        {
            foreach (string value in values)
            {
                if (s.EndsWith(value, ignoreCase, culture))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes all the words passed in the filter words parameters. The replace is NOT case sensitive.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to search.</param>
        /// <param name="filterWords">The words to repace in the input <see cref="string">string</see>.</param>
        /// <returns>The resulting <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "This is a test sentence.";
        ///         string[] filterWords = new string[] { " is a", " sentence" };
        ///         string results = s.FilterWords(filterWords); -> results in "This test."
        ///     </code>
        /// </example>
        public static string FilterWords(this string s, params string[] filterWords)
        {
            return s.FilterWords(char.MinValue, filterWords);
        }

        /// <summary>
        /// Removes all the words passed in the filter words parameters using the specified mask. 
        /// The replace is NOT case sensitive.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to search.</param>
        /// <param name="mask">A character that is inserted for each letter of the replaced word.</param>
        /// <param name="filterWords">The words to repace in the input <see cref="string">string</see>.</param>
        /// <returns>The resulting <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "This is a test sentence.";
        ///         char mask = '*';
        ///         string[] filterWords = new string[] { "test" };
        ///         actual = s.FilterWords(mask, filterWords); -> results in "This is a **** sentence."
        ///     </code>
        /// </example>
        public static string FilterWords(this string s, char mask, params string[] filterWords)
        {
            string stringMask = mask == char.MinValue ? string.Empty : mask.ToString();
            string totalMask = stringMask;

            foreach (string word in filterWords)
            {
                Regex regEx = new Regex(word, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                if (stringMask.Length > 0)
                {
                    for (int i = 1; i < word.Length; i++)
                    {
                        totalMask += stringMask;
                    }
                }

                s = regEx.Replace(s, totalMask);

                totalMask = stringMask;
            }

            return s;
        }

        /// <summary>
        /// Finds strings that exist between the indicated start and end <see cref="string">string</see> patterns.
        /// </summary>
        /// <param name="s"><see cref="string">String</see> to evaluate.</param>
        /// <param name="startString"><see cref="string">String</see> that should precede the match.</param>
        /// <param name="endString"><see cref="string">String</see> that should follow the match.</param>
        /// <returns>Match collection of found strings.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, <span>World</span>!";
        ///         string startString = "<span>";
        ///         string endString = "</span>";
        ///         MatchCollection matches = s.FindBetween(startString, endString);
        ///         string firstMatch;
        ///         if (matches.Count > 0)
        ///         {
        ///             firstMatch = matches[0].ToString();
        ///         }
        ///     </code>
        /// </example>
        public static MatchCollection FindBetween(this string s, string startString, string endString)
        {
            return s.FindBetween(startString, endString, true);
        }

        /// <summary>
        /// Finds strings that exist between the indicated start and end <see cref="string">string</see> patterns.  Optionally recursive - if true,
        /// finds the last instance of the start <see cref="string">string</see> before the end <see cref="string">string</see> prior to retrieving the results.
        /// </summary>
        /// <param name="s"><see cref="string">String</see> to evaluate.</param>
        /// <param name="startString"><see cref="string">String</see> that should precede the match.</param>
        /// <param name="endString"><see cref="string">String</see> that should follow the match.</param>
        /// <param name="recursive">If true, finds the last instance of the start <see cref="string">string</see>.  Otherwise it uses the first instance.</param>
        /// <returns>Match collection of found strings.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, <span>World</span>!";
        ///         string startString = "<span>";
        ///         string endString = "</span>";
        ///         MatchCollection matches = s.FindBetween(startString, endString, true);
        ///         string firstMatch;
        ///         if (matches.Count > 0)
        ///         {
        ///             firstMatch = matches[0].ToString();
        ///         }
        ///     </code>
        /// </example>
        public static MatchCollection FindBetween(this string s, string startString, string endString, bool recursive)
        {
            MatchCollection matches;

            startString = Regex.Escape(startString);
            endString = Regex.Escape(endString);

            Regex regex = new Regex("(?<=" + startString + ").*(?=" + endString + ")");

            matches = regex.Matches(s);

            if (!recursive)
            {
                return matches;
            }

            if (matches.Count > 0)
            {
                if (matches[0].ToString().IndexOf(Regex.Unescape(startString)) > -1)
                {
                    s = matches[0].ToString() + Regex.Unescape(endString);

                    return s.FindBetween(Regex.Unescape(startString), Regex.Unescape(endString));
                }
                else
                {
                    return matches;
                }
            }
            else
            {
                return matches;
            }
        }

        /// <summary>
        /// Prepends a prefix to a <see cref="string">string</see> if it doesn't already exist.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to alter.</param>
        /// <param name="prefix">The prefix to prepend.</param>
        /// <returns>The resulting <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = " World!";
        ///         string prefix = "Hello,";
        ///         string results = s.ForcePrefix(prefix); -> results in "Hello, World!"
        ///     </code>
        /// </example>
        public static string ForcePrefix(this string s, string prefix)
        {
            string result = s;

            if (!result.StartsWith(prefix))
            {
                result = prefix + result;
            }

            return result;
        }

        /// <summary>
        /// Appends a suffix to a <see cref="string">string</see> if it doesn't already exist.
        /// </summary>
        /// <param name="s"><see cref="string">String</see> to alter.</param>
        /// <param name="suffix">The suffix to append.</param>
        /// <returns>The resulting <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, ";
        ///         string suffix = "World!";
        ///         string results = s.ForceSuffix(suffix); -> results in "Hello, World!"
        ///     </code>
        /// </example>
        public static string ForceSuffix(this string s, string suffix)
        {
            string result = s;

            if (!result.EndsWith(suffix))
            {
                result += suffix;
            }

            return result;
        }

        /// <summary>
        /// Attempts to format a phone number to a (xxx) xxx-xxxx format.
        /// </summary>
        /// <param name="value">The phone number to format.</param>
        /// <returns>The formatted phone number.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string formatted = "4065557485".FormatPhoneNumber();
        ///     </code>
        /// </example>
        public static string FormatPhoneNumber(this string value)
        {
            string strReturnValue = string.Empty;

            if (value.Contains("(") == false && value.Contains(")") == false && value.Contains("-") == false)
            {
                if (value.Length == 7)
                {
                    strReturnValue = value.Substring(0, 3) + "-" + value.Substring(3, 4);
                }
                else if (value.Length == 10)
                {
                    strReturnValue = "(" + value.Substring(0, 3) + ") " + value.Substring(3, 3) + "-" + value.Substring(6, 4);
                }
                else if (value.Length > 10)
                {
                    string strExtensionFormat = string.Empty.PadLeft(value.Length - 10, Convert.ToChar("#"));

                    strReturnValue = "(" + value.Substring(0, 3) + ") " + value.Substring(3, 3) + "-" + value.Substring(6, 4) + " x" + value.Substring(10);
                }
            }
            else
            {
                strReturnValue = value;
            }

            return strReturnValue;
        }

        /// <summary>
        /// Detects if a <see cref="string">string</see> can be parsed to a valid date.
        /// </summary>
        /// <param name="value">Value to inspect.</param>
        /// <returns>Whether or not the <see cref="string">string</see> is formatted as a date.</returns>
        /// <example>
        ///     <code language="c#">
        ///         bool isDate = "12/31/1971".IsDate();
        ///     </code>
        /// </example>
        public static bool IsDate(this string value)
        {
            try
            {
                System.DateTime dt = System.DateTime.Parse(value);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified <see cref="string">string</see> is null or empty.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> value to check.</param>
        /// <returns>Boolean indicating whether the <see cref="string">string</see> is null or empty.</returns>
        /// <example>
        ///     <code language="c#">
        ///         if (!myString.IsEmpty())
        ///         {
        ///             doStuff(myString);
        ///         }
        ///     </code>
        /// </example>
        public static bool IsEmpty(this string s)
        {
            return (s == null) || (s.Length == 0);
        }

        /// <summary>
        /// Indicates whether the specified System.String object is
        /// null or an System.String.Empty string.
        /// </summary>
        /// <param name="s">String to evaluate.</param>
        /// <returns>Indicator of whether the string is null or empty.</returns>
        /// <example>
        ///     <code language="c#">
        ///         if (!myString.IsNullOrEmpty())
        ///         {
        ///             doStuff(myString);
        ///         }
        ///     </code>
        /// </example>
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Determines if a <see cref="string">string</see> can be converted to an integer.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to examine.</param>
        /// <returns>True if the <see cref="string">string</see> is numeric.</returns>
        /// <example>
        ///     <code language="c#">
        ///         if (myString.IsNumeric())
        ///         {
        ///             int myInt = myString.ToInt32();
        ///         }
        ///     </code>
        /// </example>
        public static bool IsNumeric(this string s)
        {
            Regex regularExpression = new Regex("^-[0-9]+$|^[0-9]+$");

            return regularExpression.Match(s).Success;
        }

        /// <summary>
        /// Detects whether this instance is a valid email credit card number format.
        /// </summary>
        /// <param name="s">Credit card number to test.</param>
        /// <returns>True if instance is valid credit card format.</returns>
        /// <example>
        ///     <code language="c#">
        ///         if (TextCardNumber.Text.IsValidCreditCardNumber())
        ///         {
        ///             ProcessCard();
        ///         }
        ///     </code>
        /// </example>
        public static bool IsValidCreditCardNumber(this string s)
        {
            return new Regex(@"^(\d{4}-){3}\d{4}$|^(\d{4} ){3}\d{4}$|^\d{16}$").IsMatch(s);
        }

        /// <summary>
        /// Detects whether this instance is a valid email address.
        /// </summary>
        /// <param name="s">Email address to test.</param>
        /// <returns>True if instance is valid email address.</returns>
        /// <example>
        ///     <code language="c#">
        ///         if (TextEmail.Text.IsValidEmailAddress())
        ///         {
        ///             ProcessRegistration();
        ///         }
        ///     </code>
        /// </example>
        public static bool IsValidEmailAddress(this string s)
        {
            return new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,6}$").IsMatch(s);
        }

        /// <summary>
        /// Detects whether the supplied <see cref="string">string</see> is a valid IP address.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to inspect.</param>
        /// <returns>Results of the check.</returns>
        /// <example>
        ///     <code language="c#">
        ///         bool isValid = ipAddress.IsValidIPAddress();
        ///     </code>
        /// </example>
        public static bool IsValidIPAddress(this string s)
        {
            return Regex.IsMatch(s, @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b");
        }

        /// <summary>
        /// Checks if url is valid.
        /// </summary>
        /// <param name="url">Url to use in the check.</param>
        /// <returns>True if the url is valid.</returns>
        /// <example>
        ///     <code language="c#">
        ///         bool isValid = TextBoxUrl.Text.IsValidUrl();
        ///     </code>
        /// </example>
        public static bool IsValidUrl(this string url)
        {
            string strRegex = @"(file|gopher|news|nntp|telnet|http|ftp|https|ftps|sftp):\/\/"
                + "?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?" // user@
                + @"(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP- 199.194.52.184
                + "|" // allows either IP or domain
                + @"([0-9a-z_!~*'()-]+\.)*" // tertiary domain(s)- www.
                + @"([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]" // second level domain
                + @"(\.[a-z]{2,6})?)" // first level domain- .com or .museum is optional
                + "(:[0-9]{1,5})?" // port number- :80
                + "((/?)|" // a slash isn't required if there is no file name
                + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$";

            return new Regex(strRegex).IsMatch(url);
        }

        /// <summary>
        /// Retrieves the left x characters of a <see cref="string">string</see>.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to examine.</param>
        /// <param name="count">The number of characters to retrieve.</param>
        /// <returns>The resulting substring.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         int charCount = 5;
        ///         string result = s.Left(charCount); // results in "Hello"
        ///     </code>
        /// </example>
        public static string Left(this string s, int count)
        {
            return s.Substring(0, count);
        }

        /// <summary>
        /// Masks a credit card string by replacing all characters except the last 4.
        /// </summary>
        /// <param name="s">Credit card number to format.</param>
        /// <param name="maskCharacter">Character to use as the mask.</param>
        /// <returns>Masked currency string.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string expected = "XXXXXXXXXXXX3456";
        ///         string actual = ccNumber.MaskCreditCard('X');
        ///     </code>
        /// </example>
        public static string MaskCreditCard(this string s, char maskCharacter)
        {
            string result = "";

            for (int i = 0; i < 12; i++)
            {
                result += maskCharacter;
            }

            result += s.Substring(s.Length - 4, 4);

            return result;
        }

        /// <summary>
        /// Retrieves a section of a string.
        /// </summary>
        /// <param name="s">The string to examine.</param>
        /// <param name="index">The start index of the substring.</param>
        /// <param name="count">The number of characters to retrieve.</param>
        /// <returns>The resulting substring.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         string results = s.Mid(2, 4);
        ///     </code>
        /// </example>
        public static string Mid(this string s, int index, int count)
        {
            return s.Substring(index, count);
        }

        /// <summary>
        /// Left pads a <see cref="string">string</see> using the supplied pad <see cref="string">string</see> for the total number of spaces.  
        /// It will not cut-off the pad even if it causes the <see cref="string">string</see> to exceed the total width.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to pad.</param>
        /// <param name="pad">The <see cref="string">string</see> to uses as padding.</param>
        /// <returns>A padded <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         string pad = " How are you?";
        ///         string results = s.PadLeft(pad);
        ///     </code>
        /// </example>
        public static string PadLeft(this string s, string pad)
        {
            return s.PadLeft(pad, s.Length + pad.Length, false);
        }

        /// <summary>
        /// Left pads a <see cref="string">string</see> using the passed pad <see cref="string">string</see> for the total number of spaces.  
        /// Optionally, it will cut-off the pad so that the <see cref="string">string</see> does not exceed the total width.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to pad.</param>
        /// <param name="pad">The <see cref="string">string</see> to uses as padding.</param>
        /// <param name="totalWidth">The total number to pad the <see cref="string">string</see>.</param>
        /// <param name="cutOff">Limits overall size of <see cref="string">string</see>.</param>
        /// <returns>A padded <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         string pad = " How are you?";
        ///         string actual = s.PadLeft(pad, 17, true);
        ///     </code>
        /// </example>
        public static string PadLeft(this string s, string pad, int totalWidth, bool cutOff)
        {
            if (s.Length >= totalWidth)
            {
                return s;
            }

            int padCount = pad.Length;

            string paddedString = s;

            while (paddedString.Length < totalWidth)
            {
                paddedString += pad;
            }

            // trim the excess.
            if (cutOff)
            {
                paddedString = paddedString.Substring(0, totalWidth);
            }

            return paddedString;
        }

        /// <summary>
        /// Right pads a <see cref="string">string</see> using the supplied pad <see cref="string">string</see> for the total number of spaces.  
        /// It will not cut-off the pad even if it causes the <see cref="string">string</see> to exceed the total width.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to pad.</param>
        /// <param name="pad">The <see cref="string">string</see> to uses as padding.</param>
        /// <returns>A padded <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "How are you?";
        ///         string pad = "Hello, World! ";
        ///         string results = s.PadRight(pad);
        ///     </code>
        /// </example>
        public static string PadRight(this string s, string pad)
        {
            return PadRight(s, pad, s.Length + pad.Length, false);
        }

        /// <summary>
        /// Right pads a <see cref="string">string</see> using the supplied pad <see cref="string">string</see> for the total number of spaces.  
        /// It will cut-off the pad so that the <see cref="string">string</see> does not exceed the total width.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to pad.</param>
        /// <param name="pad">The <see cref="string">string</see> to uses as padding.</param>
        /// <param name="length">The total length to pad the <see cref="string">string</see>.</param>
        /// <param name="cutOff">Limits the overall length of the <see cref="string">string</see>.</param>
        /// <returns>A padded <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "How are you?";
        ///         string pad = "Hello, World! ";
        ///         string results = s.PadRight(pad, 17, true);
        ///     </code>
        /// </example>
        public static string PadRight(this string s, string pad, int length, bool cutOff)
        {
            if (s.Length >= length)
            {
                return s;
            }

            string paddedString = string.Empty;

            while (paddedString.Length < length - s.Length)
            {
                paddedString += pad;
            }

            // trim the excess.
            if (cutOff)
            {
                paddedString = paddedString.Substring(0, length - s.Length);
            }

            paddedString += s;

            return paddedString;
        }

        /// <summary>
        /// Removes duplicate words from a string.
        /// </summary>
        /// <param name="s">String to evaluate.</param>
        /// <returns>A string with duplicates removed.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string expected = "To be or not - that is the question.";
        ///         string results = s.RemoveDuplicateWords();
        ///     </code>
        /// </example>
        public static string RemoveDuplicateWords(this string s)
        {
            var d = new Dictionary<string, bool>();

            StringBuilder sb = new StringBuilder();

            string[] a = s.Split(new char[] { ' ', ',', ';', '.', ':' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string current in a)
            {
                string lower = current.ToLower();

                if (!d.ContainsKey(lower))
                {
                    sb.Append(current).Append(' ');
                    d.Add(lower, true);
                }
            }

            string results = sb.ToString().Trim();

            if (s.EndsWith("."))
            {
                results += ".";
            }

            if (s.EndsWith(";"))
            {
                results += ";";
            }

            if (s.EndsWith(":"))
            {
                results += ":";
            }

            if (s.EndsWith(","))
            {
                results += ",";
            }

            return results;
        }

        /// <summary>
        /// Removes the new line (\n) and carriage return (\r) symbols.
        /// </summary>
        /// <param name="s">The string to search.</param>
        /// <returns>The resulting string.</returns>
        /// <remarks>By default, does not replace the new line with a space.</remarks>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, \r\nWorld!";
        ///         string results = s.RemoveNewLines();
        ///     </code>
        /// </example>
        public static string RemoveNewLines(this string s)
        {
            return s.RemoveNewLines(false);
        }

        /// <summary>
        /// Removes the new line (\n) and carriage return (\r) symbols.  
        /// Optionally adds a space for each newline and carriage return.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to search.</param>
        /// <param name="addSpace">If true, adds a space (" ") for each newline and carriage return found.</param>
        /// <returns>The resulting string.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, \r\nWorld!";
        ///         string results = s.RemoveNewLines(true);
        ///     </code>
        /// </example>
        public static string RemoveNewLines(this string s, bool addSpace)
        {
            string replace = string.Empty;

            if (addSpace)
            {
                replace = " ";
            }

            return s.Replace(System.Environment.NewLine, replace);
        }

        /// <summary>
        /// Removes a prefix from a <see cref="string">string</see> if it exists.
        /// </summary>
        /// <param name="s">Input <see cref="string">string</see> to remove the prefix from.</param>
        /// <param name="prefix"><see cref="string">String</see> that defines the prefix to remove.</param>
        /// <returns>The resulting <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         string results = s.RemovePrefix("Hello, ");
        ///     </code>
        /// </example>
        public static string RemovePrefix(this string s, string prefix)
        {
            return Regex.Replace(s, "^" + prefix, System.String.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Remove whitespace from within a <see cref="string">string</see>.
        /// </summary>
        /// <param name="s"><see cref="string">String</see> to remove the spaces from.</param>
        /// <returns><see cref="string">String</see> without spaces.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string phoneNumber = "555 555 5555";
        ///         string trimmed = phoneNumber.RemoveSpaces();
        ///     </code>
        /// </example>
        public static string RemoveSpaces(this string s)
        {
            return s.Replace(" ", string.Empty);
        }

        /// <summary>
        /// Removes a suffix from a <see cref="string">string</see> if it exists.
        /// </summary>
        /// <param name="s">Input <see cref="string">string</see> to remove the suffix from.</param>
        /// <param name="suffix"><see cref="string">String</see> that defines the suffix to remove.</param>
        /// <returns>The resulting <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         string results = s.RemoveSuffix(", World!");
        ///     </code>
        /// </example>
        public static string RemoveSuffix(this string s, string suffix)
        {
            return Regex.Replace(s, suffix + "$", System.String.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// A case insenstive replace function.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to examine.</param>
        /// <param name="oldString">The new value to be inserted.</param>
        /// <param name="newString">The value to replace.</param>
        /// <param name="caseSensitive">Determines whether or not to ignore case.</param>
        /// <returns>The resulting <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         string results = s.Replace("world", "Dude", false);
        ///     </code>
        /// </example>
        public static string Replace(this string s, string oldString, string newString, bool caseSensitive)
        {
            if (caseSensitive)
            {
                return s.Replace(oldString, newString);
            }
            else
            {
                Regex regEx = new Regex(oldString, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                return regEx.Replace(s, newString);
            }
        }

        /// <summary>
        /// Reverses a <see cref="string">string</see>.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to reverse.</param>
        /// <returns>The resulting <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         string results = s.Reverse();
        ///     </code>
        /// </example>
        public static string Reverse(this string s)
        {
            if (s.Length <= 1)
            {
                return s;
            }

            char[] c = s.ToCharArray();

            StringBuilder sb = new StringBuilder(c.Length);

            for (int i = c.Length - 1; i > -1; i--)
            {
                sb.Append(c[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Retrieves the right x characters of a <see cref="string">string</see>.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to examine.</param>
        /// <param name="count">The number of characters to retrieve.</param>
        /// <returns>The resulting substring.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         string results = s.Right(6);
        ///     </code>
        /// </example>
        public static string Right(this string s, int count)
        {
            return s.Substring(s.Length - count, count);
        }

        /// <summary>
        /// Converts a <see cref="string">string</see> to sentence case.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to convert.</param>
        /// <returns>The resulting <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "This Is Just a Test.";
        ///         string results = s.SentenceCase();
        ///     </code>
        /// </example>
        public static string SentenceCase(this string s)
        {
            if (s.Length < 1)
            {
                return s;
            }

            string sentence = s.ToLower();

            return sentence[0].ToString().ToUpper() + sentence.Substring(1);
        }

        /// <summary>
        /// Retrieves a section of a string by using start and end indexes.
        /// </summary>
        /// <param name="s">The string to examine.</param>
        /// <param name="startIndex">The start index of the substring.</param>
        /// <param name="endIndex">The end index of the substring.</param>
        /// <returns>The resulting substring.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         string results = s.Slice(0, 5);
        ///     </code>
        /// </example>
        public static string Slice(this string s, int startIndex, int endIndex)
        {
            return s.Substring(startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// Replaces underscores with a space.
        /// </summary>
        /// <param name="s"><see cref="string">String</see> to examine.</param>
        /// <returns>The resulting <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello_World!_How_are_you_today?";
        ///         string results = s.SlugDecode();
        ///     </code>
        /// </example>
        public static string SlugDecode(this string s)
        {
            return s.Replace("_", " ");
        }

        /// <summary>
        /// Replaces spaces with a underscores.
        /// </summary>
        /// <param name="s"><see cref="string">String</see> to examine.</param>
        /// <returns>The resulting <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello World! How are you today?";
        ///         string results = s.SlugEncode();
        ///     </code>
        /// </example>
        public static string SlugEncode(this string s)
        {
            return s.Replace(" ", "_");
        }

        /// <summary>
        /// Splits a <see cref="string">string</see> into an array by delimiter.
        /// </summary>
        /// <param name="s"><see cref="string">String</see> to split.</param>
        /// <param name="delimiter">Delimiter <see cref="string">string</see>.</param>
        /// <returns>Array of strings.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "one, two, three";
        ///         string[] results = s.Split(",");
        ///     </code>
        /// </example>
        public static string[] Split(this string s, string delimiter)
        {
            return s.Split(delimiter.ToCharArray());
        }

        /// <summary>
        /// Splits a <see cref="string">string</see> into an array by delimiter.
        /// Optionally allows for the trimming of each token during the split.
        /// </summary>
        /// <param name="s"><see cref="string">String</see> to split.</param>
        /// <param name="delimiter">Delimiter <see cref="string">string</see>.</param>
        /// <param name="trimTokens">Specifies whether to trim each item in the array during the split.</param>
        /// <returns>Array of strings.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "one, two, three";
        ///         string[] results = s.Split(",", true);
        ///     </code>
        /// </example>
        public static string[] Split(this string s, string delimiter, bool trimTokens)
        {
            if (trimTokens)
            {
                string[] results = s.Split(delimiter.ToCharArray());

                for (int i = 0; i < results.Length; i++)
                {
                    results[i] = results[i].Trim();
                }

                return results;
            }
            else
            {
                return s.Split(delimiter.ToCharArray());
            }
        }

        /// <summary>
        /// Determines whether the beginning of this instance matches any of the specified strings.
        /// </summary>
        /// <param name="s"><see cref="string">String</see> to inspect.</param>
        /// <param name="values">The strings to compare.</param>
        /// <returns>Indicator of presence of any of the supplied values at the beginning of the <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         List&lt;string&gt; values = new List&lt;string&gt;();
        ///         values.Add("Goodbye");
        ///         values.Add("Hello");
        ///         bool results = s.StartsWithAny(values);
        ///     </code>
        /// </example>
        public static bool StartsWithAny(this string s, List<string> values)
        {
            return s.StartsWithAny(values, true);
        }

        /// <summary>
        /// Determines whether the beginning of this instance matches any of the specified strings.  
        /// Optionally allows for case ignorance.
        /// </summary>
        /// <param name="s"><see cref="string">string</see> to inspect.</param>
        /// <param name="values">The strings to compare.</param>
        /// <param name="ignoreCase">True to ignore case when comparing this <see cref="string">string</see> and value.  Otherwise false.</param>
        /// <returns>Indicator of presence of any of the supplied values at the beginning of the <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         List&lt;string&gt; values = new List&lt;string&gt;();
        ///         values.Add("Goodbye");
        ///         values.Add("Hello");
        ///         bool results = s.StartsWithAny(values, true);
        ///     </code>
        /// </example>
        public static bool StartsWithAny(this string s, List<string> values, bool ignoreCase)
        {
            return s.StartsWithAny(values, ignoreCase, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Determines whether the beginning of this instance matches any of the specified strings.
        /// Optionally allows the culture to be specified.
        /// </summary>
        /// <param name="s"><see cref="string">String</see> to inspect.</param>
        /// <param name="values">The strings to compare.</param>
        /// <param name="ignoreCase">True to ignore case when comparing this <see cref="string">string</see> and value.  Otherwise false.</param>
        /// <param name="culture">Culteral information that determines how this <see cref="string">string</see> and value are compared.</param>
        /// <returns>Indicator of presence of any of the supplied values at the beginning of the <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         List&lt;string&gt; values = new List&lt;string&gt;();
        ///         values.Add("Goodbye");
        ///         values.Add("Hello");
        ///         bool results = s.StartsWithAny(values, true, CultureInfo.CurrentCulture);
        ///     </code>
        /// </example>
        public static bool StartsWithAny(this string s, List<string> values, bool ignoreCase, CultureInfo culture)
        {
            foreach (string value in values)
            {
                if (s.StartsWith(value, ignoreCase, culture))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes all HTML tags from the passed <see cref="string">string</see>.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> whose values should be replaced.</param>
        /// <returns>The resulting <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "&lt;body&gt;Hello, World!&lt;/body&gt;";
        ///         string results = s.StripTags();
        ///     </code>
        /// </example>
        public static string StripTags(this string s)
        {
            Regex stripTags = new Regex("<(.|\n)+?>");

            return stripTags.Replace(s, string.Empty);
        }

        /// <summary>
        /// Converts a <see cref="string">string</see> to title case.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to convert.</param>
        /// <returns>The resulting <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "On the waterfront";
        ///         string title = s.TitleCase();
        ///     </code>
        /// </example>
        public static string TitleCase(this string s)
        {
            return TitleCase(s, true);
        }

        /// <summary>
        /// Converts a <see cref="string">string</see> to title case.
        /// Optionally allows short words to be ignored.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to convert.</param>
        /// <param name="ignoreShortWords">If true, does not capitalize words like
        /// "a", "is", "the", etc.</param>
        /// <returns>The resulting <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "On the waterfront";
        ///         string title = s.TitleCase(false);
        ///     </code>
        /// </example>
        public static string TitleCase(this string s, bool ignoreShortWords)
        {
            List<string> ignoreWords = null;

            if (ignoreShortWords)
            {
                ignoreWords = new List<string>();
                ignoreWords.Add("a");
                ignoreWords.Add("is");
                ignoreWords.Add("was");
                ignoreWords.Add("the");
            }

            string[] tokens = s.Split(' ');

            StringBuilder sb = new StringBuilder(s.Length);

            foreach (string token in tokens)
            {
                if (ignoreShortWords == true
                    && token != tokens[0]
                    && ignoreWords.Contains(token.ToLower()))
                {
                    sb.Append(token + " ");
                }
                else
                {
                    sb.Append(token[0].ToString().ToUpper());
                    sb.Append(token.Substring(1).ToLower());
                    sb.Append(" ");
                }
            }

            return sb.ToString().Trim();
        }

        /// <summary>
        /// Converts the supplied <paramref name="Value">value</paramref> to an <see cref="Boolean">Boolean</see>.
        /// </summary>
        /// <param name="value">The <see cref="string">string</see> value to convert.</param>
        /// <returns>The resulting <see cref="Boolean">Boolean</see> value.</returns>
        /// <remarks>If an error occurs while converting the value (ie the <see cref="string">string</see> value does not convert to a boolean value), <c>false</c> will be returned.</remarks>
        /// <example>
        ///     <code language="c#">
        ///         string s = "False";
        ///         bool results = s.ToBoolean();     
        ///     </code>
        /// </example>
        public static bool ToBoolean(this string value)
        {
            try
            {
                return Convert.ToBoolean(value);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a byte array of a specified <see cref="string">string</see>.
        /// </summary>
        /// <param name="text">The text to go into the byte array.</param>
        /// <returns>A byte array of text.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         byte[] converted = s.ToByteArray();
        ///     </code>
        /// </example>
        public static byte[] ToByteArray(this string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

        /// <summary>
        /// Convert a (A)RGB string to a Color object.
        /// </summary>
        /// <param name="s">An RGB or an ARGB string.</param>
        /// <returns>A Color object.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "#ffffff";
        ///         Color expected = s.ToColor();
        ///     </code>
        /// </example>
        public static Color ToColor(this string s)
        {
            s = s.Replace("#", string.Empty);

            byte a = System.Convert.ToByte("ff", 16);

            byte pos = 0;

            if (s.Length == 8)
            {
                a = System.Convert.ToByte(s.Substring(pos, 2), 16);
                pos = 2;
            }

            byte r = System.Convert.ToByte(s.Substring(pos, 2), 16);

            pos += 2;

            byte g = System.Convert.ToByte(s.Substring(pos, 2), 16);

            pos += 2;

            byte b = System.Convert.ToByte(s.Substring(pos, 2), 16);

            return Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// Converts the supplied <paramref name="value">value</paramref> to a <see cref="Decimal">Decimal</see>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The resulting <see cref="Decimal">Decimal</see> value.</returns>
        /// <example>
        ///     <code language="c#">
        ///         Decimal result = "5".ToDecimal();
        ///     </code>
        /// </example>
        public static decimal ToDecimal(this string value)
        {
            if (string.IsNullOrEmpty(value) == true || (value == null) == true || value.IsNumeric() == false)
            {
                return 0;
            }
            else
            {
                return System.Convert.ToDecimal(value);
            }
        }

        /// <summary>
        /// Converts the supplied <paramref name="Value">value</paramref> to an <see cref="Int16">Int16</see>.
        /// </summary>
        /// <param name="value">The <see cref="string">string</see> value to convert.</param>
        /// <returns>The resulting <see cref="Int16">Int16</see> value.</returns>
        /// <example>
        ///     <code language="c#">
        ///         int number = "5".ToInt16();
        ///     </code>
        /// </example>
        public static short ToInt16(this string value)
        {
            if (string.IsNullOrEmpty(value) == true || (value == null) == true || value.IsNumeric() == false)
            {
                return System.Convert.ToInt16(0);
            }
            else
            {
                return System.Convert.ToInt16(value);
            }
        }

        /// <summary>
        /// Converts the supplied <paramref name="value">value</paramref> to an <see cref="Int32">Int32.</see>
        /// </summary>
        /// <param name="value">The <see cref="string">string</see> value to convert.</param>
        /// <returns>The resulting Integer.</returns>
        /// <example>
        ///     <code language="c#">
        ///         int number = "5".ToInt32();
        ///     </code>
        /// </example>
        public static int ToInt32(this string value)
        {
            if (string.IsNullOrEmpty(value) == true || (value == null) == true || value.IsNumeric() == false)
            {
                return System.Convert.ToInt32(0);
            }
            else
            {
                return System.Convert.ToInt32(value);
            }
        }

        /// <summary>
        /// Converts the supplied <paramref name="Value">value</paramref> to an <see cref="Int64">Int64</see>.
        /// </summary>
        /// <param name="value">The <see cref="string">string</see> value to convert.</param>
        /// <returns>The resulting <see cref="Int64">Int64</see> value.</returns>
        /// <example>
        ///     <code language="c#">
        ///         int number = "294967295".ToInt64();
        ///     </code>
        /// </example>
        public static long ToInt64(this string value)
        {
            if (string.IsNullOrEmpty(value) == true || (value == null) == true || value.IsNumeric() == false)
            {
                return System.Convert.ToInt64(0);
            }
            else
            {
                return System.Convert.ToInt64(value);
            }
        }

        /// <summary>
        /// Removes multiple spaces between words.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to trim.</param>
        /// <returns>The resulting <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello,   World!";
        ///         string trimmed = s.TrimIntraWords();
        ///     </code>
        /// </example>
        public static string TrimIntraWords(this string s)
        {
            Regex regEx = new Regex(@"[\s]+");

            return regEx.Replace(s, " ");
        }

        /// <summary>
        /// Truncates the <see cref="string">string</see> to a specified length and replace the truncated to a ...
        /// </summary>
        /// <param name="s"><see cref="string">String</see> that will be truncated.</param>
        /// <param name="maxLength">Total length of characters to maintain before the truncate happens.</param>
        /// <returns>Truncated <see cref="string">string</see>.</returns>
        /// <remarks>maxLength is inclusive of the three characters in the ellipsis.</remarks>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         string results = s.Truncate(5);
        ///     </code>
        /// </example>
        public static string Truncate(this string s, int maxLength)
        {
            // replaces the truncated string to a ...
            string suffix = "...";

            string truncatedString = s;

            if (maxLength <= 0)
            {
                return truncatedString;
            }

            int strLength = maxLength - suffix.Length;

            if (strLength <= 0)
            {
                return truncatedString;
            }

            if (s == null || s.Length <= maxLength)
            {
                return truncatedString;
            }

            truncatedString = s.Substring(0, strLength);
            truncatedString = truncatedString.TrimEnd();
            truncatedString += suffix;

            return truncatedString;
        }

        /// <summary>
        /// Counts all words in a given <see cref="string">string</see>.  Excludes whitespaces, tabs and line breaks.
        /// </summary>
        /// <param name="s"><see cref="string">String</see> to inspect.</param>
        /// <returns>The number of words in the <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World!";
        ///         int count = s.WordCount();    
        ///     </code>
        /// </example>
        public static int WordCount(this string s)
        {
            var count = 0;

            var re = new Regex(@"[^\s]+");
            var matches = re.Matches(s);
            count = matches.Count;

            return count;
        }

        /// <summary>
        /// Calculates the number of times a word exists withing a <see cref="string">string</see>.
        /// </summary>
        /// <param name="s"><see cref="string">String</see> to evaluate.</param>
        /// <param name="word">Word to search for.</param>
        /// <returns>Number of times the word exists within the <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World! How are you doing today?";
        ///         string word = "How";
        ///         int count = s.WordInstanceCount(word);
        ///     </code>
        /// </example>
        public static int WordInstanceCount(this string s, string word)
        {
            Regex r = new Regex(@"\b" + word + @"\b", RegexOptions.IgnoreCase);
            MatchCollection mc = r.Matches(s);
            return mc.Count;
        }

        /// <summary>
        /// Wraps the passed <see cref="string">string</see> up the total number of characters until the next whitespace on or after 
        /// the total character count has been reached for that line.  
        /// Uses the environment new line symbol for the break text.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to wrap.</param>
        /// <param name="charCount">The number of characters per line.</param>
        /// <returns>The resulting <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World! How are you doing today?";
        ///         string results = s.WordWrap(9);
        ///     </code>
        /// </example>
        public static string WordWrap(this string s, int charCount)
        {
            return WordWrap(s, charCount, false, System.Environment.NewLine);
        }

        /// <summary>
        /// Wraps the passed <see cref="string">string</see> up the total number of characters (if cutOff is true)
        /// or until the next whitespace (if cutOff is false).  Uses the environment new line
        /// symbol for the break text.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to wrap.</param>
        /// <param name="charCount">The number of characters per line.</param>
        /// <param name="cutOff">If true, will break in the middle of a word.</param>
        /// <returns>The resulting <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World! How are you doing today?";
        ///         string results = s.WordWrap(9, false);
        ///     </code>
        /// </example>
        public static string WordWrap(this string s, int charCount, bool cutOff)
        {
            return WordWrap(s, charCount, cutOff, System.Environment.NewLine);
        }

        /// <summary>
        /// Wraps the passed <see cref="string">string</see> up the total number of characters (if cutOff is true)
        /// or until the next whitespace (if cutOff is false).  Uses the supplied breakText
        /// for line breaks.
        /// </summary>
        /// <param name="s">The <see cref="string">string</see> to wrap.</param>
        /// <param name="charCount">The number of characters per line.</param>
        /// <param name="cutOff">If true, will break in the middle of a word.</param>
        /// <param name="breakText">The line break text to use.</param>
        /// <returns>The resulting <see cref="string">string</see>.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string s = "Hello, World! How are you doing today?";
        ///         string results = s.WordWrap(9, false, Environment.NewLine);
        ///     </code>
        /// </example>
        public static string WordWrap(this string s, int charCount, bool cutOff, string breakText)
        {
            StringBuilder sb = new StringBuilder(s.Length + 100);
            int counter = 0;

            if (cutOff)
            {
                while (counter < s.Length)
                {
                    if (s.Length > counter + charCount)
                    {
                        sb.Append(s.Substring(counter, charCount));
                        sb.Append(breakText);
                    }
                    else
                    {
                        sb.Append(s.Substring(counter));
                    }

                    counter += charCount;
                }
            }
            else
            {
                string[] strings = s.Split(' ');

                for (int i = 0; i < strings.Length; i++)
                {
                    counter += strings[i].Length + 1;

                    if (i != 0 && counter > charCount)
                    {
                        sb.Append(breakText);
                        counter = 0;
                    }

                    sb.Append(strings[i] + ' ');
                }
            }

            return sb.ToString().TrimEnd();
        }
    }
}
