using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DarkUI.Forms;
using Intersect;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Localization;
using Intersect_Editor.Classes;
using Intersect_Editor.Forms.Editors;
using Intersect_Editor.Forms.Editors.Quest;

namespace Intersect_Editor.Forms
{
    public partial class frmQuest : Form
    {
        private List<QuestBase> _changed = new List<QuestBase>();
        private byte[] _copiedItem = null;
        private QuestBase _editorItem = null;

        public frmQuest()
        {
            InitializeComponent();
            PacketHandler.GameObjectUpdatedDelegate += GameObjectUpdatedDelegate;
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

        private void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.Quest)
            {
                InitEditor();
                if (_editorItem != null && !QuestBase.Lookup.Values.Contains(_editorItem))
                {
                    _editorItem = null;
                    UpdateEditor();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            foreach (var item in _changed)
            {
                item.RestoreBackup();
                item.DeleteBackup();
            }

            _editorItem = null;
            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Send Changed items
            foreach (var item in _changed)
            {
                PacketSender.SendSaveObject(item);
                item.DeleteBackup();
            }

            _editorItem = null;
            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void lstQuests_Click(object sender, EventArgs e)
        {
            _editorItem = QuestBase.Lookup.Get(Database.GameObjectIdFromList(GameObjectType.Quest, lstQuests.SelectedIndex));
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
            if (_editorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = _editorItem.Name;
                txtBeforeDesc.Text = _editorItem.BeforeDesc;
                txtStartDesc.Text = _editorItem.StartDesc;
                txtInProgressDesc.Text = _editorItem.InProgressDesc;
                txtEndDesc.Text = _editorItem.EndDesc;

                chkRepeatable.Checked = Convert.ToBoolean(_editorItem.Repeatable);
                chkQuittable.Checked = Convert.ToBoolean(_editorItem.Quitable);
                chkLogBeforeOffer.Checked = Convert.ToBoolean(_editorItem.LogBeforeOffer);
                chkLogAfterComplete.Checked = Convert.ToBoolean(_editorItem.LogAfterComplete);

                ListQuestTasks();

                if (_changed.IndexOf(_editorItem) == -1)
                {
                    _changed.Add(_editorItem);
                    _editorItem.MakeBackup();
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
            _editorItem.Name = txtName.Text;
            lstQuests.Items[Database.GameObjectListIndex(GameObjectType.Quest, _editorItem.Id)] = txtName.Text;
        }

        private void txtStartDesc_TextChanged(object sender, EventArgs e)
        {
            _editorItem.StartDesc = txtStartDesc.Text;
        }

        private void txtEndDesc_TextChanged(object sender, EventArgs e)
        {
            _editorItem.EndDesc = txtEndDesc.Text;
        }

        private void btnEditStartEvent_Click(object sender, EventArgs e)
        {
            _editorItem.StartEvent.Name = Strings.Get("questeditor", "startevent", _editorItem.Name);
            openQuestEvent(_editorItem.StartEvent);
        }

        private void btnEditCompletionEvent_Click(object sender, EventArgs e)
        {
            _editorItem.EndEvent.Name = Strings.Get("questeditor", "endevent", _editorItem.Name);
            openQuestEvent(_editorItem.EndEvent);
        }

        private void openQuestEvent(EventBase evt)
        {
            FrmEvent editor = new FrmEvent(null) {MyEvent = evt};
            editor.InitEditor();
            editor.ShowDialog();
            Globals.MainForm.BringToFront();
            BringToFront();
        }

        private void btnAddTask_Click(object sender, EventArgs e)
        {
            var questTask = new QuestBase.QuestTask(_editorItem.NextTaskID);
            _editorItem.NextTaskID = _editorItem.NextTaskID + 1;
            if (OpenTaskEditor(questTask))
            {
                _editorItem.Tasks.Add(questTask);
                ListQuestTasks();
            }
        }

        private void ListQuestTasks()
        {
            lstTasks.Items.Clear();
            foreach (var task in _editorItem.Tasks)
            {
                lstTasks.Items.Add(task.GetTaskString());
            }
        }

        private bool OpenTaskEditor(QuestBase.QuestTask task)
        {
            var cmdWindow = new Quest_TaskEditor(task);
            var frm = new Form
            {
                Text = Strings.Get("taskeditor","title")
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
                _editorItem.Tasks.RemoveAt(lstTasks.SelectedIndex);
                ListQuestTasks();
            }
        }

        private void btnShiftTaskUp_Click(object sender, EventArgs e)
        {
            if (lstTasks.SelectedIndex > 0)
            {
                var item = _editorItem.Tasks[lstTasks.SelectedIndex];
                _editorItem.Tasks.RemoveAt(lstTasks.SelectedIndex);
                _editorItem.Tasks.Insert(lstTasks.SelectedIndex - 1, item);
                ListQuestTasks();
            }
        }

        private void btnShiftTaskDown_Click(object sender, EventArgs e)
        {
            if (lstTasks.SelectedIndex > -1 && lstTasks.SelectedIndex != lstTasks.Items.Count - 1)
            {
                var item = _editorItem.Tasks[lstTasks.SelectedIndex];
                _editorItem.Tasks.RemoveAt(lstTasks.SelectedIndex);
                _editorItem.Tasks.Insert(lstTasks.SelectedIndex + 1, item);
                ListQuestTasks();
            }
        }

        private void txtInProgressDesc_TextChanged(object sender, EventArgs e)
        {
            _editorItem.InProgressDesc = txtInProgressDesc.Text;
        }

        private void chkRepeatable_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.Repeatable = Convert.ToByte(chkRepeatable.Checked);
        }

        private void chkQuittable_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.Quitable = Convert.ToByte(chkQuittable.Checked);
        }

        private void lstTasks_DoubleClick(object sender, EventArgs e)
        {
            if (lstTasks.SelectedIndex > -1 && _editorItem.Tasks.Count > lstTasks.SelectedIndex)
            {
                if (OpenTaskEditor(_editorItem.Tasks[lstTasks.SelectedIndex]))
                {
                    ListQuestTasks();
                }
            }
        }

        private void txtBeforeDesc_TextChanged(object sender, EventArgs e)
        {
            _editorItem.BeforeDesc = txtBeforeDesc.Text;
        }

        private void chkLogBeforeOffer_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.LogBeforeOffer = Convert.ToByte(chkLogBeforeOffer.Checked);
        }

        private void chkLogAfterComplete_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.LogAfterComplete = Convert.ToByte(chkLogAfterComplete.Checked);
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.Quest);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstQuests.Focused)
            {
                if (DarkMessageBox.ShowWarning(Strings.Get("questeditor", "deleteprompt"),
                        Strings.Get("questeditor", "deletetitle"), DarkDialogButton.YesNo, Properties.Resources.Icon) == DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(_editorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstQuests.Focused)
            {
                _copiedItem = _editorItem.BinaryData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && _copiedItem != null && lstQuests.Focused)
            {
                _editorItem.Load(_copiedItem);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (_changed.Contains(_editorItem) && _editorItem != null)
            {
                if (DarkMessageBox.ShowWarning(Strings.Get("questeditor", "undoprompt"),
                        Strings.Get("questeditor", "undotitle"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    _editorItem.RestoreBackup();
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
            toolStripItemCopy.Enabled = _editorItem != null && lstQuests.Focused;
            toolStripItemPaste.Enabled = _editorItem != null && _copiedItem != null && lstQuests.Focused;
            toolStripItemDelete.Enabled = _editorItem != null && lstQuests.Focused;
            toolStripItemUndo.Enabled = _editorItem != null && lstQuests.Focused;
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
            var frm = new frmDynamicRequirements(_editorItem.Requirements, RequirementType.Quest);
            frm.ShowDialog();
        }
    }
}