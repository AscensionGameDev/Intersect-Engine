using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
#if DEBUG || DIAGNOSTIC
#endif

using System;

namespace Intersect.Client.Framework.Gwen.Renderer
{

    /// <summary>
    ///     Base renderer.
    /// </summary>
    public class Base : IDisposable
    {

        private Rectangle mClipRegion;

        //public Random rnd;
        private Point mRenderOffset;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Base" /> class.
        /// </summary>
        protected Base()
        {
            //rnd = new Random();
            mRenderOffset = Point.Empty;
            Scale = 1.0f;
            if (Ctt != null)
            {
                Ctt.Initialize();
            }
        }

        //protected ICacheToTexture m_RTT;

        public float Scale { get; set; }

        /// <summary>
        ///     Gets or sets the current drawing color.
        /// </summary>
        public virtual Color DrawColor { get; set; }

        /// <summary>
        ///     Rendering offset. No need to touch it usually.
        /// </summary>
        public Point RenderOffset
        {
            get => mRenderOffset;
            set => mRenderOffset = value;
        }

        /// <summary>
        ///     Clipping rectangle.
        /// </summary>
        public Rectangle ClipRegion
        {
            get => mClipRegion;
            set => mClipRegion = value;
        }

        /// <summary>
        ///     Indicates whether the clip region is visible.
        /// </summary>
        public bool ClipRegionVisible
        {
            get
            {
                if (mClipRegion.Width <= 0 || mClipRegion.Height <= 0)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        ///     Cache to texture provider.
        /// </summary>
        public virtual ICacheToTexture Ctt => null;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            if (Ctt != null)
            {
                Ctt.ShutDown();
            }

            GC.SuppressFinalize(this);
        }

#if DIAGNOSTIC
        ~Base()
        {
            Log.Debug($"IDisposable object finalized: {GetType()}");
        }
#endif

        /// <summary>
        ///     Starts rendering.
        /// </summary>
        public virtual void Begin()
        {
        }

        /// <summary>
        ///     Stops rendering.
        /// </summary>
        public virtual void End()
        {
        }

        public virtual GameTexture GetWhiteTexture()
        {
            return null;
        }

        /// <summary>
        ///     Draws a line.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public virtual void DrawLine(int x, int y, int a, int b)
        {
        }

        /// <summary>
        ///     Draws a solid filled rectangle.
        /// </summary>
        /// <param name="rect"></param>
        public virtual void DrawFilledRect(Rectangle rect)
        {
        }

        /// <summary>
        ///     Starts clipping to the current clipping rectangle.
        /// </summary>
        public virtual void StartClip()
        {
        }

        /// <summary>
        ///     Stops clipping.
        /// </summary>
        public virtual void EndClip()
        {
        }

        /// <summary>
        ///     Loads the specified texture.
        /// </summary>
        /// <param name="t"></param>
        public virtual void LoadTexture(GameTexture t)
        {
        }

        /// <summary>
        ///     Frees the specified texture.
        /// </summary>
        /// <param name="t">Texture to free.</param>
        public virtual void FreeTexture(GameTexture t)
        {
        }

        /// <summary>
        ///     Draws textured rectangle.
        /// </summary>
        /// <param name="t">Texture to use.</param>
        /// <param name="targetRect">Rectangle bounds.</param>
        /// <param name="u1">Texture coordinate u1.</param>
        /// <param name="v1">Texture coordinate v1.</param>
        /// <param name="u2">Texture coordinate u2.</param>
        /// <param name="v2">Texture coordinate v2.</param>
        public virtual void DrawTexturedRect(
            GameTexture t,
            Rectangle targetRect,
            Color clr,
            float u1 = 0,
            float v1 = 0,
            float u2 = 1,
            float v2 = 1
        )
        {
        }

        /// <summary>
        ///     Draws "missing image" default texture.
        /// </summary>
        /// <param name="rect">Target rectangle.</param>
        public virtual void DrawMissingImage(Rectangle rect)
        {
            //DrawColor = Color.FromArgb(255, rnd.Next(0,255), rnd.Next(0,255), rnd.Next(0, 255));
            DrawColor = Color.Red;
            DrawFilledRect(rect);
        }

        /// <summary>
        ///     Loads the specified font.
        /// </summary>
        /// <param name="font">Font to load.</param>
        /// <returns>True if succeeded.</returns>
        public virtual bool LoadFont(GameFont font)
        {
            return false;
        }

        /// <summary>
        ///     Frees the specified font.
        /// </summary>
        /// <param name="font">Font to free.</param>
        public virtual void FreeFont(GameFont font)
        {
        }

        /// <summary>
        ///     Returns dimensions of the text using specified font.
        /// </summary>
        /// <param name="font">Font to use.</param>
        /// <param name="text">Text to measure.</param>
        /// <returns>Width and height of the rendered text.</returns>
        public virtual Point MeasureText(GameFont font, string text, float scale = 1f)
        {
            return Point.Empty;
        }

        /// <summary>
        ///     Renders text using specified font.
        /// </summary>
        /// <param name="font">Font to use.</param>
        /// <param name="position">Top-left corner of the text.</param>
        /// <param name="text">Text to render.</param>
        public virtual void RenderText(GameFont font, Point position, string text, float scale = 1f)
        {
        }

        //
        // No need to implement these functions in your derived class, but if 
        // you can do them faster than the default implementation it's a good idea to.
        //

        /// <summary>
        ///     Draws a lined rectangle. Used for keyboard focus overlay.
        /// </summary>
        /// <param name="rect">Target rectangle.</param>
        public virtual void DrawLinedRect(Rectangle rect)
        {
            DrawFilledRect(new Rectangle(rect.X, rect.Y, rect.Width, 1));
            DrawFilledRect(new Rectangle(rect.X, rect.Y + rect.Height - 1, rect.Width, 1));

            DrawFilledRect(new Rectangle(rect.X, rect.Y, 1, rect.Height));
            DrawFilledRect(new Rectangle(rect.X + rect.Width - 1, rect.Y, 1, rect.Height));
        }

        /// <summary>
        ///     Draws a single pixel. Very slow, do not use. :P
        /// </summary>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        public virtual void DrawPixel(int x, int y)
        {
            // [omeg] amazing ;)
            DrawFilledRect(new Rectangle(x, y, 1, 1));
        }

        /// <summary>
        ///     Gets pixel color of a specified texture. Slow.
        /// </summary>
        /// <param name="texture">Texture.</param>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        /// <returns>Pixel color.</returns>
        public virtual Color PixelColor(GameTexture texture, uint x, uint y)
        {
            return PixelColor(texture, x, y, Color.White);
        }

        /// <summary>
        ///     Gets pixel color of a specified texture, returning default if otherwise failed. Slow.
        /// </summary>
        /// <param name="texture">Texture.</param>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        /// <param name="defaultColor">Color to return on failure.</param>
        /// <returns>Pixel color.</returns>
        public virtual Color PixelColor(GameTexture texture, uint x, uint y, Color defaultColor)
        {
            return defaultColor;
        }

        /// <summary>
        ///     Draws a round-corner rectangle.
        /// </summary>
        /// <param name="rect">Target rectangle.</param>
        /// <param name="slight"></param>
        public virtual void DrawShavedCornerRect(Rectangle rect, bool slight = false)
        {
            // Draw INSIDE the w/h.
            rect.Width -= 1;
            rect.Height -= 1;

            if (slight)
            {
                DrawFilledRect(new Rectangle(rect.X + 1, rect.Y, rect.Width - 1, 1));
                DrawFilledRect(new Rectangle(rect.X + 1, rect.Y + rect.Height, rect.Width - 1, 1));

                DrawFilledRect(new Rectangle(rect.X, rect.Y + 1, 1, rect.Height - 1));
                DrawFilledRect(new Rectangle(rect.X + rect.Width, rect.Y + 1, 1, rect.Height - 1));

                return;
            }

            DrawPixel(rect.X + 1, rect.Y + 1);
            DrawPixel(rect.X + rect.Width - 1, rect.Y + 1);

            DrawPixel(rect.X + 1, rect.Y + rect.Height - 1);
            DrawPixel(rect.X + rect.Width - 1, rect.Y + rect.Height - 1);

            DrawFilledRect(new Rectangle(rect.X + 2, rect.Y, rect.Width - 3, 1));
            DrawFilledRect(new Rectangle(rect.X + 2, rect.Y + rect.Height, rect.Width - 3, 1));

            DrawFilledRect(new Rectangle(rect.X, rect.Y + 2, 1, rect.Height - 3));
            DrawFilledRect(new Rectangle(rect.X + rect.Width, rect.Y + 2, 1, rect.Height - 3));
        }

        private int TranslateX(int x)
        {
            var x1 = x + mRenderOffset.X;

            return Util.Ceil(x1 * Scale);
        }

        private int TranslateY(int y)
        {
            var y1 = y + mRenderOffset.Y;

            return Util.Ceil(y1 * Scale);
        }

        /// <summary>
        ///     Translates a panel's local drawing coordinate into view space, taking offsets into account.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Translate(ref int x, ref int y)
        {
            x += mRenderOffset.X;
            y += mRenderOffset.Y;

            x = Util.Ceil(x * Scale);
            y = Util.Ceil(y * Scale);
        }

        /// <summary>
        ///     Translates a panel's local drawing coordinate into view space, taking offsets into account.
        /// </summary>
        public Point Translate(Point p)
        {
            var x = p.X;
            var y = p.Y;
            Translate(ref x, ref y);

            return new Point(x, y);
        }

        /// <summary>
        ///     Translates a panel's local drawing coordinate into view space, taking offsets into account.
        /// </summary>
        public Rectangle Translate(Rectangle rect)
        {
            return new Rectangle(
                TranslateX(rect.X), TranslateY(rect.Y), Util.Ceil(rect.Width * Scale), Util.Ceil(rect.Height * Scale)
            );
        }

        /// <summary>
        ///     Adds a point to the render offset.
        /// </summary>
        /// <param name="offset">Point to add.</param>
        public void AddRenderOffset(Rectangle offset)
        {
            mRenderOffset = new Point(mRenderOffset.X + offset.X, mRenderOffset.Y + offset.Y);
        }

        /// <summary>
        ///     Adds a rectangle to the clipping region.
        /// </summary>
        /// <param name="rect">Rectangle to add.</param>
        public void AddClipRegion(Rectangle rect)
        {
            rect.X = mRenderOffset.X;
            rect.Y = mRenderOffset.Y;

            var r = rect;
            if (rect.X < mClipRegion.X)
            {
                r.Width -= mClipRegion.X - r.X;
                r.X = mClipRegion.X;
            }

            if (rect.Y < mClipRegion.Y)
            {
                r.Height -= mClipRegion.Y - r.Y;
                r.Y = mClipRegion.Y;
            }

            if (rect.Right > mClipRegion.Right)
            {
                r.Width = mClipRegion.Right - r.X;
            }

            if (rect.Bottom > mClipRegion.Bottom)
            {
                r.Height = mClipRegion.Bottom - r.Y;
            }

            mClipRegion = r;
        }

    }

}
