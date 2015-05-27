using System.Globalization;
using System.IO;

namespace System.Net.Torrent.BEncode
{
    public class BInt : IBencodingType, IComparable<long>, IEquatable<long>, IEquatable<BInt>, IComparable<BInt>
    {
        private BInt()
        {
            Value = 0;
        }
        public BInt(long value)
        {
            Value = value;
        }

        public long Value { get; set; }

        /// <summary>
        /// Decode the next token as a int.
        /// Assumes the next token is a int.
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="bytesConsumed"></param>
        /// <returns>Decoded int</returns>
        public static BInt Decode(BinaryReader inputStream, ref int bytesConsumed)
        {
            // Get past 'i'
            inputStream.Read();
            bytesConsumed++;

            // Read numbers till an 'e'
            string number = "";
            char ch;

            while ((ch = inputStream.ReadChar()) != 'e')
            {
                number += ch;

                bytesConsumed++;
            }

            bytesConsumed++;

            BInt res = new BInt { Value = long.Parse(number) };

            return res;
        }

        public void Encode(BinaryWriter writer)
        {
            // Write header
            writer.Write('i');

            // Write value
            writer.Write(Value.ToString(CultureInfo.InvariantCulture).ToCharArray());

            // Write footer
            writer.Write('e');
        }

        public int CompareTo(long other)
        {
            if (Value < other)
                return -1;

            if (Value > other)
                return 1;

            return 0;
        }
        public int CompareTo(BInt other)
        {
            if (other == null)
                throw new ArgumentNullException("other");

            if (Value < other.Value)
                return -1;

            if (Value > other.Value)
                return 1;

            return 0;
        }

        public override bool Equals(object obj)
        {
            BInt other = obj as BInt;

            return Equals(other);
        }
        public bool Equals(BInt other)
        {
            if (other == null)
                return false;

            return Equals(other.Value);
        }
        public bool Equals(long other)
        {
            return Value == other;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0}", Value);
        }

        public static implicit operator BInt(long x)
        {
            return new BInt(x);
        }

        public static implicit operator long(BInt x)
        {
            return x.Value;
        }

        public static implicit operator BInt(int x)
        {
            return new BInt(x);
        }

        public static implicit operator int(BInt x)
        {
            return (int)x.Value;
        }
    }
}