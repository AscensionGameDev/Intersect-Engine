using System;
using System.Collections.Generic;
using System.Linq;
using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Network.Packets.Server;
using static Intersect.Client.Entities.Player;

namespace Intersect.Client.Interface.Game
{

    class GuildWindow
    {

        private Button mAddButton;

        private Button mAddPopupButton;

        private Button mLeave;

        private ListBox mGuildMembers;

        //Controls
        private WindowControl mGuildWindow;

        private TextBox mSearchTextbox;

        private ImagePanel mTextboxContainer;

        private bool mAddButtonUsed;

        private bool mAddPopupButtonUsed;

        //Context Menu
        private Framework.Gwen.Control.Menu mContextMenu;

        private MenuItem mPmOption;

        private MenuItem[] mPromoteOptions;

        private MenuItem[] mDemoteOptions;

        private MenuItem mKickOption;

        private MenuItem mTransferOption;

        private GuildMember mSelectedMember;

        //Init
        public GuildWindow(Canvas gameCanvas)
        {
            mGuildWindow = new WindowControl(gameCanvas, Globals.Me.Guild, false, "GuildWindow");
            mGuildWindow.DisableResizing();

            mTextboxContainer = new ImagePanel(mGuildWindow, "SearchContainer");
            mSearchTextbox = new TextBox(mTextboxContainer, "SearchTextbox");
            Interface.FocusElements.Add(mSearchTextbox);

            mGuildMembers = new ListBox(mGuildWindow, "GuildMembers");

            mAddButton = new Button(mGuildWindow, "InviteButton");
            mAddButton.SetText("+");
            mAddButton.Clicked += addButton_Clicked;

            mLeave = new Button(mGuildWindow, "LeaveButton");
            mLeave.SetText(Strings.Guilds.Leave);
            mLeave.Clicked += leave_Clicked;

            mAddPopupButton = new Button(mGuildWindow, "InvitePopupButton");
            mAddPopupButton.IsHidden = true;
            mAddPopupButton.SetText(Strings.Guilds.Invite);
            mAddPopupButton.Clicked += addPopupButton_Clicked;

            mContextMenu = new Framework.Gwen.Control.Menu(gameCanvas, "GuildContextMenu");
            mContextMenu.IsHidden = true;
            mContextMenu.IconMarginDisabled = true;

            //Add Context Menu Options
            //TODO: Is this a memory leak?
            mContextMenu.Children.Clear();

            mPmOption = mContextMenu.AddItem(Strings.Guilds.PM.ToString());
            mPmOption.Clicked += pmOption_Clicked;

            mPromoteOptions = new MenuItem[Options.Instance.Guild.Ranks.Length - 2];
            for (int i = 1; i < Options.Instance.Guild.Ranks.Length - 1; i++)
            {
                mPromoteOptions[i - 1] = mContextMenu.AddItem(Strings.Guilds.Promote.ToString(Options.Instance.Guild.Ranks[i].Title));
                mPromoteOptions[i - 1].UserData = i;
                mPromoteOptions[i - 1].Clicked += promoteOption_Clicked;
            }

            mDemoteOptions = new MenuItem[Options.Instance.Guild.Ranks.Length - 2];
            for (int i = 2; i < Options.Instance.Guild.Ranks.Length; i++)
            {
                mDemoteOptions[i - 2] = mContextMenu.AddItem(Strings.Guilds.Demote.ToString(Options.Instance.Guild.Ranks[i].Title));
                mDemoteOptions[i - 2].UserData = i;
                mDemoteOptions[i - 2].Clicked += demoteOption_Clicked;
            }

            mKickOption = mContextMenu.AddItem(Strings.Guilds.Kick.ToString());
            mKickOption.Clicked += kickOption_Clicked;

            mTransferOption = mContextMenu.AddItem(Strings.Guilds.Transfer.ToString());
            mTransferOption.Clicked += transferOption_Clicked;

            UpdateList();

            mContextMenu.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
            mGuildWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

            mAddButtonUsed = !mAddButton.IsHidden;
            mAddPopupButtonUsed = !mAddPopupButton.IsHidden;
        }

        //Methods
        public void Update()
        {
            if (mGuildWindow.IsHidden)
            {
                return;
            }

            // Force our window title to co-operate, might be empty after creating/joining a guild.
            if (mGuildWindow.Title != Globals.Me.Guild)
            {
                mGuildWindow.Title = Globals.Me.Guild;
            }
        }

        public void Show()
        {
            mGuildWindow.IsHidden = false;
        }

        public bool IsVisible()
        {
            return !mGuildWindow.IsHidden;
        }

        public void Hide()
        {
            mGuildWindow.IsHidden = true;
        }

