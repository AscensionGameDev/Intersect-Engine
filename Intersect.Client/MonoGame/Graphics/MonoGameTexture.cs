using Intersect.Client.Framework;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Localization;
using Intersect.Logging;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.IO;

namespace Intersect.Client.MonoGame.Graphics
{
    public class MonoGameTexture : GameTexture<Texture2D>
    {
        internal new MonoGameContext GameContext => base.GameContext as MonoGameContext;

        private int mHeight = -1;

        private int mWidth = -1;

        private long LastAccessTime { get; set; }

        private bool LoadError { get; set; }

        public MonoGameTexture(
            IGameContext gameContext,
            AssetReference assetReference,
            ITexturePackFrame texturePackFrame = null
        ) : base(gameContext, assetReference, texturePackFrame)
        {
            if (TexturePackFrame != null)
            {
                mWidth = TexturePackFrame.SourceBounds.Width;
                mHeight = TexturePackFrame.SourceBounds.Height;
            }
        }

        private void LoadTexture()
        {
            if (PlatformTexture != null)
            {
                return;
            }

            if (TexturePackFrame != null)
            {
                (TexturePackFrame.PackedTexture as MonoGameTexture)?.LoadTexture();

                return;
            }

            LoadError = true;

            try
            {
                using (var stream = GameContext.ContentManager.OpenRead(Reference))
                {
                    PlatformTexture = Texture2D.FromStream(GameContext.Game.GraphicsDevice, stream);
                    if (PlatformTexture == null)
                    {
                        throw new InvalidDataException("Failed to load texture, received no data.");
                    }

                    mWidth = PlatformTexture.Width;
                    mHeight = PlatformTexture.Height;
                    LoadError = false;
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, $"Failed to load texture ({Reference})");

                ChatboxMsg.AddMessage(
                    new ChatboxMsg(
                        $"{Strings.Errors.LoadFile.ToString(Strings.Words.lcase_sprite)} [{Name}]",
                        new Color(0xBF, 0x0, 0x0)
                    )
                );
            }
        }

        public void ResetAccessTime() => LastAccessTime = Globals.System.GetTimeMs() + 15000;

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

                if (LoadError)
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

                if (LoadError)
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

        public override Color GetPixel(int x, int y)
        {
            if (PlatformTexture == null)
            {
                LoadTexture();
            }

            if (LoadError)
            {
                return Color.Magenta;
            }

            var platformTexture = PlatformTexture;
            var texturePackFrame = TexturePackFrame;

            if (texturePackFrame != null)
            {
                platformTexture = texturePackFrame.PackedTexture.AsPlatformTexture<Texture2D>();
                if (texturePackFrame.IsRotated)
                {
                    var z = x;
                    x = texturePackFrame.Bounds.Right - y - texturePackFrame.Bounds.Height;
                    y = texturePackFrame.Bounds.Top + z;
                }
                else
                {
                    x += texturePackFrame.Bounds.X;
                    y += texturePackFrame.Bounds.Y;
                }
            }

            var pixel = new Microsoft.Xna.Framework.Color[1];
            platformTexture?.GetData(0, new Rectangle(x, y, 1, 1), pixel, 0, 1);

            return new Color(pixel[0].A, pixel[0].R, pixel[0].G, pixel[0].B);
        }

        public void Update()
        {
            if (PlatformTexture == null)
            {
                return;
            }

            if (LastAccessTime >= Globals.System.GetTimeMs())
            {
                return;
            }

            PlatformTexture.Dispose();
            PlatformTexture = null;
        }
    }
}
