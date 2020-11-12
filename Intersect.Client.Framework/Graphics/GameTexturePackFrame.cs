using System;

using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Graphics
{
    public class GameTexturePackFrame : ITexturePackFrame
    {
        public GameTexturePackFrame(
            AssetReference assetReference,
            Rectangle bounds,
            bool rotated,
            Rectangle sourceSpriteBounds,
            ITexture packedTexture
        )
        {
            Reference = assetReference;
            Bounds = bounds;
            IsRotated = rotated;
            SourceBounds = sourceSpriteBounds;
            PackedTexture = packedTexture;
        }

        public string Name => Reference.Name;

        public AssetReference Reference { get; }

        public Rectangle Bounds { get; }

        public bool IsRotated { get; }

        public Rectangle SourceBounds { get; }

        public ITexture PackedTexture { get; }
    }
}
