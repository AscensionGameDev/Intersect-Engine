using DarkUI.Controls;
using Intersect.Framework.Core.GameObjects.Crafting;

namespace Intersect.Editor.Forms.Editors;

/// <summary>Profession fields of a recipe (WoW-style skill colours), built in code below the classic recipe fields.</summary>
public partial class FrmCrafts
{
    private readonly Profession[] mProfessionChoices = Professions.All.ToArray();

    private bool mLoadingProfession;

    private DarkGroupBox mGrpProfession;

    private DarkComboBox mCmbProfession;

    private DarkCheckBox mChkTrainable;

    private DarkNumericUpDown mNudOrange;

    private DarkNumericUpDown mNudYellow;

    private DarkNumericUpDown mNudGreen;

    private DarkNumericUpDown mNudGray;

    private DarkButton mBtnAutoColors;

    private void BuildProfessionControls()
    {
        mGrpProfession = new DarkGroupBox
        {
            Text = "Métier (système de skill)",
            Location = new System.Drawing.Point(221, 419),
            Size = new System.Drawing.Size(567, 98),
            ForeColor = System.Drawing.Color.Gainsboro,
            BackColor = System.Drawing.Color.FromArgb(45, 45, 48),
            BorderColor = System.Drawing.Color.FromArgb(90, 90, 90),
        };

        mGrpProfession.Controls.Add(MakeLabel("Métier :", 10, 25));
        mCmbProfession = new DarkComboBox
        {
            Location = new System.Drawing.Point(70, 22),
            Size = new System.Drawing.Size(170, 21),
            DropDownStyle = ComboBoxStyle.DropDownList,
        };
        mCmbProfession.Items.Add("Aucun (recette classique)");
        mCmbProfession.Items.AddRange(mProfessionChoices.Select(profession => (object)Professions.GetName(profession)).ToArray());
        mCmbProfession.SelectedIndexChanged += (_, _) =>
        {
            if (mLoadingProfession || mEditorItem == null)
            {
                return;
            }

            mEditorItem.Profession = mCmbProfession.SelectedIndex <= 0 ? 0 : (int)mProfessionChoices[mCmbProfession.SelectedIndex - 1];
        };
        mGrpProfession.Controls.Add(mCmbProfession);

        mChkTrainable = new DarkCheckBox
        {
            Text = "Enseignée par l'entraîneur dès que le skill est atteint",
            Location = new System.Drawing.Point(260, 24),
            Size = new System.Drawing.Size(300, 19),
            Checked = true,
        };
        mChkTrainable.CheckedChanged += (_, _) =>
        {
            if (!mLoadingProfession && mEditorItem != null)
            {
                mEditorItem.Trainable = mChkTrainable.Checked;
            }
        };
        mGrpProfession.Controls.Add(mChkTrainable);

        mNudOrange = MakeSkillBox(90, 60, value => mEditorItem.SkillOrange = value);
        mNudYellow = MakeSkillBox(205, 60, value => mEditorItem.SkillYellow = value);
        mNudGreen = MakeSkillBox(305, 60, value => mEditorItem.SkillGreen = value);
        mNudGray = MakeSkillBox(395, 60, value => mEditorItem.SkillGray = value);

        mGrpProfession.Controls.Add(MakeLabel("Orange (requis) :", 10, 63, System.Drawing.Color.Orange));
        mGrpProfession.Controls.Add(MakeLabel("Jaune :", 160, 63, System.Drawing.Color.Gold));
        mGrpProfession.Controls.Add(MakeLabel("Vert :", 270, 63, System.Drawing.Color.LimeGreen));
        mGrpProfession.Controls.Add(MakeLabel("Gris :", 360, 63, System.Drawing.Color.Gray));

        mBtnAutoColors = new DarkButton
        {
            Text = "Auto (+25)",
            Location = new System.Drawing.Point(475, 58),
            Size = new System.Drawing.Size(80, 25),
            Padding = new Padding(5),
        };
        mBtnAutoColors.Click += (_, _) =>
        {
            if (mEditorItem == null)
            {
                return;
            }

            var orange = (int)mNudOrange.Value;
            mNudYellow.Value = Math.Min(Professions.MaxSkill, orange + 25);
            mNudGreen.Value = Math.Min(Professions.MaxSkill, orange + 50);
            mNudGray.Value = Math.Min(Professions.MaxSkill, orange + 75);
        };
        mGrpProfession.Controls.Add(mBtnAutoColors);

        Controls.Add(mGrpProfession);

        // Make room below the classic fields for the new group.
        ClientSize = new System.Drawing.Size(ClientSize.Width, 561);
        btnSave.Location = new System.Drawing.Point(btnSave.Location.X, 526);
        btnCancel.Location = new System.Drawing.Point(btnCancel.Location.X, 526);
    }

    private static Label MakeLabel(string text, int x, int y, System.Drawing.Color? color = null) => new()
    {
        Text = text,
        AutoSize = true,
        Location = new System.Drawing.Point(x, y),
        ForeColor = color ?? System.Drawing.Color.Gainsboro,
    };

    private DarkNumericUpDown MakeSkillBox(int x, int y, Action<int> apply)
    {
        var box = new DarkNumericUpDown
        {
            Location = new System.Drawing.Point(x, y),
            Size = new System.Drawing.Size(60, 20),
            Minimum = 0,
            Maximum = Professions.MaxSkill,
            ForeColor = System.Drawing.Color.Gainsboro,
        };
        box.ValueChanged += (_, _) =>
        {
            if (!mLoadingProfession && mEditorItem != null)
            {
                apply((int)box.Value);
            }
        };
        mGrpProfession.Controls.Add(box);

        return box;
    }

    private void LoadProfessionControls()
    {
        mLoadingProfession = true;
        try
        {
            var index = Array.IndexOf(mProfessionChoices, (Profession)mEditorItem.Profession);
            mCmbProfession.SelectedIndex = index < 0 ? 0 : index + 1;
            mChkTrainable.Checked = mEditorItem.Trainable;
            mNudOrange.Value = Math.Clamp(mEditorItem.SkillOrange, 0, Professions.MaxSkill);
            mNudYellow.Value = Math.Clamp(mEditorItem.SkillYellow, 0, Professions.MaxSkill);
            mNudGreen.Value = Math.Clamp(mEditorItem.SkillGreen, 0, Professions.MaxSkill);
            mNudGray.Value = Math.Clamp(mEditorItem.SkillGray, 0, Professions.MaxSkill);
        }
        finally
        {
            mLoadingProfession = false;
        }
    }
}
