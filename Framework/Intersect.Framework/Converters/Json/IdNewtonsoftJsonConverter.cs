using Newtonsoft.Json;

namespace Intersect.Framework.Converters.Json;

public sealed class IdNewtonsoftJsonConverter : GenericFactoryNewtonsoftJsonConverter
{
    public IdNewtonsoftJsonConverter() : base(
        typeof(IdNewtonsoftJsonConverter<>),
        typeof(Id<>)
    )
    {
    }
}

public class IdNewtonsoftJsonConverter<TId> : JsonConverter<Id<TId>>
{
    public override Id<TId> ReadJson(JsonReader reader, Type objectType, Id<TId> existingValue, bool hasExistingValue,
        JsonSerializer serializer) =>
        new Id<TId>(serializer.Deserialize<Guid>(reader));

    public override void WriteJson(JsonWriter writer, Id<TId> value, JsonSerializer serializer) =>
        serializer.Serialize(writer, value.Guid);
}