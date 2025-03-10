using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;
using Intersect.Framework.Core.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

public partial class ConditionControl_PlayerPower : UserControl
{
    public ConditionControl_PlayerPower()
    {
        InitializeComponent();
        InitLocalization();
    }

    public void InitLocalization()
    {
        grpPowerIs.Text = Strings.EventConditional.poweris;
        lblPower.Text = Strings.EventConditional.power;
        cmbPower.Items.Clear();
        cmbPower.Items.Add(Strings.EventConditional.power0);
        cmbPower.Items.Add(Strings.EventConditional.power1);
    }

    public void SetupFormValues(AccessIsCondition condition)
    {
        cmbPower.SelectedIndex = (int)condition.Access;
    }

    public void SaveFormValues(AccessIsCondition condition)
    {
        condition.Access = (Access)cmbPower.SelectedIndex;
    }
}
