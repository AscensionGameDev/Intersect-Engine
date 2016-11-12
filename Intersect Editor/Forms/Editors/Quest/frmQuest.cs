/*
    Intersect Game Engine (Editor)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Intersect_Editor.Classes;
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

        public frmQuest()
        {
            InitializeComponent();
            PacketHandler.GameObjectUpdatedDelegate += GameObjectUpdatedDelegate;
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

        private void btnNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObject.Quest);
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (_changed.Contains(_editorItem) && _editorItem != null)
            {
                _editorItem.RestoreBackup();
                UpdateEditor();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_editorItem != null)
            {
                if (
                    MessageBox.Show(
                        "Are you sure you want to delete this game object? This action cannot be reverted!",
                        "Delete Object", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(_editorItem);
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

                ListQuestRequirements();
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

        private void btnAddRequirement_Click(object sender, EventArgs e)
        {
            var evtCommand = new EventCommand();
            evtCommand.Type = EventCommandType.ConditionalBranch;
            if (OpenRequirementEditor(evtCommand))
            {
                _editorItem.Requirements.Add(evtCommand);
                ListQuestRequirements();
            }
        }

        

        private bool OpenRequirementEditor(EventCommand cmd)
        {
            var cmdWindow = new EventCommand_ConditionalBranch(cmd, null, null);
            var frm = new Form
            {
                Text = "Add/Edit Quest Requirement"
            };
            frm.Controls.Add(cmdWindow);
            frm.Size = new Size(0, 0);
            frm.AutoSize = true;
            frm.ControlBox = false;
            frm.FormBorderStyle = FormBorderStyle.FixedSingle;
            frm.StartPosition = FormStartPosition.CenterParent;
            cmdWindow.BringToFront();
            frm.ShowDialog();
            if (!cmdWindow.Cancelled)
            {
                return true;
            }
            return false;
        }

        private void btnRemoveRequirement_Click(object sender, EventArgs e)
        {
            if (lstRequirements.SelectedIndex > -1 && _editorItem.Requirements.Count > lstRequirements.SelectedIndex)
            {
                _editorItem.Requirements.RemoveAt(lstRequirements.SelectedIndex);
                ListQuestRequirements();
            }
        }

        private void ListQuestRequirements()
        {
            lstRequirements.Items.Clear();
            foreach (var cmd in _editorItem.Requirements)
            {
                lstRequirements.Items.Add(cmd.GetConditionalDesc());
            }
        }

        private void lstRequirements_DoubleClick(object sender, EventArgs e)
        {
            if (lstRequirements.SelectedIndex > -1 && _editorItem.Requirements.Count > lstRequirements.SelectedIndex)
            {
                if (OpenRequirementEditor(_editorItem.Requirements[lstRequirements.SelectedIndex]))
                {
                    ListQuestRequirements();
                }
            }
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
            var questTask = new QuestBase.QuestTask();
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
                var taskString = "";
                switch (task.Objective)
                {
                    case 0: //Event Driven
                        taskString = "Event Driven - " + task.Desc;
                        break;
                    case 1: //Gather Items
                        taskString = "Gather Items [" + ItemBase.GetName(task.Data1) + " x" + task.Data2 + "] - " + task.Desc;
                        break;
                    case 2: //Kill Npcs
                        taskString = "Kill Npc(s) [" + NpcBase.GetName(task.Data1) + " x" + task.Data2 + "] - " + task.Desc;
                        break;
                }
                lstTasks.Items.Add(taskString);
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
            frm.FormBorderStyle = FormBorderStyle.FixedSingle;
            frm.StartPosition = FormStartPosition.CenterParent;
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
            _editorItem.Quitable = Convert.ToByte(chkRepeatable.Checked);
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

        private void lstRequirements_SelectedIndexChanged(object sender, EventArgs e)
        {

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
    }
}
