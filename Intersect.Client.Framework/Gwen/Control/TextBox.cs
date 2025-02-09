using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Text box (editable).
/// </summary>
public partial class TextBox : Label
{
    public enum Sounds
    {
        AddText,
        RemoveText,
        Submit,
    }

    //Sound Effects

    protected Rectangle mCaretBounds;

    private int mCursorEnd;

    private int mCursorPos;

    protected float mLastInputTime;

    private int mMaxmimumLength = -1;


    private bool mSelectAll;

    protected Rectangle mSelectionBounds;

    private string? _soundNameAddText;
    private string? _soundNameRemoveText;
    private string? _soundNameSubmit;

    private readonly Text _placeholder;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TextBox" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name"></param>
    public TextBox(Base parent, string? name = default) : base(parent: parent, name: name)
    {
        AutoSizeToContents = false;
        SetSize(200, 20);

        MouseInputEnabled = true;
        KeyboardInputEnabled = true;

        Padding = new Padding(4, 2, 4, 2);
        TextAlign = Pos.Left | Pos.CenterV;

        mCursorPos = 0;
        mCursorEnd = 0;
        mSelectAll = false;

        TextColor = Color.FromArgb(255, 50, 50, 50); // TODO: From Skin

        IsTabable = true;

        // Some platforms it works with spaces, others without.. so why not both?
        AddAccelerator("Ctrl + C", OnCopy);
        AddAccelerator("Ctrl + X", OnCut);
        AddAccelerator("Ctrl + V", OnPaste);
        AddAccelerator("Ctrl + A", OnSelectAll);
        AddAccelerator("Ctrl+C", OnCopy);
        AddAccelerator("Ctrl+X", OnCut);
        AddAccelerator("Ctrl+V", OnPaste);
        AddAccelerator("Ctrl+A", OnSelectAll);

        _placeholder = new Text(this)
        {
            ColorOverride = new Color(255, 143, 143, 143),
            IsVisible = false,
        };
    }

    protected override bool AccelOnlyFocus => true;

    protected override bool NeedsInputChars => true;

