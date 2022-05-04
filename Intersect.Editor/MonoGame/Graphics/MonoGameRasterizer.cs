using Intersect.Client.Framework.Graphics;

using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Editor.MonoGame.Graphics;

internal sealed class MonoGameRasterizer : Rasterizer
{
    private readonly RasterizerState _rasterizerState;

    public MonoGameRasterizer(RasterizerState rasterizerState) : base(default)
    {
        _rasterizerState = rasterizerState;
    }

    public override Client.Framework.Graphics.CullMode CullMode
    {
        get => _rasterizerState.CullMode.FromMonoGame();
        set => _rasterizerState.CullMode = value.ToMonoGame();
    }

    public override float DepthBias
    {
        get => _rasterizerState.DepthBias;
        set => _rasterizerState.DepthBias = value;
    }

    public override Client.Framework.Graphics.FillMode FillMode
    {
        get => _rasterizerState.FillMode.FromMonoGame();
        set => _rasterizerState.FillMode = value.ToMonoGame();
    }

    public override bool MultiSampleAntiAlias
    {
        get => _rasterizerState.MultiSampleAntiAlias;
        set => _rasterizerState.MultiSampleAntiAlias = value;
    }

    public override bool ScissorTestEnable
    {
        get => _rasterizerState.ScissorTestEnable;
        set => _rasterizerState.ScissorTestEnable = value;
    }

    public override float SlopeScaleDepthBias
    {
        get => _rasterizerState.SlopeScaleDepthBias;
        set => _rasterizerState.SlopeScaleDepthBias = value;
    }
}
