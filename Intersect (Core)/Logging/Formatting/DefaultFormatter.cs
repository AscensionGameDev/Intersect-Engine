using System;
using System.Text;

using JetBrains.Annotations;

namespace Intersect.Logging.Formatting
{

    /// <summary>
    /// Basic formatter for text logs, includes details and timestamps.
    /// </summary>
    public class DefaultFormatter : ILogFormatter
    {

        public const string DefaultTimestampFormat = "yyyy-MM-dd HH:mm:ss.fff";

        public DefaultFormatter()
        {
            TimestampFormat = DefaultTimestampFormat;
        }

        [CanBeNull] public string TimestampFormat { get; set; }

        /// <inheritdoc />
        public string Format(
            LogConfiguration configuration,
            LogLevel logLevel,
            DateTime dateTime,
            Exception exception,
            string message,
            params object[] args
        )
        {
            var prefix = FormatPrefix(configuration, logLevel, dateTime).ToString();
            var builder = new StringBuilder();

            if (message != null)
            {
                FormatLine(builder, prefix, message, args);
            }

            if (exception != null)
            {
                FormatLine(builder, prefix, exception);
            }

            return builder.ToString();
        }

        [NotNull]
        protected virtual StringBuilder FormatPrefix(
            [NotNull] LogConfiguration configuration,
            LogLevel logLevel,
            DateTime dateTime,
            [CanBeNull] StringBuilder builder = null
        )
        {
            if (builder == null)
            {
                builder = new StringBuilder();
            }

            if (!string.IsNullOrWhiteSpace(TimestampFormat))
            {
                builder.Append(dateTime.ToString(TimestampFormat));
                builder.Append(' ');
            }

            builder.Append($"[{logLevel}]");
            builder.Append(' ');

            // ReSharper disable once InvertIf
            if (!string.IsNullOrEmpty(configuration.Tag))
            {
                builder.Append(configuration.Tag);
                builder.Append(": ");
            }

            return builder;
        }

        private static void FormatLine(
            [NotNull] StringBuilder builder,
            [NotNull] string prefix,
            [NotNull] string message,
            params object[] args
        )
        {
            builder.Append(prefix);

            if (args?.Length > 0)
            {
                builder.AppendFormat(message, args);
            }
            else
            {
                builder.Append(message);
            }

            builder.AppendLine();
        }

        private static void FormatLine(
            [NotNull] StringBuilder builder,
            [CanBeNull] string prefix,
            [NotNull] Exception exception,
            bool recurse = true
        )
        {
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                builder.AppendLine(prefix);
            }

            builder.AppendLine($@"  Message: {exception.Message}");

            if (exception.StackTrace?.Length < 10000)
            {
                builder.AppendLine($@"    Stack: {exception.StackTrace}");
            }

            builder.AppendLine();

            if (!recurse)
            {
                return;
            }

            var innerException = exception;
            while ((innerException = innerException.InnerException) != null)
            {
                builder.AppendLine(@"Caused By");
                FormatLine(builder, null, innerException, false);
            }
        }

    }

}
