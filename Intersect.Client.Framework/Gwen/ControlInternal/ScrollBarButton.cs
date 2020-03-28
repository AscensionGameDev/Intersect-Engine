using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal
{

    /// <summary>
    ///     Scrollbar button.
    /// </summary>
    public class ScrollBarButton : Button
    {

        private Pos mDirection;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ScrollBarButton" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ScrollBarButton(Base parent) : base(parent)
        {
            SetDirectionUp();
        }

        public virtual void SetDirectionUp()
        {
            mDirection = Pos.Top;
        }

        public virtual void SetDirectionDown()
        {
            mDirection = Pos.Bottom;
        }

        public virtual void SetDirectionLeft()
        {
            mDirection = Pos.Left;
        }

        public virtual void SetDirectionRight()
        {
            mDirection = Pos.Right;
        }

        public virtual Pos GetDirection()
        {
            return mDirection;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.DrawScrollButton(this, mDirection, IsDepressed, IsHovered, IsDisabled);
        }

    }

}
