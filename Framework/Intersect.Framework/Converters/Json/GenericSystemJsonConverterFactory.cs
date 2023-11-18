using System.Text.Json;
using System.Text.Json.Serialization;

namespace Intersect.Framework.Converters.Json;

public abstract class GenericSystemJsonConverterFactory : JsonConverterFactory
{
    private readonly Type _converterTypeDefinition;
    private readonly Type _serializedGenericTypeDefinition;

    protected GenericSystemJsonConverterFactory(Type converterTypeDefinition, Type serializedGenericTypeDefinition)
    {
        _converterTypeDefinition = converterTypeDefinition ??
                                   throw new ArgumentNullException(nameof(converterTypeDefinition));
        _serializedGenericTypeDefinition = serializedGenericTypeDefinition ??
                                           throw new ArgumentNullException(nameof(serializedGenericTypeDefinition));
    }

    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == _serializedGenericTypeDefinition;

    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var identifiedType = typeToConvert.GetGenericArguments()[0];
        var converterType = _converterTypeDefinition.MakeGenericType(identifiedType);
        var jsonConverter = Activator.CreateInstance(converterType) as JsonConverter;
        return jsonConverter;
    }
}