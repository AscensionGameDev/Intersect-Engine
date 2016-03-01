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
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Game
{
    public class Chatbox : IGUIElement
    {
        //Window Controls
        private WindowControl _chatboxWindow;
        private ListBox _chatboxMessages;
        private TextBox _chatboxInput;
        private Button _chatboxSendButton;

        //Init
        public Chatbox(Canvas _gameCanvas)
        {
            //Chatbox Window
            _chatboxWindow = new WindowControl(_gameCanvas, "Chatbox") { IsClosable = false };
            _chatboxWindow.SetSize(380, 140);
            _chatboxWindow.DisableResizing();
            _chatboxWindow.SetPosition(0, GameGraphics.Renderer.GetScreenHeight() - 140);
            _chatboxWindow.Margin = Margin.Zero;
            _chatboxWindow.Padding = Padding.Zero;

            _chatboxMessages = new ListBox(_chatboxWindow) { IsDisabled = true };
            _chatboxMessages.SetPosition(2, 1);
            _chatboxMessages.SetSize(376, 90);
            _chatboxMessages.ShouldDrawBackground = false;

            _chatboxInput = new TextBox(_chatboxWindow);
            _chatboxInput.SetPosition(2, 140 - 18 - 24);
            _chatboxInput.SetSize(338, 16);
            _chatboxInput.SubmitPressed += ChatBoxInput_SubmitPressed;
            _chatboxInput.Text = "Press enter to chat.";
            _chatboxInput.Clicked += ChatBoxInput_Clicked;
            Gui.FocusElements.Add(_chatboxInput);

            _chatboxSendButton = new Button(_chatboxWindow);
            _chatboxSendButton.SetSize(36, 16);
            _chatboxSendButton.SetPosition(342, 140 - 18 - 24);
            _chatboxSendButton.Text = "Send";
            _chatboxSendButton.Clicked += ChatBoxSendBtn_Clicked;
        }

        //Update
        public void Update()
        {
            if (Globals.ChatboxContent.Count > 0)
            {
                foreach (var t1 in Globals.ChatboxContent)
                {
                    var myText = Gui.WrapText(t1.Key, 360);
                    foreach (var t in myText)
                    {
                        var rw = _chatboxMessages.AddRow(t);
                        rw.SetTextColor(t1.Value);
                        rw.MouseInputEnabled = false;
                    }
                }

                Globals.ChatboxContent.Clear();
                _chatboxMessages.Redraw();
                _chatboxMessages.Think();
                _chatboxMessages.ScrollToBottom();

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
            if (_chatboxInput.Text.Trim().Length <= 0 || _chatboxInput.Text == "Press enter to chat.") return;
            PacketSender.SendChatMsg(_chatboxInput.Text.Trim());
            _chatboxInput.Text = "Press enter to chat.";
        }
        void ChatBoxSendBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_chatboxInput.Text.Trim().Length <= 0 || _chatboxInput.Text == "Press enter to chat.") return;
            PacketSender.SendChatMsg(_chatboxInput.Text.Trim());
            _chatboxInput.Text = "Press enter to chat.";
        }
    }
}
