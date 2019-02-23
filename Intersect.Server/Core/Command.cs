using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Intersect.Core;
using Intersect.Localization;
using Intersect.Logging;
using JetBrains.Annotations;

namespace Intersect.Server.Core
{
    public abstract class Command<TContext> : ICommand
        where TContext : IApplicationContext
    {
        [NotNull] private readonly IList<ICommandArgument> mArguments;

        [NotNull] private readonly IDictionary<char, ICommandArgument> mShortNameLookup;

        [NotNull] private readonly IDictionary<string, ICommandArgument> mNameLookup;

        public ImmutableList<ICommandArgument> Arguments =>
            mArguments.ToImmutableList() ?? throw new InvalidOperationException();

        public Type ContextType => typeof(TContext);

        public string Name => Localization.Name;

        public ICommandArgument FindArgument(char shortName)
        {
            return mArguments.FirstOrDefault(argument => argument?.ShortName == shortName);
        }

        public ICommandArgument FindArgument(string name)
        {
            return mArguments.FirstOrDefault(argument => argument?.Name == name);
        }

        [NotNull]
        public LocaleCommand Localization { get; }

        protected Command(
            [NotNull] LocaleCommand localization,
            [CanBeNull] params ICommandArgument[] arguments
        )
        {
            mArguments = new List<ICommandArgument>((arguments ?? new ICommandArgument[0]).Where(argument => argument != null));

            mShortNameLookup = mArguments.ToDictionary(
                argument => argument?.ShortName ??
                            throw new InvalidOperationException(@"No null arguments should be in the list."),
                argument => argument
            );

            mNameLookup = mArguments.ToDictionary(
                argument => argument?.Name ??
                            throw new InvalidOperationException(@"No null arguments should be in the list."),
                argument => argument
            );

            Localization = localization;
        }
        
        [CanBeNull]
        protected TArgument FindArgument<TArgument>()
        {
            return mArguments
                .Where(argument => argument?.GetType() == typeof(TArgument))
                .Cast<TArgument>()
                .FirstOrDefault();
        }

        [NotNull]
        protected TArgument FindArgumentOrThrow<TArgument>()
        {
            var argument = FindArgument<TArgument>();

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