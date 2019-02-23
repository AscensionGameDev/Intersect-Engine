using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Intersect.Server.Core
{
    public sealed class ArgumentValuesMap : IEnumerable<KeyValuePair<ICommandArgument, ArgumentValues>>
    {
        [NotNull] private readonly IDictionary<ICommandArgument, ArgumentValues> mValuesMap;

        public ArgumentValuesMap([CanBeNull] IEnumerable<KeyValuePair<ICommandArgument, ArgumentValues>> pairs = null)
        {
            mValuesMap = pairs?.ToDictionary(pair => pair.Key, pair => pair.Value) ?? new Dictionary<ICommandArgument, ArgumentValues>();
        }

        public IEnumerator<KeyValuePair<ICommandArgument, ArgumentValues>> GetEnumerator() => mValuesMap.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [CanBeNull]
        public ArgumentValues Find([NotNull] ICommandArgument argument)
        {
            return mValuesMap.TryGetValue(argument, out var values) ? values : null;
        }

        [CanBeNull]
        public TValue Find<TValue>([NotNull] ICommandArgument argument)
        {
            var argumentValues = Find(argument);
            return argumentValues == null ? argument.DefaultValueAsType<TValue>() : argumentValues.ToTypedValue<TValue>();
        }

        [CanBeNull]
        public IEnumerable<TValues> FindAll<TValues>([NotNull] ICommandArgument argument)
        {
            return Find(argument)?.ToTypedValues<TValues>() ?? argument.DefaultValueAsType<IEnumerable<TValues>>();
        }

        [CanBeNull]
        public TValue Find<TValue>([NotNull] CommandArgument<TValue> argument)
        {
            return Find<TValue>(argument as ICommandArgument);
        }

        [CanBeNull]
        public IEnumerable<TValues> FindAll<TValues>([NotNull] ArrayCommandArgument<TValues> argument)
        {
            return FindAll<TValues>(argument as ICommandArgument);
        }

        [NotNull]
        public ParserResult AsResult(ICommand command = null)
        {
            return new ParserResult(command, this);
        }

        [NotNull]
        public ParserResult<TCommand> AsResult<TCommand>(TCommand command)
            where TCommand : ICommand
        {
            return new ParserResult<TCommand>(command, this);
        }
    }
}