using System;
using System.Linq;

using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control.Layout
{

    /// <summary>
    ///     Base class for multi-column tables.
    /// </summary>
    public class Table : Base
    {

        private readonly int[] mColumnWidth;

        private int mColumnCount;

        private int mDefaultRowHeight;

        private int mMaxWidth; // for autosizing, if nonzero - fills last cell up to this size

        // only children of this control should be TableRow.

        private bool mSizeToContents;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Table" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Table(Base parent) : base(parent)
        {
            mColumnCount = 1;
            mDefaultRowHeight = 22;

            mColumnWidth = new int[TableRow.MAX_COLUMNS];

            for (var i = 0; i < TableRow.MAX_COLUMNS; i++)
            {
                mColumnWidth[i] = 20;
            }

            mSizeToContents = false;
        }

        /// <summary>
        ///     Column count (default 1).
        /// </summary>
        public int ColumnCount
        {
            get => mColumnCount;
            set
            {
                SetColumnCount(value);
                Invalidate();
            }
        }

        /// <summary>
        ///     Row count.
        /// </summary>
        public int RowCount => Children.Count;

        /// <summary>
        ///     Gets or sets default height for new table rows.
        /// </summary>
        public int DefaultRowHeight
        {
            get => mDefaultRowHeight;
            set => mDefaultRowHeight = value;
        }

        /// <summary>
        ///     Returns specific row of the table.
        /// </summary>
        /// <param name="index">Row index.</param>
        /// <returns>Row at the specified index.</returns>
        public TableRow this[int index] => Children[index] as TableRow;

        public override JObject GetJson()
        {
            var obj = base.GetJson();
            obj.Add("SizeToContents", mSizeToContents);
            obj.Add("DefaultRowHeight", mDefaultRowHeight);

            return base.FixJson(obj);
        }

        public override void LoadJson(JToken obj)
        {
            base.LoadJson(obj);
            if (obj["SizeToContents"] != null)
            {
                mSizeToContents = (bool) obj["SizeToContents"];
            }

            if (obj["DefaultRowHeight"] != null)
            {
                mDefaultRowHeight = (int) obj["DefaultRowHeight"];
            }
        }

        /// <summary>
        ///     Sets the number of columns.
        /// </summary>
        /// <param name="count">Number of columns.</param>
        public void SetColumnCount(int count)
        {
            if (mColumnCount == count)
            {
                return;
            }

            foreach (var row in Children.OfType<TableRow>())
            {
                row.ColumnCount = count;
            }

            mColumnCount = count;
        }

        /// <summary>
        ///     Sets the column width (in pixels).
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <param name="width">Column width.</param>
        public void SetColumnWidth(int column, int width)
        {
            if (mColumnWidth[column] == width)
            {
                return;
            }

            mColumnWidth[column] = width;
            Invalidate();
        }

        /// <summary>
        ///     Gets the column width (in pixels).
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <returns>Column width.</returns>
        public int GetColumnWidth(int column)
        {
            return mColumnWidth[column];
        }

        /// <summary>
        ///     Adds a new empty row.
        /// </summary>
        /// <returns>Newly created row.</returns>
        public TableRow AddRow()
        {
            var row = new TableRow(this);
            row.ColumnCount = mColumnCount;
            row.Height = mDefaultRowHeight;
            row.Dock = Pos.Top;

            return row;
        }

        /// <summary>
        ///     Adds a new row.
        /// </summary>
        /// <param name="row">Row to add.</param>
        public void AddRow(TableRow row)
        {
            row.Parent = this;
            row.ColumnCount = mColumnCount;
            row.Height = mDefaultRowHeight;
            row.Dock = Pos.Top;
        }

        /// <summary>
        ///     Adds a new row with specified text in first column.
        /// </summary>
        /// <param name="text">Text to add.</param>
        /// <returns>New row.</returns>
        public TableRow AddRow(string text)
        {
            var row = AddRow();
            row.SetCellText(0, text);

            return row;
        }

        /// <summary>
        ///     Removes a row by reference.
        /// </summary>
        /// <param name="row">Row to remove.</param>
        public void RemoveRow(TableRow row)
        {
            RemoveChild(row, true);
        }

        /// <summary>
        ///     Removes a row by index.
        /// </summary>
        /// <param name="idx">Row index.</param>
        public void RemoveRow(int idx)
        {
            var row = Children[idx];
            RemoveRow(row as TableRow);
        }

        /// <summary>
        ///     Removes all rows.
        /// </summary>
        public void RemoveAll()
        {
            while (RowCount > 0)
            {
                RemoveRow(0);
            }
            DoSizeToContents();
        }

        /// <summary>
        ///     Gets the index of a specified row.
        /// </summary>
        /// <param name="row">Row to search for.</param>
        /// <returns>Row index if found, -1 otherwise.</returns>
        public int GetRowIndex(TableRow row)
        {
            return Children.IndexOf(row);
        }

        /// <summary>
        ///     Lays out the control's interior according to alignment, padding, dock etc.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Layout(Skin.Base skin)
        {
            base.Layout(skin);

            var even = false;
            foreach (TableRow row in Children)
            {
                row.EvenRow = even;
                even = !even;
                for (var i = 0; i < mColumnCount; i++)
                {
                    row.SetColumnWidth(i, mColumnWidth[i]);
                }
            }
        }

        protected override void PostLayout(Skin.Base skin)
        {
            base.PostLayout(skin);
            if (mSizeToContents)
            {
                DoSizeToContents();
                mSizeToContents = false;
            }
        }

        /// <summary>
        ///     Sizes to fit contents.
        /// </summary>
        public void SizeToContents(int maxWidth)
        {
            mMaxWidth = maxWidth;
            mSizeToContents = true;
            Invalidate();
        }

        public void DoSizeToContents()
        {
            var height = 0;
            var width = 0;

            foreach (TableRow row in Children)
            {
                row.SizeToContents(); // now all columns fit but only in this particular row

                for (var i = 0; i < ColumnCount; i++)
                {
                    Base cell = row.GetColumn(i);
                    if (null != cell)
                    {
                        if (i < ColumnCount - 1 || mMaxWidth == 0)
                        {
                            mColumnWidth[i] = Math.Max(
                                mColumnWidth[i], cell.Width + cell.Margin.Left + cell.Margin.Right
                            );
                        }
                        else
                        {
                            mColumnWidth[i] = mMaxWidth - width; // last cell - fill
                        }
                    }
                }

                height += row.Height;
            }

            // sum all column widths 
            for (var i = 0; i < ColumnCount; i++)
            {
                width += mColumnWidth[i];
            }

            SetSize(width, height);

            //InvalidateParent();
        }

    }

}
