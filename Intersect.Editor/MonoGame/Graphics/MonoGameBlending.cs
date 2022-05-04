using Intersect.Client.Framework.Graphics;

using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Editor.MonoGame.Graphics;

internal sealed class MonoGameBlending : Blending
{
    private readonly BlendState _blendState;

    internal MonoGameBlending(BlendState blendState, string? name = default) : base(default, name)
    {
        _blendState = blendState ?? throw new ArgumentNullException(nameof(blendState));
    }

    public override BlendMode DestinationBlendMode
    {
        get => _blendState.ColorDestinationBlend.FromMonoGame();
        set
        {
            _blendState.ColorDestinationBlend = value.ToMonoGame();
            _blendState.AlphaDestinationBlend = value.ToMonoGame();
        }
    }

    public override BlendMode SourceBlendMode
    {
        get => _blendState.ColorSourceBlend.FromMonoGame();
        set
        {
            _blendState.ColorSourceBlend = value.ToMonoGame();
            _blendState.AlphaSourceBlend = value.ToMonoGame();
        }
    }

    internal static void InitializePresets()
    {
        NonPremultiplied = new MonoGameBlending(new())
        {
            DestinationBlendMode = BlendMode.InverseSourceAlpha,
            SourceBlendMode = BlendMode.SourceAlpha
        };
    }
}
