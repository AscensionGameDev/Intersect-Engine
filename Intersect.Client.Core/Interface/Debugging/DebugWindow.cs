using System.Diagnostics;
using Intersect.Client.Core;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Control.Layout;
using Intersect.Client.Framework.Gwen.Control.Utility;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Data;
using Intersect.Client.Interface.Debugging.Providers;
using Intersect.Client.Localization;
using Intersect.Client.Maps;
using Intersect.Client.MonoGame.NativeInterop.OpenGL;
using Intersect.Framework.Reflection;
using Intersect.IO.Files;
using static Intersect.Client.Framework.File_Management.GameContentManager;

namespace Intersect.Client.Interface.Debugging;

internal sealed partial class DebugWindow : Window
{
    private const string NoValue = "-";

    private readonly GameFont? _defaultFont;
    private bool _wasParentDrawDebugOutlinesEnabled;
    private bool _drawDebugOutlinesEnabled;
    private bool _viewClickedNodeInDebugger;
    private readonly NodeUnderCursorProvider _nodeUnderCursorProvider = new();

    public DebugWindow(Base parent) : base(parent, Strings.Debug.Title, false, nameof(DebugWindow))
    {
        _defaultFont = Current?.GetFont("sourcesansproblack", 10);

        IsHidden = true;
        IsResizable = false;
        InnerPanelPadding = new Padding(4);
        MinimumSize = new Point(360, 320);
        Size = new Point(480, 600);
        MaximumSize = new Point(800, 600);

        Tabs = CreateTabs();

        TabInfo = Tabs.AddPage(Strings.Debug.TabLabelInfo, nameof(TabInfo));
        CheckboxDrawDebugOutlines = CreateInfoCheckboxDrawDebugOutlines(TabInfo.Page);
        CheckboxEnableLayoutHotReloading = CreateInfoCheckboxEnableLayoutHotReloading(TabInfo.Page);
        CheckboxIncludeTextNodesInHover = CreateInfoCheckboxIncludeTextNodesInHover(TabInfo.Page);
        CheckboxViewClickedNodeInDebugger = CreateInfoCheckboxViewClickedNodeInDebugger(TabInfo.Page);
        ButtonShutdownServer = CreateInfoButtonShutdownServer(TabInfo.Page);
        ButtonShutdownServerAndExit = CreateInfoButtonShutdownServerAndExit(TabInfo.Page);
        TableDebugStats = CreateInfoTableDebugStats(TabInfo.Page);

        TabAssets = Tabs.AddPage(Strings.Debug.TabLabelAssets, nameof(TabAssets));
        AssetsToolsTable = CreateAssetsToolsTable(TabAssets.Page);
        AssetsList = CreateAssetsList(TabAssets.Page);
        AssetsButtonReloadAsset = CreateAssetsButtonReloadAsset(AssetsToolsTable, AssetsList);

        TabGPU = Tabs.AddPage(Strings.Debug.TabLabelGPU, nameof(TabGPU));
        GPUStatisticsTable = CreateGPUStatisticsTable(TabGPU.Page);
        GPUAllocationsTable = CreateGPUAllocationsTable(TabGPU.Page);

        AssetsToolsTable.SizeToChildren();
    }

    protected override void Dispose(bool disposing)
    {
        UnsubscribeGPU();
        RemoveIntercepts();

        base.Dispose(disposing);
    }

    private Table GPUStatisticsTable { get; }

    private Table GPUAllocationsTable { get; }

    private Table CreateGPUAllocationsTable(Base parent)
    {
        Panel gpuAllocationsPanel = new(parent: parent, name: nameof(gpuAllocationsPanel))
        {
            Dock = Pos.Fill,
        };

        ScrollControl gpuAllocationsScroller = new(parent: gpuAllocationsPanel, name: nameof(gpuAllocationsScroller))
        {
            Dock = Pos.Fill,
            InnerPanelPadding = new Padding(horizontal: 8, vertical: 0),
            ShouldCacheToTexture = true,
        };

        gpuAllocationsScroller.VerticalScrollBar.BaseNudgeAmount *= 2;

        var table = new Table(parent: gpuAllocationsScroller, name: nameof(GPUAllocationsTable))
        {
            AutoSizeToContentHeightOnChildResize = true,
            AutoSizeToContentWidthOnChildResize = true,
            CellSpacing = new Point(x: 8, y: 2),
            ColumnCount = 2,
            ColumnWidths = [null, 100],
            Dock = Pos.Fill,
            Font = _defaultFont,
            MinimumSize = new Point(x: 408, y: 0),
            SizeToContents = true,
        };
        table.SizeChanged += ResizeTableToChildrenOnSizeChanged;

        var existingTextures = Graphics.Renderer.Textures;
        foreach (var texture in existingTextures)
        {
            // _ = EnsureGPUAllocationsRowFor(gameTexture: texture, creating: true, table: table);
        }

        SubscribeGPU();

        return table;
    }

