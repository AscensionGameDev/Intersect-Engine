using System.Collections.Generic;

namespace Intersect.Server.Core.CommandParsing.Tokenization
{

    public class Tokenizer
    {

        public Tokenizer() : this(TokenizerSettings.Default)
        {
        }

        public Tokenizer(TokenizerSettings settings)
        {
            Settings = settings;
        }

        public TokenizerSettings Settings { get; }

        public IEnumerable<string> Tokenize(string input)
        {
            var tokens = new List<string>();

            var position = 0;
            while (position < input.Length)
            {
                var chr = input[position];
                if (Settings.AllowQuotedStrings && Settings.QuotationMarks.Contains(chr))
                {
                    tokens.Add(ExtractToken(input, ref position, chr));
                }
                else if (chr == Settings.Delimeter)
                {
                    tokens.Add(ExtractToken(input, ref position, chr));
                }
                else
                {
                    tokens.Add(ExtractToken(input, ref position, Settings.Delimeter));
                }
            }

            return tokens;
        }

        public string ExtractToken(string input, ref int position, char delimeter)
        {
            var offset = input[position] == delimeter ? 1 : 0;
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
