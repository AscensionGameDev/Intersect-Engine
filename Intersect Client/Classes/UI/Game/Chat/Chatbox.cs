/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using System;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Gwen.ControlInternal;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI.Game.Chat;
using Intersect_Library.Localization;

namespace Intersect_Client.Classes.UI.Game
{
    public class Chatbox
    {
        //Window Controls
        private ImagePanel _chatboxWindow;
        private ListBox _chatboxMessages;
        private TextBox _chatboxInput;
        private Button _chatboxSendButton;
        private ScrollBar _chatboxScrollBar;
        private bool _receivedMessage = false;
        private int _messageIndex = 0;
        private GameGuiBase _gameUi;

        //Init
        public Chatbox(Canvas _gameCanvas, GameGuiBase gameUi)
        {
            _gameUi = gameUi;

            //Chatbox Window
            _chatboxWindow = new ImagePanel(_gameCanvas);
            _chatboxWindow.SetSize(402,173);
            _chatboxWindow.SetPosition(0, _gameCanvas.Height - _chatboxWindow.Height);
            _chatboxWindow.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "chatbox.png");

            _chatboxMessages = new ListBox(_chatboxWindow) { IsDisabled = true };
            _chatboxMessages.SetPosition(19,20);
            _chatboxMessages.SetSize(369,113);
            _chatboxMessages.ShouldDrawBackground = false;
            _chatboxMessages.EnableScroll(false, true);
            _chatboxMessages.AutoHideBars = false;

            _chatboxScrollBar = _chatboxMessages.GetVerticalScrollBar();
            _chatboxScrollBar.RenderColor = new Color(200,40,40,40);
            _chatboxScrollBar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarnormal.png"),Dragger.ControlState.Normal);
            _chatboxScrollBar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarhover.png"), Dragger.ControlState.Hovered);
            _chatboxScrollBar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarclicked.png"), Dragger.ControlState.Clicked);

            var upButton = _chatboxScrollBar.GetScrollBarButton(Pos.Top);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrownormal.png"), Button.ControlState.Normal);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrowclicked.png"), Button.ControlState.Clicked);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrowhover.png"), Button.ControlState.Hovered);
            var downButton = _chatboxScrollBar.GetScrollBarButton(Pos.Bottom);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrownormal.png"), Button.ControlState.Normal);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrowclicked.png"), Button.ControlState.Clicked);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrowhover.png"), Button.ControlState.Hovered);


            _chatboxInput = new TextBox(_chatboxWindow);
            _chatboxInput.SetPosition(18,144);
            _chatboxInput.SetSize(314, 16);
            _chatboxInput.SubmitPressed += ChatBoxInput_SubmitPressed;
            _chatboxInput.Text = Strings.Get("chatbox","enterchat");
            _chatboxInput.Clicked += ChatBoxInput_Clicked;
            _chatboxInput.ShouldDrawBackground = false;
            _chatboxInput.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Normal);
            Gui.FocusElements.Add(_chatboxInput);

            _chatboxSendButton = new Button(_chatboxWindow);
            _chatboxSendButton.SetSize(49,18);
            _chatboxSendButton.SetPosition(_chatboxMessages.Right - _chatboxSendButton.Width,143);
            _chatboxSendButton.Text = Strings.Get("chatbox","send");
            _chatboxSendButton.Clicked += ChatBoxSendBtn_Clicked;
            _chatboxSendButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "sendnormal.png"),Button.ControlState.Normal);
            _chatboxSendButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "sendhover.png"), Button.ControlState.Hovered);
            _chatboxSendButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "sendclicked.png"), Button.ControlState.Clicked);
            _chatboxSendButton.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _chatboxSendButton.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _chatboxSendButton.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
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
            for (int i = _messageIndex; i < msgs.Count; i++)
            {
                var myText = Gui.WrapText(msgs[i].GetMessage(), 340, _chatboxWindow.Parent.Skin.DefaultFont);
                foreach (var t in myText)
                {
                    var rw = _chatboxMessages.AddRow(t.Trim());
                    rw.SetTextColor(msgs[i].GetColor());
                    rw.ShouldDrawBackground = false;
                    rw.UserData = msgs[i].GetTarget();
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
            if (_chatboxInput.Text == "Press enter to chat.") { _chatboxInput.Text = ""; }
        }
        void ChatBoxInput_SubmitPressed(Base sender, EventArgs arguments)
        {
            if (_chatboxInput.Text.Trim().Length <= 0 || _chatboxInput.Text == "Press enter to chat.")
            {
                _chatboxInput.Text = "Press enter to chat.";
                return;
            }
            PacketSender.SendChatMsg(_chatboxInput.Text.Trim());
            _chatboxInput.Text = "Press enter to chat.";
        }
        void ChatBoxSendBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_chatboxInput.Text.Trim().Length <= 0 || _chatboxInput.Text == "Press enter to chat.")
            {
                _chatboxInput.Text = "Press enter to chat.";
                return;
            }
            PacketSender.SendChatMsg(_chatboxInput.Text.Trim());
            _chatboxInput.Text = "Press enter to chat.";
        }
    }
}
