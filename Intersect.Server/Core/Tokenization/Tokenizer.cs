using JetBrains.Annotations;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Intersect.Server.Core.Tokenization
{
    public class Tokenizer
    {
        public static TokenizerSettings DefaultSettings => new TokenizerSettings
        {
            AllowQuotedStrings = true,
            Delimeter = ' ',
            QuotationMarks = new [] { '"' }.ToImmutableArray()
        };

        public TokenizerSettings Settings { get; }

        public Tokenizer() : this(DefaultSettings)
        {
        }

        public Tokenizer(TokenizerSettings settings)
        {
            Settings = settings;
        }

        [NotNull]
        public IEnumerable<string> Tokenize([NotNull] string input)
        {
            var tokens = new List<string>();

            var position = 0;
            while (position < input.Length)
            {
                var chr = input[position];
                if (Settings.AllowQuotedStrings && Settings.QuotationMarks.Contains(chr))
                {
                    tokens.Add(ExtractToken(input, ref position, chr));
                } else if (chr == Settings.Delimeter)
                {
                    tokens.Add(ExtractToken(input, ref position, chr));
                }
                else
                {
                    tokens.Add(ExtractToken(input, ref position, Settings.Delimeter, 0));
                }
            }

            return tokens;
        }

        public string ExtractToken([NotNull] string input, ref int position, char delimeter, int offset = 1)
        {
            var start = position + offset;
            var next = input.IndexOf(delimeter, start);
            if (next == -1)
            {
                next = input.Length;
            }
            position = next + 1;

            var length = next - start;
            return input.Substring(start, length);
        }
    }
}
