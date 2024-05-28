using Intersect.Client.Core;
using Intersect.Client.Entities;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Interface.Game.Entity
{

    public partial class PlayerStatusWindows
    {
        private readonly Dictionary<Guid, SpellStatus> _activeStatuses = new Dictionary<Guid, SpellStatus>();

        public Entities.Entity MyEntity;

        public readonly ImagePanel PlayerStatusWindow;

        public bool UpdateStatuses;

        //Init
        public PlayerStatusWindows(Canvas gameCanvas, Entities.Entity myEntity)
        {
            MyEntity = myEntity;
            PlayerStatusWindow = new ImagePanel(gameCanvas, nameof(PlayerStatusWindow))
            {
                ShouldCacheToTexture = true
            };

            SetEntity(myEntity);
            PlayerStatusWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
            UpdateSpellStatus();
            PlayerStatusWindow.Show();
        }

        public void SetEntity(Entities.Entity entity)
        {
            MyEntity = entity;
            if (MyEntity != null)
            {
                UpdateSpellStatus();
            }
        }

        public void Update()
        {
            if (MyEntity == null || MyEntity.IsDisposed())
            {
                if (!PlayerStatusWindow.IsHidden)
                {
                    PlayerStatusWindow.Hide();
                }

                return;
            }

            if (PlayerStatusWindow.IsHidden)
            {
                PlayerStatusWindow.Show();
            }

            if (MyEntity.IsDisposed())
            {
                Dispose();
                return;
            }

            UpdateSpellStatus();

            if (UpdateStatuses)
            {
                UpdateSpellStatus();
                UpdateStatuses = false;
            }

            foreach (var itm in _activeStatuses.Values)
            {
                itm.Update();
            }
        }

        public void UpdateSpellStatus()
        {
            if (MyEntity == null)
            {
                return;
            }

            //Remove 'Dead' Statuses
            foreach (var status in _activeStatuses.Keys.ToList())
            {
                if (!MyEntity.StatusActive(status))
                {
                    var s = _activeStatuses[status];
                    s.StatusIcon.Texture = null;
                    s.Container.Hide();
                    s.Container.Texture = null;
                    PlayerStatusWindow.RemoveChild(s.Container, true);
                    s.pnl_HoverLeave(null, null);
                    _activeStatuses.Remove(status);
                }
                else
                {
                    _activeStatuses[status].UpdateStatus(MyEntity.GetStatus(status) as Status);
                }
            }

            //Add all of the spell status effects
            for (var i = 0; i < MyEntity.Status.Count; i++)
            {
                var id = MyEntity.Status[i].SpellId;
                SpellStatus itm;
                if (!_activeStatuses.TryGetValue(id, out itm) && MyEntity.Status[i] is Status status)
                {
                    itm = new SpellStatus(this, status)
                    {
                        Container = new ImagePanel(PlayerStatusWindow, "PlayerStatusIcon")
                    };

                    itm.Setup(true);

                    itm.Container.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
                    itm.Container.Name = "";
                    _activeStatuses.Add(id, itm);
                }

                var xPadding = itm.Container.Margin.Left + itm.Container.Margin.Right;
                var yPadding = itm.Container.Margin.Top + itm.Container.Margin.Bottom;

                itm.Container.SetPosition(
                    i %
                    (PlayerStatusWindow.Width /
                     Math.Max(1, PlayerStatusWindow.Width / (itm.Container.Width + xPadding))) *
                    (itm.Container.Width + xPadding) +
                    xPadding,
                    i /
                    Math.Max(1, PlayerStatusWindow.Width / (itm.Container.Width + xPadding)) *
                    (itm.Container.Height + yPadding) +
                    yPadding
                );
            }
        }

        public void Show()
        {
            PlayerStatusWindow.Show();
        }

        public void Hide()
        {
            PlayerStatusWindow.Hide();
        }

        public void Dispose()
        {
            PlayerStatusWindow.Hide();
            Interface.GameUi.GameCanvas.RemoveChild(PlayerStatusWindow, false);
            PlayerStatusWindow.Dispose();
        }
    }
}
