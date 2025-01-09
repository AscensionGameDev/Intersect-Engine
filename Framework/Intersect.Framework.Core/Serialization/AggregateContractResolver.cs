using Newtonsoft.Json.Serialization;

namespace Intersect.Framework.Core.Serialization;

public sealed class AggregateContractResolver(
    IContractResolver fallbackContractResolver,
    params IRestrictedContractResolver[] contractResolvers
)
    : IContractResolver
{
    private readonly IContractResolver _fallbackContractResolver = fallbackContractResolver;
    private readonly IRestrictedContractResolver[] _contractResolvers = contractResolvers;

    public JsonContract ResolveContract(Type type)
    {
        foreach (var contractResolver in _contractResolvers)
        {
            if (!contractResolver.SupportsType(type))
            {
                continue;
            }

            return contractResolver.ResolveContract(type);
        }

        return _fallbackContractResolver.ResolveContract(type);
    }
}