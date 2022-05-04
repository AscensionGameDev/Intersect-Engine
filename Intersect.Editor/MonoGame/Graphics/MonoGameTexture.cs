using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Editor.MonoGame.Graphics;

using Texture = Client.Framework.Graphics.Texture;

internal sealed class MonoGameTexture : Texture
{
    internal readonly Texture2D _texture;

    internal MonoGameTexture(Texture2D texture) : base(default, default, default, default, default)
    {
        _texture = texture;
    }

    public override void SetData<T>(T[] data) => _texture.SetData(data);
}
