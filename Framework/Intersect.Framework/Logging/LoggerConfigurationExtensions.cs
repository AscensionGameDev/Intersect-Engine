using System.Diagnostics;
using System.Reflection;
using Intersect.Framework.Reflection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Intersect.Framework.Logging;

public static class LoggerConfigurationExtensions
{
    public static (ILoggerFactory, ILogger) CreateLoggerForIntersect(
        this LoggerConfiguration loggerConfiguration,
        Assembly forAssembly,
        string categoryName,
        LoggingLevelSwitch? parentLoggingLevelSwitch = null
    )
    {
        var currentProcess = Process.GetCurrentProcess();
        var processStartTime = currentProcess.StartTime;
        var executableName = Path.GetFileNameWithoutExtension(
            currentProcess.MainModule?.FileName ?? forAssembly.GetName().Name
        );

        LoggingLevelSwitch loggingLevelSwitch =
            new(Debugger.IsAttached ? LogEventLevel.Debug : LogEventLevel.Information);

        if (parentLoggingLevelSwitch is not null)
        {
            parentLoggingLevelSwitch.MinimumLevelChanged +=
                (_, args) => loggingLevelSwitch.MinimumLevel = args.NewLevel;

            loggingLevelSwitch.MinimumLevel = parentLoggingLevelSwitch.MinimumLevel;
        }

        LoggingLevelSwitch errorLevelSwitch = new();

        var serilogLogger = loggerConfiguration
            .MinimumLevel.ControlledBy(loggingLevelSwitch)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                Path.Combine(
                    "logs",
                    $"{executableName}-{processStartTime:yyyy_MM_dd-HH_mm_ss_fff}.log"
                ),
                rollOnFileSizeLimit: true,
                retainedFileTimeLimit: TimeSpan.FromDays(30)
            )
            .WriteTo.File(
                Path.Combine("logs", $"errors-{executableName}.log"),
                levelSwitch: errorLevelSwitch,
                rollOnFileSizeLimit: true,
                retainedFileTimeLimit: TimeSpan.FromDays(30)
            )
            .CreateLogger();

        var loggerFactory = new SerilogLoggerFactory(serilogLogger, dispose: true);
        var logger = loggerFactory.CreateLogger(categoryName);

        logger.LogInformation(
            "Starting {AssemblyName} v{AssemblyVersion}",
            forAssembly.GetName().Name,
            forAssembly.GetMetadataVersion()
        );

        errorLevelSwitch.MinimumLevel = LogEventLevel.Error;

        return (loggerFactory, logger);
    }
}