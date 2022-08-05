using Newtonsoft.Json;

namespace Intersect.Localization
{
    [Serializable]
    public partial class LocalizedString : Localized
    {
        private string _value;

        public LocalizedString(string value) => _value = value;

        internal event EventHandler Repopulated;

        public bool IsNull => _value == default;

        private void OnRepopulated() => Repopulated?.Invoke(this, EventArgs.Empty);

        public override string ToString() => _value;

        public string ToString(params object[] args)
        {
            try
            {
                return args?.Length == 0 ? _value : string.Format(_value, args ?? new object[] { });
            }
            catch (FormatException)
            {
                return "Format Exception!";
            }
        }

        public static LocalizedString? PickNonNull(params LocalizedString?[] localizedStrings) =>
            localizedStrings.FirstOrDefault(localizedString => localizedString?._value != default);

        public static implicit operator LocalizedString(string value) => new(value);

        public static implicit operator string(LocalizedString str) => str?._value;

        public partial class Converter : JsonConverter<LocalizedString>
        {

            public override void WriteJson(JsonWriter writer, LocalizedString value, JsonSerializer serializer)
            {
                writer.WriteValue(value?.ToString());
            }

            public override LocalizedString ReadJson(
                JsonReader reader,
                Type objectType,
                LocalizedString existingValue,
                bool hasExistingValue,
                JsonSerializer serializer
            )
            {
                if (existingValue == default)
                {
                    return new LocalizedString(reader.Value as string);
                }

                existingValue._value = reader.Value as string;
                existingValue.OnRepopulated();
                return existingValue;
            }
        }
    }
}
