using System;

using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal
{

    /// <summary>
    ///     Displays text. Always sized to contents.
    /// </summary>
    public class Text : Base
    {

        private GameFont mFont;

        private float mScale = 1f;

        private bool mShadow = false;

        private string mString;

        /// <summary>
        ///     Text color.
        /// </summary>
        private Color mTextColor;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Text" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Text(Base parent, string name = "") : base(parent, name)
        {
            mFont = Skin.DefaultFont;
            mString = String.Empty;
            TextColor = Skin.Colors.Label.Default;
            MouseInputEnabled = false;
            TextColorOverride = Color.FromArgb(0, 255, 255, 255); // A==0, override disabled
        }

        /// <summary>
        ///     Font used to display the text.
        /// </summary>
        /// <remarks>
        ///     The font is not being disposed by this class.
        /// </remarks>
        public GameFont Font
        {
            get => mFont;
            set
            {
                mFont = value;
                SizeToContents();
            }
        }

        /// <summary>
        ///     Text to display.
        /// </summary>
        public string String
        {
            get => mString;
            set
            {
                mString = value;
                SizeToContents();
            }
        }

        public Color TextColor
        {
            get => mTextColor;

            set => mTextColor = value;
        }

        public bool DrawShadow { get; set; }

        /// <summary>
        ///     Determines whether the control should be automatically resized to fit the text.
        /// </summary>
        /// <summary>
        ///     Text length in characters.
        /// </summary>
        public int Length => String?.Length ?? 0;

        /// <summary>
        ///     Text color override - used by tooltips.
        /// </summary>
        public Color TextColorOverride { get; set; }

        /// <summary>
        ///     Text override - used to display different string.
        /// </summary>
        public string TextOverride { get; set; }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            if (Length == 0 || Font == null)
            {
                return;
            }

            if (TextColorOverride.A == 0)
            {
                skin.Renderer.DrawColor = TextColor;
            }
            else
            {
                skin.Renderer.DrawColor = TextColorOverride;
            }

            skin.Renderer.RenderText(Font, Point.Empty, TextOverride ?? String, mScale);

#if DEBUG_TEXT_MEASURE
            {
                Point lastPos = Point.Empty;

                for (int i = 0; i < m_String.Length + 1; i++)
                {
                    String sub = (TextOverride ?? String).Substring(0, i);
                    Point p = Skin.Renderer.MeasureText(Font, sub);

                    Rectangle rect = new Rectangle();
                    rect.Location = lastPos;
                    rect.Size = new Size(p.X - lastPos.X, p.Y);
                    skin.Renderer.DrawColor = Color.FromArgb(64, 0, 0, 0);
                    skin.Renderer.DrawLinedRect(rect);

                    lastPos = new Point(rect.Right, 0);
                }
            }
#endif
        }

        /// <summary>
        ///     Lays out the control's interior according to alignment, padding, dock etc.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Layout(Skin.Base skin)
        {
            SizeToContents();
            base.Layout(skin);
        }

        /// <summary>
        ///     Handler invoked when control's scale changes.
        /// </summary>
        protected override void OnScaleChanged()
        {
            Invalidate();
        }

        public void SetScale(float scale)
        {
            mScale = scale;
            OnScaleChanged();
        }

        public float GetScale()
        {
            return mScale;
        }

        /// <summary>
        ///     Sizes the control to its contents.
        /// </summary>
        public void SizeToContents()
        {
            if (String == null || Font == null)
            {
                return;
            }

            var p = new Point(1, 10);

            if (Length > 0)
            {
                p = Skin.Renderer.MeasureText(Font, TextOverride ?? String, mScale);
            }
            else
            {
                p.Y = Skin.Renderer.MeasureText(Font, "|", mScale).Y;
            }

            if (p.X == Width && p.Y == Height)
            {
                return;
            }

            SetSize(p.X, p.Y);
            Invalidate();
            InvalidateParent();
        }

        /// <summary>
        ///     Gets the coordinates of specified character in the text.
        /// </summary>
        /// <param name="index">Character index.</param>
        /// <returns>Character position in local coordinates.</returns>
        public Point GetCharacterPosition(int index)
        {
            if (Length == 0 || index == 0)
            {
                return new Point(0, 0);
            }

            var sub = (TextOverride ?? String).Substring(0, index);
            var p = Skin.Renderer.MeasureText(Font, sub);

            //if(p.Y >= Font.Size)
            //	p = new Point(p.X, p.Y - Font.Size);
            p.Y = 0;

            return p;
        }

        /// <summary>
        ///     Searches for a character closest to given point.
        /// </summary>
        /// <param name="p">Point.</param>
        /// <returns>Character index.</returns>
        public int GetClosestCharacter(Point p)
        {
            var distance = MAX_COORD;
            var c = 0;

            for (var i = 0; i < String.Length + 1; i++)
            {
                var cp = GetCharacterPosition(i);
                var dist = Math.Abs(cp.X - p.X) + Math.Abs(cp.Y - p.Y); // this isn't proper // [omeg] todo: sqrt

                if (dist > distance)
                {
                    continue;
                }

                distance = dist;
                c = i;
            }

            return c;
        }

    }

}
