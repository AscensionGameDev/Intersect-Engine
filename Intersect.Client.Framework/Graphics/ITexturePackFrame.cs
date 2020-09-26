using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Graphics
{
    public interface ITexturePackFrame : IAsset
    {
        Rectangle Bounds { get; }

        bool IsRotated { get; }

        Rectangle SourceBounds { get; }

        ITexture PackedTexture { get; }
    }
}
