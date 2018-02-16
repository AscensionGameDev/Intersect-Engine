using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Editor.Classes.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandCompleteQuestTask : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventCommand mMyCommand;

        public EventCommandCompleteQuestTask(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            cmbQuests.Items.Clear();
            cmbQuests.Items.AddRange(Database.GetGameObjectList(GameObjectType.Quest));
            cmbQuests.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Quest, refCommand.Ints[0]);
        }

        private void InitLocalization()
        {
            grpCompleteTask.Text = Strings.EventCompleteQuestTask.title;
            lblQuest.Text = Strings.EventCompleteQuestTask.quest;
            lblTask.Text = Strings.EventCompleteQuestTask.task;
            btnSave.Text = Strings.EventCompleteQuestTask.okay;
            btnCancel.Text = Strings.EventCompleteQuestTask.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Ints[0] = Database.GameObjectIdFromList(GameObjectType.Quest, cmbQuests.SelectedIndex);
            mMyCommand.Ints[1] = -1;
            if (cmbQuests.SelectedIndex > -1)
            {
                var quest = QuestBase.Lookup.Get<QuestBase>(
                    Database.GameObjectIdFromList(GameObjectType.Quest, cmbQuests.SelectedIndex));
                if (quest != null)
                {
                    var i = -1;
                    foreach (var task in quest.Tasks)
                    {
                        i++;
                        if (i == cmbQuestTask.SelectedIndex)
                        {
                            mMyCommand.Ints[1] = task.Id;
                        }
                    }
                }
            }
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

        private void cmbQuests_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbQuestTask.Hide();
            lblTask.Hide();
            if (cmbQuests.SelectedIndex > -1)
            {
                var quest = QuestBase.Lookup.Get<QuestBase>(
                    Database.GameObjectIdFromList(GameObjectType.Quest, cmbQuests.SelectedIndex));
                if (quest != null)
                {
                    lblTask.Show();
                    cmbQuestTask.Show();
                    cmbQuestTask.Items.Clear();
                    foreach (var task in quest.Tasks)
                    {
                        cmbQuestTask.Items.Add(task.GetTaskString(Strings.TaskEditor.descriptions));
                        if (task.Id == mMyCommand.Ints[1])
                        {
                            cmbQuestTask.SelectedIndex = cmbQuestTask.Items.Count - 1;
                        }
                    }
                }
            }
        }
    }
}