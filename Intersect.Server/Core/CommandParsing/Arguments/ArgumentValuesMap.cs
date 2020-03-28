using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Intersect.Server.Core.CommandParsing.Commands;

using JetBrains.Annotations;

namespace Intersect.Server.Core.CommandParsing.Arguments
{

    public sealed class ArgumentValuesMap : IEnumerable<KeyValuePair<ICommandArgument, ArgumentValues>>
    {

        [NotNull] private readonly IDictionary<ICommandArgument, ArgumentValues> mValuesMap;

        public ArgumentValuesMap([CanBeNull] IEnumerable<KeyValuePair<ICommandArgument, ArgumentValues>> pairs = null)
        {
            mValuesMap = pairs?.ToDictionary(pair => pair.Key, pair => pair.Value) ??
                         new Dictionary<ICommandArgument, ArgumentValues>();
        }

        [NotNull]
        public ImmutableDictionary<ICommandArgument, ArgumentValues> Values =>
            mValuesMap.ToImmutableDictionary() ?? throw new InvalidOperationException();

        public IEnumerator<KeyValuePair<ICommandArgument, ArgumentValues>> GetEnumerator()
        {
            return mValuesMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [CanBeNull]
        public ArgumentValues Find([NotNull] ICommandArgument argument)
        {
            return TryFind(argument, out var values) ? values : null;
        }

        public bool TryFind([NotNull] ICommandArgument argument, out ArgumentValues values)
        {
            return mValuesMap.TryGetValue(argument, out values);
        }

        [CanBeNull]
        public TValue Find<TValue>([NotNull] ICommandArgument argument, int index = 0, bool allowImplicit = true)
        {
            TryFind(argument, out TValue value, index, allowImplicit);

            return value;
        }

        public bool TryFind<TValue>(
            [NotNull] ICommandArgument argument,
            out TValue value,
            int index = 0,
            bool allowImplicit = true
        )
        {
            if (TryFind(argument, out var argumentValues) && (allowImplicit || !argumentValues.IsImplicit))
            {
                value = argumentValues.ToTypedValue<TValue>(index);

                return true;
            }

            value = argument.DefaultValueAsType<TValue>();

            return false;
        }

        [CanBeNull]
        public IEnumerable<TValue> FindAll<TValue>([NotNull] ICommandArgument argument)
        {
            TryFindAll<TValue>(argument, out var values);

            return values;
        }

        public bool TryFindAll<TValue>([NotNull] ICommandArgument argument, out IEnumerable<TValue> values)
        {
            if (TryFind(argument, out var argumentValues))
            {
                values = argumentValues.ToTypedValues<TValue>();

                return true;
            }

            values = argument.DefaultValueAsType<IEnumerable<TValue>>();

            return false;
        }

        [CanBeNull]
        public TValue Find<TValue>([NotNull] CommandArgument<TValue> argument, int index = 0)
        {
            return Find<TValue>(argument as ICommandArgument, index);
        }

        public bool TryFind<TValue>([NotNull] CommandArgument<TValue> argument, out TValue value, int index = 0)
        {
            return TryFind(argument as ICommandArgument, out value, index);
        }

        [CanBeNull]
        public IEnumerable<TValue> FindAll<TValue>([NotNull] ArrayCommandArgument<TValue> argument)
        {
            return FindAll<TValue>(argument as ICommandArgument);
        }

        public bool TryFindAll<TValue>([NotNull] ArrayCommandArgument<TValue> argument, out IEnumerable<TValue> value)
        {
            return TryFindAll(argument as ICommandArgument, out value);
        }

        [NotNull]
        public ParserResult AsResult(ICommand command = null)
        {
            return new ParserResult(command, this);
        }

        [NotNull]
        public ParserResult<TCommand> AsResult<TCommand>(TCommand command) where TCommand : ICommand
        {
            return new ParserResult<TCommand>(command, this);
        }

    }

}
