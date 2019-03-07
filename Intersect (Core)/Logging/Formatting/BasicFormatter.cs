using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Logging.Formatting
{
    public class BasicFormatter : ILogFormatter
    {

        /// <inheritdoc />
        public string Format(LoggerConfiguration configuration, Exception exception, string message, params object[] args)
        {
            throw new NotImplementedException();
        }

    }
}
