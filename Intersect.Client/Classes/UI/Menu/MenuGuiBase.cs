using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using JetBrains.Annotations;

namespace Intersect.Client.UI.Menu
{
    public class MenuGuiBase
    {
        private readonly Canvas mMenuCanvas;

        public MainMenu MainMenu;
        private bool mShouldReset;

        [NotNull] private readonly ImagePanel mServerStatusArea;
        [NotNull] private readonly Label mServerStatusLabel;
        
        private static MainMenu.NetworkStatusHandler sNetworkStatusChanged;

        public MenuGuiBase(Canvas myCanvas)
        {
            mMenuCanvas = myCanvas;
            InitMenuGui();
            mServerStatusArea = new ImagePanel(mMenuCanvas, "ServerStatusArea");
            mServerStatusLabel = new Label(mServerStatusArea, "ServerStatusLabel")
            {
                Text = Strings.Server.StatusLabel.ToString(MainMenu.ActiveNetworkStatus.ToLocalizedString()),
            };
            mServerStatusArea.LoadJsonUi(GameContentManager.UI.Menu, GameGraphics.Renderer.GetResolutionString());
            MainMenu.NetworkStatusChanged += HandleNetworkStatusChanged;
        }

        ~MenuGuiBase()
        {
            // ReSharper disable once DelegateSubtraction
            MainMenu.NetworkStatusChanged -= HandleNetworkStatusChanged;
        }

        private void InitMenuGui()
        {
            MainMenu = new MainMenu(mMenuCanvas);
        }

        private void HandleNetworkStatusChanged()
        {
            mServerStatusLabel.Text = Strings.Server.StatusLabel.ToString(MainMenu.ActiveNetworkStatus.ToLocalizedString());
        }

        public void Draw()
        {
            if (mShouldReset)
            {
                MainMenu.Reset();
                mShouldReset = false;
            }
            MainMenu.Update();
            mMenuCanvas.RenderCanvas();
        }

        public void Reset()
        {
            mShouldReset = true;
        }

        //Dispose
        public void Dispose()
        {
            if (mMenuCanvas != null) mMenuCanvas.Dispose();
        }
    }
}