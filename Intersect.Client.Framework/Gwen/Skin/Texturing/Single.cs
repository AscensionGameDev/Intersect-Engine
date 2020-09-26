using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;

namespace Intersect.Client.Framework.Gwen.Skin.Texturing
{

    /// <summary>
    ///     Single textured element.
    /// </summary>
    public struct Single
    {

        private readonly ITexture mGameTexture;

        private readonly float[] mUv;

        private readonly int mWidth;

        private readonly int mHeight;

        public Single(ITexture gameTexture, float x, float y, float w, float h)
        {
            mGameTexture = gameTexture;
            float texw = 1;
            float texh = 1;
            if (mGameTexture != null)
            {
                texw = mGameTexture.Width;
                texh = mGameTexture.Height;
            }

            mUv = new float[4];
            mUv[0] = x / texw;
            mUv[1] = y / texh;
            mUv[2] = (x + w) / texw;
            mUv[3] = (y + h) / texh;

            mWidth = (int) w;
            mHeight = (int) h;
        }

        // can't have this as default param
        public void Draw(Renderer.Base render, Rectangle r)
        {
            Draw(render, r, Color.White);
        }

        public void Draw(Renderer.Base render, Rectangle r, Color col)
        {
            if (mGameTexture == null)
            {
                return;
            }

            render.DrawColor = col;
            render.DrawTexturedRect(mGameTexture, r, col, mUv[0], mUv[1], mUv[2], mUv[3]);
        }

        public void DrawCenter(Renderer.Base render, Rectangle r)
        {
            if (mGameTexture == null)
            {
                return;
            }

            DrawCenter(render, r, Color.White);
        }

        public void DrawCenter(Renderer.Base render, Rectangle r, Color col)
        {
            r.X += (int) ((r.Width - mWidth) * 0.5);
            r.Y += (int) ((r.Height - mHeight) * 0.5);
            r.Width = mWidth;
            r.Height = mHeight;

            Draw(render, r, col);
        }

    }

}
