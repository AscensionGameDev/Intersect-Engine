namespace Intersect.Client.Framework.Graphics;

public enum VertexComponent
{
    Position = 0x1 << 1,
    Color = 0x1 << 2,
    TextureUV = 0x1 << 3,
    Normal = 0x1 << 4,
    Binormal = 0x1 << 5,
    Tangent = 0x1 << 6,
    BlendIndices = 0x1 << 7,
    BlendWeight = 0x1 << 8,
    Depth = 0x1 << 9,
    Fog = 0x1 << 10,
    PointSize = 0x1 << 11,
}