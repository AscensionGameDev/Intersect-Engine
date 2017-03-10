using System;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using IntersectClientExtras.Gwen;
using Intersect_Library.Localization;

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
            Gui.InputBlockingElements.Add(_myWindow);


            Label promptLabel = new Label(_myWindow);
            promptLabel.SetText(prompt);
            promptLabel.SetPosition(_myWindow.Width / 2 - promptLabel.Width / 2, 8);

            int y = promptLabel.Y + promptLabel.Height + 8;

            _reasonLabel = new Label(_myWindow);
            _reasonLabel.Text = Strings.Get("banmute","reason");
            _reasonLabel.SetPosition(100, y);

            _reasonBox = new TextBox(_myWindow);
            _reasonBox.SetBounds(180, y-3, 220, 22);
            Gui.FocusElements.Add(_reasonBox);

            y = _reasonBox.Bottom + 6;

            _durationLabel = new Label(_myWindow);
            _durationLabel.Text = Strings.Get("banmute", "duration");
            _durationLabel.SetPosition(100, y);

            _durationBox = new ComboBox(_myWindow);
            _durationBox.SetBounds(180, y - 3, 80, 22);
            _durationBox.AddItem(Strings.Get("banmute", "1day")).UserData = "1 day";
            _durationBox.AddItem(Strings.Get("banmute", "2days")).UserData = "2 days";
            _durationBox.AddItem(Strings.Get("banmute", "3days")).UserData = "3 days";
            _durationBox.AddItem(Strings.Get("banmute", "4days")).UserData = "4 days";
            _durationBox.AddItem(Strings.Get("banmute", "5days")).UserData = "5 days";
            _durationBox.AddItem(Strings.Get("banmute", "1week")).UserData = "1 week";
            _durationBox.AddItem(Strings.Get("banmute", "2weeks")).UserData = "2 weeks";
            _durationBox.AddItem(Strings.Get("banmute", "1month")).UserData = "1 month";
            _durationBox.AddItem(Strings.Get("banmute", "2months")).UserData = "2 months";
            _durationBox.AddItem(Strings.Get("banmute", "6months")).UserData = "6 months";
            _durationBox.AddItem(Strings.Get("banmute", "1year")).UserData = "1 year";
            _durationBox.AddItem(Strings.Get("banmute", "forever")).UserData = "Indefinitely";

            _ipLabel = new Label(_myWindow);
            _ipLabel.Text = Strings.Get("banmute", "ip");
            _ipLabel.SetPosition(320, y);

            _ipCheckbox = new CheckBox(_myWindow);
            _ipCheckbox.SetPosition(400 - _ipCheckbox.Width,y);


            Button okayBtn =  new Button(_myWindow);
            okayBtn.SetSize(86,22);
            okayBtn.SetText(Strings.Get("banmute", "ok"));
            okayBtn.SetPosition(_myWindow.Width/2 - 188 /2, 90);
            okayBtn.Clicked += okayBtn_Clicked;

            Button cancelBtn = new Button(_myWindow);
            cancelBtn.SetSize(86, 22);
            cancelBtn.SetText(Strings.Get("banmute", "cancel"));
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
            switch (_durationBox.UserData.ToString())
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
