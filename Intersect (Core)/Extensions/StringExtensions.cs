using System;
using System.Globalization;

using JetBrains.Annotations;

namespace Intersect.Extensions
{
    /// <summary>
    /// Extensions for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
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

        [NotNull]
        public static string TerminateWith(this string self, char terminateWith, bool condition = true) =>
            TerminateWith(self, new[] {terminateWith}, condition);

        [NotNull]
        public static string TerminateWith(this string self, char[] terminateWith, bool condition = true) =>
            TerminateWith(self, new string(terminateWith), condition);

        [NotNull]
        public static string TerminateWith(this string self, [NotNull] string terminateWith, bool condition = true)
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
    }
}
