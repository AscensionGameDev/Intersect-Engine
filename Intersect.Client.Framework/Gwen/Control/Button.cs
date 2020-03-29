using System;

using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Input;

using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Button control.
    /// </summary>
    public class Button : Label
    {

        public enum ControlState
        {

            Normal = 0,

            Hovered,

            Clicked,

            Disabled,

        }

        private bool mCenterImage;

        private GameTexture mClickedImage;

        private string mClickedImageFilename;

        protected string mClickSound;

        private bool mDepressed;

        private GameTexture mDisabledImage;

        private string mDisabledImageFilename;

        private GameTexture mHoverImage;

        private string mHoverImageFilename;

        //Sound Effects
        protected string mHoverSound;

        protected string mMouseDownSound;

        protected string mMouseUpSound;

        private GameTexture mNormalImage;

        private string mNormalImageFilename;

        private bool mToggle;

        private bool mToggleStatus;

        /// <summary>
        ///     Control constructor.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Button(Base parent, string name = "") : base(parent, name)
        {
            AutoSizeToContents = false;
            SetSize(100, 20);
            MouseInputEnabled = true;
            Alignment = Pos.Center;
            TextPadding = new Padding(3, 3, 3, 3);
            Name = name;
        }

        /// <summary>
        ///     Indicates whether the button is depressed.
        /// </summary>
        public bool IsDepressed
        {
            get => mDepressed;
            set
            {
                if (mDepressed == value)
                {
                    return;
                }

                mDepressed = value;
                Redraw();
            }
        }

        /// <summary>
        ///     Indicates whether the button is toggleable.
        /// </summary>
        public bool IsToggle
        {
            get => mToggle;
            set => mToggle = value;
        }

        /// <summary>
        ///     Determines the button's toggle state.
        /// </summary>
        public bool ToggleState
        {
            get => mToggleStatus;
            set
            {
                if (!mToggle)
                {
                    return;
                }

                if (mToggleStatus == value)
                {
                    return;
                }

                mToggleStatus = value;

                if (Toggled != null)
                {
                    Toggled.Invoke(this, EventArgs.Empty);
                }

                if (mToggleStatus)
                {
                    if (ToggledOn != null)
                    {
                        ToggledOn.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    if (ToggledOff != null)
                    {
                        ToggledOff.Invoke(this, EventArgs.Empty);
                    }
                }

                Redraw();
            }
        }

        /// <summary>
        ///     Invoked when the button is pressed.
        /// </summary>
        public event GwenEventHandler<EventArgs> Pressed;

        /// <summary>
        ///     Invoked when the button is released.
        /// </summary>
        public event GwenEventHandler<EventArgs> Released;

        /// <summary>
        ///     Invoked when the button's toggle state has changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> Toggled;

        /// <summary>
        ///     Invoked when the button's toggle state has changed to On.
        /// </summary>
        public event GwenEventHandler<EventArgs> ToggledOn;

        /// <summary>
        ///     Invoked when the button's toggle state has changed to Off.
        /// </summary>
        public event GwenEventHandler<EventArgs> ToggledOff;

        public override JObject GetJson()
        {
            var obj = base.GetJson();
            if (this.GetType() != typeof(CheckBox))
            {
                obj.Add("NormalImage", GetImageFilename(ControlState.Normal));
                obj.Add("HoveredImage", GetImageFilename(ControlState.Hovered));
                obj.Add("ClickedImage", GetImageFilename(ControlState.Clicked));
                obj.Add("DisabledImage", GetImageFilename(ControlState.Disabled));
            }

            obj.Add("CenterImage", mCenterImage);
            if (this.GetType() != typeof(ComboBox))
            {
                obj.Add("HoverSound", mHoverSound);
                obj.Add("MouseUpSound", mMouseUpSound);
                obj.Add("MouseDownSound", mMouseDownSound);
                obj.Add("ClickSound", mClickSound);
            }

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

            if (obj["HoveredImage"] != null)
            {
                SetImage(
                    GameContentManager.Current.GetTexture(
                        GameContentManager.TextureType.Gui, (string) obj["HoveredImage"]
                    ), (string) obj["HoveredImage"], ControlState.Hovered
                );
            }

            if (obj["ClickedImage"] != null)
            {
                SetImage(
                    GameContentManager.Current.GetTexture(
                        GameContentManager.TextureType.Gui, (string) obj["ClickedImage"]
                    ), (string) obj["ClickedImage"], ControlState.Clicked
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

            if (obj["CenterImage"] != null)
            {
                mCenterImage = (bool) obj["CenterImage"];
            }

            if (this.GetType() != typeof(ComboBox) && this.GetType() != typeof(CheckBox))
            {
                if (obj["HoverSound"] != null)
                {
                    mHoverSound = (string) obj["HoverSound"];
                }

                if (obj["MouseUpSound"] != null)
                {
                    mMouseUpSound = (string) obj["MouseUpSound"];
                }

                if (obj["MouseDownSound"] != null)
                {
                    mMouseDownSound = (string) obj["MouseDownSound"];
                }

                if (obj["ClickSound"] != null)
                {
                    mClickSound = (string) obj["ClickSound"];
                }
            }
        }

        public void PlayHoverSound()
        {
            base.PlaySound(mHoverSound);
        }

        public void ClearSounds()
        {
            mMouseUpSound = "";
            mMouseDownSound = "";
            mHoverSound = "";
            mClickSound = "";
        }

        /// <summary>
        ///     Toggles the button.
        /// </summary>
        public virtual void Toggle()
        {
            ToggleState = !ToggleState;
        }

        /// <summary>
        ///     "Clicks" the button.
        /// </summary>
        public virtual void Press(Base control = null)
        {
            OnClicked(0, 0);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            base.Render(skin);

            if (ShouldDrawBackground)
            {
                var drawDepressed = IsDepressed && IsHovered;
                if (IsToggle)
                {
                    drawDepressed = drawDepressed || ToggleState;
                }

                var bDrawHovered = IsHovered && ShouldDrawHover;

                skin.DrawButton(this, drawDepressed, bDrawHovered, IsDisabled);
            }
        }

        /// <summary>
        ///     Handler invoked on mouse click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected override void OnMouseClickedLeft(int x, int y, bool down, bool automated = false)
        {
            //base.OnMouseClickedLeft(x, y, down);
            if (down)
            {
                IsDepressed = true;
                InputHandler.MouseFocus = this;

                //Play Mouse Down Sound
                base.PlaySound(mMouseDownSound);
                if (Pressed != null)
                {
                    Pressed.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                if (IsHovered && mDepressed)
                {
                    //Play Clicked Sound
                    base.PlaySound(mClickSound);
                    OnClicked(x, y);
                }

                //Play Mouse Up Sound
                base.PlaySound(mMouseUpSound);
                IsDepressed = false;
                InputHandler.MouseFocus = null;
                if (Released != null)
                {
                    Released.Invoke(this, EventArgs.Empty);
                }
            }

            Redraw();
        }

        /// <summary>
        ///     Internal OnPressed implementation.
        /// </summary>
        protected virtual void OnClicked(int x, int y)
        {
            if (IsDisabled)
            {
                return;
            }

            if (IsToggle)
            {
                Toggle();
            }

            base.OnMouseClickedLeft(x, y, true);
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
            return base.OnKeySpace(down);

            //if (down)
            //    OnClicked(0, 0);
            //return true;
        }

        /// <summary>
        ///     Default accelerator handler.
        /// </summary>
        protected override void OnAccelerator()
        {
            OnClicked(0, 0);
        }

        /// <summary>
        ///     Lays out the control's interior according to alignment, padding, dock etc.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Layout(Skin.Base skin)
        {
            base.Layout(skin);
        }

        /// <summary>
        ///     Updates control colors.
        /// </summary>
        public override void UpdateColors()
        {
            var textColor = GetTextColor(Label.ControlState.Normal);
            if (IsDisabled && GetTextColor(Label.ControlState.Disabled) != null)
            {
                textColor = GetTextColor(Label.ControlState.Disabled);
            }
            else if (IsHovered && GetTextColor(Label.ControlState.Hovered) != null)
            {
                textColor = GetTextColor(Label.ControlState.Hovered);
            }

            if (textColor != null)
            {
                TextColor = textColor;

                return;
            }

            if (IsDisabled)
            {
                TextColor = Skin.Colors.Button.Disabled;

                return;
            }

            if (IsDepressed || ToggleState)
            {
                TextColor = Skin.Colors.Button.Down;

                return;
            }

            if (IsHovered)
            {
                TextColor = Skin.Colors.Button.Hover;

                return;
            }

            TextColor = Skin.Colors.Button.Normal;
        }

        /// <summary>
        ///     Handler invoked on mouse double click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        protected override void OnMouseDoubleClickedLeft(int x, int y)
        {
            base.OnMouseDoubleClickedLeft(x, y);
            OnMouseClickedLeft(x, y, true);
        }

        /// <summary>
        ///     Sets the button's image.
        /// </summary>
        /// <param name="textureName">Texture name. Null to remove.</param>
        public void SetImage(GameTexture texture, string fileName, ControlState state)
        {
            if (texture == null && !string.IsNullOrWhiteSpace(fileName))
            {
                texture = GameContentManager.Current?.GetTexture(GameContentManager.TextureType.Gui, fileName);
            }

            switch (state)
            {
                case ControlState.Normal:
                    mNormalImageFilename = fileName;
                    mNormalImage = texture;

                    break;
                case ControlState.Hovered:
                    mHoverImageFilename = fileName;
                    mHoverImage = texture;

                    break;
                case ControlState.Clicked:
                    mClickedImageFilename = fileName;
                    mClickedImage = texture;

                    break;
                case ControlState.Disabled:
                    mDisabledImageFilename = fileName;
                    mDisabledImage = texture;

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
                case ControlState.Hovered:
                    return mHoverImage;
                case ControlState.Clicked:
                    return mClickedImage;
                case ControlState.Disabled:
                    return mDisabledImage;
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
                case ControlState.Hovered:
                    return mHoverImageFilename;
                case ControlState.Clicked:
                    return mClickedImageFilename;
                case ControlState.Disabled:
                    return mDisabledImageFilename;
                default:
                    return null;
            }
        }

        protected override void OnMouseEntered()
        {
            base.OnMouseEntered();

            //Play Mouse Entered Sound
            if (ShouldDrawHover)
            {
                base.PlaySound(mHoverSound);
            }
        }

        public void SetHoverSound(string sound)
        {
            mHoverSound = sound;
        }

    }

}
