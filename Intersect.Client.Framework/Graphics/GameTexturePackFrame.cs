using System;

using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Graphics
{
    public class GameTexturePackFrame : ITexturePackFrame
    {
        public GameTexturePackFrame(
            string name,
            Rectangle bounds,
            bool rotated,
            Rectangle sourceSpriteBounds,
            ITexture packedTexture
        )
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name.Replace('\\', '/');
            Bounds = bounds;
            IsRotated = rotated;
            SourceBounds = sourceSpriteBounds;
            PackedTexture = packedTexture;
        }

        public string Name { get; }

        public Rectangle Bounds { get; }

        public bool IsRotated { get; }

        public Rectangle SourceBounds { get; }

        public ITexture PackedTexture { get; }
    }
}
