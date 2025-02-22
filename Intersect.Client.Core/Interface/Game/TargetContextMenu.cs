using Intersect.Client.Core;
using Intersect.Client.Entities;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Enums;
using Intersect.Framework.Core;

namespace Intersect.Client.Interface.Game;

/// <summary>
/// The GUI class for the Target Context Menu that pop up on-screen during gameplay.
/// </summary>
public sealed partial class TargetContextMenu : ContextMenu
{
    private readonly MenuItem _targetNameMenuItem;
    private readonly MenuDivider _nameDivider;
    private readonly MenuItem _tradeMenuItem;
    private readonly MenuItem _partyMenuItem;
    private readonly MenuItem _friendMenuItem;
    private readonly MenuItem _guildMenuItem;
    private readonly MenuItem _privateMessageMenuItem;
    private readonly Player? _me;
    private IEntity? _entity;

    public TargetContextMenu(Canvas gameCanvas) : base(gameCanvas, nameof(TargetContextMenu))
    {
        IsHidden = true;
        IconMarginDisabled = true;
        ClearChildren();

        _me = Globals.Me;

        _targetNameMenuItem = AddItem(string.Empty);
        _nameDivider = new MenuDivider(this)
        {
            Dock = Pos.Top,
            Margin = new Margin(IconMarginDisabled ? 0 : 24, 0, 4, 0),
            Height = 1,
            MinimumSize = new Point(0, 1),
        };

        _tradeMenuItem = AddItem(Strings.EntityContextMenu.Trade);
        _tradeMenuItem.Clicked += tradeRequest_Clicked;

        _partyMenuItem = AddItem(Strings.EntityContextMenu.InviteToParty);
        _partyMenuItem.Clicked += invite_Clicked;

        _friendMenuItem = AddItem(Strings.EntityContextMenu.AddFriend);
        _friendMenuItem.Clicked += friendRequest_Clicked;

        _guildMenuItem = AddItem(Strings.EntityContextMenu.InviteToGuild);
        _guildMenuItem.Clicked += guildRequest_Clicked;

        _privateMessageMenuItem = AddItem(Strings.EntityContextMenu.PrivateMessage);
        _privateMessageMenuItem.Clicked += privateMessageRequest_Clicked;

        LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer?.GetResolutionString());
        _buildContextMenu();
    }

    public void ToggleHidden(object? target)
    {
        if (target == null || _me == null)
        {
            return;
        }

        bool shouldShowTargetNameMenuItem = false;

        float posX, posY, newX, newY;

        switch (target)
        {
            case Button button:
                _entity = _me.TargetBox?.MyEntity;
                posX = button.ToCanvas(Point.Empty).X;
                posY = button.ToCanvas(Point.Empty).Y;
                newX = posX;
                newY = posY + button.Height;
                break;

            case Player player when player != _me:
                var mousePos = Graphics.ConvertToWorldPoint(Globals.InputManager.MousePosition);
                if (!player.WorldPos.Contains(mousePos.X, mousePos.Y))
                {
                    return;
                }

                _entity = player;
                posX = InputHandler.MousePosition.X;
                posY = InputHandler.MousePosition.Y;
                newX = posX;
                newY = posY;

                shouldShowTargetNameMenuItem = true;
                break;

            default:
                return;
        }

        if (_entity == null)
        {
            return;
        }

        if (Canvas is { } canvas)
        {
            if (newX + Width >= canvas.Width)
            {
                newX = posX - Width + (target is Button button ? button.Width : 0);
            }

            if (newY + Height >= canvas.Height)
            {
                newY = posY - Height + (target is Button button ? button.Height : 0);
            }
        }

        if (IsHidden)
        {
            _buildContextMenu(shouldShowTargetNameMenuItem);
            SizeToChildren();
            Open(Pos.None);
            SetPosition(newX, newY);
        }
        else if (!Globals.InputManager.IsMouseButtonDown(MouseButton.Right))
        {
            Close();
        }
    }

    private void _buildContextMenu(bool shouldShowTargetName = false)
    {
        ClearChildren();

        if (shouldShowTargetName)
        {
            AddChild(_targetNameMenuItem);
            _targetNameMenuItem.SetText(_entity?.Name ?? string.Empty);
            _targetNameMenuItem.MouseInputEnabled = false;
            AddChild(_nameDivider);
        }

        AddChild(_tradeMenuItem);
        AddChild(_partyMenuItem);
        AddChild(_friendMenuItem);

        if (_entity is Player player && player != _me && string.IsNullOrWhiteSpace(player.Guild))
        {
            if (_me?.GuildRank?.Permissions?.Invite ?? false)
            {
                AddChild(_guildMenuItem);
            }
        }

        if (_entity is Player plyr && plyr != _me)
        {
            AddChild(_privateMessageMenuItem);
        }
    }

    void invite_Clicked(Base sender, MouseButtonState arguments)
    {
        if (_me == null || _entity is not Player || _entity == _me)
        {
            return;
        }

        if (_me.CombatTimer < Timing.Global.Milliseconds)
        {
            PacketSender.SendPartyInvite(_entity.Id);
        }
        else
        {
            PacketSender.SendChatMsg(Strings.Parties.InFight.ToString(), 4);
        }
    }

    void tradeRequest_Clicked(Base sender, MouseButtonState arguments)
    {
        if (_me == null || _entity is not Player || _entity == _me)
        {
            return;
        }

        if (_me.CombatTimer < Timing.Global.Milliseconds)
        {
            PacketSender.SendTradeRequest(_entity.Id);
        }
        else
        {
            PacketSender.SendChatMsg(Strings.Trading.InFight.ToString(), 4);
        }
    }

    void friendRequest_Clicked(Base sender, MouseButtonState arguments)
    {
        if (_me == null || _entity is not Player || _entity == _me)
        {
            return;
        }

        if (_me.CombatTimer < Timing.Global.Milliseconds)
        {
            PacketSender.SendAddFriend(_entity.Name);
        }
        else
        {
            PacketSender.SendChatMsg(Strings.Friends.InFight.ToString(), 4);
        }
    }

    void guildRequest_Clicked(Base sender, MouseButtonState arguments)
    {
        if (_me == null || _entity is not Player plyr || _entity == _me)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(plyr.Guild))
        {
            if (_me?.GuildRank?.Permissions?.Invite ?? false)
            {
                if (_me.CombatTimer < Timing.Global.Milliseconds)
                {
                    PacketSender.SendInviteGuild(_entity.Name);
                }
                else
                {
                    PacketSender.SendChatMsg(Strings.Friends.InFight.ToString(), 4);
                }
            }
        }
        else
        {
            Chat.ChatboxMsg.AddMessage(
                new Chat.ChatboxMsg(Strings.Guilds.InviteAlreadyInGuild, Color.Red, ChatMessageType.Guild)
            );
        }
    }

    void privateMessageRequest_Clicked(Base sender, MouseButtonState arguments)
    {
        if (_me == null || _entity is not Player || _entity == _me)
        {
            return;
        }

        Interface.GameUi.SetChatboxText($"/pm {_entity.Name} ");
    }
}