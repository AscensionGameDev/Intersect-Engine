using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Client.Classes.MonoGame.Graphics;

public partial class MonoTileBuffer(GraphicsDevice device) : GameTileBuffer
{
    public readonly Dictionary<AddedTile, int> _addedTileCount = [];
    private readonly List<short> _indices = [];
    private readonly Dictionary<int, int> _tileVertexOffset = [];
    private readonly List<VertexPositionTexture> _vertices = [];

    private bool _dirty;
    private bool _disposed;

    private IndexBuffer? _indexBuffer;
    private Texture2D? _platformTexture;

    internal IGameTexture? _texture;
    private VertexBuffer? _vertexBuffer;
    private int _vertexCount;

    public override bool Supported => true;

    public override bool TryAddTile(IGameTexture texture, int x, int y, int srcX, int srcY, int srcW, int srcH)
    {
        if (_vertexBuffer != null)
        {
            ApplicationContext.Context.Value?.Logger.LogError("Unable to add tile to null vertex buffer");
            return false;
        }

        var platformTexture = (texture == _texture ? _platformTexture : default) ?? texture.GetTexture<Texture2D>();
        if (platformTexture == null)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                "Unable to add tile to vertex buffer because the platform texture is null"
            );
            return false;
        }

        if (_texture == null)
        {
            _texture = texture;
            _platformTexture = platformTexture;
        }
        else if (_platformTexture != platformTexture)
        {
            ApplicationContext.Context.Value?.Logger.LogError("Mismatched platform texture");
            return false;
        }

        // var addedTileKey = new AddedTile(
        //     texture,
        //     srcX,
        //     srcY,
        //     srcW,
        //     srcH
        // );
        // _addedTileCount[addedTileKey] = _addedTileCount.GetValueOrDefault(addedTileKey, 0) + 1;

        var rotated = false;

        var texturePackFrame = texture.AtlasReference;
        if (texturePackFrame != null)
        {
            var frameBounds = texturePackFrame.Bounds;
            if (texturePackFrame.IsRotated)
            {
                rotated = true;
                (srcX, srcY) = (frameBounds.Right - srcY - srcH, frameBounds.Top + srcX);
                (srcW, srcH) = (srcH, srcW);
            }
            else
            {
                srcX += frameBounds.X;
                srcY += frameBounds.Y;
            }
        }

        var textureSizeX = 1f / platformTexture.Width;
        var textureSizeY = 1f / platformTexture.Height;

        var left = srcX * textureSizeX;
        var right = (srcX + srcW) * textureSizeX;
        var bottom = (srcY + srcH) * textureSizeY;
        var top = srcY * textureSizeY;

        _tileVertexOffset.Add((x << 16) | y, _vertexCount);

        if (rotated)
        {
            _vertices.Add(new VertexPositionTexture(new Vector3(x, y + srcH, 0), new Vector2(left, top)));
            _vertices.Add(new VertexPositionTexture(new Vector3(x, y, 0), new Vector2(right, top)));
            _vertices.Add(new VertexPositionTexture(new Vector3(x + srcW, y + srcH, 0), new Vector2(left, bottom)));
            _vertices.Add(new VertexPositionTexture(new Vector3(x + srcW, y, 0), new Vector2(right, bottom)));
        }
        else
        {
            _vertices.Add(new VertexPositionTexture(new Vector3(x, y + srcH, 0), new Vector2(left, bottom)));
            _vertices.Add(new VertexPositionTexture(new Vector3(x, y, 0), new Vector2(left, top)));
            _vertices.Add(new VertexPositionTexture(new Vector3(x + srcW, y + srcH, 0), new Vector2(right, bottom)));
            _vertices.Add(new VertexPositionTexture(new Vector3(x + srcW, y, 0), new Vector2(right, top)));
        }

        _indices.Add((short)_vertexCount);
        _indices.Add((short)(_vertexCount + 1));
        _indices.Add((short)(_vertexCount + 2));

        _indices.Add((short)(_vertexCount + 2));
        _indices.Add((short)(_vertexCount + 1));
        _indices.Add((short)(_vertexCount + 3));

        _vertexCount += 4;

        return true;
    }

    public override bool TryUpdateTile(IGameTexture texture, int x, int y, int srcX, int srcY, int srcW, int srcH)
    {
        if (_vertexBuffer == default)
        {
            ApplicationContext.Context.Value?.Logger.LogError("Unable to update tile on null vertex buffer");
            return false;
        }

        if (!_tileVertexOffset.TryGetValue((x << 16) | y, out var vertexIndex))
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                "Unable to update tile that has not been added to the vertex buffer before"
            );
            return false;
        }

        var platformTexture = (texture == _texture ? _platformTexture : default) ?? texture.GetTexture<Texture2D>();
        if (platformTexture == null)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                "Unable to add tile to vertex buffer because the platform texture is null"
            );
            return false;
        }

        if (_texture == null)
        {
            _texture = texture;
            _platformTexture = platformTexture;
        }
        else if (_platformTexture != platformTexture)
        {
            ApplicationContext.Context.Value?.Logger.LogError("Mismatched platform texture");
            return false;
        }

        var rotated = false;

        var texturePackFrame = texture.AtlasReference;
        if (texturePackFrame != null)
        {
            if (texturePackFrame.IsRotated)
            {
                rotated = true;

                var z = srcX;
                srcX = texturePackFrame.Bounds.Right - srcY - srcH;
                srcY = texturePackFrame.Bounds.Top + z;

                z = srcW;
                srcW = srcH;
                srcH = z;
            }
            else
            {
                srcX += texturePackFrame.Bounds.X;
                srcY += texturePackFrame.Bounds.Y;
            }
        }

        var textureSizeX = 1f / platformTexture.Width;
        var textureSizeY = 1f / platformTexture.Height;

        var left = srcX * textureSizeX;
        var right = (srcX + srcW) * textureSizeX;
        var bottom = (srcY + srcH) * textureSizeY;
        var top = srcY * textureSizeY;

        if (rotated)
        {
            _vertices[vertexIndex++] = new VertexPositionTexture(new Vector3(x, y + srcH, 0), new Vector2(left, top));
            _vertices[vertexIndex++] = new VertexPositionTexture(new Vector3(x, y, 0), new Vector2(right, top));
            _vertices[vertexIndex++] = new VertexPositionTexture(
                new Vector3(x + srcW, y + srcH, 0),
                new Vector2(left, bottom)
            );
            _vertices[vertexIndex] = new VertexPositionTexture(new Vector3(x + srcW, y, 0), new Vector2(right, bottom));
        }
        else
        {
            _vertices[vertexIndex++] = new VertexPositionTexture(
                new Vector3(x, y + srcH, 0),
                new Vector2(left, bottom)
            );

            _vertices[vertexIndex++] = new VertexPositionTexture(new Vector3(x, y, 0), new Vector2(left, top));
            _vertices[vertexIndex++] = new VertexPositionTexture(
                new Vector3(x + srcW, y + srcH, 0),
                new Vector2(right, bottom)
            );

            _vertices[vertexIndex] = new VertexPositionTexture(new Vector3(x + srcW, y, 0), new Vector2(right, top));
        }

        _dirty = true;

        return true;
    }

    public void Draw(BasicEffect basicEffect, FloatRect view)
    {
        if (_disposed || _vertexBuffer == default)
        {
            return;
        }

        device.SetVertexBuffer(_vertexBuffer);
        device.Indices = _indexBuffer;

        // If we use _platformTexture directly here it will get marked as "unused" and disposed
        basicEffect.Texture = _texture?.GetTexture<Texture2D>();

        foreach (var pass in basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            device.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                0,
                0,
                _vertexCount / 2
            );
        }
    }

    public override bool SetData()
    {
        if (_vertexBuffer != null && !_dirty)
        {
            return false;
        }

        if (_vertices.Count == 0)
        {
            return true;
        }

        TileBufferCount++;

        _vertexBuffer ??= new VertexBuffer(
            device,
            typeof(VertexPositionTexture),
            _vertices.Count,
            BufferUsage.WriteOnly
        );

        _vertexBuffer.SetData(_vertices.ToArray());

        _indexBuffer ??= new IndexBuffer(
            device,
            typeof(short),
            _indices.Count,
            BufferUsage.WriteOnly
        );

        if (!_dirty)
        {
            _indexBuffer.SetData(_indices.ToArray());
        }

        _dirty = false;

        return true;
    }

    public override void Dispose()
    {
        _vertexBuffer?.Dispose();
        _vertexBuffer = null;
        _indexBuffer?.Dispose();
        _indexBuffer = null;
        TileBufferCount--;
        _disposed = true;
    }

    public record struct AddedTile(IGameTexture Texture, int SrcX, int SrcY, int SrcW, int SrcH);
}