using System;

using Newtonsoft.Json;

namespace Intersect.Localization
{

    public class LocalizedStringConverter : JsonConverter<LocalizedString>
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
            return reader.Value as string;
        }

    }

    [Serializable]
    public class LocalizedString : Localized
    {

        private readonly string mValue;

        public LocalizedString(string value)
        {
            mValue = value;
        }

        public static implicit operator LocalizedString(string value)
        {
            return new LocalizedString(value);
        }

        public static implicit operator string(LocalizedString str)
        {
            return str.mValue;
        }

        public override string ToString()
        {
            return mValue;
        }

        public string ToString(params object[] args)
        {
            try
            {
                return args?.Length == 0 ? mValue : string.Format(mValue, args ?? new object[] { });
            }
            catch (FormatException)
            {
                return "Format Exception!";
            }
        }

    }

}
