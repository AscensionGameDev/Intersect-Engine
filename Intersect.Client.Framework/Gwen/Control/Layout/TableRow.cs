using System;
using System.Collections.Generic;
using System.Linq;

using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.Data;
using Intersect.Client.Framework.Gwen.Control.EventArguments;

namespace Intersect.Client.Framework.Gwen.Control.Layout
{

    /// <summary>
    ///     Single table row.
    /// </summary>
    public partial class TableRow : Base, IColorableText
    {
        private readonly List<Action> mDisposalActions;

        private readonly List<Label> mColumns;

        protected string mClickSound;

        private int mColumnCount;

        private bool mEvenRow;

        private GameFont mFont;

        private int mMaximumColumns;

        private Color mTextColor;

        private Color mTextColorOverride;

        //Sound Effects
        protected string mHoverSound;

        protected string mRightClickSound;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TableRow" /> class.
        /// </summary>
        /// <param name="parent">parent table</param>
        public TableRow(Table parent) : this(parent, parent.ColumnCount)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TableRow" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        /// <param name="columns">the number of columns this row has</param>
        public TableRow(Base parent, int columns) : base(parent)
        {
            mDisposalActions = new List<Action>();
            mColumns = new List<Label>(columns);
            mTextColor = Color.Black;
            mTextColorOverride = Color.Transparent;

            ColumnCount = columns;
            MaximumColumns = columns;
            KeyboardInputEnabled = true;
            Clicked += TableRow_Clicked;
            RightClicked += TableRow_RightClicked;
            HoverEnter += TableRow_HoverEnter;
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
            set => SetAndDoIfChanged(ref mColumnCount, value, ComputeColumns);
        }

        /// <summary>
        ///     Indicates whether the row is even or odd (used for alternate coloring).
        /// </summary>
        public bool EvenRow
        {
            get => mEvenRow;
            set => mEvenRow = value;
        }

        public GameFont Font
        {
            get => mFont;
            set
            {
                if (mFont == value)
                {
                    return;
                }

                mFont = value;
                SetTextFont(value);
            }
        }

        public int MaximumColumns
        {
            get => mMaximumColumns;
            set => SetAndDoIfChanged(ref mMaximumColumns, value, ComputeColumns);
        }

        /// <summary>
        ///     Text of the first column.
        /// </summary>
        public string Text
        {
            get => GetText(0);
            set => SetCellText(0, value);
        }

        public Color TextColor
        {
            get => mTextColor;
            set => SetAndDoIfChanged(ref mTextColor, value, () =>
            {
                foreach (var column in mColumns)
                {
                    column.TextColor = mTextColor;
                }
            });
        }

        public Color TextColorOverride
        {
            get => mTextColorOverride;
            set => SetAndDoIfChanged(ref mTextColorOverride, value, () =>
            {
                foreach (var column in mColumns)
                {
                    column.TextColorOverride = mTextColorOverride;
                }
            });
        }

