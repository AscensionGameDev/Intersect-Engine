using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Intersect.Server.Core.CommandParsing.Arguments;

namespace Intersect.Server.Core.CommandParsing
{

    public static class ParserResultExtensions
    {

        public static TValue Find<TValue>(
            this ParserResult result,
            CommandArgument<TValue> argument,
            int index = 0,
            bool allowImplicit = true
        )
        {
            return result.Parsed.Find<TValue>(argument, index, allowImplicit);
        }

        public static IEnumerable<TValue> FindAll<TValue>(
            this ParserResult result,
            ArrayCommandArgument<TValue> argument
        )
        {
            return result.Parsed.FindAll(argument);
        }

        public static bool TryFind<TValue>(
            this ParserResult result,
            CommandArgument<TValue> argument,
            out TValue value,
            int index = 0,
            bool allowImplicit = true
        )
        {
            return result.Parsed.TryFind(argument, out value, index, allowImplicit);
        }

        public static bool TryFindAll<TValue>(
            this ParserResult result,
            ArrayCommandArgument<TValue> argument,
            out IEnumerable<TValue> values
        )
        {
            return result.Parsed.TryFindAll(argument, out values);
        }

        public static ParserContext AsContext(
            this ParserResult result,
            bool filterOmitted = false,
            IEnumerable<ICommandArgument> filterOut = null
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
