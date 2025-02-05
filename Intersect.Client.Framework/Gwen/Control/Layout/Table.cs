using System.Globalization;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.Data;
using Intersect.Configuration;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control.Layout;


/// <summary>
///     Base class for multi-column tables.
/// </summary>
public partial class Table : Base, ISmartAutoSizeToContents, IColorableText
{

    private readonly List<int?> _columnWidths;
    private readonly List<Pos> _columnTextAlignments;

    private int _columnCount;
    private int _defaultRowHeight;
    private Point _cellSpacing;
    private bool _sizeToContents;

    private ITableDataProvider? _dataProvider;

    private GameFont? _font;
    private Color? _textColor;
    private Color? _textColorOverride;
    private bool _fitRowHeightToContents = true;
    private bool _autoSizeToContentWidth;
    private bool _autoSizeToContentHeight;
    private bool _autoSizeToContentWidthOnChildResize;
    private bool _autoSizeToContentHeightOnChildResize;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Table" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name"></param>
    public Table(Base parent, string? name = default) : base(parent, name)
    {
        _cellSpacing = new Point(2, 4);
        _columnCount = 1;
        _defaultRowHeight = 22;
        _sizeToContents = false;

        _columnTextAlignments = new List<Pos>(_columnCount);
        _columnWidths = new List<int?>(_columnCount);

        while (_columnWidths.Count < _columnCount)
        {
            _columnWidths.Add(null);
        }

        while (_columnTextAlignments.Count < _columnCount)
        {
            _columnTextAlignments.Add(Pos.Left | Pos.CenterV);
        }
    }

    public IReadOnlyList<Pos> ColumnAlignments
    {
        get => _columnTextAlignments.ToList();
        set
        {
            var columnCount = Math.Min(value.Count, _columnTextAlignments.Count);
            for (var column = 0; column < columnCount; ++column)
            {
                _columnTextAlignments[column] = value[column];
            }

            var tableRows = Children.OfType<TableRow>().ToArray();
            foreach (var row in tableRows)
            {
                row.ColumnTextAlignments = value;
            }
        }
    }

