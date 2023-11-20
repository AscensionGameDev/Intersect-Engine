namespace Intersect.Network;

[AttributeUsage(AttributeTargets.Class)]
public sealed class GenericPacketTypeArgumentsAttribute : Attribute
{
    public GenericPacketTypeArgumentsAttribute(params Type[] typeArguments)
    {
        TypeArguments = typeArguments;
    }

    public Type[] TypeArguments { get; }
}