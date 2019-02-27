using System.Collections.Generic;
using Intersect.Server.Core.CommandParsing.Arguments;
using JetBrains.Annotations;

namespace Intersect.Server.Core.CommandParsing
{
    public static class ParserResultExtensions
    {
        [CanBeNull]
        public static TValue Find<TValue>([NotNull] this ParserResult result, [NotNull] CommandArgument<TValue> argument, int index = 0)
        {
            return result.Parsed.Find(argument, index);
        }

        [CanBeNull]
        public static IEnumerable<TValues> FindAll<TValues>([NotNull] this ParserResult result, [NotNull] ArrayCommandArgument<TValues> argument)
        {
            return result.Parsed.FindAll(argument);
        }
    }
}