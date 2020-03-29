using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;

namespace Intersect.Client.Framework.Gwen.Skin.Texturing
{

    /// <summary>
    ///     Single textured element.
    /// </summary>
    public struct Single
    {

        private readonly GameTexture mTexture;

        private readonly float[] mUv;

        private readonly int mWidth;

        private readonly int mHeight;

        public Single(GameTexture texture, float x, float y, float w, float h)
        {
            mTexture = texture;
            float texw = 1;
            float texh = 1;
            if (mTexture != null)
            {
                texw = mTexture.GetWidth();
                texh = mTexture.GetHeight();
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
            if (mTexture == null)
            {
                return;
            }

            render.DrawColor = col;
            render.DrawTexturedRect(mTexture, r, col, mUv[0], mUv[1], mUv[2], mUv[3]);
        }

        public void DrawCenter(Renderer.Base render, Rectangle r)
        {
            if (mTexture == null)
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
