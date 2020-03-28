using System;

using Intersect.Client.Framework.Gwen.ControlInternal;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     RGBA color picker.
    /// </summary>
    public class ColorPicker : Base, IColorPicker
    {

        private Color mColor;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorPicker" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ColorPicker(Base parent) : base(parent)
        {
            MouseInputEnabled = true;

            SetSize(256, 150);
            CreateControls();
            SelectedColor = Color.FromArgb(255, 50, 60, 70);
        }

        /// <summary>
        ///     Red value of the selected color.
        /// </summary>
        public int R
        {
            get => mColor.R;
            set => mColor = Color.FromArgb(mColor.A, value, mColor.G, mColor.B);
        }

        /// <summary>
        ///     Green value of the selected color.
        /// </summary>
        public int G
        {
            get => mColor.G;
            set => mColor = Color.FromArgb(mColor.A, mColor.R, value, mColor.B);
        }

        /// <summary>
        ///     Blue value of the selected color.
        /// </summary>
        public int B
        {
            get => mColor.B;
            set => mColor = Color.FromArgb(mColor.A, mColor.R, mColor.G, value);
        }

        /// <summary>
        ///     Alpha value of the selected color.
        /// </summary>
        public int A
        {
            get => mColor.A;
            set => mColor = Color.FromArgb(value, mColor.R, mColor.G, mColor.B);
        }

        /// <summary>
        ///     Determines whether the Alpha control is visible.
        /// </summary>
        public bool AlphaVisible
        {
            get
            {
                var gb = FindChildByName("Alphagroupbox", true) as GroupBox;

                return !gb.IsHidden;
            }
            set
            {
                var gb = FindChildByName("Alphagroupbox", true) as GroupBox;
                gb.IsHidden = !value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Selected color.
        /// </summary>
        public Color SelectedColor
        {
            get => mColor;
            set
            {
                mColor = value;
                UpdateControls();
            }
        }

        /// <summary>
        ///     Invoked when the selected color has been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> ColorChanged;

        private void CreateColorControl(string name, int y)
        {
            const int colorSize = 12;

            var colorGroup = new GroupBox(this);
            colorGroup.SetPosition(10, y);
            colorGroup.SetText(name);
            colorGroup.SetSize(160, 35);
            colorGroup.Name = name + "groupbox";

            var disp = new ColorDisplay(colorGroup);
            disp.Name = name;
            disp.SetBounds(0, 10, colorSize, colorSize);

            var numeric = new TextBoxNumeric(colorGroup);
            numeric.Name = name + "Box";
            numeric.SetPosition(105, 7);
            numeric.SetSize(26, 16);
            numeric.SelectAllOnFocus = true;
            numeric.TextChanged += NumericTyped;

            var slider = new HorizontalSlider(colorGroup);
            slider.SetPosition(colorSize + 5, 10);
            slider.SetRange(0, 255);
            slider.SetSize(80, colorSize);
            slider.Name = name + "Slider";
            slider.ValueChanged += SlidersMoved;
        }

        private void NumericTyped(Base control, EventArgs args)
        {
            var box = control as TextBoxNumeric;
            if (null == box)
            {
                return;
            }

            if (box.Text == string.Empty)
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

            if (box.Name.Contains("Red"))
            {
                R = textValue;
            }

            if (box.Name.Contains("Green"))
            {
                G = textValue;
            }

            if (box.Name.Contains("Blue"))
            {
                B = textValue;
            }

            if (box.Name.Contains("Alpha"))
            {
                A = textValue;
            }

            UpdateControls();
        }

        private void CreateControls()
        {
            const int startY = 5;
            const int height = 35;

            CreateColorControl("Red", startY);
            CreateColorControl("Green", startY + height);
            CreateColorControl("Blue", startY + height * 2);
            CreateColorControl("Alpha", startY + height * 3);

            var finalGroup = new GroupBox(this);
            finalGroup.SetPosition(180, 40);
            finalGroup.SetSize(60, 60);
            finalGroup.SetText("Result");
            finalGroup.Name = "ResultGroupBox";

            var disp = new ColorDisplay(finalGroup);
            disp.Name = "Result";
            disp.SetBounds(0, 10, 32, 32);

            //disp.DrawCheckers = true;

            //UpdateControls();
        }

        private void UpdateColorControls(string name, Color col, int sliderVal)
        {
            var disp = FindChildByName(name, true) as ColorDisplay;
            disp.Color = col;

            var slider = FindChildByName(name + "Slider", true) as HorizontalSlider;
            slider.Value = sliderVal;

            var box = FindChildByName(name + "Box", true) as TextBoxNumeric;
            box.Value = sliderVal;
        }

        private void UpdateControls()
        {
            //This is a little weird, but whatever for now
            UpdateColorControls("Red", Color.FromArgb(255, SelectedColor.R, 0, 0), SelectedColor.R);
            UpdateColorControls("Green", Color.FromArgb(255, 0, SelectedColor.G, 0), SelectedColor.G);
            UpdateColorControls("Blue", Color.FromArgb(255, 0, 0, SelectedColor.B), SelectedColor.B);
            UpdateColorControls("Alpha", Color.FromArgb(SelectedColor.A, 255, 255, 255), SelectedColor.A);

            var disp = FindChildByName("Result", true) as ColorDisplay;
            disp.Color = SelectedColor;

            if (ColorChanged != null)
            {
                ColorChanged.Invoke(this, EventArgs.Empty);
            }
        }

        private void SlidersMoved(Base control, EventArgs args)
        {
            /*
            HorizontalSlider* redSlider		= gwen_cast<HorizontalSlider>(	FindChildByName( "RedSlider",   true ) );
            HorizontalSlider* greenSlider	= gwen_cast<HorizontalSlider>(	FindChildByName( "GreenSlider", true ) );
            HorizontalSlider* blueSlider	= gwen_cast<HorizontalSlider>(	FindChildByName( "BlueSlider",  true ) );
            HorizontalSlider* alphaSlider	= gwen_cast<HorizontalSlider>(	FindChildByName( "AlphaSlider", true ) );
            */

            var slider = control as HorizontalSlider;
            if (slider != null)
            {
                SetColorByName(GetColorFromName(slider.Name), (int) slider.Value);
            }

            UpdateControls();

            //SetColor( Gwen::Color( redSlider->GetValue(), greenSlider->GetValue(), blueSlider->GetValue(), alphaSlider->GetValue() ) );
        }

        /// <summary>
        ///     Lays out the control's interior according to alignment, padding, dock etc.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Layout(Skin.Base skin)
        {
            base.Layout(skin);

            SizeToChildren(false, true);
            SetSize(Width, Height + 5);

            var groupBox = FindChildByName("ResultGroupBox", true) as GroupBox;
            if (groupBox != null)
            {
                groupBox.SetPosition(groupBox.X, Height * 0.5f - groupBox.Height * 0.5f);
            }

            //UpdateControls(); // this spams events continuously every tick
        }

        private int GetColorByName(string colorName)
        {
            if (colorName == "Red")
            {
                return SelectedColor.R;
            }

            if (colorName == "Green")
            {
                return SelectedColor.G;
            }

            if (colorName == "Blue")
            {
                return SelectedColor.B;
            }

            if (colorName == "Alpha")
            {
                return SelectedColor.A;
            }

            return 0;
        }

        private static string GetColorFromName(string name)
        {
            if (name.Contains("Red"))
            {
                return "Red";
            }

            if (name.Contains("Green"))
            {
                return "Green";
            }

            if (name.Contains("Blue"))
            {
                return "Blue";
            }

            if (name.Contains("Alpha"))
            {
                return "Alpha";
            }

            return String.Empty;
        }

        private void SetColorByName(string colorName, int colorValue)
        {
            if (colorName == "Red")
            {
                R = colorValue;
            }
            else if (colorName == "Green")
            {
                G = colorValue;
            }
            else if (colorName == "Blue")
            {
                B = colorValue;
            }
            else if (colorName == "Alpha")
            {
                A = colorValue;
            }
        }

    }

}
