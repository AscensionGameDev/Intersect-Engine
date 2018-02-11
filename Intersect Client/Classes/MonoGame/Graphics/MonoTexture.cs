using System.IO;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.General;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect_Client_MonoGame.Classes.SFML.Graphics
{
    public class MonoTexture : GameTexture
    {
        private GraphicsDevice mGraphicsDevice;
        private int mHeight = -1;
        private long mLastAccessTime;
        private bool mLoadError;
        private string mName = "";
        private string mPath = "";
        private Texture2D mTex;
        private int mWidth = -1;

        public MonoTexture(GraphicsDevice graphicsDevice, string filename)
        {
            mGraphicsDevice = graphicsDevice;
            mPath = filename;
            mName = Path.GetFileName(filename);
        }

        public void LoadTexture()
        {
            mLoadError = true;
            if (File.Exists(mPath))
            {
                using (var fileStream = new FileStream(mPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    mTex = Texture2D.FromStream(mGraphicsDevice, fileStream);
                    if (mTex != null)
                    {
                        mWidth = mTex.Width;
                        mHeight = mTex.Height;
                        mLoadError = false;
                    }
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
            if (mWidth == -1)
            {
                if (mTex == null) LoadTexture();
                if (mLoadError)
                {
                    mWidth = 0;
                }
            }
            return mWidth;
        }

        public override int GetHeight()
        {
            ResetAccessTime();
            if (mHeight == -1)
            {
                if (mTex == null) LoadTexture();
                if (mLoadError)
                {
                    mHeight = 0;
                }
            }
            return mHeight;
        }

        public override object GetTexture()
        {
            ResetAccessTime();
            if (mTex == null) LoadTexture();
            return mTex;
        }

        public override Color GetPixel(int x1, int y1)
        {
            if (mTex == null) LoadTexture();
            if (mLoadError)
            {
                return Color.White;
            }
            else
            {
                Microsoft.Xna.Framework.Color[] pixel = new Microsoft.Xna.Framework.Color[1];
                mTex.GetData(0, new Microsoft.Xna.Framework.Rectangle(x1, y1, 1, 1), pixel, 0, 1);
                return new Color(pixel[0].A, pixel[0].R, pixel[0].G, pixel[0].B);
            }
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