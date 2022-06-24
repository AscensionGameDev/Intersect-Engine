using System;

using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Logging;

namespace Intersect.Client.Framework.Graphics
{

    public abstract partial class GameTexture : IAsset
    {

        public string Name => GetName() ?? throw new ArgumentNullException(nameof(GetName));

        public int Width => GetWidth();

        public int Height => GetHeight();

        public int Area => Width * Height;

        public Pointf Dimensions => new Pointf(Width, Height);

        public Pointf Center => Dimensions / 2;

        public object PlatformTextureObject => GetTexture();

        public GameTexturePackFrame TexturePackFrame => GetTexturePackFrame();

        public abstract string GetName();

        public abstract int GetWidth();

        public abstract int GetHeight();

        public abstract object GetTexture();

        public abstract Color GetPixel(int x1, int y1);

        public abstract GameTexturePackFrame GetTexturePackFrame();

        public static string ToString(GameTexture tex)
        {
            return tex?.GetName() ?? "";
        }

        public static GameTexture GetBoundingTexture(BoundsComparison boundsComparison, params GameTexture[] textures)
        {
            GameTexture boundingTexture = default;

            foreach (var texture in textures)
            {
                if (texture == default)
                {
                    continue;
                }
                else if (boundingTexture == default)
                {
                    boundingTexture = texture;
                }
                else
                {
                    var select = false;
                    switch (boundsComparison)
                    {
                        case BoundsComparison.Width:
                            select = texture.Width > boundingTexture.Width;
                            break;

                        case BoundsComparison.Height:
                            select = texture.Height > boundingTexture.Height;
                            break;

                        case BoundsComparison.Dimensions:
                            select = texture.Width >= boundingTexture.Width && texture.Height >= boundingTexture.Height;
                            break;

                        case BoundsComparison.Area:
                            select = texture.Area > boundingTexture.Area;
                            break;

                        default:
                            Log.Error(new ArgumentOutOfRangeException(nameof(boundsComparison), boundsComparison.ToString()));
                            break;
                    }

                    if (select)
                    {
                        boundingTexture = texture;
                    }
                }
            }

            return boundingTexture;
        }
    }
}
