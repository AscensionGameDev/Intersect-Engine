namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Menu strip.
    /// </summary>
    public class MenuStrip : Menu
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuStrip" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public MenuStrip(Base parent) : base(parent)
        {
            SetBounds(0, 0, 200, 22);
            Dock = Pos.Top;
            mInnerPanel.Padding = new Padding(5, 0, 0, 0);
        }

        /// <summary>
        ///     Determines whether the menu should open on mouse hover.
        /// </summary>
        protected override bool ShouldHoverOpenMenu => IsMenuOpen();

        /// <summary>
        ///     Closes the current menu.
        /// </summary>
        public override void Close()
        {
        }

        /// <summary>
        ///     Renders under the actual control (shadows etc).
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void RenderUnder(Skin.Base skin)
        {
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.DrawMenuStrip(this);
        }

        /// <summary>
        ///     Lays out the control's interior according to alignment, padding, dock etc.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Layout(Skin.Base skin)
        {
            //TODO: We don't want to do vertical sizing the same as Menu, do nothing for now
        }

        /// <summary>
        ///     Add item handler.
        /// </summary>
        /// <param name="item">Item added.</param>
        protected override void OnAddItem(MenuItem item)
        {
            item.Dock = Pos.Left;
            item.TextPadding = new Padding(5, 0, 5, 0);
            item.Padding = new Padding(10, 0, 10, 0);
            item.SizeToContents();
            item.IsOnStrip = true;
            item.HoverEnter += OnHoverItem;
        }

    }

}
