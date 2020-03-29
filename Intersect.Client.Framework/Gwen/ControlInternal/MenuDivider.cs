using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal
{

    /// <summary>
    ///     Divider menu item.
    /// </summary>
    public class MenuDivider : Base
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuDivider" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public MenuDivider(Base parent) : base(parent)
        {
            Height = 1;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.DrawMenuDivider(this);
        }

    }

}
