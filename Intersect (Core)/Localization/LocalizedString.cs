using System;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Intersect.Localization
{

    public class LocalizedStringConverter : JsonConverter<LocalizedString>
    {

        public override void WriteJson([NotNull] JsonWriter writer, LocalizedString value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString());
        }

        public override LocalizedString ReadJson(
            [NotNull] JsonReader reader,
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

        [NotNull] private readonly string mValue;

        public LocalizedString([NotNull] string value)
        {
            mValue = value;
        }

        public static implicit operator LocalizedString([NotNull] string value)
        {
            return new LocalizedString(value);
        }

        public static implicit operator string([NotNull] LocalizedString str)
        {
            return str.mValue;
        }

        public override string ToString()
        {
            return mValue;
        }

        [NotNull]
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