        public IEnumerable<string> TextColumns
        {
            get => mColumns.Select(column => column.Text);
            set
            {
                if (value == default)
                {
                    foreach (var column in mColumns)
                    {
                        column.Text = string.Empty;
                    }
                }

                var columnIndex = 0;
                foreach (var cellText in value)
                {
                    if (columnIndex >= ColumnCount)
                    {
                        ColumnCount = columnIndex + 1;
                    }

                    SetCellText(columnIndex++, cellText);
                }
            }
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

        internal Label GetColumn(int columnIndex)
        {
            if (columnIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            }

            if (columnIndex >= mColumnCount)
            {
                return default;
            }

            return mColumns[columnIndex];
        }

        /// <summary>
        ///     Invoked when the row has been selected.
        /// </summary>
        public event GwenEventHandler<ItemSelectedEventArgs> Selected;

        protected virtual void ComputeColumns()
        {
            while (mColumns.Count < ColumnCount)
            {
                mColumns.Add(new Label(this)
                {
                    Font = Font,
                    MouseInputEnabled = true,
                    Padding = Padding.Three,
                    TextColor = TextColor,
                    TextColorOverride = TextColorOverride,
                });
            }

            var lastColumnIndex = ColumnCount - 1;
            for (var columnIndex = 0; columnIndex < ColumnCount; columnIndex++)
            {
                var column = mColumns[columnIndex];
                var isLastColumn = columnIndex == lastColumnIndex;

                column.AutoSizeToContents = isLastColumn;
                if (isLastColumn)
                {
                    column.Dock = Pos.Fill;
                    column.Margin = Margin.Zero;
                }
                else
                {
                    column.Dock = Pos.Left;
                    column.Margin = new Margin(0, 0, 2, 0);
                }
            }

            while (mColumns.Count > MaximumColumns)
            {
                var column = mColumns[mColumns.Count - 1];
                column.Parent = default;
                mColumns.RemoveAt(mColumns.Count - 1);
            }
        }

        public void Listen(ITableDataProvider tableDataProvider, int row)
        {
            if (tableDataProvider == default)
            {
                throw new ArgumentNullException(nameof(tableDataProvider));
            }

            void dataChanged(object sender, TableDataChangedEventArgs args)
            {
                if (row == args.Row)
                {
                    SetCellText(args.Column, args.NewValue?.ToString());
                }
            }

            tableDataProvider.DataChanged += dataChanged;
            mDisposalActions.Add(() => tableDataProvider.DataChanged -= dataChanged);
        }

        public void Listen(ITableRowDataProvider tableRowDataProvider, int column)
        {
            if (tableRowDataProvider == default)
            {
                throw new ArgumentNullException(nameof(tableRowDataProvider));
            }

            void dataChanged(object sender, RowDataChangedEventArgs args)
            {
                if (args.Column == column)
                {
                    SetCellText(column, args.NewValue?.ToString());
                }
            }

            tableRowDataProvider.DataChanged += dataChanged;
            mDisposalActions.Add(() => tableRowDataProvider.DataChanged -= dataChanged);
        }

        public void Listen(ITableCellDataProvider tableCellDataProvider, int column)
        {
            if (tableCellDataProvider == default)
            {
                throw new ArgumentNullException(nameof(tableCellDataProvider));
            }

            void dataChanged(object sender, CellDataChangedEventArgs args)
            {
                SetCellText(column, args.NewValue?.ToString());
            }

            tableCellDataProvider.DataChanged += dataChanged;
            mDisposalActions.Add(() => tableCellDataProvider.DataChanged -= dataChanged);
        }

        public override void Dispose()
        {
            base.Dispose();

            while (mDisposalActions.Count > 0)
            {
                var lastIndex = mDisposalActions.Count - 1;
                mDisposalActions[lastIndex]?.Invoke();
                mDisposalActions.RemoveAt(lastIndex);
            }
        }

        /// <summary>
        ///     Sets the column width (in pixels).
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <param name="width">Column width.</param>
        public void SetColumnWidth(int columnIndex, int width)
        {
            var column = GetColumn(columnIndex);

            if (default == column)
            {
                return;
            }

            if (column.Width == width)
            {
                return;
            }

            column.Width = width;

            Invalidate();
        }

        /// <summary>
        ///     Sets the text of a specified cell.
        /// </summary>
        /// <param name="column">Column number.</param>
        /// <param name="text">Text to set.</param>
        /// <param name="enableMouseInput">Determines whether mouse input should be enabled for the cell.</param>
        public void SetCellText(int column, string text, bool enableMouseInput = false)
        {
            if (null == mColumns[column])
            {
                return;
            }

            mColumns[column].Text = text;
            mColumns[column].MouseInputEnabled = enableMouseInput;
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

        private Label GetCellLabel(int column)
        {
            return mColumns[column];
        }

        /// <summary>
        ///     Gets the contents of a specified cell.
        /// </summary>
        /// <param name="column">Column number.</param>
        /// <returns>Control embedded in the cell.</returns>
        public Base GetCellContents(int column) => GetCellLabel(column);

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

        public virtual void SetColumnWidths(IEnumerable<int> columnWidths)
        {
            if (default == columnWidths)
            {
                return;
            }

            var columnIndex = 0;
            foreach (var columnWidth in columnWidths)
            {
                var column = GetColumn(columnIndex++);
                if (default != column)
                {
                    column.Width = columnWidth;
                }
            }

            //Invalidate();
        }

        protected override void Layout(Skin.Base skin)
        {
            base.Layout(skin);

            var columnAlignments = (Parent as Table)?.ColumnAlignments;
            for (var columnIndex = 0; columnIndex < mColumnCount; columnIndex++)
            {
                var column = mColumns[columnIndex];
                column.Dock = columnIndex == mColumnCount - 1 ? Pos.Fill : Pos.Left;
                column.Alignment = (columnAlignments == default || columnAlignments.Count <= columnIndex) ? column.Alignment : columnAlignments[columnIndex];
            }
        }
    }
}
