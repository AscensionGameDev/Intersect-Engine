using System;
using System.Collections.Immutable;
using System.Diagnostics;

using Intersect.Logging;
using Intersect.Logging.Output;

namespace Intersect.Server.Web.RestApi.Logging
{

    internal sealed class IntersectLogger : Microsoft.Owin.Logging.ILogger
    {

        internal IntersectLogger(string name)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "OWIN" : name;
            Logger = new Logger(
                new LogConfiguration
                {
                    Tag = Name,
                    Outputs = ImmutableList.Create(new FileOutput(Log.SuggestFilename(null, null, Name + ".api")))
                }
            );
        }

        public Logger Logger { get; }

        public string Name { get; }

        /// <inheritdoc />
        public bool WriteCore(
            TraceEventType eventType,
            int eventId,
            object state,
            Exception exception,
            Func<object, Exception, string> formatter
        )
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
