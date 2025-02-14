using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.Data;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Intersect.Client.Framework.Input;
using Intersect.Client.Interface.Data;
using Intersect.Core;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Framework.Gwen.Control.Layout;

/// <summary>
///     Single table row.
/// </summary>
public partial class TableRow : Base, IColorableText
{
    private readonly List<Action> mDisposalActions = [];
    private readonly List<TableCell> _columns = [];
    private readonly List<Pos> _columnTextAlignments = [];
    private readonly List<int> _computedColumnWidths = [];

    protected string mClickSound;

    private int _columnCount;
    private bool _fitHeightToContents;

    private bool mEvenRow;

    private int _maximumColumns;

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
    /// <param name="columnWidths"></param>
    /// <param name="name"></param>
    public TableRow(Table parent, int[]? columnWidths = null, string? name = null) : this(
        parent: parent,
        columnCount: parent.ColumnCount,
        columnWidths: columnWidths,
        name: name
    )
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TableRow" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="columnCount">the number of columns this row has</param>
    /// <param name="columnWidths"></param>
    /// <param name="name"></param>
    public TableRow(Base parent, int columnCount, int[]? columnWidths = null, string? name = null) : base(parent: parent, name: name)
    {
        if (columnWidths is not null)
        {
            _computedColumnWidths.AddRange(columnWidths);
        }

        ColumnCount = columnCount;
        MaximumColumns = columnCount;
        KeyboardInputEnabled = true;
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
        get => _maximumColumns;
        set => SetAndDoIfChanged(ref _maximumColumns, value, ComputeColumns);
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

    public bool FitHeightToContents
    {
        get => _fitHeightToContents;
        set => SetAndDoIfChanged(ref _fitHeightToContents, value, Invalidate);
    }

    protected override void OnMouseClicked(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseClicked(mouseButton, mousePosition, userAction);

        PlaySound(
            mouseButton switch
            {
                MouseButton.Left => mClickSound,
                MouseButton.Right => mRightClickSound,
                _ => null,
            }
        );
    }

    protected override void OnMouseEntered()
    {
        base.OnMouseEntered();

        PlaySound(mHoverSound);
    }

    internal TableCell? GetColumn(int columnIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(columnIndex, nameof(columnIndex));
        return columnIndex >= _columnCount ? default : _columns[columnIndex];
    }

    /// <summary>
    ///     Invoked when the row has been selected.
    /// </summary>
    public event GwenEventHandler<ItemSelectedEventArgs> Selected;

    public bool SizeToChildren(bool resizeX = true, bool resizeY = true, bool recursive = false)
    {
        var columns = _columns.ToArray();
        foreach (var column in columns)
        {
            column.SizeToChildren(resizeX: resizeX, resizeY: resizeY, recursive: recursive);
        }

        return base.SizeToChildren(resizeX: resizeX, resizeY: resizeY, recursive: recursive);
    }

    protected override void OnSizeChanged(Point oldSize, Point newSize)
    {
        base.OnSizeChanged(oldSize, newSize);

        ApplicationContext.CurrentContext.Logger.LogTrace(
            "Table row {CanonicalName} resized from {OldSize} to {NewSize}",
            ParentQualifiedName,
            oldSize,
            newSize
        );

        if (oldSize.X == newSize.X)
        {
            return;
        }

        for (var columnIndex = 0; columnIndex < _computedColumnWidths.Count; ++columnIndex)
        {
            var computedColumnWidth = _computedColumnWidths[columnIndex];
            if (computedColumnWidth < 1)
            {
                continue;
            }

            if (GetColumn(columnIndex) is not { } cell)
            {
                continue;
            }

            cell.Width = computedColumnWidth;
        }
    }

    protected override void OnChildBoundsChanged(Base child, Rectangle oldChildBounds, Rectangle newChildBounds)
    {
        base.OnChildBoundsChanged(child, oldChildBounds, newChildBounds);
    }

    protected override void OnChildSizeChanged(Base child, Point oldChildSize, Point newChildSize)
    {
        base.OnChildSizeChanged(child, oldChildSize, newChildSize);

        var childCanonicalName = child.ParentQualifiedName;
        if (childCanonicalName.StartsWith('.') || childCanonicalName.EndsWith('.'))
        {
            return;
        }

        if (!IsVisible)
        {
            return;
        }

        ApplicationContext.CurrentContext.Logger.LogTrace(
            "Table row child {CanonicalName} resized from {OldChildSize} to {NewChildSize}",
            childCanonicalName,
            oldChildSize,
            newChildSize
        );
    }

    protected virtual void ComputeColumns(int oldValue, int columnCount)
    {
        _columns.Capacity = Math.Max(_columns.Capacity, columnCount);
        _columnTextAlignments.Capacity = Math.Max(_columnTextAlignments.Capacity, columnCount);

        var computedColumnWidths = _computedColumnWidths.ToArray();
        if (computedColumnWidths.Length < columnCount)
        {
            var originalLength = computedColumnWidths.Length;
            Array.Resize(ref computedColumnWidths, columnCount);
            Array.Fill(computedColumnWidths, 0, originalLength, columnCount - originalLength);
        }

        var fixedColumnWidthSum = computedColumnWidths.Sum();
        var fixedWithColumnCount = computedColumnWidths.Count(width => width > 0);
        var remainingWidth = InnerWidth - fixedColumnWidthSum;
        var dynamicWidthColumnCount = columnCount - fixedWithColumnCount;
        var dynamicColumnWidth = remainingWidth / Math.Max(1, dynamicWidthColumnCount);

        while (_columns.Count < columnCount)
        {
            var columnIndex = _columns.Count;
            var computedColumnWidth = computedColumnWidths.Skip(columnIndex).FirstOrDefault();
            if (computedColumnWidth < 1)
            {
                computedColumnWidth = dynamicColumnWidth;
            }

            var column = new TableCell(this, name: $"Column{_columns.Count}")
            {
                AutoSizeToContents = false,
                Font = Font,
                MouseInputEnabled = false,
                Padding = default,
                RestrictToParent = true,
                TextColor = TextColor,
                TextColorOverride = TextColorOverride,
            };

            if (computedColumnWidth > 0)
            {
                column.Width = computedColumnWidth;
            }

            if (_columnTextAlignments.Count > columnIndex)
            {
                column.TextAlign = _columnTextAlignments[columnIndex];
            }

            _columns.Add(column);
        }

        var lastColumnIndex = columnCount - 1;
        for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
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

    public TableRow Listen<TBaseValue>(
        int column,
        IDataProvider<TBaseValue> dataProvider,
        string? defaultText = default
    )
    {
        SetCellText(column, defaultText);

        AddDataProvider(dataProvider);
        dataProvider.ValueChanged += OnDataProviderEventHandler;
        return this;

        void OnDataProviderEventHandler(IDataProvider _, ValueChangedEventArgs<TBaseValue> args)
        {
            var adaptedValueString = args.Value?.ToString();
            SetCellText(column, adaptedValueString ?? defaultText);
        }
    }

    public TableRow Listen<TBaseValue, TDependentValue>(
        int column,
        IDataProvider<TBaseValue> dataProvider,
        ValueAdapterDelegate<TBaseValue, TDependentValue> adapter,
        string? defaultText = default
    )
    {
        SetCellText(column, defaultText);

        AddDataProvider(dataProvider);
        dataProvider.ValueChanged += OnDataProviderEventHandler;
        return this;

        void OnDataProviderEventHandler(IDataProvider _, ValueChangedEventArgs<TBaseValue> args)
        {
            var adaptedValue = adapter(args.Value, args.OldValue);
            var adaptedValueString = adaptedValue?.ToString();
            SetCellText(column, adaptedValueString ?? defaultText);
        }
    }

    public TableRow Listen(ITableDataProvider tableDataProvider, int row)
    {
        if (tableDataProvider == default)
        {
            throw new ArgumentNullException(nameof(tableDataProvider));
        }

        tableDataProvider.DataChanged += DataChanged;
        mDisposalActions.Add(() => tableDataProvider.DataChanged -= DataChanged);
        return this;

        void DataChanged(object sender, TableDataChangedEventArgs args)
        {
            if (row == args.Row)
            {
                SetCellText(args.Column, args.NewValue?.ToString());
            }
        }
    }

    public TableRow Listen(ITableRowDataProvider tableRowDataProvider, int column)
    {
        if (tableRowDataProvider == default)
        {
            throw new ArgumentNullException(nameof(tableRowDataProvider));
        }

        tableRowDataProvider.DataChanged += DataChanged;
        mDisposalActions.Add(() => tableRowDataProvider.DataChanged -= DataChanged);
        return this;

        void DataChanged(object sender, RowDataChangedEventArgs args)
        {
            if (args.Column == column)
            {
                SetCellText(column, args.NewValue?.ToString());
            }
        }
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
    /// <param name="columnIndex">Column number.</param>
    /// <param name="text">Text to set.</param>
    public void SetCellText(int columnIndex, string? text)
    {
        if (_columns.Count < _columnCount)
        {
            ComputeColumns(0, _columnCount);
        }

        var column = _columns.Skip(columnIndex).FirstOrDefault();
        if (column is null)
        {
            return;
        }

        column.Text = text;
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
        if (_fitHeightToContents)
        {
            SizeToChildren(resizeX: false);
        }

        base.Layout(skin);
    }

    public void SetComputedColumnWidths(int[] computedColumnWidths)
    {
        _computedColumnWidths.Clear();
        _computedColumnWidths.AddRange(computedColumnWidths);
    }

    public static new bool IsInstance(Base? component) => component is TableRow;
}
