using System;

namespace Intersect.Extensions
{

    public static class DateTimeExtensions
    {

        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime Clone(this DateTime dateTime)
        {
            return new DateTime(dateTime.Ticks, dateTime.Kind);
        }

        public static DateTime ConvertKind(this DateTime dateTime, DateTimeKind kind)
        {
            // If it's the same kind, just return the same DateTime.
            if (kind == dateTime.Kind)
            {
                return dateTime;
            }

            switch (kind)
            {
                case DateTimeKind.Local:
                    return dateTime.ToLocalTime();

                case DateTimeKind.Unspecified:
                    return new DateTime(dateTime.Ticks, DateTimeKind.Unspecified);

                case DateTimeKind.Utc:
                    return dateTime.ToUniversalTime();

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, @"Unknown DateTimeKind value.");
            }
        }

        public static TimeSpan AsUnixTimeSpan(this DateTime dateTime)
        {
            var dateTimeUtc = dateTime;
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                dateTimeUtc = dateTime.ToUniversalTime();
            }

            return dateTimeUtc - UnixEpoch;
        }

    }

}
