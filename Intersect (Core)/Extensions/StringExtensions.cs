using System;
using System.Globalization;

namespace Intersect.Extensions
{
    /// <summary>
    /// Extensions for <see cref="string"/>.
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// Formats the string in the current culture with the provided arguments.
        /// </summary>
        /// <param name="format">the format string</param>
        /// <param name="args">the format arguments</param>
        /// <returns>a formatted string</returns>
        /// <seealso cref="string.Format(IFormatProvider, string, object[])"/>
        public static string Format(this string format, params object[] args) =>
            Format(format, CultureInfo.CurrentCulture);

        /// <summary>
        /// Formats the string with the provided <see cref="IFormatProvider"/> and arguments.
        /// </summary>
        /// <param name="format">the format string</param>
        /// <param name="formatProvider">the format provider to use to format the provided string and arguments</param>
        /// <param name="args">the format arguments</param>
        /// <returns>a formatted string</returns>
        /// <seealso cref="string.Format(IFormatProvider, string, object[])"/>
        public static string Format(this string format, IFormatProvider formatProvider, params object[] args) =>
            string.Format(formatProvider, format, args);

        public static string TerminateWith(this string self, char terminateWith, bool condition = true) =>
            TerminateWith(self, new[] {terminateWith}, condition);

        public static string TerminateWith(this string self, char[] terminateWith, bool condition = true) =>
            TerminateWith(self, new string(terminateWith), condition);

        public static string TerminateWith(this string self, string terminateWith, bool condition = true)
        {
            if (!condition)
            {
                return self;
            }

            if (string.IsNullOrWhiteSpace(self))
            {
                return terminateWith;
            }

            if (self.EndsWith(terminateWith))
            {
                return self;
            }

            return self + terminateWith;
        }

        public static string Slice(this string self, in int offset) => Slice(self, offset, -1);

        public static string Slice(this string self, in int offset, in int length)
        {
            if (self == default)
            {
                throw new ArgumentNullException(nameof(self));
            }

            if (offset >= self.Length || offset <= -self.Length || length == 0)
            {
                return string.Empty;
            }

            var boundedOffset = (self.Length + offset) % self.Length;
            var boundedLength = length < 0 ? self.Length - boundedOffset : length;
            return self.Substring(boundedOffset, boundedLength);
        }
    }
}
