using Intersect.Client.Core;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Localization;
using static Intersect.Client.Framework.File_Management.GameContentManager;

namespace Intersect.Client.Interface.Game.Admin;

public partial class BanMuteBox : WindowControl
{
    private readonly TextBox _textboxReason;
    private readonly ComboBox? _comboboxDuration;
    private readonly LabeledCheckBox _checkboxIP;

    private static readonly List<(string Label, int Days)> _durationOptions = new()
    {
        (Strings.BanMute.OneDay, 1),
        (Strings.BanMute.TwoDays, 2),
        (Strings.BanMute.ThreeDays, 3),
        (Strings.BanMute.FourDays, 4),
        (Strings.BanMute.FiveDays, 5),
        (Strings.BanMute.OneWeek, 7),
        (Strings.BanMute.TwoWeeks, 14),
        (Strings.BanMute.OneMonth, 30),
        (Strings.BanMute.TwoMonths, 60),
        (Strings.BanMute.SixMonths, 180),
        (Strings.BanMute.OneYear, 365),
        (Strings.BanMute.Forever, int.MaxValue),
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
        foreach (var option in _durationOptions)
        {
            _ = _comboboxDuration.AddItem(option.Label, userData: option.Days);
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
            Close();
        };

        var buttonCancel = new Button(this, "ButtonCancel")
        {
            Text = Strings.BanMute.Cancel,
        };
        buttonCancel.Clicked += (s, e) => Close();

        LoadJsonUi(UI.InGame, Graphics.Renderer?.GetResolutionString(), true);

        // Set the first element by default after loading the UI (ensures deterministic selection).
        if (_comboboxDuration != null && _durationOptions.Count > 0)
        {
            _ = _comboboxDuration.SelectByUserData(_durationOptions[0].Days);
        }

        richLabelPrompt.ClearText();
        richLabelPrompt.Width = promptContainer.Width - promptContainer.VerticalScrollBar.Width;
        richLabelPrompt.AddText(prompt, labelPrompt);
        _ = richLabelPrompt.SizeToChildren(false, true);
    }

    protected override void Dispose(bool disposing)
    {
        Interface.InputBlockingComponents.Remove(this);

        if (_textboxReason != null)
        {
            Interface.FocusComponents.Remove(_textboxReason);
        }

        base.Dispose(disposing);
    }

    public int GetDuration()
    {
        var selected = _comboboxDuration?.SelectedItem;

        if (selected?.UserData is int days)
        {
            return days;
        }
        return 1; // default value should be 1 day for safety
    }

    public string GetReason()
    {
        return _textboxReason?.Text ?? string.Empty;
    }

    public bool BanIp()
    {
        return _checkboxIP?.IsChecked ?? false;
    }
}