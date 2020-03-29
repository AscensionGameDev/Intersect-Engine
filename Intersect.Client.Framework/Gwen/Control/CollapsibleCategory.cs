using System;

using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.ControlInternal;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     CollapsibleCategory control. Used in CollapsibleList.
    /// </summary>
    public class CollapsibleCategory : Base
    {

        private readonly Button mHeaderButton;

        private readonly CollapsibleList mList;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CollapsibleCategory" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public CollapsibleCategory(CollapsibleList parent) : base(parent)
        {
            mHeaderButton = new CategoryHeaderButton(this);
            mHeaderButton.Text = "Category Title"; // [omeg] todo: i18n
            mHeaderButton.Dock = Pos.Top;
            mHeaderButton.Height = 20;
            mHeaderButton.Toggled += OnHeaderToggle;

            mList = parent;

            Padding = new Padding(1, 0, 1, 5);
            SetSize(512, 512);
        }

        /// <summary>
        ///     Header text.
        /// </summary>
        public string Text
        {
            get => mHeaderButton.Text;
            set => mHeaderButton.Text = value;
        }

        /// <summary>
        ///     Determines whether the category is collapsed (closed).
        /// </summary>
        public bool IsCollapsed
        {
            get => mHeaderButton.ToggleState;
            set => mHeaderButton.ToggleState = value;
        }

        /// <summary>
        ///     Invoked when an entry has been selected.
        /// </summary>
        public event GwenEventHandler<ItemSelectedEventArgs> Selected;

        /// <summary>
        ///     Invoked when the category collapsed state has been changed (header button has been pressed).
        /// </summary>
        public event GwenEventHandler<EventArgs> Collapsed;

        /// <summary>
        ///     Gets the selected entry.
        /// </summary>
        public Button GetSelectedButton()
        {
            foreach (var child in Children)
            {
                var button = child as CategoryButton;
                if (button == null)
                {
                    continue;
                }

                if (button.ToggleState)
                {
                    return button;
                }
            }

            return null;
        }

        /// <summary>
        ///     Handler for header button toggle event.
        /// </summary>
        /// <param name="control">Source control.</param>
        protected virtual void OnHeaderToggle(Base control, EventArgs args)
        {
            if (Collapsed != null)
            {
                Collapsed.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Handler for Selected event.
        /// </summary>
        /// <param name="control">Event source.</param>
        protected virtual void OnSelected(Base control, EventArgs args)
        {
            var child = control as CategoryButton;
            if (child == null)
            {
                return;
            }

            if (mList != null)
            {
                mList.UnselectAll();
            }
            else
            {
                UnselectAll();
            }

            child.ToggleState = true;

            if (Selected != null)
            {
                Selected.Invoke(this, new ItemSelectedEventArgs(control));
            }
        }

        /// <summary>
        ///     Adds a new entry.
        /// </summary>
        /// <param name="name">Entry name (displayed).</param>
        /// <returns>Newly created control.</returns>
        public Button Add(string name)
        {
            var button = new CategoryButton(this);
            button.Text = name;
            button.Dock = Pos.Top;
            button.SizeToContents();
            button.SetSize(button.Width + 4, button.Height + 4);
            button.Padding = new Padding(5, 2, 2, 2);
            button.Clicked += OnSelected;

            return button;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.DrawCategoryInner(this, mHeaderButton.ToggleState);
            base.Render(skin);
        }

        /// <summary>
        ///     Unselects all entries.
        /// </summary>
        public void UnselectAll()
        {
            foreach (var child in Children)
            {
                var button = child as CategoryButton;
                if (button == null)
                {
                    continue;
                }

                button.ToggleState = false;
            }
        }

        /// <summary>
        ///     Function invoked after layout.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void PostLayout(Skin.Base skin)
        {
            if (IsCollapsed)
            {
                Height = mHeaderButton.Height;
            }
            else
            {
                SizeToChildren(false, true);
            }

            // alternate row coloring
            var b = true;
            foreach (var child in Children)
            {
                var button = child as CategoryButton;
                if (button == null)
                {
                    continue;
                }

                button.mAlt = b;
                button.UpdateColors();
                b = !b;
            }
        }

    }

}
