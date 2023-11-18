using System.Text.Json;
using System.Text.Json.Serialization;

namespace Intersect.Framework.Converters.Json;

/// <summary>
/// Creates <see cref="JsonConverter{T}"/> instances for <see cref="Id{T}"/>.
/// </summary>
public sealed class IdSystemJsonConverterFactory : GenericSystemJsonConverterFactory
{
    public IdSystemJsonConverterFactory() : base(
        typeof(IdJsonConverter<>),
        typeof(Id<>)
    )
    {
    }

    private class IdJsonConverter<T> : JsonConverter<Id<T>>
    {
        public override Id<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            new Id<T>(reader.GetGuid());

        public override void Write(Utf8JsonWriter writer, Id<T> value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.Guid);
    }
}