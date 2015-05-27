using System;

namespace System.Net.Ftp
{
    public static class Extensions
    {
        public static DateTime? ToDateTime(this Winapi.FILETIME time)
        {
            if (time.dwHighDateTime == 0 && time.dwLowDateTime == 0)
                return null;

            unchecked
            {
                uint low = (uint)time.dwLowDateTime;
                long ft = (((long)time.dwHighDateTime) << 32 | low);
                return DateTime.FromFileTimeUtc(ft);
            }
        }
    }
}