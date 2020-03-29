using System;
using System.Linq;

using Intersect.Server.Core.CommandParsing.Tokenization;

using JetBrains.Annotations;

namespace Intersect.Server.Core.CommandParsing
{

    public sealed class ParserSettings
    {

        [NotNull] public static readonly CommandParsingNamespace DefaultLocalization = new CommandParsingNamespace();

        [NotNull] public static readonly string DefaultPrefixLong = @"--";

        [NotNull] public static readonly string DefaultPrefixShort = @"-";

        public ParserSettings(
            [CanBeNull] string prefixShort = null,
            [CanBeNull] string prefixLong = null,
            [CanBeNull] CommandParsingNamespace localization = null,
            [CanBeNull] TokenizerSettings tokenizerSettings = null
        )
        {
            PrefixShort = ValidatePrefix(prefixShort ?? DefaultPrefixShort);
            PrefixLong = ValidatePrefix(prefixLong ?? DefaultPrefixLong);
            Localization = localization ?? DefaultLocalization;
            TokenizerSettings = tokenizerSettings ?? TokenizerSettings.Default;
        }

        [NotNull]
        public static ParserSettings Default => new ParserSettings(
            prefixShort: DefaultPrefixShort, prefixLong: DefaultPrefixLong, localization: DefaultLocalization
        );

        [NotNull]
        public string PrefixShort { get; }

        [NotNull]
        public string PrefixLong { get; }

        [NotNull]
        public CommandParsingNamespace Localization { get; }

        [NotNull]
        public TokenizerSettings TokenizerSettings { get; }

        [NotNull]
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
