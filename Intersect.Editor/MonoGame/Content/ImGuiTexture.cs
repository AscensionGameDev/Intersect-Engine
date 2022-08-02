using System.Numerics;

using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.UserInterface;

namespace Intersect.Editor.MonoGame.Content;

internal class ImGuiTexture : IAsset
{
    private readonly IntPtr _handle;
    private readonly Texture _texture;

    public ImGuiTexture(string name, Texture texture, ImGuiRenderer imGuiRenderer)
    {
        _handle = imGuiRenderer.BindTexture(texture);
        _texture = texture;

        Name = name;
    }

    public int Height => _texture.Height;

    public bool IsMipMapEnabled => _texture.IsMipMapEnabled;

    public string Name { get; }

    public Vector2 Size => new(Width, Height);

    public int Width => _texture.Width;

    public static implicit operator IntPtr(ImGuiTexture imGuiTexture) => imGuiTexture._handle;
}
