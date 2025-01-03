using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Serialization;

namespace Intersect.Framework.Core.Serialization;

public sealed class IntersectTypeSerializationBinder : ISerializationBinder
{
    private readonly DefaultSerializationBinder _defaultSerializationBinder = new();

    public Type BindToType(string assemblyName, string typeName)
    {
        return assemblyName.StartsWith("Intersect")
            ? Type.GetType(typeName)
            : _defaultSerializationBinder.BindToType(assemblyName, typeName);
    }

    public void BindToName(
        Type serializedType,
        [UnscopedRef] out string assemblyName,
        [UnscopedRef] out string typeName
    ) =>  _defaultSerializationBinder.BindToName(serializedType, out assemblyName, out typeName);
}