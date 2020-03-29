using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Intersect.Server.Core.CommandParsing.Arguments;

using JetBrains.Annotations;

namespace Intersect.Server.Core.CommandParsing
{

    public static class ParserResultExtensions
    {

        [CanBeNull]
        public static TValue Find<TValue>(
            [NotNull] this ParserResult result,
            [NotNull] CommandArgument<TValue> argument,
            int index = 0,
            bool allowImplicit = true
        )
        {
            return result.Parsed.Find<TValue>(argument, index, allowImplicit);
        }

        [CanBeNull]
        public static IEnumerable<TValue> FindAll<TValue>(
            [NotNull] this ParserResult result,
            [NotNull] ArrayCommandArgument<TValue> argument
        )
        {
            return result.Parsed.FindAll(argument);
        }

        public static bool TryFind<TValue>(
            [NotNull] this ParserResult result,
            [NotNull] CommandArgument<TValue> argument,
            out TValue value,
            int index = 0,
            bool allowImplicit = true
        )
        {
            return result.Parsed.TryFind(argument, out value, index, allowImplicit);
        }

        public static bool TryFindAll<TValue>(
            [NotNull] this ParserResult result,
            [NotNull] ArrayCommandArgument<TValue> argument,
            out IEnumerable<TValue> values
        )
        {
            return result.Parsed.TryFindAll(argument, out values);
        }

        public static ParserContext AsContext(
            [NotNull] this ParserResult result,
            bool filterOmitted = false,
            [CanBeNull] IEnumerable<ICommandArgument> filterOut = null
        )
        {
            return new ParserContext
            {
                Command = result.Command,
                Errors = result.Errors,
                Parsed = result.Parsed.Values
                    .Where(
                        pair => !filterOmitted ||
                                !result.Omitted.Contains(pair.Key) ||
                                !(filterOut?.Contains(pair.Key) ?? false)
                    )
                    .ToImmutableDictionary()
            };
        }

    }

}
