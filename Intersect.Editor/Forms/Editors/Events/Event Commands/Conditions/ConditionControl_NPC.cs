using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;
using Intersect.Framework.Core.GameObjects.NPCs;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

public partial class ConditionControl_NPC : UserControl
{
    public ConditionControl_NPC()
    {
        InitializeComponent();
        InitLocalization();
    }

    public void InitLocalization()
    {
        grpNpc.Text = Strings.EventConditional.NpcGroup;
        lblNpc.Text = Strings.EventConditional.NpcLabel;
        chkNpc.Text = Strings.EventConditional.SpecificNpcCheck;
    }

    public void SetupFormValues(NoNpcsOnMapCondition condition)
    {
        chkNpc.Checked = condition.SpecificNpc;

        if (condition.SpecificNpc)
        {
            lblNpc.Show();
            cmbNpcs.Show();
            cmbNpcs.SelectedIndex = NPCDescriptor.ListIndex(condition.NpcId);

            if (cmbNpcs.SelectedIndex == -1 && cmbNpcs.Items.Count > 0)
            {
                cmbNpcs.SelectedIndex = 0;
            }
        }
        else
        {
            lblNpc.Hide();
            cmbNpcs.Hide();
        }
    }

    public void SaveFormValues(NoNpcsOnMapCondition condition)
    {
        condition.SpecificNpc = chkNpc.Checked;
        condition.NpcId = condition.SpecificNpc ? NPCDescriptor.IdFromList(cmbNpcs.SelectedIndex) : default;
    }

    public new void Show()
    {
        cmbNpcs.Items.Clear();
        cmbNpcs.Items.AddRange(NPCDescriptor.Names);

        chkNpc.Checked = false;
        cmbNpcs.Hide();
        lblNpc.Hide();
        base.Show();
    }

    private void chkNpc_CheckedChanged(object sender, EventArgs e)
    {
        if (!chkNpc.Checked)
        {
            lblNpc.Hide();
            cmbNpcs.Hide();
            return;
        }

        lblNpc.Show();
        cmbNpcs.Show();

        if (cmbNpcs.Items.Count > 0)
        {
            cmbNpcs.SelectedIndex = 0;
        }
    }
}
