using System;
using System.Collections.Generic;

using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Client.Classes.MonoGame.Graphics
{

    public class MonoTileBuffer : GameTileBuffer
    {

        private bool disposed;

        private IndexBuffer indexBuffer;

        private List<short> indices = new List<short>();

        private GraphicsDevice mGraphicsDevice;

        private bool updatesPending = false;

        private VertexBuffer vertexBuffer;

        private int vertexCount;

        private Dictionary<Tuple<float, float>, int> verticeDict = new Dictionary<Tuple<float, float>, int>();

        private List<VertexPositionTexture> vertices = new List<VertexPositionTexture>();

        public MonoTileBuffer(GraphicsDevice device)
        {
            mGraphicsDevice = device;
        }

        public override bool Supported { get; } = true;

        public override GameTexture Texture { get; protected set; }

        public override bool AddTile(GameTexture tex, float x, float y, int srcX, int srcY, int srcW, int srcH)
        {
            var platformTex = tex?.GetTexture();
            if (platformTex == null)
            {
                return false;
            }

            if (Texture == null)
            {
                Texture = tex;
            }
            else if (Texture.GetTexture() != platformTex)
            {
                return false;
            }

            if (vertexBuffer != null)
            {
                return false;
            }

            var rotated = false;
            var pack = tex.GetTexturePackFrame();
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

            verticeDict.Add(new Tuple<float, float>(x, y), vertices.Count);

            if (rotated)
            {
                vertices.Add(new VertexPositionTexture(new Vector3(x, y + srcH, 0), new Vector2(left, top)));
                vertices.Add(new VertexPositionTexture(new Vector3(x, y, 0), new Vector2(right, top)));
                vertices.Add(new VertexPositionTexture(new Vector3(x + srcW, y + srcH, 0), new Vector2(left, bottom)));
                vertices.Add(new VertexPositionTexture(new Vector3(x + srcW, y, 0), new Vector2(right, bottom)));
            }
            else
            {
                vertices.Add(new VertexPositionTexture(new Vector3(x, y + srcH, 0), new Vector2(left, bottom)));
                vertices.Add(new VertexPositionTexture(new Vector3(x, y, 0), new Vector2(left, top)));
                vertices.Add(new VertexPositionTexture(new Vector3(x + srcW, y + srcH, 0), new Vector2(right, bottom)));
                vertices.Add(new VertexPositionTexture(new Vector3(x + srcW, y, 0), new Vector2(right, top)));
            }

            indices.Add((short) vertexCount);
            indices.Add((short) (vertexCount + 1));
            indices.Add((short) (vertexCount + 2));

            indices.Add((short) (vertexCount + 2));
            indices.Add((short) (vertexCount + 1));
            indices.Add((short) (vertexCount + 3));

            vertexCount += 4;

            return true;
        }

        public void Draw(BasicEffect basicEffect, FloatRect view)
        {
            if (vertexBuffer != null && !disposed)
            {
                mGraphicsDevice.SetVertexBuffer(vertexBuffer);
                mGraphicsDevice.Indices = indexBuffer;

                basicEffect.Texture = (Texture2D) Texture.GetTexture();
                foreach (var pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    mGraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount / 2);
                }
            }
        }

        public override bool SetData()
        {
            if (vertexBuffer != null && !updatesPending)
            {
                return false;
            }

            if (vertices.Count == 0)
            {
                return true;
            }

            TileBufferCount++;
            if (vertexBuffer == null)
            {
                vertexBuffer = new VertexBuffer(
                    mGraphicsDevice, typeof(VertexPositionTexture), vertices.Count, BufferUsage.WriteOnly
                );

                indexBuffer = new IndexBuffer(mGraphicsDevice, typeof(short), indices.Count, BufferUsage.WriteOnly);
            }

            vertexBuffer.SetData(vertices.ToArray());
            if (!updatesPending)
            {
                indexBuffer.SetData(indices.ToArray());
            }

            updatesPending = false;

            return true;
        }

        public override void Dispose()
        {
            vertexBuffer?.Dispose();
            vertexBuffer = null;
            indexBuffer?.Dispose();
            indexBuffer = null;
            TileBufferCount--;
            disposed = true;
        }

        public override bool UpdateTile(GameTexture tex, float x, float y, int srcX, int srcY, int srcW, int srcH)
        {
            var key = new Tuple<float, float>(x, y);
            var vertexIndex = -1;
            if (verticeDict.ContainsKey(key))
            {
                vertexIndex = verticeDict[key];
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

            if (vertexBuffer == null)
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
                vertices[vertexIndex++] = new VertexPositionTexture(
                    new Vector3(x, y + srcH, 0), new Vector2(left, top)
                );

                vertices[vertexIndex++] = new VertexPositionTexture(new Vector3(x, y, 0), new Vector2(right, top));
                vertices[vertexIndex++] = new VertexPositionTexture(
                    new Vector3(x + srcW, y + srcH, 0), new Vector2(left, bottom)
                );

                vertices[vertexIndex] = new VertexPositionTexture(
                    new Vector3(x + srcW, y, 0), new Vector2(right, bottom)
                );
            }
            else
            {
                vertices[vertexIndex++] = new VertexPositionTexture(
                    new Vector3(x, y + srcH, 0), new Vector2(left, bottom)
                );

                vertices[vertexIndex++] = new VertexPositionTexture(new Vector3(x, y, 0), new Vector2(left, top));
                vertices[vertexIndex++] = new VertexPositionTexture(
                    new Vector3(x + srcW, y + srcH, 0), new Vector2(right, bottom)
                );

                vertices[vertexIndex] = new VertexPositionTexture(new Vector3(x + srcW, y, 0), new Vector2(right, top));
            }

            updatesPending = true;

            return true;
        }

    }

}
