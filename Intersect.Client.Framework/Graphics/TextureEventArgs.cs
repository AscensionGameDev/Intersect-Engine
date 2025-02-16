namespace Intersect.Client.Framework.Graphics;

public class TextureEventArgs(IGameTexture gameTexture) : EventArgs
{
    public IGameTexture GameTexture { get; } = gameTexture;
}