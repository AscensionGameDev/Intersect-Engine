using System;
using System.Text;

namespace Intersect.Logging.Formatting
{
    /// <summary>
    /// Pretty log formatter that does not include timestamps or extended details.
    /// </summary>
    /// <inheritdoc />
    public class PrettyFormatter : DefaultFormatter
    {

        /// <inheritdoc />
        protected override StringBuilder FormatPrefix(
            LogConfiguration configuration,
            LogLevel logLevel,
            DateTime dateTime,
            StringBuilder builder = null
        )
        {
            if (builder == null)
            {
                builder = new StringBuilder();
            }

            // ReSharper disable once InvertIf
            if (!string.IsNullOrEmpty(configuration.Tag))
            {
                builder.Append(configuration.Tag);
                builder.Append(": ");
            }

            return builder;
        }

    }

}
