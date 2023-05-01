using System;
using System.Collections.Generic;
using System.Linq;
using Intersect.Client.Core;
using Intersect.Client.Core.Controls;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Configuration;
using Intersect.Enums;
using Intersect.Localization;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game.Chat
{

    public partial class Chatbox
    {

        private ComboBox mChannelCombobox;

        private Label mChannelLabel;

        private ImagePanel mChatbar;

        private TextBox mChatboxInput;

        private ListBox mChatboxMessages;

        private ScrollBar mChatboxScrollBar;

        private Button mChatboxSendButton;

        private Button _chatboxToggleLogButton;

        private Button _chatboxClearLogButton;

        private readonly GameTexture _chatboxTexture;

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

        // Context menu
        private Framework.Gwen.Control.Menu mContextMenu;

        private MenuItem mPMContextItem;

        private MenuItem mFriendInviteContextItem;

        private MenuItem mPartyInviteContextItem;

        private MenuItem mGuildInviteContextItem;

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
            mChatboxInput.SubmitPressed += ChatboxInput_SubmitPressed;
            mChatboxInput.Text = GetDefaultInputText();
            mChatboxInput.Clicked += ChatboxInput_Clicked;
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
            mChatboxSendButton.Clicked += ChatboxSendBtn_Clicked;

            _chatboxToggleLogButton = new Button(mChatboxWindow, "ChatboxToggleLogButton");
            _chatboxToggleLogButton.SetToolTipText(Strings.Chatbox.ToggleLogButtonToolTip);
            _chatboxToggleLogButton.Clicked += ChatboxToggleLogBtn_Clicked;

            _chatboxClearLogButton = new Button(mChatboxWindow, "ChatboxClearLogButton");
            _chatboxClearLogButton.SetToolTipText(Strings.Chatbox.ClearLogButtonToolTip);
            _chatboxClearLogButton.Clicked += ChatboxClearLogBtn_Clicked;

            mChatboxWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

            _chatboxTexture = mChatboxWindow.Texture; // store chatbox texture here so we can re-use it later.
            mChatboxText.IsHidden = true;

            // Disable this to start, since this is the default tab we open the client on.
            mTabButtons[ChatboxTab.All].Disable();

            // Platform check, are we capable of copy/pasting on this machine?
            if (GameClipboard.Instance == null || !GameClipboard.Instance.IsEnabled)
            {
                ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Chatbox.UnableToCopy, CustomColors.Alerts.Error, ChatMessageType.Error));
            }

            // Generate our context menu with basic options.
            mContextMenu = new Framework.Gwen.Control.Menu(gameCanvas, "ChatContextMenu");
            mContextMenu.IsHidden = true;
            mContextMenu.IconMarginDisabled = true;
            //TODO: Is this a memory leak?
            mContextMenu.Children.Clear();
            mPMContextItem = mContextMenu.AddItem(Strings.ChatContextMenu.PM);
            mPMContextItem.Clicked += MPMContextItem_Clicked;
            mFriendInviteContextItem = mContextMenu.AddItem(Strings.ChatContextMenu.FriendInvite);
            mFriendInviteContextItem.Clicked += MFriendInviteContextItem_Clicked;
            mPartyInviteContextItem = mContextMenu.AddItem(Strings.ChatContextMenu.PartyInvite);
            mPartyInviteContextItem.Clicked += MPartyInviteContextItem_Clicked;
            mGuildInviteContextItem = mContextMenu.AddItem(Strings.ChatContextMenu.GuildInvite);
            mGuildInviteContextItem.Clicked += MGuildInviteContextItem_Clicked;
            mContextMenu.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        }

        public void OpenContextMenu(string name)
        {
            // Clear out the old options.
            mContextMenu.RemoveChild(mPMContextItem, false);
            mContextMenu.RemoveChild(mFriendInviteContextItem, false);
            mContextMenu.RemoveChild(mPartyInviteContextItem, false);
            mContextMenu.RemoveChild(mGuildInviteContextItem, false);
            mContextMenu.Children.Clear();

            // No point showing a menu for blank space.
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            // Add our PM option!
            mContextMenu.AddChild(mPMContextItem);
            mPMContextItem.SetText(Strings.ChatContextMenu.PM.ToString(name));

            // If we do not already have this player on our friendlist, add a request button!
            if (!Globals.Me.Friends.Any(f => f.Name == name))
            {
                mContextMenu.AddChild(mFriendInviteContextItem);
                mFriendInviteContextItem.SetText(Strings.ChatContextMenu.FriendInvite.ToString(name));
            }

            // Are we in a party, the leader and is this player not yet in our party?
            if ((Globals.Me.IsInParty() && Globals.Me.Party[0].Id == Globals.Me.Id && !Globals.Me.Party.Any(p => p.Name == name)) || !Globals.Me.IsInParty())
            {
                mContextMenu.AddChild(mPartyInviteContextItem);
                mPartyInviteContextItem.SetText(Strings.ChatContextMenu.PartyInvite.ToString(name));
            }

            // Are we in a guild, able to invite players and are they not yet in our guild?
            if (Globals.Me.IsInGuild && Globals.Me.GuildRank.Permissions.Invite && !Globals.Me.GuildMembers.Any(g => g.Name == name))
            {
                mContextMenu.AddChild(mGuildInviteContextItem);
                mGuildInviteContextItem.SetText(Strings.ChatContextMenu.GuildInvite.ToString(name));
            }

            // Set our spell slot as userdata for future reference.
            mContextMenu.UserData = name;

            mContextMenu.SizeToChildren();
            mContextMenu.Open(Framework.Gwen.Pos.None);
        }

        private void MGuildInviteContextItem_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var name = (string) sender.Parent.UserData;
            PacketSender.SendInviteGuild(name);
        }

        private void MPartyInviteContextItem_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var name = (string)sender.Parent.UserData;
            PacketSender.SendPartyInvite(name);
        }

        private void MFriendInviteContextItem_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var name = (string)sender.Parent.UserData;
            PacketSender.SendAddFriend(name);
        }

        private void MPMContextItem_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var name = (string)sender.Parent.UserData;
            SetChatboxText($"/pm {name} ");
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
                if (ClientConfiguration.Instance.EnableContextMenus)
                {
                    OpenContextMenu(target);
                } 
                else
                {
                    SetChatboxText($"/pm {target} ");
                }

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
                if (Globals.Database.AutoToggleChatLog)
                {
                    ShowChatLog();
                }
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
                if (Globals.Database.AutoToggleChatLog)
                {
                    HideChatLog();
                }
            }
        }

        //Input Handlers
        //Chatbox Window
        void ChatboxInput_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (mChatboxInput.Text == GetDefaultInputText())
            {
                mChatboxInput.Text = "";
            }
        }

        void ChatboxInput_SubmitPressed(Base sender, EventArgs arguments)
        {
            TrySendMessage();
        }

        void ChatboxSendBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            TrySendMessage();
        }

        void ChatboxClearLogBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            ChatboxMsg.ClearMessages();
            mChatboxMessages.Clear();
            mChatboxMessages.GetHorizontalScrollBar().SetScrollAmount(0);
            mMessageIndex = 0;
            mReceivedMessage = true;
            mLastTab = mCurrentTab;
        }

        void ChatboxToggleLogBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (mChatboxWindow.Texture != null)
            {
                HideChatLog();
            }
            else
            {
                ShowChatLog();
            }
        }

        private void ShowChatLog()
        {
            mTabButtons[ChatboxTab.All].Show();
            mTabButtons[ChatboxTab.Local].Show();
            mTabButtons[ChatboxTab.Party].Show();
            mTabButtons[ChatboxTab.Guild].Show();
            mTabButtons[ChatboxTab.Global].Show();
            mTabButtons[ChatboxTab.System].Show();
            mChatboxMessages.Show();
            mChatboxWindow.Texture = _chatboxTexture;
        }

        private void HideChatLog()
        {
            mTabButtons[ChatboxTab.All].Hide();
            mTabButtons[ChatboxTab.Local].Hide();
            mTabButtons[ChatboxTab.Party].Hide();
            mTabButtons[ChatboxTab.Guild].Hide();
            mTabButtons[ChatboxTab.Global].Hide();
            mTabButtons[ChatboxTab.System].Hide();
            mChatboxMessages.Hide();
            mChatboxWindow.Texture = null;
        }

        void TrySendMessage()
        {
            var msg = mChatboxInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(msg) || string.Equals(msg, GetDefaultInputText(), StringComparison.Ordinal))
            {
                mChatboxInput.Text = GetDefaultInputText();

                return;
            }
            
            if (mLastChatTime > Timing.Global.MillisecondsUtc)
            {
                ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Chatbox.toofast, Color.Red, ChatMessageType.Error));
                mLastChatTime = Timing.Global.MillisecondsUtc + Options.MinChatInterval;

                return;
            }

            mLastChatTime = Timing.Global.MillisecondsUtc + Options.MinChatInterval;

            PacketSender.SendChatMsg(
                msg, byte.Parse(mChannelCombobox.SelectedItem.UserData.ToString())
            );

            mChatboxInput.Text = GetDefaultInputText();
        }

        string GetDefaultInputText()
        {
            var key1 = Controls.ActiveControls.ControlMapping[Control.Enter].Bindings[0];
            var key2 = Controls.ActiveControls.ControlMapping[Control.Enter].Bindings[1];
            if (key1.Key == Keys.None && key2.Key != Keys.None)
            {
                return Strings.Chatbox.enterchat1.ToString(
                    Strings.Keys.keydict[Enum.GetName(typeof(Keys), key2.Key).ToLower()]
                );
            }

            if (key1.Key != Keys.None && key2.Key == Keys.None)
            {
                return Strings.Chatbox.enterchat1.ToString(
                    Strings.Keys.keydict[Enum.GetName(typeof(Keys), key1.Key).ToLower()]
                );
            }

            if (key1.Key != Keys.None && key2.Key != Keys.None)
            {
                return Strings.Chatbox.enterchat2.ToString(
                    Strings.Keys.keydict[Enum.GetName(typeof(Keys), key1.Key).ToLower()],
                    Strings.Keys.keydict[Enum.GetName(typeof(Keys), key2.Key).ToLower()]
                );
            }

            return Strings.Chatbox.enterchat;
        }

    }

}
