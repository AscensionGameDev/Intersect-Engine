/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Game
{
    class DebugMenu
    {
        //Controls
        private WindowControl _debugWindow;
        private Label _fpsLabel;
        private Label _pingLabel;
        private Label _drawsLabel;
        private Label _mapLabel;
        private Label _xLabel;
        private Label _yLabel;
        private Label _zLabel;
        private Label _entitiesLabel;
        private Label _mapsLoadedLabel;
        private Label _mapsDrawnLabel;
        private Label _entitiesDrawnLabel;

        //Init
        public DebugMenu(Canvas _gameCanvas)
        {
            _debugWindow = new WindowControl(_gameCanvas, "Debug");
            _debugWindow.SetSize(200, 200);
            _debugWindow.SetPosition(0, 150);
            _debugWindow.DisableResizing();
            _debugWindow.Margin = Margin.Zero;
            _debugWindow.Padding = Padding.Zero;
            _debugWindow.Hide();

            _fpsLabel = new Label(_debugWindow);
            _fpsLabel.SetPosition(4, 4);
            _fpsLabel.Text = "FPS: ";

            _pingLabel = new Label(_debugWindow);
            _pingLabel.SetPosition(4, 16);
            _pingLabel.Text = "Ping: ";

            _drawsLabel = new Label(_debugWindow);
            _drawsLabel.SetPosition(4, 28);
            _drawsLabel.Text = "Draws: ";

            _mapLabel = new Label(_debugWindow);
            _mapLabel.SetPosition(4, 40);
            _mapLabel.Text = "Map: ";

            _xLabel = new Label(_debugWindow);
            _xLabel.SetPosition(4, 52);
            _xLabel.Text = "X: ";

            _yLabel = new Label(_debugWindow);
            _yLabel.SetPosition(4, 64);
            _yLabel.Text = "Y: ";

            _zLabel = new Label(_debugWindow);
            _zLabel.SetPosition(4, 76);
            _zLabel.Text = "Z: ";

            _entitiesLabel = new Label(_debugWindow);
            _entitiesLabel.SetPosition(4, 88);
            _entitiesLabel.Text = "Known Entities: ";

            _mapsLoadedLabel = new Label(_debugWindow);
            _mapsLoadedLabel.SetPosition(4, 100);
            _mapsLoadedLabel.Text = "Known Maps: ";

            _mapsDrawnLabel = new Label(_debugWindow);
            _mapsDrawnLabel.SetPosition(4, 112);
            _mapsDrawnLabel.Text = "Maps Drawn: ";

            _entitiesDrawnLabel = new Label(_debugWindow);
            _entitiesDrawnLabel.SetPosition(4, 124);
            _entitiesDrawnLabel.Text = "Entities Drawn: ";


        }
        public void Update()
        {
            _fpsLabel.Text = "FPS: " + GameGraphics.Renderer.GetFps();
            _pingLabel.Text = "Ping: " + GameNetwork.Ping;
            _drawsLabel.Text = "Draws: " + GameGraphics.DrawCalls;
            if (Globals.CurrentMap > -1 && Globals.GameMaps.ContainsKey(Globals.CurrentMap))
            {
                _mapLabel.Text = "Map: " + Globals.Me.CurrentMap + "  (" + Globals.GameMaps[Globals.CurrentMap].MyName + ")";
                _xLabel.Text = "X: " + Globals.Me.CurrentX;
                _yLabel.Text = "Y: " + Globals.Me.CurrentY;
                _zLabel.Text = "Z: " + Globals.Me.CurrentZ;
            }
            _entitiesLabel.Text = "Known Entities: " + Globals.Entities.Count;
            _mapsLoadedLabel.Text = "Known Maps: " + Globals.GameMaps.Count;
            _mapsDrawnLabel.Text = "Maps Drawn: " + GameGraphics.MapsDrawn;
            _entitiesDrawnLabel.Text = "Entities Drawn: " + GameGraphics.EntitiesDrawn;
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
