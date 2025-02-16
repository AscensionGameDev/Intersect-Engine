using Intersect.Client.Core;
using Intersect.Client.Entities;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Enums;
using Intersect.Framework.Core;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game;

/// <summary>
/// The GUI class for the Target Context Menu that pop up on-screen during gameplay.
/// </summary>
public sealed partial class TargetContextMenu : Framework.Gwen.Control.Menu
{
    private readonly MenuItem _targetNameMenuItem;
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
        Children.Clear();

        _me = Globals.Me;

        _targetNameMenuItem = AddItem(string.Empty);
        _targetNameMenuItem.MouseInputEnabled = false;
        AddDivider();

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
            TryShowTargetButton(shouldShowTargetNameMenuItem);
            TryShowGuildButton();
            SizeToChildren();
            Open(Pos.None);
            SetPosition(newX, newY);
        }
        else if (!Globals.InputManager.IsMouseButtonDown(MouseButton.Right))
        {
            Close();
        }
    }

    private void TryShowTargetButton(bool shouldShow)
    {
        _targetNameMenuItem.SetText(shouldShow ? _entity.Name : string.Empty);

        if (shouldShow)
        {
            var indexOf = Children.IndexOf(_targetNameMenuItem);

            if (indexOf > 0)
            {
                Children.RemoveAt(indexOf);
            }

            if (indexOf != 0)
            {
                Children.Insert(0, _targetNameMenuItem);
            }
        }
        else
        {
            Children.Remove(_targetNameMenuItem);
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

    void TryShowGuildButton()
    {
        var shouldShow = false;
        if (_entity is Player plyr && _entity != _me && string.IsNullOrWhiteSpace(plyr.Guild))
        {
            if (_me?.GuildRank?.Permissions?.Invite ?? false)
            {
                shouldShow = true;
            }
        }

        if (shouldShow)
        {
            if (!Children.Contains(_guildMenuItem))
            {
                Children.Add(_guildMenuItem);
            }
        }
        else
        {
            if (Children.Contains(_guildMenuItem))
            {
                Children.Remove(_guildMenuItem);
            }
        }
    }
}