using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

public partial class ConditionControl_SelfSwitch : UserControl
{
    public ConditionControl_SelfSwitch()
    {
        InitializeComponent();
        InitLocalization();
    }

    public void InitLocalization()
    {
        grpSelfSwitch.Text = Strings.EventConditional.selfswitchis;
        lblSelfSwitch.Text = Strings.EventConditional.selfswitch;
        lblSelfSwitchIs.Text = Strings.EventConditional.switchis;

        cmbSelfSwitch.Items.Clear();
        for (var i = 0; i < 4; i++)
        {
            cmbSelfSwitch.Items.Add(Strings.EventConditional.selfswitches[i]);
        }

        cmbSelfSwitchVal.Items.Clear();
        cmbSelfSwitchVal.Items.Add(Strings.EventConditional.False);
        cmbSelfSwitchVal.Items.Add(Strings.EventConditional.True);
    }

    public void SetupFormValues(SelfSwitchCondition condition)
    {
        cmbSelfSwitch.SelectedIndex = condition.SwitchIndex;
        cmbSelfSwitchVal.SelectedIndex = Convert.ToInt32(condition.Value);
    }

    public void SaveFormValues(SelfSwitchCondition condition)
    {
        condition.SwitchIndex = cmbSelfSwitch.SelectedIndex;
        condition.Value = Convert.ToBoolean(cmbSelfSwitchVal.SelectedIndex);
    }
}
