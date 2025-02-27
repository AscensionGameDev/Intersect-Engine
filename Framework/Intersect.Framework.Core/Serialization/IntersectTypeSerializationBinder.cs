using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Intersect.Framework.Reflection;
using Newtonsoft.Json.Serialization;

namespace Intersect.Framework.Core.Serialization;

public sealed class IntersectTypeSerializationBinder : ISerializationBinder
{
    private readonly DefaultSerializationBinder _defaultSerializationBinder = new();

    public Type BindToType(string assemblyName, string qualifiedTypeName)
    {
        if (!assemblyName.StartsWith("Intersect"))
        {
            return _defaultSerializationBinder.BindToType(assemblyName, qualifiedTypeName);
        }

        var type = Type.GetType(qualifiedTypeName);
        if (type is not null)
        {
            return type;
        }

        switch (assemblyName)
        {
            case "Intersect Core":
            case "Intersect.Framework.Core":
                break;
            default:
                throw new NotImplementedException($"{nameof(assemblyName)}=\"{assemblyName}\", {nameof(qualifiedTypeName)}=\"{qualifiedTypeName}\"");
        }

        var typeNameSegments = qualifiedTypeName.Split('.');
        var typeName = qualifiedTypeName.Split('.').Last();

        var assembly = Assembly.Load("Intersect.Framework.Core");
        var types = assembly.GetTypes();
        var matchingTypes = types.Where(t => typeName.Equals(t.Name)).ToArray();

        return matchingTypes.Length switch
        {
            1 => matchingTypes.First(),
            < 1 => throw new InvalidOperationException($"No matching types for {qualifiedTypeName}"),
            > 2 => throw new InvalidOperationException(
                $"Multiple matching types for {qualifiedTypeName}:\n{string.Join("\n", matchingTypes.Select(t => $"\t{t.GetName(qualified: true)}"))}"
            ),
            _ => throw new UnreachableException("There should be no uncovered value"),
        };
    }

    public void BindToName(
        Type serializedType,
        [UnscopedRef] out string assemblyName,
        [UnscopedRef] out string typeName
    ) =>  _defaultSerializationBinder.BindToName(serializedType, out assemblyName, out typeName);
}