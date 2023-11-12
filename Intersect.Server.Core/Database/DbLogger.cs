﻿using Microsoft.Extensions.Logging;

namespace Intersect.Server.Database;

public partial class DbLogger : ILogger
{
    private readonly Intersect.Logging.Logger _intersectLogger;

    public DbLogger(Intersect.Logging.Logger intersectLogger)
    {
        _intersectLogger = intersectLogger;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
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

        var msg = $"{eventId.Id} - {formatter(state, exception)}";
        switch (logLevel)
        {
            case LogLevel.Trace:
                _intersectLogger.Trace(msg);
                break;

            case LogLevel.Debug:
                _intersectLogger.Debug(msg);
                break;

            case LogLevel.Information:
                _intersectLogger.Info(msg);
                break;

            case LogLevel.Warning:
                _intersectLogger.Warn(msg);
                break;

            case LogLevel.Error:
                _intersectLogger.Error(msg);
                break;

            case LogLevel.Critical:
                _intersectLogger.Error(msg);
                break;

            case LogLevel.None:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
        }
    }
}