using System;

using Intersect.Client.Core;
using Intersect.Client.Networking;
using Intersect.Client.Localization;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Interface.Game
{
    public partial class FriendsRow
    {
        private Base mParent;

        private ImagePanel mRowContainer;

        private ImagePanel mOnlineSymbol;

        private ImagePanel mOfflineSymbol;

        private Label mName;

        private Label mStatus;

        private Button mTell;

        private Button mRemove;

        private string mMyName;

        private string mMyStatus;

        private bool mMyOnline;

        public FriendsRow(Base parent, string name, string status, bool online)
        {
            // Set up our basic values.
            mParent = parent;
            mMyName = name;
            mMyStatus = status;
            mMyOnline = online;

            // Generate our layout controls and load the layout from our json files.
            GenerateControls();
            mRowContainer.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

            // Update our display.
            UpdateControls();
        }

        private void GenerateControls()
        {
            // Generate our controls.
            mRowContainer = new ImagePanel(mParent, "FriendRow");
            mOnlineSymbol = new ImagePanel(mRowContainer, "OnlineSymbol");
            mOfflineSymbol = new ImagePanel(mRowContainer, "OfflineSymbol");
            mName = new Label(mRowContainer, "Name");
            mStatus = new Label(mRowContainer, "Status");
            mTell = new Button(mRowContainer, "Tell");
            mRemove = new Button(mRowContainer, "Remove");

            // Set up events.
            mTell.Clicked += MTell_Clicked;
            mRemove.Clicked += MRemove_Clicked;
        }

        private void UpdateControls()
        {
            // If this friend is online or offline, show the correct dot and if they're offline hide the tell button.
            if (mMyOnline)
            {
                mOnlineSymbol.Show();
                mOfflineSymbol.Hide();
                mTell.Show();
            }
            else
            {
                mOnlineSymbol.Hide();
                mOfflineSymbol.Show();
                mTell.Hide();
            }

            // Update our fields.
            mName.SetText(mMyName);
            mStatus.SetText(mMyOnline ? mMyStatus : Strings.Friends.Offline.ToString());

        }

        private void MRemove_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var iBox = new InputBox(
                Strings.Friends.RemoveFriend, Strings.Friends.RemoveFriendPrompt.ToString(mMyName), true,
                InputBox.InputType.YesNo, RemoveFriend, null, 0
            );
        }

        private void RemoveFriend(Object sender, EventArgs e)
        {
            PacketSender.SendRemoveFriend(mMyName);
        }

        private void MTell_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Interface.GameUi.SetChatboxText($"/pm {mMyName} ");
        }

        public Rectangle Bounds => mRowContainer.Bounds;

        public void SetPosition(float x, float y) => mRowContainer.SetPosition(x, y);

        public void Dispose()
        {
            mParent.RemoveChild(mRowContainer, true);
        }

    }
}
