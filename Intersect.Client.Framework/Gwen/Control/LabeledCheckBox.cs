using System;

using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.EventArguments;

using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     CheckBox with label.
    /// </summary>
    public class LabeledCheckBox : Base
    {

        private readonly CheckBox mCheckBox;

        private readonly Label mLabel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LabeledCheckBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public LabeledCheckBox(Base parent, string name = "") : base(parent, name)
        {
            SetSize(200, 19);
            mCheckBox = new CheckBox(this);
            mCheckBox.Dock = Pos.Left;
            mCheckBox.Margin = new Margin(0, 2, 2, 2);
            mCheckBox.IsTabable = false;
            mCheckBox.CheckChanged += OnCheckChanged;

            mLabel = new Label(this);
            mLabel.Dock = Pos.Fill;
            mLabel.Clicked += delegate(Base control, ClickedEventArgs args) { mCheckBox.Press(control); };
            mLabel.IsTabable = false;

            IsTabable = false;
        }

        /// <summary>
        ///     Indicates whether the control is checked.
        /// </summary>
        public bool IsChecked
        {
            get => mCheckBox.IsChecked;
            set => mCheckBox.IsChecked = value;
        }

        /// <summary>
        ///     Label text.
        /// </summary>
        public string Text
        {
            get => mLabel.Text;
            set => mLabel.Text = value;
        }

        /// <summary>
        ///     Invoked when the control has been checked.
        /// </summary>
        public event GwenEventHandler<EventArgs> Checked;

        /// <summary>
        ///     Invoked when the control has been unchecked.
        /// </summary>
        public event GwenEventHandler<EventArgs> UnChecked;

        /// <summary>
        ///     Invoked when the control's check has been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> CheckChanged;

        public override JObject GetJson()
        {
            var obj = base.GetJson();
            obj.Add("Label", mLabel.GetJson());
            obj.Add("Checkbox", mCheckBox.GetJson());

            return base.FixJson(obj);
        }

        public override void LoadJson(JToken obj)
        {
            base.LoadJson(obj);
            if (obj["Label"] != null)
            {
                mLabel.Dock = Pos.None;
                mLabel.LoadJson(obj["Label"]);
            }

            if (obj["Checkbox"] != null)
            {
                mCheckBox.Dock = Pos.None;
                mCheckBox.LoadJson(obj["Checkbox"]);
            }
        }

        /// <summary>
        ///     Handler for CheckChanged event.
        /// </summary>
        protected virtual void OnCheckChanged(Base control, EventArgs args)
        {
            if (mCheckBox.IsChecked)
            {
                if (Checked != null)
                {
                    Checked.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                if (UnChecked != null)
                {
                    UnChecked.Invoke(this, EventArgs.Empty);
                }
            }

            if (CheckChanged != null)
            {
                CheckChanged.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetCheckSize(int w, int h)
        {
            mCheckBox.SetSize(w, h);
        }

        public void SetImage(GameTexture texture, string fileName, CheckBox.ControlState state)
        {
            mCheckBox.SetImage(texture, fileName, state);
        }

        public void SetTextColor(Color clr, Label.ControlState state)
        {
            mLabel.SetTextColor(clr, state);
        }

        public void SetLabelDistance(int dist)
        {
            mCheckBox.Margin = new Margin(0, 2, dist, 2);
        }

        public void SetFont(GameFont font)
        {
            mLabel.Font = font;
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
            base.OnKeySpace(down);
            if (!down)
            {
                mCheckBox.IsChecked = !mCheckBox.IsChecked;
            }

            return true;
        }

    }

}
