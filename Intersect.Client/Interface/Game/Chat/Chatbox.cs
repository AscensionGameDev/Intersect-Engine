using System;
using System.Collections.Generic;
using System.Linq;
using Intersect.Client.Core;
using Intersect.Client.Core.Controls;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Configuration;
using Intersect.Enums;
using Intersect.Localization;

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
        /// A dictionary of all chat tab buttons based on the <see cref="ChatboxTab"/> enum.
        /// </summary>
        private Dictionary<ChatboxTab, Button> mTabButtons = new Dictionary<ChatboxTab, Button>();

        //Window Controls
        private ImagePanel mChatboxWindow;

        private GameInterface mGameUi;

        private long mLastChatTime = -1;

        private int mMessageIndex;

        private bool mReceivedMessage;

        /// <summary>
        /// Defines which chat tab we are currently looking at.
        /// </summary>
        private ChatboxTab mCurrentTab = ChatboxTab.All;

        /// <summary>
        /// The last tab that was looked at before switching around, if a switch was made at all.
        /// </summary>
        private ChatboxTab mLastTab = ChatboxTab.All;

        /// <summary>
        /// Keep track of what chat channel we were chatting in on certain tabs so we can remember this when switching back to them.
        /// </summary>
        private Dictionary<ChatboxTab, int> mLastChatChannel = new Dictionary<ChatboxTab, int>() {
            { ChatboxTab.All, 0 },
            { ChatboxTab.System, 0 },
        };

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
            for (var btn = 0; btn < (int)ChatboxTab.Count; btn++)
            {
                mTabButtons.Add((ChatboxTab)btn, new Button(mChatboxWindow, $"{(ChatboxTab)btn}TabButton"));
                // Do we have a localized string for this chat tab? If not assign none as the text.
                LocalizedString name;
                mTabButtons[(ChatboxTab)btn].Text = Strings.Chatbox.ChatTabButtons.TryGetValue((ChatboxTab)btn, out name) ? name : Strings.General.none;
                mTabButtons[(ChatboxTab)btn].Clicked += TabButtonClicked;
                // We'll be using the user data to determine which tab we've clicked later.
                mTabButtons[(ChatboxTab)btn].UserData = (ChatboxTab)btn;
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
            for (var i = 0; i < 4; i++)
            {
                var menuItem = mChannelCombobox.AddItem(Strings.Chatbox.channels[i]);
                menuItem.UserData = i;
                menuItem.Selected += MenuItem_Selected;
            }

            //Add admin channel only if power > 0.
            if (Globals.Me.Type > 0)
            {
                var menuItem = mChannelCombobox.AddItem(Strings.Chatbox.channeladmin);
                menuItem.UserData = 4;
                menuItem.Selected += MenuItem_Selected;
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
            mTabButtons[ChatboxTab.All].Disable();

            // Platform check, are we capable of copy/pasting on this machine?
            if (GameClipboard.Instance == null || !GameClipboard.Instance.CanCopyPaste())
            {
                ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Chatbox.UnableToCopy, CustomColors.Alerts.Error, ChatMessageType.Error));
            }
        }

        /// <summary>
        /// Handle logic after a chat channel menu item is selected.
        /// </summary>
        private void MenuItem_Selected(Base sender, ItemSelectedEventArgs arguments)
        {
            // If we're on the two generic tabs, remember which channel we're trying to type in so we can switch back to this channel when we decide to swap between tabs.
            if ((mCurrentTab == ChatboxTab.All || mCurrentTab == ChatboxTab.System))
            {
                mLastChatChannel[mCurrentTab] = (int)sender.UserData;
            }
        }

        /// <summary>
        /// Enables all the chat tab buttons.
        /// </summary>
        private void EnableChatTabs()
        {
            for (var btn = 0; btn < (int)ChatboxTab.Count; btn++)
            {
                mTabButtons[(ChatboxTab)btn].Enable();
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
            var tab = (ChatboxTab)sender.UserData;
            mCurrentTab = tab;

            // Change the default channel we're trying to chat in based on the tab we've just selected.
            SetChannelToTab(tab);
        }

        /// <summary>
        /// Sets the selected chat channel to type in by default to the channel corresponding to the provided tab.
        /// </summary>
        /// <param name="tab">The tab to use for reference as to which channel we want to speak in.</param>
        private void SetChannelToTab(ChatboxTab tab)
        {
            switch (tab)
            {
                case ChatboxTab.System:
                case ChatboxTab.All:
                        mChannelCombobox.SelectByUserData(mLastChatChannel[tab]);
                    break;

                case ChatboxTab.Local:
                    mChannelCombobox.SelectByUserData(0);
                    break;

                case ChatboxTab.Global:
                    mChannelCombobox.SelectByUserData(1);
                    break;

                case ChatboxTab.Party:
                    mChannelCombobox.SelectByUserData(2);
                    break;

                case ChatboxTab.Guild:
                    mChannelCombobox.SelectByUserData(3);
                    break;

                default:
                    // remain unchanged.
                    break;
            }
        }

        //Update
        public void Update()
        {
            var vScrollBar = mChatboxMessages.GetVerticalScrollBar();
            var scrollAmount = vScrollBar.ScrollAmount;
            var scrollBarVisible = vScrollBar.ContentSize > mChatboxMessages.Height;
            var scrollToBottom = vScrollBar.ScrollAmount == 1 || !scrollBarVisible;

            // Did the tab change recently? If so, we need to reset a few things to make it work...
            if (mLastTab != mCurrentTab)
            {
                mChatboxMessages.Clear();
                mChatboxMessages.GetHorizontalScrollBar().SetScrollAmount(0);
                mMessageIndex = 0;
                mReceivedMessage = true;

                mLastTab = mCurrentTab;
            }

            var msgs = ChatboxMsg.GetMessages(mCurrentTab);
            for (var i = mMessageIndex; i < msgs.Count; i++)
            {
                var msg = msgs[i];
                var myText = Interface.WrapText(
                    msg.Message, mChatboxMessages.Width - vScrollBar.Width - 8,
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
                    rw.RightClicked += ChatboxRow_RightClicked;
                    mReceivedMessage = true;

                    while (mChatboxMessages.RowCount > ClientConfiguration.Instance.ChatLines)
                    {
                        mChatboxMessages.RemoveRow(0);
                    }
                }

                mMessageIndex++;
            }


            if (mReceivedMessage)
            {
                mChatboxMessages.InnerPanel.SizeToChildren(false, true);
                mChatboxMessages.UpdateScrollBars();
                if (!scrollToBottom)
                {
                    vScrollBar.SetScrollAmount(scrollAmount);
                }
                else
                {
                    vScrollBar.SetScrollAmount(1);
                }
                mReceivedMessage = false;
            }
        }

        private void ChatboxRow_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            var rw = (ListBoxRow)sender;
            var target = (string)rw.UserData;
            if (!string.IsNullOrWhiteSpace(target) && target != Globals.Me.Name)
            {
                SetChatboxText($"/pm {target} ");
            }
        }

        public void SetChatboxText(string msg)
        {
            mChatboxInput.Text = msg;
            mChatboxInput.Focus();
            mChatboxInput.CursorEnd = mChatboxInput.Text.Length;
            mChatboxInput.CursorPos = mChatboxInput.Text.Length;
        }

        private void ChatboxRow_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var rw = (ListBoxRow) sender;
            var target = (string) rw.UserData;
            if (!string.IsNullOrWhiteSpace(target))
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
                mChatboxInput.Text = string.Empty;
                mChatboxInput.Focus();
            }
        }

        public bool HasFocus => mChatboxInput.HasFocus;

        public void UnFocus()
        {
            // Just focus something else if we need to unfocus.
            if (mChatboxInput.HasFocus)
            {
                mChatboxInput.Text = GetDefaultInputText();
                mChatboxMessages.Focus();
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
