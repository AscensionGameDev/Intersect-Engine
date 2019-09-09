using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Intersect.Server.Core.CommandParsing.Tokenization
{
    public sealed class TokenizerSettings
    {
        [NotNull]
        public static TokenizerSettings Default => new TokenizerSettings();

        public bool AllowQuotedStrings { get; }

        public ImmutableArray<char> QuotationMarks { get; }

        public char Delimeter { get; }

        public TokenizerSettings(
            bool allowQuotedStrings = true,
            ImmutableArray<char>? quotationMarks = null,
            char delimeter = ' '
        )
        {
            AllowQuotedStrings = allowQuotedStrings;
            QuotationMarks = quotationMarks ?? "\"".ToImmutableArray();
            Delimeter = delimeter;
        }
    }
}