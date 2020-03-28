using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;

namespace Intersect.Client.Framework.Gwen.Skin.Texturing
{

    public struct SubRect
    {

        public float[] Uv;

    }

    /// <summary>
    ///     3x3 texture grid.
    /// </summary>
    public struct Bordered
    {

        private GameTexture mTexture;

        private readonly SubRect[] mRects;

        private Margin mMargin;

        private float mWidth;

        private float mHeight;

        public Bordered(
            GameTexture texture,
            float x,
            float y,
            float w,
            float h,
            Margin inMargin,
            float drawMarginScale = 1.0f
        ) : this()
        {
            mRects = new SubRect[9];
            for (var i = 0; i < mRects.Length; i++)
            {
                mRects[i].Uv = new float[4];
            }

            Init(texture, x, y, w, h, inMargin, drawMarginScale);
        }

        void DrawRect(Renderer.Base render, int i, int x, int y, int w, int h, Color clr)
        {
            render.DrawTexturedRect(
                mTexture, new Rectangle(x, y, w, h), clr, mRects[i].Uv[0], mRects[i].Uv[1], mRects[i].Uv[2],
                mRects[i].Uv[3]
            );
        }

        void SetRect(int num, float x, float y, float w, float h)
        {
            if (mTexture == null)
            {
                return;
            }

            float texw = mTexture.GetWidth();
            float texh = mTexture.GetHeight();

            //x -= 1.0f;
            //y -= 1.0f;

            mRects[num].Uv[0] = x / texw;
            mRects[num].Uv[1] = y / texh;

            mRects[num].Uv[2] = (x + w) / texw;
            mRects[num].Uv[3] = (y + h) / texh;

            //	rects[num].uv[0] += 1.0f / m_Texture->width;
            //	rects[num].uv[1] += 1.0f / m_Texture->width;
        }

        private void Init(
            GameTexture texture,
            float x,
            float y,
            float w,
            float h,
            Margin inMargin,
            float drawMarginScale = 1.0f
        )
        {
            mTexture = texture;

            mMargin = inMargin;

            SetRect(0, x, y, mMargin.Left, mMargin.Top);
            SetRect(1, x + mMargin.Left, y, w - mMargin.Left - mMargin.Right, mMargin.Top);
            SetRect(2, x + w - mMargin.Right, y, mMargin.Right, mMargin.Top);

            SetRect(3, x, y + mMargin.Top, mMargin.Left, h - mMargin.Top - mMargin.Bottom);
            SetRect(
                4, x + mMargin.Left, y + mMargin.Top, w - mMargin.Left - mMargin.Right, h - mMargin.Top - mMargin.Bottom
            );

            SetRect(5, x + w - mMargin.Right, y + mMargin.Top, mMargin.Right, h - mMargin.Top - mMargin.Bottom - 1);

            SetRect(6, x, y + h - mMargin.Bottom, mMargin.Left, mMargin.Bottom);
            SetRect(7, x + mMargin.Left, y + h - mMargin.Bottom, w - mMargin.Left - mMargin.Right, mMargin.Bottom);
            SetRect(8, x + w - mMargin.Right, y + h - mMargin.Bottom, mMargin.Right, mMargin.Bottom);

            mMargin.Left = (int) (mMargin.Left * drawMarginScale);
            mMargin.Right = (int) (mMargin.Right * drawMarginScale);
            mMargin.Top = (int) (mMargin.Top * drawMarginScale);
            mMargin.Bottom = (int) (mMargin.Bottom * drawMarginScale);

            mWidth = w - x;
            mHeight = h - y;
        }

        // can't have this as default param
        /*public void Draw(Renderer.Base render, Rectangle r, Color col )
        {
            Draw(render, r, Color.Red);
        }*/

        public void Draw(Renderer.Base render, Rectangle r, Color col)
        {
            if (mTexture == null)
            {
                return;
            }

            render.DrawColor = col;

            if (r.Width < mWidth && r.Height < mHeight)
            {
                render.DrawTexturedRect(
                    mTexture, r, col, mRects[0].Uv[0], mRects[0].Uv[1], mRects[8].Uv[2], mRects[8].Uv[3]
                );

                return;
            }

            DrawRect(render, 0, r.X, r.Y, mMargin.Left, mMargin.Top, col);
            DrawRect(render, 1, r.X + mMargin.Left, r.Y, r.Width - mMargin.Left - mMargin.Right, mMargin.Top, col);
            DrawRect(render, 2, r.X + r.Width - mMargin.Right, r.Y, mMargin.Right, mMargin.Top, col);

            DrawRect(render, 3, r.X, r.Y + mMargin.Top, mMargin.Left, r.Height - mMargin.Top - mMargin.Bottom, col);
            DrawRect(
                render, 4, r.X + mMargin.Left, r.Y + mMargin.Top, r.Width - mMargin.Left - mMargin.Right,
                r.Height - mMargin.Top - mMargin.Bottom, col
            );

            DrawRect(
                render, 5, r.X + r.Width - mMargin.Right, r.Y + mMargin.Top, mMargin.Right,
                r.Height - mMargin.Top - mMargin.Bottom, col
            );

            DrawRect(render, 6, r.X, r.Y + r.Height - mMargin.Bottom, mMargin.Left, mMargin.Bottom, col);
            DrawRect(
                render, 7, r.X + mMargin.Left, r.Y + r.Height - mMargin.Bottom, r.Width - mMargin.Left - mMargin.Right,
                mMargin.Bottom, col
            );

            DrawRect(
                render, 8, r.X + r.Width - mMargin.Right, r.Y + r.Height - mMargin.Bottom, mMargin.Right,
                mMargin.Bottom, col
            );
        }

    }

}
