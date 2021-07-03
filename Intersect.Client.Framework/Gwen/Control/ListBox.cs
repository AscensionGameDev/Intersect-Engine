using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Control.Layout;

using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     ListBox control.
    /// </summary>
    public class ListBox : ScrollControl
    {

        private readonly List<ListBoxRow> mSelectedRows;

        private readonly Table mTable;

        private GameFont mFont;

        private string mFontInfo;

        private bool mIsToggle;

        protected string mItemClickSound;

        //Sound Effects
        protected string mItemHoverSound;

        protected string mItemRightClickSound;

        private bool mMultiSelect;

        private Pos mOldDock; // used while autosizing

        private bool mSizeToContents;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ListBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ListBox(Base parent, string name = "") : base(parent, name)
        {
            mSelectedRows = new List<ListBoxRow>();

            MouseInputEnabled = true;
            EnableScroll(false, true);
            AutoHideBars = true;
            Margin = Margin.One;

            mTable = new Table(this);
            mTable.Dock = Pos.Fill;
            mTable.ColumnCount = 1;
            mTable.BoundsChanged += TableResized;

            mMultiSelect = false;
            mIsToggle = false;
        }

        /// <summary>
        ///     Determines whether multiple rows can be selected at once.
        /// </summary>
        public bool AllowMultiSelect
        {
            get => mMultiSelect;
            set
            {
                mMultiSelect = value;
                if (value)
                {
                    IsToggle = true;
                }
            }
        }

        /// <summary>
        ///     Determines whether rows can be unselected by clicking on them again.
        /// </summary>
        public bool IsToggle
        {
            get => mIsToggle;
            set => mIsToggle = value;
        }

        /// <summary>
        ///     Number of rows in the list box.
        /// </summary>
        public int RowCount => mTable.RowCount;

        /// <summary>
        ///     Returns specific row of the ListBox.
        /// </summary>
        /// <param name="index">Row index.</param>
        /// <returns>Row at the specified index.</returns>
        public ListBoxRow this[int index] => mTable[index] as ListBoxRow;

        /// <summary>
        ///     List of selected rows.
        /// </summary>
        public IEnumerable<TableRow> SelectedRows
        {
            get
            {
                var tmp = new List<TableRow>();
                foreach (var row in mSelectedRows)
                {
                    tmp.Add((TableRow) row);
                }

                return tmp;
            }
        }

        /// <summary>
        ///     First selected row (and only if list is not multiselectable).
        /// </summary>
        public ListBoxRow SelectedRow
        {
            get
            {
                if (mSelectedRows.Count == 0)
                {
                    return null;
                }

                return mSelectedRows[0];
            }
            set
            {
                if (mTable.Children.Contains(value))
                {
                    if (AllowMultiSelect)
                    {
                        SelectRow(value, false);
                    }
                    else
                    {
                        SelectRow(value, true);
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the selected row number.
        /// </summary>
        public int SelectedRowIndex
        {
            get
            {
                var selected = SelectedRow;
                if (selected == null)
                {
                    return -1;
                }

                return mTable.GetRowIndex(selected);
            }
            set => SelectRow(value);
        }

        /// <summary>
        ///     Column count of table rows.
        /// </summary>
        public int ColumnCount
        {
            get => mTable.ColumnCount;
            set
            {
                mTable.ColumnCount = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Invoked when a row has been selected.
        /// </summary>
        public event GwenEventHandler<ItemSelectedEventArgs> RowSelected;

        /// <summary>
        ///     Invoked whan a row has beed unselected.
        /// </summary>
        public event GwenEventHandler<ItemSelectedEventArgs> RowUnselected;

        public override JObject GetJson()
        {
            var obj = base.GetJson();
            obj.Add("SizeToContents", mSizeToContents);
            obj.Add("MultiSelect", AllowMultiSelect);
            obj.Add("IsToggle", IsToggle);
            obj.Add("Font", mFontInfo);
            obj.Add("ItemHoverSound", mItemHoverSound);
            obj.Add("ItemClickSound", mItemClickSound);
            obj.Add("ItemRightClickSound", mItemRightClickSound);

            return base.FixJson(obj);
        }

        public override void LoadJson(JToken obj)
        {
            base.LoadJson(obj);
            if (obj["SizeToContents"] != null)
            {
                mSizeToContents = (bool) obj["SizeToContents"];
            }

            if (obj["MultiSelect"] != null)
            {
                AllowMultiSelect = (bool) obj["MultiSelect"];
            }

            if (obj["IsToggle"] != null)
            {
                IsToggle = (bool) obj["IsToggle"];
            }

            if (obj["ItemHoverSound"] != null)
            {
                mItemHoverSound = (string) obj["ItemHoverSound"];
            }

            if (obj["ItemClickSound"] != null)
            {
                mItemClickSound = (string) obj["ItemClickSound"];
            }

            if (obj["ItemRightClickSound"] != null)
            {
                mItemRightClickSound = (string) obj["ItemRightClickSound"];
            }

            if (obj["Font"] != null && obj["Font"].Type != JTokenType.Null)
            {
                var fontArr = ((string) obj["Font"]).Split(',');
                mFontInfo = (string) obj["Font"];
                mFont = GameContentManager.Current.GetFont(fontArr[0], int.Parse(fontArr[1]));
            }

            foreach (var itm in mTable.Children)
            {
                var row = (ListBoxRow) itm;
                row.HoverSound = mItemHoverSound;
                row.ClickSound = mItemClickSound;
                row.RightClickSound = mItemRightClickSound;
                if (mFont != null)
                {
                    row.SetTextFont(mFont);
                }
            }
        }

        /// <summary>
        ///     Selects the specified row by index.
        /// </summary>
        /// <param name="index">Row to select.</param>
        /// <param name="clearOthers">Determines whether to deselect previously selected rows.</param>
        public void SelectRow(int index, bool clearOthers = false)
        {
            if (index < 0 || index >= mTable.RowCount)
            {
                return;
            }

            SelectRow(mTable.Children[index], clearOthers);
        }

        /// <summary>
        ///     Selects the specified row(s) by text.
        /// </summary>
        /// <param name="rowText">Text to search for (exact match).</param>
        /// <param name="clearOthers">Determines whether to deselect previously selected rows.</param>
        public void SelectRows(string rowText, bool clearOthers = false)
        {
            var rows = mTable.Children.OfType<ListBoxRow>().Where(x => x.Text == rowText);
            foreach (var row in rows)
            {
                SelectRow(row, clearOthers);
            }
        }

        /// <summary>
        ///     Selects the specified row(s) by regex text search.
        /// </summary>
        /// <param name="pattern">Regex pattern to search for.</param>
        /// <param name="regexOptions">Regex options.</param>
        /// <param name="clearOthers">Determines whether to deselect previously selected rows.</param>
        public void SelectRowsByRegex(
            string pattern,
            RegexOptions regexOptions = RegexOptions.None,
            bool clearOthers = false
        )
        {
            var rows = mTable.Children.OfType<ListBoxRow>().Where(x => Regex.IsMatch(x.Text, pattern));
            foreach (var row in rows)
            {
                SelectRow(row, clearOthers);
            }
        }

        /// <summary>
        ///     Slelects the specified row.
        /// </summary>
        /// <param name="control">Row to select.</param>
        /// <param name="clearOthers">Determines whether to deselect previously selected rows.</param>
        public void SelectRow(Base control, bool clearOthers = false)
        {
            if (!AllowMultiSelect || clearOthers)
            {
                UnselectAll();
            }

            var row = control as ListBoxRow;
            if (row == null)
            {
                return;
            }

            // TODO: make sure this is one of our rows!
            row.IsSelected = true;
            mSelectedRows.Add(row);
            if (RowSelected != null)
            {
                RowSelected.Invoke(this, new ItemSelectedEventArgs(row));
            }
        }

        /// <summary>
        ///     Removes the all rows from the ListBox
        /// </summary>
        /// <param name="idx">Row index.</param>
        public void RemoveAllRows()
        {
            mTable.DeleteAllChildren();
        }

        /// <summary>
        ///     Removes the specified row by index.
        /// </summary>
        /// <param name="idx">Row index.</param>
        public void RemoveRow(int idx)
        {
            mTable.RemoveRow(idx); // this calls Dispose()
            mTable.DoSizeToContents();
        }

        /// <summary>
        ///     Adds a new row.
        /// </summary>
        /// <param name="label">Row text.</param>
        /// <returns>Newly created control.</returns>
        public ListBoxRow AddRow(string label)
        {
            return AddRow(label, String.Empty);
        }

        /// <summary>
        ///     Adds a new row.
        /// </summary>
        /// <param name="label">Row text.</param>
        /// <param name="name">Internal control name.</param>
        /// <returns>Newly created control.</returns>
        public ListBoxRow AddRow(string label, string name)
        {
            return AddRow(label, name, null);
        }

        /// <summary>
        ///     Adds a new row.
        /// </summary>
        /// <param name="label">Row text.</param>
        /// <param name="name">Internal control name.</param>
        /// <param name="userData">User data for newly created row</param>
        /// <returns>Newly created control.</returns>
        public ListBoxRow AddRow(string label, string name, Object userData)
        {
            var row = new ListBoxRow(this);
            mTable.AddRow(row);

            row.SetCellText(0, label);
            row.Name = name;
            row.UserData = userData;

            row.Selected += OnRowSelected;

            row.HoverSound = mItemHoverSound;
            row.ClickSound = mItemClickSound;
            row.RightClickSound = mItemRightClickSound;

            if (mFont != null)
            {
                row.SetTextFont(mFont);
            }

            mTable.SizeToContents(Width);
            mTable.DoSizeToContents();

            return row;
        }

        /// <summary>
        ///     Sets the column width (in pixels).
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <param name="width">Column width.</param>
        public void SetColumnWidth(int column, int width)
        {
            mTable.SetColumnWidth(column, width);
            Invalidate();
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.DrawListBox(this);
        }

        /// <summary>
        ///     Deselects all rows.
        /// </summary>
        public virtual void UnselectAll()
        {
            foreach (var row in mSelectedRows)
            {
                row.IsSelected = false;
                if (RowUnselected != null)
                {
                    RowUnselected.Invoke(this, new ItemSelectedEventArgs(row));
                }
            }

            mSelectedRows.Clear();
        }

        /// <summary>
        ///     Unselects the specified row.
        /// </summary>
        /// <param name="row">Row to unselect.</param>
        public void UnselectRow(ListBoxRow row)
        {
            row.IsSelected = false;
            mSelectedRows.Remove(row);

            if (RowUnselected != null)
            {
                RowUnselected.Invoke(this, new ItemSelectedEventArgs(row));
            }
        }

        /// <summary>
        ///     Handler for the row selection event.
        /// </summary>
        /// <param name="control">Event source.</param>
        protected virtual void OnRowSelected(Base control, ItemSelectedEventArgs args)
        {
            // [omeg] changed default behavior
            var clear = false; // !InputHandler.InputHandler.IsShiftDown;
            var row = args.SelectedItem as ListBoxRow;
            if (row == null)
            {
                return;
            }

            if (row.IsSelected)
            {
                if (IsToggle)
                {
                    UnselectRow(row);
                }
            }
            else
            {
                SelectRow(row, clear);
            }
        }

        /// <summary>
        ///     Removes all rows.
        /// </summary>
        public virtual void Clear()
        {
            UnselectAll();
            mTable.RemoveAll();
        }

        public void SizeToContents()
        {
            mSizeToContents = true;

            // docking interferes with autosizing so we disable it until sizing is done
            mOldDock = mTable.Dock;
            mTable.Dock = Pos.None;
            mTable.SizeToContents(0); // autosize without constraints
        }

        private void TableResized(Base control, EventArgs args)
        {
            if (mSizeToContents)
            {
                SetSize(mTable.Width, mTable.Height);
                mSizeToContents = false;
                mTable.Dock = mOldDock;
                Invalidate();
            }
        }

        /// <summary>
        ///     Selects the first menu item with the given text it finds.
        ///     If a menu item can not be found that matches input, nothing happens.
        /// </summary>
        /// <param name="label">The label to look for, this is what is shown to the user.</param>
        public void SelectByText(string text)
        {
            foreach (ListBoxRow item in mTable.Children)
            {
                if (item.Text == text)
                {
                    SelectedRow = item;

                    return;
                }
            }
        }

        /// <summary>
        ///     Selects the first menu item with the given internal name it finds.
        ///     If a menu item can not be found that matches input, nothing happens.
        /// </summary>
        /// <param name="name">The internal name to look for. To select by what is displayed to the user, use "SelectByText".</param>
        public void SelectByName(string name)
        {
            foreach (ListBoxRow item in mTable.Children)
            {
                if (item.Name == name)
                {
                    SelectedRow = item;

                    return;
                }
            }
        }

        /// <summary>
        ///     Selects the first menu item with the given user data it finds.
        ///     If a menu item can not be found that matches input, nothing happens.
        /// </summary>
        /// <param name="userdata">
        ///     The UserData to look for. The equivalency check uses "param.Equals(item.UserData)".
        ///     If null is passed in, it will look for null/unset UserData.
        /// </param>
        public void SelectByUserData(object userdata)
        {
            foreach (ListBoxRow item in mTable.Children)
            {
                if (userdata == null)
                {
                    if (item.UserData == null)
                    {
                        SelectedRow = item;

                        return;
                    }
                }
                else if (userdata.Equals(item.UserData))
                {
                    SelectedRow = item;

                    return;
                }
            }
        }

        public ScrollBar GetVerticalScrollBar()
        {
            return base.mVerticalScrollBar;
        }

    }

}
