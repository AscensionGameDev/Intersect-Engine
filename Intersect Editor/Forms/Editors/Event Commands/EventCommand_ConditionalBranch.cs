using System;
using System.Windows.Forms;
using Intersect.Editor.Core;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandConditionalBranch : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventPage mCurrentPage;
        private EventCommand mMyCommand;
        public bool Cancelled;

        public EventCommandConditionalBranch(EventCommand refCommand, EventPage refPage, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            mCurrentPage = refPage;
            InitLocalization();
            cmbConditionType.SelectedIndex = mMyCommand.Ints[0];
            UpdateFormElements();
            switch (cmbConditionType.SelectedIndex)
            {
                case 0: //Player Switch
                    cmbSwitch.SelectedIndex =
                        Database.GameObjectListIndex(GameObjectType.PlayerSwitch, mMyCommand.Ints[1]);
                    cmbSwitchVal.SelectedIndex = mMyCommand.Ints[2];
                    break;
                case 1: //Player Variable
                    cmbVariable.SelectedIndex = Database.GameObjectListIndex(GameObjectType.PlayerVariable,
                        mMyCommand.Ints[1]);
                    cmbVariableMod.SelectedIndex = mMyCommand.Ints[2];
                    txtVariableVal.Text = mMyCommand.Ints[3].ToString();
                    break;
                case 2: //Global Switch
                    cmbSwitch.SelectedIndex =
                        Database.GameObjectListIndex(GameObjectType.ServerSwitch, mMyCommand.Ints[1]);
                    cmbSwitchVal.SelectedIndex = mMyCommand.Ints[2];
                    break;
                case 3: //Global Variable
                    cmbVariable.SelectedIndex = Database.GameObjectListIndex(GameObjectType.ServerVariable,
                        mMyCommand.Ints[1]);
                    cmbVariableMod.SelectedIndex = mMyCommand.Ints[2];
                    txtVariableVal.Text = mMyCommand.Ints[3].ToString();
                    break;
                case 4: //Has Item
                    cmbItem.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Item, mMyCommand.Ints[1]);
                    nudItemAmount.Value = mMyCommand.Ints[2];
                    break;
                case 5: //Class Is
                    cmbClass.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Class, mMyCommand.Ints[1]);
                    break;
                case 6: //Knows spell
                    cmbSpell.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Spell, mMyCommand.Ints[1]);
                    break;
                case 7: //Level or Stat is...
                    cmbLevelComparator.SelectedIndex = mMyCommand.Ints[1];
                    nudLevelStatValue.Value = mMyCommand.Ints[2];
                    cmbLevelStat.SelectedIndex = mMyCommand.Ints[3];
                    break;
                case 8: //Self Switch
                    cmbSelfSwitch.SelectedIndex = mMyCommand.Ints[1];
                    cmbSelfSwitchVal.SelectedIndex = mMyCommand.Ints[2];
                    break;
                case 9: //Power Is
                    cmbPower.SelectedIndex = mMyCommand.Ints[1];
                    break;
                case 10: //Time is between
                    cmbTime1.SelectedIndex = Math.Min(mMyCommand.Ints[1], cmbTime1.Items.Count - 1);
                    cmbTime2.SelectedIndex = Math.Min(mMyCommand.Ints[2], cmbTime2.Items.Count - 1);
                    break;
                case 11: //Can Start Quest
                    cmbStartQuest.SelectedIndex =
                        Database.GameObjectListIndex(GameObjectType.Quest, mMyCommand.Ints[1]);
                    break;
                case 12: //Quest In Progress
                    cmbQuestInProgress.SelectedIndex =
                        Database.GameObjectListIndex(GameObjectType.Quest, mMyCommand.Ints[1]);
                    cmbTaskModifier.SelectedIndex = mMyCommand.Ints[2];
                    if (cmbTaskModifier.SelectedIndex == -1) cmbTaskModifier.SelectedIndex = 0;
                    if (cmbTaskModifier.SelectedIndex != 0)
                    {
                        //Get Quest Task Here
                        var quest =
                            QuestBase.Lookup.Get<QuestBase>(Database.GameObjectIdFromList(GameObjectType.Quest,
                                cmbQuestInProgress.SelectedIndex));
                        if (quest != null)
                        {
                            for (int i = 0; i < quest.Tasks.Count; i++)
                            {
                                if (quest.Tasks[i].Id == mMyCommand.Ints[3])
                                {
                                    cmbQuestTask.SelectedIndex = i;
                                }
                            }
                        }
                    }
                    break;
                case 13: //Quest Completed
                    cmbCompletedQuest.SelectedIndex =
                        Database.GameObjectListIndex(GameObjectType.Quest, mMyCommand.Ints[1]);
                    break;
                case 14: //Player death...
                    break;
                case 15: //No NPC's on map
                    break;
                case 16: //Gender Is
                    cmbGender.SelectedIndex = mMyCommand.Ints[1];
                    break;
            }
        }

        private void InitLocalization()
        {
            grpConditional.Text = Strings.EventConditional.title;
            lblType.Text = Strings.EventConditional.type;

            cmbConditionType.Items.Clear();
            for (int i = 0; i < Strings.EventConditional.conditions.Length; i++)
            {
                cmbConditionType.Items.Add(Strings.EventConditional.conditions[i]);
            }

            //Player Switch
            grpSwitch.Text = Strings.EventConditional.playerswitch;
            lblSwitch.Text = Strings.EventConditional.Switch;
            lblSwitchIs.Text = Strings.EventConditional.switchis;
            cmbSwitchVal.Items.Clear();
            cmbSwitchVal.Items.Add(Strings.EventConditional.False);
            cmbSwitchVal.Items.Add(Strings.EventConditional.True);

            //Player Variable
            grpPlayerVariable.Text = Strings.EventConditional.playervariable;
            lblVariable.Text = Strings.EventConditional.variable;
            lblComparator.Text = Strings.EventConditional.comparator;
            lblVariableValue.Text = Strings.EventConditional.value;
            cmbVariableMod.Items.Clear();
            for (int i = 0; i < Strings.EventConditional.comparators.Length; i++)
            {
                cmbVariableMod.Items.Add(Strings.EventConditional.comparators[i]);
            }

            //Has Item
            grpHasItem.Text = Strings.EventConditional.hasitem;
            lblItemQuantity.Text = Strings.EventConditional.hasatleast;
            lblItem.Text = Strings.EventConditional.item;

            //Class is
            grpClass.Text = Strings.EventConditional.classis;
            lblClass.Text = Strings.EventConditional.Class;

            //Knows Spell
            grpSpell.Text = Strings.EventConditional.knowsspell;
            lblSpell.Text = Strings.EventConditional.spell;

            //Level or Stat is
            grpLevelStat.Text = Strings.EventConditional.levelorstat;
            lblLvlStatValue.Text = Strings.EventConditional.levelstatvalue;
            lblLevelComparator.Text = Strings.EventConditional.comparator;
            lblLevelOrStat.Text = Strings.EventConditional.levelstatitem;
            cmbLevelStat.Items.Clear();
            cmbLevelStat.Items.Add(Strings.EventConditional.level);
            for (int i = 0; i < (int)Stats.StatCount; i++)
            {
                cmbLevelStat.Items.Add(Strings.Combat.stats[i]);
            }
            cmbLevelComparator.Items.Clear();
            for (int i = 0; i < Strings.EventConditional.comparators.Length; i++)
            {
                cmbLevelComparator.Items.Add(Strings.EventConditional.comparators[i]);
            }

            //Self Switch Is
            grpSelfSwitch.Text = Strings.EventConditional.selfswitchis;
            lblSelfSwitch.Text = Strings.EventConditional.selfswitch;
            lblSelfSwitchIs.Text = Strings.EventConditional.switchis;
            cmbSelfSwitch.Items.Clear();
            for (int i = 0; i < 4; i++)
            {
                cmbSelfSwitch.Items.Add(Strings.EventConditional.selfswitches[i]);
            }
            cmbSelfSwitchVal.Items.Clear();
            cmbSelfSwitchVal.Items.Add(Strings.EventConditional.False);
            cmbSelfSwitchVal.Items.Add(Strings.EventConditional.True);

            //Power Is
            grpPowerIs.Text = Strings.EventConditional.poweris;
            lblPower.Text = Strings.EventConditional.power;
            cmbPower.Items.Clear();
            cmbPower.Items.Add(Strings.EventConditional.power0);
            cmbPower.Items.Add(Strings.EventConditional.power1);

            //Time Is
            grpTime.Text = Strings.EventConditional.time;
            lblStartRange.Text = Strings.EventConditional.startrange;
            lblEndRange.Text = Strings.EventConditional.endrange;
            lblAnd.Text = Strings.EventConditional.and;

            //Can Start Quest
            grpStartQuest.Text = Strings.EventConditional.canstartquest;
            lblStartQuest.Text = Strings.EventConditional.startquest;

            //Quest In Progress
            grpQuestInProgress.Text = Strings.EventConditional.questinprogress;
            lblQuestProgress.Text = Strings.EventConditional.questprogress;
            lblQuestIs.Text = Strings.EventConditional.questis;
            cmbTaskModifier.Items.Clear();
            for (int i = 0; i < Strings.EventConditional.questcomparators.Length; i++)
            {
                cmbTaskModifier.Items.Add(Strings.EventConditional.questcomparators[i]);
            }
            lblQuestTask.Text = Strings.EventConditional.task;

            //Quest Completed
            grpQuestCompleted.Text = Strings.EventConditional.questcompleted;
            lblQuestCompleted.Text = Strings.EventConditional.questcompletedlabel;

            //Gender is
            grpGender.Text = Strings.EventConditional.genderis;
            lblGender.Text = Strings.EventConditional.gender;
            cmbGender.Items.Clear();
            cmbGender.Items.Add(Strings.EventConditional.male);
            cmbGender.Items.Add(Strings.EventConditional.female);

            btnSave.Text = Strings.EventConditional.okay;
            btnCancel.Text = Strings.EventConditional.cancel;
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
                    grpSwitch.Text = Strings.EventConditional.playerswitch;
                    grpSwitch.Show();
                    cmbSwitch.Items.Clear();
                    cmbSwitch.Items.AddRange(Database.GetGameObjectList(GameObjectType.PlayerSwitch));
                    if (cmbSwitch.Items.Count > 0) cmbSwitch.SelectedIndex = 0;
                    cmbSwitchVal.SelectedIndex = 0;
                    break;
                case 1: //Player Variables
                    grpPlayerVariable.Text = Strings.EventConditional.playervariable;
                    grpPlayerVariable.Show();
                    cmbVariable.Items.Clear();
                    cmbVariable.Items.AddRange(Database.GetGameObjectList(GameObjectType.PlayerVariable));
                    if (cmbVariable.Items.Count > 0) cmbVariable.SelectedIndex = 0;
                    cmbVariableMod.SelectedIndex = 0;
                    txtVariableVal.Text = @"0";
                    break;
                case 2: //Global Switch
                    grpPlayerVariable.Text = Strings.EventConditional.globalswitch;
                    grpSwitch.Show();
                    cmbSwitch.Items.Clear();
                    cmbSwitch.Items.AddRange(Database.GetGameObjectList(GameObjectType.ServerSwitch));
                    if (cmbSwitch.Items.Count > 0) cmbSwitch.SelectedIndex = 0;
                    cmbSwitchVal.SelectedIndex = 0;
                    break;
                case 3: //Global Variables
                    grpPlayerVariable.Text = Strings.EventConditional.globalvariable;
                    grpPlayerVariable.Show();
                    cmbVariable.Items.Clear();
                    cmbVariable.Items.AddRange(Database.GetGameObjectList(GameObjectType.ServerVariable));
                    if (cmbVariable.Items.Count > 0) cmbVariable.SelectedIndex = 0;
                    cmbVariableMod.SelectedIndex = 0;
                    txtVariableVal.Text = @"0";
                    break;
                case 4: //Has Item
                    grpHasItem.Show();
                    cmbItem.Items.Clear();
                    cmbItem.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));
                    if (cmbItem.Items.Count > 0) cmbItem.SelectedIndex = 0;
                    nudItemAmount.Value = 1;
                    break;
                case 5: //Class is
                    grpClass.Show();
                    cmbClass.Items.Clear();
                    cmbClass.Items.AddRange(Database.GetGameObjectList(GameObjectType.Class));
                    if (cmbClass.Items.Count > 0) cmbClass.SelectedIndex = 0;
                    break;
                case 6: //Knows spell
                    grpSpell.Show();
                    cmbSpell.Items.Clear();
                    cmbSpell.Items.AddRange(Database.GetGameObjectList(GameObjectType.Spell));
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
                        var addRange = time.ToString("h:mm:ss tt") + " " + Strings.EventConditional.to + " ";
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
                    cmbStartQuest.Items.AddRange(Database.GetGameObjectList(GameObjectType.Quest));
                    if (cmbStartQuest.Items.Count > 0) cmbStartQuest.SelectedIndex = 0;
                    break;
                case 12: //Quest In Progress
                    grpQuestInProgress.Show();
                    cmbQuestInProgress.Items.Clear();
                    cmbQuestInProgress.Items.AddRange(Database.GetGameObjectList(GameObjectType.Quest));
                    if (cmbQuestInProgress.Items.Count > 0) cmbQuestInProgress.SelectedIndex = 0;
                    cmbTaskModifier.SelectedIndex = 0;
                    break;
                case 13: //Quest Completed
                    grpQuestCompleted.Show();
                    cmbCompletedQuest.Items.Clear();
                    cmbCompletedQuest.Items.AddRange(Database.GetGameObjectList(GameObjectType.Quest));
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

            if (mCurrentPage != null)
            {
                if (mMyCommand.Ints[4] == 0)
                    // command.Ints[4 & 5] are reserved for referencing which command list the true/false braches follow
                {
                    for (var i = 0; i < 2; i++)
                    {
                        mCurrentPage.CommandLists.Add(new CommandList());
                        mMyCommand.Ints[4 + i] = mCurrentPage.CommandLists.Count - 1;
                    }
                }
            }

            mMyCommand.Ints[0] = cmbConditionType.SelectedIndex;
            switch (mMyCommand.Ints[0])
            {
                case 0: //Player Switch
                    mMyCommand.Ints[1] =
                        Database.GameObjectIdFromList(GameObjectType.PlayerSwitch, cmbSwitch.SelectedIndex);
                    mMyCommand.Ints[2] = cmbSwitchVal.SelectedIndex;
                    break;
                case 1: //Player Variable
                    mMyCommand.Ints[1] = Database.GameObjectIdFromList(GameObjectType.PlayerVariable,
                        cmbVariable.SelectedIndex);
                    mMyCommand.Ints[2] = cmbVariableMod.SelectedIndex;
                    if (int.TryParse(txtVariableVal.Text, out n))
                    {
                        mMyCommand.Ints[3] = n;
                    }
                    else
                    {
                        mMyCommand.Ints[3] = 0;
                    }
                    break;
                case 2: //Global Switch
                    mMyCommand.Ints[1] =
                        Database.GameObjectIdFromList(GameObjectType.ServerSwitch, cmbSwitch.SelectedIndex);
                    mMyCommand.Ints[2] = cmbSwitchVal.SelectedIndex;
                    break;
                case 3: //Global Variable
                    mMyCommand.Ints[1] = Database.GameObjectIdFromList(GameObjectType.ServerVariable,
                        cmbVariable.SelectedIndex);
                    mMyCommand.Ints[2] = cmbVariableMod.SelectedIndex;
                    if (int.TryParse(txtVariableVal.Text, out n))
                    {
                        mMyCommand.Ints[3] = n;
                    }
                    else
                    {
                        mMyCommand.Ints[3] = 0;
                    }
                    break;
                case 4: //Has Item
                    mMyCommand.Ints[1] = Database.GameObjectIdFromList(GameObjectType.Item, cmbItem.SelectedIndex);
                    mMyCommand.Ints[2] = (int) nudItemAmount.Value;
                    break;
                case 5: //Class Is
                    mMyCommand.Ints[1] = Database.GameObjectIdFromList(GameObjectType.Class, cmbClass.SelectedIndex);
                    break;
                case 6: //Knows spell
                    mMyCommand.Ints[1] = Database.GameObjectIdFromList(GameObjectType.Spell, cmbSpell.SelectedIndex);
                    break;
                case 7: //Level or Stat is
                    mMyCommand.Ints[1] = cmbLevelComparator.SelectedIndex;
                    mMyCommand.Ints[2] = (int) nudLevelStatValue.Value;
                    mMyCommand.Ints[3] = cmbLevelStat.SelectedIndex;
                    break;
                case 8: //Self Switch
                    mMyCommand.Ints[1] = cmbSelfSwitch.SelectedIndex;
                    mMyCommand.Ints[2] = cmbSelfSwitchVal.SelectedIndex;
                    break;
                case 9: //Power is
                    mMyCommand.Ints[1] = cmbPower.SelectedIndex;
                    break;
                case 10: //Time is between...
                    mMyCommand.Ints[1] = cmbTime1.SelectedIndex;
                    mMyCommand.Ints[2] = cmbTime2.SelectedIndex;
                    break;
                case 11: //Can Start Quest
                    mMyCommand.Ints[1] =
                        Database.GameObjectIdFromList(GameObjectType.Quest, cmbStartQuest.SelectedIndex);
                    break;
                case 12: //Quest IN Progress
                    mMyCommand.Ints[1] = Database.GameObjectIdFromList(GameObjectType.Quest,
                        cmbQuestInProgress.SelectedIndex);
                    mMyCommand.Ints[2] = cmbTaskModifier.SelectedIndex;
                    mMyCommand.Ints[3] = -1;
                    if (cmbTaskModifier.SelectedIndex != 0)
                    {
                        //Get Quest Task Here
                        var quest =
                            QuestBase.Lookup.Get<QuestBase>(Database.GameObjectIdFromList(GameObjectType.Quest,
                                cmbQuestInProgress.SelectedIndex));
                        if (quest != null)
                        {
                            if (cmbQuestTask.SelectedIndex > -1)
                            {
                                mMyCommand.Ints[3] = quest.Tasks[cmbQuestTask.SelectedIndex].Id;
                            }
                        }
                    }
                    break;
                case 13: //Quest Completed
                    mMyCommand.Ints[1] =
                        Database.GameObjectIdFromList(GameObjectType.Quest, cmbCompletedQuest.SelectedIndex);
                    break;
                case 14: //Player death...
                    break;
                case 15: //No NPC's on map
                    break;
                case 16: //Gender Is
                    mMyCommand.Ints[1] = cmbGender.SelectedIndex;
                    break;
            }
            if (mEventEditor != null)
            {
                mEventEditor.FinishCommandEdit();
            }
            else
            {
                if (ParentForm != null) ParentForm.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (mCurrentPage != null)
            {
                mEventEditor.CancelCommandEdit();
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
                QuestBase.Lookup.Get<QuestBase>(
                    Database.GameObjectIdFromList(GameObjectType.Quest, cmbQuestInProgress.SelectedIndex));
            if (quest != null)
            {
                foreach (var task in quest.Tasks)
                {
                    cmbQuestTask.Items.Add(task.GetTaskString(Strings.TaskEditor.descriptions));
                }
                if (cmbQuestTask.Items.Count > 0) cmbQuestTask.SelectedIndex = 0;
            }
        }
    }
}