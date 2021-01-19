using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

using Intersect.Core;
using Intersect.Localization;
using Intersect.Server.Core.CommandParsing.Arguments;

namespace Intersect.Server.Core.CommandParsing.Commands
{

    public abstract class Command<TContext> : ICommand where TContext : IApplicationContext
    {

        protected Command(LocaleCommand localization, params ICommandArgument[] arguments)
        {
            Localization = localization;

            var argumentList = new List<ICommandArgument>(
                (arguments ?? Array.Empty<ICommandArgument>()).Where(argument => argument != null)
            );

            UnsortedArguments = argumentList.ToImmutableList() ?? throw new InvalidOperationException();

            argumentList.Sort(
                (a, b) =>
                {
                    if (a == null)
                    {
                        return 1;
                    }

                    if (b == null)
                    {
                        return -1;
                    }

                    if (a.IsRequiredByDefault && !b.IsRequiredByDefault)
                    {
                        return -1;
                    }

                    if (!a.IsRequiredByDefault && b.IsRequiredByDefault)
                    {
                        return 1;
                    }

                    if (a.IsPositional)
                    {
                        return b.IsPositional ? 0 : -1;
                    }

                    return b.IsPositional ? 1 : 0;
                }
            );

            Arguments = argumentList.ToImmutableList() ?? throw new InvalidOperationException();

            NamedArguments =
                argumentList.Where(
                        argument => !argument?.IsPositional ??
                                    throw new InvalidOperationException(@"No null arguments should be in the list.")
                    )
                    .ToImmutableList() ??
                throw new InvalidOperationException();

            PositionalArguments =
                argumentList.Where(
                        argument => argument?.IsPositional ??
                                    throw new InvalidOperationException(@"No null arguments should be in the list.")
                    )
                    .ToImmutableList() ??
                throw new InvalidOperationException();
        }

        public LocaleCommand Localization { get; }

        public ImmutableList<ICommandArgument> Arguments { get; }

        public ImmutableList<ICommandArgument> UnsortedArguments { get; }

        public ImmutableList<ICommandArgument> NamedArguments { get; }

        public ImmutableList<ICommandArgument> PositionalArguments { get; }

        public Type ContextType => typeof(TContext);

        public string Name => Localization.Name;

        public string Description => Localization.Description;

        public ICommandArgument FindArgument(char shortName)
        {
            return Arguments.FirstOrDefault(argument => argument?.ShortName == shortName);
        }

        public ICommandArgument FindArgument(string name)
        {
            return Arguments.FirstOrDefault(argument => argument?.Name == name);
        }

        public string FormatUsage(ParserSettings parserSettings, ParserContext parserContext, bool formatPrint = false)
        {
            var usageBuilder = new StringBuilder(Name);

            Arguments.ForEach(
                argument =>
                {
                    if (argument == null)
                    {
                        return;
                    }

                    var argumentUsageBuilder = new StringBuilder(argument.Name);
                    if (!argument.IsPositional)
                    {
                        argumentUsageBuilder.Insert(0, parserSettings.PrefixLong);
                    }

                    if (!argument.IsFlag)
                    {
                        var typeName = argument.ValueType.Name;
                        if (parserSettings.Localization.TypeNames.TryGetValue(typeName, out var localizedType))
                        {
                            typeName = localizedType;
                        }

                        if (argument.IsPositional)
                        {
                            argumentUsageBuilder.Append(parserSettings.Localization.Formatting.Type.ToString(typeName));
                        }
                        else
                        {
                            argumentUsageBuilder.Append('=');
                            argumentUsageBuilder.Append(typeName);
                        }
                    }

                    var argumentUsage = argumentUsageBuilder.ToString();
                    if (!argument.IsRequired(parserContext))
                    {
                        argumentUsage = parserSettings.Localization.Formatting.Optional.ToString(argumentUsage);
                    }

                    usageBuilder.Append(' ');
                    usageBuilder.Append(argumentUsage);
                }
            );

            var usage = usageBuilder.ToString().Trim();

            if (formatPrint)
            {
                usage = parserSettings.Localization.Formatting.Usage.ToString(usage);
            }

            return usage;
        }

        public void Handle(IApplicationContext context, ParserResult result)
        {
            if (result.Errors.Any(error => error?.IsFatal ?? false))
            {
                throw new InvalidOperationException(
                    @"Errors should have been handled before invoking ICommand.Handle()."
                );
            }

            if (context.GetType() != ContextType)
            {
                throw new ArgumentException(
                    $@"Expected {ContextType.FullName} not {context.GetType().FullName}.", nameof(context)
                );
            }

            Handle((TContext) context, result);
        }

        protected TArgument FindArgument<TArgument>(int index = 0)
        {
            return Arguments.Where(argument => argument?.GetType() == typeof(TArgument))
                .Cast<TArgument>()
                .ElementAtOrDefault(index);
        }

        protected TArgument FindArgumentOrThrow<TArgument>(int index = 0)
        {
            var argument = FindArgument<TArgument>(index);

            if (argument == null)
            {
                throw new InvalidOperationException($@"Unable to find argument type {typeof(TArgument).FullName}.");
            }

            return argument;
        }

        protected abstract void Handle(TContext context, ParserResult result);

    }

}
