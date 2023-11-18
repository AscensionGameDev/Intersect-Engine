using Intersect.Framework.Reflection;
using Newtonsoft.Json;

namespace Intersect.Framework.Converters.Json;

public abstract class GenericFactoryNewtonsoftJsonConverter : JsonConverter
{
    private static readonly Dictionary<Type, JsonConverter> _converters = new Dictionary<Type, JsonConverter>();

    private readonly Type _converterTypeDefinition;
    private readonly Type _serializedGenericTypeDefinition;

    protected GenericFactoryNewtonsoftJsonConverter(Type converterTypeDefinition, Type serializedGenericTypeDefinition)
    {
        _converterTypeDefinition = converterTypeDefinition ??
                                   throw new ArgumentNullException(nameof(converterTypeDefinition));
        _serializedGenericTypeDefinition = serializedGenericTypeDefinition ??
                                           throw new ArgumentNullException(nameof(serializedGenericTypeDefinition));
    }

    public override bool CanConvert(Type objectType) =>
        objectType.IsGenericType && objectType.GetGenericTypeDefinition() == _serializedGenericTypeDefinition;

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
        JsonSerializer serializer)
    {
        if (reader.Value == default)
        {
            return reader.Value;
        }

        var actualConverter = CreateActualConverter(
            _converterTypeDefinition,
            _serializedGenericTypeDefinition,
            objectType
        );
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
            var actualConverter = CreateActualConverter(
                _converterTypeDefinition,
                _serializedGenericTypeDefinition,
                value.GetType()
            );
            actualConverter.WriteJson(writer, value, serializer);
        }
    }

    private static JsonConverter CreateActualConverter(
        Type converterTypeDefinition,
        Type serializedGenericTypeDefinition,
        Type objectType
    )
    {
        if (_converters.TryGetValue(objectType, out JsonConverter? jsonConverter))
        {
            return jsonConverter;
        }

        var targetType = objectType
                             .FindGenericTypeParameters(typeof(Nullable<>), false)
                             .FirstOrDefault()
                         ?? objectType;

        var identifiedType = targetType.FindGenericTypeParameters(serializedGenericTypeDefinition);
        var converterType = converterTypeDefinition.MakeGenericType(identifiedType);
        jsonConverter = Activator.CreateInstance(converterType) as JsonConverter;
        _converters[objectType] = jsonConverter ?? throw new InvalidOperationException();
        return jsonConverter;
    }
}
