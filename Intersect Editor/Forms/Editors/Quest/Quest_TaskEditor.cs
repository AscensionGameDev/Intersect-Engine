using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect.Editor.Forms.Editors.Quest
{
    public partial class QuestTaskEditor : UserControl
    {
        private ByteBuffer mEventBackup = new ByteBuffer();
        private QuestBase.QuestTask mMyTask;
        public bool Cancelled;

        public QuestTaskEditor(QuestBase.QuestTask refTask)
        {
            InitializeComponent();
            mMyTask = refTask;
            mEventBackup.WriteBytes(mMyTask.CompletionEvent.EventData());
            InitLocalization();
            cmbTaskType.SelectedIndex = mMyTask.Objective;
            txtStartDesc.Text = mMyTask.Desc;
            UpdateFormElements();
            switch (cmbTaskType.SelectedIndex)
            {
                case 0: //Event Driven
                    break;
                case 1: //Gather Items
                    cmbItem.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Item, mMyTask.Data1);
                    nudItemAmount.Value = mMyTask.Data2;
                    break;
                case 2: //Kill NPCS
                    cmbNpc.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Npc, mMyTask.Data1);
                    nudNpcQuantity.Value = mMyTask.Data2;
                    break;
            }
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
            mMyTask.Objective = cmbTaskType.SelectedIndex;
            mMyTask.Desc = txtStartDesc.Text;
            switch (mMyTask.Objective)
            {
                case 0: //Event Driven
                    mMyTask.Data1 = 0;
                    mMyTask.Data2 = 1;
                    break;
                case 1: //Gather Items
                    mMyTask.Data1 = Database.GameObjectIdFromList(GameObjectType.Item, cmbItem.SelectedIndex);
                    mMyTask.Data2 = (int) nudItemAmount.Value;
                    break;
                case 2: //Kill Npcs
                    mMyTask.Data1 = Database.GameObjectIdFromList(GameObjectType.Npc, cmbNpc.SelectedIndex);
                    mMyTask.Data2 = (int) nudNpcQuantity.Value;
                    break;
            }
            ParentForm.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancelled = true;
            mMyTask.CompletionEvent = new EventBase(-1, mEventBackup);
            ParentForm.Close();
        }

        private void cmbConditionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }

        private void btnEditTaskEvent_Click(object sender, EventArgs e)
        {
            mMyTask.CompletionEvent.Name = Strings.Get("taskeditor", "completionevent");
            FrmEvent editor = new FrmEvent(null)
            {
                MyEvent = mMyTask.CompletionEvent
            };
            editor.InitEditor();
            editor.ShowDialog();
            Globals.MainForm.BringToFront();
            BringToFront();
        }
    }
}