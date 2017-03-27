using System;
using System.Windows.Forms;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Localization;
using Intersect_Editor.Classes;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_CompleteQuestTask : UserControl
    {
        private readonly FrmEvent _eventEditor;
        private EventCommand _myCommand;

        public EventCommand_CompleteQuestTask(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            cmbQuests.Items.Clear();
            cmbQuests.Items.AddRange(Database.GetGameObjectList(GameObjectType.Quest));
            cmbQuests.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Quest, refCommand.Ints[0]);
        }

        private void InitLocalization()
        {
            grpCompleteTask.Text = Strings.Get("eventcompletequesttask", "title");
            lblQuest.Text = Strings.Get("eventcompletequesttask", "quest");
            lblTask.Text = Strings.Get("eventcompletequesttask", "task");
            btnSave.Text = Strings.Get("eventcompletequesttask", "okay");
            btnCancel.Text = Strings.Get("eventcompletequesttask", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Ints[0] = Database.GameObjectIdFromList(GameObjectType.Quest, cmbQuests.SelectedIndex);
            _myCommand.Ints[1] = -1;
            if (cmbQuests.SelectedIndex > -1)
            {
                var quest = QuestBase.Lookup.Get(Database.GameObjectIdFromList(GameObjectType.Quest, cmbQuests.SelectedIndex));
                if (quest != null)
                {
                    var i = -1;
                    foreach (var task in quest.Tasks)
                    {
                        i++;
                        if (i == cmbQuestTask.SelectedIndex)
                        {
                            _myCommand.Ints[1] = task.Id;
                        }
                    }
                }
            }
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }

        private void cmbQuests_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbQuestTask.Hide();
            lblTask.Hide();
            if (cmbQuests.SelectedIndex > -1)
            {
                var quest = QuestBase.Lookup.Get(Database.GameObjectIdFromList(GameObjectType.Quest, cmbQuests.SelectedIndex));
                if (quest != null)
                {
                    lblTask.Show();
                    cmbQuestTask.Show();
                    cmbQuestTask.Items.Clear();
                    foreach (var task in quest.Tasks)
                    {
                        cmbQuestTask.Items.Add(task.GetTaskString());
                        if (task.Id == _myCommand.Ints[1])
                        {
                            cmbQuestTask.SelectedIndex = cmbQuestTask.Items.Count - 1;
                        }
                    }
                }
            }
        }
    }
}