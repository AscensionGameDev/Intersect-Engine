using System.IO;

using Intersect.Editor.Core;
using Intersect.Editor.General;

using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Editor.Content
{

    public class Texture
    {

        private int mHeight = -1;

        private long mLastAccessTime;

        private bool mLoadError;

        private string mPath = "";

        private Texture2D mTex;

        private int mWidth = -1;

        public Texture(string path)
        {
            mPath = path;
            GameContentManager.AllTextures.Add(this);
        }

        public void LoadTexture()
        {
            mLoadError = true;
            if (File.Exists(mPath))
            {
                using (var fileStream = new FileStream(mPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    mTex = Texture2D.FromStream(Graphics.GetGraphicsDevice(), fileStream);
                    if (mTex != null)
                    {
                        mWidth = mTex.Width;
                        mHeight = mTex.Height;
                        mLoadError = false;
                    }
                }
            }
        }

        public string GetPath()
        {
            return mPath;
        }

        public void ResetAccessTime()
        {
            mLastAccessTime = Globals.System.GetTimeMs() + 15000;
        }

        public int GetWidth()
        {
            ResetAccessTime();
            if (mWidth == -1)
            {
                if (mTex == null)
                {
                    LoadTexture();
                }

                if (mLoadError)
                {
                    mWidth = 0;
                }
            }

            return mWidth;
        }

        public int GetHeight()
        {
            ResetAccessTime();
            if (mHeight == -1)
            {
                if (mTex == null)
                {
                    LoadTexture();
                }

                if (mLoadError)
                {
                    mHeight = 0;
                }
            }

            return mHeight;
        }

        public Texture2D GetTexture()
        {
            ResetAccessTime();
            if (mTex == null)
            {
                LoadTexture();
            }

            return mTex;
        }

        public void Update()
        {
            if (mTex != null)
            {
                if (mLastAccessTime < Globals.System.GetTimeMs())
                {
                    mTex.Dispose();
                    mTex = null;
                }
            }
        }

    }

}
