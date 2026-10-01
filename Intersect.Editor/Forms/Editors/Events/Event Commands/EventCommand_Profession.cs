using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Crafting;
using Intersect.Framework.Core.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;

/// <summary>Trainer / recipe item / gathering node actions of the profession system.</summary>
public partial class EventCommandProfession : UserControl
{
    private static readonly string[] ActionNames =
    [
        "Apprendre le métier",
        "Oublier le métier",
        "Apprendre une recette",
        "Montée de skill (récolte)",
        "Ajouter du skill",
    ];

    private readonly FrmEvent mEventEditor;

    private readonly ProfessionCommand mMyCommand;

    private readonly Profession[] mProfessions = Professions.All.ToArray();

    public EventCommandProfession(ProfessionCommand refCommand, FrmEvent editor)
    {
        InitializeComponent();
        mMyCommand = refCommand;
        mEventEditor = editor;
        InitLocalization();

        cmbAction.Items.Clear();
        cmbAction.Items.AddRange(ActionNames);
        cmbAction.SelectedIndex = Math.Clamp((int)mMyCommand.Action, 0, ActionNames.Length - 1);

        cmbProfession.Items.Clear();
        cmbProfession.Items.AddRange(mProfessions.Select(profession => (object)Professions.GetName(profession)).ToArray());
        var professionIndex = Array.IndexOf(mProfessions, mMyCommand.Profession);
        cmbProfession.SelectedIndex = Math.Max(0, professionIndex);

        cmbRecipe.Items.Clear();
        cmbRecipe.Items.AddRange(CraftingRecipeDescriptor.Names);
        cmbRecipe.SelectedIndex = Math.Max(0, CraftingRecipeDescriptor.ListIndex(mMyCommand.RecipeId));

        nudAmount.Value = Math.Clamp(mMyCommand.Amount, (int)nudAmount.Minimum, (int)nudAmount.Maximum);

        UpdateEnabledFields();
    }

    private void InitLocalization()
    {
        grpProfession.Text = Strings.EventProfession.title;
        lblAction.Text = Strings.EventProfession.action;
        lblProfession.Text = Strings.EventProfession.profession;
        lblRecipe.Text = Strings.EventProfession.recipe;
        lblAmount.Text = Strings.EventProfession.amount;
        btnSave.Text = Strings.EventProfession.okay;
        btnCancel.Text = Strings.EventProfession.cancel;
    }

    private void UpdateEnabledFields()
    {
        var action = (ProfessionAction)Math.Max(0, cmbAction.SelectedIndex);
        cmbRecipe.Enabled = action == ProfessionAction.LearnRecipe;
        cmbProfession.Enabled = action != ProfessionAction.LearnRecipe;
        nudAmount.Enabled = action is ProfessionAction.GatheringSkillUp or ProfessionAction.AddSkill;
    }

    private void cmbAction_SelectedIndexChanged(object sender, EventArgs e) => UpdateEnabledFields();

    private void btnSave_Click(object sender, EventArgs e)
    {
        mMyCommand.Action = (ProfessionAction)Math.Max(0, cmbAction.SelectedIndex);
        mMyCommand.Profession = cmbProfession.SelectedIndex >= 0 ? mProfessions[cmbProfession.SelectedIndex] : Profession.None;
        mMyCommand.RecipeId = cmbRecipe.SelectedIndex >= 0 ? CraftingRecipeDescriptor.IdFromList(cmbRecipe.SelectedIndex) : Guid.Empty;
        mMyCommand.Amount = (int)nudAmount.Value;

        mEventEditor.FinishCommandEdit();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        mEventEditor.CancelCommandEdit();
    }
}
