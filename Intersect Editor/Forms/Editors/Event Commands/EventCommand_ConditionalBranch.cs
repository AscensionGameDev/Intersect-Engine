
using System;
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Localization;

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
            InitLocalization();
            cmbConditionType.SelectedIndex = _myCommand.Ints[0];
            UpdateFormElements();
            switch (cmbConditionType.SelectedIndex)
            {
                case 0: //Player Switch
                    cmbSwitch.SelectedIndex = Database.GameObjectListIndex(GameObject.PlayerSwitch, _myCommand.Ints[1]);
                    cmbSwitchVal.SelectedIndex = _myCommand.Ints[2];
                    break;
                case 1: //Player Variable
                    cmbVariable.SelectedIndex = Database.GameObjectListIndex(GameObject.PlayerVariable, _myCommand.Ints[1]);
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
                    nudItemAmount.Value = _myCommand.Ints[2];
                    break;
                case 5: //Class Is
                    cmbClass.SelectedIndex = Database.GameObjectListIndex(GameObject.Class, _myCommand.Ints[1]);
                    break;
                case 6: //Knows spell
                    cmbSpell.SelectedIndex = Database.GameObjectListIndex(GameObject.Spell, _myCommand.Ints[1]);
                    break;
                case 7: //Level or Stat is...
                    cmbLevelComparator.SelectedIndex = _myCommand.Ints[1];
                    nudLevelStatValue.Value = _myCommand.Ints[2];
                    cmbLevelStat.SelectedIndex = _myCommand.Ints[3];
                    break;
                case 8: //Self Switch
                    cmbSelfSwitch.SelectedIndex = _myCommand.Ints[1];
                    cmbSelfSwitchVal.SelectedIndex = _myCommand.Ints[2];
                    break;
                case 9: //Power Is
                    cmbPower.SelectedIndex = _myCommand.Ints[1];
                    break;
                case 10: //Time is between
                    cmbTime1.SelectedIndex = Math.Min(_myCommand.Ints[1], cmbTime1.Items.Count - 1);
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
                case 14: //Player death...
                    break;
                case 15: //No NPC's on map
                    break;
                case 16: //Gender Is
                    cmbGender.SelectedIndex = _myCommand.Ints[1];
                    break;
            }
        }

        private void InitLocalization()
        {
            grpConditional.Text = Strings.Get("eventconditional", "title");
            lblType.Text = Strings.Get("eventconditional", "type");

            cmbConditionType.Items.Clear();
            for (int i = 0; i < 17; i++)
            {
                cmbConditionType.Items.Add(Strings.Get("eventconditional", "condition" + i));
            }

            //Player Switch
            grpSwitch.Text = Strings.Get("eventconditional", "playerswitch");
            lblSwitch.Text = Strings.Get("eventconditional", "switch");
            lblSwitchIs.Text = Strings.Get("eventconditional", "switchis");
            cmbSwitchVal.Items.Clear();
            cmbSwitchVal.Items.Add(Strings.Get("eventconditional", "false"));
            cmbSwitchVal.Items.Add(Strings.Get("eventconditional", "true"));

            //Player Variable
            grpPlayerVariable.Text = Strings.Get("eventconditional", "playervariable");
            lblVariable.Text = Strings.Get("eventconditional", "variable");
            lblComparator.Text = Strings.Get("eventconditional", "comparator");
            lblVariableValue.Text = Strings.Get("eventconditional", "value");
            cmbVariableMod.Items.Clear();
            for (int i = 0; i < 6; i++)
            {
                cmbVariableMod.Items.Add(Strings.Get("eventconditional", "comparator" + i));
            }

            //Has Item
            grpHasItem.Text = Strings.Get("eventconditional", "hasitem");
            lblItemQuantity.Text = Strings.Get("eventconditional", "hasatleast");
            lblItem.Text = Strings.Get("eventconditional", "item");

            //Class is
            grpClass.Text = Strings.Get("eventconditional", "classis");
            lblClass.Text = Strings.Get("eventconditional", "class");

            //Knows Spell
            grpSpell.Text = Strings.Get("eventconditional", "knowsspell");
            lblSpell.Text = Strings.Get("eventconditional", "spell");

            //Level or Stat is
            grpLevelStat.Text = Strings.Get("eventconditional", "levelorstat");
            lblLvlStatValue.Text = Strings.Get("eventconditional", "levelstatvalue");
            lblLevelComparator.Text = Strings.Get("eventconditional", "comparator");
            lblLevelOrStat.Text = Strings.Get("eventconditional", "levelstatitem");
            cmbLevelStat.Items.Clear();
            cmbLevelStat.Items.Add(Strings.Get("eventconditional", "level"));
            for (int i = 0; i < Options.MaxStats; i++)
            {
                cmbLevelStat.Items.Add(Strings.Get("combat", "stat" + i));
            }
            cmbLevelComparator.Items.Clear();
            for (int i = 0; i < 6; i++)
            {
                cmbLevelComparator.Items.Add(Strings.Get("eventconditional", "comparator" + i));
            }

            //Self Switch Is
            grpSelfSwitch.Text = Strings.Get("eventconditional", "selfswitchis");
            lblSelfSwitch.Text = Strings.Get("eventconditional", "selfswitch");
            lblSelfSwitchIs.Text = Strings.Get("eventconditional", "switchis");
            cmbSelfSwitch.Items.Clear();
            for (int i = 0; i < 4; i++)
            {
                cmbSelfSwitch.Items.Add(Strings.Get("eventconditional", "selfswitch" + i));
            }
            cmbSelfSwitchVal.Items.Clear();
            cmbSelfSwitchVal.Items.Add(Strings.Get("eventconditional", "false"));
            cmbSelfSwitchVal.Items.Add(Strings.Get("eventconditional", "true"));

            //Power Is
            grpPowerIs.Text = Strings.Get("eventconditional", "poweris");
            lblPower.Text = Strings.Get("eventconditional", "power");
            cmbPower.Items.Clear();
            cmbPower.Items.Add(Strings.Get("eventconditional", "power0"));
            cmbPower.Items.Add(Strings.Get("eventconditional", "power1"));

            //Time Is
            grpTime.Text = Strings.Get("eventconditional", "time");
            lblStartRange.Text = Strings.Get("eventconditional", "startrange");
            lblEndRange.Text = Strings.Get("eventconditional", "endrange");
            lblAnd.Text = Strings.Get("eventconditional", "and");

            //Can Start Quest
            grpStartQuest.Text = Strings.Get("eventconditional", "canstartquest");
            lblStartQuest.Text = Strings.Get("eventconditional", "startquest");

            //Quest In Progress
            grpQuestInProgress.Text = Strings.Get("eventconditional", "questinprogress");
            lblQuestProgress.Text = Strings.Get("eventconditional", "questprogress");
            lblQuestIs.Text = Strings.Get("eventconditional", "questis");
            cmbTaskModifier.Items.Clear();
            for (int i = 0; i < 4; i++)
            {
                cmbTaskModifier.Items.Add(Strings.Get("eventconditional", "questcomparator" + i));
            }
            lblQuestTask.Text = Strings.Get("eventconditional", "task");

            //Quest Completed
            grpQuestCompleted.Text = Strings.Get("eventconditional", "questcompleted");
            lblQuestCompleted.Text = Strings.Get("eventconditional", "questcompletedlabel");

            //Gender is
            grpGender.Text = Strings.Get("eventconditional", "genderis");
            lblGender.Text = Strings.Get("eventconditional", "gender");
            cmbGender.Items.Clear();
            cmbGender.Items.Add(Strings.Get("eventconditional", "male"));
            cmbGender.Items.Add(Strings.Get("eventconditional", "female"));

            btnSave.Text = Strings.Get("eventconditional", "okay");
            btnCancel.Text = Strings.Get("eventconditional", "cancel");
        }

        private void UpdateFormElements()
        {
            grpSwitch.Hide();
            grpPlayerVariable.Hide();
            grpHasItem.Hide();
            grpSpell.Hide();
            grpClass.Hide();
            grpLevelStat.Hide();
            grpSelfSwitch.Hide();
            grpPowerIs.Hide();
            grpTime.Hide();
            grpStartQuest.Hide();
            grpQuestInProgress.Hide();
            grpQuestCompleted.Hide();
            grpGender.Hide();
            switch (cmbConditionType.SelectedIndex)
            {
                case 0: //Player Switch
                    grpSwitch.Text = Strings.Get("eventconditional", "playerswitch");
                    grpSwitch.Show();
                    cmbSwitch.Items.Clear();
                    cmbSwitch.Items.AddRange(Database.GetGameObjectList(GameObject.PlayerSwitch));
                    if (cmbSwitch.Items.Count > 0) cmbSwitch.SelectedIndex = 0;
                    cmbSwitchVal.SelectedIndex = 0;
                    break;
                case 1: //Player Variables
                    grpPlayerVariable.Text = Strings.Get("eventconditional", "playervariable");
                    grpPlayerVariable.Show();
                    cmbVariable.Items.Clear();
                    cmbVariable.Items.AddRange(Database.GetGameObjectList(GameObject.PlayerVariable));
                    if (cmbVariable.Items.Count > 0) cmbVariable.SelectedIndex = 0;
                    cmbVariableMod.SelectedIndex = 0;
                    txtVariableVal.Text = @"0";
                    break;
                case 2: //Global Switch
                    grpPlayerVariable.Text = Strings.Get("eventconditional", "globalswitch");
                    grpSwitch.Show();
                    cmbSwitch.Items.Clear();
                    cmbSwitch.Items.AddRange(Database.GetGameObjectList(GameObject.ServerSwitch));
                    if (cmbSwitch.Items.Count > 0) cmbSwitch.SelectedIndex = 0;
                    cmbSwitchVal.SelectedIndex = 0;
                    break;
                case 3: //Global Variables
                    grpPlayerVariable.Text = Strings.Get("eventconditional", "globalvariable");
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
                    nudItemAmount.Value = 1;
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
                case 7: //Level or Stat is...
                    grpLevelStat.Show();
                    cmbLevelComparator.SelectedIndex = 0;
                    cmbLevelStat.SelectedIndex = 0;
                    nudLevelStatValue.Value = 0;
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
                        var addRange = time.ToString("h:mm:ss tt") + " " + Strings.Get("eventconditional", "to") + " ";
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
                case 14: //Player death...
                    break;
                case 15: //No NPC's on map
                    break;
                case 16: //Gender Is
                    grpGender.Show();
                    cmbGender.SelectedIndex = 0;
                    break;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int n;

            if (_currentPage != null)
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

            _myCommand.Ints[0] = cmbConditionType.SelectedIndex;
            switch (_myCommand.Ints[0])
            {
                case 0: //Player Switch
                    _myCommand.Ints[1] = Database.GameObjectIdFromList(GameObject.PlayerSwitch, cmbSwitch.SelectedIndex);
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
                    _myCommand.Ints[2] = cmbSwitchVal.SelectedIndex;
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
                    _myCommand.Ints[2] = (int)nudItemAmount.Value;
                    break;
                case 5: //Class Is
                    _myCommand.Ints[1] = Database.GameObjectIdFromList(GameObject.Class, cmbClass.SelectedIndex);
                    break;
                case 6: //Knows spell
                    _myCommand.Ints[1] = Database.GameObjectIdFromList(GameObject.Spell, cmbSpell.SelectedIndex);
                    break;
                case 7: //Level or Stat is
                    _myCommand.Ints[1] = cmbLevelComparator.SelectedIndex;
                    _myCommand.Ints[2] = (int)nudLevelStatValue.Value;
                    _myCommand.Ints[3] = cmbLevelStat.SelectedIndex;
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
                case 14: //Player death...
                    break;
                case 15: //No NPC's on map
                    break;
                case 16: //Gender Is
                    _myCommand.Ints[1] = cmbGender.SelectedIndex;
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
                _eventEditor.CancelCommandEdit();
            }
            Cancelled = true;
            if (ParentForm != null) ParentForm.Close();
        }

        private void cmbConditionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
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
