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
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using IntersectClientExtras.Gwen;

namespace Intersect_Client.Classes.UI.Game
{
    public class BanMuteBox
    {
        private WindowControl _myWindow;
        private event EventHandler _okayEventHandler;
        private TextBoxNumeric _textbox;
        private Label _reasonLabel;
        private TextBox _reasonBox;
        private Label _durationLabel;
        private ComboBox _durationBox;
        private Label _ipLabel;
        private CheckBox _ipCheckbox;

        public BanMuteBox(string title, string prompt, bool modal, EventHandler okayClicked)
        {

            _okayEventHandler = okayClicked;

            _myWindow = new WindowControl(Gui.GameUI.GameCanvas, title, modal);
            _myWindow.SetSize(500, 150);
            _myWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() / 2 - _myWindow.Width / 2, GameGraphics.Renderer.GetScreenHeight() / 2 - _myWindow.Height / 2);
            _myWindow.IsClosable = false;
            _myWindow.DisableResizing();
            _myWindow.Margin = Margin.Zero;
            _myWindow.Padding = Padding.Zero;


            Label promptLabel = new Label(_myWindow);
            promptLabel.SetText(prompt);
            promptLabel.SetPosition(_myWindow.Width / 2 - promptLabel.Width / 2, 8);

            int y = promptLabel.Y + promptLabel.Height + 8;

            _reasonLabel = new Label(_myWindow);
            _reasonLabel.Text = "Reason: ";
            _reasonLabel.SetPosition(100, y);

            _reasonBox = new TextBox(_myWindow);
            _reasonBox.SetBounds(180, y-3, 220, 22);
            Gui.FocusElements.Add(_reasonBox);

            y = _reasonBox.Bottom + 6;

            _durationLabel = new Label(_myWindow);
            _durationLabel.Text = "Duration: ";
            _durationLabel.SetPosition(100, y);

            _durationBox = new ComboBox(_myWindow);
            _durationBox.SetBounds(180, y - 3, 80, 22);
            _durationBox.AddItem("1 day");
            _durationBox.AddItem("2 days");
            _durationBox.AddItem("3 days");
            _durationBox.AddItem("4 days");
            _durationBox.AddItem("5 days");
            _durationBox.AddItem("1 week");
            _durationBox.AddItem("2 weeks");
            _durationBox.AddItem("1 month");
            _durationBox.AddItem("2 months");
            _durationBox.AddItem("6 months");
            _durationBox.AddItem("1 year");
            _durationBox.AddItem("Indefinitely");

            _ipLabel = new Label(_myWindow);
            _ipLabel.Text = "Include IP: ";
            _ipLabel.SetPosition(320, y);

            _ipCheckbox = new CheckBox(_myWindow);
            _ipCheckbox.SetPosition(400 - _ipCheckbox.Width,y);


            Button okayBtn =  new Button(_myWindow);
            okayBtn.SetSize(86,22);
            okayBtn.SetText("Okay");
            okayBtn.SetPosition(_myWindow.Width/2 - 188 /2, 90);
            okayBtn.Clicked += okayBtn_Clicked;

            Button cancelBtn = new Button(_myWindow);
            cancelBtn.SetSize(86, 22);
            cancelBtn.SetText("Cancel");
            cancelBtn.Clicked += CancelBtn_Clicked;
            cancelBtn.SetPosition(_myWindow.Width / 2 - 188 / 2 + 86 + 16, 90);

        }

        private void CancelBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Dispose();
        }

        void okayBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_okayEventHandler != null) { _okayEventHandler(this, EventArgs.Empty); }
            _myWindow.Close();
        }

        public void Dispose()
        {
            _myWindow.Close();
            Gui.GameUI.GameCanvas.RemoveChild(_myWindow,false);
            _myWindow.Dispose();
        }

        public int GetDuration() //days by default
        {
            switch (_durationBox.Text)
            {
                case "1 day":
                    return 1;
                case "2 days":
                    return 2;
                case "3 days":
                    return 3;
                case "5 days":
                    return 5;
                case "1 week":
                    return 7;
                case "2 weeks":
                    return 14;
                case "1 month":
                    return 30;
                case "2 months":
                    return 60;
                case "6 months":
                    return 180;
                case "1 year":
                    return 365;
                case "Indefinitely":
                    return 999999;
            }
            return 1;
        }

        public string GetReason()
        {
            return _reasonBox.Text;
        }

        public bool BanIp()
        {
            return _ipCheckbox.IsChecked;
        }

    }
}
