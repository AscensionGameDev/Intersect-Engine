using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DarkUI.Forms;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors.Quest
{
    public partial class FrmQuest : EditorForm
    {
        private List<QuestBase> mChanged = new List<QuestBase>();
        private string mCopiedItem;
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
            Text = Strings.QuestEditor.title;
            toolStripItemNew.Text = Strings.QuestEditor.New;
            toolStripItemDelete.Text = Strings.QuestEditor.delete;
            toolStripItemCopy.Text = Strings.QuestEditor.copy;
            toolStripItemPaste.Text = Strings.QuestEditor.paste;
            toolStripItemUndo.Text = Strings.QuestEditor.undo;

            grpQuests.Text = Strings.QuestEditor.quests;
            grpGeneral.Text = Strings.QuestEditor.general;
            lblName.Text = Strings.QuestEditor.name;

            grpLogOptions.Text = Strings.QuestEditor.logoptions;
            chkLogAfterComplete.Text = Strings.QuestEditor.showafter;
            chkLogBeforeOffer.Text = Strings.QuestEditor.showbefore;

            grpProgessionOptions.Text = Strings.QuestEditor.options;
            chkRepeatable.Text = Strings.QuestEditor.repeatable;
            chkQuittable.Text = Strings.QuestEditor.quit;

            lblBeforeOffer.Text = Strings.QuestEditor.beforeofferdesc;
            lblOffer.Text = Strings.QuestEditor.offerdesc;
            lblInProgress.Text = Strings.QuestEditor.inprogressdesc;
            lblCompleted.Text = Strings.QuestEditor.completeddesc;

            grpQuestReqs.Text = Strings.QuestEditor.requirements;
            btnEditRequirements.Text = Strings.QuestEditor.editrequirements;

            grpQuestTasks.Text = Strings.QuestEditor.tasks;
            btnAddTask.Text = Strings.QuestEditor.addtask;
            btnRemoveTask.Text = Strings.QuestEditor.removetask;

            grpActions.Text = Strings.QuestEditor.actions;
            lblOnStart.Text = Strings.QuestEditor.onstart;
            btnEditStartEvent.Text = Strings.QuestEditor.editstartevent;
            lblOnEnd.Text = Strings.QuestEditor.onend;
            btnEditCompletionEvent.Text = Strings.QuestEditor.editendevent;

            btnSave.Text = Strings.QuestEditor.save;
            btnCancel.Text = Strings.QuestEditor.cancel;
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
                item.StartEvent.RestoreBackup();
                item.StartEvent.DeleteBackup();
                item.EndEvent.RestoreBackup();
                item.EndEvent.DeleteBackup();
                foreach (var tsk in item.Tasks)
                {
                    if (tsk.CompletionEvent.Id != Guid.Empty)
                    {
                        tsk.CompletionEvent.RestoreBackup();
                    }
                    tsk.CompletionEvent.DeleteBackup();
                }
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
                PacketSender.SendSaveObject(item.StartEvent);
                PacketSender.SendSaveObject(item.EndEvent);
                foreach (var tsk in item.Tasks)
                {
                    if (tsk.EdittingEvent.Id != Guid.Empty)
                    {
                        PacketSender.SendSaveObject(tsk.EdittingEvent);
                    }
                    tsk.EdittingEvent.DeleteBackup();
                }
                item.StartEvent.DeleteBackup();
                item.EndEvent.DeleteBackup();
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
                QuestBase.Get(
                    QuestBase.IdFromList(lstQuests.SelectedIndex));
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
                    mEditorItem.StartEvent.MakeBackup();
                    mEditorItem.EndEvent.MakeBackup();
                    foreach (var tsk in mEditorItem.Tasks)
                    {
                        tsk.CompletionEvent.MakeBackup();
                        tsk.EdittingEvent = tsk.CompletionEvent;
                    }
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
            //Rename all events
            mEditorItem.StartEvent.Name = Strings.QuestEditor.startevent.ToString(mEditorItem.Name);
            mEditorItem.EndEvent.Name = Strings.QuestEditor.endevent.ToString(mEditorItem.Name);
            foreach (var tsk in mEditorItem.Tasks)
            {
                if (tsk.CompletionEvent != null)
                    tsk.CompletionEvent.Name = Strings.TaskEditor.completionevent.ToString(mEditorItem.Name);
            }
            lstQuests.Items[QuestBase.ListIndex(mEditorItem.Id)] = txtName.Text;
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
            mEditorItem.StartEvent.Name = Strings.QuestEditor.startevent.ToString(mEditorItem.Name);
            OpenQuestEvent(mEditorItem.StartEvent);
        }

        private void btnEditCompletionEvent_Click(object sender, EventArgs e)
        {
            mEditorItem.EndEvent.Name = Strings.QuestEditor.endevent.ToString(mEditorItem.Name);
            OpenQuestEvent(mEditorItem.EndEvent);
        }

        private void OpenQuestEvent(EventBase evt)
        {
            FrmEvent editor = new FrmEvent(null) {MyEvent = evt};
            editor.InitEditor(true,true);
            editor.ShowDialog();
            Globals.MainForm.BringToFront();
            BringToFront();
        }

        private void btnAddTask_Click(object sender, EventArgs e)
        {
            var questTask = new QuestBase.QuestTask(Guid.NewGuid());
            questTask.EdittingEvent = new EventBase(Guid.Empty, Guid.Empty,0, 0, false);
            questTask.EdittingEvent.Name = Strings.TaskEditor.completionevent.ToString(mEditorItem.Name);
            mEditorItem.AddEvents.Add(questTask.Id, questTask.EdittingEvent);
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
                lstTasks.Items.Add(task.GetTaskString(Strings.TaskEditor.descriptions));
            }
        }

        private bool OpenTaskEditor(QuestBase.QuestTask task)
        {
            var cmdWindow = new QuestTaskEditor(mEditorItem,task);
            var frm = new Form
            {
                Text = Strings.TaskEditor.title
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
                mEditorItem.RemoveEvents.Add(mEditorItem.Tasks[lstTasks.SelectedIndex].CompletionEvent.Id);
                if (mEditorItem.AddEvents.ContainsKey(mEditorItem.Tasks[lstTasks.SelectedIndex].Id))
                {
                    mEditorItem.AddEvents.Remove(mEditorItem.Tasks[lstTasks.SelectedIndex].Id);
                }
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
                if (DarkMessageBox.ShowWarning(Strings.QuestEditor.deleteprompt,
                        Strings.QuestEditor.deletetitle, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
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
                mCopiedItem = mEditorItem.JsonData;
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
                if (DarkMessageBox.ShowWarning(Strings.QuestEditor.undoprompt,
                        Strings.QuestEditor.undotitle, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
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