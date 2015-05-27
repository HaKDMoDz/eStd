namespace System.Net.Torrent.Misc
{
    public static class Utils
    {
        public static bool GetBit(this byte t, UInt16 n)
        {
            return (t & (1 << n)) != 0;
        }

        public static byte SetBit(this byte t, UInt16 n)
        {
            return (byte)(t | (1 << n));
        }

        public static byte[] GetBytes(this byte[] bytes, Int32 start, Int32 length = -1)
        {
            int l = length;
            if (l == -1) l = bytes.Length - start;

            byte[] intBytes = new byte[l];

            for (int i = 0; i < l; i++) intBytes[i] = bytes[start + i];

            return intBytes;
        }

        public static byte[] Cat(this byte[] first, byte[] second)
        {
            byte[] returnBytes = new byte[first.Length + second.Length];

            first.CopyTo(returnBytes, 0);
            second.CopyTo(returnBytes, first.Length);

            return returnBytes;
        }

        public static bool Contains<T>(this T[] ar, T o)
        {
            foreach (T t in ar)
            {
                if (Equals(t, o)) return true;
            }

            return false;
        }

        public static bool Contains<T>(this T[] ar, Func<T, bool> expr)
        {
            foreach (T t in ar)
            {
                if (expr != null && expr(t)) return true;
            }

            return false;
        }
    }
}