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
using System.Windows.Forms;
using System.Drawing;
using Intersect_Editor.Classes;

namespace Intersect_Editor.Forms
{
    public partial class frmQuest : Form
    {
        private ByteBuffer[] _questsBackup;
        private bool[] _changed;
        private int _editorIndex;

        public frmQuest()
        {
            InitializeComponent();
        }

        private void frmQuest_Load(object sender, EventArgs e)
        {
            scrlItem.Maximum = Constants.MaxItems;
            scrlLevel.Maximum = Constants.MaxLevel;
            scrlQuest.Maximum = Constants.MaxQuests;
            scrlSwitch.Maximum = Constants.SwitchCount;
            scrlVariable.Maximum = Constants.VariableCount;
            scrlItemReward.Maximum = Constants.MaxItems;

            lstQuests.SelectedIndex = 0;
            scrlTask.Value = 1;
        }

        public void InitEditor()
        {
            int n = 0;
            _questsBackup = new ByteBuffer[Constants.MaxQuests];
            _changed = new bool[Constants.MaxQuests];
            for (var i = 0; i < Constants.MaxQuests; i++)
            {
                _questsBackup[i] = new ByteBuffer();
                _questsBackup[i].WriteBytes(Globals.GameQuests[i].QuestData());
                lstQuests.Items.Add((i + 1) + ") " + Globals.GameQuests[i].Name);
                _changed[i] = false;
            }
            lstQuests.SelectedIndex = 0;

            cmbClass.Items.Clear();
            cmbClass.Items.Add("None");
            while (Globals.GameClasses[n].Name != "")
            {
                cmbClass.Items.Add(Globals.GameClasses[n].Name);
                n = n + 1;
                if (n >= Constants.MaxClasses)
                {
                    break;
                }
            }
            cmbClass.SelectedIndex = 0;

            UpdateEditor();
        }

        private void UpdateEditor()
        {
            _editorIndex = lstQuests.SelectedIndex;
            txtName.Text = Globals.GameQuests[_editorIndex].Name;
            txtStartDesc.Text = Globals.GameQuests[_editorIndex].StartDesc;
            txtEndDesc.Text = Globals.GameQuests[_editorIndex].EndDesc;

            cmbClass.SelectedIndex = Globals.GameQuests[_editorIndex].ClassReq;
            scrlItem.Value = Globals.GameQuests[_editorIndex].ItemReq;
            scrlLevel.Value = Globals.GameQuests[_editorIndex].LevelReq;
            scrlQuest.Value = Globals.GameQuests[_editorIndex].QuestReq;
            scrlSwitch.Value = Globals.GameQuests[_editorIndex].SwitchReq;
            scrlVariable.Value = Globals.GameQuests[_editorIndex].VariableReq;
            scrlVariableValue.Value = Globals.GameQuests[_editorIndex].VariableValue;

            scrlTask.Value = 1;
            scrlMaxTasks.Value = Globals.GameQuests[_editorIndex].Tasks.Count;
            scrlTask.Maximum = scrlMaxTasks.Value;

            if (scrlItem.Value > 0) { lblItem.Text = @"Item: " + (scrlItem.Value) + @" - " + Globals.GameItems[scrlItem.Value - 1].Name; }
            else { lblItem.Text = @"Item: 0 None"; }
            lblLevel.Text = @"Level: " + scrlLevel.Value;
            if (scrlQuest.Value > 0) { lblQuest.Text = @"Quest: " + (scrlQuest.Value) + @" - " + Globals.GameQuests[scrlQuest.Value - 1].Name; }
            else { lblQuest.Text = @"Quest: 0 None"; }
            lblSwitch.Text = @"Switch: " + scrlSwitch.Value;
            lblVariable.Text = @"Variable: " + scrlVariable.Value;
            lblVariableValue.Text = @"Variable Value: " + scrlVariableValue.Value;
            lblMaxTasks.Text = @"Max Tasks: " + scrlMaxTasks.Value;
            lblTask.Text = "Task: " + scrlTask.Value;

            UpdateTask();

            _changed[_editorIndex] = true;
        }

