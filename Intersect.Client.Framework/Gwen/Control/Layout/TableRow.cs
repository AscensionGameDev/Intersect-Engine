using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.Data;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.ControlInternal;

namespace Intersect.Client.Framework.Gwen.Control.Layout;


/// <summary>
///     Single table row.
/// </summary>
public partial class TableRow : Base, IColorableText
{
    private readonly List<Action> mDisposalActions;
    private readonly List<Label> _columns;
    private readonly List<Pos> _columnTextAlignments;

    protected string mClickSound;

    private int _columnCount;

    private bool mEvenRow;

    private int mMaximumColumns;

    private GameFont? _font;

    private Color? _textColor;

    private Color? _textColorOverride;

    //Sound Effects
    protected string mHoverSound;

    protected string mRightClickSound;
    private Point _cellSpacing;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TableRow" /> class.
    /// </summary>
    /// <param name="parent">parent table</param>
    /// <param name="name"></param>
    public TableRow(Table parent, string? name = null) : this(parent: parent, columnCount: parent.ColumnCount, name: name)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TableRow" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="columnCount">the number of columns this row has</param>
    /// <param name="name"></param>
    public TableRow(Base parent, int columnCount, string? name = null) : base(parent: parent, name: name)
    {
        mDisposalActions = [];
        _columns = new List<Label>(columnCount);
        _columnTextAlignments = new List<Pos>(columnCount);

        ColumnCount = columnCount;
        MaximumColumns = columnCount;
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

    public IReadOnlyList<Pos> ColumnTextAlignments
    {
        get => _columns.Select(column => column.TextAlign).ToArray();
        set
        {
            _columnTextAlignments.Clear();
            _columnTextAlignments.AddRange(value.Take(_columnCount));

            var limit = Math.Min(_columns.Count, value.Count);
            for (var column = 0; column < limit; ++column)
            {
                _columns[column].TextAlign = value[column];
            }
        }
    }

    /// <summary>
    ///     Column count.
    /// </summary>
    public int ColumnCount
    {
        get => _columnCount;
        set => SetAndDoIfChanged(ref _columnCount, value, ComputeColumns);
    }

    /// <summary>
    ///     Indicates whether the row is even or odd (used for alternate coloring).
    /// </summary>
    public bool EvenRow
    {
        get => mEvenRow;
        set => mEvenRow = value;
    }

    public GameFont? Font
    {
        get => _font;
        set
        {
            if (_font == value)
            {
                return;
            }

            _font = value;

            foreach (var column in _columns)
            {
                column.Font = _font;
            }
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

    public Color? TextColor
    {
        get => _textColor;
        set => SetAndDoIfChanged(ref _textColor, value, () =>
        {
            foreach (var column in _columns)
            {
                column.TextColor = _textColor;
            }
        });
    }

    public Color? TextColorOverride
    {
        get => _textColorOverride;
        set => SetAndDoIfChanged(ref _textColorOverride, value, () =>
        {
            foreach (var column in _columns)
            {
                column.TextColorOverride = TextColorOverride;
            }
        });
    }

    public IEnumerable<string> TextColumns
    {
        get => _columns.Select(column => column.Text);
        set
        {
            if (value == default)
            {
                foreach (var column in _columns)
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

    public Point CellSpacing
    {
        get => _cellSpacing;
        set
        {
            if (value == _cellSpacing)
            {
                return;
            }

            _cellSpacing = value;
            var columns = _columns.ToArray();
            var firstColumn = columns.FirstOrDefault();
            var otherColumns = columns.Skip(1).ToArray();

            if (firstColumn is not null)
            {
                firstColumn.Margin = default;
            }

            foreach (var column in otherColumns)
            {
                column.Margin = new Margin(_cellSpacing.X, 0, 0, 0);
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

    internal Label? GetColumn(int columnIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(columnIndex, nameof(columnIndex));
        return columnIndex >= _columnCount ? default : _columns[columnIndex];
    }

    /// <summary>
    ///     Invoked when the row has been selected.
    /// </summary>
    public event GwenEventHandler<ItemSelectedEventArgs> Selected;

    public override bool SizeToChildren(bool width = true, bool height = true)
    {
        var columns = _columns.ToArray();
        foreach (var column in columns)
        {
            column.SizeToChildren(width: width, height: height);
        }

        return base.SizeToChildren(width: width, height: height);
    }

    protected virtual void ComputeColumns()
    {
        while (_columns.Count < ColumnCount)
        {
            var column = new Label(this, name: $"Column{_columns.Count}")
            {
                AutoSizeToContents = false,
                Font = Font,
                MouseInputEnabled = false,
                Padding = default,
                TextColor = TextColor,
                TextColorOverride = TextColorOverride,
            };

            if (_columnTextAlignments.Count > _columns.Count)
            {
                column.TextAlign = _columnTextAlignments[_columns.Count];
            }

            _columns.Add(column);
        }

        var lastColumnIndex = ColumnCount - 1;
        for (var columnIndex = 0; columnIndex < ColumnCount; columnIndex++)
        {
            var column = _columns[columnIndex];
            var isLastColumn = columnIndex == lastColumnIndex;

            column.AutoSizeToContents = false;
            if (isLastColumn)
            {
                column.Dock = Pos.Fill;
                column.Margin = Margin.Zero;
            }
            else
            {
                column.Dock = Pos.Left;
                column.Margin = new Margin(0, 0, _cellSpacing.X, 0);
            }
        }

        while (_columns.Count > MaximumColumns)
        {
            var column = _columns[^1];
            column.Parent = default;
            _columns.RemoveAt(_columns.Count - 1);
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
    public void SetCellText(int column, string text)
    {
        if (null == _columns[column])
        {
            return;
        }

        _columns[column].Text = text;
    }

    /// <summary>
    ///     Sets the contents of a specified cell.
    /// </summary>
    /// <param name="columnIndex">Column number.</param>
    /// <param name="control">Cell contents.</param>
    /// <param name="enableMouseInput">Determines whether mouse input should be enabled for the cell.</param>
    public void SetCellContents(int columnIndex, Base? control, bool enableMouseInput = false)
    {
        if (_columns[columnIndex] is not {} column)
        {
            return;
        }

        var textElement = column.Children.OfType<Text>().FirstOrDefault();
        if (textElement is not null)
        {
            textElement.IsVisible = control is null;
        }

        var controlsToRemove = column.Children.Where(child => child is not ControlInternal.Text && child != control)
            .ToArray();

        foreach (var controlToRemove in controlsToRemove)
        {
            controlToRemove.Parent = null;
        }

        if (control is null)
        {
            return;
        }

        control.Parent = column;
        column.MouseInputEnabled = enableMouseInput;
    }

    private Label GetCellLabel(int column)
    {
        return _columns[column];
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

        for (var i = 0; i < _columnCount; i++)
        {
            var columnLabel = _columns[i];
            if (null == columnLabel)
            {
                continue;
            }

            if (!columnLabel.AutoSizeToContents)
            {
                columnLabel.SizeToChildren();
            }

            //if (i == m_ColumnCount - 1) // last column
            //    m_Columns[i].Width = Parent.Width - width; // fill if not autosized

            width += columnLabel.Width + columnLabel.Margin.Left + columnLabel.Margin.Right;
            height = Math.Max(height, columnLabel.Height + columnLabel.Margin.Top + columnLabel.Margin.Bottom);
        }

        SetSize(width, height);
    }

    public int[] CalculateColumnContentWidths() =>
        _columns.ToArray().Select(column => column.MeasureContent().X).ToArray();

    /// <summary>
    ///     Sets the text color for all cells.
    /// </summary>
    /// <param name="color">Text color.</param>
    public void SetTextColor(Color color)
    {
        for (var i = 0; i < _columnCount; i++)
        {
            if (null == _columns[i])
            {
                continue;
            }

            _columns[i].TextColorOverride = color;
        }
    }

    /// <summary>
    ///     Returns text of a specified row cell (default first).
    /// </summary>
    /// <param name="column">Column index.</param>
    /// <returns>Column cell text.</returns>
    public string GetText(int column = 0)
    {
        return _columns[column].Text;
    }

    /// <summary>
    ///     Handler for Copy event.
    /// </summary>
    /// <param name="from">Source control.</param>
    protected override void OnCopy(Base from, EventArgs args)
    {
        Platform.Neutral.SetClipboardText(Text);
    }

    public virtual void SetColumnWidths(IEnumerable<int?> columnWidths)
    {
        if (default == columnWidths)
        {
            return;
        }

        var columnIndex = 0;
        foreach (var columnWidth in columnWidths)
        {
            var column = GetColumn(columnIndex++);
            if (default == column)
            {
                continue;
            }

            if (column.AutoSizeToContents)
            {
                continue;
            }

            if (columnWidth.HasValue)
            {
                column.Width = columnWidth.Value;
            }

        }

        //Invalidate();
    }

    protected override void Layout(Skin.Base skin)
    {
        base.Layout(skin);
    }
}
