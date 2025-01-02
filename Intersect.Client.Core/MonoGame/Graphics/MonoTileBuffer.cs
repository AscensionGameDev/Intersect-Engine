using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Client.Classes.MonoGame.Graphics;


public partial class MonoTileBuffer : GameTileBuffer
{
    private bool _disposed;

    private IndexBuffer? _indexBuffer;

    private readonly List<short> _indices = [];

    private GraphicsDevice _graphicsDevice;

    private bool _updatesPending;

    private VertexBuffer? _vertexBuffer;

    private int _vertexCount;

    private GameTexture? _texture;
    private Texture2D? _platformTexture;

    private readonly Dictionary<Vector2iKey, int> _tileVertexOffset = new(Options.MapWidth * Options.MapHeight);

    private readonly List<VertexPositionTexture> _vertices = [];

    public MonoTileBuffer(GraphicsDevice device)
    {
        _graphicsDevice = device;
    }

    public override bool Supported => true;

    public override bool AddTile(GameTexture texture, int x, int y, int srcX, int srcY, int srcW, int srcH)
    {
        if (_vertexBuffer != null)
        {
            return false;
        }

        var platformTexture = (texture == _texture ? _platformTexture : default) ?? texture.GetTexture<Texture2D>();
        if (platformTexture == null)
        {
            return false;
        }

        if (_texture == null)
        {
            _texture = texture;
            _platformTexture = platformTexture;
        }
        else if (_platformTexture != platformTexture)
        {
            return false;
        }

        var rotated = false;
        var texturePackFrame = texture.GetTexturePackFrame();
        if (texturePackFrame != null)
        {
            var frameBounds = texturePackFrame.Rect;
            if (texturePackFrame.Rotated)
            {
                rotated = true;

                var z = srcX;
                srcX = frameBounds.Right - srcY - srcH;
                srcY = frameBounds.Top + z;

                z = srcW;
                srcW = srcH;
                srcH = z;
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

        _tileVertexOffset.Add(new Vector2iKey(x, y), _vertexCount);

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

        _indices.Add((short) _vertexCount);
        _indices.Add((short) (_vertexCount + 1));
        _indices.Add((short) (_vertexCount + 2));

        _indices.Add((short) (_vertexCount + 2));
        _indices.Add((short) (_vertexCount + 1));
        _indices.Add((short) (_vertexCount + 3));

        _vertexCount += 4;

        return true;
    }

    public void Draw(BasicEffect basicEffect, FloatRect view)
    {
        if (_vertexBuffer != null && !_disposed)
        {
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;

            basicEffect.Texture = (Texture2D) _texture.GetTexture();
            foreach (var pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertexCount / 2);
            }
        }
    }

    public override bool SetData()
    {
        if (_vertexBuffer != null && !_updatesPending)
        {
            return false;
        }

        if (_vertices.Count == 0)
        {
            return true;
        }

        TileBufferCount++;

        _vertexBuffer ??= new VertexBuffer(
            _graphicsDevice,
            typeof(VertexPositionTexture),
            _vertices.Count,
            BufferUsage.WriteOnly
        );

        _vertexBuffer.SetData(_vertices.ToArray());

        _indexBuffer ??= new IndexBuffer(
            _graphicsDevice,
            typeof(short),
            _indices.Count,
            BufferUsage.WriteOnly
        );

        if (!_updatesPending)
        {
            _indexBuffer.SetData(_indices.ToArray());
        }

        _updatesPending = false;

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

    public override bool UpdateTile(GameTexture tex, int x, int y, int srcX, int srcY, int srcW, int srcH)
    {
        var vertexIndex = -1;
        if (_tileVertexOffset.TryGetValue(new Vector2iKey(x, y), out var value))
        {
            vertexIndex = value;
        }

        if (vertexIndex == -1)
        {
            return false;
        }

        var platformTex = tex?.GetTexture();
        if (platformTex == null)
        {
            return false;
        }

        if (tex == null)
        {
            return false;
        }

        if (tex.GetTexture() != platformTex)
        {
            return false;
        }

        if (_vertexBuffer == null)
        {
            return false;
        }

        var pack = tex.GetTexturePackFrame();

        var rotated = false;
        if (pack != null)
        {
            if (pack.Rotated)
            {
                rotated = true;

                var z = srcX;
                srcX = pack.Rect.Right - srcY - srcH;
                srcY = pack.Rect.Top + z;

                z = srcW;
                srcW = srcH;
                srcH = z;
            }
            else
            {
                srcX += pack.Rect.X;
                srcY += pack.Rect.Y;
            }
        }

        var texture = (Texture2D) tex.GetTexture();

        var textureSizeX = 1f / texture.Width;
        var textureSizeY = 1f / texture.Height;

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

        _updatesPending = true;

        return true;
    }

}