        #region "Adding/Leaving"
        void addButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (mSearchTextbox.Text.Trim().Length >= 3) //Don't bother sending a packet less than the char limit
            {
                PacketSender.SendInviteGuild(mSearchTextbox.Text);
            }
        }

        void addPopupButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var iBox = new InputBox(
                Strings.Guilds.InviteMemberTitle, Strings.Guilds.InviteMemberPrompt.ToString(Globals.Me.Guild), true, InputBox.InputType.TextInput,
                AddMember, null, 0
            );
        }

        private void leave_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var iBox = new InputBox(
                Strings.Guilds.LeaveTitle, Strings.Guilds.LeavePrompt, true, InputBox.InputType.YesNo,
                LeaveGuild, null, 0
            );
        }

        private void LeaveGuild(object sender, EventArgs e)
        {
            PacketSender.SendLeaveGuild();
        }

        private void AddMember(Object sender, EventArgs e)
        {
            var ibox = (InputBox)sender;
            if (ibox.TextValue.Trim().Length >= 3) //Don't bother sending a packet less than the char limit
            {
                PacketSender.SendInviteGuild(ibox.TextValue);
            }
        }

        #endregion

        #region "Member List"
        public void UpdateList()
        {
            //Clear previous instances if already existing
            if (mGuildMembers != null)
            {
                mGuildMembers.Clear();
            }

            foreach (var f in Globals.Me.GuildMembers)
            {
                var str = f.Online ? Strings.Guilds.OnlineListEntry : Strings.Guilds.OfflineListEntry;
                var row = mGuildMembers.AddRow(str.ToString(Options.Instance.Guild.Ranks[f.Rank].Title, f.Name, f.Map));
                row.Name = "GuildMemberRow";
                row.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
                row.SetToolTipText(Strings.Guilds.Tooltip.ToString(f.Level, f.Class));
                row.UserData = f;
                row.Clicked += member_Clicked;
                row.RightClicked += member_RightClicked;

                //Row Render color (red = offline, green = online)
                if (f.Online == true)
                {
                    row.SetTextColor(Color.Green);
                }
                else
                {
                    row.SetTextColor(Color.Red);
                }

                row.RenderColor = new Color(50, 255, 255, 255);
            }

            //Determine if we can invite
            mAddButton.IsHidden = Globals.Me == null || Globals.Me.GuildRank == null || !Globals.Me.GuildRank.Permissions.Invite || !mAddButtonUsed;
            mTextboxContainer.IsHidden = Globals.Me == null || Globals.Me.GuildRank == null || !Globals.Me.GuildRank.Permissions.Invite || !mAddButtonUsed;
            mAddPopupButton.IsHidden = Globals.Me == null || Globals.Me.GuildRank == null || !Globals.Me.GuildRank.Permissions.Invite || !mAddPopupButtonUsed;
            mLeave.IsHidden = Globals.Me != null && Globals.Me.Rank == 0;
        }

        void member_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var row = (ListBoxRow)sender;
            var member = (GuildMember)row.UserData;

            //Only pm online players
            if (member?.Online == true && member?.Id != Globals.Me?.Id)
            {
                Interface.GameUi.SetChatboxText("/pm " + member.Name + " ");
            }
        }

        private void member_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            var row = (ListBoxRow)sender;
            var member = (GuildMember)row.UserData;

            //Only pm online players
            if (member != null && member.Id != Globals.Me?.Id)
            {
                mSelectedMember = member;

                var rank = Globals.Me?.GuildRank ?? null;

                if (rank != null)
                {
                    //Remove and re-add children
                    foreach (var child in mContextMenu.Children.ToArray())
                    {
                        mContextMenu.RemoveChild(child, false);
                    }

                    var rankIndex = Globals.Me.Rank;
                    var isOwner = rankIndex == 0;

                    if (mSelectedMember?.Online ?? false)
                    {
                        mContextMenu.AddChild(mPmOption);
                    }

                    //Promote Options
                    foreach (var opt in mPromoteOptions)
                    {
                        if ((isOwner || rank.Permissions.Promote) && (int)opt.UserData > rankIndex && (int)opt.UserData < member.Rank && member.Rank > rankIndex)
                        {
                            mContextMenu.AddChild(opt);
                        }
                    }

                    //Demote Options
                    foreach (var opt in mDemoteOptions)
                    {
                        if ((isOwner || rank.Permissions.Demote) && (int)opt.UserData > rankIndex && (int)opt.UserData > member.Rank && member.Rank > rankIndex)
                        {
                            mContextMenu.AddChild(opt);
                        }
                    }

                    if ((rank.Permissions.Kick || isOwner) && member.Rank > rankIndex)
                    {
                        mContextMenu.AddChild(mKickOption);
                    }

                    if (isOwner)
                    {
                        mContextMenu.AddChild(mTransferOption);
                    }

                    mContextMenu.IsHidden = false;
                    mContextMenu.SetSize(mContextMenu.Width, mContextMenu.Height);
                    mContextMenu.Open(Framework.Gwen.Pos.None);
                    mContextMenu.MoveTo(mContextMenu.X, mContextMenu.Y);
                }
            }
        }

        #endregion

        #region "Kicking"
        private void kickOption_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var rank = Globals.Me?.GuildRank ?? null;
            var rankIndex = Globals.Me?.Rank;
            var isOwner = rankIndex == 0;
            if (mSelectedMember != null && (rank.Permissions.Kick || isOwner) && mSelectedMember.Rank > rankIndex)
            {
                var iBox = new InputBox(
                    Strings.Guilds.KickTitle, Strings.Guilds.KickPrompt.ToString(mSelectedMember?.Name), true, InputBox.InputType.YesNo,
                    KickMember, null, mSelectedMember
                );
            }
        }

        private void KickMember(object sender, EventArgs e)
        {
            var input = (InputBox)sender;
            var member = (GuildMember)input.UserData;
            PacketSender.SendKickGuildMember(member.Id);
        }

        #endregion

        #region "Transferring"
        private void transferOption_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var rank = Globals.Me?.GuildRank ?? null;
            var rankIndex = Globals.Me?.Rank;
            var isOwner = rankIndex == 0;
            if (mSelectedMember != null && (rank.Permissions.Kick || isOwner) && mSelectedMember.Rank > rankIndex)
            {
                var iBox = new InputBox(
                    Strings.Guilds.TransferTitle, Strings.Guilds.TransferPrompt.ToString(mSelectedMember?.Name, rank.Title, Globals.Me?.Guild), true, InputBox.InputType.TextInput,
                    TransferGuild, null, mSelectedMember
                );
            }
        }

        private void TransferGuild(object sender, EventArgs e)
        {
            var input = (InputBox)sender;
            var member = (GuildMember)input.UserData;
            var text = input.TextValue;
            if (text == Globals.Me?.Guild)
            {
                PacketSender.SendTransferGuild(member.Id);
            }
        }

        #endregion



        private void pmOption_Clicked(Base sender, ClickedEventArgs arguments)
        {
            //Only pm online players
            if (mSelectedMember?.Online == true && mSelectedMember?.Id != Globals.Me?.Id)
            {
                Interface.GameUi.SetChatboxText("/pm " + mSelectedMember.Name + " ");
            }
        }

        #region "Promoting"
        private void promoteOption_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var rank = Globals.Me?.GuildRank ?? null;
            var rankIndex = Globals.Me?.Rank;
            var isOwner = rankIndex == 0;
            var newRank = (int)sender.UserData;
            if (mSelectedMember != null && (rank.Permissions.Kick || isOwner) && mSelectedMember.Rank > rankIndex)
            {
                var iBox = new InputBox(
                    Strings.Guilds.PromoteTitle, Strings.Guilds.PromotePrompt.ToString(mSelectedMember?.Name, Options.Instance.Guild.Ranks[newRank].Title), true, InputBox.InputType.YesNo,
                    PromoteMember, null, new KeyValuePair<GuildMember, int>(mSelectedMember, newRank)
                );
            }
        }

        private void PromoteMember(object sender, EventArgs e)
        {
            var input = (InputBox)sender;
            var memberRankPair = (KeyValuePair<GuildMember, int>)input.UserData;
            PacketSender.SendPromoteGuildMember(memberRankPair.Key?.Id ?? Guid.Empty, memberRankPair.Value);
        }

        #endregion


        #region "Demoting"
        private void demoteOption_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var rank = Globals.Me?.GuildRank ?? null;
            var rankIndex = Globals.Me?.Rank;
            var isOwner = rankIndex == 0;
            var newRank = (int)sender.UserData;
            if (mSelectedMember != null && (rank.Permissions.Kick || isOwner) && mSelectedMember.Rank > rankIndex)
            {
                var iBox = new InputBox(
                    Strings.Guilds.DemoteTitle, Strings.Guilds.DemotePrompt.ToString(mSelectedMember?.Name, Options.Instance.Guild.Ranks[newRank].Title), true, InputBox.InputType.YesNo,
                    DemoteMember, null, new KeyValuePair<GuildMember, int>(mSelectedMember, newRank)
                );
            }
        }

        private void DemoteMember(object sender, EventArgs e)
        {
            var input = (InputBox)sender;
            var memberRankPair = (KeyValuePair<GuildMember, int>)input.UserData;
            PacketSender.SendDemoteGuildMember(memberRankPair.Key?.Id ?? Guid.Empty, memberRankPair.Value);
        }

        #endregion

    }

}
