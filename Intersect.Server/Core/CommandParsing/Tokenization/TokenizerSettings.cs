using System.Collections.Immutable;

namespace Intersect.Server.Core.CommandParsing.Tokenization
{

    public sealed class TokenizerSettings
    {

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

        public static TokenizerSettings Default => new TokenizerSettings();

        public bool AllowQuotedStrings { get; }

        public ImmutableArray<char> QuotationMarks { get; }

        public char Delimeter { get; }

    }

}
