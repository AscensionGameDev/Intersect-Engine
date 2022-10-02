using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     RadioButton with label.
    /// </summary>
    public partial class LabeledRadioButton : Base
    {

        private readonly Label mLabel;

        private readonly RadioButton mRadioButton;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LabeledRadioButton" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public LabeledRadioButton(Base parent) : base(parent)
        {
            MouseInputEnabled = true;
            _ = SetSize(100, 20);

            mRadioButton = new RadioButton(this)
            {
                //Dock = Pos.Left, // no docking, it causes resizing
                InheritParentEnablementProperties = true,
                IsTabable = false,
                KeyboardInputEnabled = false,
                //Margin = new Margin(0, 2, 2, 2),
            };

            mLabel = new Label(this)
            {
                Alignment = Pos.CenterV | Pos.Left,
                InheritParentEnablementProperties = true,
                IsTabable = false,
                KeyboardInputEnabled = false,
            };
            mLabel.Clicked += delegate (Base control, ClickedEventArgs args) { mRadioButton.Press(control); };
        }

        /// <summary>
        ///     Label text.
        /// </summary>
        public string Text
        {
            get => mLabel.Text;
            set => mLabel.Text = value;
        }

        // todo: would be nice to remove that
        internal RadioButton RadioButton => mRadioButton;

        protected override void Layout(Skin.Base skin)
        {
            // ugly stuff because we don't have anchoring without docking (docking resizes children)
            if (mLabel.Height > mRadioButton.Height
            ) // usually radio is smaller than label so it gets repositioned to avoid clipping with negative Y
            {
                mRadioButton.Y = (mLabel.Height - mRadioButton.Height) / 2;
            }

            Align.PlaceRightBottom(mLabel, mRadioButton);
            SizeToChildren();
            base.Layout(skin);
        }

        /// <summary>
        ///     Renders the focus overlay.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void RenderFocus(Skin.Base skin)
        {
            if (InputHandler.KeyboardFocus != this)
            {
                return;
            }

            if (!IsTabable)
            {
                return;
            }

            skin.DrawKeyboardHighlight(this, RenderBounds, 0);
        }

        /// <summary>
        ///     Handler for Space keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeySpace(bool down)
        {
            if (down)
            {
                mRadioButton.IsChecked = !mRadioButton.IsChecked;
            }

            return true;
        }

        /// <summary>
        ///     Selects the radio button.
        /// </summary>
        public virtual void Select()
        {
            mRadioButton.IsChecked = true;
        }

    }

}
