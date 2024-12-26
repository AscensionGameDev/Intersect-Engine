using Newtonsoft.Json;

namespace Intersect.Localization;

public partial class LocalizedStringConverter : JsonConverter<LocalizedString>
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