using Intersect.Client.Core;
using Intersect.Client.Entities;
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
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game;

/// <summary>
/// The GUI class for the Target Context Menu that pop up on-screen during gameplay.
/// </summary>
public sealed partial class TargetContextMenu : Framework.Gwen.Control.Menu
{
    private readonly MenuItem _tradeMenuItem;
    private readonly MenuItem _partyMenuItem;
    private readonly MenuItem _friendMenuItem;
    private readonly MenuItem _guildMenuItem;
    private readonly MenuItem _privateMessageMenuItem;
    private readonly Player? _me;
    private Entity? _entity;

    public TargetContextMenu(Canvas gameCanvas) : base(gameCanvas, nameof(TargetContextMenu))
    {
        IsHidden = true;
        IconMarginDisabled = true;
        Children.Clear();

        _me = Globals.Me;

        _tradeMenuItem = AddItem(Strings.EntityContextMenu.Trade);
        _tradeMenuItem.Clicked += tradeRequest_Clicked;

        _partyMenuItem = AddItem(Strings.EntityContextMenu.Party);
        _partyMenuItem.Clicked += invite_Clicked;

        _friendMenuItem = AddItem(Strings.EntityContextMenu.Friend);
        _friendMenuItem.Clicked += friendRequest_Clicked;

        _guildMenuItem = AddItem(Strings.EntityContextMenu.Guild);
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

        float posX, posY, newX, newY;

        switch (target)
        {
            case Button button:
            {
                _entity = _me.TargetBox?.MyEntity;
                posX = button.LocalPosToCanvas(new Point(0, 0)).X;
                posY = button.LocalPosToCanvas(new Point(0, 0)).Y;
                newX = posX;
                newY = posY + button.Height;

                if (newX + Width >= Canvas?.Width)
                {
                    newX = posX - Width + button.Width;
                }

                if (newY + Height >= Canvas?.Height)
                {
                    newY = posY - Height;
                }

                break;
            }
            case Entity en:
            {
                if (en is not Player || en == _me)
                {
                    return;
                }

                var mousePos = Graphics.ConvertToWorldPoint(Globals.InputManager.MousePosition);
                bool isHovered = en.WorldPos.Contains(mousePos.X, mousePos.Y);

                if (!isHovered)
                {
                    return;
                }

                _entity = en;
                posX = InputHandler.MousePosition.X;
                posY = InputHandler.MousePosition.Y;
                newX = posX;
                newY = posY;

                if (newX + Width >= Canvas?.Width)
                {
                    newX = posX - Width;
                }

                if (newY + Height >= Canvas?.Height)
                {
                    newY = posY - Height;
                }

                break;
            }
            default:
                return;
        }

        if (this.IsHidden)
        {
            TryShowGuildButton();
            SizeToChildren();
            Open(Pos.None);
            SetPosition(newX, newY);
        }
        else if (!Globals.InputManager.MouseButtonDown(MouseButtons.Right))
        {
            Close();
        }
    }

    void invite_Clicked(Base sender, ClickedEventArgs arguments)
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

    void tradeRequest_Clicked(Base sender, ClickedEventArgs arguments)
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

    void friendRequest_Clicked(Base sender, ClickedEventArgs arguments)
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

    void guildRequest_Clicked(Base sender, ClickedEventArgs arguments)
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

    void privateMessageRequest_Clicked(Base sender, ClickedEventArgs arguments)
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