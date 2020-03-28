namespace Intersect.Client.Framework.Gwen.Control.Property
{

    /// <summary>
    ///     Text property.
    /// </summary>
    public class Text : Base
    {

        protected readonly TextBox mTextBox;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Text" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Text(Control.Base parent) : base(parent)
        {
            mTextBox = new TextBox(this);
            mTextBox.Dock = Pos.Fill;
            mTextBox.ShouldDrawBackground = false;
            mTextBox.TextChanged += OnValueChanged;
        }

        /// <summary>
        ///     Property value.
        /// </summary>
        public override string Value
        {
            get => mTextBox.Text;
            set => base.Value = value;
        }

        /// <summary>
        ///     Indicates whether the property value is being edited.
        /// </summary>
        public override bool IsEditing => mTextBox.HasFocus;

        /// <summary>
        ///     Indicates whether the control is hovered by mouse pointer.
        /// </summary>
        public override bool IsHovered => base.IsHovered | mTextBox.IsHovered;

        /// <summary>
        ///     Sets the property value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <param name="fireEvents">Determines whether to fire "value changed" event.</param>
        public override void SetValue(string value, bool fireEvents = false)
        {
            mTextBox.SetText(value, fireEvents);
        }

    }

}