    private static void ResizeTableToChildrenOnSizeChanged(Base sender, ValueChangedEventArgs<Point> args)
    {
        sender.SizeToChildren(resizeX: args.Value.X > args.OldValue.X, resizeY: true, recursive: true);
    }

    private void SubscribeGPU()
    {
        Graphics.Renderer.TextureAllocated += RendererOnTextureAllocated;
        Graphics.Renderer.TextureCreated += RendererOnTextureCreated;
        Graphics.Renderer.TextureDisposed += RendererOnTextureDisposed;
        Graphics.Renderer.TextureFreed += RendererOnTextureFreed;
    }

    private void UnsubscribeGPU()
    {
        Graphics.Renderer.TextureAllocated -= RendererOnTextureAllocated;
        Graphics.Renderer.TextureCreated -= RendererOnTextureCreated;
        Graphics.Renderer.TextureDisposed -= RendererOnTextureDisposed;
        Graphics.Renderer.TextureFreed -= RendererOnTextureFreed;
    }

    private void RendererOnTextureDisposed(object? _, TextureEventArgs args)
    {
        var gameTexture = args.GameTexture;
        if (_gpuAllocationsRowByTextureLookup.TryGetValue(gameTexture, out var existingRow))
        {
            GPUAllocationsTable.RemoveRow(existingRow);
        }
    }

    private void RendererOnTextureFreed(object? _, TextureEventArgs args)
    {
        var x = this;
        var gameTexture = args.GameTexture;
        var row = EnsureGPUAllocationsRowFor(gameTexture, creating: false);
        if (row.GetCellContents(1) is not Label statusLabel)
        {
            return;
        }

        statusLabel.Text = "Freed";
        statusLabel.TextColorOverride = new Color(255, 63, 63);
    }

    private void RendererOnTextureAllocated(object? _, TextureEventArgs args)
    {
        var gameTexture = args.GameTexture;
        var row = EnsureGPUAllocationsRowFor(gameTexture, creating: false);
        if (row.GetCellContents(1) is not Label statusLabel)
        {
            return;
        }

        statusLabel.Text = "Allocated";
        statusLabel.TextColorOverride = new Color(63, 255, 63);
    }

    private void RendererOnTextureCreated(object? _, TextureEventArgs args)
    {
        var gameTexture = args.GameTexture;
        EnsureGPUAllocationsRowFor(gameTexture, creating: true);
    }

    private readonly Dictionary<IGameTexture, TableRow> _gpuAllocationsRowByTextureLookup = [];

    private TableRow EnsureGPUAllocationsRowFor(IGameTexture gameTexture, bool creating, Table? table = null)
    {
        if (_gpuAllocationsRowByTextureLookup.TryGetValue(gameTexture, out var existingRow))
        {
            if (creating)
            {
                throw new InvalidOperationException();
            }

            return existingRow;
        }

        table ??= GPUAllocationsTable;

        existingRow = table.InsertRowSorted(
            gameTexture.Name,
            userData: gameTexture,
            keySelector: SelectRowUserDataGameTextureName
        );

        var nameCell = existingRow.GetCellContents(0);
        nameCell.UserData = gameTexture;
        nameCell.MouseInputEnabled = true;
        nameCell.Clicked += CopyAssetNameToClipboardOnNameCellClicked;
        existingRow.SetCellText(1, "Created");
        if (existingRow.GetCellContents(1) is Label statusLabel)
        {
            statusLabel.TextColorOverride = new Color(191, 191, 191);
        }
        _gpuAllocationsRowByTextureLookup[gameTexture] = existingRow;

        table.SizeToChildren(recursive: true);

        return existingRow;
    }

    private static void CopyAssetNameToClipboardOnNameCellClicked(Base sender, MouseButtonState _)
    {
        if (sender.UserData is IAsset asset)
        {
            GameClipboard.Instance.SetText(asset.Name);
        }
    }

