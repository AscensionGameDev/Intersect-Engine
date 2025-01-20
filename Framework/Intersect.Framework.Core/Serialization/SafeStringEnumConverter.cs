using Intersect.Core;
using Intersect.Framework.Reflection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Intersect.Framework.Core.Serialization;

public sealed class SafeStringEnumConverter : JsonConverter
{
    private readonly StringEnumConverter _baseConverter = new();

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        _baseConverter.WriteJson(writer, value, serializer);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        try
        {
            return _baseConverter.ReadJson(
                reader,
                objectType,
                existingValue,
                serializer
            );
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Error parsing {ObjectType}",
                objectType.GetName(qualified: true)
            );

            return existingValue;
        }
    }

    public override bool CanConvert(Type objectType) => _baseConverter.CanConvert(objectType);
}