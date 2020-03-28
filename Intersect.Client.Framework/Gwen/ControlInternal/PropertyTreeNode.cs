using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal
{

    /// <summary>
    ///     Properties node.
    /// </summary>
    public class PropertyTreeNode : TreeNode
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyTreeNode" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public PropertyTreeNode(Base parent) : base(parent)
        {
            mTitle.TextColorOverride = Skin.Colors.Properties.Title;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.DrawPropertyTreeNode(this, mInnerPanel.X, mInnerPanel.Y);
        }

    }

}
