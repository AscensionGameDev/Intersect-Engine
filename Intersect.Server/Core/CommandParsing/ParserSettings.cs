using System;
using System.Linq;

using Intersect.Server.Core.CommandParsing.Tokenization;

namespace Intersect.Server.Core.CommandParsing
{

    public sealed class ParserSettings
    {

        public static readonly CommandParsingNamespace DefaultLocalization = new CommandParsingNamespace();

        public static readonly string DefaultPrefixLong = @"--";

        public static readonly string DefaultPrefixShort = @"-";

        public ParserSettings(
            string prefixShort = null,
            string prefixLong = null,
            CommandParsingNamespace localization = null,
            TokenizerSettings tokenizerSettings = null
        )
        {
            PrefixShort = ValidatePrefix(prefixShort ?? DefaultPrefixShort);
            PrefixLong = ValidatePrefix(prefixLong ?? DefaultPrefixLong);
            Localization = localization ?? DefaultLocalization;
            TokenizerSettings = tokenizerSettings ?? TokenizerSettings.Default;
        }

        public static ParserSettings Default => new ParserSettings(
            prefixShort: DefaultPrefixShort, prefixLong: DefaultPrefixLong, localization: DefaultLocalization
        );

        public string PrefixShort { get; }

        public string PrefixLong { get; }

        public CommandParsingNamespace Localization { get; }

        public TokenizerSettings TokenizerSettings { get; }

        public static string ValidatePrefix(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                throw new ArgumentException(@"Prefix cannot be null, empty, or whitespace.");
            }

            if (prefix.Contains('='))
            {
                throw new ArgumentException(@"Prefixes cannot contain '='.");
            }

            if (prefix.Contains(' ') || prefix.Contains('\n') || prefix.Contains('\r') || prefix.Contains('\t'))
            {
                throw new ArgumentException(@"Prefixes cannot contain whitespace.");
            }

            if (prefix.Contains('\0'))
            {
                throw new ArgumentException(@"Prefixes cannot contain the null character.");
            }

            return prefix;
        }

    }

}
