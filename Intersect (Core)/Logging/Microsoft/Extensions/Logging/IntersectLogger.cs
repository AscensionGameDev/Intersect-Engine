using System;
using System.Collections.Immutable;

using Intersect.Extensions;
using Intersect.Logging.Output;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

namespace Intersect.Logging.Microsoft.Extensions.Logging
{

    internal sealed class IntersectLogger : global::Microsoft.Extensions.Logging.ILogger
    {

        public IntersectLogger([CanBeNull] string name)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "INTERSECT" : name;
            Logger = new Logger(
                new LogConfiguration
                {
                    Tag = Name,
                    Outputs = ImmutableList.Create(
                                  new FileOutput(Intersect.Logging.Log.SuggestFilename(null, null, Name + ".ext"))
                              ) ??
                              throw new InvalidOperationException()
                }
            );
        }

        [NotNull]
        public Logger Logger { get; }

        [NotNull]
        public string Name { get; }

        /// <inheritdoc />
        public void Log<TState>(
            global::Microsoft.Extensions.Logging.LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter
        )
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);
            Logger.Write(logLevel.AsIntersectLogLevel(), exception, message);
        }

        /// <inheritdoc />
        public bool IsEnabled(global::Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            return logLevel.AsIntersectLogLevel() < Logger.Configuration.LogLevel;
        }

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
        {
            return new IntersectLogScope();
        }

    }

}
