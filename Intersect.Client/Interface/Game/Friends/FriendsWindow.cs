using System;
using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game
{

    partial class FriendsWindow
    {
        private Base mParent;

        private WindowControl mWindow;

        private Button mAdd;

        private ScrollControl mFriendContainer;

        private ImagePanel mFriendListAnchor;

        private List<FriendsRow> mRows;

        public FriendsWindow(Canvas gameCanvas)
        {
            // Set up our basic values.
            mParent = gameCanvas;

            // Generate our layout controls and load the layout from our json files.
            GenerateControls();
            mWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

            // Update our display.
            UpdateList();
        }

        private void GenerateControls()
        {
            mWindow = new WindowControl(mParent, Strings.Friends.Title, false, "FriendsWindow");
            mAdd = new Button(mWindow, "AddFriendButton");
            mFriendContainer = new ScrollControl(mWindow, "FriendContainer");
            mFriendListAnchor = new ImagePanel(mFriendContainer, "FriendListAnchor");

            mWindow.DisableResizing();
            mFriendContainer.EnableScroll(false, true);

            mAdd.Clicked += addButton_Clicked;
        }

        public bool IsVisible => !mWindow.IsHidden;

        public void Show() => mWindow.Show();

        public void Hide() => mWindow.Hide();

        public void Update()
        {
            if (!IsVisible)
            {
                ClearList();
            }
        }

        private void ClearList()
        {
            // Clear out existing data.
            if (mRows != null && mRows.Count > 0)
            {
                foreach (var control in mRows)
                {
                    control.Dispose();
                }
                mRows.Clear();
            }
            else if (mRows == null)
            {
                mRows = new List<FriendsRow>();
            }
        }

        public void UpdateList()
        {
            ClearList();

            var count = 0;
            foreach (var friend in Globals.Me.Friends)
            {
                var control = new FriendsRow(mFriendContainer, friend.Name, friend.Map, friend.Online);
                control.SetPosition(mFriendListAnchor.Bounds.X, mFriendListAnchor.Bounds.Y + (count * control.Bounds.Height));

                mRows.Add(control);
                count++;
            }
        }

        void addButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var iBox = new InputBox(
                Strings.Friends.AddFriend, Strings.Friends.AddFriendPrompt, true, InputBox.InputType.TextInput,
                AddFriend, null, 0
            );
        }


        private void AddFriend(Object sender, EventArgs e)
        {
            var ibox = (InputBox) sender;
            if (ibox.TextValue.Trim().Length >= 3) //Don't bother sending a packet less than the char limit
            {
                if (Globals.Me.CombatTimer < Timing.Global.Milliseconds)
                {
                    PacketSender.SendAddFriend(ibox.TextValue);
                }
                else
                {
                    PacketSender.SendChatMsg(Strings.Friends.InFight.ToString(), 4);
                }
            }
        }

    }

}
