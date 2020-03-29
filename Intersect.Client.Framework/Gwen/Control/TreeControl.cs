using System;

using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Tree control.
    /// </summary>
    public class TreeControl : TreeNode
    {

        private readonly ScrollControl mScrollControl;

        private bool mMultiSelect;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TreeControl" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TreeControl(Base parent) : base(parent)
        {
            mTreeControl = this;

            RemoveChild(mToggleButton, true);
            mToggleButton = null;
            RemoveChild(mTitle, true);
            mTitle = null;
            RemoveChild(mInnerPanel, true);
            mInnerPanel = null;

            mMultiSelect = false;

            mScrollControl = new ScrollControl(this);
            mScrollControl.Dock = Pos.Fill;
            mScrollControl.EnableScroll(false, true);
            mScrollControl.AutoHideBars = true;
            mScrollControl.Margin = Margin.One;

            mInnerPanel = mScrollControl;

            mScrollControl.SetInnerSize(1000, 1000); // todo: why such arbitrary numbers?

            Dock = Pos.None;
        }

        /// <summary>
        ///     Determines if multiple nodes can be selected at the same time.
        /// </summary>
        public bool AllowMultiSelect
        {
            get => mMultiSelect;
            set => mMultiSelect = value;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            if (ShouldDrawBackground)
            {
                skin.DrawTreeControl(this);
            }
        }

        /// <summary>
        ///     Handler invoked when control children's bounds change.
        /// </summary>
        /// <param name="oldChildBounds"></param>
        /// <param name="child"></param>
        protected override void OnChildBoundsChanged(Rectangle oldChildBounds, Base child)
        {
            if (mScrollControl != null)
            {
                mScrollControl.UpdateScrollBars();
            }
        }

        /// <summary>
        ///     Removes all child nodes.
        /// </summary>
        public virtual void RemoveAll()
        {
            mScrollControl.DeleteAll();
        }

        /// <summary>
        ///     Handler for node added event.
        /// </summary>
        /// <param name="node">Node added.</param>
        public virtual void OnNodeAdded(TreeNode node)
        {
            node.LabelPressed += OnNodeSelected;
        }

        /// <summary>
        ///     Handler for node selected event.
        /// </summary>
        /// <param name="control">Node selected.</param>
        protected virtual void OnNodeSelected(Base control, EventArgs args)
        {
            if (!mMultiSelect /*|| InputHandler.InputHandler.IsKeyDown(Key.Control)*/)
            {
                UnselectAll();
            }
        }

    }

}
