using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal
{

    /// <summary>
    ///     Color square.
    /// </summary>
    public class ColorDisplay : Base
    {

        private Color mColor;

        //private bool m_DrawCheckers;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorDisplay" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ColorDisplay(Base parent) : base(parent)
        {
            SetSize(32, 32);
            mColor = Color.FromArgb(255, 255, 0, 0);

            //m_DrawCheckers = true;
        }

        /// <summary>
        ///     Current color.
        /// </summary>
        public Color Color
        {
            get => mColor;
            set => mColor = value;
        }

        //public bool DrawCheckers { get { return m_DrawCheckers; } set { m_DrawCheckers = value; } }
        public int R
        {
            get => mColor.R;
            set => mColor = Color.FromArgb(mColor.A, value, mColor.G, mColor.B);
        }

        public int G
        {
            get => mColor.G;
            set => mColor = Color.FromArgb(mColor.A, mColor.R, value, mColor.B);
        }

        public int B
        {
            get => mColor.B;
            set => mColor = Color.FromArgb(mColor.A, mColor.R, mColor.G, value);
        }

        public int A
        {
            get => mColor.A;
            set => mColor = Color.FromArgb(value, mColor.R, mColor.G, mColor.B);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.DrawColorDisplay(this, mColor);
        }

    }

}
