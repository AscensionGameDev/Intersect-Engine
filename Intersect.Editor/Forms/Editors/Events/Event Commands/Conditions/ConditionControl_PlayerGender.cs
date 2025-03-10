using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

public partial class ConditionControl_PlayerGender : UserControl
{
    public ConditionControl_PlayerGender()
    {
        InitializeComponent();
        InitLocalization();
    }

    public void InitLocalization()
    {
        grpGender.Text = Strings.EventConditional.genderis;
        lblGender.Text = Strings.EventConditional.gender;
        cmbGender.Items.Clear();
        cmbGender.Items.Add(Strings.EventConditional.male);
        cmbGender.Items.Add(Strings.EventConditional.female);
    }

    public void SetupFormValues(GenderIsCondition condition)
    {
        cmbGender.SelectedIndex = (int)condition.Gender;
    }

    public void SaveFormValues(GenderIsCondition condition)
    {
        condition.Gender = (Gender)cmbGender.SelectedIndex;
    }
}
