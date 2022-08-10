using Intersect.Framework.Reflection;

using Newtonsoft.Json;

namespace Intersect.Framework.Converters.Json;

public class IdNewtonsoftJsonConverter : JsonConverter
{
    private static readonly Dictionary<Type, JsonConverter> _converters = new();

    public override bool CanConvert(Type objectType) =>
        objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Id<>);

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.Value == default)
        {
            return reader.Value;
        }

        var actualConverter = CreateActualConverter(objectType);
        return actualConverter.ReadJson(reader, objectType, existingValue, serializer);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == default)
        {
            writer.WriteNull();
        }
        else
        {
            var actualConverter = CreateActualConverter(value.GetType());
            actualConverter.WriteJson(writer, value, serializer);
        }
    }

    private static JsonConverter CreateActualConverter(Type objectType)
    {
        if (_converters.TryGetValue(objectType, out JsonConverter? jsonConverter))
        {
            return jsonConverter;
        }

        var targetType = objectType
            .FindGenericTypeParameters(typeof(Nullable<>), false)
            .FirstOrDefault()
            ?? objectType;

        var identifiedType = targetType.FindGenericTypeParameters(typeof(Id<>));
        var converterType = typeof(IdNewtonsoftJsonConverter<>).MakeGenericType(identifiedType);
        jsonConverter = Activator.CreateInstance(converterType) as JsonConverter;
        _converters[objectType] = jsonConverter ?? throw new InvalidOperationException();
        return jsonConverter;
    }
}

public class IdNewtonsoftJsonConverter<TId> : JsonConverter<Id<TId>>
{
    public override Id<TId> ReadJson(JsonReader reader, Type objectType, Id<TId> existingValue, bool hasExistingValue, JsonSerializer serializer) =>
        new(serializer.Deserialize<Guid>(reader));

    public override void WriteJson(JsonWriter writer, Id<TId> value, JsonSerializer serializer) =>
        serializer.Serialize(writer, value.Guid);
}
