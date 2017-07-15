using System;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Gwen.ControlInternal;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI.Game.Chat;
using Intersect.Client.Classes.Core;

namespace Intersect_Client.Classes.UI.Game
{
    public class Chatbox
    {
        private TextBox _chatboxInput;
		private ComboBox _channelCombobox;
		private ListBox _chatboxMessages;
        private ScrollBar _chatboxScrollBar;
        private Button _chatboxSendButton;
        //Window Controls
        private ImagePanel _chatboxWindow;
        private GameGuiBase _gameUi;
        private int _messageIndex;
        private bool _receivedMessage;

        //Init
        public Chatbox(Canvas _gameCanvas, GameGuiBase gameUi)
        {
            _gameUi = gameUi;

            //Chatbox Window
            _chatboxWindow = new ImagePanel(_gameCanvas,"ChatboxWindow");
            _chatboxMessages = new ListBox(_chatboxWindow,"MessageList");
            _chatboxMessages.EnableScroll(false, true);

			_chatboxInput = new TextBox(_chatboxWindow,"ChatboxInputField");
            _chatboxInput.SubmitPressed += ChatBoxInput_SubmitPressed;
            _chatboxInput.Text = GetDefaultInputText();
            _chatboxInput.Clicked += ChatBoxInput_Clicked;
            Gui.FocusElements.Add(_chatboxInput);

            _channelCombobox = new ComboBox(_chatboxWindow,"ChatChannelCombobox");
            for (int i = 1; i < 4; i++)
            {
                var menuItem = _channelCombobox.AddItem(Strings.Get("chatbox", "channel" + i));
                menuItem.UserData = i - 1;
            }
            //Add admin channel only if power > 0.
            if (Globals.Me.type > 0)
            {
                var menuItem = _channelCombobox.AddItem(Strings.Get("chatbox", "channeladmin"));
                menuItem.UserData = 3;
            }

            _chatboxSendButton = new Button(_chatboxWindow,"ChatboxSendButton");
            _chatboxSendButton.Text = Strings.Get("chatbox", "send");
            _chatboxSendButton.Clicked += ChatBoxSendBtn_Clicked;
        }

        //Update
        public void Update()
        {
            if (_receivedMessage)
            {
                _chatboxMessages.ScrollToBottom();
                _receivedMessage = false;
            }

            var msgs = ChatboxMsg.GetMessages();
            for (var i = _messageIndex; i < msgs.Count; i++)
            {
                var msg = msgs[i];
                var myText = Gui.WrapText(msg.GetMessage(), 340, _chatboxWindow.Parent.Skin.DefaultFont);
                foreach (var t in myText)
                {
                    var rw = _chatboxMessages.AddRow(t.Trim());
                    rw.SetTextColor(msg.GetColor());
                    rw.ShouldDrawBackground = false;
                    rw.UserData = msg.GetTarget();
                    rw.Clicked += ChatboxRow_Clicked;
                    _receivedMessage = true;

                    while (_chatboxMessages.RowCount > 20)
                    {
                        _chatboxMessages.RemoveRow(0);
                    }
                }
                _messageIndex++;
            }
        }

        public void SetChatboxText(string msg)
        {
            _chatboxInput.Text = msg;
            _chatboxInput.Focus();
        }

        private void ChatboxRow_Clicked(Base sender, ClickedEventArgs arguments)
        {
            ListBoxRow rw = (ListBoxRow) sender;
            string target = (string) rw.UserData;
            if (target != "")
            {
                if (_gameUi.AdminWindowOpen())
                {
                    _gameUi.AdminWindowSelectName(target);
                }
            }
        }

        //Extra Methods
        public void Focus()
        {
            if (!_chatboxInput.HasFocus)
            {
                _chatboxInput.Focus();
                _chatboxInput.Text = "";
            }
        }

        //Input Handlers
        //Chatbox Window
        void ChatBoxInput_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_chatboxInput.Text == GetDefaultInputText())
            {
                _chatboxInput.Text = "";
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
            if (_chatboxInput.Text.Trim().Length <= 0 || _chatboxInput.Text == GetDefaultInputText())
            {
                _chatboxInput.Text = GetDefaultInputText();
                return;
            }

            PacketSender.SendChatMsg(_chatboxInput.Text.Trim(), (int)_channelCombobox.SelectedItem.UserData);
            _chatboxInput.Text = GetDefaultInputText();
        }

        string GetDefaultInputText()
        {
            var key1 = GameControls.ActiveControls.ControlMapping[Controls.Enter].key1;
            var key2 = GameControls.ActiveControls.ControlMapping[Controls.Enter].key2;
            if (key1 == Keys.None && key2 != Keys.None)
            {
                return Strings.Get("chatbox", "enterchat1", Strings.Get("keys",Enum.GetName(typeof(Keys),key2)));
            }
            else if (key1 != Keys.None && key2 == Keys.None)
            {
                return Strings.Get("chatbox", "enterchat1", Strings.Get("keys", Enum.GetName(typeof(Keys), key1)));
            }
            else if (key1 != Keys.None && key2 != Keys.None)
            {
                return Strings.Get("chatbox", "enterchat1", Strings.Get("keys", Enum.GetName(typeof(Keys), key1)), Strings.Get("keys", Enum.GetName(typeof(Keys), key2)));
            }
            return Strings.Get("chatbox", "enterchat");
        }
    }
}