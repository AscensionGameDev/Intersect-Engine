using Intersect.Client.Core;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Localization;
using static Intersect.Client.Framework.File_Management.GameContentManager;

namespace Intersect.Client.Interface.Game.Admin;

public partial class BanMuteBox : WindowControl
{
    private readonly TextBox _textboxReason;
    private readonly ComboBox _comboboxDuration;
    private readonly LabeledCheckBox _checkboxIP;

    private readonly Dictionary<string, int> _dayCount = new()
    {
        {
            Strings.BanMute.OneDay, 1
        },
        {
            Strings.BanMute.TwoDays, 2
        },
        {
            Strings.BanMute.ThreeDays, 3
        },
        {
            Strings.BanMute.FourDays, 4
        },
        {
            Strings.BanMute.FiveDays, 5
        },
        {
            Strings.BanMute.OneWeek, 7
        },
        {
            Strings.BanMute.TwoWeeks, 14
        },
        {
            Strings.BanMute.OneMonth, 30
        },
        {
            Strings.BanMute.TwoMonths, 60
        },
        {
            Strings.BanMute.SixMonths, 180
        },
        {
            Strings.BanMute.OneYear, 365
        },
        {
            Strings.BanMute.Forever, 999999
        },
    };

    public BanMuteBox(string title, string prompt, EventHandler okayHandler) : base(
        Interface.GameUi.GameCanvas,
        title,
        true,
        "BanMuteWindow"
    )
    {
        DisableResizing();
        Interface.InputBlockingComponents.Add(this);

        // Prompt label
        var promptContainer = new ScrollControl(this, "PromptContainer");
        var labelPrompt = new Label(promptContainer, "LabelPrompt");
        var richLabelPrompt = new RichLabel(promptContainer);

        // Reason label
        _ = new Label(this, "LabelReason")
        {
            Text = Strings.BanMute.Reason,
        };

        // Reason textbox
        _textboxReason = new TextBox(this, "TextboxReason");
        Interface.FocusComponents.Add(_textboxReason);

        // Duration label
        _ = new Label(this, "LabelDuration")
        {
            Text = Strings.BanMute.Duration,
        };

        // Duration combobox
        _comboboxDuration = new ComboBox(this, "ComboBoxDuration");
        foreach (var day in _dayCount)
        {
            _ = _comboboxDuration.AddItem(day.Key, userData: day.Value);
        }

        // Include IP checkbox
        _checkboxIP = new LabeledCheckBox(this, "CheckboxIp")
        {
            Text = Strings.BanMute.IncludeIp,
        };

        // Ok and Cancel buttons
        var buttonOkay = new Button(this, "ButtonOkay")
        {
            Text = Strings.BanMute.Okay,
        };
        buttonOkay.Clicked += (s, e) =>
        {
            okayHandler?.Invoke(this, EventArgs.Empty);
            Dispose();
        };

        var buttonCancel = new Button(this, "ButtonCancel")
        {
            Text = Strings.BanMute.Cancel,
        };
        buttonCancel.Clicked += (s, e) => Dispose();

        LoadJsonUi(UI.InGame, Graphics.Renderer?.GetResolutionString(), true);

        richLabelPrompt.ClearText();
        richLabelPrompt.Width = promptContainer.Width - promptContainer.VerticalScrollBar.Width;
        richLabelPrompt.AddText(prompt, labelPrompt);
        _ = richLabelPrompt.SizeToChildren(false, true);
    }

    public override void Dispose()
    {
        Close();
        Interface.GameUi.GameCanvas.RemoveChild(this, false);
        base.Dispose();
        GC.SuppressFinalize(this);
    }

    public int GetDuration()
    {
        return (int)_comboboxDuration.SelectedItem.UserData;
    }

    public string GetReason()
    {
        return _textboxReason.Text;
    }

    public bool BanIp()
    {
        return _checkboxIP.IsChecked;
    }
}