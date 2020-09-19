using System;

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

        private ListBox mGuildMembers;

        //Controls
        private WindowControl mGuildWindow;

        private TextBox mSearchTextbox;

        //Temp variables
        private string mTempName;

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
        }

        void addButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (mSearchTextbox.Text.Trim().Length >= 3) //Don't bother sending a packet less than the char limit
            {
                PacketSender.SendAddFriend(mSearchTextbox.Text);
            }
        }

        void addPopupButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            //var iBox = new InputBox(
            //    Strings.Friends.addfriend, Strings.Friends.addfriendprompt, true, InputBox.InputType.TextInput,
            //    AddFriend, null, 0
            //);
        }

        void member_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var row = (ListBoxRow) sender;

            //Only pm online players
            foreach (var member in Globals.Me.GuildMembers)
                if (member.Name.ToLower() == member.Name.ToLower())
                {
                    if (member.Online == true)
                    {
                        Interface.GameUi.SetChatboxText("/pm " + (string) row.UserData + " ");
                    }
            }
        }

        void member_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            //var row = (ListBoxRow) sender;
            //mTempName = (string) row.UserData;

            //var iBox = new InputBox(
            //    Strings.Friends.removefriend, Strings.Friends.removefriendprompt.ToString(mTempName), true,
            //    InputBox.InputType.YesNo, RemoveFriend, null, 0
            //);
        }

        private void RemoveMember(Object sender, EventArgs e)
        {
            PacketSender.SendRemoveFriend(mTempName);
        }

        private void AddMember(Object sender, EventArgs e)
        {
            var ibox = (InputBox) sender;
            if (ibox.TextValue.Trim().Length >= 3) //Don't bother sending a packet less than the char limit
            {
                PacketSender.SendAddFriend(ibox.TextValue);
            }
        }

    }

}
