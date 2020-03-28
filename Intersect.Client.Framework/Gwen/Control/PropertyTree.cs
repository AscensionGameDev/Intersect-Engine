using Intersect.Client.Framework.Gwen.ControlInternal;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Property table/tree.
    /// </summary>
    public class PropertyTree : TreeControl
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyTree" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public PropertyTree(Base parent) : base(parent)
        {
        }

        /// <summary>
        ///     Adds a new properties node.
        /// </summary>
        /// <param name="label">Node label.</param>
        /// <returns>Newly created control</returns>
        public Properties Add(string label)
        {
            TreeNode node = new PropertyTreeNode(this);
            node.Text = label;
            node.Dock = Pos.Top;

            var props = new Properties(node);
            props.Dock = Pos.Top;

            return props;
        }

    }

}
