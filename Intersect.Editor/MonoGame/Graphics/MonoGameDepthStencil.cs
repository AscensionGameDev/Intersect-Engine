using Intersect.Client.Framework.Graphics;

using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Editor.MonoGame.Graphics;

internal sealed class MonoGameDepthStencil : DepthStencil
{
    private readonly DepthStencilState _depthStencilState;

    internal MonoGameDepthStencil(DepthStencilState depthStencilState) : base(default, default)
    {
        _depthStencilState = depthStencilState;
    }

    public override bool DepthBufferEnable
    {
        get => _depthStencilState.DepthBufferEnable;
        set => _depthStencilState.DepthBufferEnable = value;
    }

    public override bool DepthBufferWriteEnable
    {
        get => _depthStencilState.DepthBufferWriteEnable;
        set => _depthStencilState.DepthBufferWriteEnable = value;
    }
}
