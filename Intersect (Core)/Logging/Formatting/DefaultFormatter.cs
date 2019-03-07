using System;
using System.Text;
using JetBrains.Annotations;

namespace Intersect.Logging.Formatting
{
    public class DefaultFormatter : ILogFormatter
    {
        public const string DefaultTimestampFormat = "yyyy-MM-dd HH:mm:ss.fff";

        [CanBeNull]
        public string TimestampFormat { get; set; }

        public DefaultFormatter()
        {
            TimestampFormat = DefaultTimestampFormat;
        }

        private string FormatPrefix([NotNull] LogConfiguration configuration, LogLevel logLevel, DateTime dateTime)
        {
            var builder = new StringBuilder();

            if (!configuration.Pretty)
            {
                if (!string.IsNullOrWhiteSpace(TimestampFormat))
                {
                    builder.Append(dateTime.ToString(TimestampFormat));
                    builder.Append(' ');
                }

                builder.Append($"[{logLevel}]");
                builder.Append(' ');
            }

            if (!string.IsNullOrEmpty(configuration.Tag))
            {
                builder.Append(configuration.Tag);
                builder.Append(": ");
            }

            return builder.ToString();
        }

        private void FormatLine([NotNull] StringBuilder builder, [NotNull] string prefix, [NotNull] string message, params object[] args)
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

        private void FormatLine([NotNull] StringBuilder builder, [CanBeNull] string prefix, [NotNull] Exception exception, bool recurse = true)
        {
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                builder.AppendLine(prefix);
            }

            builder.AppendLine($@"  Message: {exception.Message}");
            builder.AppendLine($@"    Stack: {exception.StackTrace}");
            builder.AppendLine();

            if (!recurse)
            {
                return;
            }

            Exception innerException;
            while ((innerException = exception.InnerException) != null)
            {
                builder.AppendLine(@"Caused By");
                FormatLine(builder, null, innerException, false);
            }
        }

        /// <inheritdoc />
        public string Format(LogConfiguration configuration, LogLevel logLevel, DateTime dateTime, Exception exception, string message, params object[] args)
        {
            var prefix = FormatPrefix(configuration, logLevel, dateTime);
            var builder = new StringBuilder();

            FormatLine(builder, prefix, message, args);
            FormatLine(builder, prefix, exception);

            return builder.ToString();
        }

    }
}
