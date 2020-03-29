namespace Intersect.Logging
{

    public enum LogLevel : byte
    {

        None = 0,

        Error = 0x20,

        Warn = 0x40,

        Info = 0x60,

        Trace = 0x80,

        Verbose = 0xA0,

        Debug = 0xC0,

        Diagnostic = 0xE0,

        All = 0xFF

    }

}
