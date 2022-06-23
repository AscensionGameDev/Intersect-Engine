using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Intersect.Async;
using Intersect.Client.Core;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.Data;
using Intersect.Client.Framework.Gwen.Control.Layout;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Maps;
using Intersect.Extensions;

using static Intersect.Client.Framework.File_Management.GameContentManager;

namespace Intersect.Client.Interface.Debugging
{
    internal sealed partial class DebugWindow : WindowControl
    {
        private readonly List<IDisposable> _disposables;
        private bool _wasParentDrawDebugOutlinesEnabled;
        private bool _drawDebugOutlinesEnabled;

        public DebugWindow(Base parent) : base(parent, Strings.Debug.Title, false, nameof(DebugWindow))
        {
            _disposables = new List<IDisposable>();

            DisableResizing();

            CheckboxDrawDebugOutlines = CreateCheckboxDrawDebugOutlines();
            CheckboxEnableLayoutHotReloading = CreateCheckboxEnableLayoutHotReloading();
            ButtonShutdownServer = CreateButtonShutdownServer();
            ButtonShutdownServerAndExit = CreateButtonShutdownServerAndExit();
            TableDebugStats = CreateTableDebugStats();

            LoadJsonUi(UI.Debug, Graphics.Renderer.GetResolutionString());

            IsHidden = true;
        }

        private LabeledCheckBox CheckboxDrawDebugOutlines { get; }

        private LabeledCheckBox CheckboxEnableLayoutHotReloading { get; }

        private Button ButtonShutdownServer { get; }

        private Button ButtonShutdownServerAndExit { get; }

        private Table TableDebugStats { get; }

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

        protected override void OnDetached()
        {
            base.OnDetached();
        }

        protected override void OnDetaching(Base oldParent)
        {
            base.OnDetaching(oldParent);
            oldParent.DrawDebugOutlines = _wasParentDrawDebugOutlinesEnabled;
        }

        protected override void Render(Framework.Gwen.Skin.Base skin)
        {
            base.Render(skin);
        }

        public override void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }

