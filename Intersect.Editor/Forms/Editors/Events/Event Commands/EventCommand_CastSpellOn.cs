using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Events.Commands;
using Intersect.GameObjects;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;
public partial class EventCommand_CastSpellOn : UserControl
{
    private readonly FrmEvent mEventEditor;

    private CastSpellOn mMyCommand;

    public EventCommand_CastSpellOn(CastSpellOn refCommand, FrmEvent editor)
    {
        InitializeComponent();

        mEventEditor = editor;
        mMyCommand = refCommand;

        InitLocalization();

        cmbSpell.Items.Clear();
        cmbSpell.Items.AddRange(SpellBase.Names);

        PopulateForm(mMyCommand);
    }

    private void InitLocalization()
    {
        grpCastSpellOn.Text = Strings.EventCastSpellOn.Title;

        grpTargets.Text = Strings.EventCastSpellOn.LabelTargets;

        lblSpell.Text = Strings.EventCastSpellOn.LabelSpell;
        chkApplyToSelf.Text = Strings.EventCastSpellOn.IncludeSelf;
        chkApplyToParty.Text = Strings.EventCastSpellOn.IncludePartyMembers;
        chkApplyToGuildies.Text = Strings.EventCastSpellOn.IncludeGuildies;

        btnSave.Text = Strings.EventCastSpellOn.Okay;
        btnCancel.Text = Strings.EventCastSpellOn.Cancel;
    }

    public void PopulateForm(CastSpellOn command)
    {
        cmbSpell.SelectedIndex = SpellBase.ListIndex(command.SpellId);
        chkApplyToSelf.Checked = command.Self;
        chkApplyToGuildies.Checked = command.GuildMembers;
        chkApplyToParty.Checked = command.PartyMembers;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        mMyCommand.SpellId = SpellBase.IdFromList(cmbSpell.SelectedIndex);
        mMyCommand.Self = chkApplyToSelf.Checked;
        mMyCommand.PartyMembers = chkApplyToParty.Checked;
        mMyCommand.GuildMembers = chkApplyToGuildies.Checked;

        mEventEditor.FinishCommandEdit();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        mEventEditor.CancelCommandEdit();
    }
}
