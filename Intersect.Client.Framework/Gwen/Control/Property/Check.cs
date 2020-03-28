namespace Intersect.Client.Framework.Gwen.Control.Property
{

    /// <summary>
    ///     Checkable property.
    /// </summary>
    public class Check : Base
    {

        protected readonly Control.CheckBox mCheckBox;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Check" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Check(Control.Base parent) : base(parent)
        {
            mCheckBox = new Control.CheckBox(this);
            mCheckBox.ShouldDrawBackground = false;
            mCheckBox.CheckChanged += OnValueChanged;
            mCheckBox.IsTabable = true;
            mCheckBox.KeyboardInputEnabled = true;
            mCheckBox.SetPosition(2, 1);

            Height = 18;
        }

        /// <summary>
        ///     Property value.
        /// </summary>
        public override string Value
        {
            get => mCheckBox.IsChecked ? "1" : "0";
            set => base.Value = value;
        }

        /// <summary>
        ///     Indicates whether the property value is being edited.
        /// </summary>
        public override bool IsEditing => mCheckBox.HasFocus;

        /// <summary>
        ///     Indicates whether the control is hovered by mouse pointer.
        /// </summary>
        public override bool IsHovered => base.IsHovered || mCheckBox.IsHovered;

        /// <summary>
        ///     Sets the property value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <param name="fireEvents">Determines whether to fire "value changed" event.</param>
        public override void SetValue(string value, bool fireEvents = false)
        {
            if (value == "1" || value.ToLower() == "true" || value.ToLower() == "yes")
            {
                mCheckBox.IsChecked = true;
            }
            else
            {
                mCheckBox.IsChecked = false;
            }
        }

    }

}
