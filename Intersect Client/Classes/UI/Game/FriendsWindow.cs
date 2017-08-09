using System;
using Intersect.Localization;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Game
{
    class FriendsWindow
    {
        private Button _addButton;
        private ListBox _friends;

        //Controls
        private WindowControl _friendsWindow;

        private TextBox _searchTextbox;
        private ImagePanel _textboxContainer;

        //Temp variables
        private string TempName;

        //Init
        public FriendsWindow(Canvas _gameCanvas)
        {
            _friendsWindow = new WindowControl(_gameCanvas, Strings.Get("friends", "title"), false, "FriendsWindow");
            _friendsWindow.DisableResizing();

            _textboxContainer = new ImagePanel(_friendsWindow, "SearchContainer");
            _searchTextbox = new TextBox(_textboxContainer, "SearchTextbox");
            Gui.FocusElements.Add(_searchTextbox);

            _addButton = new Button(_friendsWindow, "AddFriendButton");
            _addButton.SetText("+");
            _addButton.Clicked += addButton_Clicked;

            updateList();
        }

        //Methods
        public void Update()
        {
            if (_friendsWindow.IsHidden)
            {
                return;
            }
        }

        public void Show()
        {
            _friendsWindow.IsHidden = false;
        }

        public bool IsVisible()
        {
            return !_friendsWindow.IsHidden;
        }

        public void Hide()
        {
            _friendsWindow.IsHidden = true;
        }

        public void updateList()
        {
            //Clear previous instances if already existing
            if (_friends != null)
            {
                _friends.Clear();
            }

            _friends = new ListBox(_friendsWindow);
            _friends.SetBounds(6, 32, _friendsWindow.Width - 24, _friendsWindow.Height - 78);
            _friends.RenderColor.A = 0;

            foreach (var f in Globals.Me.Friends)
            {
                var row = _friends.AddRow(f.name + " - " + f.map);
                row.UserData = f.name;
                row.Clicked += friends_Clicked;
                row.RightClicked += friends_RightClicked;

                //Row Render color (red = offline, green = online)
                if (f.online == true)
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
            if (_searchTextbox.Text.Length >= 3) //Don't bother sending a packet less than the char limit
            {
                PacketSender.AddFriend(_searchTextbox.Text);
            }
        }

        void friends_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var row = (ListBoxRow) sender;

            //Only pm online players
            foreach (var friend in Globals.Me.Friends)
            {
                if (friend.name.ToLower() == friend.name.ToLower())
                {
                    if (friend.online == true)
                    {
                        Gui.GameUI.SetChatboxText("/pm " + (string) row.UserData + " ");
                    }
                }
            }
        }

        void friends_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            var row = (ListBoxRow) sender;
            TempName = (string) row.UserData;

            InputBox iBox = new InputBox(Strings.Get("friends", "removefriend"),
                Strings.Get("friends", "removefriendprompt", TempName),
                true, InputBox.InputType.YesNo, RemoveFriend, null, 0);
        }

        private void RemoveFriend(Object sender, EventArgs e)
        {
            PacketSender.RemoveFriend(TempName);
        }
    }
}