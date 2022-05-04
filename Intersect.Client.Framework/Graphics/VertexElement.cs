using System.Diagnostics.CodeAnalysis;

namespace Intersect.Client.Framework.Graphics;

public struct VertexElement : IEquatable<VertexElement>
{
    public readonly int Offset;

    public readonly int UsageIndex;

    public readonly VertexElementFormat VertexElementFormat;

    public readonly VertexElementUsage VertexElementUsage;

    public VertexElement(int offset, VertexElementFormat vertexElementFormat, VertexElementUsage vertexElementUsage, int usageIndex)
    {
        Offset = offset;
        UsageIndex = usageIndex;
        VertexElementFormat = vertexElementFormat;
        VertexElementUsage = vertexElementUsage;
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is VertexElement vertexElement && Equals(vertexElement);

    public bool Equals(VertexElement vertexElement) =>
        Offset == vertexElement.Offset &&
        UsageIndex == vertexElement.UsageIndex &&
        VertexElementFormat == vertexElement.VertexElementFormat &&
        VertexElementUsage == vertexElement.VertexElementUsage;

    public override int GetHashCode() => HashCode.Combine(Offset, UsageIndex, VertexElementFormat, VertexElementUsage);

    public override string ToString() => $"{{ {nameof(Offset)}={Offset}, {nameof(UsageIndex)}={UsageIndex}, {nameof(VertexElementFormat)}={VertexElementFormat}, {nameof(VertexElementUsage)}={VertexElementUsage} }}";

    public static bool operator ==(VertexElement left, VertexElement right) => left.Equals(right);

    public static bool operator !=(VertexElement left, VertexElement right) => !(left == right);
}
