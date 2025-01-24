using Intersect.Client.Core;
using Intersect.Client.Entities;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;

namespace Intersect.Client.Interface.Game.EntityPanel;


public partial class PlayerStatusWindow : ImagePanel
{
    private readonly Dictionary<Guid, SpellStatus> _activeStatuses = [];

    public readonly ImagePanel _playerStatusWindow;

    public readonly ScrollControl _playerStatusControl;

    public Player? MyEntity;

    public bool ShouldUpdateStatuses { get; set; }

    public PlayerStatusWindow(Canvas gameCanvas, string? name = default) : base(gameCanvas, name ?? nameof(PlayerStatusWindow))
    {
        _playerStatusControl = new ScrollControl(this, "PlayerStatusControl");
        MyEntity = Globals.Me;
        LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
    }

    public void Update()
    {
        if (MyEntity?.IsDisposed ?? true)
        {
            if (!IsHidden)
            {
                Hide();
            }

            return;
        }

        if (MyEntity.IsDisposed)
        {
            Dispose();
            return;
        }

        if (ShouldUpdateStatuses)
        {
            SpellStatus.UpdateSpellStatus(MyEntity, _playerStatusControl, _activeStatuses);

            foreach (var (_, activeStatus) in _activeStatuses)
            {
                activeStatus.Update();
            }

            ShouldUpdateStatuses = _activeStatuses.Count > 0;
            if (!IsVisible)
            {
                IsVisible = ShouldUpdateStatuses;
            }
        }
        else if (IsVisible)
        {
            IsVisible = false;
        }
    }
}
