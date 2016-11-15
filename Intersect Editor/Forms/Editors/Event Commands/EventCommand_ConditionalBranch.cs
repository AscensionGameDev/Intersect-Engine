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
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Events;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_ConditionalBranch : UserControl
    {
        private EventCommand _myCommand;
        private EventPage _currentPage;
        private readonly FrmEvent _eventEditor;
        public bool Cancelled = false;
        public EventCommand_ConditionalBranch(EventCommand refCommand, EventPage refPage, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            _currentPage = refPage;
            cmbConditionType.SelectedIndex = _myCommand.Ints[0];
            UpdateFormElements();
            switch (cmbConditionType.SelectedIndex)
            {
                case 0: //Player Switch
                    cmbSwitch.SelectedIndex = Database.GameObjectListIndex(GameObject.PlayerSwitch,_myCommand.Ints[1]);
                    cmbSwitchVal.SelectedIndex = _myCommand.Ints[2];
                    break;
                case 1: //Player Variable
                    cmbVariable.SelectedIndex = Database.GameObjectListIndex(GameObject.PlayerVariable,_myCommand.Ints[1]);
                    cmbVariableMod.SelectedIndex = _myCommand.Ints[2];
                    txtVariableVal.Text = _myCommand.Ints[3].ToString();
                    break;
                case 2: //Global Switch
                    cmbSwitch.SelectedIndex = Database.GameObjectListIndex(GameObject.ServerSwitch, _myCommand.Ints[1]);
                    cmbSwitchVal.SelectedIndex = _myCommand.Ints[2];
                    break;
                case 3: //Global Variable
                    cmbVariable.SelectedIndex = Database.GameObjectListIndex(GameObject.ServerVariable, _myCommand.Ints[1]);
                    cmbVariableMod.SelectedIndex = _myCommand.Ints[2];
                    txtVariableVal.Text = _myCommand.Ints[3].ToString();
                    break;
                case 4: //Has Item
                    cmbItem.SelectedIndex = Database.GameObjectListIndex(GameObject.Item, _myCommand.Ints[1]);
                    scrlItemQuantity.Value = _myCommand.Ints[2];
                    lblItemQuantity.Text = @"Has at least: " + scrlItemQuantity.Value;
                    break;
                case 5: //Class Is
                    cmbClass.SelectedIndex = Database.GameObjectListIndex(GameObject.Class, _myCommand.Ints[1]);
                    break;
                case 6: //Knows spell
                    cmbSpell.SelectedIndex = Database.GameObjectListIndex(GameObject.Spell, _myCommand.Ints[1]);
                    break;
                case 7: //Level is...
                    cmbLevelComparator.SelectedIndex = _myCommand.Ints[1];
                    scrlLevel.Value = _myCommand.Ints[2];
                    lblLevel.Text = @"Level: " + scrlLevel.Value;
                    break;
                case 8: //Self Switch
                    cmbSelfSwitch.SelectedIndex = _myCommand.Ints[1];
                    cmbSelfSwitchVal.SelectedIndex = _myCommand.Ints[2];
                    break;
                case 9: //Power Is
                    cmbPower.SelectedIndex = _myCommand.Ints[1];
                    break;
                case 10: //Time is between
                    cmbTime1.SelectedIndex = Math.Min(_myCommand.Ints[1],cmbTime1.Items.Count-1);
                    cmbTime2.SelectedIndex = Math.Min(_myCommand.Ints[2], cmbTime2.Items.Count - 1);
                    break;
                case 11: //Can Start Quest
                    cmbStartQuest.SelectedIndex = Database.GameObjectListIndex(GameObject.Quest, _myCommand.Ints[1]);
                    break;
                case 12: //Quest In Progress
                    cmbQuestInProgress.SelectedIndex = Database.GameObjectListIndex(GameObject.Quest, _myCommand.Ints[1]);
                    cmbTaskModifier.SelectedIndex = _myCommand.Ints[2];
                    if (cmbTaskModifier.SelectedIndex == -1) cmbTaskModifier.SelectedIndex = 0;
                    if (cmbTaskModifier.SelectedIndex != 0)
                    {
                        //Get Quest Task Here
                        var quest = QuestBase.GetQuest(Database.GameObjectIdFromList(GameObject.Quest, cmbQuestInProgress.SelectedIndex));
                        if (quest != null)
                        {
                            for (int i = 0; i < quest.Tasks.Count; i++)
                            {
                                if (quest.Tasks[i].Id == _myCommand.Ints[3])
                                {
                                    cmbQuestTask.SelectedIndex = i;
                                }
                            }
                        }
                    }
                    break;
                case 13: //Quest Completed
                    cmbCompletedQuest.SelectedIndex = Database.GameObjectListIndex(GameObject.Quest, _myCommand.Ints[1]);
                    break;
            }
        }

        private void UpdateFormElements()
        {
            grpSwitch.Hide();
            grpPlayerVariable.Hide();
            grpHasItem.Hide();
            grpSpell.Hide();
            grpClass.Hide();
            grpLevel.Hide();
            grpSelfSwitch.Hide();
            grpPowerIs.Hide();
            grpTime.Hide();
            grpStartQuest.Hide();
            grpQuestInProgress.Hide();
            grpQuestCompleted.Hide();
            switch (cmbConditionType.SelectedIndex)
            {
                case 0: //Player Switch
                    grpSwitch.Text = "Player Switch";
                    grpSwitch.Show();
                    cmbSwitch.Items.Clear();
                    cmbSwitch.Items.AddRange(Database.GetGameObjectList(GameObject.PlayerSwitch));
                    if (cmbSwitch.Items.Count > 0) cmbSwitch.SelectedIndex = 0;
                    cmbSwitchVal.SelectedIndex = 0;
                    break;
                case 1: //Player Variables
                    grpPlayerVariable.Text = "Player Variable";
                    grpPlayerVariable.Show();
                    cmbVariable.Items.Clear();
                    cmbVariable.Items.AddRange(Database.GetGameObjectList(GameObject.PlayerVariable));
                    if (cmbVariable.Items.Count > 0) cmbVariable.SelectedIndex = 0;
                    cmbVariableMod.SelectedIndex = 0;
                    txtVariableVal.Text = @"0";
                    break;
                case 2: //Global Switch
                    grpPlayerVariable.Text = "Global Switch";
                    grpSwitch.Show();
                    cmbSwitch.Items.Clear();
                    cmbSwitch.Items.AddRange(Database.GetGameObjectList(GameObject.ServerSwitch));
                    if (cmbSwitch.Items.Count > 0) cmbSwitch.SelectedIndex = 0;
                    cmbSwitchVal.SelectedIndex = 0;
                    break;
                case 3: //Global Variables
                    grpPlayerVariable.Text = "Global Variable";
                    grpPlayerVariable.Show();
                    cmbVariable.Items.Clear();
                    cmbVariable.Items.AddRange(Database.GetGameObjectList(GameObject.ServerVariable));
                    if (cmbVariable.Items.Count > 0) cmbVariable.SelectedIndex = 0;
                    cmbVariableMod.SelectedIndex = 0;
                    txtVariableVal.Text = @"0";
                    break;
                case 4: //Has Item
                    grpHasItem.Show();
                    cmbItem.Items.Clear();
                    cmbItem.Items.AddRange(Database.GetGameObjectList(GameObject.Item));
                    if (cmbItem.Items.Count > 0) cmbItem.SelectedIndex = 0;
                    scrlItemQuantity.Value = 1;
                    break;
                case 5: //Class is
                    grpClass.Show();
                    cmbClass.Items.Clear();
                    cmbClass.Items.AddRange(Database.GetGameObjectList(GameObject.Class));
                    if (cmbClass.Items.Count > 0) cmbClass.SelectedIndex = 0;
                    break;
                case 6: //Knows spell
                    grpSpell.Show();
                    cmbSpell.Items.Clear();
                    cmbSpell.Items.AddRange(Database.GetGameObjectList(GameObject.Spell));
                    if (cmbSpell.Items.Count > 0) cmbSpell.SelectedIndex = 0;
                    break;
                case 7: //Level is...
                    grpLevel.Show();
                    cmbLevelComparator.SelectedIndex = 0;
                    scrlLevel.Value = 0;
                    lblLevel.Text = @"Level: " + scrlLevel.Value;
                    break;
                case 8: //Self Switch
                    grpSelfSwitch.Show();
                    cmbSelfSwitch.SelectedIndex = 0;
                    cmbSelfSwitchVal.SelectedIndex = 0;
                    break;
                case 9: //Power is
                    grpPowerIs.Show();
                    cmbPower.SelectedIndex = 0;
                    break;
                case 10: //Time is between...
                    grpTime.Show();
                    cmbTime1.Items.Clear();
                    cmbTime2.Items.Clear();
                    var time = new DateTime(2000, 1, 1, 0, 0, 0);
                    for (int i = 0; i < 1440; i += TimeBase.GetTimeBase().RangeInterval)
                    {
                        var addRange = time.ToString("h:mm:ss tt") + " to ";
                        time = time.AddMinutes(TimeBase.GetTimeBase().RangeInterval);
                        addRange += time.ToString("h:mm:ss tt");
                        cmbTime1.Items.Add(addRange);
                        cmbTime2.Items.Add(addRange);
                    }
                    cmbTime1.SelectedIndex = 0;
                    cmbTime2.SelectedIndex = 0;
                    break;
                case 11: //Can Start Quest
                    grpStartQuest.Show();
                    cmbStartQuest.Items.Clear();
                    cmbStartQuest.Items.AddRange(Database.GetGameObjectList(GameObject.Quest));
                    if (cmbStartQuest.Items.Count > 0) cmbStartQuest.SelectedIndex = 0;
                    break;
                case 12: //Quest In Progress
                    grpQuestInProgress.Show();
                    cmbQuestInProgress.Items.Clear();
                    cmbQuestInProgress.Items.AddRange(Database.GetGameObjectList(GameObject.Quest));
                    if (cmbQuestInProgress.Items.Count > 0) cmbQuestInProgress.SelectedIndex = 0;
                    cmbTaskModifier.SelectedIndex = 0;
                    break;
                case 13: //Quest Completed
                    grpQuestCompleted.Show();
                    cmbCompletedQuest.Items.Clear();
                    cmbCompletedQuest.Items.AddRange(Database.GetGameObjectList(GameObject.Quest));
                    if (cmbCompletedQuest.Items.Count > 0) cmbCompletedQuest.SelectedIndex = 0;
                    break;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int n;
            if (_currentPage != null)
            {
                if (_currentPage.Conditions.IndexOf(_myCommand) == -1)
                {
                    if (_myCommand.Ints[4] == 0)
                        // command.Ints[4 & 5] are reserved for referencing which command list the true/false braches follow
                    {
                        for (var i = 0; i < 2; i++)
                        {
                            _currentPage.CommandLists.Add(new CommandList());
                            _myCommand.Ints[4 + i] = _currentPage.CommandLists.Count - 1;
                        }
                    }
                }
            }

            _myCommand.Ints[0] = cmbConditionType.SelectedIndex;
            switch (_myCommand.Ints[0])
            {
                case 0: //Player Switch
                    _myCommand.Ints[1] = Database.GameObjectIdFromList(GameObject.PlayerSwitch,cmbSwitch.SelectedIndex);
                    _myCommand.Ints[2] = cmbSwitchVal.SelectedIndex;
                    break;
                case 1: //Player Variable
                    _myCommand.Ints[1] = Database.GameObjectIdFromList(GameObject.PlayerVariable, cmbVariable.SelectedIndex);
                    _myCommand.Ints[2] = cmbVariableMod.SelectedIndex;
                    if (int.TryParse(txtVariableVal.Text, out n))
                    {
                        _myCommand.Ints[3] = n;
                    }
                    else
                    {
                        _myCommand.Ints[3] = 0;
                    }
                    break;
                case 2: //Global Switch
                    _myCommand.Ints[1] = Database.GameObjectIdFromList(GameObject.ServerSwitch, cmbSwitch.SelectedIndex);
                    _myCommand.Ints[2] =  cmbSwitchVal.SelectedIndex;
                    break;
                case 3: //Global Variable
                    _myCommand.Ints[1] = Database.GameObjectIdFromList(GameObject.ServerVariable, cmbVariable.SelectedIndex);
                    _myCommand.Ints[2] = cmbVariableMod.SelectedIndex;
                    if (int.TryParse(txtVariableVal.Text, out n))
                    {
                        _myCommand.Ints[3] = n;
                    }
                    else
                    {
                        _myCommand.Ints[3] = 0;
                    }
                    break;
                case 4: //Has Item
                    _myCommand.Ints[1] = Database.GameObjectIdFromList(GameObject.Item, cmbItem.SelectedIndex);
                    _myCommand.Ints[2] = scrlItemQuantity.Value;
                    break;
                case 5: //Class Is
                    _myCommand.Ints[1] = Database.GameObjectIdFromList(GameObject.Class, cmbClass.SelectedIndex);
                    break;
                case 6: //Knows spell
                    _myCommand.Ints[1] = Database.GameObjectIdFromList(GameObject.Spell, cmbSpell.SelectedIndex);
                    break;
                case 7: //Level is
                    _myCommand.Ints[1] = cmbLevelComparator.SelectedIndex;
                    _myCommand.Ints[2] = scrlLevel.Value;
                    break;
                case 8: //Self Switch
                    _myCommand.Ints[1] = cmbSelfSwitch.SelectedIndex;
                    _myCommand.Ints[2] = cmbSelfSwitchVal.SelectedIndex;
                    break;
                case 9: //Power is
                    _myCommand.Ints[1] = cmbPower.SelectedIndex;
                    break;
                case 10: //Time is between...
                    _myCommand.Ints[1] = cmbTime1.SelectedIndex;
                    _myCommand.Ints[2] = cmbTime2.SelectedIndex;
                    break;
                case 11: //Can Start Quest
                    _myCommand.Ints[1] = Database.GameObjectIdFromList(GameObject.Quest, cmbStartQuest.SelectedIndex);
                    break;
                case 12: //Quest IN Progress
                    _myCommand.Ints[1] = Database.GameObjectIdFromList(GameObject.Quest, cmbQuestInProgress.SelectedIndex);
                    _myCommand.Ints[2] = cmbTaskModifier.SelectedIndex;
                    _myCommand.Ints[3] = -1;
                    if (cmbTaskModifier.SelectedIndex != 0)
                    {
                        //Get Quest Task Here
                        var quest = QuestBase.GetQuest(Database.GameObjectIdFromList(GameObject.Quest, cmbQuestInProgress.SelectedIndex));
                        if (quest != null)
                        {
                            if (cmbQuestTask.SelectedIndex > -1)
                            {
                                _myCommand.Ints[3] = quest.Tasks[cmbQuestTask.SelectedIndex].Id;
                            }
                        }
                    }
                    break;
                case 13: //Quest Completed
                    _myCommand.Ints[1] = Database.GameObjectIdFromList(GameObject.Quest, cmbCompletedQuest.SelectedIndex);
                    break;
            }
            if (_eventEditor != null)
            {
                _eventEditor.FinishCommandEdit();
            }
            else
            {
                if (ParentForm != null) ParentForm.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_currentPage != null)
            {
                if (_currentPage.Conditions.IndexOf(_myCommand) > -1)
                {
                    _currentPage.Conditions.Remove(_myCommand);
                    _eventEditor.CancelCommandEdit(false,true);
                }
                else
                {
                    _eventEditor.CancelCommandEdit();
                }
            }
            Cancelled = true;
            if (ParentForm != null) ParentForm.Close();
        }

        private void cmbConditionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }

        private void scrlItemQuantity_Scroll(object sender, ScrollEventArgs e)
        {
            lblItemQuantity.Text = @"Has at least: " + scrlItemQuantity.Value;
        }

        private void scrlLevel_Scroll(object sender, ScrollEventArgs e)
        {
            lblLevel.Text = @"Level: " + scrlLevel.Value;
        }

        private void cmbTaskModifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTaskModifier.SelectedIndex == 0)
            {
                cmbQuestTask.Enabled = false;
            }
            else
            {
                cmbQuestTask.Enabled = true;
            }
        }

        private void cmbQuestInProgress_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbQuestTask.Items.Clear();
            var quest =
                QuestBase.GetQuest(Database.GameObjectIdFromList(GameObject.Quest, cmbQuestInProgress.SelectedIndex));
            if (quest != null)
            {
                foreach (var task in quest.Tasks)
                {
                    cmbQuestTask.Items.Add(task.GetTaskString());
                }
                if (cmbQuestTask.Items.Count > 0) cmbQuestTask.SelectedIndex = 0;
            }
        }
    }
}
