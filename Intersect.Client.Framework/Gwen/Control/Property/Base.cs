using System;

namespace Intersect.Client.Framework.Gwen.Control.Property
{

    /// <summary>
    ///     Base control for property entry.
    /// </summary>
    public class Base : Control.Base
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="Base" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Base(Control.Base parent) : base(parent)
        {
            Height = 17;
        }

        /// <summary>
        ///     Property value (todo: always string, which is ugly. do something about it).
        /// </summary>
        public virtual string Value
        {
            get => null;
            set => SetValue(value, false);
        }

        /// <summary>
        ///     Indicates whether the property value is being edited.
        /// </summary>
        public virtual bool IsEditing => false;

        /// <summary>
        ///     Invoked when the property value has been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> ValueChanged;

        protected virtual void DoChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged.Invoke(this, EventArgs.Empty);
            }
        }

        protected virtual void OnValueChanged(Control.Base control, EventArgs args)
        {
            DoChanged();
        }

        /// <summary>
        ///     Sets the property value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <param name="fireEvents">Determines whether to fire "value changed" event.</param>
        public virtual void SetValue(string value, bool fireEvents = false)
        {
        }

    }

}
