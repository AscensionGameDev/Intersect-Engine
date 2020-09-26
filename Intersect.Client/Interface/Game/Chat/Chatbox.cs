using System;
using System.Collections.Generic;
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

        /// <summary>
        /// A dictionary of all chat tab buttons based on the <see cref="ChatboxTabs"/> enum.
        /// </summary>
        private Dictionary<ChatboxTabs, Button> mTabButtons = new Dictionary<ChatboxTabs, Button>();

        //Window Controls
        private ImagePanel mChatboxWindow;

        private GameInterface mGameUi;

        private long mLastChatTime = -1;

        private int mMessageIndex;

        private bool mReceivedMessage;

        /// <summary>
        /// Defines which chat tab we are currently looking at.
        /// </summary>
        private ChatboxTabs mCurrentTab = ChatboxTabs.All;

        /// <summary>
        /// The last tab that was looked at before switching around, if a switch was made at all.
        /// </summary>
        private ChatboxTabs mLastTab = ChatboxTabs.All;

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

            // Generate tab butons.
            for (var btn = 0; btn < (int)ChatboxTabs.Count; btn++)
            {
                mTabButtons.Add((ChatboxTabs)btn, new Button(mChatboxWindow, $"{(ChatboxTabs)btn}TabButton"));
                mTabButtons[(ChatboxTabs)btn].Text = Strings.Chatbox.ChatTabButtons[(ChatboxTabs)btn];
                mTabButtons[(ChatboxTabs)btn].Clicked += TabButtonClicked;
                mTabButtons[(ChatboxTabs)btn].UserData = (ChatboxTabs)btn;
            }

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

            // Disable this to start, since this is the default tab we open the client on.
            mTabButtons[ChatboxTabs.All].Disable();
        }

        /// <summary>
        /// Enables all the chat tab buttons.
        /// </summary>
        private void EnableChatTabs()
        {
            for (var btn = 0; btn < (int)ChatboxTabs.Count; btn++)
            {
                mTabButtons[(ChatboxTabs)btn].Enable();
            }

        }

        /// <summary>
        /// Handles the click event of a chat tab button.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="arguments">The arguments passed by the event.</param>
        private void TabButtonClicked(Base sender, ClickedEventArgs arguments)
        {
            // Enable all buttons again!
            EnableChatTabs();

            // Disable the clicked button to fake our tab being selected and set our selected chat tab.
            sender.Disable();
            mCurrentTab = (ChatboxTabs)sender.UserData;
        }

        //Update
        public void Update()
        {
            // Did the tab change recently? If so, we need to reset a few things to make it work...
            if (mLastTab != mCurrentTab)
            {
                mChatboxMessages.Clear();
                mMessageIndex = 0;
                mReceivedMessage = true;

                mLastTab = mCurrentTab;
            }

            if (mReceivedMessage)
            {
                mChatboxMessages.ScrollToBottom();
                mReceivedMessage = false;
            }

            var msgs = ChatboxMsg.GetMessages(mCurrentTab);
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
