namespace Intersect.Client.Framework.Graphics;

public abstract partial class
    GameRenderTexture<TPlatformRenderTexture, TPlatformRenderer> :
    GameTexture<TPlatformRenderTexture, TPlatformRenderer>,
    IGameRenderTexture where TPlatformRenderTexture : class, IDisposable where TPlatformRenderer : GameRenderer
{
    protected GameRenderTexture(TPlatformRenderer renderer, TPlatformRenderTexture platformRenderTexture) : base(
        renderer,
        name: null,
        platformTexture: platformRenderTexture
    )
    {
    }

    /// <summary>
    ///     Called before a frame is drawn, if the renderer must re-created or anything it does it here.
    /// </summary>
    /// <returns></returns>
    public abstract bool Begin();

    /// <summary>
    ///     Called when the frame is done being drawn, generally used to finally display the content to the screen.
    /// </summary>
    public abstract void End();

    private protected override void InternalReload()
    {
        // Render targets should not be reloaded
    }

    /// <summary>
    ///     Clears everything off the render target with a specified color.
    /// </summary>
    public abstract void Clear(Color color);
}