using Newtonsoft.Json;

namespace Intersect.Framework.Converters.Json;

public sealed class HandleNewtonsoftJsonConverter : GenericFactoryNewtonsoftJsonConverter
{
    public HandleNewtonsoftJsonConverter() : base(typeof(HandleNewtonsoftJsonConverter<>), typeof(Handle<>))
    {
    }
}

public class HandleNewtonsoftJsonConverter<THandle> : JsonConverter<Handle<THandle>>
{
    public override Handle<THandle> ReadJson(JsonReader reader, Type objectType, Handle<THandle> existingValue,
        bool hasExistingValue, JsonSerializer serializer) =>
        new Handle<THandle>(new IntPtr(serializer.Deserialize<long>(reader)));

    public override void WriteJson(JsonWriter writer, Handle<THandle> value, JsonSerializer serializer) =>
        serializer.Serialize(writer, value.Pointer.ToInt64());
}