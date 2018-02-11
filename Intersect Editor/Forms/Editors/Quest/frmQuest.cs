using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DarkUI.Forms;
using Intersect.Editor.Classes;
using Intersect.Editor.Forms.Editors;
using Intersect.Editor.Forms.Editors.Quest;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect.Editor.Forms
{
    public partial class FrmQuest : EditorForm
    {
        private List<QuestBase> mChanged = new List<QuestBase>();
        private byte[] mCopiedItem;
        private QuestBase mEditorItem;

        public FrmQuest()
        {
            ApplyHooks();
            InitializeComponent();
            lstQuests.LostFocus += itemList_FocusChanged;
            lstQuests.GotFocus += itemList_FocusChanged;
            InitLocalization();
        }

        private void InitLocalization()
        {
            Text = Strings.Get("questeditor", "title");
            toolStripItemNew.Text = Strings.Get("questeditor", "new");
            toolStripItemDelete.Text = Strings.Get("questeditor", "delete");
            toolStripItemCopy.Text = Strings.Get("questeditor", "copy");
            toolStripItemPaste.Text = Strings.Get("questeditor", "paste");
            toolStripItemUndo.Text = Strings.Get("questeditor", "undo");

            grpQuests.Text = Strings.Get("questeditor", "quests");
            grpGeneral.Text = Strings.Get("questeditor", "general");
            lblName.Text = Strings.Get("questeditor", "name");

            grpLogOptions.Text = Strings.Get("questeditor", "logoptions");
            chkLogAfterComplete.Text = Strings.Get("questeditor", "showafter");
            chkLogBeforeOffer.Text = Strings.Get("questeditor", "showbefore");

            grpProgessionOptions.Text = Strings.Get("questeditor", "options");
            chkRepeatable.Text = Strings.Get("questeditor", "repeatable");
            chkQuittable.Text = Strings.Get("questeditor", "quit");

            lblBeforeOffer.Text = Strings.Get("questeditor", "beforeofferdesc");
            lblOffer.Text = Strings.Get("questeditor", "offerdesc");
            lblInProgress.Text = Strings.Get("questeditor", "inprogressdesc");
            lblCompleted.Text = Strings.Get("questeditor", "completeddesc");

            grpQuestReqs.Text = Strings.Get("questeditor", "requirements");
            btnEditRequirements.Text = Strings.Get("questeditor", "editrequirements");

            grpQuestTasks.Text = Strings.Get("questeditor", "tasks");
            btnAddTask.Text = Strings.Get("questeditor", "addtask");
            btnRemoveTask.Text = Strings.Get("questeditor", "removetask");

            grpActions.Text = Strings.Get("questeditor", "actions");
            lblOnStart.Text = Strings.Get("questeditor", "onstart");
            btnEditStartEvent.Text = Strings.Get("questeditor", "editstartevent");
            lblOnEnd.Text = Strings.Get("questeditor", "onend");
            btnEditCompletionEvent.Text = Strings.Get("questeditor", "editendevent");

            btnSave.Text = Strings.Get("questeditor", "save");
            btnCancel.Text = Strings.Get("questeditor", "cancel");
        }

        protected override void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.Quest)
            {
                InitEditor();
                if (mEditorItem != null && !QuestBase.Lookup.Values.Contains(mEditorItem))
                {
                    mEditorItem = null;
                    UpdateEditor();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            foreach (var item in mChanged)
            {
                item.RestoreBackup();
                item.DeleteBackup();
            }

            mEditorItem = null;
            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Send Changed items
            foreach (var item in mChanged)
            {
                PacketSender.SendSaveObject(item);
                item.DeleteBackup();
            }

            mEditorItem = null;
            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void lstQuests_Click(object sender, EventArgs e)
        {
            if (mChangingName) return;
            mEditorItem =
                QuestBase.Lookup.Get<QuestBase>(
                    Database.GameObjectIdFromList(GameObjectType.Quest, lstQuests.SelectedIndex));
            UpdateEditor();
        }

        public void InitEditor()
        {
            lstQuests.Items.Clear();
            foreach (var quest in QuestBase.Lookup)
            {
                lstQuests.Items.Add(quest.Value.Name);
            }

            UpdateEditor();
        }

        private void UpdateEditor()
        {
            if (mEditorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = mEditorItem.Name;
                txtBeforeDesc.Text = mEditorItem.BeforeDesc;
                txtStartDesc.Text = mEditorItem.StartDesc;
                txtInProgressDesc.Text = mEditorItem.InProgressDesc;
                txtEndDesc.Text = mEditorItem.EndDesc;

                chkRepeatable.Checked = Convert.ToBoolean(mEditorItem.Repeatable);
                chkQuittable.Checked = Convert.ToBoolean(mEditorItem.Quitable);
                chkLogBeforeOffer.Checked = Convert.ToBoolean(mEditorItem.LogBeforeOffer);
                chkLogAfterComplete.Checked = Convert.ToBoolean(mEditorItem.LogAfterComplete);

                ListQuestTasks();

                if (mChanged.IndexOf(mEditorItem) == -1)
                {
                    mChanged.Add(mEditorItem);
                    mEditorItem.MakeBackup();
                }
            }
            else
            {
                pnlContainer.Hide();
            }
            UpdateToolStripItems();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            mChangingName = true;
            mEditorItem.Name = txtName.Text;
            lstQuests.Items[Database.GameObjectListIndex(GameObjectType.Quest, mEditorItem.Index)] = txtName.Text;
            mChangingName = false;
        }

        private void txtStartDesc_TextChanged(object sender, EventArgs e)
        {
            mEditorItem.StartDesc = txtStartDesc.Text;
        }

        private void txtEndDesc_TextChanged(object sender, EventArgs e)
        {
            mEditorItem.EndDesc = txtEndDesc.Text;
        }

        private void btnEditStartEvent_Click(object sender, EventArgs e)
        {
            mEditorItem.StartEvent.Name = Strings.Get("questeditor", "startevent", mEditorItem.Name);
            OpenQuestEvent(mEditorItem.StartEvent);
        }

        private void btnEditCompletionEvent_Click(object sender, EventArgs e)
        {
            mEditorItem.EndEvent.Name = Strings.Get("questeditor", "endevent", mEditorItem.Name);
            OpenQuestEvent(mEditorItem.EndEvent);
        }

        private void OpenQuestEvent(EventBase evt)
        {
            FrmEvent editor = new FrmEvent(null) {MyEvent = evt};
            editor.InitEditor();
            editor.ShowDialog();
            Globals.MainForm.BringToFront();
            BringToFront();
        }

        private void btnAddTask_Click(object sender, EventArgs e)
        {
            var questTask = new QuestBase.QuestTask(mEditorItem.NextTaskId);
            mEditorItem.NextTaskId = mEditorItem.NextTaskId + 1;
            if (OpenTaskEditor(questTask))
            {
                mEditorItem.Tasks.Add(questTask);
                ListQuestTasks();
            }
        }

        private void ListQuestTasks()
        {
            lstTasks.Items.Clear();
            foreach (var task in mEditorItem.Tasks)
            {
                lstTasks.Items.Add(task.GetTaskString());
            }
        }

        private bool OpenTaskEditor(QuestBase.QuestTask task)
        {
            var cmdWindow = new QuestTaskEditor(task);
            var frm = new Form
            {
                Text = Strings.Get("taskeditor", "title")
            };
            frm.Controls.Add(cmdWindow);
            frm.Size = new Size(0, 0);
            frm.AutoSize = true;
            frm.ControlBox = false;
            frm.FormBorderStyle = FormBorderStyle.FixedDialog;
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.BackColor = cmdWindow.BackColor;
            cmdWindow.BringToFront();
            frm.ShowDialog();
            if (!cmdWindow.Cancelled)
            {
                return true;
            }
            return false;
        }

        private void btnRemoveTask_Click(object sender, EventArgs e)
        {
            if (lstTasks.SelectedIndex > -1)
            {
                mEditorItem.Tasks.RemoveAt(lstTasks.SelectedIndex);
                ListQuestTasks();
            }
        }

        private void btnShiftTaskUp_Click(object sender, EventArgs e)
        {
            if (lstTasks.SelectedIndex > 0)
            {
                var item = mEditorItem.Tasks[lstTasks.SelectedIndex];
                mEditorItem.Tasks.RemoveAt(lstTasks.SelectedIndex);
                mEditorItem.Tasks.Insert(lstTasks.SelectedIndex - 1, item);
                ListQuestTasks();
            }
        }

        private void btnShiftTaskDown_Click(object sender, EventArgs e)
        {
            if (lstTasks.SelectedIndex > -1 && lstTasks.SelectedIndex != lstTasks.Items.Count - 1)
            {
                var item = mEditorItem.Tasks[lstTasks.SelectedIndex];
                mEditorItem.Tasks.RemoveAt(lstTasks.SelectedIndex);
                mEditorItem.Tasks.Insert(lstTasks.SelectedIndex + 1, item);
                ListQuestTasks();
            }
        }

        private void txtInProgressDesc_TextChanged(object sender, EventArgs e)
        {
            mEditorItem.InProgressDesc = txtInProgressDesc.Text;
        }

        private void chkRepeatable_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Repeatable = Convert.ToByte(chkRepeatable.Checked);
        }

        private void chkQuittable_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Quitable = Convert.ToByte(chkQuittable.Checked);
        }

        private void lstTasks_DoubleClick(object sender, EventArgs e)
        {
            if (lstTasks.SelectedIndex > -1 && mEditorItem.Tasks.Count > lstTasks.SelectedIndex)
            {
                if (OpenTaskEditor(mEditorItem.Tasks[lstTasks.SelectedIndex]))
                {
                    ListQuestTasks();
                }
            }
        }

        private void txtBeforeDesc_TextChanged(object sender, EventArgs e)
        {
            mEditorItem.BeforeDesc = txtBeforeDesc.Text;
        }

        private void chkLogBeforeOffer_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.LogBeforeOffer = Convert.ToByte(chkLogBeforeOffer.Checked);
        }

        private void chkLogAfterComplete_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.LogAfterComplete = Convert.ToByte(chkLogAfterComplete.Checked);
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.Quest);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstQuests.Focused)
            {
                if (DarkMessageBox.ShowWarning(Strings.Get("questeditor", "deleteprompt"),
                        Strings.Get("questeditor", "deletetitle"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(mEditorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstQuests.Focused)
            {
                mCopiedItem = mEditorItem.BinaryData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && mCopiedItem != null && lstQuests.Focused)
            {
                mEditorItem.Load(mCopiedItem);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (mChanged.Contains(mEditorItem) && mEditorItem != null)
            {
                if (DarkMessageBox.ShowWarning(Strings.Get("questeditor", "undoprompt"),
                        Strings.Get("questeditor", "undotitle"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    mEditorItem.RestoreBackup();
                    UpdateEditor();
                }
            }
        }

        private void itemList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.Z)
                {
                    toolStripItemUndo_Click(null, null);
                }
                else if (e.KeyCode == Keys.V)
                {
                    toolStripItemPaste_Click(null, null);
                }
                else if (e.KeyCode == Keys.C)
                {
                    toolStripItemCopy_Click(null, null);
                }
            }
            else
            {
                if (e.KeyCode == Keys.Delete)
                {
                    toolStripItemDelete_Click(null, null);
                }
            }
        }

        private void UpdateToolStripItems()
        {
            toolStripItemCopy.Enabled = mEditorItem != null && lstQuests.Focused;
            toolStripItemPaste.Enabled = mEditorItem != null && mCopiedItem != null && lstQuests.Focused;
            toolStripItemDelete.Enabled = mEditorItem != null && lstQuests.Focused;
            toolStripItemUndo.Enabled = mEditorItem != null && lstQuests.Focused;
        }

        private void itemList_FocusChanged(object sender, EventArgs e)
        {
            UpdateToolStripItems();
        }

        private void form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.N)
                {
                    toolStripItemNew_Click(null, null);
                }
            }
        }

        private void btnEditRequirements_Click(object sender, EventArgs e)
        {
            var frm = new FrmDynamicRequirements(mEditorItem.Requirements, RequirementType.Quest);
            frm.ShowDialog();
        }
    }
}