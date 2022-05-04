using System.Diagnostics.CodeAnalysis;

namespace Intersect.Client.Framework.Graphics;

public struct VertexDeclaration : IEquatable<VertexDeclaration>
{
    public readonly VertexElement[] Elements;

    public readonly int Stride;

    public VertexDeclaration(int stride, VertexElement[] vertexElements)
    {
        Stride = stride;
        Elements = vertexElements;
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is VertexDeclaration vertexDeclaration && Equals(vertexDeclaration);

    public bool Equals(VertexDeclaration vertexDeclaration) => Stride == vertexDeclaration.Stride && Elements.Length == vertexDeclaration.Elements.Length && Elements.SequenceEqual(vertexDeclaration.Elements);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        hashCode.Add(Stride);

        foreach (var element in Elements)
        {
            hashCode.Add(element);
        }

        return hashCode.ToHashCode();
    }

    public override string ToString() => $"{{ {nameof(Stride)}={Stride}, {nameof(Elements)}={Elements} }}";

    public static bool operator ==(VertexDeclaration left, VertexDeclaration right) => left.Equals(right);

    public static bool operator !=(VertexDeclaration left, VertexDeclaration right) => !(left == right);
}
