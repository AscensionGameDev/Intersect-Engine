using CommandLine;
using Intersect.Framework.Reflection;
using Intersect.Plugins.Interfaces;
using Microsoft.Extensions.Logging;

namespace Intersect.Plugins.Helpers;

/// <inheritdoc cref="ICommandLineHelper" />
internal sealed partial class CommandLineHelper : PluginHelper, ICommandLineHelper
{
    private readonly Parser mParser;

    private readonly string[] mArgs;

    internal CommandLineHelper(ILogger logger, string[] args, Parser parser) : base(
        logger
    )
    {
        mArgs = args;
        mParser = parser;
    }

    /// <inheritdoc />
    public TArguments ParseArguments<TArguments>() =>
        mParser.ParseArguments<TArguments>(mArgs).MapResult(arguments => arguments, HandleErrors<TArguments>);

    private TArguments HandleErrors<TArguments>(IEnumerable<Error> errors)
    {
        var errorsAsList = errors?.ToList();

        var fatalParsingError = errorsAsList?.Any(error => error?.StopsProcessing ?? false) ?? false;

        var errorString = string.Join(
            ", ", errorsAsList?.ToList().Select(error => error?.ToString()) ?? Array.Empty<string>()
        );

        var exception = new ArgumentException(
            $@"Error parsing plugin arguments of type {typeof(TArguments).FullName}, received the following errors: {errorString}"
        );

        if (fatalParsingError)
        {
            Logger.LogCritical(
                exception,
                "Failed to parse {ArgumentsType}",
                typeof(TArguments).GetName(qualified: true)
            );
        }
        else
        {
            Logger.LogWarning(
                exception,
                "Failed to parse {ArgumentsType}",
                typeof(TArguments).GetName(qualified: true)
            );
        }

        return default;
    }
}
