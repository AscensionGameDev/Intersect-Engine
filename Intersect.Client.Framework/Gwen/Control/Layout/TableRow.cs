using System;

using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.EventArguments;

namespace Intersect.Client.Framework.Gwen.Control.Layout
{

    /// <summary>
    ///     Single table row.
    /// </summary>
    public class TableRow : Base
    {

        // [omeg] todo: get rid of this
        public const int MAX_COLUMNS = 5;

        private readonly Label[] mColumns;

        protected string mClickSound;

        private int mColumnCount;

        private bool mEvenRow;

        //Sound Effects
        protected string mHoverSound;

        protected string mRightClickSound;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TableRow" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TableRow(Base parent) : base(parent)
        {
            mColumns = new Label[MAX_COLUMNS];
            mColumnCount = 0;
            KeyboardInputEnabled = true;
            this.Clicked += TableRow_Clicked;
            this.RightClicked += TableRow_RightClicked;
            this.HoverEnter += TableRow_HoverEnter;
        }

        public string HoverSound
        {
            get => mHoverSound;
            set => mHoverSound = value;
        }

        public string ClickSound
        {
            get => mClickSound;
            set => mClickSound = value;
        }

        public string RightClickSound
        {
            get => mRightClickSound;
            set => mRightClickSound = value;
        }

        /// <summary>
        ///     Column count.
        /// </summary>
        public int ColumnCount
        {
            get => mColumnCount;
            set => SetColumnCount(value);
        }

        /// <summary>
        ///     Indicates whether the row is even or odd (used for alternate coloring).
        /// </summary>
        public bool EvenRow
        {
            get => mEvenRow;
            set => mEvenRow = value;
        }

        /// <summary>
        ///     Text of the first column.
        /// </summary>
        public string Text
        {
            get => GetText(0);
            set => SetCellText(0, value);
        }

        private void TableRow_HoverEnter(Base sender, EventArgs arguments)
        {
            PlaySound(mHoverSound);
        }

        private void TableRow_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            PlaySound(mRightClickSound);
        }

        private void TableRow_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PlaySound(mClickSound);
        }

        internal Label GetColumn(int index)
        {
            return mColumns[index];
        }

        /// <summary>
        ///     Invoked when the row has been selected.
        /// </summary>
        public event GwenEventHandler<ItemSelectedEventArgs> Selected;

        /// <summary>
        ///     Sets the number of columns.
        /// </summary>
        /// <param name="columnCount">Number of columns.</param>
        protected void SetColumnCount(int columnCount)
        {
            if (columnCount == mColumnCount)
            {
                return;
            }

            if (columnCount >= MAX_COLUMNS)
            {
                throw new ArgumentException("Invalid column count", "columnCount");
            }

            for (var i = 0; i < MAX_COLUMNS; i++)
            {
                if (i < columnCount)
                {
                    if (null == mColumns[i])
                    {
                        mColumns[i] = new Label(this);
                        mColumns[i].Padding = Padding.Three;
                        mColumns[i].Margin = new Margin(0, 0, 2, 0); // to separate them slightly
                        if (i == columnCount - 1)
                        {
                            // last column fills remaining space
                            mColumns[i].Dock = Pos.Fill;
                            mColumns[i].AutoSizeToContents = false;
                        }
                        else
                        {
                            mColumns[i].Dock = Pos.Left;
                        }
                    }
                }
                else if (null != mColumns[i])
                {
                    RemoveChild(mColumns[i], true);
                    mColumns[i] = null;
                }

                mColumnCount = columnCount;
            }
        }

        /// <summary>
        ///     Sets the column width (in pixels).
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <param name="width">Column width.</param>
        public void SetColumnWidth(int column, int width)
        {
            if (null == mColumns[column])
            {
                return;
            }

            if (mColumns[column].Width == width)
            {
                return;
            }

            mColumns[column].Width = width;
        }

        /// <summary>
        ///     Sets the text of a specified cell.
        /// </summary>
        /// <param name="column">Column number.</param>
        /// <param name="text">Text to set.</param>
        public void SetCellText(int column, string text)
        {
            if (null == mColumns[column])
            {
                return;
            }

            mColumns[column].Text = text;
        }

        /// <summary>
        ///     Sets the contents of a specified cell.
        /// </summary>
        /// <param name="column">Column number.</param>
        /// <param name="control">Cell contents.</param>
        /// <param name="enableMouseInput">Determines whether mouse input should be enabled for the cell.</param>
        public void SetCellContents(int column, Base control, bool enableMouseInput = false)
        {
            if (null == mColumns[column])
            {
                return;
            }

            control.Parent = mColumns[column];
            mColumns[column].MouseInputEnabled = enableMouseInput;
        }

        /// <summary>
        ///     Gets the contents of a specified cell.
        /// </summary>
        /// <param name="column">Column number.</param>
        /// <returns>Control embedded in the cell.</returns>
        public Base GetCellContents(int column)
        {
            return mColumns[column];
        }

        protected virtual void OnRowSelected()
        {
            if (Selected != null)
            {
                Selected.Invoke(this, new ItemSelectedEventArgs(this));
            }
        }

        /// <summary>
        ///     Sizes all cells to fit contents.
        /// </summary>
        public void SizeToContents()
        {
            var width = 0;
            var height = 0;

            for (var i = 0; i < mColumnCount; i++)
            {
                if (null == mColumns[i])
                {
                    continue;
                }

                // Note, more than 1 child here, because the 
                // label has a child built in ( The Text )
                if (mColumns[i].Children.Count > 1)
                {
                    mColumns[i].SizeToChildren();
                }
                else
                {
                    mColumns[i].SizeToContents();
                }

                //if (i == m_ColumnCount - 1) // last column
                //    m_Columns[i].Width = Parent.Width - width; // fill if not autosized

                width += mColumns[i].Width + mColumns[i].Margin.Left + mColumns[i].Margin.Right;
                height = Math.Max(height, mColumns[i].Height + mColumns[i].Margin.Top + mColumns[i].Margin.Bottom);
            }

            SetSize(width, height);
        }

        /// <summary>
        ///     Sets the text color for all cells.
        /// </summary>
        /// <param name="color">Text color.</param>
        public void SetTextColor(Color color)
        {
            for (var i = 0; i < mColumnCount; i++)
            {
                if (null == mColumns[i])
                {
                    continue;
                }

                mColumns[i].TextColorOverride = color;
            }
        }

        public void SetTextFont(GameFont font)
        {
            for (var i = 0; i < mColumnCount; i++)
            {
                if (null == mColumns[i])
                {
                    continue;
                }

                mColumns[i].Font = font;
            }
        }

        /// <summary>
        ///     Returns text of a specified row cell (default first).
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <returns>Column cell text.</returns>
        public string GetText(int column = 0)
        {
            return mColumns[column].Text;
        }

        /// <summary>
        ///     Handler for Copy event.
        /// </summary>
        /// <param name="from">Source control.</param>
        protected override void OnCopy(Base from, EventArgs args)
        {
            Platform.Neutral.SetClipboardText(Text);
        }

    }

}
