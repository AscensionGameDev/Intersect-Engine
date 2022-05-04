using System.Numerics;

using Intersect.Client.Framework.Graphics;

namespace Intersect.Editor.MonoGame.Graphics;

internal sealed class MonoGameEffect : Effect
{
    private readonly Microsoft.Xna.Framework.Graphics.BasicEffect _effect;

    public MonoGameEffect(Microsoft.Xna.Framework.Graphics.BasicEffect effect) : base(default, default)
    {
        _effect = effect;
    }

    public override Matrix4x4 Projection
    {
        get => new(
            _effect.Projection.M11, _effect.Projection.M12, _effect.Projection.M13, _effect.Projection.M14,
            _effect.Projection.M21, _effect.Projection.M22, _effect.Projection.M23, _effect.Projection.M24,
            _effect.Projection.M31, _effect.Projection.M32, _effect.Projection.M33, _effect.Projection.M34,
            _effect.Projection.M41, _effect.Projection.M42, _effect.Projection.M43, _effect.Projection.M44
        );

        set => _effect.Projection = new(
            value.M11, value.M12, value.M13, value.M14,
            value.M21, value.M22, value.M23, value.M24,
            value.M31, value.M32, value.M33, value.M34,
            value.M41, value.M42, value.M43, value.M44
        );
    }

    public override Texture Texture
    {
        get => new MonoGameTexture(_effect.Texture);
        set => _effect.Texture = (value as MonoGameTexture)?._texture;
    }

    public override bool TextureEnabled
    {
        get => _effect.TextureEnabled;
        set => _effect.TextureEnabled = value;
    }

    public override bool VertexColorEnabled
    {
        get => _effect.VertexColorEnabled;
        set => _effect.VertexColorEnabled = value;
    }

    public override Matrix4x4 View
    {
        get => new(
            _effect.View.M11, _effect.View.M12, _effect.View.M13, _effect.View.M14,
            _effect.View.M21, _effect.View.M22, _effect.View.M23, _effect.View.M24,
            _effect.View.M31, _effect.View.M32, _effect.View.M33, _effect.View.M34,
            _effect.View.M41, _effect.View.M42, _effect.View.M43, _effect.View.M44
        );

        set => _effect.View = new(
            value.M11, value.M12, value.M13, value.M14,
            value.M21, value.M22, value.M23, value.M24,
            value.M31, value.M32, value.M33, value.M34,
            value.M41, value.M42, value.M43, value.M44
        );
    }

    public override Matrix4x4 World
    {
        get => new(
            _effect.World.M11, _effect.World.M12, _effect.World.M13, _effect.World.M14,
            _effect.World.M21, _effect.World.M22, _effect.World.M23, _effect.World.M24,
            _effect.World.M31, _effect.World.M32, _effect.World.M33, _effect.World.M34,
            _effect.World.M41, _effect.World.M42, _effect.World.M43, _effect.World.M44
        );

        set => _effect.World = new(
            value.M11, value.M12, value.M13, value.M14,
            value.M21, value.M22, value.M23, value.M24,
            value.M31, value.M32, value.M33, value.M34,
            value.M41, value.M42, value.M43, value.M44
        );
    }

    public override void OnEachPass(Action action)
    {
        foreach (var pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();

            action();
        }
    }
}
