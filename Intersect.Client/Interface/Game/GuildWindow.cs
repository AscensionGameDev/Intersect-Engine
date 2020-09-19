﻿using System;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;

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
            mLeave.SetText(Strings.Guild.Leave);
            mLeave.Clicked += leave_Clicked;

            mAddPopupButton = new Button(mGuildWindow, "InvitePopupButton");
            mAddPopupButton.IsHidden = true;
            mAddPopupButton.SetText(Strings.Guild.Invite);
            mAddPopupButton.Clicked += addPopupButton_Clicked;

            UpdateList();

            mGuildWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
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

        public void UpdateList()
        {
            //Clear previous instances if already existing
            if (mGuildMembers != null)
            {
                mGuildMembers.Clear();
            }

            foreach (var f in Globals.Me.GuildMembers)
            {
                var row = mGuildMembers.AddRow(f.Name + " - " + f.Rank);
                row.UserData = f.Name;
                row.Clicked += member_Clicked;

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
        }

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
                Strings.Guild.InviteMemberTitle, Strings.Guild.InviteMemberPrompt.ToString(Globals.Me.Guild), true, InputBox.InputType.TextInput,
                AddMember, null, 0
            );
        }

        private void leave_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var iBox = new InputBox(
                Strings.Guild.LeaveTitle, Strings.Guild.LeavePrompt, true, InputBox.InputType.YesNo, 
                LeaveGuild, null, 0
            );
        }

        private void LeaveGuild(object sender, EventArgs e)
        {
            PacketSender.SendLeaveGuild();
        }

        void member_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var row = (ListBoxRow) sender;

            //Only pm online players
            foreach (var member in Globals.Me.GuildMembers)
                if (member.Name.ToLower() == ((string)row.UserData).ToLower())
                {
                    if (member.Online == true)
                    {
                        Interface.GameUi.SetChatboxText("/pm " + (string) row.UserData + " ");
                    }
            }
        }

        private void AddMember(Object sender, EventArgs e)
        {
            var ibox = (InputBox) sender;
            if (ibox.TextValue.Trim().Length >= 3) //Don't bother sending a packet less than the char limit
            {
                PacketSender.SendInviteGuild(ibox.TextValue);
            }
        }

    }

}
