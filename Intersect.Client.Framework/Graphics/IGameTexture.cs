using System.Numerics;
using System.Runtime.CompilerServices;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Graphics;

public interface IGameTexture : IAsset, IComparable<IGameTexture>, IDisposable
{
    Color this[int x, int y] { get; }
    Color this[Point point] { get; }
    long AccessTime { get; }
    bool IsMissingOrCorrupt { get; }
    bool IsPinned { get; }
    int Area { get; }
    Vector2 Dimensions { get; }
    FloatRect Bounds { get; }
    Vector2 Center { get; }

    AtlasReference? AtlasReference
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    int Width { get; }
    int Height { get; }
    bool Unload();
    object? GetTexture();
    TPlatformTexture? GetTexture<TPlatformTexture>() where TPlatformTexture : class;
    void Reload();
    Color GetPixel(int x, int y);
    string ToString();
}