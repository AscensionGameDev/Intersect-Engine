using System;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;

namespace Intersect.Client.UI.Game
{
    class FriendsWindow
    {
        private Button mAddButton;
        private Button mAddPopupButton;
        private ListBox mFriends;

        //Controls
        private WindowControl mFriendsWindow;

        private TextBox mSearchTextbox;
        private ImagePanel mTextboxContainer;

        //Temp variables
        private string mTempName;

        //Init
        public FriendsWindow(Canvas gameCanvas)
        {
            mFriendsWindow = new WindowControl(gameCanvas, Strings.Friends.title, false, "FriendsWindow");
            mFriendsWindow.DisableResizing();

            mTextboxContainer = new ImagePanel(mFriendsWindow, "SearchContainer");
            mSearchTextbox = new TextBox(mTextboxContainer, "SearchTextbox");
            Gui.FocusElements.Add(mSearchTextbox);

            mFriends = new ListBox(mFriendsWindow, "FriendsList");

            mAddButton = new Button(mFriendsWindow, "AddFriendButton");
            mAddButton.SetText("+");
            mAddButton.Clicked += addButton_Clicked;

            mAddPopupButton = new Button(mFriendsWindow, "AddFriendPopupButton");
            mAddPopupButton.IsHidden = true;
            mAddPopupButton.SetText(Strings.Friends.addfriend);
            mAddPopupButton.Clicked += addPopupButton_Clicked;

            UpdateList();

            mFriendsWindow.LoadJsonUi(GameContentManager.UI.InGame, GameGraphics.Renderer.GetResolutionString());
        }

        //Methods
        public void Update()
        {
            if (mFriendsWindow.IsHidden)
            {
                return;
            }
        }

        public void Show()
        {
            mFriendsWindow.IsHidden = false;
        }

        public bool IsVisible()
        {
            return !mFriendsWindow.IsHidden;
        }

        public void Hide()
        {
            mFriendsWindow.IsHidden = true;
        }

        public void UpdateList()
        {
            //Clear previous instances if already existing
            if (mFriends != null)
            {
                mFriends.Clear();
            }

            foreach (var f in Globals.Me.Friends)
            {
                var row = mFriends.AddRow(f.Name + " - " + f.Map);
                row.UserData = f.Name;
                row.Clicked += friends_Clicked;
                row.RightClicked += friends_RightClicked;

                //Row Render color (red = offline, green = online)
                if (f.Online == true)
                {
                    row.SetTextColor(Framework.GenericClasses.Color.Green);
                }
                else
                {
                    row.SetTextColor(Framework.GenericClasses.Color.Red);
                }
                row.RenderColor = new Framework.GenericClasses.Color(50, 255, 255, 255);
            }
        }

        void addButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (mSearchTextbox.Text.Trim().Length >= 3) //Don't bother sending a packet less than the char limit
            {
                PacketSender.AddFriend(mSearchTextbox.Text);
            }
        }

        void addPopupButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            InputBox iBox = new InputBox(Strings.Friends.addfriend,
                Strings.Friends.addfriendprompt,
                true, InputBox.InputType.TextInput, AddFriend, null, 0);
        }

        void friends_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var row = (ListBoxRow) sender;

            //Only pm online players
            foreach (var friend in Globals.Me.Friends)
            {
                if (friend.Name.ToLower() == friend.Name.ToLower())
                {
                    if (friend.Online == true)
                    {
                        Gui.GameUi.SetChatboxText("/pm " + (string) row.UserData + " ");
                    }
                }
            }
        }

        void friends_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            var row = (ListBoxRow) sender;
            mTempName = (string) row.UserData;

            InputBox iBox = new InputBox(Strings.Friends.removefriend,
                Strings.Friends.removefriendprompt.ToString(mTempName),
                true, InputBox.InputType.YesNo, RemoveFriend, null, 0);
        }

        private void RemoveFriend(Object sender, EventArgs e)
        {
            PacketSender.RemoveFriend(mTempName);
        }

        private void AddFriend(Object sender, EventArgs e)
        {
            var ibox = (InputBox) sender;
            if (ibox.TextValue.Trim().Length >= 3) //Don't bother sending a packet less than the char limit
            {
                PacketSender.AddFriend(ibox.TextValue);
            }
        }
    }
}