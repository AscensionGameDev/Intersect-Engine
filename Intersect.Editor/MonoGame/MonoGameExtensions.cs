using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Input;
using Intersect.Client.Framework.Windows;
using Intersect.Extensions;
using Intersect.Time;

using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Editor.MonoGame;

using VertexDeclaration = Client.Framework.Graphics.VertexDeclaration;
using VertexElement = Client.Framework.Graphics.VertexElement;
using VertexElementFormat = Client.Framework.Graphics.VertexElementFormat;
using VertexElementUsage = Client.Framework.Graphics.VertexElementUsage;

internal static class MonoGameExtensions
{
    public static BlendMode FromMonoGame(this Blend blend)
    {
        return blend switch
        {
            Blend.One => BlendMode.One,
            Blend.Zero => BlendMode.Zero,
            Blend.SourceColor => BlendMode.SourceColor,
            Blend.InverseSourceColor => BlendMode.InverseSourceColor,
            Blend.SourceAlpha => BlendMode.SourceAlpha,
            Blend.InverseSourceAlpha => BlendMode.InverseSourceAlpha,
            Blend.DestinationColor => BlendMode.DestinationColor,
            Blend.InverseDestinationColor => BlendMode.InverseDestinationColor,
            Blend.DestinationAlpha => BlendMode.DestinationAlpha,
            Blend.InverseDestinationAlpha => BlendMode.InverseDestinationAlpha,
            Blend.BlendFactor => BlendMode.BlendFactor,
            Blend.InverseBlendFactor => BlendMode.BlendFactor,
            Blend.SourceAlphaSaturation => BlendMode.SourceAlphaSaturation,
            _ => throw new ArgumentOutOfRangeException(nameof(blend)),
        };
    }

    public static Client.Framework.Graphics.BufferUsage FromMonoGame(this Microsoft.Xna.Framework.Graphics.BufferUsage bufferUsage)
    {
        return bufferUsage switch
        {
            Microsoft.Xna.Framework.Graphics.BufferUsage.None => Client.Framework.Graphics.BufferUsage.None,
            Microsoft.Xna.Framework.Graphics.BufferUsage.WriteOnly => Client.Framework.Graphics.BufferUsage.WriteOnly,
            _ => throw new ArgumentOutOfRangeException(nameof(bufferUsage)),
        };
    }

    public static ButtonState FromMonoGame(this Microsoft.Xna.Framework.Input.ButtonState buttonState)
    {
        return buttonState switch
        {
            Microsoft.Xna.Framework.Input.ButtonState.Released => ButtonState.Released,
            Microsoft.Xna.Framework.Input.ButtonState.Pressed => ButtonState.Pressed,
            _ => throw new ArgumentOutOfRangeException(nameof(buttonState)),
        };
    }

    public static Client.Framework.Graphics.CullMode FromMonoGame(this Microsoft.Xna.Framework.Graphics.CullMode cullMode)
    {
        return cullMode switch
        {
            Microsoft.Xna.Framework.Graphics.CullMode.None => Client.Framework.Graphics.CullMode.None,
            Microsoft.Xna.Framework.Graphics.CullMode.CullClockwiseFace => Client.Framework.Graphics.CullMode.CullClockwiseFace,
            Microsoft.Xna.Framework.Graphics.CullMode.CullCounterClockwiseFace => Client.Framework.Graphics.CullMode.CullCounterClockwiseFace,
            _ => throw new ArgumentOutOfRangeException(nameof(cullMode)),
        };
    }

    public static Client.Framework.Graphics.FillMode FromMonoGame(this Microsoft.Xna.Framework.Graphics.FillMode fillMode)
    {
        return fillMode switch
        {
            Microsoft.Xna.Framework.Graphics.FillMode.Solid => Client.Framework.Graphics.FillMode.Solid,
            Microsoft.Xna.Framework.Graphics.FillMode.WireFrame => Client.Framework.Graphics.FillMode.WireFrame,
            _ => throw new ArgumentOutOfRangeException(nameof(fillMode)),
        };
    }

    public static FrameTime FromMonoGame(this Microsoft.Xna.Framework.GameTime gameTime)
    {
        return new(gameTime.ElapsedGameTime, gameTime.TotalGameTime);
    }

    public static Client.Framework.Graphics.IndexElementSize FromMonoGame(this Microsoft.Xna.Framework.Graphics.IndexElementSize indexElementSize)
    {
        return indexElementSize switch
        {
            Microsoft.Xna.Framework.Graphics.IndexElementSize.SixteenBits => Client.Framework.Graphics.IndexElementSize.SixteenBits,
            Microsoft.Xna.Framework.Graphics.IndexElementSize.ThirtyTwoBits => Client.Framework.Graphics.IndexElementSize.ThirtyTwoBits,
            _ => throw new ArgumentOutOfRangeException(nameof(indexElementSize)),
        };
    }

