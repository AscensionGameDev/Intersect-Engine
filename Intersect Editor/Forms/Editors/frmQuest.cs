/*
    Intersect Game Engine (Server)
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
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect_Library;
using Intersect_Library.GameObjects;


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

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void lstQuests_Click(object sender, EventArgs e)
        {
            _editorItem = QuestBase.GetQuest(Database.GameObjectIdFromList(GameObject.Quest, lstQuests.SelectedIndex));
            UpdateEditor();
        }

        private void frmQuest_Load(object sender, EventArgs e)
        {
            scrlItem.Maximum = ItemBase.ObjectCount();
            scrlLevel.Maximum = Options.MaxLevel;
            scrlQuest.Maximum = QuestBase.ObjectCount();
            scrlSwitch.Maximum = PlayerSwitchBase.ObjectCount();
            scrlVariable.Maximum = PlayerVariableBase.ObjectCount();
            scrlItemReward.Maximum = ItemBase.ObjectCount();

            lstQuests.SelectedIndex = 0;
            scrlTask.Value = 1;
        }

        public void InitEditor()
        {
            foreach (var quest in QuestBase.GetObjects())
            {
                lstQuests.Items.Add(quest.Value.Name);
            }
            cmbClass.Items.Clear();
            cmbClass.Items.Add("None");
            foreach (var cls in ClassBase.GetObjects())
            {
                cmbClass.Items.Add(cls.Value.Name);
            }
            cmbClass.SelectedIndex = 0;

            UpdateEditor();
        }

        private void UpdateEditor()
        {
            if (_editorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = _editorItem.Name;
                txtStartDesc.Text = _editorItem.StartDesc;
                txtEndDesc.Text = _editorItem.EndDesc;

                cmbClass.SelectedIndex = Database.GameObjectListIndex(GameObject.Class, _editorItem.ClassReq);
                scrlItem.Value = Database.GameObjectListIndex(GameObject.Item, _editorItem.ItemReq);
                scrlLevel.Value = _editorItem.LevelReq;
                scrlQuest.Value = Database.GameObjectListIndex(GameObject.Quest, _editorItem.QuestReq);
                scrlSwitch.Value = Database.GameObjectListIndex(GameObject.PlayerSwitch, _editorItem.SwitchReq);
                scrlVariable.Value = Database.GameObjectListIndex(GameObject.PlayerVariable, _editorItem.VariableReq);
                scrlVariableValue.Value = _editorItem.VariableValue;

                scrlTask.Value = 0;
                scrlMaxTasks.Value = _editorItem.Tasks.Count;
                scrlTask.Maximum = scrlMaxTasks.Value;

                if (scrlItem.Value > 0)
                {
                    lblItem.Text = @"Item: " +
                                   ItemBase.GetName(Database.GameObjectIdFromList(GameObject.Item, scrlItem.Value));
                }
                else
                {
                    lblItem.Text = @"Item: 0 None";
                }
                lblLevel.Text = @"Level: " + scrlLevel.Value;
                if (scrlQuest.Value >= 0)
                {
                    lblQuest.Text = @"Quest: " +
                                    QuestBase.GetName(Database.GameObjectIdFromList(GameObject.Quest, scrlQuest.Value));
                }
                else
                {
                    lblQuest.Text = @"Quest: None";
                }
                if (scrlSwitch.Value == -1)
                {
                    lblSwitch.Text = @"Switch: None";
                }
                else
                {
                    lblSwitch.Text = @"Switch: " +
                                     QuestBase.GetName(Database.GameObjectIdFromList(GameObject.PlayerSwitch,
                                         scrlSwitch.Value));
                }
                if (scrlVariable.Value == -1)
                {
                    lblVariable.Text = @"Variable: None";
                }
                else
                {
                    lblVariable.Text = @"Variable: " +
                                       QuestBase.GetName(Database.GameObjectIdFromList(GameObject.PlayerVariable,
                                           scrlVariable.Value));
                }
                lblVariableValue.Text = @"Variable Value: " + scrlVariableValue.Value;
                lblMaxTasks.Text = @"Max Tasks: " + scrlMaxTasks.Value;
                lblTask.Text = "Task: " + (scrlTask.Value + 1);

                UpdateTask();

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

        private void UpdateTask()
        {
            lblObjective1.Visible = true;
            lblObjective2.Visible = true;
            scrlObjective1.Visible = true;
            scrlObjective2.Visible = true;

            txtDesc.Text = _editorItem.Tasks[scrlTask.Value].Desc;
            scrlObjective1.Value = _editorItem.Tasks[scrlTask.Value].Data1;
            scrlObjective2.Value = _editorItem.Tasks[scrlTask.Value ].Data2;

            switch (_editorItem.Tasks[scrlTask.Value].Objective)
            {
                case 0:
                    rbItem.Checked = true;
                    UpdateObjectives();
                    break;
                case 1:
                    rbNpc.Checked = true;
                    UpdateObjectives();
                    break;
                case 2:
                    rbEvent.Checked = true;
                    lblObjective1.Visible = false;
                    lblObjective2.Visible = false;
                    scrlObjective1.Visible = false;
                    scrlObjective2.Visible = false;
                    break;
            }

            scrlExp.Value = _editorItem.Tasks[scrlTask.Value].Experience;
            lblExp.Text = @"Experience: " + scrlExp.Value;

            scrlIndex.Value = 1;
            UpdateRewards();
        }

        private void UpdateRewards()
        {
            int index = scrlIndex.Value;
            lblIndex.Text = @"Rewards Index: " + (index + 1);
            scrlItemReward.Value = Database.GameObjectListIndex(GameObject.Item,_editorItem.Tasks[scrlTask.Value].Rewards[index].ItemNum);
            if (scrlItemReward.Value >= 0)
            {
                lblItemReward.Text = @"Item: " + ItemBase.GetName(_editorItem.Tasks[scrlTask.Value].Rewards[index].ItemNum);
            }
            else
            {
                lblItemReward.Text = @"Item: None";
            }
            txtAmount.Text = _editorItem.Tasks[scrlTask.Value].Rewards[index].Amount.ToString();
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

        private void scrlItem_ValueChanged(object sender, EventArgs e)
        {
            if (scrlItem.Value >= 0)
            {
                _editorItem.ItemReq = Database.GameObjectIdFromList(GameObject.Item,scrlItem.Value);
                lblItem.Text = @"Item: " + ItemBase.GetName(_editorItem.ItemReq);
            }
            else
            {
                _editorItem.ItemReq = -1;
                lblItem.Text = @"Item: None";
            }
        }

        private void scrlLevel_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.LevelReq = scrlLevel.Value;
            lblLevel.Text = @"Level: " + scrlLevel.Value;
        }

        private void scrlQuest_ValueChanged(object sender, EventArgs e)
        {
            if (scrlQuest.Value >= 0)
            {
                _editorItem.QuestReq = Database.GameObjectIdFromList(GameObject.Quest,scrlQuest.Value);
                lblQuest.Text = @"Quest: " + QuestBase.GetName(_editorItem.QuestReq);
            }
            else
            {
                _editorItem.QuestReq = -1;
                lblQuest.Text = @"Quest: None";
            }
        }

        private void scrlSwitch_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.SwitchReq = scrlSwitch.Value;
            lblSwitch.Text = @"Switch: " + scrlSwitch.Value;
        }

        private void scrlVariable_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.VariableReq = scrlVariable.Value;
            lblVariable.Text = @"Variable: " + scrlVariable.Value;
        }

        private void scrlVariableValue_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.VariableValue = scrlVariableValue.Value;
            lblVariableValue.Text = @"Variable Value: " + scrlVariableValue.Value;
        }

        private void scrlMaxTasks_ValueChanged(object sender, EventArgs e)
        {
            lblMaxTasks.Text = @"Max Tasks: " + scrlMaxTasks.Value;
            scrlTask.Maximum = scrlMaxTasks.Value;
            _editorItem.Tasks.Add(new QuestBase.QuestTask());
        }

        private void scrlTask_ValueChanged(object sender, EventArgs e)
        {
            lblTask.Text = "Task: " + scrlTask.Value;
            UpdateTask();
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            _editorItem.Tasks[scrlTask.Value].Desc = txtDesc.Text;
        }

        private void scrlObjective1_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Tasks[scrlTask.Value].Data1 = scrlObjective1.Value;
            UpdateObjectives();
        }

        private void scrlObjective2_Scroll(object sender, ScrollEventArgs e)
        {
            _editorItem.Tasks[scrlTask.Value].Data2 = scrlObjective2.Value;
            UpdateObjectives();
        }

        private void UpdateObjectives()
        {
            switch (_editorItem.Tasks[scrlTask.Value].Objective)
            {
                case 0:
                    rbItem.Checked = true;
                    scrlObjective1.Maximum = ItemBase.ObjectCount();
                    if (scrlObjective1.Value == -1)
                    {
                        lblObjective1.Text = @"Item: None";
                    }
                    else
                    {
                        lblObjective1.Text = @"Item: " + ItemBase.GetName(Database.GameObjectIdFromList(GameObject.Item, scrlObjective1.Value));
                    }
                    lblObjective2.Text = @"Quantity: x" + scrlObjective2.Value;
                    break;
                case 1:
                    rbNpc.Checked = true;
                    scrlObjective1.Maximum = NpcBase.ObjectCount();
                    if (scrlObjective1.Value == -1)
                    {
                        lblObjective1.Text = @"Npc: None";
                    }
                    else
                    {
                        lblObjective1.Text = @"Npc: " + NpcBase.GetName(Database.GameObjectIdFromList(GameObject.Npc, scrlObjective1.Value));
                    }
                    lblObjective2.Text = @"Quantity: x" + scrlObjective2.Value;
                    break;
            }
        }

        private void scrlExp_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Tasks[scrlTask.Value].Experience = scrlExp.Value;
            lblExp.Text = "Experience: " + scrlExp.Value;
        }

        private void scrlIndex_ValueChanged(object sender, EventArgs e)
        {
            UpdateRewards();
        }

        private void scrlItemReward_ValueChanged(object sender, EventArgs e)
        {
            if (scrlItemReward.Value == -1)
            {
                _editorItem.Tasks[scrlTask.Value].Rewards[scrlIndex.Value].ItemNum = -1;
                lblItemReward.Text = @"Item: None";
            }
            else
            {
                _editorItem.Tasks[scrlTask.Value].Rewards[scrlIndex.Value].ItemNum = Database.GameObjectIdFromList(GameObject.Item,scrlItemReward.Value);
                lblItemReward.Text = @"Item: " + ItemBase.GetName(_editorItem.Tasks[scrlTask.Value].Rewards[scrlIndex.Value].ItemNum);
            }
        }

        private void txtAmount_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtAmount.Text, out x);
            _editorItem.Tasks[scrlTask.Value].Rewards[scrlIndex.Value].Amount = x;
        }

        private void rbItem_Click(object sender, EventArgs e)
        {
            _editorItem.Tasks[scrlTask.Value].Objective = 0;
            UpdateTask();
        }

        private void rbNpc_Click(object sender, EventArgs e)
        {
            _editorItem.Tasks[scrlTask.Value].Objective = 1;
            UpdateTask();
        }

        private void rbEvent_Click(object sender, EventArgs e)
        {
            _editorItem.Tasks[scrlTask.Value].Objective = 2;
            UpdateTask();
        }

        private void cmbClass_Click(object sender, EventArgs e)
        {
            _editorItem.ClassReq = cmbClass.SelectedIndex;
        }

    }
}
