using System.Text.Json;
using System.Text.Json.Serialization;

namespace Intersect.Framework.Converters.Json;

/// <summary>
/// Creates <see cref="JsonConverter{T}"/> instances for <see cref="Id{T}"/>.
/// </summary>
public class IdJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Id<>);

    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var identifiedType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(IdJsonConverter<>).MakeGenericType(identifiedType);
        var jsonConverter = Activator.CreateInstance(converterType) as JsonConverter;
        return jsonConverter;
    }

    private class IdJsonConverter<T> : JsonConverter<Id<T>>
    {
        public override Id<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new Id<T>(reader.GetGuid());
        }

        public override void Write(Utf8JsonWriter writer, Id<T> value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Guid);
        }
    }
}
