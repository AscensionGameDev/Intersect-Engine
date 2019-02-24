using System.Collections.Immutable;

namespace Intersect.Server.Core.Tokenization
{
    public struct TokenizerSettings
    {
        public bool AllowQuotedStrings { get; set; }

        public ImmutableArray<char> QuotationMarks { get; set; }

        public char Delimeter { get; set; }
    }
}