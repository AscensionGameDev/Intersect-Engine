using System;
using System.Collections.Generic;
using System.Linq;

using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Input;

namespace Intersect.Client.Framework.Gwen.Control
{

    public class MultilineTextBox : Label
    {

        private readonly ScrollControl mScrollControl;

        protected Rectangle mCaretBounds;

        private Point mCursorEnd;

        private Point mCursorPos;

        private float mLastInputTime;

        private bool mSelectAll;

        private List<string> mTextLines = new List<string>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public MultilineTextBox(Base parent) : base(parent)
        {
            AutoSizeToContents = false;
            SetSize(200, 20);

            MouseInputEnabled = true;
            KeyboardInputEnabled = true;

            Alignment = Pos.Left | Pos.Top;
            TextPadding = new Padding(4, 2, 4, 2);

            mCursorPos = new Point(0, 0);
            mCursorEnd = new Point(0, 0);
            mSelectAll = false;

            TextColor = Color.FromArgb(255, 50, 50, 50); // TODO: From Skin

            IsTabable = false;
            AcceptTabs = true;

            mScrollControl = new ScrollControl(this);
            mScrollControl.Dock = Pos.Fill;
            mScrollControl.EnableScroll(true, true);
            mScrollControl.AutoHideBars = true;
            mScrollControl.Margin = Margin.One;
            mInnerPanel = mScrollControl;
            mText.Parent = mInnerPanel;
            mScrollControl.InnerPanel.BoundsChanged += new GwenEventHandler<EventArgs>(ScrollChanged);

            mTextLines.Add(String.Empty);

            // [halfofastaple] TODO Figure out where these numbers come from. See if we can remove the magic numbers.
            //	This should be as simple as 'm_ScrollControl.AutoSizeToContents = true' or 'm_ScrollControl.NoBounds()'
            mScrollControl.SetInnerSize(1000, 1000);

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

        private Point StartPoint
        {
            get
            {
                if (CursorPosition.Y == mCursorEnd.Y)
                {
                    return CursorPosition.X < CursorEnd.X ? CursorPosition : CursorEnd;
                }
                else
                {
                    return CursorPosition.Y < CursorEnd.Y ? CursorPosition : CursorEnd;
                }
            }
        }

        private Point EndPoint
        {
            get
            {
                if (CursorPosition.Y == mCursorEnd.Y)
                {
                    return CursorPosition.X > CursorEnd.X ? CursorPosition : CursorEnd;
                }
                else
                {
                    return CursorPosition.Y > CursorEnd.Y ? CursorPosition : CursorEnd;
                }
            }
        }

        /// <summary>
        ///     Indicates whether the text has active selection.
        /// </summary>
        public bool HasSelection => mCursorPos != mCursorEnd;

        /// <summary>
        ///     Get a point representing where the cursor physically appears on the screen.
        ///     Y is line number, X is character position on that line.
        /// </summary>
        public Point CursorPosition
        {
            get
            {
                if (mTextLines == null || mTextLines.Count() == 0)
                {
                    return new Point(0, 0);
                }

                var y = mCursorPos.Y;
                y = Math.Max(y, 0);
                y = Math.Min(y, mTextLines.Count() - 1);

                var x = mCursorPos
                    .X; //X may be beyond the last character, but we will want to draw it at the end of line.

                x = Math.Max(x, 0);
                x = Math.Min(x, mTextLines[y].Length);

                return new Point(x, y);
            }
            set
            {
                mCursorPos.X = value.X;
                mCursorPos.Y = value.Y;
                RefreshCursorBounds();
            }
        }

        /// <summary>
        ///     Get a point representing where the endpoint of text selection.
        ///     Y is line number, X is character position on that line.
        /// </summary>
        public Point CursorEnd
        {
            get
            {
                if (mTextLines == null || mTextLines.Count() == 0)
                {
                    return new Point(0, 0);
                }

                var y = mCursorEnd.Y;
                y = Math.Max(y, 0);
                y = Math.Min(y, mTextLines.Count() - 1);

                var x = mCursorEnd
                    .X; //X may be beyond the last character, but we will want to draw it at the end of line.

                x = Math.Max(x, 0);
                x = Math.Min(x, mTextLines[y].Length);

                return new Point(x, y);
            }
            set
            {
                mCursorEnd.X = value.X;
                mCursorEnd.Y = value.Y;
                RefreshCursorBounds();
            }
        }

        /// <summary>
        ///     Indicates whether the control will accept Tab characters as input.
        /// </summary>
        public bool AcceptTabs { get; set; }

        /// <summary>
        ///     Returns the number of lines that are in the Multiline Text Box.
        /// </summary>
        public int TotalLines => mTextLines.Count;

        /// <summary>
        ///     Gets and sets the text to display to the user. Each line is seperated by
        ///     an Environment.NetLine character.
        /// </summary>
        public override string Text
        {
            get
            {
                var ret = "";
                for (var i = 0; i < TotalLines; i++)
                {
                    ret += mTextLines[i];
                    if (i != TotalLines - 1)
                    {
                        ret += Environment.NewLine;
                    }
                }

                return ret;
            }
            set => base.Text = value;
        }

        /// <summary>
        ///     Invoked when the text has changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> TextChanged;

        public string GetTextLine(int index)
        {
            return mTextLines[index];
        }

        public void SetTextLine(int index, string value)
        {
            mTextLines[index] = value;
        }

        /// <summary>
        ///     Refreshes the cursor location and selected area when the inner panel scrolls
        /// </summary>
        /// <param name="control">The inner panel the text is embedded in</param>
        private void ScrollChanged(Base control, EventArgs args)
        {
            RefreshCursorBounds();
        }

        /// <summary>
        ///     Handler for text changed event.
        /// </summary>
        protected override void OnTextChanged()
        {
            base.OnTextChanged();
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
            //base.OnChar(chr);
            if (chr == '\t' && !AcceptTabs)
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
        protected void InsertText(string text)
        {
            // TODO: Make sure fits (implement maxlength)

            if (HasSelection)
            {
                EraseSelection();
            }

            var str = mTextLines[mCursorPos.Y];
            str = str.Insert(CursorPosition.X, text);
            mTextLines[mCursorPos.Y] = str;

            mCursorPos.X = CursorPosition.X + text.Length;
            mCursorEnd = mCursorPos;

            Invalidate();
            RefreshCursorBounds();
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

            var verticalOffset = 2 - mScrollControl.VerticalScroll;
            var verticalSize = 10 + 6;

            // Draw selection.. if selected..
            if (mCursorPos != mCursorEnd)
            {
                if (StartPoint.Y == EndPoint.Y)
                {
                    var pA = GetCharacterPosition(StartPoint);
                    var pB = GetCharacterPosition(EndPoint);

                    var selectionBounds = new Rectangle();
                    selectionBounds.X = Math.Min(pA.X, pB.X);
                    selectionBounds.Y = pA.Y - verticalOffset;
                    selectionBounds.Width = Math.Max(pA.X, pB.X) - selectionBounds.X;
                    selectionBounds.Height = verticalSize;

                    skin.Renderer.DrawColor = Color.FromArgb(200, 50, 170, 255);
                    skin.Renderer.DrawFilledRect(selectionBounds);
                }
                else
                {
                    /* Start */
                    var pA = GetCharacterPosition(StartPoint);
                    var pB = GetCharacterPosition(new Point(mTextLines[StartPoint.Y].Length, StartPoint.Y));

                    var selectionBounds = new Rectangle();
                    selectionBounds.X = Math.Min(pA.X, pB.X);
                    selectionBounds.Y = pA.Y - verticalOffset;
                    selectionBounds.Width = Math.Max(pA.X, pB.X) - selectionBounds.X;
                    selectionBounds.Height = verticalSize;

                    skin.Renderer.DrawColor = Color.FromArgb(200, 50, 170, 255);
                    skin.Renderer.DrawFilledRect(selectionBounds);

                    /* Middle */
                    for (var i = 1; i < EndPoint.Y - StartPoint.Y; i++)
                    {
                        pA = GetCharacterPosition(new Point(0, StartPoint.Y + i));
                        pB = GetCharacterPosition(new Point(mTextLines[StartPoint.Y + i].Length, StartPoint.Y + i));

                        selectionBounds = new Rectangle();
                        selectionBounds.X = Math.Min(pA.X, pB.X);
                        selectionBounds.Y = pA.Y - verticalOffset;
                        selectionBounds.Width = Math.Max(pA.X, pB.X) - selectionBounds.X;
                        selectionBounds.Height = verticalSize;

                        skin.Renderer.DrawColor = Color.FromArgb(200, 50, 170, 255);
                        skin.Renderer.DrawFilledRect(selectionBounds);
                    }

                    /* End */
                    pA = GetCharacterPosition(new Point(0, EndPoint.Y));
                    pB = GetCharacterPosition(EndPoint);

                    selectionBounds = new Rectangle();
                    selectionBounds.X = Math.Min(pA.X, pB.X);
                    selectionBounds.Y = pA.Y - verticalOffset;
                    selectionBounds.Width = Math.Max(pA.X, pB.X) - selectionBounds.X;
                    selectionBounds.Height = verticalSize;

                    skin.Renderer.DrawColor = Color.FromArgb(200, 50, 170, 255);
                    skin.Renderer.DrawFilledRect(selectionBounds);
                }
            }

            // Draw caret
            var time = Platform.Neutral.GetTimeInSeconds() - mLastInputTime;

            if (time % 1.0f <= 0.5f)
            {
                skin.Renderer.DrawColor = Color.Black;
                skin.Renderer.DrawFilledRect(mCaretBounds);
            }
        }

        protected void RefreshCursorBounds()
        {
            mLastInputTime = Platform.Neutral.GetTimeInSeconds();

            MakeCaretVisible();

            var pA = GetCharacterPosition(CursorPosition);
            var pB = GetCharacterPosition(mCursorEnd);

            //m_SelectionBounds.X = Math.Min(pA.X, pB.X);
            //m_SelectionBounds.Y = TextY - 1;
            //m_SelectionBounds.Width = Math.Max(pA.X, pB.X) - m_SelectionBounds.X;
            //m_SelectionBounds.Height = TextHeight + 2;

            mCaretBounds.X = pA.X;
            mCaretBounds.Y = pA.Y + 1;

            mCaretBounds.Y += mScrollControl.VerticalScroll;

            mCaretBounds.Width = 1;
            mCaretBounds.Height = 10 + 2;

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
            mCursorEnd = new Point(0, 0);
            mCursorPos = new Point(mTextLines.Last().Length, mTextLines.Count());

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
            if (down)
            {
                return true;
            }

            //Split current string, putting the rhs on a new line
            var currentLine = mTextLines[mCursorPos.Y];
            var lhs = currentLine.Substring(0, CursorPosition.X);
            var rhs = currentLine.Substring(CursorPosition.X);

            mTextLines[mCursorPos.Y] = lhs;
            mTextLines.Insert(mCursorPos.Y + 1, rhs);

            OnKeyDown(true);
            OnKeyHome(true);

            if (mCursorPos.Y == TotalLines - 1)
            {
                mScrollControl.ScrollToBottom();
            }

            Invalidate();
            RefreshCursorBounds();

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
            if (!down)
            {
                return true;
            }

            if (HasSelection)
            {
                EraseSelection();

                return true;
            }

            if (mCursorPos.X == 0)
            {
                if (mCursorPos.Y == 0)
                {
                    return true; //Nothing left to delete
                }
                else
                {
                    var lhs = mTextLines[mCursorPos.Y - 1];
                    var rhs = mTextLines[mCursorPos.Y];
                    mTextLines.RemoveAt(mCursorPos.Y);
                    OnKeyUp(true);
                    OnKeyEnd(true);
                    mTextLines[mCursorPos.Y] = lhs + rhs;
                }
            }
            else
            {
                var currentLine = mTextLines[mCursorPos.Y];
                var lhs = currentLine.Substring(0, CursorPosition.X - 1);
                var rhs = currentLine.Substring(CursorPosition.X);
                mTextLines[mCursorPos.Y] = lhs + rhs;
                OnKeyLeft(true);
            }

            Invalidate();
            RefreshCursorBounds();

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
            if (!down)
            {
                return true;
            }

            if (HasSelection)
            {
                EraseSelection();

                return true;
            }

            if (mCursorPos.X == mTextLines[mCursorPos.Y].Length)
            {
                if (mCursorPos.Y == mTextLines.Count - 1)
                {
                    return true; //Nothing left to delete
                }
                else
                {
                    var lhs = mTextLines[mCursorPos.Y];
                    var rhs = mTextLines[mCursorPos.Y + 1];
                    mTextLines.RemoveAt(mCursorPos.Y + 1);
                    OnKeyEnd(true);
                    mTextLines[mCursorPos.Y] = lhs + rhs;
                }
            }
            else
            {
                var currentLine = mTextLines[mCursorPos.Y];
                var lhs = currentLine.Substring(0, CursorPosition.X);
                var rhs = currentLine.Substring(CursorPosition.X + 1);
                mTextLines[mCursorPos.Y] = lhs + rhs;
            }

            Invalidate();
            RefreshCursorBounds();

            return true;
        }

        /// <summary>
        ///     Handler for Up Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeyUp(bool down)
        {
            if (!down)
            {
                return true;
            }

            if (mCursorPos.Y > 0)
            {
                mCursorPos.Y -= 1;
            }

            if (!Input.InputHandler.IsShiftDown)
            {
                mCursorEnd = mCursorPos;
            }

            Invalidate();
            RefreshCursorBounds();

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
            if (!down)
            {
                return true;
            }

            if (mCursorPos.Y < TotalLines - 1)
            {
                mCursorPos.Y += 1;
            }

            if (!Input.InputHandler.IsShiftDown)
            {
                mCursorEnd = mCursorPos;
            }

            Invalidate();
            RefreshCursorBounds();

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
            if (!down)
            {
                return true;
            }

            if (mCursorPos.X > 0)
            {
                mCursorPos.X = Math.Min(mCursorPos.X - 1, mTextLines[mCursorPos.Y].Length);
            }
            else
            {
                if (mCursorPos.Y > 0)
                {
                    OnKeyUp(down);
                    OnKeyEnd(down);
                }
            }

            if (!Input.InputHandler.IsShiftDown)
            {
                mCursorEnd = mCursorPos;
            }

            Invalidate();
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
            if (!down)
            {
                return true;
            }

            if (mCursorPos.X < mTextLines[mCursorPos.Y].Length)
            {
                mCursorPos.X = Math.Min(mCursorPos.X + 1, mTextLines[mCursorPos.Y].Length);
            }
            else
            {
                if (mCursorPos.Y < mTextLines.Count - 1)
                {
                    OnKeyDown(down);
                    OnKeyHome(down);
                }
            }

            if (!Input.InputHandler.IsShiftDown)
            {
                mCursorEnd = mCursorPos;
            }

            Invalidate();
            RefreshCursorBounds();

            return true;
        }

        /// <summary>
        ///     Handler for Home Key keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeyHome(bool down)
        {
            if (!down)
            {
                return true;
            }

            mCursorPos.X = 0;

            if (!Input.InputHandler.IsShiftDown)
            {
                mCursorEnd = mCursorPos;
            }

            Invalidate();
            RefreshCursorBounds();

            return true;
        }

        /// <summary>
        ///     Handler for End Key keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeyEnd(bool down)
        {
            if (!down)
            {
                return true;
            }

            mCursorPos.X = mTextLines[mCursorPos.Y].Length;

            if (!Input.InputHandler.IsShiftDown)
            {
                mCursorEnd = mCursorPos;
            }

            Invalidate();
            RefreshCursorBounds();

            return true;
        }

        /// <summary>
        ///     Handler for Tab Key keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeyTab(bool down)
        {
            if (!AcceptTabs)
            {
                return base.OnKeyTab(down);
            }

            if (!down)
            {
                return false;
            }

            OnChar('\t');

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

            var str = String.Empty;

            if (StartPoint.Y == EndPoint.Y)
            {
                var start = StartPoint.X;
                var end = EndPoint.X;

                str = mTextLines[mCursorPos.Y];
                str = str.Substring(start, end - start);
            }
            else
            {
                str = String.Empty;
                str += mTextLines[StartPoint.Y].Substring(StartPoint.X); //Copy start
                for (var i = 1; i < EndPoint.Y - StartPoint.Y; i++)
                {
                    str += mTextLines[StartPoint.Y + i]; //Copy middle
                }

                str += mTextLines[EndPoint.Y].Substring(0, EndPoint.X); //Copy end
            }

            return str;
        }

        //[halfofastaple] TODO Implement this and use it. The end user can work around not having it, but it is terribly convenient.
        //	See the delete key handler for help. Eventually, the delete key should use this.
        ///// <summary>
        ///// Deletes text.
        ///// </summary>
        ///// <param name="startPos">Starting cursor position.</param>
        ///// <param name="length">Length in characters.</param>
        //public void DeleteText(Point StartPos, int length) {
        //    /* Single Line Delete */
        //    if (StartPos.X + length <= m_TextLines[StartPos.Y].Length) {
        //        string str = m_TextLines[StartPos.Y];
        //        str = str.Remove(StartPos.X, length);
        //        m_TextLines[StartPos.Y] = str;

        //        if (CursorPosition.X > StartPos.X) {
        //            m_CursorPos.X = CursorPosition.X - length;
        //        }

        //        m_CursorEnd = m_CursorPos;
        //    /* Multiline Delete */
        //    } else {

        //    }
        //}

        /// <summary>
        ///     Deletes selected text.
        /// </summary>
        public void EraseSelection()
        {
            if (StartPoint.Y == EndPoint.Y)
            {
                var start = StartPoint.X;
                var end = EndPoint.X;

                mTextLines[StartPoint.Y] = mTextLines[StartPoint.Y].Remove(start, end - start);
            }
            else
            {
                /* Remove Start */
                if (StartPoint.X < mTextLines[StartPoint.Y].Length)
                {
                    mTextLines[StartPoint.Y] = mTextLines[StartPoint.Y].Remove(StartPoint.X);
                }

                /* Remove Middle */
                for (var i = 1; i < EndPoint.Y - StartPoint.Y; i++)
                {
                    mTextLines.RemoveAt(StartPoint.Y + 1);
                }

                /* Remove End */
                if (EndPoint.X < mTextLines[StartPoint.Y + 1].Length)
                {
                    mTextLines[StartPoint.Y] += mTextLines[StartPoint.Y + 1].Substring(EndPoint.X);
                }

                mTextLines.RemoveAt(StartPoint.Y + 1);
            }

            // Move the cursor to the start of the selection, 
            // since the end is probably outside of the string now.
            mCursorPos = StartPoint;
            mCursorEnd = StartPoint;

            Invalidate();
            RefreshCursorBounds();
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

            var coords = GetClosestCharacter(x, y);

            if (down)
            {
                CursorPosition = coords;

                if (!Input.InputHandler.IsShiftDown)
                {
                    CursorEnd = coords;
                }

                InputHandler.MouseFocus = this;
            }
            else
            {
                if (InputHandler.MouseFocus == this)
                {
                    CursorPosition = coords;
                    InputHandler.MouseFocus = null;
                }
            }

            Invalidate();
            RefreshCursorBounds();
        }

        /// <summary>
        ///     Returns index of the character closest to specified point (in canvas coordinates).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected override Point GetClosestCharacter(int px, int py)
        {
            var p = CanvasPosToLocal(new Point(px, py));
            var distance = Double.MaxValue;
            var best = new Point(0, 0);
            var sub = String.Empty;

            /* Find the appropriate Y row (always pick whichever y the mouse currently is on) */
            for (var y = 0; y < mTextLines.Count(); y++)
            {
                sub += mTextLines[y] + Environment.NewLine;
                var cp = Skin.Renderer.MeasureText(Font, sub);

                double yDist = Math.Abs(cp.Y - p.Y);
                if (yDist < distance)
                {
                    distance = yDist;
                    best.Y = y;
                }
            }

            /* Find the best X row, closest char */
            sub = String.Empty;
            distance = Double.MaxValue;
            for (var x = 0; x <= mTextLines[best.Y].Count(); x++)
            {
                if (x < mTextLines[best.Y].Count())
                {
                    sub += mTextLines[best.Y][x];
                }
                else
                {
                    sub += " ";
                }

                var cp = Skin.Renderer.MeasureText(Font, sub);

                double xDiff = Math.Abs(cp.X - p.X);

                if (xDiff < distance)
                {
                    distance = xDiff;
                    best.X = x;
                }
            }

            return best;
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

            var c = GetClosestCharacter(x, y);

            CursorPosition = c;

            Invalidate();
            RefreshCursorBounds();
        }

        protected virtual void MakeCaretVisible()
        {
            var caretPos = GetCharacterPosition(CursorPosition).X - TextX;

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
        ///     Handler invoked when control children's bounds change.
        /// </summary>
        /// <param name="oldChildBounds"></param>
        /// <param name="child"></param>
        protected override void OnChildBoundsChanged(Rectangle oldChildBounds, Base child)
        {
            if (mScrollControl != null)
            {
                mScrollControl.UpdateScrollBars();
            }
        }

        /// <summary>
        ///     Sets the label text.
        /// </summary>
        /// <param name="str">Text to set.</param>
        /// <param name="doEvents">Determines whether to invoke "text changed" event.</param>
        public override void SetText(string str, bool doEvents = true)
        {
            var easySplit = str.Replace("\r\n", "\n").Replace("\r", "\n");
            var lines = easySplit.Split('\n');

            mTextLines = new List<string>(lines);

            Invalidate();
            RefreshCursorBounds();
        }

        /// <summary>
        ///     Invalidates the control.
        /// </summary>
        /// <remarks>
        ///     Causes layout, repaint, invalidates cached texture.
        /// </remarks>
        public override void Invalidate()
        {
            if (mText != null)
            {
                mText.String = Text;
            }

            if (AutoSizeToContents)
            {
                SizeToContents();
            }

            base.Invalidate();
            InvalidateParent();
            OnTextChanged();
        }

        private Point GetCharacterPosition(Point cursorPosition)
        {
            if (mTextLines.Count == 0)
            {
                return new Point(0, 0);
            }

            var currLine = mTextLines[cursorPosition.Y]
                .Substring(0, Math.Min(cursorPosition.X, mTextLines[cursorPosition.Y].Length));

            var sub = "";
            for (var i = 0; i < cursorPosition.Y; i++)
            {
                sub += mTextLines[i] + "\n";
            }

            var p = new Point(Skin.Renderer.MeasureText(Font, currLine).X, Skin.Renderer.MeasureText(Font, sub).Y);

            return new Point(p.X + mText.X, p.Y + mText.Y + TextPadding.Top);
        }

        protected override bool OnMouseWheeled(int delta)
        {
            return mScrollControl.InputMouseWheeled(delta);
        }

    }

}