    private static string? SelectRowUserDataGameTextureName(Base? node)
    {
        return node?.UserData is IGameTexture gameTexture ? gameTexture.Name : null;
    }


    private Table CreateGPUStatisticsTable(Base parent)
    {
        ScrollControl gpuStatisticsScroller = new(parent, nameof(gpuStatisticsScroller))
        {
            Dock = Pos.Fill,
        };

        gpuStatisticsScroller.VerticalScrollBar.BaseNudgeAmount *= 2;

        var table = new Table(gpuStatisticsScroller, nameof(GPUStatisticsTable))
        {
            AutoSizeToContentHeightOnChildResize = true,
            AutoSizeToContentWidthOnChildResize = true,
            CellSpacing = new Point(8, 2),
            ColumnCount = 2,
            ColumnWidths = [180, null],
            Dock = Pos.Fill,
            Font = _defaultFont,
        };
        table.SizeChanged += ResizeTableToChildrenOnSizeChanged;

        _ = table.AddRow(Strings.Debug.SectionGPUStatistics, columnCount: 2, name: "SectionGPU", columnIndex: 1);
        table.AddRow(Strings.Debug.Fps, name: "FPSRow").Listen(1, new DelegateDataProvider<int>(() => Graphics.Renderer.Fps), NoValue);
        // table.AddRow(Strings.Debug.Draws, name: "DrawsRow").Listen(1, new DelegateDataProvider<int>(() => Graphics.DrawCalls), NoValue);

        table.AddRow(Strings.Debug.MapsDrawn, name: "MapsDrawnRow").Listen(1, new DelegateDataProvider<int>(() => Graphics.MapsDrawn), NoValue);
        table.AddRow(Strings.Debug.EntitiesDrawn, name: "EntitiesDrawnRow").Listen(1, new DelegateDataProvider<int>(() => Graphics.EntitiesDrawn), NoValue);
        table.AddRow(Strings.Debug.LightsDrawn, name: "LightsDrawnRow").Listen(1, new DelegateDataProvider<int>(() => Graphics.LightsDrawn), NoValue);
        table.AddRow(Strings.Debug.InterfaceObjects, name: "InterfaceObjectsRow").Listen(1, new DelegateDataProvider<int?>(() => Interface.CurrentInterface?.NodeCount, delayMilliseconds: 1000), NoValue);


        table.AddRow(Strings.Debug.RenderTargetAllocations, name: "GPURenderTargetAllocations").Listen(1, new DelegateDataProvider<ulong>(() => Graphics.Renderer.RenderTargetAllocations), NoValue);
        table.AddRow(Strings.Debug.TextureAllocations, name: "GPUTextureAllocations").Listen(1, new DelegateDataProvider<ulong>(() => Graphics.Renderer.TextureAllocations), NoValue);
        table.AddRow(Strings.Debug.TextureCount, name: "GPUTextureCount").Listen(1, new DelegateDataProvider<ulong>(() => Graphics.Renderer.TextureCount), NoValue);

        table.AddRow(Strings.Debug.FreeVRAM, name: "GPUFreeVRAM").Listen(1, new DelegateDataProvider<string>(() => FileSystemHelper.FormatSize(Graphics.Renderer.AvailableMemory)), NoValue);
        table.AddRow(Strings.Debug.TotalVRAM, name: "GPUTotalVRAM").Listen(1, new DelegateDataProvider<string>(() => FileSystemHelper.FormatSize(Graphics.Renderer.TotalMemory)), NoValue);

        table.AddRow(Strings.Debug.RenderBufferVRAMFree, name: "GPUVRAMRenderBuffers").Listen(1, new DelegateDataProvider<string>(() => FileSystemHelper.FormatSize(GL.AvailableRenderBufferMemory)), NoValue);
        table.AddRow(Strings.Debug.TextureVRAMFree, name: "GPUVRAMTextures").Listen(1, new DelegateDataProvider<string>(() => FileSystemHelper.FormatSize(GL.AvailableTextureMemory)), NoValue);
        table.AddRow(Strings.Debug.VBOVRAMFree, name: "GPUVRAMVBOs").Listen(1, new DelegateDataProvider<string>(() => FileSystemHelper.FormatSize(GL.AvailableVBOMemory)), NoValue);

        var rows = table.Children.OfType<TableRow>().ToArray();
        foreach (var row in rows)
        {
            if (row.Name.StartsWith("Section") && row.GetCellContents(1) is Label titleLabel)
            {
                titleLabel.Padding = titleLabel.Padding with { Top = 8 };
            }

            if (row.GetCellContents(0) is not Label label)
            {
                continue;
            }

            label.TextAlign = Pos.Right | Pos.CenterV;
        }

        return table;
    }