        private void UpdateTask()
        {
            lblObjective1.Visible = true;
            lblObjective2.Visible = true;
            scrlObjective1.Visible = true;
            scrlObjective2.Visible = true;

            txtDesc.Text = Globals.GameQuests[_editorIndex].Tasks[scrlTask.Value - 1].Desc;
            scrlObjective1.Value = Globals.GameQuests[_editorIndex].Tasks[scrlTask.Value - 1].Data1;
            scrlObjective2.Value = Globals.GameQuests[_editorIndex].Tasks[scrlTask.Value - 1].Data2;

            switch (Globals.GameQuests[_editorIndex].Tasks[scrlTask.Value - 1].Objective)
            {
                case 0:
                    rbItem.Checked = true;
                    scrlObjective1.Maximum = Constants.MaxItems;
                    if (scrlObjective1.Value == 0) { lblObjective1.Text = @"Item: 0 None"; }
                    else { lblObjective1.Text = @"Item: " + scrlObjective1.Value + " " + Globals.GameItems[scrlObjective1.Value - 1].Name; }
                    lblObjective2.Text = @"Quantity: x" + scrlObjective2.Value;
                    break;
                case 1:
                    rbNpc.Checked = true;
                    scrlObjective1.Maximum = Constants.MaxNpcs;
                    if (scrlObjective1.Value == 0) { lblObjective1.Text = @"Npc: 0 None"; }
                    else { lblObjective1.Text = @"Npc: " + scrlObjective1.Value + " " + Globals.GameNpcs[scrlObjective1.Value - 1].Name; }
                    lblObjective2.Text = @"Quantity: x" + scrlObjective2.Value;
                    break;
                case 2:
                    rbEvent.Checked = true;
                    lblObjective1.Visible = false;
                    lblObjective2.Visible = false;
                    scrlObjective1.Visible = false;
                    scrlObjective2.Visible = false;
                    break;
            }

            scrlExp.Value = Globals.GameQuests[_editorIndex].Tasks[scrlTask.Value - 1].Experience;
            lblExp.Text = @"Experience: " + scrlExp.Value;

            scrlIndex.Value = 1;
            UpdateRewards();
        }

        private void UpdateRewards()
        {
            int index = scrlIndex.Value - 1;
            lblIndex.Text = @"Rewards Index: " + (index + 1);
            scrlItemReward.Value = Globals.GameQuests[_editorIndex].Tasks[scrlTask.Value - 1].Rewards[index].ItemNum;
            if (scrlItemReward.Value > 0) { lblItemReward.Text = @"Item: " + (scrlItemReward.Value) + @" - " + Globals.GameItems[scrlItemReward.Value - 1].Name; }
            else { lblItemReward.Text = @"Item: 0 None"; }
            txtAmount.Text = Globals.GameQuests[_editorIndex].Tasks[scrlTask.Value - 1].Rewards[index].Amount.ToString();
        }

        private void lstQuests_Click(object sender, EventArgs e)
        {
            UpdateEditor();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            Globals.GameQuests[_editorIndex].Name = txtName.Text;
            lstQuests.Items[_editorIndex] = (_editorIndex + 1) + ") " + txtName.Text;
        }

        private void txtStartDesc_TextChanged(object sender, EventArgs e)
        {
            Globals.GameQuests[_editorIndex].StartDesc = txtStartDesc.Text;
        }

        private void txtEndDesc_TextChanged(object sender, EventArgs e)
        {
            Globals.GameQuests[_editorIndex].EndDesc = txtEndDesc.Text;
        }

        private void scrlItem_ValueChanged(object sender, EventArgs e)
        {
            Globals.GameQuests[_editorIndex].ItemReq = scrlItem.Value;
            if (scrlItem.Value > 0) { lblItem.Text = @"Item: " + (scrlItem.Value) + @" - " + Globals.GameItems[scrlItem.Value - 1].Name; }
            else { lblItem.Text = @"Item: 0 None"; }
        }

        private void scrlLevel_ValueChanged(object sender, EventArgs e)
        {
            Globals.GameQuests[_editorIndex].LevelReq = scrlLevel.Value;
            lblLevel.Text = @"Level: " + scrlLevel.Value;
        }

        private void scrlQuest_ValueChanged(object sender, EventArgs e)
        {
            Globals.GameQuests[_editorIndex].QuestReq = scrlQuest.Value;
            if (scrlQuest.Value > 0) { lblQuest.Text = @"Quest: " + (scrlQuest.Value) + @" - " + Globals.GameQuests[scrlQuest.Value - 1].Name; }
            else { lblQuest.Text = @"Quest: 0 None"; }
        }

        private void scrlSwitch_ValueChanged(object sender, EventArgs e)
        {
            Globals.GameQuests[_editorIndex].SwitchReq = scrlSwitch.Value;
            lblSwitch.Text = @"Switch: " + scrlSwitch.Value;
        }

        private void scrlVariable_ValueChanged(object sender, EventArgs e)
        {
            Globals.GameQuests[_editorIndex].VariableReq = scrlVariable.Value;
            lblVariable.Text = @"Variable: " + scrlVariable.Value;
        }

        private void scrlVariableValue_ValueChanged(object sender, EventArgs e)
        {
            Globals.GameQuests[_editorIndex].VariableValue = scrlVariableValue.Value;
            lblVariableValue.Text = @"Variable Value: " + scrlVariableValue.Value;
        }

        private void scrlMaxTasks_ValueChanged(object sender, EventArgs e)
        {
            lblMaxTasks.Text = @"Max Tasks: " + scrlMaxTasks.Value;
            scrlTask.Maximum = scrlMaxTasks.Value;
            Globals.GameQuests[_editorIndex].Tasks.Add(new QuestStruct.QuestTask());
        }

        private void scrlTask_ValueChanged(object sender, EventArgs e)
        {
            lblTask.Text = "Task: " + scrlTask.Value;
            UpdateTask();
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            Globals.GameQuests[_editorIndex].Tasks[scrlTask.Value - 1].Desc = txtDesc.Text;
        }

