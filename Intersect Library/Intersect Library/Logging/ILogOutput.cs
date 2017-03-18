using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Library.Logging
{
    public interface ILogOutput
    {
        LogLevel LogLevel { get; set; }

        void Write(string tag, LogLevel logLevel, string message);
        void Write(string tag, LogLevel logLevel, string format, params object[] args);
        void Write(string tag, LogLevel logLevel, Exception exception);
    }
}
