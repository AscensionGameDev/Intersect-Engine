using System;
using System.Windows.Forms;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Newtonsoft.Json;

namespace Intersect.Editor.Forms.Editors.Quest
{
    public partial class QuestTaskEditor : UserControl
    {
        private string mEventBackup = null;
        private QuestBase.QuestTask mMyTask;
        public bool Cancelled;

        public QuestTaskEditor(QuestBase.QuestTask refTask)
        {
            InitializeComponent();
            mMyTask = refTask;
            mEventBackup = mMyTask.EdittingEvent.JsonData;
            InitLocalization();
            cmbTaskType.SelectedIndex = mMyTask.Objective;
            txtStartDesc.Text = mMyTask.Desc;
            UpdateFormElements();
            switch (cmbTaskType.SelectedIndex)
            {
                case 0: //Event Driven
                    break;
                case 1: //Gather Items
                    cmbItem.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Item, mMyTask.Guid1);
                    nudItemAmount.Value = mMyTask.Data2;
                    break;
                case 2: //Kill NPCS
                    cmbNpc.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Npc, mMyTask.Guid1);
                    nudNpcQuantity.Value = mMyTask.Data2;
                    break;
            }
        }

        private void InitLocalization()
        {
            grpEditor.Text = Strings.TaskEditor.editor;

            lblType.Text = Strings.TaskEditor.type;
            cmbTaskType.Items.Clear();
            for (int i = 0; i < Strings.TaskEditor.types.Length; i++)
            {
                cmbTaskType.Items.Add(Strings.TaskEditor.types[i]);
            }

            lblDesc.Text = Strings.TaskEditor.desc;

            grpKillNpcs.Text = Strings.TaskEditor.killnpcs;
            lblNpc.Text = Strings.TaskEditor.npc;
            lblNpcQuantity.Text = Strings.TaskEditor.npcamount;

            grpGatherItems.Text = Strings.TaskEditor.gatheritems;
            lblItem.Text = Strings.TaskEditor.item;
            lblItemQuantity.Text = Strings.TaskEditor.gatheramount;

            lblEventDriven.Text = Strings.TaskEditor.eventdriven;

            btnEditTaskEvent.Text = Strings.TaskEditor.editcompletionevent;
            btnSave.Text = Strings.TaskEditor.ok;
            btnCancel.Text = Strings.TaskEditor.cancel;
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
                    mMyTask.Guid1 = Database.GameObjectIdFromList(GameObjectType.Item, cmbItem.SelectedIndex);
                    mMyTask.Data2 = (int) nudItemAmount.Value;
                    break;
                case 2: //Kill Npcs
                    mMyTask.Guid1 = Database.GameObjectIdFromList(GameObjectType.Npc, cmbNpc.SelectedIndex);
                    mMyTask.Data2 = (int) nudNpcQuantity.Value;
                    break;
            }
            ParentForm.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancelled = true;
            JsonConvert.PopulateObject(mEventBackup,mMyTask.EdittingEvent, new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });
            ParentForm.Close();
        }

        private void cmbConditionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }

        private void btnEditTaskEvent_Click(object sender, EventArgs e)
        {
            mMyTask.EdittingEvent.Name = Strings.TaskEditor.completionevent;
            FrmEvent editor = new FrmEvent(null)
            {
                MyEvent = mMyTask.EdittingEvent
            };
            editor.InitEditor();
            editor.ShowDialog();
            Globals.MainForm.BringToFront();
            BringToFront();
        }
    }
}