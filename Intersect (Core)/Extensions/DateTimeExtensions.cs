using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Intersect.Extensions
{
    public static class DateTimeExtensions
    {
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
                case DateTimeKind.Utc:
                    return dateTime.ToUniversalTime();

                case DateTimeKind.Local:
                    return dateTime.ToLocalTime();

                case DateTimeKind.Unspecified:
                    return new DateTime(dateTime.Ticks, DateTimeKind.Unspecified);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, @"Unknown DateTimeKind value.");
            }
        }

        public static TimeSpan AsUnixTimeSpan(this DateTime dateTime)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var dateTimeUtc = dateTime;
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                dateTimeUtc = dateTime.ToUniversalTime();
            }

            return dateTimeUtc - unixEpoch;
        }
    }
}