    /// <summary>
    ///     Column count (default 1).
    /// </summary>
    public int ColumnCount
    {
        get => _columnCount;
        set => SetAndDoIfChanged(ref _columnCount, value, SetColumnCount);
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
            var rows = Children.OfType<TableRow>().ToArray();
            foreach (var row in rows)
            {
                row.Margin = new Margin(0, 0, _cellSpacing.X, _cellSpacing.Y);
            }
        }
    }

    public ITableDataProvider? DataProvider
    {
        get => _dataProvider;
        set => SetAndDoIfChanged(ref _dataProvider, value, (oldValue, newValue) =>
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

    public GameFont? Font
    {
        get => _font;
        set => SetAndDoIfChanged(ref _font, value, () =>
        {
            foreach (var row in Children.OfType<TableRow>())
            {
                row.Font = value;
            }
        });
    }

    /// <summary>
    ///     Row count.
    /// </summary>
    public int RowCount => Children.Count;

    public Color? TextColor
    {
        get => _textColor;
        set => SetAndDoIfChanged(ref _textColor, value, () =>
        {
            foreach (IColorableText colorableText in Children)
            {
                colorableText.TextColor = value;
            }
        });
    }

    public Color? TextColorOverride
    {
        get => _textColorOverride;
        set => SetAndDoIfChanged(ref _textColorOverride, value, () =>
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
        get => _defaultRowHeight;
        set => _defaultRowHeight = value;
    }

    public IReadOnlyList<int?> ColumnWidths
    {
        get => _columnWidths.ToArray();
        set
        {
            for (var columnIndex = 0; columnIndex < _columnWidths.Count; ++columnIndex)
            {
                _columnWidths[columnIndex] = columnIndex < value.Count ? value[columnIndex] : null;
            }
        }
    }

    /// <summary>
    ///     Returns specific row of the table.
    /// </summary>
    /// <param name="row">Row index.</param>
    /// <returns>Row at the specified index.</returns>
    public TableRow? this[int row] => Children.OfType<TableRow>().Skip(row).FirstOrDefault();

    protected virtual void OnDataChanged(object sender, TableDataChangedEventArgs args)
    {
        while (Children.Count - 1 < args.Row)
        {
            _ = AddRow();
        }

        var row = Children[args.Row] as TableRow;
        row.SetCellText(args.Column, args.NewValue?.ToString());
    }

    public override JObject GetJson(bool isRoot = default)
    {
        var obj = base.GetJson(isRoot);
        obj.Add("SizeToContents", _sizeToContents);
        obj.Add("DefaultRowHeight", _defaultRowHeight);
        obj.Add(nameof(Font), Font?.ToString());
        obj.Add(nameof(TextColor), TextColor.ToString());
        obj.Add(nameof(TextColorOverride), TextColorOverride.ToString());

        return base.FixJson(obj);
    }

    public override void LoadJson(JToken obj, bool isRoot = default)
    {
        base.LoadJson(obj);
        if (obj[nameof(FitContents)] != null)
        {
            _sizeToContents = (bool)obj[nameof(FitContents)];
        }

        if (obj[nameof(DefaultRowHeight)] != null)
        {
            _defaultRowHeight = (int)obj[nameof(DefaultRowHeight)];
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
        var columnCount = ColumnCount;

        while (_columnWidths.Count < columnCount)
        {
            _columnWidths.Add(null);
        }

        while (_columnTextAlignments.Count < columnCount)
        {
            _columnTextAlignments.Add(Pos.Left | Pos.CenterV);
        }

        var rows = Children.OfType<TableRow>().ToArray();
        foreach (var row in rows)
        {
            row.ColumnCount = Math.Min(row.ColumnCount, columnCount);
        }
    }

    /// <summary>
    ///     Sets the column width (in pixels).
    /// </summary>
    /// <param name="column">Column index.</param>
    /// <param name="width">Column width.</param>
    public void SetColumnWidth(int column, int width)
    {
        if (_columnWidths[column] == width)
        {
            return;
        }

        _columnWidths[column] = width;
        Invalidate();
    }

    /// <summary>
    ///     Adds a new empty row.
    /// </summary>
    /// <returns>Newly created row.</returns>
    public TableRow AddRow() => AddRow(ColumnCount);

    public TableRow AddRow(params Base[] cellContents)
    {
        var row = AddRow();
        var columnLimit = Math.Min(cellContents.Length, row.ColumnCount);
        for (var columnIndex = 0; columnIndex < columnLimit; ++columnIndex)
        {
            var control = cellContents[columnIndex];
            row.SetCellContents(columnIndex, control);
        }

        return row;
    }

    public TableRow AddRow(int columnCount)
    {
        var row = new TableRow(this)
        {
            CellSpacing = _cellSpacing,
            ColumnCount = columnCount,
            ColumnTextAlignments = _columnTextAlignments,
            Dock = Pos.Top,
            Font = Font,
            Height = _defaultRowHeight,
            Margin = new Margin(0, 0, 0, _cellSpacing.Y),
            TextColor = TextColor,
            TextColorOverride = TextColorOverride,
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

        row.ColumnCount = Math.Min(_columnCount, row.ColumnCount);
        row.Dock = Pos.Top;
        row.Font = row.Font ?? Font;
        row.Height = _defaultRowHeight;

        row.SetColumnWidths(_columnWidths);
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

        if (_sizeToContents)
        {
            DoSizeToContents();
            _sizeToContents = false;
        }
        else
        {
            var autoSizeToContentWidth = _autoSizeToContentWidth;
            var autoSizeToContentHeight = _autoSizeToContentHeight;
            if (autoSizeToContentWidth || autoSizeToContentHeight)
            {
                DoSizeToContents(autoSizeToContentWidth, autoSizeToContentHeight);
            }
            else
            {
                ComputeColumnWidths();
            }
        }

        // ReSharper disable once InvertIf
        if (ClientConfiguration.Instance.EnableZebraStripedRows)
        {
            var even = false;
            var rows = Children.OfType<TableRow>().ToArray();
            foreach (TableRow row in rows)
            {
                row.EvenRow = even;
                even = !even;
            }
        }
    }

    protected override void PostLayout(Skin.Base skin)
    {
        base.PostLayout(skin);
    }

    public bool FitRowHeightToContents
    {
        get => _fitRowHeightToContents;
        set
        {
            if (value == _fitRowHeightToContents)
            {
                return;
            }

            _fitRowHeightToContents = value;
            Invalidate();
        }
    }

    public bool SizeToContents
    {
        get => _sizeToContents;
        set
        {
            if (value == _sizeToContents)
            {
                return;
            }

            _sizeToContents = true;
            Invalidate();
        }
    }

    /// <summary>
    ///     Sizes to fit contents.
    /// </summary>
    public void FitContents(int maxWidth)
    {
        MaximumSize = MaximumSize with { X = maxWidth };
        _sizeToContents = true;
        Invalidate();
    }

    protected virtual Point ComputeColumnWidths(TableRow[]? rows = null)
    {
        rows ??= Children.OfType<TableRow>().ToArray();

        var columnCount = ColumnCount;
        var measuredContentWidths = rows.Select(row => row.CalculateColumnContentWidths())
            .Aggregate(
                new int[columnCount],
                (widths, rowWidths) => widths.Select(
                        (columnWidth, columnIndex) => Math.Max(
                            columnWidth,
                            rowWidths.Length > columnIndex ? rowWidths[columnIndex] : 0
                        )
                    )
                    .ToArray()
            );

        var availableWidth = InnerWidth - _cellSpacing.X * Math.Max(0, _columnCount - 1);

        var requestedWidths = _columnWidths.ToArray();
        requestedWidths = measuredContentWidths.Select(
                (measuredWidth, columnIndex) =>
                {
                    if (requestedWidths.Length <= columnIndex)
                    {
                        return measuredWidth;
                    }

                    var requestedWidth = requestedWidths[columnIndex];
                    if (!requestedWidth.HasValue)
                    {
                        return requestedWidth;
                    }

                    return requestedWidth.Value < 0 ? measuredWidth : requestedWidth.Value;
                }
            )
            .ToArray();

        int flexColumnWidthSum = requestedWidths.Select(
                (requestedWidth, columnIndex) => requestedWidth.HasValue ? 0 : measuredContentWidths[columnIndex]
            )
            .Sum();
        var columnWidthRatios = requestedWidths.Select(
                (requestedWidth, columnIndex) =>
                    requestedWidth.HasValue ? float.NaN : measuredContentWidths[columnIndex] / (float)flexColumnWidthSum
            )
            .ToArray();

        var fixedColumnCount = requestedWidths.Count(requestedWidth => requestedWidth.HasValue);
        var fixedColumnWidthSum = requestedWidths.Sum(requestedWidth => requestedWidth ?? 0);
        availableWidth -= fixedColumnWidthSum;

        if (flexColumnWidthSum < availableWidth)
        {
            var extraSpace = availableWidth - flexColumnWidthSum;
            var extraSpacePerColumn = (int)Math.Floor(extraSpace / (float)fixedColumnCount);

            for (var columnIndex = 0; columnIndex < requestedWidths.Length; ++columnIndex)
            {
                var requestedWidth = requestedWidths[columnIndex];
                if (requestedWidth is not < 1)
                {
                    continue;
                }

                requestedWidths[columnIndex] += extraSpacePerColumn;
                availableWidth -= extraSpacePerColumn;
            }
        }

        var flexColumnWidths = columnWidthRatios.Select(
                ratio => float.IsNaN(ratio) ? 0 : (int)MathF.Ceiling(Math.Max(10f / ratio, availableWidth * ratio))
            )
            .ToArray();

        var actualWidth = 0;
        var actualHeight = 0;
        var columnLimit = Math.Min(columnCount, requestedWidths.Length);
        foreach (var row in rows)
        {
            var rowWidth = 0;
            for (var columnIndex = 0; columnIndex < columnLimit; ++columnIndex)
            {
                var requestedWidth = requestedWidths[columnIndex];
                var flexColumnWidth = flexColumnWidths[columnIndex];
                var cellWidth = requestedWidth ?? flexColumnWidth;
                var cell = row.GetColumn(columnIndex);
                if (cell is not null)
                {
                    cell.Width = cellWidth;
                }
                rowWidth += cellWidth;
            }

            actualWidth = Math.Max(actualWidth, rowWidth);
            actualHeight += row.OuterHeight;
        }

        return new Point(actualWidth, actualHeight);
    }

    public void DoSizeToContents(bool width = false, bool height = true)
    {
        var rows = Children.OfType<TableRow>().ToArray();
        if (_fitRowHeightToContents)
        {
            foreach (var row in rows)
            {
                row.SizeToChildren(width: width, height: height);
            }
        }

        var size = ComputeColumnWidths(rows);

        SetSize(size.X, size.Y);

        InvalidateParent();
    }

    public override void Invalidate()
    {
        base.Invalidate();
        // TODO: Remove this commented code if this doesn't cause issues
        // InvalidateChildren(true);
    }

    public void Invalidate(bool invalidateChildren, bool invalidateRecursive = true)
    {
        Invalidate();
        if (invalidateChildren)
        {
            InvalidateChildren(invalidateRecursive);
        }
    }

    protected override void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        if (Bounds.Height == 800)
        {
            Bounds.ToString();
        }

        base.OnBoundsChanged(oldBounds, newBounds);
    }

    protected override void OnChildBoundsChanged(Base child, Rectangle oldChildBounds, Rectangle newChildBounds)
    {
        base.OnChildBoundsChanged(child, oldChildBounds, newChildBounds);

        if (_autoSizeToContentWidthOnChildResize && oldChildBounds.Width != newChildBounds.Width)
        {
            Invalidate();
        }

        if (_autoSizeToContentHeightOnChildResize && oldChildBounds.Height != newChildBounds.Height)
        {
            Invalidate();
        }
    }

    public bool AutoSizeToContentWidth
    {
        get => _autoSizeToContentWidth;
        set => _autoSizeToContentWidth = value;
    }

    public bool AutoSizeToContentHeight
    {
        get => _autoSizeToContentHeight;
        set => _autoSizeToContentHeight = value;
    }

    public bool AutoSizeToContentWidthOnChildResize
    {
        get => _autoSizeToContentWidthOnChildResize;
        set => _autoSizeToContentWidthOnChildResize = value;
    }

    public bool AutoSizeToContentHeightOnChildResize
    {
        get => _autoSizeToContentHeightOnChildResize;
        set => _autoSizeToContentHeightOnChildResize = value;
    }
}
