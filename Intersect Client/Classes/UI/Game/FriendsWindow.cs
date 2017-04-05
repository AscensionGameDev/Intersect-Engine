using System;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Color = IntersectClientExtras.GenericClasses.Color;

namespace Intersect_Client.Classes.UI.Game
{
    class FriendsWindow
    {
        private ListBox _friends;
        private Button _addButton;
        private TextBox _searchTextbox;

        //Controls
        private WindowControl _friendsWindow;

        //Temp variables
        private string TempName;

        //Init
        public FriendsWindow(Canvas _gameCanvas)
        {
            _friendsWindow = new WindowControl(_gameCanvas, Strings.Get("friends", "title"));
            _friendsWindow.SetSize(228, 320);
            _friendsWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() - 210, GameGraphics.Renderer.GetScreenHeight() - 500);
            _friendsWindow.DisableResizing();
            _friendsWindow.Margin = Margin.Zero;
            _friendsWindow.Padding = new Padding(8, 5, 9, 11);
            _friendsWindow.IsHidden = true;

            _friendsWindow.SetTitleBarHeight(24);
            _friendsWindow.SetCloseButtonSize(20, 20);
            _friendsWindow.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "friendsactive.png"), WindowControl.ControlState.Active);
            _friendsWindow.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closenormal.png"), Button.ControlState.Normal);
            _friendsWindow.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closehover.png"), Button.ControlState.Hovered);
            _friendsWindow.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closeclicked.png"), Button.ControlState.Clicked);
            _friendsWindow.SetFont(Globals.ContentManager.GetFont(Gui.DefaultFont, 14));
            _friendsWindow.SetTextColor(new Color(255, 220, 220, 220), WindowControl.ControlState.Active);

            _searchTextbox = new TextBox(_friendsWindow);
            _searchTextbox.SetBounds(6, 6, _friendsWindow.Width - 42, 18);
            Gui.FocusElements.Add(_searchTextbox);

            _addButton = new Button(_friendsWindow);
            _addButton.SetSize(18, 18);
            _addButton.SetText("+");
            _addButton.SetPosition(_friendsWindow.Width - 34, 6);
            _addButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonnormal.png"), Button.ControlState.Normal);
            _addButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonhover.png"), Button.ControlState.Hovered);
            _addButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonclicked.png"), Button.ControlState.Clicked);
            _addButton.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _addButton.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _addButton.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
            _addButton.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 12);
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
            var row = (ListBoxRow)sender;
            
            //Only pm online players
            foreach (var friend in Globals.Me.Friends)
            {
                if (friend.name.ToLower() == friend.name.ToLower())
                {
                    if (friend.online == true)
                    {
                        Gui.GameUI.SetChatboxText("/pm " + (string)row.UserData + " ");
                    }
                }
            }
        }

        void friends_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            var row = (ListBoxRow)sender;
            TempName = (string)row.UserData;

            InputBox iBox = new InputBox(Strings.Get("friends", "removefriend"),
                        Strings.Get("friends", "removefriendprompt", TempName),
                        true, RemoveFriend, null, 0, false);
        }

        private void RemoveFriend(Object sender, EventArgs e)
        {
            PacketSender.RemoveFriend(TempName);
        }
    }
}
