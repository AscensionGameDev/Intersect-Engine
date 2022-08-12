using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace Intersect.Localization
{
    [Serializable]
    public partial class LocalizedString : Localized
    {
        private static readonly IEnumerable<string> _patternParameters =
            Enum.GetValues<Pluralization>()
                .Cast<object>()
                .Concat(
                    Enum.GetValues<StringCase>()
                        .Cast<object>()
                )
                .Select(value => value.ToString());
        private static readonly Regex PatternCasePlural = new($@"{{(\d+):({string.Join('|', _patternParameters)})}}", RegexOptions.IgnoreCase);

        private bool _hasPluralArgument;
        private string _value;

        public LocalizedString(string value)
        {
            _value = value;
            _hasPluralArgument = HasPluralArgument(_value);
        }

        internal event EventHandler Repopulated;

        public bool IsNull => _value == default;

        private void OnRepopulated()
        {
            Repopulated?.Invoke(this, EventArgs.Empty);
            _hasPluralArgument = HasPluralArgument(_value);
        }

        public override string ToString() => _value;

        public string ToString(params object[] args)
        {
            try
            {
                var pluralizedFormat = _value;
                var pluralizedArguments = args.ToList();
                if (_hasPluralArgument && pluralizedArguments.Count > 0)
                {
                    var startAt = 0;
                    Match match;
                    while ((match = PatternCasePlural.Match(pluralizedFormat, startAt)).Success)
                    {
                        var argumentIndexGroup = match.Groups[1];
                        var argumentIndex = int.Parse(argumentIndexGroup.Value);
                        var argumentTypeName = match.Groups[2].Value;

                        startAt = match.Index + 2 + argumentIndexGroup.Length;

                        if (!Enum.TryParse<Pluralization>(argumentTypeName, true, out var pluralization))
                        {
                            if (Enum.TryParse<StringCase>(argumentTypeName, true, out var stringCase))
                            {
                                pluralization = stringCase.Pluralize(1);
                            }
                            else
                            {
                                throw new InvalidOperationException($"Invalid case/pluralization: '{argumentTypeName}'");
                            }
                        }

                        if (pluralizedArguments[argumentIndex] is not LocalizedPluralString localizedPluralString)
                        {
                            continue;
                        }

                        pluralizedArguments[argumentIndex] = pluralization.SelectFrom(localizedPluralString);

                        pluralizedFormat = pluralizedFormat.Replace(match.Value, $"{{{argumentIndexGroup}}}");
                    }
                }
                return pluralizedArguments.Count == 0 ? pluralizedFormat : string.Format(pluralizedFormat, pluralizedArguments.ToArray() ?? Array.Empty<object>());
            }
            catch (FormatException)
            {
                return "Format Exception!";
            }
        }

        public static LocalizedString? PickNonNull(params LocalizedString?[] localizedStrings) =>
            localizedStrings.FirstOrDefault(localizedString => localizedString?._value != default);

        public static bool HasPluralArgument(string @string) => !string.IsNullOrWhiteSpace(@string) && PatternCasePlural.IsMatch(@string);

        public static implicit operator LocalizedString(string value) => new(value);

        public static implicit operator string(LocalizedString str) => str?._value;

        public partial class Converter : JsonConverter<LocalizedString>
        {
            public override void WriteJson(JsonWriter writer, LocalizedString? value, JsonSerializer serializer) =>
                serializer.Serialize(writer, value?.ToString());

            public override LocalizedString ReadJson(
                JsonReader reader,
                Type objectType,
                LocalizedString? existingValue,
                bool hasExistingValue,
                JsonSerializer serializer
            )
            {
                var value = serializer.Deserialize<string?>(reader);

                if (existingValue == default)
                {
                    return new LocalizedString(value);
                }

                existingValue._value = value;
                existingValue.OnRepopulated();
                return existingValue;
            }
        }
    }
}
