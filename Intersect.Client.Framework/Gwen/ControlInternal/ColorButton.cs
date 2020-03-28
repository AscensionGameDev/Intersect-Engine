using System;

using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal
{

    /// <summary>
    ///     Property button.
    /// </summary>
    public class ColorButton : Button
    {

        private Color mColor;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorButton" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ColorButton(Base parent) : base(parent)
        {
            mColor = Color.Black;
            Text = String.Empty;
        }

        /// <summary>
        ///     Current color value.
        /// </summary>
        public Color Color
        {
            get => mColor;
            set => mColor = value;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.Renderer.DrawColor = mColor;
            skin.Renderer.DrawFilledRect(RenderBounds);
        }

    }

}
