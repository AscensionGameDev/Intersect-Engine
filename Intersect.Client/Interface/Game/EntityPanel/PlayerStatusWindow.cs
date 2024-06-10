using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;

namespace Intersect.Client.Interface.Game.EntityPanel
{

    public partial class PlayerStatusWindow
    {
        private readonly Dictionary<Guid, SpellStatus> _activeStatuses = new Dictionary<Guid, SpellStatus>();

        public Entities.Player MyEntity;

        public bool UpdateStatuses;

        public readonly ImagePanel _playerStatusWindow;

        public readonly ScrollControl _playerStatusControl;

        public PlayerStatusWindow(Canvas gameCanvas)
        {
            MyEntity = Globals.Me;

            _playerStatusWindow = new ImagePanel(gameCanvas, "PlayerStatusWindow")
            {
                ShouldCacheToTexture = true
            };

            _playerStatusControl = new ScrollControl(_playerStatusWindow, "PlayerStatusControl");
            _playerStatusWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
            Show();
        }

        public void Update()
        {
            if (MyEntity == null || MyEntity.IsDisposed())
            {
                if (!_playerStatusWindow.IsHidden)
                {
                    Hide();
                }

                return;
            }

            if (_playerStatusWindow.IsHidden)
            {
                Show();
            }

            if (MyEntity.IsDisposed())
            {
                Dispose();
                return;
            }

            if (UpdateStatuses)
            {
                SpellStatus.UpdateSpellStatus(MyEntity, _playerStatusControl, _activeStatuses);

                foreach (var itm in _activeStatuses)
                {
                    itm.Value.Update();
                }

                UpdateStatuses = false;
            }

            _playerStatusWindow.IsHidden = _activeStatuses.Count < 1;
        }

        public void Show()
        {
            _playerStatusWindow.Show();
        }

        public void Hide()
        {
            _playerStatusWindow.Hide();
        }

        public void Dispose()
        {
            Hide();
            Interface.GameUi.GameCanvas.RemoveChild(_playerStatusWindow, false);
            Dispose();
        }
    }
}