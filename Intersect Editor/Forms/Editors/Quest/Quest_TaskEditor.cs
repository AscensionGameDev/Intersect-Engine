using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect.Editor.Forms.Editors.Quest
{
    public partial class Quest_TaskEditor : UserControl
    {
        private ByteBuffer _eventBackup = new ByteBuffer();
        private QuestBase.QuestTask _myTask;
        public bool Cancelled = false;

        public Quest_TaskEditor(QuestBase.QuestTask refTask)
        {
            InitializeComponent();
            _myTask = refTask;
            _eventBackup.WriteBytes(_myTask.CompletionEvent.EventData());
            cmbTaskType.SelectedIndex = _myTask.Objective;
            txtStartDesc.Text = _myTask.Desc;
            UpdateFormElements();
            switch (cmbTaskType.SelectedIndex)
            {
                case 0: //Event Driven
                    break;
                case 1: //Gather Items
                    cmbItem.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Item, _myTask.Data1);
                    nudItemAmount.Value = _myTask.Data2;
                    break;
                case 2: //Kill NPCS
                    cmbNpc.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Item, _myTask.Data1);
                    nudNpcQuantity.Value = _myTask.Data2;
                    break;
            }
            InitLocalization();
        }

        private void InitLocalization()
        {
            grpEditor.Text = Strings.Get("taskeditor", "editor");

            lblType.Text = Strings.Get("taskeditor", "type");
            cmbTaskType.Items.Clear();
            for (int i = 0; i < 3; i++)
            {
                cmbTaskType.Items.Add(Strings.Get("taskeditor", "type" + i));
            }

            lblDesc.Text = Strings.Get("taskeditor", "desc");

            grpKillNpcs.Text = Strings.Get("taskeditor", "killnpcs");
            lblNpc.Text = Strings.Get("taskeditor", "npc");
            lblNpcQuantity.Text = Strings.Get("taskeditor", "npcamount");

            grpGatherItems.Text = Strings.Get("taskeditor", "gatheritems");
            lblItem.Text = Strings.Get("taskeditor", "item");
            lblItemQuantity.Text = Strings.Get("taskeditor", "gatheramount");

            lblEventDriven.Text = Strings.Get("taskeditor", "eventdriven");

            btnEditTaskEvent.Text = Strings.Get("taskeditor", "editcompletionevent");
            btnSave.Text = Strings.Get("taskeditor", "ok");
            btnCancel.Text = Strings.Get("taskeditor", "cancel");

        }

        private void UpdateFormElements()
        {
            grpGatherItems.Hide();
            grpKillNpcs.Hide();
            switch (cmbTaskType.SelectedIndex)
            {
                case 0: //Event Driven
                    break;
                case 1: //Gather Items
                    grpGatherItems.Show();
                    cmbItem.Items.Clear();
                    cmbItem.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));
                    if (cmbItem.Items.Count > 0) cmbItem.SelectedIndex = 0;
                    nudItemAmount.Value = 1;
                    break;
                case 2: //Kill Npcs
                    grpKillNpcs.Show();
                    cmbNpc.Items.Clear();
                    cmbNpc.Items.AddRange(Database.GetGameObjectList(GameObjectType.Npc));
                    if (cmbNpc.Items.Count > 0) cmbNpc.SelectedIndex = 0;
                    nudNpcQuantity.Value = 1;
                    break;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myTask.Objective = cmbTaskType.SelectedIndex;
            _myTask.Desc = txtStartDesc.Text;
            switch (_myTask.Objective)
            {
                case 0: //Event Driven
                    _myTask.Data1 = 0;
                    _myTask.Data2 = 1;
                    break;
                case 1: //Gather Items
                    _myTask.Data1 = Database.GameObjectIdFromList(GameObjectType.Item, cmbItem.SelectedIndex);
                    _myTask.Data2 = (int)nudItemAmount.Value;
                    break;
                case 2: //Kill Npcs
                    _myTask.Data1 = Database.GameObjectIdFromList(GameObjectType.Npc, cmbNpc.SelectedIndex);
                    _myTask.Data2 = (int)nudNpcQuantity.Value;
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

        private void btnEditTaskEvent_Click(object sender, EventArgs e)
        {
            _myTask.CompletionEvent.Name = Strings.Get("taskeditor","completionevent");
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