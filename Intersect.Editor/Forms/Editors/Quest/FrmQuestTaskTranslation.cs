using DarkUI.Controls;
using Intersect.Editor.Core;
using Intersect.GameObjects;

namespace Intersect.Editor.Forms.Editors.Quest;

/// <summary>
/// Tiny popup for the optional English translation of a single quest task's description - the task
/// editor itself is too small/tightly packed to fit a second text field next to the original.
/// </summary>
public class FrmQuestTaskTranslation : Form
{
    private readonly QuestTaskDescriptor _task;

    private TextBox _txtDescriptionEn;

    public FrmQuestTaskTranslation(QuestTaskDescriptor task)
    {
        _task = task;

        Text = "English Translation - Task";
        Icon = Program.Icon;
        BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(360, 250);

        var lblBase = new Label
        {
            Text = "Description (original) :",
            ForeColor = System.Drawing.Color.Gainsboro,
            Location = new System.Drawing.Point(8, 8),
            AutoSize = true,
        };

        var txtBase = new TextBox
        {
            Text = task.Description,
            ReadOnly = true,
            Multiline = true,
            Location = new System.Drawing.Point(8, 24),
            Size = new Size(340, 70),
            BackColor = System.Drawing.Color.FromArgb(35, 35, 38),
            ForeColor = System.Drawing.Color.Silver,
            BorderStyle = BorderStyle.FixedSingle,
            ScrollBars = ScrollBars.Vertical,
        };

        var lblEn = new Label
        {
            Text = "Description (English) :",
            ForeColor = System.Drawing.Color.Gainsboro,
            Location = new System.Drawing.Point(8, 102),
            AutoSize = true,
        };

        _txtDescriptionEn = new TextBox
        {
            Text = task.DescriptionEn,
            Multiline = true,
            Location = new System.Drawing.Point(8, 118),
            Size = new Size(340, 70),
            BackColor = System.Drawing.Color.FromArgb(60, 63, 65),
            ForeColor = System.Drawing.Color.Gainsboro,
            BorderStyle = BorderStyle.FixedSingle,
            ScrollBars = ScrollBars.Vertical,
        };

        var btnSave = new DarkButton
        {
            Text = "Save",
            Location = new System.Drawing.Point(180, 200),
            Size = new Size(80, 26),
        };
        btnSave.Click += (_, _) =>
        {
            _task.DescriptionEn = _txtDescriptionEn.Text;
            Close();
        };

        var btnCancel = new DarkButton
        {
            Text = "Cancel",
            Location = new System.Drawing.Point(268, 200),
            Size = new Size(80, 26),
        };
        btnCancel.Click += (_, _) => Close();

        Controls.Add(lblBase);
        Controls.Add(txtBase);
        Controls.Add(lblEn);
        Controls.Add(_txtDescriptionEn);
        Controls.Add(btnSave);
        Controls.Add(btnCancel);
    }
}
