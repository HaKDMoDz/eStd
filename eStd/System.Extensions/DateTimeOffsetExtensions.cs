using System;

namespace Creek.Extensions
{
    /// <summary>
    /// 	Extension methods for the DateTimeOffset data type.
    /// </summary>
    public static class DateTimeOffsetExtensions
    {



        /// <summary>
        /// 	Converts a DateTimeOffset into a DateTime using the local system time zone.
        /// </summary>
        /// <param name = "dateTimeUtc">The base DateTimeOffset.</param>
        /// <returns>The converted DateTime</returns>
        public static DateTime ToLocalDateTime(this DateTimeOffset dateTimeUtc)
        {
            return dateTimeUtc.ToLocalDateTime(null);
        }

        /// <summary>
        /// 	Converts a DateTimeOffset into a DateTime using the specified time zone.
        /// </summary>
        /// <param name = "dateTimeUtc">The base DateTimeOffset.</param>
        /// <param name = "localTimeZone">The time zone to be used for conversion.</param>
        /// <returns>The converted DateTime</returns>
        public static DateTime ToLocalDateTime(this DateTimeOffset dateTimeUtc, TimeZoneInfo localTimeZone)
        {
            localTimeZone = (localTimeZone ?? TimeZoneInfo.Local);

            return TimeZoneInfo.ConvertTime(dateTimeUtc, localTimeZone).DateTime;
        }
    }
}
