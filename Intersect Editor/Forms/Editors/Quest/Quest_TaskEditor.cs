
using System;
using System.Windows.Forms;
using DarkUI.Controls;
using Intersect_Editor.Classes;
using Intersect;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;

namespace Intersect_Editor.Forms.Editors.Quest
{
    public partial class Quest_TaskEditor : UserControl
    {
        private QuestBase.QuestTask _myTask;
        public bool Cancelled = false;
        private ByteBuffer _eventBackup = new ByteBuffer();
        public Quest_TaskEditor(QuestBase.QuestTask refTask)
        {
            InitializeComponent();
            _myTask = refTask;
            _eventBackup.WriteBytes(_myTask.CompletionEvent.EventData());
            cmbConditionType.SelectedIndex = _myTask.Objective;
            txtStartDesc.Text = _myTask.Desc;
            UpdateFormElements();
            switch (cmbConditionType.SelectedIndex)
            {
                case 0: //Event Driven
                    break;
                case 1: //Gather Items
                    cmbItem.SelectedIndex = Database.GameObjectListIndex(GameObject.Item, _myTask.Data1);
                    scrlItemQuantity.Value = _myTask.Data2;
                    lblItemQuantity.Text = "Amount: " + scrlItemQuantity.Value;
                    break;
                case 2: //Kill NPCS
                    cmbNpc.SelectedIndex = Database.GameObjectListIndex(GameObject.Item, _myTask.Data1);
                    scrlNpcQuantity.Value = _myTask.Data2;
                    lblNpcQuantity.Text = "Amount: " + scrlNpcQuantity.Value;
                    break;
            }
        }

        private void UpdateFormElements()
        {
            grpGatherItems.Hide();
            grpKillNpcs.Hide();
            switch (cmbConditionType.SelectedIndex)
            {
                case 0: //Event Driven
                    break;
                case 1: //Gather Items
                    grpGatherItems.Show();
                    cmbItem.Items.Clear();
                    cmbItem.Items.AddRange(Database.GetGameObjectList(GameObject.Item));
                    if (cmbItem.Items.Count > 0) cmbItem.SelectedIndex = 0;
                    scrlItemQuantity.Value = 1;
                    break;
                case 2: //Kill Npcs
                    grpKillNpcs.Show();
                    cmbNpc.Items.Clear();
                    cmbNpc.Items.AddRange(Database.GetGameObjectList(GameObject.Npc));
                    if (cmbNpc.Items.Count > 0) cmbNpc.SelectedIndex = 0;
                    scrlNpcQuantity.Value = 1;
                    break;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myTask.Objective = cmbConditionType.SelectedIndex;
            _myTask.Desc = txtStartDesc.Text;
            switch (_myTask.Objective)
            {
                case 0: //Event Driven
                    _myTask.Data1 = 0;
                    _myTask.Data2 = 1;
                    break;
                case 1: //Gather Items
                    _myTask.Data1 = Database.GameObjectIdFromList(GameObject.Item, cmbItem.SelectedIndex);
                    _myTask.Data2 = scrlItemQuantity.Value;
                    break;
                case 2: //Kill Npcs
                    _myTask.Data1 = Database.GameObjectIdFromList(GameObject.Npc, cmbNpc.SelectedIndex);
                    _myTask.Data2 = scrlNpcQuantity.Value;
                    break;
            }
            ParentForm.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancelled = true;
            _myTask.CompletionEvent = new EventBase(-1, _eventBackup);
            ParentForm.Close();
        }

        private void cmbConditionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }

        private void scrlItemQuantity_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblItemQuantity.Text = "Amount: " + scrlItemQuantity.Value;
        }

        private void scrlNpcQuantity_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblNpcQuantity.Text = "Amount: " + scrlNpcQuantity.Value;
        }

        private void btnEditTaskEvent_Click(object sender, EventArgs e)
        {
            _myTask.CompletionEvent.Name = "Task Completion Event";
            FrmEvent editor = new FrmEvent(null)
            {
                MyEvent = _myTask.CompletionEvent
            };
            editor.InitEditor();
            editor.ShowDialog();
            Globals.MainForm.BringToFront();
            BringToFront();
        }
    }
}
