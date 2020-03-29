using Intersect.Client.Framework.Gwen.DragDrop;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Titlebar for DockedTabControl.
    /// </summary>
    public class TabTitleBar : Label
    {

        public TabTitleBar(Base parent) : base(parent)
        {
            AutoSizeToContents = false;
            MouseInputEnabled = true;
            TextPadding = new Padding(5, 2, 5, 2);
            Padding = new Padding(1, 2, 1, 2);

            DragAndDrop_SetPackage(true, "TabWindowMove");
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.DrawTabTitleBar(this);
        }

        public override void DragAndDrop_StartDragging(Package package, int x, int y)
        {
            DragAndDrop.SourceControl = Parent;
            DragAndDrop.SourceControl.DragAndDrop_StartDragging(package, x, y);
        }

        public void UpdateFromTab(TabButton button)
        {
            Text = button.Text;
            SizeToContents();
        }

    }

}
