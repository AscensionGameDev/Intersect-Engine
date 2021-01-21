using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Intersect.Server.Core.CommandParsing.Commands;

namespace Intersect.Server.Core.CommandParsing.Arguments
{

    public sealed class ArgumentValuesMap : IEnumerable<KeyValuePair<ICommandArgument, ArgumentValues>>
    {

        private readonly IDictionary<ICommandArgument, ArgumentValues> mValuesMap;

        public ArgumentValuesMap(IEnumerable<KeyValuePair<ICommandArgument, ArgumentValues>> pairs = null)
        {
            mValuesMap = pairs?.ToDictionary(pair => pair.Key, pair => pair.Value) ??
                         new Dictionary<ICommandArgument, ArgumentValues>();
        }

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

        public ArgumentValues Find(ICommandArgument argument)
        {
            return TryFind(argument, out var values) ? values : null;
        }

        public bool TryFind(ICommandArgument argument, out ArgumentValues values)
        {
            return mValuesMap.TryGetValue(argument, out values);
        }

        public TValue Find<TValue>(ICommandArgument argument, int index = 0, bool allowImplicit = true)
        {
            TryFind(argument, out TValue value, index, allowImplicit);

            return value;
        }

        public bool TryFind<TValue>(
            ICommandArgument argument,
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

        public IEnumerable<TValue> FindAll<TValue>(ICommandArgument argument)
        {
            TryFindAll<TValue>(argument, out var values);

            return values;
        }

        public bool TryFindAll<TValue>(ICommandArgument argument, out IEnumerable<TValue> values)
        {
            if (TryFind(argument, out var argumentValues))
            {
                values = argumentValues.ToTypedValues<TValue>();

                return true;
            }

            values = argument.DefaultValueAsType<IEnumerable<TValue>>();

            return false;
        }

        public TValue Find<TValue>(CommandArgument<TValue> argument, int index = 0)
        {
            return Find<TValue>(argument as ICommandArgument, index);
        }

        public bool TryFind<TValue>(CommandArgument<TValue> argument, out TValue value, int index = 0)
        {
            return TryFind(argument as ICommandArgument, out value, index);
        }

        public IEnumerable<TValue> FindAll<TValue>(ArrayCommandArgument<TValue> argument)
        {
            return FindAll<TValue>(argument as ICommandArgument);
        }

        public bool TryFindAll<TValue>(ArrayCommandArgument<TValue> argument, out IEnumerable<TValue> value)
        {
            return TryFindAll(argument as ICommandArgument, out value);
        }

        public ParserResult AsResult(ICommand command = null)
        {
            return new ParserResult(command, this);
        }

        public ParserResult<TCommand> AsResult<TCommand>(TCommand command) where TCommand : ICommand
        {
            return new ParserResult<TCommand>(command, this);
        }

    }

}
