using System;
using System.IO;

using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Localization;
using Intersect.Compression;
using Intersect.IO.Files;
using Intersect.Logging;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Client.MonoGame.Graphics
{
    public class MonoTexture : GameTexture
    {
        private GraphicsDevice mGraphicsDevice;

        private int mHeight = -1;

        private long mLastAccessTime;

        private bool mLoadError;

        private string mName = "";

        private GameTexturePackFrame mPackFrame;

        private readonly string mPath = "";

        private readonly string mRealPath = "";

        private Texture2D mTexture;

        private int mWidth = -1;

        private readonly Func<Stream> CreateStream;

        public MonoTexture(GraphicsDevice graphicsDevice, string filename, string realPath)
        {
            mGraphicsDevice = graphicsDevice;
            mPath = filename;
            mRealPath = realPath;
            mName = Path.GetFileName(filename);
        }

        public MonoTexture(GraphicsDevice graphicsDevice, string assetName, Func<Stream> createStream)
        {
            mGraphicsDevice = graphicsDevice;
            mPath = assetName;
            mName = assetName;
            CreateStream = createStream;
        }

        public MonoTexture(GraphicsDevice graphicsDevice, string filename, GameTexturePackFrame packFrame)
        {
            mGraphicsDevice = graphicsDevice;
            mPath = filename;
            mName = Path.GetFileName(filename);
            mPackFrame = packFrame;
            mWidth = packFrame.SourceRect.Width;
            mHeight = packFrame.SourceRect.Height;
        }

        private void Load(Stream stream)
        {
            mTexture = Texture2D.FromStream(mGraphicsDevice, stream);
            if (mTexture == null)
            {
                throw new InvalidDataException("Failed to load texture, received no data.");
            }

            mWidth = mTexture.Width;
            mHeight = mTexture.Height;
            mLoadError = false;
        }

        public void LoadTexture()
        {
            if (mTexture != null)
            {
                return;
            }

            if (CreateStream != null)
            {
                using (var stream = CreateStream())
                {
                    Load(stream);
                    return;
                }
            }

            if (mPackFrame != null)
            {
                ((MonoTexture) mPackFrame.PackTexture)?.LoadTexture();

                return;
            }

            mLoadError = true;
            if (string.IsNullOrWhiteSpace(mRealPath))
            {
                Log.Error("Invalid texture path (empty/null).");

                return;
            }

            var relativePath = FileSystemHelper.RelativePath(Directory.GetCurrentDirectory(), mPath);

            if (!File.Exists(mRealPath))
            {
                Log.Error($"Texture does not exist: {relativePath}");

                return;
            }

            using (var fileStream = File.Open(mRealPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                try
                {
                    if (Path.GetExtension(mPath) == ".asset")
                    {
                        using (var gzip = GzipCompression.CreateDecompressedFileStream(fileStream))
                        {
                            Load(gzip);
                        }
                    }
                    else
                    {
                        Load(fileStream);
                    }
                }
                catch (Exception exception)
                {
                    Log.Error(
                        exception,
                        $"Failed to load texture ({FileSystemHelper.FormatSize(fileStream.Length)}): {relativePath}"
                    );

                    ChatboxMsg.AddMessage(
                        new ChatboxMsg(
                            Strings.Errors.LoadFile.ToString(Strings.Words.lcase_sprite) + " [" + mName + "]",
                            new Color(0xBF, 0x0, 0x0), Enums.ChatMessageType.Error
                        )
                    );
                }
            }
        }

        public void ResetAccessTime()
        {
            mLastAccessTime = Globals.System.GetTimeMs() + 15000;
        }

        public override string GetName()
        {
            return mName;
        }

        public override int GetWidth()
        {
            ResetAccessTime();
            if (mWidth != -1)
            {
                return mWidth;
            }

            if (mTexture == null)
            {
                LoadTexture();
            }

            if (mLoadError)
            {
                mWidth = 0;
            }

            return mWidth;
        }

        public override int GetHeight()
        {
            ResetAccessTime();
            if (mHeight != -1)
            {
                return mHeight;
            }

            if (mTexture == null)
            {
                LoadTexture();
            }

            if (mLoadError)
            {
                mHeight = 0;
            }

            return mHeight;
        }

        public override object GetTexture()
        {
            if (mPackFrame != null)
            {
                return mPackFrame.PackTexture.GetTexture();
            }

            ResetAccessTime();

            if (mTexture == null)
            {
                LoadTexture();
            }

            return mTexture;
        }

        public override Color GetPixel(int x1, int y1)
        {
            if (mTexture == null)
            {
                LoadTexture();
            }

            if (mLoadError)
            {
                return Color.White;
            }

            var tex = mTexture;

            var pack = GetTexturePackFrame();
            if (pack != null)
            {
                tex = (Texture2D) mPackFrame.PackTexture.GetTexture();
                if (pack.Rotated)
                {
                    var z = x1;
                    x1 = pack.Rect.Right - y1 - pack.Rect.Height;
                    y1 = pack.Rect.Top + z;
                }
                else
                {
                    x1 += pack.Rect.X;
                    y1 += pack.Rect.Y;
                }
            }

            var pixel = new Microsoft.Xna.Framework.Color[1];
            tex?.GetData(0, new Rectangle(x1, y1, 1, 1), pixel, 0, 1);

            return new Color(pixel[0].A, pixel[0].R, pixel[0].G, pixel[0].B);
        }

        public override GameTexturePackFrame GetTexturePackFrame()
        {
            return mPackFrame;
        }

        public void Update()
        {
            if (mTexture == null)
            {
                return;
            }

            if (mLastAccessTime >= Globals.System.GetTimeMs())
            {
                return;
            }

            mTexture.Dispose();
            mTexture = null;
        }
    }
}