    private SearchableTree CreateAssetsList(Base parent)
    {
        var dataProvider = new AssetsSearchableTreeDataProvider(Current, this);
        SearchableTree assetList = new(parent, dataProvider, name: nameof(AssetsList))
        {
            Dock = Pos.Fill,
            FontSearch = _defaultFont,
            FontTree = _defaultFont,
            SearchPlaceholderText = Strings.Debug.AssetsSearchPlaceholder,
        };

        return assetList;
    }

    private Table CreateAssetsToolsTable(Base assetsTabPage)
    {
        Table table = new(assetsTabPage, nameof(AssetsToolsTable))
        {
            AutoSizeToContentHeightOnChildResize = true,
            AutoSizeToContentWidthOnChildResize = true,
            CellSpacing = new Point(4, 4),
            ColumnCount = 2,
            ColumnWidths = [null, null],
            Dock = Pos.Top,
            SizeToContents = true,
        };

        return table;
    }

    private Button CreateAssetsButtonReloadAsset(Table table, SearchableTree assetList)
    {
        var row = table.AddNamedRow($"{nameof(AssetsButtonReloadAsset)}Row");

        Button buttonReloadAsset = new(row, name: nameof(AssetsButtonReloadAsset))
        {
            IsDisabled = assetList.SelectedNodes.Length < 1,
            Font = _defaultFont,
            Text = Strings.Debug.ReloadAsset,
        };
        row.SetCellContents(0, buttonReloadAsset);

        assetList.SelectionChanged += AssetListOnSelectionChanged;

        buttonReloadAsset.Clicked += ButtonReloadAssetOnClicked;
        row.SetCellContents(0, buttonReloadAsset, enableMouseInput: true);

        return buttonReloadAsset;
    }

    private void ButtonReloadAssetOnClicked(Base @base, MouseButtonState mouseButtonState)
    {
        foreach (var node in AssetsList.SelectedNodes)
        {
            if (node.UserData is not SearchableTreeDataEntry entry)
            {
                continue;
            }

            if (entry.UserData is not IAsset asset)
            {
                continue;
            }

            // TODO: Audio reloading?
            if (asset is not IGameTexture texture)
            {
                continue;
            }

            texture.Reload();
        }
    }

    private void AssetListOnSelectionChanged(Base @base, EventArgs eventArgs)
    {
        AssetsButtonReloadAsset.IsDisabled = AssetsList.SelectedNodes.All(node => node.UserData is not SearchableTreeDataEntry { UserData: IGameTexture });
    }

    private TabControl Tabs { get; }

    private TabButton TabInfo { get; }

    private TabButton TabAssets { get; }

    private TabButton TabGPU { get; }

    private SearchableTree AssetsList { get; }

    private Table AssetsToolsTable { get; }

    private Button AssetsButtonReloadAsset { get; }

    private LabeledCheckBox CheckboxDrawDebugOutlines { get; }

    private LabeledCheckBox CheckboxEnableLayoutHotReloading { get; }

    private LabeledCheckBox CheckboxIncludeTextNodesInHover { get; }

    private LabeledCheckBox CheckboxViewClickedNodeInDebugger { get; }

    private Button ButtonShutdownServer { get; }

    private Button ButtonShutdownServerAndExit { get; }

    private Table TableDebugStats { get; }

    protected override void EnsureInitialized()
    {
        TableDebugStats.SizeToChildren();

        // LoadJsonUi(UI.Debug, Graphics.Renderer?.GetResolutionString());
    }

    protected override void OnAttached(Base parent)
    {
        base.OnAttached(parent);

        Root.DrawDebugOutlines = _drawDebugOutlinesEnabled;
    }

    protected override void OnAttaching(Base newParent)
    {
        base.OnAttaching(newParent);
        _wasParentDrawDebugOutlinesEnabled = newParent.DrawDebugOutlines;
    }