    public static Key FromMonoGame(this Microsoft.Xna.Framework.Input.Keys keys)
    {
        var key = (Key)(int)keys;
        return key.IsDefined() ? key : default;
    }

    public static Client.Framework.Graphics.PrimitiveType FromMonoGame(this Microsoft.Xna.Framework.Graphics.PrimitiveType primitiveType)
    {
        return primitiveType switch
        {
            Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList => Client.Framework.Graphics.PrimitiveType.TriangleList,
            Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip => Client.Framework.Graphics.PrimitiveType.TriangleStrip,
            Microsoft.Xna.Framework.Graphics.PrimitiveType.LineList => Client.Framework.Graphics.PrimitiveType.LineList,
            Microsoft.Xna.Framework.Graphics.PrimitiveType.LineStrip => Client.Framework.Graphics.PrimitiveType.LineStrip,
            _ => throw new ArgumentOutOfRangeException(nameof(primitiveType)),
        };
    }

    public static TextInputEventArgs FromMonoGame(this Microsoft.Xna.Framework.TextInputEventArgs textInputEventArgs)
    {
        return new(textInputEventArgs.Character, textInputEventArgs.Key.FromMonoGame());
    }

    public static Blend ToMonoGame(this BlendMode blendMode)
    {
        return blendMode switch
        {
            BlendMode.One => Blend.One,
            BlendMode.Zero => Blend.Zero,
            BlendMode.SourceColor => Blend.SourceColor,
            BlendMode.InverseSourceColor => Blend.InverseSourceColor,
            BlendMode.SourceAlpha => Blend.SourceAlpha,
            BlendMode.InverseSourceAlpha => Blend.InverseSourceAlpha,
            BlendMode.DestinationColor => Blend.DestinationColor,
            BlendMode.InverseDestinationColor => Blend.InverseDestinationColor,
            BlendMode.DestinationAlpha => Blend.DestinationAlpha,
            BlendMode.InverseDestinationAlpha => Blend.InverseDestinationAlpha,
            BlendMode.BlendFactor => Blend.BlendFactor,
            BlendMode.InverseBlendFactor => Blend.BlendFactor,
            BlendMode.SourceAlphaSaturation => Blend.SourceAlphaSaturation,
            _ => throw new ArgumentOutOfRangeException(nameof(blendMode)),
        };
    }

    public static Microsoft.Xna.Framework.Graphics.BufferUsage ToMonoGame(this Client.Framework.Graphics.BufferUsage bufferUsage)
    {
        return bufferUsage switch
        {
            Client.Framework.Graphics.BufferUsage.None => Microsoft.Xna.Framework.Graphics.BufferUsage.None,
            Client.Framework.Graphics.BufferUsage.WriteOnly => Microsoft.Xna.Framework.Graphics.BufferUsage.WriteOnly,
            _ => throw new ArgumentOutOfRangeException(nameof(bufferUsage)),
        };
    }

    public static Microsoft.Xna.Framework.Graphics.CullMode ToMonoGame(this Client.Framework.Graphics.CullMode cullMode)
    {
        return cullMode switch
        {
            Client.Framework.Graphics.CullMode.None => Microsoft.Xna.Framework.Graphics.CullMode.None,
            Client.Framework.Graphics.CullMode.CullClockwiseFace => Microsoft.Xna.Framework.Graphics.CullMode.CullClockwiseFace,
            Client.Framework.Graphics.CullMode.CullCounterClockwiseFace => Microsoft.Xna.Framework.Graphics.CullMode.CullCounterClockwiseFace,
            _ => throw new ArgumentOutOfRangeException(nameof(cullMode)),
        };
    }

    public static Microsoft.Xna.Framework.Graphics.FillMode ToMonoGame(this Client.Framework.Graphics.FillMode fillMode)
    {
        return fillMode switch
        {
            Client.Framework.Graphics.FillMode.Solid => Microsoft.Xna.Framework.Graphics.FillMode.Solid,
            Client.Framework.Graphics.FillMode.WireFrame => Microsoft.Xna.Framework.Graphics.FillMode.WireFrame,
            _ => throw new ArgumentOutOfRangeException(nameof(fillMode)),
        };
    }

    public static Microsoft.Xna.Framework.GameTime ToMonoGame(this FrameTime frameTime)
    {
        return new(frameTime.Total, frameTime.Delta);
    }

    public static Microsoft.Xna.Framework.Graphics.IndexElementSize ToMonoGame(this Client.Framework.Graphics.IndexElementSize indexElementSize)
    {
        return indexElementSize switch
        {
            Client.Framework.Graphics.IndexElementSize.SixteenBits => Microsoft.Xna.Framework.Graphics.IndexElementSize.SixteenBits,
            Client.Framework.Graphics.IndexElementSize.ThirtyTwoBits => Microsoft.Xna.Framework.Graphics.IndexElementSize.ThirtyTwoBits,
            _ => throw new ArgumentOutOfRangeException(nameof(indexElementSize)),
        };
    }

