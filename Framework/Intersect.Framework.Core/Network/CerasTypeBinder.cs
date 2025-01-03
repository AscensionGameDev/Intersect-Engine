using Ceras;

namespace Intersect.Network;

partial class CerasTypeBinder : ITypeBinder
{
    Dictionary<string, Type> _nameToType = new Dictionary<string, Type>();

    public CerasTypeBinder(Dictionary<string, Type> nameTypeMap)
    {
        _nameToType = nameTypeMap;
    }

    public string GetBaseName(Type type)
    {
        return _nameToType.Keys.FirstOrDefault(k => _nameToType[k] == type);
    }

    public Type GetTypeFromBase(string baseTypeName)
    {
        return _nameToType[baseTypeName];
    }

    public Type GetTypeFromBaseAndArguments(string baseTypeName, params Type[] genericTypeArguments)
    {
        throw new NotSupportedException("this binder is only for debugging");
    }
}