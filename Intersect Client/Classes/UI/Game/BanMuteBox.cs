using System;
using Intersect.Localization;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.Core;

namespace Intersect_Client.Classes.UI.Game
{
    public class BanMuteBox
    {
        private ComboBox mDurationBox;
        private Label mDurationLabel;
        private CheckBox mIpCheckbox;
        private Label mIpLabel;
        private WindowControl mMyWindow;
        private TextBox mReasonBox;
        private Label mReasonLabel;
        private TextBoxNumeric mTextbox;

        public BanMuteBox(string title, string prompt, bool modal, EventHandler okayClicked)
        {
            OkayEventHandler = okayClicked;

            mMyWindow = new WindowControl(Gui.GameUi.GameCanvas, title, modal);
            mMyWindow.SetSize(500, 150);
            mMyWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() / 2 - mMyWindow.Width / 2,
                GameGraphics.Renderer.GetScreenHeight() / 2 - mMyWindow.Height / 2);
            mMyWindow.IsClosable = false;
            mMyWindow.DisableResizing();
            mMyWindow.Margin = Margin.Zero;
            mMyWindow.Padding = Padding.Zero;
            Gui.InputBlockingElements.Add(mMyWindow);

            Label promptLabel = new Label(mMyWindow);
            promptLabel.SetText(prompt);
            promptLabel.SetPosition(mMyWindow.Width / 2 - promptLabel.Width / 2, 8);

            int y = promptLabel.Y + promptLabel.Height + 8;

            mReasonLabel = new Label(mMyWindow)
            {
                Text = Strings.Get("banmute", "reason")
            };
            mReasonLabel.SetPosition(100, y);

            mReasonBox = new TextBox(mMyWindow);
            mReasonBox.SetBounds(180, y - 3, 220, 22);
            Gui.FocusElements.Add(mReasonBox);

            y = mReasonBox.Bottom + 6;

            mDurationLabel = new Label(mMyWindow)
            {
                Text = Strings.Get("banmute", "duration")
            };
            mDurationLabel.SetPosition(100, y);

            mDurationBox = new ComboBox(mMyWindow);
            mDurationBox.SetBounds(180, y - 3, 80, 22);
            mDurationBox.AddItem(Strings.Get("banmute", "1day")).UserData = "1 day";
            mDurationBox.AddItem(Strings.Get("banmute", "2days")).UserData = "2 days";
            mDurationBox.AddItem(Strings.Get("banmute", "3days")).UserData = "3 days";
            mDurationBox.AddItem(Strings.Get("banmute", "4days")).UserData = "4 days";
            mDurationBox.AddItem(Strings.Get("banmute", "5days")).UserData = "5 days";
            mDurationBox.AddItem(Strings.Get("banmute", "1week")).UserData = "1 week";
            mDurationBox.AddItem(Strings.Get("banmute", "2weeks")).UserData = "2 weeks";
            mDurationBox.AddItem(Strings.Get("banmute", "1month")).UserData = "1 month";
            mDurationBox.AddItem(Strings.Get("banmute", "2months")).UserData = "2 months";
            mDurationBox.AddItem(Strings.Get("banmute", "6months")).UserData = "6 months";
            mDurationBox.AddItem(Strings.Get("banmute", "1year")).UserData = "1 year";
            mDurationBox.AddItem(Strings.Get("banmute", "forever")).UserData = "Indefinitely";


            mIpLabel = new Label(mMyWindow)
            {
                Text = Strings.Get("banmute", "ip")
            };
            mIpLabel.SetPosition(320, y);

            mIpCheckbox = new CheckBox(mMyWindow);
            mIpCheckbox.SetPosition(400 - mIpCheckbox.Width, y);

            Button okayBtn = new Button(mMyWindow);
            okayBtn.SetSize(86, 22);
            okayBtn.SetText(Strings.Get("banmute", "ok"));
            okayBtn.SetPosition(mMyWindow.Width / 2 - 188 / 2, 90);
            okayBtn.Clicked += okayBtn_Clicked;

            Button cancelBtn = new Button(mMyWindow);
            cancelBtn.SetSize(86, 22);
            cancelBtn.SetText(Strings.Get("banmute", "cancel"));
            cancelBtn.Clicked += CancelBtn_Clicked;
            cancelBtn.SetPosition(mMyWindow.Width / 2 - 188 / 2 + 86 + 16, 90);
        }

        private event EventHandler OkayEventHandler;

        private void CancelBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Dispose();
        }

        void okayBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (OkayEventHandler != null)
            {
                OkayEventHandler(this, EventArgs.Empty);
            }
            mMyWindow.Close();
        }

        public void Dispose()
        {
            mMyWindow.Close();
            Gui.GameUi.GameCanvas.RemoveChild(mMyWindow, false);
            mMyWindow.Dispose();
        }

        public int GetDuration() //days by default
        {
            switch (mDurationBox.SelectedItem.UserData.ToString())
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
            return mReasonBox.Text;
        }

        public bool BanIp()
        {
            return mIpCheckbox.IsChecked;
        }
    }
}