using Intersect.Localization;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Maps;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Game
{
    class DebugMenu
    {
        //Controls
        private WindowControl _debugWindow;

        private Label _drawsLabel;
        private Label _entitiesDrawnLabel;
        private Label _entitiesLabel;
        private Label _fpsLabel;
        private Label _lightsDrawnLabel;
        private Label _mapLabel;
        private Label _mapsDrawnLabel;
        private Label _mapsLoadedLabel;
        private Label _pingLabel;
        private Label _timeLabel;
        private Label _xLabel;
        private Label _yLabel;
        private Label _zLabel;

        //Init
        public DebugMenu(Canvas _gameCanvas)
        {
            _debugWindow = new WindowControl(_gameCanvas, Strings.Get("debug", "title"));
            _debugWindow.SetSize(200, 212);
            _debugWindow.SetPosition(0, 150);
            _debugWindow.DisableResizing();
            _debugWindow.Margin = Margin.Zero;
            _debugWindow.Padding = Padding.Zero;
            _debugWindow.Hide();

            _fpsLabel = new Label(_debugWindow);
            _fpsLabel.SetPosition(4, 4);

            _pingLabel = new Label(_debugWindow);
            _pingLabel.SetPosition(4, 16);

            _drawsLabel = new Label(_debugWindow);
            _drawsLabel.SetPosition(4, 28);

            _mapLabel = new Label(_debugWindow);
            _mapLabel.SetPosition(4, 40);

            _xLabel = new Label(_debugWindow);
            _xLabel.SetPosition(4, 52);

            _yLabel = new Label(_debugWindow);
            _yLabel.SetPosition(4, 64);

            _zLabel = new Label(_debugWindow);
            _zLabel.SetPosition(4, 76);

            _entitiesLabel = new Label(_debugWindow);
            _entitiesLabel.SetPosition(4, 88);

            _mapsLoadedLabel = new Label(_debugWindow);
            _mapsLoadedLabel.SetPosition(4, 100);

            _mapsDrawnLabel = new Label(_debugWindow);
            _mapsDrawnLabel.SetPosition(4, 112);

            _entitiesDrawnLabel = new Label(_debugWindow);
            _entitiesDrawnLabel.SetPosition(4, 124);

            _lightsDrawnLabel = new Label(_debugWindow);
            _lightsDrawnLabel.SetPosition(4, 136);

            _timeLabel = new Label(_debugWindow);
            _timeLabel.SetPosition(4, 148);
        }

        public void Update()
        {
            _fpsLabel.Text = Strings.Get("debug", "fps", GameGraphics.Renderer.GetFps());
            _pingLabel.Text = Strings.Get("debug", "ping", GameNetwork.Ping);
            _drawsLabel.Text = Strings.Get("debug", "draws", GameGraphics.DrawCalls);
            if (MapInstance.Lookup.Get<MapInstance>(Globals.Me.CurrentMap) != null)
            {
                _mapLabel.Text = Strings.Get("debug", "map",
                    MapInstance.Lookup.Get<MapInstance>(Globals.Me.CurrentMap).Name);
                _xLabel.Text = Strings.Get("debug", "x", Globals.Me.CurrentX);
                _yLabel.Text = Strings.Get("debug", "y", Globals.Me.CurrentY);
                _zLabel.Text = Strings.Get("debug", "z", Globals.Me.CurrentZ);
            }
            int entityCount = Globals.Entities.Count;
            foreach (MapInstance map in MapInstance.Lookup.IndexValues)
            {
                if (map != null) entityCount += map.LocalEntities.Count;
            }
            _entitiesLabel.Text = Strings.Get("debug", "knownentities", Globals.Entities.Count);
            _mapsLoadedLabel.Text = Strings.Get("debug", "knownmaps", MapInstance.Lookup.Count);
            _mapsDrawnLabel.Text = Strings.Get("debug", "mapsdrawn", GameGraphics.MapsDrawn);
            _entitiesDrawnLabel.Text = Strings.Get("debug", "entitiesdrawn", +GameGraphics.EntitiesDrawn);
            _lightsDrawnLabel.Text = Strings.Get("debug", "lightsdrawn", GameGraphics.LightsDrawn);
            _timeLabel.Text = Strings.Get("debug", "time", ClientTime.GetTime());
        }

        public void Show()
        {
            _debugWindow.IsHidden = false;
        }

        public bool IsVisible()
        {
            return !_debugWindow.IsHidden;
        }

        public void Hide()
        {
            _debugWindow.IsHidden = true;
        }
    }
}