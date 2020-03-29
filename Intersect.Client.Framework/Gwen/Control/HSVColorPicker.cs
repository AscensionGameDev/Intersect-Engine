using System;

using Intersect.Client.Framework.Gwen.ControlInternal;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     HSV color picker with "before" and "after" color boxes.
    /// </summary>
    public class HsvColorPicker : Base, IColorPicker
    {

        private readonly ColorDisplay mAfter;

        private readonly ColorDisplay mBefore;

        private readonly ColorSlider mColorSlider;

        private readonly ColorLerpBox mLerpBox;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HsvColorPicker" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public HsvColorPicker(Base parent) : base(parent)
        {
            MouseInputEnabled = true;
            SetSize(256, 128);

            //ShouldCacheToTexture = true;

            mLerpBox = new ColorLerpBox(this);
            mLerpBox.ColorChanged += ColorBoxChanged;
            mLerpBox.Dock = Pos.Left;

            mColorSlider = new ColorSlider(this);
            mColorSlider.SetPosition(mLerpBox.Width + 15, 5);
            mColorSlider.ColorChanged += ColorSliderChanged;
            mColorSlider.Dock = Pos.Left;

            mAfter = new ColorDisplay(this);
            mAfter.SetSize(48, 24);
            mAfter.SetPosition(mColorSlider.X + mColorSlider.Width + 15, 5);

            mBefore = new ColorDisplay(this);
            mBefore.SetSize(48, 24);
            mBefore.SetPosition(mAfter.X, 28);

            var x = mBefore.X;
            var y = mBefore.Y + 30;

            {
                var label = new Label(this);
                label.SetText("R:");
                label.SizeToContents();
                label.SetPosition(x, y);

                var numeric = new TextBoxNumeric(this);
                numeric.Name = "RedBox";
                numeric.SetPosition(x + 15, y - 1);
                numeric.SetSize(26, 16);
                numeric.SelectAllOnFocus = true;
                numeric.TextChanged += NumericTyped;
            }

            y += 20;

            {
                var label = new Label(this);
                label.SetText("G:");
                label.SizeToContents();
                label.SetPosition(x, y);

                var numeric = new TextBoxNumeric(this);
                numeric.Name = "GreenBox";
                numeric.SetPosition(x + 15, y - 1);
                numeric.SetSize(26, 16);
                numeric.SelectAllOnFocus = true;
                numeric.TextChanged += NumericTyped;
            }

            y += 20;

            {
                var label = new Label(this);
                label.SetText("B:");
                label.SizeToContents();
                label.SetPosition(x, y);

                var numeric = new TextBoxNumeric(this);
                numeric.Name = "BlueBox";
                numeric.SetPosition(x + 15, y - 1);
                numeric.SetSize(26, 16);
                numeric.SelectAllOnFocus = true;
                numeric.TextChanged += NumericTyped;
            }

            SetColor(DefaultColor);
        }

        /// <summary>
        ///     The "before" color.
        /// </summary>
        public Color DefaultColor
        {
            get => mBefore.Color;
            set => mBefore.Color = value;
        }

        /// <summary>
        ///     Selected color.
        /// </summary>
        public Color SelectedColor => mLerpBox.SelectedColor;

        /// <summary>
        ///     Invoked when the selected color has changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> ColorChanged;

        private void NumericTyped(Base control, EventArgs args)
        {
            var box = control as TextBoxNumeric;
            if (null == box)
            {
                return;
            }

            if (box.Text == String.Empty)
            {
                return;
            }

            var textValue = (int) box.Value;
            if (textValue < 0)
            {
                textValue = 0;
            }

            if (textValue > 255)
            {
                textValue = 255;
            }

            var newColor = SelectedColor;

            if (box.Name.Contains("Red"))
            {
                newColor = Color.FromArgb(SelectedColor.A, textValue, SelectedColor.G, SelectedColor.B);
            }
            else if (box.Name.Contains("Green"))
            {
                newColor = Color.FromArgb(SelectedColor.A, SelectedColor.R, textValue, SelectedColor.B);
            }
            else if (box.Name.Contains("Blue"))
            {
                newColor = Color.FromArgb(SelectedColor.A, SelectedColor.R, SelectedColor.G, textValue);
            }
            else if (box.Name.Contains("Alpha"))
            {
                newColor = Color.FromArgb(textValue, SelectedColor.R, SelectedColor.G, SelectedColor.B);
            }

            SetColor(newColor);
        }

        private void UpdateControls(Color color)
        {
            // [???] TODO: Make this code safer.
            // [halfofastaple] This code SHOULD (in theory) never crash/not work as intended, but referencing children by their name is unsafe.
            //		Instead, a direct reference to their objects should be maintained. Worst case scenario, we grab the wrong "RedBox".

            var redBox = FindChildByName("RedBox", false) as TextBoxNumeric;
            if (redBox != null)
            {
                redBox.SetText(color.R.ToString(), false);
            }

            var greenBox = FindChildByName("GreenBox", false) as TextBoxNumeric;
            if (greenBox != null)
            {
                greenBox.SetText(color.G.ToString(), false);
            }

            var blueBox = FindChildByName("BlueBox", false) as TextBoxNumeric;
            if (blueBox != null)
            {
                blueBox.SetText(color.B.ToString(), false);
            }

            mAfter.Color = color;

            if (ColorChanged != null)
            {
                ColorChanged.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Sets the selected color.
        /// </summary>
        /// <param name="color">Color to set.</param>
        /// <param name="onlyHue">Determines whether only the hue should be set.</param>
        /// <param name="reset">Determines whether the "before" color should be set as well.</param>
        public void SetColor(Color color, bool onlyHue = false, bool reset = false)
        {
            UpdateControls(color);

            if (reset)
            {
                mBefore.Color = color;
            }

            mColorSlider.SelectedColor = color;
            mLerpBox.SetColor(color, onlyHue);
            mAfter.Color = color;
        }

        private void ColorBoxChanged(Base control, EventArgs args)
        {
            UpdateControls(SelectedColor);
            Invalidate();
        }

        private void ColorSliderChanged(Base control, EventArgs args)
        {
            if (mLerpBox != null)
            {
                mLerpBox.SetColor(mColorSlider.SelectedColor, true);
            }

            Invalidate();
        }

    }

}
