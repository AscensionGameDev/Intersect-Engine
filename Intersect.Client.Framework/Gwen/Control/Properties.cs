using System;

using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.ControlInternal;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Properties table.
    /// </summary>
    public class Properties : Base
    {

        private readonly SplitterBar mSplitterBar;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Properties" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Properties(Base parent) : base(parent)
        {
            mSplitterBar = new SplitterBar(this);
            mSplitterBar.SetPosition(80, 0);
            mSplitterBar.Cursor = Cursors.SizeWe;
            mSplitterBar.Dragged += OnSplitterMoved;
            mSplitterBar.ShouldDrawBackground = false;
        }

        /// <summary>
        ///     Returns the width of the first column (property names).
        /// </summary>
        public int SplitWidth

            // todo: rename?
            => mSplitterBar.X;

        /// <summary>
        ///     Invoked when a property value has been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> ValueChanged;

        /// <summary>
        ///     Function invoked after layout.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void PostLayout(Skin.Base skin)
        {
            mSplitterBar.Height = 0;

            if (SizeToChildren(false, true))
            {
                InvalidateParent();
            }

            mSplitterBar.SetSize(3, Height);
        }

        /// <summary>
        ///     Handles the splitter moved event.
        /// </summary>
        /// <param name="control">Event source.</param>
        protected virtual void OnSplitterMoved(Base control, EventArgs args)
        {
            InvalidateChildren();
        }

        /// <summary>
        ///     Adds a new text property row.
        /// </summary>
        /// <param name="label">Property name.</param>
        /// <param name="value">Initial value.</param>
        /// <returns>Newly created row.</returns>
        public PropertyRow Add(string label, string value = "")
        {
            return Add(label, new Property.Text(this), value);
        }

        /// <summary>
        ///     Adds a new property row.
        /// </summary>
        /// <param name="label">Property name.</param>
        /// <param name="prop">Property control.</param>
        /// <param name="value">Initial value.</param>
        /// <returns>Newly created row.</returns>
        public PropertyRow Add(string label, Property.Base prop, string value = "")
        {
            var row = new PropertyRow(this, prop);
            row.Dock = Pos.Top;
            row.Label = label;
            row.ValueChanged += OnRowValueChanged;

            prop.SetValue(value, true);

            mSplitterBar.BringToFront();

            return row;
        }

        private void OnRowValueChanged(Base control, EventArgs args)
        {
            if (ValueChanged != null)
            {
                ValueChanged.Invoke(control, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Deletes all rows.
        /// </summary>
        public void DeleteAll()
        {
            mInnerPanel.DeleteAllChildren();
        }

    }

}
