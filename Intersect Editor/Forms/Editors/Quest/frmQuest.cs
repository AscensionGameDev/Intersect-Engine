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
using Intersect.Editor.Classes.Localization;

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
            Text = Strings.questeditor.title;
            toolStripItemNew.Text = Strings.questeditor.New;
            toolStripItemDelete.Text = Strings.questeditor.delete;
            toolStripItemCopy.Text = Strings.questeditor.copy;
            toolStripItemPaste.Text = Strings.questeditor.paste;
            toolStripItemUndo.Text = Strings.questeditor.undo;

            grpQuests.Text = Strings.questeditor.quests;
            grpGeneral.Text = Strings.questeditor.general;
            lblName.Text = Strings.questeditor.name;

            grpLogOptions.Text = Strings.questeditor.logoptions;
            chkLogAfterComplete.Text = Strings.questeditor.showafter;
            chkLogBeforeOffer.Text = Strings.questeditor.showbefore;

            grpProgessionOptions.Text = Strings.questeditor.options;
            chkRepeatable.Text = Strings.questeditor.repeatable;
            chkQuittable.Text = Strings.questeditor.quit;

            lblBeforeOffer.Text = Strings.questeditor.beforeofferdesc;
            lblOffer.Text = Strings.questeditor.offerdesc;
            lblInProgress.Text = Strings.questeditor.inprogressdesc;
            lblCompleted.Text = Strings.questeditor.completeddesc;

            grpQuestReqs.Text = Strings.questeditor.requirements;
            btnEditRequirements.Text = Strings.questeditor.editrequirements;

            grpQuestTasks.Text = Strings.questeditor.tasks;
            btnAddTask.Text = Strings.questeditor.addtask;
            btnRemoveTask.Text = Strings.questeditor.removetask;

            grpActions.Text = Strings.questeditor.actions;
            lblOnStart.Text = Strings.questeditor.onstart;
            btnEditStartEvent.Text = Strings.questeditor.editstartevent;
            lblOnEnd.Text = Strings.questeditor.onend;
            btnEditCompletionEvent.Text = Strings.questeditor.editendevent;

            btnSave.Text = Strings.questeditor.save;
            btnCancel.Text = Strings.questeditor.cancel;
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
            mEditorItem.StartEvent.Name = Strings.questeditor.startevent.ToString(mEditorItem.Name);
            OpenQuestEvent(mEditorItem.StartEvent);
        }

        private void btnEditCompletionEvent_Click(object sender, EventArgs e)
        {
            mEditorItem.EndEvent.Name = Strings.questeditor.endevent.ToString(mEditorItem.Name);
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
                Text = Strings.taskeditor.title
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
                if (DarkMessageBox.ShowWarning(Strings.questeditor.deleteprompt,
                        Strings.questeditor.deletetitle, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
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
                if (DarkMessageBox.ShowWarning(Strings.questeditor.undoprompt,
                        Strings.questeditor.undotitle, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
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