using Newtonsoft.Json.Serialization;

namespace Intersect.Framework.Core.Serialization;

public interface IRestrictedContractResolver : IContractResolver
{
    bool SupportsType(Type type);
}