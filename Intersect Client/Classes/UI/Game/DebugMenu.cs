using Intersect.Client.Classes.Core;
using Intersect.Client.Classes.Localization;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
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
            mDebugWindow = new WindowControl(gameCanvas, Strings.Debug.title);
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
            mFpsLabel.Text = Strings.Debug.fps.ToString( GameGraphics.Renderer.GetFps());
            mPingLabel.Text = Strings.Debug.ping.ToString( GameNetwork.Ping);
            mDrawsLabel.Text = Strings.Debug.draws.ToString( GameGraphics.DrawCalls);
            if (MapInstance.Lookup.Get<MapInstance>(Globals.Me.CurrentMap) != null)
            {
                mMapLabel.Text = Strings.Debug.map.ToString(
                    MapInstance.Lookup.Get<MapInstance>(Globals.Me.CurrentMap).Name);
                mXLabel.Text = Strings.Debug.x.ToString( Globals.Me.CurrentX);
                mYLabel.Text = Strings.Debug.y.ToString( Globals.Me.CurrentY);
                mZLabel.Text = Strings.Debug.z.ToString( Globals.Me.CurrentZ);
            }
            int entityCount = Globals.Entities.Count;
            foreach (MapInstance map in MapInstance.Lookup.IndexValues)
            {
                if (map != null) entityCount += map.LocalEntities.Count;
            }
            mEntitiesLabel.Text = Strings.Debug.knownentities.ToString( Globals.Entities.Count);
            mMapsLoadedLabel.Text = Strings.Debug.knownmaps.ToString( MapInstance.Lookup.Count);
            mMapsDrawnLabel.Text = Strings.Debug.mapsdrawn.ToString( GameGraphics.MapsDrawn);
            mEntitiesDrawnLabel.Text = Strings.Debug.entitiesdrawn.ToString( +GameGraphics.EntitiesDrawn);
            mLightsDrawnLabel.Text = Strings.Debug.lightsdrawn.ToString( GameGraphics.LightsDrawn);
            mTimeLabel.Text = Strings.Debug.time.ToString( ClientTime.GetTime());
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