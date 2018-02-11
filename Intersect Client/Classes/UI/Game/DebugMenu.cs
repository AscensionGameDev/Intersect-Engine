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
        private WindowControl mDebugWindow;

        private Label mDrawsLabel;
        private Label mEntitiesDrawnLabel;
        private Label mEntitiesLabel;
        private Label mFpsLabel;
        private Label mLightsDrawnLabel;
        private Label mMapLabel;
        private Label mMapsDrawnLabel;
        private Label mMapsLoadedLabel;
        private Label mPingLabel;
        private Label mTimeLabel;
        private Label mXLabel;
        private Label mYLabel;
        private Label mZLabel;

        //Init
        public DebugMenu(Canvas gameCanvas)
        {
            mDebugWindow = new WindowControl(gameCanvas, Strings.Get("debug", "title"));
            mDebugWindow.SetSize(200, 212);
            mDebugWindow.SetPosition(0, 150);
            mDebugWindow.DisableResizing();
            mDebugWindow.Margin = Margin.Zero;
            mDebugWindow.Padding = Padding.Zero;
            mDebugWindow.Hide();

            mFpsLabel = new Label(mDebugWindow);
            mFpsLabel.SetPosition(4, 4);

            mPingLabel = new Label(mDebugWindow);
            mPingLabel.SetPosition(4, 16);

            mDrawsLabel = new Label(mDebugWindow);
            mDrawsLabel.SetPosition(4, 28);

            mMapLabel = new Label(mDebugWindow);
            mMapLabel.SetPosition(4, 40);

            mXLabel = new Label(mDebugWindow);
            mXLabel.SetPosition(4, 52);

            mYLabel = new Label(mDebugWindow);
            mYLabel.SetPosition(4, 64);

            mZLabel = new Label(mDebugWindow);
            mZLabel.SetPosition(4, 76);

            mEntitiesLabel = new Label(mDebugWindow);
            mEntitiesLabel.SetPosition(4, 88);

            mMapsLoadedLabel = new Label(mDebugWindow);
            mMapsLoadedLabel.SetPosition(4, 100);

            mMapsDrawnLabel = new Label(mDebugWindow);
            mMapsDrawnLabel.SetPosition(4, 112);

            mEntitiesDrawnLabel = new Label(mDebugWindow);
            mEntitiesDrawnLabel.SetPosition(4, 124);

            mLightsDrawnLabel = new Label(mDebugWindow);
            mLightsDrawnLabel.SetPosition(4, 136);

            mTimeLabel = new Label(mDebugWindow);
            mTimeLabel.SetPosition(4, 148);
        }

        public void Update()
        {
            mFpsLabel.Text = Strings.Get("debug", "fps", GameGraphics.Renderer.GetFps());
            mPingLabel.Text = Strings.Get("debug", "ping", GameNetwork.Ping);
            mDrawsLabel.Text = Strings.Get("debug", "draws", GameGraphics.DrawCalls);
            if (MapInstance.Lookup.Get<MapInstance>(Globals.Me.CurrentMap) != null)
            {
                mMapLabel.Text = Strings.Get("debug", "map",
                    MapInstance.Lookup.Get<MapInstance>(Globals.Me.CurrentMap).Name);
                mXLabel.Text = Strings.Get("debug", "x", Globals.Me.CurrentX);
                mYLabel.Text = Strings.Get("debug", "y", Globals.Me.CurrentY);
                mZLabel.Text = Strings.Get("debug", "z", Globals.Me.CurrentZ);
            }
            int entityCount = Globals.Entities.Count;
            foreach (MapInstance map in MapInstance.Lookup.IndexValues)
            {
                if (map != null) entityCount += map.LocalEntities.Count;
            }
            mEntitiesLabel.Text = Strings.Get("debug", "knownentities", Globals.Entities.Count);
            mMapsLoadedLabel.Text = Strings.Get("debug", "knownmaps", MapInstance.Lookup.Count);
            mMapsDrawnLabel.Text = Strings.Get("debug", "mapsdrawn", GameGraphics.MapsDrawn);
            mEntitiesDrawnLabel.Text = Strings.Get("debug", "entitiesdrawn", +GameGraphics.EntitiesDrawn);
            mLightsDrawnLabel.Text = Strings.Get("debug", "lightsdrawn", GameGraphics.LightsDrawn);
            mTimeLabel.Text = Strings.Get("debug", "time", ClientTime.GetTime());
        }

        public void Show()
        {
            mDebugWindow.IsHidden = false;
        }

        public bool IsVisible()
        {
            return !mDebugWindow.IsHidden;
        }

        public void Hide()
        {
            mDebugWindow.IsHidden = true;
        }
    }
}