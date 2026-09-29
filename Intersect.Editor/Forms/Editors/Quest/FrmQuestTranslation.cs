using DarkUI.Controls;
using Intersect.Editor.Core;
using Intersect.GameObjects;

namespace Intersect.Editor.Forms.Editors.Quest;

/// <summary>
/// Small popup letting the mapper fill in optional English translations for a quest's name and
/// descriptions, alongside the base (originally-authored) text so they can compare while typing.
/// Purely optional - any field left empty just falls back to the base text for English-language
/// clients (see <see cref="QuestDescriptor.GetLocalizedName"/> and friends).
/// </summary>
public class FrmQuestTranslation : Form
{
    private readonly QuestDescriptor _quest;

    private TextBox _txtNameEn;
    private TextBox _txtStartDescEn;
    private TextBox _txtBeforeDescEn;
    private TextBox _txtInProgressDescEn;
    private TextBox _txtEndDescEn;

    public FrmQuestTranslation(QuestDescriptor quest)
    {
        _quest = quest;

        Text = $"English Translation - {quest.Name}";
        Icon = Program.Icon;
        BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(620, 460);

        var y = 8;
        y = AddPair("Name", quest.Name, quest.NameEn, out _txtNameEn, y, 40);
        y = AddPair("Start Description", quest.StartDescription, quest.StartDescriptionEn, out _txtStartDescEn, y, 90);
        y = AddPair("Before Description", quest.BeforeDescription, quest.BeforeDescriptionEn, out _txtBeforeDescEn, y, 90);
        y = AddPair("In Progress Description", quest.InProgressDescription, quest.InProgressDescriptionEn, out _txtInProgressDescEn, y, 90);
        y = AddPair("End Description", quest.EndDescription, quest.EndDescriptionEn, out _txtEndDescEn, y, 90);

        var btnSave = new DarkButton
        {
            Text = "Save",
            Location = new System.Drawing.Point(440, y + 8),
            Size = new Size(80, 26),
        };
        btnSave.Click += (_, _) => Save();

        var btnCancel = new DarkButton
        {
            Text = "Cancel",
            Location = new System.Drawing.Point(528, y + 8),
            Size = new Size(80, 26),
        };
        btnCancel.Click += (_, _) => Close();

        Controls.Add(btnSave);
        Controls.Add(btnCancel);

        ClientSize = new Size(620, y + 44);
    }

    private int AddPair(string label, string baseText, string enText, out TextBox txtEn, int y, int height)
    {
        var lblBase = new Label
        {
            Text = $"{label} (original) :",
            ForeColor = System.Drawing.Color.Gainsboro,
            Location = new System.Drawing.Point(8, y),
            AutoSize = true,
        };

        var txtBase = new TextBox
        {
            Text = baseText,
            ReadOnly = true,
            Multiline = height > 40,
            Location = new System.Drawing.Point(8, y + 16),
            Size = new Size(290, height),
            BackColor = System.Drawing.Color.FromArgb(35, 35, 38),
            ForeColor = System.Drawing.Color.Silver,
            BorderStyle = BorderStyle.FixedSingle,
            ScrollBars = height > 40 ? ScrollBars.Vertical : ScrollBars.None,
        };

        var lblEn = new Label
        {
            Text = $"{label} (English) :",
            ForeColor = System.Drawing.Color.Gainsboro,
            Location = new System.Drawing.Point(310, y),
            AutoSize = true,
        };

        txtEn = new TextBox
        {
            Text = enText,
            Multiline = height > 40,
            Location = new System.Drawing.Point(310, y + 16),
            Size = new Size(300, height),
            BackColor = System.Drawing.Color.FromArgb(60, 63, 65),
            ForeColor = System.Drawing.Color.Gainsboro,
            BorderStyle = BorderStyle.FixedSingle,
            ScrollBars = height > 40 ? ScrollBars.Vertical : ScrollBars.None,
        };

        Controls.Add(lblBase);
        Controls.Add(txtBase);
        Controls.Add(lblEn);
        Controls.Add(txtEn);

        return y + 16 + height + 12;
    }

    private void Save()
    {
        _quest.NameEn = _txtNameEn.Text;
        _quest.StartDescriptionEn = _txtStartDescEn.Text;
        _quest.BeforeDescriptionEn = _txtBeforeDescEn.Text;
        _quest.InProgressDescriptionEn = _txtInProgressDescEn.Text;
        _quest.EndDescriptionEn = _txtEndDescEn.Text;

        Close();
    }
}
