using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;

namespace Intersect.Client.Interface.Game.EntityPanel
{

    public partial class PlayerStatusWindow : ImagePanel
    {
        private readonly Dictionary<Guid, SpellStatus> _activeStatuses = new Dictionary<Guid, SpellStatus>();

        public Entities.Player MyEntity;

        public bool UpdateStatuses;

        public readonly ImagePanel _playerStatusWindow;

        public readonly ScrollControl _playerStatusControl;

        public PlayerStatusWindow(Canvas gameCanvas) : base(gameCanvas, "PlayerStatusWindow")
        {
            MyEntity = Globals.Me;
            _playerStatusControl = new ScrollControl(this, "PlayerStatusControl");
            LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        }

        public void Update()
        {
            if (MyEntity == null || MyEntity.IsDisposed())
            {
                if (!IsHidden)
                {
                    Hide();
                }

                return;
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

            IsHidden = _activeStatuses.Count < 1;

            if (!IsHidden)
            {
                Show();
            }
        }
    }
}