using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Localization;
using Intersect.IO.Files;
using Intersect.Logging;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.IO;

namespace Intersect.Client.MonoGame.Graphics
{
    public class MonoGameTexture : GameTexture<Texture2D>
    {
        private GraphicsDevice mGraphicsDevice;

        private int mHeight = -1;

        private long mLastAccessTime;

        private bool mLoadError;

        private string mName = "";

        private string mPath = "";

        private int mWidth = -1;

        private readonly Func<Stream> CreateStream;

        public MonoGameTexture(GraphicsDevice graphicsDevice, string filename) : base(Path.GetFileName(filename))
        {
            mGraphicsDevice = graphicsDevice;
            mPath = filename;
            mName = Path.GetFileName(filename);
        }

        public MonoGameTexture(GraphicsDevice graphicsDevice, string assetName, Func<Stream> createStream) : base(assetName)
        {
            mGraphicsDevice = graphicsDevice;
            mPath = assetName;
            mName = assetName;
            CreateStream = createStream;
        }

        public MonoGameTexture(GraphicsDevice graphicsDevice, string filename, ITexturePackFrame texturePackFrame) : base(Path.GetFileName(filename), texturePackFrame)
        {
            mGraphicsDevice = graphicsDevice;
            mPath = filename;
            mName = Path.GetFileName(filename);
            mWidth = TexturePackFrame.SourceBounds.Width;
            mHeight = TexturePackFrame.SourceBounds.Height;
        }

        private void Load(Stream stream)
        {
            PlatformTexture = Texture2D.FromStream(mGraphicsDevice, stream);
            if (PlatformTexture == null)
            {
                throw new InvalidDataException("Failed to load texture, received no data.");
            }

            mWidth = PlatformTexture.Width;
            mHeight = PlatformTexture.Height;
            mLoadError = false;
        }

        public void LoadTexture()
        {
            if (PlatformTexture != null)
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

            if (TexturePackFrame != null)
            {
                (TexturePackFrame.PackedTexture as MonoGameTexture)?.LoadTexture();

                return;
            }

            mLoadError = true;
            if (string.IsNullOrWhiteSpace(mPath))
            {
                Log.Error("Invalid texture path (empty/null).");

                return;
            }

            var relativePath = FileSystemHelper.RelativePath(Directory.GetCurrentDirectory(), mPath);

            if (!File.Exists(mPath))
            {
                Log.Error($"Texture does not exist: {relativePath}");

                return;
            }

            using (var fileStream = File.Open(mPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                try
                {
                    Load(fileStream);
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
                            new Color(0xBF, 0x0, 0x0)
                        )
                    );
                }
            }
        }

        public void ResetAccessTime()
        {
            mLastAccessTime = Globals.System.GetTimeMs() + 15000;
        }

        public override int Width
        {
            get
            {
                ResetAccessTime();
                if (mWidth != -1)
                {
                    return mWidth;
                }

                if (PlatformTexture == null)
                {
                    LoadTexture();
                }

                if (mLoadError)
                {
                    mWidth = 0;
                }

                return mWidth;
            }
        }

        public override int Height
        {
            get
            {
                ResetAccessTime();
                if (mHeight != -1)
                {
                    return mHeight;
                }

                if (PlatformTexture == null)
                {
                    LoadTexture();
                }

                if (mLoadError)
                {
                    mHeight = 0;
                }

                return mHeight;
            }
        }

        public override TTexture AsPlatformTexture<TTexture>()
        {
            if (TexturePackFrame == null)
            {
                ResetAccessTime();
            }

            if (PlatformTexture == null)
            {
                LoadTexture();
            }

            return base.AsPlatformTexture<TTexture>();
        }

        public override Color GetPixel(int x1, int y1)
        {
            if (PlatformTexture == null)
            {
                LoadTexture();
            }

            if (mLoadError)
            {
                return Color.White;
            }

            var tex = PlatformTexture;

            var pack = TexturePackFrame;
            if (pack != null)
            {
                tex = pack.PackedTexture.AsPlatformTexture<Texture2D>();
                if (pack.IsRotated)
                {
                    var z = x1;
                    x1 = pack.Bounds.Right - y1 - pack.Bounds.Height;
                    y1 = pack.Bounds.Top + z;
                }
                else
                {
                    x1 += pack.Bounds.X;
                    y1 += pack.Bounds.Y;
                }
            }

            var pixel = new Microsoft.Xna.Framework.Color[1];
            tex?.GetData(0, new Rectangle(x1, y1, 1, 1), pixel, 0, 1);

            return new Color(pixel[0].A, pixel[0].R, pixel[0].G, pixel[0].B);
        }

        public void Update()
        {
            if (PlatformTexture == null)
            {
                return;
            }

            if (mLastAccessTime >= Globals.System.GetTimeMs())
            {
                return;
            }

            PlatformTexture.Dispose();
            PlatformTexture = null;
        }
    }
}
