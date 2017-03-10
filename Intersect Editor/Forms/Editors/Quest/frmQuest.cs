
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect_Editor.Forms.Editors;
using Intersect_Editor.Forms.Editors.Event_Commands;
using Intersect_Editor.Forms.Editors.Quest;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Events;


namespace Intersect_Editor.Forms
{
    public partial class frmQuest : Form
    {
        private List<QuestBase> _changed = new List<QuestBase>();
        private QuestBase _editorItem = null;
        private byte[] _copiedItem = null;

        public frmQuest()
        {
            InitializeComponent();
            PacketHandler.GameObjectUpdatedDelegate += GameObjectUpdatedDelegate;
            lstQuests.LostFocus += itemList_FocusChanged;
            lstQuests.GotFocus += itemList_FocusChanged;
        }

        private void GameObjectUpdatedDelegate(GameObject type)
        {
            if (type == GameObject.Quest)
            {
                InitEditor();
                if (_editorItem != null && !QuestBase.GetObjects().ContainsValue(_editorItem))
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
            _editorItem = QuestBase.GetQuest(Database.GameObjectIdFromList(GameObject.Quest, lstQuests.SelectedIndex));
            UpdateEditor();
        }

        public void InitEditor()
        {
            lstQuests.Items.Clear();
            foreach (var quest in QuestBase.GetObjects())
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
            lstQuests.Items[Database.GameObjectListIndex(GameObject.Quest,_editorItem.GetId())] =  txtName.Text;
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
            _editorItem.StartEvent.MyName = "Quest " + _editorItem.Name + " Start Event";
            openQuestEvent(_editorItem.StartEvent);
        }

        private void btnEditCompletionEvent_Click(object sender, EventArgs e)
        {
            _editorItem.EndEvent.MyName = "Quest " + _editorItem.Name + " Completion Event";
            openQuestEvent(_editorItem.EndEvent);
        }

        private void openQuestEvent(EventBase evt)
        {
            FrmEvent editor = new FrmEvent(null);
            editor.MyEvent = evt;
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
                Text = "Add/Edit Quest Task"
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
                _editorItem.Tasks.Insert(lstTasks.SelectedIndex -1,item);
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
            PacketSender.SendCreateObject(GameObject.Quest);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstQuests.Focused)
            {
                if (
                    MessageBox.Show("Are you sure you want to delete this game object? This action cannot be reverted!",
                        "Delete Object", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(_editorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstQuests.Focused)
            {
                _copiedItem = _editorItem.GetData();
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
                if (MessageBox.Show("Are you sure you want to undo changes made to this game object? This action cannot be reverted!",
                        "Undo Changes", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
                    toolStripItemDelete_Click(null,null);
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
