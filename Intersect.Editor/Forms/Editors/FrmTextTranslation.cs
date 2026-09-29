using DarkUI.Controls;
using Intersect.Editor.Core;

namespace Intersect.Editor.Forms.Editors;

/// <summary>
/// Small reusable popup for filling in optional English translations of one or more pieces of event
/// text (dialogue, chatbox messages, dialogue options...), shown next to their original (base-language)
/// text for reference. Purely optional per field - anything left empty just falls back to the base text
/// for English-language clients.
/// </summary>
public class FrmTextTranslation : Form
{
    private readonly TextBox[] _enBoxes;
    private readonly Action<string[]> _onSave;

    /// <param name="title">Window title.</param>
    /// <param name="fields">One entry per field: (label, original/base text, current English text).</param>
    /// <param name="onSave">Called with the edited English text for each field, in the same order, when Save is clicked.</param>
    public FrmTextTranslation(string title, (string Label, string Base, string En)[] fields, Action<string[]> onSave)
    {
        _onSave = onSave;
        _enBoxes = new TextBox[fields.Length];

        Text = title;
        Icon = Program.Icon;
        BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(620, 40);

        var y = 8;
        for (var i = 0; i < fields.Length; i++)
        {
            y = AddPair(fields[i].Label, fields[i].Base, fields[i].En, i, y);
        }

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

    private int AddPair(string label, string baseText, string enText, int index, int y)
    {
        var height = baseText.Length > 60 || baseText.Contains('\n') ? 90 : 40;

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

        _enBoxes[index] = new TextBox
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
        Controls.Add(_enBoxes[index]);

        return y + 16 + height + 12;
    }

    private void Save()
    {
        _onSave(_enBoxes.Select(box => box.Text).ToArray());
        Close();
    }
}
