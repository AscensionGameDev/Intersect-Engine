using System;

namespace Intersect.Logging.Formatting
{

    public interface ILogFormatter
    {

        string Format(
            LogConfiguration configuration,
            LogLevel logLevel,
            DateTime dateTime,
            Exception exception,
            string message,
            params object[] args
        );

    }

}