    public static Microsoft.Xna.Framework.Graphics.PrimitiveType ToMonoGame(this Client.Framework.Graphics.PrimitiveType primitiveType)
    {
        return primitiveType switch
        {
            Client.Framework.Graphics.PrimitiveType.TriangleList => Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList,
            Client.Framework.Graphics.PrimitiveType.TriangleStrip => Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip,
            Client.Framework.Graphics.PrimitiveType.LineList => Microsoft.Xna.Framework.Graphics.PrimitiveType.LineList,
            Client.Framework.Graphics.PrimitiveType.LineStrip => Microsoft.Xna.Framework.Graphics.PrimitiveType.LineStrip,
            _ => throw new ArgumentOutOfRangeException(nameof(primitiveType)),
        };
    }

    public static Microsoft.Xna.Framework.Graphics.VertexDeclaration ToMonoGame(this VertexDeclaration vertexDeclaration)
    {
        return new(
            vertexDeclaration.Stride,
            vertexDeclaration.Elements
                .Select<VertexElement, Microsoft.Xna.Framework.Graphics.VertexElement>(
                    vertexElement => new(
                        vertexElement.Offset,
                        vertexElement.VertexElementFormat.ToMonoGame(),
                        vertexElement.VertexElementUsage.ToMonoGame(),
                        vertexElement.UsageIndex
                    )
                )
                .ToArray()
        );
    }

    public static Microsoft.Xna.Framework.Graphics.VertexElementFormat ToMonoGame(this VertexElementFormat vertexElementFormat)
    {
        return vertexElementFormat switch
        {
            VertexElementFormat.Single => Microsoft.Xna.Framework.Graphics.VertexElementFormat.Single,
            VertexElementFormat.Vector2 => Microsoft.Xna.Framework.Graphics.VertexElementFormat.Vector2,
            VertexElementFormat.Vector3 => Microsoft.Xna.Framework.Graphics.VertexElementFormat.Vector3,
            VertexElementFormat.Vector4 => Microsoft.Xna.Framework.Graphics.VertexElementFormat.Vector4,
            VertexElementFormat.Color => Microsoft.Xna.Framework.Graphics.VertexElementFormat.Color,
            VertexElementFormat.Byte4 => Microsoft.Xna.Framework.Graphics.VertexElementFormat.Byte4,
            VertexElementFormat.Short2 => Microsoft.Xna.Framework.Graphics.VertexElementFormat.Short2,
            VertexElementFormat.Short4 => Microsoft.Xna.Framework.Graphics.VertexElementFormat.Short4,
            VertexElementFormat.NormalizedShort2 => Microsoft.Xna.Framework.Graphics.VertexElementFormat.NormalizedShort2,
            VertexElementFormat.NormalizedShort4 => Microsoft.Xna.Framework.Graphics.VertexElementFormat.NormalizedShort4,
            VertexElementFormat.HalfVector2 => Microsoft.Xna.Framework.Graphics.VertexElementFormat.HalfVector2,
            VertexElementFormat.HalfVector4 => Microsoft.Xna.Framework.Graphics.VertexElementFormat.HalfVector4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexElementFormat)),
        };
    }

    public static Microsoft.Xna.Framework.Graphics.VertexElementUsage ToMonoGame(this VertexElementUsage vertexElementUsage)
    {
        return vertexElementUsage switch
        {
            VertexElementUsage.Position => Microsoft.Xna.Framework.Graphics.VertexElementUsage.Position,
            VertexElementUsage.Color => Microsoft.Xna.Framework.Graphics.VertexElementUsage.Color,
            VertexElementUsage.TextureCoordinate => Microsoft.Xna.Framework.Graphics.VertexElementUsage.TextureCoordinate,
            VertexElementUsage.Normal => Microsoft.Xna.Framework.Graphics.VertexElementUsage.Normal,
            VertexElementUsage.Binormal => Microsoft.Xna.Framework.Graphics.VertexElementUsage.Binormal,
            VertexElementUsage.Tangent => Microsoft.Xna.Framework.Graphics.VertexElementUsage.Tangent,
            VertexElementUsage.BlendIndices => Microsoft.Xna.Framework.Graphics.VertexElementUsage.BlendIndices,
            VertexElementUsage.BlendWeight => Microsoft.Xna.Framework.Graphics.VertexElementUsage.BlendWeight,
            VertexElementUsage.Depth => Microsoft.Xna.Framework.Graphics.VertexElementUsage.Depth,
            VertexElementUsage.Fog => Microsoft.Xna.Framework.Graphics.VertexElementUsage.Fog,
            VertexElementUsage.PointSize => Microsoft.Xna.Framework.Graphics.VertexElementUsage.PointSize,
            VertexElementUsage.Sample => Microsoft.Xna.Framework.Graphics.VertexElementUsage.Sample,
            VertexElementUsage.TessellateFactor => Microsoft.Xna.Framework.Graphics.VertexElementUsage.TessellateFactor,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexElementUsage)),
        };
    }
}
