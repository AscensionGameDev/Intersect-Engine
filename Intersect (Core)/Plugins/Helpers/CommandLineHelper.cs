using System;
using System.Collections.Generic;
using System.Linq;

using CommandLine;

using Intersect.Logging;
using Intersect.Plugins.Interfaces;

using JetBrains.Annotations;

namespace Intersect.Plugins.Helpers
{
    /// <inheritdoc cref="ICommandLineHelper" />
    internal sealed class CommandLineHelper : PluginHelper, ICommandLineHelper
    {
        [NotNull] private readonly Parser mParser;

        [NotNull] private readonly string[] mArgs;

        internal CommandLineHelper([NotNull] Logger logger, [NotNull] string[] args, [NotNull] Parser parser) : base(
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
                Logger.Error(exception);
            }
            else
            {
                Logger.Warn(exception);
            }

            return default;
        }
    }
}
