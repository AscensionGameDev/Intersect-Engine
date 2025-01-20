using System.Runtime.CompilerServices;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Static text label.
/// </summary>
public partial class Label : Base, ILabel
{

    public enum ControlState
    {

        Normal = 0,

        Hovered,

        Clicked,

        Disabled,

    }

    protected readonly Text _textElement;

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
    public Label(Base parent, string name = default, bool disableText = false) : base(parent, name)
    {
        _textElement = new Text(this)
        {
            IsHidden = disableText,
        };

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
        set => SetAndDoIfChanged(ref mAlign, value, Invalidate);
    }

    public override void Invalidate()
    {
        base.Invalidate();
    }

    private string? _text;

    /// <summary>
    ///     Text.
    /// </summary>
    public virtual string Text
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _text ?? string.Empty;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (string.Equals(value, _text, StringComparison.Ordinal))
            {
                return;
            }

            _text = value;
            _textElement.DisplayedText = _text;
        }
    }

    /// <summary>
    ///     Font.
    /// </summary>
    public GameFont? Font
    {
        get => _textElement.Font;
        set
        {
            if (value == _textElement.Font)
            {
                return;
            }

            _textElement.Font = value;
            fontInfo = $"{value?.GetName()},{value?.GetSize()}";

            if (mAutoSizeToContents)
            {
                SizeToContents();
            }

            Invalidate();
        }
    }

    /// <summary>
    /// Font Name
    /// </summary>
    public string FontName
    {
        get => _textElement.Font.GetName();
        set => Font = GameContentManager.Current?.GetFont(value, FontSize);
    }

    /// <summary>
    /// Font Size
    /// </summary>
    public int FontSize
    {
        get => _textElement?.Font?.GetSize() ?? 12;
        set => Font = GameContentManager.Current?.GetFont(FontName, value);
    }

    /// <summary>
    ///     Text color.
    /// </summary>
    public Color? TextColor
    {
        get => _textElement.Color ?? Color.White;
        set => _textElement.Color = value;
    }

    /// <summary>
    ///     Override text color (used by tooltips).
    /// </summary>
    public Color? TextColorOverride
    {
        get => _textElement.ColorOverride;
        set => _textElement.ColorOverride = value;
    }

    private string? _textOverride;

    /// <summary>
    ///     Text override - used to display different string.
    /// </summary>
    public string? TextOverride
    {
        get => _textOverride;
        set
        {
            if (string.Equals(value, _textOverride, StringComparison.Ordinal))
            {
                return;
            }

            _textOverride = value;
            _textElement.DisplayedText = _textOverride ?? _text;
        }
    }

    /// <summary>
    ///     Width of the text (in pixels).
    /// </summary>
    public int TextWidth => _textElement.Width;

    /// <summary>
    ///     Height of the text (in pixels).
    /// </summary>
    public int TextHeight => _textElement.Height;

    public int TextX => _textElement.X;

    public int TextY => _textElement.Y;

    /// <summary>
    ///     Text length (in characters).
    /// </summary>
    public int TextLength => _textElement.Length;

    public int TextRight => _textElement.Right;

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

    public override JObject GetJson(bool isRoot = default)
    {
        var obj = base.GetJson(isRoot);
        if (typeof(Label) == GetType())
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
        obj.Add("TextScale", _textElement.GetScale());

        return base.FixJson(obj);
    }

    public override void LoadJson(JToken obj, bool isRoot = default)
    {
        base.LoadJson(obj);
        if (typeof(Label) == GetType() && obj["BackgroundTemplate"] != null)
        {
            SetBackgroundTemplate(
                GameContentManager.Current.GetTexture(
                    Framework.Content.TextureType.Gui, (string)obj["BackgroundTemplate"]
                ), (string)obj["BackgroundTemplate"]
            );
        }

        if (obj["TextColor"] != null)
        {
            TextColor = Color.FromString((string)obj["TextColor"]);
        }

        mNormalTextColor = TextColor;
        if (obj["HoveredTextColor"] != null)
        {
            mHoverTextColor = Color.FromString((string)obj["HoveredTextColor"]);
        }

        if (obj["ClickedTextColor"] != null)
        {
            mClickedTextColor = Color.FromString((string)obj["ClickedTextColor"]);
        }

        mDisabledTextColor = Color.FromString((string)obj["DisabledTextColor"], new Color(255, 90, 90, 90));
        if (obj["TextAlign"] != null)
        {
            mAlign = (Pos)Enum.Parse(typeof(Pos), (string)obj["TextAlign"]);
        }

        if (obj["TextPadding"] != null)
        {
            TextPadding = Padding.FromString((string)obj["TextPadding"]);
        }

        if (obj["AutoSizeToContents"] != null)
        {
            mAutoSizeToContents = (bool)obj["AutoSizeToContents"];
        }

        var tokenFont = obj["Font"];
        if (tokenFont != null && tokenFont.Type != JTokenType.Null)
        {
            var stringFont = (string)tokenFont;
            if (!string.IsNullOrWhiteSpace(stringFont))
            {
                var fontArr = stringFont.Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
                );
                if (fontArr.Length > 1)
                {
                    fontInfo = stringFont;
                    try
                    {
                        Font = GameContentManager.Current.GetFont(fontArr[0], int.Parse(fontArr[1]));
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        if (obj["TextScale"] != null)
        {
            _textElement.SetScale((float)obj["TextScale"]);
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
            texture = GameContentManager.Current?.GetTexture(Framework.Content.TextureType.Gui, fileName);
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
        return new Point(_textElement.GetClosestCharacter(_textElement.CanvasPosToLocal(new Point(x, y))), 0);
    }

    /// <summary>
    ///     Sets the position of the internal text control.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    protected void SetTextPosition(int x, int y)
    {
        _textElement.SetPosition(x, y);
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
            x = Width - _textElement.Width - mTextPadding.Right - Padding.Right;
        }

        if (0 != (align & Pos.CenterH))
        {
            x = (int)(mTextPadding.Left +
                       Padding.Left +
                       (Width -
                        _textElement.Width -
                        mTextPadding.Left -
                        Padding.Left -
                        mTextPadding.Right -
                        Padding.Right) *
                       0.5f);
        }

        if (0 != (align & Pos.CenterV))
        {
            y = (int)(mTextPadding.Top +
                       Padding.Top +
                       (Height - _textElement.Height) * 0.5f -
                       mTextPadding.Bottom -
                       Padding.Bottom);
        }

        if (0 != (align & Pos.Bottom))
        {
            y = Height - _textElement.Height - mTextPadding.Bottom - Padding.Bottom;
        }

        _textElement.SetPosition(x, y);
    }

    /// <summary>
    ///     Sets the label text.
    /// </summary>
    /// <param name="text">Text to set.</param>
    /// <param name="doEvents">Determines whether to invoke "text changed" event.</param>
    public virtual void SetText(string text, bool doEvents = true)
    {
        if (string.Equals(_text, text, StringComparison.Ordinal))
        {
            return;
        }

        _text = text;
        _textElement.DisplayedText = text;

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
        _textElement.SetScale(scale);
        if (mAutoSizeToContents)
        {
            SizeToContents();
        }

        Invalidate();
        InvalidateParent();
    }

    public virtual void SizeToContents()
    {
        _textElement.SetPosition(mTextPadding.Left + Padding.Left, mTextPadding.Top + Padding.Top);

        if (!SetSize(
                _textElement.Width + Padding.Left + Padding.Right + mTextPadding.Left + mTextPadding.Right,
                _textElement.Height + Padding.Top + Padding.Bottom + mTextPadding.Top + mTextPadding.Bottom
            ))
        {
            return;
        }

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
        var p = _textElement.GetCharacterPosition(index);

        return new Point(p.X + _textElement.X, p.Y + _textElement.Y);
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

    public virtual Color? GetTextColor(ControlState state)
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
