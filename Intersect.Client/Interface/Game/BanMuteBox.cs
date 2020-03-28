using System;

using Intersect.Client.Core;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Game
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

            mMyWindow = new WindowControl(Interface.GameUi.GameCanvas, title, modal);
            mMyWindow.SetSize(500, 150);
            mMyWindow.SetPosition(
                Graphics.Renderer.GetScreenWidth() / 2 - mMyWindow.Width / 2,
                Graphics.Renderer.GetScreenHeight() / 2 - mMyWindow.Height / 2
            );

            mMyWindow.IsClosable = false;
            mMyWindow.DisableResizing();
            mMyWindow.Margin = Margin.Zero;
            mMyWindow.Padding = Padding.Zero;
            Interface.InputBlockingElements.Add(mMyWindow);

            var promptLabel = new Label(mMyWindow);
            promptLabel.SetText(prompt);
            promptLabel.SetPosition(mMyWindow.Width / 2 - promptLabel.Width / 2, 8);

            var y = promptLabel.Y + promptLabel.Height + 8;

            mReasonLabel = new Label(mMyWindow)
            {
                Text = Strings.BanMute.reason
            };

            mReasonLabel.SetPosition(100, y);

            mReasonBox = new TextBox(mMyWindow);
            mReasonBox.SetBounds(180, y - 3, 220, 22);
            Interface.FocusElements.Add(mReasonBox);

            y = mReasonBox.Bottom + 6;

            mDurationLabel = new Label(mMyWindow)
            {
                Text = Strings.BanMute.duration
            };

            mDurationLabel.SetPosition(100, y);

            mDurationBox = new ComboBox(mMyWindow);
            mDurationBox.SetBounds(180, y - 3, 80, 22);
            mDurationBox.AddItem(Strings.BanMute.oneday).UserData = "1 day";
            mDurationBox.AddItem(Strings.BanMute.twodays).UserData = "2 days";
            mDurationBox.AddItem(Strings.BanMute.threedays).UserData = "3 days";
            mDurationBox.AddItem(Strings.BanMute.fourdays).UserData = "4 days";
            mDurationBox.AddItem(Strings.BanMute.fivedays).UserData = "5 days";
            mDurationBox.AddItem(Strings.BanMute.oneweek).UserData = "1 week";
            mDurationBox.AddItem(Strings.BanMute.twoweeks).UserData = "2 weeks";
            mDurationBox.AddItem(Strings.BanMute.onemonth).UserData = "1 month";
            mDurationBox.AddItem(Strings.BanMute.twomonths).UserData = "2 months";
            mDurationBox.AddItem(Strings.BanMute.sixmonths).UserData = "6 months";
            mDurationBox.AddItem(Strings.BanMute.oneyear).UserData = "1 year";
            mDurationBox.AddItem(Strings.BanMute.forever).UserData = "Indefinitely";

            mIpLabel = new Label(mMyWindow)
            {
                Text = Strings.BanMute.ip
            };

            mIpLabel.SetPosition(320, y);

            mIpCheckbox = new CheckBox(mMyWindow);
            mIpCheckbox.SetPosition(400 - mIpCheckbox.Width, y);

            var okayBtn = new Button(mMyWindow);
            okayBtn.SetSize(86, 22);
            okayBtn.SetText(Strings.BanMute.ok);
            okayBtn.SetPosition(mMyWindow.Width / 2 - 188 / 2, 90);
            okayBtn.Clicked += okayBtn_Clicked;

            var cancelBtn = new Button(mMyWindow);
            cancelBtn.SetSize(86, 22);
            cancelBtn.SetText(Strings.BanMute.cancel);
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
            Interface.GameUi.GameCanvas.RemoveChild(mMyWindow, false);
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
