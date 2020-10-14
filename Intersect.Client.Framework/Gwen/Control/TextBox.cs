using System;

using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Input;

using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Text box (editable).
    /// </summary>
    public class TextBox : Label
    {

        //Sound Effects
        private string mAddTextSound;

        protected Rectangle mCaretBounds;

        private int mCursorEnd;

        private int mCursorPos;

        protected float mLastInputTime;

        private int mMaxTextLength = -1;

        private string mRemoveTextSound;

        private bool mSelectAll;

        protected Rectangle mSelectionBounds;

        private string mSubmitSound;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TextBox(Base parent, string name = "") : base(parent, name)
        {
            AutoSizeToContents = false;
            SetSize(200, 20);

            MouseInputEnabled = true;
            KeyboardInputEnabled = true;

            Alignment = Pos.Left | Pos.CenterV;
            TextPadding = new Padding(4, 2, 4, 2);

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

        /// <summary>
        ///     Invoked when the text has changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> TextChanged;

        /// <summary>
        ///     Invoked when the submit key has been pressed.
        /// </summary>
        public event GwenEventHandler<EventArgs> SubmitPressed;

        /// <summary>
        ///     Determines whether the control can insert text at a given cursor position.
        /// </summary>
        /// <param name="text">Text to check.</param>
        /// <param name="position">Cursor position.</param>
        /// <returns>True if allowed.</returns>
        protected virtual bool IsTextAllowed(string text, int position)
        {
            if (mMaxTextLength >= 0 && this.Text.Length + text.Length > mMaxTextLength)
            {
                return false;
            }

            return true;
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

            if (TextChanged != null)
            {
                TextChanged.Invoke(this, EventArgs.Empty);
            }
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

        /// <summary>
        ///     Inserts text at current cursor position, erasing selection if any.
        /// </summary>
        /// <param name="text">Text to insert.</param>
        protected virtual void InsertText(string text)
        {
            // TODO: Make sure fits (implement maxlength)

            if (HasSelection)
            {
                EraseSelection(false);
            }

            if (mCursorPos > TextLength)
            {
                mCursorPos = TextLength;
            }

            if (!IsTextAllowed(text, mCursorPos))
            {
                return;
            }

            var str = Text;
            str = str.Insert(mCursorPos, text);
            SetText(str);

            mCursorPos += text.Length;
            mCursorEnd = mCursorPos;

            RefreshCursorBounds();

            base.PlaySound(mAddTextSound);
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
                skin.Renderer.DrawColor = mNormalTextColor != null ? mNormalTextColor : TextColorOverride;
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

        /// <summary>
        ///     Handler invoked on mouse double click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        protected override void OnMouseDoubleClickedLeft(int x, int y)
        {
            //base.OnMouseDoubleClickedLeft(x, y);
            OnSelectAll(this, EventArgs.Empty);
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

            if (!Input.InputHandler.IsShiftDown)
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

            if (!Input.InputHandler.IsShiftDown)
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

            if (!Input.InputHandler.IsShiftDown)
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

            if (!Input.InputHandler.IsShiftDown)
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
                return String.Empty;
            }

            var start = Math.Min(mCursorPos, mCursorEnd);
            var end = Math.Max(mCursorPos, mCursorEnd);

            var str = Text;

            return str.Substring(start, end - start);
        }

        /// <summary>
        ///     Deletes text.
        /// </summary>
        /// <param name="startPos">Starting cursor position.</param>
        /// <param name="length">Length in characters.</param>
        public virtual void DeleteText(int startPos, int length, bool playSound = true)
        {
            var str = Text;
            str = str.Remove(startPos, length);
            SetText(str);

            if (mCursorPos > startPos)
            {
                CursorPos = mCursorPos - length;
            }

            CursorEnd = mCursorPos;

            if (length > 0 && playSound)
            {
                base.PlaySound(mRemoveTextSound);
            }
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

        /// <summary>
        ///     Handler invoked on mouse click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected override void OnMouseClickedLeft(int x, int y, bool down, bool automated = false)
        {
            base.OnMouseClickedLeft(x, y, down);
            if (mSelectAll)
            {
                OnSelectAll(this, EventArgs.Empty);

                //m_SelectAll = false;
                return;
            }

            var c = GetClosestCharacter(x, y).X;

            if (down)
            {
                CursorPos = c;

                if (!Input.InputHandler.IsShiftDown)
                {
                    CursorEnd = c;
                }

                InputHandler.MouseFocus = this;
            }
            else
            {
                if (InputHandler.MouseFocus == this)
                {
                    CursorPos = c;
                    InputHandler.MouseFocus = null;
                }
            }
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
            var idealx = (int) (-caretPos + Width * 0.5f);

            // Don't show too much whitespace to the right
            if (idealx + TextWidth < Width - TextPadding.Right - Padding.Right)
            {
                idealx = -TextWidth + (Width - TextPadding.Right - Padding.Right);
            }

            // Or the left
            if (idealx > TextPadding.Left + Padding.Left)
            {
                idealx = TextPadding.Left + Padding.Left;
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

            RefreshCursorBounds();
        }

        /// <summary>
        ///     Handler for the return key.
        /// </summary>
        protected virtual void OnReturn()
        {
            if (SubmitPressed != null)
            {
                SubmitPressed.Invoke(this, EventArgs.Empty);
                base.PlaySound(mSubmitSound);
            }
        }

        public void SetMaxLength(int val)
        {
            mMaxTextLength = val;
        }

        public override JObject GetJson()
        {
            var obj = base.GetJson();
            obj.Add("AddTextSound", mAddTextSound);
            obj.Add("RemoveTextSound", mRemoveTextSound);
            obj.Add("SubmitSound", mSubmitSound);

            return base.FixJson(obj);
        }

        public override void LoadJson(JToken obj)
        {
            base.LoadJson(obj);
            if (obj["AddTextSound"] != null)
            {
                mAddTextSound = (string) obj["AddTextSound"];
            }

            if (obj["RemoveTextSound"] != null)
            {
                mRemoveTextSound = (string) obj["RemoveTextSound"];
            }

            if (obj["SubmitSound"] != null)
            {
                mSubmitSound = (string) obj["SubmitSound"];
            }
        }

    }

}
