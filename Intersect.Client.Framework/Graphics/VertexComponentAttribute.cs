namespace Intersect.Client.Framework.Graphics;

[AttributeUsage(AttributeTargets.Struct, AllowMultiple = true, Inherited = true)]
public sealed class VertexComponentAttribute(
    int offset,
    VertexComponent component,
    VertexComponentFormat format,
    int usageIndex
)
    : Attribute
{
    public VertexComponent Component { get; } = component;
    public VertexComponentFormat Format { get; } = format;
    public int Offset { get; } = offset;
    public int UsageIndex { get; } = usageIndex;
}