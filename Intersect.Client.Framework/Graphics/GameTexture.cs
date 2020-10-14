using System;

using Intersect.Client.Framework.Content;

namespace Intersect.Client.Framework.Graphics
{

    public abstract class GameTexture : IAsset
    {

        public string Name => GetName() ?? throw new ArgumentNullException(nameof(GetName));

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

    }

}
