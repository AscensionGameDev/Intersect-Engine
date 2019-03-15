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
            int index = 0
        )
        {
            return result.Parsed.Find(argument, index);
        }

        [CanBeNull]
        public static IEnumerable<TValues> FindAll<TValues>(
            [NotNull] this ParserResult result,
            [NotNull] ArrayCommandArgument<TValues> argument
        )
        {
            return result.Parsed.FindAll(argument);
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
                Parsed = result.Parsed.Values.Where(
                        pair =>
                            !filterOmitted || !result.Omitted.Contains(pair.Key) ||
                            !(filterOut?.Contains(pair.Key) ?? false)
                    )
                    .ToImmutableDictionary()
            };
        }
    }
}