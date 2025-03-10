using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;
using Intersect.GameObjects;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

public partial class ConditionControl_QuestInProgress : UserControl
{
    public ConditionControl_QuestInProgress()
    {
        InitializeComponent();
        InitLocalization();
    }

    public void InitLocalization()
    {
        grpQuestInProgress.Text = Strings.EventConditional.questinprogress;
        lblQuestProgress.Text = Strings.EventConditional.questprogress;
        lblQuestIs.Text = Strings.EventConditional.questis;
        cmbTaskModifier.Items.Clear();
        for (var i = 0; i < Strings.EventConditional.questcomparators.Count; i++)
        {
            cmbTaskModifier.Items.Add(Strings.EventConditional.questcomparators[i]);
        }

        lblQuestTask.Text = Strings.EventConditional.task;
    }

    public void SetupFormValues(QuestInProgressCondition condition)
    {
        cmbQuestInProgress.SelectedIndex = QuestDescriptor.ListIndex(condition.QuestId);
        cmbTaskModifier.SelectedIndex = (int)condition.Progress;
        if (cmbTaskModifier.SelectedIndex == -1 && cmbTaskModifier.Items.Count > 0)
        {
            cmbTaskModifier.SelectedIndex = 0;
        }

        if (cmbTaskModifier.SelectedIndex != 0)
        {
            //Get Quest Task Here
            var quest = QuestDescriptor.Get(QuestDescriptor.IdFromList(cmbQuestInProgress.SelectedIndex));
            if (quest != null)
            {
                for (var i = 0; i < quest.Tasks.Count; i++)
                {
                    if (quest.Tasks[i].Id == condition.TaskId)
                    {
                        cmbQuestTask.SelectedIndex = i;
                    }
                }
            }
        }
    }

    public void SaveFormValues(QuestInProgressCondition condition)
    {
        condition.QuestId = QuestDescriptor.IdFromList(cmbQuestInProgress.SelectedIndex);
        condition.Progress = (QuestProgressState)cmbTaskModifier.SelectedIndex;
        condition.TaskId = Guid.Empty;

        if (cmbTaskModifier.SelectedIndex != 0)
        {
            //Get Quest Task Here
            var quest = QuestDescriptor.Get(QuestDescriptor.IdFromList(cmbQuestInProgress.SelectedIndex));
            if (quest != null)
            {
                if (cmbQuestTask.SelectedIndex > -1)
                {
                    condition.TaskId = quest.Tasks[cmbQuestTask.SelectedIndex].Id;
                }
            }
        }
    }

    public new void Show()
    {
        cmbQuestInProgress.Items.Clear();
        cmbQuestInProgress.Items.AddRange(QuestDescriptor.Names);
        base.Show();
    }

    private void cmbTaskModifier_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbTaskModifier.SelectedIndex == 0)
        {
            cmbQuestTask.Enabled = false;
        }
        else
        {
            cmbQuestTask.Enabled = true;
        }
    }

    private void cmbQuestInProgress_SelectedIndexChanged(object sender, EventArgs e)
    {
        cmbQuestTask.Items.Clear();
        var quest = QuestDescriptor.Get(QuestDescriptor.IdFromList(cmbQuestInProgress.SelectedIndex));
        if (quest != null)
        {
            foreach (var task in quest.Tasks)
            {
                cmbQuestTask.Items.Add(task.GetTaskString(Strings.TaskEditor.descriptions));
            }

            if (cmbQuestTask.Items.Count > 0)
            {
                cmbQuestTask.SelectedIndex = 0;
            }
        }
    }
}
