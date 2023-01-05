using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.Data;
using Intersect.Configuration;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control.Layout
{

    /// <summary>
    ///     Base class for multi-column tables.
    /// </summary>
    public partial class Table : Base, IColorableText
    {

        private readonly List<int> mColumnWidths;

        private readonly List<Pos> mColumnAlignments;

        private int mColumnCount;

        private int mDefaultRowHeight;

        private ITableDataProvider mDataProvider;

        private int mMaxWidth; // for autosizing, if nonzero - fills last cell up to this size

        // only children of this control should be TableRow.

        private bool mSizeToContents;

        private GameFont mFont;

        private Color mTextColor;

        private Color mTextColorOverride;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Table" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Table(Base parent, string name = default) : base(parent, name)
        {
            mColumnCount = 1;
            mColumnAlignments = new List<Pos>(ColumnCount);
            mColumnWidths = new List<int>(ColumnCount);
            mDefaultRowHeight = 22;
            mSizeToContents = false;
            mTextColor = Color.Black;
            mTextColorOverride = Color.Transparent;

            for (var i = 0; i < ColumnCount; i++)
            {
                mColumnWidths.Add(20);
            }
        }

        public IReadOnlyList<Pos> ColumnAlignments => mColumnAlignments.ToList();

        /// <summary>
        ///     Column count (default 1).
        /// </summary>
        public int ColumnCount
        {
            get => mColumnCount;
            set => SetAndDoIfChanged(ref mColumnCount, value, SetColumnCount);
        }

        public ITableDataProvider DataProvider
        {
            get => mDataProvider;
            set => SetAndDoIfChanged(ref mDataProvider, value, (oldValue, newValue) =>
            {
                if (oldValue != default)
                {
                    oldValue.DataChanged -= OnDataChanged;
                }

                if (newValue != default)
                {
                    newValue.DataChanged += OnDataChanged;
                }
            });
        }

        public GameFont Font
        {
            get => mFont;
            set => SetAndDoIfChanged(ref mFont, value, () =>
            {
                foreach (TableRow child in Children.Where(child => child is TableRow))
                {
                    if (child != default)
                    {
                        child.Font = value;
                    }
                }
            });
        }

        /// <summary>
        ///     Row count.
        /// </summary>
        public int RowCount => Children.Count;

        public Color TextColor
        {
            get => mTextColor;
            set => SetAndDoIfChanged(ref mTextColor, value, () =>
            {
                foreach (IColorableText colorableText in Children)
                {
                    colorableText.TextColor = value;
                }
            });
        }

        public Color TextColorOverride
        {
            get => mTextColorOverride;
            set => SetAndDoIfChanged(ref mTextColorOverride, value, () =>
            {
                foreach (IColorableText colorableText in Children)
                {
                    colorableText.TextColorOverride = value;
                }
            });
        }

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

        protected virtual void OnDataChanged(object sender, TableDataChangedEventArgs args)
        {
            while (Children.Count - 1 < args.Row)
            {
                _ = AddRow();
            }

            var row = Children[args.Row] as TableRow;
            row.SetCellText(args.Column, args.NewValue?.ToString());
        }

        public override JObject GetJson()
        {
            var obj = base.GetJson();
            obj.Add("SizeToContents", mSizeToContents);
            obj.Add("DefaultRowHeight", mDefaultRowHeight);
            obj.Add(nameof(ColumnAlignments), new JArray(ColumnAlignments.Select(alignment => alignment.ToString() as object).ToArray()));
            obj.Add(nameof(Font), Font?.ToString());
            obj.Add(nameof(TextColor), TextColor.ToString());
            obj.Add(nameof(TextColorOverride), TextColorOverride.ToString());

            return base.FixJson(obj);
        }

        public override void LoadJson(JToken obj)
        {
            base.LoadJson(obj);
            if (obj[nameof(SizeToContents)] != null)
            {
                mSizeToContents = (bool)obj[nameof(SizeToContents)];
            }

            if (obj[nameof(DefaultRowHeight)] != null)
            {
                mDefaultRowHeight = (int)obj[nameof(DefaultRowHeight)];
            }

            if (obj[nameof(ColumnAlignments)] != null)
            {
                var jColumnAlignments = (JArray)obj[nameof(ColumnAlignments)];
                for (var columnIndex = 0; columnIndex < Math.Min(ColumnCount, jColumnAlignments.Count); columnIndex++)
                {
                    while (mColumnAlignments.Count <= columnIndex)
                    {
                        mColumnAlignments.Add(default);
                    }

                    mColumnAlignments[columnIndex] = (Pos)Enum.Parse(typeof(Pos), (string)jColumnAlignments[columnIndex]);
                }
            }

            if (obj[nameof(Font)] != null)
            {
                var fontInfo = (string)obj[nameof(Font)];
                if (!string.IsNullOrWhiteSpace(fontInfo))
                {
                    var parts = fontInfo.Split(',');
                    var name = parts[0];
                    var size = int.Parse(parts[1], CultureInfo.InvariantCulture);
                    Font = GameContentManager.Current?.GetFont(name, size);
                }
            }

            if (obj[nameof(TextColor)] != null)
            {
                TextColor = Color.FromString((string)obj[nameof(TextColor)]);
            }

            if (obj[nameof(TextColorOverride)] != null)
            {
                TextColorOverride = Color.FromString((string)obj[nameof(TextColorOverride)]);
            }
        }

        /// <summary>
        ///     Sets the number of columns.
        /// </summary>
        /// <param name="count">Number of columns.</param>
        protected virtual void SetColumnCount()
        {
            while (mColumnWidths.Count < ColumnCount)
            {
                mColumnWidths.Add(20);
            }

            foreach (var row in Children.OfType<TableRow>())
            {
                row.ColumnCount = Math.Min(row.ColumnCount, ColumnCount);
            }
        }

        /// <summary>
        ///     Sets the column width (in pixels).
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <param name="width">Column width.</param>
        public void SetColumnWidth(int column, int width)
        {
            if (mColumnWidths[column] == width)
            {
                return;
            }

            mColumnWidths[column] = width;
            Invalidate();
        }

        protected virtual void SynchronizeColumnAlignments()
        {
            InvalidateChildren(false);
        }

        /// <summary>
        ///     Gets the column width (in pixels).
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <returns>Column width.</returns>
        public int GetColumnWidth(int column)
        {
            return mColumnWidths[column];
        }

        /// <summary>
        ///     Adds a new empty row.
        /// </summary>
        /// <returns>Newly created row.</returns>
        public TableRow AddRow() => AddRow(ColumnCount);

        public TableRow AddRow(int columnCount)
        {
            var row = new TableRow(this)
            {
                ColumnCount = columnCount,
                Dock = Pos.Top,
                Font = Font,
                Height = mDefaultRowHeight,
            };

            return row;
        }

        /// <summary>
        ///     Adds a new row.
        /// </summary>
        /// <param name="row">Row to add.</param>
        public void AddRow(TableRow row)
        {
            if (row == default)
            {
                throw new ArgumentNullException(nameof(row));
            }

            row.Parent = this;

            row.ColumnCount = Math.Min(mColumnCount, row.ColumnCount);
            row.Dock = Pos.Top;
            row.Font = row.Font ?? Font;
            row.Height = mDefaultRowHeight;

            row.SetColumnWidths(mColumnWidths);
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

        public TableRow AddRow(string text, int columnCount)
        {
            var row = AddRow(columnCount);
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

            if (mSizeToContents)
            {
                DoSizeToContents();
                mSizeToContents = false;
            }
            else
            {
                ComputeColumnWidths();
            }

            var even = false;
            foreach (TableRow row in Children)
            {
                row.EvenRow = even;
                even = ClientConfiguration.Instance.EnableZebraStripedRows && !even;

                row.SetColumnWidths(mColumnWidths);
            }
        }

        protected override void PostLayout(Skin.Base skin)
        {
            base.PostLayout(skin);
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

        protected virtual (int width, int height) ComputeColumnWidths()
        {
            var height = 0;
            var width = 0;

            foreach (TableRow row in Children)
            {
                row.SizeToContents(); // now all columns fit but only in this particular row

                for (var i = 0; i < ColumnCount; i++)
                {
                    Base cell = row.GetColumn(i);
                    if (null != cell && row.ColumnCount == ColumnCount)
                    {
                        if (i < ColumnCount - 1 || mMaxWidth == 0)
                        {
                            mColumnWidths[i] = Math.Max(
                                mColumnWidths[i], cell.Width + cell.Margin.Left + cell.Margin.Right
                            );
                        }
                        else
                        {
                            mColumnWidths[i] = mMaxWidth - width; // last cell - fill
                        }
                    }
                }

                height += row.Height;
            }

            // sum all column widths 
            width += mColumnWidths.Take(ColumnCount).Sum();

            return (width, height);
        }

        public void DoSizeToContents()
        {
            var (width, height) = ComputeColumnWidths();

            SetSize(width, height);

            //InvalidateParent();
        }

        public override void Invalidate()
        {
            base.Invalidate();
            InvalidateChildren(true);
        }

    }

}
