using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;
using Intersect.GameObjects;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

public partial class ConditionControl_QuestCanStart : UserControl
{
    public ConditionControl_QuestCanStart()
    {
        InitializeComponent();
        InitLocalization();
    }

    public void InitLocalization()
    {
        grpStartQuest.Text = Strings.EventConditional.canstartquest;
        lblStartQuest.Text = Strings.EventConditional.startquest;
    }

    public void SetupFormValues(CanStartQuestCondition condition)
    {
        cmbStartQuest.SelectedIndex = QuestDescriptor.ListIndex(condition.QuestId);

        if (cmbStartQuest.SelectedIndex == -1 && cmbStartQuest.Items.Count > 0)
        {
            cmbStartQuest.SelectedIndex = 0;
        }
    }

    public void SaveFormValues(CanStartQuestCondition condition)
    {
        condition.QuestId = QuestDescriptor.IdFromList(cmbStartQuest.SelectedIndex);
    }

    public new void Show()
    {
        cmbStartQuest.Items.Clear();
        cmbStartQuest.Items.AddRange(QuestDescriptor.Names);
        base.Show();
    }
}
