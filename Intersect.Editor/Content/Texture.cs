using System;
using System.IO;

using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.IO.Files;
using Intersect.Logging;

using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Editor.Content
{
    public class Texture
    {
        private readonly string mPath;

        private long mLastAccessTime;

        private bool mLoadError;

        private int mWidth = -1;

        private int mHeight = -1;

        private Texture2D mTexture;

        public Texture(string path)
        {
            mPath = path;
            GameContentManager.AllTextures.Add(this);
        }

        public void LoadTexture()
        {
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
                    mTexture = Texture2D.FromStream(Graphics.GetGraphicsDevice(), fileStream);
                    if (mTexture == null)
                    {
                        Log.Error($"Failed to load texture due to unknown error: {relativePath}");

                        return;
                    }

                    mWidth = mTexture.Width;
                    mHeight = mTexture.Height;
                    mLoadError = false;
                }
                catch (Exception exception)
                {
                    Log.Error(
                        exception,
                        $"Failed to load texture ({FileSystemHelper.FormatSize(fileStream.Length)}): {relativePath}"
                    );
                }
            }
        }

        public string GetPath() => mPath;

        public void ResetAccessTime()
        {
            mLastAccessTime = Globals.System.GetTimeMs() + 15000;
        }

        public int GetWidth()
        {
            ResetAccessTime();
            if (mWidth != -1)
            {
                return mWidth;
            }

            if (mTexture == null)
            {
                GetDimensions();
            }

            if (mLoadError)
            {
                mWidth = 0;
            }

            return mWidth;
        }

        public int GetHeight()
        {
            ResetAccessTime();
            if (mHeight != -1)
            {
                return mHeight;
            }

            if (mTexture == null)
            {
                GetDimensions();
            }

            if (mLoadError)
            {
                mHeight = 0;
            }

            return mHeight;
        }

        public Texture2D GetTexture()
        {
            ResetAccessTime();
            if (mTexture == null)
            {
                LoadTexture();
            }

            return mTexture;
        }

        public void GetDimensions()
        {
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
                    var img = System.Drawing.Image.FromStream(fileStream, false, false);
                    if (img == null)
                    {
                        Log.Error($"Failed to load texture due to unknown error: {relativePath}");

                        return;
                    }

                    mWidth = img.Width;
                    mHeight = img.Height;
                    mLoadError = false;
                }
                catch (Exception exception)
                {
                    Log.Error(
                        exception,
                        $"Failed to load texture ({FileSystemHelper.FormatSize(fileStream.Length)}): {relativePath}"
                    );
                }
            }
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
