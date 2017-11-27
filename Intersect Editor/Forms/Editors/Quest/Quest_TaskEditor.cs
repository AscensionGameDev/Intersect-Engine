using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Editor.Classes.Localization;

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
            grpEditor.Text = Strings.taskeditor.editor;

            lblType.Text = Strings.taskeditor.type;
            cmbTaskType.Items.Clear();
            for (int i = 0; i < Strings.taskeditor.types.Length; i++)
            {
                cmbTaskType.Items.Add(Strings.taskeditor.types[i]);
            }

            lblDesc.Text = Strings.taskeditor.desc;

            grpKillNpcs.Text = Strings.taskeditor.killnpcs;
            lblNpc.Text = Strings.taskeditor.npc;
            lblNpcQuantity.Text = Strings.taskeditor.npcamount;

            grpGatherItems.Text = Strings.taskeditor.gatheritems;
            lblItem.Text = Strings.taskeditor.item;
            lblItemQuantity.Text = Strings.taskeditor.gatheramount;

            lblEventDriven.Text = Strings.taskeditor.eventdriven;

            btnEditTaskEvent.Text = Strings.taskeditor.editcompletionevent;
            btnSave.Text = Strings.taskeditor.ok;
            btnCancel.Text = Strings.taskeditor.cancel;
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
            mMyTask.CompletionEvent.Name = Strings.taskeditor.completionevent;
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