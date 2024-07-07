using Intersect.Client.Core;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Game;


public partial class BanMuteBox
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
        mMyWindow.SetSize(400, 150);
        mMyWindow.SetPosition(
            Graphics.Renderer.GetScreenWidth() / 2 - mMyWindow.Width / 2,
            Graphics.Renderer.GetScreenHeight() / 2 - mMyWindow.Height / 2
        );
        mMyWindow.SetTextColor(Color.White, WindowControl.ControlState.Active);

        mMyWindow.IsClosable = false;
        mMyWindow.DisableResizing();
        mMyWindow.Margin = Margin.Zero;
        mMyWindow.Padding = Padding.Zero;
        Interface.InputBlockingElements.Add(mMyWindow);

        var promptLabel = new Label(mMyWindow);
        promptLabel.SetText(prompt);
        promptLabel.SetPosition(mMyWindow.Width / 2 - promptLabel.Width / 2, 8);
        promptLabel.TextColor = Color.White;

        var y = promptLabel.Y + promptLabel.Height + 8;

        mReasonLabel = new Label(mMyWindow)
        {
            Text = Strings.BanMute.Reason
        };
        mReasonLabel.SetPosition(32, y);
        mReasonLabel.TextColor = Color.White;
        
        mReasonBox = new TextBox(mMyWindow);
        mReasonBox.SetBounds(90, y - 3, 220, 22);
        Interface.FocusElements.Add(mReasonBox);
        y = mReasonBox.Bottom + 6;

        mDurationLabel = new Label(mMyWindow) { Text = Strings.BanMute.Duration };
        mDurationLabel.SetPosition(32, y);
        mDurationLabel.TextColor = Color.White;
        
        mDurationBox = new ComboBox(mMyWindow);
        mDurationBox.SetMenuBackgroundColor(Color.FromArgb(242, 27,36,49));
        mDurationBox.SetBounds(90, y - 3, 120, 22);
        mDurationBox.AddItem(Strings.BanMute.OneDay).UserData = "1 day";
        mDurationBox.AddItem(Strings.BanMute.TwoDays).UserData = "2 days";
        mDurationBox.AddItem(Strings.BanMute.ThreeDays).UserData = "3 days";
        mDurationBox.AddItem(Strings.BanMute.FourDays).UserData = "4 days";
        mDurationBox.AddItem(Strings.BanMute.FiveDays).UserData = "5 days";
        mDurationBox.AddItem(Strings.BanMute.OneWeek).UserData = "1 week";
        mDurationBox.AddItem(Strings.BanMute.TwoWeeks).UserData = "2 weeks";
        mDurationBox.AddItem(Strings.BanMute.OneMonth).UserData = "1 month";
        mDurationBox.AddItem(Strings.BanMute.TwoMonths).UserData = "2 months";
        mDurationBox.AddItem(Strings.BanMute.SixMonths).UserData = "6 months";
        mDurationBox.AddItem(Strings.BanMute.OneYear).UserData = "1 year";
        mDurationBox.AddItem(Strings.BanMute.Forever).UserData = "Indefinitely";
        
        mDurationBox.SetTextColor(Color.White, Label.ControlState.Normal);
        mDurationBox.SetTextColor(Color.White, Label.ControlState.Hovered);

        mIpLabel = new Label(mMyWindow)
        {
            Text = Strings.BanMute.IncludeIp
        };

        mIpLabel.SetPosition(235, y);
        mIpLabel.TextColor = Color.White;

        mIpCheckbox = new CheckBox(mMyWindow);
        mIpCheckbox.SetSize(22, 22);
        mIpCheckbox.SetPosition(310 - mIpCheckbox.Width, y - 2);

        var okayBtn = new Button(mMyWindow);
        okayBtn.SetSize(86, 22);
        okayBtn.SetTextColor(Color.White, Label.ControlState.Normal);
        okayBtn.SetTextColor(Color.White, Label.ControlState.Clicked);
        okayBtn.SetText(Strings.BanMute.Okay);
        okayBtn.SetPosition(mMyWindow.Width / 2 - 188 / 2, 90);
        okayBtn.Clicked += okayBtn_Clicked;

        var cancelBtn = new Button(mMyWindow);
        cancelBtn.SetTextColor(Color.White, Label.ControlState.Normal);
        cancelBtn.SetTextColor(Color.White, Label.ControlState.Clicked);
        cancelBtn.SetSize(86, 22);
        cancelBtn.SetText(Strings.BanMute.Cancel);
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
        OkayEventHandler?.Invoke(this, EventArgs.Empty);
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
