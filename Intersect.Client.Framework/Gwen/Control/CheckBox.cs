using System;

using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;

using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     CheckBox control.
    /// </summary>
    public class CheckBox : Button
    {

        public enum ControlState
        {

            Normal = 0,

            Disabled,

            CheckedNormal,

            CheckedDisabled,

        }

        private bool mChecked;

        private GameTexture mCheckedDisabledImage;

        private string mCheckedDisabledImageFilename;

        private GameTexture mCheckedNormalImage;

        private string mCheckedNormalImageFilename;

        //Sound Effects
        private string mCheckSound;

        private GameTexture mDisabledImage;

        private string mDisabledImageFilename;

        private GameTexture mNormalImage;

        private string mNormalImageFilename;

        private string mUncheckedSound;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CheckBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public CheckBox(Base parent, string name = "") : base(parent, name)
        {
            SetSize(15, 15);
            IsToggle = true;
        }

        /// <summary>
        ///     Indicates whether the checkbox is checked.
        /// </summary>
        public bool IsChecked
        {
            get => mChecked;
            set
            {
                if (mChecked == value)
                {
                    return;
                }

                mChecked = value;
                OnCheckChanged();
            }
        }

        /// <summary>
        ///     Determines whether unchecking is allowed.
        /// </summary>
        protected virtual bool AllowUncheck => true;

        public override JObject GetJson()
        {
            var obj = base.GetJson();
            obj.Add("NormalImage", GetImageFilename(ControlState.Normal));
            obj.Add("CheckedImage", GetImageFilename(ControlState.CheckedNormal));
            obj.Add("DisabledImage", GetImageFilename(ControlState.Disabled));
            obj.Add("CheckedDisabledImage", GetImageFilename(ControlState.CheckedDisabled));
            obj.Add("CheckedSound", mCheckSound);
            obj.Add("UncheckedSound", mUncheckedSound);

            return base.FixJson(obj);
        }

        public override void LoadJson(JToken obj)
        {
            base.LoadJson(obj);
            if (obj["NormalImage"] != null)
            {
                SetImage(
                    GameContentManager.Current.GetTexture(
                        GameContentManager.TextureType.Gui, (string) obj["NormalImage"]
                    ), (string) obj["NormalImage"], ControlState.Normal
                );
            }

            if (obj["CheckedImage"] != null)
            {
                SetImage(
                    GameContentManager.Current.GetTexture(
                        GameContentManager.TextureType.Gui, (string) obj["CheckedImage"]
                    ), (string) obj["CheckedImage"], ControlState.CheckedNormal
                );
            }

            if (obj["DisabledImage"] != null)
            {
                SetImage(
                    GameContentManager.Current.GetTexture(
                        GameContentManager.TextureType.Gui, (string) obj["DisabledImage"]
                    ), (string) obj["DisabledImage"], ControlState.Disabled
                );
            }

            if (obj["CheckedDisabledImage"] != null)
            {
                SetImage(
                    GameContentManager.Current.GetTexture(
                        GameContentManager.TextureType.Gui, (string) obj["CheckedDisabledImage"]
                    ), (string) obj["CheckedDisabledImage"], ControlState.CheckedDisabled
                );
            }

            if (obj["CheckedSound"] != null)
            {
                mCheckSound = (string) obj["CheckedSound"];
            }

            if (obj["UncheckedSound"] != null)
            {
                mUncheckedSound = (string) obj["UncheckedSound"];
            }
        }

        /// <summary>
        ///     Toggles the checkbox.
        /// </summary>
        public override void Toggle()
        {
            base.Toggle();
            IsChecked = !IsChecked;
            if (IsChecked)
            {
                base.PlaySound(mCheckSound);
            }
            else
            {
                base.PlaySound(mUncheckedSound);
            }
        }

        /// <summary>
        ///     Invoked when the checkbox has been checked.
        /// </summary>
        public event GwenEventHandler<EventArgs> Checked;

        /// <summary>
        ///     Invoked when the checkbox has been unchecked.
        /// </summary>
        public event GwenEventHandler<EventArgs> UnChecked;

        /// <summary>
        ///     Invoked when the checkbox state has been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> CheckChanged;

        /// <summary>
        ///     Handler for CheckChanged event.
        /// </summary>
        protected virtual void OnCheckChanged()
        {
            if (IsChecked)
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

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            base.Render(skin);
            skin.DrawCheckBox(this, mChecked, IsDepressed);
        }

        public void SetCheckSize(int w, int h)
        {
        }

        /// <summary>
        ///     Internal OnPressed implementation.
        /// </summary>
        protected override void OnClicked(int x, int y)
        {
            if (IsDisabled)
            {
                return;
            }

            if (IsChecked && !AllowUncheck)
            {
                return;
            }

            base.OnClicked(x, y);
        }

        public void SetImage(GameTexture texture, string fileName, ControlState state)
        {
            switch (state)
            {
                case ControlState.Normal:
                    mNormalImageFilename = fileName;
                    mNormalImage = texture;

                    break;
                case ControlState.Disabled:
                    mDisabledImageFilename = fileName;
                    mDisabledImage = texture;

                    break;
                case ControlState.CheckedNormal:
                    mCheckedNormalImageFilename = fileName;
                    mCheckedNormalImage = texture;

                    break;
                case ControlState.CheckedDisabled:
                    mCheckedDisabledImageFilename = fileName;
                    mCheckedDisabledImage = texture;

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public GameTexture GetImage(ControlState state)
        {
            switch (state)
            {
                case ControlState.Normal:
                    return mNormalImage;
                case ControlState.Disabled:
                    return mDisabledImage;
                case ControlState.CheckedNormal:
                    return mCheckedNormalImage;
                case ControlState.CheckedDisabled:
                    return mCheckedDisabledImage;
                default:
                    return null;
            }
        }

        public string GetImageFilename(ControlState state)
        {
            switch (state)
            {
                case ControlState.Normal:
                    return mNormalImageFilename;
                case ControlState.Disabled:
                    return mDisabledImageFilename;
                case ControlState.CheckedNormal:
                    return mCheckedNormalImageFilename;
                case ControlState.CheckedDisabled:
                    return mCheckedDisabledImageFilename;
                default:
                    return null;
            }
        }

    }

}
