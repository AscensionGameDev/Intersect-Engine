using System;

using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Control.Layout;
using Intersect.Client.Framework.Gwen.ControlInternal;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Numeric up/down.
    /// </summary>
    public class NumericUpDown : TextBoxNumeric
    {

        private readonly UpDownButtonDown mDown;

        private readonly Splitter mSplitter;

        private readonly UpDownButtonUp mUp;

        private int mMax;

        private int mMin;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NumericUpDown" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public NumericUpDown(Base parent) : base(parent)
        {
            SetSize(100, 20);

            mSplitter = new Splitter(this);
            mSplitter.Dock = Pos.Right;
            mSplitter.SetSize(13, 13);

            mUp = new UpDownButtonUp(mSplitter);
            mUp.Clicked += OnButtonUp;
            mUp.IsTabable = false;
            mSplitter.SetPanel(0, mUp, false);

            mDown = new UpDownButtonDown(mSplitter);
            mDown.Clicked += OnButtonDown;
            mDown.IsTabable = false;
            mDown.Padding = new Padding(0, 1, 1, 0);
            mSplitter.SetPanel(1, mDown, false);

            mMax = 100;
            mMin = 0;
            mValue = 0f;
            Text = "0";
        }

        /// <summary>
        ///     Minimum value.
        /// </summary>
        public int Min
        {
            get => mMin;
            set => mMin = value;
        }

        /// <summary>
        ///     Maximum value.
        /// </summary>
        public int Max
        {
            get => mMax;
            set => mMax = value;
        }

        /// <summary>
        ///     Numeric value of the control.
        /// </summary>
        public override float Value
        {
            get => base.Value;
            set
            {
                if (value < mMin)
                {
                    value = mMin;
                }

                if (value > mMax)
                {
                    value = mMax;
                }

                if (value == mValue)
                {
                    return;
                }

                base.Value = value;
            }
        }

        /// <summary>
        ///     Invoked when the value has been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> ValueChanged;

        /// <summary>
        ///     Handler for Up Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeyUp(bool down)
        {
            if (down)
            {
                OnButtonUp(null, EventArgs.Empty);
            }

            return true;
        }

        /// <summary>
        ///     Handler for Down Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeyDown(bool down)
        {
            if (down)
            {
                OnButtonDown(null, new ClickedEventArgs(0, 0, true));
            }

            return true;
        }

        /// <summary>
        ///     Handler for the button up event.
        /// </summary>
        /// <param name="control">Event source.</param>
        protected virtual void OnButtonUp(Base control, EventArgs args)
        {
            Value = mValue + 1;
        }

        /// <summary>
        ///     Handler for the button down event.
        /// </summary>
        /// <param name="control">Event source.</param>
        protected virtual void OnButtonDown(Base control, ClickedEventArgs args)
        {
            Value = mValue - 1;
        }

        /// <summary>
        ///     Determines whether the text can be assighed to the control.
        /// </summary>
        /// <param name="str">Text to evaluate.</param>
        /// <returns>True if the text is allowed.</returns>
        protected override bool IsTextAllowed(string str)
        {
            float d;
            if (!float.TryParse(str, out d))
            {
                return false;
            }

            if (d < mMin)
            {
                return false;
            }

            if (d > mMax)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Handler for the text changed event.
        /// </summary>
        protected override void OnTextChanged()
        {
            base.OnTextChanged();
            if (ValueChanged != null)
            {
                ValueChanged.Invoke(this, EventArgs.Empty);
            }
        }

    }

}