            base.Dispose();
        }

        private LabeledCheckBox CreateCheckboxDrawDebugOutlines()
        {
            var checkbox = new LabeledCheckBox(this, nameof(CheckboxDrawDebugOutlines))
            {
                IsChecked = Root?.DrawDebugOutlines ?? false,
                Text = Strings.Debug.DrawDebugOutlines,
            };

            checkbox.CheckChanged += (sender, args) =>
            {
                _drawDebugOutlinesEnabled = checkbox.IsChecked;
                if (Root != default)
                {
                    Root.DrawDebugOutlines = _drawDebugOutlinesEnabled;
                }
            };

            return checkbox;
        }

        private LabeledCheckBox CreateCheckboxEnableLayoutHotReloading()
        {
            var checkbox = new LabeledCheckBox(this, nameof(CheckboxEnableLayoutHotReloading))
            {
                IsChecked = Globals.ContentManager.ContentWatcher.Enabled,
                Text = Strings.Debug.EnableLayoutHotReloading,
            };

            checkbox.CheckChanged += (sender, args) =>
            {
                Globals.ContentManager.ContentWatcher.Enabled = checkbox.IsChecked;
            };

            checkbox.SetToolTipText(Strings.Internals.ExperimentalFeatureTooltip);
            checkbox.ToolTipFont = Skin.DefaultFont;

            return checkbox;
        }

        private Button CreateButtonShutdownServer()
        {
            var button = new Button(this, nameof(ButtonShutdownServer))
            {
                IsHidden = true,
                Text = Strings.Debug.ShutdownServer,
            };

            button.Clicked += (sender, args) =>
            {
                // TODO: Implement
            };

            return button;
        }

        private Button CreateButtonShutdownServerAndExit()
        {
            var button = new Button(this, nameof(ButtonShutdownServerAndExit))
            {
                IsHidden = true,
                Text = Strings.Debug.ShutdownServerAndExit,
            };

            button.Clicked += (sender, args) =>
            {
                // TODO: Implement
            };

            return button;
        }

        private Table CreateTableDebugStats()
        {
            var table = new Table(this, nameof(TableDebugStats))
            {
                ColumnCount = 2,
            };

            var fpsProvider = new ValueTableCellDataProvider<int>(() => Graphics.Renderer.GetFps());
            table.AddRow(Strings.Debug.Fps).Listen(fpsProvider, 1);
            _disposables.Add(fpsProvider.Generator.Start());

            var pingProvider = new ValueTableCellDataProvider<int>(() => Networking.Network.Ping);
            table.AddRow(Strings.Debug.Ping).Listen(pingProvider, 1);
            _disposables.Add(pingProvider.Generator.Start());

            var drawCallsProvider = new ValueTableCellDataProvider<int>(() => Graphics.DrawCalls);
            table.AddRow(Strings.Debug.Draws).Listen(drawCallsProvider, 1);
            _disposables.Add(drawCallsProvider.Generator.Start());

            var mapNameProvider = new ValueTableCellDataProvider<string>(() =>
            {
                var mapId = Globals.Me?.MapId ?? default;

                if (mapId == default)
                {
                    return Strings.Internals.NotApplicable;
                }

                return MapInstance.Get(mapId)?.Name ?? Strings.Internals.NotApplicable;
            });
            table.AddRow(Strings.Debug.Map).Listen(mapNameProvider, 1);
            _disposables.Add(mapNameProvider.Generator.Start());

            var coordinateXProvider = new ValueTableCellDataProvider<int>(() => Globals.Me?.X ?? -1);
            table.AddRow(Strings.Internals.CoordinateX).Listen(coordinateXProvider, 1);
            _disposables.Add(coordinateXProvider.Generator.Start());

            var coordinateYProvider = new ValueTableCellDataProvider<int>(() => Globals.Me?.Y ?? -1);
            table.AddRow(Strings.Internals.CoordinateY).Listen(coordinateYProvider, 1);
            _disposables.Add(coordinateYProvider.Generator.Start());

            var coordinateZProvider = new ValueTableCellDataProvider<int>(() => Globals.Me?.Z ?? -1);
            table.AddRow(Strings.Internals.CoordinateZ).Listen(coordinateZProvider, 1);
            _disposables.Add(coordinateZProvider.Generator.Start());

            var knownEntitiesProvider = new ValueTableCellDataProvider<int>(() => Graphics.DrawCalls);
            table.AddRow(Strings.Debug.KnownEntities).Listen(knownEntitiesProvider, 1);
            _disposables.Add(knownEntitiesProvider.Generator.Start());

            var knownMapsProvider = new ValueTableCellDataProvider<int>(() => MapInstance.Lookup.Count);
            table.AddRow(Strings.Debug.KnownMaps).Listen(knownMapsProvider, 1);
            _disposables.Add(knownMapsProvider.Generator.Start());

            var mapsDrawnProvider = new ValueTableCellDataProvider<int>(() => Graphics.MapsDrawn);
            table.AddRow(Strings.Debug.MapsDrawn).Listen(mapsDrawnProvider, 1);
            _disposables.Add(mapsDrawnProvider.Generator.Start());

            var entitiesDrawnProvider = new ValueTableCellDataProvider<int>(() => Graphics.EntitiesDrawn);
            table.AddRow(Strings.Debug.EntitiesDrawn).Listen(entitiesDrawnProvider, 1);
            _disposables.Add(entitiesDrawnProvider.Generator.Start());

            var lightsDrawnProvider = new ValueTableCellDataProvider<int>(() => Graphics.LightsDrawn);
            table.AddRow(Strings.Debug.LightsDrawn).Listen(lightsDrawnProvider, 1);
            _disposables.Add(lightsDrawnProvider.Generator.Start());

            var timeProvider = new ValueTableCellDataProvider<string>(() => Time.GetTime());
            table.AddRow(Strings.Debug.Time).Listen(timeProvider, 1);
            _disposables.Add(timeProvider.Generator.Start());

            var interfaceObjectsProvider = new ValueTableCellDataProvider<int>((cancellationToken) =>
            {
                try
                {
                    return Interface.CurrentInterface?.Children.ToArray().SelectManyRecursive(x => x.Children, cancellationToken).ToArray().Length ?? 0;
                }
                catch (ObjectDisposedException)
                {
                    throw;
                }
                catch (InvalidOperationException)
                {
                    return 0;
                }
            });
            table.AddRow(Strings.Debug.InterfaceObjects).Listen(interfaceObjectsProvider, 1);
            _disposables.Add(interfaceObjectsProvider.Generator.Start());

            _ = table.AddRow(Strings.Debug.ControlUnderCursor, 1);

            var controlUnderCursorProvider = new ControlUnderCursorProvider();
            table.AddRow(Strings.Internals.Type).Listen(controlUnderCursorProvider, 0);
            table.AddRow(Strings.Internals.Name).Listen(controlUnderCursorProvider, 1);
            table.AddRow(Strings.Internals.LocalItem.ToString(Strings.Internals.Bounds)).Listen(controlUnderCursorProvider, 2);
            table.AddRow(Strings.Internals.GlobalItem.ToString(Strings.Internals.Bounds)).Listen(controlUnderCursorProvider, 3);
            table.AddRow(Strings.Internals.Color).Listen(controlUnderCursorProvider, 4);
            table.AddRow(Strings.Internals.ColorOverride).Listen(controlUnderCursorProvider, 5);
            _disposables.Add(controlUnderCursorProvider.Generator.Start());

            return table;
        }

        private partial class ControlUnderCursorProvider : ITableDataProvider
        {
            public ControlUnderCursorProvider()
            {
                Generator = new CancellableGenerator<Base>(CreateControlUnderCursorGenerator);
            }

            public event TableDataChangedEventHandler DataChanged;

            public CancellableGenerator<Base> Generator { get; }

            public void Start()
            {
                _ = Generator.Start();
            }

            private AsyncValueGenerator<Base> CreateControlUnderCursorGenerator(CancellationToken cancellationToken)
            {
                return new AsyncValueGenerator<Base>(() => Task.Delay(100).ContinueWith((completedTask) => Interface.FindControlAtCursor(), TaskScheduler.Current), (component) =>
                {
                    DataChanged?.Invoke(this, new TableDataChangedEventArgs(0, 1, default, component?.GetType()?.Name ?? Strings.Internals.NotApplicable));
                    DataChanged?.Invoke(this, new TableDataChangedEventArgs(1, 1, default, component?.CanonicalName ?? string.Empty));
                    DataChanged?.Invoke(this, new TableDataChangedEventArgs(2, 1, default, component?.Bounds.ToString() ?? string.Empty));
                    DataChanged?.Invoke(this, new TableDataChangedEventArgs(3, 1, default, component?.BoundsGlobal.ToString() ?? string.Empty));
                    DataChanged?.Invoke(this, new TableDataChangedEventArgs(4, 1, default, (component as IColorableText)?.TextColor ?? string.Empty));
                    DataChanged?.Invoke(this, new TableDataChangedEventArgs(5, 1, default, (component as IColorableText)?.TextColorOverride ?? string.Empty));

                }, cancellationToken);
            }
        }
    }
}
