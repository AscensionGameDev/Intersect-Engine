using System;

using Intersect.Client.Core;
using Intersect.Client.Core.Controls;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Configuration;
using Intersect.Enums;

namespace Intersect.Client.Interface.Game.Chat
{

    public class Chatbox
    {

        private ComboBox mChannelCombobox;

        private Label mChannelLabel;

        private ImagePanel mChatbar;

        private TextBox mChatboxInput;

        private ListBox mChatboxMessages;

        private ScrollBar mChatboxScrollBar;

        private Button mChatboxSendButton;

        private Label mChatboxText;

        private Label mChatboxTitle;

        private Button mBtnAllTab;

        private Button mBtnLocalTab;

        private Button mBtnPartyTab;

        private Button mBtnGlobalTab;

        private Button mBtnSystemTab;

        //Window Controls
        private ImagePanel mChatboxWindow;

        private GameInterface mGameUi;

        private long mLastChatTime = -1;

        private int mMessageIndex;

        private bool mReceivedMessage;

        private ChatBoxTabs mLastTab = ChatBoxTabs.All;

        //Init
        public Chatbox(Canvas gameCanvas, GameInterface gameUi)
        {
            mGameUi = gameUi;

            //Chatbox Window
            mChatboxWindow = new ImagePanel(gameCanvas, "ChatboxWindow");
            mChatboxMessages = new ListBox(mChatboxWindow, "MessageList");
            mChatboxMessages.EnableScroll(false, true);
            mChatboxWindow.ShouldCacheToTexture = true;

            mChatboxTitle = new Label(mChatboxWindow, "ChatboxTitle");
            mChatboxTitle.Text = Strings.Chatbox.title;
            mChatboxTitle.IsHidden = true;

            mBtnAllTab = new Button(mChatboxWindow, "AllTabButton");
            mBtnAllTab.Text = Strings.Chatbox.AllButton;
            mBtnAllTab.Clicked += TabButtonClicked;

            mBtnLocalTab = new Button(mChatboxWindow, "LocalTabButton");
            mBtnLocalTab.Text = Strings.Chatbox.LocalButton;
            mBtnLocalTab.Clicked += TabButtonClicked;

            mBtnPartyTab = new Button(mChatboxWindow, "PartyTabButton");
            mBtnPartyTab.Text = Strings.Chatbox.PartyButton;
            mBtnPartyTab.Clicked += TabButtonClicked;

            mBtnGlobalTab = new Button(mChatboxWindow, "GlobalTabButton");
            mBtnGlobalTab.Text = Strings.Chatbox.GlobalButton;
            mBtnGlobalTab.Clicked += TabButtonClicked;

            mBtnSystemTab = new Button(mChatboxWindow, "SystemTabButton");
            mBtnSystemTab.Text = Strings.Chatbox.SystemButton;
            mBtnSystemTab.Clicked += TabButtonClicked;

            mChatbar = new ImagePanel(mChatboxWindow, "Chatbar");
            mChatbar.IsHidden = true;

            mChatboxInput = new TextBox(mChatboxWindow, "ChatboxInputField");
            mChatboxInput.SubmitPressed += ChatBoxInput_SubmitPressed;
            mChatboxInput.Text = GetDefaultInputText();
            mChatboxInput.Clicked += ChatBoxInput_Clicked;
            mChatboxInput.IsTabable = false;
            mChatboxInput.SetMaxLength(Options.MaxChatLength);
            Interface.FocusElements.Add(mChatboxInput);

            mChannelLabel = new Label(mChatboxWindow, "ChannelLabel");
            mChannelLabel.Text = Strings.Chatbox.channel;
            mChannelLabel.IsHidden = true;

            mChannelCombobox = new ComboBox(mChatboxWindow, "ChatChannelCombobox");
            for (var i = 0; i < 3; i++)
            {
                var menuItem = mChannelCombobox.AddItem(Strings.Chatbox.channels[i]);
                menuItem.UserData = i;
            }

            //Add admin channel only if power > 0.
            if (Globals.Me.Type > 0)
            {
                var menuItem = mChannelCombobox.AddItem(Strings.Chatbox.channeladmin);
                menuItem.UserData = 3;
            }

            mChatboxText = new Label(mChatboxWindow);
            mChatboxText.Name = "ChatboxText";
            mChatboxText.Font = mChatboxWindow.Parent.Skin.DefaultFont;

            mChatboxSendButton = new Button(mChatboxWindow, "ChatboxSendButton");
            mChatboxSendButton.Text = Strings.Chatbox.send;
            mChatboxSendButton.Clicked += ChatBoxSendBtn_Clicked;

            mChatboxWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

            mChatboxText.IsHidden = true;
        }

