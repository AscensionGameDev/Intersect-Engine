using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;
using Intersect.GameObjects;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

public partial class ConditionControl_QuestCompleted : UserControl
{
    public ConditionControl_QuestCompleted()
    {
        InitializeComponent();
        InitLocalization();
    }

    public void InitLocalization()
    {
        grpQuestCompleted.Text = Strings.EventConditional.questcompleted;
        lblQuestCompleted.Text = Strings.EventConditional.questcompletedlabel;
    }

    public void SetupFormValues(QuestCompletedCondition condition)
    {
        cmbCompletedQuest.SelectedIndex = QuestDescriptor.ListIndex(condition.QuestId);

        if (cmbCompletedQuest.SelectedIndex == -1 && cmbCompletedQuest.Items.Count > 0)
        {
            cmbCompletedQuest.SelectedIndex = 0;
        }
    }

    public void SaveFormValues(QuestCompletedCondition condition)
    {
        condition.QuestId = QuestDescriptor.IdFromList(cmbCompletedQuest.SelectedIndex);
    }

    public new void Show()
    {
        cmbCompletedQuest.Items.Clear();
        cmbCompletedQuest.Items.AddRange(QuestDescriptor.Names);
        base.Show();
    }
}