        private void scrlObjective1_ValueChanged(object sender, EventArgs e)
        {
            Globals.GameQuests[_editorIndex].Tasks[scrlTask.Value - 1].Data1 = scrlObjective1.Value;
            switch (Globals.GameQuests[_editorIndex].Tasks[scrlTask.Value - 1].Objective)
            {
                case 0:
                    rbItem.Checked = true;
                    scrlObjective1.Maximum = Constants.MaxItems;
                    if (scrlObjective1.Value == 0) { lblObjective1.Text = @"Item: 0 None"; }
                    else { lblObjective1.Text = @"Item: " + scrlObjective1.Value + " " + Globals.GameItems[scrlObjective1.Value - 1].Name; }
                    lblObjective2.Text = @"Quantity: x" + scrlObjective2.Value;
                    break;
                case 1:
                    rbNpc.Checked = true;
                    scrlObjective1.Maximum = Constants.MaxNpcs;
                    if (scrlObjective1.Value == 0) { lblObjective1.Text = @"Npc: 0 None"; }
                    else { lblObjective1.Text = @"Npc: " + scrlObjective1.Value + " " + Globals.GameNpcs[scrlObjective1.Value - 1].Name; }
                    lblObjective2.Text = @"Quantity: x" + scrlObjective2.Value;
                    break;
            }
        }

        private void scrlObjective2_Scroll(object sender, ScrollEventArgs e)
        {
            Globals.GameQuests[_editorIndex].Tasks[scrlTask.Value - 1].Data2 = scrlObjective2.Value;
            switch (Globals.GameQuests[_editorIndex].Tasks[scrlTask.Value - 1].Objective)
            {
                case 0:
                    rbItem.Checked = true;
                    scrlObjective1.Maximum = Constants.MaxItems;
                    if (scrlObjective1.Value == 0) { lblObjective1.Text = @"Item: 0 None"; }
                    else { lblObjective1.Text = @"Item: " + scrlObjective1.Value + " " + Globals.GameItems[scrlObjective1.Value - 1].Name; }
                    lblObjective2.Text = @"Quantity: x" + scrlObjective2.Value;
                    break;
                case 1:
                    rbNpc.Checked = true;
                    scrlObjective1.Maximum = Constants.MaxNpcs;
                    if (scrlObjective1.Value == 0) { lblObjective1.Text = @"Npc: 0 None"; }
                    else { lblObjective1.Text = @"Npc: " + scrlObjective1.Value + " " + Globals.GameNpcs[scrlObjective1.Value - 1].Name; }
                    lblObjective2.Text = @"Quantity: x" + scrlObjective2.Value;
                    break;
            }
        }

        private void scrlExp_ValueChanged(object sender, EventArgs e)
        {
            Globals.GameQuests[_editorIndex].Tasks[scrlTask.Value - 1].Experience = scrlExp.Value;
            lblExp.Text = "Experience: " + scrlExp.Value;
        }

        private void scrlIndex_ValueChanged(object sender, EventArgs e)
        {
            UpdateRewards();
        }

        private void scrlItemReward_ValueChanged(object sender, EventArgs e)
        {
            if (scrlItemReward.Value == 0) { lblItemReward.Text = @"Item: 0 None"; }
            else { lblItemReward.Text = @"Item: " + scrlItemReward.Value + " " + Globals.GameItems[scrlItemReward.Value - 1].Name; }
            Globals.GameQuests[_editorIndex].Tasks[scrlTask.Value - 1].Rewards[scrlIndex.Value - 1].ItemNum = scrlItemReward.Value;
        }

        private void txtAmount_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtAmount.Text, out x);
            Globals.GameQuests[_editorIndex].Tasks[scrlTask.Value - 1].Rewards[scrlIndex.Value - 1].Amount = x;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Constants.MaxQuests; i++)
            {
                if (_changed[i])
                {
                    PacketSender.SendQuest(i, Globals.GameQuests[i].QuestData());
                }
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var tempQuest = new QuestStruct();
            var tempBuff = new ByteBuffer();
            tempBuff.WriteBytes(tempQuest.QuestData());
            Globals.GameQuests[_editorIndex].Load(tempBuff.ToArray(),_editorIndex);
            tempBuff.Dispose();
            UpdateEditor();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Constants.MaxQuests; i++)
            {
                Globals.GameQuests[i].Load(_questsBackup[i].ToArray(),i);
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void rbItem_Click(object sender, EventArgs e)
        {
            Globals.GameQuests[_editorIndex].Tasks[scrlTask.Value - 1].Objective = 0;
            UpdateTask();
        }

        private void rbNpc_Click(object sender, EventArgs e)
        {
            Globals.GameQuests[_editorIndex].Tasks[scrlTask.Value - 1].Objective = 1;
            UpdateTask();
        }

        private void rbEvent_Click(object sender, EventArgs e)
        {
            Globals.GameQuests[_editorIndex].Tasks[scrlTask.Value - 1].Objective = 2;
            UpdateTask();
        }

        private void cmbClass_Click(object sender, EventArgs e)
        {
            Globals.GameQuests[_editorIndex].ClassReq = cmbClass.SelectedIndex;
        }

    }
}
