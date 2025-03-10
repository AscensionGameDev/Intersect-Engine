using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;
using Intersect.Framework.Core.GameObjects.PlayerClass;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

public partial class ConditionControl_PlayerClass : UserControl
{
    public ConditionControl_PlayerClass()
    {
        InitializeComponent();
        InitLocalization();
    }

    public void InitLocalization()
    {
        grpClass.Text = Strings.EventConditional.classis;
        lblClass.Text = Strings.EventConditional.Class;
    }

    public void SetupFormValues(ClassIsCondition condition)
    {
        cmbClass.SelectedIndex = ClassDescriptor.ListIndex(condition.ClassId);

        if (cmbClass.SelectedIndex == -1 && cmbClass.Items.Count > 0)
        {
            cmbClass.SelectedIndex = 0;
        }
    }

    public void SaveFormValues(ClassIsCondition condition)
    {
        condition.ClassId = ClassDescriptor.IdFromList(cmbClass.SelectedIndex);
    }

    public new void Show()
    {
        cmbClass.Items.Clear();
        cmbClass.Items.AddRange(ClassDescriptor.Names);
        base.Show();
    }
}
