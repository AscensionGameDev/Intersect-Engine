using System.Globalization;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.Data;
using Intersect.Configuration;
using Intersect.Core;
using Intersect.Framework.Collections;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control.Layout;


/// <summary>
///     Base class for multi-column tables.
/// </summary>
public partial class Table : Base, ISmartAutoSizeToContents, IColorableText
{
    private readonly List<int?> _columnWidths = [];
    private readonly List<int> _computedColumnWidths = [];
    private readonly List<Pos> _columnTextAlignments = [];

    private int _columnCount;
    private int _defaultRowHeight;
    private Point _cellSpacing;
    private bool _sizeToContents;

    private ITableDataProvider? _dataProvider;

    private IFont? _font;
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
        set => SetAndDoIfChanged(ref _columnCount, value, OnColumnCountChanged);
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
            if (rows.FirstOrDefault() is not { } firstRow)
            {
                return;
            }

            var rowCellSpacing = value with { Y = 0 };
            firstRow.CellSpacing = rowCellSpacing;
            foreach (var row in rows)
            {
                if (row != firstRow)
                {
                    row.Margin = new Margin(0, value.Y, 0, 0);
                }
                row.CellSpacing = rowCellSpacing;
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

    public IFont? Font
    {
        get => _font;
        set => SetAndDoIfChanged(ref _font, value, SetFont);
    }

    private static void SetFont(Base @this, IFont? value)
    {
        foreach (var row in @this.Children.OfType<TableRow>())
        {
            row.Font = value;
        }
    }

    public string? FontName
    {
        get => _font?.Name;
        set => Font = GameContentManager.Current.GetFont(value);
    }

    private int _fontSize = 10;

    public int FontSize
    {
        get => _fontSize;
        set => SetAndDoIfChanged(ref _fontSize, value, SetFontSize);
    }

    private static void SetFontSize(Base @this, int value)
    {
        foreach (var row in @this.Children.OfType<TableRow>())
        {
            row.FontSize = value;
        }
    }

    /// <summary>
    ///     Row count.
    /// </summary>
    public int RowCount => Children.Count(TableRow.IsInstance);

    public Color? TextColor
    {
        get => _textColor;
        set => SetAndDoIfChanged(ref _textColor, value, SetTextColor);
    }

    private static void SetTextColor(Base @this, Color? value)
    {
        foreach (var colorableText in @this.Children.OfType<IColorableText>())
        {
            colorableText.TextColor = value;
        }
    }

    public Color? TextColorOverride
    {
        get => _textColorOverride;
        set => SetAndDoIfChanged(ref _textColorOverride, value, SetTextColorOverride);
    }

    private static void SetTextColorOverride(Base @this, Color? value)
    {
        foreach (var colorableText in @this.Children.OfType<IColorableText>())
        {
            colorableText.TextColorOverride = value;
        }
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

    public override JObject? GetJson(bool isRoot = false, bool onlySerializeIfNotEmpty = false)
    {
        var serializedProperties = base.GetJson(isRoot, onlySerializeIfNotEmpty);
        if (serializedProperties is null)
        {
            return null;
        }

        serializedProperties.Add(nameof(SizeToContents), _sizeToContents);
        serializedProperties.Add(nameof(DefaultRowHeight), _defaultRowHeight);
        serializedProperties.Add(nameof(FontName), FontName);
        serializedProperties.Add(nameof(FontSize), FontSize);
        serializedProperties.Add(nameof(TextColor), TextColor?.ToString());
        serializedProperties.Add(nameof(TextColorOverride), TextColorOverride?.ToString());

        return base.FixJson(serializedProperties);
    }

    public override void LoadJson(JToken token, bool isRoot = default)
    {
        base.LoadJson(token, isRoot);

        if (token is not JObject obj)
        {
            return;
        }

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
                FontSize = size;
                Font = GameContentManager.Current?.GetFont(name);
            }
        }

        if (obj.TryGetValue(nameof(FontName), out var tokenFontName) &&
            tokenFontName is JValue { Type: JTokenType.String } valueFontName)
        {
            FontName = valueFontName.Value<string>();
        }

        if (obj.TryGetValue(nameof(FontSize), out var tokenFontSize) &&
            tokenFontSize is JValue { Type: JTokenType.Integer } valueFontSize)
        {
            FontSize = valueFontSize.Value<int>();
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

    /// <param name="oldValue"></param>
    /// <param name="columnCount"></param>
    protected virtual void OnColumnCountChanged(int oldValue, int columnCount)
    {
        _columnWidths.Capacity = Math.Max(_columnWidths.Capacity, columnCount);
        while (_columnWidths.Count < columnCount)
        {
            _columnWidths.Add(null);
        }

        _columnTextAlignments.Capacity = Math.Max(_columnTextAlignments.Capacity, columnCount);
        while (_columnTextAlignments.Count < columnCount)
        {
            _columnTextAlignments.Add(Pos.Left | Pos.CenterV);
        }

        _computedColumnWidths.Capacity = Math.Max(_computedColumnWidths.Capacity, columnCount);
        var currentComputedColumnWidthSum = _computedColumnWidths.Sum();
        var remainingColumnWidth = Math.Max(0, InnerWidth - currentComputedColumnWidthSum);
        var columnsToAdd = columnCount - _computedColumnWidths.Count;
        var dynamicColumnWidth = remainingColumnWidth / Math.Max(1, columnsToAdd);
        while (_computedColumnWidths.Count < columnCount)
        {
            _computedColumnWidths.Add(dynamicColumnWidth);
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

    /// <summary>
    ///     Adds a new empty row.
    /// </summary>
    /// <returns>Newly created row.</returns>
    public TableRow AddNamedRow(string? name) => AddRow(ColumnCount, name);

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

    public TableRow AddRow(int columnCount, string? name = null)
    {
        var row = new TableRow(parent: this, columnWidths: _computedColumnWidths.ToArray(), name: name)
        {
            CellSpacing = CellSpacing,
            ColumnCount = columnCount,
            ColumnTextAlignments = _columnTextAlignments,
            Dock = Pos.Top,
            FitHeightToContents = _fitRowHeightToContents,
            Font = Font,
            Height = _defaultRowHeight,
            Margin = new Margin(left: 0, top: _rowCount < 1 ? 0 : CellSpacing.Y, right: 0, bottom: 0),
            TextColor = TextColor,
            TextColorOverride = TextColorOverride,
            Width = InnerWidth,
        };

        return row;
    }

    public TableRow[] AddCells(params Base[] cellContents)
    {
        if (cellContents.Length < 1)
        {
            return [];
        }

        TableRow currentRow = AddRow();
        List<TableRow> rows = [currentRow];

        var column = 0;
        foreach (var cellContent in cellContents)
        {
            if (column >= currentRow.ColumnCount)
            {
                currentRow = AddRow();
                rows.Add(currentRow);
                column = 0;
            }

            currentRow.SetCellContents(column++, cellContent);
        }

        return rows.ToArray();
    }

    public int[] ComputedColumnWidths => _computedColumnWidths.ToArray();

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
        row.FitHeightToContents = _fitRowHeightToContents;
        row.ColumnCount = Math.Min(_columnCount, row.ColumnCount);
        row.Dock = Pos.Top;
        row.Font ??= Font;
        row.Height = _defaultRowHeight;
        row.Margin = new Margin(left: 0, top: _rowCount < 1 ? 0 : CellSpacing.Y, right: 0, bottom: 0);

        row.SetComputedColumnWidths(_computedColumnWidths.ToArray());
        row.SetColumnWidths(_columnWidths);
    }

    /// <summary>
    ///     Adds a new row with specified text in first column.
    /// </summary>
    /// <param name="text">Text to add.</param>
    /// <param name="name"></param>
    /// <returns>New row.</returns>
    public TableRow AddRow(string text, string? name = null)
    {
        var row = AddNamedRow(name: name);
        row.SetCellText(0, text);

        return row;
    }

    public TableRow InsertRowSorted<TKey>(
        string text,
        Func<Base?, TKey?> keySelector,
        string? name = null,
        object? userData = null
    ) where TKey : IComparable<TKey>
    {
        var row = AddRow(text, name: name);
        row.UserData = userData;
        ResortChild(row, keySelector);
        return row;
    }

    public TableRow AddRow(string text, int columnCount, string? name = null, int columnIndex = 0)
    {
        var row = AddRow(columnCount, name: name);
        row.SetCellText(columnIndex, text);

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
    public int GetRowIndex(TableRow row) => IndexOf(row);

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

    protected override void DoPostlayout(Skin.Base skin)
    {
        base.DoPostlayout(skin);
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
    public void FitContents(int? maxWidth = null)
    {
        MaximumSize = MaximumSize with { X = maxWidth ?? MaximumSize.X };
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

        var cellSpacingX = CellSpacing.X;
        var availableWidth = InnerWidth;

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
                {
                    if (requestedWidths.Length == 1)
                    {
                        return 1;
                    }

                    return requestedWidth.HasValue
                        ? float.NaN
                        : measuredContentWidths[columnIndex] / Math.Max((float)flexColumnWidthSum, 1);
                }
            )
            .ToArray();

        var flexColumnCount = columnWidthRatios.Count(float.IsFinite);
        var columnWidthRatioSum = columnWidthRatios.Sum(ratio => float.IsNaN(ratio) ? 0 : ratio);
        if (columnWidthRatioSum < 1f)
        {
            if (columnWidthRatioSum > 0f)
            {
                var normalization = 1f / columnWidthRatioSum;
                columnWidthRatios = columnWidthRatios.Select(ratio => ratio * normalization).ToArray();
            }
            else
            {
                var ratio = 1f / Math.Max(1, flexColumnCount);
                if (flexColumnCount == columnWidthRatios.Length)
                {
                    Array.Fill(columnWidthRatios, ratio);
                }
                else
                {
                    for (int columnIndex = 0; columnIndex < columnWidthRatios.Length; ++columnIndex)
                    {
                        if (float.IsNaN(columnWidthRatios[columnIndex]))
                        {
                            continue;
                        }

                        columnWidthRatios[columnIndex] = ratio;
                    }
                }
            }
        }

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

        var computedFlexColumnWidthSum = flexColumnWidths.Sum();
        if (computedFlexColumnWidthSum > availableWidth)
        {
            var negativePixelsToRedistribute = computedFlexColumnWidthSum - availableWidth;
            if (negativePixelsToRedistribute > columnCount)
            {
                var leftoverPixels = negativePixelsToRedistribute % columnCount;
                var pixelsPerColumn = (negativePixelsToRedistribute - leftoverPixels) / columnCount;
                flexColumnWidths = flexColumnWidths.Select(fcw => fcw - pixelsPerColumn).ToArray();
                negativePixelsToRedistribute = leftoverPixels;
            }

            var distinctColumnWidths = flexColumnWidths.Distinct().ToArray();
            if (distinctColumnWidths.Length == columnCount)
            {
                for (var index = 0; index < columnCount && negativePixelsToRedistribute > 0; ++index, --negativePixelsToRedistribute)
                {
                    --flexColumnWidths[index];
                }
            }
            else
            {
                var distinctColumnWidthsSortedByCountAscending = distinctColumnWidths.ToDictionary(
                        dw => dw,
                        dw => flexColumnWidths.Count(fcw => fcw == dw)
                    )
                    .OrderBy(p => p.Value)
                    .Select(p => p.Key)
                    .ToArray();

                foreach (var distinctColumnWidth in distinctColumnWidthsSortedByCountAscending)
                {
                    for (var index = 0;
                         index < columnCount && negativePixelsToRedistribute > 0;
                         ++index, --negativePixelsToRedistribute)
                    {
                        if (flexColumnWidths[index] == distinctColumnWidth)
                        {
                            --flexColumnWidths[index];
                        }
                    }
                }
            }
        }

        var actualWidth = 0;
        var actualHeight = 0;
        var columnLimit = Math.Min(columnCount, requestedWidths.Length);
        var computedColumnWidths = new int[Math.Max(_computedColumnWidths.Count, columnLimit)];
        var defaultColumnWidth = rows.Length > 0
            ? 0
            : (availableWidth - _cellSpacing.X * computedColumnWidths.Length) /
              Math.Max(1, computedColumnWidths.Length);
        Array.Fill(computedColumnWidths, defaultColumnWidth);

        foreach (var row in rows)
        {
            var rowCellWidths = new int[columnLimit];
            for (var columnIndex = 0; columnIndex < columnLimit; ++columnIndex)
            {
                var requestedWidth = requestedWidths[columnIndex];
                var flexColumnWidth = flexColumnWidths[columnIndex];
                var cellWidth = requestedWidth ?? flexColumnWidth;
                if (columnIndex > 0)
                {
                    cellWidth -= cellSpacingX;
                }

                rowCellWidths[columnIndex] = cellWidth;
            }

            var rowPadding = row.Padding;
            var rowPaddingH = rowPadding.Left + rowPadding.Right;
            if (rowPaddingH > 0)
            {
                var negativePixelsToRedistribute = rowPaddingH;
                if (negativePixelsToRedistribute % columnLimit == 0)
                {
                    var pixelsToRemovePerCell = negativePixelsToRedistribute / columnLimit;
                    rowCellWidths = rowCellWidths.Select(rcw => rcw - pixelsToRemovePerCell).ToArray();
                }
                else
                {
                    for (var columnIndex = 0; columnIndex < columnLimit && negativePixelsToRedistribute > 0; ++columnIndex, --negativePixelsToRedistribute)
                    {
                        --rowCellWidths[columnIndex];
                    }
                }
            }

            var rowWidth = 0;
            for (var columnIndex = 0; columnIndex < columnLimit; ++columnIndex)
            {
                var cellWidth = rowCellWidths[columnIndex];
                var cell = row.GetColumn(columnIndex);
                if (cell is not null)
                {
                    cell.Width = cellWidth;
                }

                computedColumnWidths[columnIndex] = Math.Max(cellWidth, computedColumnWidths[columnIndex]);
                rowWidth += cellWidth;
            }

            var rowDockSpacing = row.Dock.GetDockSpacing(DockChildSpacing);
            actualWidth = Math.Max(actualWidth, rowWidth) + rowDockSpacing.X;
            actualHeight += row.OuterHeight + rowDockSpacing.Y;
        }

        _computedColumnWidths.Clear();
        _computedColumnWidths.AddRange(computedColumnWidths);

        foreach (var row in rows)
        {
            row.SetComputedColumnWidths(computedColumnWidths);
        }

        ApplicationContext.CurrentContext.Logger.LogTrace(
            "Computed table '{TableName}' content size: ({Width}, {Height})",
            ParentQualifiedName,
            actualWidth,
            actualHeight
        );

        return new Point(actualWidth, actualHeight);
    }

    public void DoSizeToContents(bool resizeX = false, bool resizeY = true)
    {
        var rows = Children.OfType<TableRow>().ToArray();
        if (_fitRowHeightToContents)
        {
            foreach (var row in rows)
            {
                row.SizeToChildren(resizeX: resizeX, resizeY: resizeY, recursive: true);
            }
        }

        var size = ComputeColumnWidths(rows);

        var width = resizeX ? size.X : Width;
        var height = resizeY ? size.Y : Height;
        SetSize(width, height);

        InvalidateParent();
    }

    public override void Invalidate()
    {
        base.Invalidate();
        // TODO: Remove this commented code if this doesn't cause issues
        // InvalidateChildren(true);
    }

    public override Point GetChildrenSize()
    {
        var childrenSize = base.GetChildrenSize();
        ApplicationContext.CurrentContext.Logger.LogTrace(
            "Table {TableName} children size is {ChildrenSize}",
            ParentQualifiedName,
            childrenSize
        );
        return childrenSize;
    }

    public override bool SizeToChildren(SizeToChildrenArgs args)
    {
        ApplicationContext.CurrentContext.Logger.LogTrace(
            "Resizing Table {TableName} to children ({Args})...",
            ParentQualifiedName,
            args
        );
        return base.SizeToChildren(args);
    }

    public void Invalidate(bool invalidateChildren, bool invalidateRecursive = true)
    {
        Invalidate();
        if (invalidateChildren)
        {
            InvalidateChildren(invalidateRecursive);
        }
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

    protected override void OnSizeChanged(Point oldSize, Point newSize)
    {
        base.OnSizeChanged(oldSize, newSize);

        ApplicationContext.CurrentContext.Logger.LogTrace(
            "Table {TableName} size changed from {OldSize} to {NewSize}",
            ParentQualifiedName,
            oldSize,
            newSize
        );

        if (oldSize.X == newSize.X)
        {
            return;
        }

        if (_rowCount > 0)
        {
            return;
        }

        var columnCount = _columnCount;
        var widthPerColumn = InnerWidth / columnCount;
        var widths = new int[columnCount];
        Array.Fill(widths, widthPerColumn);
        _computedColumnWidths.Clear();
        _computedColumnWidths.AddRange(widths);
    }

    protected override void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        // ApplicationContext.CurrentContext.Logger.LogTrace(
        //     "Table {TableName} bounds changed from {OldBounds} to {NewBounds}",
        //     CanonicalName,
        //     oldBounds,
        //     newBounds
        // );
        base.OnBoundsChanged(oldBounds, newBounds);
    }

    private int _rowCount;

    protected override void OnChildAdded(Base child)
    {
        base.OnChildAdded(child);

        if (!TableRow.IsInstance(child))
        {
            return;
        }

        _rowCount = Math.Min(Children.Count, _rowCount + 1);
    }

    protected override void OnChildRemoved(Base child)
    {
        base.OnChildRemoved(child);

        if (!TableRow.IsInstance(child))
        {
            return;
        }

        _rowCount = Math.Max(0, _rowCount - 1);
    }
}
