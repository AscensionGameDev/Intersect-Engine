using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Intersect.Core;
using Intersect.Localization;
using Intersect.Server.Core.CommandParsing.Arguments;
using JetBrains.Annotations;

namespace Intersect.Server.Core.CommandParsing.Commands
{
    public abstract class Command<TContext> : ICommand
        where TContext : IApplicationContext
    {
        [NotNull] private readonly IDictionary<char, ICommandArgument> mShortNameLookup;

        [NotNull] private readonly IDictionary<string, ICommandArgument> mNameLookup;

        public ImmutableList<ICommandArgument> Arguments { get; }

        public ImmutableList<ICommandArgument> NamedArguments { get; }

        public ImmutableList<ICommandArgument> PositionalArguments { get; }

        public Type ContextType => typeof(TContext);

        public string Name => Localization.Name;

        public ICommandArgument FindArgument(char shortName)
        {
            return Arguments.FirstOrDefault(argument => argument?.ShortName == shortName);
        }

        public ICommandArgument FindArgument(string name)
        {
            return Arguments.FirstOrDefault(argument => argument?.Name == name);
        }

        [NotNull]
        public LocaleCommand Localization { get; }

        protected Command(
            [NotNull] LocaleCommand localization,
            [CanBeNull] params ICommandArgument[] arguments
        )
        {
            Localization = localization;

            var argumentList = new List<ICommandArgument>((arguments ?? new ICommandArgument[0]).Where(argument => argument != null));

            Arguments = argumentList.ToImmutableList() ?? throw new InvalidOperationException();

            NamedArguments = argumentList.Where(argument =>
                                      !argument?.IsPositional ??
                                      throw new InvalidOperationException(@"No null arguments should be in the list.")
                                  ).ToImmutableList() ?? throw new InvalidOperationException();

            PositionalArguments = argumentList.Where(argument =>
                                      argument?.IsPositional ??
                                      throw new InvalidOperationException(@"No null arguments should be in the list.")
                                  ).ToImmutableList() ?? throw new InvalidOperationException();

            mShortNameLookup = NamedArguments.ToDictionary(
                argument => argument?.ShortName ??
                            throw new InvalidOperationException(@"No null arguments should be in the list."),
                argument => argument
            );

            mNameLookup = NamedArguments.ToDictionary(
                argument => argument?.Name ??
                            throw new InvalidOperationException(@"No null arguments should be in the list."),
                argument => argument
            );
        }

        [CanBeNull]
        protected TArgument FindArgument<TArgument>(int index = 0)
        {
            return Arguments
                .Where(argument => argument?.GetType() == typeof(TArgument))
                .Cast<TArgument>()
                .ElementAtOrDefault(index);
        }

        [NotNull]
        protected TArgument FindArgumentOrThrow<TArgument>(int index = 0)
        {
            var argument = FindArgument<TArgument>(index);

            if (argument == null)
            {
                throw new InvalidOperationException($@"Unable to find argument type {typeof(TArgument).FullName}.");
            }

            return argument;
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
                throw new ArgumentException($@"Expected {ContextType.FullName} not {context.GetType().FullName}.",
                    nameof(context));
            }

            Handle((TContext) context, result);
        }

        protected abstract void Handle([NotNull] TContext context, [NotNull] ParserResult result);
    }
}