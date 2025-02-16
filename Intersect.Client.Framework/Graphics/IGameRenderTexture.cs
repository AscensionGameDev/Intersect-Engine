namespace Intersect.Client.Framework.Graphics;

public interface IGameRenderTexture : IGameTexture
{
    bool Begin();

    void Clear(Color black);

    void End();
}