using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Network.Packets.Server;

namespace Intersect.Client.Interface.Game;

partial class GuildWindow : WindowControl
{
    private readonly ImagePanel _textboxContainer;
    private readonly TextBox _textboxSearch;
    private readonly ListBox _listGuildMembers;
    private readonly Button _buttonAdd;
    private readonly Button _buttonLeave;
    private readonly Button _buttonAddPopup;
    private readonly Framework.Gwen.Control.Menu _contextMenu;
    private readonly MenuItem _privateMessageOption;
    private readonly MenuItem[] _promoteOptions;
    private readonly MenuItem[] _demoteOptions;
    private readonly MenuItem _kickOption;
    private readonly MenuItem _transferOption;

    private readonly bool _addButtonUsed;
    private readonly bool _addPopupButtonUsed;
    private GuildMember? _selectedMember;

    public GuildWindow(Canvas gameCanvas) : base(gameCanvas, Globals.Me?.Guild, false, nameof(GuildWindow))
    {
        DisableResizing();

        // Textbox Search
        _textboxContainer = new ImagePanel(this, "SearchContainer");
        _textboxSearch = new TextBox(_textboxContainer, "SearchTextbox");
        Interface.FocusComponents.Add(_textboxSearch);

        // List of Guild Members
        _listGuildMembers = new ListBox(this, "GuildMembers");

        #region Action Buttons

        // Add Button
        _buttonAdd = new Button(this, "InviteButton")
        {
            Text = Strings.Guilds.Add
        };
        _buttonAdd.Clicked += (s, e) =>
        {
            if (_textboxSearch.Text.Trim().Length >= 3)
            {
                PacketSender.SendInviteGuild(_textboxSearch.Text);
            }
        };

        // Leave Button
        _buttonLeave = new Button(this, "LeaveButton")
        {
            Text = Strings.Guilds.Leave
        };
        _buttonLeave.Clicked += (s, e) =>
        {
            _ = new InputBox(
                title: Strings.Guilds.LeaveTitle,
                prompt: Strings.Guilds.LeavePrompt.ToString(Globals.Me?.Guild),
                inputType: InputBox.InputType.YesNo,
                onSuccess: (s, e) => PacketSender.SendLeaveGuild()
            );
        };

        // Add Popup Button
        _buttonAddPopup = new Button(this, "InvitePopupButton")
        {
            Text = Strings.Guilds.Invite,
            IsHidden = true
        };
        _buttonAddPopup.Clicked += (s, e) =>
        {
            new InputBox(
                title: Strings.Guilds.InviteMemberTitle,
                prompt: Strings.Guilds.InviteMemberPrompt.ToString(Globals.Me?.Guild),
                inputType: InputBox.InputType.TextInput,
                onSuccess: (s, e) =>
                {
                    if (s is InputBox inputBox && inputBox.TextValue.Trim().Length >= 3)
                    {
                        PacketSender.SendInviteGuild(inputBox.TextValue);
                    }
                }
            ).Focus();
        };

        #endregion

        #region Context Menu Options

        // Context Menu
        _contextMenu = new Framework.Gwen.Control.Menu(gameCanvas, "GuildContextMenu")
        {
            IsHidden = true,
            IconMarginDisabled = true
        };

        //Add Context Menu Options
        //TODO: Is this a memory leak?
        _contextMenu.Children.Clear();

        // Private Message
        _privateMessageOption = _contextMenu.AddItem(Strings.Guilds.PM);
        _privateMessageOption.Clicked += (s, e) =>
        {
            if (_selectedMember?.Online == true && _selectedMember?.Id != Globals.Me?.Id)
            {
                Interface.GameUi.SetChatboxText("/pm " + _selectedMember!.Name + " ");
            }
        };

        // Promote Options
        _promoteOptions = new MenuItem[Options.Instance.Guild.Ranks.Length - 2];
        for (int i = 1; i < Options.Instance.Guild.Ranks.Length - 1; i++)
        {
            _promoteOptions[i - 1] = _contextMenu.AddItem(Strings.Guilds.Promote.ToString(Options.Instance.Guild.Ranks[i].Title));
            _promoteOptions[i - 1].UserData = i;
            _promoteOptions[i - 1].Clicked += promoteOption_Clicked;
        }

        // Demote Options
        _demoteOptions = new MenuItem[Options.Instance.Guild.Ranks.Length - 2];
        for (int i = 2; i < Options.Instance.Guild.Ranks.Length; i++)
        {
            _demoteOptions[i - 2] = _contextMenu.AddItem(Strings.Guilds.Demote.ToString(Options.Instance.Guild.Ranks[i].Title));
            _demoteOptions[i - 2].UserData = i;
            _demoteOptions[i - 2].Clicked += demoteOption_Clicked;
        }

        // Kick Option
        _kickOption = _contextMenu.AddItem(Strings.Guilds.Kick);
        _kickOption.Clicked += kickOption_Clicked;

        // Transfer Option
        _transferOption = _contextMenu.AddItem(Strings.Guilds.Transfer);
        _transferOption.Clicked += transferOption_Clicked;

        #endregion

        UpdateList();

        _contextMenu.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer?.GetResolutionString());
        LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer?.GetResolutionString());

        _addButtonUsed = !_buttonAdd.IsHidden;
        _addPopupButtonUsed = !_buttonAddPopup.IsHidden;
    }

    //Methods
    public void Update()
    {
        if (IsHidden)
        {
            return;
        }

        // Force our window title to co-operate, might be empty after creating/joining a guild.
        if (!string.IsNullOrEmpty(Globals.Me?.Guild) && Title != Globals.Me.Guild)
        {
            Title = Globals.Me.Guild;
        }
    }

    public override void Hide()
    {
        _contextMenu?.Close();
        base.Hide();
    }

    #region "Member List"

    public void UpdateList()
    {
        //Clear previous instances if already existing
        _listGuildMembers.Clear();

        foreach (var member in Globals.Me?.GuildMembers ?? [])
        {
            var str = member.Online ? Strings.Guilds.OnlineListEntry : Strings.Guilds.OfflineListEntry;
            var row = _listGuildMembers.AddRow(str.ToString(Options.Instance.Guild.Ranks[member.Rank].Title, member.Name, member.MapName));
            row.Name = "GuildMemberRow";
            row.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer?.GetResolutionString());
            row.SetToolTipText(Strings.Guilds.Tooltip.ToString(member.Level, member.ClassName));
            row.UserData = member;
            row.Clicked += member_Clicked;

            //Row Render color (red = offline, green = online)
            if (member.Online == true)
            {
                row.SetTextColor(Color.Green);
            }
            else
            {
                row.SetTextColor(Color.Red);
            }

            row.RenderColor = new Color(50, 255, 255, 255);
        }

        var isInviteDenied = Globals.Me == null || Globals.Me.GuildRank == null || !Globals.Me.GuildRank.Permissions.Invite;

        //Determine if we can invite
        _buttonAdd.IsHidden = isInviteDenied || !_addButtonUsed;
        _textboxContainer.IsHidden = isInviteDenied || !_addButtonUsed;
        _buttonAddPopup.IsHidden = isInviteDenied || !_addPopupButtonUsed;
        _buttonLeave.IsHidden = Globals.Me != null && Globals.Me.Rank == 0;
    }

    private void member_Clicked(Base sender, MouseButtonState arguments)
    {
        if (arguments.MouseButton == MouseButton.Right)
        {
            member_RightClicked(sender, arguments);
            return;
        }

        if (arguments.MouseButton != MouseButton.Left)
        {
            return;
        }

        if (sender is ListBoxRow { UserData: GuildMember { Online: true } member } &&
            member.Id != Globals.Me?.Id
           )
        {
            Interface.GameUi.SetChatboxText($"/pm {member.Name}");
        }
    }

    private void member_RightClicked(Base sender, MouseButtonState arguments)
    {
        if (sender is not ListBoxRow row || row.UserData is not GuildMember member)
        {
            return;
        }

        if (Globals.Me == default || member.Id == Globals.Me.Id)
        {
            return;
        }

        _selectedMember = member;

        var rank = Globals.Me.GuildRank ?? default;
        if (rank == null)
        {
            return;
        }

        //Remove and re-add children
        foreach (var child in _contextMenu.Children.ToArray())
        {
            _contextMenu.RemoveChild(child, false);
        }

        var rankIndex = Globals.Me.Rank;
        var isOwner = rankIndex == 0;

        if (_selectedMember.Online)
        {
            _contextMenu.AddChild(_privateMessageOption);
        }

        //Promote Options
        foreach (var opt in _promoteOptions)
        {
            var isAllowed = (isOwner || rank.Permissions.Promote);
            var hasLowerRank = (int)opt.UserData > rankIndex;
            var canRankChange = (int)opt.UserData < member.Rank && member.Rank > rankIndex;

            if (isAllowed && hasLowerRank && canRankChange)
            {
                _contextMenu.AddChild(opt);
            }
        }

        //Demote Options
        foreach (var opt in _demoteOptions)
        {
            var isAllowed = (isOwner || rank.Permissions.Demote);
            var hasLowerRank = (int)opt.UserData > rankIndex;
            var canRankChange = (int)opt.UserData > member.Rank && member.Rank > rankIndex;

            if (isAllowed && hasLowerRank && canRankChange)
            {
                _contextMenu.AddChild(opt);
            }
        }

        if ((rank.Permissions.Kick || isOwner) && member.Rank > rankIndex)
        {
            _contextMenu.AddChild(_kickOption);
        }

        if (isOwner)
        {
            _contextMenu.AddChild(_transferOption);
        }

        _ = _contextMenu.SizeToChildren();
        _contextMenu.Open(Framework.Gwen.Pos.None);
    }

    #endregion

    #region Guild Actions

    private void promoteOption_Clicked(Base sender, MouseButtonState arguments)
    {
        if (Globals.Me == default || Globals.Me.GuildRank == default || _selectedMember == default)
        {
            return;
        }

        var rank = Globals.Me.GuildRank;
        var rankIndex = Globals.Me.Rank;
        var isOwner = rankIndex == 0;
        var newRank = (int)sender.UserData;

        if (!(rank.Permissions.Promote || isOwner) || _selectedMember.Rank <= rankIndex)
        {
            return;
        }

        _ = new InputBox(
            Strings.Guilds.PromoteTitle,
            Strings.Guilds.PromotePrompt.ToString(_selectedMember.Name, Options.Instance.Guild.Ranks[newRank].Title),
            InputBox.InputType.YesNo,
            userData: new Tuple<GuildMember, int>(_selectedMember, newRank),
            onSuccess: (s, e) =>
            {
                if (s is InputBox inputBox && inputBox.UserData is Tuple<GuildMember, int> memberRankPair)
                {
                    var (member, newRank) = memberRankPair;
                    PacketSender.SendPromoteGuildMember(member.Id, newRank);
                }
            }
        );
    }

    private void demoteOption_Clicked(Base sender, MouseButtonState arguments)
    {
        if (Globals.Me == default || Globals.Me.GuildRank == default || _selectedMember == default)
        {
            return;
        }

        var rank = Globals.Me.GuildRank;
        var rankIndex = Globals.Me.Rank;
        var isOwner = rankIndex == 0;
        var newRank = (int)sender.UserData;

        if (!(rank.Permissions.Demote || isOwner) || _selectedMember.Rank <= rankIndex)
        {
            return;
        }

        _ = new InputBox(
            Strings.Guilds.DemoteTitle,
            Strings.Guilds.DemotePrompt.ToString(_selectedMember.Name, Options.Instance.Guild.Ranks[newRank].Title),
            InputBox.InputType.YesNo,
            userData: new Tuple<GuildMember, int>(_selectedMember, newRank),
            onSuccess: (s, e) =>
            {
                if (s is InputBox inputBox && inputBox.UserData is Tuple<GuildMember, int> memberRankPair)
                {
                    var (member, newRank) = memberRankPair;
                    PacketSender.SendDemoteGuildMember(member.Id, newRank);
                }
            }
        );
    }

    private void kickOption_Clicked(Base sender, MouseButtonState arguments)
    {
        if (Globals.Me == default || Globals.Me.GuildRank == default || _selectedMember == default)
        {
            return;
        }

        var rank = Globals.Me.GuildRank;
        var rankIndex = Globals.Me.Rank;
        var isOwner = rankIndex == 0;

        if (!(rank.Permissions.Kick || isOwner) || _selectedMember.Rank <= rankIndex)
        {
            return;
        }

        _ = new InputBox(
            Strings.Guilds.KickTitle,
            Strings.Guilds.KickPrompt.ToString(_selectedMember?.Name),
            InputBox.InputType.YesNo,
            userData: _selectedMember,
            onSuccess: (s, e) =>
            {
                if (s is InputBox inputBox && inputBox.UserData is GuildMember member)
                {
                    PacketSender.SendKickGuildMember(member.Id);
                }
            }
        );
    }

    private void transferOption_Clicked(Base sender, MouseButtonState arguments)
    {
        if (Globals.Me == default || Globals.Me.GuildRank == default || _selectedMember == default)
        {
            return;
        }

        var rank = Globals.Me.GuildRank;
        var rankIndex = Globals.Me.Rank;
        var isOwner = rankIndex == 0;

        if (!(rank.Permissions.Kick || isOwner) || _selectedMember.Rank <= rankIndex)
        {
            return;
        }

        _ = new InputBox(
            Strings.Guilds.TransferTitle,
            Strings.Guilds.TransferPrompt.ToString(_selectedMember?.Name, rank.Title, Globals.Me?.Guild),
            InputBox.InputType.TextInput,
            userData: _selectedMember,
            onSuccess: (s, e) =>
            {
                if (s is InputBox inputBox && inputBox.TextValue == Globals.Me?.Guild && inputBox.UserData is GuildMember member)
                {
                    PacketSender.SendTransferGuild(member.Id);
                }
            }
        );
    }

    #endregion
}