        private void TabButtonClicked(Base sender, ClickedEventArgs arguments)
        {
            switch (sender.Name)
            {
                case "AllTabButton":
                    Globals.CurrentChatTab = ChatBoxTabs.All;
                    break;

                case "LocalTabButton":
                    Globals.CurrentChatTab = ChatBoxTabs.Local;
                    break;

                case "PartyTabButton":
                    Globals.CurrentChatTab = ChatBoxTabs.Party;
                    break;

                case "GlobalTabButton":
                    Globals.CurrentChatTab = ChatBoxTabs.Global;
                    break;

                case "SystemTabButton":
                    Globals.CurrentChatTab = ChatBoxTabs.System;
                    break;
            }
        }

        //Update
        public void Update()
        {
            // Did the tab change recently? If so, we need to reset a few things to make it work...
            if (mLastTab != Globals.CurrentChatTab)
            {
                mChatboxMessages.Clear();
                mMessageIndex = 0;
                mReceivedMessage = true;

                mLastTab = Globals.CurrentChatTab;
            }

            if (mReceivedMessage)
            {
                mChatboxMessages.ScrollToBottom();
                mReceivedMessage = false;
            }

            var msgs = ChatboxMsg.GetMessages(Globals.CurrentChatTab);
            for (var i = mMessageIndex; i < msgs.Count; i++)
            {
                var msg = msgs[i];
                var myText = Interface.WrapText(
                    msg.Message, mChatboxMessages.Width - mChatboxMessages.GetVerticalScrollBar().Width - 8,
                    mChatboxText.Font
                );

                foreach (var t in myText)
                {
                    var rw = mChatboxMessages.AddRow(t.Trim());
                    rw.SetTextFont(mChatboxText.Font);
                    rw.SetTextColor(msg.Color);
                    rw.ShouldDrawBackground = false;
                    rw.UserData = msg.Target;
                    rw.Clicked += ChatboxRow_Clicked;
                    mReceivedMessage = true;

                    while (mChatboxMessages.RowCount > ClientConfiguration.Instance.ChatLines)
                    {
                        mChatboxMessages.RemoveRow(0);
                    }
                }

                mMessageIndex++;
            }
        }

        public void SetChatboxText(string msg)
        {
            mChatboxInput.Text = msg;
            mChatboxInput.Focus();
        }

        private void ChatboxRow_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var rw = (ListBoxRow) sender;
            var target = (string) rw.UserData;
            if (target != "")
            {
                if (mGameUi.AdminWindowOpen())
                {
                    mGameUi.AdminWindowSelectName(target);
                }
            }
        }

        //Extra Methods
        public void Focus()
        {
            if (!mChatboxInput.HasFocus)
            {
                mChatboxInput.Focus();
                mChatboxInput.Text = "";
            }
        }

        //Input Handlers
        //Chatbox Window
        void ChatBoxInput_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (mChatboxInput.Text == GetDefaultInputText())
            {
                mChatboxInput.Text = "";
            }
        }

        void ChatBoxInput_SubmitPressed(Base sender, EventArgs arguments)
        {
            TrySendMessage();
        }

        void ChatBoxSendBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            TrySendMessage();
        }

        void TrySendMessage()
        {
            if (mLastChatTime > Globals.System.GetTimeMs())
            {
                ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Chatbox.toofast, Color.Red, ChatMessageType.Error));
                mLastChatTime = Globals.System.GetTimeMs() + Options.MinChatInterval;

                return;
            }

            mLastChatTime = Globals.System.GetTimeMs() + Options.MinChatInterval;

            if (mChatboxInput.Text.Trim().Length <= 0 || mChatboxInput.Text == GetDefaultInputText())
            {
                mChatboxInput.Text = GetDefaultInputText();

                return;
            }

            PacketSender.SendChatMsg(
                mChatboxInput.Text.Trim(), byte.Parse(mChannelCombobox.SelectedItem.UserData.ToString())
            );

            mChatboxInput.Text = GetDefaultInputText();
        }

        string GetDefaultInputText()
        {
            var key1 = Controls.ActiveControls.ControlMapping[Control.Enter].Key1;
            var key2 = Controls.ActiveControls.ControlMapping[Control.Enter].Key2;
            if (key1 == Keys.None && key2 != Keys.None)
            {
                return Strings.Chatbox.enterchat1.ToString(
                    Strings.Keys.keydict[Enum.GetName(typeof(Keys), key2).ToLower()]
                );
            }
            else if (key1 != Keys.None && key2 == Keys.None)
            {
                return Strings.Chatbox.enterchat1.ToString(
                    Strings.Keys.keydict[Enum.GetName(typeof(Keys), key1).ToLower()]
                );
            }
            else if (key1 != Keys.None && key2 != Keys.None)
            {
                return Strings.Chatbox.enterchat1.ToString(
                    Strings.Keys.keydict[Enum.GetName(typeof(Keys), key1).ToLower()],
                    Strings.Keys.keydict[Enum.GetName(typeof(Keys), key2).ToLower()]
                );
            }

            return Strings.Chatbox.enterchat;
        }

    }

}
