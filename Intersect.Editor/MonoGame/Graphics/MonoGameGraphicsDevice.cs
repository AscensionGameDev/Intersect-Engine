using Intersect.Client.Framework.Graphics;
using Intersect.Numerics;

namespace Intersect.Editor.MonoGame.Graphics;

using Color = Intersect.Graphics.Color;
using Point = Numerics.Point;
using MGColor = Microsoft.Xna.Framework.Color;
using MGGraphicsDevice = Microsoft.Xna.Framework.Graphics.GraphicsDevice;

internal sealed class MonoGameGraphicsDevice : GraphicsDevice
{
    private readonly MGGraphicsDevice _graphicsDevice;

    internal MonoGameGraphicsDevice(MGGraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
    }

    public override Point BackBufferSize
    {
        get => new(_graphicsDevice.PresentationParameters.BackBufferWidth, _graphicsDevice.PresentationParameters.BackBufferHeight);
        set
        {
            _graphicsDevice.PresentationParameters.BackBufferWidth = value.X;
            _graphicsDevice.PresentationParameters.BackBufferHeight = value.Y;
        }
    }

    public override Color BlendFactor
    {
        get => new(_graphicsDevice.BlendFactor.PackedValue);
        set => _graphicsDevice.BlendFactor = new MGColor(value.PackedValue);
    }
    
    public override Blending Blending
    {
        get => new MonoGameBlending(_graphicsDevice.BlendState);
        set
        {
            if (value == default)
            {
                return;
            }

            _graphicsDevice.BlendState = new()
            {
                ColorSourceBlend = value.SourceBlendMode.ToMonoGame(),
                AlphaSourceBlend = value.SourceBlendMode.ToMonoGame(),
                ColorDestinationBlend = value.DestinationBlendMode.ToMonoGame(),
                AlphaDestinationBlend = value.DestinationBlendMode.ToMonoGame(),
            };
        }
    }
    
    public override Rectangle ClippingBounds
    {
        get
        {
            var clippingRectangle = _graphicsDevice.ScissorRectangle;
            return new(clippingRectangle.X, clippingRectangle.Y, clippingRectangle.Width, clippingRectangle.Height);
        }

        set
        {
            _graphicsDevice.ScissorRectangle = new(
                value.X,
                value.Y,
                value.Width,
                value.Height
            );
        }
    }
    
    public override DepthStencil DepthStencil
    {
        get => new MonoGameDepthStencil(_graphicsDevice.DepthStencilState);
        set
        {
            if (value == default)
            {
                return;
            }

            _graphicsDevice.DepthStencilState = new()
            {
                DepthBufferEnable = value.DepthBufferEnable,
                DepthBufferWriteEnable = value.DepthBufferWriteEnable,
            };
        }
    }
    
    public override Rasterizer Rasterizer
    {
        get => new MonoGameRasterizer(_graphicsDevice.RasterizerState);
        set
        {
            if (value == default)
            {
                return;
            }

            _graphicsDevice.RasterizerState = new()
            {
                CullMode = value.CullMode.ToMonoGame(),
                DepthBias = value.DepthBias,
                FillMode = value.FillMode.ToMonoGame(),
                MultiSampleAntiAlias = value.MultiSampleAntiAlias,
                ScissorTestEnable = value.ScissorTestEnable,
                SlopeScaleDepthBias = value.SlopeScaleDepthBias,
            };
        }
    }
    
    public override Viewport Viewport
    {
        get
        {
            var viewport = _graphicsDevice.Viewport;
            return new(viewport.X, viewport.Y, viewport.Width, viewport.Height);
        }

        set
        {
            _graphicsDevice.Viewport = new(
                value.X,
                value.Y,
                value.Width,
                value.Height
            );
        }
    }

    public override Effect CreateEffect() => new MonoGameEffect(new Microsoft.Xna.Framework.Graphics.BasicEffect(_graphicsDevice));

    public override IndexBuffer CreateIndexBuffer(IndexElementSize indexElementSize, int indexCount, BufferUsage bufferUsage)
    {
        return new MonoGameIndexBuffer(
            this,
            new(
                _graphicsDevice,
                indexElementSize.ToMonoGame(),
                indexCount,
                bufferUsage.ToMonoGame()
            )
        );
    }

    public override IndexBuffer CreateIndexBuffer(Type indexType, int indexCount, BufferUsage usage)
    {
        throw new NotImplementedException();
    }

    public override Rasterizer CreateRasterizer(CullMode cullMode = CullMode.None, float depthBias = 0, FillMode fillMode = FillMode.Solid, bool multiSampleAntiAlias = false, bool scissorTestEnable = false, float slopeScaleDepthBias = 0)
    {
        return new MonoGameRasterizer(new()
        {
            CullMode = cullMode.ToMonoGame(),
            DepthBias = depthBias,
            FillMode = fillMode.ToMonoGame(),
            MultiSampleAntiAlias = multiSampleAntiAlias,
            ScissorTestEnable = scissorTestEnable,
            SlopeScaleDepthBias = slopeScaleDepthBias,
        });
    }

    public override Texture CreateTexture(int width, int height, bool mipMapEnabled)
    {
        return new MonoGameTexture(new(_graphicsDevice, width, height, mipMapEnabled, Microsoft.Xna.Framework.Graphics.SurfaceFormat.Color));
    }

    public override VertexBuffer CreateVertexBuffer(VertexDeclaration vertexDeclaration, int vertexCount, BufferUsage bufferUsage)
    {
        return new MonoGameVertexBuffer(
            this,
            new Microsoft.Xna.Framework.Graphics.VertexBuffer(
                _graphicsDevice,
                vertexDeclaration.ToMonoGame(),
                vertexCount,
                bufferUsage.ToMonoGame()
            ),
            vertexDeclaration,
            vertexCount,
            bufferUsage,
            false
        );
    }

    public override VertexBuffer CreateVertexBuffer(VertexDeclaration vertexDeclaration, int vertexCount, BufferUsage bufferUsage, bool dynamic)
    {
        throw new NotImplementedException();
    }

    public override VertexBuffer CreateVertexBuffer(Type type, int vertexCount, BufferUsage bufferUsage)
    {
        throw new NotImplementedException();
    }

    public override void DrawIndexedPrimitives(PrimitiveType primitiveType, int baseVertex, int startIndex, int primitiveCount)
    {
        _graphicsDevice.DrawIndexedPrimitives(primitiveType.ToMonoGame(), baseVertex, startIndex, primitiveCount);
    }

    public override void SetIndexBuffer(IndexBuffer indexBuffer)
    {
        if (indexBuffer is not MonoGameIndexBuffer monoGameIndexBuffer)
        {
            throw new ArgumentException($"Index buffer of type {indexBuffer.GetType().FullName} is not supported by {GetType().FullName}", nameof(indexBuffer));
        }

        _graphicsDevice.Indices = monoGameIndexBuffer._indexBuffer;
    }

    public override void SetVertexBuffer(VertexBuffer vertexBuffer, int vertexOffset = 0)
    {
        if (vertexBuffer is not MonoGameVertexBuffer monoGameVertexBuffer)
        {
            throw new ArgumentException($"Vertex buffer of type {vertexBuffer.GetType().FullName} is not supported by {GetType().FullName}", nameof(vertexBuffer));
        }

        _graphicsDevice.SetVertexBuffer(monoGameVertexBuffer._vertexBuffer, vertexOffset);
    }

    static MonoGameGraphicsDevice()
    {
        MonoGameBlending.InitializePresets();
    }
}
