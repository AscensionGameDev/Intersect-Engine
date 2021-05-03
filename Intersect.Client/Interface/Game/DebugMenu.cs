using Intersect.Client.Core;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Maps;
using Intersect.Extensions;
using System.Linq;

namespace Intersect.Client.Interface.Game
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

        private Label mInterfaceObjectsLabel;

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

            mInterfaceObjectsLabel = new Label(mDebugWindow);
            mInterfaceObjectsLabel.SetPosition(4, 160);
        }

        public void Update()
        {
            if (mDebugWindow.IsHidden)
            {
                return;
            }

            mFpsLabel.Text = Strings.Debug.fps.ToString(Graphics.Renderer.GetFps());
            mPingLabel.Text = Strings.Debug.ping.ToString(Networking.Network.Ping);
            mDrawsLabel.Text = Strings.Debug.draws.ToString(Graphics.DrawCalls);
            if (MapInstance.Get(Globals.Me.CurrentMap) != null)
            {
                mMapLabel.Text = Strings.Debug.map.ToString(MapInstance.Get(Globals.Me.CurrentMap).Name);
                mXLabel.Text = Strings.Debug.x.ToString(Globals.Me.X);
                mYLabel.Text = Strings.Debug.y.ToString(Globals.Me.Y);
                mZLabel.Text = Strings.Debug.z.ToString(Globals.Me.Z);
            }

            var entityCount = Globals.Entities.Count;
            foreach (MapInstance map in MapInstance.Lookup.Values)
            {
                if (map != null)
                {
                    entityCount += map.LocalEntities.Count;
                }
            }

            mEntitiesLabel.Text = Strings.Debug.knownentities.ToString(Globals.Entities.Count);
            mMapsLoadedLabel.Text = Strings.Debug.knownmaps.ToString(MapInstance.Lookup.Count);
            mMapsDrawnLabel.Text = Strings.Debug.mapsdrawn.ToString(Graphics.MapsDrawn);
            mEntitiesDrawnLabel.Text = Strings.Debug.entitiesdrawn.ToString(+Graphics.EntitiesDrawn);
            mLightsDrawnLabel.Text = Strings.Debug.lightsdrawn.ToString(Graphics.LightsDrawn);
            mTimeLabel.Text = Strings.Debug.time.ToString(Time.GetTime());
        }

        public void Show()
        {
            mDebugWindow.IsHidden = false;

            //This linq query takes too long and hurts fps, so let's only update this label when opening the debug menu
            mInterfaceObjectsLabel.Text = Strings.Debug.interfaceobjects.ToString(Interface.GameUi.GameCanvas.Children.ToArray().SelectManyRecursive(x => x.Children).ToArray().Length);
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
