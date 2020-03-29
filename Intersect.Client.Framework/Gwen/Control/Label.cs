using System;

using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.ControlInternal;

using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Static text label.
    /// </summary>
    public class Label : Base
    {

        public enum ControlState
        {

            Normal = 0,

            Hovered,

            Clicked,

            Disabled,

        }

        protected readonly Text mText;

        private string fontInfo;

        private Pos mAlign;

        private bool mAutoSizeToContents;

        private string mBackgroundTemplateFilename;

        private GameTexture mBackgroundTemplateTex;

        protected Color mClickedTextColor;

        protected Color mDisabledTextColor;

        protected Color mHoverTextColor;

        protected Color mNormalTextColor;

        private Padding mTextPadding;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Label" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Label(Base parent, string name = "") : base(parent, name)
        {
            mText = new Text(this);

            //m_Text.Font = Skin.DefaultFont;

            MouseInputEnabled = false;
            SetSize(100, 10);
            Alignment = Pos.Left | Pos.Top;

            mAutoSizeToContents = true;
        }

        public GameTexture ToolTipBackground { get; set; }

        /// <summary>
        ///     Text alignment.
        /// </summary>
        public Pos Alignment
        {
            get => mAlign;
            set
            {
                mAlign = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Text.
        /// </summary>
        public virtual string Text
        {
            get => mText?.String;
            set => SetText(value);
        }

        /// <summary>
        ///     Font.
        /// </summary>
        public GameFont Font
        {
            get => mText?.Font;
            set
            {
                if (value != null)
                {
                    if (mText != null)
                    {
                        mText.Font = value;
                        fontInfo = $"{value?.GetName()},{value?.GetSize()}";
                    }

                    if (mAutoSizeToContents)
                    {
                        SizeToContents();
                    }

                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Font Name
        /// </summary>
        public string FontName
        {
            get => mText?.Font?.GetName() ?? "arial";
            set => Font = GameContentManager.Current?.GetFont(value, FontSize);
        }

        /// <summary>
        /// Font Size
        /// </summary>
        public int FontSize
        {
            get => mText?.Font?.GetSize() ?? 12;
            set => Font = GameContentManager.Current?.GetFont(FontName, value);
        }

        /// <summary>
        ///     Text color.
        /// </summary>
        public Color TextColor
        {
            get => mText.TextColor;
            set => mText.TextColor = value;
        }

        /// <summary>
        ///     Override text color (used by tooltips).
        /// </summary>
        public Color TextColorOverride
        {
            get => mText.TextColorOverride;
            set => mText.TextColorOverride = value;
        }

        /// <summary>
        ///     Text override - used to display different string.
        /// </summary>
        public string TextOverride
        {
            get => mText.TextOverride;
            set => mText.TextOverride = value;
        }

        /// <summary>
        ///     Width of the text (in pixels).
        /// </summary>
        public int TextWidth => mText.Width;

        /// <summary>
        ///     Height of the text (in pixels).
        /// </summary>
        public int TextHeight => mText.Height;

        public int TextX => mText.X;

        public int TextY => mText.Y;

        /// <summary>
        ///     Text length (in characters).
        /// </summary>
        public int TextLength => mText.Length;

        public int TextRight => mText.Right;

        /// <summary>
        ///     Determines if the control should autosize to its text.
        /// </summary>
        public bool AutoSizeToContents
        {
            get => mAutoSizeToContents;
            set
            {
                mAutoSizeToContents = value;
                Invalidate();
                InvalidateParent();
            }
        }

        /// <summary>
        ///     Text padding.
        /// </summary>
        public Padding TextPadding
        {
            get => mTextPadding;
            set
            {
                mTextPadding = value;
                Invalidate();
                InvalidateParent();
            }
        }

        public override JObject GetJson()
        {
            var obj = base.GetJson();
            if (this.GetType() == typeof(Label))
            {
                obj.Add("BackgroundTemplate", mBackgroundTemplateFilename);
            }

            obj.Add("TextColor", Color.ToString(TextColor));
            obj.Add("HoveredTextColor", Color.ToString(mHoverTextColor));
            obj.Add("ClickedTextColor", Color.ToString(mClickedTextColor));
            obj.Add("DisabledTextColor", Color.ToString(mDisabledTextColor));
            obj.Add("TextAlign", mAlign.ToString());
            obj.Add("TextPadding", Padding.ToString(mTextPadding));
            obj.Add("AutoSizeToContents", mAutoSizeToContents);
            obj.Add("Font", fontInfo);
            obj.Add("TextScale", mText.GetScale());

            return base.FixJson(obj);
        }

        public override void LoadJson(JToken obj)
        {
            base.LoadJson(obj);
            if (this.GetType() == typeof(Label) && obj["BackgroundTemplate"] != null)
            {
                SetBackgroundTemplate(
                    GameContentManager.Current.GetTexture(
                        GameContentManager.TextureType.Gui, (string) obj["BackgroundTemplate"]
                    ), (string) obj["BackgroundTemplate"]
                );
            }

            if (obj["TextColor"] != null)
            {
                TextColor = Color.FromString((string) obj["TextColor"]);
            }

            mNormalTextColor = TextColor;
            if (obj["HoveredTextColor"] != null)
            {
                mHoverTextColor = Color.FromString((string) obj["HoveredTextColor"]);
            }

            if (obj["ClickedTextColor"] != null)
            {
                mClickedTextColor = Color.FromString((string) obj["ClickedTextColor"]);
            }

            mDisabledTextColor = Color.FromString((string) obj["DisabledTextColor"], new Color(255, 90, 90, 90));
            if (obj["TextAlign"] != null)
            {
                mAlign = (Pos) Enum.Parse(typeof(Pos), (string) obj["TextAlign"]);
            }

            if (obj["TextPadding"] != null)
            {
                TextPadding = Padding.FromString((string) obj["TextPadding"]);
            }

            if (obj["AutoSizeToContents"] != null)
            {
                mAutoSizeToContents = (bool) obj["AutoSizeToContents"];
            }

            if (obj["Font"] != null && obj["Font"].Type != JTokenType.Null)
            {
                var fontArr = ((string) obj["Font"]).Split(',');
                fontInfo = (string) obj["Font"];
                Font = GameContentManager.Current.GetFont(fontArr[0], int.Parse(fontArr[1]));
            }

            if (obj["TextScale"] != null)
            {
                mText.SetScale((float) obj["TextScale"]);
            }
        }

        public GameTexture GetTemplate()
        {
            return mBackgroundTemplateTex;
        }

        public void SetBackgroundTemplate(GameTexture texture, string fileName)
        {
            if (texture == null && !string.IsNullOrWhiteSpace(fileName))
            {
                texture = GameContentManager.Current?.GetTexture(GameContentManager.TextureType.Gui, fileName);
            }

            mBackgroundTemplateFilename = fileName;
            mBackgroundTemplateTex = texture;
        }

        public virtual void MakeColorNormal()
        {
            TextColor = Skin.Colors.Label.Default;
        }

        public virtual void MakeColorBright()
        {
            TextColor = Skin.Colors.Label.Bright;
        }

        public virtual void MakeColorDark()
        {
            TextColor = Skin.Colors.Label.Dark;
        }

        public virtual void MakeColorHighlight()
        {
            TextColor = Skin.Colors.Label.Highlight;
        }

        public override event Base.GwenEventHandler<ClickedEventArgs> Clicked
        {
            add
            {
                base.Clicked += value;
                MouseInputEnabled = ClickEventAssigned;
            }
            remove
            {
                base.Clicked -= value;
                MouseInputEnabled = ClickEventAssigned;
            }
        }

        public override event Base.GwenEventHandler<ClickedEventArgs> DoubleClicked
        {
            add
            {
                base.DoubleClicked += value;
                MouseInputEnabled = ClickEventAssigned;
            }
            remove
            {
                base.DoubleClicked -= value;
                MouseInputEnabled = ClickEventAssigned;
            }
        }

        public override event Base.GwenEventHandler<ClickedEventArgs> RightClicked
        {
            add
            {
                base.RightClicked += value;
                MouseInputEnabled = ClickEventAssigned;
            }
            remove
            {
                base.RightClicked -= value;
                MouseInputEnabled = ClickEventAssigned;
            }
        }

        public override event Base.GwenEventHandler<ClickedEventArgs> DoubleRightClicked
        {
            add
            {
                base.DoubleRightClicked += value;
                MouseInputEnabled = ClickEventAssigned;
            }
            remove
            {
                base.DoubleRightClicked -= value;
                MouseInputEnabled = ClickEventAssigned;
            }
        }

        /// <summary>
        ///     Returns index of the character closest to specified point (in canvas coordinates).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual Point GetClosestCharacter(int x, int y)
        {
            return new Point(mText.GetClosestCharacter(mText.CanvasPosToLocal(new Point(x, y))), 0);
        }

        /// <summary>
        ///     Sets the position of the internal text control.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected void SetTextPosition(int x, int y)
        {
            mText.SetPosition(x, y);
        }

        /// <summary>
        ///     Handler for text changed event.
        /// </summary>
        protected virtual void OnTextChanged()
        {
        }

        /// <summary>
        ///     Lays out the control's interior according to alignment, padding, dock etc.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Layout(Skin.Base skin)
        {
            base.Layout(skin);

            var align = mAlign;

            if (mAutoSizeToContents)
            {
                SizeToContents();
            }

            var x = mTextPadding.Left + Padding.Left;
            var y = mTextPadding.Top + Padding.Top;

            if (0 != (align & Pos.Right))
            {
                x = Width - mText.Width - mTextPadding.Right - Padding.Right;
            }

            if (0 != (align & Pos.CenterH))
            {
                x = (int) (mTextPadding.Left +
                           Padding.Left +
                           (Width -
                            mText.Width -
                            mTextPadding.Left -
                            Padding.Left -
                            mTextPadding.Right -
                            Padding.Right) *
                           0.5f);
            }

            if (0 != (align & Pos.CenterV))
            {
                y = (int) (mTextPadding.Top +
                           Padding.Top +
                           (Height - mText.Height) * 0.5f -
                           mTextPadding.Bottom -
                           Padding.Bottom);
            }

            if (0 != (align & Pos.Bottom))
            {
                y = Height - mText.Height - mTextPadding.Bottom - Padding.Bottom;
            }

            mText.SetPosition(x, y);
        }

        /// <summary>
        ///     Sets the label text.
        /// </summary>
        /// <param name="str">Text to set.</param>
        /// <param name="doEvents">Determines whether to invoke "text changed" event.</param>
        public virtual void SetText(string str, bool doEvents = true)
        {
            if (Text == str)
            {
                return;
            }

            mText.String = str;
            if (mAutoSizeToContents)
            {
                SizeToContents();
            }

            Invalidate();
            InvalidateParent();

            if (doEvents)
            {
                OnTextChanged();
            }
        }

        public virtual void SetTextScale(float scale)
        {
            mText.SetScale(scale);
            if (mAutoSizeToContents)
            {
                SizeToContents();
            }

            Invalidate();
            InvalidateParent();
        }

        public virtual void SizeToContents()
        {
            mText.SetPosition(mTextPadding.Left + Padding.Left, mTextPadding.Top + Padding.Top);
            mText.SizeToContents();

            SetSize(
                mText.Width + Padding.Left + Padding.Right + mTextPadding.Left + mTextPadding.Right,
                mText.Height + Padding.Top + Padding.Bottom + mTextPadding.Top + mTextPadding.Bottom
            );

            ProcessAlignments();
            InvalidateParent();
        }

        /// <summary>
        ///     Gets the coordinates of specified character.
        /// </summary>
        /// <param name="index">Character index.</param>
        /// <returns>Character coordinates (local).</returns>
        public virtual Point GetCharacterPosition(int index)
        {
            var p = mText.GetCharacterPosition(index);

            return new Point(p.X + mText.X, p.Y + mText.Y);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.DrawLabel(this);
        }

        /// <summary>
        ///     Updates control colors.
        /// </summary>
        public override void UpdateColors()
        {
            var textColor = GetTextColor(ControlState.Normal);
            if (IsDisabled && GetTextColor(ControlState.Disabled) != null)
            {
                textColor = GetTextColor(ControlState.Disabled);
            }
            else if (IsHovered && GetTextColor(ControlState.Hovered) != null)
            {
                textColor = GetTextColor(ControlState.Hovered);
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

            if (IsHovered && ClickEventAssigned)
            {
                TextColor = Skin.Colors.Button.Hover;

                return;
            }

            TextColor = Skin.Colors.Button.Normal;
        }

        public virtual void SetTextColor(Color clr, ControlState state)
        {
            switch (state)
            {
                case ControlState.Normal:
                    mNormalTextColor = clr;

                    break;
                case ControlState.Hovered:
                    mHoverTextColor = clr;

                    break;
                case ControlState.Clicked:
                    mClickedTextColor = clr;

                    break;
                case ControlState.Disabled:
                    mDisabledTextColor = clr;

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            UpdateColors();
        }

        public virtual Color GetTextColor(ControlState state)
        {
            switch (state)
            {
                case ControlState.Normal:
                    return mNormalTextColor;
                case ControlState.Hovered:
                    return mHoverTextColor;
                case ControlState.Clicked:
                    return mClickedTextColor;
                case ControlState.Disabled:
                    return mDisabledTextColor;
                default:
                    return null;
            }
        }

    }

}
