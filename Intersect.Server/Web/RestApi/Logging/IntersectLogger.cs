using Intersect.Logging;
using System;
using System.Diagnostics;

using JetBrains.Annotations;
using System.Collections.Immutable;
using Intersect.Logging.Output;

namespace Intersect.Server.Web.RestApi.Logging
{
    internal sealed class IntersectLogger : Microsoft.Owin.Logging.ILogger
    {
        [NotNull]
        public Logger Logger { get; }

        [NotNull]
        public string Name { get; }

        internal IntersectLogger([NotNull] string name)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "OWIN" : name;
            Logger = new Logger(new LogConfiguration
            {
                Tag = Name,
                Outputs = ImmutableList.Create(new FileOutput(Log.SuggestFilename(null, null, Name + ".api")))
            });
        }

        /// <inheritdoc />
        public bool WriteCore(TraceEventType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            var message = formatter?.Invoke(state, exception) ?? exception?.Message;

            switch (eventType)
            {
                case TraceEventType.Start:
                case TraceEventType.Resume:
                case TraceEventType.Suspend:
                case TraceEventType.Stop:
                case TraceEventType.Transfer:
                    Logger.Diagnostic(message);
                    return true;

                case TraceEventType.Information:
                    Logger.Info(message);
                    return true;

                case TraceEventType.Critical:
                case TraceEventType.Error:
                    Logger.Error(exception, message);
                    return true;

                case TraceEventType.Warning:
                    Logger.Warn(exception, message);
                    return true;

                case TraceEventType.Verbose:
                    Logger.Verbose(message);
                    return true;

                default:
                    throw new ArgumentOutOfRangeException(nameof(eventType));
            }
        }

    }
}
