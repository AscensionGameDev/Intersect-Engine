using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace Intersect.Logging.Formatting
{
    public interface ILogFormatter
    {

        string Format(
            [NotNull] LogConfiguration configuration,
            LogLevel logLevel,
            DateTime dateTime,
            [CanBeNull] Exception exception,
            [CanBeNull] string message,
            [CanBeNull] params object[] args
        );

    }
}