    /// <summary>
    ///     Determines whether text should be selected when the control is focused.
    /// </summary>
    public bool SelectAllOnFocus
    {
        get => mSelectAll;
        set
        {
            mSelectAll = value;
            if (value)
            {
                OnSelectAll(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    ///     Indicates whether the text has active selection.
    /// </summary>
    public virtual bool HasSelection => mCursorPos != mCursorEnd;

    /// <summary>
    ///     Current cursor position (character index).
    /// </summary>
    public int CursorPos
    {
        get => mCursorPos;
        set
        {
            if (mCursorPos == value)
            {
                return;
            }

            mCursorPos = value;
            RefreshCursorBounds();
        }
    }

    public int CursorEnd
    {
        get => mCursorEnd;
        set
        {
            if (mCursorEnd == value)
            {
                return;
            }

            mCursorEnd = value;
            RefreshCursorBounds();
        }
    }

    public override GameFont? Font
    {
        get => base.Font;
        set
        {
            base.Font = value;
            _placeholder.Font = Font;
        }
    }

    public int MaximumLength { get => mMaxmimumLength; set => mMaxmimumLength = value; }

    public string? PlaceholderText
    {
        get => _placeholder.DisplayedText;
        set => _placeholder.DisplayedText = value;
    }

    /// <summary>
    ///     Invoked when the text has changed.
    /// </summary>
    public event GwenEventHandler<EventArgs>? TextChanged;

    /// <summary>
    ///     Invoked when the submit key has been pressed.
    /// </summary>
    public event GwenEventHandler<EventArgs>? SubmitPressed;

    /// <summary>
    ///     Determines whether the control can insert text at a given cursor position.
    /// </summary>
    /// <param name="text">Text to check.</param>
    /// <param name="position">Cursor position.</param>
    /// <returns>True if allowed.</returns>
    protected virtual bool IsTextAllowed(string text, int position)
    {
        return MaximumLength < 0 || Text.Length + (text?.Length ?? 0) <= MaximumLength;
    }

    /// <summary>
    ///     Renders the focus overlay.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void RenderFocus(Skin.Base skin)
    {
        // nothing
    }

    /// <summary>
    ///     Handler for text changed event.
    /// </summary>
    protected override void OnTextChanged()
    {
        base.OnTextChanged();

        if (mCursorPos > TextLength)
        {
            mCursorPos = TextLength;
        }

        if (mCursorEnd > TextLength)
        {
            mCursorEnd = TextLength;
        }

        UpdatePlaceholder();

        RefreshCursorBounds();

        TextChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///     Handler for character input event.
    /// </summary>
    /// <param name="chr">Character typed.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnChar(char chr)
    {
        base.OnChar(chr);

        if (chr == '\t')
        {
            return false;
        }

        InsertText(chr.ToString());

        return true;
    }

    public void SetSound(Sounds sound, string? name)
    {
        switch (sound)
        {
            case Sounds.AddText:
                _soundNameAddText = name;
                break;
            case Sounds.RemoveText:
                _soundNameRemoveText = name;
                break;
            case Sounds.Submit:
                _soundNameSubmit = name;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(sound), sound, null);
        }
    }

    /// <summary>
    ///     Inserts text at current cursor position, erasing selection if any.
    /// </summary>
    /// <param name="text">Text to insert.</param>
    protected virtual void InsertText(string text)
    {
        ReplaceSelection(text);
        base.PlaySound(_soundNameAddText);
    }

    private void ValidateCursor()
    {
        var start = Math.Min(mCursorPos, mCursorEnd);
        var end = Math.Max(mCursorPos, mCursorEnd);
        mCursorPos = Math.Min(start, TextLength);
        mCursorEnd = Math.Min(end, TextLength);
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
            skin.DrawTextBox(this);
        }

        if (!HasFocus)
        {
            return;
        }

        // Draw selection.. if selected..
        if (mCursorPos != mCursorEnd)
        {
            skin.Renderer.DrawColor = Color.FromArgb(200, 50, 170, 255);
            skin.Renderer.DrawFilledRect(mSelectionBounds);
        }

        // Draw caret
        var time = Platform.Neutral.GetTimeInSeconds() - mLastInputTime;

        if (time % 1.0f <= 0.5f)
        {
            skin.Renderer.DrawColor = TextColorOverride is { A: > 0 } textColorOverride
                ? textColorOverride
                : TextColor ?? Color.White;
            skin.Renderer.DrawFilledRect(mCaretBounds);
        }
    }

    protected virtual void RefreshCursorBounds()
    {
        mLastInputTime = Platform.Neutral.GetTimeInSeconds();

        MakeCaretVisible();

        var pA = GetCharacterPosition(mCursorPos);
        var pB = GetCharacterPosition(mCursorEnd);

        mSelectionBounds.X = Math.Min(pA.X, pB.X);
        mSelectionBounds.Y = TextY - 1;
        mSelectionBounds.Width = Math.Max(pA.X, pB.X) - mSelectionBounds.X;
        mSelectionBounds.Height = TextHeight + 2;

        mCaretBounds.X = pA.X;
        mCaretBounds.Y = TextY - 1;
        mCaretBounds.Width = 1;
        mCaretBounds.Height = TextHeight + 2;

        Redraw();
    }

    /// <summary>
    ///     Handler for Paste event.
    /// </summary>
    /// <param name="from">Source control.</param>
    protected override void OnPaste(Base from, EventArgs args)
    {
        base.OnPaste(from, args);

        if (IsDisabledByTree)
        {
            return;
        }

        InsertText(Platform.Neutral.GetClipboardText());
    }

    /// <summary>
    ///     Handler for Copy event.
    /// </summary>
    /// <param name="from">Source control.</param>
    protected override void OnCopy(Base from, EventArgs args)
    {
        if (!HasSelection)
        {
            return;
        }

        base.OnCopy(from, args);

        Platform.Neutral.SetClipboardText(GetSelection());
    }

    /// <summary>
    ///     Handler for Cut event.
    /// </summary>
    /// <param name="from">Source control.</param>
    protected override void OnCut(Base from, EventArgs args)
    {
        if (IsDisabledByTree)
        {
            OnCopy(from, args);
            return;
        }

        if (!HasSelection)
        {
            return;
        }

        base.OnCut(from, args);

        Platform.Neutral.SetClipboardText(GetSelection());
        EraseSelection();
    }

    /// <summary>
    ///     Handler for Select All event.
    /// </summary>
    /// <param name="from">Source control.</param>
    protected override void OnSelectAll(Base from, EventArgs args)
    {
        //base.OnSelectAll(from);
        mCursorEnd = 0;
        mCursorPos = TextLength;

        RefreshCursorBounds();
    }

    protected override void OnMouseDoubleClicked(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseDoubleClicked(mouseButton, mousePosition, userAction);

        if (mouseButton == MouseButton.Left)
        {
            OnSelectAll(this, EventArgs.Empty);
        }
    }

    /// <summary>
    ///     Handler for Return keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyReturn(bool down)
    {
        base.OnKeyReturn(down);
        if (down)
        {
            return true;
        }

        OnReturn();

        // Try to move to the next control, as if tab had been pressed
        OnKeyTab(true);

        // If we still have focus, blur it.
        if (HasFocus)
        {
            Blur();
        }

        return true;
    }

    /// <summary>
    ///     Handler for Backspace keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyBackspace(bool down)
    {
        base.OnKeyBackspace(down);

        if (!down)
        {
            return true;
        }

        if (HasSelection)
        {
            EraseSelection();

            return true;
        }

        if (mCursorPos == 0)
        {
            return true;
        }

        DeleteText(mCursorPos - 1, 1);

        return true;
    }

    /// <summary>
    ///     Handler for Delete keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyDelete(bool down)
    {
        base.OnKeyDelete(down);

        if (!down)
        {
            return true;
        }

        if (HasSelection)
        {
            EraseSelection();

            return true;
        }

        if (mCursorPos >= TextLength)
        {
            return true;
        }

        DeleteText(mCursorPos, 1);

        return true;
    }

    /// <summary>
    ///     Handler for Left Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyLeft(bool down)
    {
        base.OnKeyLeft(down);
        if (!down)
        {
            return true;
        }

        if (mCursorPos > 0)
        {
            mCursorPos--;
        }

        if (!InputHandler.IsShiftDown)
        {
            mCursorEnd = mCursorPos;
        }

        RefreshCursorBounds();

        return true;
    }

    /// <summary>
    ///     Handler for Right Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyRight(bool down)
    {
        base.OnKeyRight(down);
        if (!down)
        {
            return true;
        }

        if (mCursorPos < TextLength)
        {
            mCursorPos++;
        }

        if (!InputHandler.IsShiftDown)
        {
            mCursorEnd = mCursorPos;
        }

        RefreshCursorBounds();

        return true;
    }

    /// <summary>
    ///     Handler for Home keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyHome(bool down)
    {
        base.OnKeyHome(down);
        if (!down)
        {
            return true;
        }

        mCursorPos = 0;

        if (!InputHandler.IsShiftDown)
        {
            mCursorEnd = mCursorPos;
        }

        RefreshCursorBounds();

        return true;
    }

    /// <summary>
    ///     Handler for End keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyEnd(bool down)
    {
        base.OnKeyEnd(down);
        mCursorPos = TextLength;

        if (!InputHandler.IsShiftDown)
        {
            mCursorEnd = mCursorPos;
        }

        RefreshCursorBounds();

        return true;
    }

    /// <summary>
    ///     Returns currently selected text.
    /// </summary>
    /// <returns>Current selection.</returns>
    public string GetSelection()
    {
        if (!HasSelection)
        {
            return string.Empty;
        }

        var start = Math.Min(mCursorPos, mCursorEnd);
        var end = Math.Max(mCursorPos, mCursorEnd);

        return Text?.Substring(start, end - start) ?? string.Empty;
    }

    /// <summary>
    ///     Deletes text.
    /// </summary>
    /// <param name="startPos">Starting cursor position.</param>
    /// <param name="length">Length in characters.</param>
    public virtual void DeleteText(int startPos, int length, bool playSound = true) => ReplaceText(startPos, length, string.Empty, playSound);

    public virtual void ReplaceText(int startPos, int length, string? replacement, bool playSound = true)
    {
        try
        {
            var text = Text ?? string.Empty;
            if (startPos < 0)
            {
                if (length > -startPos)
                {
                    length += startPos;
                }
                else
                {
                    length = 0;
                }

                startPos = 0;

                mCursorPos = Math.Max(startPos, mCursorPos);
            }

            if (length > 0)
            {
                text = text.Remove(startPos, length);
            }

            text = text.Insert(startPos, replacement ?? string.Empty);

            SetText(text);

            if (mCursorPos > startPos)
            {
                CursorPos = Math.Max(0, mCursorPos) + (replacement?.Length ?? 0) - length;
            }

            CursorEnd = mCursorPos;

            if (length > 0 && playSound)
            {
                PlaySound(_soundNameRemoveText);
            }
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Failed to replace {Length} characters in '{Text}' starting at {StartPosition} with '{ReplacementText}",
                length,
                Text,
                startPos,
                replacement
            );
        }
    }

    public virtual void ReplaceSelection(string? replacement, bool playSound = true)
    {
        ValidateCursor();

        var text = Text ?? string.Empty;

        var start = Math.Min(mCursorPos, mCursorEnd);

        var currentLength = text.Length;
        if (currentLength > 0 && start < 0)
        {
            // Make sure that start is not more negative than the text length
            start = Math.Max(-currentLength, start);

            // Treat the negative start as an offset from the end
            start += currentLength;
        }

        // Bound the end to no earlier than the start
        var end = Math.Max(start, Math.Max(mCursorPos, mCursorEnd));

        // How much text are we deleting?
        var deletionLength = end - start;

        // How long is the remaining text going to be after deletion?
        var textLength = currentLength - deletionLength;

        // What is the string length limit (below 0 maximum length is "unlimited")
        var maximumLength = MaximumLength < 0 ? int.MaxValue : MaximumLength;

        // How much text can we insert before reaching the maximum length?
        var maximumReplacementLength = maximumLength - textLength;

        // This number should never be less than 0
        maximumReplacementLength = Math.Max(0, maximumReplacementLength);

        // How long is the text we are inserting in place of the selection?
        var replacementLength = replacement?.Length ?? 0;

        // Bound it to the limit
        replacementLength = Math.Min(replacementLength, maximumReplacementLength);

        // Get the replacement substring
        var actualReplacement = replacement?[..replacementLength];

        ReplaceText(start, deletionLength, actualReplacement, playSound);

        // Move the cursor, reset to 0 length cursor
        mCursorPos += replacementLength;
        mCursorEnd = mCursorPos;

        RefreshCursorBounds();
    }

    /// <summary>
    ///     Deletes selected text.
    /// </summary>
    public virtual void EraseSelection(bool playSound = true)
    {
        var start = Math.Min(mCursorPos, mCursorEnd);
        var end = Math.Max(mCursorPos, mCursorEnd);

        DeleteText(start, end - start, playSound);

        // Move the cursor to the start of the selection,
        // since the end is probably outside of the string now.
        mCursorPos = start;
        mCursorEnd = start;
    }

    protected override void OnMouseDown(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseDown(mouseButton, mousePosition, userAction);

        if (mouseButton != MouseButton.Left)
        {
            return;
        }

        if (mSelectAll)
        {
            OnSelectAll(this, EventArgs.Empty);

            //m_SelectAll = false;
            return;
        }

        var closestCharacterCursor = GetClosestCharacter(mousePosition).X;
        CursorPos = closestCharacterCursor;

        if (!InputHandler.IsShiftDown)
        {
            CursorEnd = closestCharacterCursor;
        }

        InputHandler.MouseFocus = this;
    }

    protected override void OnMouseUp(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseUp(mouseButton, mousePosition, userAction);

        if (mouseButton != MouseButton.Left)
        {
            return;
        }

        if (InputHandler.MouseFocus != this)
        {
            return;
        }

        var closestCharacterCursor = GetClosestCharacter(mousePosition).X;

        CursorPos = closestCharacterCursor;
        InputHandler.MouseFocus = null;
    }

    /// <summary>
    ///     Handler invoked on mouse moved event.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <param name="dx">X change.</param>
    /// <param name="dy">Y change.</param>
    protected override void OnMouseMoved(int x, int y, int dx, int dy)
    {
        base.OnMouseMoved(x, y, dx, dy);
        if (InputHandler.MouseFocus != this)
        {
            return;
        }

        var c = GetClosestCharacter(x, y).X;

        CursorPos = c;
    }

    protected virtual void MakeCaretVisible()
    {
        var caretPos = GetCharacterPosition(mCursorPos).X - TextX;

        // If the caret is already in a semi-good position, leave it.
        {
            var realCaretPos = caretPos + TextX;
            if (realCaretPos > Width * 0.1f && realCaretPos < Width * 0.9f)
            {
                return;
            }
        }

        // The ideal position is for the caret to be right in the middle
        var idealx = (int)(-caretPos + Width * 0.5f);

        // Don't show too much whitespace to the right
        if (idealx + TextWidth < Width - Padding.Right)
        {
            idealx = -TextWidth + (Width - Padding.Right);
        }

        // Or the left
        if (idealx > Padding.Left)
        {
            idealx = Padding.Left;
        }

        SetTextPosition(idealx, TextY);
    }

    /// <summary>
    ///     Lays out the control's interior according to alignment, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Layout(Skin.Base skin)
    {
        base.Layout(skin);

        UpdatePlaceholder();

        RefreshCursorBounds();
    }

    private void UpdatePlaceholder()
    {
        _placeholder.IsVisible = string.IsNullOrEmpty(Text) && !string.IsNullOrWhiteSpace(PlaceholderText);
        AlignTextElement(_placeholder);
    }

    // Textbox only uses Normal and Disabled
    public override void UpdateColors()
    {
        var textColor = GetTextColor(ComponentState.Normal) ?? Skin.Colors.Label.Normal;
        if (IsDisabledByTree)
        {
            textColor = GetTextColor(ComponentState.Disabled) ?? Skin.Colors.Label.Disabled;
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (textColor == null)
        {
            ApplicationContext.CurrentContext.Logger.LogError(
                "Text color for the current control state of '{ComponentName}' is somehow null IsDisabled={IsDisabled} IsActive={IsActive} IsHovered={IsHovered}",
                CanonicalName,
                IsDisabled,
                IsActive,
                IsHovered
            );

            textColor = new Color(r: 255, g: 0, b: 255);
        }

        TextColor = textColor;
    }

    /// <summary>
    ///     Handler for the return key.
    /// </summary>
    protected virtual void OnReturn()
    {
        SubmitPressed?.Invoke(this, EventArgs.Empty);
        base.PlaySound(_soundNameSubmit);
    }

    public void SetMaxLength(int val)
    {
        MaximumLength = val;
    }

    public override JObject? GetJson(bool isRoot = false, bool onlySerializeIfNotEmpty = false)
    {
        var serializedProperties = base.GetJson(isRoot, onlySerializeIfNotEmpty);
        if (serializedProperties is null)
        {
            return null;
        }

        serializedProperties.Add("AddTextSound", _soundNameAddText);
        serializedProperties.Add("RemoveTextSound", _soundNameRemoveText);
        serializedProperties.Add("SubmitSound", _soundNameSubmit);

        return base.FixJson(serializedProperties);
    }

    public override void LoadJson(JToken obj, bool isRoot = default)
    {
        base.LoadJson(obj);
        if (obj["AddTextSound"] != null)
        {
            _soundNameAddText = (string)obj["AddTextSound"];
        }

        if (obj["RemoveTextSound"] != null)
        {
            _soundNameRemoveText = (string)obj["RemoveTextSound"];
        }

        if (obj["SubmitSound"] != null)
        {
            _soundNameSubmit = (string)obj["SubmitSound"];
        }
    }

}