    protected override void OnDetaching(Base oldParent)
    {
        base.OnDetaching(oldParent);
        oldParent.DrawDebugOutlines = _wasParentDrawDebugOutlinesEnabled;
    }

    private TabControl CreateTabs()
    {
        return new TabControl(this, name: "DebugTabs")
        {
            Dock = Pos.Fill,
        };
    }

    private LabeledCheckBox CreateInfoCheckboxDrawDebugOutlines(Base parent)
    {
        _drawDebugOutlinesEnabled = Root?.DrawDebugOutlines ?? false;

        var checkbox = new LabeledCheckBox(parent, nameof(CheckboxDrawDebugOutlines))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            IsChecked = _drawDebugOutlinesEnabled,
            Text = Strings.Debug.DrawDebugOutlines,
        };

        checkbox.CheckChanged += CheckboxDrawDebugOutlinesOnCheckChanged;

        return checkbox;
    }

    private void CheckboxDrawDebugOutlinesOnCheckChanged(ICheckbox sender, EventArgs eventArgs)
    {
        _drawDebugOutlinesEnabled = sender.IsChecked;
        if (Root is { } root)
        {
            root.DrawDebugOutlines = _drawDebugOutlinesEnabled;
        }
    }

    private LabeledCheckBox CreateInfoCheckboxEnableLayoutHotReloading(Base parent)
    {
        var checkbox = new LabeledCheckBox(parent, nameof(CheckboxEnableLayoutHotReloading))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            IsChecked = Globals.ContentManager.ContentWatcher.Enabled,
            Text = Strings.Debug.EnableLayoutHotReloading,
            TextColorOverride = Color.Yellow,
        };

        checkbox.CheckChanged += CheckboxEnableLayoutHotReloadOnCheckChanged;

        checkbox.SetToolTipText(Strings.Internals.ExperimentalFeatureTooltip);
        checkbox.TooltipFont = Skin.DefaultFont;

        return checkbox;
    }

    private static void CheckboxEnableLayoutHotReloadOnCheckChanged(ICheckbox sender, EventArgs _)
    {
        Globals.ContentManager.ContentWatcher.Enabled = sender.IsChecked;
    }

    private LabeledCheckBox CreateInfoCheckboxIncludeTextNodesInHover(Base parent)
    {
        var checkbox = new LabeledCheckBox(parent, nameof(CheckboxIncludeTextNodesInHover))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            IsChecked = _nodeUnderCursorProvider.Filter.HasFlag(NodeFilter.IncludeText),
            Text = Strings.Debug.IncludeTextNodesInHover,
        };

        checkbox.CheckChanged += CheckboxIncludesTextNodesInHoverOnCheckChanged;

        return checkbox;
    }

    private void CheckboxIncludesTextNodesInHoverOnCheckChanged(ICheckbox checkbox, EventArgs eventArgs)
    {
        if (_nodeUnderCursorProvider.Filter.HasFlag(NodeFilter.IncludeText))
        {
            _nodeUnderCursorProvider.Filter &= ~NodeFilter.IncludeText;
        }
        else
        {
            _nodeUnderCursorProvider.Filter |= NodeFilter.IncludeText;
        }
    }

    private LabeledCheckBox CreateInfoCheckboxViewClickedNodeInDebugger(Base parent)
    {
        var checkbox = new LabeledCheckBox(parent, nameof(CheckboxViewClickedNodeInDebugger))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            IsChecked = _viewClickedNodeInDebugger,
            Text = Strings.Debug.ViewClickedNodeInDebugger,
            TooltipText = Strings.Debug.ViewClickedNodeInDebuggerTooltip,
        };

        checkbox.CheckChanged += CheckboxViewClickedNodeInDebuggerOnCheckChanged;

        return checkbox;
    }

    private void CheckboxViewClickedNodeInDebuggerOnCheckChanged(ICheckbox checkbox, EventArgs eventArgs)
    {
        _viewClickedNodeInDebugger = !_viewClickedNodeInDebugger;
        if (_viewClickedNodeInDebugger)
        {
            AddIntercepts();
        }
        else
        {
            RemoveIntercepts();
        }
    }

    private void AddIntercepts()
    {
        Input.MouseDownIntercept += MouseDownIntercept;
        Input.MouseUpIntercept += MouseUpIntercept;
    }

    private void RemoveIntercepts()
    {
        Input.MouseDownIntercept -= MouseDownIntercept;
        Input.MouseUpIntercept -= MouseUpIntercept;
    }

    private bool MouseDownIntercept(Keys modifier, MouseButton mouseButton)
    {
        if (IsVisibleInTree && _viewClickedNodeInDebugger)
        {
            return true;
        }

        RemoveIntercepts();
        return false;
    }

    private bool MouseUpIntercept(Keys modifier, MouseButton mouseButton)
    {
        if (!IsVisibleInTree || !_viewClickedNodeInDebugger)
        {
            RemoveIntercepts();
            return false;
        }

        var node = GetNodeUnderCursor();
        Debugger.Break();
        return true;
    }

    private Button CreateInfoButtonShutdownServer(Base parent)
    {
        var button = new Button(parent, nameof(ButtonShutdownServer))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            IsHidden = true,
            Text = Strings.Debug.ShutdownServer,
        };

        button.Clicked += (_, _) =>
        {
            // TODO: Implement
        };

        return button;
    }

    private Button CreateInfoButtonShutdownServerAndExit(Base parent)
    {
        var button = new Button(parent, nameof(ButtonShutdownServerAndExit))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            IsHidden = true,
            Text = Strings.Debug.ShutdownServerAndExit,
        };

        button.Clicked += (_, _) =>
        {
            // TODO: Implement
        };

        return button;
    }

    private Table CreateInfoTableDebugStats(Base parent)
    {
        ScrollControl debugStatsScroller = new(parent, nameof(debugStatsScroller))
        {
            Dock = Pos.Fill,
        };

        debugStatsScroller.VerticalScrollBar.BaseNudgeAmount *= 2;

        var table = new Table(debugStatsScroller, nameof(TableDebugStats))
        {
            AutoSizeToContentHeightOnChildResize = true,
            AutoSizeToContentWidthOnChildResize = true,
            CellSpacing = new Point(8, 2),
            ColumnCount = 2,
            ColumnWidths = [180, null],
            Dock = Pos.Fill,
            Font = _defaultFont,
        };
        table.SizeChanged += ResizeTableToChildrenOnSizeChanged;

        table.AddRow(Strings.Debug.Fps, name: "FPSRow").Listen(1, new DelegateDataProvider<int?>(() => Graphics.Renderer?.Fps), NoValue);
        // table.AddRow(Strings.Debug.Draws, name: "DrawsRow").Listen(1, new DelegateDataProvider<int>(() => Graphics.DrawCalls), NoValue);
        table.AddRow(Strings.Debug.Ping, name: "PingRow").Listen(1, new DelegateDataProvider<int>(() => Networking.Network.Ping, delayMilliseconds: 5000), NoValue);

        table.AddRow(Strings.Debug.Map, name: "MapRow").Listen(1, new DelegateDataProvider<string?>(() => MapInstance.TryGet(Globals.Me?.MapId ?? default, out var mapInstance) ? mapInstance.Name : default), NoValue);
        table.AddRow(Strings.Internals.CoordinateX, name: "PlayerXRow").Listen(1, new DelegateDataProvider<int?>(() => Globals.Me?.X), NoValue);
        table.AddRow(Strings.Internals.CoordinateY, name: "PlayerYRow").Listen(1, new DelegateDataProvider<int?>(() => Globals.Me?.Y), NoValue);
        table.AddRow(Strings.Internals.CoordinateZ, name: "PlayerZRow").Listen(1, new DelegateDataProvider<int?>(() => Globals.Me?.Z), NoValue);

        table.AddRow(Strings.Debug.Time, name: "TimeRow").Listen(1, new DelegateDataProvider<string>(Time.GetTime), NoValue);
        // table.AddRow(Strings.Debug.KnownEntities, name: "KnownEntitiesRow").Listen(1, new DelegateDataProvider<int?>(() => 0), NoValue);
        table.AddRow(Strings.Debug.KnownMaps, name: "KnownMapsRow").Listen(1, new DelegateDataProvider<int>(() => MapInstance.Lookup.Count), NoValue);
        table.AddRow(Strings.Debug.MapsDrawn, name: "MapsDrawnRow").Listen(1, new DelegateDataProvider<int>(() => Graphics.MapsDrawn), NoValue);
        table.AddRow(Strings.Debug.EntitiesDrawn, name: "EntitiesDrawnRow").Listen(1, new DelegateDataProvider<int>(() => Graphics.EntitiesDrawn), NoValue);
        table.AddRow(Strings.Debug.LightsDrawn, name: "LightsDrawnRow").Listen(1, new DelegateDataProvider<int>(() => Graphics.LightsDrawn), NoValue);
        table.AddRow(Strings.Debug.InterfaceObjects, name: "InterfaceObjectsRow").Listen(1, new DelegateDataProvider<int?>(() => Interface.CurrentInterface?.NodeCount, delayMilliseconds: 1000), NoValue);

        // _ = table.AddRow(Strings.Debug.SectionGPUStatistics, columnCount: 2, name: "SectionGPU", columnIndex: 1);

        table.AddRow(Strings.Debug.RenderTargetAllocations, name: "GPURenderTargetAllocations").Listen(1, new DelegateDataProvider<ulong>(() => Graphics.Renderer.RenderTargetAllocations), NoValue);
        table.AddRow(Strings.Debug.TextureAllocations, name: "GPUTextureAllocations").Listen(1, new DelegateDataProvider<ulong>(() => Graphics.Renderer.TextureAllocations), NoValue);
        table.AddRow(Strings.Debug.TextureCount, name: "GPUTextureCount").Listen(1, new DelegateDataProvider<ulong>(() => Graphics.Renderer.TextureCount), NoValue);

        table.AddRow(Strings.Debug.FreeVRAM, name: "GPUFreeVRAM").Listen(1, new DelegateDataProvider<string>(() => FileSystemHelper.FormatSize(Graphics.Renderer.AvailableMemory)), NoValue);
        table.AddRow(Strings.Debug.TotalVRAM, name: "GPUTotalVRAM").Listen(1, new DelegateDataProvider<string>(() => FileSystemHelper.FormatSize(Graphics.Renderer.TotalMemory)), NoValue);

        table.AddRow(Strings.Debug.RenderBufferVRAMFree, name: "GPUVRAMRenderBuffers").Listen(1, new DelegateDataProvider<string>(() => FileSystemHelper.FormatSize(GL.AvailableRenderBufferMemory)), NoValue);
        table.AddRow(Strings.Debug.TextureVRAMFree, name: "GPUVRAMTextures").Listen(1, new DelegateDataProvider<string>(() => FileSystemHelper.FormatSize(GL.AvailableTextureMemory)), NoValue);
        table.AddRow(Strings.Debug.VBOVRAMFree, name: "GPUVRAMVBOs").Listen(1, new DelegateDataProvider<string>(() => FileSystemHelper.FormatSize(GL.AvailableVBOMemory)), NoValue);

        // _ = table.AddRow(Strings.Debug.ControlUnderCursor, columnCount: 2, name: "SectionUI", columnIndex: 1);

        table.AddRow(Strings.Internals.Type, name: "TypeRow").Listen(1, _nodeUnderCursorProvider, (node, _) => node?.GetType().GetName(), Strings.Internals.NotApplicable);
        table.AddRow(Strings.Internals.Name, name: "NameRow").Listen(1, _nodeUnderCursorProvider, (node, _) => node?.ParentQualifiedName, NoValue);
        table.AddRow(Strings.Internals.IsVisibleInTree, name: "IsVisible").Listen(1, _nodeUnderCursorProvider, (node, _) => node?.IsVisibleInTree,  NoValue);
        table.AddRow(Strings.Internals.IsVisibleInParent, name: "IsVisibleInParent").Listen(1, _nodeUnderCursorProvider, (node, _) => node?.IsVisibleInParent,  NoValue);
        table.AddRow(Strings.Internals.IsDisabled, name: "IsDisabled").Listen(1, _nodeUnderCursorProvider, (node, _) => node?.IsDisabled,  NoValue);
        table.AddRow(Strings.Internals.IsDisabledByTree, name: "IsDisabledByTree").Listen(1, _nodeUnderCursorProvider, (node, _) => node?.IsDisabledByTree,  NoValue);
        table.AddRow(Strings.Internals.LocalItem.ToString(Strings.Internals.Bounds), name: "LocalBoundsRow").Listen(1, _nodeUnderCursorProvider, (node, _) => node?.Bounds,  NoValue);
        table.AddRow(Strings.Internals.GlobalItem.ToString(Strings.Internals.Bounds), name: "GlobalBoundsRow").Listen(1, _nodeUnderCursorProvider, (node, _) => node?.GlobalBounds,  NoValue);
        table.AddRow(Strings.Internals.InnerBounds, name: "InnerBoundsRow").Listen(1, _nodeUnderCursorProvider, (node, _) => node?.InnerBounds,  NoValue);
        table.AddRow(Strings.Internals.OuterBounds, name: "OuterBounds").Listen(1, _nodeUnderCursorProvider, (node, _) => node?.OuterBounds,  NoValue);
        table.AddRow(Strings.Internals.Margin, name: "MarginRow").Listen(1, _nodeUnderCursorProvider, (node, _) => node?.Margin, NoValue);
        table.AddRow(Strings.Internals.Padding, name: "PaddingRow").Listen(1, _nodeUnderCursorProvider, (node, _) => node?.Padding, NoValue);
        table.AddRow(Strings.Internals.Dock, name: "Dock").Listen(1, _nodeUnderCursorProvider, (node, _) => node?.Dock, NoValue);
        table.AddRow(Strings.Internals.Alignment, name: "Alignment").Listen(1, _nodeUnderCursorProvider, (node, _) => node is not { Alignment.Length: >0 } ? null : string.Join(", ", node.Alignment), NoValue);
        table.AddRow(Strings.Internals.Color, name: "ColorRow").Listen(1, _nodeUnderCursorProvider, (node, _) => (node as IColorableText)?.TextColor, NoValue);
        table.AddRow(Strings.Internals.ColorOverride, name: "ColorOverrideRow").Listen(1, _nodeUnderCursorProvider, (node, _) => (node as IColorableText)?.TextColorOverride, NoValue);
        table.AddRow(Strings.Internals.TextAlign, name: "TextAlign").Listen(1, _nodeUnderCursorProvider, (node, _) => (node as Label)?.TextAlign, NoValue);
        table.AddRow(Strings.Internals.Font, name: "Font").Listen(1, _nodeUnderCursorProvider, (node, _) => (node as Label)?.FontName, NoValue);
        table.AddRow(Strings.Internals.FontSize, name: "FontSize").Listen(1, _nodeUnderCursorProvider, (node, _) => (node as Label)?.FontSize, NoValue);
        table.AddRow(Strings.Internals.AutoSizeToContents, name: nameof(IAutoSizeToContents.AutoSizeToContents)).Listen(1, _nodeUnderCursorProvider, (node, _) => (node as IAutoSizeToContents)?.AutoSizeToContents, NoValue);
        table.AddRow(Strings.Internals.AutoSizeToContentWidth, name: nameof(ISmartAutoSizeToContents.AutoSizeToContentWidth)).Listen(1, _nodeUnderCursorProvider, (node, _) => (node as ISmartAutoSizeToContents)?.AutoSizeToContentWidth, NoValue);
        table.AddRow(Strings.Internals.AutoSizeToContentHeight, name: nameof(ISmartAutoSizeToContents.AutoSizeToContentHeight)).Listen(1, _nodeUnderCursorProvider, (node, _) => (node as ISmartAutoSizeToContents)?.AutoSizeToContentHeight, NoValue);
        table.AddRow(Strings.Internals.AutoSizeToContentWidthOnChildResize, name: nameof(ISmartAutoSizeToContents.AutoSizeToContentWidthOnChildResize)).Listen(1, _nodeUnderCursorProvider, (node, _) => (node as ISmartAutoSizeToContents)?.AutoSizeToContentWidthOnChildResize, NoValue);
        table.AddRow(Strings.Internals.AutoSizeToContentHeightOnChildResize, name: nameof(ISmartAutoSizeToContents.AutoSizeToContentHeightOnChildResize)).Listen(1, _nodeUnderCursorProvider, (node, _) => (node as ISmartAutoSizeToContents)?.AutoSizeToContentHeightOnChildResize, NoValue);

        var rows = table.Children.OfType<TableRow>().ToArray();
        foreach (var row in rows)
        {
            if (row.Name.StartsWith("Section") && row.GetCellContents(1) is Label titleLabel)
            {
                titleLabel.Padding = titleLabel.Padding with { Top = 8 };
            }

            if (row.GetCellContents(0) is not Label label)
            {
                continue;
            }

            label.TextAlign = Pos.Right | Pos.CenterV;
        }

        return table;
    }

    private Base? GetNodeUnderCursor() => Interface.FindComponentUnderCursor(_nodeUnderCursorProvider.Filter);
}
