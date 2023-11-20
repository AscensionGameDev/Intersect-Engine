using System.Text.Json;
using System.Text.Json.Serialization;

namespace Intersect.Framework.Converters.Json;

/// <summary>
/// Creates <see cref="JsonConverter{T}"/> instances for <see cref="Id{T}"/>.
/// </summary>
public sealed class HandleSystemJsonConverterFactory : GenericSystemJsonConverterFactory
{
    public HandleSystemJsonConverterFactory() : base(
        typeof(HandleJsonConverter<>),
        typeof(Handle<>)
    )
    {
    }

    private class HandleJsonConverter<T> : JsonConverter<Handle<T>>
    {
        public override Handle<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new Handle<T>(new IntPtr(reader.GetInt64()));
        }

        public override void Write(Utf8JsonWriter writer, Handle<T> value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.Pointer.ToInt64());
        }
    }
}