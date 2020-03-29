using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal
{

    /// <summary>
    ///     Window close button.
    /// </summary>
    public class CloseButton : Button
    {

        private readonly WindowControl mWindow;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CloseButton" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        /// <param name="owner">Window that owns this button.</param>
        public CloseButton(Base parent, WindowControl owner, string name = "") : base(parent, name)
        {
            mWindow = owner;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.DrawWindowCloseButton(this, IsDepressed && IsHovered, IsHovered && ShouldDrawHover, !mWindow.IsOnTop);
        }

    }

}
