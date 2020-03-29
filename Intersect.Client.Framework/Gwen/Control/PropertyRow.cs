using System;

using Intersect.Client.Framework.Gwen.ControlInternal;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Single property row.
    /// </summary>
    public class PropertyRow : Base
    {

        private readonly Label mLabel;

        private readonly Property.Base mProperty;

        private bool mLastEditing;

        private bool mLastHover;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyRow" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        /// <param name="prop">Property control associated with this row.</param>
        public PropertyRow(Base parent, Property.Base prop) : base(parent)
        {
            var label = new PropertyRowLabel(this);
            label.Dock = Pos.Left;
            label.Alignment = Pos.Left | Pos.Top;
            label.Margin = new Margin(2, 2, 0, 0);
            mLabel = label;

            mProperty = prop;
            mProperty.Parent = this;
            mProperty.Dock = Pos.Fill;
            mProperty.ValueChanged += OnValueChanged;
        }

        /// <summary>
        ///     Indicates whether the property value is being edited.
        /// </summary>
        public bool IsEditing => mProperty != null && mProperty.IsEditing;

        /// <summary>
        ///     Property value.
        /// </summary>
        public string Value
        {
            get => mProperty.Value;
            set => mProperty.Value = value;
        }

        /// <summary>
        ///     Indicates whether the control is hovered by mouse pointer.
        /// </summary>
        public override bool IsHovered => base.IsHovered || mProperty != null && mProperty.IsHovered;

        /// <summary>
        ///     Property name.
        /// </summary>
        public string Label
        {
            get => mLabel.Text;
            set => mLabel.Text = value;
        }

        /// <summary>
        ///     Invoked when the property value has changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> ValueChanged;

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            /* SORRY */
            if (IsEditing != mLastEditing)
            {
                OnEditingChanged();
                mLastEditing = IsEditing;
            }

            if (IsHovered != mLastHover)
            {
                OnHoverChanged();
                mLastHover = IsHovered;
            }
            /* SORRY */

            skin.DrawPropertyRow(this, mLabel.Right, IsEditing, IsHovered | mProperty.IsHovered);
        }

        /// <summary>
        ///     Lays out the control's interior according to alignment, padding, dock etc.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Layout(Skin.Base skin)
        {
            var parent = Parent as Properties;
            if (null == parent)
            {
                return;
            }

            mLabel.Width = parent.SplitWidth;

            if (mProperty != null)
            {
                Height = mProperty.Height;
            }
        }

        protected virtual void OnValueChanged(Base control, EventArgs args)
        {
            if (ValueChanged != null)
            {
                ValueChanged.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnEditingChanged()
        {
            mLabel.Redraw();
        }

        private void OnHoverChanged()
        {
            mLabel.Redraw();
        }

    }

}
