using System;

using Intersect.Client.Framework.Gwen.ControlInternal;
using Intersect.Client.Framework.Gwen.Input;

namespace Intersect.Client.Framework.Gwen.Control.Property
{

    /// <summary>
    ///     Color property.
    /// </summary>
    public class Color : Text
    {

        protected readonly ColorButton mButton;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Color" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Color(Control.Base parent) : base(parent)
        {
            mButton = new ColorButton(mTextBox);
            mButton.Dock = Pos.Right;
            mButton.Width = 20;
            mButton.Margin = new Margin(1, 1, 1, 2);
            mButton.Clicked += OnButtonPressed;
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
        public override bool IsEditing => mTextBox == InputHandler.KeyboardFocus;

        /// <summary>
        ///     Color-select button press handler.
        /// </summary>
        /// <param name="control">Event source.</param>
        protected virtual void OnButtonPressed(Control.Base control, EventArgs args)
        {
            var menu = new Menu(GetCanvas());
            menu.SetSize(256, 180);
            menu.DeleteOnClose = true;
            menu.IconMarginDisabled = true;

            var picker = new HsvColorPicker(menu);
            picker.Dock = Pos.Fill;
            picker.SetSize(256, 128);

            var split = mTextBox.Text.Split(' ');

            picker.SetColor(GetColorFromText(), false, true);
            picker.ColorChanged += OnColorChanged;

            menu.Open(Pos.Right | Pos.Top);
        }

        /// <summary>
        ///     Color changed handler.
        /// </summary>
        /// <param name="control">Event source.</param>
        protected virtual void OnColorChanged(Control.Base control, EventArgs args)
        {
            var picker = control as HsvColorPicker;
            SetTextFromColor(picker.SelectedColor);
            DoChanged();
        }

        /// <summary>
        ///     Sets the property value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <param name="fireEvents">Determines whether to fire "value changed" event.</param>
        public override void SetValue(string value, bool fireEvents = false)
        {
            mTextBox.SetText(value, fireEvents);
        }

        private void SetTextFromColor(Intersect.Color color)
        {
            mTextBox.Text = String.Format("{0} {1} {2}", color.R, color.G, color.B);
        }

        private Intersect.Color GetColorFromText()
        {
            var split = mTextBox.Text.Split(' ');

            byte red = 0;
            byte green = 0;
            byte blue = 0;
            byte alpha = 255;

            if (split.Length > 0 && split[0].Length > 0)
            {
                Byte.TryParse(split[0], out red);
            }

            if (split.Length > 1 && split[1].Length > 0)
            {
                Byte.TryParse(split[1], out green);
            }

            if (split.Length > 2 && split[2].Length > 0)
            {
                Byte.TryParse(split[2], out blue);
            }

            return Intersect.Color.FromArgb(alpha, red, green, blue);
        }

        protected override void DoChanged()
        {
            base.DoChanged();
            mButton.Color = GetColorFromText();
        }

    }

}
